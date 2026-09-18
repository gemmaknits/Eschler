/*  Did we miss any money?

    The scanner reports what it found; this reports what it did NOT. For every
    sheet it walks the cells and counts the ones that unambiguously hold money -
    a currency symbol or the word USD/THB/baht next to a number - and were not
    turned into a price line.

    Deliberately narrow. A bare "150" could be a width, a weight or a price and
    counting those would drown the answer in noise; a cell saying "$ 2.95" could
    not be anything else. So this under-reports on purpose: anything it does
    flag is worth looking at. */
using ClosedXML.Excel;
using System.Text.RegularExpressions;

static class Coverage
{
    static readonly Regex Money = new(
        @"(\$|USD|US\$|THB|baht|฿)\s*\d|(\d[\d,]*(\.\d+)?)\s*(USD|THB|baht|฿)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static int Run(string wbPath, string outCsv = null)
    {
        var missedRows = new List<string>();
        using var wb = new XLWorkbook(wbPath);
        var problems = new List<string>();
        Console.WriteLine("sheet\tmoney_cells\tcaptured\tmissed\tmissed_examples");

        foreach (var ws in wb.Worksheets)
        {
            List<Line> lines;
            try { lines = Scan.Sheet(ws, problems); }
            catch { continue; }

            /* Every (row, column) the scan turned into a price. */
            var got = new HashSet<(int, int)>(lines.Select(l => (l.Row, l.Col)));

            var used = ws.RangeUsed();
            if (used == null) continue;
            int r0 = used.FirstRow().RowNumber(), r1 = used.LastRow().RowNumber();
            int c0 = used.FirstColumn().ColumnNumber(), c1 = used.LastColumn().ColumnNumber();

            int money = 0, captured = 0;
            var examples = new List<string>();

            for (int r = r0; r <= r1; r++)
                for (int c = c0; c <= c1; c++)
                {
                    var v = ws.Cell(r, c).GetFormattedString();
                    if (string.IsNullOrWhiteSpace(v) || !Money.IsMatch(v)) continue;
                    /* a column HEADING that names a currency is not a price */
                    if (Vocab2.IsMoneyLabel(v) && !Regex.IsMatch(v, @"\d")) continue;
                    money++;
                    if (got.Contains((r, c - c0))) { captured++; continue; }

                    /* Every uncaptured one goes to the report, not just the
                       three shown here - a person has to be able to look at
                       the whole list and decide. */
                    var flat = v.Replace("\"", "\"\"").Replace("\t", " ")
                                .Replace("\n", " ").Replace("\r", " ").Trim();

                    /* Not read as a price - but is it at least VISIBLE?

                       A price inside a sentence keeps its words: they are
                       attached to the nearby prices as a note, so the reviewer
                       already sees the term in the grid. Only the rest of this
                       list needs a person to go and look at the workbook. */
                    var probe = flat.Length > 40 ? flat.Substring(0, 40) : flat;
                    var kept = flat.Length >= 12
                               && lines.Any(l => l.BlockNote != null && l.BlockNote.Contains(probe));

                    missedRows.Add($"\"{ws.Name.Replace("\"", "\"\"")}\",{r},{c}," +
                                   $"{(kept ? "yes" : "no")},\"{flat}\"");
                    if (examples.Count < 3) examples.Add($"r{r}:{flat}");
                }

            if (money == 0) continue;
            Console.WriteLine($"{ws.Name}\t{money}\t{captured}\t{money - captured}\t{string.Join(" | ", examples)}");
        }
        if (outCsv != null)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("sheet,source_row,col,kept_as_note,cell_text");
            foreach (var m in missedRows) sb.AppendLine(m);
            File.WriteAllText(outCsv, sb.ToString(), new System.Text.UTF8Encoding(true));
            Console.Error.WriteLine($"uncaptured money cells written: {missedRows.Count}");
        }
        return 0;
    }
}
