# Suggest customer_id for each distinct so_price_list.customer_excel by fuzzy-matching
# against customers.name / name2 / namet.
#
# Writes a review file (customer_map_review.csv). Changes NOTHING in the database -
# the mapping is the user's call; this only ranks candidates.

# -ParentOnly restricts candidates to parent_customer_flag='Y', dropping the
# branch / ship-to duplicate records that make most same-company contests look
# ambiguous when they are not.
param([switch]$ParentOnly)

$ErrorActionPreference = 'Stop'
$cs  = 'Server=172.16.3.10;Database=gemmasoft;User Id=sa;Password=sql@min;Connect Timeout=30'
$out = Join-Path $PSScriptRoot 'customer_map_review.csv'

# Sheets that are price *sources*, not customers. The lookup query treats
# customer_id IS NULL as the CENTER-style fallback row, so these stay unmapped.
$NotCustomers = @(
    'CENTER',
    'CENTER PRICE THB',
    'PL Recycle fabric 11 Nov 2022',
    'special price for EMB customer'
)

# Corporate boilerplate - carries no identifying signal.
$StopTokens = @(
    'CO','LTD','LTDS','LIMITED','COMPANY','COMPANIES','INC','CORP','CORPORATION',
    'GMBH','AG','KG','BV','NV','SA','SAS','SARL','SPA','SRL','PLC','PCL','PUBLIC',
    'PVT','PTE','PT','LLC','MFG','MANUFACTURING','INDUSTRIAL','INDUSTRIES',
    'GROUP','HOLDING','HOLDINGS','HEAD','OFFICE','BRANCH','THE','AND'
) | ForEach-Object { $_ } | Group-Object | ForEach-Object { $_.Name }
$StopSet = @{}; foreach ($t in $StopTokens) { $StopSet[$t] = $true }

# Noise on the workbook side: article codes, "art.", sheet-title chatter, dates.
$ExcelNoise = @('ART','PRICE','SPECIAL','FOR','CUSTOMER','UPDATE','RECORD','FABRIC',
                'RECYCLE','JAN','FEB','MAR','APR','MAY','JUN','JUL','AUG','SEP','OCT','NOV','DEC')
$NoiseSet = @{}; foreach ($t in $ExcelNoise) { $NoiseSet[$t] = $true }

# Country words in a sheet name are highly discriminative - "Liberty Vietnam" and
# "Triumph Hong Kong" name a specific legal entity within a group. Without this,
# raw SO volume drags them to whichever group member trades most.
$GeoToCtry = @{
    'THAILAND'='TH'; 'THAI'='TH'; 'GERMANY'='GY'; 'GERMAN'='GY'; 'DEUTSCHLAND'='GY'
    'VIETNAM'='VN'; 'VIETNAMESE'='VN'; 'HONGKONG'='HK'; 'HK'='HK'
    'CHINA'='CH'; 'CHINESE'='CH'; 'INDIA'='IN'; 'JAPAN'='JP'
    'INDONESIA'='IA'; 'PHILIPPINES'='PH'; 'TAIWAN'='TW'; 'FRANCE'='FR'
    'AMERICA'='US'; 'USA'='US'; 'LANKA'='SL'; 'SRILANKA'='SL'
    'CAMBODIA'='KH'; 'TURKEY'='TU'; 'BANGLADESH'='BD'; 'ETHIOPIA'='ET'
}

# Pull country codes implied by a raw name ("Hong Kong" spans two tokens).
function GeoCodes([string]$norm) {
    $found = @{}
    $squashed = $norm -replace ' ', ''
    foreach ($k in $GeoToCtry.Keys) {
        if ($squashed -like "*$k*") { $found[$GeoToCtry[$k]] = $true }
    }
    return @($found.Keys)
}

function Normalize([string]$s) {
    if ([string]::IsNullOrWhiteSpace($s)) { return '' }
    # strip diacritics
    $n = $s.Normalize([Text.NormalizationForm]::FormD)
    $sb = New-Object Text.StringBuilder
    foreach ($ch in $n.ToCharArray()) {
        if ([Globalization.CharUnicodeInfo]::GetUnicodeCategory($ch) -ne [Globalization.UnicodeCategory]::NonSpacingMark) {
            [void]$sb.Append($ch)
        }
    }
    $t = $sb.ToString().ToUpperInvariant()
    $t = $t -replace '&', ' AND '
    $t = $t -replace '[^A-Z0-9]', ' '
    return ($t -replace '\s+', ' ').Trim()
}

function Tokens([string]$norm, [bool]$isExcel) {
    if ($norm -eq '') { return @() }
    $out = New-Object Collections.Generic.List[string]
    foreach ($w in $norm.Split(' ')) {
        if ($w -eq '') { continue }
        if ($StopSet.ContainsKey($w)) { continue }
        if ($isExcel) {
            if ($NoiseSet.ContainsKey($w)) { continue }
            # article codes: any token containing a digit and >=3 chars
            if ($w.Length -ge 3 -and $w -match '\d') { continue }
        }
        $out.Add($w)
    }
    return $out.ToArray()
}

function Levenshtein([string]$a, [string]$b) {
    if ($a -eq $b) { return 0 }
    if ($a.Length -eq 0) { return $b.Length }
    if ($b.Length -eq 0) { return $a.Length }
    $prev = New-Object 'int[]' ($b.Length + 1)
    $cur  = New-Object 'int[]' ($b.Length + 1)
    for ($j = 0; $j -le $b.Length; $j++) { $prev[$j] = $j }
    for ($i = 1; $i -le $a.Length; $i++) {
        $cur[0] = $i
        for ($j = 1; $j -le $b.Length; $j++) {
            $cost = if ($a[$i-1] -eq $b[$j-1]) { 0 } else { 1 }
            $d = $prev[$j] + 1
            $ins = $cur[$j-1] + 1
            if ($ins -lt $d) { $d = $ins }
            $sub = $prev[$j-1] + $cost
            if ($sub -lt $d) { $d = $sub }
            $cur[$j] = $d
        }
        $tmp = $prev; $prev = $cur; $cur = $tmp
    }
    return $prev[$b.Length]
}

function LevRatio([string]$a, [string]$b) {
    $m = [Math]::Max($a.Length, $b.Length)
    if ($m -eq 0) { return 0.0 }
    return 1.0 - ([double](Levenshtein $a $b) / $m)
}

# Score one workbook name against one candidate customer name. 0..100.
function Score($xNorm, $xTok, $cNorm, $cTok) {
    if ($cNorm -eq '' -or $xNorm -eq '') { return 0.0 }

    if ($xNorm -eq $cNorm) { return 100.0 }

    $xs = ($xTok | Sort-Object) -join ' '
    $cs2 = ($cTok | Sort-Object) -join ' '
    if ($xs -ne '' -and $xs -eq $cs2) { return 97.0 }

    $best = 0.0

    if ($xTok.Count -gt 0 -and $cTok.Count -gt 0) {
        $xset = @{}; foreach ($t in $xTok) { $xset[$t] = $true }
        $cset = @{}; foreach ($t in $cTok) { $cset[$t] = $true }
        $inter = 0
        foreach ($k in $xset.Keys) { if ($cset.ContainsKey($k)) { $inter++ } }
        $union = $xset.Count + $cset.Count - $inter

        # every workbook token present in the candidate -> strong containment
        if ($inter -eq $xset.Count) {
            $ratio = [double]$xset.Count / [double]$cset.Count
            $s = 84.0 + (10.0 * $ratio)
            if ($s -gt $best) { $best = $s }
        }
        if ($union -gt 0) {
            $s = 72.0 * ([double]$inter / [double]$union)
            if ($s -gt $best) { $best = $s }
        }
        # first significant token agreeing is a meaningful signal
        if ($xTok[0] -eq $cTok[0]) {
            $s = 60.0 + (20.0 * ([double]$inter / [double][Math]::Max($xset.Count,$cset.Count)))
            if ($s -gt $best) { $best = $s }
        }
    }

    # raw string similarity, both on full and de-boilerplated forms
    $s = 70.0 * (LevRatio $xNorm $cNorm); if ($s -gt $best) { $best = $s }
    if ($xs -ne '' -and $cs2 -ne '') {
        $s = 74.0 * (LevRatio $xs $cs2); if ($s -gt $best) { $best = $s }
    }
    return $best
}

# --- load ---
Write-Host "Loading customers + customer_excel values..."
$cn = New-Object System.Data.SqlClient.SqlConnection $cs
$cn.Open()

# dbo.so links to customers by custcd (char(5)), NOT customer_id. Real order
# history is a far stronger disambiguator than name similarity when several
# customer rows share a name, so carry it alongside each candidate.
$cust = New-Object Collections.Generic.List[object]
$cmd = $cn.CreateCommand()
$cmd.CommandText = @"
SELECT c.customer_id, c.custcd, ISNULL(c.name,'') AS name, ISNULL(c.name2,'') AS name2,
       ISNULL(c.namet,'') AS namet, ISNULL(c.ctry,'') AS ctry, ISNULL(c.city,'') AS city,
       ISNULL(c.bill_to_flag,'') AS bill_to, c.active,
       ISNULL(s.so_orders, 0) AS so_orders, s.last_so
FROM dbo.customers c
LEFT JOIN (
    SELECT custcd, COUNT(*) AS so_orders, MAX(sodt) AS last_so
    FROM dbo.so GROUP BY custcd
) s ON s.custcd = c.custcd
WHERE (@parentOnly = 0 OR c.parent_customer_flag = 'Y')
"@
[void]$cmd.Parameters.AddWithValue('@parentOnly', $(if ($ParentOnly) { 1 } else { 0 }))
$rd = $cmd.ExecuteReader()
while ($rd.Read()) {
    $cust.Add([pscustomobject]@{
        customer_id = [int64]$rd['customer_id']; custcd = "$($rd['custcd'])".Trim()
        name = "$($rd['name'])".Trim(); name2 = "$($rd['name2'])".Trim(); namet = "$($rd['namet'])".Trim()
        ctry = "$($rd['ctry'])".Trim(); city = "$($rd['city'])".Trim()
        bill_to = "$($rd['bill_to'])".Trim(); active = $rd['active']
        so_orders = [int]$rd['so_orders']
        last_so = $(if ($rd['last_so'] -is [DBNull]) { '' } else { ([datetime]$rd['last_so']).ToString('yyyy-MM-dd') })
    })
}
$rd.Close()

$excel = New-Object Collections.Generic.List[object]
$cmd = $cn.CreateCommand()
$cmd.CommandText = "SELECT customer_excel, COUNT(*) AS n FROM dbo.so_price_list WHERE customer_excel IS NOT NULL GROUP BY customer_excel"
$rd = $cmd.ExecuteReader()
while ($rd.Read()) { $excel.Add([pscustomobject]@{ customer_excel = "$($rd[0])"; rows = [int]$rd[1] }) }
$rd.Close()
$cn.Close()

Write-Host "  customers      : $($cust.Count)"
Write-Host "  customer_excel : $($excel.Count)"

# precompute candidate normal forms (name / name2 / namet each tried)
foreach ($c in $cust) {
    $c | Add-Member -NotePropertyName forms -NotePropertyValue @(
        foreach ($f in @($c.name, $c.name2, $c.namet)) {
            if (-not [string]::IsNullOrWhiteSpace($f)) {
                $nf = Normalize $f
                [pscustomobject]@{ raw = $f; norm = $nf; tok = (Tokens $nf $false) }
            }
        }
    )
}

# --- match ---
$results = New-Object Collections.Generic.List[object]
foreach ($x in ($excel | Sort-Object -Property rows -Descending)) {
    $isNotCust = $NotCustomers -contains $x.customer_excel
    $xNorm = Normalize $x.customer_excel
    $xTok  = Tokens $xNorm $true

    $xGeo = GeoCodes $xNorm

    $scored = New-Object Collections.Generic.List[object]
    foreach ($c in $cust) {
        $best = 0.0; $via = ''
        foreach ($f in $c.forms) {
            $s = Score $xNorm $xTok $f.norm $f.tok
            if ($s -gt $best) { $best = $s; $via = $f.raw }
        }
        if ($best -le 0) { continue }

        # Geography agreement, when the sheet name states one at all.
        $geoNote = ''
        if ($xGeo.Count -gt 0) {
            $cGeo = @(GeoCodes (Normalize "$($c.name) $($c.name2)"))
            if ($xGeo -contains $c.ctry -or ($cGeo | Where-Object { $xGeo -contains $_ })) {
                $best += 9.0; $geoNote = "ctry+"
            } else {
                $best -= 7.0; $geoNote = "ctry-"
            }
            if ($best -lt 0) { $best = 0.0 }
            if ($best -gt 100) { $best = 100.0 }
        }
        $scored.Add([pscustomobject]@{ cust = $c; score = $best; via = $via; geo = $geoNote })
    }

    # Rank on name score first, but treat order history as the tiebreaker: among
    # near-equal names (same-name branch rows), the one actually used on sales
    # orders is the one the price list means. Bucket the score so a 1-2 point
    # scoring artefact can't outrank real SO usage.
    $top = $scored | Sort-Object -Property `
                @{Expression={ [math]::Floor($_.score / 5) }; Descending=$true},
                @{Expression={ $_.cust.so_orders }; Descending=$true},
                @{Expression='score'; Descending=$true},
                @{Expression={ if ($_.cust.bill_to -eq 'Y') {0} else {1} }},
                @{Expression={ $_.cust.customer_id }} |
           Select-Object -First 3

    $i = 0
    foreach ($t in $top) {
        $i++
        $results.Add([pscustomobject]@{
            customer_excel = $x.customer_excel
            rows           = $x.rows
            rank           = $i
            score          = [math]::Round($t.score, 1)
            customer_id    = $t.cust.customer_id
            custcd         = $t.cust.custcd
            customer_name  = $t.cust.name
            matched_via    = $t.via
            ctry           = $t.cust.ctry
            city           = $t.cust.city
            bill_to        = $t.cust.bill_to
            so_orders      = $t.cust.so_orders
            last_so        = $t.cust.last_so
            geo            = $t.geo
            verdict        = if ($isNotCust) { 'NOT_A_CUSTOMER' }
                             elseif ($i -gt 1) { '' }
                             elseif ($t.score -ge 97) { 'AUTO' }
                             elseif ($t.score -ge 85) { 'LIKELY' }
                             elseif ($t.score -ge 70) { 'REVIEW' }
                             else { 'WEAK' }
        })
    }
    if ($top.Count -eq 0) {
        $results.Add([pscustomobject]@{
            customer_excel = $x.customer_excel; rows = $x.rows; rank = 1; score = 0
            customer_id = ''; custcd = ''; customer_name = ''; matched_via = ''
            ctry = ''; city = ''; bill_to = ''; so_orders = ''; last_so = ''; geo = ''
            verdict = if ($isNotCust) { 'NOT_A_CUSTOMER' } else { 'NO_MATCH' }
        })
    }
}

$results | Export-Csv -Path $out -NoTypeInformation -Encoding UTF8
Write-Host ""
Write-Host "Wrote $out"

# --- console summary: best candidate per workbook name ---
"{0,-31} {1,-5} {2,-5} {3,-8} {4,-6} {5,-11} {6}" -f 'customer_excel','rows','score','verdict','SOs','last SO','best candidate'
"-" * 130
foreach ($g in ($results | Where-Object { $_.rank -eq 1 })) {
    "{0,-31} {1,-5} {2,-5} {3,-8} {4,-6} {5,-11} {6}" -f `
        $g.customer_excel.Substring(0,[Math]::Min(31,$g.customer_excel.Length)),
        $g.rows, $g.score, $g.verdict, $g.so_orders, $g.last_so,
        $(if ($g.customer_id -ne '') { "[$($g.customer_id)] $($g.custcd) $($g.customer_name)" } else { '-' })
}
""
"verdict counts:"
$results | Where-Object { $_.rank -eq 1 } | Group-Object verdict |
    Sort-Object Count -Descending | ForEach-Object { "  {0,-15} {1}" -f $_.Name, $_.Count }


