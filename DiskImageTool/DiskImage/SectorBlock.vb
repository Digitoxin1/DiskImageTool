Namespace DiskImage
    Public Class SectorBlock
        Sub New(Index As Integer, SectorStart As UInteger, SectorCount As UInteger, Offset As UInteger, Size As UInteger, Description As String)
            _Index = Index
            _Description = Description
            _SectorStart = SectorStart
            _SectorCount = SectorCount
            _Offset = Offset
            _Size = Size
        End Sub

        Public ReadOnly Property Description As String
        Public ReadOnly Property Index As Integer
        Public ReadOnly Property Offset As UInteger
        Public ReadOnly Property SectorCount As UInteger
        Public ReadOnly Property SectorStart As UInteger
        Public ReadOnly Property Size As UInteger
    End Class
End Namespace
