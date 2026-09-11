/* ============================================================================
   SO_PRICE_LIST_PKG  -  part 2 of 3 : write procedures
   Target: SQL Server 2014. @logempcd carries the acting user into the audit
   columns, per house convention.

   Upsert shape: pass the id to update, NULL to insert. Both return the row id
   as a single-column result set named so the API reads it the same way either
   way.
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
   validate_price_list_name  -  list_name has a filtered UNIQUE index, so a
   clashing save fails at the database. Check first and report it as a message
   instead of surfacing a raw index violation.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_validate_price_list_name','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_validate_price_list_name;
GO
-- =============================================
-- Description: Is this list_name free? Returns is_valid + message.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_validate_price_list_name 'CENTER', null, ''
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_validate_price_list_name]
    @list_name               nvarchar(60),
    @so_price_list_header_id bigint      = null,   -- exclude self when renaming
    @logempcd                varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @taken_by bigint;

    IF @list_name IS NULL OR LTRIM(RTRIM(@list_name)) = ''
    BEGIN
        SELECT CAST(0 AS bit) AS is_valid,
               N'List name is required.' AS message,
               CAST(NULL AS bigint) AS conflicting_header_id;
        RETURN;
    END

    SELECT TOP 1 @taken_by = so_price_list_header_id
    FROM   SO.so_price_list_header
    WHERE  list_name = @list_name
      AND  delete_mark <> 'Y'
      AND (@so_price_list_header_id IS NULL
           OR so_price_list_header_id <> @so_price_list_header_id);

    IF @taken_by IS NOT NULL
        SELECT CAST(0 AS bit) AS is_valid,
               N'Another price list already uses this name.' AS message,
               @taken_by AS conflicting_header_id;
    ELSE
        SELECT CAST(1 AS bit) AS is_valid,
               N'' AS message,
               CAST(NULL AS bigint) AS conflicting_header_id;
END
GO

/* ---------------------------------------------------------------------------
   update_price_list  -  insert or update a header.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_update_price_list','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_update_price_list;
GO
-- =============================================
-- Description: Upsert a price list header. NULL id inserts, otherwise updates.
--              customer_name is snapshotted from customers at save time.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_update_price_list null,'ANITA 2027','New season',173,...,'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_update_price_list]
    @so_price_list_header_id bigint        = null,
    @list_name               nvarchar(60)  = null,
    @list_desc               nvarchar(400) = null,
    @customer_id             bigint        = null,
    @customer_excel          nvarchar(120) = null,
    @list_date               date          = null,
    @valid_from              date          = null,
    @valid_to                date          = null,
    @terms                   nvarchar(60)  = null,
    @quote_ref               nvarchar(200) = null,
    @sonoid                  nvarchar(30)  = null,
    @so_line_id              bigint        = null,
    @notes                   nvarchar(500) = null,
    @logempcd                varchar(15)   = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @list_name IS NULL OR LTRIM(RTRIM(@list_name)) = ''
    BEGIN
        RAISERROR('List name is required.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM SO.so_price_list_header
               WHERE list_name = @list_name AND delete_mark <> 'Y'
                 AND (@so_price_list_header_id IS NULL
                      OR so_price_list_header_id <> @so_price_list_header_id))
    BEGIN
        RAISERROR('Another price list already uses this name.', 16, 1);
        RETURN;
    END

    IF @valid_from IS NOT NULL AND @valid_to IS NOT NULL AND @valid_to < @valid_from
    BEGIN
        RAISERROR('Valid to cannot be earlier than valid from.', 16, 1);
        RETURN;
    END

    /* snapshot the customer name so the list still reads correctly if the
       customer record is later renamed */
    DECLARE @customer_name nvarchar(100) = null;
    IF @customer_id IS NOT NULL
        SELECT @customer_name = name FROM dbo.customers WHERE customer_id = @customer_id;

    IF @so_price_list_header_id IS NULL
    BEGIN
        INSERT INTO SO.so_price_list_header
            (list_name, list_desc, customer_id, customer_name, customer_excel,
             list_date, valid_from, valid_to, terms, quote_ref,
             sonoid, so_line_id, notes, created_by)
        VALUES
            (@list_name, @list_desc, @customer_id, @customer_name, @customer_excel,
             @list_date, @valid_from, @valid_to, @terms, @quote_ref,
             @sonoid, @so_line_id, @notes, @logempcd);

        SET @so_price_list_header_id = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE SO.so_price_list_header
        SET    list_name         = @list_name,
               list_desc         = @list_desc,
               customer_id       = @customer_id,
               customer_name     = @customer_name,
               customer_excel    = @customer_excel,
               list_date         = @list_date,
               valid_from        = @valid_from,
               valid_to          = @valid_to,
               terms             = @terms,
               quote_ref         = @quote_ref,
               sonoid            = @sonoid,
               so_line_id        = @so_line_id,
               notes             = @notes,
               last_updated_date = SYSDATETIME(),
               updated_by        = @logempcd
        WHERE  so_price_list_header_id = @so_price_list_header_id
          AND  delete_mark <> 'Y';

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('Price list not found, or already deleted.', 16, 1);
            RETURN;
        END
    END

    SELECT @so_price_list_header_id AS so_price_list_header_id;
END
GO

/* ---------------------------------------------------------------------------
   update_price_list_detail  -  insert or update one price line.
   Duplicates on the business key are ALLOWED by design: order entry shows both
   and the user picks. So this does not reject a clash - it reports how many
   lines now share the key, letting the UI flag it.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_update_price_list_detail','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_update_price_list_detail;
GO
-- =============================================
-- Description: Upsert one price line. Returns the id and the resulting
--              conflict_count for its business key.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_update_price_list_detail null,31,'255484',...,'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_update_price_list_detail]
    @so_price_list_detail_id bigint        = null,
    @so_price_list_header_id bigint        = null,
    @set_no                  int           = null,   -- which grid row to join
    @design_no               nvarchar(60)  = null,   -- the identifier; bind as string
    @article                 nvarchar(30)  = null,   -- mirror of design_no, kept for get_price
    @article_variant         nvarchar(20)  = null,
    @fabric_name             nvarchar(200) = null,
    @composition             nvarchar(200) = null,
    @full_width_cm           nvarchar(60)  = null,
    @usable_width_cm         nvarchar(60)  = null,
    @weight_gsm              nvarchar(60)  = null,
    @moq                     nvarchar(60)  = null,
    @qty_min                 int           = null,
    @qty_max                 int           = null,
    @clear_qty_max           bit           = 0,      -- explicit: NULL is a real value
    @qty_unit                nvarchar(10)  = N'MTS',
    @color_tier              nvarchar(30)  = null,
    @currency                char(3)       = null,
    @price                   decimal(18,4) = null,
    @line_no                 int           = null,
    @after_line_no           int           = null,   -- insert after this line; NULL appends
    @active                  char(1)       = null,   -- 'Y' | 'N'
    @notes                   nvarchar(500) = null,
    @logempcd                varchar(15)   = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    /* --- validate --- */
    IF @so_price_list_detail_id IS NULL AND @so_price_list_header_id IS NULL
    BEGIN
        RAISERROR('Price list is required for a new line.', 16, 1);
        RETURN;
    END

    IF @so_price_list_detail_id IS NULL
       AND NOT EXISTS (SELECT 1 FROM SO.so_price_list_header
                       WHERE so_price_list_header_id = @so_price_list_header_id
                         AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('Price list not found, or already deleted.', 16, 1);
        RETURN;
    END

    /* design_no is the identifier; article mirrors it.

       Accept either one and fill in the other, so the two can never drift
       apart. The grid sends design_no; get_price and the VB.NET order-entry
       client still read article, which is why both columns are written. */
    IF @design_no IS NULL OR LTRIM(RTRIM(@design_no)) = '' SET @design_no = @article;
    IF @article   IS NULL OR LTRIM(RTRIM(@article))   = '' SET @article   = @design_no;

    IF @active IS NOT NULL AND @active NOT IN ('Y','N')
    BEGIN
        RAISERROR('Active must be Y or N.', 16, 1);
        RETURN;
    END

    IF @currency IS NOT NULL AND @currency NOT IN ('USD','THB')
    BEGIN
        RAISERROR('Currency must be USD or THB.', 16, 1);
        RETURN;
    END

    /* The unit has to be one the system knows. dbo.uom is that list, and "M"
       was never in it - which is how 8,742 lines came to carry a unit that
       meant nothing. A unit that is not there is refused here, so it cannot be
       stored and discovered later. */
    IF @qty_unit IS NOT NULL AND LTRIM(RTRIM(@qty_unit)) <> ''
       AND SO.F_SO_PRICE_LIST_uom_ok(@qty_unit) = 0
    BEGIN
        RAISERROR('Unit "%s" is not a unit of measure in the system. Pick one from the list.',
                  16, 1, @qty_unit);
        RETURN;
    END

    IF @qty_min IS NOT NULL AND @qty_min < 0
    BEGIN
        RAISERROR('Minimum quantity cannot be negative.', 16, 1);
        RETURN;
    END

    IF @qty_min IS NOT NULL AND @qty_max IS NOT NULL AND @qty_max < @qty_min
    BEGIN
        RAISERROR('Maximum quantity cannot be less than minimum quantity.', 16, 1);
        RETURN;
    END

    IF @price IS NOT NULL AND @price < 0
    BEGIN
        RAISERROR('Price cannot be negative.', 16, 1);
        RETURN;
    END

    IF @so_price_list_detail_id IS NULL
    BEGIN
        IF @qty_min    IS NULL SET @qty_min = 0;
        IF @qty_unit IS NULL OR LTRIM(RTRIM(@qty_unit)) = '' SET @qty_unit = N'MTS';
        IF @color_tier IS NULL OR LTRIM(RTRIM(@color_tier)) = ''
            SET @color_tier = N'Unspecified';

        /* Which grid row does this line join?

           A set is one article, variant and quantity band - it spans EVERY
           colour tier, because tiers are columns within the row. So the lookup
           must NOT match on colour tier: doing so meant a new tier started its
           own set, and typing into an empty tier cell split the row in two.

           The caller should pass @set_no, since only it knows which row was
           clicked when several share an article and band. Without it, join the
           first set with that article and band, or start a new one. */
        IF @set_no IS NULL
            SELECT TOP 1 @set_no = set_no
            FROM   SO.so_price_list_detail
            WHERE  so_price_list_header_id = @so_price_list_header_id
              AND  design_no = @design_no
              AND  ISNULL(article_variant,'') = ISNULL(@article_variant,'')
              AND  qty_min = @qty_min
              AND  ISNULL(qty_max,-1) = ISNULL(@qty_max,-1)
              AND  qty_unit = @qty_unit
              AND  delete_mark <> 'Y'
            ORDER BY set_no;

        IF @set_no IS NULL
            SELECT @set_no = ISNULL(MAX(set_no), 0) + 1
            FROM   SO.so_price_list_detail
            WHERE  so_price_list_header_id = @so_price_list_header_id;

        /* The cell may already hold a line.

           Typing one currency creates its counterpart alongside it at 0, so
           the other half of the pair usually EXISTS before it is first typed
           into. Filling it in is an update of that line - a set holds at most
           one row per tier and currency, and inserting here would put a second
           Dark USD in the same grid row.

           The grid passes the detail id and never reaches this, but a caller
           that only knows which cell was clicked would otherwise duplicate. */
        SELECT TOP 1 @so_price_list_detail_id = so_price_list_detail_id
        FROM   SO.so_price_list_detail
        WHERE  so_price_list_header_id = @so_price_list_header_id
          AND  set_no     = @set_no
          AND  color_tier = @color_tier
          AND  currency   = @currency
          AND  delete_mark <> 'Y'
        ORDER BY so_price_list_detail_id;
    END

    IF @so_price_list_detail_id IS NULL
    BEGIN
        /* Where in the set does the new line go?

           line_no is the position the user put the line at, and it has to stay
           there. A tier added between lines 2 and 3 belongs between 2 and 3 -
           it is NOT re-sorted into tier order, because the order within a set
           is the user's, not the tier list's.

           So make room and drop the pair into the gap. One line_no per tier
           row, NOT per currency: the USD and THB halves are the same worksheet
           line, so they share a number and the shift is 1, not 2.
           @after_line_no NULL means append to the end of the set. */
        IF @line_no IS NULL
        BEGIN
            IF @after_line_no IS NULL
                SELECT @line_no = ISNULL(MAX(line_no), 0) + 1
                FROM   SO.so_price_list_detail
                WHERE  so_price_list_header_id = @so_price_list_header_id
                  AND  set_no = @set_no;
            ELSE
            BEGIN
                SET @line_no = @after_line_no + 1;

                UPDATE SO.so_price_list_detail
                SET    line_no           = line_no + 1,
                       last_updated_date = SYSDATETIME(),
                       updated_by        = @logempcd
                WHERE  so_price_list_header_id = @so_price_list_header_id
                  AND  set_no  = @set_no
                  AND  line_no >= @line_no
                  AND  delete_mark <> 'Y';
            END
        END

        INSERT INTO SO.so_price_list_detail
            (so_price_list_header_id, set_no, line_no, article, design_no, article_variant,
             fabric_name, composition, full_width_cm, usable_width_cm, weight_gsm,
             moq, qty_min, qty_max, qty_unit, color_tier, currency, price,
             active, notes, created_by)
        VALUES
            (@so_price_list_header_id, @set_no, @line_no, @article, @design_no, @article_variant,
             @fabric_name, @composition, @full_width_cm, @usable_width_cm, @weight_gsm,
             @moq, @qty_min, @qty_max, @qty_unit, @color_tier, @currency, @price,
             ISNULL(@active,'Y'), @notes, @logempcd);

        SET @so_price_list_detail_id = SCOPE_IDENTITY();

        /* ------------------------------------------------------------------
           A price line always comes as a USD/THB pair.

           The grid shows the two currencies folded into one row, so creating
           only the half that was typed leaves a half-empty row and no cell to
           type the other price into later. Insert the counterpart at 0 so both
           cells exist from the start; entering the real figure then UPDATES
           that row rather than inserting a second one.

           The counterpart carries the SAME line_no: one worksheet line holds
           both currencies, so line_no numbers the tier row, not the currency.
           (set_no, line_no) therefore names one tier row of one grid row, and
           reading a pair back is a single equality test rather than arithmetic
           on adjacent numbers.

           Skipped when the counterpart already exists - typing USD after THB
           must not create a duplicate.
           ------------------------------------------------------------------ */
        DECLARE @other char(3) =
            CASE @currency WHEN 'USD' THEN 'THB' WHEN 'THB' THEN 'USD' END;

        IF @other IS NOT NULL
           AND NOT EXISTS (
               SELECT 1 FROM SO.so_price_list_detail
               WHERE  so_price_list_header_id = @so_price_list_header_id
                 AND  set_no = @set_no
                 AND  color_tier = @color_tier
                 AND  currency = @other
                 AND  delete_mark <> 'Y')
        BEGIN
            INSERT INTO SO.so_price_list_detail
                (so_price_list_header_id, set_no, line_no, article, design_no, article_variant,
                 fabric_name, composition, full_width_cm, usable_width_cm, weight_gsm,
                 moq, qty_min, qty_max, qty_unit, color_tier, currency, price,
                 active, notes, created_by)
            VALUES
                (@so_price_list_header_id, @set_no, @line_no, @article, @design_no, @article_variant,
                 @fabric_name, @composition, @full_width_cm, @usable_width_cm, @weight_gsm,
                 @moq, @qty_min, @qty_max, @qty_unit, @color_tier, @other, 0,
                 ISNULL(@active,'Y'), @notes, @logempcd);
        END

    END
    ELSE
    BEGIN
        /* only overwrite what was supplied - the grid edits one cell at a time */
        UPDATE SO.so_price_list_detail
        SET    design_no         = ISNULL(@design_no,       design_no),
               article           = ISNULL(@article,         article),
               article_variant   = ISNULL(@article_variant, article_variant),
               fabric_name       = ISNULL(@fabric_name,     fabric_name),
               composition       = ISNULL(@composition,     composition),
               full_width_cm     = ISNULL(@full_width_cm,   full_width_cm),
               usable_width_cm   = ISNULL(@usable_width_cm, usable_width_cm),
               weight_gsm        = ISNULL(@weight_gsm,      weight_gsm),
               moq               = ISNULL(@moq,             moq),
               qty_min           = ISNULL(@qty_min,         qty_min),
               /* An open upper bound is a real value, so it cannot be signalled
                  by passing NULL - that is indistinguishable from "leave alone",
                  and a price-only save would silently wipe the band. */
               qty_max           = CASE WHEN @clear_qty_max = 1 THEN NULL
                                        ELSE ISNULL(@qty_max, qty_max) END,
               qty_unit          = ISNULL(@qty_unit,        qty_unit),
               color_tier        = ISNULL(@color_tier,      color_tier),
               currency          = ISNULL(@currency,        currency),
               price             = ISNULL(@price,           price),
               line_no           = ISNULL(@line_no,         line_no),
               active            = ISNULL(@active,          active),
               notes             = ISNULL(@notes,           notes),
               last_updated_date = SYSDATETIME(),
               updated_by        = @logempcd
        WHERE  so_price_list_detail_id = @so_price_list_detail_id
          AND  delete_mark <> 'Y';

        IF @@ROWCOUNT = 0
        BEGIN
            RAISERROR('Price line not found, or already deleted.', 16, 1);
            RETURN;
        END
    END

    /* how many lines now share this business key? 1 = order entry resolves it
       automatically, more = the user will be asked to choose */
    DECLARE @conflict_count int;

    SELECT @conflict_count = COUNT(*)
    FROM   SO.so_price_list_detail d
    JOIN   SO.so_price_list_detail me
             ON me.so_price_list_detail_id = @so_price_list_detail_id
    WHERE  d.so_price_list_header_id = me.so_price_list_header_id
      AND  d.design_no               = me.design_no
      AND  ISNULL(d.article_variant,'') = ISNULL(me.article_variant,'')
      AND  d.qty_min                 = me.qty_min
      AND  ISNULL(d.qty_max,-1)      = ISNULL(me.qty_max,-1)
      AND  d.qty_unit                = me.qty_unit
      AND  d.color_tier              = me.color_tier
      AND  d.currency                = me.currency
      AND  d.delete_mark <> 'Y';

    SELECT @so_price_list_detail_id AS so_price_list_detail_id,
           @conflict_count          AS conflict_count;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 2 (update) created';
GO

