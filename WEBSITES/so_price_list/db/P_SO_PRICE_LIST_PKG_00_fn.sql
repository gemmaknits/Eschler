/* ============================================================================
   SO_PRICE_LIST_PKG  -  part 0 : helpers the other parts share

   Deploy this FIRST - parts 01 and 02 both call the function below.

   Colour tiers have a reading order that is neither alphabetical nor the order
   they were entered: bleach states first, then light to dark, then the catch-all.
   That order was written out as a CASE in two places and as TIER_ORDER in the
   browser; a third copy would eventually disagree with the other two, so it
   lives here once and the procedures call it.
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

IF OBJECT_ID('SO.F_SO_PRICE_LIST_tier_order','FN') IS NOT NULL
    DROP FUNCTION SO.F_SO_PRICE_LIST_tier_order;
GO

-- SELECT SO.F_SO_PRICE_LIST_tier_order('All_colors')   -->  9
CREATE FUNCTION [SO].[F_SO_PRICE_LIST_tier_order] (@color_tier nvarchar(30))
RETURNS int
WITH SCHEMABINDING
AS
BEGIN
    /* unknown tiers fall to the end, where the caller sorts them by name */
    RETURN CASE @color_tier
                WHEN N'PFE/PFD'    THEN 1
                WHEN N'PFE'        THEN 2
                WHEN N'PFD'        THEN 3
                WHEN N'Greige'     THEN 4
                WHEN N'White'      THEN 5
                WHEN N'Light'      THEN 6
                WHEN N'Medium'     THEN 7
                WHEN N'Dark'       THEN 8
                WHEN N'All_colors' THEN 9
                ELSE 99 END;
END
GO

PRINT 'SO.F_SO_PRICE_LIST_tier_order created';
GO
