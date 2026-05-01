Imports Greaseweazle.Actions

Namespace Flux.Greaseweazle
    Public Class TrackStatus
        Implements ITrackStatus

        Private ReadOnly _StatusCollection As New Dictionary(Of String, TrackStatusInfo)

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

        Public ReadOnly Property TrackFound As Boolean Implements ITrackStatus.TrackFound
            Get
                Return True
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

            If Not (_CurrentStatusInfo.Track < _Range.TrackEnd OrElse _CurrentStatusInfo.Side < _Range.HeadEnd) Then
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

        Public Sub OnConvertTrackProcessed(args As TrackProcessedEventArgs, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(args.Track.Cyl, args.Track.Head)

            Select Case args.Outcome
                Case TrackDecodeOutcome.OutOfRange
                    StatusInfo.OutOfRange = True
                    StatusInfo.OutOfRangeFormat = If(args.FormatName, "")

                Case TrackDecodeOutcome.Decoded
                    If args.DecodedSectorsTotal > 0 Then
                        Dim Bad As UShort = CUShort(Math.Max(0, args.DecodedSectorsTotal - args.DecodedSectorsFound))
                        StatusInfo.BadSectors += Bad
                        _TotalBadSectors += CUInt(Bad)
                        If Bad > 0 Then
                            StatusInfo.Failed = True
                        End If
                    End If
            End Select

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read, doubleStep)
        End Sub

        Public Sub OnConvertUnexpectedSector(args As UnexpectedSectorEventArgs, doubleStep As Boolean)
            UpdateStatusUnexpectedSector(args, ActionTypeEnum.Read, doubleStep)
        End Sub

        Public Sub OnEraseTrack(args As EraseTrackEventArgs)
            Dim StatusInfo = GetStatusInfo(args.Cyl, args.Head)

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Erase, DoubleStep:=False)
        End Sub

        Public Sub OnReadTrackGaveUp(args As ReadTrackGaveUpEventArgs, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(args.Track)

            StatusInfo.Failed = True

            Dim Bad As UShort = CUShort(Math.Max(0, args.MissingSectors))
            StatusInfo.BadSectors += Bad

            _TotalBadSectors += CUInt(Bad)

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read, doubleStep)
        End Sub

        Public Sub OnReadTrackProcessed(args As TrackProcessedEventArgs, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(args.Track)

            If args.SeekRetry > 0 AndAlso args.Retry > 0 Then
                StatusInfo.Retries += 1
            End If

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read, doubleStep)
        End Sub

        Public Sub OnReadUnexpectedSector(args As UnexpectedSectorEventArgs, doubleStep As Boolean)
            UpdateStatusUnexpectedSector(args, ActionTypeEnum.Read, doubleStep)
        End Sub

        Public Sub OnSummary(grid As SectorSummaryGrid)
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

                    Dim StatusInfo As TrackStatusInfo = Nothing
                    If _StatusCollection.TryGetValue(Key, StatusInfo) AndAlso Row.Cells(i) = SectorSummaryCell.Bad Then
                        StatusInfo.BadSectorList.Add(CUShort(Sector + 1))
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

        Public Sub OnWriteTrackErasing(args As WriteTrackErasingEventArgs, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(args.Track)

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Erase, doubleStep)
        End Sub

        Public Sub OnWriteTrackOutOfRange(args As WriteTrackOutOfRangeEventArgs, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(args.Track)

            StatusInfo.OutOfRange = True
            StatusInfo.OutOfRangeFormat = If(args.FormatName, "")

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Write, doubleStep)
        End Sub

        Public Sub OnWriteTrackWriting(args As WriteTrackWritingEventArgs, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(args.Track)

            If args.RetryNumber > 0 Then
                StatusInfo.Retries = CUShort(Math.Max(CInt(StatusInfo.Retries), args.RetryNumber))
            End If

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Write, doubleStep)
        End Sub

        Public Sub OnWriteVerifyFailed(cyl As Integer, head As Integer, doubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(cyl, head)

            StatusInfo.Failed = True

            UpdateTrackStatus(StatusInfo, ActionTypeEnum.Write, doubleStep)
        End Sub

        Public Sub UpdateTrackStatusAborted() Implements ITrackStatus.UpdateTrackStatusAborted
            If _Failed Then
                Exit Sub
            End If

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
            _Failed = True
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

        Friend Sub ProcessOutputLineRead(line As String, DoubleStep As Boolean) Implements ITrackStatus.ProcessOutputLineRead
            'Unused
        End Sub

        Private Shared Function BuildTooltip(StatusInfo As TrackStatusInfo) As String
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
                For Each Value In StatusInfo.UnexpectedSectors.Values.OrderBy(Function(v) v.R).ToList()
                    Tooltip.Add(String.Format("C:{0} H:{1} R:{2} N:{3} ({4})", Value.C, Value.H, Value.R, Value.N, 128 * 2 ^ Value.N))
                Next
            End If

            Return String.Join(vbNewLine, Tooltip)
        End Function

        Private Shared Function GetTrackStatus(Statusinfo As TrackStatusInfo, Action As ActionTypeEnum) As (Status As TrackStatusEnum, Label As String, StatusColor As (ForeColor As Color, BackColor As Color))
            Dim Status As TrackStatusEnum
            Dim Label As String = ""
            Dim StatusColor? As (ForeColor As Color, BackColor As Color) = Nothing

            If Statusinfo.Failed Then
                Status = TrackStatusEnum.Failed
                If Statusinfo.IsReadStatus Then
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
        Private Shared Function GetUnexpectedSectorKey(args As UnexpectedSectorEventArgs) As String
            Return args.C & "." & args.H & "." & args.R & "." & args.N
        End Function

        Private Function GetCurrentTrackStatusData(Action As ActionTypeEnum, DoubleStep As Boolean) As BaseFluxForm.TrackStatusData
            Dim Data = GetTrackStatus(_CurrentStatusInfo, Action)

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

        Private Function GetStatusInfo(Track As TrackInfo) As TrackStatusInfo
            Return GetStatusInfo(Track.Cyl, Track.Head)
        End Function

        Private Function GetStatusInfo(Track As Integer, Side As Integer) As TrackStatusInfo
            Dim Key = Track & "." & Side

            Dim StatusInfo As TrackStatusInfo = Nothing
            If Not _StatusCollection.TryGetValue(Key, StatusInfo) Then
                StatusInfo = New TrackStatusInfo With {.Track = Track, .Side = Side}
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

        Private Sub UpdateStatusUnexpectedSector(args As UnexpectedSectorEventArgs, Action As ActionTypeEnum, DoubleStep As Boolean)
            Dim StatusInfo = GetStatusInfo(args.Track)

            Dim Key = GetUnexpectedSectorKey(args)

            If Not StatusInfo.UnexpectedSectors.ContainsKey(Key) Then
                StatusInfo.UnexpectedSectors.Add(Key, args)
                _TotalUnexpectedSectors += 1
            End If

            UpdateTrackStatus(StatusInfo, Action, DoubleStep)
        End Sub

        Private Sub UpdateTrackStatus(Statusinfo As TrackStatusInfo, Action As ActionTypeEnum, DoubleStep As Boolean)
            SetCurrentTrackStatusComplete(DoubleStep)

            Statusinfo.IsReadStatus = (Action = ActionTypeEnum.Read)

            _CurrentStatusInfo = Statusinfo

            Dim StatusData = GetCurrentTrackStatusData(Action, DoubleStep)

            RaiseEvent UpdateStatus(StatusData)
        End Sub

        Private Sub UpdateTrackStatusType(Status As TrackStatusEnum)
            RaiseEvent UpdateStatusType(GetTrackStatusText(Status))
        End Sub

        Private Class TrackStatusInfo
            Public Property BadSectorList As New List(Of UShort)
            Public Property BadSectors As UShort = 0
            Public Property Failed As Boolean
            Public Property OutOfRange As Boolean = False
            Public Property OutOfRangeFormat As String = ""
            Public Property IsReadStatus As Boolean
            Public Property Retries As UShort = 0
            Public Property Side As Integer
            Public Property Track As Integer
            Public Property UnexpectedSectors As New Dictionary(Of String, UnexpectedSectorEventArgs)
        End Class
    End Class
End Namespace
