Namespace ImageFormats.TD0

    Public Class TD0Header
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
            Sides = 9
            Crc16 = 10
        End Enum

        Public Sub New()
            _header = New Byte(LENGTH - 1) {}
            ' Set default signature
            SignatureText = "TD"
        End Sub

        Public Sub New(fileBytes As Byte())
            _header = New Byte(LENGTH - 1) {}
            If fileBytes Is Nothing OrElse fileBytes.Length < LENGTH Then
                Return
            End If

            Array.Copy(fileBytes, 0, _header, 0, LENGTH)
        End Sub

        Public Property CheckSequence As Byte
            Get
                Return _header(Offset.CheckSequence)
            End Get
            Set(value As Byte)
                _header(Offset.CheckSequence) = value
            End Set
        End Property

        Public Property DataRate As TD0DataRate
            Get
                Return CType(CByte(_header(Offset.DataRate) And &H3), TD0DataRate)
            End Get
            Set(value As TD0DataRate)
                Dim b As Byte = _header(Offset.DataRate)

                b = CByte((b And Not &H3) Or (CByte(value) And &H3))

                _header(Offset.DataRate) = b
            End Set
        End Property

        Public Property DosAllocationFlag As Byte
            Get
                Return _header(Offset.DosAllocFlag)
            End Get
            Set(value As Byte)
                _header(Offset.DosAllocFlag) = value
            End Set
        End Property

        Public Property DriveType As TD0DriveType
            Get
                Return CType(_header(Offset.DriveType), TD0DriveType)
            End Get
            Set(value As TD0DriveType)
                _header(Offset.DriveType) = CByte(value)
            End Set
        End Property

        Public Property HasCommentBlock As Boolean
            Get
                Return (_header(Offset.Stepping) And &H80) <> 0
            End Get
            Set(value As Boolean)
                Dim b As Byte = _header(Offset.Stepping)

                b = If(value, CByte(b Or &H80), CByte(b And Not &H80))

                _header(Offset.Stepping) = b
            End Set
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

        Public Property IsSingleDensity As Boolean
            ' bit7 (old/legacy, sometimes unused)
            Get
                Return (_header(Offset.DataRate) And &H80) <> 0
            End Get
            Set(value As Boolean)
                Dim b As Byte = _header(Offset.DataRate)

                b = If(value, CByte(b Or &H80), CByte(b And Not &H80))

                _header(Offset.DataRate) = b
            End Set
        End Property

        Public ReadOnly Property IsValidSignature As Boolean
            ' "TD" or "td"
            Get
                Return SignatureText = "TD" OrElse SignatureText = "td"
            End Get
        End Property

        Public Property Sequence As Byte
            Get
                Return _header(Offset.Sequence)
            End Get
            Set(value As Byte)
                _header(Offset.Sequence) = value
            End Set
        End Property

        Public Property Sides As Byte
            ' 1=single, else=double (per notes)
            Get
                Return _header(Offset.Sides)
            End Get
            Set(value As Byte)
                _header(Offset.Sides) = value
            End Set
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

        Public Property Stepping As TD0Stepping
            Get
                Return CType(CByte(_header(Offset.Stepping) And &H3), TD0Stepping)
            End Get
            Set(value As TD0Stepping)
                Dim b As Byte = _header(Offset.Stepping)

                b = CByte((b And Not &H3) Or (CByte(value) And &H3))

                _header(Offset.Stepping) = b
            End Set
        End Property

        Public ReadOnly Property StoredCrc16 As UShort
            Get
                Return ReadUInt16LE(_header, Offset.Crc16)
            End Get
        End Property

        Public Property VersionMajor As Integer
            Get
                Return (VersionRaw \ 10) Mod 10
            End Get
            Set(value As Integer)
                If value < 0 OrElse value > 9 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value))
                End If

                Dim minor As Integer = VersionMinor
                VersionRaw = (value * 10) + minor
            End Set
        End Property

        Public Property VersionMinor As Integer
            Get
                Return VersionRaw Mod 10
            End Get
            Set(value As Integer)
                If value < 0 OrElse value > 9 Then
                    Throw New ArgumentOutOfRangeException(NameOf(value))
                End If

                Dim major As Integer = VersionMajor
                VersionRaw = (major * 10) + value
            End Set
        End Property

        Public Property VersionRaw As Byte
            Get
                Return _header(Offset.VersionRaw)
            End Get
            Set(value As Byte)
                _header(Offset.VersionRaw) = value
            End Set
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
    End Class

End Namespace
