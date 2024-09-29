
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace MFM
        Module MFMLoader
            Public Function MFMImageLoad(Data() As Byte) As MFMByteArray
                Dim Image As MFMByteArray = Nothing

                Dim MFMImage = New MFMImage
                Dim Result = MFMImage.Load(Data)
                If Result Then
                    Dim DiskFormat As FloppyDiskFormat = MFMGetImageFormat(MFMImage)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New MFMByteArray(MFMImage, DiskFormat)
                    End If
                End If

                Return Image
            End Function

            Private Function MFMGetImageFormat(MFMImage As MFMImage) As FloppyDiskFormat
                Dim DiskFormat As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown
                Dim MaxSectors As Byte
                Dim MFMTrack As MFMTrack

                Dim Bitrate As UShort = (Math.Round(MFMImage.BitRate / 50) * 50)

                If Bitrate = 500 Then
                    MaxSectors = 18
                ElseIf Bitrate = 1000 Then
                    MaxSectors = 36
                Else
                    MaxSectors = 9
                End If

                Dim SectorCount As Byte = 0

                For Track = 0 To MFMImage.TrackCount - 1 Step MFMImage.TrackStep
                    For Side = 0 To MFMImage.SideCount - 1
                        MFMTrack = MFMImage.GetTrack(Track, Side)
                        If MFMTrack.MFMData IsNot Nothing Then
                            For Each Sector In MFMTrack.MFMData.Sectors
                                If Sector.SectorId >= 1 And Sector.SectorId <= MaxSectors Then
                                    If Sector.SectorId > SectorCount Then
                                        SectorCount = Sector.SectorId
                                    End If
                                End If
                            Next
                        End If
                    Next
                Next

                If MFMImage.TrackCount \ MFMImage.TrackStep < 45 And SectorCount > 9 Then
                    SectorCount = 9
                End If

                If MFMImage.TrackCount \ MFMImage.TrackStep >= 79 Then
                    If SectorCount > 15 Then
                        DiskFormat = FloppyDiskFormat.Floppy1440
                    ElseIf SectorCount > 9 Then
                        DiskFormat = FloppyDiskFormat.Floppy1200
                    Else
                        DiskFormat = FloppyDiskFormat.Floppy720
                    End If
                Else
                    If MFMImage.SideCount = 1 Then
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
