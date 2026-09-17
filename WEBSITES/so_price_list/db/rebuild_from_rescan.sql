/* ============================================================================
   Rebuild the price lines from the re-scan.

   WHAT IS KEPT
   Everything a person put there. A header that already exists for a sheet
   keeps its id, its name, its customer mapping, its validity dates and its
   grid shape - so the customer matching done earlier is not thrown away and
   the lists keep their identity. Only the price LINES are rebuilt.

   WHAT IS REPLACED
   Every detail line of every sheet the re-scan covers. The old lines came from
   an import that read one table per sheet and missed the rest; they are not
   worth merging with, and a merge would leave the two readings interleaved
   with no way to tell which is which. SO.so_price_list_detail_snap_20260911
   holds the previous state if any of this has to be undone.

   SET AND LINE NUMBERING
   A set is one grid row: one design and one quantity band, holding every
   colour tier and both currencies. Sets are numbered in the order the rows
   appear in the workbook, so a list reads the way its sheet does. Within a
   set, line_no numbers the TIER rows, and the USD and THB halves of one tier
   share a number - the rule the grid already works to.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO

/* Without this a statement-level failure inside the transaction does not roll
   it back: the DELETE commits, the INSERT does not, and the table is left
   empty. That happened once - the snapshot is what made it recoverable. */
SET XACT_ABORT ON;
GO

DECLARE @who varchar(15) = 'RESCAN';

BEGIN TRAN;

/* ---- 1. a header per sheet, reusing the one that is already there ---- */
INSERT INTO SO.so_price_list_header (list_name, list_desc, source_sheet, created_by)
SELECT DISTINCT
       LEFT(s.sheet, 60),
       'Imported from sheet ''' + s.sheet + ''' of Eschler_Updated Special Price list_2025.xlsx',
       s.sheet,
       @who
FROM   SO.so_price_list_stage s
WHERE  NOT EXISTS (SELECT 1 FROM SO.so_price_list_header h
                   WHERE h.source_sheet = s.sheet AND h.delete_mark <> 'Y')
  AND  NOT EXISTS (SELECT 1 FROM SO.so_price_list_header h2
                   WHERE h2.list_name = LEFT(s.sheet, 60) AND h2.delete_mark <> 'Y');

PRINT CONCAT('headers created: ', @@ROWCOUNT);

/* ---- 2. clear the lines of every sheet being rebuilt ----
   Hard, not soft: these are superseded readings of the same cells, and leaving
   4,478 of them marked deleted would clutter the table for good. The snapshot
   is the way back. */
DELETE d
FROM   SO.so_price_list_detail d
JOIN   SO.so_price_list_header h ON h.so_price_list_header_id = d.so_price_list_header_id
WHERE  h.source_sheet IN (SELECT DISTINCT sheet FROM SO.so_price_list_stage);

PRINT CONCAT('old lines removed: ', @@ROWCOUNT);

/* ---- 3. insert the re-scanned lines ---- */
;WITH base AS (
    SELECT h.so_price_list_header_id AS header_id, s.*
    FROM   SO.so_price_list_stage s
    JOIN   SO.so_price_list_header h
             ON h.source_sheet = s.sheet AND h.delete_mark <> 'Y'
),
/* Where the set first appears in the sheet. Computed on its own, because a
   window function cannot be nested inside another one's ORDER BY. */
anchored AS (
    /* header_row is part of the key on purpose. A sheet often holds the SAME
       design and quantity band quoted TWICE, in two tables - ANITA prices
       255028 once in a 2018 quote and again in 2022. Keyed without it, the two
       collapse into one grid row, the two USD prices collide in one cell, and
       the pivot splits them back apart arbitrarily: one row keeps the THB and
       the other looks half empty. Two quotes are two lines. */
    SELECT *,
           MIN(source_row) OVER (PARTITION BY header_id, header_row, design_no,
                                              qty_min, ISNULL(qty_max, -1)) AS first_row
    FROM   base
),
mapped AS (
    SELECT *,
           /* one grid row = one design and one quantity band, in sheet order */
           DENSE_RANK() OVER (
               PARTITION BY header_id
               ORDER BY first_row, header_row, design_no, qty_min, ISNULL(qty_max, -1)) AS set_no,
           /* tiers in reading order within the set; both currencies share it */
           DENSE_RANK() OVER (
               PARTITION BY header_id, header_row, design_no, qty_min, ISNULL(qty_max, -1)
               ORDER BY SO.F_SO_PRICE_LIST_tier_order(color_tier), color_tier) AS line_no
    FROM   anchored
)
INSERT INTO SO.so_price_list_detail
       (so_price_list_header_id, set_no, line_no, article, design_no, article_variant,
        fabric_name, composition, full_width_cm, usable_width_cm, weight_gsm, moq,
        qty_min, qty_max, qty_unit, color_tier, currency, price,
        active, source_row, notes, created_by)
SELECT header_id, set_no, line_no,
       /* article is what the sheet said; design_no is the identifier. These
          were the same column until the workbook turned out to re-quote a
          design with a supplier reference after it, so now article carries
          "254990 (#040047)" and design_no carries "254990".

          get_price is still called with @article by the order-entry client, so
          a bracketed article will NOT match a plain lookup - see the note at
          the foot of this file. */
       article, design_no, NULL,
       fabric_name, composition, full_width_cm, usable_width_cm, weight_gsm, moq,
       ISNULL(qty_min, 0), qty_max, N'MTS', color_tier, currency, price,
       'Y', source_row,
       /* Which quote this line came from, then whatever the row's own remark
          said. Two identical-looking rows - the 2018 and the 2022 quote for
          255028 - are only tellable apart by this, and telling them apart is
          the reviewer's whole job. */
       LEFT(LTRIM(RTRIM(
            ISNULL(block_note, N'')
          + CASE WHEN block_note IS NOT NULL AND remark IS NOT NULL
                 THEN N' -- ' ELSE N'' END
          + ISNULL(remark, N''))), 2000),
       @who
FROM   mapped;

PRINT CONCAT('lines inserted: ', @@ROWCOUNT);

COMMIT;
GO

/* ---- 4. the grid shape of each list, from what its lines actually use ---- */
UPDATE h
SET    h.tier_set     = t.tiers,
       h.currency_set = c.currencies
FROM   SO.so_price_list_header h
CROSS APPLY (
    SELECT STUFF((SELECT ',' + x.color_tier
                  FROM (SELECT DISTINCT color_tier FROM SO.so_price_list_detail
                        WHERE so_price_list_header_id = h.so_price_list_header_id
                          AND delete_mark <> 'Y') x
                  ORDER BY x.color_tier FOR XML PATH('')), 1, 1, '') AS tiers) t
CROSS APPLY (
    SELECT STUFF((SELECT ',' + LTRIM(RTRIM(y.currency))
                  FROM (SELECT DISTINCT currency FROM SO.so_price_list_detail
                        WHERE so_price_list_header_id = h.so_price_list_header_id
                          AND delete_mark <> 'Y') y
                  ORDER BY y.currency FOR XML PATH('')), 1, 1, '') AS currencies) c
WHERE  h.delete_mark <> 'Y' AND t.tiers IS NOT NULL;
GO

PRINT '--- result ---';
SELECT COUNT(*) AS lines, COUNT(DISTINCT design_no) AS designs,
       COUNT(DISTINCT so_price_list_header_id) AS lists
FROM   SO.so_price_list_detail WHERE delete_mark <> 'Y';
GO
