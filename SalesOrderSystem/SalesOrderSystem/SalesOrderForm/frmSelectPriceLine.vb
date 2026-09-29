' Popup "LOV"-style dialog: shows the candidate price line(s) returned by
' SO.P_SO_PRICE_LIST_PKG_select_price_line for one order line's design/color/uom/qty,
' and lets the user pick one. Always shown, even when there is only one candidate.
' -- John 29/09/2026
Public Class frmSelectPriceLine
    Private _selectedDetailId As Nullable(Of Int64)

    Public Function ShowAndSelect(dt As DataTable) As Nullable(Of Int64)
        _selectedDetailId = Nothing
        dgvResults.AutoGenerateColumns = True
        dgvResults.DataSource = dt
        ConfigureColumns()
        If dgvResults.Rows.Count > 0 Then dgvResults.Rows(0).Selected = True
        Me.ShowDialog()
        Return _selectedDetailId
    End Function

    Private Sub ConfigureColumns()
        Dim hiddenCols() As String = {"so_price_list_header_id", "so_price_list_detail_id", "set_no", "line_no"}
        For Each colName In hiddenCols
            If dgvResults.Columns.Contains(colName) Then dgvResults.Columns(colName).Visible = False
        Next

        Dim headers As New Dictionary(Of String, String) From {
            {"matched_design_no", "Design No"},
            {"color_tier", "Color Tier"},
            {"qty_min", "Qty Min"},
            {"qty_max", "Qty Max"},
            {"uom", "UOM"},
            {"curr", "Currency"},
            {"price", "Price"},
            {"notes", "Notes"},
            {"resolution", "Resolution"}
        }
        For Each kv In headers
            If dgvResults.Columns.Contains(kv.Key) Then dgvResults.Columns(kv.Key).HeaderText = kv.Value
        Next
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

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        _selectedDetailId = Nothing
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class
