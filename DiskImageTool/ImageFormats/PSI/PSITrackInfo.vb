Namespace ImageFormats.PSI
    Public Class PSITrackInfo
        Public Sub New()
            _FirstSector = -1
            _LastSector = -1
            _SectorSize = -1
            _SectorCount = 0
        End Sub

        Public Property FirstSector As Integer
        Public Property LastSector As Integer
        Public Property SectorSize As Integer
        Public Property SectorCount As Integer
    End Class
End Namespace

