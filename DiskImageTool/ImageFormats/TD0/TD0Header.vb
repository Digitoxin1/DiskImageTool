Namespace ImageFormats.TD0

    Public Structure TD0Header
        Public Const LENGTH As Integer = 12
        Private Const CHECKSUM_LENGTH As Integer = 10

        Private ReadOnly _header As Byte()

        Private Enum Offset As Integer
            Signature0 = 0
            Signature1 = 1
            Sequence = 2
            CheckSequence = 3
            VersionRaw = 4
            DataRate = 5
            DriveType = 6
            Stepping = 7
            DosAllocFlag = 8
            SidesField = 9
            Crc16 = 10
        End Enum

        Public Sub New(fileBytes As Byte())
            _header = New Byte(LENGTH - 1) {}
            If fileBytes Is Nothing OrElse fileBytes.Length < LENGTH Then
                Return
            End If

            Array.Copy(fileBytes, 0, _header, 0, LENGTH)
        End Sub

        Public ReadOnly Property CheckSequence As Byte
            Get
                Return _header(Offset.CheckSequence)
            End Get
        End Property

        Public ReadOnly Property DataRate As Byte
            Get
                Return _header(Offset.DataRate)
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
                Return _header(Offset.DosAllocFlag)
            End Get
        End Property

        Public ReadOnly Property DriveType As Byte
            Get
                Return _header(Offset.DriveType)
            End Get
        End Property

        Public ReadOnly Property HasCommentBlock As Boolean
            ' bit7 of stepping
            Get
                Return (Stepping And &H80) <> 0
            End Get
        End Property

        Public Property IsCompressed As Boolean
            ' "td" indicates compressed payload
            Get
                Return SignatureText = "td"
            End Get
            Set(value As Boolean)
                SignatureText = If(value, "td", "TD")
            End Set
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
                Return _header(Offset.Sequence)
            End Get
        End Property

        Public ReadOnly Property SidesField As Byte
            ' 1=single, else=double (per notes)
            Get
                Return _header(Offset.SidesField)
            End Get
        End Property

        Public ReadOnly Property Signature0 As Byte
            Get
                Return _header(Offset.Signature0)
            End Get
        End Property

        Public ReadOnly Property Signature1 As Byte
            Get
                Return _header(Offset.Signature1)
            End Get
        End Property

        Public Property SignatureText As String
            Get
                Return ChrW(Signature0) & ChrW(Signature1)
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) OrElse value.Length < 2 Then
                    Return
                End If

                _header(Offset.Signature0) = CByte(AscW(value(0)))
                _header(Offset.Signature1) = CByte(AscW(value(1)))
            End Set
        End Property
        Public ReadOnly Property Stepping As Byte
            Get
                Return _header(Offset.Stepping)
            End Get
        End Property

        Public ReadOnly Property StoredCrc16 As UShort
            Get
                Return ReadUInt16LE(_header, Offset.Crc16)
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

        Public ReadOnly Property VersionRaw As Byte
            Get
                Return _header(Offset.VersionRaw)
            End Get
        End Property
        Public ReadOnly Property VersionString As String
            Get
                Return $"{VersionMajor}.{VersionMinor}"
            End Get
        End Property

        Public Function ComputedCrc16() As UShort
            Return TD0Crc16.Compute(_header, 0, CHECKSUM_LENGTH)
        End Function

        Public Function CrcValid() As Boolean
            Return StoredCrc16 = ComputedCrc16()
        End Function

        Public Function GetBytes() As Byte()
            Dim b(LENGTH - 1) As Byte
            Array.Copy(_header, 0, b, 0, LENGTH)
            Return b
        End Function

        Public Sub RefreshStoredCrc16()
            WriteUInt16LE(_header, Offset.Crc16, ComputedCrc16())
        End Sub
    End Structure

End Namespace
