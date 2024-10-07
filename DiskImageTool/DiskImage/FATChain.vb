Namespace DiskImage
    Public Class FATChain
        Public Sub New(DirectoryEntry As DirectoryEntry)
            _DirectoryEntry = DirectoryEntry
        End Sub
        Public Property Clusters As New List(Of UShort)
        Public Property CrossLinks As New List(Of DirectoryEntry)
        Public Property HasCircularChain As Boolean = False
        Public ReadOnly Property DirectoryEntry As DirectoryEntry
        Public Property OpenChain As Boolean = True
    End Class
End Namespace
