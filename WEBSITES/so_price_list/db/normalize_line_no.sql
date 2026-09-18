/* ============================================================================
   line_no  -  one number per tier row, shared by its USD and THB halves.

   The original backfill numbered every detail row separately, ordered by
   currency then tier, so one worksheet line came out as two numbers and the
   pair was not even adjacent:

       line 1  THB  All_colors      line 3  USD  All_colors
       line 2  THB  PFE/PFD         line 4  USD  PFE/PFD

   A price line is ONE line with two currencies on it, so the pair shares a
   line_no and (set_no, line_no) names a single tier row:

       line 1  PFE/PFD     USD + THB
       line 2  All_colors  USD + THB

   Ordering for this one-time pass is the tier reading order, since the numbers
   being replaced were alphabetical-by-currency and carried no user intent. From
   here on line_no is the user's: an inserted tier keeps the position it was
   given and is never re-sorted.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

SET NOCOUNT ON;
GO

PRINT '--- before ---';
SELECT COUNT(*) AS tier_rows_whose_pair_disagrees
FROM (
    SELECT so_price_list_header_id, set_no, color_tier
    FROM   SO.so_price_list_detail
    WHERE  delete_mark <> 'Y'
    GROUP BY so_price_list_header_id, set_no, color_tier
    HAVING COUNT(DISTINCT line_no) > 1) z;
GO

BEGIN TRAN;

;WITH renum AS (
    SELECT so_price_list_detail_id,
           DENSE_RANK() OVER (
               PARTITION BY so_price_list_header_id, set_no
               ORDER BY SO.F_SO_PRICE_LIST_tier_order(color_tier),
                        color_tier) AS new_line_no
    FROM   SO.so_price_list_detail
    WHERE  delete_mark <> 'Y'
)
UPDATE d
SET    d.line_no = r.new_line_no
FROM   SO.so_price_list_detail d
JOIN   renum r ON r.so_price_list_detail_id = d.so_price_list_detail_id
WHERE  d.line_no <> r.new_line_no;

PRINT CONCAT('rows renumbered: ', @@ROWCOUNT);

COMMIT;
GO

PRINT '--- after ---';
SELECT COUNT(*) AS tier_rows_whose_pair_disagrees
FROM (
    SELECT so_price_list_header_id, set_no, color_tier
    FROM   SO.so_price_list_detail
    WHERE  delete_mark <> 'Y'
    GROUP BY so_price_list_header_id, set_no, color_tier
    HAVING COUNT(DISTINCT line_no) > 1) z;

/* a (set, line) must now name exactly one tier, with at most one row per currency */
SELECT COUNT(*) AS lines_holding_two_tiers
FROM (
    SELECT so_price_list_header_id, set_no, line_no
    FROM   SO.so_price_list_detail
    WHERE  delete_mark <> 'Y'
    GROUP BY so_price_list_header_id, set_no, line_no
    HAVING COUNT(DISTINCT color_tier) > 1) z;
GO
