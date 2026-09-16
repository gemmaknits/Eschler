/* ============================================================================
   Move the price list tables from dbo into schema SO, alongside the package.

   ALTER SCHEMA ... TRANSFER carries the table's indexes, defaults, check
   constraints and foreign keys with it, so FK_spld_header (detail -> header)
   survives untouched - both ends move.

   Nothing outside the SO_PRICE_LIST_PKG procedures references these tables:
   no views, no foreign keys reaching in from dbo. The procedures are updated
   in the same deployment.

   Idempotent: a table already in SO is skipped.
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

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'SO')
    EXEC('CREATE SCHEMA SO');
GO

/* The three live tables only. so_price_list_bak_20260907 is a point-in-time
   backup from the currency migration and stays in dbo - it is not part of the
   application and nothing references it. */
DECLARE @tables TABLE (name sysname);
INSERT INTO @tables (name) VALUES
    ('so_price_list'),
    ('so_price_list_header'),
    ('so_price_list_detail');

DECLARE @t sysname, @sql nvarchar(400);
DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT name FROM @tables;
OPEN cur;
FETCH NEXT FROM cur INTO @t;
WHILE @@FETCH_STATUS = 0
BEGIN
    IF EXISTS (SELECT 1 FROM sys.tables
               WHERE name = @t AND SCHEMA_NAME(schema_id) = 'dbo')
    BEGIN
        SET @sql = N'ALTER SCHEMA SO TRANSFER dbo.' + QUOTENAME(@t) + N';';
        EXEC sp_executesql @sql;
        PRINT '  moved dbo.' + @t + ' -> SO.' + @t;
    END
    ELSE IF EXISTS (SELECT 1 FROM sys.tables
                    WHERE name = @t AND SCHEMA_NAME(schema_id) = 'SO')
        PRINT '  SO.' + @t + ' already there';
    ELSE
        PRINT '  ' + @t + ' not found - skipped';

    FETCH NEXT FROM cur INTO @t;
END
CLOSE cur;
DEALLOCATE cur;
GO

SELECT SCHEMA_NAME(t.schema_id) AS sch, t.name, p.rows
FROM   sys.tables t
JOIN   sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0,1)
WHERE  t.name LIKE 'so_price_list%'
ORDER BY t.name;
GO

PRINT 'tables moved to schema SO';
GO
