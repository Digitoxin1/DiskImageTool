Public Class TextViewForm

    Public Sub New(Caption As String, Content As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Text = Caption
        TextBox1.Text = Content
        TextBox1.SelectionStart = 0
    End Sub

    Private Sub TextViewForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub
End Class