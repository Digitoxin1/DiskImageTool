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

        Public Function AnalyzeFluxImage(FilePath As String, AllowSCP As Boolean, Optional ReadHeaders As Boolean = False) As FluxSetInfo
            Dim FileExt = IO.Path.GetExtension(FilePath).ToLower

            Dim Response = New FluxSetInfo(False, 0, 0)

            If FileExt = ".raw" Then
                Response = GetFluxSetInfoRaw(FilePath, ReadHeaders)
                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_InvalidKryofluxFile, MsgBoxStyle.Exclamation)
                    Return Response
                End If
            ElseIf AllowSCP AndAlso FileExt = ".scp" Then
                Response = GetFluxSetInfoSCP(FilePath)
                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_InvalidSCPFile, MsgBoxStyle.Exclamation)
                    Return Response
                End If
            Else
                MsgBox(My.Resources.Dialog_InvalidFileType, MsgBoxStyle.Exclamation)
                Return Response
            End If

            If Response.SideCount > 2 Then
                Response.SideCount = 2
            End If

            If Response.TrackCount > 42 And Response.TrackCount < 80 Then
                Response.TrackCount = 80
            End If

            Response.Result = True

            Return Response
        End Function

        Public Function BrowseFolder(Description As String, CurrentPath As String) As String
            Using ofd As New FolderBrowserDialog()
                ofd.Description = Description
                ofd.SelectedPath = CurrentPath
                ofd.ShowNewFolderButton = True
                If ofd.ShowDialog() = DialogResult.OK Then
                    Return ofd.SelectedPath
                End If
            End Using

            Return ""
        End Function

        Public Sub BumpTabIndexes(panel As FlowLayoutPanel, Increment As Integer)
            For Each ctrl As Control In panel.Controls
                ctrl.TabIndex += Increment
            Next
        End Sub

        Public Function CanToggleSequenceAtCaretOrSelection(tb As TextBoxBase) As Boolean
            Return CanToggleSequenceAtSelection(tb) OrElse CanToggleSequenceAtCaretAfterToken(tb) OrElse CanToggleSequenceAtCaretAfterNumber(tb)
        End Function

        Public Function ConvertFluxImage(FilePath As String, AllowSCP As Boolean, importHandler As ConvertImageForm.ImportProcessEventHandler, LaunchedFromDialog As Boolean) As (Result As DialogResult, OutputFile As String, NewFileName As String)
            Dim TempPath = InitTempImagePath()

            If TempPath = "" Then
                MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Critical)
                Return (DialogResult.Abort, "", "")
            End If

            Dim AnalyzeResponse = AnalyzeFluxImage(FilePath, AllowSCP, True)

            If Not AnalyzeResponse.Result Then
                Return (DialogResult.Abort, "", "")
            End If

            Using dlg As New ConvertImageForm(TempPath, FilePath, AnalyzeResponse, LaunchedFromDialog)

                If importHandler IsNot Nothing Then
                    AddHandler dlg.ImportProcess, importHandler
                End If

                Dim result As DialogResult = DialogResult.Cancel
                Try
                    result = dlg.ShowDialog(App.CurrentFormInstance)
                Finally
                    If importHandler IsNot Nothing Then
                        RemoveHandler dlg.ImportProcess, importHandler
                    End If
                End Try

                If result = DialogResult.OK Or result = DialogResult.Retry Then
                    If Not String.IsNullOrEmpty(dlg.OutputFilePath) Then
                        Return (result, dlg.OutputFilePath, dlg.GetNewFileName)
                    End If
                End If

                Return (result, "", "")
            End Using
        End Function

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

            Try
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
            Catch ex As Exception
                Dim Msg = My.Resources.Dialog_DetectFormatError & vbNewLine & vbNewLine & ex.Message
                MsgBox(Msg, MsgBoxStyle.Critical)
                DetectedFormat = FloppyDiskFormat.FloppyUnknown
            End Try

            If DeleteWhenDone Then
                DeleteTempFileIfExists(FileName)
            End If

            Return DetectedFormat
        End Function

        Public Function GetFirstRawInFolder(folderPath As String) As String
            Try
                Return IO.Directory.EnumerateFiles(folderPath, "*.raw", IO.SearchOption.TopDirectoryOnly).FirstOrDefault()
            Catch
                ' If unreadable, treat as no .raw files
                Return Nothing
            End Try
        End Function

        Public Function GetFluxSetInfoRaw(FilePath As String, Optional ReadHeaders As Boolean = False) As FluxSetInfo
            Dim Response As New FluxSetInfo(False, 0, 0)

            If String.IsNullOrWhiteSpace(FilePath) OrElse Not IO.File.Exists(FilePath) Then
                Return Response
            End If

            If Not FilePath.EndsWith(".raw", StringComparison.OrdinalIgnoreCase) Then
                Return Response
            End If

            Dim ParentDir = IO.Path.GetDirectoryName(FilePath)
            If String.IsNullOrEmpty(ParentDir) OrElse Not IO.Directory.Exists(ParentDir) Then
                Return Response
            End If

            Dim BaseName = IO.Path.GetFileName(FilePath)
            Dim PrefixMatch = Regex.Match(BaseName, REGEX_RAW_FILE, RegexOptions.IgnoreCase)
            If Not PrefixMatch.Success Then
                Return Response
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

                        If trk >= Response.TrackCount Then
                            Response.TrackCount = trk + 1
                        End If

                        If side >= Response.SideCount Then
                            Response.SideCount = side + 1
                        End If

                        If ReadHeaders Then
                            Dim TrackHeaders = ReadRawHeaderInfo(file)
                            Response.AddTrackHeaders(trk, side, TrackHeaders)
                        End If
                    End If
                End If
            Next

            Response.Result = True

            Return Response
        End Function

        Public Function GetFluxSetInfoSCP(FilePath As String) As FluxSetInfo
            Dim Response As New FluxSetInfo(False, 0, 0)

            If String.IsNullOrWhiteSpace(FilePath) OrElse Not IO.File.Exists(FilePath) Then
                Return Response
            End If

            If Not FilePath.EndsWith(".scp", StringComparison.OrdinalIgnoreCase) Then
                Return Response
            End If

            Using fs As New IO.FileStream(FilePath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
                Using br As New IO.BinaryReader(fs, System.Text.Encoding.ASCII, leaveOpen:=False)
                    Dim sig = br.ReadBytes(3) ' "SCP"
                    If sig.Length <> 3 OrElse sig(0) <> AscW("S"c) OrElse sig(1) <> AscW("C"c) OrElse sig(2) <> AscW("P"c) Then
                        Return Response
                    End If

                    ' Seek to relevant header bytes
                    fs.Position = &H6
                    Dim startTrack As Integer = br.ReadByte()  ' byte 0x06
                    Dim endTrack As Integer = br.ReadByte()    ' byte 0x07
                    Dim flags As Integer = br.ReadByte()       ' byte 0x08
                    Dim heads As Integer = br.ReadByte()       ' byte 0x09

                    Dim sideCount As Integer
                    Select Case heads
                        Case 0 : sideCount = 2
                        Case 1, 2 : sideCount = 1
                        Case Else : sideCount = 2
                    End Select

                    Response.Result = True
                    Response.TrackCount = endTrack \ sideCount + 1
                    Response.SideCount = sideCount

                    Return Response
                End Using
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
            If name Is Nothing Then
                name = ""
            End If

            name = name.Trim

            If String.IsNullOrWhiteSpace(name) Then Return False

            ' Check Windows filename invalid characters
            If name.IndexOfAny(IO.Path.GetInvalidFileNameChars()) >= 0 Then
                Return False
            End If

            ' Check reserved device names
            Dim baseName = IO.Path.GetFileNameWithoutExtension(name).ToUpperInvariant()

            If RESERVED_FILE_NAMES.Contains(baseName) Then
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

        Public Function OpenFluxImage(AllowSCP As Boolean) As String
            Using dlg As New OpenFileDialog With {
                .Title = "Open Flux Image",
                .FilterIndex = 1,
                .CheckFileExists = True,
                .AddExtension = True,
                .Multiselect = False
            }
                If AllowSCP Then
                    dlg.Filter = "Flux dumps (*.raw;*.scp)|*.raw;*.scp|KryoFlux RAW (*.raw)|*.raw|SuperCard Pro (*.scp)|*.scp"
                Else
                    dlg.Filter = "KryoFlux RAW (*.raw)|*.raw"
                End If

                If dlg.ShowDialog(App.CurrentFormInstance) = DialogResult.OK Then
                    Return dlg.FileName
                End If
            End Using

            Return Nothing
        End Function

        Public Sub PopulateFileExtensions(Combo As ComboBox, SelectedFormat As FloppyDiskFormat?)
            Dim FileExtensions = BASIC_SECTOR_FILE_EXTENSIONS.Split(","c).ToList()

            Dim SelectedExtension As String = ""

            If SelectedFormat.HasValue Then
                SelectedExtension = App.UserState.GetPreferredExtension(SelectedFormat.Value)
            End If

            If SelectedExtension = "" Then
                SelectedExtension = App.UserState.GetPreferredExtension(FloppyDiskFormat.FloppyUnknown)
            End If

            Dim items As New List(Of FileExtensionItem)

            For Each ext In FileExtensions
                items.Add(New FileExtensionItem(ext, FloppyDiskFormat.FloppyUnknown))
            Next

            If SelectedFormat.HasValue AndAlso SelectedFormat.Value <> FloppyDiskFormat.FloppyUnknown Then
                Dim Params = FloppyDiskFormatGetParams(SelectedFormat.Value)
                If Not String.IsNullOrWhiteSpace(Params.FileExtension) Then
                    Dim idx = items.FindIndex(Function(i) i.Extension.Equals(Params.FileExtension, StringComparison.OrdinalIgnoreCase))
                    If idx = -1 Then
                        items.Add(New FileExtensionItem(Params.FileExtension, SelectedFormat.Value))
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

        Public Sub PopulateImageFormats(Combo As ComboBox, Opt As DriveOption, IncludeUnknown As Boolean, Doublestep As Boolean)
            If Opt.Id = "" Then
                ClearImageFormats(Combo)
            Else
                PopulateImageFormats(Combo, Opt.SelectedFormat, Opt.DetectedFormat, IncludeUnknown, Doublestep)
            End If
        End Sub

        Public Sub PopulateImageFormats(Combo As ComboBox, SelectedFormat As FloppyDiskFormat?, DetectedFormat As FloppyDiskFormat?, IncludeUnknown As Boolean, Doublestep As Boolean)
            Dim list = FloppyDiskFormatGetComboList(IncludeUnknown)

            For i As Integer = 0 To list.Count - 1
                If TypeOf list(i) IsNot FloppyDiskParams Then
                    Continue For
                End If
                Dim item = DirectCast(list(i), FloppyDiskParams)
                If DetectedFormat.HasValue AndAlso item.Format = DetectedFormat.Value Then
                    item.Detected = True
                Else
                    item.Detected = False
                End If

                If Doublestep AndAlso item.IsStandard AndAlso item.DriveType = FloppyDriveType.Drive525DoubleDensity Then
                    item.Doublestep = True
                Else
                    item.Doublestep = False
                End If

                list(i) = item
            Next

            With Combo
                .DisplayMember = ""
                .ValueMember = Nothing
                .DataSource = list
                .DropDownStyle = ComboBoxStyle.DropDownList
            End With

            If SelectedFormat.HasValue Then
                Dim idx = list.FindIndex(Function(p)
                                             If TypeOf p IsNot FloppyDiskParams Then
                                                 Return False
                                             End If

                                             Dim item = DirectCast(p, FloppyDiskParams)

                                             Return p.Format = SelectedFormat.Value
                                         End Function)
                If idx >= 0 Then
                    Combo.SelectedIndex = idx
                End If
            End If
        End Sub

        Public Function ReadRawHeaderInfo(path As String) As Dictionary(Of String, String)
            Dim fields As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            Dim fsOptions = IO.FileOptions.SequentialScan
            Using fs As New IO.FileStream(path, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, 4096, fsOptions)
                Using br As New IO.BinaryReader(fs, Text.Encoding.ASCII, leaveOpen:=False)

                    Dim sawType4 As Boolean = False

                    While fs.Position < fs.Length
                        Dim sign As Integer = br.Read()
                        If sign = -1 Then
                            Exit While
                        End If

                        ' We only care about leading OOB blocks; bail once we hit non-0x0D.
                        If sign <> &HD Then
                            Exit While
                        End If

                        Dim oobType As Integer = br.Read()
                        If oobType = -1 Then
                            Exit While
                        End If

                        Dim sizeLo As Integer = br.Read()
                        Dim sizeHi As Integer = br.Read()
                        If sizeLo = -1 OrElse sizeHi = -1 Then
                            Exit While
                        End If

                        Dim size As Integer = sizeLo Or (sizeHi << 8)

                        If size <= 0 Then
                            ' malformed; bail
                            Exit While
                        End If

                        ' Read payload bytes
                        Dim payload As Byte() = br.ReadBytes(size)
                        If payload.Length < size Then
                            Exit While ' truncated
                        End If

                        If oobType = &H4 Then
                            ' KFInfo string: ASCII key=value pairs
                            sawType4 = True
                            ParseKfInfoPayload(payload, fields)
                        Else
                            ' First non-0x04 OOB after we’ve seen at least one 0x04:
                            ' assume we’re past the header area and stop.
                            If sawType4 Then
                                Exit While
                            End If
                            ' otherwise (weird file where 0x04 comes later): just continue
                        End If
                    End While
                End Using
            End Using

            Return fields
        End Function

        Public Function RemovePathFromLog(logText As String) As String
            Const Prefix As String = "Stream file:"

            Dim lines = logText.Split({vbCrLf, vbLf}, StringSplitOptions.None)

            For i As Integer = 0 To lines.Length - 1
                If lines(i).StartsWith(Prefix, StringComparison.OrdinalIgnoreCase) Then

                    Dim fullPath As String = lines(i).Substring("Stream file:".Length).Trim()

                    Dim lastFolder As String = IO.Path.GetFileName(IO.Path.GetDirectoryName(fullPath))
                    Dim lastItem As String = IO.Path.GetFileName(fullPath)
                    Dim shortPath As String = $"{lastFolder}\{lastItem}"

                    lines(i) = $"{Prefix} {shortPath}"
                End If
            Next

            Return String.Join(vbCrLf, lines)
        End Function

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

        Public Sub SaveLogFile(LogFilePath As String, LogText As String, RemovePath As Boolean)
            If RemovePath Then
                LogText = RemovePathFromLog(LogText)
            End If

            IO.File.WriteAllText(LogFilePath, LogText & vbNewLine)
        End Sub

        Public Function ToggleNumberTokenAtCaretOrSelection(tb As TextBoxBase) As Boolean
            If tb Is Nothing Then
                Return False
            End If

            If tb.SelectionLength > 0 Then
                Return ToggleNumberTokenAtSelection(tb)
            End If

            Dim text As String = tb.Text
            If String.IsNullOrEmpty(text) Then
                Return False
            End If

            Dim caret As Integer = tb.SelectionStart
            If caret <= 0 OrElse caret > text.Length Then
                Return False
            End If

            ' Case A: caret after token ...<123>|
            If CanToggleSequenceAtCaretAfterToken(tb) Then
                ' Find digit run inside the token by scanning left
                Dim endOfDigits As Integer = caret - 1 ' index of '>'
                Dim i As Integer = caret - 2 ' should be a digit

                While i >= 0 AndAlso Char.IsDigit(text(i))
                    i -= 1
                End While
                ' i is now at '<'
                Dim absDigitStart As Integer = i + 1
                Dim digitLen As Integer = endOfDigits - absDigitStart
                Dim absDigitEnd As Integer = absDigitStart + digitLen

                tb.SuspendLayout()
                Try
                    ' remove '>' then '<'
                    tb.Select(absDigitEnd, 1) : tb.SelectedText = ""
                    tb.Select(absDigitStart - 1, 1) : tb.SelectedText = ""

                    ' Put caret after digits
                    tb.Select(absDigitStart + digitLen - 1, 0)
                Finally
                    tb.ResumeLayout()
                End Try

                Return True
            End If

            ' Case B: caret after digits ...123|
            If CanToggleSequenceAtCaretAfterNumber(tb) Then
                ' Find digit run before caret
                Dim endIdx As Integer = caret ' one past last digit
                Dim startIdx As Integer = caret - 1
                While startIdx >= 0 AndAlso Char.IsDigit(text(startIdx))
                    startIdx -= 1
                End While
                startIdx += 1

                Dim absDigitStart As Integer = startIdx
                Dim digitLen As Integer = endIdx - startIdx
                If digitLen <= 0 Then
                    Return False
                End If

                Dim absDigitEnd As Integer = absDigitStart + digitLen

                tb.SuspendLayout()
                Try
                    ' add '>' then '<'
                    tb.Select(absDigitEnd, 0) : tb.SelectedText = ">"
                    tb.Select(absDigitStart, 0) : tb.SelectedText = "<"

                    ' Put caret after '>'
                    tb.Select(absDigitStart + digitLen + 2, 0)
                Finally
                    tb.ResumeLayout()
                End Try

                Return True
            End If

            Return False
        End Function

        Public Function UShortListToRanges(values As List(Of UShort)) As String
            If values Is Nothing OrElse values.Count = 0 Then Return ""

            ' Sort and de-duplicate
            Dim nums = values.Distinct().OrderBy(Function(n) n).ToList()

            Dim ranges As New List(Of String)()
            Dim rangeStart As UShort = nums(0)
            Dim prev As UShort = nums(0)

            For i As Integer = 1 To nums.Count - 1
                Dim current = nums(i)

                If current = prev + 1 Then
                    ' still in a run
                    prev = current
                Else
                    ' range ended
                    If rangeStart = prev Then
                        ranges.Add(rangeStart.ToString())
                    Else
                        ranges.Add($"{rangeStart}-{prev}")
                    End If

                    ' start new range
                    rangeStart = current
                    prev = current
                End If
            Next

            ' close final range
            If rangeStart = prev Then
                ranges.Add(rangeStart.ToString())
            Else
                ranges.Add($"{rangeStart}-{prev}")
            End If

            Return String.Join(", ", ranges)
        End Function

        Public Function ValidateFileName(name As String) As String
            name = name.Trim

            If name.EndsWith(".") Then
                Return "File name cannot end with a period."
            End If

            ' Check Windows filename invalid characters
            For Each c In IO.Path.GetInvalidFileNameChars()
                If name.Contains(c) Then
                    Return $"File name contains an invalid character: {c}"
                End If
            Next

            ' Check reserved device names
            Dim baseName = IO.Path.GetFileNameWithoutExtension(name).ToUpperInvariant()

            If RESERVED_FILE_NAMES.Contains(baseName) Then
                Return "File name is a reserved system name."
            End If

            Return ""
        End Function

        Public Function ValidatePathName(name As String, Optional allowRelative As Boolean = True) As String
            If name Is Nothing Then
                name = ""
            End If

            Dim p = name.Trim()

            ' Quick invalid char check for *path* (note: differs from filename)
            For Each c In IO.Path.GetInvalidPathChars()
                If p.IndexOf(c) >= 0 Then
                    Return $"Path contains an invalid character: {c}"
                End If
            Next

            ' Reject wildcard characters commonly not allowed in paths.
            ' (These are not always included in GetInvalidPathChars.)
            Dim extraInvalid() As Char = {"*"c, "?"c, """"c, "<"c, ">"c, "|"c}

            For Each c In extraInvalid
                If p.Contains(c) Then
                    Return $"Path contains an invalid character: {c}"
                End If
            Next

            ' Disallow trailing spaces/dots in any segment (Windows rules)
            Dim segments = p.Split(New Char() {"\"c}, StringSplitOptions.None)
            For Each seg In segments
                If seg.Length = 0 Then Continue For ' allows leading \\ and repeated slashes like C:\\ (we'll catch some later)
                If seg.EndsWith(" ") OrElse seg.EndsWith(".") Then
                    Return "A path folder/file segment cannot end with a space or period."
                End If
            Next

            ' Basic rooted/relative rules
            Dim isUnc = p.StartsWith("\\")
            Dim isDriveRooted = (p.Length >= 3 AndAlso Char.IsLetter(p(0)) AndAlso p(1) = ":"c AndAlso (p(2) = "\"c OrElse p(2) = "/"c))
            Dim isDriveRelative = (p.Length >= 2 AndAlso Char.IsLetter(p(0)) AndAlso p(1) = ":"c AndAlso (p.Length = 2 OrElse (p(2) <> "\"c AndAlso p(2) <> "/"c)))

            If isDriveRelative Then
                Return "Drive-relative paths like 'C:folder' are ambiguous. Use 'C:\folder' instead."
            End If

            If Not allowRelative AndAlso Not isUnc AndAlso Not isDriveRooted AndAlso Not IO.Path.IsPathRooted(p) Then
                Return "Path must be absolute (e.g., C:\Folder or \\Server\Share)."
            End If

            ' UNC needs at least \\server\share
            If isUnc Then
                Dim uncParts = p.TrimStart("\"c).Split("\"c)
                If uncParts.Length < 2 OrElse String.IsNullOrWhiteSpace(uncParts(0)) OrElse String.IsNullOrWhiteSpace(uncParts(1)) Then
                    Return "UNC paths must look like \\Server\Share\Optional\Path."
                End If
            End If

            ' Validate each segment as a *file name* segment (no reserved device names, etc.)
            ' Skip the drive ("C:") and UNC server/share header parts
            For i = 0 To segments.Length - 1
                Dim seg = segments(i)
                If seg.Length = 0 Then
                    Continue For
                End If

                ' Skip server/share segments for UNC header validation already done
                If isUnc AndAlso i < 2 Then
                    Continue For
                End If

                ' Skip drive segment like "C:"
                If i = 0 AndAlso seg.EndsWith(":") Then
                    Continue For
                End If

                Dim baseName = IO.Path.GetFileNameWithoutExtension(seg).ToUpperInvariant()
                If RESERVED_FILE_NAMES.Contains(baseName) Then
                    Return $"Path contains a reserved name segment: {seg}"
                End If
            Next

            ' Finally: attempt to normalize; this catches some malformed cases.
            Try
                ' If rooted, GetFullPath should work even if it doesn't exist.
                If IO.Path.IsPathRooted(p) OrElse isUnc OrElse isDriveRooted Then
                    IO.Path.GetFullPath(p.Replace("/"c, "\"c))
                ElseIf allowRelative Then
                    ' For relative, still try to normalize by anchoring to current dir
                    IO.Path.GetFullPath(IO.Path.Combine(Environment.CurrentDirectory, p.Replace("/"c, "\"c)))
                End If
            Catch ex As Exception
                Return "Path is not valid: " & ex.Message
            End Try

            Return ""
        End Function

        Private Function CanToggleSequenceAtCaretAfterNumber(tb As TextBoxBase) As Boolean
            If tb Is Nothing Then
                Return False
            End If

            Dim text As String = tb.Text

            If String.IsNullOrEmpty(text) Then
                Return False
            End If

            Dim caret As Integer = tb.SelectionStart
            If caret <= 0 OrElse caret > text.Length Then
                Return False
            End If

            If Not Char.IsDigit(text(caret - 1)) Then
                Return False
            End If

            Dim i As Integer = caret - 1
            While i >= 0 AndAlso Char.IsDigit(text(i))
                i -= 1
            End While

            Dim len As Integer = (caret - 1) - i

            Return (len > 0 AndAlso len <= 4)
        End Function

        Private Function CanToggleSequenceAtCaretAfterToken(tb As TextBoxBase) As Boolean
            If tb Is Nothing Then
                Return False
            End If

            Dim text As String = tb.Text

            If String.IsNullOrEmpty(text) Then
                Return False
            End If

            Dim caret As Integer = tb.SelectionStart

            If caret <= 0 OrElse caret > text.Length Then
                Return False
            End If

            If text(caret - 1) <> ">"c Then
                Return False
            End If

            ' Walk left over digits and ensure a '<' is immediately before them
            Dim i As Integer = caret - 2
            If i < 0 OrElse Not Char.IsDigit(text(i)) Then
                Return False
            End If

            While i >= 0 AndAlso Char.IsDigit(text(i))
                i -= 1
            End While

            If i < 0 OrElse text(i) <> "<"c Then
                Return False
            End If

            Dim len As Integer = (caret - 1) - (i + 1)
            Return (len > 0 AndAlso len <= 4)
        End Function

        Private Function CanToggleSequenceAtSelection(tb As TextBoxBase) As Boolean
            If tb Is Nothing OrElse tb.SelectionLength <= 0 Then
                Return False
            End If

            Dim selText As String = tb.SelectedText
            If String.IsNullOrEmpty(selText) Then
                Return False
            End If

            For i As Integer = 0 To selText.Length - 1
                If Char.IsDigit(selText(i)) Then
                    Dim j As Integer = i
                    While j < selText.Length AndAlso Char.IsDigit(selText(j))
                        j += 1
                    End While

                    Dim len = j - i
                    Return (len > 0 AndAlso len <= 4)
                End If
            Next

            Return False
        End Function

        Private Sub ClearImageFormats(Combo As ComboBox)
            Dim list As New List(Of Object) From {
                My.Resources.Label_PleaseSelect
            }

            With Combo
                .DisplayMember = ""
                .ValueMember = Nothing
                .DataSource = list
                .DropDownStyle = ComboBoxStyle.DropDownList
            End With
        End Sub

        Private Function ContainsRawFileInFolder(folderPath As String) As Boolean
            Try
                ' Stop on first match
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

        Private Sub ParseKfInfoPayload(payload As Byte(), fields As Dictionary(Of String, String))
            ' payload is ASCII, often NUL-terminated
            Dim s As String = Text.Encoding.ASCII.GetString(payload).TrimEnd(ChrW(0))

            ' Split on ", "
            Dim parts = s.Split(New String() {", "}, StringSplitOptions.RemoveEmptyEntries)
            For Each part In parts
                Dim idx = part.IndexOf("="c)
                If idx > 0 Then
                    Dim key = part.Substring(0, idx).Trim()
                    Dim value = part.Substring(idx + 1).Trim()
                    If key.Length > 0 Then
                        fields(key) = value   ' last one wins if duplicate
                    End If
                End If
            Next
        End Sub

        Private Function ToggleNumberTokenAtSelection(tb As TextBoxBase) As Boolean
            If tb Is Nothing OrElse tb.SelectionLength <= 0 Then
                Return False
            End If

            Dim selStart As Integer = tb.SelectionStart
            Dim selText As String = tb.SelectedText

            If String.IsNullOrEmpty(selText) Then
                Return False
            End If

            ' Find first run of digits in selection
            Dim digitStartInSel As Integer = -1
            Dim digitLen As Integer = 0

            For i As Integer = 0 To selText.Length - 1
                If Char.IsDigit(selText(i)) Then
                    digitStartInSel = i
                    Dim j As Integer = i
                    While j < selText.Length AndAlso Char.IsDigit(selText(j))
                        j += 1
                    End While
                    digitLen = j - i
                    Exit For
                End If
            Next

            If digitStartInSel < 0 OrElse digitLen <= 0 Then
                Return False
            End If

            Dim absDigitStart As Integer = selStart + digitStartInSel
            Dim absDigitEnd As Integer = absDigitStart + digitLen

            ' Detect existing token in full text
            Dim hasLeft As Boolean = (absDigitStart > 0 AndAlso tb.Text(absDigitStart - 1) = "<"c)
            Dim hasRight As Boolean = (absDigitEnd < tb.TextLength AndAlso tb.Text(absDigitEnd) = ">"c)
            Dim alreadyTokenized As Boolean = hasLeft AndAlso hasRight

            tb.SuspendLayout()
            Try
                If alreadyTokenized Then
                    ' remove '>' then '<'
                    tb.Select(absDigitEnd, 1) : tb.SelectedText = ""
                    tb.Select(absDigitStart - 1, 1) : tb.SelectedText = ""

                    ' Select digits
                    tb.Select(absDigitStart - 1, digitLen)
                Else
                    ' add '>' then '<'
                    tb.Select(absDigitEnd, 0) : tb.SelectedText = ">"
                    tb.Select(absDigitStart, 0) : tb.SelectedText = "<"

                    ' Select <digits>
                    tb.Select(absDigitStart, digitLen + 2)
                End If
            Finally
                tb.ResumeLayout()
            End Try

            Return True
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

        Public Structure FluxSetInfo
            Public ReadOnly Headers As Dictionary(Of TrackSide, Dictionary(Of String, String))
            Public Sub New(Result As Boolean, TrackCount As Integer, SideCount As Integer)
                Me.Result = Result
                Me.TrackCount = TrackCount
                Me.SideCount = SideCount
                Me.Headers = New Dictionary(Of TrackSide, Dictionary(Of String, String))
            End Sub

            Public Property Result As Boolean
            Public Property SideCount As Integer
            Public Property TrackCount As Integer

            Public Sub AddTrackHeaders(Track As UShort, Side As Byte, Data As Dictionary(Of String, String))
                Dim ts As New TrackSide(Track, Side)
                Me.Headers(ts) = Data
            End Sub
        End Structure

        Public Structure TrackSide
            Public ReadOnly Side As Byte
            Public ReadOnly Track As UShort
            Public Sub New(Track As UShort, Side As Byte)
                Me.Track = Track
                Me.Side = Side
            End Sub
        End Structure

        Public Class DriveOption
            Private ReadOnly _Id As String
            Private ReadOnly _Tracks As Byte
            Private ReadOnly _Type As FloppyDriveType
            Private _DetectedFormat As FloppyDiskFormat? = Nothing
            Private _Label As String
            Private _SelectedFormat As FloppyDiskFormat? = Nothing

            Public Sub New()
                _Id = ""
                _Type = FloppyDriveType.DriveUnknown
                _Tracks = 0
            End Sub

            Public Sub New(Id As String, Type As FloppyDriveType, Tracks As Byte)
                _Id = Id
                _Type = Type
                _Tracks = Tracks
            End Sub

            Public Property DetectedFormat As FloppyDiskFormat?
                Get
                    Return _DetectedFormat
                End Get
                Set
                    _DetectedFormat = Value
                End Set
            End Property

            Public ReadOnly Property Id As String
                Get
                    Return _Id
                End Get
            End Property

            Public Property Label As String
                Get
                    Return _Label
                End Get
                Set
                    _Label = Value
                End Set
            End Property

            Public Property SelectedFormat As FloppyDiskFormat?
                Get
                    Return _SelectedFormat
                End Get
                Set
                    _SelectedFormat = Value
                End Set
            End Property

            Public ReadOnly Property Tracks As Byte
                Get
                    Return _Tracks
                End Get
            End Property

            Public ReadOnly Property Type As FloppyDriveType
                Get
                    Return _Type
                End Get
            End Property

            Public Sub ResetFormats()
                _SelectedFormat = Nothing
                _DetectedFormat = Nothing
            End Sub

            Public Overrides Function ToString() As String
                Return Label
            End Function
        End Class
    End Module
End Namespace
