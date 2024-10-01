Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace _86F
        Module _86FLoader
            Public Function ImageLoad(Data() As Byte) As _86FByteArray
                Dim Image As _86FByteArray = Nothing

                Dim F86Image = New _86FImage()
                Dim Result = F86Image.Load(Data)
                If Result Then
                    Dim DiskFormat As FloppyDiskFormat = GetImageFormat(F86Image)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New _86FByteArray(F86Image, DiskFormat)
                    End If
                End If

                Return Image
            End Function

            Private Function GetImageFormat(F86Image As _86FImage) As FloppyDiskFormat
                Dim DiskFormat As FloppyDiskFormat
                Dim MaxSectors As Byte
                Dim F86Track As _86FTrack

                Dim SectorCount As Byte = 0

                For Track = 0 To F86Image.TrackCount - 1
                    For Side = 0 To F86Image.Sides - 1
                        F86Track = F86Image.GetTrack(Track, Side)
                        If F86Track IsNot Nothing AndAlso F86Track.MFMData IsNot Nothing Then
                            If F86Track.BitRate = BitRate.BitRate1000 Then
                                MaxSectors = 36
                            ElseIf F86Track.BitRate = BitRate.BitRate500 Then
                                MaxSectors = 18
                            Else
                                MaxSectors = 9
                            End If
                            For Each Sector In F86Track.MFMData.Sectors
                                If Sector.SectorId >= 1 And Sector.SectorId <= MaxSectors Then
                                    If Sector.SectorId > SectorCount Then
                                        SectorCount = Sector.SectorId
                                    End If
                                End If
                            Next
                        End If
                    Next
                Next

                If F86Image.TrackCount > 79 Then
                    If SectorCount > 15 Then
                        DiskFormat = FloppyDiskFormat.Floppy1440
                    ElseIf SectorCount > 9 Then
                        DiskFormat = FloppyDiskFormat.Floppy1200
                    Else
                        DiskFormat = FloppyDiskFormat.Floppy720
                    End If
                Else
                    If F86Image.Sides = 1 Then
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
