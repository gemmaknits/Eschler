/*  Which column is USD and which is THB, decided from the figures.

    Several sheets put the two currencies side by side under identical tier
    labels and never say which is which:

        PFE/PFD | All colors | PFE/PFD | All colors
           2.00 |       2.60 |      60 |        78

    Reading the header cannot settle that, and guessing from a currency word
    somewhere on the sheet gets it backwards about as often as it gets it right.
    The figures settle it: the same fabric, the same tier, roughly thirty times
    the number. So within one table, where a tier has two price columns and
    neither column SAID what it was, the cheaper column is USD and the dearer
    is THB.

    A column whose own label named a currency is never touched. */
static class Money
{
    public static int Fix(List<Line> lines)
    {
        int changed = 0;

        foreach (var table in lines.GroupBy(l => (l.Sheet, l.HeaderRow)))
        foreach (var tier in table.GroupBy(l => l.Tier))
        {
            var cols = tier.GroupBy(l => l.Col)
                           .Select(g => new {
                               Col = g.Key,
                               Stated = g.Any(x => x.CurrencyStated),
                               Median = Median(g.Select(x => x.Price).ToList()),
                               Rows = g.ToList()
                           })
                           .OrderBy(x => x.Median)
                           .ToList();

            if (cols.Count != 2) continue;            // no pair to compare
            if (cols[0].Stated || cols[1].Stated) continue;
            if (cols[0].Median <= 0) continue;

            // Only act on a gap big enough to be a currency rather than a
            // price difference between two colour tiers.
            var ratio = cols[1].Median / cols[0].Median;
            if (ratio < 8) continue;

            foreach (var r in cols[0].Rows) { if (r.Currency != "USD") changed++; r.Currency = "USD"; }
            foreach (var r in cols[1].Rows) { if (r.Currency != "THB") changed++; r.Currency = "THB"; }
        }
        return changed;
    }

    static decimal Median(List<decimal> xs)
    {
        if (xs.Count == 0) return 0;
        xs.Sort();
        return xs[xs.Count / 2];
    }
}
