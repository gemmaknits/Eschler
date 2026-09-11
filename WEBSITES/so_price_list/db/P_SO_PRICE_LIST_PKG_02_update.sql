/* ============================================================================
   SO_PRICE_LIST_PKG  -  part 2 of 3 : write procedures
   Target: SQL Server 2014. @logempcd carries the acting user into the audit
   columns, per house convention.

   Upsert shape: pass the id to update, NULL to insert. Both return the row id
   as a single-column result set named so the API reads it the same way either
   way.
   ============================================================================ */

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
    FROM   dbo.so_price_list_header
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

    IF EXISTS (SELECT 1 FROM dbo.so_price_list_header
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
        INSERT INTO dbo.so_price_list_header
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
        UPDATE dbo.so_price_list_header
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
    @article                 nvarchar(30)  = null,   -- bind as string, always
    @design_no               char(20)      = null,
    @article_variant         nvarchar(20)  = null,
    @fabric_name             nvarchar(120) = null,
    @composition             nvarchar(200) = null,
    @full_width_cm           nvarchar(30)  = null,
    @usable_width_cm         nvarchar(30)  = null,
    @weight_gsm              nvarchar(30)  = null,
    @moq                     nvarchar(30)  = null,
    @qty_min                 int           = null,
    @qty_max                 int           = null,   -- NULL = open upper bound
    @qty_unit                char(2)       = 'M',
    @color_tier              nvarchar(30)  = null,
    @currency                char(3)       = null,
    @price                   decimal(18,4) = null,
    @line_no                 int           = null,
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
       AND NOT EXISTS (SELECT 1 FROM dbo.so_price_list_header
                       WHERE so_price_list_header_id = @so_price_list_header_id
                         AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('Price list not found, or already deleted.', 16, 1);
        RETURN;
    END

    IF @currency IS NOT NULL AND @currency NOT IN ('USD','THB')
    BEGIN
        RAISERROR('Currency must be USD or THB.', 16, 1);
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
        IF @qty_unit   IS NULL SET @qty_unit = 'M';
        IF @color_tier IS NULL OR LTRIM(RTRIM(@color_tier)) = ''
            SET @color_tier = N'Unspecified';

        /* append to the end of the list unless a position was given */
        IF @line_no IS NULL
            SELECT @line_no = ISNULL(MAX(line_no), 0) + 1
            FROM   dbo.so_price_list_detail
            WHERE  so_price_list_header_id = @so_price_list_header_id;

        INSERT INTO dbo.so_price_list_detail
            (so_price_list_header_id, line_no, article, design_no, article_variant,
             fabric_name, composition, full_width_cm, usable_width_cm, weight_gsm,
             moq, qty_min, qty_max, qty_unit, color_tier, currency, price,
             notes, created_by)
        VALUES
            (@so_price_list_header_id, @line_no, @article, @design_no, @article_variant,
             @fabric_name, @composition, @full_width_cm, @usable_width_cm, @weight_gsm,
             @moq, @qty_min, @qty_max, @qty_unit, @color_tier, @currency, @price,
             @notes, @logempcd);

        SET @so_price_list_detail_id = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        /* only overwrite what was supplied - the grid edits one cell at a time */
        UPDATE dbo.so_price_list_detail
        SET    article           = ISNULL(@article,         article),
               design_no         = ISNULL(@design_no,       design_no),
               article_variant   = ISNULL(@article_variant, article_variant),
               fabric_name       = ISNULL(@fabric_name,     fabric_name),
               composition       = ISNULL(@composition,     composition),
               full_width_cm     = ISNULL(@full_width_cm,   full_width_cm),
               usable_width_cm   = ISNULL(@usable_width_cm, usable_width_cm),
               weight_gsm        = ISNULL(@weight_gsm,      weight_gsm),
               moq               = ISNULL(@moq,             moq),
               qty_min           = ISNULL(@qty_min,         qty_min),
               qty_max           = @qty_max,          -- NULL is meaningful here
               qty_unit          = ISNULL(@qty_unit,        qty_unit),
               color_tier        = ISNULL(@color_tier,      color_tier),
               currency          = ISNULL(@currency,        currency),
               price             = ISNULL(@price,           price),
               line_no           = ISNULL(@line_no,         line_no),
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
    FROM   dbo.so_price_list_detail d
    JOIN   dbo.so_price_list_detail me
             ON me.so_price_list_detail_id = @so_price_list_detail_id
    WHERE  d.so_price_list_header_id = me.so_price_list_header_id
      AND  d.article                 = me.article
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

