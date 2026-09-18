/* ============================================================================
   article holds the sheet's full text, so it needs room for it.

   article was nvarchar(30), which was enough while it was only ever a mirror
   of design_no. It is not any more: the workbook re-quotes a design with a
   second reference after it, and the longest of those - "257225AA/23 (11072
   item customer)" - is 33 characters. The rebuild failed on Msg 8152 and
   rolled back cleanly, which is what XACT_ABORT is there for.

   120, matching design_no, rather than 34: there is no cost to the headroom
   and no appetite for doing this again.

   SAFE FOR THE ORDER-ENTRY CLIENT. Widening a column only accepts more than it
   did before; every value that fitted still fits, and nothing that reads it
   needs to change. IX_spld_article is rebuilt implicitly by the ALTER.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO

IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID('SO.so_price_list_detail')
             AND name = 'article' AND max_length < 240)   -- 240 bytes = 120 nchar
BEGIN
    ALTER TABLE SO.so_price_list_detail ALTER COLUMN article nvarchar(120) NULL;
    PRINT 'so_price_list_detail.article widened to nvarchar(120)';
END
ELSE
    PRINT 'article is already wide enough - nothing to do';
GO

/* The raw extract table carries the same column and the same reason. */
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID('SO.so_price_list')
             AND name = 'article' AND max_length < 240)
BEGIN
    ALTER TABLE SO.so_price_list ALTER COLUMN article nvarchar(120) NULL;
    PRINT 'so_price_list.article widened to nvarchar(120)';
END
GO

SELECT c.name AS column_name, t.name AS type_name, c.max_length / 2 AS nchars
FROM   sys.columns c
JOIN   sys.types t ON t.user_type_id = c.user_type_id
WHERE  c.object_id = OBJECT_ID('SO.so_price_list_detail')
  AND  c.name IN ('article', 'design_no');
GO
