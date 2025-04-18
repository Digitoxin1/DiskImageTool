Public Class TextViewForm
    Private m_SaveFileName As String

    Public Sub New(Caption As String, Content As String, Editable As Boolean, EnableSave As Boolean, Optional SaveFileName As String = "")

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Text = Caption
        TextBox1.Text = Content
        TextBox1.SelectionStart = 0
        TextBox1.ReadOnly = Not Editable

        m_SaveFileName = SaveFileName

        If Not EnableSave Then
            Panel1.Visible = False
            TextBox1.Height = Me.ClientSize.Height - TextBox1.Top * 2
        End If
    End Sub

    Private Sub TextViewForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        Dim Dialog = New SaveFileDialog With {
           .FileName = m_SaveFileName,
           .DefaultExt = "txt",
           .Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
        }

        If Dialog.ShowDialog = DialogResult.OK Then
            IO.File.WriteAllText(Dialog.FileName, TextBox1.Text)
        End If
    End Sub
End Class