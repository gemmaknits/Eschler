/*  Scanning a sheet into price lines.

    The workbook does not have one table per sheet. Acundis alone holds
    thirteen, each with its own header, and the column order changes between
    them - one table puts Date in column A and shifts everything right, another
    swaps Composition and Weight, a third splits its prices into USD and THB
    groups. So the scan finds each header where it stands and reads the rows
    beneath it on that header's terms.

    Every emitted line carries the sheet, the row it came from and the header
    row it was read under, so any figure can be traced back and checked. */
using ClosedXML.Excel;

sealed class Line
{
    public string Sheet, Article, Fabric, Composition, FullWidth, UsableWidth,
                  Weight, Moq, QtyRaw, Tier, Currency, Remark, DateRaw, BlockNote;
    public int Row, HeaderRow, QtyMin, Col;
    /* true when a LABEL named the currency; false when it was inferred */
    public bool CurrencyStated;
    public int? QtyMax;
    public decimal Price;
}

sealed class Table
{
    public int HeaderRow;
    /* 1, or 2 when the tier labels sit on the row below the names. */
    public int Depth = 1;
    public Dictionary<int, Role> Roles = new();
    public Dictionary<int, string> TierOf = new();       // price column -> colour tier
    public Dictionary<int, string> CurrencyOf = new();   // price column -> USD/THB
    /* set only where a LABEL named the currency, so the repeated-tier rule
       never overrules something the sheet actually said */
    public Dictionary<int, string> StatedCurrency = new();
    /* what the sheet calls this block of prices - the line above the header */
    public string BlockNote;
    /* for tables that put the quantity bands across the top */
    public Dictionary<int, (int Min, int? Max)> QtyOfColumn = new();
}

static class Scan
{
    /* A header needs a price column and something to identify the line by;
       three recognised labels on their own is a coincidence, not a table. */
    static Table ReadHeader(string[][] grid, int r, int cols, string[] sheetCurrencyHints)
    {
        /* Headers are sometimes two rows deep. Several sheets name the money
           on one row and the colour tier on the next:

               DATE | ARTICLE # | Composition | ... | USD FOB BKK / METER | USD FOB BKK / METER
                    |           | g/m2        | ... | PFE/PFD             | ALL COLOR

           Read on its own, neither row is a table: the first has no tier, the
           second has nothing to identify a line by. Together they are one. So
           each column takes whichever of the two rows actually says something,
           and a price column takes its tier from one and its currency from
           the other. */
        var t = new Table { HeaderRow = r };
        int priced = 0, identifying = 0;
        int depth = 1;
        int labelsOnTopRow = 0;
        /* price columns that were recognised ONLY by a money word */
        var moneyOnly = new List<int>();
        /* Some headers run three rows deep: the names, a terms line, then the
           currencies. BIGA Lanka writes "Greige price/kg" and "PFD" on one row,
           "Term : Ex work" on the next, and the USD / THB split on the one
           after that. So a column with nothing directly below it looks one
           row further down. */
        var blank = new string[cols];

        /* A row is only part of the header if it is not DATA. The test is
           whether it carries an article: a continuation row holds labels and
           units ("g/m2", "CM", "PFE/PFD"), never a design number.

           Without this, the first data row was being read as header text, and
           a MOQ column whose first value is "3,000 m." was taken for a column
           of quantity bands - turning quantities into prices of 3000 on
           Chantasia, WEDTEX and Setafil. */
        var below  = (r + 1 < grid.Length) && IsHeaderContinuation(grid[r + 1])
                     ? grid[r + 1] : blank;
        var below2 = (r + 2 < grid.Length) && IsHeaderContinuation(grid[r + 2])
                     ? grid[r + 2] : blank;

        for (int c = 0; c < cols; c++)
        {
            var top = grid[r][c];
            var bot = !string.IsNullOrWhiteSpace(below[c]) ? below[c] : below2[c];
            var botDepth = !string.IsNullOrWhiteSpace(below[c]) ? 2 : 3;
            var roleTop = string.IsNullOrWhiteSpace(top) ? Role.None : Vocab.Of(top);
            var roleBot = string.IsNullOrWhiteSpace(bot) ? Role.None : Vocab.Of(bot);
            if (roleTop != Role.None || Vocab.Tier(top) != null
                || Vocab2.IsMoneyLabel(top) || Vocab2.IsQtyHeader(top, out _, out _))
                labelsOnTopRow++;

            // A euro column is left out rather than converted or guessed at.
            if (Vocab2.IsEuro(top) || Vocab2.IsEuro(bot)) continue;

            // the tier can only come from a label that names one
            var tier = Vocab.Tier(top) ?? Vocab.Tier(bot);

            /* Quantity bands written ACROSS the top instead of down the side:
                   Quality | Group | Product | Name | color Type | 200 m | 400 m | 1000 m+
               Each of those is a price column whose band comes from its own
               header, and the colour tier comes from the row. */
            if (tier == null && (Vocab2.IsQtyHeader(top, out var qm, out var qx)
                              || Vocab2.IsQtyHeader(bot, out qm, out qx)))
            {
                t.Roles[c] = Role.Price;
                t.TierOf[c] = null;                 // resolved per row
                t.QtyOfColumn[c] = (qm, qx);
                t.CurrencyOf[c] = Vocab.Currency(top) ?? Vocab.Currency(bot)
                                  ?? GroupCurrency(grid, r, c, cols)
                                  ?? sheetCurrencyHints.FirstOrDefault();
                priced++;
                continue;
            }

            /* A column that names money but no tier - "USD/ m", "Price/M.",
               "New FOB Bangkok Price USD/m." - is still a price column. */
            bool money = tier == null && roleTop == Role.None && roleBot == Role.None
                         && (Vocab2.IsMoneyLabel(top) || Vocab2.IsMoneyLabel(bot));

            var role = tier != null || money ? Role.Price
                     : roleTop != Role.None ? roleTop
                     : roleBot;
            if (role == Role.None) continue;
            if (role != roleTop && roleBot != Role.None) depth = Math.Max(depth, botDepth);
            if (tier == null && Vocab.Currency(bot) != null && role == Role.Price)
                depth = Math.Max(depth, botDepth);

            /* "Color" on its own is either a tier-value column holding PFD /
               White / Color-Black, or the All-colors PRICE column. The cells
               below it say which, so look rather than guess. */
            if (role == Role.TierValue && LooksNumeric(grid, r + 1, c))
            { role = Role.Price; tier = "All_colors"; }
            else if (role == Role.Price && tier == "All_colors" && LooksTierNames(grid, r + 1, c))
            { role = Role.TierValue; tier = null; }

            /* A column named after a colour that holds WORDS is a description,
               not prices. The PT Busana quotation form has a COLOR column
               reading "White & Black" and "Other colour", with the money in a
               column called PRICE - and treating COLOR as a price column threw
               that whole sheet away. */
            if (role == Role.Price && tier != null && LooksTexty(grid, r + 1, c))
                continue;

            t.Roles[c] = role;

            if (role == Role.Price)
            {
                if (tier == null && money) moneyOnly.Add(c);
                t.TierOf[c] = tier;
                // currency: either label, else a group heading above, else the
                // sheet's own wording
                var stated = Vocab.Currency(top) ?? Vocab.Currency(bot)
                             ?? GroupCurrency(grid, r, c, cols);
                if (stated != null) t.StatedCurrency[c] = stated;
                t.CurrencyOf[c] = stated ?? sheetCurrencyHints.FirstOrDefault();
                priced++;
            }
            else if (role is Role.Article or Role.QtyTier) identifying++;
        }

        /* A money label next to real tier columns is a GROUP HEADING, not a
           price column of its own.

               ... | USD per meter / FOB | USD per meter / FOB | THB per meter ...
               Qty | Article |           | PFE/PFD | All colors | PFD/PFE | ...

           The heading is merged across its columns, so every column it spans
           reads as "money" - including the one holding "3,000 m.", which then
           came through as a price of 3000. Where the table names its tiers,
           those are the price columns and these are dropped. */
        /* ...and only when a tier column actually holds NUMBERS. A tier-named
           column full of words is not competing for the same job. */
        if (moneyOnly.Count > 0
            && t.TierOf.Any(kv => kv.Value != null && LooksNumeric(grid, r + 1, kv.Key)))
            foreach (var c in moneyOnly)
            {
                t.Roles.Remove(c);
                t.TierOf.Remove(c);
                t.CurrencyOf.Remove(c);
                priced--;
            }

        /* The header row must say something itself. Looking one and two rows
           down for labels makes a BLANK row above a real header look like a
           header too - it reads every label from below and matches. That
           shifted the whole table up by one and broke the USD/THB split on the
           CENTER sheet, which had been right. */
        if (labelsOnTopRow == 0) return null;

        if (priced == 0 || identifying == 0) return null;
        t.Depth = depth;
        AssignCurrencyBlocks(t, sheetCurrencyHints);
        return t;
    }

    /* A repeated tier label means a second currency.

           MOQ | Qty | Article | ... | PFE/PFD | All colors | PFE/PFD | All colors
           ... |     | 251051  | ... |    2.00 |       2.60 |      60 |         78

       Four price columns, two tiers, and nothing in the header saying which
       money is which - the sheet simply puts USD first and THB second. The
       repeat is the signal: when a tier appears again, a new currency block
       has started. 2.00 against 60 for the same tier confirms it.

       The first block takes the currency the sheet states, the second takes
       the other. Where a column's own label already named a currency, that
       wins and nothing here overrides it. */
    static void AssignCurrencyBlocks(Table t, string[] sheetHints)
    {
        var cols = t.TierOf.Keys.OrderBy(c => c).ToList();
        var seen = new HashSet<string>();
        var block = new Dictionary<int, int>();
        int b = 0;
        foreach (var c in cols)
        {
            var tier = t.TierOf[c];
            if (tier == null) continue;
            if (!seen.Add(tier)) { b++; seen.Clear(); seen.Add(tier); }
            block[c] = b;
        }
        if (b == 0) return;                       // one block: nothing to split

        var first = sheetHints.FirstOrDefault() ?? "USD";
        var second = first == "USD" ? "THB" : "USD";

        foreach (var (c, idx) in block)
        {
            // a label that named its own currency is never overruled
            var stated = t.StatedCurrency.GetValueOrDefault(c);
            if (stated != null) continue;
            t.CurrencyOf[c] = idx == 0 ? first : second;
        }
    }

    /* The sheet's own name for a block of prices: the nearest non-empty text
       above the header that is not itself part of a header. "Quoted Price by
       K. Sivy on 20.12.2022", "Price valid 01.01.2013 - 31.12.2015".

       Two quotes for the same design and quantity band are two different
       worksheet lines, and telling them apart is the reviewer's whole job -
       without the line that dates them they are identical on screen. */
    static string BlockTitle(string[][] grid, int headerRow)
    {
        /* Usually the title is IN the header row, in the first column, with the
           group headings beside it:

               Quoted Price by K. Sivy on 20.12.2022 | | USD per meter / FOB | ...
               Qty | Article | PFE/PFD | All colors

           So look along the header row first, and take the leftmost cell that
           is not a label the scanner recognises. */
        foreach (var raw in grid[headerRow])
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            var v = raw.Trim();

            /* A column label is short - "Qty", "Article", "USD per meter / FOB".
               A title is prose: "Quoted Price by K. Sivy line on 01.10.2018".
               Length is what separates them, and it has to, because the title
               contains the word "price" and would otherwise be mistaken for a
               money label and skipped. */
            bool looksLikeALabel =
                v.Length <= 25 &&
                (Vocab.Of(v) != Role.None || Vocab.Tier(v) != null
                 || Vocab2.IsMoneyLabel(v) || Vocab2.IsQtyHeader(v, out _, out _));
            if (looksLikeALabel) continue;
            if (v.Length <= 25 && Vocab.Of(v) != Role.None) continue;

            return Trim300(v);
        }

        /* Otherwise the line above, as long as it is not DATA - a row carrying
           an article is the previous table's last line, not this one's title. */
        for (int r = headerRow - 1; r >= 0 && r >= headerRow - 3; r--)
        {
            if (!IsHeaderContinuation(grid[r])) break;      // hit data: stop
            var joined = string.Join(" ", grid[r].Where(v => !string.IsNullOrWhiteSpace(v))).Trim();
            if (joined.Length == 0) continue;
            return Trim300(joined);
        }
        return null;
    }

    static string Trim300(string s) => s.Length > 300 ? s.Substring(0, 300) : s;

    /* A header row may be continued on the row below - names on one line,
       units and tier labels on the next. A DATA row never is. The difference
       is an article: a continuation carries labels, not design numbers. */
    static bool IsHeaderContinuation(string[] row)
    {
        int pricey = 0;
        foreach (var v in row)
        {
            if (string.IsNullOrWhiteSpace(v)) continue;
            if (Parse.Article(v) != null) return false;
            if (Parse.Price(v, out _, out _)) pricey++;
        }

        /* A header carries labels, not figures. Testing only for an article
           was not enough: a data row whose design cell is blank - because the
           design carries down from the row above - looked like a header.

           On ANITA that row read "2,000 m. +  $ 3.90", and "2,000 m. +" was
           taken for a column of quantity bands, so the "Qty per color" column
           became a price column and every band under it was lost. Two figures
           on a row is enough to say it is data. */
        return pricey < 2;
    }

    /* The first row below a header that is actually DATA. A header can be two
       or three rows deep, and sampling the row immediately below it reads the
       units line - "CM.", "THB/KG", "THB/M" - as if it were data. On Ausco
       that made every price column look like a column of words, and the sheet
       fell from 170 lines to 20. */
    static int FirstDataRow(string[][] grid, int from)
    {
        int r = from;
        while (r < grid.Length && IsHeaderContinuation(grid[r])
               && grid[r].Any(v => !string.IsNullOrWhiteSpace(v))) r++;
        return r;
    }

    /* Do the cells under this header look like money, or like tier names?
       Used only to settle a column headed "Color", which different sheets use
       for both. Looks at a handful of rows, not the whole table. */
    static bool LooksNumeric(string[][] grid, int from, int c)
    {
        int seen = 0, numeric = 0;
        for (int r = FirstDataRow(grid, from); r < grid.Length && seen < 6; r++)
        {
            var v = grid[r][c];
            if (string.IsNullOrWhiteSpace(v)) continue;
            seen++;
            if (Parse.Price(v, out _, out _)) numeric++;
        }
        return seen > 0 && numeric * 2 > seen;
    }

    /* A column of WORDS - "White & Black", "Other colour".

       Words, not merely "not a number": a price column often carries "-" or
       "n/a" where there is no price, and treating those as prose demoted real
       tier columns and lost ten designs. So a cell only counts as text when it
       holds at least three letters. */
    static bool LooksTexty(string[][] grid, int from, int c)
    {
        int wordy = 0, priced = 0;
        int seen = 0;
        for (int r = FirstDataRow(grid, from); r < grid.Length && seen < 8; r++)
        {
            var v = grid[r][c];
            if (string.IsNullOrWhiteSpace(v)) continue;
            seen++;
            if (Parse.Price(v, out _, out _)) { priced++; continue; }
            if (v.Count(char.IsLetter) >= 3) wordy++;
        }
        /* One real price anywhere in the column and it stays a price column.
           A looser test demoted YEH Pattana's "All color" column - which holds
           "THB 175/meter" - and with no price column left the header stopped
           being recognised at all, so two quotes were silently attributed to
           whatever design was carried down from the table above. */
        return priced == 0 && wordy >= 2;
    }

    static bool LooksTierNames(string[][] grid, int from, int c)
    {
        int seen = 0, tiers = 0;
        for (int r = FirstDataRow(grid, from); r < grid.Length && seen < 6; r++)
        {
            var v = grid[r][c];
            if (string.IsNullOrWhiteSpace(v)) continue;
            seen++;
            if (Vocab.Tier(v) != null) tiers++;
        }
        return seen > 0 && tiers * 2 > seen;
    }

    /* "USD per meter / FOB" and "THB per meter exclude Vat/ DDP" sit ABOVE the
       tier labels and span several columns. Look up a few rows, and leftwards,
       because a merged group heading is written only in its first column. */
    static string GroupCurrency(string[][] grid, int headerRow, int col, int cols)
    {
        for (int r = headerRow - 1; r >= Math.Max(0, headerRow - 3); r--)
            for (int c = col; c >= 0; c--)
            {
                var v = grid[r][c];
                if (string.IsNullOrWhiteSpace(v)) continue;
                var cur = Vocab.Currency(v);
                if (cur != null) return cur;
                break;   // a non-empty cell that says nothing about currency
            }
        return null;
    }

    public static List<Line> Sheet(IXLWorksheet ws, List<string> problems)
    {
        var used = ws.RangeUsed();
        var outp = new List<Line>();
        if (used is null) return outp;

        int r0 = used.FirstRow().RowNumber(), r1 = used.LastRow().RowNumber();
        int c0 = used.FirstColumn().ColumnNumber(), c1 = used.LastColumn().ColumnNumber();
        int rows = r1 - r0 + 1, cols = c1 - c0 + 1;

        var grid = new string[rows][];
        for (int i = 0; i < rows; i++)
        {
            grid[i] = new string[cols];
            for (int j = 0; j < cols; j++)
            {
                var cell = ws.Cell(r0 + i, c0 + j);
                if (cell.IsMerged()) cell = cell.MergedRange().FirstCell();
                grid[i][j] = Parse.Clean(cell.IsEmpty() ? "" : cell.GetFormattedString());
            }
        }

        /* Currency words anywhere on the sheet, as a last resort - and ONLY when
           the sheet speaks with one voice. Inter-Spitzen prices fabric at 1.05
           and 0.98 a metre, plainly dollars, but carries a stray "THB" far from
           any table; taking the first word found labelled 130 lines wrongly.
           Where a sheet mentions both, nothing on it is decided this way. */
        var hints = new List<string>();
        foreach (var row in grid)
            foreach (var v in row)
            {
                if (string.IsNullOrWhiteSpace(v) || v.Length > 60) continue;
                var cur = Vocab.Currency(v);
                if (cur != null && !hints.Contains(cur)) hints.Add(cur);
            }

        /* The sheet's NAME outranks stray words inside it. "CENTER PRICE THB"
           holds 578 lines at 185, 160, 60 a metre - baht, and it says so in the
           tab. Only when the name is silent does a single currency word found
           on the sheet stand in for it. */
        var named = Vocab.Currency(ws.Name);
        var sheetVoice = named != null ? new[] { named }
                       : hints.Count == 1 ? hints.ToArray()
                       : System.Array.Empty<string>();

        Table t = null;
        string art = null, fab = null, comp = null, fw = null, uw = null,
               wt = null, moq = null, dt = null;
        int blanks = 0;

        for (int i = 0; i < rows; i++)
        {
            var maybe = ReadHeader(grid, i, cols, sheetVoice);
            if (maybe != null)
            {
                t = maybe;
                /* What this block of prices is. The line above a header is
                   usually the sheet saying where the quote came from:

                       Quoted Price by K. Sivy line on 01.10.2018
                       Qty | Article | PFE/PFD | All colors
                       200-600m. | 255028 | | 6.17

                   ANITA holds two quotes for 255028, one from 2018 and one
                   from 2022, with prices that partly agree. Without this the
                   two are indistinguishable in the grid and nobody can say
                   which to keep. */
                t.BlockNote = BlockTitle(grid, maybe.HeaderRow);
                // skip the second header row, so its unit labels are not
                // mistaken for data
                i += t.Depth - 1;
                // a new table starts fresh - nothing carries across its header
                art = fab = comp = fw = uw = wt = moq = dt = null;
                blanks = 0;
                continue;
            }
            if (t == null) continue;

            bool empty = grid[i].All(string.IsNullOrWhiteSpace);
            if (empty) { if (++blanks >= 6) t = null; continue; }
            blanks = 0;

            // carry the line's own fields forward: a qty tier row leaves them blank
            foreach (var (c, role) in t.Roles)
            {
                var v = grid[i][c];
                if (string.IsNullOrWhiteSpace(v)) continue;
                switch (role)
                {
                    case Role.Article:     { var a = Parse.Article(v); if (a != null) art = a; break; }
                    case Role.Fabric:      fab = v; break;
                    case Role.Composition: comp = v; break;
                    case Role.FullWidth:   fw = v; break;
                    case Role.UsableWidth: uw = v; break;
                    case Role.Weight:      wt = v; break;
                    case Role.Moq:         moq = v; break;
                    case Role.Date:        dt = v; break;
                }
            }

            if (art == null) continue;

            // the quantity band for this row, else the one carried down
            string qtyRaw = null;
            foreach (var (c, role) in t.Roles)
                if (role == Role.QtyTier && !string.IsNullOrWhiteSpace(grid[i][c]))
                    qtyRaw = grid[i][c];

            string remark = null;
            foreach (var (c, role) in t.Roles)
                if (role == Role.Remark && !string.IsNullOrWhiteSpace(grid[i][c]))
                    remark = grid[i][c];

            /* The colour tier can be a value ON the row rather than the name of
               a column - "color Type" holding PFD, White, Color/Black against
               one article. Where it is, it wins for every price on that row. */
            string rowTier = null;
            foreach (var (c, role) in t.Roles)
                if (role == Role.TierValue && !string.IsNullOrWhiteSpace(grid[i][c]))
                    rowTier = Vocab.Tier(grid[i][c]);

            /* The quantities on this row, so a quantity cannot be read as a
               price further along it. */
            Parse.QtyBand(qtyRaw ?? "", out var qmin0, out var qmax0);
            decimal moqNumber = 0;
            if (!string.IsNullOrWhiteSpace(moq) && Parse.QtyBand(moq, out var mq, out _))
                moqNumber = mq;

            foreach (var (c, colTier) in t.TierOf)
            {
                /* colTier null means the column did not name one: either the
                   bands run across the top, or the column is labelled after the
                   money. Take the row's tier, and fall back to All_colors -
                   a single unlabelled price is a price for any colour. */
                var tier = rowTier ?? colTier ?? "All_colors";

                /* A cell that restates its own column's band is a repeated
                   HEADER, not a price. Sheets that put the bands across the top
                   reprint that row every few articles - STG does it three times
                   for one - and "200 m" under the 200 column was coming through
                   as a price of 200.

                   Narrow on purpose: only in a band-as-column table, and only
                   when the cell names that very band. Testing the cell alone
                   was too broad and threw away real greige prices quoted per
                   kilo - 415, 430, 450 - because "kg" reads as a quantity unit
                   too. Those are prices, and they are back. */
                /* "3,000 m." is a quantity wherever it appears, and a merged
                   "USD per meter / FOB" heading spans columns that hold no
                   prices - the MOQ under one came through as a price of 3000
                   on ANITA's 2022 quote. */
                if (Vocab3.IsPlainQuantity(grid[i][c])) continue;

                if (t.QtyOfColumn.TryGetValue(c, out var ownBand)
                    && Vocab2.IsQtyHeader(grid[i][c], out var cellMin, out _)
                    && cellMin == ownBand.Min)
                    continue;

                if (!Parse.Price(grid[i][c], out var price, out var curCell)) continue;

                /* A figure that IS the row's quantity is a quantity, not a
                   price. "3,000m." in a MOQ column came through as a price of
                   3000 where a column was read loosely. Fabric is not sold at
                   three thousand dollars a metre. */
                if (price == qmin0 || (qmax0.HasValue && price == qmax0.Value)
                    || price == moqNumber) continue;

                var currency = curCell ?? t.CurrencyOf.GetValueOrDefault(c) ?? "USD";

                int qmin; int? qmax;
                if (t.QtyOfColumn.TryGetValue(c, out var band))
                { qmin = band.Min; qmax = band.Max; }
                else
                    Parse.QtyBand(qtyRaw ?? moq ?? "", out qmin, out qmax);

                outp.Add(new Line {
                    Sheet = ws.Name, Row = r0 + i, HeaderRow = r0 + t.HeaderRow,
                    Article = art, Fabric = fab, Composition = comp,
                    FullWidth = fw, UsableWidth = uw, Weight = wt, Moq = moq,
                    QtyRaw = qtyRaw, QtyMin = qmin, QtyMax = qmax,
                    Tier = tier, Currency = currency, Price = price, Col = c,
                    CurrencyStated = curCell != null || t.StatedCurrency.ContainsKey(c),
                    Remark = remark, DateRaw = dt, BlockNote = t.BlockNote
                });
            }
        }
        return outp;
    }
}

static partial class ScanHelpers { }
