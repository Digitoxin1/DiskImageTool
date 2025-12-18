Namespace ImageFormats.TD0
    Public Structure TD0SectorHeader
        Public Const LENGTH As Integer = 6

        Private ReadOnly _header As Byte()

        Private Enum Offset As Integer
            Cylinder = 0
            HeadRaw = 1
            SectorId = 2
            SizeCode = 3
            Flags = 4
            CrcLow = 5
        End Enum

        Private Enum HeadBits As Byte
            HeadMask = &H1
        End Enum

        Public Sub New(imagebuf As Byte(), offset As Integer)
            _header = New Byte(LENGTH - 1) {}
            If imagebuf Is Nothing Then
                Return
            End If
            If offset < 0 OrElse (offset + LENGTH) > imagebuf.Length Then
                Return
            End If
            Array.Copy(imagebuf, offset, _header, 0, LENGTH)
        End Sub

        Public ReadOnly Property Cylinder As Byte
            Get
                Return _header(Offset.Cylinder)
            End Get
        End Property

        Public ReadOnly Property Flags As TD0SectorFlags
            Get
                Return CType(_header(Offset.Flags), TD0SectorFlags)
            End Get
        End Property

        Public ReadOnly Property HasCrcError As Boolean
            Get
                Return (Flags And TD0SectorFlags.CrcError) <> 0
            End Get
        End Property

        Public ReadOnly Property HasDataBlock As Boolean
            Get
                Return (Flags And (TD0SectorFlags.DosSkipped Or TD0SectorFlags.NoData)) = 0
            End Get
        End Property

        Public ReadOnly Property HasDataButNoId As Boolean
            Get
                Return (Flags And TD0SectorFlags.DataNoId) <> 0
            End Get
        End Property

        Public ReadOnly Property HasIdButNoData As Boolean
            Get
                Return (Flags And TD0SectorFlags.NoData) <> 0
            End Get
        End Property

        Public ReadOnly Property Head As Byte
            Get
                Return CByte(HeadRaw And HeadBits.HeadMask)
            End Get
        End Property

        Public ReadOnly Property HeadRaw As Byte
            Get
                Return _header(Offset.HeadRaw)
            End Get
        End Property
        Public ReadOnly Property IsDeletedDataMark As Boolean
            Get
                Return (Flags And TD0SectorFlags.DeletedData) <> 0
            End Get
        End Property

        Public ReadOnly Property IsDosSkipped As Boolean
            Get
                Return (Flags And TD0SectorFlags.DosSkipped) <> 0
            End Get
        End Property

        Public ReadOnly Property IsDuplicate As Boolean
            Get
                Return (Flags And TD0SectorFlags.Duplicated) <> 0
            End Get
        End Property

        Public ReadOnly Property SectorId As Byte
            Get
                Return _header(Offset.SectorId)
            End Get
        End Property

        Public ReadOnly Property SizeCode As Byte
            Get
                Return _header(Offset.SizeCode)
            End Get
        End Property

        Public Property StoredCrcLow As Byte
            Get
                Return _header(Offset.CrcLow)
            End Get
            Set(value As Byte)
                _header(Offset.CrcLow) = value
            End Set
        End Property

        Public Function ComputedCrc16(data As Byte()) As Byte
            If data Is Nothing Then
                Return 0
            End If

            Dim crc As UShort = TD0Crc16.Compute(data, 0, data.Length)

            Return CByte(crc And &HFFUS)
        End Function

        Public Function CrcValid(data As Byte()) As Boolean
            Return StoredCrcLow = ComputedCrc16(data)
        End Function

        Public Sub RefreshStoredCrc16(data As Byte())
            StoredCrcLow = ComputedCrc16(data)
        End Sub

        Public Function GetBytes() As Byte()
            Dim b(LENGTH - 1) As Byte
            Array.Copy(_header, 0, b, 0, LENGTH)
            Return b
        End Function

        Public Function GetSectorSizeBytes() As Integer
            Dim sc As Integer = CInt(SizeCode)

            If sc < 0 OrElse sc > 7 Then
                Return 0
            End If

            Return 128 << sc
        End Function
    End Structure
End Namespace
