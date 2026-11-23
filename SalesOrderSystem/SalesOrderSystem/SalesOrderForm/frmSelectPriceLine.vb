' Popup "LOV"-style dialog: shows the candidate price line(s) returned by
' SO.P_SO_PRICE_LIST_PKG_select_price_line for one order line's design/color/uom/qty,
' and lets the user pick one. Always shown, even when there is only one candidate.
' -- John 29/09/2026
Public Class frmSelectPriceLine
    Private _selectedDetailId As Nullable(Of Int64)

    ''' <summary>Unit price and currency of the row the user picked, so the caller
    ''' can push them onto the SO line alongside the detail id. -- John 01/10/2026</summary>
    Private _selectedPrice As Nullable(Of Decimal)
    Private _selectedCurrency As String
    Public ReadOnly Property SelectedPrice As Nullable(Of Decimal)
        Get
            Return _selectedPrice
        End Get
    End Property
    Public ReadOnly Property SelectedCurrency As String
        Get
            Return _selectedCurrency
        End Get
    End Property

    ''' <summary>How the price was worked out, kept on the SO line (soitm.price_list_uom, price_list_uom_conv,
    ''' price_list_exrt, price_list_system_note): the unit the list prices in, the unit factor, the exchange
    ''' rate used and the SP's conversion note. -- John 23/11/2026</summary>
    Private _selectedPriceUom As String
    Private _selectedUomConv As Nullable(Of Decimal)
    Private _selectedExRate As Nullable(Of Decimal)
    Private _selectedNote As String
    Public ReadOnly Property SelectedPriceUom As String
        Get
            Return _selectedPriceUom
        End Get
    End Property
    Public ReadOnly Property SelectedUomConv As Nullable(Of Decimal)
        Get
            Return _selectedUomConv
        End Get
    End Property
    Public ReadOnly Property SelectedExRate As Nullable(Of Decimal)
        Get
            Return _selectedExRate
        End Get
    End Property
    Public ReadOnly Property SelectedNote As String
        Get
            Return _selectedNote
        End Get
    End Property

    'Value of a column in the picked row, or Nothing if the SP did not return it or it is NULL.
    Private Function PickedValue(colName As String) As Object
        If Not dgvResults.Columns.Contains(colName) Then Return Nothing
        Dim v As Object = dgvResults.CurrentRow.Cells(colName).Value
        If v Is Nothing OrElse IsDBNull(v) Then Return Nothing
        Return v
    End Function

    ''' <summary>
    ''' Pass the calling form as owner -- without it, ShowDialog() in this MDI app
    ''' can open the modal dialog behind the main window's Z-order: it's really
    ''' open and blocking, just invisible, which looks exactly like "no popup".
    ''' -- John 30/09/2026
    ''' </summary>
    Public Function ShowAndSelect(dt As DataTable, owner As IWin32Window) As Nullable(Of Int64)
        _selectedDetailId = Nothing
        dgvResults.AutoGenerateColumns = True
        dgvResults.DataSource = dt
        ConfigureColumns()
        If dgvResults.Rows.Count > 0 Then dgvResults.Rows(0).Selected = True
        Me.ShowDialog(owner)
        Return _selectedDetailId
    End Function

    Private Sub ConfigureColumns()
        'John 02/11/2026 Show every column the SP returns, in the SP's own order; only the ids are hidden.
        Dim hiddenCols() As String = {"so_price_list_header_id", "so_price_list_detail_id"}
        For Each colName In hiddenCols
            If dgvResults.Columns.Contains(colName) Then dgvResults.Columns(colName).Visible = False
        Next

        Dim headers As New Dictionary(Of String, String) From {
            {"resolution", "Resolution"},
            {"order_price", "Order Price"},
            {"order_curr", "Order Curr"},
            {"order_uom", "Order UOM"},
            {"price", "Price"},
            {"price_curr", "Price Curr"},
            {"price_uom", "Price UOM"},
            {"exchange_rate_used", "Exchange Rate Used"},
            {"uom_factor", "UOM Factor"},
            {"conversion_note", "Conversion Note"},
            {"matched_design_no", "Design No"},
            {"color_tier", "Color Tier"},
            {"qty_min", "Qty Min"},
            {"qty_max", "Qty Max"},
            {"set_no", "Set No"},
            {"line_no", "Line No"},
            {"notes", "Notes"}
        }
        For Each kv In headers
            If dgvResults.Columns.Contains(kv.Key) Then dgvResults.Columns(kv.Key).HeaderText = kv.Value
        Next

        'Many columns: size each to its content (the grid scrolls sideways) and open the dialog wider.
        dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        Dim wa As Rectangle = Screen.FromControl(Me).WorkingArea
        Me.Width = Math.Min(1400, wa.Width - 40)
    End Sub

    Private Sub btnSelect_Click(sender As Object, e As EventArgs) Handles btnSelect.Click
        CommitSelection()
    End Sub

    Private Sub dgvResults_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvResults.CellDoubleClick
        If e.RowIndex >= 0 Then CommitSelection()
    End Sub

    Private Sub CommitSelection()
        If dgvResults.CurrentRow Is Nothing Then
            MessageBox.Show("Please select a price line.", "Select Price Line", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim raw As Object = dgvResults.CurrentRow.Cells("so_price_list_detail_id").Value
        If raw Is Nothing OrElse IsDBNull(raw) Then
            _selectedDetailId = Nothing
        Else
            _selectedDetailId = Convert.ToInt64(raw)
        End If

        'John 02/11/2026 The SP now returns order_price / order_curr (the price already converted to the
        'order's currency and unit) instead of curr; fall back to price / price_curr if they are absent.
        Dim priceCol As String = If(dgvResults.Columns.Contains("order_price"), "order_price", "price")
        Dim priceRaw As Object = dgvResults.CurrentRow.Cells(priceCol).Value
        _selectedPrice = If(priceRaw Is Nothing OrElse IsDBNull(priceRaw), Nothing, CType(Convert.ToDecimal(priceRaw), Decimal?))

        Dim currCol As String = If(dgvResults.Columns.Contains("order_curr"), "order_curr", "price_curr")
        Dim currRaw As Object = If(dgvResults.Columns.Contains(currCol), dgvResults.CurrentRow.Cells(currCol).Value, Nothing)
        _selectedCurrency = If(currRaw Is Nothing OrElse IsDBNull(currRaw), Nothing, currRaw.ToString.Trim)

        'John 23/11/2026 What the SP says the price was worked out from (limits match the soitm columns:
        'char(10), numeric(12,4), numeric(12,4), nvarchar(100)).
        Dim v As Object
        v = PickedValue("price_uom")
        _selectedPriceUom = If(v Is Nothing, Nothing, v.ToString.Trim)
        v = PickedValue("uom_factor")
        _selectedUomConv = If(v Is Nothing, Nothing, CType(Math.Round(Convert.ToDecimal(v), 4), Decimal?))
        v = PickedValue("exchange_rate_used")
        _selectedExRate = If(v Is Nothing, Nothing, CType(Math.Round(Convert.ToDecimal(v), 4), Decimal?))
        v = PickedValue("conversion_note")
        If v Is Nothing Then
            _selectedNote = Nothing
        Else
            _selectedNote = v.ToString.Trim
            If _selectedNote.Length > 100 Then _selectedNote = _selectedNote.Substring(0, 100)
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        _selectedDetailId = Nothing
        _selectedPrice = Nothing
        _selectedCurrency = Nothing
        _selectedPriceUom = Nothing
        _selectedUomConv = Nothing
        _selectedExRate = Nothing
        _selectedNote = Nothing
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class
