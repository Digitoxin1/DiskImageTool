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

        If BootSector1.BytesPerSector <> BootSector2.BytesPerSector Or Debug Then
            AppendDifference(Builder, "Bytes per Sector", Name1, BootSector1.BytesPerSector, Name2, BootSector2.BytesPerSector)
            Modifications = True
        End If

        If BootSector1.SectorsPerCluster <> BootSector2.SectorsPerCluster Or Debug Then
            AppendDifference(Builder, "Sectors per Cluster", Name1, BootSector1.SectorsPerCluster, Name2, BootSector2.SectorsPerCluster)
            Modifications = True
        End If

        If BootSector1.ReservedSectorCount <> BootSector2.ReservedSectorCount Or Debug Then
            AppendDifference(Builder, "Reserved Sectors", Name1, BootSector1.ReservedSectorCount, Name2, BootSector2.ReservedSectorCount)
            Modifications = True
        End If

        If BootSector1.NumberOfFATs <> BootSector2.NumberOfFATs Or Debug Then
            AppendDifference(Builder, "Number of FATs", Name1, BootSector1.NumberOfFATs, Name2, BootSector2.NumberOfFATs)
            Modifications = True
        End If

        If BootSector1.RootEntryCount <> BootSector2.RootEntryCount Or Debug Then
            AppendDifference(Builder, "Root Directory Entries", Name1, BootSector1.RootEntryCount, Name2, BootSector2.RootEntryCount)
            Modifications = True
        End If

        If BootSector1.SectorCountSmall <> BootSector2.SectorCountSmall Or Debug Then
            AppendDifference(Builder, "Total Sector Count", Name1, BootSector1.SectorCountSmall, Name2, BootSector2.SectorCountSmall)
            Modifications = True
        End If

        If BootSector1.MediaDescriptor <> BootSector2.MediaDescriptor Or Debug Then
            AppendDifference(Builder, "Media Descriptor", Name1, BootSector1.MediaDescriptor.ToString("X2"), Name2, BootSector2.MediaDescriptor.ToString("X2"))
            Modifications = True
        End If

        If BootSector1.SectorsPerFAT <> BootSector2.SectorsPerFAT Or Debug Then
            AppendDifference(Builder, "Sectors per FAT", Name1, BootSector1.SectorsPerFAT, Name2, BootSector2.SectorsPerFAT)
            Modifications = True
        End If

        If BootSector1.SectorsPerTrack <> BootSector2.SectorsPerTrack Or Debug Then
            AppendDifference(Builder, "Sectors per Track", Name1, BootSector1.SectorsPerTrack, Name2, BootSector2.SectorsPerTrack)
            Modifications = True
        End If

        If BootSector1.NumberOfHeads <> BootSector2.NumberOfHeads Or Debug Then
            AppendDifference(Builder, "Number of Heads", Name1, BootSector1.NumberOfHeads, Name2, BootSector2.NumberOfHeads)
            Modifications = True
        End If

        If BootSector1.HiddenSectors <> BootSector2.HiddenSectors Or Debug Then
            AppendDifference(Builder, "Hidden Sectors", Name1, BootSector1.HiddenSectors, Name2, BootSector2.HiddenSectors)
            Modifications = True
        End If

        If Not Modifications Then

        End If
    End Sub

End Module
