
Imports DiskImageTool.Bitstream.IBM_MFM

Namespace DiskImage
    Public Module FloppyDiskFunctions
        Public Enum FloppyDiskFormat As Byte
            FloppyUnknown = 0
            Floppy160 = 1
            Floppy180 = 2
            Floppy320 = 3
            Floppy360 = 4
            Floppy720 = 5
            Floppy1200 = 6
            Floppy1440 = 7
            Floppy2880 = 8
            FloppyDMF1024 = 9
            FloppyDMF2048 = 10
            FloppyProCopy = 11
            FloppyXDF35 = 12
            FloppyXDF525 = 13
            FloppyXDFMicro = 14
            FloppyTandy2000 = 15
            FloppyNoBPB = 16
            Floppy2HD = 17
        End Enum

        Public Function BPBCompare(BPB As BiosParameterBlock, Params As FloppyDiskParams, CheckMediaDescriptor As Boolean) As Boolean
            Return BPB.BytesPerSector = Params.BytesPerSector _
                AndAlso BPB.NumberOfFATs = Params.NumberOfFATs _
                AndAlso BPB.NumberOfHeads = Params.NumberOfHeads _
                AndAlso BPB.ReservedSectorCount = Params.ReservedSectorCount _
                AndAlso BPB.RootEntryCount = Params.RootEntryCount _
                AndAlso BPB.SectorCountSmall = Params.SectorCountSmall _
                AndAlso BPB.SectorsPerCluster = Params.SectorsPerCluster _
                AndAlso BPB.SectorsPerFAT = Params.SectorsPerFAT _
                AndAlso BPB.SectorsPerTrack = Params.SectorsPerTrack _
                AndAlso (Not CheckMediaDescriptor Or BPB.MediaDescriptor = Params.MediaDescriptor)
        End Function

        Public Function BuildBPB(Params As FloppyDiskParams) As BiosParameterBlock
            Dim BPB = New BiosParameterBlock()

            With Params
                BPB.BytesPerSector = .BytesPerSector
                BPB.MediaDescriptor = .MediaDescriptor
                BPB.NumberOfFATs = .NumberOfFATs
                BPB.NumberOfHeads = .NumberOfHeads
                BPB.ReservedSectorCount = .ReservedSectorCount
                BPB.RootEntryCount = .RootEntryCount
                BPB.SectorCountSmall = .SectorCountSmall
                BPB.SectorsPerCluster = .SectorsPerCluster
                BPB.SectorsPerFAT = .SectorsPerFAT
                BPB.SectorsPerTrack = .SectorsPerTrack
            End With

            Return BPB
        End Function

        Public Function BuildBPB(DiskFormat As FloppyDiskFormat) As BiosParameterBlock
            Return BuildBPB(GetFloppyDiskParams(DiskFormat))
        End Function

        Public Function BuildBPB(Size As Integer) As BiosParameterBlock
            Return BuildBPB(GetFloppyDiskParams(Size))
        End Function

        Public Function BuildBPB(MediaDescriptor As Byte) As BiosParameterBlock
            Return BuildBPB(GetFloppyDiskParams(GetFloppyDiskFomat(MediaDescriptor)))
        End Function

        Public Function DiskHasWriteSplices(Disk As Disk) As Boolean
            Dim Result As Boolean = False

            If Disk.Image.IsBitstreamImage Then
                Dim Image = Disk.Image.BitstreamImage

                For Track = 0 To Image.TrackCount - 1 Step Image.TrackStep
                    For Side = 0 To Image.SideCount - 1
                        Dim BitstreamTrack = Image.GetTrack(Track, Side)
                        If BitstreamTrack.TrackType = Bitstream.BitstreamTrackType.MFM Or BitstreamTrack.TrackType = Bitstream.BitstreamTrackType.FM Then
                            Dim RegionData = Bitstream.IBM_MFM.MFMGetRegionList(BitstreamTrack.Bitstream, BitstreamTrack.TrackType)
                            For Each RegionSector In RegionData.Sectors
                                If RegionSector.WriteSplice Then
                                    Result = True
                                    Exit For
                                End If
                            Next
                        End If
                        If Result Then
                            Exit For
                        End If
                    Next
                    If Result Then
                        Exit For
                    End If
                Next
            End If

            Return Result
        End Function

        Public Function GetDirectoryEntryFromCluster(Disk As Disk, Cluster As Integer) As DirectoryEntry
            If Disk.IsValidImage Then
                If Disk.RootDirectory.FATAllocation.FileAllocation.ContainsKey(Cluster) Then
                    Dim OffsetList = Disk.RootDirectory.FATAllocation.FileAllocation.Item(Cluster)
                    Return OffsetList.Item(0)
                End If
            End If

            Return Nothing
        End Function

        Public Function GetFATIndex(Disk As Disk, Sector As UInteger) As Integer
            Dim NumberOfFATs As Byte

            If IsDiskFormatXDF(Disk.DiskFormat) Then
                NumberOfFATs = 1
            Else
                NumberOfFATs = Disk.BPB.NumberOfFATs
            End If

            For Index As Byte = 0 To NumberOfFATs - 1
                Dim Length As UInteger = Disk.BPB.SectorsPerFAT
                Dim Start As UInteger = Disk.BPB.FATRegionStart + Length * Index

                If Sector >= Start And Sector < Start + Length Then
                    Return Index
                End If
            Next

            Return 0
        End Function

        Public Function GetFloppyDiskMediaDescriptor(Size As Integer) As Byte
            Return GetFloppyDiskMediaDescriptor(GetFloppyDiskFormat(Size))
        End Function

        Public Function GetFloppyDiskMediaDescriptor(DiskFormat As FloppyDiskFormat) As Byte
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160
                    Return &HFE
                Case FloppyDiskFormat.Floppy180
                    Return &HFC
                Case FloppyDiskFormat.Floppy320
                    Return &HFF
                Case FloppyDiskFormat.Floppy360
                    Return &HFD
                Case FloppyDiskFormat.Floppy720
                    Return &HF9
                Case FloppyDiskFormat.Floppy1200
                    Return &HF9
                Case FloppyDiskFormat.Floppy1440
                    Return &HF0
                Case FloppyDiskFormat.FloppyDMF1024
                    Return &HF0
                Case FloppyDiskFormat.FloppyDMF2048
                    Return &HF0
                Case FloppyDiskFormat.Floppy2880
                    Return &HF0
                Case FloppyDiskFormat.FloppyProCopy
                    Return &HF0
                Case FloppyDiskFormat.FloppyXDF35
                    Return &HF0
                Case FloppyDiskFormat.FloppyXDF525
                    Return &HF9
                Case FloppyDiskFormat.FloppyXDFMicro
                    Return &HF9
                Case FloppyDiskFormat.FloppyTandy2000
                    Return &HED
                Case FloppyDiskFormat.Floppy2HD
                    Return &HFE
                Case Else
                    Return &HF0
            End Select
        End Function

        Public Function GetFloppyDiskGaps(DiskFormat As FloppyDiskFormat) As FloppyDiskGaps
            Dim Gaps As FloppyDiskGaps

            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160, FloppyDiskFormat.Floppy180, FloppyDiskFormat.Floppy320, FloppyDiskFormat.Floppy360, FloppyDiskFormat.Floppy720
                    Gaps.Gap4A = 80
                    Gaps.Gap1 = 50
                    Gaps.Gap2 = 22
                    Gaps.Gap3 = 80
                Case FloppyDiskFormat.Floppy1200
                    Gaps.Gap4A = 80
                    Gaps.Gap1 = 50
                    Gaps.Gap2 = 22
                    Gaps.Gap3 = 84
                Case FloppyDiskFormat.Floppy1440
                    Gaps.Gap4A = 80
                    Gaps.Gap1 = 50
                    Gaps.Gap2 = 22
                    Gaps.Gap3 = 108
                Case FloppyDiskFormat.Floppy2880
                    Gaps.Gap4A = 80
                    Gaps.Gap1 = 50
                    Gaps.Gap2 = 41
                    Gaps.Gap3 = 84
                Case FloppyDiskFormat.FloppyProCopy
                    Gaps.Gap4A = 80
                    Gaps.Gap1 = 50
                    Gaps.Gap2 = 22
                    Gaps.Gap3 = 100
                Case FloppyDiskFormat.FloppyDMF1024, FloppyDiskFormat.FloppyDMF2048
                    Gaps.Gap4A = 0
                    Gaps.Gap1 = 108
                    Gaps.Gap2 = 22
                    Gaps.Gap3 = 8
                Case Else
                    Gaps.Gap4A = 80
                    Gaps.Gap1 = 50
                    Gaps.Gap2 = 22
                    Gaps.Gap3 = 80
            End Select

            Return Gaps
        End Function


        Public Function GetFloppyDiskParams(DiskFormat As FloppyDiskFormat) As FloppyDiskParams
            Dim Params As FloppyDiskParams

            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HFE
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 1
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 64
                    Params.SectorCountSmall = 320
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 1
                    Params.SectorsPerTrack = 8

                Case FloppyDiskFormat.Floppy180
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HFC
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 1
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 64
                    Params.SectorCountSmall = 360
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 2
                    Params.SectorsPerTrack = 9

                Case FloppyDiskFormat.Floppy320
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HFF
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 112
                    Params.SectorCountSmall = 640
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 1
                    Params.SectorsPerTrack = 8

                Case FloppyDiskFormat.Floppy360
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HFD
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 112
                    Params.SectorCountSmall = 720
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 2
                    Params.SectorsPerTrack = 9

                Case FloppyDiskFormat.Floppy720
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF9
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 112
                    Params.SectorCountSmall = 1440
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 3
                    Params.SectorsPerTrack = 9

                Case FloppyDiskFormat.Floppy1200
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF9
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 224
                    Params.SectorCountSmall = 2400
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 7
                    Params.SectorsPerTrack = 15

                Case FloppyDiskFormat.Floppy1440
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 224
                    Params.SectorCountSmall = 2880
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 9
                    Params.SectorsPerTrack = 18

                Case FloppyDiskFormat.FloppyDMF1024
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 16
                    Params.SectorCountSmall = 3360
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 5
                    Params.SectorsPerTrack = 21

                Case FloppyDiskFormat.FloppyDMF2048
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 16
                    Params.SectorCountSmall = 3360
                    Params.SectorsPerCluster = 4
                    Params.SectorsPerFAT = 3
                    Params.SectorsPerTrack = 21

                Case FloppyDiskFormat.Floppy2880
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 240
                    Params.SectorCountSmall = 5760
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 9
                    Params.SectorsPerTrack = 36

                Case FloppyDiskFormat.FloppyProCopy
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 16
                    Params.SectorCountSmall = 2880
                    Params.SectorsPerCluster = 2
                    Params.SectorsPerFAT = 5
                    Params.SectorsPerTrack = 18

                Case FloppyDiskFormat.FloppyXDF35
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF0
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 224
                    Params.SectorCountSmall = 3680
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 11
                    Params.SectorsPerTrack = 23

                Case FloppyDiskFormat.FloppyXDF525
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF9
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 224
                    Params.SectorCountSmall = 3040
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 9
                    Params.SectorsPerTrack = 19

                Case FloppyDiskFormat.FloppyXDFMicro
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HF9
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 1
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 16
                    Params.SectorCountSmall = 8
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 1
                    Params.SectorsPerTrack = 8

                Case FloppyDiskFormat.FloppyTandy2000
                    Params.BytesPerSector = 512
                    Params.MediaDescriptor = &HED
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 112
                    Params.SectorCountSmall = 1440
                    Params.SectorsPerCluster = 4
                    Params.SectorsPerFAT = 2
                    Params.SectorsPerTrack = 9

                Case FloppyDiskFormat.Floppy2HD
                    Params.BytesPerSector = 1024
                    Params.MediaDescriptor = &HFE
                    Params.NumberOfFATs = 2
                    Params.NumberOfHeads = 2
                    Params.ReservedSectorCount = 1
                    Params.RootEntryCount = 192
                    Params.SectorCountSmall = 1232
                    Params.SectorsPerCluster = 1
                    Params.SectorsPerFAT = 2
                    Params.SectorsPerTrack = 8

                Case Else
                    Params.BytesPerSector = 0
                    Params.MediaDescriptor = 0
                    Params.NumberOfFATs = 0
                    Params.NumberOfHeads = 0
                    Params.ReservedSectorCount = 0
                    Params.RootEntryCount = 0
                    Params.SectorCountSmall = 0
                    Params.SectorsPerCluster = 0
                    Params.SectorsPerFAT = 0
                    Params.SectorsPerTrack = 0
            End Select

            Return Params
        End Function

        Public Function GetFloppyDiskParams(Size As Integer) As FloppyDiskParams
            Return GetFloppyDiskParams(GetFloppyDiskFormat(Size))
        End Function

        Public Function GetFloppyDiskFomat(MediaDescriptor As Byte) As FloppyDiskFormat
            Select Case MediaDescriptor
                Case &HFE
                    Return FloppyDiskFormat.Floppy160
                Case &HFC
                    Return FloppyDiskFormat.Floppy180
                Case &HFF
                    Return FloppyDiskFormat.Floppy320
                Case &HFD
                    Return FloppyDiskFormat.Floppy360
                Case &HED
                    Return FloppyDiskFormat.FloppyTandy2000
                Case Else
                    Return FloppyDiskFormat.FloppyUnknown
            End Select
        End Function

        Public Function GetFloppyDiskFormat(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean) As FloppyDiskFormat
            Return GetFloppyDiskFormat(BPB, CheckMediaDescriptor, True)
        End Function

        Public Function GetFloppyDiskFormat(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean, IgnoreNoBPB As Boolean) As FloppyDiskFormat
            Dim Items = System.Enum.GetValues(GetType(FloppyDiskFormat))

            For Each DiskFormat As FloppyDiskFormat In Items
                If DiskFormat <> FloppyDiskFormat.FloppyUnknown Or (IgnoreNoBPB And DiskFormat <> FloppyDiskFormat.FloppyNoBPB) Then
                    Dim Params = GetFloppyDiskParams(DiskFormat)
                    If BPBCompare(BPB, Params, CheckMediaDescriptor) Then
                        Return DiskFormat
                    End If
                End If
            Next

            Return FloppyDiskFormat.FloppyUnknown
        End Function

        Public Function GetFloppyDiskSize(DiskFormat As FloppyDiskFormat) As Integer
            Dim Size As Integer

            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160
                    Size = 160
                Case FloppyDiskFormat.Floppy180
                    Size = 180
                Case FloppyDiskFormat.Floppy320
                    Size = 320
                Case FloppyDiskFormat.Floppy360
                    Size = 360
                Case FloppyDiskFormat.Floppy720
                    Size = 720
                Case FloppyDiskFormat.Floppy1200
                    Size = 1200
                Case FloppyDiskFormat.Floppy1440
                    Size = 1440
                Case FloppyDiskFormat.FloppyDMF1024
                    Size = 1680
                Case FloppyDiskFormat.FloppyDMF2048
                    Size = 1680
                Case FloppyDiskFormat.FloppyProCopy
                    Size = 1440
                Case FloppyDiskFormat.FloppyXDF525
                    Size = 1520
                Case FloppyDiskFormat.FloppyXDF35
                    Size = 1840
                Case FloppyDiskFormat.FloppyXDFMicro
                    Size = 4
                Case FloppyDiskFormat.Floppy2880
                    Size = 2880
                Case FloppyDiskFormat.FloppyTandy2000
                    Size = 720
                Case FloppyDiskFormat.Floppy2HD
                    Size = 1232
                Case Else
                    Size = 0
            End Select

            Return Size * 1024
        End Function

        Public Function GetFloppyDiskFormat(Size As Integer) As FloppyDiskFormat
            Size = Math.Round(Size / 2048, 0, MidpointRounding.AwayFromZero) * 2

            Select Case Size
                Case 160
                    Return FloppyDiskFormat.Floppy160
                Case 180
                    Return FloppyDiskFormat.Floppy180
                Case 320
                    Return FloppyDiskFormat.Floppy320
                Case 360
                    Return FloppyDiskFormat.Floppy360
                Case 720
                    Return FloppyDiskFormat.Floppy720
                Case 1200
                    Return FloppyDiskFormat.Floppy1200
                Case 1440
                    Return FloppyDiskFormat.Floppy1440
                Case 1680
                    Return FloppyDiskFormat.FloppyDMF2048
                Case 1520
                    Return FloppyDiskFormat.FloppyXDF525
                Case 1840
                    Return FloppyDiskFormat.FloppyXDF35
                Case 4
                    Return FloppyDiskFormat.FloppyXDFMicro
                Case 2880
                    Return FloppyDiskFormat.Floppy2880
                Case 1232
                    Return FloppyDiskFormat.Floppy2HD
                Case Else
                    Return FloppyDiskFormat.FloppyUnknown
            End Select
        End Function

        Public Function GetFloppyDiskFormat(Name As String) As FloppyDiskFormat
            Select Case Name
                Case "160K"
                    Return FloppyDiskFormat.Floppy160
                Case "180K"
                    Return FloppyDiskFormat.Floppy180
                Case "320K"
                    Return FloppyDiskFormat.Floppy320
                Case "360K"
                    Return FloppyDiskFormat.Floppy360
                Case "720K"
                    Return FloppyDiskFormat.Floppy720
                Case "1.2M"
                    Return FloppyDiskFormat.Floppy1200
                Case "1.44M"
                    Return FloppyDiskFormat.Floppy1440
                Case "DMF (1024)"
                    Return FloppyDiskFormat.FloppyDMF1024
                Case "DMF (2048)"
                    Return FloppyDiskFormat.FloppyDMF2048
                Case "2.88M"
                    Return FloppyDiskFormat.Floppy2880
                Case "ProCopy"
                    Return FloppyDiskFormat.FloppyProCopy
                Case "XDF 5.25"""
                    Return FloppyDiskFormat.FloppyXDF525
                Case "XDF 3.5"""
                    Return FloppyDiskFormat.FloppyXDF35
                Case "XDF Micro"
                    Return FloppyDiskFormat.FloppyXDFMicro
                Case "Tandy 2000"
                    Return FloppyDiskFormat.FloppyTandy2000
                Case "2HD (1.23M)"
                    Return FloppyDiskFormat.Floppy2HD
                Case "NO BPB"
                    Return FloppyDiskFormat.FloppyNoBPB
                Case Else
                    Return FloppyDiskFormat.FloppyUnknown
            End Select
        End Function

        Public Function GetFloppyDiskFormatName(DiskFormat As FloppyDiskFormat, Optional Extended As Boolean = False) As String
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160
                    Return "160K"
                Case FloppyDiskFormat.Floppy180
                    Return "180K"
                Case FloppyDiskFormat.Floppy320
                    Return "320K"
                Case FloppyDiskFormat.Floppy360
                    Return "360K"
                Case FloppyDiskFormat.Floppy720
                    Return "720K"
                Case FloppyDiskFormat.Floppy1200
                    Return "1.2M"
                Case FloppyDiskFormat.Floppy1440
                    Return "1.44M"
                Case FloppyDiskFormat.FloppyDMF1024
                    If Extended Then
                        Return "DMF (1024)"
                    Else
                        Return "DMF"
                    End If
                Case FloppyDiskFormat.FloppyDMF2048
                    If Extended Then
                        Return "DMF (2048)"
                    Else
                        Return "DMF"
                    End If
                Case FloppyDiskFormat.Floppy2880
                    Return "2.88M"
                Case FloppyDiskFormat.FloppyProCopy
                    Return "ProCopy"
                Case FloppyDiskFormat.FloppyXDF525
                    Return "XDF 5.25"""
                Case FloppyDiskFormat.FloppyXDF35
                    Return "XDF 3.5"""
                Case FloppyDiskFormat.FloppyXDFMicro
                    Return "XDF Micro"
                Case FloppyDiskFormat.FloppyTandy2000
                    Return "Tandy 2000"
                Case FloppyDiskFormat.Floppy2HD
                    Return "2HD (1.23M)"
                Case FloppyDiskFormat.FloppyNoBPB
                    Return "No BPB"
                Case Else
                    Return "Custom"
            End Select
        End Function

        Public Function GetImageFileExtensionByFormat(DiskFormat As FloppyDiskFormat) As String
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy160
                    Return ".160"
                Case FloppyDiskFormat.Floppy180
                    Return ".180"
                Case FloppyDiskFormat.Floppy320
                    Return ".320"
                Case FloppyDiskFormat.Floppy360
                    Return ".360"
                Case FloppyDiskFormat.Floppy720
                    Return ".720"
                Case FloppyDiskFormat.Floppy1200
                    Return ".120"
                Case FloppyDiskFormat.Floppy1440
                    Return ".144"
                Case FloppyDiskFormat.FloppyDMF1024
                    Return ".dmf"
                Case FloppyDiskFormat.FloppyDMF2048
                    Return ".dmf"
                Case FloppyDiskFormat.Floppy2880
                    Return ".288"
                Case FloppyDiskFormat.FloppyProCopy
                    Return ""
                Case FloppyDiskFormat.FloppyXDF525
                    Return ".xdf"
                Case FloppyDiskFormat.FloppyXDF35
                    Return ".xdf"
                Case FloppyDiskFormat.FloppyXDFMicro
                    Return ".xdf"
                Case FloppyDiskFormat.Floppy2HD
                    Return ".hdm"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GetFileFilterDescriptionByFormat(DiskFormat As FloppyDiskFormat) As String
            Dim Description As String = GetFloppyDiskFormatName(DiskFormat)
            Select Case DiskFormat
                Case FloppyDiskFormat.FloppyProCopy
                    Description = ""
                Case FloppyDiskFormat.FloppyNoBPB
                    Description = ""
                Case FloppyDiskFormat.FloppyUnknown
                    Description = ""
            End Select

            If Description <> "" Then
                Description &= " Floppy Image"
            End If

            Return Description
        End Function

        Public Function GetFloppyDiskFormatName(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean, Optional Extended As Boolean = False) As String
            Return GetFloppyDiskFormatName(GetFloppyDiskFormat(BPB, CheckMediaDescriptor), Extended)
        End Function

        Public Function GetFloppyDiskFormatName(Size As Integer, Optional Extended As Boolean = False) As String
            Return GetFloppyDiskFormatName(GetFloppyDiskFormat(Size), Extended)
        End Function

        Public Function GetFloppyDiskFormatName(MediaDescriptor As Byte, Optional Extended As Boolean = False) As String
            Return GetFloppyDiskFormatName(GetFloppyDiskFomat(MediaDescriptor), Extended)
        End Function

        Public Function GetFloppyDiskRegionName(Disk As Disk, Sector As UInteger, Cluster As UShort) As String
            If Sector = 0 Then
                Return My.Resources.Label_BootSector
            ElseIf Disk.IsValidImage Then
                If Disk.RootDirectory.SectorChain.Contains(Sector) Then
                    Return My.Resources.HexView_RootDirectory
                ElseIf IsFATArea(Disk, Sector) Then
                    Dim FATIndex = GetFATIndex(Disk, Sector)
                    Return My.Resources.SummaryPanel_FAT & " " & (FATIndex + 1).ToString
                ElseIf Cluster > 1 Then
                    Return My.Resources.DataInspector_Label_DataArea
                End If
            End If

            Return ""
        End Function

        Public Function IsClusterEmpty(FloppyImage As IFloppyImage, BPB As BiosParameterBlock, Cluster As UShort) As Boolean
            Dim Offset = BPB.ClusterToOffset(Cluster)
            Dim ClusterSize As UInteger = BPB.BytesPerCluster()

            Dim Data = FloppyImage.GetBytes(Offset, ClusterSize)
            Dim EmptyByte As Byte = Data(0)
            If Not Disk.FreeClusterBytes.Contains(EmptyByte) Then
                Return False
            Else
                For Each B In Data
                    If B <> EmptyByte Then
                        Return False
                        Exit For
                    End If
                Next
            End If

            Return True
        End Function

        Public Function IsDiskFormatXDF(DiskFormat As FloppyDiskFormat) As Boolean
            Return (DiskFormat = FloppyDiskFormat.FloppyXDF525 Or DiskFormat = FloppyDiskFormat.FloppyXDF35)
        End Function

        Public Function IsDiskFormatValidForRead(DiskFormat As FloppyDiskFormat) As Boolean
            If DiskFormat = FloppyDiskFormat.FloppyDMF1024 _
                Or DiskFormat = FloppyDiskFormat.FloppyDMF2048 _
                Or DiskFormat = FloppyDiskFormat.FloppyNoBPB _
                Or DiskFormat = FloppyDiskFormat.FloppyProCopy _
                Or DiskFormat = FloppyDiskFormat.FloppyXDF35 _
                Or DiskFormat = FloppyDiskFormat.FloppyXDF525 _
                Or DiskFormat = FloppyDiskFormat.FloppyXDFMicro Then
                Return False
            End If

            Return True
        End Function

        Public Function IsFATArea(Disk As Disk, Sector As UInteger) As Boolean
            Dim NumberOfFATs As Byte

            If IsDiskFormatXDF(Disk.DiskFormat) Then
                NumberOfFATs = 1
            Else
                NumberOfFATs = Disk.BPB.NumberOfFATs
            End If

            Dim Length As UInteger = Disk.BPB.SectorsPerFAT * NumberOfFATs
            Dim Start As UInteger = Disk.BPB.FATRegionStart


            Return Sector >= Start And Sector < Start + Length
        End Function

        Public Structure FloppyDiskGaps
            Dim Gap4A As UInteger
            Dim Gap1 As UInteger
            Dim Gap2 As UInteger
            Dim Gap3 As UInteger
        End Structure

        Public Structure FloppyDiskParams
            Dim BytesPerSector As UShort
            Dim MediaDescriptor As Byte
            Dim NumberOfFATs As Byte
            Dim NumberOfHeads As UShort
            Dim ReservedSectorCount As UShort
            Dim RootEntryCount As UShort
            Dim SectorCountSmall As UShort
            Dim SectorsPerCluster As Byte
            Dim SectorsPerFAT As UShort
            Dim SectorsPerTrack As UShort
        End Structure
    End Module
End Namespace
