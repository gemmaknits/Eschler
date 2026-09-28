/* ============================================================================
   Staging for the re-scan of the workbook.

   The live tables are NOT touched by this. The workbook was re-read with a
   scanner that finds each table where it stands rather than assuming one
   layout per sheet, and it returns considerably more than the first import
   did. That has to be comparable against what is live before any of it
   replaces anything.

   Every row carries the sheet, the row it came from and the header row it was
   read under, so any figure here can be traced back to a cell and checked.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO

IF OBJECT_ID('SO.so_price_list_stage','U') IS NOT NULL
    DROP TABLE SO.so_price_list_stage;
GO

CREATE TABLE SO.so_price_list_stage (
    stage_id          bigint IDENTITY(1,1) PRIMARY KEY,
    sheet             nvarchar(120)  NOT NULL,
    source_row        int            NOT NULL,
    header_row        int            NOT NULL,
    /* The article as the sheet writes it. Usually the design number and
       nothing else, but ANITA, BIGA and Biga Thailand re-quote a design with a
       second reference after it - "254990 (#040047)", "255359 (255026 width
       155cm)", "255123 ( B GRADE)". The full text is kept here and the code in
       front of the bracket becomes the design number, which is what the rest of
       the system knows the fabric by.

       block_note below was referenced by rebuild_from_rescan.sql but missing
       from this table, so the two scripts did not agree; it is here now. */
    article           nvarchar(120)  NOT NULL,
    fabric_name       nvarchar(200)  NULL,
    composition       nvarchar(300)  NULL,
    full_width_cm     nvarchar(60)   NULL,
    usable_width_cm   nvarchar(60)   NULL,
    weight_gsm        nvarchar(60)   NULL,
    moq               nvarchar(60)   NULL,
    qty_raw           nvarchar(120)  NULL,
    qty_min           int            NULL,
    qty_max           int            NULL,
    color_tier        nvarchar(30)   NULL,
    currency          char(3)        NULL,
    price             decimal(18,4)  NULL,
    date_raw          nvarchar(120)  NULL,
    remark            nvarchar(1000) NULL,
    block_note        nvarchar(max)  NULL,
    design_no         nvarchar(60)   NOT NULL
);
GO

CREATE NONCLUSTERED INDEX IX_stage_sheet   ON SO.so_price_list_stage (sheet, design_no);
CREATE NONCLUSTERED INDEX IX_stage_design  ON SO.so_price_list_stage (design_no);
GO

PRINT 'SO.so_price_list_stage created (empty)';
GO
