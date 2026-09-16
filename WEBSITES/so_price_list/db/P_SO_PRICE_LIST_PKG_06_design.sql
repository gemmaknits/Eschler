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
        /* refdesno usually names the fabric, but 403 of its 2,279 values are
           design numbers and some rows point at themselves - 205699AA carries
           refdesno '205699AA'. A self-reference names nothing, so it is
           dropped rather than written into the fabric column.

           dm also carries a composition of its own, filled in on rows where
           designs.compo is not. */
        SELECT TOP 1
               NULLIF(
                 CASE WHEN LTRIM(RTRIM(ISNULL(mm.refdesno,''))) = LTRIM(RTRIM(mm.Design_no))
                      THEN '' ELSE LTRIM(RTRIM(ISNULL(mm.refdesno,''))) END
                 COLLATE DATABASE_DEFAULT, '') AS fabric_name,
               NULLIF(LTRIM(RTRIM(mm.compo)) COLLATE DATABASE_DEFAULT, '') AS dm_compo
        FROM   dbo.dm mm
        WHERE  LTRIM(RTRIM(mm.Design_no)) COLLATE DATABASE_DEFAULT = @key COLLATE DATABASE_DEFAULT
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
               OR (SELECT dm_compo FROM m) IS NOT NULL
               OR (SELECT compo FROM g) IS NOT NULL
             THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS found,

        NULLIF((SELECT fabric_name FROM m), '')          AS fabric_name,
        COALESCE(NULLIF((SELECT compo FROM g), ''),
                 (SELECT dm_compo FROM m))            AS composition,

        /* "15 - 20 g/m2" when a range is on record, "18 g/m2" when only the
           single figure is, nothing when neither. */
        CASE
          WHEN NULLIF(NULLIF((SELECT gsm_min FROM g),''),'0') IS NOT NULL
           AND NULLIF(NULLIF((SELECT gsm_max FROM g),''),'0') IS NOT NULL
          THEN (SELECT gsm_min FROM g) + ' - ' + (SELECT gsm_max FROM g) + ' g/m2'
          WHEN NULLIF(NULLIF((SELECT gsm FROM g),''),'0') IS NOT NULL
          THEN (SELECT gsm FROM g) + ' g/m2'
        END AS weight_gsm,

        CASE
          WHEN NULLIF(NULLIF((SELECT usew_min FROM g),''),'0') IS NOT NULL
           AND NULLIF(NULLIF((SELECT usew_max FROM g),''),'0') IS NOT NULL
          THEN (SELECT usew_min FROM g) + ' - ' + (SELECT usew_max FROM g) + ' cm.'
          WHEN NULLIF(NULLIF((SELECT usew FROM g),''),'0') IS NOT NULL
          THEN (SELECT usew FROM g) + ' cm.'
        END AS usable_width_cm,

        CASE
          WHEN NULLIF(NULLIF((SELECT fw_min FROM g),''),'0') IS NOT NULL
           AND NULLIF(NULLIF((SELECT fw_max FROM g),''),'0') IS NOT NULL
          THEN (SELECT fw_min FROM g) + ' - ' + (SELECT fw_max FROM g) + ' cm.'
          WHEN NULLIF(NULLIF((SELECT fw FROM g),''),'0') IS NOT NULL
          THEN (SELECT fw FROM g) + ' cm.'
        END AS full_width_cm;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 6 (design lookup) created';
GO

/* ---------------------------------------------------------------------------
   select_design_list  -  the design list of values.

   A design number in a price list is often the STEM of the numbers the ERP
   actually carries: the list says 255699AA, dm holds 255699AA/14, /15 and /16.
   Typing the stem and tabbing out therefore has to offer the real ones rather
   than report "not found", which is what this feeds.

   Matching is deliberately wide - the stem anywhere in the number, or in the
   fabric name - because someone reviewing a price list may remember the fabric
   ("PORTO") and not the number. Ordering puts the closest first:

     1  exact
     2  starts with what was typed   (255699AA -> 255699AA/14)
     3  contains it
     4  matched on the fabric name instead

   refdesno is NULL on plenty of rows (of the three 255699AA variants, only /14
   has one), so composition and weight come along to tell them apart.
   --------------------------------------------------------------------------- */
IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_select_design_list','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_select_design_list;
GO
-- =============================================
-- Description: Designs matching a partial number or fabric name, closest first.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_select_design_list '255699', 200, 'SURES'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_select_design_list]
    @filter    nvarchar(60) = null,
    @top_n     int          = 200,
    @logempcd  varchar(15)  = ''
AS
BEGIN
    SET NOCOUNT ON;

    IF @top_n IS NULL OR @top_n <= 0 SET @top_n = 200;

    DECLARE @f nvarchar(60) = LTRIM(RTRIM(ISNULL(@filter, '')));

    /* An empty filter would drag the whole design master back, which is neither
       useful in a picker nor cheap. */
    IF @f = ''
    BEGIN
        SELECT TOP 0
               CAST('' AS nvarchar(60)) AS design_no,
               CAST('' AS nvarchar(200)) AS fabric_name,
               CAST('' AS nvarchar(200)) AS composition,
               CAST('' AS nvarchar(40))  AS weight_gsm,
               CAST('' AS nvarchar(40))  AS usable_width_cm,
               CAST(0 AS int)            AS rank_order;
        RETURN;
    END

    SELECT TOP (@top_n)
           d.design_no,
           d.fabric_name,
           /* designs is the fuller record, but dm has a composition too and
              carries it on rows where designs does not. */
           COALESCE(g.composition, d.dm_compo) AS composition,
           g.weight_gsm,
           g.usable_width_cm,
           d.rank_order
    FROM (
        SELECT LTRIM(RTRIM(m.Design_no)) COLLATE DATABASE_DEFAULT AS design_no,
               /* refdesno is a REFERENCE DESIGN field, not a fabric-name field.
                  It usually holds a name ("PORTO INVISIBLE TM EMB") but 403 of
                  its 2,279 values are design numbers, and some rows point at
                  themselves - 205699AA carries refdesno '205699AA'.

                  A self-reference names nothing, so it is dropped. Inheriting a
                  parent's refdesno was tried and removed: it put the parent's
                  design NUMBER in the fabric column, which is the kind of wrong
                  value this whole review exists to get rid of. */
               NULLIF(
                 CASE WHEN LTRIM(RTRIM(ISNULL(m.refdesno,''))) = LTRIM(RTRIM(m.Design_no))
                      THEN '' ELSE LTRIM(RTRIM(ISNULL(m.refdesno,''))) END
                 COLLATE DATABASE_DEFAULT, '') AS fabric_name,
               /* dm carries a composition of its own, and it is filled in on
                  rows where designs.compo is not. */
               NULLIF(LTRIM(RTRIM(m.compo)) COLLATE DATABASE_DEFAULT, '') AS dm_compo,
               CASE
                 WHEN LTRIM(RTRIM(m.Design_no)) COLLATE DATABASE_DEFAULT = @f COLLATE DATABASE_DEFAULT THEN 1
                 WHEN LTRIM(RTRIM(m.Design_no)) COLLATE DATABASE_DEFAULT LIKE @f + '%' COLLATE DATABASE_DEFAULT THEN 2
                 WHEN LTRIM(RTRIM(m.Design_no)) COLLATE DATABASE_DEFAULT LIKE '%' + @f + '%' COLLATE DATABASE_DEFAULT THEN 3
                 ELSE 4
               END AS rank_order
        FROM   dbo.dm m
        WHERE  LTRIM(RTRIM(m.Design_no)) COLLATE DATABASE_DEFAULT LIKE '%' + @f + '%' COLLATE DATABASE_DEFAULT
           OR  LTRIM(RTRIM(ISNULL(m.refdesno,''))) COLLATE DATABASE_DEFAULT LIKE '%' + @f + '%' COLLATE DATABASE_DEFAULT
    ) d
    OUTER APPLY (
        SELECT TOP 1
               NULLIF(LTRIM(RTRIM(s.compo)) COLLATE DATABASE_DEFAULT, '') AS composition,
               CASE
                 WHEN NULLIF(NULLIF(LTRIM(RTRIM(s.gmpersqm_min)) COLLATE DATABASE_DEFAULT,''),'0') IS NOT NULL
                  AND NULLIF(NULLIF(LTRIM(RTRIM(s.gmpersqm_max)) COLLATE DATABASE_DEFAULT,''),'0') IS NOT NULL
                 THEN LTRIM(RTRIM(s.gmpersqm_min)) COLLATE DATABASE_DEFAULT + ' - '
                    + LTRIM(RTRIM(s.gmpersqm_max)) COLLATE DATABASE_DEFAULT + ' g/m2'
                 WHEN NULLIF(NULLIF(LTRIM(RTRIM(s.gmpersqm)) COLLATE DATABASE_DEFAULT,''),'0') IS NOT NULL
                 THEN LTRIM(RTRIM(s.gmpersqm)) COLLATE DATABASE_DEFAULT + ' g/m2'
               END AS weight_gsm,
               CASE
                 WHEN NULLIF(NULLIF(LTRIM(RTRIM(s.Usewth_min)) COLLATE DATABASE_DEFAULT,''),'0') IS NOT NULL
                  AND NULLIF(NULLIF(LTRIM(RTRIM(s.Usewth_max)) COLLATE DATABASE_DEFAULT,''),'0') IS NOT NULL
                 THEN LTRIM(RTRIM(s.Usewth_min)) COLLATE DATABASE_DEFAULT + ' - '
                    + LTRIM(RTRIM(s.Usewth_max)) COLLATE DATABASE_DEFAULT + ' cm.'
                 WHEN NULLIF(NULLIF(LTRIM(RTRIM(s.Usewth)) COLLATE DATABASE_DEFAULT,''),'0') IS NOT NULL
                 THEN LTRIM(RTRIM(s.Usewth)) COLLATE DATABASE_DEFAULT + ' cm.'
               END AS usable_width_cm
        FROM   dbo.designs s
        WHERE  LTRIM(RTRIM(s.Design_no)) COLLATE DATABASE_DEFAULT = d.design_no
    ) g
    ORDER BY d.rank_order, d.design_no;
END
GO

PRINT 'SO_PRICE_LIST_PKG design LOV created';
GO
