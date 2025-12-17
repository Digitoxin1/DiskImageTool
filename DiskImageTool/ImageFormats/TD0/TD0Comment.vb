Imports System.Text

Namespace ImageFormats.TD0
    Public NotInheritable Class TD0Comment
        Public Const HEADER_LENGTH As Integer = 10

        Private ReadOnly _data As Byte()
        Private ReadOnly _header As Byte()

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
                Return ReadUInt16LE(_header, 2)
            End Get
        End Property

        Public ReadOnly Property Day As Integer
            Get
                Return _header(6)
            End Get
        End Property

        Public ReadOnly Property Hour As Integer
            Get
                Return _header(7)
            End Get
        End Property

        Public ReadOnly Property Minute As Integer
            Get
                Return _header(8)
            End Get
        End Property

        Public ReadOnly Property Month As Integer
            Get
                Return _header(5) + 1
            End Get
        End Property

        Public ReadOnly Property RawData As Byte()
            Get
                Return _data
            End Get
        End Property

        Public ReadOnly Property Second As Integer
            Get
                Return _header(9)
            End Get
        End Property

        Public ReadOnly Property StoredCrc16 As UShort
            Get
                Return ReadUInt16LE(_header, 0)
            End Get
        End Property

        Public ReadOnly Property Text As String
            Get
                If _data Is Nothing OrElse _data.Length = 0 Then
                    Return ""
                End If
                ' NUL-terminated lines, convert NUL to CRLF
                Return Encoding.ASCII.GetString(_data).Replace(ChrW(0), vbCrLf).Trim()
            End Get
        End Property

        Public ReadOnly Property TotalLength As Integer
            Get
                Return HEADER_LENGTH + DataLength
            End Get
        End Property

        Public ReadOnly Property Year As Integer
            Get
                ' year since 1900
                Return 1900 + _header(4)
            End Get
        End Property

        Public Function CommentCrcValid() As Boolean
            Return StoredCrc16 = ComputedCrc16()
        End Function

        Public Function ComputedCrc16() As UShort
            Dim crc As UShort = 0US

            ' header bytes [2..9]
            For i = 2 To 9
                crc = TD0Crc16.Update(crc, _header(i))
            Next

            ' data bytes
            For Each b In _data
                crc = TD0Crc16.Update(crc, b)
            Next

            Return crc
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
    End Class
End Namespace
