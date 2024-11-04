Namespace ImageFormats
    Namespace PSI
        Public Class GCRSectorHeader
            Private ReadOnly _ChunkData() As Byte

            Public Sub New()
                _ChunkData = New Byte(17) {}
            End Sub

            Public Sub New(ChunkData() As Byte)
                _ChunkData = ChunkData
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

            Public Property Flags As GCRSectorFlags
                Get
                    Return _ChunkData(5)
                End Get
                Set(value As GCRSectorFlags)
                    _ChunkData(5) = value
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

            Public Property SectorFormat As Byte
                Get
                    Return _ChunkData(4)
                End Get
                Set(value As Byte)
                    _ChunkData(4) = value
                End Set
            End Property

            Public Property TagData As Byte()
                Get
                    Dim Buffer(11) As Byte
                    Array.Copy(_ChunkData, 6, Buffer, 0, Buffer.Length)
                    Return Buffer
                End Get
                Set(value As Byte())
                    Array.Copy(value, 0, _ChunkData, 6, 12)
                End Set
            End Property
        End Class
    End Namespace
End Namespace
