/* ============================================================================
   qty_unit becomes a real unit of measure.

   The price lines carry "M", which is not a unit this system knows - dbo.uom
   holds 23 of them and M is not among them. The metre is MTS (uom_id 14, the
   length class, alongside YDS). So every line moves to MTS and the column is
   widened to hold it: it was char(2), which could not store a three-letter
   code at all.

   From here the unit is CHECKED against dbo.uom on the way in, so a typo
   cannot be saved. dbo.uom belongs to the ERP; this reads it and never writes.

   COLLATION
   dbo.uom is Thai_CI_AI like the other ERP tables, so comparisons against it
   are COLLATE DATABASE_DEFAULT or they fail outright with Msg 468.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO
SET XACT_ABORT ON;
GO

PRINT '--- before ---';
SELECT qty_unit, COUNT(*) AS rows FROM SO.so_price_list_detail GROUP BY qty_unit;
GO

/* char(2) cannot hold "MTS". Widened, and left nvarchar so it matches the
   other text columns rather than padding every value to a fixed width.

   A default constraint and the business-key index both depend on the column,
   so both have to go first and come back after - the ALTER is refused
   otherwise (Msg 5074). */
IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_spld_qty_unit')
    ALTER TABLE SO.so_price_list_detail DROP CONSTRAINT DF_spld_qty_unit;
GO
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_spld_business_key'
           AND object_id = OBJECT_ID('SO.so_price_list_detail'))
    DROP INDEX IX_spld_business_key ON SO.so_price_list_detail;
GO
IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.types t ON t.user_type_id = c.user_type_id
           WHERE c.object_id = OBJECT_ID('SO.so_price_list_detail')
             AND c.name = 'qty_unit' AND (t.name <> 'nvarchar' OR c.max_length < 20))
BEGIN
    ALTER TABLE SO.so_price_list_detail ALTER COLUMN qty_unit nvarchar(10) NULL;
    PRINT 'qty_unit widened to nvarchar(10)';
END
ELSE PRINT 'qty_unit already wide enough';
GO

UPDATE SO.so_price_list_detail
SET    qty_unit = N'MTS'
WHERE  LTRIM(RTRIM(ISNULL(qty_unit, ''))) IN ('M', 'MT', 'MTR', 'METER', 'METRE', '')
   OR  qty_unit IS NULL;
PRINT CONCAT('lines moved to MTS: ', @@ROWCOUNT);
GO

/* ---- put the default and the index back, now naming MTS ---- */
ALTER TABLE SO.so_price_list_detail
    ADD CONSTRAINT DF_spld_qty_unit DEFAULT (N'MTS') FOR qty_unit;
GO
CREATE NONCLUSTERED INDEX IX_spld_business_key
    ON SO.so_price_list_detail
       (so_price_list_header_id, design_no, article_variant,
        qty_min, qty_max, qty_unit, color_tier, currency)
    WHERE delete_mark <> 'Y';
GO

/* ---- what is left, and does all of it exist in dbo.uom? ---- */
PRINT '--- after ---';
SELECT qty_unit, COUNT(*) AS rows FROM SO.so_price_list_detail GROUP BY qty_unit;

SELECT COUNT(*) AS lines_with_a_unit_not_in_dbo_uom
FROM   SO.so_price_list_detail d
WHERE  d.delete_mark <> 'Y'
  AND  NOT EXISTS (SELECT 1 FROM dbo.uom u
                   WHERE LTRIM(RTRIM(u.uom)) COLLATE DATABASE_DEFAULT
                       = LTRIM(RTRIM(d.qty_unit)) COLLATE DATABASE_DEFAULT);
GO
