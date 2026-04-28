Namespace Flux.Greaseweazle
    Public Class TrackRead
        Public Property DestSide As Integer
        Public Property DestTrack As Integer
        Public Property Details As TrackReadDetails = Nothing
        Public Property DetailString As String
        Public Property FailedSectors As Integer? = Nothing
        Public ReadOnly Property IsRemapped As Boolean
            Get
                Return SrcTrack <> DestTrack OrElse SrcSide <> DestSide
            End Get
        End Property
        Public Property OutOfRange As TrackReadOutOfRange = Nothing
        Public Property SrcSide As Integer
        Public Property SrcTrack As Integer
        Public Property SrcType As String
    End Class
End Namespace