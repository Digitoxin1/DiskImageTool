Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace TC
        Module TCLoader
            Public Function ImageLoad(Data() As Byte) As TranscopyByteArray
                Dim Image As TranscopyByteArray = Nothing

                Dim tc = New TransCopyImage()
                Dim Result = tc.Load(Data)
                If Result Then
                    Dim DiskFormat = Bitstream.BitstreamGetImageFormat(tc)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New TranscopyByteArray(tc, DiskFormat)
                    End If
                End If

                Return Image
            End Function
        End Module
    End Namespace
End Namespace
