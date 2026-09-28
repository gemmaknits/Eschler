/* ============================================================================
   SO_PRICE_LIST_PKG part 11 - the price line for a sales order line.

   Given what an order line knows - the price list, the design, a colour code,
   a unit, a quantity and a currency - find the ONE price line that applies.

   WHEN IT CANNOT DECIDE, IT SAYS SO BY RETURNING MORE.
   This is the same bargain get_price already makes with the order-entry
   client: the data is not clean enough to guarantee a single answer, and
   silently choosing one of several would be worse than showing them.

       both resolved      -> the single line that applies
       quantity unclear   -> every band of that design at that colour tier
       colour unclear     -> every line of that design

   A `resolution` column says which of the three happened, so the caller can
   tell an answer from a list of candidates without counting rows. It is last,
   so anything reading the specified columns by position is unaffected.

   COLOUR TIERS, in the order they are tried:

     1. the tier named exactly by @p_color_code       ('PFD' -> PFD)
     2. for PFE or PFD only, a combined tier          ('PFE' -> PFE/PFD)
     3. All_colors, the catch-all

   Step 3 is NOT in the written spec and is the one judgement call here. The
   book holds nine tiers, and 5,020 of 8,650 active lines are All_colors: an
   ordinary colour code like BLACK matches no tier at all, and without this
   step every such lookup would come back "colour unclear" and resolve
   nothing. If a real colour should instead be refused rather than priced at
   All_colors, delete step 3 and the proc will report it as ambiguous.

   DESIGN NUMBERS. The order line usually carries a fully qualified design -
   255434AA/11 - where the price list may only hold the base, 255434. So the
   full number is tried first, and failing that the LONGEST stored design that
   is a prefix of it wins.

   Longest, not "strip the slash": the book holds all three shapes - 312 plain
   (255434), 76 with letters (205652BA), 305 with a slash (255434AA/11) - and
   some start with a letter (W7LF001-1A/14), so no stripping rule fits them
   all. A prefix search needs no format at all, and prefers 255434AA over
   255434 when both exist, which is the more specific answer.

   IT WORKS BOTH WAYS. The spec's example has the parameter longer than what
   is stored, but the live book shows the opposite just as often. On the ANITA
   list a reviewer renamed the 2021 revision of 254990 to 254990AA/11 and
   withdrew the superseded 2012 quote, which still carries the plain number -
   so asking for 254990 found only withdrawn rows and returned nothing. So
   after looking for a stored design that is a prefix of the parameter, it
   looks for stored designs that START WITH the parameter.

   That direction can find several - 254990AA/11 and 254990AA/14 and
   254990BB/11 are all "254990 something" and are genuinely different fabrics.
   Choosing one would be guessing, so all of them come back as candidates with
   resolution 'design ambiguous', which is the same bargain the colour and
   quantity rules already make.

   Either way the comparison must be at least 5 characters. The book contains
   junk design numbers read out of quantity cells - '250', '300kg' - and '250'
   is a prefix of every design in the 250-thousands.

   When the price comes from a base design rather than the one asked for, the
   caller has to know: matched_design_no says which design actually supplied
   it. A silent substitution in an order-entry path is exactly the kind of
   thing nobody notices until an invoice is wrong.

   QUANTITY BANDS. qty_max NULL means "and up" - it is an open top band, not a
   missing value, and 1,031 rows in the book rely on that. A band matches when
   qty >= qty_min AND (qty_max IS NULL OR qty <= qty_max).

   Ambiguity is real and not rare: 188 design-and-band pairs in the book exist
   as two or more sets - the same design, the same band, different prices, the
   alternatives the grid brackets together. Those cannot be resolved here and
   must go back to a person.

   PADDING. @p_uom is char(10) and @p_curr is char(10), so both arrive padded
   with spaces; qty_unit is nvarchar(20) and currency is char(3). Every
   comparison below trims, or nothing would ever match.

   QUOTED_IDENTIFIER
   so_price_list_detail has filtered indexes. This procedure only reads, but
   the house rule is that every file sets it, because sqlcmd defaults it OFF
   where SSMS defaults it ON.
   ============================================================================ */

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
SET NOCOUNT ON;
GO

IF OBJECT_ID('SO.P_SO_PRICE_LIST_PKG_select_price_line','P') IS NOT NULL
    DROP PROCEDURE SO.P_SO_PRICE_LIST_PKG_select_price_line;
GO
-- =============================================
-- Description: The price line that applies to one sales order line, or the
--              candidates to choose from when it cannot be decided.
-- =============================================
-- SO.P_SO_PRICE_LIST_PKG_select_price_line 3, '257144', 'PFE', 'MTS', 800, 'USD'
CREATE PROCEDURE [SO].[P_SO_PRICE_LIST_PKG_select_price_line]
    @p_so_price_list_header_id bigint,          -- NULL = look in every live list
    @p_design_no               varchar(30),
    @p_color_code              varchar(30),
    @p_uom                     char(10),
    @p_qty                     numeric(12,2),
    @p_curr                    char(10)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @design varchar(30)  = LTRIM(RTRIM(@p_design_no));
    DECLARE @colour varchar(30)  = LTRIM(RTRIM(@p_color_code));
    DECLARE @uom    varchar(20)  = LTRIM(RTRIM(@p_uom));
    DECLARE @curr   varchar(3)   = LTRIM(RTRIM(@p_curr));

    IF @design IS NULL OR @design = ''
    BEGIN
        RAISERROR('A design no is required.', 16, 1);
        RETURN;
    END

    /* ---- 0. which design this order line is actually priced from ----
       The full number first; then the longest stored design that is a prefix
       of it. Scope, active and delete_mark are applied here too, so a design
       that exists only on a deleted list cannot be chosen. */
    DECLARE @match_design nvarchar(60) = NULL;

    SELECT TOP 1 @match_design = d.design_no
    FROM   SO.so_price_list_detail d
    JOIN   SO.so_price_list_header h
             ON h.so_price_list_header_id = d.so_price_list_header_id
            AND h.delete_mark <> 'Y'
    WHERE  d.delete_mark <> 'Y' AND d.active = 'Y'
      AND  (@p_so_price_list_header_id IS NULL
            OR d.so_price_list_header_id = @p_so_price_list_header_id)
      AND  LTRIM(RTRIM(d.design_no)) = @design;

    IF @match_design IS NULL
        SELECT TOP 1 @match_design = z.design_no
        FROM  (SELECT DISTINCT LTRIM(RTRIM(d.design_no)) AS design_no
               FROM   SO.so_price_list_detail d
               JOIN   SO.so_price_list_header h
                        ON h.so_price_list_header_id = d.so_price_list_header_id
                       AND h.delete_mark <> 'Y'
               WHERE  d.delete_mark <> 'Y' AND d.active = 'Y'
                 AND  (@p_so_price_list_header_id IS NULL
                       OR d.so_price_list_header_id = @p_so_price_list_header_id)) z
        WHERE  LEN(z.design_no) >= 5
          AND  LEN(z.design_no) < LEN(@design)
          AND  LEFT(@design, LEN(z.design_no)) = z.design_no
        ORDER  BY LEN(z.design_no) DESC;

    /* The other direction: the order line carries the base and the list holds
       a qualified design. More than one may qualify, and they are different
       fabrics, so they are all returned rather than one being picked. */
    DECLARE @wider TABLE (design_no nvarchar(60));

    IF @match_design IS NULL AND LEN(@design) >= 5
        INSERT INTO @wider (design_no)
        SELECT DISTINCT LTRIM(RTRIM(d.design_no))
        FROM   SO.so_price_list_detail d
        JOIN   SO.so_price_list_header h
                 ON h.so_price_list_header_id = d.so_price_list_header_id
                AND h.delete_mark <> 'Y'
        WHERE  d.delete_mark <> 'Y' AND d.active = 'Y'
          AND  (@p_so_price_list_header_id IS NULL
                OR d.so_price_list_header_id = @p_so_price_list_header_id)
          AND  LEN(LTRIM(RTRIM(d.design_no))) > LEN(@design)
          AND  LEFT(LTRIM(RTRIM(d.design_no)), LEN(@design)) = @design;

    IF @match_design IS NULL AND (SELECT COUNT(*) FROM @wider) = 1
        SELECT @match_design = design_no FROM @wider;

    /* Several qualified designs start with it - hand them all back. */
    IF @match_design IS NULL AND (SELECT COUNT(*) FROM @wider) > 1
    BEGIN
        SELECT d.so_price_list_header_id, d.so_price_list_detail_id, d.set_no, d.line_no,
               d.color_tier, d.qty_min, d.qty_max, d.currency AS curr, d.price, d.notes,
               d.qty_unit AS uom,
               'design ambiguous' AS resolution, d.design_no AS matched_design_no
        FROM   SO.so_price_list_detail d
        JOIN   SO.so_price_list_header h
                 ON h.so_price_list_header_id = d.so_price_list_header_id
                AND h.delete_mark <> 'Y'
        JOIN   @wider w ON w.design_no = LTRIM(RTRIM(d.design_no))
        WHERE  d.delete_mark <> 'Y' AND d.active = 'Y'
          AND  (@p_so_price_list_header_id IS NULL
                OR d.so_price_list_header_id = @p_so_price_list_header_id)
        ORDER  BY d.so_price_list_header_id, d.set_no, d.line_no;
        RETURN;
    END

    /* Nothing resembling it anywhere: say so by returning no rows, as an
       unknown design already did. */
    IF @match_design IS NULL RETURN;

    /* ---- 1. everything this order line could possibly be priced from ---- */
    DECLARE @cand TABLE (
        so_price_list_header_id bigint,
        so_price_list_detail_id bigint,
        set_no      int,
        line_no     int,
        color_tier  nvarchar(60),
        qty_min     int,
        qty_max     int NULL,
        curr        char(3),
        price       decimal(18,4),
        notes       nvarchar(2000),
        uom         nvarchar(20)
    );

    INSERT INTO @cand
    SELECT d.so_price_list_header_id, d.so_price_list_detail_id, d.set_no, d.line_no,
           d.color_tier, d.qty_min, d.qty_max, d.currency, d.price, d.notes, d.qty_unit
    FROM   SO.so_price_list_detail d
    JOIN   SO.so_price_list_header h
             ON h.so_price_list_header_id = d.so_price_list_header_id
            AND h.delete_mark <> 'Y'
    WHERE  d.delete_mark <> 'Y'
      AND  d.active = 'Y'
      AND  (@p_so_price_list_header_id IS NULL
            OR d.so_price_list_header_id = @p_so_price_list_header_id)
      AND  LTRIM(RTRIM(d.design_no)) = @match_design
      AND  (@curr IS NULL OR @curr = '' OR LTRIM(RTRIM(d.currency))  = @curr)
      AND  (@uom  IS NULL OR @uom  = '' OR LTRIM(RTRIM(d.qty_unit)) = @uom);

    /* ---- 2. which colour tier applies ---- */
    DECLARE @tier nvarchar(60) = NULL;

    -- named exactly
    SELECT TOP 1 @tier = color_tier FROM @cand
    WHERE  LTRIM(RTRIM(color_tier)) = @colour;

    -- a combined tier, for the two codes that have one
    IF @tier IS NULL AND @colour IN ('PFE', 'PFD')
        SELECT TOP 1 @tier = color_tier FROM @cand
        WHERE  LTRIM(RTRIM(color_tier)) IN ('PFE/PFD', 'PFD/PFE');

    -- the catch-all; see the header note, this step is a judgement call
    IF @tier IS NULL
        SELECT TOP 1 @tier = color_tier FROM @cand
        WHERE  LTRIM(RTRIM(color_tier)) = 'All_colors';

    /* ---- 3. which quantity band applies, within that tier ---- */
    /* A set is identified by (header, set_no), never by set_no alone - the
       numbering restarts in every price list. With @p_so_price_list_header_id
       NULL this searches several lists at once, and counting set_no on its own
       would collapse list 3's set 10 and list 31's set 10 into one and report
       a single confident match where there are two candidates. */
    DECLARE @sets int = 0, @set_no int = NULL, @set_hdr bigint = NULL;

    IF @tier IS NOT NULL
    BEGIN
        SELECT @sets = COUNT(*)
        FROM  (SELECT DISTINCT so_price_list_header_id, set_no
               FROM   @cand
               WHERE  color_tier = @tier
                 AND  @p_qty >= qty_min
                 AND  (qty_max IS NULL OR @p_qty <= qty_max)) z;

        IF @sets = 1
            SELECT TOP 1 @set_hdr = so_price_list_header_id, @set_no = set_no
            FROM   @cand
            WHERE  color_tier = @tier
              AND  @p_qty >= qty_min
              AND  (qty_max IS NULL OR @p_qty <= qty_max);
    END

    /* ---- 4. answer, or candidates ---- */
    IF @tier IS NOT NULL AND @set_no IS NOT NULL
        SELECT so_price_list_header_id, so_price_list_detail_id, set_no, line_no,
               color_tier, qty_min, qty_max, curr, price, notes, uom,
               'matched' AS resolution, @match_design AS matched_design_no
        FROM   @cand
        WHERE  color_tier = @tier
          AND  so_price_list_header_id = @set_hdr AND set_no = @set_no
        ORDER  BY so_price_list_header_id, set_no, line_no;

    ELSE IF @tier IS NOT NULL
        /* the colour is settled, the quantity is not - every band of it */
        SELECT so_price_list_header_id, so_price_list_detail_id, set_no, line_no,
               color_tier, qty_min, qty_max, curr, price, notes, uom,
               'quantity ambiguous' AS resolution, @match_design AS matched_design_no
        FROM   @cand
        WHERE  color_tier = @tier
        ORDER  BY so_price_list_header_id, set_no, line_no;

    ELSE
        /* nothing settled - everything this design has */
        SELECT so_price_list_header_id, so_price_list_detail_id, set_no, line_no,
               color_tier, qty_min, qty_max, curr, price, notes, uom,
               'colour and quantity ambiguous' AS resolution, @match_design AS matched_design_no
        FROM   @cand
        ORDER  BY so_price_list_header_id, set_no, line_no;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 11 (price line for an SO line) created';
GO
