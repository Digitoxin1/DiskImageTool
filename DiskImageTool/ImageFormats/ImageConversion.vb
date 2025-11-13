Imports DiskImageTool.Bitstream
Imports DiskImageTool.Bitstream.IBM_MFM
Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports DiskImageTool.ImageFormats.D86F

Namespace ImageFormats
    Module ImageConversion
        Public Function BasicSectorTo86FImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As D86F.D86FImage
            Dim Params = FloppyDiskFormatGetParams(DiskFormat)
            Dim TrackCount = Params.BPBParams.TrackCount
            Dim Hole = Get86FHole(DiskFormat)

            Dim RPM As D86F.RPM = D86F.GetRPM(Params.RPM)
            Dim BitRate As D86F.BitRate = D86F.GetBitRate(Params.BitRateKbps, True)

            Dim Image = New D86F.D86FImage(TrackCount, Params.BPBParams.NumberOfHeads) With {
                .BitcellMode = True,
                .AlternateBitcellCalculation = True,
                .Hole = Hole
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side As UShort = 0 To Params.BPBParams.NumberOfHeads - 1
                    Dim D86FTrack = New D86F.D86FTrack(Track, Side, 0) With {
                        .RPM = RPM,
                        .BitRate = BitRate,
                        .Encoding = D86F.Encoding.MFM
                    }

                    Dim MFMBitstream = MFMBitstreamFromSectorImage(Data, Params, Track, Side)

                    D86FTrack.Bitstream = MFMBitstream.Bitstream
                    D86FTrack.BitCellCount = MFMBitstream.Bitstream.Length

                    Image.SetTrack(Track, Side, D86FTrack)
                Next
            Next

            Return Image
        End Function

        Public Function BasicSectorToHFEImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As HFE.HFEImage
            Dim Params = FloppyDiskFormatGetParams(DiskFormat)
            Dim TrackCount = Params.BPBParams.TrackCount

            Dim HFE = New HFE.HFEImage(TrackCount, Params.BPBParams.NumberOfHeads) With {
                .BitRate = Params.BitRateKbps,
                .RPM = Params.RPM,
                .FloppyInterfaceMode = GetHFEFloppyInterfaceMode(DiskFormat)
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side As UShort = 0 To Params.BPBParams.NumberOfHeads - 1
                    Dim HFETrack = New HFE.HFETrack(Track, Side) With {
                        .BitRate = Params.BitRateKbps,
                        .RPM = Params.RPM
                    }

                    Dim MFMBitstream = MFMBitstreamFromSectorImage(Data, Params, Track, Side)

                    HFETrack.Bitstream = MFMBitstream.Bitstream

                    HFE.SetTrack(Track, Side, HFETrack)
                Next
            Next

            Return HFE
        End Function

        Public Function BasicSectorToIMDImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As IMD.IMDImage
            Dim Params = FloppyDiskFormatGetParams(DiskFormat)
            Dim TrackCount = Params.BPBParams.TrackCount

            Dim IMD = New IMD.IMDImage With {
                .Comment = "",
                .TrackCount = TrackCount,
                .SideCount = Params.BPBParams.NumberOfHeads
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side = 0 To Params.BPBParams.NumberOfHeads - 1
                    Dim IMDTrack = New IMD.IMDTrack With {
                        .Track = Track,
                        .Side = Side,
                        .Mode = GetIMDMode(Params.BitRateKbps, True),
                        .SectorSize = ImageFormats.IMD.SectorSize.Sectorsize512
                    }
                    For SectorId = 1 To Params.BPBParams.SectorsPerTrack
                        Dim IMDSector = New IMD.IMDSector(Params.BPBParams.BytesPerSector) With {
                            .Track = Track,
                            .Side = Side,
                            .SectorId = SectorId
                        }

                        Dim ImageOffset = GetImageOffset(Params.BPBParams, Track, Side, SectorId)
                        Dim Size = Math.Min(Params.BPBParams.BytesPerSector, Data.Length - ImageOffset)
                        Dim Buffer(Params.BPBParams.BytesPerSector - 1) As Byte
                        Array.Copy(Data, ImageOffset, Buffer, 0, Size)
                        IMDSector.Data = Buffer
                        IMDTrack.Sectors.Add(IMDSector)
                    Next
                    IMD.Tracks.Add(IMDTrack)
                Next
            Next

            Return IMD
        End Function

        Public Function BasicSectorToMFMImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As MFM.MFMImage
            Dim Params = FloppyDiskFormatGetParams(DiskFormat)
            Dim TrackCount = Params.BPBParams.TrackCount

            Dim MFM = New MFM.MFMImage(TrackCount, Params.BPBParams.NumberOfHeads, 1) With {
                .BitRate = Params.BitRateKbps,
                .RPM = Params.RPM
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side As UShort = 0 To Params.BPBParams.NumberOfHeads - 1
                    Dim MFMTrack = New MFM.MFMTrack(Track, Side) With {
                        .BitRate = Params.BitRateKbps,
                        .RPM = Params.RPM
                    }

                    Dim MFMBitstream = MFMBitstreamFromSectorImage(Data, Params, Track, Side)

                    MFMTrack.Bitstream = MFMBitstream.Bitstream

                    MFM.SetTrack(Track, Side, MFMTrack)
                Next
            Next

            Return MFM
        End Function

        Public Function BasicSectorToPRIImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As PRI.PRIImage
            Dim Params = FloppyDiskFormatGetParams(DiskFormat)
            Dim TrackCount = Params.BPBParams.TrackCount

            Dim PRI = New PRI.PRIImage(TrackCount, Params.BPBParams.NumberOfHeads)

            For Track As UShort = 0 To TrackCount - 1
                For Side = 0 To Params.BPBParams.NumberOfHeads - 1
                    Dim MFMBitstream = MFMBitstreamFromSectorImage(Data, Params, Track, Side)

                    Dim PRITrack = New PRI.PRITrack With {
                        .Track = Track,
                        .Side = Side,
                        .Length = MFMBitstream.Bitstream.Length,
                        .BitClockRate = Params.BitRateKbps * 2000,
                        .Bitstream = MFMBitstream.Bitstream
                    }
                    PRI.AddTrack(PRITrack)
                Next
            Next

            Return PRI
        End Function

        Public Function BasicSectorToPSIImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As PSI.PSISectorImage
            Dim Params = FloppyDiskFormatGetParams(DiskFormat)
            Dim TrackCount = Params.BPBParams.TrackCount

            Dim PSI = New PSI.PSISectorImage
            PSI.Header.FormatVersion = 0
            PSI.Header.DefaultSectorFormat = GetPSISectorFormat(DiskFormat)

            For Track As UShort = 0 To TrackCount - 1
                For Side = 0 To Params.BPBParams.NumberOfHeads - 1
                    Dim TrackOffset As UInteger = Params.Gaps.Gap4A + MFM_PREAMBLE_BYTES + Params.Gaps.Gap1
                    For SectorId = 1 To Params.BPBParams.SectorsPerTrack
                        TrackOffset += MFM_PREAMBLE_BYTES
                        Dim ImageOffset = GetImageOffset(Params.BPBParams, Track, Side, SectorId)
                        Dim Size = Math.Min(Params.BPBParams.BytesPerSector, Data.Length - ImageOffset)
                        Dim Buffer = New Byte(Size - 1) {}
                        Array.Copy(Data, ImageOffset, Buffer, 0, Size)

                        Dim PSISector = New PSI.PSISector With {
                                .HasDataCRCError = False,
                                .IsAlternateSector = False,
                                .Track = Track,
                                .Side = Side,
                                .Sector = SectorId,
                                .Data = Buffer,
                                .Offset = TrackOffset * 8
                            }

                        TrackOffset += MFM_IDAREA_BYTES + MFM_CRC_BYTES + Params.Gaps.Gap2 + MFM_PREAMBLE_BYTES + Size + MFM_CRC_BYTES + Params.Gaps.Gap3

                        PSI.Sectors.Add(PSISector)
                    Next
                Next
            Next

            Return PSI
        End Function

        Public Function BasicSectorToTranscopyImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As TC.TransCopyImage
            Dim Params = FloppyDiskFormatGetParams(DiskFormat)
            Dim TrackCount = Params.BPBParams.TrackCount

            Dim Transcopy = New TC.TransCopyImage(TrackCount, Params.BPBParams.NumberOfHeads, 1) With {
                .DiskType = GetTranscopyDiskType(DiskFormat)
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side As UShort = 0 To Params.BPBParams.NumberOfHeads - 1
                    Dim TransCopyTrack = New TC.TransCopyTrack(Track, Side) With {
                        .TrackType = GetTranscopyDiskType(DiskFormat),
                        .CopyAcrossIndex = True
                    }

                    Dim MFMBitstream = MFMBitstreamFromSectorImage(Data, Params, Track, Side)

                    TransCopyTrack.Bitstream = MFMBitstream.Bitstream
                    TransCopyTrack.SetTimings(MFMBitstream.AddressMarkIndexes)

                    Transcopy.SetTrack(Track, Side, TransCopyTrack)
                Next
            Next

            Return Transcopy
        End Function

        Public Function BitstreamTo86FImage(Image As IBitstreamImage) As D86F.D86FImage
            Dim BitstreamTrack As IBitstreamTrack
            Dim BitRate As UShort = 250
            Dim RPM As UShort = 300

            Dim TrackCount = GetValidTrackCount(Image, False)
            Dim NewTrackCount = TrackCount \ Image.TrackStep

            Dim D86FImage = New D86F.D86FImage(NewTrackCount, Image.SideCount) With {
                .BitcellMode = True,
                .AlternateBitcellCalculation = True,
                .HasSurfaceData = Image.HasSurfaceData,
                .Hole = D86F.DiskHole.DD
            }

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    Dim D86FTrack As ImageFormats.D86F.D86FTrack

                    If BitstreamTrack.TrackType = BitstreamTrackType.MFM Or BitstreamTrack.TrackType = BitstreamTrackType.FM Then
                        Dim DriveSpeed = GetDriveSpeed(BitstreamTrack, Image.TrackStep)

                        If Track = 0 And Side = 0 Then
                            If BitstreamTrack.MFMData IsNot Nothing Then
                                D86FImage.Hole = Get86FHole(GetTrackFormat(BitstreamTrack.MFMData.Size))
                            End If
                            BitRate = DriveSpeed.BitRate
                            RPM = DriveSpeed.RPM
                        End If

                        Dim IsMFM As Boolean = BitstreamTrack.TrackType = BitstreamTrackType.MFM

                        D86FTrack = New D86F.D86FTrack(NewTrack, Side, 0) With {
                            .Bitstream = BitstreamTrack.Bitstream,
                            .BitCellCount = BitstreamTrack.Bitstream.Length,
                            .BitRate = D86F.GetBitRate(DriveSpeed.BitRate, IsMFM),
                            .RPM = D86F.GetRPM(DriveSpeed.RPM),
                            .SurfaceData = BitstreamTrack.SurfaceData
                        }

                        If BitstreamTrack.TrackType = BitstreamTrackType.MFM Then
                            D86FTrack.Encoding = D86F.Encoding.MFM
                        Else
                            D86FTrack.Encoding = D86F.Encoding.FM
                        End If
                    Else
                        D86FTrack = GetEmpty86FTrack(Track, Side, BitRate, RPM)
                    End If

                    D86FImage.SetTrack(NewTrack, Side, D86FTrack)
                Next
            Next

            Return D86FImage
        End Function

        Public Function BitstreamToHFEImage(Image As IBitstreamImage) As HFE.HFEImage
            Dim BitstreamTrack As IBitstreamTrack

            Dim TrackCount = GetValidTrackCount(Image, False)
            Dim NewTrackCount = TrackCount \ Image.TrackStep

            Dim TotalTrackCount = AdjustTrackCount(NewTrackCount)

            Dim HFE = New HFE.HFEImage(TotalTrackCount, Image.SideCount) With {
                .BitRate = 250,
                .RPM = 300
            }

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    Dim HFETrack As ImageFormats.HFE.HFETrack

                    If BitstreamTrack.TrackType = BitstreamTrackType.MFM Or BitstreamTrack.TrackType = BitstreamTrackType.FM Then
                        Dim DriveSpeed = GetDriveSpeed(BitstreamTrack, Image.TrackStep)

                        If Track = 0 And Side = 0 Then
                            HFE.BitRate = DriveSpeed.BitRate
                            HFE.RPM = DriveSpeed.RPM
                            HFE.FloppyInterfaceMode = GetHFEFloppyInterfaceMode(GetTrackFormat(BitstreamTrack.Bitstream.Length))
                        End If

                        HFETrack = New HFE.HFETrack(NewTrack, Side) With {
                            .Bitstream = BitstreamTrack.Bitstream,
                            .BitRate = DriveSpeed.BitRate,
                            .RPM = DriveSpeed.RPM
                        }
                    Else
                        HFETrack = GetEmptyHFETrack(NewTrack, Side, HFE.BitRate, HFE.RPM)
                    End If

                    HFE.SetTrack(NewTrack, Side, HFETrack)
                Next
            Next

            If NewTrackCount < TotalTrackCount Then
                For Track = NewTrackCount To TotalTrackCount - 1
                    For Side = 0 To Image.SideCount - 1
                        Dim HFETrack = GetEmptyHFETrack(Track, Side, HFE.BitRate, HFE.RPM)
                        HFE.SetTrack(Track, Side, HFETrack)
                    Next
                Next
            End If

            Return HFE
        End Function

        Public Function BitstreamToIMDImage(Image As IBitstreamImage) As IMD.IMDImage
            Dim BitstreamTrack As IBitstreamTrack

            Dim TrackCount = GetValidTrackCount(Image, True)
            Dim NewTrackCount = TrackCount \ Image.TrackStep

            Dim IMD = New IMD.IMDImage With {
                .Comment = "",
                .TrackCount = NewTrackCount,
                .SideCount = Image.SideCount
            }

            For Track As UShort = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    Dim DriveSpeed = GetDriveSpeed(BitstreamTrack, Image.TrackStep)
                    Dim IsMFM = BitstreamTrack.TrackType = BitstreamTrackType.MFM

                    Dim IMDTrack = New IMD.IMDTrack With {
                        .Track = NewTrack,
                        .Side = Side,
                        .Mode = GetIMDMode(DriveSpeed.BitRate, IsMFM)
                    }

                    Dim TrackSectorSize As UInteger = 0

                    If BitstreamTrack.MFMData IsNot Nothing Then
                        For Each Sector In BitstreamTrack.MFMData.Sectors
                            Dim SectorSize = Sector.GetSizeBytes

                            If SectorSize > TrackSectorSize Then
                                TrackSectorSize = SectorSize
                            End If

                            Dim IMDSector = New IMD.IMDSector(SectorSize) With {
                            .Track = Sector.Track,
                            .Side = Sector.Side,
                            .SectorId = Sector.SectorId
                        }
                            If Sector.DAMFound Then
                                IMDSector.Unavailable = False
                                IMDSector.Data = Sector.Data
                                IMDSector.ChecksumError = Not Sector.InitialDataChecksumValid
                                IMDSector.Deleted = (Sector.DAM = MFMAddressMark.DeletedData)
                            Else
                                IMDSector.Unavailable = True
                            End If

                            IMDTrack.Sectors.Add(IMDSector)
                        Next
                    End If

                    IMDTrack.SectorSize = GetIMDSectorSize(TrackSectorSize)

                    IMD.Tracks.Add(IMDTrack)
                Next
            Next

            Return IMD
        End Function

        Public Function BitstreamToMFMImage(Image As IBitstreamImage) As MFM.MFMImage
            Dim BitstreamTrack As IBitstreamTrack

            Dim TrackCount = GetValidTrackCount(Image, True)
            Dim NewTrackCount = TrackCount \ Image.TrackStep

            Dim TotalTrackCount = AdjustTrackCount(NewTrackCount)

            Dim MFM = New MFM.MFMImage(TotalTrackCount, Image.SideCount, 1) With {
                .BitRate = 250,
                .RPM = 300
            }

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    Dim MFMTrack As ImageFormats.MFM.MFMTrack

                    If BitstreamTrack.TrackType = BitstreamTrackType.MFM Then
                        Dim DriveSpeed = GetDriveSpeed(BitstreamTrack, Image.TrackStep)

                        If Track = 0 And Side = 0 Then
                            MFM.BitRate = DriveSpeed.BitRate
                            MFM.RPM = DriveSpeed.RPM
                        End If

                        MFMTrack = New MFM.MFMTrack(NewTrack, Side) With {
                            .Bitstream = BitstreamTrack.Bitstream,
                            .BitRate = DriveSpeed.BitRate,
                            .RPM = DriveSpeed.RPM
                        }
                    Else
                        MFMTrack = GetEmptyMFMTrack(NewTrack, Side, MFM.BitRate, MFM.RPM)
                    End If

                    MFM.SetTrack(NewTrack, Side, MFMTrack)
                Next
            Next

            If NewTrackCount < TotalTrackCount Then
                For Track = NewTrackCount To TotalTrackCount - 1
                    For Side = 0 To Image.SideCount - 1
                        Dim MFMTrack = GetEmptyMFMTrack(Track, Side, MFM.BitRate, MFM.RPM)
                        MFM.SetTrack(Track, Side, MFMTrack)
                    Next
                Next
            End If

            Return MFM
        End Function

        Public Function BitstreamToPRIImage(Image As IBitstreamImage) As PRI.PRIImage
            Dim BitstreamTrack As IBitstreamTrack
            Dim BitRate As UShort = 250
            Dim RPM As UShort = 300

            Dim TrackCount = GetValidTrackCount(Image, True)
            Dim NewTrackCount = TrackCount \ Image.TrackStep

            Dim PRI = New PRI.PRIImage(NewTrackCount, Image.SideCount)

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    Dim PRITrack As ImageFormats.PRI.PRITrack

                    If BitstreamTrack.Decoded Then
                        Dim DriveSpeed = GetDriveSpeed(BitstreamTrack, Image.TrackStep)

                        If Track = 0 And Side = 0 Then
                            BitRate = DriveSpeed.BitRate
                            RPM = DriveSpeed.RPM
                        End If

                        PRITrack = New PRI.PRITrack With {
                            .Track = NewTrack,
                            .Side = Side,
                            .Length = BitstreamTrack.Bitstream.Length,
                            .BitClockRate = BitstreamTrack.BitRate * 2000,
                            .Bitstream = BitstreamTrack.Bitstream,
                            .SurfaceData = BitstreamTrack.SurfaceData
                        }
                    Else
                        PRITrack = GetEmptyPRITrack(NewTrack, Side, BitRate, RPM)
                    End If

                    PRI.AddTrack(PRITrack)
                Next
            Next

            Return PRI
        End Function

        Public Function BitstreamToPSIImage(Image As IBitstreamImage) As PSI.PSISectorImage
            Dim PSI = New PSI.PSISectorImage
            Dim BitstreamTrack As IBitstreamTrack
            Dim DiskFormat As MFMTrackFormat = MFMTrackFormat.TrackFormatUnknown

            Dim TrackCount = GetValidTrackCount(Image, False)

            PSI.Header.FormatVersion = 0

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    If BitstreamTrack.Decoded Then
                        Dim TrackFormat = GetTrackFormat(BitstreamTrack.Bitstream.Length)

                        If DiskFormat = MFMTrackFormat.TrackFormatUnknown And TrackFormat <> MFMTrackFormat.TrackFormatUnknown Then
                            DiskFormat = TrackFormat
                        End If

                        For Each Sector In BitstreamTrack.MFMData.Sectors
                            Dim PSISector = PSISectorFromMFMSector(NewTrack, Side, Sector, TrackFormat)

                            If BitstreamTrack.SurfaceData IsNot Nothing Then
                                Dim Buffer = MFMGetBytes(BitstreamTrack.SurfaceData, Sector.DataOffset, Sector.Data.Length)
                                Dim HasWeakBits As Boolean = False
                                For Each b In Buffer
                                    If b > 0 Then
                                        HasWeakBits = True
                                        Exit For
                                    End If
                                Next
                                If HasWeakBits Then
                                    PSISector.Weak = Buffer
                                End If
                            End If

                            PSI.Sectors.Add(PSISector)
                        Next
                    End If
                Next
            Next

            PSI.Header.DefaultSectorFormat = GetPSISectorFormat(DiskFormat)

            Return PSI
        End Function

        Public Function BitstreamToTranscopyImage(Image As IBitstreamImage) As TC.TransCopyImage
            Dim BitstreamTrack As IBitstreamTrack

            Dim TrackCount = GetValidTrackCount(Image, False)
            Dim NewTrackCount = TrackCount \ Image.TrackStep

            Dim Transcopy = New TC.TransCopyImage(NewTrackCount, Image.SideCount, 1)

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    Dim TrackType As TC.TransCopyDiskType
                    Dim BitCount As Integer = 6250 * 16

                    If BitstreamTrack.TrackType = BitstreamTrackType.MFM And BitstreamTrack.Decoded Then
                        TrackType = GetTranscopyDiskType(GetTrackFormat(BitstreamTrack.Bitstream.Length))
                    ElseIf BitstreamTrack.TrackType = BitstreamTrackType.FM Then
                        BitCount = 3125 * 16
                        TrackType = TC.TransCopyDiskType.FMSingleDensity
                    Else
                        TrackType = TC.TransCopyDiskType.Unknown
                    End If

                    If Track = 0 And Side = 0 Then
                        Transcopy.DiskType = TrackType
                    End If

                    Dim TransCopyTrack = New TC.TransCopyTrack(NewTrack, Side) With {
                        .TrackType = TrackType,
                        .CopyAcrossIndex = True
                    }
                    If BitstreamTrack.Bitstream.Length > 0 Then
                        TransCopyTrack.Bitstream = BitstreamTrack.Bitstream
                    Else
                        TransCopyTrack.Bitstream = New BitArray(BitCount)
                    End If

                    If BitstreamTrack.MFMData IsNot Nothing Then
                        TransCopyTrack.SetTimings(BitstreamTrack.MFMData.AddressMarkIndexes)
                    End If
                    Transcopy.SetTrack(NewTrack, Side, TransCopyTrack)
                Next
            Next

            Return Transcopy
        End Function

        Private Function AdjustTrackCount(TrackCount As UShort) As UShort
            Dim NewTrackCount As UShort

            If TrackCount < 40 Then
                NewTrackCount = 40
            ElseIf TrackCount > 42 And TrackCount < 80 Then
                NewTrackCount = 80
            Else
                NewTrackCount = TrackCount
            End If

            Return NewTrackCount
        End Function

        Private Function Get86FHole(TrackFormat As MFMTrackFormat) As D86F.DiskHole
            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatHD, MFMTrackFormat.TrackFormatHD1200
                    Return D86F.DiskHole.HD
                Case MFMTrackFormat.TrackFormatED
                    Return D86F.DiskHole.ED
                Case Else
                    Return D86F.DiskHole.DD
            End Select
        End Function

        Private Function Get86FHole(DiskFormat As FloppyDiskFormat) As D86F.DiskHole
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy1200, FloppyDiskFormat.Floppy1440, FloppyDiskFormat.Floppy2HD
                    Return D86F.DiskHole.HD
                Case FloppyDiskFormat.Floppy2880
                    Return D86F.DiskHole.ED
                Case Else
                    Return D86F.DiskHole.DD
            End Select
        End Function

        Private Function GetDriveSpeed(BitstreamTrack As IBitstreamTrack, TrackStep As Byte) As (BitRate As UShort, RPM As UShort)
            Dim DriveSpeed As (BitRate As UShort, RPM As UShort)

            DriveSpeed.BitRate = RoundBitRate(BitstreamTrack.BitRate)
            DriveSpeed.RPM = RoundRPM(BitstreamTrack.RPM)

            If TrackStep = 2 Then
                If DriveSpeed.BitRate = 300 Then
                    DriveSpeed.BitRate = 250
                    If DriveSpeed.RPM = 360 Then
                        DriveSpeed.RPM = 300
                    End If
                ElseIf DriveSpeed.BitRate = 150 Then
                    DriveSpeed.BitRate = 125
                    If DriveSpeed.RPM = 360 Then
                        DriveSpeed.RPM = 300
                    End If
                End If
            End If

            Return DriveSpeed
        End Function

        Private Function GetDriveSpeed(TrackFormat As MFMTrackFormat) As (BitRate As UShort, RPM As UShort)
            Dim DriveSpeed As (BitRate As UShort, RPM As UShort)

            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatDD
                    DriveSpeed.BitRate = 250
                    DriveSpeed.RPM = 300
                Case MFMTrackFormat.TrackFormatHD
                    DriveSpeed.BitRate = 500
                    DriveSpeed.RPM = 300
                Case MFMTrackFormat.TrackFormatHD1200
                    DriveSpeed.BitRate = 500
                    DriveSpeed.RPM = 360
                Case MFMTrackFormat.TrackFormatED
                    DriveSpeed.BitRate = 1000
                    DriveSpeed.RPM = 300
            End Select

            Return DriveSpeed
        End Function

        Private Function GetEmpty86FTrack(Track As UShort, Side As Byte, BitRate As UShort, RPM As UShort) As D86F.D86FTrack
            Dim Bitstream = New BitArray(MFMGetSize(RPM, BitRate))

            Dim D86FTrack = New D86F.D86FTrack(Track, Side, 0) With {
                .Bitstream = Bitstream,
                .BitCellCount = Bitstream.Length,
                .BitRate = D86F.GetBitRate(BitRate, True),
                .RPM = D86F.GetRPM(RPM)
            }

            Return D86FTrack
        End Function

        Private Function GetEmptyHFETrack(Track As UShort, Side As Byte, BitRate As UShort, RPM As UShort) As HFE.HFETrack
            Dim HFETrack = New HFE.HFETrack(Track, Side) With {
                .Bitstream = New BitArray(MFMGetSize(RPM, BitRate)),
                .BitRate = BitRate,
                .RPM = RPM
            }
            Return HFETrack
        End Function

        Private Function GetEmptyMFMTrack(Track As UShort, Side As Byte, BitRate As UShort, RPM As UShort) As MFM.MFMTrack
            Dim MFMTrack = New MFM.MFMTrack(Track, Side) With {
                .Bitstream = New BitArray(MFMGetSize(RPM, BitRate)),
                .BitRate = BitRate,
                .RPM = RPM
            }
            Return MFMTrack
        End Function

        Private Function GetEmptyPRITrack(Track As UShort, Side As Byte, BitRate As UShort, RPM As UShort) As PRI.PRITrack
            Dim Bitstream = New BitArray(MFMGetSize(RPM, BitRate))

            Dim PRITrack = New PRI.PRITrack With {
                .Track = Track,
                .Side = Side,
                .Length = Bitstream.Length,
                .BitClockRate = BitRate * 2000,
                .Bitstream = Bitstream,
                .SurfaceData = Nothing
            }
            Return PRITrack
        End Function

        Private Function GetHFEFloppyInterfaceMode(TrackFormat As MFMTrackFormat) As HFE.HFEFloppyinterfaceMode
            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatHD, MFMTrackFormat.TrackFormatHD1200
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_HD_FLOPPYMODE
                Case MFMTrackFormat.TrackFormatED
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_ED_FLOPPYMODE
                Case Else
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_DD_FLOPPYMODE
            End Select
        End Function

        Private Function GetHFEFloppyInterfaceMode(DiskFormat As FloppyDiskFormat) As HFE.HFEFloppyinterfaceMode
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy1200, FloppyDiskFormat.Floppy1440, FloppyDiskFormat.Floppy2HD
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_HD_FLOPPYMODE
                Case FloppyDiskFormat.Floppy2880
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_ED_FLOPPYMODE
                Case Else
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_DD_FLOPPYMODE
            End Select
        End Function

        Private Function GetIMDMode(Bitrate As UShort, IsMFM As Boolean) As IMD.TrackMode
            If IsMFM Then
                If Bitrate = 1000 Then
                    Return IMD.TrackMode.MFM500kbps
                ElseIf Bitrate = 500 Then
                    Return IMD.TrackMode.MFM500kbps
                ElseIf Bitrate = 300 Then
                    Return IMD.TrackMode.MFM300kbps
                Else
                    Return IMD.TrackMode.MFM250kbps
                End If
            Else
                If Bitrate = 500 Then
                    Return IMD.TrackMode.FM500kbps
                ElseIf Bitrate = 300 Then
                    Return IMD.TrackMode.FM300kbps
                Else
                    Return IMD.TrackMode.FM250kbps
                End If
            End If

            Return IMD.TrackMode.FM250kbps
        End Function

        Private Function GetIMDSectorSize(Size As UInteger) As IMD.SectorSize
            Select Case Size
                Case 128
                    Return IMD.SectorSize.SectorSize128
                Case 256
                    Return IMD.SectorSize.SectorSize256
                Case 512
                    Return IMD.SectorSize.Sectorsize512
                Case 1024
                    Return IMD.SectorSize.SectorSize1024
                Case 2048
                    Return IMD.SectorSize.SectorSize2048
                Case 4096
                    Return IMD.SectorSize.SectorSize4096
                Case 8192
                    Return IMD.SectorSize.SectorSize8192
                Case Else
                    Return IMD.SectorSize.Sectorsize512
            End Select
        End Function

        Private Function GetImageOffset(Params As FloppyDiskBPBParams, Track As UShort, Side As UShort, SectorId As UShort) As UInteger
            Return (Track * Params.NumberOfHeads * Params.SectorsPerTrack + Params.SectorsPerTrack * Side + (SectorId - 1)) * Params.BytesPerSector
        End Function

        Private Function GetMFMSectorSize(BytesPerSector As UInteger) As IBM_MFM_Bitstream.MFMSectorSize
            Select Case BytesPerSector
                Case 128
                    Return IBM_MFM_Bitstream.MFMSectorSize.SectorSize_128
                Case 256
                    Return IBM_MFM_Bitstream.MFMSectorSize.SectorSize_256
                Case 512
                    Return IBM_MFM_Bitstream.MFMSectorSize.SectorSize_512
                Case 1024
                    Return IBM_MFM_Bitstream.MFMSectorSize.SectorSize_1024
                Case 2048
                    Return IBM_MFM_Bitstream.MFMSectorSize.SectorSize_2048
                Case 4096
                    Return IBM_MFM_Bitstream.MFMSectorSize.SectorSize_4096
                Case 8192
                    Return IBM_MFM_Bitstream.MFMSectorSize.SectorSize_8192
                Case 16384
                    Return IBM_MFM_Bitstream.MFMSectorSize.SectorSize_16384
                Case Else
                    Return IBM_MFM_Bitstream.MFMSectorSize.SectorSize_512
            End Select
        End Function


        Private Function GETPSIEncodingSubType(TrackFormat As MFMTrackFormat) As PSI.MFMEncodingSubtype
            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatHD, MFMTrackFormat.TrackFormatHD1200
                    Return PSI.MFMEncodingSubtype.HighDensity
                Case MFMTrackFormat.TrackFormatED
                    Return PSI.MFMEncodingSubtype.ExtraDensity
                Case Else
                    Return PSI.MFMEncodingSubtype.DoubleDensity
            End Select

        End Function

        Private Function GetPSISectorFormat(TrackFormat As MFMTrackFormat) As PSI.DefaultSectorFormat
            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatDD
                    Return PSI.DefaultSectorFormat.IBM_MFM_DD
                Case MFMTrackFormat.TrackFormatHD, MFMTrackFormat.TrackFormatHD1200
                    Return PSI.DefaultSectorFormat.IBM_MFM_HD
                Case MFMTrackFormat.TrackFormatED
                    Return PSI.DefaultSectorFormat.IBM_MFM_ED
                Case Else
                    Return 0
            End Select

        End Function

        Private Function GetPSISectorFormat(DiskFormat As FloppyDiskFormat) As PSI.DefaultSectorFormat
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy1200, FloppyDiskFormat.Floppy1440, FloppyDiskFormat.Floppy2HD
                    Return PSI.DefaultSectorFormat.IBM_MFM_HD
                Case FloppyDiskFormat.Floppy2880
                    Return PSI.DefaultSectorFormat.IBM_MFM_ED
                Case Else
                    Return PSI.DefaultSectorFormat.IBM_MFM_DD
            End Select
        End Function

        Private Function GetTranscopyDiskType(TrackFormat As MFMTrackFormat) As TC.TransCopyDiskType
            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatDD
                    Return TC.TransCopyDiskType.MFMDoubleDensity
                Case MFMTrackFormat.TrackFormatHD, MFMTrackFormat.TrackFormatHD1200, MFMTrackFormat.TrackFormatED
                    Return TC.TransCopyDiskType.MFMHighDensity
                Case Else
                    Return TC.TransCopyDiskType.Unknown
            End Select
        End Function

        Private Function GetTranscopyDiskType(DiskFormat As FloppyDiskFormat) As TC.TransCopyDiskType
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy1200, FloppyDiskFormat.Floppy1440, FloppyDiskFormat.Floppy2880, FloppyDiskFormat.Floppy2HD
                    Return TC.TransCopyDiskType.MFMHighDensity
                Case Else
                    Return TC.TransCopyDiskType.MFMDoubleDensity
            End Select
        End Function

        Private Function GetValidTrackCount(Image As IBitstreamImage, MFMOnly As Boolean) As UShort
            Dim BitstreamTrack As IBitstreamTrack
            Dim TrackCount As UShort = 0
            Dim ValidTrack As Boolean

            For Track = 0 To Image.TrackCount - 1 Step Image.TrackStep
                BitstreamTrack = Image.GetTrack(Track, 0)

                If MFMOnly Then
                    ValidTrack = BitstreamTrack.TrackType = BitstreamTrackType.MFM
                Else
                    ValidTrack = BitstreamTrack.TrackType <> BitstreamTrackType.Other
                End If

                If ValidTrack Then
                    TrackCount = Track + Image.TrackStep
                End If
            Next

            Return TrackCount
        End Function

        Private Function MFMBitstreamFromSectorImage(Data() As Byte, Params As FloppyDiskParams, Track As UShort, Side As Byte) As IBM_MFM_Bitstream
            Dim MFMBitstream = New IBM_MFM_Bitstream(Params.Gaps.Gap4A, Params.Gaps.Gap1)

            For SectorId = 1 To Params.BPBParams.SectorsPerTrack
                Dim ImageOffset = GetImageOffset(Params.BPBParams, Track, Side, SectorId)
                Dim Size = Math.Min(Params.BPBParams.BytesPerSector, Data.Length - ImageOffset)
                Dim Buffer = New Byte(Size - 1) {}
                Array.Copy(Data, ImageOffset, Buffer, 0, Size)

                MFMBitstream.AddSectorId(Track, Side, SectorId, GetMFMSectorSize(Params.BPBParams.BytesPerSector), Params.Gaps.Gap2)
                MFMBitstream.AddData(Buffer, Params.Gaps.Gap3)
            Next

            MFMBitstream.Finish(Params.RPM, Params.BitRateKbps)

            Return MFMBitstream
        End Function

        Private Function PSISectorFromMFMSector(Track As UShort, Side As Byte, Sector As IBM_MFM_Sector, TrackFormat As MFMTrackFormat) As PSI.PSISector
            Dim DataFieldCRCError = Sector.DataChecksum <> Sector.CalculateDataChecksum
            Dim IDFieldCRCError = Sector.IDChecksum <> Sector.CalculateIDChecksum
            Dim DeletedDAM = Sector.DAM = MFMAddressMark.DeletedData
            Dim MissingDAM = Not Sector.DAMFound

            Dim PSISector = New PSI.PSISector With {
                .HasDataCRCError = DataFieldCRCError,
                .IsAlternateSector = False,
                .Track = Track,
                .Side = Side,
                .Sector = Sector.SectorId,
                .Data = Sector.Data,
                .Offset = (Sector.Offset + 64) / 2
            }
            If Sector.Track <> Track Or Sector.Side <> Side Or DeletedDAM Or MissingDAM Or IDFieldCRCError Then
                PSISector.MFMHeader = New PSI.IBMSectorHeader With {
                    .Cylinder = Sector.Track,
                    .Head = Sector.Side,
                    .Sector = Sector.SectorId,
                    .Size = Sector.Size,
                    .EncodingSubType = GETPSIEncodingSubType(TrackFormat),
                    .IDFieldCRCError = IDFieldCRCError,
                    .DataFieldCRCError = DataFieldCRCError,
                    .DeletedDAM = DeletedDAM,
                    .MissingDAM = MissingDAM
                }

            End If

            Return PSISector
        End Function
    End Module
End Namespace
