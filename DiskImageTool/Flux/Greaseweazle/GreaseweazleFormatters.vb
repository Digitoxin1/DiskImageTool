Imports System.Globalization
Imports System.Text
Imports Greaseweazle.Actions
Imports Greaseweazle.Tools

Namespace Flux.Greaseweazle
    Public Module GreaseweazleFormatters

        Public Function FormatBandwidth(result As BandwidthResult) As String
            If result Is Nothing Then Return String.Empty

            Dim sb As New StringBuilder()

            sb.AppendLine("")
            sb.AppendLine(String.Format("{0,-19}{1,-7}/   {2,-7}/   {3,-7}", "", "Min.", "Mean", "Max."))
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture,
                                        "Write Bandwidth: {0,8:F3} / {1,8:F3} / {2,8:F3} Mbps",
                                        result.WriteRow.Min, result.WriteRow.Mean, result.WriteRow.Max))
            If result.WriteGarbled Then
                sb.AppendLine("ERROR: USB write data garbled (Host -> Device)")
                Return sb.ToString()
            End If

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture,
                                        "Read Bandwidth:  {0,8:F3} / {1,8:F3} / {2,8:F3} Mbps",
                                        result.ReadRow.Min, result.ReadRow.Mean, result.ReadRow.Max))
            If result.ReadGarbled Then
                sb.AppendLine("ERROR: USB read data garbled (Device -> Host)")
                Return sb.ToString()
            End If

            sb.AppendLine("")

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture,
                                        "Estimated Consistent Min. Bandwidth: {0:F3} Mbps",
                                        result.Summary.EstimatedMinMbps))
            If result.Summary.BelowRequirement Then
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture,
                                            " -> **WARNING** BELOW REQUIRED MIN.: {0:F3} Mbps",
                                            result.Summary.RequiredMinMbps))
            Else
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture,
                                            " -> Max. Flux Rate: {0:F3} Msamples/sec",
                                            result.Summary.MaxFluxRateMsps))
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture,
                                            " -> Min. Ave. Flux: {0:F3} us",
                                            result.Summary.MinAvgFluxUs))
            End If

            Return sb.ToString()
        End Function

        Public Function FormatCleanCylinderSeekedFragment(e As CleanCylinderEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return " " & e.Cylinder.ToString(CultureInfo.InvariantCulture)
        End Function

        Public Function FormatCleanPassStartedFragment(e As CleanPassStartedEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Dim Prefix = If(e.PassIndex > 0, Environment.NewLine, String.Empty)

            Return Prefix & String.Format("Pass {0}:", e.PassIndex + 1)
        End Function

        Public Function FormatConvertHardSectorsAppliedLine(e As ConvertHardSectorsEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return String.Format(CultureInfo.InvariantCulture,
                                 "{0}: Converted to {1} hard sectors",
                                 FormatConvertTrackSpec(e.Track), e.HardSectorCount)
        End Function

        Public Function FormatConvertStartedLines(e As ConvertStartedEventArgs) As IEnumerable(Of String)
            Dim lines As New List(Of String)

            If e Is Nothing Then Return lines

            If Not String.IsNullOrEmpty(e.FormatName) Then
                lines.Add("Format " & e.FormatName)
            End If

            lines.Add(String.Format(CultureInfo.InvariantCulture,
                                    "Converting {0} -> {1}", e.InTracks, e.OutTracks))
            Return lines
        End Function

        Public Function FormatConvertTrackProcessedLine(e As TrackProcessedEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Dim spec = FormatConvertTrackSpec(e.Track)

            Select Case e.Outcome
                Case TrackDecodeOutcome.NoFormat
                    Return String.Format("{0}: {1}", spec, e.FluxSummary)

                Case TrackDecodeOutcome.OutOfRange
                    Return String.Format("{0}: WARNING: Out of range for format '{1}': Track skipped",
                                         spec, If(e.FormatName, ""))

                Case TrackDecodeOutcome.Decoded
                    Return String.Format("{0}: {1} from {2}", spec, e.DecodedSummary, e.FluxSummary)

                Case Else
                    Return String.Empty
            End Select
        End Function

        Public Function FormatConvertUnexpectedSectorLine(e As UnexpectedSectorEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return String.Format(CultureInfo.InvariantCulture,
                                 "{0}: Ignoring unexpected sector C:{1} H:{2} R:{3} N:{4}",
                                 FormatConvertTrackSpec(e.Track), e.C, e.H, e.R, e.N)
        End Function

        Public Function FormatEraseStartedLine(e As EraseStartedEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return String.Format("Erasing {0}, revs={1}", e.Tracks, e.Revs)
        End Function

        Public Function FormatEraseTrackStartedLine(e As EraseTrackEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return String.Format("T{0}.{1}: Erasing Track", e.Cyl, e.Head)
        End Function

        Public Function FormatInfo(result As DeviceInfoResult) As String
            Dim sb As New StringBuilder()

            sb.AppendLine(Info.PrintInfoLine("Host Tools", result.HostToolsVersion))
            sb.AppendLine("Device:")

            Select Case result.ConnectionState
                Case DeviceConnectionState.TestMode
                    Return sb.ToString()
                Case DeviceConnectionState.NotFound
                    sb.AppendLine("  Not found")
                    Return sb.ToString()
            End Select

            Dim dev = result.Device

            If Not String.IsNullOrEmpty(dev.Port) Then
                sb.AppendLine(Info.PrintInfoLine("Port", dev.Port, 2))
            End If

            sb.AppendLine(Info.PrintInfoLine("Model", Info.ModelName(dev.HwModel, dev.HwSubmodel), 2))

            Dim mcuStrs As New List(Of String)()
            Dim mcuName = Info.McuName(dev.McuId)
            If Not String.IsNullOrEmpty(mcuName) Then mcuStrs.Add(mcuName)
            If dev.McuMhz <> 0 Then mcuStrs.Add(String.Format(CultureInfo.InvariantCulture, "{0}MHz", dev.McuMhz))
            If dev.McuSramKb <> 0 Then mcuStrs.Add(String.Format(CultureInfo.InvariantCulture, "{0}kB SRAM", dev.McuSramKb))
            If mcuStrs.Count > 0 Then
                sb.AppendLine(Info.PrintInfoLine("MCU", String.Join(", ", mcuStrs), 2))
            End If

            Dim fwver = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", dev.FirmwareMajor, dev.FirmwareMinor)
            If dev.IsBootloader Then fwver &= " (Bootloader)"
            sb.AppendLine(Info.PrintInfoLine("Firmware", fwver, 2))

            sb.AppendLine(Info.PrintInfoLine("Serial",
                                             If(String.IsNullOrEmpty(dev.SerialNumber), "Unknown", dev.SerialNumber),
                                             2))

            Dim usbStrs As New List(Of String)() From {Info.UsbSpeedName(dev.UsbSpeedRaw)}
            If dev.UsbBufferKb <> 0 Then
                usbStrs.Add(String.Format(CultureInfo.InvariantCulture, "{0}kB Buffer", dev.UsbBufferKb))
            End If
            sb.AppendLine(Info.PrintInfoLine("USB", String.Join(", ", usbStrs), 2))

            If dev.FirmwareUpdate IsNot Nothing Then
                sb.AppendLine("")
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture,
                                            "*** New firmware version {0}.{1} is available",
                                            dev.FirmwareUpdate.LatestMajor,
                                            dev.FirmwareUpdate.LatestMinor))
                For Each line In BuildUpdateInstructionLines(dev.JumperlessUpdate, dev.HwModel)
                    sb.AppendLine(line)
                Next
            End If

            Return sb.ToString()
        End Function

        Public Function FormatReadHardSectorsLine(e As HardSectorsDetectedEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return String.Format(CultureInfo.InvariantCulture,
                                 "Drive reports {0} hard sectors", e.HardSectorCount)
        End Function

        Public Function FormatReadStartedLines(e As ReadStartedEventArgs) As IEnumerable(Of String)
            Dim lines As New List(Of String)

            If e Is Nothing Then Return lines

            lines.Add(String.Format(CultureInfo.InvariantCulture,
                                    "Reading {0} revs={1}", e.Tracks, e.RevsDisplay))
            If Not String.IsNullOrEmpty(e.FormatName) Then
                lines.Add("Format " & e.FormatName)
            End If
            Return lines
        End Function

        Public Function FormatReadTrackGaveUpLine(e As ReadTrackGaveUpEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return String.Format(CultureInfo.InvariantCulture,
                                 "{0}: Giving up: {1} sectors missing",
                                 FormatTrackSpec(e.Track), e.MissingSectors)
        End Function

        Public Function FormatReadTrackProcessedLine(e As TrackProcessedEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Dim spec = FormatTrackSpec(e.Track)

            Select Case e.Outcome
                Case TrackDecodeOutcome.NoFormat
                    Return String.Format("{0}: {1}", spec, e.FluxSummary)

                Case TrackDecodeOutcome.OutOfRange
                    Return String.Format("{0}: WARNING: Out of range for format '{1}': No format conversion applied: {2}",
                                         spec, If(e.FormatName, ""), e.FluxSummary)

                Case TrackDecodeOutcome.Decoded
                    Dim line = String.Format("{0}: {1} from {2}", spec, e.DecodedSummary, e.FluxSummary)
                    If e.Retry <> 0 Then
                        line &= String.Format(" (Retry #{0}.{1})", e.SeekRetry, e.Retry)
                    End If
                    Return line

                Case Else
                    Return String.Empty
            End Select
        End Function

        Public Function FormatReadUnexpectedSectorLine(e As UnexpectedSectorEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return String.Format(CultureInfo.InvariantCulture,
                                 "{0}: Ignoring unexpected sector C:{1} H:{2} R:{3} N:{4}",
                                 FormatTrackSpec(e.Track), e.C, e.H, e.R, e.N)
        End Function
        Public Function FormatSectorSummaryLines(grid As SectorSummaryGrid) As IEnumerable(Of String)
            Dim lines As New List(Of String)

            If grid Is Nothing OrElse Not grid.HasContent Then Return lines

            Dim inv = CultureInfo.InvariantCulture

            Dim tens As String = "Cyl-> "
            Dim p As Integer = -1
            For Each c In grid.Cyls
                tens &= If(c \ 10 = p, " ", (c \ 10).ToString(inv))
                p = c \ 10
            Next
            lines.Add(tens)

            Dim ones As String = "H. S: "
            For Each c In grid.Cyls
                ones &= (c Mod 10).ToString(inv)
            Next
            lines.Add(ones)

            For Each row In grid.Rows
                Dim line = String.Format(inv, "{0}.{1,2}: ", row.Head, row.Sector)
                For Each cell In row.Cells
                    Select Case cell
                        Case SectorSummaryCell.Empty : line &= " "
                        Case SectorSummaryCell.Good : line &= "."
                        Case SectorSummaryCell.Bad : line &= "X"
                    End Select
                Next
                lines.Add(line)
            Next

            If grid.TotalSectors <> 0 Then
                lines.Add(String.Format(inv,
                                        "Found {0} sectors of {1} ({2}%)",
                                        grid.GoodSectors,
                                        grid.TotalSectors,
                                        (grid.GoodSectors * 100) \ grid.TotalSectors))
            End If

            Return lines
        End Function

        Public Function FormatWriteHardSectorsLine(e As HardSectorsDetectedEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return String.Format(CultureInfo.InvariantCulture, "Drive reports {0} hard sectors", e.HardSectorCount)
        End Function

        Public Function FormatWriteStartedLines(e As WriteStartedEventArgs) As IEnumerable(Of String)
            Dim lines As New List(Of String)

            If e Is Nothing Then Return lines

            If Not String.IsNullOrEmpty(e.FormatName) Then
                lines.Add("Format " & e.FormatName)
            End If

            lines.Add("Writing " & e.Tracks)

            If Not String.IsNullOrEmpty(e.PrecompSummary) Then
                lines.Add(e.PrecompSummary)
            End If

            Return lines
        End Function

        Public Function FormatWriteTrackErasingLine(e As WriteTrackErasingEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return String.Format("{0}: Erasing Track", FormatWriteTrackSpec(e.Track))
        End Function

        Public Function FormatWriteTrackOutOfRangeLine(e As WriteTrackOutOfRangeEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Return String.Format("{0}: WARNING: Out of range for format '{1}': Track skipped", FormatWriteTrackSpec(e.Track), If(e.FormatName, ""))
        End Function

        Public Function FormatWriteTrackWritingLine(e As WriteTrackWritingEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Dim line = String.Format("{0}: Writing Track", FormatWriteTrackSpec(e.Track))

            If e.RetryNumber <> 0 Then
                line &= String.Format(CultureInfo.InvariantCulture, " (Verify Failure: Retry #{0})", e.RetryNumber)
            Else
                line &= String.Format(" ({0})", If(e.FluxSummary, ""))
            End If

            Return line
        End Function

        Public Function FormatWriteVerifyOutcomeLine(e As WriteVerifyOutcomeEventArgs) As String
            If e Is Nothing Then Return String.Empty

            Select Case e.Outcome
                Case WriteVerifyOutcome.AllVerified
                    Return "All tracks verified"

                Case Else
                    Dim head As String
                    If e.VerifiedCount = 0 Then
                        head = "No tracks verified "
                    Else
                        head = String.Format(CultureInfo.InvariantCulture,
                                             "{0} tracks verified; {1} tracks *not* verified ",
                                             e.VerifiedCount, e.NotVerifiedCount)
                    End If

                    Dim reason = If(e.Outcome = WriteVerifyOutcome.VerifyDisabled, "disabled", "unavailable")

                    Return head & String.Format(CultureInfo.InvariantCulture, "(Reason: Verify {0})", reason)
            End Select
        End Function

        Private Function BuildUpdateInstructionLines(jumperlessUpdate As Boolean, hwModel As Integer) As List(Of String)
            Dim lines As New List(Of String) From {"To perform an Update:"}

            If Not jumperlessUpdate Then
                lines.Add(" - Disconnect from USB")
                Dim pins = If(hwModel <> 1, "RXI-TXO", "DCLK-GND")
                lines.Add(String.Format(" - Install the Update Jumper at pins {0}", pins))
                lines.Add(" - Reconnect to USB")
            End If

            lines.Add(" - Run ""gw update"" to download and install latest firmware")

            Return lines
        End Function

        Private Function FormatConvertTrackSpec(t As TrackInfo) As String
            Dim spec = String.Format(CultureInfo.InvariantCulture, "T{0}.{1}", t.Cyl, t.Head)

            If t.PhysicalCyl <> t.Cyl OrElse t.PhysicalHead <> t.Head Then
                spec &= String.Format(CultureInfo.InvariantCulture, " <- Image {0}.{1}", t.PhysicalCyl, t.PhysicalHead)
            End If

            Return spec
        End Function

        Private Function FormatTrackSpec(t As TrackInfo) As String
            Dim spec = String.Format(CultureInfo.InvariantCulture, "T{0}.{1}", t.Cyl, t.Head)

            If t.PhysicalCyl <> t.Cyl OrElse t.PhysicalHead <> t.Head Then
                spec &= String.Format(CultureInfo.InvariantCulture, " <- Drive {0}.{1}", t.PhysicalCyl, t.PhysicalHead)
            End If

            Return spec
        End Function

        Private Function FormatWriteTrackSpec(t As TrackInfo) As String
            Dim spec = String.Format(CultureInfo.InvariantCulture, "T{0}.{1}", t.Cyl, t.Head)

            If t.PhysicalCyl <> t.Cyl OrElse t.PhysicalHead <> t.Head Then
                spec &= String.Format(CultureInfo.InvariantCulture, " -> Drive {0}.{1}", t.PhysicalCyl, t.PhysicalHead)
            End If

            Return spec
        End Function

    End Module

End Namespace
