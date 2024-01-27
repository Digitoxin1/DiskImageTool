Public Class MyBitConverter
    Public Shared Function ToInt16(value As Byte(), bigEndien As Boolean) As Int16
        If bigEndien Then
            Return ToInt16(ToUInt16(value, bigEndien))
        Else
            Return BitConverter.ToInt16(value, 0)
        End If
    End Function

    Public Shared Function ToInt24(value As Byte(), bigEndien As Boolean) As Int32
        Return ToInt24(ToUInt24(value, bigEndien))
    End Function

    Public Shared Function ToInt32(value As Byte(), bigEndien As Boolean) As Int32
        If bigEndien Then
            Return ToInt32(ToUInt32(value, bigEndien))
        Else
            Return BitConverter.ToInt32(value, 0)
        End If
    End Function

    Public Shared Function ToInt64(value As Byte(), bigEndien As Boolean) As Int64
        If bigEndien Then
            Return ToInt64(ToUInt64(value, bigEndien))
        Else
            Return BitConverter.ToInt64(value, 0)
        End If
    End Function

    Public Shared Function ToByte(value As SByte) As Byte
        If value < 0 Then
            Return value + (2 ^ 8)
        Else
            Return value
        End If
    End Function

    Public Shared Function ToSByte(value As Byte) As SByte
        If value < (2 ^ 8 / 2) Then
            Return value
        Else
            Return value - (2 ^ 8)
        End If
    End Function

    Public Shared Function ToInt16(value As UInt16) As Int16
        If value < (2 ^ 16 / 2) Then
            Return value
        Else
            Return value - (2 ^ 16)
        End If
    End Function

    Public Shared Function ToInt24(value As UInt32) As Int32
        If value < (2 ^ 24 / 2) Then
            Return value
        Else
            Return value - (2 ^ 24)
        End If
    End Function

    Public Shared Function ToInt32(value As UInt32) As Int32
        If value < (2 ^ 32 / 2) Then
            Return value
        Else
            Return value - (2 ^ 32)
        End If
    End Function

    Public Shared Function ToInt64(value As UInt64) As Int64
        Return BitConverter.ToInt64(BitConverter.GetBytes(value), 0)
    End Function

    Public Shared Function ToUInt16(value As Byte(), bigEndien As Boolean) As UInt16
        If bigEndien Then
            Return CType(value(0), UInt16) << 8 Or value(1)
        Else
            Return BitConverter.ToUInt16(value, 0)
        End If
    End Function

    Public Shared Function ToUInt24(value As Byte(), bigEndien As Boolean) As UInt32
        If bigEndien Then
            Return CType(value(0), UInt32) << 16 Or CType(value(1), UInt16) << 8 Or value(2)
        Else
            Return CType(value(2), UInt32) << 16 Or CType(value(1), UInt16) << 8 Or value(0)
        End If
    End Function

    Public Shared Function ToUInt32(value As Byte(), bigEndien As Boolean) As UInt32
        If bigEndien Then
            Return CType(value(0), UInt32) << 24 Or CType(value(1), UInt32) << 16 Or CType(value(2), UInt16) << 8 Or value(3)
        Else
            Return BitConverter.ToUInt32(value, 0)
        End If
    End Function

    Public Shared Function ToUInt64(value As Byte(), bigEndien As Boolean) As UInt64
        If bigEndien Then
            Dim num As UInt64 = CType(value(0), UInt32) << 24 Or CType(value(1), UInt32) << 16 Or CType(value(2), UInt16) << 8 Or value(3)
            Dim num2 As UInt32 = CType(value(4), UInt32) << 24 Or CType(value(5), UInt32) << 16 Or CType(value(6), UInt16) << 8 Or value(7)

            Return num2 Or num << 32
        Else
            Return BitConverter.ToUInt64(value, 0)
        End If
    End Function
End Class
