Public Enum AbortReadResult
    [Continue] = 0
    Discard = 1
    Keep = 2
End Enum

Public Class AbortReadForm
    Private _Result As AbortReadResult = AbortReadResult.Continue

    Public Sub New(Caption As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        LblCaption.Text = Caption
        ActiveControl = BtnContinue
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.Escape Then
            _Result = AbortReadResult.Continue
            Close()
            Return True
        End If

        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Public Property Result As AbortReadResult
        Get
            Return _Result
        End Get
        Set
            _Result = Value
        End Set
    End Property

    Public Shared Function Display(owner As IWin32Window, Caption As String) As AbortReadResult
        Using dlg As New AbortReadForm(Caption)
            dlg.ShowDialog(owner)

            Return dlg.Result
        End Using
    End Function

    Private Sub LocalizeForm()
        BtnKeep.Text = My.Resources.Label_Keep
        BtnDiscard.Text = "&" & My.Resources.Label_Discard
        BtnContinue.Text = My.Resources.Label_Continue
        Me.Text = My.Resources.Dialog_AbortReadTitle
    End Sub

#Region "Events"

    Private Sub Button_Click(sender As Object, e As EventArgs) Handles BtnKeep.Click, BtnDiscard.Click, BtnContinue.Click
        If sender Is BtnKeep Then
            _Result = AbortReadResult.Keep
        ElseIf sender Is BtnDiscard Then
            _Result = AbortReadResult.Discard
        Else
            _Result = AbortReadResult.Continue
        End If

        Me.Close()
    End Sub

    Private Sub AbortReadForm_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        e.Graphics.DrawImage(SystemIcons.Question.ToBitmap, 22, 24)
    End Sub

    Private Sub AbortReadForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        BtnContinue.Select()
    End Sub

#End Region

End Class
