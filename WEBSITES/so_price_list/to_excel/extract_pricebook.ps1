$ErrorActionPreference = 'Stop'
$SRC = (Join-Path $PSScriptRoot 'Eschler_Updated Special Price list_2025_unlock.xlsx')
$OUT = (Join-Path $PSScriptRoot 'price_book_extracted.xlsx')
if (Test-Path $OUT) { Remove-Item $OUT -Force }

# --- schemas ---
$pbHeaders = @(
 'price_id','customer','article','article_variant','fabric_name','composition',
 'full_width_cm','usable_width_cm','weight_gsm','moq',
 'qty_min','qty_max','qty_unit','color_tier',
 'price_usd','price_thb','terms',
 'valid_from','valid_to','quote_ref','source_sheet','source_row','notes'
)
$ambHeaders = @('source_sheet','source_row','reason','col_A','col_B','col_C','col_D','col_E','col_F','col_G','col_H','col_I','col_J','col_K','col_L','col_M','col_N')

$pbRows  = New-Object System.Collections.ArrayList
$ambRows = New-Object System.Collections.ArrayList
$log     = New-Object System.Collections.ArrayList
$nextId  = 1

function IsArticle([string]$s) {
    if (-not $s) { return $false }
    return ($s -match '^[23]\d{5}([A-Z]{0,4}(/\d{1,2}([A-Z\-\.]{0,5})?)?)?$')
}
function ParseQtyBand([string]$s) {
    if (-not $s) { return $null,$null }
    $t = $s -replace ',',''
    if ($t -match '^\s*(\d+)\s*-\s*(\d+)\s*[mMkKgG]*') { return [int]$Matches[1], [int]$Matches[2] }
    if ($t -match '^\s*(\d+)\s*[mMkKgG]*\+') { return [int]$Matches[1], $null }
    if ($t -match '^\s*(\d+)\s*[mMkKgG]*') { return [int]$Matches[1], $null }
    return $null,$null
}
function AsNum($v) {
    if ($null -eq $v -or $v -eq '') { return $null }
    try { return [double]$v } catch { return $null }
}

# --- helper: bulk-read the used range as a 2D object array ($data[row,col], 1-based) ---
function Read-Grid($ws) {
    $used = $ws.UsedRange
    $rows = $used.Rows.Count
    $cols = $used.Columns.Count
    if ($rows -eq 0 -or $cols -eq 0) { return $null, 0, 0 }
    $data = $used.Value2   # single COM call โ€” fast
    if ($rows -eq 1 -and $cols -eq 1) {
        # scalar case: wrap so downstream indexing works
        $arr = New-Object 'object[,]' 1,1
        $arr[0,0] = $data
        $data = $arr
        # $data is 0-based here โ€” convert to 1-based emulation isn't possible; use a wrapper
    }
    return $data, $rows, $cols
}
function G($data, $r, $c) {
    # Value2 returns 1-based array from Excel; guard against nulls
    try { $v = $data[$r,$c] } catch { return '' }
    if ($null -eq $v) { return '' }
    return "$v".Trim()
}
function GN($data, $r, $c) {
    try { $v = $data[$r,$c] } catch { return $null }
    if ($null -eq $v -or $v -eq '') { return $null }
    try { return [double]$v } catch { return $null }
}

# --- header-aware detection helpers ---
# Map raw header text to a schema slot. Returns @{ role='article|fabric|comp|fw|uw|wt|moq|qty|qmin|qmax|color|other'; color=<name>; currency=<USD|THB|$null>; } or $null.
function Classify-Header([string]$s) {
    if (-not $s) { return $null }
    $t = ($s -replace '\s+',' ').Trim().ToLower()
    if ($t -eq '') { return $null }
    # article
    if ($t -eq 'article' -or $t -eq 'art' -or $t -eq 'article no' -or $t -eq 'art no' -or $t -eq 'article number') { return @{role='article'} }
    if ($t -eq 'fabric name' -or $t -eq 'fabric' -or $t -eq 'quality' -or $t -eq 'product') { return @{role='fabric'} }
    if ($t -eq 'composition' -or $t -eq 'comp') { return @{role='comp'} }
    if ($t -match '^full ?width' -or $t -eq 'full w' -or $t -eq 'full width (cm)' -or $t -eq 'full width cm') { return @{role='fw'} }
    if ($t -match '^use?able ?width' -or $t -eq 'usable w' -or $t -eq 'usable width (cm)' -or $t -eq 'width') { return @{role='uw'} }
    if ($t -match '^weight' -or $t -eq 'gsm' -or $t -match 'g\s*/\s*m2' -or $t -match 'g/mยฒ') { return @{role='wt'} }
    if ($t -eq 'moq' -or $t -match '^min\.? ?order' -or $t -eq 'minimum order') { return @{role='moq'} }
    if ($t -eq 'qty' -or $t -eq 'qty per color' -or $t -eq 'quantity' -or $t -eq 'band' -or $t -eq 'qty band' -or $t -match '^qty ?/ ?color') { return @{role='qty'} }
    if ($t -eq 'qty min' -or $t -eq 'min qty' -or $t -eq 'from qty' -or $t -eq 'min') { return @{role='qmin'} }
    if ($t -eq 'qty max' -or $t -eq 'max qty' -or $t -eq 'to qty' -or $t -eq 'max') { return @{role='qmax'} }
    if ($t -match '^valid( from)?$' -or $t -eq 'from' -or $t -eq 'valid from' -or $t -eq 'date from') { return @{role='vf'} }
    if ($t -eq 'valid to' -or $t -eq 'to' -or $t -eq 'date to' -or $t -eq 'expiry') { return @{role='vt'} }
    if ($t -match '^terms?$' -or $t -match '^price terms$' -or $t -eq 'fob' -or $t -eq 'ddp' -or $t -eq 'cif') { return @{role='terms'} }
    if ($t -eq 'notes' -or $t -eq 'remark' -or $t -eq 'remarks' -or $t -eq 'note') { return @{role='notes'} }
    # colors โ€” currency may be embedded in the same cell (e.g. "USD PFE/PFD")
    $cur = $null
    if ($t -match '\busd\b' -or $t -match '\bus\$\b' -or $t -match '^\$') { $cur = 'USD' }
    elseif ($t -match '\bthb\b' -or $t -match 'baht' -or $t -match '^เธฟ' -or $t -match '\bthb/m') { $cur = 'THB' }
    $ct = $t -replace '\busd\b','' -replace '\bthb\b','' -replace '/ ?meter','' -replace '/m\b','' -replace 'per meter','' -replace '\$','' -replace 'เธฟ',''
    $ct = ($ct -replace '\s+',' ').Trim()
    switch -Regex ($ct) {
        '^pfe\s*/\s*pfd$' { return @{role='color'; color='PFE/PFD'; currency=$cur} }
        '^pfd\s*/\s*pfe$' { return @{role='color'; color='PFE/PFD'; currency=$cur} }
        '^pfe$'           { return @{role='color'; color='PFE';     currency=$cur} }
        '^pfd$'           { return @{role='color'; color='PFD';     currency=$cur} }
        '^white$'         { return @{role='color'; color='White';   currency=$cur} }
        '^light$'         { return @{role='color'; color='Light';   currency=$cur} }
        '^medium$'        { return @{role='color'; color='Medium';  currency=$cur} }
        '^dark$'          { return @{role='color'; color='Dark';    currency=$cur} }
        '^black$'         { return @{role='color'; color='Black';   currency=$cur} }
        '^all colors?$'   { return @{role='color'; color='All_colors'; currency=$cur} }
        '^color\s*/\s*black$' { return @{role='color'; color='Color_Black'; currency=$cur} }
        '^solid( dyed)?$' { return @{role='color'; color='Solid_Dyed'; currency=$cur} }
        '^greige$'        { return @{role='color'; color='Greige';    currency=$cur} }
        '^natural$'       { return @{role='color'; color='Natural';   currency=$cur} }
        '^grey|^gray$'    { return @{role='color'; color='Grey';      currency=$cur} }
        '^price$'         { return @{role='color'; color='Price';     currency=$cur} }
    }
    return $null
}

# Detect the header row: pick the row with the most classifiable headers (min threshold = 3).
# Returns @{hdrRow=<1-based row>; cols=@{article=<c>;fabric=<c>;...}; colors=@(@{col=<c>;color=..;currency=..}, ...)}
function Detect-Header($data, $nRows, $nCols) {
    $scanRows = [Math]::Min($nRows, 40)
    $best = $null; $bestHits = 0
    for ($r = 1; $r -le $scanRows; $r++) {
        $hits = 0; $roles = @{}; $colors = @()
        for ($c = 1; $c -le $nCols; $c++) {
            $cls = Classify-Header (G $data $r $c)
            if (-not $cls) { continue }
            $hits++
            if ($cls.role -eq 'color') { $colors += @{col=$c; color=$cls.color; currency=$cls.currency} }
            elseif (-not $roles.ContainsKey($cls.role)) { $roles[$cls.role] = $c }
        }
        if ($hits -gt $bestHits) { $bestHits = $hits; $best = @{hdrRow=$r; cols=$roles; colors=$colors} }
    }
    if ($bestHits -lt 3) { return $null }
    return $best
}

# Detect currency super-header above hdrRow: text like "USD per meter" spanning multiple color columns
function Fill-CurrencyContext($data, $nRows, $nCols, $hdr) {
    if (-not $hdr) { return }
    # scan up to 4 rows above hdrRow
    $bands = @()  # @{start;end;currency}
    for ($r = [Math]::Max(1, $hdr.hdrRow - 4); $r -lt $hdr.hdrRow; $r++) {
        $curStart = 0; $curCurr = $null
        for ($c = 1; $c -le $nCols; $c++) {
            $t = (G $data $r $c).ToLower()
            if ($t) {
                $cur = $null
                if ($t -match '\busd\b|^\$|per meter\s*/\s*fob|fob.*usd|usd\s*/\s*m|usd per|us\$') { $cur = 'USD' }
                elseif ($t -match '\bthb\b|baht|เธฟ|thb/m|thb per') { $cur = 'THB' }
                if ($cur) {
                    if ($curCurr -and $curStart -gt 0) { $bands += @{start=$curStart; end=$c-1; currency=$curCurr} }
                    $curStart = $c; $curCurr = $cur
                }
            }
        }
        if ($curCurr -and $curStart -gt 0) { $bands += @{start=$curStart; end=$nCols; currency=$curCurr} }
    }
    # apply bands to any color col without a currency
    foreach ($col in $hdr.colors) {
        if (-not $col.currency) {
            foreach ($b in $bands) {
                if ($col.col -ge $b.start -and $col.col -le $b.end) { $col.currency = $b.currency; break }
            }
        }
    }
}

# Generic header-aware parser for any customer sheet.
# Rules:
#  * Row with an article code in the article col โ’ establishes context (fabric/comp/widths/weight/moq).
#  * Same row & subsequent rows may hold qty band + numeric prices per color col.
#  * Blank article-col rows inherit the current article context (multi-tier layout).
function Parse-Generic($ws, $sheetName) {
    $data, $nRows, $nCols = Read-Grid $ws
    if (-not $data) { return 0 }
    $hdr = Detect-Header $data $nRows $nCols
    if (-not $hdr) { return 0 }
    Fill-CurrencyContext $data $nRows $nCols $hdr

    $artCol = $hdr.cols['article']
    if (-not $artCol) { return 0 }
    $qmiCol = $hdr.cols['qmin']
    $qmxCol = $hdr.cols['qmax']
    $qtyCol = $hdr.cols['qty']
    $fabCol = $hdr.cols['fabric']
    $comCol = $hdr.cols['comp']
    $fwCol  = $hdr.cols['fw']
    $uwCol  = $hdr.cols['uw']
    $wtCol  = $hdr.cols['wt']
    $moqCol = $hdr.cols['moq']
    $vfCol  = $hdr.cols['vf']
    $vtCol  = $hdr.cols['vt']
    $termsCol = $hdr.cols['terms']
    $noteCol  = $hdr.cols['notes']

    $ctx = $null
    $emitted = 0
    for ($r = $hdr.hdrRow + 1; $r -le $nRows; $r++) {
        $art = G $data $r $artCol
        $qtyRaw = if ($qtyCol) { G $data $r $qtyCol } else { '' }
        $qminRaw = if ($qmiCol) { G $data $r $qmiCol } else { '' }
        $qmaxRaw = if ($qmxCol) { G $data $r $qmxCol } else { '' }

        if (IsArticle $art) {
            $ctx = @{
                article = $art
                fabric  = if ($fabCol) { G $data $r $fabCol } else { '' }
                comp    = if ($comCol) { G $data $r $comCol } else { '' }
                fw      = if ($fwCol)  { G $data $r $fwCol }  else { '' }
                uw      = if ($uwCol)  { G $data $r $uwCol }  else { '' }
                wt      = if ($wtCol)  { G $data $r $wtCol }  else { '' }
                moq     = if ($moqCol) { G $data $r $moqCol } else { '' }
                terms   = if ($termsCol){ G $data $r $termsCol } else { '' }
                vf      = if ($vfCol)  { G $data $r $vfCol }  else { '' }
                vt      = if ($vtCol)  { G $data $r $vtCol }  else { '' }
                notes   = if ($noteCol){ G $data $r $noteCol } else { '' }
            }
        } elseif (-not $ctx) {
            continue  # no context yet and no article on this row
        }

        # figure out qty band
        $qmin = $null; $qmax = $null
        if ($qminRaw -ne '') { $t = 0; if ([int]::TryParse(($qminRaw -replace ',',''), [ref]$t)) { $qmin = $t } }
        if ($qmaxRaw -ne '') { $t = 0; if ([int]::TryParse(($qmaxRaw -replace ',',''), [ref]$t)) { $qmax = $t } }
        if ($null -eq $qmin -and $qtyRaw -ne '') { $qmin,$qmax = ParseQtyBand $qtyRaw }
        if ($null -eq $qmin -and (IsArticle $art)) { $qmin = 1 }   # article row without explicit qty
        if ($null -eq $qmin) { continue }

        # emit for each color col with a numeric price
        foreach ($cc in $hdr.colors) {
            $val = GN $data $r $cc.col
            if ($null -eq $val) { continue }
            $usd = $null; $thb = $null
            if ($cc.currency -eq 'USD') { $usd = $val }
            elseif ($cc.currency -eq 'THB') { $thb = $val }
            else {
                # heuristic: <20 โ’ USD, else THB
                if ($val -lt 20) { $usd = $val } else { $thb = $val }
            }
            [void]$pbRows.Add(@($nextId, $sheetName, $ctx.article, '', $ctx.fabric, $ctx.comp, $ctx.fw, $ctx.uw, $ctx.wt, $ctx.moq, $qmin, $qmax, 'M', $cc.color, $usd, $thb, $ctx.terms, $ctx.vf, $ctx.vt, 'header-parsed', $sheetName, $r, $ctx.notes))
            $script:nextId++
            $emitted++
        }
    }
    return $emitted
}

# --- parser: CENTER PRICE USD for internal ---
# Header at row 14: MOQ | Qty per color | Article | Fabric | Composition | Full W | Useable W | Weight | USD PFE | USD All | THB PFE | THB All
function Parse-CenterUSD($ws, $sheetName) {
    $data, $nRows, $nCols = Read-Grid $ws
    if (-not $data) { return }
    $ctx = $null
    for ($r = 15; $r -le $nRows; $r++) {
        $cB = G $data $r 2
        $cC = G $data $r 3
        if (IsArticle $cC) {
            $ctx = @{
                article = $cC
                fabric  = G $data $r 4
                comp    = G $data $r 5
                fw      = G $data $r 6
                uw      = G $data $r 7
                wt      = G $data $r 8
                moq     = G $data $r 1
            }
            $qty = $cB
        } elseif ($ctx -and $cB) {
            $qty = $cB
        } else {
            if (-not $cB -and -not $cC) { $ctx = $null }
            continue
        }
        $qmin,$qmax = ParseQtyBand $qty
        if ($null -eq $qmin) { continue }
        $usd_pfe = GN $data $r 9
        $usd_all = GN $data $r 10
        $thb_pfe = GN $data $r 11
        $thb_all = GN $data $r 12
        if (-not ($usd_pfe -or $usd_all -or $thb_pfe -or $thb_all)) { continue }
        foreach ($tup in @(@('PFE/PFD',$usd_pfe,$thb_pfe), @('All_colors',$usd_all,$thb_all))) {
            $color,$usd,$thb = $tup
            if ($null -eq $usd -and $null -eq $thb) { continue }
            [void]$pbRows.Add(@($nextId,'CENTER',$ctx.article,'',$ctx.fabric,$ctx.comp,$ctx.fw,$ctx.uw,$ctx.wt,$ctx.moq,$qmin,$qmax,'M',$color,$usd,$thb,'FOB Bangkok','','','Center price sheet',$sheetName,$r,''))
            $script:nextId++
        }
    }
}

# --- fallback: detect any row containing an article code, dump raw cells ---
function Dump-Ambiguous($ws, $sheetName, $reason) {
    $data, $nRows, $nCols = Read-Grid $ws
    if (-not $data) { return }
    $maxRow = [Math]::Min($nRows, 2000)
    $maxCol = [Math]::Min($nCols, 14)
    for ($r = 1; $r -le $maxRow; $r++) {
        $vals = @()
        $hasArticle = $false
        for ($c = 1; $c -le $maxCol; $c++) {
            $t = G $data $r $c
            $vals += $t
            if (-not $hasArticle -and (IsArticle $t)) { $hasArticle = $true }
        }
        if (-not $hasArticle) { continue }
        while ($vals.Count -lt 14) { $vals += '' }
        [void]$ambRows.Add(@($sheetName, $r, $reason) + $vals[0..13])
    }
}

# --- open source, iterate sheets ---
Write-Host "Opening: $SRC"
$excel = New-Object -ComObject Excel.Application
$excel.DisplayAlerts = $false
$excel.ScreenUpdating = $false
try {
    $wb = $excel.Workbooks.Open($SRC, [Type]::Missing, $true)
    $total = $wb.Worksheets.Count
    for ($i = 1; $i -le $total; $i++) {
        $ws = $wb.Worksheets.Item($i)
        $name = $ws.Name
        try {
            if ($name -eq 'CENTER PRICE USD for internal') {
                Parse-CenterUSD $ws $name
                [void]$log.Add("$i/$total [$name] parsed as CENTER USD")
            } else {
                $emitted = Parse-Generic $ws $name
                if ($emitted -gt 0) {
                    [void]$log.Add("$i/$total [$name] header-parsed; emitted $emitted rows")
                } else {
                    Dump-Ambiguous $ws $name 'no-header-detected'
                    [void]$log.Add("$i/$total [$name] no header detected -> ambiguous")
                }
            }
        } catch {
            [void]$log.Add("$i/$total [$name] ERROR: $($_.Exception.Message)")
        }
        if ($i % 10 -eq 0) { Write-Host "processed $i/$total sheets  pb=$($pbRows.Count)  amb=$($ambRows.Count)" }
    }
    $wb.Close($false)
} finally {
    try { if ($excel) { $excel.Quit(); [void][Runtime.InteropServices.Marshal]::ReleaseComObject($excel) } } catch {}
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
}
Write-Host "Extraction done. price_book=$($pbRows.Count)  ambiguous=$($ambRows.Count)"

# --- SAFETY: dump to CSVs immediately so we don't lose data if xlsx write fails ---
$pbCsv  = (Join-Path $PSScriptRoot 'price_book_extracted.pb.csv')
$ambCsv = (Join-Path $PSScriptRoot 'price_book_extracted.amb.csv')
function CsvEsc($v) { if ($null -eq $v) { return '' }; $s = "$v"; if ($s -match '[",\r\n]') { return '"' + $s.Replace('"','""') + '"' }; return $s }
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine(($pbHeaders | ForEach-Object { CsvEsc $_ }) -join ',')
foreach ($row in $pbRows) { [void]$sb.AppendLine(($row | ForEach-Object { CsvEsc $_ }) -join ',') }
[System.IO.File]::WriteAllText($pbCsv, $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine(($ambHeaders | ForEach-Object { CsvEsc $_ }) -join ',')
foreach ($row in $ambRows) { [void]$sb.AppendLine(($row | ForEach-Object { CsvEsc $_ }) -join ',') }
[System.IO.File]::WriteAllText($ambCsv, $sb.ToString(), [System.Text.UTF8Encoding]::new($false))
Write-Host "CSV backups written:`n  $pbCsv`n  $ambCsv"

# --- write output workbook ---
Write-Host "Writing output: $OUT"
$excel = New-Object -ComObject Excel.Application
$excel.DisplayAlerts = $false; $excel.ScreenUpdating = $false
try {
    $wb = $excel.Workbooks.Add()
    while ($wb.Worksheets.Count -gt 1) { $wb.Worksheets.Item($wb.Worksheets.Count).Delete() }

    # -- price_book sheet --
    $pb = $wb.Worksheets.Item(1); $pb.Name = 'price_book'
    $textCols = @(2,3,4,5,6,7,8,9,10,13,14,17,20,21,23)
    foreach ($tc in $textCols) { $pb.Columns.Item($tc).NumberFormat = '@' }
    $pb.Columns.Item(18).NumberFormat = 'yyyy-mm-dd'
    $pb.Columns.Item(19).NumberFormat = 'yyyy-mm-dd'
    $pb.Columns.Item(15).NumberFormat = '0.00'
    $pb.Columns.Item(16).NumberFormat = '0.00'
    for ($c=0; $c -lt $pbHeaders.Count; $c++) { $pb.Cells(1, $c+1).Value2 = $pbHeaders[$c] }
    $pb.Range($pb.Cells(1,1), $pb.Cells(1,$pbHeaders.Count)).Font.Bold = $true
    for ($r=0; $r -lt $pbRows.Count; $r++) {
        for ($c=0; $c -lt $pbHeaders.Count; $c++) {
            $v = $pbRows[$r][$c]
            if ($null -ne $v -and $v -ne '') { $pb.Cells($r+2, $c+1).Value2 = $v }
        }
    }
    $pb.Application.ActiveWindow.SplitRow = 1
    $pb.Application.ActiveWindow.FreezePanes = $true

    # -- ambiguous sheet --
    $amb = $wb.Worksheets.Add([System.Reflection.Missing]::Value, $pb)
    $amb.Name = 'ambiguous'
    foreach ($tc in 1..$ambHeaders.Count) { $amb.Columns.Item($tc).NumberFormat = '@' }
    for ($c=0; $c -lt $ambHeaders.Count; $c++) { $amb.Cells(1, $c+1).Value2 = $ambHeaders[$c] }
    $amb.Range($amb.Cells(1,1), $amb.Cells(1,$ambHeaders.Count)).Font.Bold = $true
    for ($r=0; $r -lt $ambRows.Count; $r++) {
        for ($c=0; $c -lt $ambHeaders.Count; $c++) {
            $v = $ambRows[$r][$c]
            if ($null -ne $v -and $v -ne '') { $amb.Cells($r+2, $c+1).Value2 = $v }
        }
    }

    # -- parse_log sheet --
    $lg = $wb.Worksheets.Add([System.Reflection.Missing]::Value, $amb)
    $lg.Name = 'parse_log'
    $lg.Cells(1,1).Value2 = 'sheet_summary'
    $lg.Cells(1,1).Font.Bold = $true
    for ($r=0; $r -lt $log.Count; $r++) { $lg.Cells($r+2, 1).Value2 = $log[$r] }

    $wb.SaveAs($OUT, 51)
    $wb.Close($false)
} finally {
    try { if ($excel) { $excel.Quit(); [void][Runtime.InteropServices.Marshal]::ReleaseComObject($excel) } } catch {}
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
}
Write-Host "Saved: $OUT"
Write-Host "  price_book rows: $($pbRows.Count)"
Write-Host "  ambiguous rows:  $($ambRows.Count)"

