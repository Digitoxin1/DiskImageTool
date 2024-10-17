Imports System.Security.Cryptography
Imports System.Text

Module HashFunctions
    Public Function HashBytesToString(b() As Byte) As String
        Dim sBuilder As New StringBuilder()

        For n As Integer = 0 To b.Length - 1
            sBuilder.Append(b(n).ToString("X2"))
        Next n

        Return sBuilder.ToString
    End Function

    Public Function CRC32Hash(Data() As Byte) As String
        Using Hasher As CRC32Hash = DiskImageTool.CRC32Hash.Create()
            Return HashBytesToString(Hasher.ComputeHash(Data))
        End Using
    End Function

    Public Function MD5Hash(Data() As Byte) As String
        Using Hasher As MD5 = MD5.Create()
            Return HashBytesToString(Hasher.ComputeHash(Data))
        End Using
    End Function

    Public Function SHA1Hash(Data() As Byte) As String
        Using Hasher As SHA1 = SHA1.Create()
            Return HashBytesToString(Hasher.ComputeHash(Data))
        End Using
    End Function
End Module
