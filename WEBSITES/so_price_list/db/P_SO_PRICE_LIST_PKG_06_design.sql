/* ============================================================================
   SO_PRICE_LIST_PKG  -  part 6 : design lookup

   Typing a design number fills in what the company already knows about it, so
   the fabric name and the spec are not retyped (and mistyped) per price list.

   WHERE IT COMES FROM
     dbo.dm       refdesno   -> fabric name  ("TATTOO", "ELASTIC TULLE PASCAL")
     dbo.designs  compo      -> composition
                  gmpersqm   -> weight, as a min-max range
                  Usewth     -> usable width, as a min-max range
                  Fwth       -> full width, as a min-max range

   Both are ERP tables owned by other systems. This READS them and never writes.

   COLLATION
   dm and designs are Thai_CI_AI; the SO tables are the database default. An
   unqualified join between them fails outright:

       Msg 468 - Cannot resolve the collation conflict between "Thai_CI_AI" and
       "SQL_Latin1_General_CP1_CI_AS" in the equal to operation

   so every comparison across the boundary is COLLATE DATABASE_DEFAULT.

   FORMAT
   The masters hold bare numbers, but a price list reads "15 - 20 g/m2" and
   "147 - 152 cm." - and the min/max columns reproduce exactly that, checked
   against the imported data. So the range is built here rather than leaving
   the browser to invent a format.

   NO MATCH IS NORMAL
   31 of the 261 designs in the price lists are not in dm at all. Those return
   an empty row rather than a guess: stripping the /suffix off 255703/11 to
   find a base design would attach a fabric name that may not be that fabric.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

SET NOCOUNT ON;
GO

IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_select_design','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_select_design;
GO
-- =============================================
-- Description: Fabric name and spec for one design number, for the grid to
--              prefill. Returns one row; found = 0 when the design is unknown.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_select_design '255167', 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_select_design]
    @design_no nvarchar(60),
    @logempcd  varchar(15) = ''
AS
BEGIN
    SET NOCOUNT ON;

    IF @design_no IS NULL OR LTRIM(RTRIM(@design_no)) = ''
    BEGIN
        RAISERROR('Design no is required.', 16, 1);
        RETURN;
    END

    DECLARE @key nvarchar(60) = LTRIM(RTRIM(@design_no));

    ;WITH m AS (
        SELECT TOP 1 LTRIM(RTRIM(refdesno)) COLLATE DATABASE_DEFAULT AS fabric_name
        FROM   dbo.dm
        WHERE  LTRIM(RTRIM(Design_no)) COLLATE DATABASE_DEFAULT = @key COLLATE DATABASE_DEFAULT
    ),
    g AS (
        SELECT TOP 1
               LTRIM(RTRIM(compo)) COLLATE DATABASE_DEFAULT        AS compo,
               LTRIM(RTRIM(gmpersqm)) COLLATE DATABASE_DEFAULT     AS gsm,
               LTRIM(RTRIM(gmpersqm_min)) COLLATE DATABASE_DEFAULT AS gsm_min,
               LTRIM(RTRIM(gmpersqm_max)) COLLATE DATABASE_DEFAULT AS gsm_max,
               LTRIM(RTRIM(Usewth)) COLLATE DATABASE_DEFAULT       AS usew,
               LTRIM(RTRIM(Usewth_min)) COLLATE DATABASE_DEFAULT   AS usew_min,
               LTRIM(RTRIM(Usewth_max)) COLLATE DATABASE_DEFAULT   AS usew_max,
               LTRIM(RTRIM(Fwth)) COLLATE DATABASE_DEFAULT         AS fw,
               LTRIM(RTRIM(Fwth_min)) COLLATE DATABASE_DEFAULT     AS fw_min,
               LTRIM(RTRIM(Fwth_max)) COLLATE DATABASE_DEFAULT     AS fw_max
        FROM   dbo.designs
        WHERE  LTRIM(RTRIM(Design_no)) COLLATE DATABASE_DEFAULT = @key COLLATE DATABASE_DEFAULT
    )
    SELECT
        @key AS design_no,
        CASE WHEN (SELECT fabric_name FROM m) IS NOT NULL
               OR (SELECT compo FROM g) IS NOT NULL
             THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS found,

        NULLIF((SELECT fabric_name FROM m), '')          AS fabric_name,
        NULLIF((SELECT compo FROM g), '')                AS composition,

        /* "15 - 20 g/m2" when a range is on record, "18 g/m2" when only the
           single figure is, nothing when neither. */
        CASE
          WHEN NULLIF((SELECT gsm_min FROM g),'') IS NOT NULL
           AND NULLIF((SELECT gsm_max FROM g),'') IS NOT NULL
          THEN (SELECT gsm_min FROM g) + ' - ' + (SELECT gsm_max FROM g) + ' g/m2'
          WHEN NULLIF((SELECT gsm FROM g),'') IS NOT NULL
          THEN (SELECT gsm FROM g) + ' g/m2'
        END AS weight_gsm,

        CASE
          WHEN NULLIF((SELECT usew_min FROM g),'') IS NOT NULL
           AND NULLIF((SELECT usew_max FROM g),'') IS NOT NULL
          THEN (SELECT usew_min FROM g) + ' - ' + (SELECT usew_max FROM g) + ' cm.'
          WHEN NULLIF((SELECT usew FROM g),'') IS NOT NULL
          THEN (SELECT usew FROM g) + ' cm.'
        END AS usable_width_cm,

        CASE
          WHEN NULLIF((SELECT fw_min FROM g),'') IS NOT NULL
           AND NULLIF((SELECT fw_max FROM g),'') IS NOT NULL
          THEN (SELECT fw_min FROM g) + ' - ' + (SELECT fw_max FROM g) + ' cm.'
          WHEN NULLIF((SELECT fw FROM g),'') IS NOT NULL
          THEN (SELECT fw FROM g) + ' cm.'
        END AS full_width_cm;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 6 (design lookup) created';
GO
