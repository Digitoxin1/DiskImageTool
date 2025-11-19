Namespace Flux.Greaseweazle
    Public Class TrackStatus
        Inherits ConsoleParser

        Private ReadOnly _ParentForm As BaseForm
        Private ReadOnly _StatusCollection As Dictionary(Of String, TrackStatusInfo)
        Private _CurrentStatusInfo As TrackStatusInfo = Nothing
        Private _Range As TrackRange = Nothing
        Private _TotalBadSectors As UInteger = 0
        Private _TotalUnexpectedSectors As UInteger = 0
        Public Enum ActionTypeEnum
            Unknown
            Read
            Write
            [Erase]
            Import
            Complete
        End Enum

        Private Enum TrackStatusEnum
            Reading
            Erasing
            Writing
            Retry
            Failed
            Success
            Aborted
            [Error]
        End Enum

        Public Sub New(ParentForm As BaseForm)
            MyBase.New

            _ParentForm = ParentForm
            _StatusCollection = New Dictionary(Of String, TrackStatusInfo)
        End Sub

        Public Function CanKeepProcessing() As Boolean
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

        Public Sub Clear()
            _StatusCollection.Clear()
            _CurrentStatusInfo = Nothing
            _TotalBadSectors = 0
            _TotalUnexpectedSectors = 0
            _Range = Nothing
        End Sub

        Public Sub ProcessOutputLineRead(line As String, InfoAction As ActionTypeEnum, DoubleStep As Boolean)
            If _Range Is Nothing Then
                _Range = ParseDiskRange(line)
            End If

            Dim Summary = ParseTrackReadSummary(line)
            If Summary IsNot Nothing Then
                Dim Details = ParseTrackReadDetails(Summary.Details)
                If Details IsNot Nothing Then
                    Dim ProcessBadSectors = (InfoAction = ActionTypeEnum.Import)
                    Dim Statusinfo = UpdateStatusInfo(Summary, Details, ProcessBadSectors, InfoAction)
                    UpdateTrackStatus(Statusinfo, ActionTypeEnum.Read, DoubleStep)
                    Return
                End If

                If InfoAction = ActionTypeEnum.Read Then
                    Dim FailedSectors = ParseTrackReadFailed(Summary.Details)
                    If FailedSectors.HasValue Then
                        Dim Statusinfo = UpdateStatusInfo(Summary, FailedSectors.Value, InfoAction)
                        UpdateTrackStatus(Statusinfo, ActionTypeEnum.Read, DoubleStep)
                        Return
                    End If

                    Dim OutOfRange = ParseTrackReadOutOfRange(Summary.Details)
                    If OutOfRange IsNot Nothing Then
                        Dim Statusinfo = UpdateStatusInfo(Summary, OutOfRange, InfoAction)
                        UpdateTrackStatus(Statusinfo, ActionTypeEnum.Read, DoubleStep)
                        Return
                    End If
                End If
            End If

            Dim TrackInfoUnexpected = ParseTrackUnexpected(line)
            If TrackInfoUnexpected IsNot Nothing Then
                Dim StatusInfo = UpdateStatusInfo(TrackInfoUnexpected, InfoAction)
                UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read, DoubleStep)
                Return
            End If
        End Sub

        Public Sub ProcessOutputLineWrite(line As String, InfoAction As ActionTypeEnum, DoubleStep As Boolean)
            If _Range Is Nothing Then
                _Range = ParseDiskRange(line)
            End If

            Dim TrackInfo = ParseTrackWrite(line)

            If TrackInfo IsNot Nothing Then
                Dim Action As ActionTypeEnum
                If TrackInfo.Action = "Erasing" Then
                    Action = ActionTypeEnum.Erase
                ElseIf TrackInfo.Action = "Writing" Then
                    Action = ActionTypeEnum.Write
                Else
                    Action = ActionTypeEnum.Unknown
                End If

                Dim Statusinfo = UpdateStatusInfo(TrackInfo, InfoAction)
                UpdateTrackStatus(Statusinfo, Action, DoubleStep)
                Return
            End If
        End Sub

        Public Sub UpdateTrackStatusAborted()
            UpdateTrackStatusType(TrackStatusEnum.Aborted)
        End Sub

        Public Sub UpdateTrackStatusComplete(DoubleStep As Boolean, Optional KeepProcessing As Boolean = False)
            SetCurrentTrackStatusComplete(DoubleStep)

            If KeepProcessing Then
                Exit Sub
            End If

            UpdateTrackStatusType(TrackStatusEnum.Success)
        End Sub

        Public Sub UpdateTrackStatusError()
            UpdateTrackStatusType(TrackStatusEnum.Error)
        End Sub

        Friend Function GetNextTrackRange() As (Ranges As List(Of (StartTrack As UShort, EndTrack As UShort)), Heads As CommandLineBuilder.TrackHeads, [Continue] As Boolean)
            Dim Response As (Ranges As List(Of (StartTrack As UShort, EndTrack As UShort)), Heads As CommandLineBuilder.TrackHeads, [Continue] As Boolean)

            Response.Ranges = New List(Of (StartTrack As UShort, EndTrack As UShort))

            If _CurrentStatusInfo.Side < _Range.HeadEnd Then
                Response.Continue = True
                Response.Heads = GetTrackHeads(_CurrentStatusInfo.Side + 1)
                Response.Ranges.Add((_CurrentStatusInfo.Track, _CurrentStatusInfo.Track))
            Else
                Response.Continue = False
                Response.Heads = CommandLineBuilder.TrackHeads.both
                Response.Ranges.Add((_CurrentStatusInfo.Track + 1, _Range.TrackEnd))
            End If

            Return Response
        End Function
        Private Shared Function GetTrackStatus(Statusinfo As TrackStatusInfo, Action As ActionTypeEnum) As (Status As TrackStatusEnum, Label As String)
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
            ElseIf Action = ActionTypeEnum.Complete Then
                If Statusinfo.Retries > 0 Then
                    Status = TrackStatusEnum.Retry
                    Label = Statusinfo.Retries
                Else
                    Status = TrackStatusEnum.Success
                    Label = ""
                End If
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

            Return (Status, Label)
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
                        Return (Color.Black, Color.FromArgb(240, 240, 240))
                    Else
                        Return (Color.Black, Color.LightGreen)
                    End If
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

        Private Function GetCurrentTrackStatusData(Action As ActionTypeEnum, DoubleStep As Boolean) As BaseForm.TrackStatusData
            Dim Data = GetTrackStatus(_CurrentStatusInfo, Action)
            Dim Colors = GetTrackStatusColor(Data.Status, _CurrentStatusInfo)

            Dim StatusData As BaseForm.TrackStatusData
            With StatusData
                .StatusText = GetTrackStatusText(Data.Status, _CurrentStatusInfo.Retries)
                .CellText = Data.Label
                .Track = _CurrentStatusInfo.Track
                .Side = _CurrentStatusInfo.Side
                .SideVisible = True
                .TotalBadSectors = _TotalBadSectors
                .TotalUnexpectedSectors = _TotalUnexpectedSectors
                .ForeColor = Colors.ForeColor
                .BackColor = Colors.BackColor
                .Tooltip = ""
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

                _ParentForm.GridMarkTrack(StatusData)
            End If
        End Sub

        Private Function UpdateStatusInfo(TrackInfo As TrackWrite, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(TrackInfo.SrcTrack, TrackInfo.SrcSide)

            StatusInfo.Action = Action
            StatusInfo.Retries = Math.Max(StatusInfo.Retries, TrackInfo.Retry)
            If TrackInfo.Failed Then
                StatusInfo.Failed = TrackInfo.Failed
            End If

            Return StatusInfo
        End Function

        Private Function UpdateStatusInfo(Summary As TrackReadSummary, Details As TrackReadDetails, ProcessBadSectors As Boolean, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(Summary.DestTrack, Summary.DestSide)

            StatusInfo.Action = Action

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

            Return StatusInfo
        End Function

        Private Function UpdateStatusInfo(TrackInfo As ConsoleParser.UnexpectedSector, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(TrackInfo.Track, TrackInfo.Side)

            StatusInfo.Action = Action

            If Not StatusInfo.UnexpectedSectors.ContainsKey(TrackInfo.Key) Then
                StatusInfo.UnexpectedSectors.Add(TrackInfo.Key, TrackInfo)
                _TotalUnexpectedSectors += 1
            End If

            Return StatusInfo
        End Function

        Private Function UpdateStatusInfo(Summary As TrackReadSummary, FailedSectors As Integer, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(Summary.DestTrack, Summary.DestSide)

            StatusInfo.Action = Action

            StatusInfo.Failed = True
            StatusInfo.BadSectors = FailedSectors
            _TotalBadSectors += FailedSectors

            Return StatusInfo
        End Function

        Private Function UpdateStatusInfo(Summary As TrackReadSummary, OutOfRage As TrackReadOutOfRange, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(Summary.DestTrack, Summary.DestSide)

            StatusInfo.Action = Action

            StatusInfo.OutOfRange = True
            StatusInfo.OutOfRangeFormat = OutOfRage.Format

            Return StatusInfo
        End Function

        Private Sub UpdateTrackStatus(Statusinfo As TrackStatusInfo, Action As ActionTypeEnum, DoubleStep As Boolean)
            SetCurrentTrackStatusComplete(DoubleStep)

            _CurrentStatusInfo = Statusinfo

            Dim StatusData = GetCurrentTrackStatusData(Action, DoubleStep)
            _ParentForm.UpdateStatus(StatusData)
        End Sub

        Private Sub UpdateTrackStatusType(Status As TrackStatusEnum)
            _ParentForm.UpdateTrackStatusType(GetTrackStatusText(Status))
        End Sub

        Private Class TrackStatusInfo
            Public Sub New()
                _UnexpectedSectors = New Dictionary(Of String, ConsoleParser.UnexpectedSector)
            End Sub

            Public Property Action As ActionTypeEnum
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
