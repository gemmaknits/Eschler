/* ============================================================================
   active flag on so_price_list_detail

   Two of the 117 source sheets - Ausco and GLAMORISE - carry an ACTIVE (Y/N)
   column that the original extract did not pick up. A loose sweep of every
   sheet (header text containing "ACTIV", plus any column whose values are
   overwhelmingly Y/N) found no others, labelled or otherwise.

   Default 'Y': the other 51 lists never stated it, and a price line that has
   not been retired is in force. Only the sheets that actually say N get N, so
   "no opinion" and "active" collapse to the same thing - which is how the data
   reads today.

   Backfilled by source_sheet + source_row, an exact join: both sheets have a
   source_row on every line and one detail line per worksheet row.
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

IF COL_LENGTH('SO.so_price_list_detail','active') IS NULL
BEGIN
    ALTER TABLE SO.so_price_list_detail
        ADD active char(1) NOT NULL
            CONSTRAINT DF_so_price_list_detail_active DEFAULT ('Y');
    PRINT 'added so_price_list_detail.active';
END
ELSE PRINT 'so_price_list_detail.active already exists';
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints
               WHERE name = 'CK_so_price_list_detail_active')
    ALTER TABLE SO.so_price_list_detail
        ADD CONSTRAINT CK_so_price_list_detail_active CHECK (active IN ('Y','N'));
GO

/* Staging table for the workbook scan, so the backfill is a plain join that can
   be inspected before and after rather than a pile of literals. */
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

PRINT 'active flag column + staging table ready';
GO
