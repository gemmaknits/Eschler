/* ============================================================================
   price_line_date - when the price on a grid row was quoted.

   The imported notes carry this date as prose and nothing could read it:
   "Quoted Price by K. Sivy on 20.12.2022", "valid on 30th September 2026".
   A real column gives the reviewers somewhere to put it while they are going
   through the data, and something to sort and filter on afterwards.

   GRANULARITY: one date per GRID ROW, not per price cell.

   A grid row is one set - a design and a quantity band - and it holds several
   detail lines, one per colour tier and currency. This column is written to
   every line of the set and edited once, exactly like fabric_name, composition
   and moq, which are properties of the row rather than of an individual price.
   That is what lets it be a single column in the grid.

   NOT read by order entry. get_price is deliberately left alone: it is called
   by the VB.NET client and that contract must not change. This date is a
   record for the people reviewing the book, nothing more.

   `date`, not `datetime`. Nobody quotes a price at 14:32.

   QUOTED_IDENTIFIER
   so_price_list_detail has filtered indexes, so any session that writes to it
   needs QUOTED_IDENTIFIER ON or the write fails with Msg 1934. sqlcmd defaults
   it OFF where SSMS defaults it ON, hence the explicit SET below.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/* Idempotent: safe to re-run, and safe to run against a database where an
   earlier attempt got half way. */
IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID('SO.so_price_list_detail')
                 AND name = 'price_line_date')
BEGIN
    ALTER TABLE SO.so_price_list_detail ADD price_line_date date NULL;
    PRINT 'price_line_date added to SO.so_price_list_detail';
END
ELSE
    PRINT 'price_line_date already present - nothing to do';
GO

/* Nullable with no default and no backfill. Every existing line gets NULL,
   which reads as "nobody has said yet" - the honest answer. Guessing a date
   from the import would be inventing one, and these 8,582 lines are being
   reviewed by hand precisely because the source data cannot be trusted. */

SELECT COUNT(*)                                  AS detail_lines,
       COUNT(price_line_date)                    AS with_a_date,
       COUNT(*) - COUNT(price_line_date)         AS still_blank
FROM   SO.so_price_list_detail
WHERE  delete_mark <> 'Y';
GO
