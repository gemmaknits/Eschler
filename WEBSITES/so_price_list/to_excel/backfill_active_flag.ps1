# Backfill so_price_list_detail.active from the workbook scan.
#
# Joins on header.source_sheet + detail.source_row - the worksheet row number
# the extractor recorded - so every update is traceable back to a cell in the
# original file. Idempotent: re-running sets the same values.

$ErrorActionPreference = 'Stop'
$csvPath = Join-Path $PSScriptRoot 'active_flags.csv'
$cs = 'Server=172.16.3.10;Database=gemmasoft;User Id=sa;Password=sql@min;Connect Timeout=30'

$flags = @(Import-Csv -Path $csvPath)
Write-Host "flag rows from workbook: $($flags.Count)"

$dt = New-Object System.Data.DataTable
[void]$dt.Columns.Add('source_sheet', [string])
[void]$dt.Columns.Add('source_row',   [int])
[void]$dt.Columns.Add('active',       [string])
foreach ($f in $flags) {
    $row = $dt.NewRow()
    $row['source_sheet'] = $f.source_sheet
    $row['source_row']   = [int]$f.source_row
    $row['active']       = $f.active
    $dt.Rows.Add($row)
}

$cn = New-Object System.Data.SqlClient.SqlConnection $cs
$cn.Add_InfoMessage({ param($s,$e) Write-Host "  [sql] $($e.Message)" })
$cn.Open()

# --- stage ---
$cmd = $cn.CreateCommand()
$cmd.CommandText = 'TRUNCATE TABLE SO.so_price_list_active_stage;'
[void]$cmd.ExecuteNonQuery()

$bulk = New-Object System.Data.SqlClient.SqlBulkCopy($cn)
$bulk.DestinationTableName = 'SO.so_price_list_active_stage'
foreach ($c in $dt.Columns) { [void]$bulk.ColumnMappings.Add($c.ColumnName, $c.ColumnName) }
$bulk.WriteToServer($dt)
$bulk.Close()
Write-Host "staged: $($dt.Rows.Count)"

# --- what will match, before changing anything ---
$cmd.CommandText = @"
SELECT s.source_sheet,
       COUNT(*)                                            AS flag_rows,
       SUM(CASE WHEN d.so_price_list_detail_id IS NULL THEN 1 ELSE 0 END) AS no_detail_line,
       SUM(CASE WHEN d.so_price_list_detail_id IS NOT NULL THEN 1 ELSE 0 END) AS will_update,
       SUM(CASE WHEN d.so_price_list_detail_id IS NOT NULL AND s.active='N' THEN 1 ELSE 0 END) AS will_set_N
FROM SO.so_price_list_active_stage s
LEFT JOIN SO.so_price_list_header h
       ON h.source_sheet = s.source_sheet AND h.delete_mark <> 'Y'
LEFT JOIN SO.so_price_list_detail d
       ON d.so_price_list_header_id = h.so_price_list_header_id
      AND d.source_row = s.source_row
      AND d.delete_mark <> 'Y'
GROUP BY s.source_sheet;
"@
$rd = $cmd.ExecuteReader()
Write-Host ""
Write-Host "sheet            flags  no line  will update  -> N"
while ($rd.Read()) {
    "{0,-16} {1,-6} {2,-8} {3,-12} {4}" -f $rd['source_sheet'], $rd['flag_rows'],
        $rd['no_detail_line'], $rd['will_update'], $rd['will_set_N']
}
$rd.Close()

# --- apply ---
$cmd.CommandText = @"
UPDATE d
SET    d.active = s.active
FROM   SO.so_price_list_detail d
JOIN   SO.so_price_list_header h
         ON h.so_price_list_header_id = d.so_price_list_header_id
JOIN   SO.so_price_list_active_stage s
         ON s.source_sheet = h.source_sheet
        AND s.source_row   = d.source_row
WHERE  d.delete_mark <> 'Y' AND h.delete_mark <> 'Y'
  AND  d.active <> s.active;
SELECT @@ROWCOUNT AS rows_changed;
"@
$changed = $cmd.ExecuteScalar()
Write-Host ""
Write-Host "rows changed: $changed"

# --- verify ---
$cmd.CommandText = @"
SELECT h.source_sheet, d.active, COUNT(*) AS lines
FROM SO.so_price_list_detail d
JOIN SO.so_price_list_header h ON h.so_price_list_header_id = d.so_price_list_header_id
WHERE h.source_sheet IN ('Ausco','GLAMORISE') AND d.delete_mark <> 'Y'
GROUP BY h.source_sheet, d.active
ORDER BY h.source_sheet, d.active;
"@
$rd = $cmd.ExecuteReader()
Write-Host ""
Write-Host "resulting split on the two sheets:"
while ($rd.Read()) { "  {0,-14} {1}  {2} lines" -f $rd[0], $rd[1], $rd[2] }
$rd.Close()

$cmd.CommandText = "SELECT active, COUNT(*) FROM SO.so_price_list_detail WHERE delete_mark<>'Y' GROUP BY active"
$rd = $cmd.ExecuteReader()
Write-Host ""
Write-Host "whole table:"
while ($rd.Read()) { "  {0}  {1} lines" -f $rd[0], $rd[1] }
$rd.Close()

$cn.Close()
Write-Host "Done."
