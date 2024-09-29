Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace TC
        Module TCLoader
            Public Function TranscopyImageLoad(Data() As Byte) As TranscopyByteArray
                Dim Image As TranscopyByteArray = Nothing

                Dim tc = New TransCopyImage()
                Dim Result = tc.Initialize(Data)
                If Result Then
                    Dim DiskFormat = TranscopyGetImageFormat(tc)
                    If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                        Image = New TranscopyByteArray(tc, DiskFormat)
                    End If
                End If

                Return Image
            End Function

            Private Function TranscopyGetImageFormat(tc As TransCopyImage) As FloppyDiskFormat
                Dim DiskFormat As FloppyDiskFormat
                Dim MaxSectors As Byte
                Dim TranscopyCylinder As TransCopyCylinder

                Dim SectorCount As Byte = 0

                For Cylinder = 0 To tc.CylinderEnd
                    For Side = 0 To tc.Sides - 1
                        TranscopyCylinder = tc.GetCylinder(Cylinder, Side)
                        If TranscopyCylinder.MFMData IsNot Nothing Then
                            If TranscopyCylinder.TrackType = TransCopyDiskType.MFMDoubleDensity Or TranscopyCylinder.TrackType = TransCopyDiskType.MFMDoubleDensity360RPM Then
                                MaxSectors = 9
                            Else
                                MaxSectors = 18
                            End If
                            For Each Sector In TranscopyCylinder.MFMData.Sectors
                                If Sector.SectorId >= 1 And Sector.SectorId <= MaxSectors Then
                                    If Sector.SectorId > SectorCount Then
                                        SectorCount = Sector.SectorId
                                    End If
                                End If
                            Next
                        End If
                    Next
                Next

                If tc.CylinderEnd < 45 And SectorCount > 9 Then
                    SectorCount = 9
                End If

                If tc.CylinderEnd >= 79 Then
                    If SectorCount > 15 Then
                        DiskFormat = FloppyDiskFormat.Floppy1440
                    ElseIf SectorCount > 9 Then
                        DiskFormat = FloppyDiskFormat.Floppy1200
                    Else
                        DiskFormat = FloppyDiskFormat.Floppy720
                    End If
                Else
                    If tc.Sides = 1 Then
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
