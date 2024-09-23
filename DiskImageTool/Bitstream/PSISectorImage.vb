Namespace PSIImage
    Public Enum DefaultSectorFormat As UInt16
        IBM_FM = 256
        IBM_MFM_DD = 512
        IBM_MFM_HD = 513
        IBM_MFM_ED = 514
        MAG_GCR = 768
    End Enum

    Public Enum SectorFlags As Byte
        Compressed = 1
        AlternateSector = 2
        DataCRCError = 4
    End Enum

    Public Enum MFMSectorFlags As Byte
        IDFieldCRCError = 1
        DataFieldCRCError = 2
        DeletedDAM = 4
        MissingDAM = 8
    End Enum

    Public Enum GCRSectorFlags As Byte
        IDFieldChecksumError = 1
        DataFieldCChecksumError = 2
        MissingDataMark = 4
    End Enum

    Public Enum MFMEncodingSubtype As Byte
        DoubleDensity = 0
        HighDensity = 1
        ExtraDensity = 2
    End Enum

    Public Class PSISectorImage
        Private Structure ReadResponse
            Dim Offset As UInt32
            Dim ChecksumVerified As Boolean
        End Structure

        Private _CurrentSector As PSISector
        Public Sub New()
            Initialize()
        End Sub
        Public Property Header As PSIFileHeader
        Public Property Sectors As List(Of PSISector)
        Public Property Comment As String

        Public Sub Export(FilePath As String)
            Dim Buffer() As Byte

            Using fs As IO.FileStream = IO.File.OpenWrite(FilePath)
                Buffer = New PSIChunk("PSI ", _Header.ChunkData).ToBytes
                fs.Write(Buffer, 0, Buffer.Length)

                If _Comment.Length > 0 Then
                    Buffer = New PSIChunk("TEXT", Text.Encoding.UTF8.GetBytes(_Comment)).ToBytes
                    fs.Write(Buffer, 0, Buffer.Length)
                End If

                For Each Sector In _Sectors
                    Buffer = New PSIChunk("SECT", Sector.ChunkData).ToBytes
                    fs.Write(Buffer, 0, Buffer.Length)

                    If Sector.FMHeader IsNot Nothing Then
                        Buffer = New PSIChunk("IBMF", Sector.FMHeader.ChunkData).ToBytes
                        fs.Write(Buffer, 0, Buffer.Length)
                    End If

                    If Sector.MFMHeader IsNot Nothing Then
                        Buffer = New PSIChunk("IBMM", Sector.MFMHeader.ChunkData).ToBytes
                        fs.Write(Buffer, 0, Buffer.Length)
                    End If

                    If Sector.GCRHeader IsNot Nothing Then
                        Buffer = New PSIChunk("MACG", Sector.GCRHeader.ChunkData).ToBytes
                        fs.Write(Buffer, 0, Buffer.Length)
                    End If

                    If Sector.Offset > 0 Then
                        Dim OffsetBuffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(Sector.Offset))
                        Buffer = New PSIChunk("OFFS", OffsetBuffer).ToBytes
                        fs.Write(Buffer, 0, Buffer.Length)
                    End If

                    If Sector.SectorReadTime > 0 Then
                        Dim OffsetBuffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(Sector.SectorReadTime))
                        Buffer = New PSIChunk("TIME", OffsetBuffer).ToBytes
                        fs.Write(Buffer, 0, Buffer.Length)
                    End If

                    If Sector.Data.Length > 0 Then
                        Buffer = New PSIChunk("DATA", Sector.Data).ToBytes
                        fs.Write(Buffer, 0, Buffer.Length)
                    End If

                    If Sector.Weak.Length > 0 Then
                        Buffer = New PSIChunk("WEAK", Sector.Weak).ToBytes
                        fs.Write(Buffer, 0, Buffer.Length)
                    End If
                Next

                Buffer = New PSIChunk("END ").ToBytes
                fs.Write(Buffer, 0, Buffer.Length)
            End Using
        End Sub

        Public Function Import(FilePath As String) As Boolean
            Return Import(IO.File.ReadAllBytes(FilePath))
        End Function

        Public Function Import(Buffer() As Byte) As Boolean
            Dim Result As Boolean = False
            Dim Chunk As PSIChunk
            Dim ChunkCount As UInteger = 0
            Dim Response As ReadResponse

            Initialize()

            Response.Offset = 0

            Do While Response.Offset + 8 < Buffer.Length
                Chunk = New PSIChunk()
                Response = Chunk.ReadFromBuffer(Buffer, Response.Offset)
                If Response.ChecksumVerified Then
                    If ChunkCount = 0 AndAlso Chunk.ChunkID <> "PSI " Then
                        Exit Do
                    ElseIf Chunk.ChunkID = "END " Then
                        Result = True
                        Exit Do
                    Else
                        ProcessChunk(Chunk)
                    End If
                Else
                    Exit Do
                End If
                ChunkCount += 1
            Loop

            Return Result
        End Function

        Private Sub Initialize()
            _Header = New PSIFileHeader
            _Sectors = New List(Of PSISector)
            _CurrentSector = Nothing
            _Comment = ""
        End Sub

        Private Sub ProcessChunk(Chunk As PSIChunk)
            If Chunk.ChunkID = "PSI " Then
                _Header = New PSIFileHeader(Chunk.ChunkData)

            ElseIf Chunk.ChunkID = "TEXT" Then
                _Comment &= Text.Encoding.UTF8.GetString(Chunk.ChunkData)

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

            ElseIf Chunk.ChunkID = "OFFS" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.Offset = MyBitConverter.ToUInt32(Chunk.ChunkData, True)
                End If

            ElseIf Chunk.ChunkID = "TIME" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.SectorReadTime = MyBitConverter.ToUInt32(Chunk.ChunkData, True)
                End If

            ElseIf Chunk.ChunkID = "IBMM" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.MFMHeader = New IBMSectorHeader(Chunk.ChunkData)
                End If

            ElseIf Chunk.ChunkID = "IBMF" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.FMHeader = New IBMSectorHeader(Chunk.ChunkData)
                End If

            ElseIf Chunk.ChunkID = "MACG" Then
                If _CurrentSector IsNot Nothing Then
                    _CurrentSector.GCRHeader = New GCRSectorHeader(Chunk.ChunkData)
                End If
            End If
        End Sub

        Private Class PSIChunk
            Private _ChunkID As Byte()
            Private _ChunkData As Byte()
            Private _ChunkCRC As UInt32 = 0

            Public Sub New()
                _ChunkID = New Byte(3) {}
                _ChunkData = New Byte(-1) {}
            End Sub

            Public Sub New(ChunkID As String)
                _ChunkID = Text.Encoding.UTF8.GetBytes(Left(ChunkID, 4))
                _ChunkData = New Byte(-1) {}
            End Sub

            Public Sub New(ChunkID As String, ChunkData() As Byte)
                _ChunkID = Text.Encoding.UTF8.GetBytes(Left(ChunkID, 4))
                _ChunkData = ChunkData
            End Sub

            Public Property ChunkID As String
                Get
                    Return Text.Encoding.UTF8.GetString(_ChunkID, 0, 4)
                End Get
                Set(value As String)
                    _ChunkID = Text.Encoding.UTF8.GetBytes(Left(value, 4))
                End Set
            End Property

            Public ReadOnly Property ChunkSize As UInt32
                Get
                    Return _ChunkData.Length
                End Get
            End Property

            Public Property ChunkData As Byte()
                Get
                    Return _ChunkData
                End Get
                Set
                    _ChunkData = Value
                End Set
            End Property

            Public Function ReadFromBuffer(Buffer() As Byte, Offset As UInt32) As ReadResponse
                Dim Response As ReadResponse

                Array.Copy(Buffer, Offset, _ChunkID, 0, 4)

                Dim ChunkSize = MyBitConverter.ToUInt32(Buffer, True, Offset + 4)

                If ChunkSize <= Buffer.Length - Offset - 12 Then
                    _ChunkData = New Byte(ChunkSize - 1) {}
                    Array.Copy(Buffer, Offset + 8, _ChunkData, 0, ChunkSize)
                    _ChunkCRC = MyBitConverter.ToUInt32(Buffer, True, Offset + 8 + ChunkSize)
                End If

                Response.Offset = Offset + 12 + ChunkSize
                Response.ChecksumVerified = CheckCRC()

                Return Response
            End Function

            Public Function ToBytes() As Byte()
                Dim ChunkSize = BitConverter.GetBytes(MyBitConverter.SwapEndian(CUInt(_ChunkData.Length)))

                Dim Buffer(_ChunkData.Length + 12 - 1) As Byte

                Array.Copy(_ChunkID, 0, Buffer, 0, 4)
                Array.Copy(ChunkSize, 0, Buffer, 4, 4)
                If _ChunkData.Length > 0 Then
                    Array.Copy(_ChunkData, 0, Buffer, 8, _ChunkData.Length)
                End If

                Dim ChunkCRC = PSI_CRC(Buffer, Buffer.Length - 4)
                Dim CRCBytes = BitConverter.GetBytes(MyBitConverter.SwapEndian(ChunkCRC))

                Array.Copy(CRCBytes, 0, Buffer, 8 + _ChunkData.Length, 4)

                Return Buffer
            End Function

            Private Function CheckCRC() As Boolean
                Dim Buffer = ToBytes()
                Dim CRC = MyBitConverter.ToUInt32(Buffer, True, Buffer.Length - 4)

                Return (_ChunkCRC = CRC)
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

        Public Property FormatVersion As UShort
            Get
                Return MyBitConverter.ToUInt16(_ChunkData, True, 0)
            End Get
            Set(value As UShort)
                Dim Buffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(value))
                Array.Copy(Buffer, 0, _ChunkData, 0, 2)
            End Set
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
    End Class

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

    Public Class IBMSectorHeader
        Private ReadOnly _ChunkData() As Byte
        Public Sub New()
            _ChunkData = New Byte(5) {}
        End Sub
        Public Sub New(ChunkData() As Byte)
            _ChunkData = ChunkData
        End Sub

        Public ReadOnly Property ChunkData As Byte()
            Get
                Return _ChunkData
            End Get
        End Property

        Public Property Cylinder As Byte
            Get
                Return _ChunkData(0)
            End Get
            Set(value As Byte)
                _ChunkData(0) = value
            End Set
        End Property
        Public Property Head As Byte
            Get
                Return _ChunkData(1)
            End Get
            Set(value As Byte)
                _ChunkData(1) = value
            End Set
        End Property
        Public Property Sector As Byte
            Get
                Return _ChunkData(2)
            End Get
            Set(value As Byte)
                _ChunkData(2) = value
            End Set
        End Property
        Public Property Size As Byte
            Get
                Return _ChunkData(3)
            End Get
            Set(value As Byte)
                _ChunkData(3) = value
            End Set
        End Property
        Public Property Flags As MFMSectorFlags
            Get
                Return _ChunkData(4)
            End Get
            Set(value As MFMSectorFlags)
                _ChunkData(4) = value
            End Set
        End Property
        Public Property EncodingSubType As MFMEncodingSubtype
            Get
                Return _ChunkData(5)
            End Get
            Set(value As MFMEncodingSubtype)
                _ChunkData(5) = value
            End Set
        End Property
    End Class

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
        Public Property Flags As GCRSectorFlags
            Get
                Return _ChunkData(5)
            End Get
            Set(value As GCRSectorFlags)
                _ChunkData(5) = value
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
