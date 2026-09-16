# Eschler Price List — Session Handoff

Continuation notes for the price-list workstream. Read this first when you resume in a new Claude Code session.

## Goal

Turn the multi-tab `Eschler_Updated Special Price list_2025.xlsx` workbook (117 customer sheets, mixed layouts) into a single normalized table (`gemmasoft.dbo.so_price_list`) that a web app can serve for price lookups by (customer × article × color × qty × as-of date).

## What's already built

| Layer | Artifact | Location |
|---|---|---|
| Data model design | Normalized price_book schema (~24 fields), documented and rendered in a mock UI. | (in this file) |
| Excel template | Working `.xlsx` with `price_book` + `lookup` sheets. Formulas use `AGGREGATE` (not MATCH) to natively iterate the condition array without CSE. | [to_excel/price_book_template.xlsx](to_excel/price_book_template.xlsx) built by [to_excel/build_price_book_template.ps1](to_excel/build_price_book_template.ps1) |
| Form-based mock UI | Header form + colors-as-chips + qty-tier grid with USD/THB pairs per color. localStorage persistence. | [mock_ui/index.html](mock_ui/index.html) |
| Spreadsheet-style mock UI | Flat grid, sticky Customer/Article, click-to-edit, keyboard nav, two-row header (Qty / USD / THB / metadata groups). No colors. Colors columns are `PFE/PFD, White, Light, Medium, Dark, All colors` twice (once under USD, once under THB). | [mock_ui/grid.html](mock_ui/grid.html) |
| Extraction script | Header-aware parser for 117 sheets. Handles CENTER USD explicitly; generic parser detects header row keywords + currency super-headers ("USD per meter" / "THB per meter") for every other sheet. Emits CSV first (safety), then xlsx. | [to_excel/extract_pricebook.ps1](to_excel/extract_pricebook.ps1) |
| CSV → xlsx builder | Reliable Excel COM writer that avoids the `Int32 → String` cast bug by writing everything as strings. Text-formatted cells preserve text; General-formatted cells auto-convert numeric-looking strings. | [to_excel/csv_to_xlsx.ps1](to_excel/csv_to_xlsx.ps1) |
| Extracted data | 3,639 clean rows + 1,344 ambiguous rows across the workbook. | [to_excel/price_book_extracted.xlsx](to_excel/price_book_extracted.xlsx), [to_excel/price_book_extracted.pb.csv](to_excel/price_book_extracted.pb.csv), [to_excel/price_book_extracted.amb.csv](to_excel/price_book_extracted.amb.csv) |
| DB table | `gemmasoft.dbo.so_price_list` created, 34 columns, PK + filtered-unique index + lookup index. Loaded with all 3,639 rows via SqlBulkCopy. | [to_excel/load_so_price_list.ps1](to_excel/load_so_price_list.ps1) |

## Environment / connection

- SQL Server: `172.16.3.10`, db `gemmasoft`, user `sa`, password `sql@min` (SQL Server 2014, `12.00.5000`).
- Tables involved: `customers`, `dm`, `designs`, `pptc_job_order_status`, plus new `so_price_list`.
- Source workbook (owned by `eschlerth@gmail.com`, shared with user): the local unlocked copy at `D:\Claude\Eschler-costing\to_excel\Eschler_Updated Special Price list_2025_unlock.xlsx` opens cleanly via Excel COM. The original `..._2025.xlsx` triggers Protected View — do NOT try to open it via COM.
- Excel COM landmines learned the hard way:
  1. **PowerShell parsing:** `$arr[$r+1, $c]` in indexing parses as `$r + (1,$c)` (comma binds tighter). Always parenthesize: `$arr[($r+1), $c]`. In method calls `Cells(a+b, c)` is fine.
  2. **Cell format vs value type mismatch:** setting `Value2 = Int32` on a text-formatted (`@`) cell throws `InvalidCastException`. Safest cross-type approach: **write every value as `[string]`** — text-format cells keep it as text (preserving leading zeros / article codes), General-format cells auto-convert numeric-looking strings back to numbers.
  3. **Bulk `Value2 = 2D array`:** works fine for multi-row ranges. **Fails for 1-row ranges** ("Object[,] cannot cast to String") because Excel wants 1D `object[]` when the range has one row. If you need row-at-a-time writes, go cell-by-cell.
  4. **Article codes as text:** the extractor and loader treat the article column as text throughout. If you ever `WHERE article = 256524` (number) instead of `'256524'` (text), lookups miss everything.

## What `dbo.so_price_list` looks like

```
so_price_list_id     bigint IDENTITY PK
customer_id          bigint NULL          -- FK candidate to customers.customer_id (unenforced; user fills post-upload)
customer_name        nvarchar(100) NULL   -- snapshot of customers.name at mapping time
customer_excel       nvarchar(120) NULL   -- raw sheet name / customer text from the workbook
article              nvarchar(30)  NULL   -- raw article as uploaded
design_no            char(20)      NULL   -- validated against dm.Design_no (post-mapping)
article_variant      nvarchar(20)  NULL
fabric_name          nvarchar(120) NULL   -- excel value or dm.refdesno for new manual entries
composition          nvarchar(200) NULL   -- excel value or dm.compo
full_width_cm        nvarchar(30)  NULL   -- excel value or designs.Fwth
usable_width_cm      nvarchar(30)  NULL   -- excel value or designs.Usewth
weight_gsm           nvarchar(30)  NULL   -- excel value or designs.gmpersqm
moq                  nvarchar(30)  NULL
qty_min              int           NOT NULL
qty_max              int           NULL   -- NULL = open upper bound
qty_unit             char(2)       NOT NULL DEFAULT 'M'   -- 'M' or 'KG'
color_tier           nvarchar(30)  NOT NULL   -- PFE/PFD, White, Light, Medium, Dark, All_colors, Color_Black, PFE_Stock, Greige, ...
price_usd            decimal(18,4) NULL
price_thb            decimal(18,4) NULL
terms                nvarchar(60)  NULL
valid_from           date          NULL
valid_to             date          NULL
sonoid               nvarchar(30)  NULL
so_line_id           bigint        NULL
quote_ref            nvarchar(200) NULL
source_sheet         nvarchar(60)  NULL   -- original workbook tab name
source_row           int           NULL   -- original workbook row #
notes                nvarchar(500) NULL
creation_date        datetime      NOT NULL DEFAULT SYSDATETIME()
created_by           nvarchar(30)  NULL
last_updated_date    datetime      NULL
updated_by           nvarchar(30)  NULL
delete_mark          char(1)       NOT NULL DEFAULT 'N'
deleted_by           nvarchar(30)  NULL
```

Indexes:
- `PK_so_price_list` on `so_price_list_id`
- `UX_so_price_list_active` — unique on `(customer_id, design_no, article_variant, qty_min, qty_max, qty_unit, color_tier)` filtered `WHERE delete_mark <> 'Y' AND customer_id IS NOT NULL AND design_no IS NOT NULL`. Won't fire until you've mapped `customer_id` and `design_no` — deliberate, so bulk-import can complete before manual mapping.
- `IX_so_price_list_design_customer` on `(design_no, customer_id)` INCLUDE `(color_tier, qty_min, qty_max, price_usd, price_thb, valid_from, valid_to, delete_mark)`.

## Open items (pick up from here)

### Immediate data-quality cleanups

1. **USD/THB stored as split rows** — the header-aware extractor emits one row per (color × currency), so ANITA/256524/Light 701-2500 shows up as two rows (`usd=5.63, thb=NULL`) and (`usd=NULL, thb=169`). Design of `so_price_list` expects a single row with both. Merge query TBD:
   ```sql
   -- outline: for each (customer_excel, article, article_variant, qty_min, qty_max, qty_unit, color_tier) group,
   -- pick MAX(price_usd), MAX(price_thb) into one row, soft-delete the others.
   ```
2. **CENTER vs CENTER PRICE THB** — the master price sheets emit two customers today; they should map to the same `customer_id`.
3. **1,344 ambiguous rows** still in `price_book_extracted.xlsx` on the `ambiguous` tab (not in DB). Distribution:
   - Biga Thailand (444), Chantasia_UPDATE (195), BIGA Lanka greige & PFD (111), Chantasia_record (94), Setafil Thailand (86), STG (73), Mastex (59), WEDTEX (38), Gamma 28 (33), NAYLIT (27), Mas IntimatesUnichela (25), Crystal Martin (19)…
   - Ask user which sheets to prioritize. Each needs a small parser tweak — usually adding a header keyword the generic parser doesn't recognize yet.

### Customer / article mapping pass (user-owned but assistable)

- Populate `customer_id` and `customer_name` by joining `customer_excel` to `customers` on `name` (or best-fuzzy-match if names diverge).
- Populate `design_no` by matching `article` against `dm.Design_no`.
- After both cols are populated the filtered unique index kicks in — if there are pre-existing duplicates the ALTER will fail loudly. Run the dupe check first:
  ```sql
  SELECT customer_id, design_no, article_variant, qty_min, qty_max, qty_unit, color_tier, COUNT(*)
  FROM dbo.so_price_list
  WHERE customer_id IS NOT NULL AND design_no IS NOT NULL AND delete_mark <> 'Y'
  GROUP BY customer_id, design_no, article_variant, qty_min, qty_max, qty_unit, color_tier
  HAVING COUNT(*) > 1;
  ```

### FK constraints (optional, decide after cleanup)

Not created intentionally. When ready:
```sql
-- customer_id → customers.customer_id (verify customers.customer_id has a unique index first)
ALTER TABLE dbo.so_price_list
  ADD CONSTRAINT FK_so_price_list_customer
  FOREIGN KEY (customer_id) REFERENCES dbo.customers(customer_id);

-- design_no → dm.Design_no (dm.Design_no is PK-like — char(20) NOT NULL, likely already unique)
ALTER TABLE dbo.so_price_list
  ADD CONSTRAINT FK_so_price_list_design
  FOREIGN KEY (design_no) REFERENCES dbo.dm(Design_no);
```

### Web app (user's stated eventual goal)

- Frontend: promote one of the mocks (grid.html is the current preferred direction). Wire it to a `GET /so_price_list?article=&customer=&color=&qty=&as_of=` endpoint.
- Backend: minimal stub over `gemmasoft.dbo.so_price_list`. The lookup query pattern is:
  ```sql
  SELECT TOP 1 *
  FROM dbo.so_price_list
  WHERE design_no = @design_no
    AND (customer_id = @customer_id OR customer_id IS NULL)  -- exact customer, else CENTER-style row
    AND color_tier  = @color_tier
    AND qty_unit    = @qty_unit
    AND qty_min <= @qty
    AND (qty_max IS NULL OR qty_max >= @qty)
    AND delete_mark <> 'Y'
    AND (valid_from IS NULL OR valid_from <= @as_of)
    AND (valid_to   IS NULL OR valid_to   >= @as_of)
  ORDER BY
    CASE WHEN customer_id = @customer_id THEN 0 ELSE 1 END,   -- exact-customer wins
    COALESCE(valid_from, '1900-01-01') DESC;                  -- latest quote wins
  ```

## Session-hygiene tips

- If Excel COM starts throwing weird errors, kill leftover EXCEL.EXE processes first:
  ```
  taskkill //F //IM EXCEL.EXE
  ```
- When running long extraction/load PowerShell scripts, run them **in the background** and use the Monitor tool with a `until grep -q "Saved: |exited with"` loop. Filter the tail with grep so you don't drown in per-cell chatter.
- Windows Powershell 5.1 is the primary shell. Bash is available for text munging (grep/sort/awk).
- The `csv_to_xlsx.ps1` all-string trick is the reliable fallback when bulk Value2 writes misbehave.

## Related work in this project (not price-list)

- `pptc_job_order_status` duplicate cleanup and root-cause: `dbo.P_PPTC_GAMMA_PKG_update_job_order_status` INSERT runs concurrently, needs `WITH (UPDLOCK, HOLDLOCK)` on the pjos LEFT JOIN + a filtered unique index. Full analysis in the earlier session transcript.
- Costing SP → xlsx pipeline (unrelated to price list but same repo): [to_excel/run_costing_export.ps1](to_excel/run_costing_export.ps1) exports MMMYY-named `.xlsx` files. Uses the same `<ignoredError sqref>` XML injection trick to suppress the green "Number stored as text" marker.

## Quick-start recipes for the next session

**Rerun extraction from source workbook:**
```
powershell -File to_excel\extract_pricebook.ps1
powershell -File to_excel\csv_to_xlsx.ps1
```

**Reload so_price_list from the CSV (idempotent bulk, doesn't dedupe — TRUNCATE first if you want a clean state):**
```
powershell -File to_excel\load_so_price_list.ps1
```

**Row counts for sanity:**
```sql
SELECT COUNT(*) FROM dbo.so_price_list;                                     -- total
SELECT customer_excel, COUNT(*) FROM dbo.so_price_list GROUP BY customer_excel ORDER BY 2 DESC;
SELECT color_tier, COUNT(*) FROM dbo.so_price_list GROUP BY color_tier ORDER BY 2 DESC;
```
