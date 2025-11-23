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
            Dim Sector = ParseSector(line)

            If Sector.HasValue Then
                If Sector.Value.Mark = "FE" Then
                    Dim SectorData = ParseSectorData(Sector.Value.Message)
                    If SectorData.HasValue Then
                        _TrackFound = True
                        Dim StatusInfo = UpdateStatusInfo(Sector.Value, SectorData)
                        UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read)
                    End If
                End If
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

        Private Function GetBackgroundColor(Status As String, Flags As String) As Color
            Select Case Status.ToLower
                Case "ok"
                    If Flags <> "" Then
                        Return Color.Yellow
                    Else
                        Return Color.LightGreen
                    End If
                Case "error"
                    Return Color.Red
                Case "mismatch"
                    Return Color.FromArgb(255, 190, 100)
                Case "unformatted"
                    Return Color.FromArgb(240, 240, 240)
                Case Else
                    Return Color.Yellow
            End Select
        End Function

        Private Function GetCurrentTrackStatusData(Action As ActionTypeEnum) As BaseFluxForm.TrackStatusData
            Dim StatusText As String = ""
            Dim BackColor As Color = Color.Empty
            Dim CellText As String = ""
            Dim Tooltip As String = ""
            Dim BadSectorCount = 0

            BadSectorCount = _CurrentStatusInfo.BadSectorList.Count

            If Action = ActionTypeEnum.Read Then
                StatusText = GetTrackStatusText(TrackStatusEnum.Reading)
                CellText = "R"
                BackColor = Color.LightBlue
            ElseIf Action = ActionTypeEnum.Complete Then
                StatusText = GetTrackStatusText(TrackStatusEnum.Success)
                BackColor = GetBackgroundColor("ok", "")
                If BadSectorCount > 0 Then
                    CellText = BadSectorCount.ToString
                Else
                    CellText = ""
                End If
            End If

            Dim StatusData As BaseFluxForm.TrackStatusData
            With StatusData
                .CellText = CellText
                .Track = _CurrentStatusInfo.Track
                .Side = _CurrentStatusInfo.Side
                .SideVisible = True
                .TotalBadSectors = _TotalBadSectors
                .StatusText = StatusText
                .ForeColor = Color.Black
                .BackColor = BackColor
                .Tooltip = Tooltip
            End With

            Return StatusData
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

        Private Function UpdateStatusInfo(SectorInfo As SectorInfo, SectorData As SectorData) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(SectorData.Cylinder, SectorData.Head)

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
                _BadSectorList = New List(Of Byte)
            End Sub

            Public Property BadSectorList As List(Of Byte)

            Public ReadOnly Property Failed
                Get
                    Return BadSectorList.Count > 0
                End Get
            End Property

            Public Property Side As Byte
            Public Property Track As Byte
        End Class
    End Class
End Namespace
