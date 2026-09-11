/* ============================================================================
   SO_PRICE_LIST_PKG  -  part 3 of 3 : delete + copy
   Deletes are SOFT throughout (delete_mark / deleted_by), matching the tables.
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
   delete_price_list  -  soft-delete a header and all of its lines.
   Frees the list_name, because the UNIQUE index is filtered on delete_mark.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_delete_price_list','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_delete_price_list;
GO
-- =============================================
-- Description: Soft-delete a price list and cascade to its detail lines.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_delete_price_list 54, 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_delete_price_list]
    @so_price_list_header_id bigint,
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

    DECLARE @lines int;

    BEGIN TRAN;

        UPDATE SO.so_price_list_detail
        SET    delete_mark       = 'Y',
               deleted_by        = @logempcd,
               last_updated_date = SYSDATETIME(),
               updated_by        = @logempcd
        WHERE  so_price_list_header_id = @so_price_list_header_id
          AND  delete_mark <> 'Y';

        SET @lines = @@ROWCOUNT;

        UPDATE SO.so_price_list_header
        SET    delete_mark       = 'Y',
               deleted_by        = @logempcd,
               last_updated_date = SYSDATETIME(),
               updated_by        = @logempcd
        WHERE  so_price_list_header_id = @so_price_list_header_id;

    COMMIT;

    SELECT @so_price_list_header_id AS so_price_list_header_id,
           @lines                   AS deleted_line_count;
END
GO

/* ---------------------------------------------------------------------------
   delete_price_list_detail  -  soft-delete one line.
   Handy for resolving a duplicate: retire the wrong line, keep the right one,
   and the cell stops asking the user to choose.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_delete_price_list_detail','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_delete_price_list_detail;
GO
-- =============================================
-- Description: Soft-delete one price line. Returns how many lines still share
--              its business key, so the caller can see a conflict clear.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_delete_price_list_detail 3417, 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_delete_price_list_detail]
    @so_price_list_detail_id bigint,
    @logempcd                varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @hdr bigint, @design_no nvarchar(60), @variant nvarchar(20),
            @qmin int, @qmax int, @qunit char(2), @tier nvarchar(30), @ccy char(3);

    SELECT @hdr = so_price_list_header_id, @design_no = design_no,
           @variant = article_variant, @qmin = qty_min, @qmax = qty_max,
           @qunit = qty_unit, @tier = color_tier, @ccy = currency
    FROM   SO.so_price_list_detail
    WHERE  so_price_list_detail_id = @so_price_list_detail_id
      AND  delete_mark <> 'Y';

    IF @hdr IS NULL
    BEGIN
        RAISERROR('Price line not found, or already deleted.', 16, 1);
        RETURN;
    END

    UPDATE SO.so_price_list_detail
    SET    delete_mark       = 'Y',
           deleted_by        = @logempcd,
           last_updated_date = SYSDATETIME(),
           updated_by        = @logempcd
    WHERE  so_price_list_detail_id = @so_price_list_detail_id;

    SELECT @so_price_list_detail_id AS so_price_list_detail_id,
           (SELECT COUNT(*)
            FROM   SO.so_price_list_detail
            WHERE  so_price_list_header_id = @hdr
              AND  design_no = @design_no
              AND  ISNULL(article_variant,'') = ISNULL(@variant,'')
              AND  qty_min = @qmin
              AND  ISNULL(qty_max,-1) = ISNULL(@qmax,-1)
              AND  qty_unit = @qunit
              AND  color_tier = @tier
              AND  currency = @ccy
              AND  delete_mark <> 'Y') AS remaining_count;
END
GO

/* ---------------------------------------------------------------------------
   copy_price_list  -  clone a header and its lines.
   The expected way to roll a list forward to a new season or hand an existing
   set of prices to another customer, without retyping it.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_copy_price_list','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_copy_price_list;
GO
-- =============================================
-- Description: Duplicate a price list, optionally to a different customer and
--              validity window. Returns the new header id.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_copy_price_list 31,'CENTER 2027',null,'2027-01-01',null,'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_copy_price_list]
    @source_header_id bigint,
    @list_name        nvarchar(60),
    @customer_id      bigint      = null,   -- null = keep the source's customer
    @valid_from       date        = null,
    @valid_to         date        = null,
    @logempcd         varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM SO.so_price_list_header
                   WHERE so_price_list_header_id = @source_header_id
                     AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('Source price list not found, or already deleted.', 16, 1);
        RETURN;
    END

    IF @list_name IS NULL OR LTRIM(RTRIM(@list_name)) = ''
    BEGIN
        RAISERROR('List name is required.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM SO.so_price_list_header
               WHERE list_name = @list_name AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('Another price list already uses this name.', 16, 1);
        RETURN;
    END

    DECLARE @new_id bigint, @lines int, @customer_name nvarchar(100);

    IF @customer_id IS NOT NULL
        SELECT @customer_name = name FROM dbo.customers WHERE customer_id = @customer_id;

    BEGIN TRAN;

        INSERT INTO SO.so_price_list_header
            (list_name, list_desc, customer_id, customer_name, customer_excel,
             list_date, valid_from, valid_to, terms, quote_ref,
             sonoid, so_line_id, source_sheet, notes, created_by)
        SELECT @list_name,
               N'Copied from ' + h.list_name,
               ISNULL(@customer_id, h.customer_id),
               CASE WHEN @customer_id IS NULL THEN h.customer_name ELSE @customer_name END,
               h.customer_excel,
               CAST(GETDATE() AS date),
               @valid_from, @valid_to,
               h.terms, h.quote_ref,
               h.sonoid, h.so_line_id, h.source_sheet, h.notes,
               @logempcd
        FROM   SO.so_price_list_header h
        WHERE  h.so_price_list_header_id = @source_header_id;

        SET @new_id = SCOPE_IDENTITY();

        /* set_no travels with the copy. It is NOT NULL and has no default, so
           leaving it out of this list did not merely lose the grouping - the
           insert itself would fail, taking the whole copy with it. */
        INSERT INTO SO.so_price_list_detail
            (so_price_list_header_id, set_no, line_no, article, design_no, article_variant,
             fabric_name, composition, full_width_cm, usable_width_cm, weight_gsm,
             moq, qty_min, qty_max, qty_unit, color_tier, currency, price,
             source_row, notes, created_by)
        SELECT @new_id, d.set_no, d.line_no, d.article, d.design_no, d.article_variant,
               d.fabric_name, d.composition, d.full_width_cm, d.usable_width_cm,
               d.weight_gsm, d.moq, d.qty_min, d.qty_max, d.qty_unit,
               d.color_tier, d.currency, d.price,
               d.source_row, d.notes, @logempcd
        FROM   SO.so_price_list_detail d
        WHERE  d.so_price_list_header_id = @source_header_id
          AND  d.delete_mark <> 'Y';

        SET @lines = @@ROWCOUNT;

    COMMIT;

    SELECT @new_id AS so_price_list_header_id,
           @lines  AS copied_line_count;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 3 (delete/copy) created';
GO

