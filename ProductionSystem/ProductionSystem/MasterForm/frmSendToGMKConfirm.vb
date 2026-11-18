Imports System.Drawing
Imports System.Windows.Forms

'=====================================================================
' Confirmation window of the "Send to GMK" button (created 16/10/2026)
' Shows the values that would change in GMK; the user presses Send or Cancel.
' The flow of the button is in frmDesignNew (SendToGmkFlow), the data work is in classDesignNew.
' The layout is maintained by the WinForms Designer; runtime data is loaded below.
'=====================================================================
Public Partial Class frmSendToGMKConfirm

    Public Sub New()
        InitializeComponent()
        LoadDifferencePreview("ตัวอย่าง Design", Nothing)
    End Sub

    Public Sub New(ByVal designNo As String, ByVal diff As classDesignNew.DiffResult)
        Me.New()
        LoadDifferencePreview(designNo, diff)
    End Sub

    Private Sub LoadDifferencePreview(ByVal designNo As String, ByVal diff As classDesignNew.DiffResult)
        lvDifferences.Items.Clear()

        If diff Is Nothing Then
            lblInfo.Text = "Design No.: " & designNo & vbCrLf & "ตัวอย่างหน้าจอ Send to GMK"
            txtNote.Text = "หมายเหตุ: ไม่ส่ง Machine group และสถานะ/เวอร์ชัน Spec ไป GMK"
            Return
        End If

        For Each item As classDesignNew.DiffItem In diff.Items
            Dim li As New ListViewItem(item.Field)
            li.SubItems.Add(item.GmkValue)
            li.SubItems.Add(item.EschlerValue)
            lvDifferences.Items.Add(li)
        Next

        lblInfo.Text = "Design No.: " & designNo & vbCrLf & _
                       "ค่าต่อไปนี้ใน GMK จะถูกแทนที่ด้วยค่าจาก Eschler (" & diff.Items.Count.ToString() & " รายการ)"

        Dim note As String = "หมายเหตุ: ไม่ส่ง Machine group และสถานะ/เวอร์ชัน Spec ไป GMK"
        If diff.Warnings.Count > 0 Then
            note &= vbCrLf & "ค่าที่ไม่ได้ส่ง (ตามกติกา):" & vbCrLf & String.Join(vbCrLf, diff.Warnings.ToArray())
        End If
        txtNote.Text = note
    End Sub

End Class
