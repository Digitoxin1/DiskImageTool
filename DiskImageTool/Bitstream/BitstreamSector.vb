Namespace Bitstream
    Public Class BitstreamSector
        Public Sub New(MFMSector As IBM_MFM.IBM_MFM_Sector)
            _MFMSector = MFMSector
            _Data = MFMSector.Data
            _Size = MFMSector.GetSizeBytes
        End Sub

        Public Sub New(Data() As Byte, Size As UInteger)
            _MFMSector = Nothing
            _Data = Data
            _Size = Size
        End Sub

        Public ReadOnly Property Data As Byte()
        Public Property IsStandard As Boolean
        Public ReadOnly Property MFMSector As IBM_MFM.IBM_MFM_Sector
        Public ReadOnly Property Size As UInteger
    End Class
End Namespace
