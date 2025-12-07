Namespace Flux.Greaseweazle
    Public Class TrackStatus
        Inherits ConsoleParser
        Implements ITrackStatus

        Private ReadOnly _StatusCollection As Dictionary(Of String, TrackStatusInfo)
        Private ReadOnly _TrackFound As Boolean = True
        Private _CurrentStatusInfo As TrackStatusInfo = Nothing
        Private _Failed As Boolean = False
        Private _Range As TrackRange? = Nothing
        Private _TotalBadSectors As UInteger = 0
        Private _TotalUnexpectedSectors As UInteger = 0

        Private Enum TrackStatusEnum
            Reading
            Erasing
            Writing
            Retry
            Failed
            Success
            Aborted
            [Error]
            Unexpected
        End Enum

        Friend Event UpdateGridTooltip(Track As Integer, Side As Integer, Tooltip As String) Implements ITrackStatus.UpdateGridTooltip
        Friend Event UpdateGridTrack(StatusData As BaseFluxForm.TrackStatusData) Implements ITrackStatus.UpdateGridTrack
        Friend Event UpdateStatus(StatusData As BaseFluxForm.TrackStatusData) Implements ITrackStatus.UpdateStatus
        Friend Event UpdateStatusType(StatusText As String) Implements ITrackStatus.UpdateStatusType

        Public Sub New()
            MyBase.New

            _StatusCollection = New Dictionary(Of String, TrackStatusInfo)
        End Sub

        Public ReadOnly Property Failed As Boolean Implements ITrackStatus.Failed
            Get
                Return _Failed
            End Get
        End Property

        Public ReadOnly Property TrackFound As Boolean Implements ITrackStatus.TrackFound
            Get
                Return _TrackFound
            End Get
        End Property

        Public Function CanKeepProcessing() As Boolean Implements ITrackStatus.CanKeepProcessing
            If _CurrentStatusInfo Is Nothing Then
                Return False
            End If

            If Not _Range.HasValue Then
                Return False
            End If

            If Not _CurrentStatusInfo.Failed Then
                Return False
            End If

            If Not (_CurrentStatusInfo.Track < _Range.Value.TrackEnd Or _CurrentStatusInfo.Side < _Range.value.HeadEnd) Then
                Return False
            End If

            Return True
        End Function

        Public Sub Clear() Implements ITrackStatus.Clear
            _StatusCollection.Clear()
            _CurrentStatusInfo = Nothing
            _TotalBadSectors = 0
            _TotalUnexpectedSectors = 0
            _Range = Nothing
            _Failed = False
        End Sub

        Public Sub UpdateTrackStatusAborted() Implements ITrackStatus.UpdateTrackStatusAborted
            UpdateTrackStatusType(TrackStatusEnum.Aborted)
        End Sub

        Public Sub UpdateTrackStatusComplete(DoubleStep As Boolean, Optional KeepProcessing As Boolean = False) Implements ITrackStatus.UpdateTrackStatusComplete
            SetCurrentTrackStatusComplete(DoubleStep)

            If KeepProcessing Then
                Exit Sub
            End If

            UpdateTrackStatusType(TrackStatusEnum.Success)
        End Sub

        Public Sub UpdateTrackStatusError() Implements ITrackStatus.UpdateTrackStatusError
            UpdateTrackStatusType(TrackStatusEnum.Error)
        End Sub

        Friend Function GetNextTrackRange() As (Ranges As List(Of (StartTrack As UShort, EndTrack As UShort)), Heads As TrackHeads, [Continue] As Boolean) Implements ITrackStatus.GetNextTrackRange
            Dim Response As (Ranges As List(Of (StartTrack As UShort, EndTrack As UShort)), Heads As TrackHeads, [Continue] As Boolean)

            Response.Ranges = New List(Of (StartTrack As UShort, EndTrack As UShort))

            If Not _Range.HasValue Then
                Response.Continue = False
                Response.Heads = TrackHeads.both

            ElseIf _CurrentStatusInfo.Side < _Range.Value.HeadEnd Then
                Response.Continue = True
                Response.Heads = GetTrackHeads(_CurrentStatusInfo.Side + 1)
                Response.Ranges.Add((_CurrentStatusInfo.Track, _CurrentStatusInfo.Track))

            Else
                Response.Continue = False
                Response.Heads = TrackHeads.both
                Response.Ranges.Add((_CurrentStatusInfo.Track + 1, _Range.Value.TrackEnd))
            End If

            Return Response
        End Function

        Friend Sub ProcessOutputLineRead(line As String, InfoAction As ActionTypeEnum, DoubleStep As Boolean) Implements ITrackStatus.ProcessOutputLineRead
            Dim Response = ParseCommandFailed(line)
            If Response IsNot Nothing Then
                _Failed = True
                UpdateTrackStatusError()
                Return
            End If

            If Not _Range.HasValue Then
                _Range = ParseDiskRange(line)
            End If

            Dim Summary = ParseTrackReadSummary(line)

            If Summary.HasValue Then
                Dim Details = ParseTrackReadDetails(Summary.Value.Details)

                If Details.HasValue Then
                    Dim ProcessBadSectors = (InfoAction = ActionTypeEnum.Import)
                    UpdateStatusTrackRead(Summary.Value, Details.Value, ProcessBadSectors, InfoAction, ActionTypeEnum.Read, DoubleStep)
                    Return
                End If

                If InfoAction = ActionTypeEnum.Read Then
                    Dim FailedSectors = ParseTrackReadFailed(Summary.Value.Details)
                    If FailedSectors.HasValue Then
                        UpdateStatusFailedSectors(Summary.Value, FailedSectors.Value, InfoAction, ActionTypeEnum.Read, DoubleStep)
                        Return
                    End If

                    Dim OutOfRange = ParseTrackReadOutOfRange(Summary.Value.Details)

                    If OutOfRange.HasValue Then
                        UpdateStatusOutOfRange(Summary.Value, OutOfRange.Value, InfoAction, ActionTypeEnum.Read, DoubleStep)
                        Return
                    End If
                End If
            End If

            Dim TrackInfoUnexpected = ParseTrackUnexpected(line)

            If TrackInfoUnexpected.HasValue Then
                UpdateStatusUnexpectedSector(TrackInfoUnexpected.Value, InfoAction, ActionTypeEnum.Read, DoubleStep)
                Return
            End If

            Dim SectorGridLine = ParseSectorGridLine(line)
            If SectorGridLine.HasValue Then
                UpdateStatusSectorGridLine(SectorGridLine.Value.Side, SectorGridLine.Value.Sector, SectorGridLine.Value.Tracks)
                Return
            End If

            Dim SectorGridEnd = ParseSectorGridEnd(line)
            If SectorGridEnd.HasValue Then
                UpdateStatusSectorGridEnd(SectorGridEnd.Value.FoundSectors, SectorGridEnd.Value.TotalSectors, SectorGridEnd.Value.Percentage)
                Return
            End If
        End Sub

        Friend Sub ProcessOutputLineWrite(line As String, InfoAction As ActionTypeEnum, DoubleStep As Boolean) Implements ITrackStatus.ProcessOutputLineWrite
            Dim Response = ParseCommandFailed(line)
            If Response IsNot Nothing Then
                _Failed = True
                UpdateTrackStatusError()
                Return
            End If

            If Not _Range.HasValue Then
                _Range = ParseDiskRange(line)
            End If

            Dim TrackInfo = ParseTrackWrite(line)

            If TrackInfo.HasValue Then
                Dim Action As ActionTypeEnum
                If TrackInfo.Value.Action = "Erasing" Then
                    Action = ActionTypeEnum.Erase
                ElseIf TrackInfo.Value.Action = "Writing" Then
                    Action = ActionTypeEnum.Write
                Else
                    Action = ActionTypeEnum.Unknown
                End If

                UpdateStatusTrackWrite(TrackInfo.Value, InfoAction, Action, DoubleStep)
                Return
            End If
        End Sub

        Private Shared Function GetTrackStatus(Statusinfo As TrackStatusInfo, Action As ActionTypeEnum) As (Status As TrackStatusEnum, Label As String, StatusColor As (ForeColor As Color, BackColor As Color))
            Dim Status As TrackStatusEnum
            Dim Label As String = ""
            Dim StatusColor? As (ForeColor As Color, BackColor As Color) = Nothing

            If Statusinfo.Failed Then
                Status = TrackStatusEnum.Failed
                If Statusinfo.Action = ActionTypeEnum.Read Or Statusinfo.Action = ActionTypeEnum.Import Then
                    Label = Statusinfo.BadSectors
                Else
                    Label = Statusinfo.Retries
                End If

            ElseIf Action = ActionTypeEnum.Complete Then
                If Statusinfo.UnexpectedSectors.Count > 0 Then
                    Status = TrackStatusEnum.Success
                    Label = "U"
                    StatusColor = GetTrackStatusColor(TrackStatusEnum.Unexpected, Statusinfo)
                ElseIf Statusinfo.Retries > 0 Then
                    Status = TrackStatusEnum.Retry
                    Label = Statusinfo.Retries
                Else
                    Status = TrackStatusEnum.Success
                    Label = ""
                End If

            ElseIf Statusinfo.Retries > 0 Then
                Status = TrackStatusEnum.Retry
                Label = Statusinfo.Retries

            ElseIf Action = ActionTypeEnum.Erase Then
                Status = TrackStatusEnum.Erasing
                Label = "E"

            ElseIf Action = ActionTypeEnum.Write Then
                Status = TrackStatusEnum.Writing
                Label = "W"

            ElseIf Action = ActionTypeEnum.Read Then
                Status = TrackStatusEnum.Reading
                Label = "R"
            End If

            If Not StatusColor.HasValue Then
                StatusColor = GetTrackStatusColor(Status, Statusinfo)
            End If


            Return (Status, Label, StatusColor.Value)
        End Function

        Private Shared Function GetTrackStatusColor(Status As TrackStatusEnum, StatusInfo As TrackStatusInfo) As (ForeColor As Color, BackColor As Color)
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
                    If StatusInfo.OutOfRange Then
                        Return (Color.Black, Color.FromArgb(225, 225, 225))
                    Else
                        Return (Color.Black, Color.LightGreen)
                    End If
                Case TrackStatusEnum.Unexpected
                    Return (Color.Black, Color.Yellow)
                Case Else
                    Return (Color.Black, Color.Gray)
            End Select
        End Function

        Private Shared Function GetTrackStatusText(Status As TrackStatusEnum, Optional Retries As UShort = 0) As String
            Select Case Status
                Case TrackStatusEnum.Reading
                    Return My.Resources.Label_Reading
                Case TrackStatusEnum.Erasing
                    Return My.Resources.Label_Erasing
                Case TrackStatusEnum.Writing
                    Return My.Resources.Label_Writing
                Case TrackStatusEnum.Retry
                    Return My.Resources.Label_Retrying & If(Retries > 0, " " & Retries, "")
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

        Private Function BuildTooltip(StatusInfo As TrackStatusInfo) As String
            Dim Tooltip As New List(Of String)

            If StatusInfo.Retries > 0 Then
                Tooltip.Add(My.Resources.Label_Retries & ":  " & StatusInfo.Retries)
            End If

            Dim Sectors As New List(Of String)

            If StatusInfo.BadSectors > 0 Then
                Sectors.Add(StatusInfo.BadSectors & " " & My.Resources.Label_Bad)
            End If

            If Sectors.Count > 0 Then
                Tooltip.Add(My.Resources.Label_Sectors & ":  " & String.Join(", ", Sectors))
            End If

            If StatusInfo.BadSectorList.Count > 0 Then
                Dim RangeList = UShortListToRanges(StatusInfo.BadSectorList)
                Tooltip.Add(My.Resources.Label_BadSectorIds & ":  " & RangeList)
            End If

            If StatusInfo.UnexpectedSectors.Count > 0 Then
                Tooltip.Add(My.Resources.Label_UnexpectedSectors & ":")
                For Each Value In StatusInfo.UnexpectedSectors.Values.OrderBy(Function(v) v.SectorId).ToList()
                    Tooltip.Add(String.Format("C:{0} H:{1} R:{2} N:{3} ({4})", Value.Cylinder, Value.Head, Value.SectorId, Value.SizeId, 128 * 2 ^ Value.SizeId))
                Next
            End If

            Return String.Join(vbNewLine, Tooltip)
        End Function

        Private Function GetCurrentTrackStatusData(Action As ActionTypeEnum, DoubleStep As Boolean) As BaseFluxForm.TrackStatusData
            Dim Data = GetTrackStatus(_CurrentStatusInfo, Action)

            Dim TooltipText As String = ""


            Dim StatusData As BaseFluxForm.TrackStatusData
            With StatusData
                .StatusText = GetTrackStatusText(Data.Status, _CurrentStatusInfo.Retries)
                .CellText = Data.Label
                .Track = _CurrentStatusInfo.Track
                .Side = _CurrentStatusInfo.Side
                .SideVisible = True
                .TotalBadSectors = _TotalBadSectors
                .TotalUnexpectedSectors = _TotalUnexpectedSectors
                .ForeColor = Data.StatusColor.ForeColor
                .BackColor = Data.StatusColor.BackColor
                .Tooltip = BuildTooltip(_CurrentStatusInfo)
            End With

            If DoubleStep Then
                StatusData.Track *= 2
            End If

            Return StatusData
        End Function

        Private Function GetStatusInfo(Track As Integer, Side As Integer) As TrackStatusInfo
            Dim Key = Track & "." & Side
            Dim StatusInfo As TrackStatusInfo

            If _StatusCollection.ContainsKey(Key) Then
                StatusInfo = _StatusCollection.Item(Key)
            Else
                StatusInfo = New TrackStatusInfo With {
                    .Track = Track,
                    .Side = Side
                }
                _StatusCollection.Add(Key, StatusInfo)
            End If

            Return StatusInfo
        End Function

        Private Sub SetCurrentTrackStatusComplete(DoubleStep As Boolean)
            If _CurrentStatusInfo IsNot Nothing Then
                Dim StatusData = GetCurrentTrackStatusData(ActionTypeEnum.Complete, DoubleStep)

                RaiseEvent UpdateGridTrack(StatusData)
            End If
        End Sub

        Private Sub UpdateStatusFailedSectors(Summary As TrackReadSummary, FailedSectors As Integer, InfoAction As ActionTypeEnum, Action As ActionTypeEnum, DoubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(Summary.DestTrack, Summary.DestSide)

            StatusInfo.Action = InfoAction

            StatusInfo.Failed = True
            StatusInfo.BadSectors = FailedSectors
            _TotalBadSectors += FailedSectors

            UpdateTrackStatus(StatusInfo, Action, DoubleStep)
        End Sub

        Private Sub UpdateStatusOutOfRange(Summary As TrackReadSummary, OutOfRage As TrackReadOutOfRange, InfoAction As ActionTypeEnum, Action As ActionTypeEnum, DoubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(Summary.DestTrack, Summary.DestSide)

            StatusInfo.Action = InfoAction

            StatusInfo.OutOfRange = True
            StatusInfo.OutOfRangeFormat = OutOfRage.Format

            UpdateTrackStatus(StatusInfo, Action, DoubleStep)
        End Sub

        Private Sub UpdateStatusSectorGridEnd(FoundSectors As Integer, TotalSectors As Integer, Percentage As Integer)
            For Each StatusInfo In _StatusCollection.Values
                If StatusInfo.BadSectorList.Count > 0 Then
                    Dim Tooltip = BuildTooltip(StatusInfo)
                    RaiseEvent UpdateGridTooltip(StatusInfo.Track, StatusInfo.Side, Tooltip)
                End If
            Next
        End Sub

        Private Sub UpdateStatusSectorGridLine(Side As Integer, Sector As Integer, Tracks As List(Of Boolean))
            For Track = 0 To Tracks.Count - 1
                Dim Key = Track & "." & Side

                If _StatusCollection.ContainsKey(Key) Then
                    Dim StatusInfo = _StatusCollection.Item(Key)
                    If Tracks(Track) Then
                        StatusInfo.BadSectorList.Add(Sector + 1)
                    End If
                End If
            Next
        End Sub

        Private Sub UpdateStatusTrackRead(Summary As TrackReadSummary, Details As TrackReadDetails, ProcessBadSectors As Boolean, InfoAction As ActionTypeEnum, Action As ActionTypeEnum, DoubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(Summary.DestTrack, Summary.DestSide)

            StatusInfo.Action = InfoAction

            If Details.Seek > 0 And Details.Retry > 0 Then
                StatusInfo.Retries += 1
            End If

            If ProcessBadSectors Then
                StatusInfo.BadSectors += Details.BadSectors
                _TotalBadSectors += Details.BadSectors
                If Details.BadSectors > 0 Then
                    StatusInfo.Failed = True
                End If
            End If

            UpdateTrackStatus(StatusInfo, Action, DoubleStep)
        End Sub

        Private Sub UpdateStatusTrackWrite(TrackInfo As TrackWrite, InfoAction As ActionTypeEnum, Action As ActionTypeEnum, DoubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(TrackInfo.SrcTrack, TrackInfo.SrcSide)

            StatusInfo.Action = InfoAction
            StatusInfo.Retries = Math.Max(StatusInfo.Retries, TrackInfo.Retry)
            If TrackInfo.Failed Then
                StatusInfo.Failed = TrackInfo.Failed
            End If

            UpdateTrackStatus(StatusInfo, Action, DoubleStep)
        End Sub

        Private Sub UpdateStatusUnexpectedSector(TrackInfo As UnexpectedSector, InfoAction As ActionTypeEnum, Action As ActionTypeEnum, DoubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(TrackInfo.Track, TrackInfo.Side)

            StatusInfo.Action = InfoAction

            If Not StatusInfo.UnexpectedSectors.ContainsKey(TrackInfo.Key) Then
                StatusInfo.UnexpectedSectors.Add(TrackInfo.Key, TrackInfo)
                _TotalUnexpectedSectors += 1
            End If

            UpdateTrackStatus(StatusInfo, Action, DoubleStep)
        End Sub

        Private Sub UpdateTrackStatus(Statusinfo As TrackStatusInfo, Action As ActionTypeEnum, DoubleStep As Boolean)
            SetCurrentTrackStatusComplete(DoubleStep)

            _CurrentStatusInfo = Statusinfo

            Dim StatusData = GetCurrentTrackStatusData(Action, DoubleStep)

            RaiseEvent UpdateStatus(StatusData)
        End Sub

        Private Sub UpdateTrackStatusType(Status As TrackStatusEnum)
            RaiseEvent UpdateStatusType(GetTrackStatusText(Status))
        End Sub

        Private Class TrackStatusInfo
            Public Sub New()
                _UnexpectedSectors = New Dictionary(Of String, ConsoleParser.UnexpectedSector)
                _BadSectorList = New List(Of UShort)
            End Sub

            Public Property Action As ActionTypeEnum
            Public Property BadSectorList As List(Of UShort)
            Public Property BadSectors As UShort = 0
            Public Property Failed As Boolean
            Public Property OutOfRange As Boolean = False
            Public Property OutOfRangeFormat As String = ""
            Public Property Retries As UShort = 0
            Public Property Side As Integer
            Public Property Track As Integer
            Public Property UnexpectedSectors As Dictionary(Of String, ConsoleParser.UnexpectedSector)
        End Class
    End Class
End Namespace
