Imports DiskImageTool.DiskImage
Imports DiskImageTool.DiskImage.BootSector
Imports DiskImageTool.DiskImage.BiosParameterBlock
Imports DiskImageTool.DiskImage.DirectoryEntry

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
            MsgBox("No Data Available at this location. Check the image size.", MsgBoxStyle.Exclamation)
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
            MsgBox("No Data Available at this location. Check the image size.", MsgBoxStyle.Exclamation)
            Return False
        End If
    End Function

    Public Function HexViewBadSectors(Disk As Disk) As HexViewSectorData
        Dim HexViewSectorData As New HexViewSectorData(Disk) With {
            .Description = "Bad Sectors"
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

    Public Function HexViewLostClusters(Disk As Disk) As HexViewSectorData
        Dim HexViewSectorData As New HexViewSectorData(Disk) With {
            .Description = "Lost Clusters"
        }

        Dim Offset As UInteger = 0
        Dim Length As UInteger = 0
        Dim LastCluster As UShort = 0

        For Each Cluster In Disk.FAT.LostClusters
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
            .Description = "Boot Sector"
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

        HighlightedRegions.AddBootSectorOffset(BootSectorOffsets.JmpBoot, ForeColor)
        If Disk.IsValidImage Then
            If Disk.BootSector.CheckJumpInstruction(False) AndAlso Disk.BootSector.BPB.IsValid Then
                If BootStrapStart < 3 Or BootStrapStart >= BootSectorOffsets.OEMName + BootSectorSizes.OEMName Then
                    HighlightedRegions.AddBootSectorOffset(BootSectorOffsets.OEMName, Color.Red)
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

    Public Function HexViewDirectoryEntry(Disk As Disk, Offset As UInteger) As HexViewSectorData
        Dim HexViewSectorData As HexViewSectorData
        Dim Caption As String

        If Offset = 0 Then
            Caption = "Root Directory"
            HexViewSectorData = New HexViewSectorData(Disk, Disk.Directory.SectorChain)
            HighlightDirectoryData(Disk, HexViewSectorData, True)
        Else
            Dim DirectoryEntry = Disk.GetDirectoryEntryByOffset(Offset)
            If Not (DirectoryEntry.IsValidFile OrElse DirectoryEntry.IsValidDirectory) Then
                Return Nothing
            End If

            Caption = IIf(DirectoryEntry.IsDirectory, "Directory", "File") & " - " & DirectoryEntry.GetFullFileName

            If DirectoryEntry.IsDeleted Then
                Caption = "Deleted " & Caption
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
        End If

        HexViewSectorData.Description = Caption

        Return HexViewSectorData
    End Function

    Public Function HexViewFAT(Disk As Disk) As HexViewSectorData
        Dim HexViewSectorData = New HexViewSectorData(Disk) With {
            .Description = "File Allocation Table"
        }

        Dim HighlightedRegions As New HighlightedRegions
        Dim OriginalData() As Byte = Nothing

        Dim NumberOfFATs As Byte
        If Disk.DiskType = FloppyDiskType.FloppyXDF Then
            NumberOfFATs = 1
        Else
            NumberOfFATs = Disk.BPB.NumberOfFATs
        End If

        For Index = 0 To NumberOfFATs - 1
            Dim Length As UInteger = Disk.SectorToBytes(Disk.BPB.SectorsPerFAT)
            Dim Start As UInteger = Disk.SectorToBytes(Disk.BPB.FATRegionStart) + Length * Index
            Dim Data = Disk.Data.GetBytes(Start, Length)

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

    Private Sub HighlightDirectoryData(Disk As Disk, HexViewSectorData As HexViewSectorData, CheckBootSector As Boolean)
        Dim EndOfDirectory As Boolean = False

        For Index = 0 To HexViewSectorData.SectorData.BlockCount - 1
            Dim HasBootSector As Boolean = False
            Dim BootSectorOffset As UInteger = 0
            Dim SectorBlock = HexViewSectorData.SectorData.GetBlock(Index)
            Dim HighlightedRegions As New HighlightedRegions
            HexViewSectorData.HighlightedRegionList.Add(HighlightedRegions)
            Dim OffsetStart As UInteger = Disk.SectorToBytes(SectorBlock.SectorStart)
            Dim OffsetEnd As UInteger = Disk.SectorToBytes(SectorBlock.SectorStart + SectorBlock.SectorCount) - 1
            For Offset As UInteger = OffsetStart To OffsetEnd Step DirectoryEntry.DIRECTORY_ENTRY_SIZE
                Dim FirstByte = Disk.Data.GetByte(Offset)
                If FirstByte = 0 Then
                    EndOfDirectory = True
                End If
                If Not HasBootSector And CheckBootSector Then
                    If BootSector.ValidJumpInstructuon.Contains(FirstByte) Then
                        If OffsetEnd - Offset + 1 >= BootSector.BOOT_SECTOR_SIZE Then
                            Dim BootSectorData = Disk.Data.GetBytes(Offset, DiskImage.BootSector.BOOT_SECTOR_SIZE)
                            Dim BootSector = New BootSector(New ImageByteArray(BootSectorData))
                            If BootSector.IsValidImage Then
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
                        If DirectoryEntryHasData(Disk.Data, Offset) Then
                            HighlightedRegions.AddItem(Offset - OffsetStart, DirectoryEntry.DIRECTORY_ENTRY_SIZE, Color.Red)
                        End If
                    End If
                Else
                    Dim Attributes As Byte = Disk.Data.GetByte(Offset + DirectoryEntry.DirectoryEntryOffsets.Attributes)
                    If (Attributes And AttributeFlags.LongFileName) = AttributeFlags.LongFileName Then
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
        For Index = 0 To HexViewSectorData.SectorData.BlockCount - 1
            Dim SectorBlock = HexViewSectorData.SectorData.GetBlock(Index)
            Dim HighlightedRegions As New HighlightedRegions
            HexViewSectorData.HighlightedRegionList.Add(HighlightedRegions)

            If HighlightAssigned Then
                Dim BytesRemaining = FileSize
                For J = 0 To SectorBlock.SectorCount - 1
                    Dim Sector = SectorBlock.SectorStart + J
                    Dim BlockOffset = J * Disk.BYTES_PER_SECTOR
                    Dim BlockSize = Math.Min(BytesRemaining, Disk.BYTES_PER_SECTOR)
                    If BlockSize > 0 Then
                        Dim Cluster = Disk.BPB.SectorToCluster(Sector)
                        If Disk.FAT.FileAllocation.ContainsKey(Cluster) Then
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
