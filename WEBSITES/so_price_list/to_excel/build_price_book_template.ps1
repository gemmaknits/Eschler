$ErrorActionPreference='Stop'
$out = (Join-Path $PSScriptRoot 'price_book_template.xlsx')
if (Test-Path $out) { Remove-Item $out -Force }

$headers = @(
 'price_id','customer','article','article_variant','fabric_name','composition',
 'full_width_cm','usable_width_cm','weight_gsm','moq',
 'qty_min','qty_max','qty_unit','color_tier',
 'price_usd','price_thb','terms',
 'valid_from','valid_to','quote_ref','source_sheet','notes'
)

# helper to build a row
function Row($id,$cust,$art,$var,$fab,$comp,$fw,$uw,$wt,$moq,$qmin,$qmax,$qu,$ct,$usd,$thb,$terms,$vf,$vt,$qr,$src,$note){
    ,@($id,$cust,$art,$var,$fab,$comp,$fw,$uw,$wt,$moq,$qmin,$qmax,$qu,$ct,$usd,$thb,$terms,$vf,$vt,$qr,$src,$note)
}

$fab_pq  = 'CO/PA PIQUET'; $com_pq='59% CO, 41% PA'; $fw_pq='160-165'; $uw_pq='155-160'; $wt_pq='65-70'
$terms_ct='FOB Bangkok';  $terms_an='FOB Bangkok / DDP excl VAT'
$vf_ct='2020-03-04';      $vf_an='2013-01-01'; $vt_an='2015-12-31'
$note_ct='Center price 4-Mar-2020'; $src_ct='CENTER_Main'; $src_an='ANITA_Thailand_Germany'

$rows = [System.Collections.ArrayList]::new()
# ---- CENTER 256524 ----
[void]$rows.Add((Row  1 'CENTER' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq '3,000 m'  200  600 'M' 'PFE/PFD'    3.05 $null $terms_ct $vf_ct $null $note_ct $src_ct ''))
[void]$rows.Add((Row  2 'CENTER' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq '3,000 m'  200  600 'M' 'All_colors' 3.85 $null $terms_ct $vf_ct $null $note_ct $src_ct ''))
[void]$rows.Add((Row  3 'CENTER' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq '3,000 m'  601 2000 'M' 'PFE/PFD'    2.75 $null $terms_ct $vf_ct $null $note_ct $src_ct ''))
[void]$rows.Add((Row  4 'CENTER' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq '3,000 m'  601 2000 'M' 'All_colors' 3.55 $null $terms_ct $vf_ct $null $note_ct $src_ct ''))
[void]$rows.Add((Row  5 'CENTER' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq '3,000 m' 2001 5000 'M' 'PFE/PFD'    2.65 $null $terms_ct $vf_ct $null $note_ct $src_ct ''))
[void]$rows.Add((Row  6 'CENTER' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq '3,000 m' 2001 5000 'M' 'All_colors' 3.40 $null $terms_ct $vf_ct $null $note_ct $src_ct ''))
# ---- ANITA 256524, three bands, five color tiers each ----
$anitaQr = 'ANITA sheet'
[void]$rows.Add((Row  7 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq ''  200  700 'M' 'PFE/PFD' 5.20 156 $terms_an $vf_an $vt_an $anitaQr $src_an ''))
[void]$rows.Add((Row  8 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq ''  200  700 'M' 'White'   5.97 179 $terms_an $vf_an $vt_an $anitaQr $src_an ''))
[void]$rows.Add((Row  9 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq ''  200  700 'M' 'Light'   6.10 183 $terms_an $vf_an $vt_an $anitaQr $src_an ''))
[void]$rows.Add((Row 10 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq ''  200  700 'M' 'Medium'  6.10 183 $terms_an $vf_an $vt_an $anitaQr $src_an ''))
[void]$rows.Add((Row 11 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq ''  200  700 'M' 'Dark'    6.10 183 $terms_an $vf_an $vt_an $anitaQr $src_an ''))
[void]$rows.Add((Row 12 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq ''  701 2500 'M' 'PFE/PFD' 4.93 148 $terms_an $vf_an $vt_an $anitaQr $src_an 'Made in Thailand'))
[void]$rows.Add((Row 13 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq ''  701 2500 'M' 'White'   5.50 165 $terms_an $vf_an $vt_an $anitaQr $src_an 'Made in Thailand'))
[void]$rows.Add((Row 14 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq ''  701 2500 'M' 'Light'   5.63 169 $terms_an $vf_an $vt_an $anitaQr $src_an 'Made in Thailand'))
[void]$rows.Add((Row 15 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq ''  701 2500 'M' 'Medium'  5.63 169 $terms_an $vf_an $vt_an $anitaQr $src_an 'Made in Thailand'))
[void]$rows.Add((Row 16 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq ''  701 2500 'M' 'Dark'    5.83 175 $terms_an $vf_an $vt_an $anitaQr $src_an 'Made in Thailand'))
[void]$rows.Add((Row 17 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq '' 2501 10000 'M' 'PFE/PFD' 4.77 143 $terms_an $vf_an $vt_an $anitaQr $src_an ''))
[void]$rows.Add((Row 18 'ANITA' '256524' '' $fab_pq $com_pq $fw_pq $uw_pq $wt_pq '' 2501 10000 'M' 'Light'   5.33 160 $terms_an $vf_an $vt_an $anitaQr $src_an ''))
# ---- ETH (Ethiopia) 06.02.2023, PFD only ----
[void]$rows.Add((Row 19 'ETH'      '256524' '' 'CO/PA PIQUE' $com_pq '' '155-160' '' '' 1000 $null 'M' 'PFD' $null 98 'THB Ex works' '2023-02-06' $null 'Price offer 06.02.2023' 'ETH_Sheet' ''))
[void]$rows.Add((Row 20 'ETH'      '256524' '' 'CO/PA PIQUE' $com_pq '' '155-160' '' '' 5000 $null 'M' 'PFD' $null 94 'THB Ex works' '2023-02-06' $null 'Price offer 06.02.2023' 'ETH_Sheet' ''))
# ---- BISCHOFF stock 6-Jun-2024 ----
[void]$rows.Add((Row 21 'BISCHOFF' '256524' '' $fab_pq $com_pq '' '' '' '' 1 $null 'M' 'PFE_Stock' $null 99 '' '2024-06-06' $null 'PFE STOCK for Bischoff' 'Dated_Quotes' ''))
# ---- Other articles as examples ----
[void]$rows.Add((Row 22 'ANITA'  '251205' '' '' '' '' '' '' ''  200  700 'M' 'Light'      6.27 188 $terms_an $vf_an $vt_an $anitaQr $src_an ''))
[void]$rows.Add((Row 23 'CENTER' '251166' '' 'ELASTIC CHARMEUSE' '80% PA, 20% ELASTANE' '150-155' '145-150' '145-160' '3,000 m' 200 600 'M' 'All_colors' 5.33 $null $terms_ct $vf_ct $null $note_ct $src_ct ''))
[void]$rows.Add((Row 24 'CENTER' '251195' '' 'PA CHARMEUSE SILK TOUCH' '100% PA' '150-155' '145-150' '36-41' '3,000 m' 200 600 'M' 'All_colors' 2.90 $null $terms_ct $vf_ct $null $note_ct $src_ct ''))

Write-Host "Seeded rows: $($rows.Count)"

# --- build workbook via Excel COM ---
$excel = New-Object -ComObject Excel.Application
$excel.DisplayAlerts=$false; $excel.ScreenUpdating=$false
try {
    $wb = $excel.Workbooks.Add()
    while ($wb.Worksheets.Count -gt 1) { $wb.Worksheets.Item($wb.Worksheets.Count).Delete() }

    # ---- price_book sheet ----
    $pb = $wb.Worksheets.Item(1); $pb.Name='price_book'
    $nCols = $headers.Count
    $nRows = $rows.Count

    # headers
    $hdrArr = New-Object 'object[,]' 1, $nCols
    for ($c=0; $c -lt $nCols; $c++){ $hdrArr[0,$c] = $headers[$c] }
    $pb.Range($pb.Cells(1,1), $pb.Cells(1,$nCols)).Value2 = $hdrArr

    # data โ€” convert 'yyyy-MM-dd' strings in valid_from(col 18) and valid_to(col 19) to DateTime
    $dataArr = New-Object 'object[,]' $nRows, $nCols
    for ($r=0; $r -lt $nRows; $r++) {
        for ($c=0; $c -lt $nCols; $c++) {
            $v = $rows[$r][$c]
            if ($null -eq $v -or ($v -is [string] -and $v -eq '')) {
                $dataArr[$r,$c] = $null
            } elseif (($c -eq 17 -or $c -eq 18) -and ($v -is [string]) -and ($v -match '^\d{4}-\d{2}-\d{2}$')) {
                $dataArr[$r,$c] = ([datetime]::ParseExact($v,'yyyy-MM-dd',$null)).ToOADate()
            } else {
                $dataArr[$r,$c] = $v
            }
        }
    }
    # Set column formats BEFORE writing so numeric-looking codes (e.g. "256524") stay as text
    $textCols = @(2,3,4,5,6,7,8,9,10,13,14,17,20,21,22)
    foreach ($tc in $textCols) { $pb.Columns.Item($tc).NumberFormat = '@' }
    $pb.Columns.Item(18).NumberFormat = 'yyyy-mm-dd'
    $pb.Columns.Item(19).NumberFormat = 'yyyy-mm-dd'
    $pb.Columns.Item(15).NumberFormat = '0.00'
    $pb.Columns.Item(16).NumberFormat = '0.00'

    # write cell-by-cell (fast enough at ~500 cells)
    for ($r=0; $r -lt $nRows; $r++) {
        for ($c=0; $c -lt $nCols; $c++) {
            $v = $dataArr[$r,$c]
            if ($null -ne $v) { $pb.Cells($r+2, $c+1).Value2 = $v }
        }
    }

    # header style
    $hdrRange = $pb.Range($pb.Cells(1,1), $pb.Cells(1,$nCols))
    $hdrRange.Font.Bold = $true
    $hdrRange.Interior.Color = 15132390

    # freeze top row
    $pb.Activate(); $excel.ActiveWindow.SplitRow = 1; $excel.ActiveWindow.FreezePanes = $true

    # convert data range to an Excel Table (ListObject) for structured refs
    $tbl = $pb.ListObjects.Add(1, $pb.Range($pb.Cells(1,1), $pb.Cells(1+$nRows,$nCols)), $null, 1)
    $tbl.Name = 'price_book_tbl'
    $tbl.TableStyle = 'TableStyleMedium2'

    $pb.Columns.AutoFit() | Out-Null

    # ---- lookup sheet ----
    $lk = $wb.Worksheets.Add([System.Reflection.Missing]::Value, $pb)
    $lk.Name = 'lookup'

    $lk.Cells(1,1).Value2 = 'LOOKUP INPUTS'
    $lk.Cells(1,1).Font.Bold = $true

    $inputs = @(
        @('Article',    '256524'),
        @('Customer',   'ANITA'),
        @('Color tier', 'Light'),
        @('Qty',        1000),
        @('Qty unit',   'M'),
        @('As-of date', '=TODAY()')
    )
    # set input-cell formats BEFORE writing so text inputs like article "256524" stay text
    $lk.Range('B2:B4').NumberFormat = '@'   # article, customer, color tier (text)
    $lk.Cells(6,2).NumberFormat = '@'       # qty unit (text)
    $lk.Cells(7,2).NumberFormat = 'yyyy-mm-dd'  # as-of date

    for ($i=0; $i -lt $inputs.Count; $i++) {
        $lk.Cells($i+2,1).Value2 = $inputs[$i][0]
        $v = $inputs[$i][1]
        if ($v -is [string] -and $v.StartsWith('=')) { $lk.Cells($i+2,2).Formula = $v }
        else { $lk.Cells($i+2,2).Value2 = $v }
    }
    $lk.Range('A2:A7').Font.Bold = $true
    $lk.Range('B2:B7').Interior.Color = 16776960   # yellow input cells

    $lk.Cells(9,1).Value2 = 'RESULT (best match โ€” exact customer preferred; CENTER fallback shown below)'
    $lk.Cells(9,1).Font.Bold = $true

    $lk.Cells(10,1).Value2 = 'Matching rule: article = input; customer = input (else CENTER); color_tier = input; qty_unit = input; qty_min <= Qty AND (qty_max blank OR qty_max >= Qty). Validity is DISPLAYED so you can judge if a quote is stale; As-of date is for reference only.'
    $lk.Cells(10,1).WrapText = $true
    $lk.Range('A10:I10').Merge() | Out-Null

    $labels = @('Match source','Row #','USD/unit','THB/unit','Qty band','Terms','Valid','Source sheet','Notes')
    for ($i=0; $i -lt $labels.Count; $i++){ $lk.Cells(12, $i+1).Value2 = $labels[$i] }
    $lk.Range($lk.Cells(12,1), $lk.Cells(12,$labels.Count)).Font.Bold = $true
    $lk.Range($lk.Cells(12,1), $lk.Cells(12,$labels.Count)).Interior.Color = 15132390

    # ---- formula templates (single-quoted so " is literal) ----
    # coerce text inputs with ""& so a numeric-typed input still matches the text column;
    # qty_max blank-or->=Qty expressed with arithmetic OR because IF() doesn't array-iterate outside CSE
    $condExact = '(INDEX(price_book_tbl[article],0)=""&$B$2)*(INDEX(price_book_tbl[customer],0)=""&$B$3)*(INDEX(price_book_tbl[color_tier],0)=""&$B$4)*(INDEX(price_book_tbl[qty_unit],0)=""&$B$6)*(INDEX(price_book_tbl[qty_min],0)<=$B$5)*((INDEX(price_book_tbl[qty_max],0)="")+((INDEX(price_book_tbl[qty_max],0)<>"")*(INDEX(price_book_tbl[qty_max],0)>=$B$5)))'
    $condCenter = $condExact.Replace('$B$3','"CENTER"')

    # AGGREGATE(15=SMALL, 6=ignore errors) natively iterates the array without CSE
    $tMatch   = '=IFERROR(AGGREGATE(15,6,ROW(INDEX(price_book_tbl[article],0))/({0}),1)-ROW(price_book_tbl[[#Headers],[article]]),"")'
    $tIndex   = '=IFERROR(INDEX(price_book_tbl[{0}],{1}),"")'
    $tQtyBand = '=IFERROR(INDEX(price_book_tbl[qty_min],{0})&" - "&IF(INDEX(price_book_tbl[qty_max],{0})="","+",INDEX(price_book_tbl[qty_max],{0}))&" "&INDEX(price_book_tbl[qty_unit],{0}),"")'
    $tValid   = '=IFERROR(TEXT(INDEX(price_book_tbl[valid_from],{0}),"yyyy-mm-dd")&" .. "&IF(INDEX(price_book_tbl[valid_to],{0})="","open",TEXT(INDEX(price_book_tbl[valid_to],{0}),"yyyy-mm-dd")),"")'

    # ---- row 13: exact customer match ----
    $lk.Cells(13,1).Value2 = 'Exact customer'
    $lk.Cells(13,2).Formula = ($tMatch -f $condExact)
    $lk.Cells(13,3).Formula = ($tIndex -f 'price_usd','$B$13')
    $lk.Cells(13,4).Formula = ($tIndex -f 'price_thb','$B$13')
    $lk.Cells(13,5).Formula = ($tQtyBand -f '$B$13')
    $lk.Cells(13,6).Formula = ($tIndex -f 'terms','$B$13')
    $lk.Cells(13,7).Formula = ($tValid -f '$B$13')
    $lk.Cells(13,8).Formula = ($tIndex -f 'source_sheet','$B$13')
    $lk.Cells(13,9).Formula = ($tIndex -f 'notes','$B$13')

    # ---- row 14: CENTER fallback ----
    $lk.Cells(14,1).Value2 = 'CENTER fallback'
    $lk.Cells(14,2).Formula = ($tMatch -f $condCenter)
    $lk.Cells(14,3).Formula = ($tIndex -f 'price_usd','$B$14')
    $lk.Cells(14,4).Formula = ($tIndex -f 'price_thb','$B$14')
    $lk.Cells(14,5).Formula = ($tQtyBand -f '$B$14')
    $lk.Cells(14,6).Formula = ($tIndex -f 'terms','$B$14')
    $lk.Cells(14,7).Formula = ($tValid -f '$B$14')
    $lk.Cells(14,8).Formula = ($tIndex -f 'source_sheet','$B$14')
    $lk.Cells(14,9).Formula = ($tIndex -f 'notes','$B$14')

    $lk.Range('C13:C14').NumberFormat = '0.00'
    $lk.Range('D13:D14').NumberFormat = '0.00'

    $lk.Columns.AutoFit() | Out-Null
    $lk.Activate()
    $lk.Cells(2,2).Select()

    $wb.SaveAs($out, 51)
    $wb.Close($false)
    "Saved: $out"
} finally {
    $excel.Quit()
    [void][Runtime.InteropServices.Marshal]::ReleaseComObject($excel)
    [GC]::Collect(); [GC]::WaitForPendingFinalizers()
}

