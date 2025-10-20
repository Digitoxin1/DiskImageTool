Namespace Bitstream
    Namespace IBM_MFM
        Public Class BitstreamRegionData
            Public Sub New()
                _Regions = New List(Of BitstreamRegion)
                _Sectors = New List(Of BitstreamRegionSector)
            End Sub

            Property Regions As List(Of BitstreamRegion)
            Property Sectors As List(Of BitstreamRegionSector)
            Property Track As UShort = 0
            Property Side As Byte = 0
            Property NumBytes As UInteger = 0
            Property NumBits As UInteger = 0
            Property Aligned As Boolean = True
            Property Gap4A As UShort = 0
            Property Gap1 As UShort = 0
            Property Encoding As String = ""
            Property IAMNulls As UInteger = 0
        End Class
    End Namespace
End Namespace
