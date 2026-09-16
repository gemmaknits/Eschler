/* ============================================================================
   Duplicate price lines inside a single list.

   The import carries 203 groups that collide on the detail business key, 148
   of them with CONFLICTING prices. Until these are resolved the business-key
   index on so_price_list_detail cannot be made UNIQUE, which means order entry
   can pull two different prices for the same (article, qty, colour, currency).

   Nothing here changes data. Run 1 to see the scale, 2 to see the conflicts,
   then decide - the resolution is a commercial call, not a technical one.
   ============================================================================ */

/* --- 1. scale of the problem, per list -------------------------------- */
WITH k AS (
    SELECT d.so_price_list_header_id, h.list_name,
           d.article, d.article_variant, d.qty_min, d.qty_max,
           d.qty_unit, d.color_tier, d.currency,
           COUNT(*)                AS n,
           COUNT(DISTINCT d.price) AS distinct_prices
    FROM SO.so_price_list_detail d
    JOIN SO.so_price_list_header h ON h.so_price_list_header_id = d.so_price_list_header_id
    WHERE d.delete_mark <> 'Y'
    GROUP BY d.so_price_list_header_id, h.list_name, d.article, d.article_variant,
             d.qty_min, d.qty_max, d.qty_unit, d.color_tier, d.currency
    HAVING COUNT(*) > 1
)
SELECT list_name,
       COUNT(*)                                                    AS dup_groups,
       SUM(CASE WHEN distinct_prices > 1 THEN 1 ELSE 0 END)        AS price_conflicts,
       SUM(n - 1)                                                  AS extra_rows
FROM k
GROUP BY list_name
ORDER BY SUM(CASE WHEN distinct_prices > 1 THEN 1 ELSE 0 END) DESC, COUNT(*) DESC;

/* --- 2. the conflicting lines themselves, side by side ---------------- */
WITH k AS (
    SELECT so_price_list_header_id, article, article_variant, qty_min, qty_max,
           qty_unit, color_tier, currency
    FROM SO.so_price_list_detail
    WHERE delete_mark <> 'Y'
    GROUP BY so_price_list_header_id, article, article_variant, qty_min, qty_max,
             qty_unit, color_tier, currency
    HAVING COUNT(DISTINCT price) > 1
)
SELECT h.list_name, d.so_price_list_detail_id, d.source_row, d.line_no,
       d.article, d.article_variant, d.qty_min, d.qty_max, d.qty_unit,
       d.color_tier, d.currency, d.price, d.moq, d.fabric_name
FROM SO.so_price_list_detail d
JOIN k ON k.so_price_list_header_id = d.so_price_list_header_id
      AND k.article = d.article
      AND ISNULL(k.article_variant,'') = ISNULL(d.article_variant,'')
      AND k.qty_min = d.qty_min
      AND ISNULL(k.qty_max,-1) = ISNULL(d.qty_max,-1)
      AND k.qty_unit = d.qty_unit
      AND k.color_tier = d.color_tier
      AND k.currency = d.currency
JOIN SO.so_price_list_header h ON h.so_price_list_header_id = d.so_price_list_header_id
WHERE d.delete_mark <> 'Y'
ORDER BY h.list_name, d.article, d.qty_min, d.color_tier, d.currency, d.source_row;

/* --- 3. identical-price duplicates: safe to collapse automatically ----
   These 55 groups carry the same price twice, so keeping the earliest row
   loses nothing. Review the SELECT before running the UPDATE.

UPDATE d
SET    delete_mark = 'Y', deleted_by = 'SURES', last_updated_date = SYSDATETIME()
FROM   SO.so_price_list_detail d
JOIN (
    SELECT so_price_list_detail_id,
           ROW_NUMBER() OVER (
             PARTITION BY so_price_list_header_id, article, article_variant,
                          qty_min, qty_max, qty_unit, color_tier, currency, price
             ORDER BY source_row, so_price_list_detail_id) AS rn
    FROM SO.so_price_list_detail
    WHERE delete_mark <> 'Y'
) x ON x.so_price_list_detail_id = d.so_price_list_detail_id
WHERE x.rn > 1;
*/
