/* ============================================================================
   SO.so_line_price_list_id - which price line each order line should point at.

   A REVIEW TABLE, not a backfill. The order lines live in dbo.soitm, which
   belongs to the ERP; nothing here writes to it. This works out what
   soitm.price_list_detail_id would be for every line still missing one, and
   puts the answer somewhere a person can check it and apply what they agree
   with.

   WHY NOT JUST WRITE IT. Of the 342 order lines in scope, the procedure
   resolves 15 to a single price line. The rest either find nothing (247) or
   find several candidates (80), and a guess written into a sales order is
   worse than a blank.

   The reason is not the matching: 273 of the 342 reference a design that is
   not on the price list their own order points at. 24 point at
   price_list_header_id 686, which does not exist, and 28 at a list called
   "Test" holding 8 lines. That is an assignment problem in dbo.so, and it has
   to be fixed before any of this improves.

   SCOPE, as asked: only where dbo.so.price_list_header_id IS NOT NULL, and
   only lines whose soitm.price_list_detail_id is still NULL.

   THE RATE DATE is the order's own sodt, not today, so a 2024 order is priced
   at the 2024 rate. The conversion only matters where a list is kept in one
   currency and sold in another.

   price_list_detail_id is the answer and is NULL wherever the procedure could
   not settle on one. Everything after it is there to make that judgement
   reviewable - what was asked, what came back, and why nothing did. Drop those
   columns if they are in the way; the first two are the ones that matter.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO

IF OBJECT_ID('SO.so_line_price_list_id','U') IS NOT NULL
    DROP TABLE SO.so_line_price_list_id;
GO

CREATE TABLE SO.so_line_price_list_id (
    /* the two that matter */
    so_line_id              bigint       NOT NULL PRIMARY KEY,
    price_list_detail_id    bigint       NULL,

    /* what was asked, so the answer can be judged */
    sono                    varchar(30)  NULL,
    so_price_list_header_id bigint       NULL,
    design_no               varchar(60)  NULL,
    color_code              varchar(60)  NULL,
    uom                     varchar(20)  NULL,
    qty                     numeric(12,2) NULL,
    curr                    varchar(10)  NULL,
    order_date              date         NULL,

    /* what came back */
    resolution              varchar(40)  NULL,
    candidates              int          NULL,
    matched_design_no       varchar(60)  NULL,
    color_tier              varchar(60)  NULL,
    qty_min                 int          NULL,
    qty_max                 int          NULL,
    price                   decimal(18,4) NULL,
    price_curr              varchar(10)  NULL,
    order_price             decimal(18,4) NULL,
    order_curr              varchar(10)  NULL,
    exchange_rate_used      decimal(18,6) NULL,

    /* and why nothing did, where that is the case */
    why_not                 varchar(80)  NULL,
    generated_on            datetime     NOT NULL DEFAULT SYSDATETIME()
);
GO

CREATE NONCLUSTERED INDEX IX_slpli_resolution ON SO.so_line_price_list_id (resolution);
CREATE NONCLUSTERED INDEX IX_slpli_header     ON SO.so_line_price_list_id (so_price_list_header_id);
GO

PRINT 'SO.so_line_price_list_id created (empty)';
GO
