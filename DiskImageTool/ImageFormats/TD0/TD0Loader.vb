Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats.TD0

    Module TD0Loader

        Public Function ImageLoad(data() As Byte) As TD0FloppyImage
            Dim img As TD0FloppyImage = Nothing

            Dim td0 As New TD0Image()
            If td0.Load(data) Then
                Dim bytesPerSector As UInteger = Bitstream.GetBytesPerSector(td0)
                Dim diskFormat As FloppyDiskFormat = GetImageFormat(td0)

                If diskFormat <> FloppyDiskFormat.FloppyUnknown Then
                    img = New TD0FloppyImage(td0, diskFormat, bytesPerSector)
                End If
            End If

            Return img
        End Function

        Private Function GetImageFormat(td0 As TD0Image) As FloppyDiskFormat
            Dim total As UInteger = 0

            If td0.Tracks IsNot Nothing AndAlso td0.Tracks.Count > 0 Then
                For Each s In td0.Tracks(0).Sectors
                    If s IsNot Nothing AndAlso Not s.Header.NoData AndAlso Not s.Header.HasCrcError AndAlso s.Data IsNot Nothing Then
                        total += CUInt(s.Data.Length)
                    End If
                Next
            End If

            Dim sectorCount As UInteger = total \ 512UI

            If td0.TrackCount >= 79 Then
                If sectorCount >= 36 Then
                    Return FloppyDiskFormat.Floppy2880
                End If
                If sectorCount >= 18 Then
                    Return FloppyDiskFormat.Floppy1440
                End If
                If sectorCount >= 15 Then
                    Return FloppyDiskFormat.Floppy1200
                End If

                Return FloppyDiskFormat.Floppy720
            Else
                If td0.SideCount <= 1 Then
                    If sectorCount < 9 Then
                        Return FloppyDiskFormat.Floppy160
                    Else
                        Return FloppyDiskFormat.Floppy180
                    End If
                Else
                    If sectorCount < 9 Then
                        Return FloppyDiskFormat.Floppy320
                    Else
                        Return FloppyDiskFormat.Floppy360
                    End If
                End If
            End If
        End Function
    End Module
End Namespace
