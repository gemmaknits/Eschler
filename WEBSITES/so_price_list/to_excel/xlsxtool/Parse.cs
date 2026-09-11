/*  Reading the values the sheets actually hold.

    Every rule here comes from a form seen in the workbook, not from a guess:
      qty   "200-599m."  "601 - 2,000 m."  "5,000m.+"  "2,000 m."
            "5,000 m. UP * 2 shipments"  "1,000 m."
      price "$ 3.20"  "$2.40"  "THB 216.00"  "6.16666666666667"  "-"
      art   "255167"  "257373AA/11"  "W7LF001-1A/14"
            and NOT "same as Anita price", "Original sample", "(VERSION PES OF 257206)"
*/
using System.Globalization;
using System.Text.RegularExpressions;

static class Parse
{
    /* An article is a code, not a sentence. It must start alphanumeric, carry
       at least three digits, and hold no spaces - which is what separates
       "257373AA/11" from "same as Anita price" and "Original sample". Those
       sentences are real cells in the sheet and were being read as articles. */
    static readonly Regex ArticleShape = new(@"^[A-Za-z0-9][A-Za-z0-9\-/._]{2,29}$", RegexOptions.Compiled);

    public static string Article(string raw)
    {
        var s = (raw ?? "").Trim().TrimEnd('.', ',');
        if (s.Length < 3 || s.Length > 30) return null;
        if (!ArticleShape.IsMatch(s)) return null;
        if (s.Count(char.IsDigit) < 3) return null;
        /* "25-30" and "155-165" are a weight and a width, not articles. Two
           short numbers around a dash is a RANGE - real codes here are six
           digits, or carry letters ("257373AA/11", "W7LF001-1A/14"). This
           catches a spec column being read as the article when a table's
           header was misread. */
        if (Regex.IsMatch(s, @"^\d{1,4}\s*-\s*\d{1,4}$")) return null;
        return s;
    }

    /* A quantity band. A single figure is a MINIMUM ("2,000 m." means 2,000 and
       up), a range is both ends, and a trailing "+" or "UP" is open-ended.
       Trailing prose ("UP * 2 shipments") is ignored, not treated as a failure. */
    public static bool QtyBand(string raw, out int min, out int? max)
    {
        min = 0; max = null;
        var s = (raw ?? "").Trim();
        if (s.Length == 0) return false;

        var nums = Regex.Matches(s.Replace(",", ""), @"\d+")
                        .Select(m => m.Value).ToList();
        if (nums.Count == 0) return false;

        bool openEnded = s.Contains('+') || Regex.IsMatch(s, @"\bup\b", RegexOptions.IgnoreCase);
        // a range needs a separator BETWEEN two numbers, not just two numbers
        bool isRange = nums.Count >= 2 && Regex.IsMatch(
            s.Replace(",", ""), @"\d+\s*(?:-|–|—|to)\s*\d+", RegexOptions.IgnoreCase);

        if (!int.TryParse(nums[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out min)) return false;
        if (isRange && !openEnded)
        {
            var m2 = Regex.Match(s.Replace(",", ""), @"(\d+)\s*(?:-|–|—|to)\s*(\d+)", RegexOptions.IgnoreCase);
            if (m2.Success && int.TryParse(m2.Groups[1].Value, out var lo)
                           && int.TryParse(m2.Groups[2].Value, out var hi))
            { min = lo; max = hi; }
        }
        return min >= 0;
    }

    /* A price, with whatever currency marker was typed against it. Returns the
       currency only when the CELL says so - the column header decides otherwise. */
    public static bool Price(string raw, out decimal value, out string currencyFromCell)
    {
        value = 0; currencyFromCell = null;
        var s = (raw ?? "").Trim();
        if (s.Length == 0) return false;
        if (s is "-" or "–" or "—" or "n/a" or "N/A" or "x" or "X") return false;

        if (Regex.IsMatch(s, @"thb|baht", RegexOptions.IgnoreCase)) currencyFromCell = "THB";
        else if (s.Contains('$') || Regex.IsMatch(s, @"\busd\b", RegexOptions.IgnoreCase)) currencyFromCell = "USD";

        var cleaned = Regex.Replace(s, @"[^\d.]", "");
        if (cleaned.Length == 0) return false;
        // "1.2.3" is not a price
        if (cleaned.Count(ch => ch == '.') > 1) return false;
        if (!decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out value)) return false;

        // A price of 0 carries no information, and the sheets use it as a blank.
        if (value <= 0) return false;
        // Guard against reading a width or a weight as a price.
        if (value > 100000) return false;
        return true;
    }

    public static string Clean(string raw)
    {
        var s = (raw ?? "").Replace("\n", " ").Replace("\r", " ").Trim();
        return Regex.Replace(s, @"\s{2,}", " ");
    }
}
