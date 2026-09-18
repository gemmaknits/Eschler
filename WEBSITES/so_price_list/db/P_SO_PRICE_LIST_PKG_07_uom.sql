/* ============================================================================
   SO_PRICE_LIST_PKG  -  part 7 : units of measure

   The unit on a price line has to be one the system knows. dbo.uom is that
   list - 23 rows, owned by the ERP, read here and never written. The price
   lines used to carry "M", which is not in it at all; they are MTS now.

   Two things live here: the list, for the picker in the grid, and the check
   the write procedures call so a unit that is not in dbo.uom cannot be saved.

   dbo.uom is Thai_CI_AI, so every comparison is COLLATE DATABASE_DEFAULT.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO

IF OBJECT_ID('SO.F_SO_PRICE_LIST_uom_ok','FN') IS NOT NULL
    DROP FUNCTION SO.F_SO_PRICE_LIST_uom_ok;
GO
-- SELECT SO.F_SO_PRICE_LIST_uom_ok('MTS')  -->  1
CREATE FUNCTION [SO].[F_SO_PRICE_LIST_uom_ok] (@uom nvarchar(10))
RETURNS bit
AS
BEGIN
    IF @uom IS NULL OR LTRIM(RTRIM(@uom)) = '' RETURN 0;
    IF EXISTS (SELECT 1 FROM dbo.uom u
               WHERE LTRIM(RTRIM(u.uom)) COLLATE DATABASE_DEFAULT
                   = LTRIM(RTRIM(@uom))  COLLATE DATABASE_DEFAULT)
        RETURN 1;
    RETURN 0;
END
GO

IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_select_uom','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_select_uom;
GO
-- =============================================
-- Description: The units of measure, for the picker. Length units first -
--              a price list is quoted per metre - then the rest by name.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_select_uom '', 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_select_uom]
    @filter   nvarchar(40) = null,
    @logempcd varchar(15)  = ''
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @f nvarchar(40) = LTRIM(RTRIM(ISNULL(@filter, '')));

    /* The class MTS belongs to is the one a fabric price is quoted in, so it
       sorts first - the picker should open on the units actually wanted. */
    DECLARE @length_class bigint =
        (SELECT TOP 1 uom_class_id FROM dbo.uom
         WHERE LTRIM(RTRIM(uom)) COLLATE DATABASE_DEFAULT = N'MTS' COLLATE DATABASE_DEFAULT);

    SELECT  LTRIM(RTRIM(u.uom)) COLLATE DATABASE_DEFAULT AS uom,
            u.uom_name,
            u.uom_id,
            CAST(CASE WHEN u.uom_class_id = @length_class THEN 1 ELSE 0 END AS bit) AS is_length
    FROM    dbo.uom u
    WHERE   @f = ''
       OR   LTRIM(RTRIM(u.uom)) COLLATE DATABASE_DEFAULT LIKE '%' + @f + '%' COLLATE DATABASE_DEFAULT
       OR   ISNULL(u.uom_name,'') COLLATE DATABASE_DEFAULT LIKE '%' + @f + '%' COLLATE DATABASE_DEFAULT
    ORDER BY CASE WHEN u.uom_class_id = @length_class THEN 0 ELSE 1 END,
             LTRIM(RTRIM(u.uom));
END
GO

PRINT 'SO_PRICE_LIST_PKG part 7 (uom) created';
GO
