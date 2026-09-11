/* ============================================================================
   SO_PRICE_LIST_PKG  -  part 5 : inserting a set between two others

   A set is one grid row: the USD and THB rows for one article, quantity band
   and colour tier. Inserting between set 1 and set 2 shifts everything from 2
   onward up by one and drops the new set into the gap, so set_no stays a dense
   1..n with no decimals to exhaust.

   Renumbering is cheap here - the largest list is 1,988 rows - and it happens
   inside one transaction, so a reader never sees two sets claiming the same
   number.
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

IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_insert_price_list_set','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_insert_price_list_set;
GO
-- =============================================
-- Description: Insert a new price line (its USD and THB halves) after a given
--              set, shifting the sets below it down.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_insert_price_list_set 29, 1, '254555', null, 200, 600, 'M', 'PFE/PFD', 'USD', 3.25, ..., 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_insert_price_list_set]
    @so_price_list_header_id bigint,
    @after_set_no            int           = null,   -- NULL or 0 = put it first
    @design_no               nvarchar(60)  = null,
    @article                 nvarchar(30)  = null,   -- old alias for @design_no
    @article_variant         nvarchar(20)  = null,
    @qty_min                 int           = null,
    @qty_max                 int           = null,
    @qty_unit                char(2)       = 'M',
    @color_tier              nvarchar(30)  = null,
    @currency                char(3)       = 'USD',  -- the half you typed
    @price                   decimal(18,4) = null,
    @fabric_name             nvarchar(200) = null,
    @composition             nvarchar(200) = null,
    @full_width_cm           nvarchar(60)  = null,
    @usable_width_cm         nvarchar(60)  = null,
    @weight_gsm              nvarchar(60)  = null,
    @moq                     nvarchar(60)  = null,
    @notes                   nvarchar(500) = null,
    @logempcd                varchar(15)   = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM SO.so_price_list_header
                   WHERE so_price_list_header_id = @so_price_list_header_id
                     AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('Price list not found, or already deleted.', 16, 1);
        RETURN;
    END

    /* design_no identifies the line; article mirrors it so get_price keeps
       working. Either name may be supplied. */
    IF @design_no IS NULL OR LTRIM(RTRIM(@design_no)) = '' SET @design_no = @article;
    IF @article   IS NULL OR LTRIM(RTRIM(@article))   = '' SET @article   = @design_no;

    IF @design_no IS NULL OR LTRIM(RTRIM(@design_no)) = ''
    BEGIN
        RAISERROR('Design no is required.', 16, 1);
        RETURN;
    END

    IF @currency IS NULL OR @currency NOT IN ('USD','THB')
    BEGIN
        RAISERROR('Currency must be USD or THB.', 16, 1);
        RETURN;
    END

    IF @qty_min IS NULL SET @qty_min = 0;
    IF @qty_min < 0
    BEGIN
        RAISERROR('Minimum quantity cannot be negative.', 16, 1);
        RETURN;
    END

    IF @qty_max IS NOT NULL AND @qty_max < @qty_min
    BEGIN
        RAISERROR('Maximum quantity cannot be less than minimum quantity.', 16, 1);
        RETURN;
    END

    IF @price IS NOT NULL AND @price < 0
    BEGIN
        RAISERROR('Price cannot be negative.', 16, 1);
        RETURN;
    END

    IF @qty_unit   IS NULL SET @qty_unit = 'M';
    IF @color_tier IS NULL OR LTRIM(RTRIM(@color_tier)) = ''
        SET @color_tier = N'Unspecified';
    IF @after_set_no IS NULL SET @after_set_no = 0;

    DECLARE @new_set int = @after_set_no + 1;
    DECLARE @other   char(3) = CASE @currency WHEN 'USD' THEN 'THB' ELSE 'USD' END;

    BEGIN TRAN;

        /* make room: everything at or below the insertion point moves down */
        UPDATE SO.so_price_list_detail
        SET    set_no = set_no + 1,
               last_updated_date = SYSDATETIME(),
               updated_by = @logempcd
        WHERE  so_price_list_header_id = @so_price_list_header_id
          AND  set_no >= @new_set
          AND  delete_mark <> 'Y';

        /* Both halves of the pair - a price line is always USD and THB, so both
           cells exist to type into; the one that was not typed starts at 0.

           Both carry line_no 1: they are one worksheet line, and line_no counts
           tier rows within the set, not currencies. */
        INSERT INTO SO.so_price_list_detail
            (so_price_list_header_id, set_no, line_no, article, design_no, article_variant,
             fabric_name, composition, full_width_cm, usable_width_cm, weight_gsm,
             moq, qty_min, qty_max, qty_unit, color_tier, currency, price,
             active, notes, created_by)
        SELECT @so_price_list_header_id, @new_set, v.line_no, @article, @design_no, @article_variant,
               @fabric_name, @composition, @full_width_cm, @usable_width_cm, @weight_gsm,
               @moq, @qty_min, @qty_max, @qty_unit, @color_tier, v.ccy, v.price,
               'Y', @notes, @logempcd
        FROM  (VALUES (1, @currency, @price),
                      (1, @other,    CAST(0 AS decimal(18,4)))) AS v(line_no, ccy, price);

    COMMIT;

    SELECT @new_set AS set_no,
           (SELECT COUNT(*) FROM SO.so_price_list_detail
            WHERE so_price_list_header_id = @so_price_list_header_id
              AND set_no = @new_set AND delete_mark <> 'Y') AS rows_created;
END
GO

/* ---------------------------------------------------------------------------
   renumber_price_list_sets  -  compact set_no back to 1..n.

   Soft-deleting rows leaves gaps. Nothing breaks, but a tidy sequence keeps
   the insert arithmetic obvious, so this is here to run after a clean-up.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_renumber_price_list_sets','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_renumber_price_list_sets;
GO
-- =============================================
-- Description: Compact a price list's set_no to a dense 1..n, order preserved.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_renumber_price_list_sets 29, 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_renumber_price_list_sets]
    @so_price_list_header_id bigint,
    @logempcd                varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    ;WITH ranked AS (
        SELECT so_price_list_detail_id,
               DENSE_RANK() OVER (ORDER BY set_no) AS new_set_no
        FROM   SO.so_price_list_detail
        WHERE  so_price_list_header_id = @so_price_list_header_id
          AND  delete_mark <> 'Y'
    )
    UPDATE d
    SET    d.set_no = r.new_set_no
    FROM   SO.so_price_list_detail d
    JOIN   ranked r ON r.so_price_list_detail_id = d.so_price_list_detail_id
    WHERE  d.set_no <> r.new_set_no;

    SELECT @@ROWCOUNT AS rows_renumbered;
END
GO


/* ---------------------------------------------------------------------------
   copy_price_list_set  -  duplicate a grid row and drop it in somewhere else.

   This is the right-click Copy / Insert pair. A grid row is a whole set - every
   colour tier, both currencies - so copying one means copying all of its lines,
   not just the cell that was clicked.

   line_no is carried over UNCHANGED, so the copy reads top to bottom exactly
   like the row it came from: the tiers stay in the order the user put them in,
   and each USD/THB pair keeps the single line_no it shares. Only set_no moves,
   which is what decides where the row lands.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_copy_price_list_set','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_copy_price_list_set;
GO
-- =============================================
-- Description: Copy every line of one set and insert it after another set.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_copy_price_list_set 29, 1, 4, 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_copy_price_list_set]
    @so_price_list_header_id bigint,
    @source_set_no           int,                  -- the row that was copied
    @after_set_no            int         = null,   -- NULL or 0 = put it first
    @logempcd                varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM SO.so_price_list_header
                   WHERE so_price_list_header_id = @so_price_list_header_id
                     AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('Price list not found, or already deleted.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM SO.so_price_list_detail
                   WHERE so_price_list_header_id = @so_price_list_header_id
                     AND set_no = @source_set_no
                     AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('The line being copied no longer exists.', 16, 1);
        RETURN;
    END

    IF @after_set_no IS NULL SET @after_set_no = 0;

    DECLARE @new_set int = @after_set_no + 1;

    BEGIN TRAN;

        /* Make room first. The source may itself sit below the insertion point,
           in which case this shifts it too - so read its number afterwards, not
           before, or the copy silently duplicates the wrong row. */
        UPDATE SO.so_price_list_detail
        SET    set_no            = set_no + 1,
               last_updated_date = SYSDATETIME(),
               updated_by        = @logempcd
        WHERE  so_price_list_header_id = @so_price_list_header_id
          AND  set_no >= @new_set
          AND  delete_mark <> 'Y';

        DECLARE @from int =
            CASE WHEN @source_set_no >= @new_set THEN @source_set_no + 1
                 ELSE @source_set_no END;

        INSERT INTO SO.so_price_list_detail
            (so_price_list_header_id, set_no, line_no, article, design_no, article_variant,
             fabric_name, composition, full_width_cm, usable_width_cm, weight_gsm,
             moq, qty_min, qty_max, qty_unit, color_tier, currency, price,
             active, notes, created_by)
        SELECT d.so_price_list_header_id, @new_set, d.line_no, d.article, d.design_no,
               d.article_variant, d.fabric_name, d.composition, d.full_width_cm,
               d.usable_width_cm, d.weight_gsm, d.moq, d.qty_min, d.qty_max, d.qty_unit,
               d.color_tier, d.currency, d.price, d.active, d.notes, @logempcd
        FROM   SO.so_price_list_detail d
        WHERE  d.so_price_list_header_id = @so_price_list_header_id
          AND  d.set_no = @from
          AND  d.delete_mark <> 'Y';

        DECLARE @rows int = @@ROWCOUNT;

    COMMIT;

    SELECT @new_set AS set_no, @rows AS rows_created;
END
GO

/* ---------------------------------------------------------------------------
   delete_price_list_set  -  withdraw a whole grid row.

   SOFT, like every other delete here: delete_mark goes to 'Y' and the rows stay.
   Nothing reads them afterwards - every select filters delete_mark <> 'Y' - so
   the row leaves the grid, but a mistaken delete is still recoverable in the
   table rather than gone.

   set_no is deliberately NOT renumbered. The rows below keep the numbers they
   had, so anything the user had positioned stays where it was put;
   renumber_price_list_sets is there for a deliberate tidy-up.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_delete_price_list_set','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_delete_price_list_set;
GO
-- =============================================
-- Description: Soft-delete every price line in one set (one grid row).
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_delete_price_list_set 29, 3, 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_delete_price_list_set]
    @so_price_list_header_id bigint,
    @set_no                  int,
    @logempcd                varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    UPDATE SO.so_price_list_detail
    SET    delete_mark       = 'Y',
           deleted_by        = @logempcd,
           last_updated_date = SYSDATETIME(),
           updated_by        = @logempcd
    WHERE  so_price_list_header_id = @so_price_list_header_id
      AND  set_no = @set_no
      AND  delete_mark <> 'Y';

    /* Captured immediately: the IF below is itself a statement, so by the time
       the SELECT runs @@ROWCOUNT no longer refers to the UPDATE. */
    DECLARE @lines int = @@ROWCOUNT;

    IF @lines = 0
    BEGIN
        RAISERROR('That line no longer exists, or was already deleted.', 16, 1);
        RETURN;
    END

    SELECT @set_no AS set_no, @lines AS lines_deleted;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 5 (sets) created';
GO
