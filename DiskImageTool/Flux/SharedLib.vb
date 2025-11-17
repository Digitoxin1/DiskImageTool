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
