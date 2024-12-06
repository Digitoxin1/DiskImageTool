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

    Public Function Add(Key As String, FileName As String, Compressed As Boolean, Optional CompressedFile As String = "") As ImageData
        If Not _FileNames.ContainsKey(Key) Then
            Dim ImageData = New ImageData(FileName)
            If Compressed Then
                ImageData.Compressed = True
                ImageData.CompressedFile = CompressedFile
            End If

            _FileNames.Add(Key, ImageData)

            Return ImageData
        End If

        Return Nothing
    End Function
End Class
