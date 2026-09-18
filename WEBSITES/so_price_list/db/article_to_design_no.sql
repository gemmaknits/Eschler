/* ============================================================================
   design_no becomes the identifier; article becomes its mirror.

   The number on a price line is a DESIGN number - that is what it is called on
   the worksheet and in the order-entry client - but it had been loaded into a
   column named article, leaving design_no empty on all 4,468 rows. This moves
   the values across and repoints the indexes.

   TWO COLUMNS, ON PURPOSE
   article is NOT dropped and is NOT left to go stale: every write keeps the two
   in step. The reason is P_SO_PRICE_LIST_PKG_get_price, which the VB.NET
   order-entry client calls with @article. That is an external contract this
   change must not break, and a mirrored column keeps it working untouched while
   the grid moves to design_no. Dropping article is a later, separate decision -
   it cannot be undone, and nothing here depends on it.

   COLUMN TYPE
   design_no was char(20): fixed width, so every value would come back padded
   with trailing spaces and reach the browser as "254746              ". It is
   widened to nvarchar(60) to match article exactly, which makes the swap
   mechanical - no truncation (the longest value is 11 characters) and no
   padding to trim in the API or the grid.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

SET NOCOUNT ON;
GO

PRINT '--- before ---';
SELECT COUNT(*) AS rows_live,
       SUM(CASE WHEN design_no IS NULL THEN 1 ELSE 0 END) AS design_no_empty
FROM   SO.so_price_list_detail WHERE delete_mark <> 'Y';
GO

/* ---- 1. widen design_no: char(20) pads, nvarchar(60) matches article ----

   Every index touching design_no has to go first, INCLUDED columns as well as
   key columns - IX_spld_lookup carries it as an include, which does not show up
   in a key-column listing but still blocks the ALTER with

       Msg 5074 - The index 'IX_spld_lookup' is dependent on column 'design_no'

   They are all recreated in step 3, pointed at design_no. */
IF EXISTS (SELECT 1 FROM sys.indexes
           WHERE name = 'IX_spld_design' AND object_id = OBJECT_ID('SO.so_price_list_detail'))
    DROP INDEX IX_spld_design ON SO.so_price_list_detail;
GO
IF EXISTS (SELECT 1 FROM sys.indexes
           WHERE name = 'IX_spld_lookup' AND object_id = OBJECT_ID('SO.so_price_list_detail'))
    DROP INDEX IX_spld_lookup ON SO.so_price_list_detail;
GO
IF EXISTS (SELECT 1 FROM sys.indexes
           WHERE name = 'IX_spld_business_key' AND object_id = OBJECT_ID('SO.so_price_list_detail'))
    DROP INDEX IX_spld_business_key ON SO.so_price_list_detail;
GO

IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.types t ON t.user_type_id = c.user_type_id
           WHERE c.object_id = OBJECT_ID('SO.so_price_list_detail')
             AND c.name = 'design_no' AND t.name <> 'nvarchar')
BEGIN
    ALTER TABLE SO.so_price_list_detail ALTER COLUMN design_no nvarchar(60) NULL;
    PRINT 'design_no widened to nvarchar(60)';
END
ELSE PRINT 'design_no already nvarchar';
GO

/* ---- 2. move the values across ---- */
UPDATE SO.so_price_list_detail
SET    design_no = LTRIM(RTRIM(article))
WHERE  design_no IS NULL
   OR  LTRIM(RTRIM(ISNULL(design_no,''))) <> LTRIM(RTRIM(ISNULL(article,'')));
PRINT CONCAT('rows filled: ', @@ROWCOUNT);
GO

/* ---- 3. recreate the indexes, now pointed at design_no ----
   The business key index is NOT unique, by design: two lines may share a key
   and carry different prices, and the grid shows both rather than hiding one. */
CREATE NONCLUSTERED INDEX IX_spld_business_key
    ON SO.so_price_list_detail
       (so_price_list_header_id, design_no, article_variant,
        qty_min, qty_max, qty_unit, color_tier, currency)
    WHERE delete_mark <> 'Y';
GO

CREATE NONCLUSTERED INDEX IX_spld_lookup
    ON SO.so_price_list_detail (so_price_list_header_id, design_no);
GO

/* put back the plain design_no index, dropped above to allow the ALTER -
   it answers "which lists carry this design", across headers */
CREATE NONCLUSTERED INDEX IX_spld_design
    ON SO.so_price_list_detail (design_no);
GO

/* article keeps its own lookup: get_price still filters on it. */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_spld_article' AND object_id = OBJECT_ID('SO.so_price_list_detail'))
    CREATE NONCLUSTERED INDEX IX_spld_article
        ON SO.so_price_list_detail (so_price_list_header_id, article);
GO

/* ---- 4. what came out ---- */
PRINT '--- after ---';
SELECT COUNT(*) AS rows_live,
       SUM(CASE WHEN design_no IS NULL OR LTRIM(RTRIM(design_no)) = '' THEN 1 ELSE 0 END) AS design_no_empty,
       SUM(CASE WHEN LTRIM(RTRIM(ISNULL(design_no,''))) <> LTRIM(RTRIM(ISNULL(article,''))) THEN 1 ELSE 0 END) AS out_of_step,
       COUNT(DISTINCT design_no) AS distinct_designs
FROM   SO.so_price_list_detail WHERE delete_mark <> 'Y';
GO
