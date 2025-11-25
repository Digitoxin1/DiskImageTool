Namespace DiskImage
    Module EnumDescriptions
        Public Function BootSectorDescription(Offset As BootSector.BootSectorOffsets) As String
            Select Case Offset
                Case BootSector.BootSectorOffsets.JmpBoot
                    Return My.Resources.BootSector_JmpBoot
                Case BootSector.BootSectorOffsets.OEMName
                    Return My.Resources.Label_OEMName
                Case BootSector.BootSectorOffsets.DriveNumber
                    Return My.Resources.BootSector_DriveNumber
                Case BootSector.BootSectorOffsets.Reserved
                    Return My.Resources.Label_Reserved
                Case BootSector.BootSectorOffsets.ExtendedBootSignature
                    Return My.Resources.BootSector_ExtendedBootSignature
                Case BootSector.BootSectorOffsets.VolumeSerialNumber
                    Return My.Resources.BootSector_VolumeSerialNumber
                Case BootSector.BootSectorOffsets.VolumeLabel
                    Return My.Resources.Label_VolumeLabel
                Case BootSector.BootSectorOffsets.FileSystemType
                    Return My.Resources.BootSector_FileSystemType
                Case BootSector.BootSectorOffsets.BootStrapSignature
                    Return My.Resources.BootSector_BootStrapSignature
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function BPBDescription(Offset As BiosParameterBlock.BPBOoffsets) As String
            Select Case Offset
                Case BiosParameterBlock.BPBOoffsets.BytesPerSector
                    Return My.Resources.BPB_BytesPerSector
                Case BiosParameterBlock.BPBOoffsets.SectorsPerCluster
                    Return My.Resources.BPB_SectorsPerCluster
                Case BiosParameterBlock.BPBOoffsets.ReservedSectorCount
                    Return My.Resources.BPB_ReservedSectorCount
                Case BiosParameterBlock.BPBOoffsets.NumberOfFATs
                    Return My.Resources.BPB_NumberOfFATs
                Case BiosParameterBlock.BPBOoffsets.RootEntryCount
                    Return My.Resources.BPB_RootEntryCount
                Case BiosParameterBlock.BPBOoffsets.SectorCountSmall
                    Return My.Resources.BPB_SectorCountSmall
                Case BiosParameterBlock.BPBOoffsets.MediaDescriptor
                    Return My.Resources.Label_MediaDescriptor
                Case BiosParameterBlock.BPBOoffsets.SectorsPerFAT
                    Return My.Resources.BPB_SectorsPerFAT
                Case BiosParameterBlock.BPBOoffsets.SectorsPerTrack
                    Return My.Resources.BPB_SectorsPerTrack
                Case BiosParameterBlock.BPBOoffsets.NumberOfHeads
                    Return My.Resources.BPB_NumberOfHeads
                Case BiosParameterBlock.BPBOoffsets.HiddenSectors
                    Return My.Resources.BPB_HiddenSectors
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function DirectorytEntryDescription(Offset As DirectoryEntry.DirectoryEntryOffsets) As String
            Select Case Offset
                Case DirectoryEntry.DirectoryEntryOffsets.FileName
                    Return My.Resources.Label_Name
                Case DirectoryEntry.DirectoryEntryOffsets.Extension
                    Return My.Resources.DirectorytEntry_Extension
                Case DirectoryEntry.DirectoryEntryOffsets.Attributes
                    Return My.Resources.DirectorytEntry_Attributes
                Case DirectoryEntry.DirectoryEntryOffsets.ReservedForWinNT
                    Return My.Resources.DirectorytEntry_ReservedForWinNT
                Case DirectoryEntry.DirectoryEntryOffsets.CreationMillisecond
                    Return My.Resources.DirectorytEntry_CreationMillisecond
                Case DirectoryEntry.DirectoryEntryOffsets.CreationTime
                    Return My.Resources.DirectorytEntry_CreationTime
                Case DirectoryEntry.DirectoryEntryOffsets.CreationDate
                    Return My.Resources.DirectorytEntry_CreationDate
                Case DirectoryEntry.DirectoryEntryOffsets.LastAccessDate
                    Return My.Resources.DirectorytEntry_LastAccessDate
                Case DirectoryEntry.DirectoryEntryOffsets.ReservedForFAT32
                    Return My.Resources.DirectorytEntry_ReservedForFAT32
                Case DirectoryEntry.DirectoryEntryOffsets.LastWriteTime
                    Return My.Resources.DirectorytEntry_LastWriteTime
                Case DirectoryEntry.DirectoryEntryOffsets.LastWriteDate
                    Return My.Resources.DirectorytEntry_LastWriteDate
                Case DirectoryEntry.DirectoryEntryOffsets.StartingCluster
                    Return My.Resources.DirectorytEntry_StartingCluster
                Case DirectoryEntry.DirectoryEntryOffsets.FileSize
                    Return My.Resources.Label_Size
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function DirectorytEntryLFNDescription(Offset As DirectoryEntry.LFNOffsets) As String
            Select Case Offset
                Case DirectoryEntry.LFNOffsets.Sequence
                    Return My.Resources.DirectorytEntryLFN_Sequence
                Case DirectoryEntry.LFNOffsets.FilePart1
                    Return String.Format(My.Resources.DirectorytEntryLFN_FilePart, "1")
                Case DirectoryEntry.LFNOffsets.Attributes
                    Return My.Resources.DirectorytEntryLFN_Attributes
                Case DirectoryEntry.LFNOffsets.Type
                    Return My.Resources.DirectorytEntryLFN_Type
                Case DirectoryEntry.LFNOffsets.Checksum
                    Return My.Resources.DirectorytEntryLFN_Checksum
                Case DirectoryEntry.LFNOffsets.FilePart2
                    Return String.Format(My.Resources.DirectorytEntryLFN_FilePart, "2")
                Case DirectoryEntry.LFNOffsets.StartingCluster
                    Return My.Resources.DirectorytEntryLFN_StartingCluster
                Case DirectoryEntry.LFNOffsets.FilePart3
                    Return String.Format(My.Resources.DirectorytEntryLFN_FilePart, "3")
                Case Else
                    Return Offset.ToString
            End Select
        End Function

        Public Function GetImageTypeName(ImageType As FloppyImageType) As String
            Select Case ImageType
                Case FloppyImageType.BasicSectorImage
                    Return My.Resources.FloppyImageType_BasicSectorImage
                Case FloppyImageType.HFEImage
                    Return My.Resources.FloppyImageType_HFEImage
                Case FloppyImageType.MFMImage
                    Return My.Resources.FloppyImageType_MFMImage
                Case FloppyImageType.PSIImage
                    Return My.Resources.FloppyImageType_PSIImage
                Case FloppyImageType.PRIImage
                    Return My.Resources.FloppyImageType_PRIImage
                Case FloppyImageType.TranscopyImage
                    Return My.Resources.FloppyImageType_TranscopyImage
                Case FloppyImageType.D86FImage
                    Return My.Resources.FloppyImageType_D86FImage
                Case FloppyImageType.IMDImage
                    Return My.Resources.FloppyImageType_IMDImage
                Case Else
                    Return My.Resources.Label_Unknown
            End Select
        End Function
    End Module
End Namespace
