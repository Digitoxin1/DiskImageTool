Namespace Flux.Kryoflux
    Public Class TrackStatus
        Inherits ConsoleParser
        Implements ITrackStatus

        Private ReadOnly _Failed As Boolean = False
        Private ReadOnly _StatusCollection As Dictionary(Of String, TrackStatusInfo)
        Private _CurrentStatusInfo As TrackStatusInfo = Nothing
        Private _TotalBadSectors As UInteger = 0
        Private _TotalUnexpectedSectors As UInteger = 0
        Private _TrackFound As Boolean = False

        Private Enum TrackStatusEnum
            Reading
            Erasing
            Writing
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
            _TotalUnexpectedSectors = 0
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
            Dim TrackSummary = ParseTrackSummary(line)

            If TrackSummary IsNot Nothing Then
                _TrackFound = True
                Dim TrackInfo = ParseTrackInfo(TrackSummary.Details)
                If TrackInfo IsNot Nothing Then
                    Dim StatusInfo = UpdateStatusInfo(TrackInfo, TrackSummary.Track, TrackSummary.Side, InfoAction)
                    UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read)
                Else
                    Dim StatusInfo = UpdateStatusInfo(TrackSummary, ActionTypeEnum.Import)
                    UpdateTrackStatus(StatusInfo, ActionTypeEnum.Read)
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
                Case TrackStatusEnum.Erasing
                    Return My.Resources.Label_Erasing
                Case TrackStatusEnum.Writing
                    Return My.Resources.Label_Writing
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

            If StatusInfo.TrackInfo Is Nothing Then
                Return String.Join(vbNewLine, Tooltip)
            End If

            If StatusInfo.TrackInfo.Status <> "" Then
                Tooltip.Add(My.Resources.Label_Staus & ":  " & UppercaseFirst(StatusInfo.TrackInfo.Status))
            End If

            Dim Sectors As New List(Of String)

            If StatusInfo.TrackInfo.BadSectorCount > 0 Then
                Sectors.Add(StatusInfo.TrackInfo.BadSectorCount & " " & My.Resources.Label_Bad)
            End If

            If StatusInfo.TrackInfo.MissingSectorCount > 0 Then
                Sectors.Add(StatusInfo.TrackInfo.MissingSectorCount & " " & My.Resources.Label_Missing)
            End If

            If StatusInfo.TrackInfo.ModifiedSectorCount > 0 Then
                Sectors.Add(StatusInfo.TrackInfo.ModifiedSectorCount & " " & My.Resources.Label_Modified)
            End If

            If Sectors.Count > 0 Then
                Tooltip.Add(My.Resources.Label_Sectors & ":  " & String.Join(", ", Sectors))
            End If

            If StatusInfo.TrackInfo.Flags.Length > 0 Then
                Tooltip.Add(My.Resources.Label_Flags & ":")
                For Each Flag As Char In StatusInfo.TrackInfo.Flags
                    Dim Description = GetFlagDescription(Flag, StatusInfo.TrackInfo)
                    If Description <> "" Then
                        Tooltip.Add(Description)
                    End If
                Next
            End If

            Return String.Join(vbNewLine, Tooltip)
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
            Dim Tooltip As String = BuildTooltip(_CurrentStatusInfo)
            Dim BadSectorCount = 0
            Dim Flags = ""
            Dim Status = ""

            If _CurrentStatusInfo.TrackInfo IsNot Nothing Then
                BadSectorCount = _CurrentStatusInfo.TrackInfo.BadSectorCount + _CurrentStatusInfo.TrackInfo.MissingSectorCount
                Flags = _CurrentStatusInfo.TrackInfo.Flags
                Status = _CurrentStatusInfo.TrackInfo.Status
            End If


            If Action = ActionTypeEnum.Read Then
                StatusText = GetTrackStatusText(TrackStatusEnum.Reading)
                CellText = "R"
                BackColor = Color.LightBlue
            ElseIf Action = ActionTypeEnum.Complete Then
                StatusText = GetTrackStatusText(TrackStatusEnum.Success)
                BackColor = GetBackgroundColor(Status, Flags)
                If BadSectorCount > 0 Then
                    CellText = BadSectorCount.ToString
                ElseIf Status <> "" Then
                    If Flags.Length > 1 Then
                        CellText = "+" & Flags.Length
                    Else
                        CellText = Flags
                    End If
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
                .TotalUnexpectedSectors = _TotalUnexpectedSectors
                .StatusText = StatusText
                .ForeColor = Color.Black
                .BackColor = BackColor
                .Tooltip = Tooltip
            End With

            Return StatusData
        End Function

        Private Function GetFlagDescription(Flag As String, TrackInfo As TrackInfo) As String
            Dim Details As String
            Select Case Flag
                Case "B"
                    Details = My.Resources.Kryoflux_Flag_B
                Case "C"
                    Details = My.Resources.Kryoflux_Flag_C
                Case "E"
                    Details = My.Resources.Kryoflux_Flag_E
                Case "H"
                    Details = My.Resources.Kryoflux_Flag_H
                Case "I"
                    Details = My.Resources.Kryoflux_Flag_I
                Case "L"
                    Details = My.Resources.Kryoflux_Flag_L
                Case "N"
                    Details = My.Resources.Kryoflux_Flag_N
                Case "P"
                    Details = My.Resources.Kryoflux_Flag_P
                Case "S"
                    Details = My.Resources.Kryoflux_Flag_S
                Case "T"
                    Details = My.Resources.Kryoflux_Flag_T
                    Details &= " " & TrackInfo.MFMTrack.ToString("D3") & "[" & TrackInfo.PhysicalTrack.ToString("D3") & "]"
                Case "X"
                    Details = My.Resources.Kryoflux_Flag_X
                Case "Z"
                    Details = My.Resources.Kryoflux_Flag_Z
                Case Else
                    Details = ""
            End Select

            Return Flag & If(Details.Length > 0, ":  " & Details, "")
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
        Private Sub SetCurrentTrackStatusComplete()
            If _CurrentStatusInfo IsNot Nothing Then
                Dim StatusData = GetCurrentTrackStatusData(ActionTypeEnum.Complete)

                RaiseEvent UpdateGridTrack(StatusData)
            End If
        End Sub

        Private Function UpdateStatusInfo(TrackInfo As TrackInfo, Track As Integer, Side As Integer, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(Track, Side)

            StatusInfo.Action = Action
            StatusInfo.Track = Track
            StatusInfo.Side = Side
            StatusInfo.TrackInfo = TrackInfo
            _TotalBadSectors += TrackInfo.BadSectorCount

            Return StatusInfo
        End Function

        Private Function UpdateStatusInfo(TrackInfo As TrackSummary, Action As ActionTypeEnum) As TrackStatusInfo
            Dim StatusInfo = GetStatusInfo(TrackInfo.Track, TrackInfo.Side)

            StatusInfo.Action = Action
            StatusInfo.Track = TrackInfo.Track
            StatusInfo.Side = TrackInfo.Side
            StatusInfo.ErrorMessage = TrackInfo.Details

            Return StatusInfo
        End Function

        Private Sub UpdateTrackStatus(Statusinfo As TrackStatusInfo, Action As ActionTypeEnum)
            SetCurrentTrackStatusComplete()

            _CurrentStatusInfo = Statusinfo

            Dim StatusData = GetCurrentTrackStatusData(Action)

            RaiseEvent UpdateStatus(StatusData)
        End Sub

        Private Sub UpdateTrackStatusType(Status As TrackStatusEnum)
            RaiseEvent UpdateStatusType(GetTrackStatusText(Status))
        End Sub

        Private Function UppercaseFirst(value As String) As String
            If String.IsNullOrEmpty(value) Then Return value
            If value.Length = 1 Then Return value.ToUpper()

            Return Char.ToUpper(value(0)) & value.Substring(1)
        End Function

        Private Class TrackStatusInfo
            Public Property Action As ActionTypeEnum
            Public Property ErrorMessage As String
            Public ReadOnly Property Failed
                Get
                    Return TrackInfo.BadSectorCount > 0 Or TrackInfo.MissingSectorCount > 0
                End Get
            End Property

            Public Property Side As Integer
            Public Property Track As Integer
            Public Property TrackInfo As TrackInfo
        End Class
    End Class
End Namespace
