Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats.MFM
    Module MFMLoader
        Public Function ImageLoad(Data() As Byte) As MFMFloppyImage
            Dim Image As MFMFloppyImage = Nothing

            Dim MFMImage As New MFMImage
            Dim Result = MFMImage.Load(Data)
            If Result Then
                Dim BytesPerSector As UInteger = Bitstream.GetBytesPerSector(MFMImage)
                Dim DiskFormat As FloppyDiskFormat = Bitstream.BitstreamGetImageFormat(MFMImage, BytesPerSector)
                If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                    Image = New MFMFloppyImage(MFMImage, DiskFormat, BytesPerSector)
                End If
            End If

            Return Image
        End Function
    End Module
End Namespace
