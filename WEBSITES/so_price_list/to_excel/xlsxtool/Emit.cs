using ClosedXML.Excel;
using System.Text;

static class Emit
{
    static string Q(string s)
    {
        s ??= "";
        return s.Contains(',') || s.Contains('"') || s.Contains('\n')
            ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
    }

    public static int Run(string path, string outCsv, string only)
    {
        using var wb = new XLWorkbook(path);
        var problems = new List<string>();
        var all = new List<Line>();

        foreach (var ws in wb.Worksheets)
        {
            if (!string.IsNullOrEmpty(only) &&
                !ws.Name.Equals(only, StringComparison.OrdinalIgnoreCase)) continue;
            try {
                var got = Scan.Sheet(ws, problems);
                // settle USD vs THB from the figures before anything is written
                var fixedUp = Money.Fix(got);
                if (fixedUp > 0) problems.Add($"{ws.Name}\tcurrency re-read from values on {fixedUp} lines");
                all.AddRange(got);
            }
            catch (Exception ex) { problems.Add($"{ws.Name}\tFAILED\t{ex.Message}"); }
        }

        var sb = new StringBuilder();
        sb.AppendLine("sheet,source_row,header_row,article,fabric_name,composition," +
                      "full_width_cm,usable_width_cm,weight_gsm,moq,qty_raw,qty_min,qty_max," +
                      "color_tier,currency,price,date_raw,remark,block_note");
        foreach (var l in all)
            sb.AppendLine(string.Join(",", new[] {
                Q(l.Sheet), l.Row.ToString(), l.HeaderRow.ToString(), Q(l.Article),
                Q(l.Fabric), Q(l.Composition), Q(l.FullWidth), Q(l.UsableWidth),
                Q(l.Weight), Q(l.Moq), Q(l.QtyRaw), l.QtyMin.ToString(),
                l.QtyMax?.ToString() ?? "", Q(l.Tier), Q(l.Currency),
                l.Price.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Q(l.DateRaw), Q(l.Remark), Q(l.BlockNote)
            }));
        File.WriteAllText(outCsv, sb.ToString(), new UTF8Encoding(true));

        Console.Error.WriteLine($"lines: {all.Count}");
        Console.Error.WriteLine($"sheets with lines: {all.Select(a => a.Sheet).Distinct().Count()}");
        Console.Error.WriteLine($"distinct articles: {all.Select(a => a.Article).Distinct().Count()}");
        foreach (var p in problems) Console.Error.WriteLine(p);
        return 0;
    }
}
