Imports DiskImageTool.DiskImage
Imports DiskImageTool.DiskImage.BootSector

Module HexViews
    Public Function DisplayHexViewForm(HexViewSectorData As HexViewSectorData) As Boolean
        Return DisplayHexViewForm(HexViewSectorData, False, False, False)
    End Function

    Public Function DisplayHexViewForm(HexViewSectorData As HexViewSectorData, SyncBlocks As Boolean) As Boolean
        Return DisplayHexViewForm(HexViewSectorData, False, False, SyncBlocks)
    End Function

    Public Function DisplayHexViewForm(HexViewSectorData As HexViewSectorData, SectorNavigator As Boolean, ClusterNavigator As Boolean, SyncBlocks As Boolean) As Boolean
        Dim frmHexView As New HexViewForm(HexViewSectorData, SectorNavigator, ClusterNavigator, SyncBlocks)
        frmHexView.ShowDialog()

        Return frmHexView.Modified
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
                Offset = Disk.BootSector.ClusterToOffset(Cluster)
                Length = 0
            End If
            Length += Disk.BootSector.BytesPerCluster
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
        If Disk.BootSector.HasValidJumpInstruction(False) Then
            ForeColor = Color.Green
        Else
            ForeColor = Color.Black
        End If

        HighlightedRegions.AddOffset(BootSectorOffsets.JmpBoot, ForeColor)
        If Disk.IsValidImage Then
            HighlightedRegions.AddOffset(BootSectorOffsets.OEMName, Color.Red)
            HighlightedRegions.AddOffset(BootSectorOffsets.BytesPerSector, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.SectorsPerCluster, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.ReservedSectorCount, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.NumberOfFATs, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.RootEntryCount, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.SectorCountSmall, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.MediaDescriptor, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.SectorsPerFAT, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.SectorsPerTrack, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.NumberOfHeads, Color.Blue)
            HighlightedRegions.AddOffset(BootSectorOffsets.HiddenSectors, Color.Blue)

            If Disk.BootSector.HasValidExtendedBootSignature And BootStrapStart >= BootSectorOffsets.FileSystemType + BootSectorSizes.FileSystemType Then
                HighlightedRegions.AddOffset(BootSectorOffsets.DriveNumber, Color.Purple)
                HighlightedRegions.AddOffset(BootSectorOffsets.Reserved, Color.Purple)
                HighlightedRegions.AddOffset(BootSectorOffsets.ExtendedBootSignature, Color.Purple)
                HighlightedRegions.AddOffset(BootSectorOffsets.VolumeSerialNumber, Color.Purple)
                HighlightedRegions.AddOffset(BootSectorOffsets.VolumeLabel, Color.Purple)
                HighlightedRegions.AddOffset(BootSectorOffsets.FileSystemType, Color.Purple)
            End If
        End If

        If BootStrapStart > 2 And BootStrapLength > 1 Then
            If Disk.BootSector.HasValidJumpInstruction(False) Then
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

        HighlightedRegions.AddOffset(BootSectorOffsets.BootStrapSignature, ForeColor)

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
            If Not DirectoryEntry.IsValid Then
                Return Nothing
            End If

            Caption = IIf(DirectoryEntry.IsDirectory, "Directory", "File") & " - " & DirectoryEntry.GetFullFileName

            If DirectoryEntry.IsDeleted Then
                Caption = "Deleted " & Caption
                Dim DataOffset = Disk.BootSector.ClusterToOffset(DirectoryEntry.StartingCluster)
                Dim Length As UInteger
                Dim FileSize As UInteger
                If DirectoryEntry.IsDirectory Then
                    Length = Disk.BootSector.BytesPerCluster
                    FileSize = Length
                Else
                    Length = Math.Ceiling(DirectoryEntry.FileSize / Disk.BootSector.BytesPerCluster) * Disk.BootSector.BytesPerCluster
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
        For Index = 0 To Disk.BootSector.NumberOfFATs - 1
            Dim Length As UInteger = Disk.SectorToBytes(Disk.BootSector.SectorsPerFAT)
            Dim Start As UInteger = Disk.SectorToBytes(Disk.BootSector.FATRegionStart) + Length * Index
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
                    HighlightedRegions.AddItem(Offset - OffsetStart, DirectoryEntry.DIRECTORY_ENTRY_SIZE, Color.Blue)
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
                        Dim Cluster = Disk.BootSector.SectorToCluster(Sector)
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
