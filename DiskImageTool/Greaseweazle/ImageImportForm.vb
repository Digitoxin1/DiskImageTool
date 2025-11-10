Imports System.Globalization
Imports System.Text.RegularExpressions
Imports DiskImageTool.Greaseweazle

Public Class ImageImportForm
    Private WithEvents Process As ConsoleProcessRunner
    Private Const TOTAL_TRACKS As UShort = 83

    Private ReadOnly _TrackStatus As Dictionary(Of String, TrackStatusInfo)

    Private ReadOnly RxConverting As New Regex(
            "^\s*Converting\s+c=(?<cs>\d+)(?:-(?<ce>\d+))?\s*:\s*h=(?<hs>\d+)(?:-(?<he>\d+))?\s*->\s*c=(?<ds>\d+)(?:-(?<de>\d+))?\s*:\s*h=(?<dhs>\d+)(?:-(?<dhe>\d+))?\s*$",
        RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Private ReadOnly RxLine As New Regex(
        "^T(?<srcTrack>\d+)\.(?<srcSide>\d+)( <- Image (?<imgTrack>\d+)\.(?<imgSide>\d+))?: (?<system>\w+) (?<encoding>\w+)( \((?<sectorsFound>\d+)\/(?<sectorsTotal>\d+) sectors\))?( from (?<sourceType>.+))? \((?<fluxCount>\d+) flux in (?<fluxTimeMS>\d+(\.?\d+))?ms\)",
        RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Private ReadOnly RxUnexpected As New Regex(
        "^T(?<srcTrack>\d+)\.(?<srcSide>\d+): Ignoring unexpected sector C:(?<cylinder>\d+) H:(?<head>\d+) R:(?<sectorId>\d+) N:(?<sizeId>\d+)",
        RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

    Private _InputFilePath As String
    Private _OutputFilePath As String = ""
    Private _ProcessRunning As Boolean = False
    Private _SideCount As Integer
    Private _TrackCount As Integer
    Private _DoubleStep As Boolean = False
    Private _TS0 As TableLayoutPanel
    Private _TS1 As TableLayoutPanel
    Private _TotalBadSectors As UInteger = 0
    Private _TotalUnexpectedSectors As UInteger = 0
    Public Sub New(FilePath As String, TrackCount As Integer, SideCount As Integer)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _InputFilePath = FilePath
        _TrackStatus = New Dictionary(Of String, TrackStatusInfo)
        _TrackCount = TrackCount
        _SideCount = SideCount
        TrackGridInit(_TrackCount, _SideCount)
        Dim ImageFormat = DetectImageFormat()
        PopulateImageFormats(ImageFormat)
        PopulateOutputTypes()

        Process = New ConsoleProcessRunner With {
            .EventContext = Threading.SynchronizationContext.Current
        }

        SetTilebarText()
        ResetStatusBar()
    End Sub

    Public ReadOnly Property OutputFilePath As String
        Get
            Return _OutputFilePath
        End Get
    End Property

    Private Sub ResetStatusBar()
        StatusType.Text = ""
        StatusTrack.Text = ""
        StatusSide.Text = ""
        StatusBadSectors.Text = ""
        StatusUnexpected.Text = ""
        _TotalBadSectors = 0
        _TotalUnexpectedSectors = 0
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        If Process.IsRunning Then
            Try
                Process.Cancel()
            Catch ex As Exception
            End Try
        Else
            ClearOutputFile()
        End If
    End Sub

    Private Sub ClearOutputFile()
        If Not String.IsNullOrEmpty(_OutputFilePath) Then
            DeleteFileIfExists(_OutputFilePath)
        End If
        _OutputFilePath = ""
    End Sub

    Private Function DetectImageFormat() As GreaseweazleImageFormat
        Dim Response = ConvertFirstTrack(_InputFilePath)

        If Not Response.Item1 Then
            Return GreaseweazleImageFormat.None
        End If

        Dim DetectedFormat As DiskImage.FloppyDiskFormat = DiskImage.FloppyDiskFormat.FloppyUnknown
        Dim Buffer As Byte()

        Using fs As New IO.FileStream(Response.Item2, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
            Using br As New IO.BinaryReader(fs, System.Text.Encoding.ASCII, leaveOpen:=False)
                Buffer = br.ReadBytes(513)
            End Using
        End Using

        If Buffer.Length >= 512 Then
            Dim BootSector = New DiskImage.BootSector(Buffer)
            Dim MediaDescriptor As Byte = 0
            If Buffer.Length = 513 Then
                MediaDescriptor = Buffer(512)
            End If
            DetectedFormat = DiskImage.GetFloppyDiskFormat(BootSector.BPB, MediaDescriptor)
        End If

        DeleteFileIfExists(Response.Item2)

        Return GreaseweazleImageFormatFromFloppyDiskFormat(DetectedFormat)
    End Function

    Private Function GenerateCommandLine(ImageFormat As GreaseweazleImageFormat, OutputType As GreaseweazleOutputType, DoubleStep As Boolean) As String
        Dim Builder = New CommandLineBuilder(CommandLineBuilder.CommandAction.convert) With {
            .InFile = _InputFilePath,
            .OutFile = _OutputFilePath
        }

        If OutputType <> GreaseweazleOutputType.HFE Then
            Builder.Format = GreaseweazleImageFormatCommandLine(ImageFormat)
        Else
            Builder.BitRate = GreaseweazleImageFormatBitrate(ImageFormat)
            Builder.AdjustSpeed = GreaseweazleImageFormatRPM(ImageFormat) & "rpm"
        End If

        If DoubleStep Then
            Builder.HeadStep = 2
        End If

        Return Builder.Arguments
    End Function

    Private Function GetDouble(m As Match, name As String) As Double
        Dim v As Double
        Double.TryParse(m.Groups(name).Value, NumberStyles.Float, CultureInfo.InvariantCulture, v)
        Return v
    End Function

    Private Function GetInt(m As Match, name As String) As Integer
        Dim v As Integer
        Integer.TryParse(m.Groups(name).Value, NumberStyles.None, CultureInfo.InvariantCulture, v)
        Return v
    End Function

    Private Function GetIntOrDefault(m As Match, name As String, def As Integer) As Integer
        If Not m.Groups(name).Success Then Return def
        Return GetInt(m, name)
    End Function

    Private Function ParseConvertingLine(line As String) As ConvertingRange
        If String.IsNullOrWhiteSpace(line) Then
            Return Nothing
        End If

        Dim m = RxConverting.Match(line)
        If Not m.Success Then
            Return Nothing
        End If

        Dim r As New ConvertingRange With {
            .SrcCylStart = GetInt(m, "cs"),
            .SrcCylEnd = GetIntOrDefault(m, "ce", GetInt(m, "cs")),
            .SrcHeadStart = GetInt(m, "hs"),
            .SrcHeadEnd = GetIntOrDefault(m, "he", GetInt(m, "hs")),
            .DstCylStart = GetInt(m, "ds"),
            .DstCylEnd = GetIntOrDefault(m, "de", GetInt(m, "ds")),
            .DstHeadStart = GetInt(m, "dhs"),
            .DstHeadEnd = GetIntOrDefault(m, "dhe", GetInt(m, "dhs"))
        }

        Return r
    End Function

    Private Function ParseTrackInfoUnexpected(line As String) As TrackInfoUnexpected
        If String.IsNullOrWhiteSpace(line) Then
            Return Nothing
        End If

        Dim Match = RxUnexpected.Match(line)
        If Not Match.Success Then
            Return Nothing
        End If

        Dim info As New TrackInfoUnexpected With {
            .SourceTrack = GetInt(Match, "srcTrack"),
            .SourceSide = GetInt(Match, "srcSide"),
            .Cylinder = GetInt(Match, "cylinder"),
            .Head = GetInt(Match, "head"),
            .SectorId = GetInt(Match, "sectorid"),
            .SizeId = GetInt(Match, "sizeid")
        }

        Return info
    End Function

    Private Function ParseTrackInfo(line As String) As TrackInfo
        If String.IsNullOrWhiteSpace(line) Then
            Return Nothing
        End If

        Dim Match = RxLine.Match(line)
        If Not Match.Success Then
            Return Nothing
        End If


        Dim info As New TrackInfo With {
            .SourceTrack = GetInt(Match, "srcTrack"),
            .SourceSide = GetInt(Match, "srcSide"),
            .ImageTrack = GetIntOrDefault(Match, "imgTrack", .SourceTrack),
            .ImageSide = GetIntOrDefault(Match, "imgSide", .SourceSide),
            .System = Match.Groups("syatem").Value.Trim(),
            .Encoding = Match.Groups("encoding").Value.Trim(),
            .SectorsFound = GetInt(Match, "sectorsFound"),
            .SectorsTotal = GetInt(Match, "sectorsTotal"),
            .SourceType = Match.Groups("sourceType").Value.Trim(),
            .FluxCount = GetInt(Match, "fluxCount"),
            .FluxTimeMs = GetDouble(Match, "fluxTimeMS")
        }

        Return info
    End Function

    Private Sub PopulateImageFormats(SelectedValue As GreaseweazleImageFormat)
        Dim DriveList As New List(Of KeyValuePair(Of String, GreaseweazleImageFormat))
        For Each ImageFormat As GreaseweazleImageFormat In [Enum].GetValues(GetType(GreaseweazleImageFormat))
            DriveList.Add(New KeyValuePair(Of String, GreaseweazleImageFormat)(
                GreaseweazleImageFormatDescription(ImageFormat), ImageFormat)
            )
        Next

        With ComboImageFormat
            .DisplayMember = "Key"
            .ValueMember = "Value"
            .DataSource = DriveList
            .DropDownStyle = ComboBoxStyle.DropDownList
            .SelectedValue = SelectedValue
        End With
    End Sub

    Private Sub PopulateOutputTypes()
        Dim DriveList As New List(Of KeyValuePair(Of String, GreaseweazleOutputType))
        For Each OutputType As GreaseweazleOutputType In [Enum].GetValues(GetType(GreaseweazleOutputType))
            DriveList.Add(New KeyValuePair(Of String, GreaseweazleOutputType)(
                GreaseweazleOutputTypeDescription(OutputType), OutputType)
            )
        Next

        With ComboOutputType
            .DisplayMember = "Key"
            .ValueMember = "Value"
            .DataSource = DriveList
            .DropDownStyle = ComboBoxStyle.DropDownList
        End With
    End Sub

    Private Sub ProcessOutputLine(line As String)
        TextBoxConsole.AppendText(line & vbCrLf)

        Dim TrackInfo = ParseTrackInfo(line)
        If TrackInfo IsNot Nothing Then
            Dim Statusinfo = UpdateStatusInfo(TrackInfo)
            ProcessTrackStatus(Statusinfo)
            Return
        End If

        Dim TrackInfoUnexpected = ParseTrackInfoUnexpected(line)
        If TrackInfoUnexpected IsNot Nothing Then
            Dim StatusInfo = UpdateStatusInfo(TrackInfoUnexpected)
            ProcessTrackStatus(StatusInfo)
            Return
        End If
    End Sub

    Private Sub ProcessTrackStatus(StatusInfo As TrackStatusInfo)
        Dim Table As TableLayoutPanel

        If StatusInfo.Side = 0 Then
            Table = _TS0
        ElseIf StatusInfo.Side = 1 Then
            Table = _TS1
        Else
            Table = Nothing
        End If

        If Table IsNot Nothing Then
            Dim Track = StatusInfo.Track
            If _DoubleStep Then
                Track *= 2
            End If

            Dim BackColor As Color
            Dim Text As String = ""
            If StatusInfo.BadSectors > 0 Then
                BackColor = Color.Red
                Text = StatusInfo.BadSectors
            ElseIf StatusInfo.UnexpectedSectors > 0 Then
                BackColor = Color.Yellow
                Text = StatusInfo.UnexpectedSectors
            Else
                BackColor = Color.LightGreen
            End If
            FloppyGridSetLabel(Table, Track, Text, BackColor)

            StatusTrack.Text = My.Resources.Label_Track & " " & Track
            StatusSide.Text = My.Resources.Label_Side & " " & StatusInfo.Side

            If _TotalBadSectors = 1 Then
                StatusBadSectors.Text = _TotalBadSectors & " " & My.Resources.Label_BadSector
            ElseIf _TotalBadSectors > 1 Then
                StatusBadSectors.Text = _TotalBadSectors & " " & My.Resources.Label_BadSectors
            End If

            If _TotalUnexpectedSectors = 1 Then
                StatusUnexpected.Text = _TotalUnexpectedSectors & " " & My.Resources.Label_UnexpectedSector
            ElseIf _TotalUnexpectedSectors > 1 Then
                StatusUnexpected.Text = _TotalUnexpectedSectors & " " & My.Resources.Label_UnexpectedSectors
            End If
        End If
    End Sub

    Private Sub RefreshButtonState()
        Dim ImageFormat As GreaseweazleImageFormat = ComboImageFormat.SelectedValue

        ComboImageFormat.Enabled = Not _ProcessRunning
        ComboOutputType.Enabled = Not _ProcessRunning

        ButtonProcess.Enabled = ImageFormat <> GreaseweazleImageFormat.None
        If _ProcessRunning Then
            ButtonProcess.Text = My.Resources.Label_Abort
        Else
            ButtonProcess.Text = My.Resources.Label_Process
        End If

        Select Case ImageFormat
            Case GreaseweazleImageFormat.IBM_160, GreaseweazleImageFormat.IBM_180, GreaseweazleImageFormat.IBM_320, GreaseweazleImageFormat.IBM_360
                CheckBoxDoublestep.Enabled = Not _ProcessRunning AndAlso _TrackCount > 42
                CheckBoxDoublestep.Checked = _TrackCount > 79
            Case Else
                CheckBoxDoublestep.Enabled = False
                CheckBoxDoublestep.Checked = False
        End Select

        ButtonImport.Enabled = Not _ProcessRunning And Not String.IsNullOrEmpty(_OutputFilePath)
    End Sub

    Private Sub SetTilebarText()
        Dim DisplayFileName = IO.Path.GetFileName(_InputFilePath)
        Dim FileExt = IO.Path.GetExtension(DisplayFileName).ToLower
        If FileExt = ".raw" Then
            Dim ParentFolder As String = IO.Path.GetFileName(IO.Directory.GetParent(_InputFilePath).FullName)
            DisplayFileName = IO.Path.Combine(ParentFolder, "*.raw")
        End If

        Me.Text = My.Resources.Caption_ImportFluxImage & " - " & DisplayFileName
    End Sub

    Private Sub ToggleProcessRunning(Value As Boolean)
        _ProcessRunning = Value
        RefreshButtonState()
    End Sub

    Private Sub TrackGridInit(Tracks As UShort, Sides As Byte)
        _TS0 = FloppyGridInit(TableSide0, My.Resources.Label_Side & " 0", Tracks, Math.Max(Tracks, TOTAL_TRACKS))

        Dim TrackCount As UShort = 0
        If Sides > 1 Then
            TrackCount = Tracks
        End If
        _TS1 = FloppyGridInit(TableSide1, My.Resources.Label_Side & " 1", TrackCount, Math.Max(Tracks, TOTAL_TRACKS))
    End Sub

    Private Sub TrackGridReset(Tracks As UShort, Sides As Byte)
        FloppyGridReset(_TS0, Tracks, Math.Max(Tracks, TOTAL_TRACKS))

        Dim TrackCount As UShort = 0
        If Sides > 1 Then
            TrackCount = Tracks
        End If
        FloppyGridReset(_TS1, TrackCount, Math.Max(Tracks, TOTAL_TRACKS))
    End Sub

    Private Function UpdateStatusInfo(TrackInfo As TrackInfo) As TrackStatusInfo
        Dim Key = TrackInfo.SourceTrack & "." & TrackInfo.SourceSide
        Dim StatusInfo As TrackStatusInfo

        If _TrackStatus.ContainsKey(Key) Then
            StatusInfo = _TrackStatus.Item(Key)
        Else
            StatusInfo = New TrackStatusInfo With {
                .Track = TrackInfo.SourceTrack,
                .Side = TrackInfo.SourceSide
            }
            _TrackStatus.Add(Key, StatusInfo)
        End If
        StatusInfo.BadSectors += TrackInfo.BadSectors
        _TotalBadSectors += TrackInfo.BadSectors

        Return StatusInfo
    End Function

    Private Function UpdateStatusInfo(TrackInfo As TrackInfoUnexpected) As TrackStatusInfo
        Dim Key = TrackInfo.SourceTrack & "." & TrackInfo.SourceSide
        Dim StatusInfo As TrackStatusInfo

        If _TrackStatus.ContainsKey(Key) Then
            StatusInfo = _TrackStatus.Item(Key)
        Else
            StatusInfo = New TrackStatusInfo With {
                .Track = TrackInfo.SourceTrack,
                .Side = TrackInfo.SourceSide
            }
            _TrackStatus.Add(Key, StatusInfo)
        End If
        StatusInfo.UnexpectedSectors += 1
        _TotalUnexpectedSectors += 1

        Return StatusInfo
    End Function

    Private Sub ProcessImage()
        If Process.IsRunning Then
            Process.Cancel()
            Exit Sub
        End If

        ClearOutputFile()
        ResetStatusBar()

        Dim ImageFormat As GreaseweazleImageFormat = ComboImageFormat.SelectedValue
        Dim OutputType As GreaseweazleOutputType = ComboOutputType.SelectedValue

        Dim TempPath = InitTempImagePath()
        Dim FileName = "New Image" & GreaseweazleOutputTypeFileExt(OutputType)

        If TempPath = "" Then
            MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        TextBoxConsole.Clear()
        _OutputFilePath = GenerateUniqueFileName(TempPath, FileName)

        _TrackStatus.Clear()
        TrackGridReset(_TrackCount, _SideCount)

        Dim DoubleStep As Boolean = CheckBoxDoublestep.Enabled AndAlso CheckBoxDoublestep.Checked
        _DoubleStep = DoubleStep

        ToggleProcessRunning(True)

        Dim Arguments = GenerateCommandLine(ImageFormat, OutputType, DoubleStep)
        Process.StartAsync(My.Settings.GW_Path, Arguments)
    End Sub
#Region "Events"

    Private Sub ButtonProcess_Click(sender As Object, e As EventArgs) Handles ButtonProcess.Click
        ProcessImage()
    End Sub


    Private Sub ComboImageFormat_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboImageFormat.SelectedIndexChanged
        RefreshButtonState()
    End Sub

    Private Sub ImageImportForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Process.IsRunning Then
            Try
                Process.Cancel()
            Catch ex As Exception
            End Try
        ElseIf e.CloseReason = CloseReason.UserClosing Then
            ClearOutputFile()
        End If
    End Sub

    Private Sub Process_ErrorLineReceived(line As String) Handles Process.ErrorLineReceived
        ProcessOutputLine(line)
    End Sub

    Private Sub Process_ProcessExited(exitCode As Integer) Handles Process.ProcessExited
        If exitCode = -1 Then
            ClearOutputFile()
            StatusType.Text = My.Resources.Label_Aborted
        Else
            StatusType.Text = My.Resources.Label_Complete
        End If
        ToggleProcessRunning(False)
    End Sub

    Private Sub Process_ProcessFailed(message As String, ex As Exception) Handles Process.ProcessFailed
        StatusType.Text = My.Resources.Label_Failed
        ToggleProcessRunning(False)
    End Sub
#End Region
    Private Class ConvertingRange
        Public Property DstCylEnd As Integer
        Public Property DstCylStart As Integer
        Public Property DstHeadEnd As Integer
        Public Property DstHeadStart As Integer
        Public Property SrcCylEnd As Integer
        Public Property SrcCylStart As Integer
        Public Property SrcHeadEnd As Integer
        Public Property SrcHeadStart As Integer
    End Class

    Private Class TrackInfoUnexpected
        Public Property SourceTrack As Integer
        Public Property SourceSide As Integer
        Public Property Cylinder As Integer
        Public Property Head As Integer
        Public Property SectorId As Integer
        Public Property SizeId As Integer
    End Class

    Private Class TrackInfo
        Public ReadOnly Property BadSectors As Integer
            Get
                Return _SectorsTotal - _SectorsFound
            End Get
        End Property

        Public Property Encoding As String
        Public Property FluxCount As Integer
        Public Property FluxTimeMs As Double
        Public Property ImageSide As Integer
        Public Property ImageTrack As Integer
        Public ReadOnly Property IsRemapped As Boolean
            Get
                Return _ImageTrack <> _SourceTrack OrElse _ImageSide <> _SourceSide
            End Get
        End Property
        Public Property SectorsFound As Integer
        Public Property SectorsTotal As Integer
        Public Property SourceSide As Integer
        Public Property SourceTrack As Integer
        Public Property SourceType As String
        Public Property System As String
    End Class

    Private Class TrackStatusInfo
        Public Property BadSectors As UShort = 0
        Public Property Side As Integer
        Public Property Track As Integer
        Public Property UnexpectedSectors As UShort = 0
    End Class

    Private Sub Process_ProcessStarted(exePath As String, arguments As String) Handles Process.ProcessStarted
        StatusType.Text = My.Resources.Label_Processing
    End Sub
End Class
