Public Class SaveAllForm
    Private _Result As MsgBoxResult = MsgBoxResult.Cancel

    Public Sub New(Caption As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LblCaption.Text = Caption
    End Sub

    Public Property Result As MsgBoxResult
        Get
            Return _Result
        End Get
        Set
            _Result = Value
        End Set
    End Property

    Private Sub SaveAllForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        e.Graphics.DrawImage(SystemIcons.Question.ToBitmap, 22, 24)
    End Sub

    Private Sub Button_Click(sender As Object, e As EventArgs) Handles BtnYes.Click, BtnNo.Click, BtnCancel.Click, BtnYesToAll.Click, BtnNoToall.Click
        If sender Is BtnYes Then
            _Result = MsgBoxResult.Yes
        ElseIf sender Is BtnNo Then
            _Result = MsgBoxResult.No
        ElseIf sender Is BtnCancel Then
            _Result = MsgBoxResult.Cancel
        ElseIf sender Is BtnYesToAll Then
            _Result = MsgBoxResult.Retry
        ElseIf sender Is BtnNoToall Then
            _Result = MsgBoxResult.Ignore
        End If

        Me.Close()
    End Sub
End Class