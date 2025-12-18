Imports System.Text

Namespace ImageFormats.TD0
    Public NotInheritable Class TD0Comment
        Public Const HEADER_LENGTH As Integer = 10
        Private Const CRC_END As Integer = Offset.Second
        Private Const CRC_START As Integer = Offset.DataLength
        Private _data As Byte()
        Private ReadOnly _header As Byte()

        Private Enum Offset As Integer
            Crc16 = 0
            DataLength = 2
            Year = 4
            Month0 = 5
            Day = 6
            Hour = 7
            Minute = 8
            Second = 9
        End Enum

        Public Sub New()
            _header = New Byte(HEADER_LENGTH - 1) {}
            _data = Array.Empty(Of Byte)()
        End Sub

        Public Sub New(imagebuf As Byte(), offset As Integer)
            _header = New Byte(HEADER_LENGTH - 1) {}
            _data = Array.Empty(Of Byte)()

            If imagebuf Is Nothing Then
                Return
            End If

            If offset < 0 OrElse (offset + HEADER_LENGTH) > imagebuf.Length Then
                Return
            End If

            Array.Copy(imagebuf, offset, _header, 0, HEADER_LENGTH)

            Dim len As Integer = DataLength
            Dim dataStart As Integer = offset + HEADER_LENGTH
            If len < 0 Then Return
            If (dataStart + len) > imagebuf.Length Then
                Return
            End If

            If len > 0 Then
                Dim tmp(len - 1) As Byte
                Array.Copy(imagebuf, dataStart, tmp, 0, len)
                _data = tmp
            End If
        End Sub

        Public ReadOnly Property DataLength As Integer
            Get
                Return ReadUInt16LE(_header, Offset.DataLength)
            End Get
        End Property

        Public Property Day As Integer
            Get
                Return _header(Offset.Day)
            End Get
            Set(value As Integer)
                _header(Offset.Day) = CByte(value)
            End Set
        End Property

        Public Property Hour As Integer
            Get
                Return _header(Offset.Hour)
            End Get
            Set(value As Integer)
                _header(Offset.Hour) = CByte(value)
            End Set
        End Property

        Public Property Minute As Integer
            Get
                Return _header(Offset.Minute)
            End Get
            Set(value As Integer)
                _header(Offset.Minute) = CByte(value)
            End Set
        End Property

        Public Property Month As Integer
            Get
                Return _header(Offset.Month0) + 1
            End Get
            Set(value As Integer)
                _header(Offset.Month0) = CByte(value - 1)
            End Set
        End Property

        Public ReadOnly Property RawData As Byte()
            Get
                Return _data
            End Get
        End Property

        Public Property Second As Integer
            Get
                Return _header(Offset.Second)
            End Get
            Set(value As Integer)
                _header(Offset.Second) = CByte(value)
            End Set
        End Property

        Public ReadOnly Property StoredCrc16 As UShort
            Get
                Return ReadUInt16LE(_header, Offset.Crc16)
            End Get
        End Property

        Public Property Text As String
            Get
                If _data Is Nothing OrElse _data.Length = 0 Then
                    Return ""
                End If
                ' NUL-terminated lines, convert NUL to CRLF
                Return Encoding.ASCII.GetString(_data).Replace(ChrW(0), vbCrLf).Trim()
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) Then
                    _data = Array.Empty(Of Byte)()
                Else
                    ' Normalize line endings, then convert CR/LF to NULs
                    Dim normalized As String = value.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)

                    Dim asciiBytes As Byte() = Encoding.ASCII.GetBytes(normalized.Replace(vbLf, ChrW(0)))

                    _data = asciiBytes
                End If

                WriteUInt16LE(_header, Offset.DataLength, _data.Length)
            End Set
        End Property

        Public ReadOnly Property TotalLength As Integer
            Get
                Return HEADER_LENGTH + DataLength
            End Get
        End Property

        Public Property Year As Integer
            Get
                ' year since 1900
                Return 1900 + _header(Offset.Year)
            End Get
            Set(value As Integer)
                _header(Offset.Year) = CByte(value - 1900)
            End Set
        End Property

        Public Function ComputedCrc16() As UShort
            Dim crc As UShort = 0US

            For i = CRC_START To CRC_END
                crc = TD0Crc16.Update(crc, _header(i))
            Next

            ' data bytes
            For Each b In _data
                crc = TD0Crc16.Update(crc, b)
            Next

            Return crc
        End Function

        Public Function CrcValid() As Boolean
            Return StoredCrc16 = ComputedCrc16()
        End Function

        Public Function GetBytes() As Byte()
            Dim totalLen As Integer = TotalLength
            Dim buf(totalLen - 1) As Byte
            ' header
            Array.Copy(_header, 0, buf, 0, HEADER_LENGTH)
            ' data
            Array.Copy(_data, 0, buf, HEADER_LENGTH, _data.Length)

            Return buf
        End Function

        Public Function GetTimestamp() As DateTime?
            Try
                ' TD0 stores month as 0–11
                Dim y As Integer = Year
                Dim m As Integer = Month
                Dim d As Integer = Day
                Dim h As Integer = Hour
                Dim mi As Integer = Minute
                Dim s As Integer = Second

                ' Basic sanity checks (cheap + effective)
                If y < 1900 OrElse y > 2100 Then Return Nothing
                If m < 1 OrElse m > 12 Then Return Nothing
                If d < 1 OrElse d > 31 Then Return Nothing
                If h < 0 OrElse h > 23 Then Return Nothing
                If mi < 0 OrElse mi > 59 Then Return Nothing
                If s < 0 OrElse s > 59 Then Return Nothing

                Return New DateTime(y, m, d, h, mi, s, DateTimeKind.Unspecified)

            Catch
                ' Handles invalid day/month combos (Feb 30, etc)
                Return Nothing
            End Try
        End Function

        Public Sub RefreshStoredCrc16()
            WriteUInt16LE(_header, Offset.Crc16, ComputedCrc16())
        End Sub

        Public Sub SetTimestamp(ts As DateTime)
            If ts.Year < 1900 OrElse ts.Year > 2155 Then
                Throw New ArgumentOutOfRangeException(NameOf(ts), ts, "TD0 timestamp year must be between 1900 and 2155.")
            End If

            Year = ts.Year
            Month = ts.Month
            Day = ts.Day
            Hour = ts.Hour
            Minute = ts.Minute
            Second = ts.Second
        End Sub
    End Class
End Namespace
