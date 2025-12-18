Namespace ImageFormats.TD0
    Public Class TD0TrackHeader
        Public Const LENGTH As Integer = 4
        Private Const CHECKSUM_LENGTH As Integer = 3

        Private ReadOnly _header As Byte()

        Private Enum HeadFlag As Byte
            FM = &H80
            HeadMask = &H1
        End Enum

        Private Enum Offset As Integer
            SectorCount = 0
            PhysicalCylinder = 1
            HeadAndFlags = 2
            CrcLow = 3
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

        Public ReadOnly Property IsEndOfTracks As Boolean
            Get
                Return SectorCount = &HFF
            End Get
        End Property

        Public Property IsFMTrack As Boolean
            Get
                Return (_header(Offset.HeadAndFlags) And HeadFlag.FM) <> 0
            End Get
            Set(value As Boolean)
                _header(Offset.HeadAndFlags) = SetFlagByte(_header(Offset.HeadAndFlags), CByte(HeadFlag.FM), value)
            End Set
        End Property

        Public Property PhysicalCylinder As Byte
            Get
                Return _header(Offset.PhysicalCylinder)
            End Get
            Set(value As Byte)
                _header(Offset.PhysicalCylinder) = value
            End Set
        End Property

        Public Property PhysicalHead As Byte
            Get
                Return CByte(_header(Offset.HeadAndFlags) And HeadFlag.HeadMask)
            End Get
            Set(value As Byte)
                Dim headBit As Byte = If(value >= 1, CByte(1), CByte(0))

                Dim b As Byte = _header(Offset.HeadAndFlags)
                b = CByte((b And Not HeadFlag.HeadMask) Or headBit)

                _header(Offset.HeadAndFlags) = b
            End Set
        End Property

        Public Property SectorCount As Byte
            Get
                Return _header(Offset.SectorCount)
            End Get
            Set(value As Byte)
                _header(Offset.SectorCount) = value
            End Set
        End Property

        Public ReadOnly Property StoredCrcLow As Byte
            Get
                Return _header(Offset.CrcLow)
            End Get
        End Property

        Public Function ComputedCrc16() As UShort
            Return TD0Crc16.Compute(_header, 0, CHECKSUM_LENGTH)
        End Function

        Public Function ComputedCrcLow() As Byte
            Return CByte(ComputedCrc16() And &HFFUS)
        End Function

        Public Function CrcValid() As Boolean
            If IsEndOfTracks Then
                Return True
            End If

            Return StoredCrcLow = ComputedCrcLow()
        End Function

        Public Function GetBytes() As Byte()
            Dim b(LENGTH - 1) As Byte
            Array.Copy(_header, 0, b, 0, LENGTH)

            Return b
        End Function

        Public Sub RefreshStoredCrcLow()
            _header(Offset.CrcLow) = ComputedCrcLow()
        End Sub
    End Class
End Namespace
