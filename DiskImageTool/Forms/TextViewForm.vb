Public Class TextViewForm
    Private m_SaveFileName As String

    Public Sub New(Caption As String, Content As String, Editable As Boolean, EnableSave As Boolean, Optional SaveFileName As String = "")

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        Me.Text = Caption
        TextBox1.Text = Content
        TextBox1.SelectionStart = 0
        TextBox1.ReadOnly = Not Editable

        m_SaveFileName = SaveFileName

        If Not EnableSave Then
            PanelBottom.Visible = False
            TextBox1.Height = Me.ClientSize.Height - TextBox1.Top * 2
        End If
    End Sub

    Public Shared Sub Display(Caption As String, Content As String, Editable As Boolean, EnableSave As Boolean, Optional SaveFileName As String = "")
        Using dlg As New TextViewForm(Caption, Content, Editable, EnableSave, SaveFileName)
            dlg.ShowDialog(App.CurrentFormInstance)
        End Using
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        Dim Extension = IO.Path.GetExtension(m_SaveFileName).ToLower
        Dim FilterIndex As Integer = 1
        If Extension <> ".txt" Then
            FilterIndex = 2
        End If

        Using Dialog As New SaveFileDialog With {
               .FileName = m_SaveFileName,
               .DefaultExt = "txt",
               .Filter = My.Resources.FileType_Text & " (*.txt)|*.txt|" & My.Resources.FileType_All & " (*.*)|*.*",
               .FilterIndex = FilterIndex
            }

            If Not String.IsNullOrEmpty(m_SaveFileName) Then
                Dim Path = IO.Path.GetDirectoryName(m_SaveFileName)

                Dialog.FileName = IO.Path.GetFileName(m_SaveFileName)

                If Not String.IsNullOrEmpty(Path) AndAlso IO.Directory.Exists(Path) Then
                    Dialog.InitialDirectory = Path
                    Dialog.RestoreDirectory = True
                End If
            End If

            If Dialog.ShowDialog = DialogResult.OK Then
                IO.File.WriteAllText(Dialog.FileName, TextBox1.Text)
            End If
        End Using
    End Sub

    Private Sub LocalizeForm()
        BtnClose.Text = WithoutHotkey(My.Resources.Menu_Close)
        BtnSave.Text = WithoutHotkey(My.Resources.Menu_Save)
    End Sub

    Private Sub TextViewForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub
End Class