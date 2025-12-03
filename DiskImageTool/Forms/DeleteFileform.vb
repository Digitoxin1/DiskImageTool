Public Class DeleteFileForm
    Public Structure DeleteFileFormResult
        Dim FillChar As Byte
        Dim Cancelled As Boolean
        Dim Clear As Boolean
    End Structure

    Private _Result As DeleteFileFormResult

    Public Sub New(Caption As String, Title As String, CanFill As Boolean)

        ' This call is required by the designer.
        InitializeComponent()
        LocalizeForm()

        ' Add any initialization after the InitializeComponent() call.
        LblCaption.Text = Caption
        Me.Text = Title
        _Result.Cancelled = True
        _Result.FillChar = &H0
        _Result.Clear = False
        RadioFillF6.Enabled = CanFill
        RadioFill00.Enabled = CanFill
    End Sub

    Private Sub LocalizeForm()
        BtnCancel.Text = My.Resources.Menu_No
        BtnOK.Text = My.Resources.Menu_Yes
        GroupBox1.Text = My.Resources.Label_FillSectorsWith
        RadioFillKeep.Text = My.Resources.Label_KeepData
    End Sub

    Public ReadOnly Property Result As DeleteFileFormResult
        Get
            Return _Result
        End Get
    End Property

    Public Shared Function Display(Caption As String, Title As String, CanFill As Boolean) As DeleteFileFormResult
        Using Form As New DeleteFileForm(Caption, Title, CanFill)
            Form.ShowDialog()
            Return Form.Result
        End Using
    End Function

    Private Sub SetResult(Cancelled As Boolean)
        If RadioFillKeep.Checked Then
            _Result.Clear = False
        ElseIf RadioFillF6.Checked Then
            _Result.FillChar = &HF6
            _Result.Clear = True
        Else
            _Result.FillChar = &H0
            _Result.Clear = True
        End If
        _Result.Cancelled = Cancelled
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        SetResult(False)
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        SetResult(True)
    End Sub

    Private Sub ClearSectorsForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        e.Graphics.DrawImage(SystemIcons.Question.ToBitmap, 22, 24)
    End Sub
End Class