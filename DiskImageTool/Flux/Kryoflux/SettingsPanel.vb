Imports System.ComponentModel

Namespace Flux.Kryoflux
    Public Class SettingsPanel
        Private ReadOnly ToolTip1 As New ToolTip()
        Private _Initialized As Boolean = False

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            LocalizeForm()
        End Sub

        Private Sub LocalizeForm()
            LabelApplicationPath.Text = My.Resources.Label_ApplicationPath
            ButtonBrowse.Text = My.Resources.Label_Browse
            ButtonClear.Text = My.Resources.Menu_Clear
            LabelLogFile.Text = My.Resources.Label_LogFilename
        End Sub

        Public ReadOnly Property Initialized As Boolean
            Get
                Return _Initialized
            End Get
        End Property

        Public Sub Initialize()
            InitializeFields()
            _Initialized = True
        End Sub

        Public Sub UpdateSettings()
            If Not _Initialized Then
                Exit Sub
            End If

            If TextBoxPath.Text = "" OrElse IO.File.Exists(TextBoxPath.Text) Then
                Settings.AppPath = TextBoxPath.Text
            End If

            Settings.LogFileName = TextBoxLogFile.Text.Trim
            Settings.LogStripPath = CheckBoxStripPath.Checked
        End Sub

        Private Sub InitializeFields()
            SetExePath(Settings.AppPath)

            TextBoxLogFile.Text = Settings.LogFileName
            CheckBoxStripPath.Checked = Settings.LogStripPath
        End Sub

        Private Sub SetExePath(path As String)
            TextBoxPath.Text = path
            ToolTip1.SetToolTip(TextBoxPath, path)
        End Sub
#Region "Events"

        Private Sub ButtonBrowse_Click(sender As Object, e As EventArgs) Handles ButtonBrowse.Click
            Using ofd As New OpenFileDialog()
                ofd.Title = My.Resources.Greaseweazle_SelectExecutable
                ofd.Filter = "Executable files (*.exe)|*.exe"
                ofd.CheckFileExists = True
                ofd.Multiselect = False
                ' If you already have a path, start there; otherwise use Program Files
                If IO.File.Exists(TextBoxPath.Text) Then
                    ofd.InitialDirectory = IO.Path.GetDirectoryName(TextBoxPath.Text)
                    ofd.FileName = IO.Path.GetFileName(TextBoxPath.Text)
                Else
                    ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                End If

                If ofd.ShowDialog(Me) = DialogResult.OK Then
                    If IsPathValid(ofd.FileName) Then
                        SetExePath(ofd.FileName)
                    Else
                        Dim Msg = String.Format(My.Resources.Dialog_PleaseSelect, ".exe")
                        MessageBox.Show(Msg, My.Resources.Dialog_InvalidSelection, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                End If
            End Using
        End Sub

        Private Sub ButtonClear_Click(sender As Object, e As EventArgs) Handles ButtonClear.Click
            SetExePath("")
        End Sub
        Private Sub TextBoxLogFile_Validating(sender As Object, e As CancelEventArgs) Handles TextBoxLogFile.Validating
            Dim name = TextBoxLogFile.Text.Trim()

            If Not IsValidFileName(name) Then
                e.Cancel = True   ' Prevent leaving the field
                MessageBox.Show(
                    My.Resources.Dialog_InvalidFilename,
                    My.Resources.Label_InvalidFilename,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                )
                ' Optionally select all text so user can fix it immediately:
                TextBoxLogFile.SelectAll()
            End If
        End Sub

        Private Sub TextBoxPath_DragDrop(sender As Object, e As DragEventArgs) Handles TextBoxPath.DragDrop
            Dim files = CType(e.Data.GetData(DataFormats.FileDrop), String())
            ' Prefer the first valid .exe (resolve .lnk if needed)
            For Each f In files
                Dim candidate As String = f
                If IO.Path.GetExtension(f).Equals(".lnk", StringComparison.OrdinalIgnoreCase) Then
                    candidate = ResolveShortcutTarget(f) ' may return original if resolution fails
                End If
                If IsPathValid(candidate) Then
                    SetExePath(candidate)
                    Exit For
                End If
            Next
        End Sub

        Private Sub TextBoxPath_DragEnter(sender As Object, e As DragEventArgs) Handles TextBoxPath.DragEnter
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Dim files = CType(e.Data.GetData(DataFormats.FileDrop), String())
                ' Accept only if there is at least one .exe (or a .lnk that points to an .exe)
                If files.Any(Function(f) IsPathValid(f) OrElse (IO.Path.GetExtension(f).Equals(".lnk", StringComparison.OrdinalIgnoreCase) AndAlso
                                                            IsPathValid(ResolveShortcutTarget(f)))) Then
                    e.Effect = DragDropEffects.Copy
                    Return
                End If
            End If
            e.Effect = DragDropEffects.None
        End Sub
#End Region
    End Class
End Namespace
