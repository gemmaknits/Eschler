/* ============================================================================
   Fill SO.so_line_price_list_id.

   Reads dbo.soitm and dbo.so; writes ONLY to SO.so_line_price_list_id. The
   ERP tables are not touched. Safe to re-run - it empties the table first and
   works it out again.

   One call to the price-line procedure per order line, which needs a cursor:
   the procedure returns a result set and decides a colour tier and a quantity
   band per line, so it cannot be applied set-wise.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

IF OBJECT_ID('tempdb..#scope') IS NOT NULL DROP TABLE #scope;
IF OBJECT_ID('tempdb..#out')   IS NOT NULL DROP TABLE #out;

/* Scope, exactly as asked: the order has a price list, the line has no price
   line yet. */
SELECT i.so_line_id,
       LTRIM(RTRIM(i.sono))                         AS sono,
       s.price_list_header_id                       AS hdr,
       LTRIM(RTRIM(i.design_no))                    AS design_no,
       LTRIM(RTRIM(ISNULL(i.col, '')))              AS colour,
       LTRIM(RTRIM(i.uom))                          AS uom,
       CAST(ISNULL(i.qty, 0) AS numeric(12,2))      AS qty,
       LTRIM(RTRIM(i.curr))                         AS curr,
       CAST(s.sodt AS date)                         AS order_date
INTO   #scope
FROM   dbo.soitm i
JOIN   dbo.so s ON s.sono = i.sono
WHERE  i.price_list_detail_id IS NULL
  AND  s.price_list_header_id IS NOT NULL;

DECLARE @scope_n int = (SELECT COUNT(*) FROM #scope);
PRINT CONCAT('order lines in scope: ', @scope_n);

/* Mirrors the procedure's result, in its order - INSERT ... EXEC matches by
   position, so this has to move whenever that does. */
CREATE TABLE #out (
    resolution              varchar(40),
    order_price             decimal(18,4),
    order_curr              varchar(3),
    order_uom               varchar(20),
    price                   decimal(18,4),
    price_curr              char(3),
    price_uom               nvarchar(20),
    exchange_rate_used      decimal(18,6),
    uom_factor              decimal(18,6),
    matched_design_no       nvarchar(60),
    color_tier              nvarchar(60),
    qty_min                 int,
    qty_max                 int,
    so_price_list_detail_id bigint,
    so_price_list_header_id bigint,
    set_no                  int,
    line_no                 int,
    notes                   nvarchar(2000));

TRUNCATE TABLE SO.so_line_price_list_id;

DECLARE @id bigint, @sono varchar(30), @h bigint, @d varchar(30), @c varchar(30),
        @u char(10), @q numeric(12,2), @cur char(10), @dt date, @n int;

DECLARE cur CURSOR FAST_FORWARD FOR
    SELECT so_line_id, sono, hdr, design_no, colour, uom, qty, curr, order_date FROM #scope;
OPEN cur;
FETCH NEXT FROM cur INTO @id, @sono, @h, @d, @c, @u, @q, @cur, @dt;

WHILE @@FETCH_STATUS = 0
BEGIN
    DELETE FROM #out;
    BEGIN TRY
        INSERT INTO #out
        EXEC SO.P_SO_PRICE_LIST_PKG_select_price_line @h, @d, @c, @u, @q, @cur, @dt;
    END TRY
    BEGIN CATCH
        /* a line the procedure refuses outright - recorded below as 'no rows',
           never hidden */
    END CATCH

    SET @n = (SELECT COUNT(*) FROM #out);

    INSERT INTO SO.so_line_price_list_id
        (so_line_id, price_list_detail_id, sono, so_price_list_header_id, design_no,
         color_code, uom, qty, curr, order_date, resolution, candidates,
         matched_design_no, color_tier, qty_min, qty_max, price, price_curr,
         order_price, order_curr, order_uom, exchange_rate_used, uom_factor, price_uom, why_not)
    SELECT @id,
           /* Only a single, unambiguous match is carried forward. Several
              candidates means a person has to choose. */
           CASE WHEN @n = 1 AND (SELECT MAX(resolution) FROM #out) = 'matched'
                THEN (SELECT MAX(so_price_list_detail_id) FROM #out) END,
           @sono, @h, @d, NULLIF(@c, ''), @u, @q, @cur, @dt,
           ISNULL((SELECT MAX(resolution) FROM #out), 'no rows'),
           @n,
           (SELECT MAX(matched_design_no) FROM #out),
           CASE WHEN @n = 1 THEN (SELECT MAX(color_tier)  FROM #out) END,
           CASE WHEN @n = 1 THEN (SELECT MAX(qty_min)     FROM #out) END,
           CASE WHEN @n = 1 THEN (SELECT MAX(qty_max)     FROM #out) END,
           CASE WHEN @n = 1 THEN (SELECT MAX(price)       FROM #out) END,
           CASE WHEN @n = 1 THEN (SELECT MAX(price_curr)  FROM #out) END,
           CASE WHEN @n = 1 THEN (SELECT MAX(order_price) FROM #out) END,
           CASE WHEN @n = 1 THEN (SELECT MAX(order_curr)  FROM #out) END,
           CASE WHEN @n = 1 THEN (SELECT MAX(order_uom)   FROM #out) END,
           CASE WHEN @n = 1 THEN (SELECT MAX(exchange_rate_used) FROM #out) END,
           CASE WHEN @n = 1 THEN (SELECT MAX(uom_factor)  FROM #out) END,
           CASE WHEN @n = 1 THEN (SELECT MAX(price_uom)   FROM #out) END,
           /* Why nothing came back, in the order the procedure would have
              failed. The first of these that is true is the reason. */
           CASE WHEN @n > 0 THEN NULL
                WHEN NOT EXISTS (SELECT 1 FROM SO.so_price_list_header h
                                 WHERE h.so_price_list_header_id = @h AND h.delete_mark <> 'Y')
                     THEN 'the order points at a price list that does not exist'
                WHEN NOT EXISTS (SELECT 1 FROM SO.so_price_list_detail d
                                 WHERE d.so_price_list_header_id = @h AND d.delete_mark <> 'Y' AND d.active = 'Y'
                                   AND LTRIM(RTRIM(d.design_no)) COLLATE DATABASE_DEFAULT = @d COLLATE DATABASE_DEFAULT)
                     THEN 'design is not on the order''s price list'
                WHEN NOT EXISTS (SELECT 1 FROM SO.so_price_list_detail d
                                 WHERE d.so_price_list_header_id = @h AND d.delete_mark <> 'Y' AND d.active = 'Y'
                                   AND LTRIM(RTRIM(d.design_no)) COLLATE DATABASE_DEFAULT = @d COLLATE DATABASE_DEFAULT
                                   AND LTRIM(RTRIM(d.qty_unit)) COLLATE DATABASE_DEFAULT = @u COLLATE DATABASE_DEFAULT)
                     THEN 'design is on the list but not in this unit'
                ELSE 'no tier or band on that design fits this line'
           END;

    FETCH NEXT FROM cur INTO @id, @sono, @h, @d, @c, @u, @q, @cur, @dt;
END
CLOSE cur; DEALLOCATE cur;

PRINT '--- what it found ---';
SELECT resolution,
       COUNT(*)                                                       AS lines,
       SUM(CASE WHEN price_list_detail_id IS NOT NULL THEN 1 ELSE 0 END) AS ready_to_apply
FROM   SO.so_line_price_list_id
GROUP  BY resolution
ORDER  BY lines DESC;

SELECT 'READY TO APPLY' AS summary, COUNT(*) AS lines
FROM   SO.so_line_price_list_id WHERE price_list_detail_id IS NOT NULL;

SELECT why_not, COUNT(*) AS lines
FROM   SO.so_line_price_list_id WHERE why_not IS NOT NULL
GROUP  BY why_not ORDER BY lines DESC;
GO

/* ---------------------------------------------------------------------------
   To apply the ones you agree with, when you are ready. NOT run here.

       UPDATE i
       SET    i.price_list_detail_id = m.price_list_detail_id
       FROM   dbo.soitm i
       JOIN   SO.so_line_price_list_id m ON m.so_line_id = i.so_line_id
       WHERE  m.price_list_detail_id IS NOT NULL
         AND  i.price_list_detail_id IS NULL;
   --------------------------------------------------------------------------- */
