# Export so_price_list_header/detail as JSON for the spreadsheet mock UI.
#
# The DB is TALL (one row per currency); the grid is WIDE (colour tiers as
# columns, once under USD and once under THB). This pivots tall -> wide.
#
# Where a wide cell maps to more than one detail row - the 148 conflicting
# price groups - the cell keeps ALL of them so the UI can show the conflict
# and let the user pick, instead of silently dropping one.

$ErrorActionPreference = 'Stop'
$cs  = 'Server=172.16.3.10;Database=gemmasoft;User Id=sa;Password=sql@min;Connect Timeout=30'
$out = Join-Path $PSScriptRoot '..\mock_ui\grid_data.json'

$cn = New-Object System.Data.SqlClient.SqlConnection $cs
$cn.Open()

# --- headers ---
$cmd = $cn.CreateCommand()
$cmd.CommandText = @"
SELECT so_price_list_header_id, list_name, ISNULL(list_desc,'') AS list_desc,
       ISNULL(CONVERT(varchar(10),valid_from,120),'') AS valid_from,
       ISNULL(CONVERT(varchar(10),valid_to,120),'')   AS valid_to,
       ISNULL(terms,'') AS terms, ISNULL(quote_ref,'') AS quote_ref,
       ISNULL(customer_excel,'') AS customer_excel,
       ISNULL(CAST(customer_id AS varchar(20)),'') AS customer_id,
       ISNULL(customer_name,'') AS customer_name,
       ISNULL(source_sheet,'') AS source_sheet
FROM dbo.so_price_list_header
WHERE delete_mark <> 'Y'
ORDER BY list_name;
"@
$headers = @{}
$hOrder = New-Object Collections.Generic.List[object]
$rd = $cmd.ExecuteReader()
while ($rd.Read()) {
    $h = [ordered]@{
        id = [int]$rd['so_price_list_header_id']
        list_name = "$($rd['list_name'])"; list_desc = "$($rd['list_desc'])"
        valid_from = "$($rd['valid_from'])"; valid_to = "$($rd['valid_to'])"
        terms = "$($rd['terms'])"; quote_ref = "$($rd['quote_ref'])"
        customer_excel = "$($rd['customer_excel'])"
        customer_id = "$($rd['customer_id'])"; customer_name = "$($rd['customer_name'])"
        source_sheet = "$($rd['source_sheet'])"
        tiers = @(); rows = @()
    }
    $headers[$h.id] = $h
    $hOrder.Add($h)
}
$rd.Close()

# --- details ---
$cmd = $cn.CreateCommand()
$cmd.CommandText = @"
SELECT so_price_list_detail_id, so_price_list_header_id, line_no,
       ISNULL(article,'') AS article, ISNULL(article_variant,'') AS article_variant,
       ISNULL(fabric_name,'') AS fabric_name, ISNULL(composition,'') AS composition,
       ISNULL(full_width_cm,'') AS full_width_cm, ISNULL(usable_width_cm,'') AS usable_width_cm,
       ISNULL(weight_gsm,'') AS weight_gsm, ISNULL(moq,'') AS moq,
       qty_min, ISNULL(CAST(qty_max AS varchar(20)),'') AS qty_max, qty_unit,
       color_tier, currency, price, ISNULL(source_row,0) AS source_row
FROM dbo.so_price_list_detail
WHERE delete_mark <> 'Y'
ORDER BY so_price_list_header_id, line_no;
"@
$rd = $cmd.ExecuteReader()
$wide = @{}          # header_id -> (rowkey -> wide row)
$tierSeen = @{}      # header_id -> (tier -> true)
$nDetail = 0
while ($rd.Read()) {
    $nDetail++
    $hid  = [int]$rd['so_price_list_header_id']
    $art  = "$($rd['article'])"
    $vr   = "$($rd['article_variant'])"
    $qmin = [int]$rd['qty_min']
    $qmax = "$($rd['qty_max'])"
    $qu   = "$($rd['qty_unit'])".Trim()
    $tier = "$($rd['color_tier'])"
    $ccy  = "$($rd['currency'])".Trim()
    $key  = "$art`u{1}$vr`u{1}$qmin`u{1}$qmax`u{1}$qu"

    if (-not $wide.ContainsKey($hid))     { $wide[$hid] = @{} }
    if (-not $tierSeen.ContainsKey($hid)) { $tierSeen[$hid] = @{} }
    $tierSeen[$hid][$tier] = $true

    if (-not $wide[$hid].ContainsKey($key)) {
        $wide[$hid][$key] = [ordered]@{
            art = $art; var = $vr; qmin = $qmin; qmax = $qmax; qu = $qu
            fab = "$($rd['fabric_name'])"; comp = "$($rd['composition'])"
            fw  = "$($rd['full_width_cm'])"; uw = "$($rd['usable_width_cm'])"
            wt  = "$($rd['weight_gsm'])";    moq = "$($rd['moq'])"
            src = [int]$rd['source_row']
            cells = @{}   # "USD|PFE/PFD" -> [ {id, price, src}, ... ]
        }
    }
    $cellKey = "$ccy|$tier"
    $row = $wide[$hid][$key]
    if (-not $row.cells.ContainsKey($cellKey)) { $row.cells[$cellKey] = New-Object Collections.Generic.List[object] }
    $row.cells[$cellKey].Add([ordered]@{
        id    = [int]$rd['so_price_list_detail_id']
        price = [double]$rd['price']
        src   = [int]$rd['source_row']
    })
}
$rd.Close()
$cn.Close()

# --- assemble, preserving a sensible tier column order ---
$TierOrder = @('PFE/PFD','PFE','PFD','Greige','White','Light','Medium','Dark','All_colors','Price')
foreach ($h in $hOrder) {
    $seen = @()
    if ($tierSeen.ContainsKey($h.id)) { $seen = @($tierSeen[$h.id].Keys) }
    $h.tiers = @($TierOrder | Where-Object { $seen -contains $_ }) +
               @($seen | Where-Object { $TierOrder -notcontains $_ } | Sort-Object)
    if ($wide.ContainsKey($h.id)) {
        $h.rows = @($wide[$h.id].Values | Sort-Object @{E={$_.art}}, @{E={[int]$_.qmin}})
    }
}

$payload = [ordered]@{
    generated  = (Get-Date).ToString('yyyy-MM-dd HH:mm')
    n_headers  = $hOrder.Count
    n_details  = $nDetail
    lists      = $hOrder
}
$json = $payload | ConvertTo-Json -Depth 12 -Compress
Set-Content -LiteralPath $out -Value $json -Encoding UTF8

Write-Host "headers : $($hOrder.Count)"
Write-Host "details : $nDetail"
Write-Host "wide rows: $(($hOrder | ForEach-Object { $_.rows.Count } | Measure-Object -Sum).Sum)"
Write-Host "wrote   : $out  ($([math]::Round((Get-Item $out).Length/1KB,1)) KB)"
