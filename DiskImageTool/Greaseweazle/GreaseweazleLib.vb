Imports System.Text
Imports System.Text.RegularExpressions

Namespace Greaseweazle
    Module GreaseweazleLib
        Public Enum GreaseweazleFloppyType
            None
            F525_DD_360K
            F525_HD_12M
            F35_DD_720K
            F35_HD_144M
            F35_ED_288M
        End Enum

        Public Enum GreaseweazleImageFormat
            None
            IBM_160
            IBM_180
            IBM_320
            IBM_360
            IBM_1200
            IBM_720
            IBM_800
            IBM_1440
            IBM_2880
            IBM_DMF
        End Enum

        Public Enum GreaseweazleInterface
            IBM
            Shugart
        End Enum
        Public Enum GreaseweazleOutputType
            IMA
            IMG
            HFE
        End Enum

        Public Function ConvertFirstTrack(FilePath As String) As (Boolean, String)
            Dim AppPath As String = My.Settings.GW_Path
            Dim TempPath = InitTempImagePath()

            If TempPath = "" Then
                Return (False, "")
            End If

            Dim FileName = GenerateUniqueFileName(TempPath, "temp.ima")

            Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.convert) With {
            .InFile = FilePath,
            .OutFile = FileName,
            .Format = "ibm.scan",
            .Heads = CommandLineBuilder.TrackHeads.head0
        }
            Builder.AddCylinder(0)

            Dim psi As New ProcessStartInfo() With {
               .FileName = AppPath,
               .Arguments = Builder.Arguments,
               .RedirectStandardOutput = False,
               .RedirectStandardError = False,
               .UseShellExecute = False,
               .CreateNoWindow = True
           }

            Using proc As New Process()
                proc.StartInfo = psi
                proc.Start()
                proc.WaitForExit()
            End Using

            Return (IO.File.Exists(FileName), FileName)
        End Function

        Public Sub DisplayGreaseweazleBandwidth(ParentForm As Form)
            Dim AppPath As String = My.Settings.GW_Path

            If Not IsValidGreaseweazlePath(AppPath) Then
                DisplayInvalidApplicationPathMsg()
                Exit Sub
            End If

            ParentForm.Cursor = Cursors.WaitCursor
            Application.DoEvents()

            Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.bandwidth)
            Dim Arguments = Builder.Arguments

            Dim Content As String = ""
            Try
                Content = GetGreaseweazleOutput(AppPath, Arguments)
            Finally
                ParentForm.Cursor = Cursors.Default
            End Try

            Dim frmTextView = New TextViewForm("Greaseweazle - " & My.Resources.Label_Bandwidth, Content, False, True, "GreaseweazleBandwidth.txt")
            frmTextView.ShowDialog(ParentForm)
        End Sub

        Public Sub DisplayGreaseweazleInfo(ParentForm As Form)
            Dim AppPath As String = My.Settings.GW_Path

            If Not IsValidGreaseweazlePath(AppPath) Then
                DisplayInvalidApplicationPathMsg()
                Exit Sub
            End If

            ParentForm.Cursor = Cursors.WaitCursor
            Application.DoEvents()

            Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.info)
            Dim Arguments = Builder.Arguments

            Dim Content As String = ""
            Try
                Content = GetGreaseweazleOutput(AppPath, Arguments)
            Finally
                ParentForm.Cursor = Cursors.Default
            End Try

            Dim frmTextView = New TextViewForm("Greaseweazle - " & My.Resources.label_info, Content, False, True, "GreaseweazleInfo.txt")
            frmTextView.ShowDialog(ParentForm)
        End Sub

        Public Sub DisplayInvalidApplicationPathMsg()
            MessageBox.Show(My.Resources.Dialog_InvalidApplicationPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Function GetGreaseweazleFloppyTypeDescription(Value As GreaseweazleFloppyType) As String
            Select Case Value
                Case GreaseweazleFloppyType.F525_DD_360K
                    Return "5.25"" 360 KB (Double Density)"
                Case GreaseweazleFloppyType.F35_DD_720K
                    Return "3.5"" 720 KB (Double Density)"
                Case GreaseweazleFloppyType.F525_HD_12M
                    Return "5.25"" 1.2 MB (High Density)"
                Case GreaseweazleFloppyType.F35_HD_144M
                    Return "3.5"" 1.44 MB (High Density)"
                Case GreaseweazleFloppyType.F35_ED_288M
                    Return "3.5"" 2.88 MB (Extra Density)"
                Case Else
                    Return "None"
            End Select
        End Function

        Public Function GetGreaseweazleFloppyTypeName(Value As GreaseweazleFloppyType) As String
            Select Case Value
                Case GreaseweazleFloppyType.F525_DD_360K
                    Return "360"
                Case GreaseweazleFloppyType.F35_DD_720K
                    Return "720"
                Case GreaseweazleFloppyType.F525_HD_12M
                    Return "1200"
                Case GreaseweazleFloppyType.F35_HD_144M
                    Return "1440"
                Case GreaseweazleFloppyType.F35_ED_288M
                    Return "2880"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GetGreaseweazleFoppyTypeFromName(Value As String) As GreaseweazleFloppyType
            Select Case Value
                Case "360"
                    Return GreaseweazleFloppyType.F525_DD_360K
                Case "720"
                    Return GreaseweazleFloppyType.F35_DD_720K
                Case "1200"
                    Return GreaseweazleFloppyType.F525_HD_12M
                Case "1440"
                    Return GreaseweazleFloppyType.F35_HD_144M
                Case "2880"
                    Return GreaseweazleFloppyType.F35_ED_288M
                Case Else
                    Return GreaseweazleFloppyType.None
            End Select
        End Function

        Public Function GetGreaseweazleInterfaceName(Value As GreaseweazleInterface) As String
            Select Case Value
                Case GreaseweazleInterface.Shugart
                    Return "Shugart"
                Case Else
                    Return "IBM"
            End Select
        End Function

        Public Function GetGreaseweazleInterfacFromName(Value As String) As GreaseweazleInterface
            Select Case Value
                Case "Shugart"
                    Return GreaseweazleInterface.Shugart
                Case Else
                    Return GreaseweazleInterface.IBM
            End Select
        End Function

        Public Function GetGreaseweazleOutput(AppPath As String, Arguments As String) As String
            Dim psi As New ProcessStartInfo() With {
                .FileName = AppPath,
                .Arguments = Arguments,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .UseShellExecute = False,
                .CreateNoWindow = True
            }

            Dim outputBuilder As New StringBuilder()

            Using proc As New Process()
                proc.StartInfo = psi
                AddHandler proc.OutputDataReceived, Sub(sender, e)
                                                        If e.Data IsNot Nothing Then
                                                            SyncLock outputBuilder
                                                                outputBuilder.AppendLine(e.Data)
                                                            End SyncLock
                                                        End If
                                                    End Sub
                AddHandler proc.ErrorDataReceived, Sub(sender, e)
                                                       If e.Data IsNot Nothing Then
                                                           SyncLock outputBuilder
                                                               outputBuilder.AppendLine(e.Data)
                                                           End SyncLock
                                                       End If
                                                   End Sub

                proc.Start()
                proc.BeginOutputReadLine()
                proc.BeginErrorReadLine()
                proc.WaitForExit()
            End Using

            Return outputBuilder.ToString
        End Function

        Public Function GetFirstRawFile(FilePath As String) As String
            Dim RawFileName As String = ""

            For Each file In IO.Directory.EnumerateFiles(FilePath, "*.raw", IO.SearchOption.TopDirectoryOnly)
                Dim name = IO.Path.GetFileNameWithoutExtension(file)
                Dim PrefixMatch = Regex.Match(name, "^(.*?)(\d+)\.\d+$")
                If PrefixMatch.Success Then
                    RawFileName = file
                    Exit For
                End If
            Next

            Return RawFileName
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

            Dim BaseName = IO.Path.GetFileNameWithoutExtension(FilePath)

            Dim PrefixMatch = Regex.Match(BaseName, "^(.*?)(\d+)\.\d+$")
            If Not PrefixMatch.Success Then
                Return (False, TrackCount, SideCount)
            End If

            Dim Prefix As String = PrefixMatch.Groups(1).Value
            Dim rxPattern As String = "^" & Regex.Escape(Prefix) & "(?<trk>\d+)\.(?<side>\d+)\.raw$"
            Dim rx As New Regex(rxPattern, RegexOptions.IgnoreCase)

            For Each file In IO.Directory.EnumerateFiles(ParentDir, Prefix & "*.raw", IO.SearchOption.TopDirectoryOnly)
                Dim name = IO.Path.GetFileName(file)
                Dim m = rx.Match(name)
                If m.Success Then
                    Dim trk As Integer = Integer.Parse(m.Groups("trk").Value)
                    Dim side As Integer = Integer.Parse(m.Groups("side").Value)

                    If trk >= TrackCount Then
                        TrackCount = trk + 1
                    End If

                    If side >= SideCount Then
                        SideCount = side + 1
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

        Public Function GreaseweazleImageFormatRPM(Value As GreaseweazleImageFormat) As Integer
            Select Case Value
                Case GreaseweazleImageFormat.IBM_1200
                    Return 360
                Case Else
                    Return 300
            End Select
        End Function

        Public Function GreaseweazleImageFormatBitrate(Value As GreaseweazleImageFormat) As Integer
            Select Case Value
                Case GreaseweazleImageFormat.IBM_1200, GreaseweazleImageFormat.IBM_1440, GreaseweazleImageFormat.IBM_DMF
                    Return 500
                Case GreaseweazleImageFormat.IBM_2880
                    Return 1000
                Case Else
                    Return 250
            End Select
        End Function

        Public Function GreaseweazleImageFormatCommandLine(Value As GreaseweazleImageFormat) As String
            Select Case Value
                Case GreaseweazleImageFormat.None
                    Return "ibm.scan"
                Case GreaseweazleImageFormat.IBM_160
                    Return "ibm.160"
                Case GreaseweazleImageFormat.IBM_180
                    Return "ibm.180"
                Case GreaseweazleImageFormat.IBM_320
                    Return "ibm.320"
                Case GreaseweazleImageFormat.IBM_360
                    Return "ibm.360"
                Case GreaseweazleImageFormat.IBM_1200
                    Return "ibm.1200"
                Case GreaseweazleImageFormat.IBM_720
                    Return "ibm.720"
                Case GreaseweazleImageFormat.IBM_800
                    Return "ibm.800"
                Case GreaseweazleImageFormat.IBM_1440
                    Return "ibm.1440"
                Case GreaseweazleImageFormat.IBM_2880
                    Return "ibm.2880"
                Case GreaseweazleImageFormat.IBM_DMF
                    Return "ibm.dmf"
                Case Else
                    Return "ibm.scan"
            End Select
        End Function

        Public Function GreaseweazleImageFormatDescription(Value As GreaseweazleImageFormat) As String
            Select Case Value
                Case GreaseweazleImageFormat.None
                    Return "Please Select"
                Case GreaseweazleImageFormat.IBM_160
                    Return "5.25"" 160 KB (SS, DD)"
                Case GreaseweazleImageFormat.IBM_180
                    Return "5.25"" 180 KB (SS, DD)"
                Case GreaseweazleImageFormat.IBM_320
                    Return "5.25"" 320 KB (DS, DD)"
                Case GreaseweazleImageFormat.IBM_360
                    Return "5.25"" 360 KB (DS, DD)"
                Case GreaseweazleImageFormat.IBM_1200
                    Return "5.25"" 1.2 MB (DS, HD)"
                Case GreaseweazleImageFormat.IBM_720
                    Return "3.5"" 720 KB (DS, DD)"
                Case GreaseweazleImageFormat.IBM_800
                    Return "3.5"" 800 KB (DS, DD)"
                Case GreaseweazleImageFormat.IBM_1440
                    Return "3.5"" 1.44 MB (DS, HD)"
                Case GreaseweazleImageFormat.IBM_2880
                    Return "3.5"" 2.88 MB (DS, ED)"
                Case GreaseweazleImageFormat.IBM_DMF
                    Return "3.5"" 1.68 MB (DS, HD, DMF)"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GreaseweazleImageFormatFromFloppyDiskFormat(Format As DiskImage.FloppyDiskFormat) As GreaseweazleImageFormat
            Select Case Format
                Case DiskImage.FloppyDiskFormat.Floppy160
                    Return GreaseweazleImageFormat.IBM_160
                Case DiskImage.FloppyDiskFormat.Floppy180
                    Return GreaseweazleImageFormat.IBM_180
                Case DiskImage.FloppyDiskFormat.Floppy320
                    Return GreaseweazleImageFormat.IBM_320
                Case DiskImage.FloppyDiskFormat.Floppy360
                    Return GreaseweazleImageFormat.IBM_360
                Case DiskImage.FloppyDiskFormat.Floppy720
                    Return GreaseweazleImageFormat.IBM_720
                Case DiskImage.FloppyDiskFormat.Floppy1200
                    Return GreaseweazleImageFormat.IBM_1200
                Case DiskImage.FloppyDiskFormat.Floppy1440
                    Return GreaseweazleImageFormat.IBM_1440
                Case DiskImage.FloppyDiskFormat.Floppy2880
                    Return GreaseweazleImageFormat.IBM_2880
                Case DiskImage.FloppyDiskFormat.FloppyDMF1024, DiskImage.FloppyDiskFormat.FloppyDMF2048
                    Return GreaseweazleImageFormat.IBM_DMF
                Case Else
                    Return GreaseweazleImageFormat.None
            End Select
        End Function

        Public Function GreaseweazleImageFormatSideCount(Value As GreaseweazleImageFormat) As Integer
            Select Case Value
                Case GreaseweazleImageFormat.IBM_160, GreaseweazleImageFormat.IBM_180
                    Return 1
                Case Else
                    Return 2
            End Select
        End Function

        Public Function GreaseweazleImageFormatTrackCount(Value As GreaseweazleImageFormat) As Integer
            Select Case Value
                Case GreaseweazleImageFormat.IBM_160, GreaseweazleImageFormat.IBM_180, GreaseweazleImageFormat.IBM_320, GreaseweazleImageFormat.IBM_360
                    Return 40
                Case Else
                    Return 80
            End Select
        End Function

        Public Function GreaseweazleOutputTypeDescription(Value As GreaseweazleOutputType) As String
            Select Case Value
                Case GreaseweazleOutputType.HFE
                    Return "HxC HFE Image (.hfe)"
                Case GreaseweazleOutputType.IMA
                    Return "Basic Sector Image (.ima)"
                Case GreaseweazleOutputType.IMG
                    Return "Basic Sector Image (.img)"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GreaseweazleOutputTypeFileExt(Value As GreaseweazleOutputType) As String
            Select Case Value
                Case GreaseweazleOutputType.HFE
                    Return ".hfe"
                Case GreaseweazleOutputType.IMA
                    Return ".ima"
                Case GreaseweazleOutputType.IMG
                    Return ".img"
                Case Else
                    Return ".ima"
            End Select
        End Function

        Public Function ImportFluxImage(FilePath As String, ParentForm As Form) As String
            Dim FileExt = IO.Path.GetExtension(FilePath).ToLower
            Dim TrackCount As Integer = 0
            Dim SideCount As Integer = 0

            If FileExt = ".raw" Then
                Dim Response = GetTrackCountRaw(FilePath)
                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_InvalidKryofluxFile, MsgBoxStyle.Exclamation)
                    Return ""
                Else
                    TrackCount = Response.Tracks
                    SideCount = Response.Sides
                End If
            ElseIf FileExt = ".scp" Then
                Dim Response = GetTrackCountSCP(FilePath)
                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_InvalidSCPFile, MsgBoxStyle.Exclamation)
                Else
                    TrackCount = Response.Tracks
                    SideCount = Response.Sides
                End If
            Else
                MsgBox(My.Resources.Dialog_InvalidFileType, MsgBoxStyle.Exclamation)
                Return ""
            End If

            If SideCount > 2 Then
                SideCount = 2
            End If

            If TrackCount > 42 And TrackCount < 80 Then
                TrackCount = 80
            End If

            Dim Form As New ImageImportForm(FilePath, TrackCount, SideCount)
            If Form.ShowDialog(ParentForm) = DialogResult.OK Then
                If Not String.IsNullOrEmpty(Form.OutputFilePath) Then
                    Return Form.OutputFilePath
                End If
            End If

            Return ""
        End Function

        Public Function IsExecutable(path As String) As Boolean
            Return Not String.IsNullOrWhiteSpace(path) AndAlso
           IO.File.Exists(path) AndAlso
           IO.Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase)
        End Function

        Public Function IsValidGreaseweazlePath(Path As String) As Boolean
            Dim IsValid As Boolean

            If My.Settings.GW_Path = "" Then
                IsValid = False
            ElseIf Not IsExecutable(My.Settings.GW_Path) Then
                IsValid = False
            ElseIf Not IO.File.Exists(My.Settings.GW_Path) Then
                IsValid = False
            Else
                IsValid = True
            End If

            Return IsValid
        End Function

        Public Function OpenFluxImage(ParentForm As Form) As String
            Using Dialog As New OpenFileDialog With {
                .Title = "Open Flux Image",
                .Filter = "Flux dumps (*.raw;*.scp)|*.raw;*.scp|" &
                    "KryoFlux RAW (*.raw)|*.raw|" &
                    "SuperCard Pro (*.scp)|*.scp",
                .FilterIndex = 1,
                .CheckFileExists = True,
                .AddExtension = True,
                .Multiselect = False
            }

                If Dialog.ShowDialog(ParentForm) = DialogResult.OK Then
                    Return Dialog.FileName
                End If
            End Using

            Return Nothing
        End Function
    End Module
End Namespace