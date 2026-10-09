Namespace DiskImage
    Module EnumDescriptions
        Public Function BootSectorDescription(Offset As BootSector.BootSectorOffsets) As String
            Select Case Offset
                Case BootSector.BootSectorOffsets.JmpBoot
                    Return My.Resources.BootSector_JmpBoot
                Case BootSector.BootSectorOffsets.OEMName
                    Return My.Resources.Label_OEMName
                Case BootSector.BootSectorOffsets.DriveNumber
                    Return My.Resources.Label_DriveNumber
                Case BootSector.BootSectorOffsets.Reserved
                    Return My.Resources.Label_Reserved
                Case BootSector.BootSectorOffsets.ExtendedBootSignature
                    Return My.Resources.Label_ExtendedBootSignature
                Case BootSector.BootSectorOffsets.VolumeSerialNumber
                    Return My.Resources.Label_VolumeSerialNumber
                Case BootSector.BootSectorOffsets.VolumeLabel
                    Return My.Resources.Label_VolumeLabel
                Case BootSector.BootSectorOffsets.FileSystemType
                    Return My.Resources.Label_FileSystemType
                Case BootSector.BootSectorOffsets.BootStrapSignature
                    Return My.Resources.Label_BootStrapSignature
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function BPBDescription(Offset As BiosParameterBlock.BPBOoffsets) As String
            Select Case Offset
                Case BiosParameterBlock.BPBOoffsets.BytesPerSector
                    Return My.Resources.Label_BytesPerSector
                Case BiosParameterBlock.BPBOoffsets.SectorsPerCluster
                    Return My.Resources.Label_SectorsPerCluster
                Case BiosParameterBlock.BPBOoffsets.ReservedSectorCount
                    Return My.Resources.Label_ReservedSectorCount
                Case BiosParameterBlock.BPBOoffsets.NumberOfFATs
                    Return My.Resources.Label_NumberOfFATs
                Case BiosParameterBlock.BPBOoffsets.RootEntryCount
                    Return My.Resources.Label_RootEntryCount
                Case BiosParameterBlock.BPBOoffsets.SectorCountSmall
                    Return My.Resources.Label_SectorCountSmall
                Case BiosParameterBlock.BPBOoffsets.MediaDescriptor
                    Return My.Resources.Label_MediaDescriptor
                Case BiosParameterBlock.BPBOoffsets.SectorsPerFAT
                    Return My.Resources.Label_SectorsPerFAT
                Case BiosParameterBlock.BPBOoffsets.SectorsPerTrack
                    Return My.Resources.Label_SectorsPerTrack
                Case BiosParameterBlock.BPBOoffsets.NumberOfHeads
                    Return My.Resources.Label_NumberOfHeads
                Case BiosParameterBlock.BPBOoffsets.HiddenSectors
                    Return My.Resources.Label_HiddenSectors
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function DirectorytEntryDescription(Offset As DirectoryEntry.DirectoryEntryOffsets) As String
            Select Case Offset
                Case DirectoryEntry.DirectoryEntryOffsets.FileName
                    Return My.Resources.Label_Name
                Case DirectoryEntry.DirectoryEntryOffsets.Extension
                    Return My.Resources.DirectoryEntry_Extension
                Case DirectoryEntry.DirectoryEntryOffsets.Attributes
                    Return My.Resources.Label_Attributes
                Case DirectoryEntry.DirectoryEntryOffsets.ReservedForWinNT
                    Return My.Resources.DirectoryEntry_ReservedForWinNT
                Case DirectoryEntry.DirectoryEntryOffsets.CreationMillisecond
                    Return My.Resources.DirectoryEntry_CreationMillisecond
                Case DirectoryEntry.DirectoryEntryOffsets.CreationTime
                    Return My.Resources.DirectoryEntry_CreationTime
                Case DirectoryEntry.DirectoryEntryOffsets.CreationDate
                    Return My.Resources.DirectoryEntry_CreationDate
                Case DirectoryEntry.DirectoryEntryOffsets.LastAccessDate
                    Return My.Resources.DirectoryEntry_LastAccessDate
                Case DirectoryEntry.DirectoryEntryOffsets.ReservedForFAT32
                    Return My.Resources.DirectoryEntry_ReservedForFAT32
                Case DirectoryEntry.DirectoryEntryOffsets.LastWriteTime
                    Return My.Resources.DirectoryEntry_LastWriteTime
                Case DirectoryEntry.DirectoryEntryOffsets.LastWriteDate
                    Return My.Resources.DirectoryEntry_LastWriteDate
                Case DirectoryEntry.DirectoryEntryOffsets.StartingCluster
                    Return My.Resources.DirectoryEntry_StartingCluster
                Case DirectoryEntry.DirectoryEntryOffsets.FileSize
                    Return My.Resources.Label_Size
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function DirectorytEntryLFNDescription(Offset As DirectoryEntry.LFNOffsets) As String
            Select Case Offset
                Case DirectoryEntry.LFNOffsets.Sequence
                    Return My.Resources.DirectoryEntryLFN_Sequence
                Case DirectoryEntry.LFNOffsets.FilePart1
                    Return String.Format(My.Resources.DirectoryEntryLFN_FilePart, "1")
                Case DirectoryEntry.LFNOffsets.Attributes
                    Return My.Resources.DirectoryEntryLFN_Attributes
                Case DirectoryEntry.LFNOffsets.Type
                    Return My.Resources.DirectoryEntryLFN_Type
                Case DirectoryEntry.LFNOffsets.Checksum
                    Return My.Resources.DirectoryEntryLFN_Checksum
                Case DirectoryEntry.LFNOffsets.FilePart2
                    Return String.Format(My.Resources.DirectoryEntryLFN_FilePart, "2")
                Case DirectoryEntry.LFNOffsets.StartingCluster
                    Return My.Resources.DirectoryEntryLFN_StartingCluster
                Case DirectoryEntry.LFNOffsets.FilePart3
                    Return String.Format(My.Resources.DirectoryEntryLFN_FilePart, "3")
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function GetImageTypeName(ImageType As FloppyImageType) As String
            Select Case ImageType
                Case FloppyImageType.BasicSectorImage
                    Return My.Resources.FloppyImageType_BasicSectorImage
                Case FloppyImageType.HFEImage
                    Return String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_HFE)
                Case FloppyImageType.MFMImage
                    Return String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_MFM)
                Case FloppyImageType.PSIImage
                    Return String.Format(My.Resources.FloppyImageType_SectorImage, My.Resources.FloppyImageType_PCE)
                Case FloppyImageType.PRIImage
                    Return String.Format(My.Resources.FloppyImageType_BitstreamImage, My.Resources.FloppyImageType_PCE)
                Case FloppyImageType.TranscopyImage
                    Return String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_TC)
                Case FloppyImageType.D86FImage
                    Return String.Format(My.Resources.FloppyImageType_Image, My.Resources.FloppyImageType_86F)
                Case FloppyImageType.IMDImage
                    Return String.Format(My.Resources.FloppyImageType_SectorImage, My.Resources.FloppyImageType_IMD)
                Case FloppyImageType.TD0Image
                    Return String.Format(My.Resources.FloppyImageType_SectorImage, My.Resources.FloppyImageType_TD0)
                Case Else
                    Return My.Resources.Label_Unknown
            End Select
        End Function
    End Module
End Namespace
