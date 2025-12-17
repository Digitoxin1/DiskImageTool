Namespace ImageFormats.TD0
    Public Structure TD0TrackHeader
        Public Const LENGTH As Integer = 4

        Private ReadOnly _header As Byte()

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
                Return _header(2)
            End Get
        End Property

        Public ReadOnly Property IsEndOfTracks As Boolean
            Get
                Return SectorCount = &HFF
            End Get
        End Property

        Public ReadOnly Property IsFMTrack As Boolean
            Get
                Return (HeadAndFlags And &H80) <> 0
            End Get
        End Property

        Public ReadOnly Property PhysicalCylinder As Byte
            Get
                Return _header(1)
            End Get
        End Property

        Public ReadOnly Property PhysicalHead As Byte
            Get
                Return CByte(HeadAndFlags And 1)
            End Get
        End Property

        Public ReadOnly Property SectorCount As Byte
            Get
                Return _header(0)
            End Get
        End Property
        Public ReadOnly Property StoredCrcLow As Byte
            Get
                Return _header(3)
            End Get
        End Property

        Public Function ComputedCrc16() As UShort
            Return TD0Crc16.Compute(_header, 0, 3)
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
