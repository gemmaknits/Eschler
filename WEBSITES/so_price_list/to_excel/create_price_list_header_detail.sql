/* ============================================================================
   Split SO.so_price_list into header + detail.

   WHY: order entry selects a price LIST, rather than the app resolving a
   customer name to a customer_id. That makes customer identity a property of
   the list (a default/hint), not a join key the lookup depends on - so the
   unresolved customer_excel -> customer_id mapping stops being a blocker.

   GRAIN
     header : one per price list. customer_excel <-> source_sheet is exactly
              1:1 in the imported workbook (53/53/53), so the import produces
              53 headers.
     detail : one per (article x variant x qty band x colour tier x CURRENCY).

   WHAT MOVED UP, AND WHY IT WAS SAFE
     terms, valid_from, valid_to, quote_ref are constant within every one of
     the 53 lists - verified, 0 lists carry more than one distinct value.
     moq (15 lists), fabric_name / composition / widths / weight (~30 lists)
     DO vary within a list, so they stay on the detail line.
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID('SO.so_price_list_detail', 'U') IS NOT NULL DROP TABLE SO.so_price_list_detail;
IF OBJECT_ID('SO.so_price_list_header', 'U') IS NOT NULL DROP TABLE SO.so_price_list_header;
GO

/* --- header ------------------------------------------------------------ */
CREATE TABLE SO.so_price_list_header
(
    so_price_list_header_id  bigint         IDENTITY(1,1) NOT NULL,

    -- identity the user picks from during order entry
    list_name                nvarchar(60)   NOT NULL,
    list_desc                nvarchar(400)  NULL,

    -- who the list is for. A hint/default now, not a lookup dependency:
    -- customer_id may stay NULL for master lists (CENTER) and for the sheets
    -- whose workbook name has no confident match in dbo.customers.
    customer_id              bigint         NULL,
    customer_name            nvarchar(100)  NULL,   -- snapshot of customers.name at mapping time
    customer_excel           nvarchar(120)  NULL,   -- raw sheet name / customer text from the workbook

    -- commercial terms, constant across the list
    list_date                date           NULL,   -- date the list was issued/agreed
    valid_from               date           NULL,
    valid_to                 date           NULL,
    terms                    nvarchar(60)   NULL,
    quote_ref                nvarchar(200)  NULL,
    sonoid                   nvarchar(30)   NULL,   -- originating quote / SO, when list is order-specific
    so_line_id               bigint         NULL,

    -- provenance
    source_sheet             nvarchar(60)   NULL,   -- original workbook tab name
    notes                    nvarchar(500)  NULL,

    -- audit
    creation_date            datetime       NOT NULL CONSTRAINT DF_splh_creation_date DEFAULT (SYSDATETIME()),
    created_by               nvarchar(30)   NULL,
    last_updated_date        datetime       NULL,
    updated_by               nvarchar(30)   NULL,
    delete_mark              char(1)        NOT NULL CONSTRAINT DF_splh_delete_mark DEFAULT ('N'),
    deleted_by               nvarchar(30)   NULL,

    CONSTRAINT PK_so_price_list_header PRIMARY KEY CLUSTERED (so_price_list_header_id)
);
GO

/* list_name is what the order-entry picker shows, so it must be unambiguous
   among live lists. Filtered, so a soft-deleted list frees its name. */
CREATE UNIQUE NONCLUSTERED INDEX UX_splh_list_name
    ON SO.so_price_list_header (list_name)
    WHERE delete_mark <> 'Y';
GO

/* Finding the candidate lists for a customer at order entry. */
CREATE NONCLUSTERED INDEX IX_splh_customer
    ON SO.so_price_list_header (customer_id)
    INCLUDE (list_name, valid_from, valid_to, delete_mark);
GO

/* --- detail ------------------------------------------------------------ */
CREATE TABLE SO.so_price_list_detail
(
    so_price_list_detail_id  bigint         IDENTITY(1,1) NOT NULL,
    so_price_list_header_id  bigint         NOT NULL,
    line_no                  int            NULL,   -- display order within the list

    -- what
    article                  nvarchar(30)   NULL,   -- raw article as uploaded; ALWAYS text, never numeric
    design_no                char(20)       NULL,   -- validated against dm.Design_no post-mapping
    article_variant          nvarchar(20)   NULL,
    fabric_name              nvarchar(120)  NULL,
    composition              nvarchar(200)  NULL,
    full_width_cm            nvarchar(30)   NULL,
    usable_width_cm          nvarchar(30)   NULL,
    weight_gsm               nvarchar(30)   NULL,
    moq                      nvarchar(30)   NULL,

    -- price grain
    qty_min                  int            NOT NULL,
    qty_max                  int            NULL,   -- NULL = open upper bound
    qty_unit                 char(2)        NOT NULL CONSTRAINT DF_spld_qty_unit DEFAULT ('M'),  -- 'M' | 'KG'
    color_tier               nvarchar(30)   NOT NULL,
    currency                 char(3)        NOT NULL,   -- 'USD' | 'THB'
    price                    decimal(18,4)  NULL,       -- value in `currency`

    -- provenance
    source_row               int            NULL,   -- original workbook row #
    notes                    nvarchar(500)  NULL,

    -- audit
    creation_date            datetime       NOT NULL CONSTRAINT DF_spld_creation_date DEFAULT (SYSDATETIME()),
    created_by               nvarchar(30)   NULL,
    last_updated_date        datetime       NULL,
    updated_by               nvarchar(30)   NULL,
    delete_mark              char(1)        NOT NULL CONSTRAINT DF_spld_delete_mark DEFAULT ('N'),
    deleted_by               nvarchar(30)   NULL,

    CONSTRAINT PK_so_price_list_detail PRIMARY KEY CLUSTERED (so_price_list_detail_id),
    CONSTRAINT FK_spld_header FOREIGN KEY (so_price_list_header_id)
        REFERENCES SO.so_price_list_header (so_price_list_header_id),
    CONSTRAINT CK_spld_currency CHECK (currency IN ('USD','THB'))
);
GO

/* Business key of a price line.
   DELIBERATELY NOT UNIQUE YET: the imported workbook contains 203 groups that
   collide on this key, 148 of them with CONFLICTING prices (e.g. CENTER article
   255484, 200-600 M, PFE/PFD, USD priced at both 2.1117 and 2.16). Something
   distinguishing those lines in the sheet is not being captured by the
   extractor. Resolve them, then swap this for the UNIQUE version below.
   Detection query: see resolve_duplicate_price_lines.sql */
CREATE NONCLUSTERED INDEX IX_spld_business_key
    ON SO.so_price_list_detail (so_price_list_header_id, article, article_variant,
                                 qty_min, qty_max, qty_unit, color_tier, currency)
    WHERE delete_mark <> 'Y';
GO

/*  -- once the duplicates are resolved:
DROP INDEX IX_spld_business_key ON SO.so_price_list_detail;
CREATE UNIQUE NONCLUSTERED INDEX UX_spld_business_key
    ON SO.so_price_list_detail (so_price_list_header_id, article, article_variant,
                                 qty_min, qty_max, qty_unit, color_tier, currency)
    WHERE delete_mark <> 'Y';
*/

/* The order-entry price probe: header chosen by the user, then article + tier
   + qty band. Covering, so the probe never touches the base table. */
CREATE NONCLUSTERED INDEX IX_spld_lookup
    ON SO.so_price_list_detail (so_price_list_header_id, article)
    INCLUDE (design_no, color_tier, qty_min, qty_max, qty_unit,
             currency, price, moq, delete_mark);
GO

/* design_no path, for lookups that start from the design rather than the list. */
CREATE NONCLUSTERED INDEX IX_spld_design
    ON SO.so_price_list_detail (design_no)
    INCLUDE (so_price_list_header_id, color_tier, qty_min, qty_max,
             currency, price, delete_mark);
GO

PRINT 'Created SO.so_price_list_header + SO.so_price_list_detail';
GO
