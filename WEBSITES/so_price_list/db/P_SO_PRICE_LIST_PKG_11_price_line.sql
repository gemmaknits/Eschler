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

   COLOUR TIERS. There are two kinds of request and they behave differently.

   UNDYED - PFE, PFD, PFE/PFD, PFD/PFE, Greige. These are cloth states, not
   colours, and they are priced quite differently from dyed goods. Such a
   request is answered ONLY from an undyed tier:

     1. the tier named exactly           ('PFD' -> PFD)
     2. for PFE or PFD, a combined tier  ('PFE' -> PFE/PFD, or PFD/PFE)

   and if the design has no undyed tier at all, NOTHING is returned. It must
   not fall through to All_colors: on the ANITA list 257035AA/12 is priced only
   at All_colors, and asking for PFE was being answered with the dyed price of
   5.95 - a quote for goods the customer did not order.

   DYED - anything else, including a real colour code like BLACK and the shade
   tiers White, Light, Medium, Dark:

     1. the tier named exactly           ('Light' -> Light)
     2. All_colors, the catch-all

   All_colors is 5,020 of the 8,650 active lines and is what an ordinary colour
   is meant to be priced from, so that fallback stays for dyed requests only.

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

   UNITS, AND CONVERTING WHEN THE BOOK IS KEPT IN ANOTHER ONE.

   The price book is entirely in metres; orders are not. Of the order lines
   waiting to be priced, 192 are MTS, 63 YDS, 30 KGS and 34 are hangers, sets
   or rolls. A price per metre is converted to the unit the order is in:

       a yard is 0.9144 metres, fixed;
       a kilo is dbo.designs.mtkg metres - metres per kilo, by design,
       ranging from 0.29 to 95 across the book.

   Everything converts through metres: price_per_order_unit = price_per_metre
   x metres_in_an_order_unit. uom_factor carries that multiplier and order_uom
   says what the converted price is per, so the arithmetic can be checked.

   THE QUANTITY BAND IS CONVERTED TOO, and this matters more than the price.
   A 40 kg order compared against a 200-600 metre band falls below every band
   and prices at nothing; at 11.5 metres per kilo it is really 460 metres and
   belongs in the middle band. The quantity is restated in the price list's
   own unit before any band is compared, so the band chosen is the one the
   customer is actually buying into.

   HANGER, SET and ROLL have no length at all, and a kilo has none without an
   mtkg - only 11 of the 30 KGS lines have one. Those return nothing rather
   than a price per metre pretending to be a price per piece.

   ydkg is NOT used even though the column exists: it is 0.00 on most rows, so
   yards come from the fixed 0.9144 instead.

   CURRENCY, AND CONVERTING WHEN THERE IS NO PRICE IN IT.

   Some lists are kept in USD only and sold in baht - Chantasia_UPDATE has 309
   USD lines and 3 THB ones - so asking for a THB price there found nothing at
   all. Now, when the requested currency has no line, a currency that does is
   used and its price converted with dbo.exchange_rate.

   The original price and its currency are returned untouched; order_price and
   order_curr carry the converted figure, and exchange_rate_used carries the
   rate it was converted at. Nobody can audit a converted price without the
   rate, and an order-entry figure that cannot be reproduced is not worth
   having. When no conversion was needed the rate is 1 and order_price equals
   price.

   THE RATE is the most recent one published ON OR BEFORE the date - not an
   exact match on it. There are up to 104 rates a day and none at all at
   weekends, so an exact match would fail every Saturday.

   ONE DIRECTION ONLY. Every row in exchange_rate converts INTO baht: curr_to
   is THB in all 36,825 of them. So a USD price can be sold in THB, and a THB
   price cannot be sold in USD - that returns nothing rather than quietly
   inverting the rate. Inverting is arithmetic, not a price: a dealing spread
   is not symmetric, and inventing the other side of it in an order-entry path
   is not a decision this procedure should make.

   THE COLUMN ORDER is the order an order-entry screen reads in: can I use
   this (resolution), what do I charge (order_price, order_curr, order_uom),
   what was that worked out from (price, price_curr, price_uom, and the two
   factors), which line it came from, then the keys and the notes.

   price_curr and price_uom were called curr and uom. With order_curr and
   order_uom beside them the old names said nothing about which side of the
   conversion they were on, and on a list kept in USD per metre and sold in
   baht per yard that is the whole question.

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
    @p_curr                    char(10),
    /* The date the rate is taken from. NULL means today, which is what the
       caller usually wants; an order being re-priced can pass its own date. */
    @p_order_date              date = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @design varchar(30)  = LTRIM(RTRIM(@p_design_no));
    DECLARE @colour varchar(30)  = LTRIM(RTRIM(@p_color_code));
    DECLARE @uom    varchar(20)  = LTRIM(RTRIM(@p_uom));
    DECLARE @curr   varchar(3)   = LTRIM(RTRIM(@p_curr));
    DECLARE @on_date varchar(8)   = CONVERT(varchar(8), ISNULL(@p_order_date, CAST(GETDATE() AS date)), 112);

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
        SELECT
               'design ambiguous' AS resolution,
               /* This branch runs before a currency or unit is settled - these
                  are candidates across several designs, shown exactly as the
                  book prices them. */
               d.price AS order_price,
               LTRIM(RTRIM(d.currency)) AS order_curr,
               LTRIM(RTRIM(d.qty_unit)) AS order_uom,
               d.price AS price,
               d.currency AS price_curr,
               d.qty_unit AS price_uom,
               CAST(1 AS decimal(18,6)) AS exchange_rate_used,
               CAST(1 AS decimal(18,6)) AS uom_factor,
               d.design_no AS matched_design_no,
               d.color_tier,
               d.qty_min,
               d.qty_max,
               d.so_price_list_detail_id,
               d.so_price_list_header_id,
               d.set_no,
               d.line_no,
               d.notes
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
      /* Every currency and every unit, so one can stand in for another. */;

    /* ---- 1a. which unit this is priced in, and the factor to the order's ----

       metres_per_unit: MTS 1, YDS 0.9144, KGS the design's mtkg. Anything else
       has no length and cannot be bridged. */
    DECLARE @mtkg float =
        (SELECT TOP 1 g.mtkg FROM dbo.designs g
         WHERE  RTRIM(g.Design_no) COLLATE DATABASE_DEFAULT
                IN (@design COLLATE DATABASE_DEFAULT, @match_design COLLATE DATABASE_DEFAULT)
           AND  g.mtkg > 0
         ORDER  BY CASE WHEN RTRIM(g.Design_no) COLLATE DATABASE_DEFAULT
                             = @design COLLATE DATABASE_DEFAULT THEN 0 ELSE 1 END);

    DECLARE @mpu_order float =
        CASE UPPER(@uom) WHEN 'MTS' THEN 1.0 WHEN 'M' THEN 1.0
                         WHEN 'YDS' THEN 0.9144 WHEN 'YD' THEN 0.9144
                         WHEN 'KGS' THEN @mtkg  WHEN 'KG' THEN @mtkg END;

    DECLARE @price_uom varchar(20) = NULL;
    DECLARE @uom_factor float = 1;

    IF @uom IS NULL OR @uom = ''
        SET @price_uom = NULL;                        -- caller did not say
    ELSE IF EXISTS (SELECT 1 FROM @cand WHERE LTRIM(RTRIM(uom)) = @uom)
        SET @price_uom = @uom;                        -- priced in it already
    ELSE
    BEGIN
        /* Nothing in the order's unit. Take the unit with the most lines that
           can be converted into it. */
        IF @mpu_order IS NULL RETURN;                 -- no length: nothing to convert through

        SELECT TOP 1 @price_uom = u.cur, @uom_factor = @mpu_order / u.mpu
        FROM  (SELECT LTRIM(RTRIM(x.uom)) AS cur, COUNT(*) AS lines,
                      CASE UPPER(LTRIM(RTRIM(x.uom)))
                           WHEN 'MTS' THEN 1.0 WHEN 'M'  THEN 1.0
                           WHEN 'YDS' THEN 0.9144 WHEN 'YD' THEN 0.9144
                           WHEN 'KGS' THEN @mtkg WHEN 'KG' THEN @mtkg END AS mpu
               FROM   @cand x
               GROUP  BY LTRIM(RTRIM(x.uom))) u
        WHERE  u.mpu IS NOT NULL AND u.mpu > 0
        ORDER  BY u.lines DESC;

        /* Priced, but in a unit this order cannot be converted from. */
        IF @price_uom IS NULL RETURN;
    END

    IF @price_uom IS NOT NULL
        DELETE FROM @cand WHERE LTRIM(RTRIM(uom)) <> @price_uom;

    /* The order quantity, restated in the unit the bands are written in.

       MULTIPLIED, not divided. uom_factor is metres in one order unit, so 40
       kilos at 9.34 metres a kilo is 40 x 9.34 = 373.6 metres. Dividing gave
       4.28, which falls below every band - every kilo order priced as if it
       were the smallest possible. The yard case hid it: 800 / 0.9144 and
       800 x 0.9144 both land in the same band. */
    DECLARE @qty_match numeric(18,4) = @p_qty;
    IF @uom_factor <> 1 AND @uom_factor > 0
        SET @qty_match = CAST(@p_qty * @uom_factor AS numeric(18,4));

    /* ---- 1b. which currency this is priced from, and at what rate ---- */
    DECLARE @price_curr varchar(3) = NULL;
    DECLARE @rate decimal(18,6) = 1;

    IF @curr IS NULL OR @curr = ''
        SET @price_curr = NULL;                       -- caller did not say; take it as priced
    ELSE IF EXISTS (SELECT 1 FROM @cand WHERE LTRIM(RTRIM(curr)) = @curr)
        SET @price_curr = @curr;                      -- priced in it already
    ELSE
    BEGIN
        /* Nothing in the requested currency. Take the currency that has the
           most lines here and a published rate into the requested one. */
        SELECT TOP 1 @price_curr = c.cur, @rate = c.rate
        FROM  (SELECT LTRIM(RTRIM(x.curr)) AS cur, COUNT(*) AS lines,
                      (SELECT TOP 1 e.exchange_rate
                       FROM   dbo.exchange_rate e
                       WHERE  e.curr_fr = LTRIM(RTRIM(x.curr)) COLLATE DATABASE_DEFAULT
                         AND  e.curr_to = @curr COLLATE DATABASE_DEFAULT
                         AND  e.curr_date <= @on_date
                       ORDER  BY e.curr_date DESC, e.curr_time DESC) AS rate
               FROM   @cand x
               GROUP  BY LTRIM(RTRIM(x.curr))) c
        WHERE  c.rate IS NOT NULL
        ORDER  BY c.lines DESC;

        /* Priced in something, but nothing that can be sold in the currency
           asked for - see the one-direction note at the top. */
        IF @price_curr IS NULL RETURN;
    END

    IF @price_curr IS NOT NULL
        DELETE FROM @cand WHERE LTRIM(RTRIM(curr)) <> @price_curr;

    /* ---- 2. which colour tier applies ---- */
    DECLARE @tier nvarchar(60) = NULL;

    /* A cloth state rather than a colour. */
    DECLARE @undyed bit =
        CASE WHEN UPPER(@colour) IN ('PFE', 'PFD', 'PFE/PFD', 'PFD/PFE', 'GREIGE')
             THEN 1 ELSE 0 END;

    -- named exactly
    SELECT TOP 1 @tier = color_tier FROM @cand
    WHERE  UPPER(LTRIM(RTRIM(color_tier))) = UPPER(@colour);

    -- a combined tier, for the two codes that have one
    IF @tier IS NULL AND UPPER(@colour) IN ('PFE', 'PFD')
        SELECT TOP 1 @tier = color_tier FROM @cand
        WHERE  UPPER(LTRIM(RTRIM(color_tier))) IN ('PFE/PFD', 'PFD/PFE');

    /* The catch-all, for dyed requests only. An undyed one is never answered
       from All_colors - see the note at the top. */
    IF @tier IS NULL AND @undyed = 0
        SELECT TOP 1 @tier = color_tier FROM @cand
        WHERE  LTRIM(RTRIM(color_tier)) = 'All_colors';

    /* Undyed, and this design has no undyed tier: there is no right answer,
       and showing the dyed rows would invite the wrong one. */
    IF @tier IS NULL AND @undyed = 1 RETURN;

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
                 AND  @qty_match >= qty_min
                 AND  (qty_max IS NULL OR @qty_match <= qty_max)) z;

        IF @sets = 1
            SELECT TOP 1 @set_hdr = so_price_list_header_id, @set_no = set_no
            FROM   @cand
            WHERE  color_tier = @tier
              AND  @qty_match >= qty_min
              AND  (qty_max IS NULL OR @qty_match <= qty_max);
    END

    /* ---- 4. answer, or candidates ---- */
    IF @tier IS NOT NULL AND @set_no IS NOT NULL
        SELECT
               /* can I use this? */
               'matched' AS resolution,
               /* what to charge */
               CAST(price * @uom_factor * @rate AS decimal(18,4)) AS order_price,
               ISNULL(NULLIF(@curr, ''), curr) AS order_curr,
               ISNULL(NULLIF(@uom,  ''), uom)  AS order_uom,
               /* what that was worked out from */
               price      AS price,
               curr       AS price_curr,
               uom        AS price_uom,
               @rate      AS exchange_rate_used,
               CAST(@uom_factor AS decimal(18,6)) AS uom_factor,
               /* which line it came from */
               @match_design AS matched_design_no,
               color_tier,
               qty_min,
               qty_max,
               /* keys, and the small print */
               so_price_list_detail_id,
               so_price_list_header_id,
               set_no,
               line_no,
               notes
        FROM   @cand
        WHERE  color_tier = @tier
          AND  so_price_list_header_id = @set_hdr AND set_no = @set_no
        ORDER  BY so_price_list_header_id, set_no, line_no;

    ELSE IF @tier IS NOT NULL
        /* the colour is settled, the quantity is not - every band of it */
        SELECT
               /* can I use this? */
               'quantity ambiguous' AS resolution,
               /* what to charge */
               CAST(price * @uom_factor * @rate AS decimal(18,4)) AS order_price,
               ISNULL(NULLIF(@curr, ''), curr) AS order_curr,
               ISNULL(NULLIF(@uom,  ''), uom)  AS order_uom,
               /* what that was worked out from */
               price      AS price,
               curr       AS price_curr,
               uom        AS price_uom,
               @rate      AS exchange_rate_used,
               CAST(@uom_factor AS decimal(18,6)) AS uom_factor,
               /* which line it came from */
               @match_design AS matched_design_no,
               color_tier,
               qty_min,
               qty_max,
               /* keys, and the small print */
               so_price_list_detail_id,
               so_price_list_header_id,
               set_no,
               line_no,
               notes
        FROM   @cand
        WHERE  color_tier = @tier
        ORDER  BY so_price_list_header_id, set_no, line_no;

    ELSE
        /* nothing settled - everything this design has */
        SELECT
               /* can I use this? */
               'colour and quantity ambiguous' AS resolution,
               /* what to charge */
               CAST(price * @uom_factor * @rate AS decimal(18,4)) AS order_price,
               ISNULL(NULLIF(@curr, ''), curr) AS order_curr,
               ISNULL(NULLIF(@uom,  ''), uom)  AS order_uom,
               /* what that was worked out from */
               price      AS price,
               curr       AS price_curr,
               uom        AS price_uom,
               @rate      AS exchange_rate_used,
               CAST(@uom_factor AS decimal(18,6)) AS uom_factor,
               /* which line it came from */
               @match_design AS matched_design_no,
               color_tier,
               qty_min,
               qty_max,
               /* keys, and the small print */
               so_price_list_detail_id,
               so_price_list_header_id,
               set_no,
               line_no,
               notes
        FROM   @cand
        ORDER  BY so_price_list_header_id, set_no, line_no;
END
GO

PRINT 'SO_PRICE_LIST_PKG part 11 (price line for an SO line) created';
GO
