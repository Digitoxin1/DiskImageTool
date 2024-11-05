Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace TC
        Module TCLoader
            Public Function ImageLoad(Data() As Byte) As TranscopyFloppyImage
                Dim Image As TranscopyFloppyImage = Nothing

                Dim tc = New TransCopyImage()
                Dim Result = tc.Load(Data)
                If Result Then
                    Dim DiskFormat = Bitstream.BitstreamGetImageFormat(tc)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New TranscopyFloppyImage(tc, DiskFormat)
                    End If
                End If

                Return Image
            End Function
        End Module
    End Namespace
End Namespace
