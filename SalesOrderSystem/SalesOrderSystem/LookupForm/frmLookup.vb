Imports System.Data
Imports System.Windows.Forms

Public Class frmLookup
    Inherits System.Windows.Forms.Form

    Public Property SelectedRow As DataRow

    Private _dt As DataTable
    Private _displayColumns As List(Of String)
    Private _headers As List(Of String)
    Private _filterView As DataView

    Public Sub New(dt As DataTable, displayColumns As List(Of String), headers As List(Of String))
        InitializeComponent()
        _dt = dt
        _displayColumns = displayColumns
        _headers = headers
    End Sub

    Private Sub frmLookup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        ApplyFilter("")
        txtSearch.Focus()
    End Sub

    Private Sub SetupGrid()
        dgvData.AutoGenerateColumns = False
        dgvData.Columns.Clear()
        For i As Integer = 0 To _displayColumns.Count - 1
            Dim col As New DataGridViewTextBoxColumn()
            col.DataPropertyName = _displayColumns(i)
            col.HeaderText = _headers(i)
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
            col.ReadOnly = True
            dgvData.Columns.Add(col)
        Next
    End Sub

    Private Sub ApplyFilter(searchText As String)
        _filterView = New DataView(_dt)
        If searchText.Trim.Length > 0 Then
            Dim parts As New List(Of String)
            For Each colName As String In _displayColumns
                If _dt.Columns.Contains(colName) Then
                    Dim escaped As String = searchText.Replace("'", "''")
                    parts.Add(String.Format("CONVERT([{0}], System.String) LIKE '%{1}%'", colName, escaped))
                End If
            Next
            If parts.Count > 0 Then
                _filterView.RowFilter = String.Join(" OR ", parts)
            End If
        End If
        dgvData.DataSource = _filterView
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        ApplyFilter(txtSearch.Text)
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        ConfirmSelection()
    End Sub

    Private Sub dgvData_DoubleClick(sender As Object, e As EventArgs) Handles dgvData.DoubleClick
        ConfirmSelection()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub ConfirmSelection()
        If dgvData.CurrentRow IsNot Nothing Then
            Dim drv As DataRowView = TryCast(dgvData.CurrentRow.DataBoundItem, DataRowView)
            If drv IsNot Nothing Then
                SelectedRow = drv.Row
                Me.DialogResult = DialogResult.OK
                Me.Close()
            End If
        End If
    End Sub

    Private Sub txtSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSearch.KeyDown
        If e.KeyCode = Keys.Down AndAlso dgvData.Rows.Count > 0 Then
            dgvData.Focus()
            dgvData.CurrentCell = dgvData.Rows(0).Cells(0)
        ElseIf e.KeyCode = Keys.Enter Then
            ConfirmSelection()
        End If
    End Sub

    Private Sub dgvData_KeyDown(sender As Object, e As KeyEventArgs) Handles dgvData.KeyDown
        If e.KeyCode = Keys.Enter Then
            ConfirmSelection()
        ElseIf e.KeyCode = Keys.Escape Then
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End If
    End Sub
End Class
