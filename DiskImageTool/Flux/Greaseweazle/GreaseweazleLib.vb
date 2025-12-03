Imports System.Text.RegularExpressions
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Module GreaseweazleLib

        Public Function Settings() As GreaseweazleSettings
            Return App.Globals.AppSettings.Greaseweazle
        End Function

        Public Function GenerateCommandLineImport(InputFilePath As String, OutputFilePath As String, DiskParams As FloppyDiskParams, OutputType As ImageImportOutputTypes, DoubleStep As Boolean) As String
            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.convert) With {
                .InFile = InputFilePath,
                .OutFile = OutputFilePath
            }

            If Not DiskParams.IsStandard Then
                OutputType = ImageImportOutputTypes.HFE
            End If

            If OutputType <> ImageImportOutputTypes.HFE Then
                Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(DiskParams.Format)
                Builder.Format = GreaseweazleImageFormatCommandLine(ImageFormat)
            Else
                Builder.BitRate = DiskParams.BitRateKbps
                Builder.AdjustSpeed = DiskParams.RPM & "rpm"
            End If

            If DoubleStep Then
                Builder.HeadStep = 2
            End If

            Return Builder.Arguments
        End Function

        Public Function GenerateCommandLineRead(FilePath As String,
                                                Opt As DriveOption,
                                                DiskParams As FloppyDiskParams,
                                                OutputType As ReadDiskOutputTypes,
                                                DoubleStep As Boolean,
                                                Retries As UInteger,
                                                SeekRetries As UInteger,
                                                Revs As UInteger,
                                                Optional TrackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)) = Nothing,
                                                Optional Heads As TrackHeads? = Nothing) As String

            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.read) With {
                .Drive = Opt.Id,
                .File = FilePath,
                .Retries = Retries,
                .SeekRetries = SeekRetries,
                .Revs = Revs
            }

            Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(DiskParams.Format)

            If OutputType <> ReadDiskOutputTypes.HFE OrElse ImageFormat <> GreaseweazleImageFormat.None Then
                Builder.Format = GreaseweazleImageFormatCommandLine(ImageFormat)
            End If

            If OutputType = ReadDiskOutputTypes.HFE Then
                Builder.BitRate = DiskParams.BitRateKbps
                Builder.AdjustSpeed = DiskParams.RPM & "rpm"
                Builder.Raw = True

            ElseIf OutputType = ReadDiskOutputTypes.RAW Then
                Builder.AdjustSpeed = DiskParams.RPM & "rpm"
                Builder.Raw = True

                If TrackRanges Is Nothing Then
                    Builder.AddCylinder(0, Opt.Tracks - 1)
                Else
                    For Each Range In TrackRanges
                        Builder.AddCylinder(Range.StartTrack, Range.EndTrack)
                    Next
                End If

                If Heads.HasValue Then
                    Builder.Heads = Heads.Value
                Else
                    Builder.Heads = If(DiskParams.BPBParams.NumberOfHeads > 1, TrackHeads.both, TrackHeads.head0)
                End If
            End If

            If DoubleStep Then
                Builder.HeadStep = 2
            End If

            Return Builder.Arguments
        End Function

        Public Function GetTrackHeads(StartHead As Integer, Optional EndHead As Integer = -1) As TrackHeads
            If EndHead = -1 Then
                EndHead = StartHead
            End If

            If StartHead = 0 And EndHead = 0 Then
                Return TrackHeads.head0
            ElseIf StartHead = 1 And EndHead = 1 Then
                Return TrackHeads.head1
            Else
                Return TrackHeads.both
            End If
        End Function

        Public Sub BandwidthDisplay(ParentForm As Form)
            If Not Settings.IsPathValid Then
                DisplayInvalidApplicationPathMsg()
                Exit Sub
            End If

            ParentForm.Cursor = Cursors.WaitCursor
            Application.DoEvents()

            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.bandwidth) With {
            .Device = Settings.ComPort
        }

            Dim Arguments = Builder.Arguments

            Dim Content As String = ""
            Try
                Dim Result = ConsoleProcessRunner.RunProcess(Settings.AppPath, Arguments)
                Content = Result.CombinedOutput
            Finally
                ParentForm.Cursor = Cursors.Default
            End Try

            TextViewForm.Display("Greaseweazle - " & My.Resources.Label_Bandwidth, Content, False, True, "GreaseweazleBandwidth.txt", ParentForm)
        End Sub

        Public Function BuildRanges(values As HashSet(Of UShort)) As List(Of (StartTrack As UShort, EndTrack As UShort))
            Dim result As New List(Of (UShort, UShort))()

            If values Is Nothing OrElse values.Count = 0 Then
                Return result
            End If

            ' Sort the values first
            Dim sorted = values.OrderBy(Function(v) v).ToList()

            Dim rangeStart As UShort = sorted(0)
            Dim rangeEnd As UShort = sorted(0)

            For i As Integer = 1 To sorted.Count - 1
                Dim v = sorted(i)

                If v = rangeEnd + 1US Then
                    ' extend range
                    rangeEnd = v
                Else
                    ' push previous range
                    result.Add((rangeStart, rangeEnd))
                    ' start new range
                    rangeStart = v
                    rangeEnd = v
                End If
            Next

            ' add final range
            result.Add((rangeStart, rangeEnd))

            Return result
        End Function

        Public Function ConvertFirstTrack(FilePath As String) As (Result As Boolean, FileName As String)
            Dim TempPath = InitTempImagePath()

            If TempPath = "" Then
                Return (False, "")
            End If

            Dim FileName = GenerateUniqueFileName(TempPath, "temp.ima")

            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.convert) With {
            .InFile = FilePath,
            .OutFile = FileName,
            .Format = "ibm.scan",
            .Heads = TrackHeads.head0
        }
            Builder.AddCylinder(0)

            ConsoleProcessRunner.RunProcess(Settings.AppPath, Builder.Arguments)

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

        Public Sub InfoDisplay(ParentForm As Form)
            If Not Settings.IsPathValid Then
                DisplayInvalidApplicationPathMsg()
                Exit Sub
            End If

            ParentForm.Cursor = Cursors.WaitCursor
            Application.DoEvents()

            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.info) With {
            .Device = Settings.ComPort
        }
            Dim Arguments = Builder.Arguments

            Dim Content As String = ""
            Try
                Dim Result = ConsoleProcessRunner.RunProcess(Settings.AppPath, Arguments)
                Content = Result.CombinedOutput
            Finally
                ParentForm.Cursor = Cursors.Default
            End Try

            TextViewForm.Display("Greaseweazle - " & My.Resources.Label_Info, Content, False, True, "GreaseweazleInfo.txt", ParentForm)
        End Sub

        Public Sub PopulateDrives(Combo As ComboBox, Format As FloppyMediaType, Optional LastUsedDrive As String = "")
            Dim DriveList As New List(Of DriveOption)

            Dim SelectedOption As DriveOption = Nothing
            Dim CachedOption As DriveOption = Nothing

            Dim AddItem As Action(Of String, String, Byte) =
            Sub(labelPrefix As String, id As String, index As Byte)
                Dim t = Settings.Drives(index).Type
                If t = FloppyMediaType.MediaUnknown Then
                    Exit Sub
                End If

                Dim opt As New DriveOption With {
                    .Id = id,
                    .Type = t,
                    .Tracks = Settings.Drives(index).Tracks,
                    .Label = $"{labelPrefix}:   {GreaseweazleFloppyTypeDescription(t)}"
                }
                DriveList.Add(opt)

                If SelectedOption Is Nothing AndAlso t = Format Then
                    SelectedOption = opt
                End If
                If CachedOption Is Nothing AndAlso Not String.IsNullOrEmpty(LastUsedDrive) AndAlso id = LastUsedDrive Then
                    CachedOption = opt
                End If
            End Sub

            If Settings.Interface = GreaseweazleSettings.GreaseweazleInterface.Shugart Then
                AddItem("DS0", "0", 0)
                AddItem("DS1", "1", 1)
                AddItem("DS2", "2", 2)
            Else
                AddItem("A", "A", 0)
                AddItem("B", "B", 1)
            End If

            Dim placeholder As DriveOption = Nothing
            If DriveList.Count <> 1 Then
                placeholder = New DriveOption With {
                .Id = "",
                .Type = FloppyMediaType.MediaUnknown,
                .Tracks = 0,
                .Label = If(DriveList.Count = 0, My.Resources.Label_NoDrivesFound, My.Resources.Label_PleaseSelect)
            }
                DriveList.Insert(0, placeholder)
            End If

            With Combo
                .DropDownStyle = ComboBoxStyle.DropDownList
                .DataSource = DriveList
                .DisplayMember = NameOf(DriveOption.Label)
                .ValueMember = ""
                If SelectedOption IsNot Nothing Then
                    .SelectedItem = SelectedOption
                ElseIf CachedOption IsNot Nothing Then
                    .SelectedItem = CachedOption
                Else
                    .SelectedItem = placeholder
                End If
                If .SelectedIndex = -1 Then
                    .SelectedIndex = 0
                End If
            End With
        End Sub

        Public Function ReadFirstTrack(DriveId As String) As (Result As Boolean, FileName As String, Output As String)
            Dim TempPath = InitTempImagePath()

            If TempPath = "" Then
                Return (False, "", "")
            End If

            Dim FileName = GenerateUniqueFileName(TempPath, "temp.ima")

            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.read) With {
            .Device = Settings.ComPort,
            .Drive = DriveId,
            .File = FileName,
            .Format = "ibm.scan",
            .Heads = TrackHeads.head0
        }
            Builder.AddCylinder(0)

            Dim Result = ConsoleProcessRunner.RunProcess(Settings.AppPath, Builder.Arguments)

            Return (IO.File.Exists(FileName), FileName, Result.CombinedOutput)
        End Function

        Public Function ReadFluxImage(ParentForm As Form, importHandler As ReadDiskForm.ImportProcessEventHandler) As (Result As Boolean, OutputFile As String, NewFileName As String)
            Using Form As New ReadDiskForm()

                If importHandler IsNot Nothing Then
                    AddHandler Form.ImportProcess, importHandler
                End If

                Dim result As DialogResult = DialogResult.Cancel
                Try
                    result = Form.ShowDialog(ParentForm)
                Finally
                    If importHandler IsNot Nothing Then
                        RemoveHandler Form.ImportProcess, importHandler
                    End If
                End Try

                If result = DialogResult.OK Then
                    If Not String.IsNullOrEmpty(Form.NewFilePath) Then
                        Return (True, Form.NewFilePath, Form.NewFileName)
                    End If
                End If

                Return (False, "", "")
            End Using
        End Function

        Public Sub Reset(TextBox As TextBox)
            Dim Builder As New CommandLineBuilder(CommandLineBuilder.CommandAction.reset) With {
            .Device = Settings.ComPort
        }

            Dim Arguments = Builder.Arguments
            Dim Result = ConsoleProcessRunner.RunProcess(Settings.AppPath, Arguments)
            TextBox.Text = Result.CombinedOutput

            MsgBox(My.Resources.Dialog_GreaseweazleReset, MsgBoxStyle.Information)
        End Sub
    End Module
End Namespace