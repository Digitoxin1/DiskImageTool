Namespace ImageFormats.TD0
    <Flags>
    Public Enum TD0SectorFlags As Byte
        None = 0
        Duplicated = &H1
        CrcError = &H2
        DeletedData = &H4
        DosSkipped = &H10 ' no data block follows; fill typically F6
        NoData = &H20     ' ID but no data; no data block follows
        DataNoId = &H40   ' data but no ID (bogus header)
    End Enum

    Public Enum TD0EncodingMethod As Byte
        Raw = 0
        Repeat2BytePattern = 1
        Rle = 2
    End Enum

    Public Enum TD0DataRate As Byte
        Rate250Kbps = 0
        Rate300Kbps = 1
        Rate500Kbps = 2
    End Enum

    Public Enum TD0DriveType As Byte
        Drive5_25_96TPI = 0
        Drive360K = 1
        Drive1200K = 2
        Drive720K = 3
        Drive1440K = 4
        Drive8_Inch = 5
        Drive3_5_Unknown = 6
    End Enum

    Public Enum TD0Stepping As Byte
        SteppingSingle = 0
        SteppingDouble = 1
        SteppingEvenOnly = 2
    End Enum

    Friend Module TD0Helpers
        Friend Const TD0_MAX_BUFSZ As Integer = 1024 * 1024 * 4 ' 4 MiB
        Friend Function ReadUInt16LE(buf As Byte(), offset As Integer) As Integer
            Return CInt(buf(offset)) Or (CInt(buf(offset + 1)) << 8)
        End Function

        Friend Sub WriteUInt16LE(buf As Byte(), offset As Integer, value As UShort)
            buf(offset) = CByte(value And &HFFUS)
            buf(offset + 1) = CByte((value >> 8) And &HFFUS)
        End Sub

        Friend Sub WriteUInt16LE(buf As Byte(), offset As Integer, value As Integer)
            buf(offset) = CByte(value And &HFF)
            buf(offset + 1) = CByte((value >> 8) And &HFF)
        End Sub

        Friend Function SetFlagByte(original As Byte, flag As Byte, enabled As Boolean) As Byte
            If enabled Then
                Return CByte(original Or flag)
            Else
                Return CByte(original And Not flag)
            End If
        End Function
    End Module
End Namespace
