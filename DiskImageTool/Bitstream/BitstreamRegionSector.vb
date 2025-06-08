Namespace Bitstream
    Namespace IBM_MFM
        Public Class BitstreamRegionSector
            Public Property DAM As Byte
            Public Property DataChecksumValid As Boolean
            Public Property DataLength As UInteger
            Public Property DataStartIndex As UInteger
            Public Property Gap3 As UShort
            Public Property HasData As Boolean
            Public Property HasWeakBits As Boolean
            Public Property IDAMChecksumValid As Boolean
            Public Property Length As UInteger
            Public Property Overlaps As Boolean
            Public Property SectorId As Byte
            Public Property SectorIndex As UShort
            Public Property Side As Byte
            Public Property StartIndex As UInteger
            Public Property Track As Byte
            Public Property WriteSplice As Boolean
        End Class
    End Namespace
End Namespace
