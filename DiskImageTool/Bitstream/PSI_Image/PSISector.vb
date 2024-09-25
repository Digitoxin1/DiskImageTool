Namespace PSI_Image
    Public Class PSISector
        Private ReadOnly _ChunkData() As Byte
        Public Sub New()
            _ChunkData = New Byte(7) {}
            Initialize()
        End Sub
        Public Sub New(ChunkData() As Byte)
            _ChunkData = ChunkData
            Initialize()
        End Sub

        Private Sub Initialize()
            _Data = New Byte(-1) {}
            _Weak = New Byte(-1) {}
            _Offset = 0
            _SectorReadTime = 0
            _MFMHeader = Nothing
            _FMHeader = Nothing
            _GCRHeader = Nothing
        End Sub

        Public ReadOnly Property ChunkData As Byte()
            Get
                Return _ChunkData
            End Get
        End Property

        Public Property Cylinder As UShort
            Get
                Return MyBitConverter.ToUInt16(_ChunkData, True, 0)
            End Get
            Set(value As UShort)
                Dim Buffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(value))
                Array.Copy(Buffer, 0, _ChunkData, 0, 2)
            End Set
        End Property

        Public Property Head As Byte
            Get
                Return _ChunkData(2)
            End Get
            Set(value As Byte)
                _ChunkData(2) = value
            End Set
        End Property
        Public Property Sector As Byte
            Get
                Return _ChunkData(3)
            End Get
            Set(value As Byte)
                _ChunkData(3) = value
            End Set
        End Property
        Public Property Size As UShort

            Get
                Return MyBitConverter.ToUInt16(_ChunkData, True, 4)
            End Get
            Set(value As UShort)
                Dim Buffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(value))
                Array.Copy(Buffer, 0, _ChunkData, 4, 2)
            End Set
        End Property
        Public Property Flags As SectorFlags
            Get
                Return _ChunkData(6)
            End Get
            Set(value As SectorFlags)
                _ChunkData(6) = value
            End Set
        End Property

        Public Property CompressedSectorData As Byte
            Get
                Return _ChunkData(7)
            End Get
            Set(value As Byte)
                _ChunkData(7) = CompressedSectorData
            End Set
        End Property
        Public Property Data As Byte()
        Public Property Weak As Byte()
        Public Property Offset As UInt32
        Public Property SectorReadTime As UInt32
        Public Property MFMHeader As IBMSectorHeader
        Public Property FMHeader As IBMSectorHeader
        Public Property GCRHeader As GCRSectorHeader

        Public Function IsCompressed() As Boolean
            Return (Flags And SectorFlags.Compressed) > 0
        End Function

        Public Function IsAlternateSector() As Boolean
            Return (Flags And SectorFlags.AlternateSector) > 0
        End Function

        Public Function HasDataCRCError() As Boolean
            Return (Flags And SectorFlags.DataCRCError) > 0
        End Function
    End Class
End Namespace
