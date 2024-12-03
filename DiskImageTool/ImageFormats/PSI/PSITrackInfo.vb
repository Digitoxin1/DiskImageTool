Namespace ImageFormats
    Namespace PSI
        Public Class PSITrackInfo
            Public Sub New()
                _FirstSector = -1
                _LastSector = -1
                _SectorSize = -1
            End Sub

            Public Property FirstSector As Integer
            Public Property LastSector As Integer
            Public Property SectorSize As Integer
        End Class
    End Namespace
End Namespace

