Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Greaseweazle
    Public Class BaseForm
        Friend WithEvents Process As ConsoleProcessRunner
        Private WithEvents TS0 As FloppyTrackGrid
        Private WithEvents TS1 As FloppyTrackGrid
        Private Const TOTAL_TRACKS As UShort = 84
        Private ReadOnly _Parser As ConsoleOutputParser
        Private ReadOnly _TrackStatus As Dictionary(Of String, TrackStatusInfo)
        Private _CancelButtonClicked As Boolean = False
        Private _CurrentStatusInfo As TrackStatusInfo = Nothing
        Private _Sides As Byte
        Private _TotalBadSectors As UInteger = 0
        Private _TotalUnexpectedSectors As UInteger = 0
        Private _Tracks As UShort

        Public Enum ActionTypeEnum
            Read
            Write
            [Erase]
            Import
        End Enum

        Public Enum TrackStatusEnum
            Reading
            Erasing
            Writing
            Retry
            Failed
            Success
            Aborted
            [Error]
        End Enum

        Public Event CheckChanged(sender As Object, Checked As Boolean, Side As Byte)
        Public Event SelectionChanged(sender As Object, Track As UShort, Side As Byte, Enabled As Boolean)

        Public Sub New(Optional UseGrid As Boolean = True)
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Process = New ConsoleProcessRunner With {
                .EventContext = Threading.SynchronizationContext.Current
            }

            _Parser = New ConsoleOutputParser
            _TrackStatus = New Dictionary(Of String, TrackStatusInfo)

            If UseGrid Then
                TS0 = New FloppyTrackGrid(TOTAL_TRACKS, My.Resources.Label_Side & " 0") With {
                .Anchor = AnchorStyles.Top Or AnchorStyles.Right
            }
                TS1 = New FloppyTrackGrid(TOTAL_TRACKS, My.Resources.Label_Side & " 1") With {
                    .Anchor = AnchorStyles.Top Or AnchorStyles.Right,
                    .Margin = New Padding(32, 3, 3, 3)
                }
            End If
        End Sub

        Public ReadOnly Property CancelButtonClicked As Boolean
            Get
                Return _CancelButtonClicked
            End Get
        End Property

        Public ReadOnly Property CurrentStatusInfo As TrackStatusInfo
            Get
                Return _CurrentStatusInfo
            End Get
        End Property

        Public ReadOnly Property Parser As ConsoleOutputParser
            Get
                Return _Parser
            End Get
        End Property

        Public ReadOnly Property TableSide0 As FloppyTrackGrid
            Get
                Return TS0
            End Get
        End Property

        Public ReadOnly Property TableSide1 As FloppyTrackGrid
            Get
                Return TS1
            End Get
        End Property

        Public Function CancelProcessIfRunning() As Boolean
            If Process.IsRunning Then
                If Not ConfirmCancel() Then
                    Return True
                End If
                Process.Cancel()
                Return True
            End If

            Return False
        End Function

        Public Sub ClearStatusBar()
            StatusType.Text = ""
            StatusTrack.Text = ""
            StatusSide.Text = ""
            StatusSide.Visible = False
            StatusBadSectors.Text = ""
            StatusBadSectors.Visible = False
            StatusUnexpected.Text = ""
            StatusUnexpected.Visible = False
            _TotalBadSectors = 0
            _TotalUnexpectedSectors = 0
        End Sub

        Public Sub ClearTrackStatus()
            _TrackStatus.Clear()
            _CurrentStatusInfo = Nothing
        End Sub

        Public Function ConfirmWrite(Title As String, DriveName As String) As Boolean
            Dim Msg = String.Format(My.Resources.Dialog_ConfirmWrite, vbNewLine, DriveName, Title)
            Return MsgBox(Msg, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2 + MsgBoxStyle.Exclamation, Title) = MsgBoxResult.Yes
        End Function

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

        Public Function GetDriveName(Drive As String) As String
            Select Case Drive
                Case "A", "B"
                    Return Drive & ":"
                Case Else
                    Return "DS" & Drive & ":"
            End Select
        End Function

        Public Sub GridReset()
            GridReset(_Tracks, _Sides)
        End Sub

        Public Sub GridReset(Tracks As UShort, Sides As Byte, Optional ResetSelected As Boolean = True)
            _Tracks = Tracks
            _Sides = Sides

            GridResetTracks(TS0, Tracks, False, ResetSelected)
            GridResetTracks(TS1, Tracks, Sides < 2, ResetSelected)
        End Sub

        Public Sub SaveLog()
            Dim FileName As String = GreaseweazleSettings.LogFileName
            Dim Extension = IO.Path.GetExtension(FileName).ToLower
            Dim FilterIndex As Integer = 1
            If Extension <> ".txt" Then
                FilterIndex = 2
            End If

            Using Dialog As New SaveFileDialog With {
                   .FileName = FileName,
                   .DefaultExt = "txt",
                   .Filter = My.Resources.FileType_Text & " (*.txt)|*.txt|" & My.Resources.FileType_All & " (*.*)|*.*",
                   .FilterIndex = FilterIndex
                }

                If Dialog.ShowDialog = DialogResult.OK Then
                    IO.File.WriteAllText(Dialog.FileName, TextBoxConsole.Text & vbNewLine)
                End If
            End Using
        End Sub

        Public Function UpdateStatusInfo(TrackInfo As ConsoleOutputParser.TrackInfoWrite, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(TrackInfo.SrcTrack, TrackInfo.SrcSide)

            StatusInfo.Action = Action
            StatusInfo.Retries = Math.Max(StatusInfo.Retries, TrackInfo.Retry)
            If TrackInfo.Failed Then
                StatusInfo.Failed = TrackInfo.Failed
            End If

            Return StatusInfo
        End Function

        Public Function UpdateStatusInfo(TrackInfo As ConsoleOutputParser.TrackInfoRead, ProcessBadSectors As Boolean, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(TrackInfo.DestTrack, TrackInfo.DestSide)

            StatusInfo.Action = Action

            If TrackInfo.Seek > 0 And TrackInfo.Retry > 0 Then
                StatusInfo.Retries += 1
            End If

            If ProcessBadSectors Then
                StatusInfo.BadSectors += TrackInfo.BadSectors
                _TotalBadSectors += TrackInfo.BadSectors
                If TrackInfo.BadSectors > 0 Then
                    StatusInfo.Failed = True
                End If
            End If

            Return StatusInfo
        End Function

        Public Function UpdateStatusInfo(TrackInfo As ConsoleOutputParser.UnexpectedSector, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(TrackInfo.Track, TrackInfo.Side)

            StatusInfo.Action = Action

            If Not StatusInfo.UnexpectedSectors.ContainsKey(TrackInfo.Key) Then
                StatusInfo.UnexpectedSectors.Add(TrackInfo.Key, TrackInfo)
                _TotalUnexpectedSectors += 1
            End If

            Return StatusInfo
        End Function

        Public Function UpdateStatusInfo(TrackInfo As ConsoleOutputParser.TrackInfoReadFailed, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(TrackInfo.DestTrack, TrackInfo.DestSide)

            StatusInfo.Action = Action

            StatusInfo.Failed = True
            StatusInfo.BadSectors = TrackInfo.Sectors
            _TotalBadSectors += TrackInfo.Sectors

            Return StatusInfo
        End Function
        Public Sub UpdateTrackStatus(Statusinfo As TrackStatusInfo, Action As String, DoubleStep As Boolean)
            If _CurrentStatusInfo IsNot Nothing Then
                ProcessTrackStatusInfo(_CurrentStatusInfo, "Complete", DoubleStep)
            End If

            _CurrentStatusInfo = Statusinfo

            Dim Status = ProcessTrackStatusInfo(Statusinfo, Action, DoubleStep)

            StatusType.Text = GetTrackStatusText(Status, _CurrentStatusInfo.Retries)
            StatusTrack.Text = My.Resources.Label_Track & " " & Statusinfo.Track
            StatusSide.Text = My.Resources.Label_Side & " " & Statusinfo.Side
            StatusSide.Visible = True

            If _TotalBadSectors = 1 Then
                _StatusBadSectors.Text = _TotalBadSectors & " " & My.Resources.Label_BadSector
                _StatusBadSectors.Visible = True
            ElseIf _TotalBadSectors > 1 Then
                _StatusBadSectors.Text = _TotalBadSectors & " " & My.Resources.Label_BadSectors
                _StatusBadSectors.Visible = True
            Else
                _StatusBadSectors.Visible = False
            End If

            If _TotalUnexpectedSectors = 1 Then
                _StatusUnexpected.Text = _TotalUnexpectedSectors & " " & My.Resources.Label_UnexpectedSector
                _StatusUnexpected.Visible = True
            ElseIf _TotalUnexpectedSectors > 1 Then
                _StatusUnexpected.Text = _TotalUnexpectedSectors & " " & My.Resources.Label_UnexpectedSectors
                _StatusUnexpected.Visible = True
            Else
                _StatusUnexpected.Visible = False
            End If
        End Sub

        Public Sub UpdateTrackStatusComplete(Aborted As Boolean, DoubleStep As Boolean, Optional KeepProcessing As Boolean = False)
            If Aborted Then
                StatusType.Text = GetTrackStatusText(TrackStatusEnum.Aborted)
            Else
                If CurrentStatusInfo IsNot Nothing Then
                    ProcessTrackStatusInfo(CurrentStatusInfo, "Complete", DoubleStep)
                End If

                If KeepProcessing Then
                    Exit Sub
                End If

                If CurrentStatusInfo IsNot Nothing AndAlso CurrentStatusInfo.Failed Then
                    StatusType.Text = GetTrackStatusText(TrackStatusEnum.Failed)
                Else
                    StatusType.Text = GetTrackStatusText(TrackStatusEnum.Success)
                End If
            End If
        End Sub

        Public Sub UpdateTrackStatusError()
            StatusType.Text = GetTrackStatusText(TrackStatusEnum.Error)
        End Sub

        Friend Function GetTrackHeads(StartHead As Integer, Optional EndHead As Integer = -1) As CommandLineBuilder.TrackHeads
            If EndHead = -1 Then
                EndHead = StartHead
            End If

            If StartHead = 0 And EndHead = 0 Then
                Return CommandLineBuilder.TrackHeads.head0
            ElseIf StartHead = 1 And EndHead = 1 Then
                Return CommandLineBuilder.TrackHeads.head1
            Else
                Return CommandLineBuilder.TrackHeads.both
            End If
        End Function

        Protected Overridable Sub OnAfterBaseFormClosing(e As FormClosingEventArgs)
            ' Default: do nothing
        End Sub

        Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
            If Process.IsRunning Then
                If e.CloseReason = CloseReason.UserClosing OrElse _CancelButtonClicked Then
                    If Not ConfirmCancel() Then
                        e.Cancel = True
                        _CancelButtonClicked = False
                        Exit Sub
                    End If
                End If
                Try
                    Process.Cancel()
                Catch ex As Exception
                End Try
            End If

            If e.Cancel Then
                _CancelButtonClicked = False
                Return
            End If

            OnAfterBaseFormClosing(e)

            _CancelButtonClicked = False

            If e.Cancel Then
                Return
            End If

            MyBase.OnFormClosing(e)
        End Sub

        Private Function ConfirmCancel() As Boolean
            Return MsgBox(My.Resources.Dialog_ConfirmCancel, MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2 + MsgBoxStyle.Exclamation) = MsgBoxResult.Yes
        End Function
        Private Function GetStatusInfo(Track As Integer, Side As Integer) As TrackStatusInfo
            Dim Key = Track & "." & Side
            Dim StatusInfo As TrackStatusInfo

            If _TrackStatus.ContainsKey(Key) Then
                StatusInfo = _TrackStatus.Item(Key)
            Else
                StatusInfo = New TrackStatusInfo With {
                    .Track = Track,
                    .Side = Side
                }
                _TrackStatus.Add(Key, StatusInfo)
            End If

            Return StatusInfo
        End Function

        Private Function GetTrackStatusColor(Status As TrackStatusEnum) As (ForeColor As Color, BackColor As Color)
            Select Case Status
                Case TrackStatusEnum.Reading
                    Return (Color.Black, Color.LightBlue)
                Case TrackStatusEnum.Erasing
                    Return (Color.Black, Color.MediumPurple)
                Case TrackStatusEnum.Writing
                    Return (Color.Black, Color.Orange)
                Case TrackStatusEnum.Retry
                    Return (Color.Black, Color.Yellow)
                Case TrackStatusEnum.Failed
                    Return (Color.Black, Color.Red)
                Case TrackStatusEnum.Success
                    Return (Color.Black, Color.LightGreen)
                Case Else
                    Return (Color.Black, Color.Gray)
            End Select
        End Function

        Private Function GetTrackStatusText(Status As TrackStatusEnum, Optional Retries As UShort = 0) As String
            Select Case Status
                Case TrackStatusEnum.Reading
                    Return My.Resources.Label_Reading
                Case TrackStatusEnum.Erasing
                    Return My.Resources.Label_Erasing
                Case TrackStatusEnum.Writing
                    Return My.Resources.Label_Writing
                Case TrackStatusEnum.Retry
                    Return My.Resources.Label_Retrying & IIf(Retries > 0, " " & Retries, "")
                Case TrackStatusEnum.Failed
                    Return My.Resources.Label_Failed
                Case TrackStatusEnum.Success
                    Return My.Resources.Label_Complete
                Case TrackStatusEnum.Aborted
                    Return My.Resources.Label_Aborted
                Case TrackStatusEnum.Error
                    Return My.Resources.Label_Error
                Case Else
                    Return ""
            End Select
        End Function

        Private Function GridGetTable(Side As Byte) As FloppyTrackGrid
            If Side = 0 Then
                Return TS0
            ElseIf Side = 1 Then
                Return TS1
            Else
                Return Nothing
            End If
        End Function

        Private Sub GridMarkTrack(Track As Integer, Side As Integer, Status As TrackStatusEnum, Label As String, DoubleStep As Boolean)
            Dim Table = GridGetTable(Side)

            If Table IsNot Nothing Then
                Dim Colors = GetTrackStatusColor(Status)
                If DoubleStep Then
                    Track *= 2
                End If
                Table.SetCell(Track, Text:=Label, BackColor:=Colors.BackColor, ForeColor:=Colors.ForeColor)
            End If
        End Sub
        Private Sub GridResetTracks(Grid As FloppyTrackGrid, Tracks As UShort, Disabled As Boolean, Optional ResetSelected As Boolean = True)
            Grid.ActiveTrackCount = Tracks
            Grid.ResetAll()

            If ResetSelected Then
                Grid.IsChecked = False
            End If
            Grid.Disabled = Disabled
        End Sub

        Private Function ProcessTrackStatusInfo(Statusinfo As TrackStatusInfo, Action As String, DoubleStep As Boolean) As TrackStatusEnum
            Dim StatusData = ProcessTrackStatusInfo(Statusinfo, Action)

            GridMarkTrack(Statusinfo.Track, Statusinfo.Side, StatusData.Status, StatusData.Label, DoubleStep)

            Return StatusData.Status
        End Function

        Private Function ProcessTrackStatusInfo(Statusinfo As TrackStatusInfo, Action As String) As (Status As TrackStatusEnum, Label As String)
            Dim Status As TrackStatusEnum
            Dim Label As String = ""

            If Statusinfo.Failed Then
                Status = TrackStatusEnum.Failed
                If Statusinfo.Action = ActionTypeEnum.Read Or Statusinfo.Action = ActionTypeEnum.Import Then
                    Label = Statusinfo.BadSectors
                Else
                    Label = Statusinfo.Retries
                End If
            ElseIf Statusinfo.Retries > 0 Then
                Status = TrackStatusEnum.Retry
                Label = Statusinfo.Retries
            ElseIf Action = "Complete" Then
                If Statusinfo.Retries > 0 Then
                    Status = TrackStatusEnum.Retry
                    Label = Statusinfo.Retries
                Else
                    Status = TrackStatusEnum.Success
                    Label = ""
                End If
            ElseIf Action = "Erasing" Then
                Status = TrackStatusEnum.Erasing
                Label = "E"
            ElseIf Action = "Writing" Then
                Status = TrackStatusEnum.Writing
                Label = "W"
            ElseIf Action = "Reading" Then
                Status = TrackStatusEnum.Reading
                Label = "R"
            End If

            Return (Status, Label)
        End Function
#Region "Events"

        Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
            _CancelButtonClicked = True
        End Sub


        Private Sub ButtonSaveLog_Click(sender As Object, e As EventArgs) Handles ButtonSaveLog.Click
            SaveLog()
        End Sub

        Private Sub TS0_CheckChanged(sender As Object, Checked As Boolean) Handles TS0.CheckChanged
            If Checked Then
                TS0.SetCellsSelected(TS1.SelectedTracks, Checked)
            End If

            RaiseEvent CheckChanged(Me, Checked, 0)
        End Sub

        Private Sub TS0_SelectionChanged(sender As Object, Track As Integer, Selected As Boolean) Handles TS0.SelectionChanged
            If TS1.SelectEnabled Then
                If TS1.IsChecked Then
                    TS1.SetCellSelected(Track, Selected, True)
                End If
            End If

            RaiseEvent SelectionChanged(Me, Track, 0, Selected)
        End Sub

        Private Sub TS1_CheckChanged(sender As Object, Checked As Boolean) Handles TS1.CheckChanged
            If Checked Then
                TS1.SetCellsSelected(TS0.SelectedTracks, Checked)
            End If

            RaiseEvent CheckChanged(Me, Checked, 1)
        End Sub

        Private Sub TS1_SelectionChanged(sender As Object, Track As Integer, Selected As Boolean) Handles TS1.SelectionChanged
            If TS0.SelectEnabled Then
                If TS0.IsChecked Then
                    TS0.SetCellSelected(Track, Selected, True)
                End If
            End If

            RaiseEvent SelectionChanged(Me, Track, 1, Selected)
        End Sub
#End Region
        Public Class TrackStatusInfo
            Public Sub New()
                _UnexpectedSectors = New Dictionary(Of String, ConsoleOutputParser.UnexpectedSector)
            End Sub

            Public Property Action As ActionTypeEnum
            Public Property BadSectors As UShort = 0
            Public Property Failed As Boolean
            Public Property Retries As UShort = 0
            Public Property Side As Integer
            Public Property Track As Integer
            Public Property UnexpectedSectors As Dictionary(Of String, ConsoleOutputParser.UnexpectedSector)
        End Class
    End Class
End Namespace
