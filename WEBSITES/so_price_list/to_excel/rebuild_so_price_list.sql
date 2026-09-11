/* ============================================================================
   Rebuild dbo.so_price_list with currency + price (tall) instead of
   price_usd + price_thb (wide).

   One row per (customer x article x variant x qty band x color tier x CURRENCY).
   The CENTER sheet carries both currencies on one worksheet line; the loader
   fans that into two rows. All other sheets already emit one row per currency.

   Entry surfaces (Excel template, mock_ui/grid.html) stay WIDE for convenience
   - a single visual row with USD and THB side by side - and the loader/API is
   what splits and re-pairs. Deliberate: storage is normalized, entry is not.

   Safe to run: nothing in the table is mapped or hand-edited (customer_id and
   design_no are all NULL, no rows edited or soft-deleted) and every row is
   reproducible from price_book_extracted.pb.csv via load_so_price_list.ps1.
   A timestamped backup is taken anyway.
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

/* --- 1. Safety copy of the current (wide) table ------------------------- */
DECLARE @bak sysname = N'so_price_list_bak_' + CONVERT(char(8), GETDATE(), 112);

IF OBJECT_ID('dbo.so_price_list', 'U') IS NOT NULL
BEGIN
    IF OBJECT_ID('dbo.' + @bak, 'U') IS NOT NULL
        EXEC(N'DROP TABLE dbo.' + @bak + N';');

    EXEC(N'SELECT * INTO dbo.' + @bak + N' FROM dbo.so_price_list;');
    PRINT 'Backed up to dbo.' + @bak;

    DROP TABLE dbo.so_price_list;
    PRINT 'Dropped dbo.so_price_list';
END
GO

/* --- 2. Recreate, tall ------------------------------------------------- */
CREATE TABLE dbo.so_price_list
(
    so_price_list_id   bigint         IDENTITY(1,1) NOT NULL,

    -- who
    customer_id        bigint         NULL,          -- FK candidate -> customers.customer_id (unenforced)
    customer_name      nvarchar(100)  NULL,          -- snapshot of customers.name at mapping time
    customer_excel     nvarchar(120)  NULL,          -- raw sheet name / customer text from the workbook

    -- what
    article            nvarchar(30)   NULL,          -- raw article as uploaded; ALWAYS text, never numeric
    design_no          char(20)       NULL,          -- validated against dm.Design_no post-mapping
    article_variant    nvarchar(20)   NULL,
    fabric_name        nvarchar(120)  NULL,
    composition        nvarchar(200)  NULL,
    full_width_cm      nvarchar(30)   NULL,
    usable_width_cm    nvarchar(30)   NULL,
    weight_gsm         nvarchar(30)   NULL,
    moq                nvarchar(30)   NULL,

    -- price grain
    qty_min            int            NOT NULL,
    qty_max            int            NULL,          -- NULL = open upper bound
    qty_unit           char(2)        NOT NULL CONSTRAINT DF_so_price_list_qty_unit DEFAULT ('M'),   -- 'M' | 'KG'
    color_tier         nvarchar(30)   NOT NULL,      -- PFE/PFD, White, Light, Medium, Dark, All_colors, ...
    currency           char(3)        NOT NULL,      -- 'USD' | 'THB'  <-- part of the grain
    price              decimal(18,4)  NULL,          -- value in `currency`

    -- commercial
    terms              nvarchar(60)   NULL,
    valid_from         date           NULL,
    valid_to           date           NULL,
    sonoid             nvarchar(30)   NULL,
    so_line_id         bigint         NULL,
    quote_ref          nvarchar(200)  NULL,

    -- provenance
    source_sheet       nvarchar(60)   NULL,          -- original workbook tab name
    source_row         int            NULL,          -- original workbook row #
    notes              nvarchar(500)  NULL,

    -- audit
    creation_date      datetime       NOT NULL CONSTRAINT DF_so_price_list_creation_date DEFAULT (SYSDATETIME()),
    created_by         nvarchar(30)   NULL,
    last_updated_date  datetime       NULL,
    updated_by         nvarchar(30)   NULL,
    delete_mark        char(1)        NOT NULL CONSTRAINT DF_so_price_list_delete_mark DEFAULT ('N'),
    deleted_by         nvarchar(30)   NULL,

    CONSTRAINT PK_so_price_list PRIMARY KEY CLUSTERED (so_price_list_id),
    CONSTRAINT CK_so_price_list_currency CHECK (currency IN ('USD','THB'))
);
GO

/* --- 3. Indexes -------------------------------------------------------- */

/* Business key. `currency` is in the key: without it the USD and THB rows for
   the same price line collide. Still filtered on customer_id/design_no so bulk
   import can complete before the manual mapping pass. */
CREATE UNIQUE NONCLUSTERED INDEX UX_so_price_list_active
    ON dbo.so_price_list (customer_id, design_no, article_variant,
                          qty_min, qty_max, qty_unit, color_tier, currency)
    WHERE delete_mark <> 'Y' AND customer_id IS NOT NULL AND design_no IS NOT NULL;
GO

/* Lookup path. INCLUDE carries currency+price so the price probe stays covering. */
CREATE NONCLUSTERED INDEX IX_so_price_list_design_customer
    ON dbo.so_price_list (design_no, customer_id)
    INCLUDE (color_tier, qty_min, qty_max, currency, price,
             valid_from, valid_to, delete_mark);
GO

PRINT 'Recreated dbo.so_price_list (tall: currency + price)';
GO
