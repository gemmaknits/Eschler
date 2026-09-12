/*  xlsxtool  -  read the price-book workbook.

    "dump <file> <sheet>"   every non-empty cell of one sheet, as TSV
    "sheets <file>"         every sheet name, with its used range

    Reading only. The workbook is the source of truth and is never written. */
using ClosedXML.Excel;

if (args.Length < 2) { Console.Error.WriteLine("usage: xlsxtool sheets|dump <file> [sheet]"); return 1; }

var mode = args[0];
var path = args[1];

/* the vocabulary probe opens no workbook - it answers questions about labels */
if (mode == "vocab")
{
    for (int i = 1; i < args.Length; i++)
    {
        var okP = Parse.Price(args[i], out var pv, out var pc);
        var okB = Parse.BandEqualsPrice(args[i], out var bmin, out var bmax, out var bv, out _);
        var okQ = Parse.QtyBand(args[i], out var qn, out var qx);
        Console.WriteLine($"[{args[i]}] role={Vocab.Of(args[i])} tier={Vocab.Tier(args[i]) ?? "-"} money={Vocab2.IsMoneyLabel(args[i])}");
        Console.WriteLine($"    price={(okP ? pv.ToString() + " " + (pc ?? "") : "no")}  band={(okQ ? qn + "-" + (qx?.ToString() ?? "open") : "no")}  band=price={(okB ? bmin + "-" + (bmax?.ToString() ?? "open") + " @ " + bv : "no")}");
    }
    return 0;
}

using var wb = new XLWorkbook(path);

if (mode == "headers") return Headers.Run(path);

if (mode == "explain")
    return Explain.Run(args[1], args[2], args.Length > 3 ? int.Parse(args[3]) : 0);

if (mode == "coverage")
    return Coverage.Run(args[1], args.Length > 2 ? args[2] : null);

if (mode == "scan")
    return Emit.Run(path, args.Length > 2 ? args[2] : "scan.csv",
                          args.Length > 3 ? args[3] : null);

if (mode == "sheets")
{
    foreach (var ws in wb.Worksheets)
    {
        var r = ws.RangeUsed();
        Console.WriteLine($"{ws.Name}\t{(r == null ? 0 : r.RowCount())}\t{(r == null ? 0 : r.ColumnCount())}\t{ws.Visibility}");
    }
    return 0;
}

if (mode == "dump")
{
    if (args.Length < 3) { Console.Error.WriteLine("dump needs a sheet name"); return 1; }
    var ws = wb.Worksheets.FirstOrDefault(w =>
        string.Equals(w.Name, args[2], StringComparison.OrdinalIgnoreCase));
    if (ws is null) { Console.Error.WriteLine($"no sheet '{args[2]}'"); return 1; }

    var used = ws.RangeUsed();
    if (used is null) return 0;

    int firstRow = used.FirstRow().RowNumber(), lastRow = used.LastRow().RowNumber();
    int firstCol = used.FirstColumn().ColumnNumber(), lastCol = used.LastColumn().ColumnNumber();

    for (int r = firstRow; r <= lastRow; r++)
    {
        var cells = new List<string>();
        bool any = false;
        for (int c = firstCol; c <= lastCol; c++)
        {
            var cell = ws.Cell(r, c);
            // A merged cell carries its value only in the top-left; read that,
            // so a heading spanning four columns is not lost on three of them.
            if (cell.IsMerged()) cell = cell.MergedRange().FirstCell();
            var v = cell.IsEmpty() ? "" : cell.GetFormattedString();
            v = (v ?? "").Replace("\t", " ").Replace("\r", " ").Replace("\n", " ").Trim();
            if (v.Length > 0) any = true;
            cells.Add(v);
        }
        if (any) Console.WriteLine($"{r}\t{string.Join("\t", cells)}");
    }
    return 0;
}

Console.Error.WriteLine("unknown mode");
return 1;
