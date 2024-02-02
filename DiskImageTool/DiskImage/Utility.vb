Imports System.IO
Imports System.IO.Compression

Namespace DiskImage
    Public Structure SectorRange
        Dim Start As UInteger
        Dim Count As UInteger
    End Structure
    Module Functions
        Public ReadOnly InvalidFileChars() As Byte = {&H22, &H2A, &H2B, &H2C, &H2E, &H2F, &H3A, &H3B, &H3C, &H3D, &H3E, &H3F, &H5B, &H5C, &H5D, &H7C}

        Public Function BootSectorDescription(Offset As DiskImage.BootSector.BootSectorOffsets) As String
            Select Case Offset
                Case DiskImage.BootSector.BootSectorOffsets.JmpBoot
                    Return "Bootstrap Jump"
                Case DiskImage.BootSector.BootSectorOffsets.OEMName
                    Return "OEM Name"
                Case DiskImage.BootSector.BootSectorOffsets.DriveNumber
                    Return "Drive Number"
                Case DiskImage.BootSector.BootSectorOffsets.Reserved
                    Return "Reserved"
                Case DiskImage.BootSector.BootSectorOffsets.ExtendedBootSignature
                    Return "Extended Boot Signature"
                Case DiskImage.BootSector.BootSectorOffsets.VolumeSerialNumber
                    Return "Volume Serial Number"
                Case DiskImage.BootSector.BootSectorOffsets.VolumeLabel
                    Return "Volume Label"
                Case DiskImage.BootSector.BootSectorOffsets.FileSystemType
                    Return "File System ID"
                Case DiskImage.BootSector.BootSectorOffsets.BootStrapSignature
                    Return "Boot Sector Signature"
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function BPBDescription(Offset As DiskImage.BiosParameterBlock.BPBOoffsets) As String
            Select Case Offset
                Case DiskImage.BiosParameterBlock.BPBOoffsets.BytesPerSector
                    Return "Bytes per Sector"
                Case DiskImage.BiosParameterBlock.BPBOoffsets.SectorsPerCluster
                    Return "Sectors per Cluster"
                Case DiskImage.BiosParameterBlock.BPBOoffsets.ReservedSectorCount
                    Return "Reserved Sectors"
                Case DiskImage.BiosParameterBlock.BPBOoffsets.NumberOfFATs
                    Return "Number of FATs"
                Case DiskImage.BiosParameterBlock.BPBOoffsets.RootEntryCount
                    Return "Root Directory Entries"
                Case DiskImage.BiosParameterBlock.BPBOoffsets.SectorCountSmall
                    Return "Total Sector Count"
                Case DiskImage.BiosParameterBlock.BPBOoffsets.MediaDescriptor
                    Return "Media Descriptor"
                Case DiskImage.BiosParameterBlock.BPBOoffsets.SectorsPerFAT
                    Return "Sectors per FAT"
                Case DiskImage.BiosParameterBlock.BPBOoffsets.SectorsPerTrack
                    Return "Sectors per Track"
                Case DiskImage.BiosParameterBlock.BPBOoffsets.NumberOfHeads
                    Return "Number of Heads"
                Case DiskImage.BiosParameterBlock.BPBOoffsets.HiddenSectors
                    Return "Hidden Sectors"
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function DirectorytEntryDescription(Offset As DiskImage.DirectoryEntry.DirectoryEntryOffsets) As String
            Select Case Offset
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.FileName
                    Return "Name"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.Extension
                    Return "Extension"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.Attributes
                    Return "Attributes"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.ReservedForWinNT
                    Return "Reserved For Windows NT"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.CreationMillisecond
                    Return "Creation Time Tenths"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.CreationTime
                    Return "Creation Time"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.CreationDate
                    Return "Creation Date"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.LastAccessDate
                    Return "Last Access Date"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.ReservedForFAT32
                    Return "Reserved for FAT 32"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.LastWriteTime
                    Return "Last Write Time"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.LastWriteDate
                    Return "Last Write Date"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.StartingCluster
                    Return "Starting Cluster"
                Case DiskImage.DirectoryEntry.DirectoryEntryOffsets.FileSize
                    Return "Size"
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function DirectorytEntryLFNDescription(Offset As DiskImage.DirectoryEntry.LFNOffsets) As String
            Select Case Offset
                Case DiskImage.DirectoryEntry.LFNOffsets.Sequence
                    Return "LFN Sequence"
                Case DiskImage.DirectoryEntry.LFNOffsets.FilePart1
                    Return "LFN Name 1"
                Case DiskImage.DirectoryEntry.LFNOffsets.Attributes
                    Return "LFN Attributes"
                Case DiskImage.DirectoryEntry.LFNOffsets.Type
                    Return "LFN Type"
                Case DiskImage.DirectoryEntry.LFNOffsets.Checksum
                    Return "LFN Checksum"
                Case DiskImage.DirectoryEntry.LFNOffsets.FilePart2
                    Return "LFN Name 2"
                Case DiskImage.DirectoryEntry.LFNOffsets.StartingCluster
                    Return "LFN Starting Cluster"
                Case DiskImage.DirectoryEntry.LFNOffsets.FilePart3
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

        Public Function DiskImageLoad(ImageData As LoadedImageData) As DiskImage.Disk
            Dim Data() As Byte

            Try
                If ImageData.Compressed Then
                    ImageData.ReadOnly = True
                    Data = OpenFileFromZIP(ImageData.SourceFile, ImageData.CompressedFile)
                Else
                    ImageData.ReadOnly = IsFileReadOnly(ImageData.SourceFile)
                    Data = IO.File.ReadAllBytes(ImageData.SourceFile)
                End If
            Catch ex As Exception
                Return Nothing
            End Try

            Return New DiskImage.Disk(Data, ImageData.FATIndex, ImageData.Modifications)
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

        Public Function GetDataFromChain(FileBytes As ByteArray, SectorChain As List(Of UInteger)) As Byte()
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

        Public Function DirectoryEntryHasData(FileBytes As ByteArray, Offset As UInteger) As Boolean
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

        Public Function GetDirectoryData(Data As DirectoryData, FileBytes As ByteArray, OffsetStart As UInteger, OffsetEnd As UInteger, EndOfDirectory As Boolean, CheckBootSector As Boolean) As Boolean
            Dim EntryCount = (OffsetEnd - OffsetStart) \ DirectoryEntry.DIRECTORY_ENTRY_SIZE

            Data.MaxEntries += EntryCount

            If EntryCount > 0 Then
                For Entry As UInteger = 0 To EntryCount - 1
                    Dim Offset = OffsetStart + (Entry * DirectoryEntry.DIRECTORY_ENTRY_SIZE)
                    Dim FirstByte = FileBytes.GetByte(Offset)
                    If FirstByte = 0 Then
                        EndOfDirectory = True
                    End If
                    If Not Data.HasBootSector And CheckBootSector Then
                        If BootSector.ValidJumpInstructuon.Contains(FirstByte) Then
                            If OffsetEnd - Offset >= BootSector.BOOT_SECTOR_SIZE Then
                                Dim BootSectorData = FileBytes.GetBytes(Offset, DiskImage.BootSector.BOOT_SECTOR_SIZE)
                                Dim BootSector = New BootSector(New ImageByteArray(BootSectorData))
                                If BootSector.IsValidImage Then
                                    Data.HasBootSector = True
                                    Data.BootSectorOffset = Offset
                                    EndOfDirectory = True
                                End If
                            End If
                        End If
                    End If
                    If EndOfDirectory Then
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

            Return EndOfDirectory
        End Function

        Public Sub ResizeArray(ByRef b() As Byte, Length As UInteger, Padding As Byte)
            Dim Size = b.Length - 1
            If Size <> Length - 1 Then
                ReDim Preserve b(Length - 1)
                For Counter As UInteger = Size + 1 To Length - 1
                    b(Counter) = Padding
                Next
            End If
        End Sub

        Public Function OpenFileFromZIP(ZipFileName As String, FileName As String) As Byte()
            Dim Data As New MemoryStream()
            Dim Archive As ZipArchive = ZipFile.OpenRead(ZipFileName)
            Dim Entry = Archive.GetEntry(FileName)
            Entry.Open.CopyTo(Data)

            Return Data.ToArray
        End Function
    End Module
End Namespace
