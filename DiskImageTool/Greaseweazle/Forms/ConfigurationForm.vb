Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Greaseweazle
    Public Class ConfigurationForm
        Private ReadOnly ToolTip1 As New ToolTip()

        Private Function GetComboDriveType(index As Byte) As ComboBox
            Select Case index
                Case 0
                    Return ComboDriveType0
                Case 1
                    Return ComboDriveType1
                Case Else
                    Return ComboDriveType2
            End Select
        End Function

        Private Function GetNumericTracks(index As Byte) As NumericUpDown
            Select Case index
                Case 0
                    Return NumericTracks0
                Case 1
                    Return NumericTracks1
                Case Else
                    Return NumericTracks2
            End Select
        End Function

        Private Sub InitializeDriveType(index As Byte)
            Dim Combo = GetComboDriveType(index)
            Dim CurrentValue = GreaseweazleSettings.DriveType(index)

            Dim DriveList As New List(Of KeyValuePair(Of String, FloppyMediaType))

            For Each FloppyType As FloppyMediaType In [Enum].GetValues(GetType(FloppyMediaType))
                DriveList.Add(New KeyValuePair(Of String, FloppyMediaType)(
                    GreaseweazleFloppyTypeDescription(FloppyType), FloppyType)
                )
            Next

            InitializeCombo(Combo, DriveList, CurrentValue)
        End Sub

        Private Sub InitializeFields()
            SetExePath(GreaseweazleSettings.AppPath)

            InitializeInterfaceName(ComboInterface, GreaseweazleSettings.Interface)
            InitializePorts(GreaseweazleSettings.COMPort)
            For i = 0 To 2
                InitializeDriveType(i)
                InitializeTracks(i)
            Next
        End Sub

        Private Sub InitializeInput(index As Byte, Type As FloppyMediaType, Tracks As Byte)
            Dim Input = GetNumericTracks(index)

            Dim MinMax = Settings.GetMinMax(Type)

            Input.Minimum = MinMax.Min
            Input.Maximum = MinMax.Max
            If Tracks = 0 Then
                Input.Value = MinMax.Max
            Else
                Input.Value = Math.Min(Math.Max(Tracks, MinMax.Min), MinMax.Max)
            End If
        End Sub

        Private Sub InitializeInterfaceName(Combo As ComboBox, CurrentValue As Settings.GreaseweazleInterface)
            Dim DriveList As New List(Of KeyValuePair(Of String, Settings.GreaseweazleInterface))

            For Each InterfaceType As Settings.GreaseweazleInterface In [Enum].GetValues(GetType(Settings.GreaseweazleInterface))
                DriveList.Add(New KeyValuePair(Of String, Settings.GreaseweazleInterface)(
                    GreaseweazleInterfaceName(InterfaceType), InterfaceType)
                )
            Next

            InitializeCombo(Combo, DriveList, CurrentValue)

            RefreshDriveTypes(CurrentValue)
        End Sub

        Private Sub InitializePorts(CurrentValue As String)
            Dim PortList As New List(Of KeyValuePair(Of String, String)) From {
                New KeyValuePair(Of String, String)(
                   "Auto", "")
            }

            Dim ports = IO.Ports.SerialPort.GetPortNames()

            If CurrentValue <> "" Then
                If Not ports.Contains(CurrentValue, StringComparer.OrdinalIgnoreCase) Then
                    ports = ports.Append(CurrentValue).ToArray()
                End If
            End If

            Dim SortedPorts = ports.OrderBy(Function(name)
                                                Dim num As Integer = Integer.Parse(name.Substring(3))
                                                Return num
                                            End Function)

            For Each p In SortedPorts
                PortList.Add(New KeyValuePair(Of String, String)(
                   p, p)
               )
            Next

            InitializeCombo(ComboPorts, PortList, CurrentValue)
        End Sub
        Private Sub InitializeTracks(index As Byte)
            Dim Type = GreaseweazleSettings.DriveType(index)
            Dim Tracks = GreaseweazleSettings.TrackCount(index)

            InitializeInput(index, Type, Tracks)
        End Sub

        Private Sub RefreshDriveTypes(DriveInterface As Settings.GreaseweazleInterface)
            If DriveInterface = Settings.GreaseweazleInterface.Shugart Then
                LabelDriveType0.Text = String.Format(My.Resources.Label_DriveType, "0")
                LabelDriveType1.Text = String.Format(My.Resources.Label_DriveType, "1")
                LabelDriveType2.Text = String.Format(My.Resources.Label_DriveType, "2")

                LabelDriveType2.Visible = True
                ComboDriveType2.Visible = True

                LabelTracks2.Visible = True
                NumericTracks2.Visible = True
            Else
                LabelDriveType0.Text = String.Format(My.Resources.Label_DriveType, "A")
                LabelDriveType1.Text = String.Format(My.Resources.Label_DriveType, "B")

                LabelDriveType2.Visible = False
                ComboDriveType2.Visible = False

                LabelTracks2.Visible = False
                NumericTracks2.Visible = False
            End If
        End Sub

        Private Sub RefreshInfo()
            Dim Enabled As Boolean = Settings.IsPathValid(TextBoxPath.Text)

            ButtonInfo.Enabled = Enabled
            TextBoxInfo.Enabled = Enabled
        End Sub

        Private Sub RefreshInput(index As Byte)
            Dim CurrentType As FloppyMediaType = GetComboDriveType(index).SelectedValue

            Dim Tracks As Byte = 0

            If CurrentType = GreaseweazleSettings.DriveType(index) Then
                Tracks = GreaseweazleSettings.TrackCount(index)
            End If

            InitializeInput(index, CurrentType, Tracks)

            GetNumericTracks(index).Enabled = (CurrentType <> FloppyMediaType.MediaUnknown)
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
                GreaseweazleSettings.AppPath = TextBoxPath.Text
            End If

            GreaseweazleSettings.Interface = ComboInterface.SelectedValue
            GreaseweazleSettings.COMPort = ComboPorts.SelectedValue
            GreaseweazleSettings.SetDrive(0, ComboDriveType0.SelectedValue, NumericTracks0.Value)
            GreaseweazleSettings.SetDrive(1, ComboDriveType1.SelectedValue, NumericTracks1.Value)

            If DirectCast(ComboInterface.SelectedValue, Settings.GreaseweazleInterface) = Settings.GreaseweazleInterface.Shugart Then
                GreaseweazleSettings.SetDrive(2, ComboDriveType2.SelectedValue, NumericTracks2.Value)
            Else
                GreaseweazleSettings.SetDrive(2, FloppyMediaType.MediaUnknown, 0)
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
                    If Settings.IsPathValid(ofd.FileName) Then
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
            Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.info) With {
                .Device = ComboPorts.SelectedValue
            }

            Dim Arguments = Builder.Arguments
            RunAppAndCaptureOutput(TextBoxPath.Text, Arguments)
        End Sub

        Private Sub ComboDriveType0_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDriveType0.SelectedIndexChanged
            RefreshInput(0)
        End Sub

        Private Sub ComboDriveType1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDriveType1.SelectedIndexChanged
            RefreshInput(1)
        End Sub

        Private Sub ComboDriveType2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDriveType2.SelectedIndexChanged
            RefreshInput(2)
        End Sub

        Private Sub ComboInterface_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboInterface.SelectedIndexChanged
            RefreshDriveTypes(DirectCast(ComboInterface.SelectedValue, Settings.GreaseweazleInterface))
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
                If Settings.IsPathValid(candidate) Then
                    SetExePath(candidate)
                    Exit For
                End If
            Next
        End Sub

        Private Sub TextBoxPath_DragEnter(sender As Object, e As DragEventArgs) Handles TextBoxPath.DragEnter
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Dim files = CType(e.Data.GetData(DataFormats.FileDrop), String())
                ' Accept only if there is at least one .exe (or a .lnk that points to an .exe)
                If files.Any(Function(f) Settings.IsPathValid(f) OrElse (IO.Path.GetExtension(f).Equals(".lnk", StringComparison.OrdinalIgnoreCase) AndAlso
                                                            Settings.IsPathValid(ResolveShortcutTarget(f)))) Then
                    e.Effect = DragDropEffects.Copy
                    Return
                End If
            End If
            e.Effect = DragDropEffects.None
        End Sub
#End Region
    End Class
End Namespace
