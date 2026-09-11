$ErrorActionPreference = 'Stop'

# Paths follow this script, so the folder can move without edits.
$csv = Join-Path $PSScriptRoot 'price_book_extracted.pb.csv'
$cs  = 'Server=172.16.3.10;Database=gemmasoft;User Id=sa;Password=sql@min;Connect Timeout=30'
$who = 'SURES'

# The CSV is WIDE (price_usd + price_thb on one line, which is how the CENTER
# sheet and the entry surfaces are laid out). dbo.so_price_list is TALL: one row
# per currency. This script is where the fan-out happens.

# --- 1. Load CSV ---
Write-Host "Loading CSV..."
$recs = @(Import-Csv -Path $csv)
Write-Host "  source rows: $($recs.Count)"

# --- 2. Build DataTable matching so_price_list column set (subset we populate) ---
$dt = New-Object System.Data.DataTable
[void]$dt.Columns.Add('customer_excel',   [string])
[void]$dt.Columns.Add('article',          [string])
[void]$dt.Columns.Add('article_variant',  [string])
[void]$dt.Columns.Add('fabric_name',      [string])
[void]$dt.Columns.Add('composition',      [string])
[void]$dt.Columns.Add('full_width_cm',    [string])
[void]$dt.Columns.Add('usable_width_cm',  [string])
[void]$dt.Columns.Add('weight_gsm',       [string])
[void]$dt.Columns.Add('moq',              [string])
[void]$dt.Columns.Add('qty_min',          [int])
[void]$dt.Columns.Add('qty_max',          [int])
[void]$dt.Columns.Add('qty_unit',         [string])
[void]$dt.Columns.Add('color_tier',       [string])
[void]$dt.Columns.Add('currency',         [string])
[void]$dt.Columns.Add('price',            [decimal])
[void]$dt.Columns.Add('terms',            [string])
[void]$dt.Columns.Add('valid_from',       [datetime])
[void]$dt.Columns.Add('valid_to',         [datetime])
[void]$dt.Columns.Add('quote_ref',        [string])
[void]$dt.Columns.Add('source_sheet',     [string])
[void]$dt.Columns.Add('source_row',       [int])
[void]$dt.Columns.Add('notes',            [string])
[void]$dt.Columns.Add('created_by',       [string])

function AsInt($v)    { if ([string]::IsNullOrEmpty($v)) { return [System.DBNull]::Value }; $n=0; if ([int]::TryParse($v,[ref]$n)) { return $n } else { return [System.DBNull]::Value } }
function AsDec($v)    { if ([string]::IsNullOrEmpty($v)) { return [System.DBNull]::Value }; $n=[decimal]0; if ([decimal]::TryParse($v,[ref]$n)) { return $n } else { return [System.DBNull]::Value } }
function AsDate($v)   { if ([string]::IsNullOrEmpty($v)) { return [System.DBNull]::Value }; $d=[datetime]::MinValue; if ([datetime]::TryParse($v,[ref]$d)) { return $d } else { return [System.DBNull]::Value } }
function AsText($v)   { if ([string]::IsNullOrEmpty($v)) { return [System.DBNull]::Value }; return $v }

# Emit one DataTable row for one (source row x currency) pair.
function Add-PriceRow($r, [string]$ccy, $priceVal) {
    $row = $dt.NewRow()
    $row['customer_excel']  = AsText $r.customer
    $row['article']         = AsText $r.article
    $row['article_variant'] = AsText $r.article_variant
    $row['fabric_name']     = AsText $r.fabric_name
    $row['composition']     = AsText $r.composition
    $row['full_width_cm']   = AsText $r.full_width_cm
    $row['usable_width_cm'] = AsText $r.usable_width_cm
    $row['weight_gsm']      = AsText $r.weight_gsm
    $row['moq']             = AsText $r.moq
    $row['qty_min']         = AsInt  $r.qty_min
    $row['qty_max']         = AsInt  $r.qty_max
    $row['qty_unit']        = if ([string]::IsNullOrEmpty($r.qty_unit))   { 'M' }           else { $r.qty_unit }
    $row['color_tier']      = if ([string]::IsNullOrEmpty($r.color_tier)) { 'Unspecified' } else { $r.color_tier }
    $row['currency']        = $ccy
    $row['price']           = $priceVal
    $row['terms']           = AsText $r.terms
    $row['valid_from']      = AsDate $r.valid_from
    $row['valid_to']        = AsDate $r.valid_to
    $row['quote_ref']       = AsText $r.quote_ref
    $row['source_sheet']    = AsText $r.source_sheet
    $row['source_row']      = AsInt  $r.source_row
    $row['notes']           = AsText $r.notes
    $row['created_by']      = $who
    $dt.Rows.Add($row)
}

Write-Host "Building DataTable (wide -> tall)..."
$nUsd = 0; $nThb = 0; $nSkipped = 0
foreach ($r in $recs) {
    $usd = AsDec $r.price_usd
    $thb = AsDec $r.price_thb

    # A source line with neither price carries no price fact - don't invent one.
    if ($usd -is [System.DBNull] -and $thb -is [System.DBNull]) { $nSkipped++; continue }

    if ($usd -isnot [System.DBNull]) { Add-PriceRow $r 'USD' $usd; $nUsd++ }
    if ($thb -isnot [System.DBNull]) { Add-PriceRow $r 'THB' $thb; $nThb++ }
}
Write-Host "  USD rows      : $nUsd"
Write-Host "  THB rows      : $nThb"
Write-Host "  skipped (no price): $nSkipped"
Write-Host "  DataTable rows: $($dt.Rows.Count)"

# --- 3. SqlBulkCopy ---
Write-Host "Bulk-loading into dbo.so_price_list..."
$cn = New-Object System.Data.SqlClient.SqlConnection $cs
$cn.Open()
$bulk = New-Object System.Data.SqlClient.SqlBulkCopy($cn)
$bulk.DestinationTableName = 'dbo.so_price_list'
$bulk.BulkCopyTimeout = 300
$bulk.BatchSize = 1000
foreach ($col in $dt.Columns) {
    [void]$bulk.ColumnMappings.Add($col.ColumnName, $col.ColumnName)
}
$bulk.WriteToServer($dt)
$bulk.Close()

# --- 4. Verify ---
$cmd = $cn.CreateCommand()
$cmd.CommandText = "SELECT currency, COUNT(*) FROM dbo.so_price_list WHERE created_by='$who' GROUP BY currency ORDER BY currency"
$rd = $cmd.ExecuteReader()
while ($rd.Read()) { Write-Host ("  {0} : {1}" -f $rd[0], $rd[1]) }
$rd.Close()
$cmd.CommandText = "SELECT COUNT(*) FROM dbo.so_price_list WHERE created_by='$who'"
Write-Host "so_price_list rows created_by=$who : $($cmd.ExecuteScalar())"
$cn.Close()
Write-Host "Done."
