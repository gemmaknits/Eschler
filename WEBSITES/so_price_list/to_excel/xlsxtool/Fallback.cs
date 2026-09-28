/*  Rows that belong to no table.

    A few sheets quote without a header at all. Mas Intimates writes the design
    on a line of its own and the bands underneath it:

        255102              MOQ, 3,000m
                            200-599m        $4.35/m
                            600-2,000m      $ 4.05/m
                            2,001-5,000m    $3.85/m

    There is nothing here for ReadHeader to recognise - no labels, no column
    names - so the whole block was silent. This picks those rows up afterwards,
    and ONLY those: a row the main scan already read is never revisited.

    Kept deliberately tight, because a rule this loose is how invented prices
    get in. A row qualifies only when it is unmistakable:

      - the main scan produced nothing for it
      - exactly ONE cell holds money
      - exactly ONE other cell is a plain quantity band
      - a design was stated on its own line not far above

    Anything less clear is left for uncaptured_money.csv and a person. */

static class Fallback
{
    public const int LookBack = 15;

    public static List<Line> Rows(string[][] grid, int rows, int cols, int r0,
                                  HashSet<int> covered, string[] sheetVoice, string sheetName)
    {
        var outp = new List<Line>();
        string design = null;
        int designRow = -99;

        for (int i = 0; i < rows; i++)
        {
            /* A line of its own carrying a design and no money: the heading for
               the rows that follow. */
            string loneDesign = null;
            int moneyCells = 0, bandCells = 0, moneyCol = -1, bandCol = -1;
            decimal price = 0; string curCell = null;
            int qmin = 0; int? qmax = null;

            for (int c = 0; c < cols; c++)
            {
                var v = grid[i][c];
                if (string.IsNullOrWhiteSpace(v)) continue;

                var a = Parse.Article(v);
                if (a != null && loneDesign == null) loneDesign = a;

                if (Parse.Price(v, out var p, out var cur)
                    && (cur != null || v.Contains('.')))
                { moneyCells++; moneyCol = c; price = p; curCell = cur; continue; }

                if (Vocab3.IsPlainQuantity(v) || Parse.QtyBand(v, out _, out _))
                {
                    if (Parse.QtyBand(v, out var qn, out var qx))
                    { bandCells++; bandCol = c; qmin = qn; qmax = qx; }
                }
            }

            if (loneDesign != null && moneyCells == 0)
            {
                design = loneDesign; designRow = i;      // the block's heading
                continue;
            }

            if (covered.Contains(i)) continue;
            if (design == null || i - designRow > LookBack) continue;
            if (moneyCells != 1 || bandCells != 1) continue;
            if (moneyCol == bandCol) continue;
            if (price == qmin || (qmax.HasValue && price == qmax.Value)) continue;

            outp.Add(new Line {
                Sheet = sheetName, Row = r0 + i, HeaderRow = r0 + designRow,
                /* Set here as well as in Scan: this is the second place a Line
                   is built, and leaving it out left 57 headerless lines with a
                   blank design number - they loaded, and showed as rows with
                   no design at all. */
                Article = design, DesignNo = Parse.DesignNo(design),
                QtyMin = qmin, QtyMax = qmax,
                QtyRaw = grid[i][bandCol],
                Tier = "All_colors",
                Currency = curCell ?? sheetVoice.FirstOrDefault() ?? "USD",
                CurrencyStated = curCell != null,
                Price = price, Col = moneyCol,
                BlockNote = "read without a header - design taken from the line above"
            });
        }
        return outp;
    }
}
