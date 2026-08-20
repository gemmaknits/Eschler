<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmStockSummary
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.btnPrint = New System.Windows.Forms.ToolStripButton()
        Me.btnMinimized = New System.Windows.Forms.ToolStripButton()
        Me.btnExit = New System.Windows.Forms.ToolStripButton()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.dtpDateTo = New System.Windows.Forms.DateTimePicker()
        Me.dtpDateFr = New System.Windows.Forms.DateTimePicker()
        Me.lblDateTo = New System.Windows.Forms.Label()
        Me.lblDateFr = New System.Windows.Forms.Label()
        Me.lblArticleNo = New System.Windows.Forms.Label()
        Me.txtArticleNo = New System.Windows.Forms.TextBox()
        Me.lblCustomer = New System.Windows.Forms.Label()
        Me.txtCustomer = New System.Windows.Forms.TextBox()
        Me.ToolStrip1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()

        ' ToolStrip1
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.btnPrint, Me.btnMinimized, Me.btnExit})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(480, 25)
        Me.ToolStrip1.TabIndex = 0

        ' btnPrint
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(51, 22)
        Me.btnPrint.Text = "&Print"

        ' btnMinimized
        Me.btnMinimized.Name = "btnMinimized"
        Me.btnMinimized.Size = New System.Drawing.Size(69, 22)
        Me.btnMinimized.Text = "Minimized"

        ' btnExit
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(40, 22)
        Me.btnExit.Text = "E&xit"

        ' GroupBox1
        Me.GroupBox1.Controls.Add(Me.dtpDateTo)
        Me.GroupBox1.Controls.Add(Me.dtpDateFr)
        Me.GroupBox1.Controls.Add(Me.lblDateTo)
        Me.GroupBox1.Controls.Add(Me.lblDateFr)
        Me.GroupBox1.Controls.Add(Me.lblArticleNo)
        Me.GroupBox1.Controls.Add(Me.txtArticleNo)
        Me.GroupBox1.Controls.Add(Me.lblCustomer)
        Me.GroupBox1.Controls.Add(Me.txtCustomer)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 32)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(456, 82)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.Text = "Filter"

        ' lblDateFr
        Me.lblDateFr.AutoSize = True
        Me.lblDateFr.Location = New System.Drawing.Point(8, 22)
        Me.lblDateFr.Name = "lblDateFr"
        Me.lblDateFr.Text = "Date From :"

        ' dtpDateFr
        Me.dtpDateFr.Format = System.Windows.Forms.DateTimePickerFormat.Short
        Me.dtpDateFr.Location = New System.Drawing.Point(80, 18)
        Me.dtpDateFr.Name = "dtpDateFr"
        Me.dtpDateFr.Size = New System.Drawing.Size(110, 20)
        Me.dtpDateFr.TabIndex = 0

        ' lblDateTo
        Me.lblDateTo.AutoSize = True
        Me.lblDateTo.Location = New System.Drawing.Point(206, 22)
        Me.lblDateTo.Name = "lblDateTo"
        Me.lblDateTo.Text = "Date To :"

        ' dtpDateTo
        Me.dtpDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short
        Me.dtpDateTo.Location = New System.Drawing.Point(270, 18)
        Me.dtpDateTo.Name = "dtpDateTo"
        Me.dtpDateTo.Size = New System.Drawing.Size(110, 20)
        Me.dtpDateTo.TabIndex = 1

        ' lblArticleNo
        Me.lblArticleNo.AutoSize = True
        Me.lblArticleNo.Location = New System.Drawing.Point(8, 50)
        Me.lblArticleNo.Name = "lblArticleNo"
        Me.lblArticleNo.Text = "Article No. :"

        ' txtArticleNo
        Me.txtArticleNo.Location = New System.Drawing.Point(80, 47)
        Me.txtArticleNo.Name = "txtArticleNo"
        Me.txtArticleNo.Size = New System.Drawing.Size(130, 20)
        Me.txtArticleNo.TabIndex = 2

        ' lblCustomer
        Me.lblCustomer.AutoSize = True
        Me.lblCustomer.Location = New System.Drawing.Point(222, 50)
        Me.lblCustomer.Name = "lblCustomer"
        Me.lblCustomer.Text = "Customer :"

        ' txtCustomer
        Me.txtCustomer.Location = New System.Drawing.Point(288, 47)
        Me.txtCustomer.Name = "txtCustomer"
        Me.txtCustomer.Size = New System.Drawing.Size(158, 20)
        Me.txtCustomer.TabIndex = 3

        ' frmStockSummary
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(480, 130)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmStockSummary"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Stock Greige Summary"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents btnPrint As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnMinimized As System.Windows.Forms.ToolStripButton
    Friend WithEvents btnExit As System.Windows.Forms.ToolStripButton
    Friend GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents dtpDateFr As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpDateTo As System.Windows.Forms.DateTimePicker
    Friend lblDateFr As System.Windows.Forms.Label
    Friend lblDateTo As System.Windows.Forms.Label
    Friend lblArticleNo As System.Windows.Forms.Label
    Friend WithEvents txtArticleNo As System.Windows.Forms.TextBox
    Friend lblCustomer As System.Windows.Forms.Label
    Friend WithEvents txtCustomer As System.Windows.Forms.TextBox
End Class
