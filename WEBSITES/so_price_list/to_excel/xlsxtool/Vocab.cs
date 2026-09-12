/*  What the header words mean.

    Built from a survey of all 676 header rows in the workbook, not from an
    assumed layout - including the typos the sheets actually contain ("mcq" for
    MOQ on 84 rows, "compositon", "usable withd"). A label this does not know
    is reported rather than guessed at. */

enum Role { None, Article, Composition, Fabric, Moq, QtyTier, FullWidth,
            UsableWidth, Weight, Date, Remark, Price,
            /* a column whose CELLS name the colour tier, rather than a column
               that IS one tier - "color Type" holding PFD / White / Color-Black */
            TierValue }

static class Vocab
{
    static string Norm(string s) =>
        (s ?? "").ToLowerInvariant().Replace(" ", " ").Trim();

    /* The colour tier a price column is for, in the vocabulary the database
       already uses. Null means the label is not a price column at all. */
    public static string Tier(string label)
    {
        var s = Norm(label);
        if (s.Length == 0) return null;
        // strip a trailing unit note: "all colors (usd/m)" -> "all colors"
        var bare = System.Text.RegularExpressions.Regex
            .Replace(s, @"\s*\((?:usd|thb|baht)\s*/\s*(?:m|kg|mtr|meter)\.?\)\s*$", "").Trim();
        bare = bare.TrimEnd('.', ':').Trim();

        if (bare is "pfe/pfd" or "pfd/pfe" or "pfe / pfd" or "pfd / pfe") return "PFE/PFD";
        if (bare is "pfe") return "PFE";
        if (bare is "pfd") return "PFD";
        if (bare.StartsWith("all color")) return "All_colors";
        if (bare.StartsWith("greige")) return "Greige";
        if (bare is "white") return "White";
        if (bare is "black and white" or "black & white") return "All_colors";
        if (bare is "light") return "Light";
        if (bare is "medium") return "Medium";
        if (bare is "dark") return "Dark";
        if (bare is "color" or "colour" or "colors" or "colours") return "All_colors";
        return null;
    }

    /* USD or THB, when the label says so itself. */
    public static string Currency(string label)
    {
        var s = Norm(label);
        if (s.Length == 0) return null;
        if (s.Contains("thb") || s.Contains("baht")) return "THB";
        if (s.Contains("usd") || s.Contains("us$") || s.Contains("$")) return "USD";
        return null;
    }

    public static Role Of(string label)
    {
        var s = Norm(label).TrimEnd('.', ':').Trim();
        if (s.Length == 0 || s.Length > 40) return Role.None;

        if (s.Contains("article")) return Role.Article;   // "eschler article no.", "eth article"
        if (s is "product" or "product no" or "quality no") return Role.Article;
        if (s is "color type" or "colour type" or "color" or "colour"
         or "color/type" or "type of color") return Role.TierValue;

        if (s is "article" or "article #" or "article no" or "article number"
              or "design" or "design no" or "design #" or "art" or "art.") return Role.Article;
        if (s.StartsWith("article") || s.StartsWith("design no")) return Role.Article;

        if (s.StartsWith("composition") || s.StartsWith("compositon")
         || s == "compo" || s.StartsWith("compo ")) return Role.Composition;

        if (s.StartsWith("fabric")) return Role.Fabric;

        // "mcq" is a typo for MOQ that appears on 84 header rows
        if (s.StartsWith("moq") || s.StartsWith("mcq")) return Role.Moq;

        if (s.StartsWith("qty") || s.StartsWith("quantity")) return Role.QtyTier;

        if (s.StartsWith("full width") || s.StartsWith("full wth")) return Role.FullWidth;
        if (s.StartsWith("useable width") || s.StartsWith("usable width")
         || s.StartsWith("usable withd") || s.StartsWith("useable wth")
         || s.StartsWith("usable with")) return Role.UsableWidth;
        if (s == "width") return Role.FullWidth;

        if (s.StartsWith("weight")) return Role.Weight;
        if (s == "date") return Role.Date;
        if (s.StartsWith("remark")) return Role.Remark;

        return Tier(s) != null ? Role.Price : Role.None;
    }
}

static class Vocab2
{
    /* A price column need not name a colour tier. Plenty of tables carry one
       price and label it after the money instead: "USD/ m", "Price/M.",
       "New FOB Bangkok Price USD/m.", "Eschler price". Those were being read
       as no price column at all, which silenced whole sheets. */
    public static bool IsMoneyLabel(string label)
    {
        var s = (label ?? "").ToLowerInvariant().Trim();
        if (s.Length == 0 || s.Length > 60) return false;
        if (s.Contains("yield") || s.Contains("width") || s.Contains("weight")) return false;
        return s.Contains("price") || s.Contains("usd") || s.Contains("thb")
            || s.Contains("baht") || s.Contains("$") || s.Contains("/m")
            || s.Contains("per meter") || s.Contains("per metre");
    }

    /* Euro columns exist on a few sheets. The price book is USD and THB, and a
       euro figure written into either would be a straight error, so it is left
       out and reported rather than converted or guessed at. */
    public static bool IsEuro(string label)
    {
        var s = (label ?? "").ToLowerInvariant();
        return s.Contains("euro") || s.Contains("eur ") || s.Contains("€");
    }

    /* Does this header cell name a quantity band rather than a field?
       STG and others put the bands ACROSS the top - 200 m | 400 m | 1000 m+ -
       and the colour tier down the side. */
    public static bool IsQtyHeader(string label, out int min, out int? max)
    {
        min = 0; max = null;
        var s = (label ?? "").Trim();
        if (s.Length == 0 || s.Length > 24) return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(s, @"\d")) return false;
        // must look like a quantity, not a date or a width
        if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\d{1,2}[/.-]\d{1,2}[/.-]\d{2,4}")) return false;
        if (s.ToLowerInvariant().Contains("cm")) return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(
                s, @"(m\.?\b|meter|mtr|pcs|kg|\+|up)", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            return false;
        return Parse.QtyBand(s, out min, out max);
    }
}

static class Vocab3
{
    /* The WHOLE cell is a quantity: a number followed by a length unit, and
       nothing else. "3,000 m.", "500 meters", "1,000 yds".

       Not a price. Prices carry a currency or a decimal ("USD 2.95/ m",
       "2.78"), and a greige price per kilo ("430/kg") is deliberately outside
       this - kg is a weight the fabric is SOLD by, so those are real money.

       Wanted because merged group headings like "USD per meter / FOB" span
       columns that hold no prices at all, and the MOQ sitting under one came
       through as a price of 3000. */
    static readonly System.Text.RegularExpressions.Regex PlainQty = new(
        @"^\d[\d,]*(\.\d+)?\s*(m|m\.|mt|mtr|mtrs|meter|meters|metre|metres|yd|yds|yard|yards)\.?$",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase
      | System.Text.RegularExpressions.RegexOptions.Compiled);

    public static bool IsPlainQuantity(string cell)
    {
        var s = (cell ?? "").Trim();
        return s.Length > 0 && s.Length <= 20 && PlainQty.IsMatch(s);
    }
}
