/*  What the scanner decided about one sheet.

    Prints every header it found and what it made of each column. Written
    because "the price is captured but the quantity is not" is impossible to
    reason about from the outside - the answer is always in this mapping, and
    guessing at it from the data costs more than printing it. */
using ClosedXML.Excel;

static class Explain
{
    public static int Run(string wbPath, string sheetName)
    {
        using var wb = new XLWorkbook(wbPath);
        var ws = wb.Worksheets.FirstOrDefault(w => w.Name == sheetName)
              ?? wb.Worksheets.FirstOrDefault(w => w.Name.Trim() == sheetName.Trim());
        if (ws == null) { Console.Error.WriteLine($"no sheet '{sheetName}'"); return 2; }

        var problems = new List<string>();
        var lines = Scan.Sheet(ws, problems);

        foreach (var hr in lines.Select(l => l.HeaderRow).Distinct().OrderBy(x => x))
        {
            var sample = lines.First(l => l.HeaderRow == hr);
            Console.WriteLine($"=== header row {hr} ===");
            var cols = lines.Where(l => l.HeaderRow == hr)
                            .Select(l => l.Col).Distinct().OrderBy(c => c);
            Console.WriteLine($"  price columns: {string.Join(", ", cols)}");
            Console.WriteLine($"  first line: design={sample.Article} qty='{sample.QtyRaw}' " +
                              $"({sample.QtyMin}-{(sample.QtyMax?.ToString() ?? "open")}) " +
                              $"tier={sample.Tier} {sample.Currency} {sample.Price}");
        }
        foreach (var p in problems) Console.Error.WriteLine(p);
        return 0;
    }
}
