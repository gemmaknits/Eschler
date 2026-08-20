Imports System.Data.SqlClient
Imports System.Text
Imports System.IO

Public Class frmStockSummary
    Dim clsConn As New classConnection
    Dim clsUser As New classUserInfo

    Public Property UserInfo() As classUserInfo
        Get
            UserInfo = clsUser
        End Get
        Set(ByVal NewValue As classUserInfo)
            clsUser = NewValue
        End Set
    End Property

    Private Sub frmStockSummary_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        dtpDateFr.Value = DateAdd(DateInterval.Month, -3, Now)
        dtpDateTo.Value = Now
    End Sub

    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        ' Force any DateTimePicker still being edited to commit its typed value
        Me.ActiveControl = Nothing
        Me.Cursor = Cursors.WaitCursor
        Try
            Dim html As String = GenerateHtml()
            Dim tempPath As String = Path.Combine(Path.GetTempPath(), "StockGreigeSummary.html")
            File.WriteAllText(tempPath, html, Encoding.UTF8)
            Process.Start(tempPath)
        Catch ex As Exception
            MessageBox.Show("Error generating report: " & ex.Message, "System Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.Enter Then
            Me.BeginInvoke(New Action(Sub() btnPrint_Click(btnPrint, EventArgs.Empty)))
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub btnMinimized_Click(sender As Object, e As EventArgs) Handles btnMinimized.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    ' ─────────────────────────────────────────────────────────────
    ' Data loading
    ' ─────────────────────────────────────────────────────────────

    Private Function LoadSummary(datefr As String, dateto As String, customer As String) As DataTable
        Dim conn As New SqlConnection((New classConnection).connection)
        Dim comm As New SqlCommand("SO.p_stk_glamorise_greige_summary", conn)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@datefr", datefr)
        comm.Parameters.AddWithValue("@dateto", dateto)
        comm.Parameters.AddWithValue("@customer", customer)
        Dim da As New SqlDataAdapter(comm)
        Dim dt As New DataTable
        da.Fill(dt)
        Return dt
    End Function

    Private Function LoadDetail(designNo As String, datefr As String, dateto As String, customer As String) As DataSet
        Dim conn As New SqlConnection((New classConnection).connection)
        Dim comm As New SqlCommand("SO.p_stk_glamorise_greige_detail", conn)
        comm.CommandType = CommandType.StoredProcedure
        comm.Parameters.AddWithValue("@design_no", designNo)
        comm.Parameters.AddWithValue("@datefr", datefr)
        comm.Parameters.AddWithValue("@dateto", dateto)
        comm.Parameters.AddWithValue("@customer", customer)
        Dim da As New SqlDataAdapter(comm)
        Dim ds As New DataSet
        da.Fill(ds) ' Tables(0) = ST orders, Tables(1) = OC orders
        Return ds
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' HTML generation
    ' ─────────────────────────────────────────────────────────────

    Private Function GenerateHtml() As String
        Dim articleFilter As String = txtArticleNo.Text.Trim().ToUpper()
        Dim customerFilter As String = txtCustomer.Text.Trim()

        Dim datefr As String
        Dim dateto As String
        Dim datefrDisplay As String
        Dim datetoDisplay As String

        datefr = dtpDateFr.Value.ToString("yyyyMMdd")
        dateto = dtpDateTo.Value.ToString("yyyyMMdd")
        datefrDisplay = dtpDateFr.Value.ToString("dd/MM/yyyy")
        datetoDisplay = dtpDateTo.Value.ToString("dd/MM/yyyy")

        Dim reportDate As String = Now.ToString("dd/MM/yyyy")
        Dim dtSummary As DataTable = LoadSummary(datefr, dateto, customerFilter)

        If articleFilter <> "" Then
            Dim toRemove As New List(Of DataRow)
            For Each r As DataRow In dtSummary.Rows
                If Not r("design_no").ToString().Trim().ToUpper().Contains(articleFilter) Then
                    toRemove.Add(r)
                End If
            Next
            For Each r As DataRow In toRemove
                dtSummary.Rows.Remove(r)
            Next
        End If

        Dim sb As New StringBuilder

        sb.AppendLine("<!DOCTYPE html>")
        sb.AppendLine("<html lang='en'><head>")
        sb.AppendLine("<meta charset='utf-8'>")
        sb.AppendLine("<title>Stock Greige Summary</title>")
        sb.AppendLine("<script src='https://cdn.jsdelivr.net/npm/exceljs@4.4.0/dist/exceljs.min.js'></script>")
        sb.AppendLine(BuildStyles())
        sb.AppendLine(BuildScript())
        sb.AppendLine("</head><body>")
        sb.AppendLine("<div class='page'>")

        ' Page header
        sb.AppendLine("<div class='page-hdr'>")
        sb.AppendLine("<h2>Stock Greige Summary</h2>")
        Dim filterInfo As String = $"Period: <strong>{datefrDisplay} &ndash; {datetoDisplay}</strong>"
        If articleFilter <> "" Then
            filterInfo &= $" &nbsp;|&nbsp; Article No.: <strong>{H(articleFilter)}</strong>"
        End If
        If customerFilter <> "" Then
            filterInfo &= $" &nbsp;|&nbsp; Customer: <strong>{H(customerFilter)}</strong>"
        End If
        sb.AppendLine($"<span class='subtitle'>{filterInfo} &nbsp;&nbsp; <span class='hint'>Click a row to view its detail below</span></span>")
        sb.AppendLine("<button class='btn-export' onclick='exportExcel()'>&#8681; Export to Excel</button>")
        sb.AppendLine("</div>")

        ' Top pane: summary table
        sb.AppendLine("<div class='pane-top'>")
        sb.AppendLine("<table class='tbl-main'>")
        sb.AppendLine("<thead><tr>")
        sb.AppendLine("<th>#</th><th>Date</th><th>Article No.</th><th>Article</th><th>Composition</th>")
        sb.AppendLine("<th>Balance Greige (YDS)</th><th>Greige Ready (YDS)</th><th>Greige Pending (YDS)</th>")
        sb.AppendLine("<th>Remark</th>")
        sb.AppendLine("</tr></thead><tbody>")

        ' ── Pass 1: Summary rows ──────────────────────────────────
        Dim seq As Integer = 0
        Dim summaryJsonRows As New List(Of String)

        For Each row As DataRow In dtSummary.Rows
            seq += 1
            Dim designNo As String = row("design_no").ToString()
            Dim divId As String = "det_" & seq.ToString()

            Dim balVal As Double = SafeDbl(row("balance_greige_yds"))
            Dim readyVal As Double = SafeDbl(row("greige_ready_yds"))
            Dim pendingVal As Double = SafeDbl(row("greige_pending_yds"))
            Dim lastStDate As String = row("last_st_date").ToString()

            sb.AppendLine($"<tr class='sum-row' data-id='{divId}' onclick=""showDetail('{divId}')"">")
            sb.AppendLine($"<td class='center'>{seq}</td>")
            sb.AppendLine($"<td class='center'>{H(lastStDate)}</td>")
            sb.AppendLine($"<td class='design-cell'>{H(designNo)}</td>")
            sb.AppendLine($"<td>{H(row("article").ToString())}</td>")
            sb.AppendLine($"<td>{H(row("composition").ToString())}</td>")
            sb.AppendLine($"<td class='num'>{Fmt(balVal)}</td>")
            sb.AppendLine($"<td class='num'>{Fmt(readyVal)}</td>")
            sb.AppendLine($"<td class='num'>{Fmt(pendingVal)}</td>")
            sb.AppendLine($"<td class='remark'>{H(row("sono_remark").ToString())}</td>")
            sb.AppendLine("</tr>")

            ' JSON for summary export
            summaryJsonRows.Add("[" &
                JQ(lastStDate) & "," &
                JQ(designNo.Trim()) & "," &
                JQ(row("article").ToString()) & "," &
                JQ(row("composition").ToString()) & "," &
                balVal.ToString("G") & "," &
                readyVal.ToString("G") & "," &
                pendingVal.ToString("G") & "," &
                JQ(row("sono_remark").ToString()) &
                "]")
        Next

        If seq = 0 Then
            sb.AppendLine("<tr><td colspan='9' class='center' style='padding:20px;color:#888;'>No data found for the selected period.</td></tr>")
        End If

        sb.AppendLine("</tbody></table>")
        sb.AppendLine("</div>") ' pane-top

        ' Bottom pane: detail (shown when row clicked)
        sb.AppendLine("<div class='pane-divider'></div>")
        sb.AppendLine("<div class='pane-bottom'>")
        sb.AppendLine("<div id='empty-msg' class='empty-msg'>&#9757; Select a design from the summary above to view its detail.</div>")

        ' ── Pass 2: Detail sections ───────────────────────────────
        Dim detailJsonParts As New List(Of String)
        Dim detSeq As Integer = 0

        For Each row As DataRow In dtSummary.Rows
            detSeq += 1
            Dim designNo As String = row("design_no").ToString()
            Dim divId As String = "det_" & detSeq.ToString()
            Dim ds As DataSet = LoadDetail(designNo, datefr, dateto, customerFilter)

            sb.AppendLine($"<div id='{divId}' class='detail-section'>")
            sb.AppendLine("<div class='detail-wrap'>")
            sb.AppendLine($"<p class='det-title'><strong>Product :</strong> {H(designNo)} &nbsp; {H(row("article").ToString())} &nbsp;&nbsp; <strong>Composition :</strong> {H(row("composition").ToString())}</p>")

            ' Build OC lookup keyed by st_no
            Dim ocByStNo As New Dictionary(Of String, List(Of DataRow))
            If ds.Tables.Count > 1 Then
                For Each ocr As DataRow In ds.Tables(1).Rows
                    Dim stn As String = ocr("st_no").ToString()
                    If Not ocByStNo.ContainsKey(stn) Then
                        ocByStNo(stn) = New List(Of DataRow)
                    End If
                    ocByStNo(stn).Add(ocr)
                Next
            End If

            sb.AppendLine("<div style='overflow-x:auto'>")
            sb.AppendLine("<table class='tbl-merged'>")
            sb.AppendLine("<thead>")
            sb.AppendLine("<tr>")
            sb.AppendLine("<th colspan='7' class='sec-hdr-left'>GREIGE BOOKING</th>")
            sb.AppendLine("<th colspan='10' class='sec-hdr-right'>ORDER</th>")
            sb.AppendLine("</tr>")
            sb.AppendLine("<tr>")
            sb.AppendLine("<th>S/T No.</th><th>Date</th><th>Product</th><th>Design</th><th>CUS Color</th><th class='num'>QTY</th><th>UOM</th>")
            sb.AppendLine("<th>O/C No.</th><th>OC Date</th><th>Customer PO</th><th>Product</th><th>CUS Color</th><th>Code Color</th><th class='num'>QTY</th><th>UOM</th><th class='num'>TTL Order</th><th class='num'>Balance Qty.</th>")
            sb.AppendLine("</tr>")
            sb.AppendLine("</thead><tbody>")

            Dim grandStQty As Double = 0
            Dim grandOcQty As Double = 0

            ' JSON for this design
            Dim jpSt As New List(Of String)

            If ds.Tables.Count > 0 Then
                For Each stRow As DataRow In ds.Tables(0).Rows
                    Dim stNo As String = stRow("st_no").ToString()
                    Dim stQty As Double = SafeDbl(stRow("st_qty"))
                    grandStQty += stQty

                    Dim ocList As List(Of DataRow)
                    If ocByStNo.ContainsKey(stNo) Then
                        ocList = ocByStNo(stNo)
                    Else
                        ocList = New List(Of DataRow)
                    End If

                    Dim ocTtl As Double = 0
                    For Each ocr As DataRow In ocList
                        ocTtl += SafeDbl(ocr("oc_qty"))
                    Next
                    grandOcQty += ocTtl
                    Dim stBal As Double = stQty - ocTtl
                    Dim rowspan As Integer = Math.Max(ocList.Count, 1)

                    If ocList.Count = 0 Then
                        sb.AppendLine("<tr>")
                        sb.AppendLine($"<td class='st-cell'>{H(stNo)}</td>")
                        sb.AppendLine($"<td class='st-cell'>{H(stRow("st_date").ToString())}</td>")
                        sb.AppendLine($"<td class='st-cell'>{H(stRow("product").ToString())}</td>")
                        sb.AppendLine($"<td class='st-cell'>{H(stRow("customer_design").ToString())}</td>")
                        sb.AppendLine($"<td class='st-cell'>{H(stRow("cus_color").ToString())}</td>")
                        sb.AppendLine($"<td class='st-cell num'>{Fmt(stQty)}</td>")
                        sb.AppendLine($"<td class='st-cell'>{H(stRow("st_uom").ToString())}</td>")
                        sb.AppendLine("<td colspan='8'></td>")
                        sb.AppendLine($"<td class='num ttl-cell'>{Fmt(ocTtl)}</td>")
                        sb.AppendLine($"<td class='num bal-cell'>{Fmt(stBal)}</td>")
                        sb.AppendLine("</tr>")
                    Else
                        For i As Integer = 0 To ocList.Count - 1
                            Dim ocr As DataRow = ocList(i)
                            Dim ocQty As Double = SafeDbl(ocr("oc_qty"))
                            sb.AppendLine("<tr>")
                            If i = 0 Then
                                sb.AppendLine($"<td rowspan='{rowspan}' class='st-cell'>{H(stNo)}</td>")
                                sb.AppendLine($"<td rowspan='{rowspan}' class='st-cell'>{H(stRow("st_date").ToString())}</td>")
                                sb.AppendLine($"<td rowspan='{rowspan}' class='st-cell'>{H(stRow("product").ToString())}</td>")
                                sb.AppendLine($"<td rowspan='{rowspan}' class='st-cell'>{H(stRow("customer_design").ToString())}</td>")
                                sb.AppendLine($"<td rowspan='{rowspan}' class='st-cell'>{H(stRow("cus_color").ToString())}</td>")
                                sb.AppendLine($"<td rowspan='{rowspan}' class='st-cell num'>{Fmt(stQty)}</td>")
                                sb.AppendLine($"<td rowspan='{rowspan}' class='st-cell'>{H(stRow("st_uom").ToString())}</td>")
                            End If
                            sb.AppendLine($"<td>{H(ocr("oc_no").ToString())}</td>")
                            sb.AppendLine($"<td>{H(ocr("oc_date").ToString())}</td>")
                            sb.AppendLine($"<td>{H(ocr("cust_po").ToString())}</td>")
                            sb.AppendLine($"<td>{H(ocr("article").ToString())}</td>")
                            sb.AppendLine($"<td>{H(ocr("cus_color").ToString())}</td>")
                            sb.AppendLine($"<td>{H(ocr("color_code").ToString())}</td>")
                            sb.AppendLine($"<td class='num'>{Fmt(ocQty)}</td>")
                            sb.AppendLine($"<td>{H(ocr("oc_uom").ToString())}</td>")
                            If i = ocList.Count - 1 Then
                                sb.AppendLine($"<td class='num ttl-cell'>{Fmt(ocTtl)}</td>")
                                sb.AppendLine($"<td class='num bal-cell'>{Fmt(stBal)}</td>")
                            Else
                                sb.AppendLine("<td></td><td></td>")
                            End If
                            sb.AppendLine("</tr>")
                        Next
                    End If

                    ' Build JSON for this ST row
                    Dim jpOc As New List(Of String)
                    For Each ocr As DataRow In ocList
                        jpOc.Add("{" &
                            """ocNo"":" & JQ(ocr("oc_no").ToString()) & "," &
                            """ocDate"":" & JQ(ocr("oc_date").ToString()) & "," &
                            """custPo"":" & JQ(ocr("cust_po").ToString()) & "," &
                            """article"":" & JQ(ocr("article").ToString()) & "," &
                            """cusCo"":" & JQ(ocr("cus_color").ToString()) & "," &
                            """colorCode"":" & JQ(ocr("color_code").ToString()) & "," &
                            """ocQty"":" & SafeDbl(ocr("oc_qty")).ToString("G") & "," &
                            """ocUom"":" & JQ(ocr("oc_uom").ToString()) &
                            "}")
                    Next

                    jpSt.Add("{" &
                        """stNo"":" & JQ(stNo) & "," &
                        """stDate"":" & JQ(stRow("st_date").ToString()) & "," &
                        """product"":" & JQ(stRow("product").ToString()) & "," &
                        """design"":" & JQ(stRow("customer_design").ToString()) & "," &
                        """cusColor"":" & JQ(stRow("cus_color").ToString()) & "," &
                        """stQty"":" & stQty.ToString("G") & "," &
                        """stUom"":" & JQ(stRow("st_uom").ToString()) & "," &
                        """ttlOrder"":" & ocTtl.ToString("G") & "," &
                        """balQty"":" & stBal.ToString("G") & "," &
                        """ocRows"":[" & String.Join(",", jpOc) & "]" &
                        "}")
                Next
            End If

            sb.AppendLine("<tr class='tot-row'>")
            sb.AppendLine("<td colspan='5'><strong>Total</strong></td>")
            sb.AppendLine($"<td class='num'><strong>{Fmt(grandStQty)}</strong></td>")
            sb.AppendLine("<td></td>")
            sb.AppendLine("<td colspan='7'></td>")
            sb.AppendLine($"<td class='num'><strong>{Fmt(grandOcQty)}</strong></td>")
            sb.AppendLine("<td></td>")
            sb.AppendLine($"<td class='num'><strong>{Fmt(grandStQty - grandOcQty)}</strong></td>")
            sb.AppendLine("</tr>")
            sb.AppendLine("</tbody></table></div>")
            sb.AppendLine("</div>") ' detail-wrap
            sb.AppendLine("</div>") ' detail-section

            ' Finalize JSON entry for this design
            detailJsonParts.Add(
                JQ(divId) & ":{" &
                """sheetName"":" & JQ(SafeSheetName(designNo.Trim())) & "," &
                """design"":" & JQ(designNo.Trim()) & "," &
                """article"":" & JQ(row("article").ToString()) & "," &
                """composition"":" & JQ(row("composition").ToString()) & "," &
                """grandStQty"":" & grandStQty.ToString("G") & "," &
                """grandOcQty"":" & grandOcQty.ToString("G") & "," &
                """stRows"":[" & String.Join(",", jpSt) & "]" &
                "}")
        Next

        sb.AppendLine("</div>") ' pane-bottom
        sb.AppendLine("</div>") ' page

        ' Embed data for Excel export
        sb.AppendLine("<script>")
        sb.AppendLine("window.__summaryHeaders=['Date','Design','Article','Composition','Balances Greige (mt.)','Greige ready knited','Greige pending','Remark'];")
        sb.AppendLine("window.__summaryRows=[" & String.Join(",", summaryJsonRows) & "];")
        sb.AppendLine("window.__detailData={" & String.Join(",", detailJsonParts) & "};")
        sb.AppendLine("</script>")

        sb.AppendLine("</body></html>")
        Return sb.ToString()
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' Helpers
    ' ─────────────────────────────────────────────────────────────

    Private Function H(s As String) As String
        Return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;")
    End Function

    Private Function JQ(s As String) As String
        ' JSON-quoted string
        Dim v As String = s.Replace("\", "\\").Replace("""", "\""").Replace(Chr(13), "").Replace(Chr(10), " ").Replace(Chr(9), " ")
        Return """" & v & """"
    End Function

    Private Function SafeDbl(v As Object) As Double
        If IsDBNull(v) OrElse v Is Nothing Then Return 0
        Return CDbl(v)
    End Function

    Private Function Fmt(v As Double) As String
        Return String.Format("{0:N0}", v)
    End Function

    Private Function SafeSheetName(s As String) As String
        Dim result As String = s
        For Each c As Char In "\/|?*:[]".ToCharArray()
            result = result.Replace(c.ToString(), "")
        Next
        If result.Length > 31 Then result = result.Substring(0, 31)
        Return result
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' CSS
    ' ─────────────────────────────────────────────────────────────

    Private Function BuildStyles() As String
        Return "
<style>
  *{box-sizing:border-box;margin:0;padding:0}
  html,body{height:100%;overflow:hidden}
  body{font-family:Arial,sans-serif;font-size:12px;background:#e6e6e6;color:#333}

  /* Layout */
  .page{display:flex;flex-direction:column;height:100vh;padding:8px 14px}
  .page-hdr{flex-shrink:0;padding-bottom:6px;display:flex;align-items:flex-start;gap:14px;flex-wrap:wrap}
  .page-hdr-text{flex:1}
  h2{color:#222;font-size:15px;font-weight:600;margin-bottom:2px;letter-spacing:.3px}
  .subtitle{color:#666;font-size:11px}
  .hint{color:#888;font-style:italic}
  .pane-top{flex:1;overflow-y:auto;background:#fff;border:1px solid #d0d0d0;border-bottom:none;min-height:0}
  .pane-divider{flex-shrink:0;height:3px;background:#999}
  .pane-bottom{flex:1;overflow-y:auto;background:#fff;border:1px solid #d0d0d0;border-top:none;padding:12px 16px;min-height:0}
  .empty-msg{color:#aaa;font-size:12px;padding:40px;text-align:center;letter-spacing:.2px}
  .detail-section{display:none}
  .detail-section.visible{display:block}
  .det-title{font-size:12px;color:#333;font-weight:600;margin-bottom:10px;padding-bottom:6px;border-bottom:1px solid #ddd}

  /* Export button */
  .btn-export{margin-left:auto;padding:4px 14px;background:#444;color:#fff;border:none;font-size:11px;font-family:Arial,sans-serif;font-weight:600;cursor:pointer;letter-spacing:.3px;white-space:nowrap;align-self:flex-start}
  .btn-export:hover{background:#222}

  /* Summary table */
  .tbl-main{width:100%;border-collapse:collapse;font-size:12px}
  .tbl-main thead{position:sticky;top:0;z-index:1}
  .tbl-main thead tr{background:#444;color:#fff}
  .tbl-main th{padding:7px 10px;text-align:left;white-space:nowrap;font-weight:600;letter-spacing:.2px}
  .tbl-main td{padding:5px 10px;border-bottom:1px solid #ebebeb;vertical-align:top}
  .sum-row{cursor:pointer}
  .sum-row:hover{background:#f5f5f5}
  .active-row{background:#e8e8e8 !important}
  .active-row .design-cell{text-decoration:underline}
  .design-cell{color:#222;font-weight:600}
  .num{text-align:right;white-space:nowrap}
  .center{text-align:center}
  .remark{color:#666;font-size:11px;max-width:280px;line-height:1.4}

  /* Detail merged table */
  .tbl-merged{width:100%;border-collapse:collapse;font-size:11px}
  .tbl-merged thead{position:sticky;top:0;z-index:1}
  .tbl-merged th{padding:5px 8px;text-align:left;white-space:nowrap;border:1px solid #ccc;font-weight:600}
  .tbl-merged td{padding:4px 8px;border:1px solid #e0e0e0;white-space:nowrap;vertical-align:middle}
  .tbl-merged tbody tr:hover td{background:#f7f7f7}
  .sec-hdr-left{background:#555;color:#fff;text-align:center;font-size:11px;font-weight:600;letter-spacing:.3px}
  .sec-hdr-right{background:#777;color:#fff;text-align:center;font-size:11px;font-weight:600;letter-spacing:.3px}
  .st-cell{background:#f7f7f7;font-weight:600;color:#222;vertical-align:middle}
  .ttl-cell{background:#efefef;font-weight:700;color:#222}
  .bal-cell{background:#e4e4e4;font-weight:700;color:#222}
  .tot-row{background:#e8e8e8 !important}
  .tot-row td{border-top:1px solid #aaa}
</style>"
    End Function

    ' ─────────────────────────────────────────────────────────────
    ' JavaScript
    ' ─────────────────────────────────────────────────────────────

    Private Function BuildScript() As String
        Return "
<script>
  function showDetail(id){
    var target=document.getElementById(id);
    if(!target) return;
    var isOpen=target.classList.contains('visible');
    document.querySelectorAll('.detail-section').forEach(function(s){s.classList.remove('visible');});
    document.querySelectorAll('.sum-row').forEach(function(r){r.classList.remove('active-row');});
    var emptyMsg=document.getElementById('empty-msg');
    if(isOpen){
      emptyMsg.style.display='';
    } else {
      emptyMsg.style.display='none';
      target.classList.add('visible');
      document.querySelectorAll('.sum-row').forEach(function(r){
        if(r.getAttribute('data-id')===id) r.classList.add('active-row');
      });
      document.querySelector('.pane-bottom').scrollTop=0;
    }
  }

  async function exportExcel(){
    if(typeof ExcelJS==='undefined'){alert('Excel library not loaded. Check your internet connection.');return;}

    var thin={style:'thin'};
    var border={top:thin,left:thin,bottom:thin,right:thin};
    var noWrap={wrapText:false};

    function calcWidths(headers,rows){
      return headers.map(function(h,i){
        var max=String(h||'').length;
        rows.forEach(function(r){
          var v=r[i]!==undefined&&r[i]!==null?String(r[i]):'';
          if(v.length>max)max=v.length;
        });
        return Math.min(Math.max(max+2,10),80);
      });
    }

    function addSheet(wb,name,headers,rows,title){
      var ws=wb.addWorksheet(name);
      var widths=calcWidths(headers,rows);
      ws.columns=widths.map(function(w){return{width:w};});

      if(title){
        var tRow=ws.addRow([title]);
        tRow.font={bold:true};
        tRow.getCell(1).border=border;
        ws.mergeCells(tRow.number,1,tRow.number,headers.length);
      }

      var hRow=ws.addRow(headers);
      for(var c=1;c<=headers.length;c++){
        var hc=hRow.getCell(c);
        hc.font={bold:true};
        hc.border=border;
        hc.alignment=noWrap;
      }

      rows.forEach(function(rowData){
        var row=ws.addRow(rowData);
        for(var c=1;c<=headers.length;c++){
          var cell=row.getCell(c);
          cell.border=border;
          cell.alignment=noWrap;
          if(typeof cell.value==='number') cell.numFmt='#,##0';
        }
      });
    }

    var wb=new ExcelJS.Workbook();

    // Summary sheet (always)
    addSheet(wb,'Summary',window.__summaryHeaders||[],window.__summaryRows||[]);

    // Detail sheet (only if a design is clicked)
    var visible=document.querySelector('.detail-section.visible');
    if(visible&&window.__detailData&&window.__detailData[visible.id]){
      var det=window.__detailData[visible.id];
      var dh=['S/T No.','S/T Date','Product','Design','CUS Color','S/T QTY','UOM',
              'O/C No.','OC Date','Customer PO','Product','CUS Color','Code Color','OC QTY','UOM','TTL Order','Balance Qty'];
      var dRows=[];
      (det.stRows||[]).forEach(function(st){
        if(!st.ocRows||st.ocRows.length===0){
          dRows.push([st.stNo,st.stDate,st.product,st.design,st.cusColor,st.stQty,st.stUom,
                      '','','','','','','','',st.ttlOrder,st.balQty]);
        } else {
          st.ocRows.forEach(function(oc,idx){
            var isLast=idx===st.ocRows.length-1;
            dRows.push([st.stNo,st.stDate,st.product,st.design,st.cusColor,st.stQty,st.stUom,
                        oc.ocNo,oc.ocDate,oc.custPo,oc.article,oc.cusCo,oc.colorCode,oc.ocQty,oc.ocUom,
                        isLast?st.ttlOrder:'',isLast?st.balQty:'']);
          });
        }
      });
      dRows.push(['Total','','','','',det.grandStQty,'','','','','','','',det.grandOcQty,'','',det.grandStQty-det.grandOcQty]);
      var title='Product : '+det.design+'   '+det.article+'          Composition : '+det.composition;
      addSheet(wb,det.sheetName||'Detail',dh,dRows,title);
    }

    var buf=await wb.xlsx.writeBuffer();
    var blob=new Blob([buf],{type:'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'});
    var url=URL.createObjectURL(blob);
    var a=document.createElement('a');
    a.href=url;
    a.download='StockGreigeSummary.xlsx';
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }
</script>"
    End Function

End Class
