Imports System.Security.Cryptography
Imports System.Text

Module Functions
    Public Sub ListViewAddColumn(LV As ListView, Name As String, Text As String, Width As Integer, Index As Integer)
        Dim Column As New ColumnHeader With {
            .Name = Name,
            .Width = Width,
            .Text = Text
        }
        LV.Columns.Insert(Index, Column)
    End Sub

    Public Sub ListViewDoubleBuffer(lv As ListView)
        lv.GetType() _
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic) _
            .SetValue(lv, True, Nothing)
    End Sub

    Public Function MD5Hash(Data() As Byte) As String
        Using hasher As MD5 = MD5.Create()
            Dim dbytes As Byte() = hasher.ComputeHash(Data)
            Dim sBuilder As New StringBuilder()

            For n As Integer = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("X2"))
            Next n

            Return sBuilder.ToString
        End Using
    End Function

    Public Function SHA1Hash(Data() As Byte) As String
        Using hasher As SHA1 = SHA1.Create()
            Dim dbytes As Byte() = hasher.ComputeHash(Data)
            Dim sBuilder As New StringBuilder()

            For n As Integer = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("X2"))
            Next n

            Return sBuilder.ToString
        End Using
    End Function
End Module
