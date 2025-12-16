Module FileIO
    Public Function FileOpenBinary(FilePath As String) As (Result As Boolean, Data As Byte(), ErrorMsg As String)
        Return TryIO(Function() IO.File.ReadAllBytes(FilePath))
    End Function

    Public Function FileOpenText(FilePath As String) As (Result As Boolean, Text As String, ErrorMsg As String)
        Return TryIO(Function() IO.File.ReadAllText(FilePath))
    End Function

    Public Function FileOpenText(FilePath As String, Encoding As Text.Encoding) As (Result As Boolean, Text As String, ErrorMsg As String)
        Return TryIO(Function() IO.File.ReadAllText(FilePath, Encoding))
    End Function

    Public Function FileWriteBinary(FilePath As String, Data As Byte()) As (Result As Boolean, ErrorMsg As String)
        Return TryIO(Sub()
                         EnsureParentDirectory(FilePath)
                         IO.File.WriteAllBytes(FilePath, Data)
                     End Sub)
    End Function

    Public Function FileWriteText(FilePath As String, Text As String) As (Result As Boolean, ErrorMsg As String)
        Return TryIO(Sub()
                         EnsureParentDirectory(FilePath)
                         IO.File.WriteAllText(FilePath, Text)
                     End Sub)
    End Function

    Public Function FileWriteText(FilePath As String, Text As String, Encoding As Text.Encoding) As (Result As Boolean, ErrorMsg As String)
        Return TryIO(Sub()
                         EnsureParentDirectory(FilePath)
                         IO.File.WriteAllText(FilePath, Text, Encoding)
                     End Sub)
    End Function

    Public Sub EnsureParentDirectory(FilePath As String)
        Dim Path = IO.Path.GetDirectoryName(FilePath)

        If String.IsNullOrEmpty(Path) Then
            Return
        End If

        IO.Directory.CreateDirectory(Path)
    End Sub

    Private Function TryIO(Of T)(op As Func(Of T)) As (Result As Boolean, Value As T, ErrorMsg As String)
        Try
            Return (True, op(), "")
        Catch ex As Exception
            Return (False, Nothing, ex.Message)
        End Try
    End Function

    Private Function TryIO(op As Action) As (Result As Boolean, ErrorMsg As String)
        Try
            op()
            Return (True, "")
        Catch ex As Exception
            Return (False, ex.Message)
        End Try
    End Function
End Module
