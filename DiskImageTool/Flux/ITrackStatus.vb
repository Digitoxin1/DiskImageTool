Namespace Flux
    Public Interface ITrackStatus
        Enum FluxDevice
            Greaseweazle
            Kryoflux
        End Enum

        Enum ActionTypeEnum
            Unknown
            Read
            Write
            [Erase]
            Import
            Complete
        End Enum

        ReadOnly Property Failed As Boolean
        ReadOnly Property TrackFound As Boolean
        Sub Clear()
        Sub ProcessOutputLineRead(line As String, InfoAction As ActionTypeEnum, DoubleStep As Boolean)
        Sub UpdateTrackStatusAborted()
        Sub UpdateTrackStatusComplete(DoubleStep As Boolean)
        Sub UpdateTrackStatusError()
    End Interface
End Namespace

