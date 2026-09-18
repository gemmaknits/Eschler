/* ============================================================================
   set_no - the grid row identity.

   A price list holds one detail row per currency and colour tier, but the grid
   shows one row per worksheet line: USD and THB and every tier side by side.
   set_no names that group, so ordering and insertion work on what the user
   actually sees instead of being inferred from source_row each time.

     set_no   - the grid row's position within the price list: 1, 2, 3 ...
     line_no  - which tier row within the set. The USD and THB halves of one
                worksheet line SHARE it - see normalize_line_no.sql, which
                replaced this file's per-currency numbering.

   Reading order is therefore ORDER BY set_no, line_no.

   INSERTING BETWEEN
   A new row between set 1 and set 2 renumbers the list - cheap at this size
   (largest list is 1,988 rows) and it keeps set_no a dense, readable 1..n with
   no decimals to run out of.

   BACKFILL
   (header, source_row) is already a clean set: no set spans two articles or
   two quantity bands. The exception is 8 worksheet lines that carry two rows
   of the SAME tier and currency - those cannot be one set, so they are split,
   matching what the grid already does when it buckets them into separate rows.
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

IF COL_LENGTH('SO.so_price_list_detail','set_no') IS NULL
BEGIN
    ALTER TABLE SO.so_price_list_detail ADD set_no int NULL;
    PRINT 'added SO.so_price_list_detail.set_no';
END
ELSE PRINT 'set_no already exists';
GO

/* ---- backfill set_no and line_no together ---- */
WITH occ AS (
    /* how many times this tier+currency has already appeared on the same
       worksheet line - the 2nd occurrence starts a new set */
    SELECT so_price_list_detail_id,
           so_price_list_header_id,
           source_row,
           currency,
           color_tier,
           ROW_NUMBER() OVER (
               PARTITION BY so_price_list_header_id, source_row, color_tier, currency
               ORDER BY so_price_list_detail_id) - 1 AS dup_ix
    FROM   SO.so_price_list_detail
    WHERE  delete_mark <> 'Y'
),
numbered AS (
    SELECT o.so_price_list_detail_id,
           DENSE_RANK() OVER (
               PARTITION BY o.so_price_list_header_id
               ORDER BY ISNULL(o.source_row, 2147483647), o.dup_ix) AS new_set_no,
           ROW_NUMBER() OVER (
               PARTITION BY o.so_price_list_header_id, o.source_row, o.dup_ix
               ORDER BY o.currency, o.color_tier)                    AS new_line_no
    FROM   occ o
)
UPDATE d
SET    d.set_no  = n.new_set_no,
       d.line_no = n.new_line_no
FROM   SO.so_price_list_detail d
JOIN   numbered n ON n.so_price_list_detail_id = d.so_price_list_detail_id;
GO

/* Anything soft-deleted keeps a set of its own rather than a NULL. */
UPDATE SO.so_price_list_detail SET set_no = 0 WHERE set_no IS NULL;
GO

ALTER TABLE SO.so_price_list_detail ALTER COLUMN set_no int NOT NULL;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_spld_set' AND object_id = OBJECT_ID('SO.so_price_list_detail'))
    CREATE NONCLUSTERED INDEX IX_spld_set
        ON SO.so_price_list_detail (so_price_list_header_id, set_no, line_no)
        WHERE delete_mark <> 'Y';
GO

/* ---- what the backfill produced ---- */
SELECT COUNT(*)                                   AS detail_rows,
       COUNT(DISTINCT CAST(so_price_list_header_id AS varchar(12))
                    + ':' + CAST(set_no AS varchar(12))) AS sets,
       MIN(set_no) AS lo_set, MAX(set_no) AS hi_set
FROM   SO.so_price_list_detail
WHERE  delete_mark <> 'Y';
GO

/* a set must never hold the same tier+currency twice */
SELECT COUNT(*) AS sets_with_a_clash FROM (
    SELECT so_price_list_header_id, set_no, color_tier, currency
    FROM   SO.so_price_list_detail WHERE delete_mark <> 'Y'
    GROUP BY so_price_list_header_id, set_no, color_tier, currency
    HAVING COUNT(*) > 1) z;
GO

PRINT 'set_no backfilled';
GO
