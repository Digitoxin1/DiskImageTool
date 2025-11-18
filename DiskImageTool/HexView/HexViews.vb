Imports DiskImageTool.DiskImage
Imports DiskImageTool.DiskImage.BootSector
Imports DiskImageTool.DiskImage.BiosParameterBlock

Module HexViews
    Public Function DisplayHexViewForm(HexViewSectorData As HexViewSectorData) As Boolean
        Return DisplayHexViewForm(HexViewSectorData, False, False, False)
    End Function

    Public Function DisplayHexViewForm(HexViewSectorData As HexViewSectorData, SyncBlocks As Boolean) As Boolean
        Return DisplayHexViewForm(HexViewSectorData, False, False, SyncBlocks)
    End Function

    Public Function DisplayHexViewForm(HexViewSectorData As HexViewSectorData, SectorNavigator As Boolean, ClusterNavigator As Boolean, SyncBlocks As Boolean) As Boolean
        If HexViewSectorData.SectorData.BlockCount > 0 Then
            Dim frmHexView As New HexViewForm(HexViewSectorData, SectorNavigator, ClusterNavigator, SyncBlocks)
            frmHexView.ShowDialog()

            Return frmHexView.Modified
        Else
            MsgBox(My.Resources.Dialog_CheckImageSize, MsgBoxStyle.Exclamation)
            Return False
        End If
    End Function

    Public Function DisplayHexViewForm(HexViewSectorData As HexViewSectorData, SectorNavigator As Boolean, ClusterNavigator As Boolean, SyncBlocks As Boolean, Cluster As UShort) As Boolean
        If HexViewSectorData.SectorData.BlockCount > 0 Then
            Dim frmHexView As New HexViewForm(HexViewSectorData, SectorNavigator, ClusterNavigator, SyncBlocks)
            frmHexView.SetStartingCluster(Cluster)
            frmHexView.ShowDialog()

            Return frmHexView.Modified
        Else
            MsgBox(My.Resources.Dialog_CheckImageSize, MsgBoxStyle.Exclamation)
            Return False
        End If
    End Function

    Public Function HexDisplayBadSectors(Disk As Disk) As Boolean
        Dim HexViewSectorData = HexViewBadSectors(Disk)

        Return DisplayHexViewForm(HexViewSectorData)
    End Function

    Public Function HexDisplayBootSector(Disk As Disk) As Boolean
        Dim HexViewSectorData = HexViewBootSector(Disk)

        Return DisplayHexViewForm(HexViewSectorData)
    End Function

    Public Function HexDisplayDirectory(Disk As Disk, Directory As IDirectory) As Boolean
        Dim Result As Boolean

        If Directory Is Disk.RootDirectory Then
            Result = HexDisplayRootDirectory(Disk)
        Else
            Result = HexDisplayDirectoryEntry(Disk, Directory.ParentEntry)
        End If

        Return Result
    End Function

    Public Function HexDisplayDirectoryEntry(Disk As Disk, DirectoryEntry As DirectoryEntry) As Boolean
        Dim HexViewSectorData = HexViewDirectoryEntry(Disk, DirectoryEntry)

        If HexViewSectorData Is Nothing Then
            Return False
        End If

        Return DisplayHexViewForm(HexViewSectorData)
    End Function

    Public Function HexDisplayDisk(Disk As Disk) As Boolean
        Dim HexViewSectorData As New HexViewSectorData(Disk, 0, Disk.Image.Length) With {
            .Description = "Disk"
        }

        Return DisplayHexViewForm(HexViewSectorData, True, True, False)
    End Function

    Public Function HexDisplayFAT(Disk As Disk) As Boolean
        Dim HexViewSectorData = HexViewFAT(Disk)

        Dim SyncBlocks = Disk.BPB.NumberOfFATEntries > 1 AndAlso Not Disk.DiskParams.IsXDF

        Return DisplayHexViewForm(HexViewSectorData, SyncBlocks)
    End Function

    Public Function HexDisplayFreeClusters(Disk As Disk) As Boolean
        Dim HexViewSectorData As New HexViewSectorData(Disk, Disk.FAT.GetFreeClusters(FAT12.FreeClusterEmum.WithData).ToList) With {
            .Description = "Free Clusters"
        }

        Return DisplayHexViewForm(HexViewSectorData)
    End Function

    Public Function HexDisplayLostSectors(Disk As Disk) As Boolean
        Dim HexViewSectorData = HexViewLostClusters(Disk)

        Return DisplayHexViewForm(HexViewSectorData)
    End Function

    Public Function HexDisplayOverdumpData(Disk As Disk) As Boolean
        Dim Offset = Disk.BPB.ReportedImageSize() + 1

        Dim HexViewSectorData As New HexViewSectorData(Disk, Offset, Disk.Image.Length - Offset) With {
            .Description = "Disk"
        }

        Return DisplayHexViewForm(HexViewSectorData, True, True, False)
    End Function

    Public Sub HexDisplayRawTrackData(Disk As Disk, TrackIndex As Integer)
        Dim Track As UShort
        Dim Side As Byte
        Dim AllTracks As Boolean

        If TrackIndex = -1 Then
            Track = 0
            Side = 0
            AllTracks = True
        Else
            Track = TrackIndex \ Disk.Image.SideCount
            Side = TrackIndex Mod Disk.Image.SideCount
            AllTracks = False
        End If

        Dim Image = Disk.Image.BitstreamImage

        If Image IsNot Nothing Then
            Dim frmHexView As New HexViewRawForm(Disk, Track, Side, AllTracks)
            frmHexView.ShowDialog()
        End If
    End Sub

    Public Function HexDisplayRootDirectory(Disk As Disk) As Boolean
        Dim HexViewSectorData = HexViewRootDirectory(Disk)

        If HexViewSectorData Is Nothing Then
            Return False
        End If

        Return DisplayHexViewForm(HexViewSectorData)
    End Function

    Public Function HexViewBadSectors(Disk As Disk) As HexViewSectorData
        Dim HexViewSectorData As New HexViewSectorData(Disk) With {
            .Description = My.Resources.HexView_BadSectors
        }

        Dim Offset As UInteger = 0
        Dim Length As UInteger = 0
        Dim LastCluster As UShort = 0

        For Each Cluster In Disk.FAT.BadClusters
            If Cluster > LastCluster + 1 Then
                If Length > 0 Then
                    HexViewSectorData.SectorData.AddBlockByOffset(Offset, Length)
                End If
                Offset = Disk.BPB.ClusterToOffset(Cluster)
                Length = 0
            End If
            Length += Disk.BPB.BytesPerCluster
            LastCluster = Cluster
        Next

        If Length > 0 Then
            HexViewSectorData.SectorData.AddBlockByOffset(Offset, Length)
        End If

        Return HexViewSectorData
    End Function

    Public Function HexViewBootSector(Disk As Disk) As HexViewSectorData
        Dim HexViewSectorData As New HexViewSectorData(Disk) With {
            .Description = My.Resources.HexView_BootSector
        }

        Dim HighlightedRegions As New HighlightedRegions

        HexViewSectorData.SectorData.AddBlock(0, 1)
        HexViewSectorData.HighlightedRegionList.Add(HighlightedRegions)

        Dim BootStrapStart = Disk.BootSector.GetBootStrapOffset
        Dim BootStrapLength = BootSectorOffsets.BootStrapSignature - BootStrapStart

        Dim ForeColor As Color
        If Disk.BootSector.CheckJumpInstruction(False, True) Then
            ForeColor = Color.Green
        Else
            ForeColor = Color.Black
        End If

        HighlightedRegions.AddBootSectorOffset(BootSectorDescription(BootSectorOffsets.JmpBoot), Disk.BootSector.GetJmpBootOffset, BootSectorSizes.JmpBoot, ForeColor)
        If Disk.IsValidImage Then
            If Disk.BootSector.CheckJumpInstruction(False) AndAlso Disk.BootSector.BPB.IsValid Then
                If BootStrapStart < 3 Or BootStrapStart >= Disk.BootSector.GetOEMNameOffset + Disk.BootSector.GetOEMNameSize Then
                    HighlightedRegions.AddBootSectorOffset(BootSectorDescription(BootSectorOffsets.OEMName), Disk.BootSector.GetOEMNameOffset, Disk.BootSector.GetOEMNameSize, Color.Red)
                End If
                If BootStrapStart < 3 Or BootStrapStart >= BPBOoffsets.HiddenSectors + BPBSizes.HiddenSectors Then
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.BytesPerSector, Color.Blue)
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.SectorsPerCluster, Color.Blue)
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.ReservedSectorCount, Color.Blue)
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.NumberOfFATs, Color.Blue)
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.RootEntryCount, Color.Blue)
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.SectorCountSmall, Color.Blue)
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.MediaDescriptor, Color.Blue)
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.SectorsPerFAT, Color.Blue)
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.SectorsPerTrack, Color.Blue)
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.NumberOfHeads, Color.Blue)
                    HighlightedRegions.AddBPBoffset(BPBOoffsets.HiddenSectors, Color.Blue)
                End If

                If Disk.BootSector.HasValidExtendedBootSignature And BootStrapStart >= BootSectorOffsets.FileSystemType + BootSectorSizes.FileSystemType Then
                    HighlightedRegions.AddBootSectorOffset(BootSectorOffsets.DriveNumber, Color.Purple)
                    HighlightedRegions.AddBootSectorOffset(BootSectorOffsets.Reserved, Color.Purple)
                    HighlightedRegions.AddBootSectorOffset(BootSectorOffsets.ExtendedBootSignature, Color.Purple)
                    HighlightedRegions.AddBootSectorOffset(BootSectorOffsets.VolumeSerialNumber, Color.Purple)
                    HighlightedRegions.AddBootSectorOffset(BootSectorOffsets.VolumeLabel, Color.Purple)
                    HighlightedRegions.AddBootSectorOffset(BootSectorOffsets.FileSystemType, Color.Purple)
                End If
            End If
        End If

        If BootStrapStart > 2 And BootStrapLength > 1 Then
            If Disk.BootSector.CheckJumpInstruction(False, True) Then
                HighlightedRegions.AddItem(BootStrapStart, BootStrapLength, ForeColor, "Boot Strap Code")
            Else
                HighlightedRegions.AddItem(BootStrapStart, BootStrapLength, ForeColor)
            End If
        End If

        If Disk.BootSector.HasValidBootStrapSignature Then
            ForeColor = Color.Goldenrod
        Else
            ForeColor = Color.Black
        End If

        HighlightedRegions.AddBootSectorOffset(BootSectorOffsets.BootStrapSignature, ForeColor)

        Return HexViewSectorData
    End Function

    Public Function HexViewDirectoryEntry(Disk As Disk, DirectoryEntry As DirectoryEntry) As HexViewSectorData
        Dim HexViewSectorData As HexViewSectorData
        Dim Caption As String

        If Not (DirectoryEntry.IsValidFile OrElse DirectoryEntry.IsValidDirectory) Then
            Return Nothing
        End If

        If DirectoryEntry.IsDirectory Then
            If DirectoryEntry.IsDeleted Then
                Caption = My.Resources.HexView_DeletedDirectory
            Else
                Caption = My.Resources.HexView_Directory
            End If
        Else
            If DirectoryEntry.IsDeleted Then
                Caption = My.Resources.HexView_DeletedFile
            Else
                Caption = My.Resources.HexView_File
            End If
        End If

        Caption &= " - " & DirectoryEntry.GetShortFileName(True)

        If DirectoryEntry.IsDeleted Then
            Dim DataOffset = Disk.BPB.ClusterToOffset(DirectoryEntry.StartingCluster)
            Dim Length As UInteger
            Dim FileSize As UInteger
            If DirectoryEntry.IsDirectory Then
                Length = Disk.BPB.BytesPerCluster
                FileSize = Length
            Else
                Length = Math.Ceiling(DirectoryEntry.FileSize / Disk.BPB.BytesPerCluster) * Disk.BPB.BytesPerCluster
                FileSize = DirectoryEntry.FileSize
            End If

            HexViewSectorData = New HexViewSectorData(Disk, DataOffset, Length)
            HighlightSectorData(Disk, HexViewSectorData, FileSize, True)
        Else
            HexViewSectorData = New HexViewSectorData(Disk, DirectoryEntry.FATChain.Clusters)
            If Not DirectoryEntry.IsDirectory Then
                HighlightSectorData(Disk, HexViewSectorData, DirectoryEntry.FileSize, False)
            Else
                HighlightDirectoryData(Disk, HexViewSectorData, False)
            End If
        End If

        HexViewSectorData.Description = Caption

        Return HexViewSectorData
    End Function

    Public Function HexViewFAT(Disk As Disk) As HexViewSectorData
        Dim HexViewSectorData As New HexViewSectorData(Disk) With {
            .Description = My.Resources.HexView_FAT
        }

        Dim HighlightedRegions As New HighlightedRegions
        Dim OriginalData() As Byte = Nothing

        Dim NumberOfFATs As Byte
        If Disk.DiskParams.IsXDF Then
            NumberOfFATs = 1
        Else
            NumberOfFATs = Disk.BPB.NumberOfFATs
        End If

        For Index As Byte = 0 To NumberOfFATs - 1
            Dim Length As UInteger = Disk.BPB.SectorToBytes(Disk.BPB.SectorsPerFAT)
            Dim Start As UInteger = Disk.BPB.SectorToBytes(Disk.BPB.FATRegionStart) + Length * Index
            Dim Data = Disk.Image.GetBytes(Start, Length)

            HexViewSectorData.SectorData.AddBlockByOffset(Start, Length, "FAT " & Index + 1)
            HexViewSectorData.HighlightedRegionList.Add(HighlightedRegions)

            If Index = 0 Then
                OriginalData = Data
            Else
                Dim HighlightStart As Integer = -2
                Dim HighlightLength As Integer = 0
                For Counter = 0 To Data.Length - 1
                    If Data(Counter) <> OriginalData(Counter) Then
                        If Counter > HighlightStart + 1 Then
                            If HighlightLength > 0 Then
                                HighlightedRegions.AddItem(HighlightStart, HighlightLength, Color.Red)
                            End If
                            HighlightStart = Counter
                            HighlightLength = 1
                        Else
                            HighlightLength += 1
                        End If
                    End If
                Next
                If HighlightLength > 0 Then
                    HighlightedRegions.AddItem(HighlightStart, HighlightLength, Color.Red)
                End If
            End If
        Next

        Return HexViewSectorData
    End Function

    Public Function HexViewLostClusters(Disk As Disk) As HexViewSectorData
        Dim HexViewSectorData As New HexViewSectorData(Disk) With {
            .Description = My.Resources.HexView_LostClusters
        }

        Dim Offset As UInteger = 0
        Dim Length As UInteger = 0
        Dim LastCluster As UShort = 0

        For Each Cluster In Disk.RootDirectory.FATAllocation.LostClusters
            If Cluster > LastCluster + 1 Then
                If Length > 0 Then
                    HexViewSectorData.SectorData.AddBlockByOffset(Offset, Length)
                End If
                Offset = Disk.BPB.ClusterToOffset(Cluster)
                Length = 0
            End If
            Length += Disk.BPB.BytesPerCluster
            LastCluster = Cluster
        Next

        If Length > 0 Then
            HexViewSectorData.SectorData.AddBlockByOffset(Offset, Length)
        End If

        Return HexViewSectorData
    End Function
    Public Function HexViewRootDirectory(Disk As Disk) As HexViewSectorData
        Dim HexViewSectorData As New HexViewSectorData(Disk, Disk.RootDirectory.SectorChain)
        HighlightDirectoryData(Disk, HexViewSectorData, True)

        HexViewSectorData.Description = My.Resources.HexView_RootDirectory

        Return HexViewSectorData
    End Function
    Private Sub HighlightDirectoryData(Disk As Disk, HexViewSectorData As HexViewSectorData, CheckBootSector As Boolean)
        Dim EndOfDirectory As Boolean = False

        For Index As UInteger = 0 To HexViewSectorData.SectorData.BlockCount - 1
            Dim HasBootSector As Boolean = False
            Dim BootSectorOffset As UInteger = 0
            Dim SectorBlock = HexViewSectorData.SectorData.GetBlock(Index)
            Dim HighlightedRegions As New HighlightedRegions
            HexViewSectorData.HighlightedRegionList.Add(HighlightedRegions)
            Dim OffsetStart As UInteger = Disk.BPB.SectorToBytes(SectorBlock.SectorStart)
            Dim OffsetEnd As UInteger = Disk.BPB.SectorToBytes(SectorBlock.SectorStart + SectorBlock.SectorCount) - 1
            For Offset As UInteger = OffsetStart To OffsetEnd Step DirectoryEntry.DIRECTORY_ENTRY_SIZE
                Dim FirstByte = Disk.Image.GetByte(Offset)
                If FirstByte = 0 Then
                    EndOfDirectory = True
                End If
                If Not HasBootSector And CheckBootSector Then
                    If BootSector.ValidJumpInstructuon.Contains(FirstByte) Then
                        If OffsetEnd - Offset + 1 >= BootSector.BOOT_SECTOR_SIZE Then
                            Dim BootSectorData = Disk.Image.GetBytes(Offset, DiskImage.BootSector.BOOT_SECTOR_SIZE)
                            Dim BootSector As New BootSector(BootSectorData)
                            If BootSector.BPB.IsValid Then
                                HasBootSector = True
                                BootSectorOffset = Offset
                                EndOfDirectory = True
                                HighlightedRegions.AddItem(Offset - OffsetStart, DiskImage.BootSector.BOOT_SECTOR_SIZE, Color.Purple)
                            End If
                        End If
                    End If
                End If
                If EndOfDirectory Then
                    If Not HasBootSector Or Offset < BootSectorOffset Or Offset > BootSectorOffset + DiskImage.BootSector.BOOT_SECTOR_SIZE Then
                        If DirectoryEntryHasData(Disk.Image, Offset) Then
                            HighlightedRegions.AddItem(Offset - OffsetStart, DirectoryEntry.DIRECTORY_ENTRY_SIZE, Color.Red)
                        End If
                    End If
                Else
                    Dim Attributes As Byte = Disk.Image.GetByte(Offset + DirectoryEntry.DirectoryEntryOffsets.Attributes)
                    If (Attributes And DirectoryEntry.AttributeFlags.LongFileName) = DirectoryEntry.AttributeFlags.LongFileName Then
                        HighlightedRegions.AddDirectoryEntryLFNOffset(Offset - OffsetStart, DirectoryEntry.LFNOffsets.Sequence, Color.Purple)
                        HighlightedRegions.AddDirectoryEntryLFNOffset(Offset - OffsetStart, DirectoryEntry.LFNOffsets.FilePart1, Color.Purple)
                        HighlightedRegions.AddDirectoryEntryLFNOffset(Offset - OffsetStart, DirectoryEntry.LFNOffsets.Attributes, Color.Purple)
                        HighlightedRegions.AddDirectoryEntryLFNOffset(Offset - OffsetStart, DirectoryEntry.LFNOffsets.Type, Color.Purple)
                        HighlightedRegions.AddDirectoryEntryLFNOffset(Offset - OffsetStart, DirectoryEntry.LFNOffsets.Checksum, Color.Purple)
                        HighlightedRegions.AddDirectoryEntryLFNOffset(Offset - OffsetStart, DirectoryEntry.LFNOffsets.FilePart2, Color.Purple)
                        HighlightedRegions.AddDirectoryEntryLFNOffset(Offset - OffsetStart, DirectoryEntry.LFNOffsets.StartingCluster, Color.Purple)
                        HighlightedRegions.AddDirectoryEntryLFNOffset(Offset - OffsetStart, DirectoryEntry.LFNOffsets.FilePart3, Color.Purple)
                    Else
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.FileName, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.Extension, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.Attributes, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.ReservedForWinNT, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.CreationMillisecond, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.CreationTime, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.CreationDate, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.LastAccessDate, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.ReservedForFAT32, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.LastWriteTime, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.LastWriteDate, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.StartingCluster, Color.Blue)
                        HighlightedRegions.AddDirectoryEntryOffset(Offset - OffsetStart, DirectoryEntry.DirectoryEntryOffsets.FileSize, Color.Blue)
                    End If
                    'HighlightedRegions.AddItem(Offset - OffsetStart, DirectoryEntry.DIRECTORY_ENTRY_SIZE, Color.Blue)
                End If
            Next
        Next
    End Sub

    Private Sub HighlightSectorData(Disk As Disk, HexViewSectorData As HexViewSectorData, FileSize As UInteger, HighlightAssigned As Boolean)
        For Index As UInteger = 0 To HexViewSectorData.SectorData.BlockCount - 1
            Dim SectorBlock = HexViewSectorData.SectorData.GetBlock(Index)
            Dim HighlightedRegions As New HighlightedRegions
            HexViewSectorData.HighlightedRegionList.Add(HighlightedRegions)

            If HighlightAssigned Then
                Dim BytesRemaining = FileSize
                For J = 0 To SectorBlock.SectorCount - 1
                    Dim Sector = SectorBlock.SectorStart + J
                    Dim BlockOffset = J * Disk.BPB.BytesPerSector
                    Dim BlockSize = Math.Min(BytesRemaining, Disk.BPB.BytesPerSector)
                    If BlockSize > 0 Then
                        Dim Cluster = Disk.BPB.SectorToCluster(Sector)
                        If Disk.RootDirectory.FATAllocation.FileAllocation.ContainsKey(Cluster) Then
                            HighlightedRegions.AddItem(BlockOffset, BlockSize, Color.Red)
                        End If
                    End If
                    BytesRemaining -= BlockSize
                Next
            End If

            If SectorBlock.Size > FileSize Then
                Dim Start = FileSize
                Dim Size = SectorBlock.Size - FileSize
                If Size > 0 Then
                    HighlightedRegions.AddItem(Start, Size, Color.DarkGray)
                End If
            End If

            FileSize -= Math.Min(FileSize, SectorBlock.Size)
        Next
    End Sub
End Module
