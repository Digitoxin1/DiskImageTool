Namespace ImageFormats.TD0
    Public Structure TD0TrackHeader
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
        Public ReadOnly Property HeadAndFlags As Byte
            Get
                Return _header(Offset.HeadAndFlags)
            End Get
        End Property

        Public ReadOnly Property IsEndOfTracks As Boolean
            Get
                Return SectorCount = &HFF
            End Get
        End Property

        Public ReadOnly Property IsFMTrack As Boolean
            Get
                Return (HeadAndFlags And HeadFlag.FM) <> 0
            End Get
        End Property

        Public ReadOnly Property PhysicalCylinder As Byte
            Get
                Return _header(Offset.PhysicalCylinder)
            End Get
        End Property

        Public ReadOnly Property PhysicalHead As Byte
            Get
                Return CByte(HeadAndFlags And HeadFlag.HeadMask)
            End Get
        End Property

        Public ReadOnly Property SectorCount As Byte
            Get
                Return _header(Offset.SectorCount)
            End Get
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
    End Structure
End Namespace
