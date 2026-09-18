/*  Survey mode: what do the header rows of these sheets actually say?

    The workbook has no single table shape - each sheet holds several tables,
    each with its own header, and the column order changes between them. So the
    parser cannot be written against an assumed layout; it has to be written
    against the vocabulary the sheets really use, which is what this collects. */
using ClosedXML.Excel;

static class Headers
{
    // A row is a header if it carries several of these words. Deliberately
    // loose - the point is to SEE the variety, not to judge it yet.
    static readonly string[] Hints = {
        "article", "design", "qty", "moq", "composition", "compo", "fabric",
        "width", "weight", "price", "pfe", "pfd", "all color", "all colour",
        "usd", "thb", "date", "remark", "gsm", "g/m2", "per color", "per meter"
    };

    public static int Run(string path)
    {
        using var wb = new XLWorkbook(path);
        foreach (var ws in wb.Worksheets)
        {
            var used = ws.RangeUsed();
            if (used is null) continue;
            int r0 = used.FirstRow().RowNumber(), r1 = used.LastRow().RowNumber();
            int c0 = used.FirstColumn().ColumnNumber(), c1 = used.LastColumn().ColumnNumber();

            for (int r = r0; r <= r1; r++)
            {
                var vals = new List<string>();
                for (int c = c0; c <= c1; c++)
                {
                    var cell = ws.Cell(r, c);
                    if (cell.IsMerged()) cell = cell.MergedRange().FirstCell();
                    var v = cell.IsEmpty() ? "" : cell.GetFormattedString();
                    vals.Add((v ?? "").Replace("\t"," ").Replace("\n"," ").Trim());
                }
                int hits = vals.Count(v => v.Length > 0 && v.Length < 40 &&
                    Hints.Any(h => v.ToLowerInvariant().Contains(h)));
                if (hits >= 3)
                    Console.WriteLine($"{ws.Name}\t{r}\t{hits}\t{string.Join(" | ", vals.Where(v => v.Length > 0))}");
            }
        }
        return 0;
    }
}
