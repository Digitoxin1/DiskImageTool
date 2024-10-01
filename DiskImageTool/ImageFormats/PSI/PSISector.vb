Namespace ImageFormats
    Namespace PSI
        Public Class PSISector
            Private ReadOnly _ChunkData() As Byte
            Private _Data As Byte()
            Private _HasWeakBits As Boolean
            Private _Weak As Byte()

            Public Sub New()
                _ChunkData = New Byte(7) {}
                Initialize()
            End Sub
            Public Sub New(ChunkData() As Byte)
                _ChunkData = ChunkData
                Initialize()
            End Sub

            Private Sub Initialize()
                Dim Size = Me.Size

                _Data = New Byte(Size - 1) {}
                _Weak = New Byte(-1) {}
                _HasWeakBits = False
                _Offset = 0
                _SectorReadTime = 0
                _MFMHeader = Nothing
                _FMHeader = Nothing
                _GCRHeader = Nothing

                If IsCompressed() Then
                    Dim CompressedSectorData = Me.CompressedSectorData
                    For i = 0 To Size - 1
                        _Data(i) = CompressedSectorData
                    Next
                End If
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
                    _ChunkData(7) = value
                End Set
            End Property

            Public Property IsCompressed() As Boolean
                Get
                    Return (Flags And SectorFlags.Compressed) > 0
                End Get
                Set(value As Boolean)
                    Flags = MyBitConverter.ToggleBit(Flags, SectorFlags.Compressed, value)
                End Set
            End Property

            Public Property IsAlternateSector() As Boolean
                Get
                    Return (Flags And SectorFlags.AlternateSector) > 0
                End Get
                Set(value As Boolean)
                    Flags = MyBitConverter.ToggleBit(Flags, SectorFlags.AlternateSector, value)
                End Set
            End Property

            Public Property HasDataCRCError() As Boolean
                Get
                    Return (Flags And SectorFlags.DataCRCError) > 0
                End Get
                Set(value As Boolean)
                    Flags = MyBitConverter.ToggleBit(Flags, SectorFlags.DataCRCError, value)
                End Set
            End Property

            Public Property Data As Byte()
                Get
                    Return _Data
                End Get
                Set
                    _Data = Value
                    UpdateDataProperties()
                End Set
            End Property

            Private Sub UpdateDataProperties()
                Dim CompressedSectorData As Byte = 0
                Dim IsCompressed As Boolean = True

                For i = 0 To _Data.Length - 1
                    If i = 0 Then
                        CompressedSectorData = _Data(i)
                    ElseIf CompressedSectorData <> _Data(i) Then
                        IsCompressed = False
                        CompressedSectorData = 0
                        Exit For
                    End If
                Next

                Me.Size = _Data.Length
                Me.IsCompressed = IsCompressed
                Me.CompressedSectorData = CompressedSectorData
            End Sub

            Public Property HasWeakBits As Boolean
                Get
                    Return _HasWeakBits
                End Get
                Set
                    _HasWeakBits = Value
                End Set
            End Property

            Public Property Weak As Byte()
                Get
                    Return _Weak
                End Get
                Set
                    _Weak = Value
                    _HasWeakBits = True
                End Set
            End Property

            Public Property Offset As UInt32
            Public Property SectorReadTime As UInt32
            Public Property MFMHeader As IBMSectorHeader
            Public Property FMHeader As IBMSectorHeader
            Public Property GCRHeader As GCRSectorHeader
        End Class
    End Namespace
End Namespace
