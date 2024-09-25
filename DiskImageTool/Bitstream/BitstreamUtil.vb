Module BitstreamUtil
    Private Function CheckDoubleSizeSectors(Sectors As List(Of IBM_MFM.MFMSector)) As Boolean
        Dim Result As Boolean = True
        Dim SectorCount As UShort

        SectorCount = 0
        For Each MFMSector In Sectors
            If MFMSector.Data IsNot Nothing Then
                If MFMSector.SectorId >= 1 And MFMSector.SectorId <= 8 Then
                    If MFMSector.SectorId > 4 Then
                        Result = False
                        Exit For
                    ElseIf MFMSector.GetSizeBytes <> 1024 Then
                        Result = False
                        Exit For
                    Else
                        SectorCount += 1
                    End If
                End If
            End If
        Next
        If SectorCount <> 4 Then
            Result = False
        End If

        Return Result
    End Function

    Private Function GetImageOffset(ImageParams As DiskImage.FloppyDiskParams, Track As UShort, Side As Byte, Sector As UShort) As UInteger
        Dim Offset = Track * ImageParams.NumberOfHeads * ImageParams.SectorsPerTrack + ImageParams.SectorsPerTrack * Side + Sector

        Return Offset * 512
    End Function

    Public Function PSIGetImageType(PSI As PSI_Image.PSISectorImage) As DiskImage.FloppyDiskType
        Dim DiskType As DiskImage.FloppyDiskType
        Dim MaxSectors As Byte

        If PSI.Header.DefaultSectorFormat = PSI_Image.DefaultSectorFormat.IBM_MFM_DD Then
            MaxSectors = 9
        ElseIf PSI.Header.DefaultSectorFormat = PSI_Image.DefaultSectorFormat.IBM_MFM_HD Then
            MaxSectors = 18
        ElseIf PSI.Header.DefaultSectorFormat = PSI_Image.DefaultSectorFormat.IBM_MFM_ED Then
            MaxSectors = 36
        End If

        Dim SectorCount As Byte = 0
        Dim CylinderCount As UShort = 0
        Dim HeadCount As UShort = 0

        For Each Sector In PSI.Sectors
            If Sector.Sector >= 1 And Sector.Sector <= MaxSectors Then
                If Sector.Sector > SectorCount Then
                    SectorCount = Sector.Sector
                End If
            End If
            If Sector.Cylinder + 1 > CylinderCount Then
                CylinderCount = Sector.Cylinder + 1
            End If
            If Sector.Head + 1 > HeadCount Then
                HeadCount = Sector.Head + 1
            End If
        Next

        If PSI.Header.DefaultSectorFormat = PSI_Image.DefaultSectorFormat.IBM_MFM_DD Then
            If CylinderCount >= 79 Then
                DiskType = DiskImage.FloppyDiskType.Floppy720
            Else
                If HeadCount = 1 Then
                    If SectorCount < 9 Then
                        DiskType = DiskImage.FloppyDiskType.Floppy160
                    Else
                        DiskType = DiskImage.FloppyDiskType.Floppy180
                    End If
                Else
                    If SectorCount < 9 Then
                        DiskType = DiskImage.FloppyDiskType.Floppy320
                    Else
                        DiskType = DiskImage.FloppyDiskType.Floppy360
                    End If
                End If
            End If
        ElseIf PSI.Header.DefaultSectorFormat = PSI_Image.DefaultSectorFormat.IBM_MFM_HD Then
            If SectorCount > 15 Then
                DiskType = DiskImage.FloppyDiskType.Floppy1440
            Else
                DiskType = DiskImage.FloppyDiskType.Floppy1200
            End If
        Else
            DiskType = DiskImage.FloppyDiskType.Floppy2880

        End If

        Return DiskType
    End Function

    Public Function TranscopyGetImageType(tc As Transcopy.TransCopyImage) As DiskImage.FloppyDiskType
        Dim DiskType As DiskImage.FloppyDiskType
        Dim MaxSectors As Byte

        Dim SectorCount As Byte = 0
        For Each Cylinder In tc.Cylinders
            If Cylinder.DecodedData IsNot Nothing Then
                If Cylinder.TrackType = Transcopy.TransCopyDiskType.MFMDoubleDensity Or Cylinder.TrackType = Transcopy.TransCopyDiskType.MFMDoubleDensity360RPM Then
                    MaxSectors = 9
                Else
                    MaxSectors = 18
                End If
                For Each Sector In Cylinder.DecodedData.Sectors
                    If Sector.SectorId >= 1 And Sector.SectorId <= MaxSectors Then
                        If Sector.SectorId > SectorCount Then
                            SectorCount = Sector.SectorId
                        End If
                    End If
                Next
            End If
        Next

        If tc.CylinderEnd < 45 And SectorCount > 9 Then
            SectorCount = 9
        End If

        If tc.CylinderEnd >= 79 Then
            If SectorCount > 15 Then
                DiskType = DiskImage.FloppyDiskType.Floppy1440
            ElseIf SectorCount > 9 Then
                DiskType = DiskImage.FloppyDiskType.Floppy1200
            Else
                DiskType = DiskImage.FloppyDiskType.Floppy720
            End If
        Else
            If tc.Sides = 1 Then
                If SectorCount < 9 Then
                    DiskType = DiskImage.FloppyDiskType.Floppy160
                Else
                    DiskType = DiskImage.FloppyDiskType.Floppy180
                End If
            Else
                If SectorCount < 9 Then
                    DiskType = DiskImage.FloppyDiskType.Floppy320
                Else
                    DiskType = DiskImage.FloppyDiskType.Floppy360
                End If
            End If
        End If

        Return DiskType
    End Function

    Public Function PSIToSectorImage(PSI As PSI_Image.PSISectorImage, DiskType As DiskImage.FloppyDiskType) As SectorImageData
        Dim ImageSize = DiskImage.GetFloppyDiskSize(DiskType)
        Dim ImageParams = DiskImage.GetFloppyDiskParams(DiskType)

        Dim SectorImageData As New SectorImageData(ImageSize)

        Dim TrackCount = Int(ImageParams.SectorCountSmall / ImageParams.SectorsPerTrack / ImageParams.NumberOfHeads)

        For Each PSISector In PSI.Sectors
            Dim MaxSize As UInteger = ImageParams.BytesPerSector
            Dim MaxSectors As UShort = ImageParams.SectorsPerTrack

            If Not PSISector.IsAlternateSector Then
                If PSISector.Cylinder < TrackCount And PSISector.Head < ImageParams.NumberOfHeads Then
                    If PSISector.Sector >= 1 And PSISector.Sector <= MaxSectors Then
                        Dim SectorId = PSISector.Sector - 1
                        Dim Offset = GetImageOffset(ImageParams, PSISector.Cylinder, PSISector.Head, SectorId)
                        Dim Sector = Offset \ ImageParams.BytesPerSector
                        If PSISector.Size <= MaxSize Then
                            Dim Size As UShort
                            Dim Data() As Byte
                            If PSISector.IsCompressed Then
                                Data = New Byte(PSISector.Size - 1) {}
                                For i = 0 To Data.Length - 1
                                    Data(i) = PSISector.CompressedSectorData
                                Next
                                Size = PSISector.Size
                            Else
                                Data = PSISector.Data
                                Size = Math.Min(PSISector.Size, PSISector.Data.Length)
                            End If
                            Array.Copy(Data, 0, SectorImageData.Data, Offset, Size)
                            If Size <> ImageParams.BytesPerSector Then
                                If Not SectorImageData.ProtectedSectors.Contains(Sector) Then
                                    SectorImageData.ProtectedSectors.Add(Sector)
                                End If
                            End If
                        Else
                            If Not SectorImageData.ProtectedSectors.Contains(Sector) Then
                                SectorImageData.ProtectedSectors.Add(Sector)
                            End If
                        End If
                    End If
                End If
            End If
        Next

        Return SectorImageData
    End Function

    Public Function TranscopyToSectorImage(tc As Transcopy.TransCopyImage, DiskType As DiskImage.FloppyDiskType) As SectorImageData
        Dim ImageSize = DiskImage.GetFloppyDiskSize(DiskType)
        Dim ImageParams = DiskImage.GetFloppyDiskParams(DiskType)

        Dim SectorImageData As New SectorImageData(ImageSize)

        Dim TrackCount = Int(ImageParams.SectorCountSmall / ImageParams.SectorsPerTrack / ImageParams.NumberOfHeads)

        For Each Cylinder In tc.Cylinders
            Dim MaxSize As UInteger = ImageParams.BytesPerSector
            Dim MaxSectors As UShort = ImageParams.SectorsPerTrack
            If Cylinder.DecodedData IsNot Nothing Then
                If Cylinder.Track < TrackCount And Cylinder.Side < ImageParams.NumberOfHeads Then
                    For Each MFMSector In Cylinder.DecodedData.Sectors
                        If MFMSector.DAMFound Then
                            If MFMSector.SectorId >= 1 And MFMSector.SectorId <= MaxSectors Then
                                Dim SectorId = MFMSector.SectorId - 1
                                Dim Offset = GetImageOffset(ImageParams, Cylinder.Track, Cylinder.Side, SectorId)
                                Dim Sector = Offset \ ImageParams.BytesPerSector
                                Dim Size = MFMSector.GetSizeBytes
                                Dim ValidChecksum = MFMSector.DataChecksum = MFMSector.CalculateDataChecksum
                                If MFMSector.DAM = IBM_MFM.MFMAddressMark.Data Then
                                    Array.Copy(MFMSector.Data, 0, SectorImageData.Data, Offset, Math.Min(Size, MaxSize))
                                    If Not MFMSector.Overlaps And Size = ImageParams.BytesPerSector And ValidChecksum Then
                                        SectorImageData.SectorMap(Sector) = Cylinder.BitstreamOffset + MFMSector.DataOffset
                                    Else
                                        If Not SectorImageData.ProtectedSectors.Contains(Sector) Then
                                            SectorImageData.ProtectedSectors.Add(Sector)
                                        End If
                                    End If
                                Else
                                    If Not SectorImageData.ProtectedSectors.Contains(Sector) Then
                                        SectorImageData.ProtectedSectors.Add(Sector)
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If
            End If
        Next

        Return SectorImageData
    End Function

    Public Class SectorImageData
        Public Sub New(ImageSize As UInteger)
            _Data = New Byte(ImageSize - 1) {}
            _SectorMap = New UInteger(ImageSize \ 512 - 1) {}
            _ProtectedSectors = New HashSet(Of UInteger)
        End Sub

        Public Property SectorMap As UInteger()
        Public Property Data As Byte()
        Public Property ProtectedSectors As HashSet(Of UInteger)
    End Class
End Module
