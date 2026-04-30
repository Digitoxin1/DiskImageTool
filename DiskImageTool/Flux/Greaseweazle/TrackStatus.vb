Imports Greaseweazle.Actions

Namespace Flux.Greaseweazle
    Public Class TrackStatus
        Implements ITrackStatus

        Private ReadOnly _StatusCollection As Dictionary(Of String, TrackStatusInfo)
        Private ReadOnly _TrackFound As Boolean = True

        Private _CurrentStatusInfo As TrackStatusInfo = Nothing
        Private _Failed As Boolean = False
        Private _Range As TrackRange = Nothing
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

            If _Range Is Nothing Then
                Return False
            End If

            If Not _CurrentStatusInfo.Failed Then
                Return False
            End If

            If Not (_CurrentStatusInfo.Track < _Range.TrackEnd Or _CurrentStatusInfo.Side < _Range.HeadEnd) Then
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

        Public Sub OnConvertSummary(grid As SectorSummaryGrid)
            OnReadSummary(grid)
        End Sub

        Public Sub OnConvertTrackProcessed(cyl As Integer, head As Integer,
                                           outcome As ConvertTrackOutcome,
                                           decodedSectorsFound As Integer,
                                           decodedSectorsTotal As Integer,
                                           formatName As String,
                                           doubleStep As Boolean)

            Dim StatusInfo = GetStatusInfo(cyl, head)

            Select Case outcome
                Case ConvertTrackOutcome.OutOfRange
                    StatusInfo.Action = ActionTypeEnum.Read
                    StatusInfo.OutOfRange = True
                    StatusInfo.OutOfRangeFormat = If(formatName, "")

                Case ConvertTrackOutcome.Decoded
                    StatusInfo.Action = ActionTypeEnum.Read
                    If decodedSectorsTotal > 0 Then
                        Dim Bad As UShort = CUShort(Math.Max(0, decodedSectorsTotal - decodedSectorsFound))
                        StatusInfo.BadSectors += Bad
                        _TotalBadSectors += CUInt(Bad)
                        If Bad > 0 Then
                            StatusInfo.Failed = True
                        End If
                    End If

                Case Else
                    StatusInfo.Action = ActionTypeEnum.Read
            End Select

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read, doubleStep)
        End Sub

        Public Sub OnConvertUnexpectedSector(cyl As Integer, head As Integer, c As Integer, h As Integer, r As Integer, n As Integer, doubleStep As Boolean)
            Dim Info = New UnexpectedSector With {
                .Track = cyl,
                .Side = head,
                .Cylinder = c,
                .Head = h,
                .SectorId = r,
                .SizeId = n
            }

            UpdateStatusUnexpectedSector(Info, ActionTypeEnum.Import, ActionTypeEnum.Read, doubleStep)
        End Sub
        Public Sub OnEraseFailed()
            _Failed = True
            UpdateTrackStatusError()
        End Sub

        Public Sub OnEraseTrack(cyl As Integer, head As Integer)
            Dim StatusInfo = GetStatusInfo(cyl, head)
            StatusInfo.Action = ActionTypeEnum.Erase
            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Erase, DoubleStep:=False)
        End Sub

        Public Sub OnReadFailed()
            _Failed = True

            UpdateTrackStatusError()
        End Sub

        Public Sub OnReadSummary(grid As SectorSummaryGrid)
            If grid Is Nothing OrElse grid.Rows Is Nothing OrElse grid.Cyls Is Nothing Then
                Exit Sub
            End If

            For Each Row In grid.Rows
                Dim Side = Row.Head
                Dim Sector = Row.Sector

                For i = 0 To Row.Cells.Count - 1
                    If i >= grid.Cyls.Count Then
                        Exit For
                    End If

                    Dim Cyl = grid.Cyls(i)
                    Dim Key = Cyl & "." & Side

                    If _StatusCollection.ContainsKey(Key) Then
                        Dim StatusInfo = _StatusCollection.Item(Key)

                        If Row.Cells(i) = SectorSummaryCell.Bad Then
                            StatusInfo.BadSectorList.Add(CUShort(Sector + 1))
                        End If
                    End If
                Next
            Next

            For Each StatusInfo In _StatusCollection.Values
                If StatusInfo.BadSectorList.Count > 0 Then
                    Dim Tooltip = BuildTooltip(StatusInfo)

                    RaiseEvent UpdateGridTooltip(StatusInfo.Track, StatusInfo.Side, Tooltip)
                End If
            Next
        End Sub

        Public Sub OnReadTrackGaveUp(cyl As Integer, head As Integer, missingSectors As Integer, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(cyl, head)

            StatusInfo.Action = ActionTypeEnum.Read

            StatusInfo.Failed = True

            Dim Bad As UShort = CUShort(Math.Max(0, missingSectors))
            StatusInfo.BadSectors += Bad

            _TotalBadSectors += CUInt(Bad)

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read, doubleStep)
        End Sub

        Public Sub OnReadTrackProcessed(cyl As Integer, head As Integer, seekRetry As Integer, retry As Integer, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(cyl, head)

            StatusInfo.Action = ActionTypeEnum.Read

            If seekRetry > 0 And retry > 0 Then
                StatusInfo.Retries += CUShort(1)
            End If

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read, doubleStep)
        End Sub

        Public Sub OnReadUnexpectedSector(cyl As Integer, head As Integer, c As Integer, h As Integer, r As Integer, n As Integer, doubleStep As Boolean)
            Dim Info = New UnexpectedSector With {
                .Track = cyl,
                .Side = head,
                .Cylinder = c,
                .Head = h,
                .SectorId = r,
                .SizeId = n
            }

            UpdateStatusUnexpectedSector(Info, ActionTypeEnum.Read, ActionTypeEnum.Read, doubleStep)
        End Sub

        Public Sub OnWriteFailed()
            _Failed = True
            UpdateTrackStatusError()
        End Sub

        Public Sub OnWriteTrackErasing(cyl As Integer, head As Integer, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(cyl, head)

            StatusInfo.Action = ActionTypeEnum.Erase

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Erase, doubleStep)
        End Sub

        Public Sub OnWriteTrackOutOfRange(cyl As Integer, head As Integer, formatName As String, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(cyl, head)

            StatusInfo.Action = ActionTypeEnum.Write

            StatusInfo.OutOfRange = True
            StatusInfo.OutOfRangeFormat = If(formatName, "")

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Write, doubleStep)
        End Sub

        Public Sub OnWriteTrackWriting(cyl As Integer, head As Integer, retryNumber As Integer, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(cyl, head)

            StatusInfo.Action = ActionTypeEnum.Write

            If retryNumber > 0 Then
                StatusInfo.Retries = CUShort(Math.Max(CInt(StatusInfo.Retries), retryNumber))
            End If

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Write, doubleStep)
        End Sub

        Public Sub OnWriteVerifyFailed(cyl As Integer, head As Integer, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(cyl, head)

            StatusInfo.Action = ActionTypeEnum.Write
            StatusInfo.Failed = True

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Write, doubleStep)
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

            If _Range Is Nothing Then
                Response.Continue = False
                Response.Heads = TrackHeads.both

            ElseIf _CurrentStatusInfo.Side < _Range.HeadEnd Then
                Response.Continue = True
                Response.Heads = GetTrackHeads(_CurrentStatusInfo.Side + 1)
                Response.Ranges.Add((_CurrentStatusInfo.Track, _CurrentStatusInfo.Track))

            Else
                Response.Continue = False
                Response.Heads = TrackHeads.both
                Response.Ranges.Add((_CurrentStatusInfo.Track + 1, _Range.TrackEnd))
            End If

            Return Response
        End Function

        Friend Sub OnWriteStarted(trackRanges As List(Of (StartTrack As UShort, EndTrack As UShort)), heads As TrackHeads)
            If _Range IsNot Nothing Then
                Exit Sub
            End If

            If trackRanges Is Nothing OrElse trackRanges.Count = 0 Then
                Exit Sub
            End If

            Dim TrackStart As Integer = trackRanges(0).StartTrack
            Dim TrackEnd As Integer = trackRanges(0).EndTrack

            For Each R In trackRanges
                If R.StartTrack < TrackStart Then TrackStart = R.StartTrack
                If R.EndTrack > TrackEnd Then TrackEnd = R.EndTrack
            Next

            Dim HeadStart As Integer = If(heads = TrackHeads.head1, 1, 0)
            Dim HeadEnd As Integer = If(heads = TrackHeads.head0, 0, 1)

            _Range = New TrackRange With {
                .Action = "Writing",
                .TrackStart = TrackStart,
                .TrackEnd = TrackEnd,
                .HeadStart = HeadStart,
                .HeadEnd = HeadEnd
            }
        End Sub

        Friend Sub ProcessOutputLineRead(line As String, InfoAction As ActionTypeEnum, DoubleStep As Boolean) Implements ITrackStatus.ProcessOutputLineRead
            'Unused
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
                _UnexpectedSectors = New Dictionary(Of String, UnexpectedSector)
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
            Public Property UnexpectedSectors As Dictionary(Of String, UnexpectedSector)
        End Class
    End Class
End Namespace
