Namespace Flux.PcImgCnv
    Public Class TrackStatus
        Inherits ConsoleParser
        Implements ITrackStatus

        Private ReadOnly _Failed As Boolean = False
        Private ReadOnly _StatusCollection As Dictionary(Of String, TrackStatusInfo)
        Private _CurrentStatusInfo As TrackStatusInfo = Nothing
        Private _TotalBadSectors As UInteger = 0
        Private _TrackFound As Boolean = False

        Private Enum TrackStatusEnum
            Reading
            Failed
            Success
            Aborted
            [Error]
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
            Return False
        End Function

        Public Sub Clear() Implements ITrackStatus.Clear
            _StatusCollection.Clear()
            _CurrentStatusInfo = Nothing
            _TotalBadSectors = 0
            _TrackFound = False
        End Sub

        Public Sub UpdateTrackStatusAborted() Implements ITrackStatus.UpdateTrackStatusAborted
            UpdateTrackStatusType(TrackStatusEnum.Aborted)
        End Sub

        Public Sub UpdateTrackStatusComplete(DoubleStep As Boolean, Optional KeepProcessing As Boolean = False) Implements ITrackStatus.UpdateTrackStatusComplete
            SetCurrentTrackStatusComplete()
            UpdateTrackStatusType(TrackStatusEnum.Success)
        End Sub

        Public Sub UpdateTrackStatusError() Implements ITrackStatus.UpdateTrackStatusError
            UpdateTrackStatusType(TrackStatusEnum.Error)
        End Sub

        Friend Function GetNextTrackRange() As (Ranges As List(Of (StartTrack As UShort, EndTrack As UShort)), Heads As TrackHeads, [Continue] As Boolean) Implements ITrackStatus.GetNextTrackRange
            Dim Response As (Ranges As List(Of (StartTrack As UShort, EndTrack As UShort)), Heads As TrackHeads, [Continue] As Boolean)

            Response.Ranges = New List(Of (StartTrack As UShort, EndTrack As UShort))
            Response.Heads = TrackHeads.both
            Response.[Continue] = False

            Return Response
        End Function

        Friend Sub ProcessOutputLineRead(line As String, InfoAction As ActionTypeEnum, DoubleStep As Boolean) Implements ITrackStatus.ProcessOutputLineRead
            Dim RawSector = ParseRawSector(line)

            If RawSector.HasValue Then
                If RawSector.Value.Mark = "FE" Then
                    Dim SectorData = ParseRawSectorData(RawSector.Value.Message)
                    If SectorData.HasValue Then
                        _TrackFound = True
                        Dim StatusInfo = UpdateStatusInfo(RawSector.Value, SectorData)
                        UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read)
                    End If

                ElseIf RawSector.Value.Mark = "FB" AndAlso _CurrentStatusInfo IsNot Nothing Then
                    Dim Message = ParseRawMessage(RawSector.Value.Message)
                    If String.IsNullOrEmpty(Message) Then
                        'Do Nothing
                    ElseIf Message.ToLower = "crc bad" Then
                        _CurrentStatusInfo.BadSectorList.Add(_CurrentStatusInfo.SectorId)
                    End If
                End If

                Exit Sub
            End If

            Dim RemasteredSector = ParseRemasteredSector(line)
            If RemasteredSector.HasValue Then
                _TrackFound = True
                Dim StatusInfo = UpdateStatusInfo(RemasteredSector.Value)
                UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read)
            End If
        End Sub

        Friend Sub ProcessOutputLineWrite(line As String, InfoAction As ActionTypeEnum, DoubleStep As Boolean) Implements ITrackStatus.ProcessOutputLineWrite
            Exit Sub
        End Sub

        Private Shared Function GetTrackStatusText(Status As TrackStatusEnum) As String
            Select Case Status
                Case TrackStatusEnum.Reading
                    Return My.Resources.Label_Reading
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

            Dim Sectors As New List(Of String)

            If StatusInfo.BadSectorList.Count > 0 Then
                Sectors.Add(StatusInfo.BadSectorList.Count & " " & My.Resources.Label_Bad)
            End If

            If Sectors.Count > 0 Then
                Tooltip.Add(My.Resources.Label_Sectors & ":  " & String.Join(", ", Sectors))
            End If

            If StatusInfo.BadSectorList.Count > 0 Then
                Dim RangeList = UShortListToRanges(StatusInfo.BadSectorList)
                Tooltip.Add(My.Resources.Label_BadSectorIds & ":  " & RangeList)
            End If

            If Not String.IsNullOrEmpty(StatusInfo.Format) Then
                Tooltip.Add(My.Resources.Label_Format & ":  " & StatusInfo.Format)
            End If

            If StatusInfo.Modified Then
                Tooltip.Add(My.Resources.Label_Modified)
            End If

            If StatusInfo.Messages IsNot Nothing AndAlso StatusInfo.Messages.Count > 0 Then
                Tooltip.Add("")
                For Each Message As String In StatusInfo.Messages
                    Tooltip.Add(Message)
                Next
            End If

            Return String.Join(vbNewLine, Tooltip)
        End Function

        Private Function GetCurrentTrackStatusData(Action As ActionTypeEnum) As BaseFluxForm.TrackStatusData
            Dim StatusText As String = ""
            Dim BackColor As Color = Color.Empty
            Dim CellText As String = ""
            Dim Tooltip As String = ""

            Dim BadSectorCount = _CurrentStatusInfo.BadSectorList.Count
            Dim Format = _CurrentStatusInfo.Format
            Dim Modified = _CurrentStatusInfo.Modified
            Dim HasMessages = _CurrentStatusInfo.Messages IsNot Nothing AndAlso _CurrentStatusInfo.Messages.Count > 0

            If Action = ActionTypeEnum.Read Then
                StatusText = GetTrackStatusText(TrackStatusEnum.Reading)
                CellText = "R"
                BackColor = Color.LightBlue

            ElseIf Action = ActionTypeEnum.Complete Then
                StatusText = GetTrackStatusText(TrackStatusEnum.Success)

                If BadSectorCount > 0 Then
                    CellText = BadSectorCount.ToString
                ElseIf Modified Then
                    CellText = "M"
                Else
                    CellText = ""
                End If

                If BadSectorCount > 0 Then
                    BackColor = Color.Red
                ElseIf Not String.IsNullOrEmpty(Format) AndAlso Format <> "IBM MFM" Then
                    BackColor = Color.FromArgb(255, 190, 100)
                ElseIf Modified Then
                    BackColor = Color.Yellow
                ElseIf HasMessages Then
                    BackColor = Color.MediumSeaGreen
                Else
                    BackColor = Color.LightGreen
                End If

                Tooltip = BuildTooltip(_CurrentStatusInfo)
            End If

            Return New BaseFluxForm.TrackStatusData With {
                .CellText = CellText,
                .Track = _CurrentStatusInfo.Track,
                .Side = _CurrentStatusInfo.Side,
                .SideVisible = True,
                .TotalBadSectors = _TotalBadSectors,
                .StatusText = StatusText,
                .ForeColor = Color.Black,
                .BackColor = BackColor,
                .Tooltip = Tooltip
            }
        End Function

        Private Function GetStatusInfo(Track As Byte, Side As Byte) As TrackStatusInfo
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

        Private Sub SetCurrentTrackStatusComplete()
            If _CurrentStatusInfo IsNot Nothing Then
                Dim StatusData = GetCurrentTrackStatusData(ActionTypeEnum.Complete)

                RaiseEvent UpdateGridTrack(StatusData)
            End If
        End Sub

        Private Function UpdateStatusInfo(SectorInfo As RawSectorInfo, SectorData As RawSectorData) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(SectorData.Cylinder, SectorData.Head)

            StatusInfo.SectorId = SectorData.SectorId

            Return StatusInfo
        End Function

        Private Function UpdateStatusInfo(SectorInfo As RemasteredSectorInfo) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(SectorInfo.Track, SectorInfo.Side)

            StatusInfo.Format = SectorInfo.Format
            StatusInfo.Modified = SectorInfo.Modified
            StatusInfo.Messages = SectorInfo.Messages

            Return StatusInfo
        End Function

        Private Sub UpdateTrackStatus(Statusinfo As TrackStatusInfo, Action As ActionTypeEnum)
            If _CurrentStatusInfo IsNot Statusinfo Then
                SetCurrentTrackStatusComplete()

                _CurrentStatusInfo = Statusinfo

                Dim StatusData = GetCurrentTrackStatusData(Action)

                RaiseEvent UpdateStatus(StatusData)
            End If
        End Sub
        Private Sub UpdateTrackStatusType(Status As TrackStatusEnum)
            RaiseEvent UpdateStatusType(GetTrackStatusText(Status))
        End Sub

        Private Class TrackStatusInfo
            Public Sub New()
                _BadSectorList = New List(Of UShort)
            End Sub

            Public Property BadSectorList As List(Of UShort)

            Public ReadOnly Property Failed
                Get
                    Return BadSectorList.Count > 0
                End Get
            End Property

            Public Property Format As String = ""
            Public Property Messages As List(Of String)
            Public Property Modified As Boolean
            Public Property SectorId As Byte
            Public Property Side As Byte
            Public Property Track As Byte
        End Class
    End Class
End Namespace
