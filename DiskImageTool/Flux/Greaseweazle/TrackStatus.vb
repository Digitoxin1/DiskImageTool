Namespace Flux.Greaseweazle
    Public Class TrackStatus
        Inherits ConsoleParser

        Private ReadOnly _ParentForm As BaseForm
        Private ReadOnly _StatusCollection As Dictionary(Of String, TrackStatusInfo)
        Private _CurrentStatusInfo As TrackStatusInfo = Nothing
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

        Public Sub New(ParentForm As BaseForm)
            MyBase.New

            _ParentForm = ParentForm
            _StatusCollection = New Dictionary(Of String, TrackStatusInfo)
        End Sub

        Public Property CurrentStatusInfo As TrackStatusInfo
            Get
                Return _CurrentStatusInfo
            End Get
            Set(value As TrackStatusInfo)
                _CurrentStatusInfo = value
            End Set
        End Property

        Public Sub Clear()
            _StatusCollection.Clear()
            _CurrentStatusInfo = Nothing
            _TotalBadSectors = 0
            _TotalUnexpectedSectors = 0
        End Sub

        Public Function UpdateStatusInfo(TrackInfo As TrackInfoWrite, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(TrackInfo.SrcTrack, TrackInfo.SrcSide)

            StatusInfo.Action = Action
            StatusInfo.Retries = Math.Max(StatusInfo.Retries, TrackInfo.Retry)
            If TrackInfo.Failed Then
                StatusInfo.Failed = TrackInfo.Failed
            End If

            Return StatusInfo
        End Function

        Public Function UpdateStatusInfo(TrackInfo As ConsoleParser.TrackInfoRead, ProcessBadSectors As Boolean, Action As ActionTypeEnum) As TrackStatusInfo
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

        Public Function UpdateStatusInfo(TrackInfo As ConsoleParser.UnexpectedSector, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(TrackInfo.Track, TrackInfo.Side)

            StatusInfo.Action = Action

            If Not StatusInfo.UnexpectedSectors.ContainsKey(TrackInfo.Key) Then
                StatusInfo.UnexpectedSectors.Add(TrackInfo.Key, TrackInfo)
                _TotalUnexpectedSectors += 1
            End If

            Return StatusInfo
        End Function

        Public Function UpdateStatusInfo(TrackInfo As ConsoleParser.TrackInfoReadFailed, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(TrackInfo.DestTrack, TrackInfo.DestSide)

            StatusInfo.Action = Action

            StatusInfo.Failed = True
            StatusInfo.BadSectors = TrackInfo.Sectors
            _TotalBadSectors += TrackInfo.Sectors

            Return StatusInfo
        End Function

        Public Sub UpdateTrackStatus(Statusinfo As TrackStatusInfo, Action As ActionTypeEnum, DoubleStep As Boolean)
            SetCurrentTrackStatusComplete(DoubleStep)

            _CurrentStatusInfo = Statusinfo

            Dim StatusData = GetCurrentTrackStatusData(Action, DoubleStep)
            _ParentForm.UpdateStatus(StatusData)
        End Sub

        Public Sub UpdateTrackStatusComplete(Aborted As Boolean, DoubleStep As Boolean, Optional KeepProcessing As Boolean = False)
            If Aborted Then
                UpdateTrackStatusType(TrackStatusEnum.Aborted)
            Else
                SetCurrentTrackStatusComplete(DoubleStep)

                If KeepProcessing Then
                    Exit Sub
                End If

                UpdateTrackStatusType(TrackStatusEnum.Success)
            End If
        End Sub

        Public Sub UpdateTrackStatusError()
            UpdateTrackStatusType(TrackStatusEnum.Error)
        End Sub

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

        Private Shared Function GetTrackStatusColor(Status As TrackStatusEnum) As (ForeColor As Color, BackColor As Color)
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

        Private Shared Function GetTrackStatusText(Status As TrackStatusEnum, Optional Retries As UShort = 0) As String
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

        Private Function GetCurrentTrackStatusData(Action As ActionTypeEnum, DoubleStep As Boolean) As BaseForm.TrackStatusData
            Dim Data = GetTrackStatus(_CurrentStatusInfo, Action)
            Dim Colors = GetTrackStatusColor(Data.Status)

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

        Private Sub UpdateTrackStatusType(Status As TrackStatusEnum)
            _ParentForm.UpdateTrackStatusType(GetTrackStatusText(Status))
        End Sub
        Public Class TrackStatusInfo
            Public Sub New()
                _UnexpectedSectors = New Dictionary(Of String, ConsoleParser.UnexpectedSector)
            End Sub

            Public Property Action As ActionTypeEnum
            Public Property BadSectors As UShort = 0
            Public Property Failed As Boolean
            Public Property Retries As UShort = 0
            Public Property Side As Integer
            Public Property Track As Integer
            Public Property UnexpectedSectors As Dictionary(Of String, ConsoleParser.UnexpectedSector)
        End Class
    End Class
End Namespace
