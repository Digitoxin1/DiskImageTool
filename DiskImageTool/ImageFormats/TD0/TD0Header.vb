Namespace ImageFormats.TD0

    Public Structure TD0Header
        Public Const LENGTH As Integer = 12

        Private ReadOnly _header As Byte()

        Public Sub New(fileBytes As Byte())
            _header = New Byte(LENGTH - 1) {}
            If fileBytes Is Nothing OrElse fileBytes.Length < LENGTH Then
                Return
            End If

            Array.Copy(fileBytes, 0, _header, 0, _header.Length)
        End Sub

        Public ReadOnly Property CheckSequence As Byte
            Get
                Return _header(3)
            End Get
        End Property

        Public ReadOnly Property DataRate As Byte
            Get
                Return _header(5)
            End Get
        End Property

        Public ReadOnly Property DataRateCode As Integer
            ' 0=250,1=300,2=500 (lower 2 bits)
            Get
                Return CInt(DataRate And &H3)
            End Get
        End Property

        Public ReadOnly Property DosAllocationFlag As Byte
            Get
                Return _header(8)
            End Get
        End Property

        Public ReadOnly Property DriveType As Byte
            Get
                Return _header(6)
            End Get
        End Property

        Public ReadOnly Property HasCommentBlock As Boolean
            ' bit7 of stepping
            Get
                Return (Stepping And &H80) <> 0
            End Get
        End Property

        Public ReadOnly Property IsCompressed As Boolean
            ' "td" indicates compressed payload
            Get
                Return SignatureText = "td"
            End Get
        End Property

        Public ReadOnly Property IsSingleDensityGlobal As Boolean
            ' bit7 (old/legacy, sometimes unused)
            Get
                Return (DataRate And &H80) <> 0
            End Get
        End Property

        Public ReadOnly Property IsValidSignature As Boolean
            ' "TD" or "td"
            Get
                Return SignatureText = "TD" OrElse SignatureText = "td"
            End Get
        End Property

        Public ReadOnly Property Sequence As Byte
            Get
                Return _header(2)
            End Get
        End Property

        Public ReadOnly Property SidesField As Byte
            ' 1=single, else=double (per notes)
            Get
                Return _header(9)
            End Get
        End Property

        Public ReadOnly Property Signature0 As Byte
            Get
                Return _header(0)
            End Get
        End Property

        Public ReadOnly Property Signature1 As Byte
            Get
                Return _header(1)
            End Get
        End Property

        Public ReadOnly Property SignatureText As String
            Get
                Return ChrW(Signature0) & ChrW(Signature1)
            End Get
        End Property
        Public ReadOnly Property Stepping As Byte
            Get
                Return _header(7)
            End Get
        End Property

        Public ReadOnly Property StoredCrc16 As UShort
            Get
                Return ReadUInt16LE(_header, 10)
            End Get
        End Property

        Public ReadOnly Property VersionRaw As Byte
            Get
                Return _header(4)
            End Get
        End Property

        Public ReadOnly Property VersionMajor As Integer
            Get
                Return (VersionRaw \ 10) Mod 10
            End Get
        End Property

        Public ReadOnly Property VersionMinor As Integer
            Get
                Return VersionRaw Mod 10
            End Get
        End Property

        Public ReadOnly Property VersionString As String
            Get
                Return $"{VersionMajor}.{VersionMinor}"
            End Get
        End Property

        Public Function ComputedCrc16() As UShort
            Return TD0Crc16.Compute(_header, 0, 10)
        End Function

        Public Function CrcValid() As Boolean
            Return StoredCrc16 = ComputedCrc16()
        End Function

        Public Function GetBytes() As Byte()
            Dim b(LENGTH - 1) As Byte
            Array.Copy(_header, 0, b, 0, LENGTH)
            Return b
        End Function
    End Structure

End Namespace
