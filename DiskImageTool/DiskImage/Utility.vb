Imports System.Reflection

Namespace DiskImage
    Module Functions
        Public ReadOnly InvalidFileChars() As Byte = {&H22, &H2A, &H2B, &H2C, &H2E, &H2F, &H3A, &H3B, &H3C, &H3D, &H3E, &H3F, &H5B, &H5C, &H5D, &H7C}
        Public ReadOnly EmptyDirectoryEntry() As Byte = {&HE5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}

        Public Function BootSectorDescription(Offset As BootSector.BootSectorOffsets) As String
            Select Case Offset
                Case BootSector.BootSectorOffsets.JmpBoot
                    Return "Bootstrap Jump"
                Case BootSector.BootSectorOffsets.OEMName
                    Return "OEM Name"
                Case BootSector.BootSectorOffsets.DriveNumber
                    Return "Drive Number"
                Case BootSector.BootSectorOffsets.Reserved
                    Return "Reserved"
                Case BootSector.BootSectorOffsets.ExtendedBootSignature
                    Return "Extended Boot Signature"
                Case BootSector.BootSectorOffsets.VolumeSerialNumber
                    Return "Volume Serial Number"
                Case BootSector.BootSectorOffsets.VolumeLabel
                    Return "Volume Label"
                Case BootSector.BootSectorOffsets.FileSystemType
                    Return "File System ID"
                Case BootSector.BootSectorOffsets.BootStrapSignature
                    Return "Boot Sector Signature"
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function BPBDescription(Offset As BiosParameterBlock.BPBOoffsets) As String
            Select Case Offset
                Case BiosParameterBlock.BPBOoffsets.BytesPerSector
                    Return "Bytes per Sector"
                Case BiosParameterBlock.BPBOoffsets.SectorsPerCluster
                    Return "Sectors per Cluster"
                Case BiosParameterBlock.BPBOoffsets.ReservedSectorCount
                    Return "Reserved Sectors"
                Case BiosParameterBlock.BPBOoffsets.NumberOfFATs
                    Return "Number of FATs"
                Case BiosParameterBlock.BPBOoffsets.RootEntryCount
                    Return "Root Directory Entries"
                Case BiosParameterBlock.BPBOoffsets.SectorCountSmall
                    Return "Total Sector Count"
                Case BiosParameterBlock.BPBOoffsets.MediaDescriptor
                    Return "Media Descriptor"
                Case BiosParameterBlock.BPBOoffsets.SectorsPerFAT
                    Return "Sectors per FAT"
                Case BiosParameterBlock.BPBOoffsets.SectorsPerTrack
                    Return "Sectors per Track"
                Case BiosParameterBlock.BPBOoffsets.NumberOfHeads
                    Return "Number of Heads"
                Case BiosParameterBlock.BPBOoffsets.HiddenSectors
                    Return "Hidden Sectors"
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function CleanDOSFileName(FileName As String) As String
            Dim FilePart As String = IO.Path.GetFileNameWithoutExtension(FileName).ToUpper
            Dim Extension As String = IO.Path.GetExtension(FileName).ToUpper

            If FilePart.Length > 8 Then
                FilePart = FilePart.Substring(0, 8)
            End If

            If Extension.Length > 4 Then
                Extension = Extension.Substring(0, 4)
            End If

            FilePart = FilePart.Replace(" ", "_")

            Return FilePart & Extension
        End Function

        Public Function DirectoryExpand(Disk As Disk, Directory As IDirectory, FreeClusters As SortedSet(Of UShort)) As Boolean
            If Directory.IsRootDirectory Then
                Return False
            End If

            Dim Cluster = Disk.FAT.GetNextFreeCluster(FreeClusters)
            If Cluster = 0 Then
                Return False
            End If

            Dim ClusterSize = Disk.BPB.BytesPerCluster

            FreeClusters.Remove(Cluster)

            Dim LastCluster = Directory.ClusterChain.Last
            Disk.FATTables.UpdateTableEntry(LastCluster, Cluster)
            Disk.FATTables.UpdateTableEntry(Cluster, FAT12.FAT_LAST_CLUSTER_END)

            Directory.ClusterChain.Add(Cluster)
            Directory.Data.MaxEntries += Disk.BPB.BytesPerCluster \ DirectoryEntry.DIRECTORY_ENTRY_SIZE

            Dim ClusterOffset = Disk.BPB.ClusterToOffset(Cluster)
            Dim Buffer = New Byte(ClusterSize - 1) {}
            Disk.Image.SetBytes(Buffer, ClusterOffset)

            Return True
        End Function

        Public Sub DirectoryEntryAddFile(Disk As Disk, Entry As DirectoryEntry, FileName As String, FreeClusters As SortedSet(Of UShort), Optional StartingCluster As UShort = 2)
            Dim FileInfo = New IO.FileInfo(FileName)
            Dim ClusterSize = Disk.BPB.BytesPerCluster

            Dim FirstCluster As UShort = 0

            If FileInfo.Length > 0 Then
                'Load file into buffer, padding with empty space if needed            
                Dim FileSize = Math.Ceiling(FileInfo.Length / ClusterSize) * ClusterSize
                Dim FileBuffer = ReadFileIntoBuffer(FileInfo, FileSize, 0)

                Dim LastCluster As UShort = 0

                For Counter As Integer = 0 To FileBuffer.Length - 1 Step ClusterSize
                    Dim Cluster = Disk.FAT.GetNextFreeCluster(FreeClusters, StartingCluster)
                    If Cluster > 0 Then
                        FreeClusters.Remove(Cluster)
                        If Counter = 0 Then
                            FirstCluster = Cluster
                        Else
                            Disk.FATTables.UpdateTableEntry(LastCluster, Cluster)
                        End If
                        Dim ClusterOffset = Disk.BPB.ClusterToOffset(Cluster)
                        Dim Buffer = Disk.Image.GetBytes(ClusterOffset, ClusterSize)
                        Array.Copy(FileBuffer, Counter, Buffer, 0, ClusterSize)
                        Disk.Image.SetBytes(Buffer, ClusterOffset)
                        LastCluster = Cluster
                        StartingCluster = Cluster + 1
                    End If
                Next

                If LastCluster > 0 Then
                    Disk.FATTables.UpdateTableEntry(LastCluster, FAT12.FAT_LAST_CLUSTER_END)
                End If
            End If

            Dim NewEntry = New DirectoryEntryBase
            NewEntry.SetFileInfo(FileInfo, False, False)
            NewEntry.StartingCluster = FirstCluster

            Entry.Data = NewEntry.Data
            Entry.InitFatChain()
        End Sub

        Public Function DirectorytEntryDescription(Offset As DirectoryEntry.DirectoryEntryOffsets) As String
            Select Case Offset
                Case DirectoryEntry.DirectoryEntryOffsets.FileName
                    Return "Name"
                Case DirectoryEntry.DirectoryEntryOffsets.Extension
                    Return "Extension"
                Case DirectoryEntry.DirectoryEntryOffsets.Attributes
                    Return "Attributes"
                Case DirectoryEntry.DirectoryEntryOffsets.ReservedForWinNT
                    Return "Reserved For Windows NT"
                Case DirectoryEntry.DirectoryEntryOffsets.CreationMillisecond
                    Return "Creation Time Tenths"
                Case DirectoryEntry.DirectoryEntryOffsets.CreationTime
                    Return "Creation Time"
                Case DirectoryEntry.DirectoryEntryOffsets.CreationDate
                    Return "Creation Date"
                Case DirectoryEntry.DirectoryEntryOffsets.LastAccessDate
                    Return "Last Access Date"
                Case DirectoryEntry.DirectoryEntryOffsets.ReservedForFAT32
                    Return "Reserved for FAT 32"
                Case DirectoryEntry.DirectoryEntryOffsets.LastWriteTime
                    Return "Last Write Time"
                Case DirectoryEntry.DirectoryEntryOffsets.LastWriteDate
                    Return "Last Write Date"
                Case DirectoryEntry.DirectoryEntryOffsets.StartingCluster
                    Return "Starting Cluster"
                Case DirectoryEntry.DirectoryEntryOffsets.FileSize
                    Return "Size"
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function DirectorytEntryLFNDescription(Offset As DirectoryEntry.LFNOffsets) As String
            Select Case Offset
                Case DirectoryEntry.LFNOffsets.Sequence
                    Return "LFN Sequence"
                Case DirectoryEntry.LFNOffsets.FilePart1
                    Return "LFN Name 1"
                Case DirectoryEntry.LFNOffsets.Attributes
                    Return "LFN Attributes"
                Case DirectoryEntry.LFNOffsets.Type
                    Return "LFN Type"
                Case DirectoryEntry.LFNOffsets.Checksum
                    Return "LFN Checksum"
                Case DirectoryEntry.LFNOffsets.FilePart2
                    Return "LFN Name 2"
                Case DirectoryEntry.LFNOffsets.StartingCluster
                    Return "LFN Starting Cluster"
                Case DirectoryEntry.LFNOffsets.FilePart3
                    Return "LFN Name 3"
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function ClusterListToSectorList(BPB As BiosParameterBlock, ClusterList As List(Of UShort)) As List(Of UInteger)
            Dim SectorList As New List(Of UInteger)

            For Each Cluster In ClusterList
                Dim Sector = BPB.ClusterToSector(Cluster)
                For Index = 0 To BPB.SectorsPerCluster - 1
                    SectorList.Add(Sector + Index)
                Next
            Next

            Return SectorList
        End Function

        Public Function DateToFATDate(D As Date) As UShort
            Dim FATDate As UShort = D.Year - 1980

            FATDate <<= 4
            FATDate += D.Month
            FATDate <<= 5
            FATDate += D.Day

            Return FATDate
        End Function

        Public Function DateToFATMilliseconds(D As Date) As Byte
            Return D.Millisecond \ 10 + (D.Second Mod 2) * 100
        End Function

        Public Function DateToFATTime(D As Date) As UShort
            Dim DTTime As UShort = D.Hour

            DTTime <<= 6
            DTTime += D.Minute
            DTTime <<= 5
            DTTime += D.Second \ 2

            Return DTTime
        End Function

        Public Function GetBadSectors(BPB As BiosParameterBlock, BadClusters As List(Of UShort)) As HashSet(Of UInteger)
            Dim BadSectors As New HashSet(Of UInteger)

            For Each Cluster In BadClusters
                Dim Sector = BPB.ClusterToSector(Cluster)
                For Index = 0 To BPB.SectorsPerCluster - 1
                    BadSectors.Add(Sector + Index)
                Next
            Next

            Return BadSectors
        End Function

        Public Function GetDataFromChain(FileBytes As IByteArray, SectorChain As List(Of UInteger)) As Byte()
            Dim SectorSize As UInteger = Disk.BYTES_PER_SECTOR
            Dim Content(SectorChain.Count * SectorSize - 1) As Byte
            Dim ContentOffset As UInteger = 0

            For Each Sector In SectorChain
                Dim Offset As UInteger = Disk.SectorToBytes(Sector)
                If FileBytes.Length < Offset + SectorSize Then
                    SectorSize = Math.Max(FileBytes.Length - Offset, 0)
                End If
                If SectorSize > 0 Then
                    FileBytes.CopyTo(Offset, Content, ContentOffset, SectorSize)
                    ContentOffset += SectorSize
                Else
                    Exit For
                End If
            Next

            Return Content
        End Function

        Public Function DirectoryEntryHasData(FileBytes As IByteArray, Offset As UInteger) As Boolean
            Dim Result As Boolean = False

            If FileBytes.GetByte(Offset) = &HE5 Then
                For Offset2 As UInteger = Offset + 1 To Offset + DirectoryEntry.DIRECTORY_ENTRY_SIZE - 1
                    If FileBytes.GetByte(Offset2) <> 0 Then
                        Result = True
                        Exit For
                    End If
                Next
            ElseIf FileBytes.GetByte(Offset) <> 0 Then
                Result = True
            Else
                Dim HexF6Count As UInteger = 0
                For Offset2 As UInteger = Offset + 1 To Offset + DirectoryEntry.DIRECTORY_ENTRY_SIZE - 1
                    If FileBytes.GetByte(Offset2) = &HF6 Then
                        HexF6Count += 1
                    ElseIf FileBytes.GetByte(Offset2) <> 0 Then
                        Result = True
                        Exit For
                    End If
                Next
                If Not Result Then
                    If HexF6Count > 0 And HexF6Count < DirectoryEntry.DIRECTORY_ENTRY_SIZE - 2 Then
                        Result = True
                    End If
                End If
            End If

            Return Result
        End Function

        Public Sub GetDirectoryData(Data As DirectoryData, FileBytes As IByteArray, OffsetStart As UInteger, OffsetEnd As UInteger, CheckBootSector As Boolean)
            Dim EntryCount = (OffsetEnd - OffsetStart) \ DirectoryEntry.DIRECTORY_ENTRY_SIZE

            Data.MaxEntries += EntryCount

            If EntryCount > 0 Then
                For Entry As UInteger = 0 To EntryCount - 1
                    Dim Offset = OffsetStart + (Entry * DirectoryEntry.DIRECTORY_ENTRY_SIZE)
                    Dim FirstByte = FileBytes.GetByte(Offset)
                    If FirstByte = 0 Then
                        Data.EndOfDirectory = True
                    End If
                    If Not Data.HasBootSector And CheckBootSector Then
                        If BootSector.ValidJumpInstructuon.Contains(FirstByte) Then
                            If OffsetEnd - Offset >= BootSector.BOOT_SECTOR_SIZE Then
                                Dim BootSectorData = FileBytes.GetBytes(Offset, DiskImage.BootSector.BOOT_SECTOR_SIZE)
                                Dim BootSector = New BootSector(BootSectorData)
                                If BootSector.BPB.IsValid Then
                                    Data.HasBootSector = True
                                    Data.BootSectorOffset = Offset
                                    Data.EndOfDirectory = True
                                End If
                            End If
                        End If
                    End If
                    If Data.EndOfDirectory Then
                        If Not Data.HasAdditionalData Then
                            If Not Data.HasBootSector Or Offset < Data.BootSectorOffset Or Offset > Data.BootSectorOffset + DiskImage.BootSector.BOOT_SECTOR_SIZE Then
                                If DirectoryEntryHasData(FileBytes, Offset) Then
                                    Data.HasAdditionalData = True
                                End If
                            End If
                        End If
                    Else
                        Data.EntryCount += 1
                        If FileBytes.GetByte(Offset + 11) <> &HF Then 'Exclude LFN entries
                            Dim FilePart = FileBytes.ToUInt16(Offset)
                            If FilePart <> &H202E And FilePart <> &H2E2E Then 'Exclude '.' and '..' entries
                                Data.FileCount += 1
                                If FirstByte = DirectoryEntry.CHAR_DELETED Then
                                    Data.DeletedFileCount += 1
                                End If
                            End If
                        End If
                    End If
                Next
            End If
        End Sub

        Public Function GetSubDirectoryFromParentOffset(Disk As Disk, Offset As UInteger) As IDirectory
            Dim Directory As IDirectory
            If Offset = 0 Then
                Directory = Disk.Directory
            Else
                Directory = Disk.GetDirectoryEntryByOffset(Offset).SubDirectory
            End If

            Return Directory
        End Function

        Private Function CalcXDFChecksumBlock(Data() As Byte, Start As UInteger, Length As UShort) As UInteger
            Dim Checksum As UInt32 = &HABDC
            Dim Loc2 As UInt16

            Start <<= 9

            For i = 0 To Length - 1
                Loc2 = Data((Data(i + Start) * &H13) Mod Length + Start)
                Checksum = (Checksum + (Loc2 >> 5) + ((Loc2 And &H1F) << 4)) And &HFFFF&
            Next

            Return Checksum
        End Function

        Public Function CalcXDFChecksum(Data() As Byte, SectorsPerFAT As UInteger) As UInteger
            Dim Checksum As UInteger = &H12345678

            Checksum = (Checksum << 1) + CalcXDFChecksumBlock(Data, 1, &HA00)
            Checksum = (Checksum << 1) + CalcXDFChecksumBlock(Data, (SectorsPerFAT << 1) + 1, &HA00)
            Checksum = (Checksum << 1) + CalcXDFChecksumBlock(Data, (SectorsPerFAT << 1) + 6, &HA00)

            For i = 0 To &HB6 - 1
                Dim Start = ((SectorsPerFAT << 1) + &H15) + Data(&H80 + i) + (Checksum And &H7FF)
                Checksum = (Checksum << 1) + CalcXDFChecksumBlock(Data, Start, &H200)
            Next

            Return Checksum
        End Function

        Private Function ReadFileIntoBuffer(FileInfo As IO.FileInfo, FileSize As UInteger, FillChar As Byte) As Byte()
            Dim FileBuffer(FileSize - 1) As Byte
            Dim n As Integer
            Using fs = FileInfo.OpenRead()
                n = fs.Read(FileBuffer, 0, Math.Min(FileInfo.Length, FileBuffer.Length))
            End Using
            For Counter As Integer = n To FileBuffer.Length - 1
                FileBuffer(Counter) = FillChar
            Next

            Return FileBuffer
        End Function
    End Module
End Namespace
