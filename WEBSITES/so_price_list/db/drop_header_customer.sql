/* ============================================================================
   The header's own customer columns go.

   They were never populated by the import - at the snapshot taken before any
   of this work, all 53 headers had customer_id and customer_name NULL. The
   three values that existed by the time the assignment table arrived were set
   through the app on the same day, and all three are already rows in
   SO.so_price_list_customers. So nothing is carried in these columns that is
   not carried better elsewhere, and keeping them "for what the import matched"
   was a reason built on something that never happened.

   A price list is quoted to several customers. so_price_list_customers says
   that; a single column on the header cannot.

   Checked before dropping: every header customer_id has a matching assignment.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO
SET XACT_ABORT ON;
GO

/* Refuse to run if anything would be lost. */
IF EXISTS (
    SELECT 1 FROM SO.so_price_list_header h
    WHERE h.customer_id IS NOT NULL AND h.delete_mark <> 'Y'
      AND NOT EXISTS (SELECT 1 FROM SO.so_price_list_customers c
                      WHERE c.so_price_list_header_id = h.so_price_list_header_id
                        AND c.customer_id = h.customer_id))
BEGIN
    RAISERROR('Some header customer_id has no matching assignment - not dropping.', 16, 1);
    RETURN;
END
GO

/* Indexes and defaults on the columns have to go first. */
DECLARE @sql nvarchar(max) = N'';
SELECT @sql = @sql + N'DROP INDEX ' + QUOTENAME(i.name)
                   + N' ON SO.so_price_list_header;' + CHAR(10)
FROM   sys.indexes i
WHERE  i.object_id = OBJECT_ID('SO.so_price_list_header')
  AND  i.type > 0 AND i.is_primary_key = 0
  AND  EXISTS (SELECT 1 FROM sys.index_columns ic
               JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
               WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
                 AND c.name IN ('customer_id','customer_name'));

SELECT @sql = @sql + N'ALTER TABLE SO.so_price_list_header DROP CONSTRAINT '
                   + QUOTENAME(dc.name) + N';' + CHAR(10)
FROM   sys.default_constraints dc
JOIN   sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
WHERE  dc.parent_object_id = OBJECT_ID('SO.so_price_list_header')
  AND  c.name IN ('customer_id','customer_name');

IF @sql <> N'' EXEC sp_executesql @sql;
GO

IF COL_LENGTH('SO.so_price_list_header','customer_id') IS NOT NULL
BEGIN
    ALTER TABLE SO.so_price_list_header DROP COLUMN customer_id;
    PRINT 'customer_id dropped';
END
GO
IF COL_LENGTH('SO.so_price_list_header','customer_name') IS NOT NULL
BEGIN
    ALTER TABLE SO.so_price_list_header DROP COLUMN customer_name;
    PRINT 'customer_name dropped';
END
GO

SELECT COUNT(*) AS lists, SUM(CASE WHEN ac.n > 0 THEN 1 ELSE 0 END) AS lists_with_customers
FROM   SO.so_price_list_header h
OUTER APPLY (SELECT COUNT(*) AS n FROM SO.so_price_list_customers c
             WHERE c.so_price_list_header_id = h.so_price_list_header_id) ac
WHERE  h.delete_mark <> 'Y';
GO
