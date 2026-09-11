/* ============================================================================
   active flag on the raw flat table, SO.so_price_list

   so_price_list is the pre-split extract (one row per currency, 4,468 rows).
   It keeps source_sheet and source_row on the row itself, so the backfill is a
   direct join with no header lookup.

   Same rule as the detail table: default 'Y', and only the two sheets that
   actually carry an ACTIVE column (Ausco, GLAMORISE) get an 'N'.
   ============================================================================ */

SET NOCOUNT ON;
GO

IF COL_LENGTH('SO.so_price_list','active') IS NULL
BEGIN
    ALTER TABLE SO.so_price_list
        ADD active char(1) NOT NULL
            CONSTRAINT DF_so_price_list_active DEFAULT ('Y');
    PRINT 'added so_price_list.active';
END
ELSE PRINT 'so_price_list.active already exists';
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints
               WHERE name = 'CK_so_price_list_active')
    ALTER TABLE SO.so_price_list
        ADD CONSTRAINT CK_so_price_list_active CHECK (active IN ('Y','N'));
GO

IF OBJECT_ID('SO.so_price_list_active_stage','U') IS NOT NULL
    DROP TABLE SO.so_price_list_active_stage;
GO
CREATE TABLE SO.so_price_list_active_stage
(
    source_sheet nvarchar(120) NOT NULL,
    source_row   int           NOT NULL,
    active       char(1)       NOT NULL
);
GO

PRINT 'raw-table active column + staging ready';
GO
