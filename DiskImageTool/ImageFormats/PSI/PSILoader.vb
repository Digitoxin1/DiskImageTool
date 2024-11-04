Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Namespace PSI
        Module PSILoader
            Public Function ImageLoad(Data() As Byte) As PSIByteArray
                Dim Image As PSIByteArray = Nothing

                Dim PSI = New PSISectorImage()
                Dim Result = PSI.Load(Data)
                If Result Then
                    If PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_DD _
                        Or PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_HD _
                        Or PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_ED Then

                        Dim DiskFormat = GetImageFormat(PSI)
                        If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                            Image = New PSIByteArray(PSI, DiskFormat)
                        End If
                    End If
                End If

                Return Image
            End Function

            Private Function GetImageFormat(PSI As PSISectorImage) As FloppyDiskFormat
                Dim DiskFormat As FloppyDiskFormat
                Dim SectorCount As Byte

                Dim TotalSize As UShort = 0
                For Each Sector In PSI.Sectors
                    If Sector.Track = 0 And Sector.Side = 0 Then
                        If Not Sector.HasDataCRCError Then
                            TotalSize += Sector.Size
                        End If
                    End If
                Next
                SectorCount = TotalSize \ 512

                If PSI.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_DD Then
                    If PSI.TrackCount >= 79 Then
                        DiskFormat = FloppyDiskFormat.Floppy720
                    Else
                        If PSI.SideCount = 1 Then
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