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

    /* A code with a second code after it in brackets: "254990 (#040047)".
       ANITA writes the supplier's own reference that way when a design is
       re-quoted. Both halves matter, and they are not the same thing: the
       sheet's full text is the ARTICLE, and the part in front of the bracket
       is the DESIGN NUMBER the rest of the system knows the fabric by.

       Before this the whole cell failed to parse as an article at all, so
       every row of the revised table was skipped - the table was found, and
       then discarded line by line for having no article. */
    static readonly Regex BracketedShape = new(
        @"^([A-Za-z0-9][A-Za-z0-9\-/._]{2,29})\s*\(\s*([^)]{1,25}?)\s*\)$",
        RegexOptions.Compiled);

    public static string Article(string raw)
    {
        var s = (raw ?? "").Trim().TrimEnd('.', ',');
        if (s.Length < 3 || s.Length > 56) return null;
        if (!ArticleShape.IsMatch(s) && !BracketedShape.IsMatch(s)) return null;
        if (s.Count(char.IsDigit) < 3) return null;
        /* "25-30" and "155-165" are a weight and a width, not articles. Two
           short numbers around a dash is a RANGE - real codes here are six
           digits, or carry letters ("257373AA/11", "W7LF001-1A/14"). This
           catches a spec column being read as the article when a table's
           header was misread. */
        if (Regex.IsMatch(s, @"^\d{1,4}\s*-\s*\d{1,4}$")) return null;
        return s;
    }

    /* The design number inside an article. Everything that is not a bracketed
       pair is already its own design number and comes back unchanged. */
    public static string DesignNo(string article)
    {
        if (string.IsNullOrWhiteSpace(article)) return article;
        var m = BracketedShape.Match(article.Trim());
        return m.Success ? m.Groups[1].Value : article.Trim();
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

        /* A price is very often written with its unit after it - "$ 2.11 /m.",
           "USD 1.15/m", "THB 47.-/M." - and stripping the letters leaves the
           unit's full stop behind: "2.11." reads as two decimal points and was
           thrown out as malformed. Chief You is written entirely that way and
           yielded NOTHING; so did most of Crystal Martin and Hanes Global.

           Trimming the stops at the ends keeps "1.2.3" rejected, which is the
           case the test was actually for. */
        cleaned = cleaned.Trim('.');

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

    /* One cell carrying both the band and the price:

           200-599  m = 4.90/ m
           600-1999 m = 4.55/ m
           2,000 +    = 4.40/ m

       Hanes Global writes its whole price column that way, in a column headed
       "PRICE of REF in linear meter". Split at the "=", read the band from the
       left and the price from the right.

       The two halves are checked against each other: the left must parse as a
       quantity and the right as a price, and the right must NOT itself look
       like a quantity. Without that, "200-599 m = 600-999 m" - a cross
       reference, not a price - would come through as a price of 600. */
    public static bool BandEqualsPrice(string raw, out int qmin, out int? qmax,
                                       out decimal value, out string currencyFromCell)
    {
        qmin = 0; qmax = null; value = 0; currencyFromCell = null;

        var s = (raw ?? "").Trim();
        int eq = s.IndexOf('=');
        if (eq <= 0 || eq == s.Length - 1) return false;

        var left = s.Substring(0, eq).Trim();
        var right = s.Substring(eq + 1).Trim();
        if (left.Length == 0 || right.Length == 0) return false;

        if (!QtyBand(left, out qmin, out qmax)) return false;
        if (Vocab3.IsPlainQuantity(right)) return false;
        if (!Price(right, out value, out currencyFromCell)) return false;

        return true;
    }

    public static string Clean(string raw)
    {
        var s = (raw ?? "").Replace("\n", " ").Replace("\r", " ").Trim();
        return Regex.Replace(s, @"\s{2,}", " ");
    }
}
