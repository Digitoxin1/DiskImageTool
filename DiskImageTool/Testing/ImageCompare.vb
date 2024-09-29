Imports System.Text
Imports DiskImageTool.DiskImage
Imports DiskImageTool.DiskImage.BootSector

Module ImageCompare
    Public Function CompareImages(ImageData1 As LoadedImageData, ImageData2 As LoadedImageData) As String
        Dim Builder As New StringBuilder

        Dim Disk1 = DiskImageLoad(ImageData1)
        Dim Disk2 = DiskImageLoad(ImageData2)

        CompareBootSectors(Builder, Disk1.BootSector, Disk2.BootSector)

        Return Builder.ToString
    End Function

    Private Sub AppendDifference(Builder As StringBuilder, Caption As String, ParamArray Values() As String)
        Builder.AppendLine(Caption)
        For Counter = 0 To Values.Length - 1 Step 2
            Builder.AppendLine(Values(Counter) & ": " & Values(Counter + 1))
        Next
        Builder.AppendLine("")
    End Sub

    Private Sub CompareBootSectors(Builder As StringBuilder, BootSector1 As BootSector, BootSector2 As BootSector)
        Dim Debug As Boolean = False
        Dim Name1 As String = "Image 1"
        Dim Name2 As String = "Image 2"
        Dim Modifications As Boolean = False

        Dim BootStrapStart1 = BootSector1.GetBootStrapOffset
        Dim BootStrapStart2 = BootSector2.GetBootStrapOffset
        Dim HasExtended1 = BootStrapStart1 >= BootSectorOffsets.FileSystemType + BootSectorSizes.FileSystemType
        Dim HasExtended2 = BootStrapStart2 >= BootSectorOffsets.FileSystemType + BootSectorSizes.FileSystemType

        Builder.AppendLine("Boot Sector")
        Builder.AppendLine("----------------------")

        If Not BootSector1.JmpBoot.CompareTo(BootSector2.JmpBoot) Or Debug Then
            AppendDifference(Builder, "Jump Instruction", Name1, BitConverter.ToString(BootSector1.JmpBoot), Name2, BitConverter.ToString(BootSector2.JmpBoot))
            Modifications = True
        End If

        If Not BootSector1.OEMName.CompareTo(BootSector2.OEMName) Or Debug Then
            AppendDifference(Builder, "OEM Name", Name1, BootSector1.GetOEMNameString & vbTab & BitConverter.ToString(BootSector1.OEMName), Name2, BootSector2.GetOEMNameString & vbTab & BitConverter.ToString(BootSector2.OEMName))
            Modifications = True
        End If

        If BootSector1.BPB.BytesPerSector <> BootSector2.BPB.BytesPerSector Or Debug Then
            AppendDifference(Builder, "Bytes per Sector", Name1, BootSector1.BPB.BytesPerSector, Name2, BootSector2.BPB.BytesPerSector)
            Modifications = True
        End If

        If BootSector1.BPB.SectorsPerCluster <> BootSector2.BPB.SectorsPerCluster Or Debug Then
            AppendDifference(Builder, "Sectors per Cluster", Name1, BootSector1.BPB.SectorsPerCluster, Name2, BootSector2.BPB.SectorsPerCluster)
            Modifications = True
        End If

        If BootSector1.BPB.ReservedSectorCount <> BootSector2.BPB.ReservedSectorCount Or Debug Then
            AppendDifference(Builder, "Reserved Sectors", Name1, BootSector1.BPB.ReservedSectorCount, Name2, BootSector2.BPB.ReservedSectorCount)
            Modifications = True
        End If

        If BootSector1.BPB.NumberOfFATs <> BootSector2.BPB.NumberOfFATs Or Debug Then
            AppendDifference(Builder, "Number of FATs", Name1, BootSector1.BPB.NumberOfFATs, Name2, BootSector2.BPB.NumberOfFATs)
            Modifications = True
        End If

        If BootSector1.BPB.RootEntryCount <> BootSector2.BPB.RootEntryCount Or Debug Then
            AppendDifference(Builder, "Root Directory Entries", Name1, BootSector1.BPB.RootEntryCount, Name2, BootSector2.BPB.RootEntryCount)
            Modifications = True
        End If

        If BootSector1.BPB.SectorCountSmall <> BootSector2.BPB.SectorCountSmall Or Debug Then
            AppendDifference(Builder, "Total Sector Count", Name1, BootSector1.BPB.SectorCountSmall, Name2, BootSector2.BPB.SectorCountSmall)
            Modifications = True
        End If

        If BootSector1.BPB.MediaDescriptor <> BootSector2.BPB.MediaDescriptor Or Debug Then
            AppendDifference(Builder, "Media Descriptor", Name1, BootSector1.BPB.MediaDescriptor.ToString("X2"), Name2, BootSector2.BPB.MediaDescriptor.ToString("X2"))
            Modifications = True
        End If

        If BootSector1.BPB.SectorsPerFAT <> BootSector2.BPB.SectorsPerFAT Or Debug Then
            AppendDifference(Builder, "Sectors per FAT", Name1, BootSector1.BPB.SectorsPerFAT, Name2, BootSector2.BPB.SectorsPerFAT)
            Modifications = True
        End If

        If BootSector1.BPB.SectorsPerTrack <> BootSector2.BPB.SectorsPerTrack Or Debug Then
            AppendDifference(Builder, "Sectors per Track", Name1, BootSector1.BPB.SectorsPerTrack, Name2, BootSector2.BPB.SectorsPerTrack)
            Modifications = True
        End If

        If BootSector1.BPB.NumberOfHeads <> BootSector2.BPB.NumberOfHeads Or Debug Then
            AppendDifference(Builder, "Number of Heads", Name1, BootSector1.BPB.NumberOfHeads, Name2, BootSector2.BPB.NumberOfHeads)
            Modifications = True
        End If

        If BootSector1.BPB.HiddenSectors <> BootSector2.BPB.HiddenSectors Or Debug Then
            AppendDifference(Builder, "Hidden Sectors", Name1, BootSector1.BPB.HiddenSectors, Name2, BootSector2.BPB.HiddenSectors)
            Modifications = True
        End If

        If Not Modifications Then

        End If
    End Sub

End Module
