Imports DiskImageTool.Bitstream
Imports DiskImageTool.Bitstream.IBM_MFM
Imports DiskImageTool.DiskImage.FloppyDiskFunctions
Imports DiskImageTool.ImageFormats.IMD

Namespace ImageFormats
    Module ImageConversion
        Public Function BasicSectorTo86FImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As _86F._86FImage
            Dim Params = GetFloppyDiskParams(DiskFormat)
            Dim DriveSpeed = GetDriveSpeed(DiskFormat)
            Dim TrackCount = GetTrackCount(Params)
            Dim Hole As _86F.DiskHole

            Dim RPM As _86F.RPM = _86F.GetRPM(DriveSpeed.RPM)
            Dim BitRate As _86F.BitRate = _86F.GetBitRate(DriveSpeed.BitRate, True)

            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160, FloppyDiskFormat.Floppy180, FloppyDiskFormat.Floppy320, FloppyDiskFormat.Floppy360, FloppyDiskFormat.Floppy720
                    Hole = _86F.DiskHole.DD
                Case FloppyDiskFormat.Floppy1200, FloppyDiskFormat.Floppy1440
                    Hole = _86F.DiskHole.HD
                Case FloppyDiskFormat.Floppy2880
                    Hole = _86F.DiskHole.ED
                Case Else
                    Hole = _86F.DiskHole.DD
            End Select

            Dim Image = New _86F._86FImage(TrackCount, Params.NumberOfHeads) With {
                .BitcellMode = True,
                .AlternateBitcellCalculation = True,
                .Hole = Hole
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side As UShort = 0 To Params.NumberOfHeads - 1
                    Dim F86Track = New _86F._86FTrack(Track, Side, 0) With {
                        .RPM = RPM,
                        .BitRate = BitRate,
                        .Encoding = _86F.Encoding.MFM
                    }

                    Dim MFMBitstream = MFMBitstreamFromSectorImage(Data, Params, Track, Side, DriveSpeed.RPM, DriveSpeed.BitRate)

                    F86Track.Bitstream = MFMBitstream.Bitstream
                    F86Track.BitCellCount = MFMBitstream.Bitstream.Length

                    Image.SetTrack(Track, Side, F86Track)
                Next
            Next

            Return Image
        End Function

        Public Function BasicSectorToHFEImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As HFE.HFEImage
            Dim Params = GetFloppyDiskParams(DiskFormat)
            Dim DriveSpeed = GetDriveSpeed(DiskFormat)
            Dim TrackCount = GetTrackCount(Params)

            Dim HFE = New HFE.HFEImage(TrackCount, Params.NumberOfHeads) With {
                .BitRate = DriveSpeed.BitRate,
                .RPM = DriveSpeed.RPM,
                .FloppyInterfaceMode = GetHFEFloppyInterfaceMode(DiskFormat)
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side As UShort = 0 To Params.NumberOfHeads - 1
                    Dim HFETrack = New HFE.HFETrack(Track, Side) With {
                        .BitRate = DriveSpeed.BitRate,
                        .RPM = DriveSpeed.RPM
                    }

                    Dim MFMBitstream = MFMBitstreamFromSectorImage(Data, Params, Track, Side, DriveSpeed.RPM, DriveSpeed.BitRate)

                    HFETrack.Bitstream = MFMBitstream.Bitstream

                    HFE.SetTrack(Track, Side, HFETrack)
                Next
            Next

            Return HFE
        End Function

        Public Function BasicSectorToIMDImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As IMD.IMDImage
            Dim Params = GetFloppyDiskParams(DiskFormat)
            Dim TrackCount = GetTrackCount(Params)

            Dim IMD = New IMD.IMDImage With {
                .Comment = "",
                .TrackCount = TrackCount,
                .SideCount = Params.NumberOfHeads
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side = 0 To Params.NumberOfHeads - 1
                    Dim IMDTrack = New IMDTrack With {
                        .Track = Track,
                        .Side = Side,
                        .Mode = GetIMDMode(DiskFormat),
                        .SectorSize = SectorSize.Sectorsize512
                    }
                    For SectorId = 1 To Params.SectorsPerTrack
                        Dim IMDSector = New IMDSector(Params.BytesPerSector) With {
                            .Track = Track,
                            .Side = Side,
                            .SectorId = SectorId
                        }

                        Dim ImageOffset = GetImageOffset(Params, Track, Side, SectorId)
                        Dim Size = Math.Min(Params.BytesPerSector, Data.Length - ImageOffset)
                        Dim Buffer(Params.BytesPerSector - 1) As Byte
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
            Dim Params = GetFloppyDiskParams(DiskFormat)
            Dim DriveSpeed = GetDriveSpeed(DiskFormat)
            Dim TrackCount = GetTrackCount(Params)

            Dim MFM = New MFM.MFMImage(TrackCount, Params.NumberOfHeads, 1) With {
                .BitRate = DriveSpeed.BitRate,
                .RPM = DriveSpeed.RPM
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side As UShort = 0 To Params.NumberOfHeads - 1
                    Dim MFMTrack = New MFM.MFMTrack(Track, Side) With {
                        .BitRate = DriveSpeed.BitRate,
                        .RPM = DriveSpeed.RPM
                    }

                    Dim MFMBitstream = MFMBitstreamFromSectorImage(Data, Params, Track, Side, DriveSpeed.RPM, DriveSpeed.BitRate)

                    MFMTrack.Bitstream = MFMBitstream.Bitstream

                    MFM.SetTrack(Track, Side, MFMTrack)
                Next
            Next

            Return MFM
        End Function

        Public Function BasicSectorToPSIImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As PSI.PSISectorImage
            Dim Params = GetFloppyDiskParams(DiskFormat)
            Dim TrackCount = GetTrackCount(Params)

            Dim PSI = New PSI.PSISectorImage
            PSI.Header.FormatVersion = 0
            PSI.Header.DefaultSectorFormat = GetPSISectorFormat(DiskFormat)

            For Track As UShort = 0 To TrackCount - 1
                For Side = 0 To Params.NumberOfHeads - 1
                    Dim TrackOffset As UInteger = MFM_GAP4A_SIZE + MFM_SYNC_SIZE + MFM_ADDRESS_MARK_SIZE + MFM_GAP1_SIZE
                    For SectorId = 1 To Params.SectorsPerTrack
                        TrackOffset += MFM_SYNC_SIZE + MFM_ADDRESS_MARK_SIZE
                        Dim ImageOffset = GetImageOffset(Params, Track, Side, SectorId)
                        Dim Size = Math.Min(Params.BytesPerSector, Data.Length - ImageOffset)
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

                        TrackOffset += MFM_IDAM_SIZE + MFM_GAP2_SIZE + MFM_SYNC_SIZE + MFM_ADDRESS_MARK_SIZE + Size + 2 + MFM_GAP3_SIZE

                        PSI.Sectors.Add(PSISector)
                    Next
                Next
            Next

            Return PSI
        End Function

        Public Function BasicSectorToTranscopyImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As TC.TransCopyImage
            Dim Params = GetFloppyDiskParams(DiskFormat)
            Dim DriveSpeed = GetDriveSpeed(DiskFormat)
            Dim TrackCount = GetTrackCount(Params)

            Dim Transcopy = New TC.TransCopyImage(TrackCount, Params.NumberOfHeads, 1) With {
                .DiskType = GetTranscopyDiskType(DiskFormat)
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side As UShort = 0 To Params.NumberOfHeads - 1
                    Dim TransCopyTrack = New TC.TransCopyTrack(Track, Side) With {
                        .TrackType = GetTranscopyDiskType(DiskFormat),
                        .CopyAcrossIndex = True
                    }

                    Dim MFMBitstream = MFMBitstreamFromSectorImage(Data, Params, Track, Side, DriveSpeed.RPM, DriveSpeed.BitRate)

                    TransCopyTrack.Bitstream = MFMBitstream.Bitstream
                    TransCopyTrack.SetTimings(MFMBitstream.AddressMarkIndexes)

                    Transcopy.SetTrack(Track, Side, TransCopyTrack)
                Next
            Next

            Return Transcopy
        End Function

        Public Function BitstreamTo86FImage(Image As IBitstreamImage) As _86F._86FImage
            Dim BitstreamTrack As IBitstreamTrack

            Dim TrackCount = GetValidTrackCount(Image, False)
            Dim NewTrackCount = TrackCount \ Image.TrackStep

            Dim F86Image = New _86F._86FImage(NewTrackCount, Image.SideCount) With {
                .BitcellMode = True,
                .AlternateBitcellCalculation = True
            }

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    If Track = 0 And Side = 0 Then
                        If BitstreamTrack.MFMData IsNot Nothing Then
                            F86Image.Hole = Get86FHole(GetTrackFormat(BitstreamTrack.MFMData.Size))
                        End If
                    End If

                    Dim DriveSpeed = GetDriveSpeed(BitstreamTrack, Image.TrackStep)

                    Dim IsMFM As Boolean = BitstreamTrack.TrackType = BitstreamTrackType.MFM

                    Dim F86Track = New _86F._86FTrack(NewTrack, Side, 0) With {
                        .Bitstream = BitstreamTrack.Bitstream,
                        .BitCellCount = BitstreamTrack.Bitstream.Length,
                        .BitRate = _86F.GetBitRate(DriveSpeed.BitRate, IsMFM),
                        .RPM = _86F.GetRPM(DriveSpeed.RPM)
                    }

                    If BitstreamTrack.TrackType = BitstreamTrackType.MFM Then
                        F86Track.Encoding = _86F.Encoding.MFM
                    Else
                        F86Track.Encoding = _86F.Encoding.FM
                    End If

                    F86Image.SetTrack(NewTrack, Side, F86Track)
                Next
            Next

            Return F86Image
        End Function

        Public Function BitstreamToHFEImage(Image As IBitstreamImage) As HFE.HFEImage
            Dim BitstreamTrack As IBitstreamTrack

            Dim TrackCount = GetValidTrackCount(Image, False)
            Dim NewTrackCount = TrackCount \ Image.TrackStep

            Dim HFE = New HFE.HFEImage(NewTrackCount, Image.SideCount) With {
                .BitRate = 0,
                .RPM = 0
            }

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    Dim DriveSpeed = GetDriveSpeed(BitstreamTrack, Image.TrackStep)

                    If Track = 0 And Side = 0 Then
                        HFE.BitRate = DriveSpeed.BitRate
                        HFE.RPM = DriveSpeed.RPM
                        HFE.FloppyInterfaceMode = GetHFEFloppyInterfaceMode(GetTrackFormat(BitstreamTrack.Bitstream.Length))
                    End If

                    Dim HFETrack = New HFE.HFETrack(NewTrack, Side) With {
                        .Bitstream = BitstreamTrack.Bitstream,
                        .BitRate = DriveSpeed.BitRate,
                        .RPM = DriveSpeed.RPM
                    }

                    HFE.SetTrack(NewTrack, Side, HFETrack)
                Next
            Next

            Return HFE
        End Function

        Public Function BitstreamToIMDImage(Image As IBitstreamImage) As IMD.IMDImage
            Dim BitstreamTrack As IBitstreamTrack

            Dim TrackCount = GetValidTrackCount(Image, True)

            Dim IMD = New IMD.IMDImage With {
                .Comment = "",
                .TrackCount = TrackCount,
                .SideCount = Image.SideCount
            }

            For Track As UShort = 0 To TrackCount - 1
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    Dim DriveSpeed = GetDriveSpeed(BitstreamTrack, Image.TrackStep)
                    Dim IsMFM = BitstreamTrack.TrackType = BitstreamTrackType.MFM

                    Dim IMDTrack = New IMDTrack With {
                        .Track = Track,
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

                            Dim IMDSector = New IMDSector(SectorSize) With {
                            .Track = Sector.Track,
                            .Side = Sector.Side,
                            .SectorId = Sector.SectorId
                        }
                            If Sector.DAMFound Then
                                IMDSector.Unavailable = False
                                IMDSector.Data = Sector.Data
                                IMDSector.ChecksumError = Not Sector.InitialChecksumValid
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

            Dim MFM = New MFM.MFMImage(NewTrackCount, Image.SideCount, 1) With {
                .BitRate = 0,
                .RPM = 0
            }

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                Dim NewTrack = Track \ Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    Dim DriveSpeed = GetDriveSpeed(BitstreamTrack, Image.TrackStep)

                    If Track = 0 And Side = 0 Then
                        MFM.BitRate = DriveSpeed.BitRate
                        MFM.RPM = DriveSpeed.RPM
                    End If

                    Dim MFMTrack = New MFM.MFMTrack(NewTrack, Side) With {
                        .Bitstream = BitstreamTrack.Bitstream,
                        .BitRate = DriveSpeed.BitRate,
                        .RPM = DriveSpeed.RPM
                    }

                    MFM.SetTrack(NewTrack, Side, MFMTrack)
                Next
            Next

            Return MFM
        End Function

        Public Function BitstreamToPSIImage(Image As IBitstreamImage) As PSI.PSISectorImage
            Dim PSI = New PSI.PSISectorImage
            Dim BitstreamTrack As IBitstreamTrack
            Dim DiskFormat As MFMTrackFormat = MFMTrackFormat.TrackFormatUnknown

            Dim TrackCount = GetValidTrackCount(Image, False)

            PSI.Header.FormatVersion = 0

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    If BitstreamTrack.Decoded Then
                        Dim TrackFormat = GetTrackFormat(BitstreamTrack.MFMData.Size)

                        If DiskFormat = MFMTrackFormat.TrackFormatUnknown And TrackFormat <> MFMTrackFormat.TrackFormatUnknown Then
                            DiskFormat = TrackFormat
                        End If

                        For Each Sector In BitstreamTrack.MFMData.Sectors
                            Dim PSISector = PSISectorFromMFMSector(Sector)

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

        Private Function Get86FHole(TrackFormat As MFMTrackFormat) As _86F.DiskHole
            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatDD
                    Return _86F.DiskHole.DD
                Case MFMTrackFormat.TrackFormatHD
                    Return _86F.DiskHole.HD
                Case MFMTrackFormat.TrackFormatHD1200
                    Return _86F.DiskHole.HD
                Case MFMTrackFormat.TrackFormatED
                    Return _86F.DiskHole.ED
                Case Else
                    Return _86F.DiskHole.DD
            End Select
        End Function

        Private Function GetDriveSpeed(BitstreamTrack As IBitstreamTrack, TrackStep As Byte) As DriveSpeed
            Dim DriveSpeed As New DriveSpeed With {
                .BitRate = RoundBitRate(BitstreamTrack.BitRate),
                .RPM = RoundRPM(BitstreamTrack.RPM)
            }

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

        Private Function GetDriveSpeed(TrackFormat As MFMTrackFormat) As DriveSpeed
            Dim DriveSpeed As New DriveSpeed

            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatDD
                    DriveSpeed.SetValue(300, 250)
                Case MFMTrackFormat.TrackFormatHD
                    DriveSpeed.SetValue(300, 500)
                Case MFMTrackFormat.TrackFormatHD1200
                    DriveSpeed.SetValue(360, 500)
                Case MFMTrackFormat.TrackFormatED
                    DriveSpeed.SetValue(300, 1000)
            End Select

            Return DriveSpeed
        End Function

        Private Function GetDriveSpeed(DiskFormat As FloppyDiskFormat) As DriveSpeed
            Dim DriveSpeed As New DriveSpeed

            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy1200
                    DriveSpeed.SetValue(360, 500)
                Case FloppyDiskFormat.Floppy1440
                    DriveSpeed.SetValue(360, 500)
                Case FloppyDiskFormat.Floppy2880
                    DriveSpeed.SetValue(300, 1000)
                Case FloppyDiskFormat.FloppyDMF1024
                    DriveSpeed.SetValue(360, 500)
                Case FloppyDiskFormat.FloppyDMF2048
                    DriveSpeed.SetValue(360, 500)
                Case FloppyDiskFormat.FloppyXDF525
                    DriveSpeed.SetValue(360, 500)
                Case FloppyDiskFormat.FloppyXDF35
                    DriveSpeed.SetValue(360, 500)
                Case Else
                    DriveSpeed.SetValue(300, 250)
            End Select

            Return DriveSpeed
        End Function

        Private Function GetHFEFloppyInterfaceMode(TrackFormat As MFMTrackFormat) As HFE.HFEFloppyinterfaceMode
            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatDD
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_DD_FLOPPYMODE
                Case MFMTrackFormat.TrackFormatHD
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_HD_FLOPPYMODE
                Case MFMTrackFormat.TrackFormatHD1200
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_HD_FLOPPYMODE
                Case MFMTrackFormat.TrackFormatED
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_ED_FLOPPYMODE
                Case Else
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_DD_FLOPPYMODE
            End Select
        End Function

        Private Function GetHFEFloppyInterfaceMode(DiskFormat As FloppyDiskFormat) As HFE.HFEFloppyinterfaceMode
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy1200
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_HD_FLOPPYMODE
                Case FloppyDiskFormat.Floppy1440
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_HD_FLOPPYMODE
                Case FloppyDiskFormat.Floppy2880
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_ED_FLOPPYMODE
                Case Else
                    Return HFE.HFEFloppyinterfaceMode.IBMPC_DD_FLOPPYMODE
            End Select
        End Function

        Private Function GetIMDMode(DiskFormat As FloppyDiskFormat) As TrackMode
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy1200
                    Return TrackMode.MFM500kbps
                Case FloppyDiskFormat.Floppy1440
                    Return TrackMode.MFM500kbps
                Case FloppyDiskFormat.Floppy2880
                    Return TrackMode.MFM500kbps
                Case Else
                    Return TrackMode.MFM250kbps
            End Select
        End Function

        Private Function GetIMDMode(Bitrate As UShort, IsMFM As Boolean) As TrackMode
            If IsMFM Then
                If Bitrate = 500 Then
                    Return TrackMode.MFM500kbps
                ElseIf Bitrate = 300 Then
                    Return TrackMode.MFM300kbps
                Else
                    Return TrackMode.MFM250kbps
                End If
            Else
                If Bitrate = 500 Then
                    Return TrackMode.FM500kbps
                ElseIf Bitrate = 300 Then
                    Return TrackMode.FM300kbps
                Else
                    Return TrackMode.FM250kbps
                End If
            End If

            Return TrackMode.FM250kbps
        End Function

        Private Function GetIMDSectorSize(Size As UInteger) As IMD.SectorSize
            Select Case Size
                Case 128
                    Return SectorSize.SectorSize128
                Case 256
                    Return SectorSize.SectorSize256
                Case 512
                    Return SectorSize.Sectorsize512
                Case 1024
                    Return SectorSize.SectorSize1024
                Case 2048
                    Return SectorSize.SectorSize2048
                Case 4096
                    Return SectorSize.SectorSize4096
                Case 8192
                    Return SectorSize.SectorSize8192
                Case Else
                    Return SectorSize.Sectorsize512
            End Select
        End Function

        Private Function GetImageOffset(Params As FloppyDiskParams, Track As UShort, Side As UShort, SectorId As UShort) As UInteger
            Return (Track * Params.NumberOfHeads * Params.SectorsPerTrack + Params.SectorsPerTrack * Side + (SectorId - 1)) * Params.BytesPerSector
        End Function

        Private Function GetPSISectorFormat(TrackFormat As MFMTrackFormat) As PSI.DefaultSectorFormat
            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatDD
                    Return PSI.DefaultSectorFormat.IBM_MFM_DD
                Case MFMTrackFormat.TrackFormatHD
                    Return PSI.DefaultSectorFormat.IBM_MFM_HD
                Case MFMTrackFormat.TrackFormatHD1200
                    Return PSI.DefaultSectorFormat.IBM_MFM_HD
                Case MFMTrackFormat.TrackFormatED
                    Return PSI.DefaultSectorFormat.IBM_MFM_ED
                Case Else
                    Return 0
            End Select

        End Function

        Private Function GetPSISectorFormat(DiskFormat As FloppyDiskFormat) As PSI.DefaultSectorFormat
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy1200
                    Return PSI.DefaultSectorFormat.IBM_MFM_HD
                Case FloppyDiskFormat.Floppy1440
                    Return PSI.DefaultSectorFormat.IBM_MFM_HD
                Case FloppyDiskFormat.Floppy2880
                    Return PSI.DefaultSectorFormat.IBM_MFM_ED
                Case Else
                    Return PSI.DefaultSectorFormat.IBM_MFM_DD
            End Select
        End Function

        Private Function GetTrackCount(Params As FloppyDiskParams) As UShort
            Return Params.SectorCountSmall \ Params.SectorsPerTrack \ Params.NumberOfHeads
        End Function

        Private Function GetTranscopyDiskType(TrackFormat As MFMTrackFormat) As TC.TransCopyDiskType
            Select Case TrackFormat
                Case MFMTrackFormat.TrackFormatDD
                    Return TC.TransCopyDiskType.MFMDoubleDensity
                Case MFMTrackFormat.TrackFormatHD
                    Return TC.TransCopyDiskType.MFMHighDensity
                Case MFMTrackFormat.TrackFormatHD1200
                    Return TC.TransCopyDiskType.MFMHighDensity
                Case MFMTrackFormat.TrackFormatED
                    Return TC.TransCopyDiskType.MFMHighDensity
                Case Else
                    Return TC.TransCopyDiskType.Unknown
            End Select
        End Function

        Private Function GetTranscopyDiskType(DiskFormat As FloppyDiskFormat) As TC.TransCopyDiskType
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy1200
                    Return TC.TransCopyDiskType.MFMHighDensity
                Case FloppyDiskFormat.Floppy1440
                    Return TC.TransCopyDiskType.MFMHighDensity
                Case FloppyDiskFormat.Floppy2880
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

        Private Function MFMBitstreamFromSectorImage(Data() As Byte, Params As FloppyDiskParams, Track As UShort, Side As Byte, RPM As UShort, BitRate As UShort) As IBM_MFM_Bitstream
            Dim MFMBitstream = New IBM_MFM_Bitstream(MFM_GAP4A_SIZE, MFM_GAP1_SIZE)

            For SectorId = 1 To Params.SectorsPerTrack
                Dim ImageOffset = GetImageOffset(Params, Track, Side, SectorId)
                Dim Size = Math.Min(Params.BytesPerSector, Data.Length - ImageOffset)
                Dim Buffer = New Byte(Size - 1) {}
                Array.Copy(Data, ImageOffset, Buffer, 0, Size)

                MFMBitstream.AddSectorId(Track, Side, SectorId, IBM_MFM_Bitstream.MFMSectorSize.SectorSize_512)
                MFMBitstream.AddData(Buffer, MFM_GAP3_SIZE)
            Next

            MFMBitstream.Finish(RPM, BitRate)

            Return MFMBitstream
        End Function

        Private Function PSISectorFromMFMSector(Sector As IBM_MFM_Sector) As PSI.PSISector
            Dim DataChecksumValid = Sector.DataChecksum = Sector.CalculateDataChecksum

            Dim PSISector = New PSI.PSISector With {
                   .HasDataCRCError = Not DataChecksumValid,
                   .IsAlternateSector = False,
                   .Track = Sector.Track,
                   .Side = Sector.Side,
                   .Sector = Sector.SectorId,
                   .Data = Sector.Data,
                   .Offset = (Sector.Offset + 64) / 2
                }

            Return PSISector
        End Function

        Private Class DriveSpeed
            Private _BitRate As UShort
            Private _RPM As UShort
            Public Sub New()
                _RPM = 0
                _BitRate = 0
            End Sub

            Public Property BitRate As UShort
                Get
                    Return _BitRate
                End Get
                Set
                    _BitRate = Value
                End Set
            End Property

            Public Property RPM As UShort
                Get
                    Return _RPM
                End Get
                Set
                    _RPM = Value
                End Set
            End Property

            Public Sub SetValue(RPM As UShort, BitRate As UShort)
                _RPM = RPM
                _BitRate = BitRate
            End Sub
        End Class
    End Module
End Namespace
