Namespace Bitstream
    Namespace IBM_MFM
        Public Class BitstreamRegionSector
            Public Property DataChecksumValid As Boolean
            Public Property DataLength As UInteger
            Public Property DataStartIndex As UInteger
            Public Property Gap3 As UShort
            Public Property Length As UInteger
            Public Property SectorId As Byte
            Public Property SectorIndex As UShort
            Public Property Side As Byte
            Public Property StartIndex As UInteger
            Public Property Track As Byte
            Public Overrides Function ToString() As String
                'Return _SectorIndex & " (" & _SectorId & ")"
                Return _SectorId
            End Function
        End Class
    End Namespace
End Namespace
