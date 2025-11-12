Imports System.Text.RegularExpressions

Namespace Greaseweazle
    Module GreaseweazleLib
        Private Const REGEX_RAW_FILE As String = "^(?<diskId>.+?)\.?(?<track>\d{2})\.(?<side>\d)\.(?<ext>raw|stream)$"

        Public Sub BandwidthDisplay(ParentForm As Form)
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
                Dim Result = ConsoleProcessRunner.RunProcess(AppPath, Arguments)
                Content = Result.CombinedOutput
            Finally
                ParentForm.Cursor = Cursors.Default
            End Try

            Dim frmTextView = New TextViewForm("Greaseweazle - " & My.Resources.Label_Bandwidth, Content, False, True, "GreaseweazleBandwidth.txt")
            frmTextView.ShowDialog(ParentForm)
        End Sub

        Public Function ConvertFirstTrack(FilePath As String) As (Result As Boolean, FileName As String)
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

            ConsoleProcessRunner.RunProcess(AppPath, Builder.Arguments, captureOutput:=False, captureError:=False)

            Return (IO.File.Exists(FileName), FileName)
        End Function

        Public Sub DisplayInvalidApplicationPathMsg()
            MessageBox.Show(My.Resources.Dialog_InvalidApplicationPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Function GetFirstRawFile(FilePath As String) As String
            Dim RawFileName As String = ""

            For Each file In IO.Directory.EnumerateFiles(FilePath, "*.raw", IO.SearchOption.TopDirectoryOnly)
                Dim name = IO.Path.GetFileName(file)
                Dim PrefixMatch = Regex.Match(name, REGEX_RAW_FILE, RegexOptions.IgnoreCase)
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

        Public Sub WriteImageToDisk(ParentForm As Form, Image As DiskImageContainer)
            Dim Form As New WriteDiskForm(Image)
            Form.ShowDialog(ParentForm)
        End Sub

        Public Function ImportFluxImage(FilePath As String, ParentForm As Form) As (Result As Boolean, OutputFile As String, NewFileName As String)
            Dim FileExt = IO.Path.GetExtension(FilePath).ToLower
            Dim TrackCount As Integer = 0
            Dim SideCount As Integer = 0

            If FileExt = ".raw" Then
                Dim Response = GetTrackCountRaw(FilePath)
                If Not Response.Result Then
                    MsgBox(My.Resources.Dialog_InvalidKryofluxFile, MsgBoxStyle.Exclamation)
                    Return (False, "", "")
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
                Return (False, "", "")
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
                    Return (True, Form.OutputFilePath, Form.GetNewFileName)
                End If
            End If

            Return (False, "", "")
        End Function

        Public Sub InfoDisplay(ParentForm As Form)
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
                Dim Result = ConsoleProcessRunner.RunProcess(AppPath, Arguments)
                Content = Result.CombinedOutput
            Finally
                ParentForm.Cursor = Cursors.Default
            End Try

            Dim frmTextView = New TextViewForm("Greaseweazle - " & My.Resources.Label_Info, Content, False, True, "GreaseweazleInfo.txt")
            frmTextView.ShowDialog(ParentForm)
        End Sub

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