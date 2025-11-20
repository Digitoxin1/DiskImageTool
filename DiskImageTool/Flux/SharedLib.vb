Imports System.Text.RegularExpressions
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux
    Module SharedLib
        Public Const REGEX_RAW_FILE As String = "^(?<diskId>.+?)\.?(?<track>\d{2})\.(?<side>\d)\.(?<ext>raw|stream)$"
        Public ReadOnly RESERVED_FILE_NAMES As String() = {
                "CON", "PRN", "AUX", "NUL",
                "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
            }

        Public Function AnalyzeFluxImage(FilePath As String, AllowSCP As Boolean) As (Result As Boolean, TrackCount As Integer, SideCount As Integer)
            Dim AnalyzeResponse As (Result As Boolean, TrackCount As Integer, SideCount As Integer)

            Dim FileExt = IO.Path.GetExtension(FilePath).ToLower
            AnalyzeResponse.Result = False
            AnalyzeResponse.TrackCount = 0
            AnalyzeResponse.SideCount = 0

            If FileExt = ".raw" Then
                Dim Response = GetTrackCountRaw(FilePath)
                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_InvalidKryofluxFile, MsgBoxStyle.Exclamation)
                    Return AnalyzeResponse
                Else
                    AnalyzeResponse.TrackCount = Response.Tracks
                    AnalyzeResponse.SideCount = Response.Sides
                End If
            ElseIf AllowSCP AndAlso FileExt = ".scp" Then
                Dim Response = GetTrackCountSCP(FilePath)
                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_InvalidSCPFile, MsgBoxStyle.Exclamation)
                    Return AnalyzeResponse
                Else
                    AnalyzeResponse.TrackCount = Response.Tracks
                    AnalyzeResponse.SideCount = Response.Sides
                End If
            Else
                MsgBox(My.Resources.Dialog_InvalidFileType, MsgBoxStyle.Exclamation)
                Return AnalyzeResponse
            End If

            If AnalyzeResponse.SideCount > 2 Then
                AnalyzeResponse.SideCount = 2
            End If

            If AnalyzeResponse.TrackCount > 42 And AnalyzeResponse.TrackCount < 80 Then
                AnalyzeResponse.TrackCount = 80
            End If

            AnalyzeResponse.Result = True

            Return AnalyzeResponse
        End Function

        Public Function BrowseFolder(CurrentPath As String) As String
            Using ofd As New FolderBrowserDialog()
                ofd.Description = "Flux Set Root Folder"
                ofd.SelectedPath = CurrentPath
                ofd.ShowNewFolderButton = True
                If ofd.ShowDialog() = DialogResult.OK Then
                    Return ofd.SelectedPath
                End If
            End Using

            Return ""
        End Function

        Public Sub BumpTabIndexes(panel As FlowLayoutPanel)
            For Each ctrl As Control In panel.Controls
                ctrl.TabIndex += 1
            Next
        End Sub

        Public Sub DeleteFilesAndFolderIfEmpty(folderPath As String, ParamArray patterns As String())
            If String.IsNullOrWhiteSpace(folderPath) Then
                Exit Sub
            End If

            If Not IO.Directory.Exists(folderPath) Then
                Exit Sub
            End If

            ' Normalize patterns
            Dim validPatterns = patterns.Where(Function(p) Not String.IsNullOrWhiteSpace(p)).Distinct(StringComparer.OrdinalIgnoreCase).ToList()

            If validPatterns.Count = 0 Then
                Exit Sub
            End If

            ' Delete all matching files
            For Each pattern In validPatterns
                For Each filePath In IO.Directory.GetFiles(folderPath, pattern, IO.SearchOption.TopDirectoryOnly)
                    Try
                        IO.File.Delete(filePath)
                    Catch ex As IO.IOException
                        Debug.WriteLine($"Failed to delete {filePath}: {ex.Message}")
                    Catch ex As UnauthorizedAccessException
                        Debug.WriteLine($"Access denied deleting {filePath}: {ex.Message}")
                    End Try
                Next
            Next

            ' Remove directory if empty
            Try
                If Not IO.Directory.EnumerateFileSystemEntries(folderPath).Any() Then
                    IO.Directory.Delete(folderPath)
                End If
            Catch ex As IO.IOException
                ' Optional: swallow errors
            Catch ex As UnauthorizedAccessException
                ' Optional: swallow errors
            End Try
        End Sub

        Public Function DetectImageFormat(FileName As String, DeleteWhenDone As Boolean) As DiskImage.FloppyDiskFormat
            Dim Buffer As Byte()
            Dim SecondOffset As Long = 4096
            Dim Length As Integer = 513
            Dim DetectedFormat As FloppyDiskFormat

            Using fs As New IO.FileStream(FileName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
                Using br As New IO.BinaryReader(fs, System.Text.Encoding.ASCII, leaveOpen:=False)
                    Buffer = br.ReadBytes(Length)
                    DetectedFormat = FloppyDiskFormatGet(Buffer)
                    If DetectedFormat = FloppyDiskFormat.FloppyXDFMicro Then
                        If fs.Length >= SecondOffset + Length Then
                            fs.Seek(SecondOffset, IO.SeekOrigin.Begin)
                            Buffer = br.ReadBytes(513)
                            DetectedFormat = FloppyDiskFormatGet(Buffer)
                        End If
                    End If
                End Using
            End Using

            If DeleteWhenDone Then
                DeleteFileIfExists(FileName)
            End If

            Return DetectedFormat
        End Function

        Public Function FluxDeviceGetInfo(Device As ITrackStatus.FluxDevice) As FluxDeviceInfo?
            Select Case Device
                Case ITrackStatus.FluxDevice.Greaseweazle
                    Return New FluxDeviceInfo(Device, "Greaseweazle", True, True, App.Globals.AppSettings.Greaseweazle)
                Case ITrackStatus.FluxDevice.Kryoflux
                    Return New FluxDeviceInfo(Device, "KryoFlux", False, False, App.Globals.AppSettings.Kryoflux)
                Case Else
                    Return Nothing
            End Select
        End Function

        Public Function FluxDeviceGetList() As List(Of FluxDeviceInfo)
            Dim list As New List(Of FluxDeviceInfo)

            For Each dev As ITrackStatus.FluxDevice In [Enum].GetValues(GetType(ITrackStatus.FluxDevice))

                ' Skip if not available
                If Not FluxDeviceIsAvailable(dev) Then
                    Continue For
                End If

                ' Get FluxDeviceInfo
                Dim info = FluxDeviceGetInfo(dev)
                If info.HasValue Then
                    list.Add(info.Value)
                End If
            Next

            Return list
        End Function

        Public Function FluxDeviceIsAvailable(Device As ITrackStatus.FluxDevice) As Boolean
            Select Case Device
                Case ITrackStatus.FluxDevice.Greaseweazle
                    Return App.AppSettings.Greaseweazle.IsPathValid
                Case ITrackStatus.FluxDevice.Kryoflux
                    Return App.AppSettings.Kryoflux.IsPathValid
                Case Else
                    Return False
            End Select
        End Function

        Public Function GenerateOutputFile(Extension As String) As String
            Dim TempPath = InitTempImagePath()

            If TempPath = "" Then
                MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Critical)
                Return ""
            End If

            Dim FileName = "New Image" & Extension
            Return GenerateUniqueFileName(TempPath, FileName)
        End Function

        Public Function GetFirstRawInFolder(folderPath As String) As String
            Try
                Return IO.Directory.EnumerateFiles(folderPath, "*.raw", IO.SearchOption.TopDirectoryOnly).FirstOrDefault()
            Catch
                ' If unreadable, treat as no .raw files
                Return Nothing
            End Try
        End Function

        Public Function GetTrackCountRaw(FilePath As String) As (Result As Boolean, Tracks As Integer, Sides As Integer)
            Dim TrackCount As Integer = 0
            Dim SideCount As Integer = 0

            If String.IsNullOrWhiteSpace(FilePath) OrElse Not IO.File.Exists(FilePath) Then
                Return (False, TrackCount, SideCount)
            End If

            If Not FilePath.EndsWith(".raw", StringComparison.OrdinalIgnoreCase) Then
                Return (False, TrackCount, SideCount)
            End If

            Dim ParentDir = IO.Path.GetDirectoryName(FilePath)
            If String.IsNullOrEmpty(ParentDir) OrElse Not IO.Directory.Exists(ParentDir) Then
                Return (False, TrackCount, SideCount)
            End If

            Dim BaseName = IO.Path.GetFileName(FilePath)
            Dim PrefixMatch = Regex.Match(BaseName, REGEX_RAW_FILE, RegexOptions.IgnoreCase)
            If Not PrefixMatch.Success Then
                Return (False, TrackCount, SideCount)
            End If

            Dim Prefix As String = PrefixMatch.Groups("diskId").Value
            Dim rx As New Regex(REGEX_RAW_FILE, RegexOptions.IgnoreCase)

            For Each file In IO.Directory.EnumerateFiles(ParentDir, Prefix & "*.raw", IO.SearchOption.TopDirectoryOnly)
                Dim name = IO.Path.GetFileName(file)
                Dim m = rx.Match(name)
                If m.Success Then
                    Dim diskId As String = m.Groups("diskId").Value
                    If String.Equals(diskId, Prefix, StringComparison.OrdinalIgnoreCase) Then
                        Dim trk As Integer = Integer.Parse(m.Groups("track").Value)
                        Dim side As Integer = Integer.Parse(m.Groups("side").Value)

                        If trk >= TrackCount Then
                            TrackCount = trk + 1
                        End If

                        If side >= SideCount Then
                            SideCount = side + 1
                        End If
                    End If
                End If
            Next

            Return (True, TrackCount, SideCount)
        End Function

        Public Function GetTrackCountSCP(FilePath As String) As (Result As Boolean, Tracks As Integer, Sides As Integer)
            If String.IsNullOrWhiteSpace(FilePath) OrElse Not IO.File.Exists(FilePath) Then
                Return (False, 0, 0)
            End If

            If Not FilePath.EndsWith(".scp", StringComparison.OrdinalIgnoreCase) Then
                Return (False, 0, 0)
            End If

            Using fs As New IO.FileStream(FilePath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
                Using br As New IO.BinaryReader(fs, System.Text.Encoding.ASCII, leaveOpen:=False)
                    Dim sig = br.ReadBytes(3) ' "SCP"
                    If sig.Length <> 3 OrElse sig(0) <> AscW("S"c) OrElse sig(1) <> AscW("C"c) OrElse sig(2) <> AscW("P"c) Then
                        Return (False, 0, 0)
                    End If

                    ' Seek to relevant header bytes
                    fs.Position = &H6
                    Dim startTrack As Integer = br.ReadByte()  ' byte 0x06
                    Dim endTrack As Integer = br.ReadByte()    ' byte 0x07
                    Dim flags As Integer = br.ReadByte()       ' byte 0x08
                    Dim heads As Integer = br.ReadByte()       ' byte 0x0A

                    Dim sideCount As Integer
                    Select Case heads
                        Case 0 : sideCount = 2
                        Case 1, 2 : sideCount = 1
                        Case Else : sideCount = 2
                    End Select

                    Return (True, endTrack \ sideCount + 1, sideCount)
                End Using
            End Using
        End Function

        Public Function ImportFluxImage(FilePath As String, ParentForm As MainForm) As (Result As Boolean, OutputFile As String, NewFileName As String)
            Dim AnalyzeResponse = AnalyzeFluxImage(FilePath, True)

            If Not AnalyzeResponse.Result Then
                Return (False, "", "")
            End If

            Using form As New ImportImageForm(FilePath, AnalyzeResponse.TrackCount, AnalyzeResponse.SideCount)

                Dim handler As ImportImageForm.ImportRequestedEventHandler =
                    Sub(File, NewName)
                        ParentForm.ProcessImportedImage(File, NewName)
                    End Sub

                AddHandler form.ImportRequested, handler

                Dim result As DialogResult = DialogResult.Cancel
                Try
                    result = form.ShowDialog(ParentForm)
                Finally
                    RemoveHandler form.ImportRequested, handler
                End Try

                If result Then
                    If Not String.IsNullOrEmpty(form.OutputFilePath) Then
                        Return (True, form.OutputFilePath, form.GetNewFileName)
                    End If
                End If

                Return (False, "", "")
            End Using
        End Function
        Public Sub InitializeCombo(Combo As ComboBox, DataSource As Object, CurrentValue As Object)
            Combo.DisplayMember = "Key"
            Combo.ValueMember = "Value"
            Combo.DataSource = DataSource
            Combo.DropDownStyle = ComboBoxStyle.DropDownList

            If CurrentValue IsNot Nothing Then
                Combo.SelectedValue = CurrentValue
            End If
        End Sub
        Public Function IsExecutable(path As String) As Boolean
            Return Not String.IsNullOrWhiteSpace(path) AndAlso
            IO.File.Exists(path) AndAlso
            IO.Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase)
        End Function

        Public Function IsPathValid(Path As String) As Boolean
            Dim IsValid As Boolean

            If Path = "" Then
                IsValid = False
            ElseIf Not IsExecutable(Path) Then
                IsValid = False
            Else
                IsValid = True
            End If

            Return IsValid
        End Function

        Public Function IsValidFileName(name As String) As Boolean
            If String.IsNullOrWhiteSpace(name) Then Return False

            ' Check Windows filename invalid characters
            If name.IndexOfAny(IO.Path.GetInvalidFileNameChars()) >= 0 Then
                Return False
            End If

            ' Check reserved device names
            If RESERVED_FILE_NAMES.Contains(name.Trim().ToUpper()) Then
                Return False
            End If

            Return True
        End Function

        Public Function IsValidFluxImport(path As String, allowSCP As Boolean) As (Result As Boolean, File As String)
            If IO.File.Exists(path) Then
                ' Direct file: only .raw or .scp
                Dim ext = IO.Path.GetExtension(path).ToLowerInvariant()
                If ext = ".raw" OrElse (ext = ".scp" AndAlso allowSCP) Then
                    Return (True, path)
                End If

            ElseIf IO.Directory.Exists(path) Then
                Dim rawFile = GetFirstRawInFolder(path)
                If rawFile IsNot Nothing Then
                    Return (True, rawFile)
                End If
            End If

            Return (False, "")
        End Function

        Public Function OpenFluxImage(ParentForm As Form, AllowSCP As Boolean) As String
            Using Dialog As New OpenFileDialog With {
                .Title = "Open Flux Image",
                .FilterIndex = 1,
                .CheckFileExists = True,
                .AddExtension = True,
                .Multiselect = False
            }
                If AllowSCP Then
                    Dialog.Filter = "Flux dumps (*.raw;*.scp)|*.raw;*.scp|KryoFlux RAW (*.raw)|*.raw|SuperCard Pro (*.scp)|*.scp"
                Else
                    Dialog.Filter = "KryoFlux RAW (*.raw)|*.raw"
                End If

                If Dialog.ShowDialog(ParentForm) = DialogResult.OK Then
                    Return Dialog.FileName
                End If
            End Using

            Return Nothing
        End Function

        Public Sub PopulateFileExtensions(Combo As ComboBox, SelectedFormat As FloppyDiskFormat)
            Dim FileExtensions = BASIC_SECTOR_FILE_EXTENSIONS.Split(","c).ToList()

            Dim SelectedExtension As String = App.AppSettings.GetPreferredExtension(SelectedFormat)
            If SelectedExtension = "" Then
                SelectedExtension = App.AppSettings.GetPreferredExtension(FloppyDiskFormat.FloppyUnknown)
            End If

            Dim items As New List(Of FileExtensionItem)

            For Each ext In FileExtensions
                items.Add(New FileExtensionItem(ext, FloppyDiskFormat.FloppyUnknown))
            Next

            If SelectedFormat <> FloppyDiskFormat.FloppyUnknown Then
                Dim Params = FloppyDiskFormatGetParams(SelectedFormat)
                If Not String.IsNullOrWhiteSpace(Params.FileExtension) Then
                    Dim idx = items.FindIndex(Function(i) i.Extension.Equals(Params.FileExtension, StringComparison.OrdinalIgnoreCase))
                    If idx = -1 Then
                        items.Add(New FileExtensionItem(Params.FileExtension, SelectedFormat))
                    End If
                End If
            End If

            With Combo
                .DataSource = Nothing
                .Items.Clear()
                .DisplayMember = "Extension"
                .DataSource = items

                Dim selIdx = items.FindIndex(Function(i) i.Extension.Equals(SelectedExtension, StringComparison.OrdinalIgnoreCase))
                If selIdx >= 0 Then
                    .SelectedIndex = selIdx
                ElseIf items.Count > 0 Then
                    .SelectedIndex = 0
                End If

                .DropDownStyle = ComboBoxStyle.DropDownList
            End With
        End Sub

        Public Sub PopulateImageFormats(Combo As ComboBox, Opt As DriveOption)
            If Opt.Id = "" Then
                ClearImageFormats(Combo)
            Else
                PopulateImageFormats(Combo, Opt.SelectedFormat, Opt.DetectedFormat)
            End If
        End Sub

        Public Sub PopulateImageFormats(Combo As ComboBox, SelectedFormat As FloppyDiskFormat?, DetectedFormat As FloppyDiskFormat?)
            Dim list = FloppyDiskFormatGetComboList()

            For i As Integer = 0 To list.Count - 1
                Dim item = list(i)
                If DetectedFormat.HasValue AndAlso item.Format = DetectedFormat.Value Then
                    item.Detected = True
                Else
                    item.Detected = False
                End If
                list(i) = item
            Next

            With Combo
                .DisplayMember = "Description"
                .ValueMember = Nothing
                .DataSource = list
                .DropDownStyle = ComboBoxStyle.DropDownList
            End With

            If SelectedFormat.HasValue Then
                Dim idx = list.FindIndex(Function(p) p.Format = SelectedFormat.Value)
                If idx >= 0 Then
                    Combo.SelectedIndex = idx
                End If
            End If
        End Sub

        Public Function ResolveShortcutTarget(lnkPath As String) As String
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

        Private Sub ClearImageFormats(Combo As ComboBox)
            Dim list As New List(Of FloppyDiskParams) From {
                CreatePlaceholderParams(My.Resources.Label_PleaseSelect)
            }

            With Combo
                .DisplayMember = "Description"
                .ValueMember = Nothing
                .DataSource = list
                .DropDownStyle = ComboBoxStyle.DropDownList
            End With
        End Sub

        Private Function ContainsRawFileInFolder(folderPath As String) As Boolean
            Try
                ' Recursive search; stop on first match
                For Each f In IO.Directory.EnumerateFiles(folderPath, "*.raw", IO.SearchOption.TopDirectoryOnly)
                    Return True
                Next
            Catch ex As UnauthorizedAccessException
                ' Ignore folders we can't read
            Catch ex As IO.IOException
                ' Ignore IO issues
            End Try

            Return False
        End Function

        Public Structure FileExtensionItem
            Public Sub New(ext As String, fmt As FloppyDiskFormat?)
                Extension = ext
                Format = fmt
            End Sub

            Public Property Extension As String
            Public Property Format As FloppyDiskFormat?
            Public Overrides Function ToString() As String
                Return Extension
            End Function
        End Structure

        Public Structure FluxDeviceInfo
            Public Sub New(Device As ITrackStatus.FluxDevice, Name As String, AllowSCP As Boolean, AllowHFE As Boolean, Settings As ISettings)
                Me.Device = Device
                Me.Name = Name
                Me.AllowSCP = AllowSCP
                Me.AllowHFE = AllowHFE
                Me.Settings = Settings
            End Sub

            ReadOnly Property AllowHFE As Boolean
            ReadOnly Property AllowSCP As Boolean
            ReadOnly Property Device As ITrackStatus.FluxDevice
            ReadOnly Property Name As String
            ReadOnly Property Settings As ISettings
        End Structure

        Public Class DriveOption
            Public Property DetectedFormat As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown
            Public Property Id As String
            Public Property Label As String
            Public Property SelectedFormat As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown
            Public Property Tracks As Byte
            Public Property Type As FloppyMediaType = FloppyMediaType.MediaUnknown
            Public Overrides Function ToString() As String
                Return Label
            End Function
        End Class
    End Module
End Namespace
