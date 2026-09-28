/* ============================================================================
   SO_PRICE_LIST_PKG part 10 - removing a column from a list.

   Taking a colour tier or a currency off a list means taking away the price
   lines behind it. That is only safe while there is nothing IN them, so the
   rule is enforced here rather than in the browser: a column whose lines carry
   a real price is refused outright, and the caller is told how many.

   "A real price" is a price that is neither NULL nor zero. Zero is not a
   quote - it is what the other half of a USD/THB pair is born as when only one
   side was typed, and those halves are exactly what this is for. Of 236
   tier/currency columns in the book, two are entirely zero.

   SOFT, like every other delete here: delete_mark goes to 'Y' and the rows
   stay, so a column removed by mistake is recoverable in the table.

   QUOTED_IDENTIFIER
   so_price_list_detail has filtered indexes, so any session writing to it
   needs QUOTED_IDENTIFIER ON or the write fails with Msg 1934. sqlcmd defaults
   it OFF where SSMS defaults it ON.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO

IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_remove_price_list_column','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_remove_price_list_column;
GO
-- =============================================
-- Description: Soft-delete a list's lines for one colour tier or one currency,
--              but only while none of them hold a price.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_remove_price_list_column 3, 'Light', null, 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_remove_price_list_column]
    @so_price_list_header_id bigint,
    @color_tier              nvarchar(60) = null,   -- one of these two,
    @currency                char(3)      = null,   -- not both
    @logempcd                varchar(15)  = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF (@color_tier IS NULL AND @currency IS NULL)
    OR (@color_tier IS NOT NULL AND @currency IS NOT NULL)
    BEGIN
        RAISERROR('Give either a colour tier or a currency, not both.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM SO.so_price_list_header
                   WHERE so_price_list_header_id = @so_price_list_header_id
                     AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('Price list not found, or already deleted.', 16, 1);
        RETURN;
    END

    /* Anything actually quoted in this column stops the whole thing. */
    DECLARE @priced int =
        (SELECT COUNT(*)
         FROM   SO.so_price_list_detail
         WHERE  so_price_list_header_id = @so_price_list_header_id
           AND  delete_mark <> 'Y'
           AND  price IS NOT NULL AND price <> 0
           AND  ((@color_tier IS NOT NULL AND color_tier = @color_tier)
              OR (@currency   IS NOT NULL AND LTRIM(RTRIM(currency)) = LTRIM(RTRIM(@currency)))));

    IF @priced > 0
    BEGIN
        DECLARE @what nvarchar(60) = ISNULL(@color_tier, @currency);
        RAISERROR('This list has %d price(s) at %s. Clear them before removing the column.',
                  16, 1, @priced, @what);
        RETURN;
    END

    UPDATE SO.so_price_list_detail
    SET    delete_mark       = 'Y',
           deleted_by        = @logempcd,
           last_updated_date = SYSDATETIME(),
           updated_by        = @logempcd
    WHERE  so_price_list_header_id = @so_price_list_header_id
      AND  delete_mark <> 'Y'
      AND  ((@color_tier IS NOT NULL AND color_tier = @color_tier)
         OR (@currency   IS NOT NULL AND LTRIM(RTRIM(currency)) = LTRIM(RTRIM(@currency))));

    SELECT @@ROWCOUNT AS lines_removed;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 10 (columns) created';
GO
