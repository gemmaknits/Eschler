# Scan the source workbook for an ACTIVE (Y/N) column and emit one row per
# (sheet, excel row, active) so it can be backfilled onto so_price_list_detail
# by source_sheet + source_row.
#
# The extractor recorded source_row as the worksheet row number, so this is an
# exact join - no guessing, and it can be re-run and re-verified at will.

$ErrorActionPreference = 'Stop'
$SRC = Join-Path $PSScriptRoot 'Eschler_Updated Special Price list_2025_unlock.xlsx'
$OUT = Join-Path $PSScriptRoot 'active_flags.csv'

$xl = New-Object -ComObject Excel.Application
$xl.Visible = $false; $xl.DisplayAlerts = $false; $xl.EnableEvents = $false

$rows = New-Object Collections.Generic.List[object]
$summary = New-Object Collections.Generic.List[object]

try {
    $wb = $xl.Workbooks.Open($SRC, $false, $true)   # read-only
    Write-Host "sheets: $($wb.Worksheets.Count)"

    foreach ($ws in $wb.Worksheets) {
        $name = $ws.Name
        $ur = $ws.UsedRange
        $nRows = $ur.Rows.Count
        $nCols = [Math]::Min($ur.Columns.Count, 40)
        if ($nRows -lt 2 -or $nCols -lt 1) { continue }

        $arr = $ur.Value2
        if ($null -eq $arr) { continue }

        # Find a header cell that says ACTIVE, within the first 6 rows.
        $activeCol = 0; $headerRow = 0
        for ($r = 1; $r -le [Math]::Min(6, $nRows) -and $activeCol -eq 0; $r++) {
            for ($c = 1; $c -le $nCols; $c++) {
                $v = $arr[$r, $c]
                if ($null -ne $v) {
                    $s = ([string]$v).Trim().ToUpperInvariant()
                    if ($s -eq 'ACTIVE' -or $s -eq 'ACTIVE?' -or $s -eq 'ACTIVE Y/N') {
                        $activeCol = $c; $headerRow = $r; break
                    }
                }
            }
        }
        if ($activeCol -eq 0) {
            [void]$summary.Add([pscustomobject]@{ sheet=$name; active_col=''; y=0; n=0; other=0; note='no ACTIVE column' })
            continue
        }

        $y = 0; $n = 0; $other = 0
        for ($r = $headerRow + 1; $r -le $nRows; $r++) {
            $v = $arr[$r, $activeCol]
            if ($null -eq $v) { continue }
            $s = ([string]$v).Trim().ToUpperInvariant()
            if ($s -eq '') { continue }

            $flag = $null
            if     ($s -eq 'Y' -or $s -eq 'YES' -or $s -eq 'A' -or $s -eq 'ACTIVE')   { $flag = 'Y'; $y++ }
            elseif ($s -eq 'N' -or $s -eq 'NO'  -or $s -eq 'INACTIVE')                { $flag = 'N'; $n++ }
            else { $other++; continue }   # header repeats, stray notes - skip

            $rows.Add([pscustomobject]@{
                source_sheet = $name
                source_row   = $r
                active       = $flag
            })
        }
        [void]$summary.Add([pscustomobject]@{
            sheet=$name; active_col=$activeCol; y=$y; n=$n; other=$other; note='' })
        Write-Host ("  {0,-38} col {1,-3} Y={2,-5} N={3,-4} skipped={4}" -f $name, $activeCol, $y, $n, $other)
    }
    $wb.Close($false)
}
finally {
    $xl.Quit()
    [void][Runtime.InteropServices.Marshal]::ReleaseComObject($xl)
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
}

$rows | Export-Csv -Path $OUT -NoTypeInformation -Encoding UTF8

Write-Host ""
Write-Host "sheets WITH an ACTIVE column:"
$summary | Where-Object { $_.active_col -ne '' } |
    ForEach-Object { "  {0,-38} Y={1,-5} N={2}" -f $_.sheet, $_.y, $_.n }
Write-Host ""
Write-Host "flag rows written : $($rows.Count)  -> $OUT"
Write-Host "sheets with ACTIVE: $(($summary | Where-Object { $_.active_col -ne '' }).Count) of $($summary.Count)"
