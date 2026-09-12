| `Article` | contains `article`; `product`; `quality no`; contains `design`; `eth item`; `suplier code` | Each of these was a whole sheet nobody could see: ANITA heads its later tables `Eschler Design :`, Crystal Martin `ETH ITEM`, Hanes Global `Suplier code` - on a customer's own form OUR number is the supplier's code. Bare `item` is deliberately NOT matched: Chantasia has a `Chantasia item` column holding the customer's code beside a real Article column. |
# Reading the Eschler price-book workbook

How `Eschler_Updated Special Price list_2025.xlsx` is turned into price lines,
why each rule exists, and how to tell whether a change made things better or
worse.

Written to be picked up cold. If you are starting a new session with no memory
of this work, read **Start here** and **How to tell if you broke it** before
changing anything in `xlsxtool/`.

---

## Start here

```
cd to_excel/xlsxtool
dotnet build -c Release

# what the scanner reads
dotnet run -c Release --no-build -- scan  "<workbook.xlsx>" ../full_scan.csv

# what it does NOT read - the check that matters
dotnet run -c Release --no-build -- coverage "<workbook.xlsx>" ../uncaptured_money.csv

# one sheet, cell by cell
dotnet run -c Release --no-build -- dump "<workbook.xlsx>" "<sheet name>"

# what the scanner decided about a sheet's tables
dotnet run -c Release --no-build -- explain "<workbook.xlsx>" "<sheet name>"

# what a single label means to the vocabulary (opens no workbook)
dotnet run -c Release --no-build -- vocab "Qty per color" "All color (USD/M)"
```

Sheet names sometimes carry a **trailing space** — `"ANITA_Thailand & Germany "`.
`sheets` prints them exactly; copy from there or the tool will say it cannot
find the sheet.

Current numbers, for comparison after a change:

| | |
|---|---|
| price lines | 8,613 |
| distinct designs | 667 |
| sheets producing lines | 100 of 117 |
| money cells captured | 886 of 1,138 |
| USD prices over 100 (implausible) | 1.4% |

---

## The shape of the problem

The workbook is not a database export. It is twenty years of quotations kept
by hand, and **no two sheets agree on anything**:

- A sheet holds many tables, not one. Acundis has thirteen, each with its own
  header and its own column order.
- Headers run one, two or three rows deep.
- The same design is quoted repeatedly over the years, in separate blocks, at
  different prices. Both are correct; they are different quotes.
- Prices are written as `2.11`, `$ 2.11 /m.`, `THB 216.00`, `USD4.93/m.`, and
  inside sentences.
- Colour tiers are sometimes column headings and sometimes values in a column.
- Quantity bands are sometimes down the side and sometimes across the top.

So the scanner does not assume a layout. It finds each header where it stands
and reads the rows beneath it on that header's terms.

---

## The model

One emitted **line** is one price:

```
sheet, source_row, header_row, design_no, fabric_name, composition,
full_width_cm, usable_width_cm, weight_gsm, moq, qty_raw, qty_min, qty_max,
color_tier, currency, price, date_raw, remark, block_note
```

`source_row` and `header_row` are Excel row numbers, so **every figure can be
traced back to the cell it came from**. Keep it that way; it is what makes a
disagreement settleable.

In the database these become `SO.so_price_list_detail`, grouped into grid rows:

- **set_no** — one grid row: one design, one quantity band, one quote block.
- **line_no** — the tier row within the set. **USD and THB share it**: they are
  one worksheet line with two currencies, not two lines.

---

## Column recognition (`Vocab.cs`)

A header cell is matched to a role. The roles that matter:

| Role | Matched by | Notes |
|---|---|---|
| `Article` | contains `article`; `product`; `quality no`; contains `design` | **`design` matters** — ANITA heads its later tables `Eschler Design :`. Without it the table has nothing to identify a line by and is rejected wholesale. |
| `QtyTier` | starts with `qty` / `quantity` | the band down the side |
| `Moq` | `moq`, `mcq` | |
| `Price` | any label naming a colour tier | see below |
| `TierValue` | `color type`, and `color`/`colour` when the cells below hold tier NAMES | the tier is a value on the row |
| `Composition`, `Fabric`, `FullWidth`, `UsableWidth`, `Weight`, `Date`, `Remark` | the obvious spellings | |

**Colour tiers** map to a fixed set: `PFE/PFD`, `PFE`, `PFD`, `Greige`, `White`,
`Light`, `Medium`, `Dark`, `All_colors`. A trailing unit note is stripped first,
so `All color (USD/M)` is `All_colors`.

Every unrecognised spelling **silences a whole price column**. Chief You writes
`All dye colors`; that one word cost twenty-nine prices. When a sheet yields
nothing, suspect the vocabulary first — `vocab` answers it in one command.

---

## Finding a table (`Scan.ReadHeader`)

A row is a header when it has **a price column and something to identify a line
by**. Three recognised labels alone is a coincidence, not a table.

### Multi-row headers

Headers run up to three rows. A column takes whichever row actually says
something — the money from one, the tier from the next:

```
DATE | ARTICLE # | ... | USD FOB BKK / METER | USD FOB BKK / METER
     |           | ... | PFE/PFD             | ALL COLOR
```

### What counts as part of the header — the rule most bugs came from

`IsHeaderContinuation` decides whether the row below is more header or the
first row of data. It answers **no** if the row holds an article, **or if two
or more of its cells parse as prices**.

Both halves were learned the hard way:

- Without the article test, the first DATA row was read as header text. A MOQ
  column whose first value is `3,000 m.` became a column of quantity bands, and
  quantities came through as prices of 3000 — 1,460 phantom lines across
  Chantasia, WEDTEX and Setafil.
- Without the price test, a data row whose design cell is **blank** — because
  the design carries down from the row above — still looked like a header. On
  ANITA that row read `2,000 m. +   $ 3.90`, so `Qty per color` was taken for a
  band column and every quantity under it was lost.

### The header row must say something itself

A blank row above a real header can read every label from below and match,
which shifts the whole table up by one. That broke the USD/THB split on CENTER,
which had been right. So at least one label must sit on the header row itself.

---

## Deciding what a column holds

Order matters. For each column:

1. **Euro** — left out. The price book is USD and THB; a euro figure written
   into either would be wrong.
2. **Quantity bands across the top** (`200 m | 400 m | 1000 m+`) — each is a
   price column carrying its own band, and the tier comes from the row.
3. **A money label with no tier** (`USD/ m`, `Price/M.`) — a price column.
   PT Mirae, Milavitsa and UTAX are built entirely of these.
4. **A tier label** — a price column for that tier.

Then two corrections, both made by **looking at the cells**, not the label:

- `Color` alone is either a tier-value column (`PFD`, `White`, `Color/Black`)
  or the All-colors price column. The cells below say which.
- A tier-named column holding **words** is a description. PT Busana has a
  `COLOR` column reading "White & Black" with its money in a column called
  `PRICE`; reading COLOR as prices discarded the real column and lost the sheet.
  A column with **any** real price in it is never demoted.

Sampling for these tests starts at the **first data row**, not the row below the
header. On a three-row header the row below is still header text — that made
every price column on Ausco look like words and dropped it from 268 lines to 20.

### Merged group headings are not price columns

```
... | USD per meter / FOB | USD per meter / FOB | THB per meter ...
Qty | Article |            | PFE/PFD | All colors | PFD/PFE | All colors
```

The heading is merged across its columns, so every column it spans reads as
"money" — including one holding `3,000 m.`. **Where a table names its tiers,
those are the price columns** and the money-labelled ones are dropped. Only
when a tier column actually holds numbers, or a genuine single-price table
loses its only column.

---

## Reading a price (`Parse.Price`)

Strip everything but digits and dots, then **trim dots from both ends**.

That trim is not cosmetic. `$ 2.11 /m.` strips to `2.11.` — the unit's full
stop — which reads as two decimal points and was thrown out as malformed.
**Chief You is written entirely that way and yielded nothing**; Crystal Martin,
Hanes Global, Mas Intimates and Savage were mostly empty for the same reason.
Trimming still rejects `1.2.3`, which is what the test was actually for.

Rejected: `-`, `n/a`, `x`, zero (the sheets use it as a blank), anything over
100,000.

### A quantity is not a price

Two guards, both narrow on purpose:

- A cell that **restates its own column's band** is a repeated header. STG
  reprints `200 m | 400 m | 600 m` every few articles.
- A cell that is a **bare number with a length unit** — `3,000 m.`,
  `500 meters` — is a quantity wherever it appears.

Greige quoted **per kilo** is deliberately outside both: `kg` is how the fabric
is sold, so `430/kg` is money. An earlier, looser version of this rule threw
away 34 designs of real greige prices.

---

## Which quote a line came from

The line above — or the first column of — a header usually says:

```
Quoted Price by K. Sivy on 20.12.2022
Qty | Article | PFE/PFD | All colors
```

That text is carried on every line as `block_note`, and into `notes` in the
database. ANITA prices 255028 in a 2018 quote and again in 2022 with partly
matching figures; without the dating line the two are **identical on screen**
and nobody can say which to keep.

A title is prose, a column label is short. The separation is length — a title
often contains the word "price" and would otherwise be mistaken for a money
label and skipped.

**`header_row` is part of the grid-row key.** Keyed without it, two quotes for
one design and band collapse into one row, the two USD prices collide in a
single cell, and the pivot splits them apart arbitrarily — one row keeps the
THB, the other looks half empty. That is what "the USD price is not complete"
turned out to mean.

This is common: Biga Thailand has 64 such bands across 34 designs, Chantasia 41,
Ausco 19.

---

## How to tell if you broke it

Never judge a change by the sheet that prompted it. Run all four:

**1. Nothing lost.** Compare designs against what is already loaded:

```sql
SELECT DISTINCT design_no FROM SO.so_price_list_detail WHERE delete_mark <> 'Y';
```

Any design that disappears must be explained. Two real examples of a fix that
looked right and was not: one lost 34 designs of greige prices quoted per kilo,
another dropped Ausco from 268 lines to 20.

**2. Coverage went up, not down.** `coverage` counts cells that can only be
money — a currency symbol or the word USD/THB beside a number — and says how
many became price lines. 886 of 1,138 today.

**3. Implausible prices stayed low.** USD over 100 is 1.4%. It peaked at 12.7%
when quantities were being read as prices.

**4. Spot-check the sheet you changed** with `dump` and `explain`, and read the
figures against the workbook.

### When a sheet yields nothing

In the order that has actually paid off:

1. `vocab` the header labels — an unknown spelling for the design column or a
   tier silences everything.
2. `explain` the sheet — is a header found at all, and what are its price
   columns?
3. `dump` the header rows and count columns by hand.
4. Check `IsHeaderContinuation` against the row below the header.

---

## What is deliberately NOT extracted

`uncaptured_money.csv` lists every cell holding money that did not become a
price line — sheet, row, column, text. About 320 remain, and most are prose:

```
add sea freight cost USD1.35/m. based on FOB Thailand price
K.Sivy reduce price to be THB 480/kg (including TC for only this order)
hydrophilic = surcharge US$0.50/m.
THB310/m. / US$9.68/m. FOB TH          <- two prices in one cell
```

These are **conditions attached to a price, or ambiguous** — not base prices.
Guessing at them would put invented numbers in front of the reviewers, which is
worse than a gap they can see. The file exists so a person can read them and
decide.

This is an assisted extraction, not a guarantee. Every list carries
`excel_data_verified`, and a person signs it.

---

## Loading into the database

1. `scan` → `full_scan.csv`
2. Bulk-copy into `SO.so_price_list_stage` (all columns, including `block_note`)
3. `db/rebuild_from_rescan.sql`

The rebuild **keeps the headers** — their names, customer assignments, validity
dates and grid shape — and replaces only the price lines. It is wrapped in a
transaction with `SET XACT_ABORT ON`.

That last part is not optional. Without it a truncation error committed the
DELETE and rolled back nothing, leaving the table with eight rows. Take a
snapshot before a rebuild:

```sql
SELECT * INTO SO.so_price_list_detail_snap_<date> FROM SO.so_price_list_detail;
```

---

## Things that will bite

- **Sheet names with trailing spaces.** `"ANITA_Thailand & Germany "`.
- **The ERP tables are `Thai_CI_AI`**; the SO tables are the database default.
  Any comparison across them needs `COLLATE DATABASE_DEFAULT` or it fails
  outright with `Msg 468`, not merely mis-sorts.
- **`dbo.dm.refdesno` is a reference-design field, not a fabric-name field.**
  It usually holds a name, but 403 of its 2,279 values are design numbers and
  some rows point at themselves. Inheriting a parent's `refdesno` was tried and
  removed: it put a design NUMBER in the fabric column.
- **`QUOTED_IDENTIFIER` must be ON** when creating any procedure that writes to
  `so_price_list_detail` — it carries filtered indexes, and a module created
  with it OFF fails at run time with `Msg 1934`. sqlcmd defaults it OFF, SSMS
  ON. It is set inside the .sql files so the deploy tool cannot decide it.
- **`dbo.dm`, `dbo.designs`, `dbo.customers`, `dbo.uom` belong to other
  systems.** Read them; never write them.

---

## Two more cell shapes

**The band and the price in one cell.** Hanes Global writes its price column as

```
200-599  m = 4.90/ m
600-1999 m = 4.55/ m
2,000 +    = 4.40/ m
```

`Parse.BandEqualsPrice` splits at the `=`, reads the band from the left and the
price from the right, and the cell's own band overrides the row's. Both halves
are checked against each other — the right-hand side must parse as a price and
must **not** itself look like a quantity, so a cross-reference
(`200-599 m = 600-999 m`) is not read as a price of 600.

**Several money columns that are not all ours.** Crystal Martin quotes

```
TARGET PRICE | PA PRICE | ESCHLER PRICE (NORMAL) | ESCHLER PRICE ( RECYCLED)
```

side by side. All four are money and all four are captured, because a reviewer
correcting a price list needs to see what was on the table — but `TARGET PRICE`
is the customer's target, not ours. So **a money column with no colour tier
carries its own heading into the line's note**, and the grid shows
`QUALITY -- TARGET PRICE` against that figure. Without it the four are
indistinguishable and someone would take a customer's target for our price.

---

## Rows that belong to no table

A few sheets quote with no header at all. Mas Intimates writes the design on a
line of its own and the bands underneath it:

```
255102        MOQ, 3,000m
              200-599m       .35/m
              600-2,000m     $ 4.05/m
```

`Fallback.cs` picks these up AFTER the main scan and only on rows nothing else
read. It is deliberately tight, because a loose rule here is how invented prices
get in. A row qualifies only when it is unmistakable: exactly one cell holds
money, exactly one other is a plain quantity band, and a design was stated on
its own line within fifteen rows above. Anything less clear is left for
`uncaptured_money.csv` and a person.

---

## Footnotes under a block

Hop Lun states its freight surcharges on their own lines beneath the prices they
apply to:

```
1. 200-600m. => add sea freight cost USD1.35/m. based on FOB Thailand price.
2. 601-2,00m. => add sea freight cost USD0.50/m.
```

Those are a CONDITION on a price, not a price, so they are not read as money -
but a price list without them is misleading, and they were going nowhere at all.
Each is appended to the note of the lines in the block above it.

The scan stops at the next block, and a row sitting directly above a header is
that header's TITLE rather than this block's footnote. Without that second test
a Hop Lun block swallowed "Here below price quoted by K. Sivy on 18.06.20",
which introduces the prices that follow it.

---

## What is left, and why

One layout is understood and not handled: **Hanes Global's first table**, a
customer's own 29-column procurement form whose header is repeated on the row
below with the money columns replaced by "based on Incoterm validated". Read on
its own that second row is a valid header carrying one price column where the
real one has four, so it replaces it. A containment rule was tried and removed -
it recovered nothing and cost four cells elsewhere. The rest of that sheet reads
normally.

The remaining uncaptured cells are prose, and prose is where this stops. A
price inside a sentence - "K.Sivy reduce price to THB 480/kg (including TC for
only this order)" - is a condition, a negotiation or a one-off, and turning it
into a price line would put an invented number in front of a reviewer. They are
listed instead.