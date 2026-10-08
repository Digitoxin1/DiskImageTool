Public Class TextViewForm
    Private m_SaveFileName As String
    Private m_OriginalBytes() As Byte

    Public Sub New(Caption As String, Content As String, Editable As Boolean, WrapText As Boolean, EnableSave As Boolean, Optional SaveFileName As String = "", Optional OriginalBytes() As Byte = Nothing)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        Me.Text = Caption
        TextBox1.Text = Content
        TextBox1.SelectionStart = 0
        TextBox1.ReadOnly = Not Editable
        CheckWordWrap.Checked = WrapText
        ApplyWordWrap(WrapText)
        LayoutWordWrap()

        m_SaveFileName = SaveFileName
        m_OriginalBytes = OriginalBytes

        If Not EnableSave Then
            PanelBottom.Visible = False
        End If
    End Sub

    Public Shared Sub Display(Caption As String, Content As String, Editable As Boolean, WrapText As Boolean, EnableSave As Boolean, Optional SaveFileName As String = "", Optional OriginalBytes() As Byte = Nothing)
        Using dlg As New TextViewForm(Caption, Content, Editable, WrapText, EnableSave, SaveFileName, OriginalBytes)
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
                If m_OriginalBytes IsNot Nothing Then
                    IO.File.WriteAllBytes(Dialog.FileName, m_OriginalBytes)
                Else
                    IO.File.WriteAllText(Dialog.FileName, TextBox1.Text)
                End If
            End If
        End Using
    End Sub

    Private Sub ApplyWordWrap(Wrap As Boolean)
        If Wrap Then
            TextBox1.ScrollBars = ScrollBars.Vertical
            TextBox1.WordWrap = True
        Else
            TextBox1.WordWrap = False
            TextBox1.ScrollBars = ScrollBars.Both
        End If
    End Sub

    Private Sub CheckWordWrap_CheckedChanged(sender As Object, e As EventArgs) Handles CheckWordWrap.CheckedChanged
        ApplyWordWrap(CheckWordWrap.Checked)
    End Sub

    Private Sub LayoutWordWrap()
        Dim Used = PanelBottom.Padding.Horizontal _
            + BtnClose.Margin.Horizontal + BtnClose.Width _
            + BtnSave.Margin.Horizontal + BtnSave.Width _
            + CheckWordWrap.Margin.Horizontal + CheckWordWrap.Width
        Dim SpacerWidth = Math.Max(0, PanelBottom.ClientSize.Width - Used)
        If PanelSpacer.Width <> SpacerWidth Then
            PanelSpacer.Width = SpacerWidth
        End If
    End Sub

    Private Sub LocalizeForm()
        BtnClose.Text = WithoutHotkey(My.Resources.Menu_Close)
        BtnSave.Text = WithoutHotkey(My.Resources.Menu_Save)
        CheckWordWrap.Text = My.Resources.Label_WordWrap
    End Sub

    Private Sub PanelBottom_Resize(sender As Object, e As EventArgs) Handles PanelBottom.Resize
        LayoutWordWrap()
    End Sub

    Private Sub TextViewForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub
End Class