/* ============================================================================
   SO_PRICE_LIST_PKG  -  part 1 of 3 : read procedures
   Naming: P_<PACKAGE>_PKG_<verb>_<subject>, matching the house convention.

   Target: SQL Server 2014 (12.0). No STRING_AGG / TRIM / DROP IF EXISTS.
   Every proc takes @logempcd last, per house style.

   article is TEXT everywhere - 487 of 4,468 lines look like '255484AA/11',
   not a number. The @article parameters below are nvarchar, so SQL coerces at
   the parameter boundary and the comparison stays text-to-text. The care is
   needed on the CALLER side: bind article as a string (mssql NVarChar), never
   as Int, or the alphanumeric codes are rejected before they reach the proc.
   ============================================================================ */

/* Baked in, not left to the deploy tool: so_price_list_detail carries FILTERED
   indexes, and any INSERT or UPDATE from a module created with QUOTED_IDENTIFIER
   OFF fails at run time with error 1934. sqlcmd defaults it OFF, SSMS ON, which
   is why the same file could deploy working procedures one day and broken ones
   the next. */
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

SET NOCOUNT ON;
GO

/* ---------------------------------------------------------------------------
   select_price_list  -  headers for the order-entry picker.
   Returns line + conflict counts so the picker can warn before the list opens.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_select_price_list','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_select_price_list;
GO
-- =============================================
-- Description: Price list headers, filtered for the list picker.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_select_price_list null, null, null, null, ''
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_select_price_list]
    @so_price_list_header_id bigint       = null,   -- one list, or all
    @customer_id             bigint       = null,   -- lists for this customer
    @search                  nvarchar(100)= null,   -- name / desc / workbook name
    @as_of                   date         = null,   -- only lists live on this date
    @logempcd                varchar(15)  = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  h.so_price_list_header_id,
            h.list_name,
            h.list_desc,
            h.customer_id,
            h.customer_name,
            h.customer_excel,
            h.list_date,
            h.valid_from,
            h.valid_to,
            h.terms,
            h.quote_ref,
            h.sonoid,
            h.so_line_id,
            h.source_sheet,
            h.notes,
            /* which tier/currency columns this list's grid shows. The app needs
               these to render a list that has no lines yet - there is nothing
               to infer columns from until the first price is entered. */
            h.tier_set,
            h.currency_set,
            h.hide_inactive,
            h.creation_date,
            h.created_by,
            h.last_updated_date,
            h.updated_by,
            ISNULL(d.line_count, 0)    AS line_count,
            ISNULL(c.conflict_count,0) AS conflict_count,
            CASE WHEN h.valid_to IS NOT NULL
                  AND h.valid_to < CAST(GETDATE() AS date) THEN 'Y' ELSE 'N' END AS expired_flag
    FROM    SO.so_price_list_header h
    LEFT JOIN (
            SELECT so_price_list_header_id, COUNT(*) AS line_count
            FROM   SO.so_price_list_detail
            WHERE  delete_mark <> 'Y'
            GROUP BY so_price_list_header_id
    ) d ON d.so_price_list_header_id = h.so_price_list_header_id
    LEFT JOIN (
            /* cells that resolve to more than one LIVE price line */
            SELECT so_price_list_header_id, COUNT(*) AS conflict_count
            FROM (
                SELECT so_price_list_header_id
                FROM   SO.so_price_list_detail
                WHERE  delete_mark <> 'Y'
                  AND  active = 'Y'
                GROUP BY so_price_list_header_id, design_no, article_variant,
                         qty_min, qty_max, qty_unit, color_tier, currency
                HAVING COUNT(*) > 1
            ) z
            GROUP BY so_price_list_header_id
    ) c ON c.so_price_list_header_id = h.so_price_list_header_id
    WHERE   h.delete_mark <> 'Y'
      AND  (@so_price_list_header_id IS NULL OR h.so_price_list_header_id = @so_price_list_header_id)
      AND  (@customer_id IS NULL OR h.customer_id = @customer_id)
      AND  (@search IS NULL OR @search = ''
            OR h.list_name      LIKE '%' + @search + '%'
            OR h.list_desc      LIKE '%' + @search + '%'
            OR h.customer_excel LIKE '%' + @search + '%')
      AND  (@as_of IS NULL
            OR ((h.valid_from IS NULL OR h.valid_from <= @as_of)
            AND (h.valid_to   IS NULL OR h.valid_to   >= @as_of)))
    ORDER BY h.list_name;
END
GO

/* ---------------------------------------------------------------------------
   select_price_list_detail  -  the lines of one list.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_select_price_list_detail','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_select_price_list_detail;
GO
-- =============================================
-- Description: Detail lines for one price list. conflict_count marks lines
--              sharing a business key with another line (order entry must ask).
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_select_price_list_detail 31, null, null, null, ''
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_select_price_list_detail]
    @so_price_list_header_id bigint       = null,
    @design_no               nvarchar(60) = null,   -- exact; text, never numeric
    @article                 nvarchar(30) = null,   -- accepted as an alias for @design_no
    @search                  nvarchar(100)= null,   -- design / fabric / composition
    @conflicts_only          bit          = 0,
    @logempcd                varchar(15)  = ''
AS
BEGIN
    SET NOCOUNT ON;

    /* Only ACTIVE lines compete. A withdrawn line is not an alternative price,
       so it must not inflate the count shown against the ones that are. */
    ;WITH dup AS (
        SELECT so_price_list_header_id, design_no, article_variant,
               qty_min, qty_max, qty_unit, color_tier, currency,
               COUNT(*) AS n
        FROM   SO.so_price_list_detail
        WHERE  delete_mark <> 'Y'
          AND  active = 'Y'
          AND (@so_price_list_header_id IS NULL
               OR so_price_list_header_id = @so_price_list_header_id)
        GROUP BY so_price_list_header_id, design_no, article_variant,
                 qty_min, qty_max, qty_unit, color_tier, currency
    )
    SELECT  d.so_price_list_detail_id,
            d.so_price_list_header_id,
            /* set_no is the grid row: the USD and THB halves of one price line
               carry the same value, line_no orders them within it. */
            d.set_no,
            d.line_no,
            d.article,
            d.design_no,
            d.article_variant,
            d.fabric_name,
            d.composition,
            d.full_width_cm,
            d.usable_width_cm,
            d.weight_gsm,
            d.moq,
            d.qty_min,
            d.qty_max,
            d.qty_unit,
            d.color_tier,
            d.currency,
            d.price,
            d.active,
            d.source_row,
            d.notes,
            d.creation_date,
            d.created_by,
            d.last_updated_date,
            d.updated_by,
            /* 0 for a withdrawn line: it carries no badge of its own. */
            CASE WHEN d.active = 'Y' THEN ISNULL(dup.n, 1) ELSE 0 END AS conflict_count
    FROM    SO.so_price_list_detail d
    LEFT JOIN dup ON dup.so_price_list_header_id = d.so_price_list_header_id
                AND dup.design_no              = d.design_no
                AND ISNULL(dup.article_variant,'') = ISNULL(d.article_variant,'')
                AND dup.qty_min                = d.qty_min
                AND ISNULL(dup.qty_max,-1)     = ISNULL(d.qty_max,-1)
                AND dup.qty_unit               = d.qty_unit
                AND dup.color_tier             = d.color_tier
                AND dup.currency               = d.currency
    WHERE   d.delete_mark <> 'Y'
      AND  (@so_price_list_header_id IS NULL
            OR d.so_price_list_header_id = @so_price_list_header_id)
      /* @article is the old name for this filter; either one narrows by design */
      AND  (COALESCE(NULLIF(@design_no,''), NULLIF(@article,'')) IS NULL
            OR d.design_no = COALESCE(NULLIF(@design_no,''), NULLIF(@article,'')))
      AND  (@search  IS NULL OR @search  = ''
            OR d.design_no   LIKE '%' + @search + '%'
            OR d.fabric_name LIKE '%' + @search + '%'
            OR d.composition LIKE '%' + @search + '%')
      AND  (@conflicts_only = 0 OR (d.active = 'Y' AND ISNULL(dup.n,0) > 1))
    /* Workbook order, which is what set_no preserves - and what makes an
       inserted line stay where it was put. */
    ORDER BY d.so_price_list_header_id, d.set_no, d.line_no;
END
GO

/* ---------------------------------------------------------------------------
   get_price  -  THE order-entry lookup.

   Returns every price line matching the criteria. The caller decides:
     0 rows  -> no price on this list
     1 row   -> resolves automatically
     2+ rows -> ask the user which line applies
   That is deliberate. The business key is not unique in the imported data
   (203 colliding groups, 148 with differing prices), and the decision was to
   keep both rows and let the user choose rather than silently pick one.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_get_price','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_get_price;
GO
-- =============================================
-- Description: Resolve a price for (list, article, colour tier, qty, currency).
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_get_price 31, '255484', 'PFE/PFD', 300, 'M', 'USD', null, ''
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_get_price]
    @so_price_list_header_id bigint,
    @article                 nvarchar(30),
    @color_tier              nvarchar(30) = null,   -- null = every tier
    @qty                     int          = null,   -- null = every band
    @qty_unit                char(2)      = 'M',
    @currency                char(3)      = null,   -- null = both
    @as_of                   date         = null,   -- validity check on the header
    @include_inactive        bit          = 0,      -- retired lines stay hidden
    @logempcd                varchar(15)  = ''
AS
BEGIN
    SET NOCOUNT ON;

    IF @as_of IS NULL SET @as_of = CAST(GETDATE() AS date);

    SELECT  d.so_price_list_detail_id,
            d.so_price_list_header_id,
            h.list_name,
            d.article,
            d.design_no,
            d.article_variant,
            d.fabric_name,
            d.color_tier,
            d.qty_min,
            d.qty_max,
            d.qty_unit,
            d.currency,
            d.price,
            d.active,
            d.moq,
            h.terms,
            h.valid_from,
            h.valid_to,
            d.source_row,
            /* narrowest qty band first: a 200-600 line beats an open-ended one */
            CASE WHEN d.qty_max IS NULL THEN 2147483647 ELSE d.qty_max - d.qty_min END
                AS band_width,
            COUNT(*) OVER () AS match_count
    FROM    SO.so_price_list_detail d
    JOIN    SO.so_price_list_header h
              ON h.so_price_list_header_id = d.so_price_list_header_id
    WHERE   d.so_price_list_header_id = @so_price_list_header_id
      AND   d.article    = @article
      AND   d.delete_mark <> 'Y'
      AND   h.delete_mark <> 'Y'
      /* Ausco and GLAMORISE carry an ACTIVE flag in the workbook; an 'N' line
         is a price that has been withdrawn, so it is not offered by default. */
      AND  (@include_inactive = 1 OR d.active = 'Y')
      AND  (@color_tier IS NULL OR @color_tier = '' OR d.color_tier = @color_tier)
      AND  (@currency   IS NULL OR @currency   = '' OR d.currency   = @currency)
      AND  (@qty_unit   IS NULL OR d.qty_unit  = @qty_unit)
      AND  (@qty IS NULL
            OR (d.qty_min <= @qty AND (d.qty_max IS NULL OR d.qty_max >= @qty)))
      AND  (h.valid_from IS NULL OR h.valid_from <= @as_of)
      AND  (h.valid_to   IS NULL OR h.valid_to   >= @as_of)
    ORDER BY d.currency,
             d.color_tier,
             band_width,          -- most specific band wins when several match
             d.source_row,
             d.so_price_list_detail_id;
END
GO

/* ---------------------------------------------------------------------------
   select_customer  -  LOV for attaching a customer to a header.
   parent_customer_flag and parent_customer_id disagree on 253 of 719 rows, so
   @parent_only accepts either signal rather than trusting the flag alone.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_select_customer','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_select_customer;
GO
-- =============================================
-- Description: Customer lookup, ranked by real sales-order activity.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_select_customer 'anita', 1, 50, ''
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_select_customer]
    @search      nvarchar(100) = null,
    @parent_only bit           = 0,
    @top_n       int           = 50,
    @logempcd    varchar(15)   = ''
AS
BEGIN
    SET NOCOUNT ON;

    IF @top_n IS NULL OR @top_n <= 0 SET @top_n = 50;

    SELECT TOP (@top_n)
            c.customer_id,
            c.custcd,
            c.name          AS customer_name,
            c.name2,
            c.ctry,
            c.city,
            c.active,
            c.parent_customer_id,
            c.parent_customer_flag,
            ISNULL(s.so_orders, 0) AS so_orders,
            s.last_so_date
    FROM    dbo.customers c
    LEFT JOIN (
            SELECT custcd, COUNT(*) AS so_orders, MAX(sodt) AS last_so_date
            FROM   dbo.so
            GROUP BY custcd
    ) s ON s.custcd = c.custcd
    WHERE  (@search IS NULL OR @search = ''
            OR c.name  LIKE '%' + @search + '%'
            OR c.name2 LIKE '%' + @search + '%'
            OR c.custcd LIKE '%' + @search + '%')
      AND  (@parent_only = 0
            OR c.parent_customer_flag = 'Y'
            OR c.parent_customer_id = c.customer_id)
    /* the record actually traded on is nearly always the right one */
    ORDER BY ISNULL(s.so_orders,0) DESC, c.name;
END
GO

/* ---------------------------------------------------------------------------
   select_color_tier  -  distinct tiers, for column headers and pickers.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_select_color_tier','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_select_color_tier;
GO
-- =============================================
-- Description: Colour tiers in use, optionally within one list.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_select_color_tier 31, ''
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_select_color_tier]
    @so_price_list_header_id bigint      = null,
    @logempcd                varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT  d.color_tier,
            COUNT(*) AS line_count,
            /* stable display order; unknown tiers fall to the end alphabetically.
               Same function the insert uses to order lines within a set, so the
               columns and the rows behind them can never disagree. */
            SO.F_SO_PRICE_LIST_tier_order(d.color_tier) AS sort_order
    FROM    SO.so_price_list_detail d
    WHERE   d.delete_mark <> 'Y'
      AND  (@so_price_list_header_id IS NULL
            OR d.so_price_list_header_id = @so_price_list_header_id)
    GROUP BY d.color_tier
    ORDER BY sort_order, d.color_tier;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 1 (select) created';
GO

