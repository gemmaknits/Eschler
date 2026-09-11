/*  What the header words mean.

    Built from a survey of all 676 header rows in the workbook, not from an
    assumed layout - including the typos the sheets actually contain ("mcq" for
    MOQ on 84 rows, "compositon", "usable withd"). A label this does not know
    is reported rather than guessed at. */

enum Role { None, Article, Composition, Fabric, Moq, QtyTier, FullWidth,
            UsableWidth, Weight, Date, Remark, Price }

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
