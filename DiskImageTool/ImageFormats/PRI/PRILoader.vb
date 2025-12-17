Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats.PRI
    Module PRILoader
        Public Function ImageLoad(Data() As Byte) As PRIFloppyImage
            Dim Image As PRIFloppyImage = Nothing

            Dim PRI As New PRIImage()
            Dim Result = PRI.Load(Data)
            If Result Then
                Dim BytesPerSector As UInteger = Bitstream.GetBytesPerSector(PRI)
                Dim DiskFormat = Bitstream.BitstreamGetImageFormat(PRI, BytesPerSector)
                If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                    Image = New PRIFloppyImage(PRI, DiskFormat, BytesPerSector)
                End If
            End If

            Return Image
        End Function
    End Module
End Namespace
