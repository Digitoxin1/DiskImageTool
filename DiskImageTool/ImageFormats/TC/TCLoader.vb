Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace TC
        Module TCLoader
            Public Function ImageLoad(Data() As Byte) As TranscopyFloppyImage
                Dim Image As TranscopyFloppyImage = Nothing

                Dim tc As New TransCopyImage()
                Dim Result = tc.Load(Data)
                If Result Then
                    Dim BytesPerSector As UInteger = Bitstream.GetBytesPerSector(tc)
                    Dim DiskFormat = Bitstream.BitstreamGetImageFormat(tc, BytesPerSector)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New TranscopyFloppyImage(tc, DiskFormat, BytesPerSector)
                    End If
                End If

                Return Image
            End Function
        End Module
    End Namespace
End Namespace
