Imports DiskImageTool.Bitstream
Imports DiskImageTool.Bitstream.IBM_MFM
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace ImageFormats
    Module ImageConversion
        Private Class DriveSpeed
            Private _RPM As UShort
            Private _BitRate As UShort

            Public Sub New()
                _RPM = 0
                _BitRate = 0
            End Sub

            Public Sub SetValue(RPM As UShort, BitRate As UShort)
                _RPM = RPM
                _BitRate = BitRate
            End Sub

            Public Property RPM As UShort
                Get
                    Return _RPM
                End Get
                Set
                    _RPM = Value
                End Set
            End Property

            Public Property BitRate As UShort
                Get
                    Return _BitRate
                End Get
                Set
                    _BitRate = Value
                End Set
            End Property
        End Class

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

        Public Function BasicSectorToPSIImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As PSI.PSISectorImage
            Dim Params = GetFloppyDiskParams(DiskFormat)
            Dim CylinderCount = GetTrackCount(Params)

            Dim PSI = New PSI.PSISectorImage
            PSI.Header.FormatVersion = 0
            PSI.Header.DefaultSectorFormat = GetPSISectorFormat(DiskFormat)

            For Cylinder As UShort = 0 To CylinderCount - 1
                For Head = 0 To Params.NumberOfHeads - 1
                    Dim TrackOffset As UInteger = MFM_GAP4A_SIZE + MFM_SYNC_SIZE + MFM_ADDRESS_MARK_SIZE + MFM_GAP1_SIZE
                    For SectorId = 1 To Params.SectorsPerTrack
                        TrackOffset += MFM_SYNC_SIZE + MFM_ADDRESS_MARK_SIZE
                        Dim ImageOffset = GetImageOffset(Params, Cylinder, Head, SectorId)
                        Dim Size = Math.Min(Params.BytesPerSector, Data.Length - ImageOffset)
                        Dim Buffer = New Byte(Size - 1) {}
                        Array.Copy(Data, ImageOffset, Buffer, 0, Size)

                        Dim PSISector = New PSI.PSISector With {
                                .HasDataCRCError = False,
                                .IsAlternateSector = False,
                                .Cylinder = Cylinder,
                                .Head = Head,
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

        Public Function BasicSectorToHFEImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As HFE.HFEImage
            Dim Params = GetFloppyDiskParams(DiskFormat)
            Dim DriveSpeed = GetDriveSpeed(DiskFormat)
            Dim TrackCount = GetTrackCount(Params)

            Dim HFE = New HFE.HFEImage(TrackCount, Params.NumberOfHeads) With {
                .BitRate = DriveSpeed.BitRate,
                .FloppyRPM = DriveSpeed.RPM
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

        Public Function BasicSectorToTranscopyImage(Data() As Byte, DiskFormat As FloppyDiskFormat) As TC.TransCopyImage
            Dim Params = GetFloppyDiskParams(DiskFormat)
            Dim DriveSpeed = GetDriveSpeed(DiskFormat)
            Dim CylinderCount = GetTrackCount(Params)

            Dim Transcopy = New TC.TransCopyImage With {
                .DiskType = GetTranscopyDiskType(DiskFormat)
            }
            Transcopy.Initialize(CylinderCount, Params.NumberOfHeads, 1)

            For Cylinder As UShort = 0 To CylinderCount - 1
                For Head As UShort = 0 To Params.NumberOfHeads - 1
                    Dim TransCopyCylinder = New TC.TransCopyCylinder(Cylinder, Head) With {
                        .TrackType = GetTranscopyDiskType(DiskFormat),
                        .CopyAcrossIndex = True
                    }

                    Dim MFMBitstream = MFMBitstreamFromSectorImage(Data, Params, Cylinder, Head, DriveSpeed.RPM, DriveSpeed.BitRate)

                    TransCopyCylinder.Bitstream = MFMBitstream.Bitstream
                    TransCopyCylinder.SetTimings(MFMBitstream.AddressMarkIndexes)

                    Transcopy.SetCylinder(Cylinder, Head, TransCopyCylinder)
                Next
            Next

            Return Transcopy
        End Function

        Public Function BitstreamTo86FImage(Image As IBitstreamImage) As _86F._86FImage
            Dim BitstreamTrack As IBitstreamTrack
            Dim TrackCount As UShort = 0

            For Track = 0 To Image.TrackCount - 1 Step Image.TrackStep
                BitstreamTrack = Image.GetTrack(Track, 0)
                If BitstreamTrack.TrackType <> BitstreamTrackType.Other Then
                    TrackCount = Track + 1
                End If
            Next

            Dim F86Image = New _86F._86FImage(TrackCount, Image.SideCount) With {
                .BitcellMode = True,
                .AlternateBitcellCalculation = True
            }

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    If Track = 0 And Side = 0 Then
                        If BitstreamTrack.MFMData IsNot Nothing Then
                            F86Image.Hole = Get86FHole(GetTrackFormat(BitstreamTrack.MFMData.Size))
                        End If
                    End If

                    Dim IsMFM As Boolean = BitstreamTrack.TrackType = BitstreamTrackType.MFM Or BitstreamTrack.TrackType = BitstreamTrackType.Other
                    Dim BitRate As UShort = RoundBitRate(BitstreamTrack.BitRate)

                    If Image.TrackStep = 2 Then
                        If IsMFM Then
                            If BitRate = 300 Then
                                BitRate = 250
                            End If
                        Else
                            If BitRate = 150 Then
                                BitRate = 125
                            End If
                        End If
                    End If

                    Dim F86Track = New _86F._86FTrack(Track, Side, 0) With {
                        .Bitstream = BitstreamTrack.Bitstream,
                        .BitCellCount = BitstreamTrack.Bitstream.Length,
                        .BitRate = _86F.GetBitRate(BitRate, IsMFM),
                        .RPM = _86F.GetRPM(BitstreamTrack.RPM)
                    }

                    If BitstreamTrack.TrackType = BitstreamTrackType.FM Then
                        F86Track.Encoding = _86F.Encoding.FM
                    Else
                        F86Track.Encoding = _86F.Encoding.MFM
                    End If

                    F86Image.SetTrack(Track \ Image.TrackStep, Side, F86Track)
                Next
            Next

            Return F86Image
        End Function

        Public Function BitstreamToMFMImage(Image As IBitstreamImage) As MFM.MFMImage
            Dim BitstreamTrack As IBitstreamTrack
            Dim TrackCount As UShort = 0

            For Track = 0 To Image.TrackCount - 1 Step Image.TrackStep
                BitstreamTrack = Image.GetTrack(Track, 0)
                If BitstreamTrack.TrackType = BitstreamTrackType.MFM Then
                    TrackCount = Track + 1
                End If
            Next

            Dim MFM = New MFM.MFMImage(TrackCount, Image.SideCount, Image.TrackStep) With {
                .BitRate = 0,
                .RPM = 0
            }

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    If Track = 0 And Side = 0 Then
                        MFM.BitRate = BitstreamTrack.BitRate
                        MFM.RPM = BitstreamTrack.RPM
                    End If

                    Dim MFMTrack = New MFM.MFMTrack(Track, Side) With {
                        .Bitstream = BitstreamTrack.Bitstream,
                        .BitRate = BitstreamTrack.BitRate,
                        .RPM = BitstreamTrack.RPM
                    }

                    MFM.SetTrack(Track \ Image.TrackStep, Side, MFMTrack)
                Next
            Next

            Return MFM
        End Function

        Public Function BitstreamToHFEImage(Image As IBitstreamImage) As HFE.HFEImage
            Dim BitstreamTrack As IBitstreamTrack
            Dim TrackCount As UShort = 0

            For Track = 0 To Image.TrackCount - 1 Step Image.TrackStep
                BitstreamTrack = Image.GetTrack(Track, 0)
                If BitstreamTrack.TrackType = BitstreamTrackType.MFM Then
                    TrackCount = Track + 1
                End If
            Next

            Dim HFE = New HFE.HFEImage(TrackCount, Image.SideCount) With {
                .BitRate = 0,
                .FloppyRPM = 0
            }

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    If Track = 0 And Side = 0 Then
                        HFE.BitRate = BitstreamTrack.BitRate
                        HFE.FloppyRPM = BitstreamTrack.RPM
                    End If

                    Dim HFETrack = New HFE.HFETrack(Track, Side) With {
                        .Bitstream = BitstreamTrack.Bitstream,
                        .BitRate = BitstreamTrack.BitRate,
                        .RPM = BitstreamTrack.RPM
                    }

                    HFE.SetTrack(Track \ Image.TrackStep, Side, HFETrack)
                Next
            Next

            Return HFE
        End Function

        Public Function BitstreamToPSIImage(Image As IBitstreamImage) As PSI.PSISectorImage
            Dim PSI = New PSI.PSISectorImage
            Dim BitstreamTrack As IBitstreamTrack
            Dim DiskFormat As MFMTrackFormat = MFMTrackFormat.TrackFormatUnknown
            Dim TrackCount As UShort

            For Track = 0 To Image.TrackCount - 1 Step Image.TrackStep
                BitstreamTrack = Image.GetTrack(Track, 0)
                If BitstreamTrack.TrackType = BitstreamTrackType.MFM Then
                    TrackCount = Track + 1
                End If
            Next

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
            Dim DiskType As TC.TransCopyDiskType = TC.TransCopyDiskType.Unknown
            Dim TrackCount As UShort = 0

            For Track = 0 To Image.TrackCount - 1 Step Image.TrackStep
                BitstreamTrack = Image.GetTrack(Track, 0)
                If BitstreamTrack.TrackType <> BitstreamTrackType.Other Then
                    TrackCount = Track + 1
                End If
            Next

            Dim Transcopy = New TC.TransCopyImage
            Transcopy.Initialize(TrackCount \ Image.TrackStep, Image.SideCount, 1)

            For Track = 0 To TrackCount - 1 Step Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    BitstreamTrack = Image.GetTrack(Track, Side)

                    Dim TrackType As TC.TransCopyDiskType
                    Dim BitCount As Integer = 6250 * 16

                    If BitstreamTrack.TrackType = BitstreamTrackType.MFM And BitstreamTrack.Decoded Then
                        TrackType = GetTranscopyDiskType(GetTrackFormat(BitstreamTrack.MFMData.Size))
                        If DiskType = TC.TransCopyDiskType.Unknown And TrackType <> TC.TransCopyDiskType.Unknown Then
                            DiskType = TrackType
                        End If
                    ElseIf BitstreamTrack.TrackType = BitstreamTrackType.FM Then
                        BitCount = 3125 * 16
                        TrackType = TC.TransCopyDiskType.FMSingleDensity
                    Else
                        TrackType = TC.TransCopyDiskType.Unknown
                    End If

                    Dim TransCopyCylinder = New TC.TransCopyCylinder(Track, Side) With {
                        .TrackType = TrackType,
                        .CopyAcrossIndex = True
                    }
                    If BitstreamTrack.Bitstream.Length > 0 Then
                        TransCopyCylinder.Bitstream = BitstreamTrack.Bitstream
                    Else
                        TransCopyCylinder.Bitstream = New BitArray(BitCount)
                    End If

                    If BitstreamTrack.MFMData IsNot Nothing Then
                        TransCopyCylinder.SetTimings(BitstreamTrack.MFMData.AddressMarkIndexes)
                    End If
                    Transcopy.SetCylinder(Track \ Image.TrackStep, Side, TransCopyCylinder)
                Next
            Next

            Transcopy.DiskType = DiskType

            Return Transcopy
        End Function

        Private Function GetTrackCount(Params As FloppyDiskParams) As UShort
            Return Params.SectorCountSmall \ Params.SectorsPerTrack \ Params.NumberOfHeads
        End Function


        Private Function GetImageOffset(Params As FloppyDiskParams, Track As UShort, Side As UShort, SectorId As UShort) As UInteger
            Return (Track * Params.NumberOfHeads * Params.SectorsPerTrack + Params.SectorsPerTrack * Side + (SectorId - 1)) * Params.BytesPerSector
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
                   .Cylinder = Sector.Track,
                   .Head = Sector.Side,
                   .Sector = Sector.SectorId,
                   .Data = Sector.Data,
                   .Offset = (Sector.Offset + 64) / 2
                }

            Return PSISector
        End Function
    End Module
End Namespace
