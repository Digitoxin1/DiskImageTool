Namespace DiskImage
    Public Module FloppyDiskFunctions
        Private ReadOnly DiskParamsArray As FloppyDiskParams() = {
            New FloppyDiskParams(
                FloppyDiskFormat.FloppyUnknown,
                New FloppyDiskBPBParams(0, &HF0, 0, 0, 0, 0, 0, 0, 0, 0),
                GapsStandard,
                "",
                FloppyDriveType.DriveUnknown
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.Floppy160,
                New FloppyDiskBPBParams(512, &HFE, 2, 1, 1, 64, 320, 1, 1, 8),
                GapsStandard,
                ".160",
                FloppyDriveType.Drive525DoubleDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.Floppy180,
                New FloppyDiskBPBParams(512, &HFC, 2, 1, 1, 64, 360, 1, 2, 9),
                GapsStandard,
                ".180",
                FloppyDriveType.Drive525DoubleDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.Floppy320,
                New FloppyDiskBPBParams(512, &HFF, 2, 2, 1, 112, 640, 2, 1, 8),
                GapsStandard,
                ".320",
                FloppyDriveType.Drive525DoubleDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.Floppy360,
                New FloppyDiskBPBParams(512, &HFD, 2, 2, 1, 112, 720, 2, 2, 9),
                GapsStandard,
                ".360",
                FloppyDriveType.Drive525DoubleDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.Floppy720,
                New FloppyDiskBPBParams(512, &HF9, 2, 2, 1, 112, 1440, 2, 3, 9),
                GapsStandard,
                ".720",
                FloppyDriveType.Drive35DoubleDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.Floppy1200,
                New FloppyDiskBPBParams(512, &HF9, 2, 2, 1, 224, 2400, 1, 7, 15),
                Gaps1200,
                ".120",
                FloppyDriveType.Drive525HighDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.Floppy1440,
                New FloppyDiskBPBParams(512, &HF0, 2, 2, 1, 224, 2880, 1, 9, 18),
                Gaps1440,
                ".144",
                FloppyDriveType.Drive35HighDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.Floppy2880,
                New FloppyDiskBPBParams(512, &HF0, 2, 2, 1, 240, 5760, 2, 9, 36),
                Gaps2880,
                ".288",
                FloppyDriveType.Drive35ExtraHighDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.FloppyDMF1024,
                New FloppyDiskBPBParams(512, &HF0, 2, 2, 1, 16, 3360, 2, 5, 21),
                GapsDmf,
                ".dmf",
                FloppyDriveType.Drive35HighDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.FloppyDMF2048,
                New FloppyDiskBPBParams(512, &HF0, 2, 2, 1, 16, 3360, 4, 3, 21),
                GapsDmf,
                ".dmf",
                FloppyDriveType.Drive35HighDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.FloppyProCopy,
                New FloppyDiskBPBParams(512, &HF0, 2, 2, 1, 16, 2880, 2, 5, 18),
                GapsProCopy,
                "",
                FloppyDriveType.Drive35HighDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.FloppyXDF35,
                New FloppyDiskBPBParams(512, &HF0, 2, 2, 1, 224, 3680, 1, 11, 23),
                GapsStandard,
                ".xdf",
                FloppyDriveType.Drive35HighDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.FloppyXDF525,
                New FloppyDiskBPBParams(512, &HF9, 2, 2, 1, 224, 3040, 1, 9, 19),
                GapsStandard,
                ".xdf",
                FloppyDriveType.Drive525HighDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.FloppyXDFMicro,
                New FloppyDiskBPBParams(512, &HF9, 2, 1, 1, 16, 8, 1, 1, 8),
                GapsStandard,
                ".xdf",
                FloppyDriveType.DriveUnknown
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.FloppyTandy2000,
                New FloppyDiskBPBParams(512, &HED, 2, 2, 1, 112, 1440, 4, 2, 9),
                GapsStandard,
                "",
                FloppyDriveType.Drive525DoubleDensity
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.FloppyNoBPB,
                New FloppyDiskBPBParams(0, &HF0, 0, 0, 0, 0, 0, 0, 0, 0),
                GapsStandard,
                "",
                FloppyDriveType.DriveUnknown
            ),
            New FloppyDiskParams(
                FloppyDiskFormat.Floppy2HD,
                New FloppyDiskBPBParams(1024, &HFE, 2, 2, 1, 192, 1232, 1, 2, 8),
                GapsStandard,
                ".hdm",
                FloppyDriveType.Drive35HighDensity
            )
        }

        Private ReadOnly Gaps1200 As New FloppyDiskGaps(80, 50, 22, 84)
        Private ReadOnly Gaps1440 As New FloppyDiskGaps(80, 50, 22, 108)
        Private ReadOnly Gaps2880 As New FloppyDiskGaps(80, 50, 41, 84)
        Private ReadOnly GapsDmf As New FloppyDiskGaps(0, 108, 22, 8)
        Private ReadOnly GapsProCopy As New FloppyDiskGaps(80, 50, 22, 100)
        Private ReadOnly GapsStandard As New FloppyDiskGaps(80, 50, 22, 80)

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

        Public Enum FloppyDiskRegionEnum
            None = 0
            BootSector = 1
            FAT1 = 2
            FAT2 = 4
            RootDirectory = 8
            DataArea = 16
        End Enum

        Public Enum FloppyDriveType As Byte
            DriveUnknown = 0
            Drive525DoubleDensity = 1
            Drive525HighDensity = 2
            Drive35DoubleDensity = 4
            Drive35HighDensity = 8
            Drive35ExtraHighDensity = 16
        End Enum

        Public Function BuildBPB(DiskFormat As FloppyDiskFormat) As BiosParameterBlock
            Return FloppyDiskFormatGetParams(DiskFormat).BPBParams.GetBPB
        End Function

        Public Function BuildBPB(Size As Integer) As BiosParameterBlock
            Return FloppyDiskBPBParamsGet(Size).GetBPB
        End Function

        Public Function BuildBPB(MediaDescriptor As Byte) As BiosParameterBlock
            Return FloppyDiskFormatGetParams(FloppyDiskFormatGet(MediaDescriptor)).BPBParams.GetBPB
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

        Public Function FloppyDiskBPBParamsGet(Size As Integer) As FloppyDiskBPBParams
            Return FloppyDiskFormatGetParams(FloppyDiskFormatGet(Size)).BPBParams
        End Function

        Public Function FloppyDiskFormatGet(MediaDescriptor As Byte) As FloppyDiskFormat
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

        Public Function FloppyDiskFormatGet(Buffer() As Byte) As FloppyDiskFormat
            If Buffer.Length >= 512 Then
                Dim BootSector As New BootSector(Buffer)
                Dim MediaDescriptor As Byte = 0
                If Buffer.Length = 513 Then
                    MediaDescriptor = Buffer(512)
                End If

                If BootSector.BPB.IsValid Then
                    Return FloppyDiskFormatGet(BootSector.BPB, False, True)
                Else
                    Return FloppyDiskFormatGet(MediaDescriptor)
                End If
            Else
                Return FloppyDiskFormat.FloppyUnknown
            End If
        End Function

        Public Function FloppyDiskFormatGet(BPB As BiosParameterBlock) As FloppyDiskFormat
            Return FloppyDiskFormatGet(BPB, False, True)
        End Function

        Public Function FloppyDiskFormatGet(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean) As FloppyDiskFormat
            Return FloppyDiskFormatGet(BPB, CheckMediaDescriptor, True)
        End Function

        Public Function FloppyDiskFormatGet(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean, IgnoreNoBPB As Boolean) As FloppyDiskFormat
            Dim Items = System.Enum.GetValues(GetType(FloppyDiskFormat))

            For Each DiskFormat As FloppyDiskFormat In Items
                If DiskFormat <> FloppyDiskFormat.FloppyUnknown Or (IgnoreNoBPB And DiskFormat <> FloppyDiskFormat.FloppyNoBPB) Then
                    Dim BPBParams = FloppyDiskFormatGetParams(DiskFormat).BPBParams
                    If BPBParams.CompareBPB(BPB, CheckMediaDescriptor) Then
                        Return DiskFormat
                    End If
                End If
            Next

            Return FloppyDiskFormat.FloppyUnknown
        End Function

        Public Function FloppyDiskFormatGet(Size As Integer) As FloppyDiskFormat
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

        Public Function FloppyDiskFormatGet(Name As String) As FloppyDiskFormat
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

        Public Function FloppyDiskFormatGetComboList(IncludeUnknown As Boolean) As List(Of Object)
            Dim result As New List(Of Object) From {
                My.Resources.Label_PleaseSelect, ' Add placeholder
                StrDup(30, ChrW(&H2014))
            }

            ' Standard 5.25"
            result.AddRange(
                DiskParamsArray.
                    Where(Function(p) p.IsStandard AndAlso p.Is525).
                    OrderBy(Function(p) p.BPBParams.SizeInBytes).
                    Cast(Of Object)
            )

            result.Add(StrDup(30, ChrW(&H2014)))

            ' Standard 3.5"            
            result.AddRange(
                DiskParamsArray.
                    Where(Function(p) p.IsStandard AndAlso Not p.Is525).
                    OrderBy(Function(p) p.BPBParams.SizeInBytes).
                    Cast(Of Object)
            )

            result.Add(StrDup(30, ChrW(&H2014)))

            ' Special 5.25"
            result.AddRange(
                DiskParamsArray.
                    Where(Function(p) Not p.IsStandard AndAlso Not p.IsNonImage AndAlso p.Is525).
                    OrderBy(Function(p) p.BPBParams.SizeInBytes).
                    Cast(Of Object)
            )

            ' Special 3.5"
            result.AddRange(
                DiskParamsArray.
                    Where(Function(p) Not p.IsStandard AndAlso Not p.IsNonImage AndAlso Not p.Is525).
                    OrderBy(Function(p) p.BPBParams.SizeInBytes).
                    Cast(Of Object)
            )

            If IncludeUnknown Then
                result.Add(StrDup(30, ChrW(&H2014)))

                result.Add(FloppyDiskFormatGetParams(FloppyDiskFormat.FloppyUnknown))
            End If

            Return result
        End Function

        Public Function FloppyDiskFormatGetFileFilterDescription(DiskFormat As FloppyDiskFormat) As String
            Dim Description As String = FloppyDiskFormatGetName(DiskFormat)
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

        Public Function FloppyDiskFormatGetName(DiskFormat As FloppyDiskFormat, Optional Extended As Boolean = False) As String
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
        Public Function FloppyDiskFormatGetName(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean, Optional Extended As Boolean = False) As String
            Return FloppyDiskFormatGetName(FloppyDiskFormatGet(BPB, CheckMediaDescriptor), Extended)
        End Function

        Public Function FloppyDiskFormatGetParams(Format As FloppyDiskFormat) As FloppyDiskParams
            If Format >= DiskParamsArray.Length Then
                Return DiskParamsArray(FloppyDiskFormat.FloppyUnknown)
            End If

            Return DiskParamsArray(Format)
        End Function

        Public Function FloppyDiskFormatIsStandard(DiskFormat As FloppyDiskFormat) As Boolean
            Select Case DiskFormat
                Case FloppyDiskFormat.Floppy2HD, FloppyDiskFormat.FloppyXDF35, FloppyDiskFormat.FloppyXDF525, FloppyDiskFormat.FloppyXDFMicro,
                             FloppyDiskFormat.FloppyTandy2000, FloppyDiskFormat.FloppyNoBPB, FloppyDiskFormat.FloppyUnknown

                    Return False
                Case Else
                    Return True
            End Select
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

            If Disk.DiskParams.IsXDF Then
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
        Public Function GetFloppyDiskRegionName(Disk As Disk, Sector As UInteger, Cluster As UShort) As (AreaName As String, Region As FloppyDiskRegionEnum)
            If Sector = 0 Then
                Return (My.Resources.Label_BootSector, FloppyDiskRegionEnum.BootSector)
            ElseIf Disk.IsValidImage Then
                If Disk.RootDirectory.SectorChain.Contains(Sector) Then
                    Return (WithoutHotkey(My.Resources.Menu_RootDirectory), FloppyDiskRegionEnum.RootDirectory)
                ElseIf IsFATArea(Disk, Sector) Then
                    Dim FATIndex = GetFATIndex(Disk, Sector)
                    Dim Region As FloppyDiskRegionEnum = If(FATIndex = 0, FloppyDiskRegionEnum.FAT1, FloppyDiskRegionEnum.FAT2)
                    Return (My.Resources.Label_FATShort & " " & (FATIndex + 1).ToString, Region)
                ElseIf Cluster > 1 Then
                    Return (My.Resources.Label_DataArea, FloppyDiskRegionEnum.DataArea)
                End If
            End If

            Return ("", FloppyDiskRegionEnum.None)
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
        Public Function IsFATArea(Disk As Disk, Sector As UInteger) As Boolean
            Dim NumberOfFATs As Byte

            If Disk.DiskParams.IsXDF Then
                NumberOfFATs = 1
            Else
                NumberOfFATs = Disk.BPB.NumberOfFATs
            End If

            Dim Length As UInteger = Disk.BPB.SectorsPerFAT * NumberOfFATs
            Dim Start As UInteger = Disk.BPB.FATRegionStart


            Return Sector >= Start And Sector < Start + Length
        End Function

        Public Structure FloppyDiskBPBParams
            Public ReadOnly BytesPerSector As UShort
            Public ReadOnly MediaDescriptor As Byte
            Public ReadOnly NumberOfFATs As Byte
            Public ReadOnly NumberOfHeads As UShort
            Public ReadOnly ReservedSectorCount As UShort
            Public ReadOnly RootEntryCount As UShort
            Public ReadOnly SectorCountSmall As UShort
            Public ReadOnly SectorsPerCluster As Byte
            Public ReadOnly SectorsPerFAT As UShort
            Public ReadOnly SectorsPerTrack As UShort


            Public Sub New(BytesPerSector As UShort, MediaDescriptor As Byte, NumberOfFATs As Byte, NumberOfHeads As UShort, ReservedSectorCount As UShort, RootEntryCount As UShort, SectorCountSmall As UShort, SectorsPerCluster As Byte, SectorsPerFAT As UShort, SectorsPerTrack As UShort)
                Me.BytesPerSector = BytesPerSector
                Me.MediaDescriptor = MediaDescriptor
                Me.NumberOfFATs = NumberOfFATs
                Me.NumberOfHeads = NumberOfHeads
                Me.ReservedSectorCount = ReservedSectorCount
                Me.RootEntryCount = RootEntryCount
                Me.SectorCountSmall = SectorCountSmall
                Me.SectorsPerCluster = SectorsPerCluster
                Me.SectorsPerFAT = SectorsPerFAT
                Me.SectorsPerTrack = SectorsPerTrack
            End Sub

            Public ReadOnly Property SizeInBytes As UInteger
                Get
                    Return CUInt(BytesPerSector) * SectorCountSmall
                End Get
            End Property

            Public ReadOnly Property TrackCount As UShort
                Get
                    If SectorsPerTrack = 0 OrElse NumberOfHeads = 0 Then
                        Return 0
                    End If

                    Return CUShort(SectorCountSmall \ (SectorsPerTrack * NumberOfHeads))
                End Get
            End Property

            Public Function CompareBPB(BPB As BiosParameterBlock, CheckMediaDescriptor As Boolean) As Boolean
                Return BPB.BytesPerSector = BytesPerSector _
                    AndAlso BPB.NumberOfFATs = NumberOfFATs _
                    AndAlso BPB.NumberOfHeads = NumberOfHeads _
                    AndAlso BPB.ReservedSectorCount = ReservedSectorCount _
                    AndAlso BPB.RootEntryCount = RootEntryCount _
                    AndAlso BPB.SectorCountSmall = SectorCountSmall _
                    AndAlso BPB.SectorsPerCluster = SectorsPerCluster _
                    AndAlso BPB.SectorsPerFAT = SectorsPerFAT _
                    AndAlso BPB.SectorsPerTrack = SectorsPerTrack _
                    AndAlso (Not CheckMediaDescriptor Or BPB.MediaDescriptor = MediaDescriptor)
            End Function

            Public Function GetBPB() As BiosParameterBlock
                Dim BPB As New BiosParameterBlock With {
                    .BytesPerSector = BytesPerSector,
                    .MediaDescriptor = MediaDescriptor,
                    .NumberOfFATs = NumberOfFATs,
                    .NumberOfHeads = NumberOfHeads,
                    .ReservedSectorCount = ReservedSectorCount,
                    .RootEntryCount = RootEntryCount,
                    .SectorCountSmall = SectorCountSmall,
                    .SectorsPerCluster = SectorsPerCluster,
                    .SectorsPerFAT = SectorsPerFAT,
                    .SectorsPerTrack = SectorsPerTrack
                }

                Return BPB
            End Function
        End Structure

        Public Structure FloppyDiskGaps
            Public ReadOnly Gap1 As UInteger
            Public ReadOnly Gap2 As UInteger
            Public ReadOnly Gap3 As UInteger
            Public ReadOnly Gap4A As UInteger

            Public Sub New(gap4A As Integer, gap1 As Integer, gap2 As Integer, gap3 As Integer)
                Me.Gap4A = gap4A
                Me.Gap1 = gap1
                Me.Gap2 = gap2
                Me.Gap3 = gap3
            End Sub
        End Structure

        Public Structure FloppyDiskParams
            Public Sub New(Format As FloppyDiskFormat, BPBParams As FloppyDiskBPBParams, Gaps As FloppyDiskGaps, FileExtension As String, DriveType As FloppyDriveType)
                Me.Format = Format
                Me.BPBParams = BPBParams
                Me.Gaps = Gaps
                Me.FileExtension = FileExtension
                Me.DriveType = DriveType
                Me.Detected = False
            End Sub

            Public ReadOnly Property BitRateKbps As UShort
                Get
                    Select Case Format
                        Case FloppyDiskFormat.FloppyTandy2000
                            Return 500
                    End Select

                    Select Case DriveType
                        Case FloppyDriveType.Drive525DoubleDensity, FloppyDriveType.Drive35DoubleDensity
                            Return 250

                        Case FloppyDriveType.Drive525HighDensity, FloppyDriveType.Drive35HighDensity
                            Return 500

                        Case FloppyDriveType.Drive35ExtraHighDensity
                            Return 1000

                        Case Else
                            Return 500
                    End Select
                End Get
            End Property

            Public ReadOnly Property BPBParams As FloppyDiskBPBParams

            Public Overrides Function ToString() As String
                Return Description
            End Function

            Public ReadOnly Property Description As String
                Get
                    Select Case Format
                        Case FloppyDiskFormat.FloppyNoBPB, FloppyDiskFormat.FloppyUnknown
                            Return "Custom Format"
                    End Select

                    Dim D As String

                    If Is525 Then
                        D = "5.25"""
                    Else
                        D = "3.5"""
                    End If

                    Dim SizeKB As Double = BPBParams.SizeInBytes / 1024
                    If SizeKB < 1000 Then
                        D &= $" {SizeKB:0}K"
                    Else
                        Dim SizeMB As Double = SizeKB / 1000
                        D &= $" {SizeMB:0.##}M"
                    End If

                    Dim Sides As String
                    Select Case BPBParams.NumberOfHeads
                        Case 1
                            Sides = "SS"
                        Case Else
                            Sides = "DS"
                    End Select

                    Dim Density As String
                    Select Case DriveType
                        Case FloppyDriveType.Drive35HighDensity, FloppyDriveType.Drive525HighDensity
                            Density = "HD"
                        Case FloppyDriveType.Drive35ExtraHighDensity
                            Density = "ED"
                        Case Else
                            Density = "DD"
                    End Select

                    Dim Special As String
                    Select Case Format
                        Case FloppyDiskFormat.FloppyDMF1024
                            Special = "DMF 1024"
                        Case FloppyDiskFormat.FloppyDMF2048
                            Special = "DMF 2048"
                        Case FloppyDiskFormat.FloppyXDF35, FloppyDiskFormat.FloppyXDF525, FloppyDiskFormat.FloppyXDFMicro
                            Special = "XDF"
                        Case FloppyDiskFormat.Floppy2HD
                            Special = "2HD"
                        Case FloppyDiskFormat.FloppyProCopy
                            Special = "ProCopy"
                        Case FloppyDiskFormat.FloppyTandy2000
                            Special = "Tandy 2000"
                        Case Else
                            Special = ""
                    End Select

                    If Special <> "" Then
                        D &= $" ({Sides}, {Density}, {Special})"
                    Else
                        D &= $" ({Sides}, {Density})"
                    End If

                    If Me.Detected Then
                        D &= " *"
                    End If

                    Return D
                End Get
            End Property

            Public Property Detected As Boolean
            Public ReadOnly Property FileExtension As String
            Public ReadOnly Property Format As FloppyDiskFormat
            Public ReadOnly Property Gaps As FloppyDiskGaps
            Public ReadOnly Property Is525 As Boolean
                Get
                    Select Case DriveType
                        Case FloppyDriveType.Drive525DoubleDensity, FloppyDriveType.Drive525HighDensity
                            Return True
                        Case Else
                            Return False
                    End Select
                End Get
            End Property

            Public ReadOnly Property IsNonImage As Boolean
                Get
                    Select Case Format
                        Case FloppyDiskFormat.FloppyUnknown, FloppyDiskFormat.FloppyNoBPB, FloppyDiskFormat.FloppyXDFMicro
                            Return True
                        Case Else
                            Return False
                    End Select
                End Get
            End Property

            Public ReadOnly Property IsStandard As Boolean
                Get
                    Return FloppyDiskFormatIsStandard(Format)
                End Get
            End Property

            Public ReadOnly Property IsXDF As Boolean
                Get
                    Return Format = FloppyDiskFormat.FloppyXDF35 Or Format = FloppyDiskFormat.FloppyXDF525
                End Get
            End Property

            Public ReadOnly Property DriveType As FloppyDriveType
            Public ReadOnly Property RPM As UShort
                Get
                    Select Case Format
                        Case FloppyDiskFormat.FloppyTandy2000, FloppyDiskFormat.Floppy2HD
                            Return 360
                    End Select

                    Select Case DriveType
                        Case FloppyDriveType.Drive525DoubleDensity
                            Return 300

                        Case FloppyDriveType.Drive525HighDensity
                            Return 360

                        Case FloppyDriveType.Drive35DoubleDensity, FloppyDriveType.Drive35HighDensity, FloppyDriveType.Drive35ExtraHighDensity
                            Return 300

                        Case Else
                            Return 300
                    End Select
                End Get
            End Property
        End Structure
    End Module
End Namespace
