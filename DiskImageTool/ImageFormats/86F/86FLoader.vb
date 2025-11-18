Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace D86F
        Module D86FLoader
            Public Function ImageLoad(Data() As Byte) As D86FFloppyImage
                Dim Image As D86FFloppyImage = Nothing

                Dim D86FImage As New D86FImage()
                Dim Result = D86FImage.Load(Data)
                If Result Then
                    Dim BytesPerSector As UInteger = Bitstream.GetBytesPerSector(D86FImage)
                    Dim DiskFormat As FloppyDiskFormat = Bitstream.BitstreamGetImageFormat(D86FImage, BytesPerSector)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New D86FFloppyImage(D86FImage, DiskFormat, BytesPerSector)
                    End If
                End If

                Return Image
            End Function
        End Module
    End Namespace
End Namespace
