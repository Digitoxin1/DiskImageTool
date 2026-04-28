Namespace Flux.Greaseweazle
    Public Class UnexpectedSector
        Public Property Cylinder As Integer
        Public Property Head As Integer
        Public ReadOnly Property Key As String
            Get
                Return String.Format("{0},{1},{2},{3}", Cylinder, Head, SectorId, SizeId)
            End Get
        End Property

        Public Property SectorId As Integer
        Public Property Side As Integer
        Public Property SizeId As Integer
        Public Property Track As Integer
    End Class
End Namespace