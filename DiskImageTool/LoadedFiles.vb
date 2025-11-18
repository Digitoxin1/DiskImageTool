Public Class LoadedFiles
    Private ReadOnly _FileNames As Dictionary(Of String, ImageData)

    Public Sub New()
        _FileNames = New Dictionary(Of String, ImageData)
    End Sub

    Public ReadOnly Property FileNames As Dictionary(Of String, ImageData)
        Get
            Return _FileNames
        End Get
    End Property

    Public Function Add(Key As String, FileName As String, FileType As ImageData.FileTypeEnum, Optional CompressedFile As String = "", Optional NewFileName As String = "") As ImageData
        If Not _FileNames.ContainsKey(Key) Then
            Dim ImageData As New ImageData(FileName) With {
                .FileType = FileType
            }
            If ImageData.FileType = ImageData.FileTypeEnum.Compressed Then
                ImageData.CompressedFile = CompressedFile
            ElseIf ImageData.FileType = ImageData.FileTypeEnum.NewImage Then
                ImageData.NewFileName = NewFileName
            End If

            _FileNames.Add(Key, ImageData)

            Return ImageData
        End If

        Return Nothing
    End Function
End Class
