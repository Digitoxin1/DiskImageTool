Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace PSI
        Module PSILoader
            Public Function PSIImageLoad(Data() As Byte) As PSIByteArray
                Dim Image As PSIByteArray = Nothing

                Dim PSI = New PSISectorImage()
                Dim Result = PSI.Import(Data)
                If Result Then
                    If PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_DD _
                        Or PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_HD _
                        Or PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_ED Then

                        Dim DiskFormat = PSIGetImageFormat(PSI)
                        If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                            Image = New PSIByteArray(PSI, DiskFormat)
                        End If
                    End If
                End If

                Return Image
            End Function

            Private Function PSIGetImageFormat(PSI As PSISectorImage) As FloppyDiskFormat
                Dim DiskFormat As FloppyDiskFormat
                Dim MaxSectors As Byte

                If PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_HD Then
                    MaxSectors = 18
                ElseIf PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_ED Then
                    MaxSectors = 36
                Else
                    MaxSectors = 9
                End If

                Dim SectorCount As Byte = 0

                For Each Sector In PSI.Sectors
                    If Sector.Sector >= 1 And Sector.Sector <= MaxSectors Then
                        If Sector.Sector > SectorCount Then
                            SectorCount = Sector.Sector
                        End If
                    End If
                Next

                If PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_DD Then
                    If PSI.CylinderCount >= 79 Then
                        DiskFormat = FloppyDiskFormat.Floppy720
                    Else
                        If PSI.HeadCount = 1 Then
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
                ElseIf PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_HD Then
                    If SectorCount > 15 Then
                        DiskFormat = FloppyDiskFormat.Floppy1440
                    Else
                        DiskFormat = FloppyDiskFormat.Floppy1200
                    End If
                Else
                    DiskFormat = FloppyDiskFormat.Floppy2880
                End If

                Return DiskFormat
            End Function
        End Module
    End Namespace
End Namespace