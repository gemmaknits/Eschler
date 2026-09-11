/* ============================================================================
   Populate so_price_list_header + so_price_list_detail from the flat
   SO.so_price_list (4,468 tall rows, 53 lists).

   Run create_price_list_header_detail.sql first.
   Idempotent: clears both tables before loading.
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @who nvarchar(30) = N'SURES';

BEGIN TRAN;

DELETE FROM SO.so_price_list_detail;
DELETE FROM SO.so_price_list_header;
DBCC CHECKIDENT ('SO.so_price_list_detail', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('SO.so_price_list_header', RESEED, 0) WITH NO_INFOMSGS;

/* --- 1. headers: one per customer_excel (= source_sheet, verified 1:1) ---
   terms / valid_from / valid_to / quote_ref are constant within each list, so
   MAX() collapses them without losing information. */
INSERT INTO SO.so_price_list_header
    (list_name, list_desc, customer_id, customer_name, customer_excel,
     list_date, valid_from, valid_to, terms, quote_ref,
     source_sheet, notes, created_by)
SELECT
    p.customer_excel                                        AS list_name,
    N'Imported from sheet ''' + MAX(p.source_sheet) + N''' of '
      + N'Eschler_Updated Special Price list_2025.xlsx'     AS list_desc,
    NULL                                                    AS customer_id,      -- filled by the mapping pass
    NULL                                                    AS customer_name,
    p.customer_excel,
    NULL                                                    AS list_date,
    MAX(p.valid_from)                                       AS valid_from,
    MAX(p.valid_to)                                         AS valid_to,
    MAX(p.terms)                                            AS terms,
    MAX(p.quote_ref)                                        AS quote_ref,
    MAX(p.source_sheet)                                     AS source_sheet,
    NULL                                                    AS notes,
    @who
FROM SO.so_price_list p
WHERE p.delete_mark <> 'Y'
GROUP BY p.customer_excel;

PRINT 'headers inserted: ' + CAST(@@ROWCOUNT AS varchar(10));

/* --- 2. details ------------------------------------------------------- */
INSERT INTO SO.so_price_list_detail
    (so_price_list_header_id, line_no,
     article, design_no, article_variant, fabric_name, composition,
     full_width_cm, usable_width_cm, weight_gsm, moq,
     qty_min, qty_max, qty_unit, color_tier, currency, price,
     source_row, notes, created_by)
SELECT
    h.so_price_list_header_id,
    ROW_NUMBER() OVER (PARTITION BY h.so_price_list_header_id
                       ORDER BY p.source_row, p.article, p.qty_min, p.color_tier, p.currency),
    p.article, p.design_no, p.article_variant, p.fabric_name, p.composition,
    p.full_width_cm, p.usable_width_cm, p.weight_gsm, p.moq,
    p.qty_min, p.qty_max, p.qty_unit, p.color_tier, p.currency, p.price,
    p.source_row, p.notes, @who
FROM SO.so_price_list p
JOIN SO.so_price_list_header h
  ON h.customer_excel = p.customer_excel
WHERE p.delete_mark <> 'Y';

PRINT 'details inserted: ' + CAST(@@ROWCOUNT AS varchar(10));

COMMIT;

/* --- 3. reconcile against the flat table ------------------------------ */
SELECT
    (SELECT COUNT(*) FROM SO.so_price_list WHERE delete_mark <> 'Y')     AS flat_rows,
    (SELECT COUNT(*) FROM SO.so_price_list_detail)                        AS detail_rows,
    (SELECT COUNT(*) FROM SO.so_price_list_header)                        AS header_rows,
    (SELECT CAST(SUM(price) AS decimal(18,2)) FROM SO.so_price_list
       WHERE delete_mark <> 'Y' AND currency='USD')                        AS flat_usd_sum,
    (SELECT CAST(SUM(price) AS decimal(18,2)) FROM SO.so_price_list_detail
       WHERE currency='USD')                                               AS detail_usd_sum,
    (SELECT CAST(SUM(price) AS decimal(18,2)) FROM SO.so_price_list
       WHERE delete_mark <> 'Y' AND currency='THB')                        AS flat_thb_sum,
    (SELECT CAST(SUM(price) AS decimal(18,2)) FROM SO.so_price_list_detail
       WHERE currency='THB')                                               AS detail_thb_sum;

/* rows per list, largest first */
SELECT h.so_price_list_header_id AS hdr, h.list_name,
       SUM(CASE WHEN d.currency='USD' THEN 1 ELSE 0 END) AS usd,
       SUM(CASE WHEN d.currency='THB' THEN 1 ELSE 0 END) AS thb,
       COUNT(*) AS lines
FROM SO.so_price_list_header h
JOIN SO.so_price_list_detail d ON d.so_price_list_header_id = h.so_price_list_header_id
GROUP BY h.so_price_list_header_id, h.list_name
ORDER BY COUNT(*) DESC;
