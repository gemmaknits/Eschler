<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSendToGMKConfirm
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Private components As System.ComponentModel.IContainer
    Friend WithEvents lvDifferences As System.Windows.Forms.ListView
    Friend WithEvents txtNote As System.Windows.Forms.TextBox
    Friend WithEvents pnlButtons As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnSend As System.Windows.Forms.Button
    Friend WithEvents lblInfo As System.Windows.Forms.Label

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.lvDifferences = New System.Windows.Forms.ListView()
        Me.txtNote = New System.Windows.Forms.TextBox()
        Me.pnlButtons = New System.Windows.Forms.FlowLayoutPanel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSend = New System.Windows.Forms.Button()
        Me.lblInfo = New System.Windows.Forms.Label()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'lvDifferences
        '
        Me.lvDifferences.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvDifferences.FullRowSelect = True
        Me.lvDifferences.GridLines = True
        Me.lvDifferences.HideSelection = False
        Me.lvDifferences.Location = New System.Drawing.Point(0, 56)
        Me.lvDifferences.Name = "lvDifferences"
        Me.lvDifferences.Size = New System.Drawing.Size(844, 365)
        Me.lvDifferences.TabIndex = 0
        Me.lvDifferences.UseCompatibleStateImageBehavior = False
        Me.lvDifferences.View = System.Windows.Forms.View.Details
        Me.lvDifferences.Columns.Add("Field", 230)
        Me.lvDifferences.Columns.Add("GMK (current)", 290)
        Me.lvDifferences.Columns.Add("Eschler (will be sent)", 290)
        '
        'txtNote
        '
        Me.txtNote.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.txtNote.Location = New System.Drawing.Point(0, 421)
        Me.txtNote.Multiline = True
        Me.txtNote.Name = "txtNote"
        Me.txtNote.ReadOnly = True
        Me.txtNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtNote.Size = New System.Drawing.Size(844, 50)
        Me.txtNote.TabIndex = 1
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnSend)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        Me.pnlButtons.Location = New System.Drawing.Point(0, 471)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Padding = New System.Windows.Forms.Padding(10, 8, 10, 8)
        Me.pnlButtons.Size = New System.Drawing.Size(844, 50)
        Me.pnlButtons.TabIndex = 2
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(704, 8)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(120, 30)
        Me.btnCancel.TabIndex = 0
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnSend
        '
        Me.btnSend.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnSend.Location = New System.Drawing.Point(578, 8)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(120, 30)
        Me.btnSend.TabIndex = 1
        Me.btnSend.Text = "Send to GMK"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'lblInfo
        '
        Me.lblInfo.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblInfo.Location = New System.Drawing.Point(0, 0)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Padding = New System.Windows.Forms.Padding(8, 8, 8, 0)
        Me.lblInfo.Size = New System.Drawing.Size(844, 56)
        Me.lblInfo.TabIndex = 3
        Me.lblInfo.Text = "Design No.: ตัวอย่าง Design" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "ตัวอย่างหน้าจอ Send to GMK"
        '
        'frmSendToGMKConfirm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(844, 521)
        Me.Controls.Add(Me.lvDifferences)
        Me.Controls.Add(Me.txtNote)
        Me.Controls.Add(Me.pnlButtons)
        Me.Controls.Add(Me.lblInfo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.MinimizeBox = False
        Me.Name = "frmSendToGMKConfirm"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Send to GMK"
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub
End Class
