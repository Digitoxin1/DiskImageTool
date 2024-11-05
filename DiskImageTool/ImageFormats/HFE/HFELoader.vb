Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace HFE
        Module HFELoader
            Public Function ImageLoad(Data() As Byte) As HFEFloppyImage
                Dim Image As HFEFloppyImage = Nothing

                Dim HFEImage = New HFEImage
                Dim Result = HFEImage.Load(Data)
                If Result Then
                    Dim DiskFormat As FloppyDiskFormat = Bitstream.BitstreamGetImageFormat(HFEImage)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New HFEFloppyImage(HFEImage, DiskFormat)
                    End If
                End If

                Return Image
            End Function
        End Module
    End Namespace
End Namespace

