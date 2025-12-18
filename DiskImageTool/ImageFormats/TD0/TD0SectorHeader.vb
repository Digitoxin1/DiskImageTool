Namespace ImageFormats.TD0
    Public Class TD0SectorHeader
        Public Const LENGTH As Integer = 6

        Private ReadOnly _header As Byte()

        Private Enum HeadBits As Byte
            HeadMask = &H1
        End Enum

        Private Enum Offset As Integer
            Cylinder = 0
            HeadRaw = 1
            SectorId = 2
            SizeCode = 3
            Flags = 4
            CrcLow = 5
        End Enum

        Public Sub New()
            _header = New Byte(LENGTH - 1) {}
        End Sub

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

        Public Property Cylinder As Byte
            Get
                Return _header(Offset.Cylinder)
            End Get
            Set(value As Byte)
                _header(Offset.Cylinder) = value
            End Set
        End Property

        Public Property DataNoId As Boolean
            Get
                Return (Flags And TD0SectorFlags.DataNoId) <> 0
            End Get
            Set(value As Boolean)
                _header(Offset.Flags) = SetFlagByte(_header(Offset.Flags), CByte(TD0SectorFlags.DataNoId), value)
            End Set
        End Property

        Public Property DosSkipped As Boolean
            Get
                Return (Flags And TD0SectorFlags.DosSkipped) <> 0
            End Get
            Set(value As Boolean)
                _header(Offset.Flags) = SetFlagByte(_header(Offset.Flags), CByte(TD0SectorFlags.DosSkipped), value)
            End Set
        End Property

        Public ReadOnly Property Flags As TD0SectorFlags
            Get
                Return CType(_header(Offset.Flags), TD0SectorFlags)
            End Get
        End Property

        Public Property HasCrcError As Boolean
            Get
                Return (Flags And TD0SectorFlags.CrcError) <> 0
            End Get
            Set(value As Boolean)
                _header(Offset.Flags) = SetFlagByte(_header(Offset.Flags), CByte(TD0SectorFlags.CrcError), value)
            End Set
        End Property
        Public ReadOnly Property Head As Byte
            Get
                Return CByte(HeadRaw And HeadBits.HeadMask)
            End Get
        End Property

        Public Property HeadRaw As Byte
            Get
                Return _header(Offset.HeadRaw)
            End Get
            Set(value As Byte)
                _header(Offset.HeadRaw) = value
            End Set
        End Property

        Public Property IsDeletedDataMark As Boolean
            Get
                Return (Flags And TD0SectorFlags.DeletedData) <> 0
            End Get
            Set(value As Boolean)
                _header(Offset.Flags) = SetFlagByte(_header(Offset.Flags), CByte(TD0SectorFlags.DeletedData), value)
            End Set
        End Property

        Public Property IsDosSkipped As Boolean
            Get
                Return (Flags And TD0SectorFlags.DosSkipped) <> 0
            End Get
            Set(value As Boolean)
                _header(Offset.Flags) = SetFlagByte(_header(Offset.Flags), CByte(TD0SectorFlags.DosSkipped), value)
            End Set
        End Property

        Public Property IsDuplicated As Boolean
            Get
                Return (Flags And TD0SectorFlags.Duplicated) <> 0
            End Get
            Set(value As Boolean)
                _header(Offset.Flags) = SetFlagByte(_header(Offset.Flags), CByte(TD0SectorFlags.Duplicated), value)
            End Set
        End Property

        Public Property NoData As Boolean
            Get
                Return (Flags And TD0SectorFlags.NoData) <> 0
            End Get
            Set(value As Boolean)
                _header(Offset.Flags) = SetFlagByte(_header(Offset.Flags), CByte(TD0SectorFlags.NoData), value)
            End Set
        End Property

        Public Property SectorId As Byte
            Get
                Return _header(Offset.SectorId)
            End Get
            Set(value As Byte)
                _header(Offset.SectorId) = value
            End Set
        End Property

        Public Property SizeCode As Byte
            Get
                Return _header(Offset.SizeCode)
            End Get
            Set(value As Byte)
                _header(Offset.SizeCode) = value
            End Set
        End Property

        Public ReadOnly Property StoredCrcLow As Byte
            Get
                Return _header(Offset.CrcLow)
            End Get
        End Property

        Public Function ComputedCrcLow(data As Byte()) As Byte
            If data Is Nothing Then
                Return 0
            End If

            Dim crc As UShort = TD0Crc16.Compute(data, 0, data.Length)

            Return CByte(crc And &HFFUS)
        End Function

        Public Function CrcValid(data As Byte()) As Boolean
            Return StoredCrcLow = ComputedCrcLow(data)
        End Function

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

        Public Sub RefreshStoredCrcLow(data As Byte())
            _header(Offset.CrcLow) = ComputedCrcLow(data)
        End Sub

        Public Function TrySetSectorSizeBytes(sizeBytes As Integer) As Boolean
            If sizeBytes < 128 Then
                Return False
            End If

            If (sizeBytes And (sizeBytes - 1)) <> 0 Then
                Return False ' not power of two
            End If

            If sizeBytes Mod 128 <> 0 Then
                Return False
            End If

            Dim sc As Integer = CInt(Math.Log(sizeBytes \ 128, 2))

            If sc < 0 OrElse sc > 7 Then
                Return False
            End If

            SizeCode = sc

            Return True
        End Function
    End Class
End Namespace
