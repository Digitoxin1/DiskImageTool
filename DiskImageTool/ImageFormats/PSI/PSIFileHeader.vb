Namespace ImageFormats
    Namespace PSI
        Public Class PSIFileHeader
            Private ReadOnly _ChunkData() As Byte

            Public Sub New()
                _ChunkData = New Byte(3) {}
            End Sub

            Public Sub New(ChunkData() As Byte)
                _ChunkData = ChunkData
            End Sub

            Public ReadOnly Property ChunkData As Byte()
                Get
                    Return _ChunkData
                End Get
            End Property

            Public Property DefaultSectorFormat As DefaultSectorFormat
                Get
                    Return MyBitConverter.ToUInt16(_ChunkData, True, 2)
                End Get
                Set(value As DefaultSectorFormat)
                    Dim Buffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(value))
                    Array.Copy(Buffer, 0, _ChunkData, 2, 2)
                End Set
            End Property

            Public Property FormatVersion As UShort
                Get
                    Return MyBitConverter.ToUInt16(_ChunkData, True, 0)
                End Get
                Set(value As UShort)
                    Dim Buffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(value))
                    Array.Copy(Buffer, 0, _ChunkData, 0, 2)
                End Set
            End Property
        End Class
    End Namespace
End Namespace
