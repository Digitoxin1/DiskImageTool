
Namespace PSIImage
    Public Enum DefaultSectorFormat
        Unknown
        IBM_FM
        IBM_MFM_DD
        IBM_MFM_HD
        IBM_MFM_ED
        MAG_GCR
    End Enum

    Public Enum SectorFlags
        Compressed = 1
        AlternateSector = 2
        DataCRCError = 4
    End Enum

    Public Enum MFMSectorFlags
        IDFieldCRCError = 1
        DataFieldCRCError = 2
        DeletedDAM = 4
        MissingDAM = 8
    End Enum

    Public Enum MFMEncodingSubtype
        DoubleDensity = 0
        HighDensity = 1
        ExtraDensity = 2
    End Enum

    Public Class PSISectorImage
        Private _CurrentSector As PSISector
        Public Sub New()
            _DefaultSectorFormat = DefaultSectorFormat.Unknown
            _FormatVersion = 0
            _Sectors = New List(Of PSISector)
            _CurrentSector = Nothing
            _Comment = ""
        End Sub

        Public Property DefaultSectorFormat As DefaultSectorFormat
        Public Property FormatVersion As UShort
        Public Property Sectors As List(Of PSISector)
        Public Property Comment As String

        Public Function Load(Buffer() As Byte) As Boolean
            Dim Result As Boolean = False
            Dim Chunk As Chunk

            Dim Offset As UInt32 = 0

            Chunk = New Chunk()
            Offset = Chunk.ReadFromBuffer(Buffer, Offset)
            If Chunk.ChunkID = "PSI " AndAlso Chunk.CheckCRC Then
                ProcessChunk(Chunk)
                Do While Offset + 8 < Buffer.Length
                    Chunk = New Chunk()
                    Offset = Chunk.ReadFromBuffer(Buffer, Offset)
                    If Chunk.ChunkID = "END " Then
                        Result = True
                        Exit Do
                    ElseIf Chunk.CheckCRC Then
                        ProcessChunk(Chunk)
                    End If
                Loop
            End If

            Return Result
        End Function

        Private Sub ProcessChunk(Chunk As Chunk)
            Debug.WriteLine("ChunkId: " & Chunk.ChunkID)
            If Chunk.ChunkID = "PSI " Then
                Dim FileHeader = New PSIFileHeader(Chunk.ChunkData)
                _DefaultSectorFormat = FileHeader.DefaultSectorFormat
                _FormatVersion = FileHeader.FormatVersion

            ElseIf Chunk.ChunkID = "SECT" Then
                Dim Sector = New PSISector(Chunk.ChunkData)
                _Sectors.Add(Sector)
                _CurrentSector = Sector

            ElseIf Chunk.ChunkID = "DATA" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.Data = Chunk.ChunkData
                End If

            ElseIf Chunk.ChunkID = "WEAK" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.Weak = Chunk.ChunkData
                End If

            ElseIf Chunk.ChunkID = "TEXT" Then
                _Comment &= Text.Encoding.UTF8.GetString(Chunk.ChunkData)

            ElseIf Chunk.ChunkID = "OFFS" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.Offset = ToBigEndianUInt32(Chunk.ChunkData)
                End If

            ElseIf Chunk.ChunkID = "TIME" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.SectorReadTime = ToBigEndianUInt32(Chunk.ChunkData)
                End If

            ElseIf Chunk.ChunkID = "IBMM" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.MFM = New PSIIBMSector(Chunk.ChunkData)
                End If

            ElseIf Chunk.ChunkID = "IBMF" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.FM = New PSIIBMSector(Chunk.ChunkData)
                End If
            End If
        End Sub

        Private Class Chunk
            Private _ChunkID As Byte()
            Private _ChunkSize As Byte()

            Public Sub New()
                _ChunkID = New Byte(3) {}
                _ChunkSize = New Byte(3) {}
                _ChunkData = New Byte(-1) {}
                _ChunkCRC = 0
            End Sub

            Public Property ChunkID As String
                Get
                    Return Text.Encoding.UTF8.GetString(_ChunkID, 0, 4)
                End Get
                Set
                    _ChunkID = Text.Encoding.UTF8.GetBytes(Left(Value, 4))
                End Set
            End Property

            Public Property ChunkSize As UInt32
                Get
                    Return ToBigEndianUInt32(_ChunkSize)
                End Get
                Set
                    _ChunkSize = BitConverter.GetBytes(ToBigEndianUInt32(Value))
                End Set
            End Property

            Public Property ChunkData As Byte()
            Public Property ChunkCRC As UInt32

            Public Function CheckCRC() As Boolean
                Dim ChunkSize = Me.ChunkSize

                Dim Buffer(ChunkSize + 8 - 1) As Byte

                Array.Copy(_ChunkID, 0, Buffer, 0, 4)
                Array.Copy(_ChunkSize, 0, Buffer, 4, 4)
                Array.Copy(_ChunkData, 0, Buffer, 8, ChunkSize)

                Dim Checksum = PSI_CRC(Buffer, Buffer.Length)

                Return (Checksum = _ChunkCRC)
            End Function

            Public Function ReadFromBuffer(Buffer() As Byte, Offset As UInt32) As UInt32
                Array.Copy(Buffer, Offset, _ChunkID, 0, 4)
                Array.Copy(Buffer, Offset + 4, _ChunkSize, 0, 4)

                Dim ChunkSize = Me.ChunkSize

                If ChunkSize <= Buffer.Length - Offset - 12 Then
                    _ChunkData = New Byte(ChunkSize - 1) {}
                    Array.Copy(Buffer, Offset + 8, _ChunkData, 0, ChunkSize)
                    _ChunkCRC = ToBigEndianUInt32(Buffer, Offset + 8 + ChunkSize)
                End If

                Return Offset + 12 + ChunkSize
            End Function

            Private Function PSI_CRC(buf() As Byte, cnt As Integer) As UInteger
                Dim i As Integer
                Dim j As Integer
                Dim crc As UInteger = 0

                For i = 0 To cnt - 1
                    crc = crc Xor (CUInt(buf(i)) << 24)

                    For j = 0 To 7
                        If (crc And &H80000000UI) <> 0 Then
                            crc = (crc << 1) Xor &H1EDC6F41UI
                        Else
                            crc <<= 1
                        End If
                    Next
                Next

                Return crc And &HFFFFFFFFUI
            End Function
        End Class
    End Class

    Public Class PSIFileHeader
        Private ReadOnly _ChunkData() As Byte
        Public Sub New(ChunkData() As Byte)
            _ChunkData = ChunkData
        End Sub
        Public ReadOnly Property FormatVersion As UShort
            Get
                Return ToBigEndianUInt16(_ChunkData, 0)
            End Get
        End Property

        Public ReadOnly Property DefaultSectorFormat As DefaultSectorFormat
            Get
                Return GetSectorFormat(ToBigEndianUInt16(_ChunkData, 2))
            End Get
        End Property

        Private Function GetSectorFormat(Value As UInt16) As DefaultSectorFormat
            Dim Result As DefaultSectorFormat

            Select Case Value
                Case 256
                    Result = DefaultSectorFormat.IBM_FM
                Case 512
                    Result = DefaultSectorFormat.IBM_MFM_DD
                Case 513
                    Result = DefaultSectorFormat.IBM_MFM_HD
                Case 514
                    Result = DefaultSectorFormat.IBM_MFM_ED
                Case 768
                    Result = DefaultSectorFormat.MAG_GCR
                Case Else
                    Result = DefaultSectorFormat.Unknown
            End Select

            Return Result
        End Function
    End Class

    Public Class PSISector
        Private ReadOnly _ChunkData() As Byte
        Public Sub New(ChunkData() As Byte)
            _ChunkData = ChunkData
            _Data = New Byte(-1) {}
            _Weak = New Byte(-1) {}
            _Offset = 0
            _SectorReadTime = 0
            _MFM = Nothing
            _FM = Nothing
        End Sub
        Public ReadOnly Property Cylinder As UShort
            Get
                Return ToBigEndianUInt16(_ChunkData, 0)
            End Get
        End Property
        Public ReadOnly Property Head As Byte
            Get
                Return _ChunkData(2)
            End Get
        End Property
        Public ReadOnly Property Sector As Byte
            Get
                Return _ChunkData(3)
            End Get
        End Property
        Public ReadOnly Property Size As UShort
            Get
                Return ToBigEndianUInt16(_ChunkData, 4)
            End Get
        End Property
        Public ReadOnly Property Flags As SectorFlags
            Get
                Return _ChunkData(6)
            End Get
        End Property

        Public ReadOnly Property CkmpressedSectorData As Byte
            Get
                Return _ChunkData(7)
            End Get
        End Property
        Public Property Data As Byte()
        Public Property Weak As Byte()
        Public Property Offset As UInt32
        Public Property SectorReadTime As UInt32
        Public Property MFM As PSIIBMSector
        Public Property FM As PSIIBMSector

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

    Public Class PSIIBMSector
        Private ReadOnly _ChunkData() As Byte
        Public Sub New(ChunkData() As Byte)
            _ChunkData = ChunkData
        End Sub

        Public ReadOnly Property Cylinder As Byte
            Get
                Return _ChunkData(0)
            End Get
        End Property
        Public ReadOnly Property Head As Byte
            Get
                Return _ChunkData(1)
            End Get
        End Property
        Public ReadOnly Property Sector As Byte
            Get
                Return _ChunkData(2)
            End Get
        End Property
        Public ReadOnly Property Size As Byte
            Get
                Return _ChunkData(3)
            End Get
        End Property
        Public ReadOnly Property Flags As MFMSectorFlags
            Get
                Return _ChunkData(4)
            End Get
        End Property
        Public ReadOnly Property EncodingSubType As MFMEncodingSubtype
            Get
                Return _ChunkData(5)
            End Get
        End Property
    End Class

End Namespace
