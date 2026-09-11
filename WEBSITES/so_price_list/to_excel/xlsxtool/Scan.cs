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
                  Weight, Moq, QtyRaw, Tier, Currency, Remark, DateRaw;
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
        bool usedSecondRow = false;
        var below = (r + 1 < grid.Length) ? grid[r + 1] : new string[cols];

        for (int c = 0; c < cols; c++)
        {
            var top = grid[r][c];
            var bot = below[c];
            var roleTop = string.IsNullOrWhiteSpace(top) ? Role.None : Vocab.Of(top);
            var roleBot = string.IsNullOrWhiteSpace(bot) ? Role.None : Vocab.Of(bot);

            // the tier can only come from a label that names one
            var tier = Vocab.Tier(top) ?? Vocab.Tier(bot);
            var role = tier != null ? Role.Price
                     : roleTop != Role.None ? roleTop
                     : roleBot;
            if (role == Role.None) continue;
            if (role != roleTop && roleBot != Role.None) usedSecondRow = true;

            t.Roles[c] = role;

            if (role == Role.Price)
            {
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

        if (priced == 0 || identifying == 0) return null;
        t.Depth = usedSecondRow ? 2 : 1;
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
                // skip the second header row, so its unit labels are not
                // mistaken for data
                if (t.Depth == 2) i++;
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

            foreach (var (c, tier) in t.TierOf)
            {
                if (tier == null) continue;
                if (!Parse.Price(grid[i][c], out var price, out var curCell)) continue;

                var currency = curCell ?? t.CurrencyOf.GetValueOrDefault(c) ?? "USD";
                Parse.QtyBand(qtyRaw ?? moq ?? "", out var qmin, out var qmax);

                outp.Add(new Line {
                    Sheet = ws.Name, Row = r0 + i, HeaderRow = r0 + t.HeaderRow,
                    Article = art, Fabric = fab, Composition = comp,
                    FullWidth = fw, UsableWidth = uw, Weight = wt, Moq = moq,
                    QtyRaw = qtyRaw, QtyMin = qmin, QtyMax = qmax,
                    Tier = tier, Currency = currency, Price = price, Col = c,
                    CurrencyStated = curCell != null || t.StatedCurrency.ContainsKey(c),
                    Remark = remark, DateRaw = dt
                });
            }
        }
        return outp;
    }
}
