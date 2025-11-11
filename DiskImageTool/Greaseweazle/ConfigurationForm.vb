Namespace Greaseweazle
    Public Class ConfigurationForm
        Private ReadOnly ToolTip1 As New ToolTip()

        Private Sub InitializeDriveType(Combo As ComboBox, CurrentValue As GreaseweazleFloppyType)
            Dim DriveList As New List(Of KeyValuePair(Of String, GreaseweazleFloppyType))

            For Each FloppyType As GreaseweazleFloppyType In [Enum].GetValues(GetType(GreaseweazleFloppyType))
                DriveList.Add(New KeyValuePair(Of String, GreaseweazleFloppyType)(
                    GreaseweazleFloppyTypeDescription(FloppyType), FloppyType)
                )
            Next

            InitializeCombo(Combo, DriveList, CurrentValue)
        End Sub

        Private Sub InitializeFields()
            SetExePath(My.Settings.GW_Path)

            InitializeInterfaceName(ComboInterface, GreaseweazleInterfaceFromName(My.Settings.GW_Interface))
            InitializeDriveType(ComboDriveType0, GreaseweazleFloppyTypeFromName(My.Settings.GW_DriveType0))
            InitializeDriveType(ComboDriveType1, GreaseweazleFloppyTypeFromName(My.Settings.GW_DriveType1))
            InitializeDriveType(ComboDriveType2, GreaseweazleFloppyTypeFromName(My.Settings.GW_DriveType2))
        End Sub

        Private Sub InitializeInterfaceName(Combo As ComboBox, CurrentValue As GreaseweazleInterface)
            Dim DriveList As New List(Of KeyValuePair(Of String, GreaseweazleInterface))

            For Each InterfaceType As GreaseweazleInterface In [Enum].GetValues(GetType(GreaseweazleInterface))
                DriveList.Add(New KeyValuePair(Of String, GreaseweazleInterface)(
                    GreaseweazleInterfaceName(InterfaceType), InterfaceType)
                )
            Next

            InitializeCombo(Combo, DriveList, CurrentValue)

            RefreshDriveTypes(CurrentValue)
        End Sub

        Private Sub RefreshDriveTypes(DriveInterface As GreaseweazleInterface)
            If DriveInterface = GreaseweazleInterface.Shugart Then
                LabelDriveType0.Text = String.Format(My.Resources.Label_DriveType, "0")
                LabelDriveType1.Text = String.Format(My.Resources.Label_DriveType, "1")
                LabelDriveType2.Text = String.Format(My.Resources.Label_DriveType, "2")

                LabelDriveType2.Visible = True
                ComboDriveType2.Visible = True
            Else
                LabelDriveType0.Text = String.Format(My.Resources.Label_DriveType, "A")
                LabelDriveType1.Text = String.Format(My.Resources.Label_DriveType, "B")

                LabelDriveType2.Visible = False
                ComboDriveType2.Visible = False
            End If
        End Sub

        Private Sub RefreshInfo()
            Dim Enabled As Boolean = IsValidGreaseweazlePath(TextBoxPath.Text)

            ButtonInfo.Enabled = Enabled
            TextBoxInfo.Enabled = Enabled
        End Sub

        Private Function ResolveShortcutTarget(lnkPath As String) As String
            Try
                If Not IO.Path.GetExtension(lnkPath).Equals(".lnk", StringComparison.OrdinalIgnoreCase) Then
                    Return lnkPath
                End If
                Dim shell = CreateObject("WScript.Shell")
                Dim shortcut = shell.CreateShortcut(lnkPath)
                Dim target As String = CStr(shortcut.TargetPath)
                If Not String.IsNullOrWhiteSpace(target) Then
                    Return target
                End If
            Catch
                ' ignore and fall through
            End Try

            Return lnkPath
        End Function

        Private Sub RunAppAndCaptureOutput(appPath As String, arguments As String)
            If String.IsNullOrWhiteSpace(appPath) OrElse Not IO.File.Exists(appPath) Then
                DisplayInvalidApplicationPathMsg()
                Exit Sub
            End If

            Me.Cursor = Cursors.WaitCursor
            Application.DoEvents()

            Dim Content As String = ""
            Try
                Dim Result = ConsoleProcessRunner.RunProcess(appPath, arguments)
                Content = Result.CombinedOutput
            Finally
                Me.Cursor = Cursors.Default
            End Try

            TextBoxInfo.Text = Content
        End Sub

        Private Sub SetExePath(path As String)
            TextBoxPath.Text = path
            ToolTip1.SetToolTip(TextBoxPath, path)
            RefreshInfo()
        End Sub

#Region "Events"
        Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
            If TextBoxPath.Text = "" OrElse IO.File.Exists(TextBoxPath.Text) Then
                My.Settings.GW_Path = TextBoxPath.Text
            End If

            My.Settings.GW_Interface = GreaseweazleInterfaceName(ComboInterface.SelectedValue)
            My.Settings.GW_DriveType0 = GreaseweazleFloppyTypeName(ComboDriveType0.SelectedValue)
            My.Settings.GW_DriveType1 = GreaseweazleFloppyTypeName(ComboDriveType1.SelectedValue)

            If DirectCast(ComboInterface.SelectedValue, GreaseweazleInterface) = GreaseweazleInterface.Shugart Then
                My.Settings.GW_DriveType2 = GreaseweazleFloppyTypeName(ComboDriveType2.SelectedValue)
            Else
                My.Settings.GW_DriveType2 = ""
            End If
        End Sub

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
                    If IsExecutable(ofd.FileName) Then
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

        Private Sub ButtonInfo_Click(sender As Object, e As EventArgs) Handles ButtonInfo.Click
            Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.info)
            Dim Arguments = Builder.Arguments
            RunAppAndCaptureOutput(TextBoxPath.Text, Arguments)
        End Sub

        Private Sub ComboInterface_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboInterface.SelectedIndexChanged
            RefreshDriveTypes(DirectCast(ComboInterface.SelectedValue, GreaseweazleInterface))
        End Sub

        Private Sub GreaseweazleConfigurationForm_Load(sender As Object, e As EventArgs) Handles Me.Load
            InitializeFields()
        End Sub

        Private Sub TextBoxPath_DragDrop(sender As Object, e As DragEventArgs) Handles TextBoxPath.DragDrop
            Dim files = CType(e.Data.GetData(DataFormats.FileDrop), String())
            ' Prefer the first valid .exe (resolve .lnk if needed)
            For Each f In files
                Dim candidate As String = f
                If IO.Path.GetExtension(f).Equals(".lnk", StringComparison.OrdinalIgnoreCase) Then
                    candidate = ResolveShortcutTarget(f) ' may return original if resolution fails
                End If
                If IsExecutable(candidate) Then
                    SetExePath(candidate)
                    Exit For
                End If
            Next
        End Sub

        Private Sub TextBoxPath_DragEnter(sender As Object, e As DragEventArgs) Handles TextBoxPath.DragEnter
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Dim files = CType(e.Data.GetData(DataFormats.FileDrop), String())
                ' Accept only if there is at least one .exe (or a .lnk that points to an .exe)
                If files.Any(Function(f) IsExecutable(f) OrElse (IO.Path.GetExtension(f).Equals(".lnk", StringComparison.OrdinalIgnoreCase) AndAlso
                                                            IsExecutable(ResolveShortcutTarget(f)))) Then
                    e.Effect = DragDropEffects.Copy
                    Return
                End If
            End If
            e.Effect = DragDropEffects.None
        End Sub
#End Region
    End Class
End Namespace
