/*  What the scanner decided about a sheet, and why.

    Two questions cost more time than anything else when a price does not come
    through:

        which tables did it find?          explain <file> <sheet>
        why is THIS row not a header?      explain <file> <sheet> <row>

    Both were being answered by inference from the emitted data, which took
    several rounds each. They are one command now. */
using ClosedXML.Excel;

static class Explain
{
    public static int Run(string wbPath, string sheetName, int probeRow = 0)
    {
        using var wb = new XLWorkbook(wbPath);
        var ws = wb.Worksheets.FirstOrDefault(w => w.Name == sheetName)
              ?? wb.Worksheets.FirstOrDefault(w => w.Name.Trim() == sheetName.Trim());
        if (ws == null) { Console.Error.WriteLine($"no sheet '{sheetName}'"); return 2; }

        if (probeRow > 0) { Probe(ws, probeRow); return 0; }

        var problems = new List<string>();
        var lines = Scan.Sheet(ws, problems);

        foreach (var hr in lines.Select(l => l.HeaderRow).Distinct().OrderBy(x => x))
        {
            var sample = lines.First(l => l.HeaderRow == hr);
            var cols = lines.Where(l => l.HeaderRow == hr)
                            .Select(l => l.Col).Distinct().OrderBy(c => c);
            Console.WriteLine($"=== header row {hr} ===");
            Console.WriteLine($"  price columns: {string.Join(", ", cols)}");
            Console.WriteLine($"  first line: design={sample.Article} qty='{sample.QtyRaw}' " +
                              $"({sample.QtyMin}-{(sample.QtyMax?.ToString() ?? "open")}) " +
                              $"tier={sample.Tier} {sample.Currency} {sample.Price}");
        }
        foreach (var p in problems) Console.Error.WriteLine(p);
        return 0;
    }

    /* Build the grid exactly as Scan.Sheet does, then report the decision for
       one candidate header row, column by column. */
    static void Probe(IXLWorksheet ws, int excelRow)
    {
        var used = ws.RangeUsed();
        if (used is null) { Console.WriteLine("sheet is empty"); return; }

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

        int r = excelRow - r0;
        if (r < 0 || r >= rows) { Console.WriteLine("row is outside the used range"); return; }

        Console.WriteLine($"row {excelRow} (grid {r}); used range starts row {r0}, col {c0}");
        Console.WriteLine($"  row below is a header continuation : " +
            (r + 1 < rows ? Scan.IsHeaderContinuation(grid[r + 1]).ToString() : "n/a"));
        Console.WriteLine($"  two rows below                     : " +
            (r + 2 < rows ? Scan.IsHeaderContinuation(grid[r + 2]).ToString() : "n/a"));

        for (int c = 0; c < cols; c++)
        {
            var top = grid[r][c];
            if (string.IsNullOrWhiteSpace(top)) continue;
            Console.WriteLine($"  col{c} [{top}]  role={Vocab.Of(top)}  tier={Vocab.Tier(top) ?? "-"}" +
                              $"  money={Vocab2.IsMoneyLabel(top)}" +
                              $"  qtyhdr={Vocab2.IsQtyHeader(top, out _, out _)}");
        }

        var t = Scan.ReadHeader(grid, r, cols, new string[0]);
        Console.WriteLine(t == null
            ? "  => NOT accepted as a header"
            : $"  => accepted, depth {t.Depth}, price columns " +
              string.Join(",", t.TierOf.Keys.OrderBy(x => x)));
    }
}
