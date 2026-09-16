$ErrorActionPreference = 'Stop'
$pbCsv  = (Join-Path $PSScriptRoot 'price_book_extracted.pb.csv')
$ambCsv = (Join-Path $PSScriptRoot 'price_book_extracted.amb.csv')
$OUT    = (Join-Path $PSScriptRoot 'price_book_extracted.xlsx')
if (Test-Path $OUT) { Remove-Item $OUT -Force }

# per-column type: 'n' = numeric, 't' = text โ€” indexed by column position (1-based)
$pbTypes  = @('n','t','t','t','t','t','t','t','t','t','n','n','t','t','n','n','t','t','t','t','t','n','t')
$ambTypes = @('t','n','t','t','t','t','t','t','t','t','t','t','t','t','t','t','t')

function Write-Sheet($ws, $records, $types, $headers) {
    # set column formats FIRST so numeric-looking strings in text cols preserve
    for ($c = 0; $c -lt $types.Count; $c++) {
        $ws.Columns.Item($c+1).NumberFormat = if ($types[$c] -eq 't') { '@' } else { 'General' }
    }
    for ($c = 0; $c -lt $headers.Count; $c++) { $ws.Cells(1, $c+1).Value2 = $headers[$c] }
    $ws.Range($ws.Cells(1,1), $ws.Cells(1,$headers.Count)).Font.Bold = $true

    $nRows = $records.Count
    $nCols = $headers.Count
    if ($nRows -eq 0) { return }

    for ($r = 0; $r -lt $nRows; $r++) {
        $rec = $records[$r]
        for ($c = 0; $c -lt $nCols; $c++) {
            $v = $rec.($headers[$c])
            if ($null -eq $v -or $v -eq '') { continue }
            # always assign as string; text-formatted ('@') cells keep it text,
            # General-formatted cells auto-convert numeric-looking strings to numbers
            $ws.Cells($r+2, $c+1).Value2 = [string]$v
        }
        if (($r+1) % 250 -eq 0) { Write-Host "  wrote $($r+1)/$nRows rows" }
    }
}

Write-Host "Loading CSVs..."
$pbRecs  = @(Import-Csv -Path $pbCsv)
$ambRecs = @(Import-Csv -Path $ambCsv)
Write-Host "  pb : $($pbRecs.Count) rows"
Write-Host "  amb: $($ambRecs.Count) rows"
$pbHeaders  = $pbRecs[0].PSObject.Properties.Name
$ambHeaders = $ambRecs[0].PSObject.Properties.Name

Write-Host "Building workbook..."
$excel = New-Object -ComObject Excel.Application
$excel.DisplayAlerts = $false; $excel.ScreenUpdating = $false
try {
    $wb = $excel.Workbooks.Add()
    while ($wb.Worksheets.Count -gt 1) { $wb.Worksheets.Item($wb.Worksheets.Count).Delete() }

    $pb = $wb.Worksheets.Item(1); $pb.Name = 'price_book'
    Write-Host "-- price_book sheet --"
    Write-Sheet $pb $pbRecs $pbTypes $pbHeaders
    $pb.Application.ActiveWindow.SplitRow = 1
    $pb.Application.ActiveWindow.FreezePanes = $true

    $amb = $wb.Worksheets.Add([System.Reflection.Missing]::Value, $pb)
    $amb.Name = 'ambiguous'
    Write-Host "-- ambiguous sheet --"
    Write-Sheet $amb $ambRecs $ambTypes $ambHeaders

    $wb.SaveAs($OUT, 51)
    $wb.Close($false)
    Write-Host "Saved: $OUT"
} finally {
    try { if ($excel) { $excel.Quit(); [void][Runtime.InteropServices.Marshal]::ReleaseComObject($excel) } } catch {}
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
}

