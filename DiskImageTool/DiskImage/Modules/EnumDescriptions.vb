Namespace DiskImage
    Module EnumDescriptions
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

        Public Function GetImageTypeName(ImageType As FloppyImageType) As String
            Select Case ImageType
                Case FloppyImageType.BasicSectorImage
                    Return "Basic Sector Image"
                Case FloppyImageType.HFEImage
                    Return "HxC HFE Image"
                Case FloppyImageType.MFMImage
                    Return "HxC MFM Image"
                Case FloppyImageType.PSIImage
                    Return "PCE Sector Image"
                Case FloppyImageType.PRIImage
                    Return "PCE Bitstream Image"
                Case FloppyImageType.TranscopyImage
                    Return "Transcopy Image"
                Case FloppyImageType.D86FImage
                    Return "86Box 86F Image"
                Case FloppyImageType.IMDImage
                    Return "ImageDisk Sector Image"
                Case Else
                    Return "Unknown"
            End Select
        End Function
    End Module
End Namespace
