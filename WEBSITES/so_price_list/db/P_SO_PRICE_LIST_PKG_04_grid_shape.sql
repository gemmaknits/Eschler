/* ============================================================================
   Grid shape on the header + row-level editing.

   WHY THE HEADER NEEDS THIS
   The grid derives its colour-tier columns from the lines it loads. A brand new
   list has no lines, so there is nothing to derive and nowhere to type. The tier
   and currency set therefore have to be a property of the LIST, chosen up front,
   not inferred afterwards.

   Defaults come from the imported data, not taste: of 53 lists, 25 use
   {PFE/PFD, All_colors}, 16 use {All_colors}, 6 use {PFE/PFD} - 47 of 53 use
   nothing but those two tiers, and 51 use at most two. Currency is single per
   list in 48 of 53 (41 USD, 7 THB). So a new list opens with those two tiers in
   USD rather than a wall of twelve mostly-empty columns.
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

IF COL_LENGTH('SO.so_price_list_header','tier_set') IS NULL
    ALTER TABLE SO.so_price_list_header ADD tier_set nvarchar(200) NULL;
GO
IF COL_LENGTH('SO.so_price_list_header','currency_set') IS NULL
    ALTER TABLE SO.so_price_list_header ADD currency_set nvarchar(20) NULL;
GO
/* Whether this list's grid hides withdrawn lines. A per-list view preference,
   remembered so whoever opens the list next sees it the way it was left.

   Default 'Y' - hidden. A withdrawn price is one nobody should be quoting, so
   the normal view is the one without them; showing them is the deliberate act,
   taken per list. */
IF COL_LENGTH('SO.so_price_list_header','hide_inactive') IS NULL
    ALTER TABLE SO.so_price_list_header
        ADD hide_inactive char(1) NOT NULL
            CONSTRAINT DF_so_price_list_header_hide_inactive DEFAULT ('Y');
GO

/* Move an existing 'N' default over to 'Y', and bring the rows with it. */
IF EXISTS (SELECT 1 FROM sys.default_constraints
           WHERE name = 'DF_so_price_list_header_hide_inactive'
             AND definition LIKE '%N%')
BEGIN
    ALTER TABLE SO.so_price_list_header
        DROP CONSTRAINT DF_so_price_list_header_hide_inactive;
    ALTER TABLE SO.so_price_list_header
        ADD CONSTRAINT DF_so_price_list_header_hide_inactive
            DEFAULT ('Y') FOR hide_inactive;
    UPDATE SO.so_price_list_header SET hide_inactive = 'Y' WHERE hide_inactive = 'N';
    PRINT 'hide_inactive default moved to Y';
END
GO

/* Backfill the 53 imported lists from what their lines actually use, so an
   existing list opens on exactly the columns it already needs. */
UPDATE h
SET    h.tier_set = t.tiers,
       h.currency_set = c.currencies
FROM   SO.so_price_list_header h
CROSS APPLY (
    SELECT STUFF((SELECT ',' + x.color_tier
                  FROM (SELECT DISTINCT color_tier
                        FROM SO.so_price_list_detail
                        WHERE so_price_list_header_id = h.so_price_list_header_id
                          AND delete_mark <> 'Y') x
                  ORDER BY x.color_tier
                  FOR XML PATH('')), 1, 1, '') AS tiers
) t
CROSS APPLY (
    SELECT STUFF((SELECT ',' + LTRIM(RTRIM(y.currency))
                  FROM (SELECT DISTINCT currency
                        FROM SO.so_price_list_detail
                        WHERE so_price_list_header_id = h.so_price_list_header_id
                          AND delete_mark <> 'Y') y
                  ORDER BY y.currency
                  FOR XML PATH('')), 1, 1, '') AS currencies
) c
WHERE h.delete_mark <> 'Y'
  AND h.tier_set IS NULL
  AND t.tiers IS NOT NULL;
GO

PRINT 'header grid-shape columns added and backfilled';
GO

/* ---------------------------------------------------------------------------
   update_price_list_grid_shape  -  set the columns a list shows.
   Removing a tier only hides the column; the price lines behind it are left
   alone, so the change is reversible and never destroys data.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_update_price_list_grid_shape','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_update_price_list_grid_shape;
GO
-- =============================================
-- Description: Store the colour tiers and currencies a price list's grid shows.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_update_price_list_grid_shape 31,'PFE/PFD,All_colors','USD,THB','SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_update_price_list_grid_shape]
    @so_price_list_header_id bigint,
    @tier_set                nvarchar(200) = null,
    @currency_set            nvarchar(20)  = null,
    @hide_inactive           char(1)       = null,   -- 'Y' | 'N'
    @logempcd                varchar(15)   = ''
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM SO.so_price_list_header
                   WHERE so_price_list_header_id = @so_price_list_header_id
                     AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('Price list not found, or already deleted.', 16, 1);
        RETURN;
    END

    IF @hide_inactive IS NOT NULL AND @hide_inactive NOT IN ('Y','N')
    BEGIN
        RAISERROR('Hide inactive must be Y or N.', 16, 1);
        RETURN;
    END

    UPDATE SO.so_price_list_header
    SET    tier_set          = ISNULL(@tier_set, tier_set),
           currency_set      = ISNULL(@currency_set, currency_set),
           hide_inactive     = ISNULL(@hide_inactive, hide_inactive),
           last_updated_date = SYSDATETIME(),
           updated_by        = @logempcd
    WHERE  so_price_list_header_id = @so_price_list_header_id;

    SELECT so_price_list_header_id, tier_set, currency_set, hide_inactive
    FROM   SO.so_price_list_header
    WHERE  so_price_list_header_id = @so_price_list_header_id;
END
GO

/* ---------------------------------------------------------------------------
   update_price_list_row  -  edit the row-level fields of a grid row.

   One grid row is one (article x variant x qty band) and stands for EVERY price
   line underneath it - up to 12 of them across tiers and currencies. Changing
   the article or the qty band on screen has to move all of them together, or
   the row silently splits in two. That is what this does; editing a single
   price still goes through update_price_list_detail.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_update_price_list_row','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_update_price_list_row;
GO
-- =============================================
-- Description: Apply row-level edits to every price line sharing a grid row.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_update_price_list_row 31,'255484','',200,600,'M','255484',null,250,650,'M',...,'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_update_price_list_row]
    @so_price_list_header_id bigint,
    -- which row (its key as currently stored)
    @article                 nvarchar(30),
    @article_variant         nvarchar(20)  = null,
    @qty_min                 int,
    @qty_max                 int           = null,
    @qty_unit                char(2)       = 'M',
    -- new values; NULL means leave alone, except new_qty_max (see below)
    @new_article             nvarchar(30)  = null,
    @new_article_variant     nvarchar(20)  = null,
    @new_qty_min             int           = null,
    @new_qty_max             int           = null,
    @clear_qty_max           bit           = 0,   -- explicit, since NULL is a real value
    @new_qty_unit            char(2)       = null,
    @fabric_name             nvarchar(120) = null,
    @composition             nvarchar(200) = null,
    @full_width_cm           nvarchar(30)  = null,
    @usable_width_cm         nvarchar(30)  = null,
    @weight_gsm              nvarchar(30)  = null,
    @moq                     nvarchar(30)  = null,
    @design_no               char(20)      = null,
    @active                  char(1)       = null,   -- 'Y' | 'N', whole row
    @logempcd                varchar(15)   = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @qmin int = ISNULL(@new_qty_min, @qty_min);
    DECLARE @qmax int = CASE WHEN @clear_qty_max = 1 THEN NULL
                             ELSE ISNULL(@new_qty_max, @qty_max) END;

    IF @qmin < 0
    BEGIN
        RAISERROR('Minimum quantity cannot be negative.', 16, 1);
        RETURN;
    END

    IF @qmax IS NOT NULL AND @qmax < @qmin
    BEGIN
        RAISERROR('Maximum quantity cannot be less than minimum quantity.', 16, 1);
        RETURN;
    END

    IF @active IS NOT NULL AND @active NOT IN ('Y','N')
    BEGIN
        RAISERROR('Active must be Y or N.', 16, 1);
        RETURN;
    END

    IF @new_article IS NOT NULL AND LTRIM(RTRIM(@new_article)) = ''
    BEGIN
        RAISERROR('Article is required.', 16, 1);
        RETURN;
    END

    UPDATE SO.so_price_list_detail
    SET    article           = ISNULL(@new_article,         article),
           article_variant   = ISNULL(@new_article_variant, article_variant),
           qty_min           = @qmin,
           qty_max           = @qmax,
           qty_unit          = ISNULL(@new_qty_unit,        qty_unit),
           fabric_name       = ISNULL(@fabric_name,     fabric_name),
           composition       = ISNULL(@composition,     composition),
           full_width_cm     = ISNULL(@full_width_cm,   full_width_cm),
           usable_width_cm   = ISNULL(@usable_width_cm, usable_width_cm),
           weight_gsm        = ISNULL(@weight_gsm,      weight_gsm),
           moq               = ISNULL(@moq,             moq),
           design_no         = ISNULL(@design_no,       design_no),
           active            = ISNULL(@active,         active),
           last_updated_date = SYSDATETIME(),
           updated_by        = @logempcd
    WHERE  so_price_list_header_id = @so_price_list_header_id
      AND  article    = @article
      AND  ISNULL(article_variant,'') = ISNULL(@article_variant,'')
      AND  qty_min    = @qty_min
      AND  ISNULL(qty_max,-1) = ISNULL(@qty_max,-1)
      AND  qty_unit   = @qty_unit
      AND  delete_mark <> 'Y';

    SELECT @@ROWCOUNT AS lines_updated;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 4 (grid shape + row edit) created';
GO
