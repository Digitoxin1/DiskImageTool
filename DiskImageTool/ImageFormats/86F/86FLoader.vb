Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace D86F
        Module D86FLoader
            Public Function ImageLoad(Data() As Byte) As D86FFloppyImage
                Dim Image As D86FFloppyImage = Nothing

                Dim D86FImage = New D86FImage()
                Dim Result = D86FImage.Load(Data)
                If Result Then
                    Dim DiskFormat As FloppyDiskFormat = Bitstream.BitstreamGetImageFormat(D86FImage)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New D86FFloppyImage(D86FImage, DiskFormat)
                    End If
                End If

                Return Image
            End Function
        End Module
    End Namespace
End Namespace
