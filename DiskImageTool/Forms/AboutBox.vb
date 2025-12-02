Public NotInheritable Class AboutBox
    Private ReadOnly _NameSpace As String = New StubClass().GetType.Namespace

    Private Sub AboutBox1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Set the title of the form.
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = My.Resources.Label_About & " " & ApplicationTitle
        ' Initialize all of the text displayed on the About Box.
        ' TODO: Customize the application's assembly information in the "Application" pane of the project 
        '    properties dialog (under the "Project" menu).
        Me.LabelProductName.Text = My.Application.Info.ProductName
        Me.LabelVersionCaption.Text = My.Resources.Label_Version & ":"
        Me.LabelVersion.Text = GetVersionString()
        Me.LabelDBMain.Text = My.Resources.Label_Database & ":"
        Me.LabelDBVersionMain.Text = If(String.IsNullOrEmpty(App.TitleDB.Version), "N/A", App.TitleDB.Version)
        Me.LabelDBCountMain.Text = InParens(String.Format(My.Resources.Label_Entries, App.TitleDB.MainCount))
        If String.IsNullOrEmpty(App.TitleDB.MainPath) Then
            Me.LabelDBVersionMain.LinkBehavior = LinkBehavior.NeverUnderline
            Me.LabelDBVersionMain.Enabled = False
        End If
        Me.LabelURL.Text = My.Resources.URL_Repository
        Me.TextBoxDescription.Text = GetResource("License.txt")

        OKButton.Text = My.Resources.Menu_Ok
    End Sub

    Private Function GetResource(Name As String) As String
        Dim Value As String

        Dim Assembly As Reflection.[Assembly] = Reflection.[Assembly].GetExecutingAssembly()
        Dim Stream = Assembly.GetManifestResourceStream(_NameSpace & "." & Name)
        If Stream Is Nothing Then
            Throw New Exception("Unable to load resource " & Name)
        Else
            Dim TextStreamReader As New IO.StreamReader(Stream)
            Value = TextStreamReader.ReadToEnd()
            TextStreamReader.Close()
        End If

        Return Value
    End Function

    Private Sub LabelDBVersion_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LabelDBVersionMain.LinkClicked
        If Not String.IsNullOrEmpty(App.TitleDB.MainPath) Then
            Dim Folder = IO.Path.GetDirectoryName(App.TitleDB.MainPath)
            If IO.Directory.Exists(Folder) Then
                Try
                    Dim psi As New ProcessStartInfo() With {
                        .FileName = Folder,
                        .UseShellExecute = True
                    }
                    Process.Start(psi)
                Catch ex As Exception
                End Try
            End If
        End If
    End Sub

    Private Sub LabelURL_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LabelURL.LinkClicked
        Process.Start(My.Resources.URL_Repository)
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub
End Class
