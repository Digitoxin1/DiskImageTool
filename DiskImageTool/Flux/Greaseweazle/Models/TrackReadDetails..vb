Namespace Flux.Greaseweazle
    Public Class TrackReadDetails
        Public ReadOnly Property BadSectors As Integer
            Get
                Return SectorsTotal - SectorsFound
            End Get
        End Property
        Public Property Encoding As String
        Public Property FluxCount As Integer
        Public Property FluxTimeMs As Double
        Public Property Retry As Integer
        Public Property SectorsFound As Integer
        Public Property SectorsTotal As Integer
        Public Property Seek As Integer
        Public Property SrcFormat As String
        Public Property System As String
    End Class
End Namespace
