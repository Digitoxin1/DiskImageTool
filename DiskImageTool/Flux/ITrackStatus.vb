Namespace Flux
    Friend Interface ITrackStatus
        Event UpdateGridTooltip(Track As Integer, Side As Integer, Tooltip As String)
        Event UpdateGridTrack(StatusData As BaseFluxForm.TrackStatusData)
        Event UpdateStatus(StatusData As BaseFluxForm.TrackStatusData)
        Event UpdateStatusType(StatusText As String)
        ReadOnly Property Failed As Boolean
        ReadOnly Property TrackFound As Boolean
        Function CanKeepProcessing() As Boolean
        Sub Clear()
        Function GetNextTrackRange() As (Ranges As List(Of (StartTrack As UShort, EndTrack As UShort)), Heads As TrackHeads, [Continue] As Boolean)
        Sub ProcessOutputLineRead(line As String, InfoAction As ActionTypeEnum, DoubleStep As Boolean)
        Sub ProcessOutputLineWrite(line As String, InfoAction As ActionTypeEnum, DoubleStep As Boolean)
        Sub UpdateTrackStatusAborted()
        Sub UpdateTrackStatusComplete(DoubleStep As Boolean, Optional KeepProcessing As Boolean = False)
        Sub UpdateTrackStatusError()
    End Interface
End Namespace

