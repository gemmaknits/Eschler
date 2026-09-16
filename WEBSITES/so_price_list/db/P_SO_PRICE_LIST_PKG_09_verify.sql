/* ============================================================================
   SO_PRICE_LIST_PKG  -  part 9 : "I have checked this list"

   The price lines were read out of a workbook by a scanner. It found a great
   deal more than the first import did, but it is an assisted reading and not
   a guarantee - some prices are still wrong, some units and tiers are still
   guesses. A person has to go through a list, correct what is wrong, and say
   so. This is where they say so.

   ONE TICK PER PRICE LIST
   The unit is the list, not the line. "I checked everything" is a statement
   about a sheet somebody sat down with; 8,734 line-by-line ticks would be a
   worse record of the same thing and nobody would finish it.

   WHO, NOT JUST WHETHER
   A confirmation nobody signed is worth very little, so verified_by and
   verified_date are part of it - and the procedure REFUSES an anonymous one.
   That matters here more than elsewhere in this app: several edits have
   already been saved with a blank user because the page was opened without
   one on the URL.

   Unticking is allowed and clears both. Somebody finding a mistake in a list
   already marked verified needs to be able to put it back.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO

IF COL_LENGTH('SO.so_price_list_header','excel_data_verified') IS NULL
BEGIN
    ALTER TABLE SO.so_price_list_header
        ADD excel_data_verified char(1) NOT NULL
            CONSTRAINT DF_splh_excel_data_verified DEFAULT ('N');
    PRINT 'excel_data_verified added, default N';
END
ELSE PRINT 'excel_data_verified already exists';
GO

IF COL_LENGTH('SO.so_price_list_header','verified_by') IS NULL
BEGIN
    ALTER TABLE SO.so_price_list_header ADD verified_by varchar(15) NULL;
    ALTER TABLE SO.so_price_list_header ADD verified_date datetime2(0) NULL;
    PRINT 'verified_by / verified_date added';
END
ELSE PRINT 'verified_by / verified_date already exist';
GO

/* ---------------------------------------------------------------------------
   set_price_list_verified  -  tick or untick one list.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_set_price_list_verified','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_set_price_list_verified;
GO
-- =============================================
-- Description: Record that a person has checked a price list's data, or take
--              that back. Refuses to record an unsigned confirmation.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_set_price_list_verified 3, 'Y', 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_set_price_list_verified]
    @so_price_list_header_id bigint,
    @verified                char(1)     = 'Y',
    @logempcd                varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @verified IS NULL OR @verified NOT IN ('Y','N')
    BEGIN
        RAISERROR('Verified must be Y or N.', 16, 1);
        RETURN;
    END

    /* A confirmation is somebody's word. Without a name it is nobody's, and
       this is the one place in the app where that is not good enough. */
    IF @verified = 'Y' AND (@logempcd IS NULL OR LTRIM(RTRIM(@logempcd)) = '')
    BEGIN
        RAISERROR('Who is confirming this? Open the price book from the desktop client, or set your user before ticking.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM SO.so_price_list_header
                   WHERE so_price_list_header_id = @so_price_list_header_id
                     AND delete_mark <> 'Y')
    BEGIN
        RAISERROR('Price list not found, or already deleted.', 16, 1);
        RETURN;
    END

    UPDATE SO.so_price_list_header
    SET    excel_data_verified = @verified,
           /* unticking clears the signature: the list is no longer confirmed
              by anybody, and leaving a stale name there would say it is */
           verified_by       = CASE WHEN @verified = 'Y' THEN @logempcd ELSE NULL END,
           verified_date     = CASE WHEN @verified = 'Y' THEN SYSDATETIME() ELSE NULL END,
           last_updated_date = SYSDATETIME(),
           updated_by        = @logempcd
    WHERE  so_price_list_header_id = @so_price_list_header_id;

    SELECT so_price_list_header_id, list_name,
           excel_data_verified, verified_by, verified_date
    FROM   SO.so_price_list_header
    WHERE  so_price_list_header_id = @so_price_list_header_id;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 9 (verify) created';
GO
