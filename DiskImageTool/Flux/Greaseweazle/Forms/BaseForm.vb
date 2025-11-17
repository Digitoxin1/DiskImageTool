Namespace Flux
    Namespace Greaseweazle
        Public Class BaseForm
            Inherits Flux.BaseForm

            Private ReadOnly _Parser As ConsoleParser
            Private ReadOnly _TrackStatus As Dictionary(Of String, TrackStatusInfo)
            Private _CurrentStatusInfo As TrackStatusInfo = Nothing

            Public Sub New(Optional UseGrid As Boolean = True)
                MyBase.New(GreaseweazleSettings.LogFileName, UseGrid)

                _Parser = New ConsoleParser
                _TrackStatus = New Dictionary(Of String, TrackStatusInfo)
            End Sub

            Public ReadOnly Property CurrentStatusInfo As TrackStatusInfo
                Get
                    Return _CurrentStatusInfo
                End Get
            End Property

            Public ReadOnly Property Parser As ConsoleParser
                Get
                    Return _Parser
                End Get
            End Property

            Public Sub ClearTrackStatus()
                _TrackStatus.Clear()
                _CurrentStatusInfo = Nothing
            End Sub

            Public Function UpdateStatusInfo(TrackInfo As ConsoleParser.TrackInfoWrite, Action As ActionTypeEnum) As TrackStatusInfo
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
                    TotalBadSectors += TrackInfo.BadSectors
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
                    TotalUnexpectedSectors += 1
                End If

                Return StatusInfo
            End Function

            Public Function UpdateStatusInfo(TrackInfo As ConsoleParser.TrackInfoReadFailed, Action As ActionTypeEnum) As TrackStatusInfo
                Dim StatusInfo = GetStatusInfo(TrackInfo.DestTrack, TrackInfo.DestSide)

                StatusInfo.Action = Action

                StatusInfo.Failed = True
                StatusInfo.BadSectors = TrackInfo.Sectors
                TotalBadSectors += TrackInfo.Sectors

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

                If TotalBadSectors = 1 Then
                    StatusBadSectors.Text = TotalBadSectors & " " & My.Resources.Label_BadSector
                    StatusBadSectors.Visible = True
                ElseIf TotalBadSectors > 1 Then
                    StatusBadSectors.Text = TotalBadSectors & " " & My.Resources.Label_BadSectors
                    StatusBadSectors.Visible = True
                Else
                    StatusBadSectors.Visible = False
                End If

                If TotalUnexpectedSectors = 1 Then
                    StatusUnexpected.Text = TotalUnexpectedSectors & " " & My.Resources.Label_UnexpectedSector
                    StatusUnexpected.Visible = True
                ElseIf TotalUnexpectedSectors > 1 Then
                    StatusUnexpected.Text = TotalUnexpectedSectors & " " & My.Resources.Label_UnexpectedSectors
                    StatusUnexpected.Visible = True
                Else
                    StatusUnexpected.Visible = False
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
End Namespace

