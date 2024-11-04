Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace HFE
        Module HFELoader
            Public Function ImageLoad(Data() As Byte) As HFEByteArray
                Dim Image As HFEByteArray = Nothing

                Dim HFEImage = New HFEImage
                Dim Result = HFEImage.Load(Data)
                If Result Then
                    Dim DiskFormat As FloppyDiskFormat = Bitstream.BitstreamGetImageFormat(HFEImage)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New HFEByteArray(HFEImage, DiskFormat)
                    End If
                End If

                Return Image
            End Function
        End Module
    End Namespace
End Namespace

