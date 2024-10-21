Namespace Bitstream
    Namespace IBM_MFM
        Public Class BitstreamRegion
            Public Sub New(RegionType As MFMRegionType, StartIndex As UInteger, Length As UInteger, Sector As BitstreamRegionSector, BitOffset As UInteger)
                _RegionType = RegionType
                _StartIndex = StartIndex
                _Length = Length
                _Sector = Sector
                _BitOffset = BitOffset
            End Sub

            Public Property RegionType As MFMRegionType
            Public Property StartIndex As UInteger
            Public Property Length As UInteger
            Public Property Sector As BitstreamRegionSector
            Public Property BitOffset As UInteger
        End Class
    End Namespace
End Namespace
