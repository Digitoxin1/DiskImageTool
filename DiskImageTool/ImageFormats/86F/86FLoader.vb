Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace _86F
        Module _86FLoader
            Public Function ImageLoad(Data() As Byte) As _86FByteArray
                Dim Image As _86FByteArray = Nothing

                Dim F86Image = New _86FImage()
                Dim Result = F86Image.Load(Data)
                If Result Then
                    Dim DiskFormat As FloppyDiskFormat = Bitstream.BitstreamGetImageFormat(F86Image)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New _86FByteArray(F86Image, DiskFormat)
                    End If
                End If

                Return Image
            End Function
        End Module
    End Namespace
End Namespace
