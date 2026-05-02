Imports System.Text
Imports System.Text.RegularExpressions
Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports Greaseweazle.Actions
Imports Greaseweazle.Infrastructure
Imports Greaseweazle.Shared
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Module GreaseweazleLib
        Public Const DEFAULT_CYLS As UInteger = 80
        Public Const DEFAULT_LINGER As UInteger = 100
        Public Const DEFAULT_PASSES As UInteger = 3
        Public Const DEFAULT_RETRIES As UInteger = 3
        Public Const DEFAULT_REVS As UInteger = 1
        Public Const DEFAULT_SEEK_RETRIES As UInteger = 0
        Public Const MAX_CYLS As UInteger = 80
        Public Const MAX_LINGER As UInteger = 1000
        Public Const MAX_PASSES As UInteger = 9
        Public Const MAX_RETRIES As UInteger = 99
        Public Const MAX_REVS As Byte = 20
        Public Const MIN_CYLS As UInteger = 1
        Public Const MIN_LINGER As UInteger = 1
        Public Const MIN_PASSES As UInteger = 1
        Public Const MIN_RETRIES As UInteger = 0
        Public Const MIN_REVS As Byte = 1

        Public Sub BandwidthDisplay(ParentForm As Form)
            ParentForm.Cursor = Cursors.WaitCursor
            Application.DoEvents()

            Dim Content As String = ""
            Try
                Dim LocalEngine As New GreaseweazleEngine()
                Dim Result = LocalEngine.Bandwidth.Run(New BandwidthOptions With {
                    .Live = True,
                    .Device = ConfiguredDevice()
                })
                Content = GreaseweazleFormatters.FormatBandwidth(Result)
            Catch ex As Exception
                Content = "Command Failed: " & ex.Message
            Finally
                ParentForm.Cursor = Cursors.Default
            End Try

            TextViewForm.Display("Greaseweazle - " & My.Resources.Label_Bandwidth, Content, False, True, "GreaseweazleBandwidth.txt")
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

        Public Function BuildUserSpec(ranges As List(Of (StartTrack As UShort, EndTrack As UShort)), heads As TrackHeads, Optional doubleStep As Boolean = False, Optional divide As Boolean = False) As TrackSetSpec
            Dim spec As New TrackSetSpec()

            If ranges IsNot Nothing Then
                Dim seen As New HashSet(Of Integer)()
                For Each R In ranges
                    Dim StartTrack = R.StartTrack
                    Dim EndTrack = R.EndTrack
                    If doubleStep And divide Then
                        StartTrack \= 2
                        EndTrack \= 2
                    End If
                    For c As Integer = StartTrack To EndTrack
                        If seen.Add(c) Then
                            spec.Cyls.Add(c)
                        End If
                    Next
                Next
                spec.Cyls.Sort()
            End If

            Select Case heads
                Case TrackHeads.head0
                    spec.Heads.Add(0)
                Case TrackHeads.head1
                    spec.Heads.Add(1)
                Case Else
                    spec.Heads.Add(0)
                    spec.Heads.Add(1)
            End Select

            If doubleStep Then
                spec.Step = 2
            End If

            Return spec
        End Function

        ' Resolves the configured COM port for DLL options, mapping an empty user
        ' setting to Nothing so the engine performs auto-discovery (matches the CLI
        ' behaviour where an empty --device is omitted entirely).
        Public Function ConfiguredDevice() As String
            Dim Port = Settings.ComPort
            Return If(String.IsNullOrEmpty(Port), Nothing, Port)
        End Function

        Public Function ConvertFirstTrack(FilePath As String, BothSides As Boolean, Optional ImageParams As FloppyDiskParams? = Nothing) As (Result As Boolean, FileName As String)
            Dim TempPath = InitTempImagePath()

            If TempPath = "" Then
                Return (False, "")
            End If

            Dim FileName = GenerateUniqueFileName(TempPath, "temp.ima")

            Dim Format As String = "ibm.scan"

            If ImageParams.HasValue Then
                Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(ImageParams.Value.Format)
                Format = GreaseweazleImageFormatString(ImageFormat)
            End If

            Dim spec As New TrackSetSpec
            spec.Cyls.Add(0)
            spec.Heads.Add(0)
            If BothSides Then
                spec.Heads.Add(1)
            End If

            Dim Opts As New ConvertOptions With {
                .InputFile = FilePath,
                .OutputFile = FileName,
                .Format = Format,
                .TrackSet = spec,
                .OutTrackSet = spec
            }

            Try
                Dim LocalEngine As New GreaseweazleEngine()
                LocalEngine.Convert.Run(Opts)
            Catch ex As Exception
            End Try

            Return (IO.File.Exists(FileName), FileName)
        End Function

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

        Public Function GetInfo() As String
            Dim Content As String

            Try
                Dim LocalEngine As New GreaseweazleEngine()
                Dim Result = LocalEngine.Info.Run(New InfoOptions With {
                    .Live = True,
                    .Device = ConfiguredDevice()
                })
                Content = FormatInfo(Result)
            Catch ex As Exception
                Content = "Command Failed: " & ex.Message
            End Try

            Return Content
        End Function

        Public Sub InfoDisplay(ParentForm As Form)
            ParentForm.Cursor = Cursors.WaitCursor
            Application.DoEvents()

            Dim Content = GetInfo()

            ParentForm.Cursor = Cursors.Default

            TextViewForm.Display("Greaseweazle - " & My.Resources.Label_Info, Content, False, True, "GreaseweazleInfo.txt")
        End Sub

        ' Maps a UI-side drive id ("A"/"B"/"0".."3") to a Greaseweazle DriveSpec.
        ' Mirrors the CLI token table from the API reference: A/B -> IBMPC, 0..3 -> Shugart.
        Public Function MakeDriveSpec(id As String) As DriveSpec
            Select Case id
                Case "A"
                    Return New DriveSpec With {
                        .Bus = UsbProtocol.BusType.IBMPC,
                        .UnitId = 0
                    }
                Case "B"
                    Return New DriveSpec With {
                        .Bus = UsbProtocol.BusType.IBMPC,
                        .UnitId = 1
                    }
                Case "0", "1", "2", "3"
                    Return New DriveSpec With {
                        .Bus = UsbProtocol.BusType.Shugart,
                        .UnitId = Integer.Parse(id)
                    }
                Case Else
                    Throw New ArgumentException("Unrecognised drive id: " & id)
            End Select
        End Function

        Public Function PopulateDrives(Combo As ComboBox, Format As FloppyDriveType, Optional LastUsedDrive As String = "") As DriveOption
            Dim DriveList As New List(Of DriveOption)

            Dim SelectedOption As DriveOption = Nothing
            Dim CachedOption As DriveOption = Nothing

            Dim AddItem As Action(Of String, String, Byte) =
            Sub(labelPrefix As String, id As String, index As Byte)
                Dim t = Settings.Drives(index).Type
                If t = FloppyDriveType.DriveUnknown Then
                    Exit Sub
                End If

                Dim opt As New DriveOption(id, t, Settings.Drives(index).Tracks) With {
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
                placeholder = New DriveOption() With {
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

            Return Combo.SelectedItem
        End Function

        Public Function ReadFirstTrack(DriveId As String, BothSides As Boolean, Optional ImageParams As FloppyDiskParams? = Nothing) As (Result As Boolean, FileName As String, Output As String)
            Dim TempPath = InitTempImagePath()

            If TempPath = "" Then
                Return (False, "", "")
            End If

            Dim FileName = GenerateUniqueFileName(TempPath, "temp.ima")

            Dim Format As String = "ibm.scan"
            If ImageParams.HasValue Then
                Dim ImageFormat = GreaseweazleImageFormatFromFloppyDiskFormat(ImageParams.Value.Format)
                Format = GreaseweazleImageFormatString(ImageFormat)
            End If

            Dim Sb As New StringBuilder()

            Dim Drive As DriveSpec
            Try
                Drive = MakeDriveSpec(DriveId)
            Catch ex As Exception
                Sb.AppendLine("Command Failed: " & ex.Message)
                Return (False, FileName, Sb.ToString())
            End Try

            Dim LocalEngine As New GreaseweazleEngine()
            Dim Cmd = LocalEngine.Read

            Dim StartedHandler As EventHandler(Of ReadStartedEventArgs) =
                Sub(snd, args)
                    For Each Line In FormatReadStartedLines(args)
                        Sb.AppendLine(Line)
                    Next
                End Sub
            Dim HardSectorsHandler As EventHandler(Of HardSectorsDetectedEventArgs) =
                Sub(snd, args) Sb.AppendLine(FormatReadHardSectorsLine(args))
            Dim TrackHandler As EventHandler(Of TrackProcessedEventArgs) =
                Sub(snd, args) Sb.AppendLine(FormatReadTrackProcessedLine(args))
            Dim GaveUpHandler As EventHandler(Of ReadTrackGaveUpEventArgs) =
                Sub(snd, args) Sb.AppendLine(FormatReadTrackGaveUpLine(args))
            Dim SummaryHandler As EventHandler(Of SectorSummaryReadyEventArgs) =
                Sub(snd, args)
                    For Each Line In GreaseweazleFormatters.FormatSectorSummaryLines(args.Grid)
                        Sb.AppendLine(Line)
                    Next
                End Sub

            AddHandler Cmd.Started, StartedHandler
            AddHandler Cmd.HardSectorsDetected, HardSectorsHandler
            AddHandler Cmd.TrackProcessed, TrackHandler
            AddHandler Cmd.TrackGaveUp, GaveUpHandler
            AddHandler Cmd.SummaryReady, SummaryHandler

            Dim spec As New TrackSetSpec
            spec.Cyls.Add(0)
            spec.Heads.Add(0)
            If BothSides Then
                spec.Heads.Add(1)
            End If

            Try
                Dim Opts As New ReadOptions With {
                    .FileName = FileName,
                    .Format = Format,
                    .TrackSet = spec,
                    .Revs = 3,
                    .Live = True,
                    .Device = ConfiguredDevice(),
                    .Drive = Drive
                }

                Cmd.Run(Opts)
            Catch ex As Exception
                Sb.AppendLine("Command Failed: " & ex.Message)
            Finally
                RemoveHandler Cmd.Started, StartedHandler
                RemoveHandler Cmd.HardSectorsDetected, HardSectorsHandler
                RemoveHandler Cmd.TrackProcessed, TrackHandler
                RemoveHandler Cmd.TrackGaveUp, GaveUpHandler
                RemoveHandler Cmd.SummaryReady, SummaryHandler
            End Try

            Return (IO.File.Exists(FileName), FileName, Sb.ToString())
        End Function

        Public Function ReadFluxImage(importHandler As ReadDiskForm.ImportProcessEventHandler) As (Result As Boolean, OutputFile As String, NewFileName As String)
            Using dlg As New ReadDiskForm()

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

                If result = DialogResult.OK Then
                    If Not String.IsNullOrEmpty(dlg.NewFilePath) Then
                        Return (True, dlg.NewFilePath, dlg.NewFileName)
                    End If
                End If

                Return (False, "", "")
            End Using
        End Function

        Public Function Settings() As GreaseweazleSettings
            Return App.Globals.AppSettings.Greaseweazle
        End Function
    End Module
End Namespace