Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace PRI
        Module PRILoader
            Public Function ImageLoad(Data() As Byte) As PRIFloppyImage
                Dim Image As PRIFloppyImage = Nothing

                Dim PRI = New PRIImage()
                Dim Result = PRI.Load(Data)
                If Result Then
                    Dim DiskFormat = Bitstream.BitstreamGetImageFormat(PRI)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New PRIFloppyImage(PRI, DiskFormat)
                    End If
                End If

                Return Image
            End Function
        End Module
    End Namespace
End Namespace
