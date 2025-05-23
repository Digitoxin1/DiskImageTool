Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace IMD
        Module IMDLoader
            Public Function ImageLoad(Data() As Byte) As IMDFloppyImage
                Dim Image As IMDFloppyImage = Nothing

                Dim IMD = New IMDImage()
                Dim Result = IMD.Load(Data)
                If Result Then
                    Dim BytesPerSector As UInteger = Bitstream.GetBytesPerSector(IMD)
                    Dim DiskFormat = GetImageFormat(IMD)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New IMDFloppyImage(IMD, DiskFormat, BytesPerSector)
                    End If
                End If

                Return Image
            End Function

            Private Function GetImageFormat(IMD As IMDImage) As FloppyDiskFormat
                Dim DiskFormat As FloppyDiskFormat
                Dim SectorCount As Byte

                Dim TotalSize As UShort = 0
                If IMD.TrackCount > 0 Then
                    For Each Sector In IMD.Tracks.Item(0).Sectors
                        If Not Sector.ChecksumError Then
                            TotalSize += Sector.Data.Length
                        End If
                    Next
                End If
                SectorCount = TotalSize \ 512

                If IMD.TrackCount >= 79 Then
                    If SectorCount >= 36 Then
                        DiskFormat = FloppyDiskFormat.Floppy2880
                    ElseIf SectorCount >= 18 Then
                        DiskFormat = FloppyDiskFormat.Floppy1440
                    ElseIf SectorCount >= 15 Then
                        DiskFormat = FloppyDiskFormat.Floppy1200
                    Else
                        DiskFormat = FloppyDiskFormat.Floppy720
                    End If
                Else
                    If IMD.SideCount = 1 Then
                        If SectorCount < 9 Then
                            DiskFormat = FloppyDiskFormat.Floppy160
                        Else
                            DiskFormat = FloppyDiskFormat.Floppy180
                        End If
                    Else
                        If SectorCount < 9 Then
                            DiskFormat = FloppyDiskFormat.Floppy320
                        Else
                            DiskFormat = FloppyDiskFormat.Floppy360
                        End If
                    End If
                End If

                Return DiskFormat
            End Function
        End Module
    End Namespace
End Namespace
