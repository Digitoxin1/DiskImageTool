Namespace DiskImage
    Public Class FATChain
        Public Sub New(Offset As UInteger)
            _Offset = Offset
        End Sub
        Public Property Clusters As New List(Of UShort)
        Public Property CrossLinks As New List(Of UInteger)
        Public Property HasCircularChain As Boolean = False
        Public ReadOnly Property Offset As UInteger
        Public Property OpenChain As Boolean = True
    End Class
End Namespace
