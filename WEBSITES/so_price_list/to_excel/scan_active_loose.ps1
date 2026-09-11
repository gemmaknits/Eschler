# Broader sweep: any header containing "ACTIV" anywhere in the first 8 rows,
# OR any column whose values are overwhelmingly Y/N. Catches sheets that label
# the flag differently, or do not label it at all.

$ErrorActionPreference = 'Stop'
$SRC = Join-Path $PSScriptRoot 'Eschler_Updated Special Price list_2025_unlock.xlsx'

$xl = New-Object -ComObject Excel.Application
$xl.Visible = $false; $xl.DisplayAlerts = $false; $xl.EnableEvents = $false
$hits = New-Object Collections.Generic.List[object]

try {
    $wb = $xl.Workbooks.Open($SRC, $false, $true)
    foreach ($ws in $wb.Worksheets) {
        $name = $ws.Name
        $ur = $ws.UsedRange
        $nRows = [Math]::Min($ur.Rows.Count, 400)     # enough to judge a column
        $nCols = [Math]::Min($ur.Columns.Count, 40)
        if ($nRows -lt 3 -or $nCols -lt 1) { continue }
        $arr = $ur.Value2
        if ($null -eq $arr) { continue }

        # (a) header text containing ACTIV
        for ($r = 1; $r -le [Math]::Min(8, $nRows); $r++) {
            for ($c = 1; $c -le $nCols; $c++) {
                $v = $arr[$r, $c]
                if ($null -ne $v) {
                    $s = ([string]$v).Trim().ToUpperInvariant()
                    if ($s -like '*ACTIV*') {
                        $hits.Add([pscustomobject]@{ sheet=$name; kind='header'; col=$c; row=$r; detail=$s })
                    }
                }
            }
        }

        # (b) a column that is mostly bare Y/N values
        for ($c = 1; $c -le $nCols; $c++) {
            $yn = 0; $filled = 0
            for ($r = 1; $r -le $nRows; $r++) {
                $v = $arr[$r, $c]
                if ($null -eq $v) { continue }
                $s = ([string]$v).Trim().ToUpperInvariant()
                if ($s -eq '') { continue }
                $filled++
                if ($s -eq 'Y' -or $s -eq 'N' -or $s -eq 'YES' -or $s -eq 'NO') { $yn++ }
            }
            if ($filled -ge 5 -and $yn / [double]$filled -ge 0.8) {
                $hits.Add([pscustomobject]@{
                    sheet=$name; kind='Y/N column'; col=$c; row=0
                    detail="$yn of $filled values are Y/N" })
            }
        }
    }
    $wb.Close($false)
}
finally {
    $xl.Quit()
    [void][Runtime.InteropServices.Marshal]::ReleaseComObject($xl)
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
}

Write-Host "--- candidates ---"
$hits | Sort-Object sheet, kind, col | ForEach-Object {
    "  {0,-38} {1,-11} col {2,-3} {3}" -f $_.sheet, $_.kind, $_.col, $_.detail
}
Write-Host ""
Write-Host "distinct sheets flagged: $(($hits | Select-Object -ExpandProperty sheet -Unique).Count)"
