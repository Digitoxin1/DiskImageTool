Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace MFM
        Module MFMLoader
            Public Function ImageLoad(Data() As Byte) As MFMFloppyImage
                Dim Image As MFMFloppyImage = Nothing

                Dim MFMImage = New MFMImage
                Dim Result = MFMImage.Load(Data)
                If Result Then
                    Dim DiskFormat As FloppyDiskFormat = Bitstream.BitstreamGetImageFormat(MFMImage)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New MFMFloppyImage(MFMImage, DiskFormat)
                    End If
                End If

                Return Image
            End Function
        End Module
    End Namespace
End Namespace
