Imports DiskImageTool.DiskImage
Imports BootSectorOffsets = DiskImageTool.DiskImage.BootSector.BootSectorOffsets
Imports BPBOffsets = DiskImageTool.DiskImage.BiosParameterBlock.BPBOoffsets

Module SummaryPanel
    Public Const NULL_CHAR As Char = "�"

    Public Sub PopulateSummaryPanelError(ListViewSummary As ListView, InvalidImage As Boolean)
        With ListViewSummary
            Dim DiskGroup = .Groups.Add("Disk", "Disk")
            Dim Msg As String

            If InvalidImage Then
                Msg = "Invalid Image Format"
            Else
                Msg = "Error Loading File"
            End If

            Dim Item = New ListViewItem("  " & Msg, DiskGroup) With {
                .ForeColor = Color.Red
            }
            .Items.Add(Item)
        End With
    End Sub

    Public Sub PopulateSummaryPanelMain(ListViewSummary As ListView, Disk As Disk, TitleDB As FloppyDB, BootStrapDB As BootstrapDB, MD5 As String)
        Dim TitleFound As Boolean = False

        If TitleDB.TitleCount > 0 Then
            Dim TitleFindResult = TitleDB.TitleFind(MD5)
            If TitleFindResult.TitleData IsNot Nothing Then
                TitleFound = True
                PopulateSummaryPanelTitle(ListViewSummary, TitleFindResult.TitleData)
            End If
        End If

        Dim DiskGroup = PopulateSummaryPanelDisk(ListViewSummary, Disk)

        With ListViewSummary
            If Not Disk.IsValidImage Then
                .AddItem(DiskGroup, "File System", "Unknown", Color.Red)
            Else
                Dim OEMNameResponse = BootStrapDB.CheckOEMName(Disk.BootSector)

                If OEMNameResponse.NoBootLoader Then
                    If Disk.BootSector.CheckJumpInstruction(False, True) Then
                        .AddItem(DiskGroup, "Bootstrap", "No Boot Loader", Color.Red)
                    Else
                        .AddItem(DiskGroup, "Bootstrap", "Custom Boot Loader", Color.Red)
                    End If
                ElseIf Not Disk.BootSector.BPB.IsValid Then
                    .AddItem(DiskGroup, "Boot Record", "No BPB", Color.Red)
                End If

                If Not Disk.BootSector.BPB.IsValid Then
                    If Not Disk.FATTables.FATsMatch Then
                        .AddItem(DiskGroup, "FAT", "Mismatched", Color.Red)
                    End If
                End If

                If Disk.BootSector.BPB.IsValid Then
                    PopulateSummaryPanelBootRecord(ListViewSummary, Disk, OEMNameResponse)
                End If

                PopulateSummaryPanelFileSystem(ListViewSummary, Disk)
                PopulateSummaryPanelBootstrap(ListViewSummary, Disk, OEMNameResponse)
            End If
        End With
    End Sub

    Private Function GetFileSystemInfo(Disk As Disk) As FileSystemInfo
        Dim fsi As FileSystemInfo

        fsi.OldestFileDate = Nothing
        fsi.NewestFileDate = Nothing
        fsi.VolumeLabel = Nothing

        Dim FileList = Disk.RootDirectory.GetFileList()

        For Each DirectoryEntry In FileList
            If DirectoryEntry.IsValid Then
                If fsi.VolumeLabel Is Nothing AndAlso DirectoryEntry.IsValidVolumeName AndAlso DirectoryEntry.ParentDirectory.IsRootDirectory Then
                    fsi.VolumeLabel = DirectoryEntry
                End If
                Dim LastWriteDate = DirectoryEntry.GetLastWriteDate
                If LastWriteDate.IsValidDate Then
                    If fsi.OldestFileDate Is Nothing Then
                        fsi.OldestFileDate = LastWriteDate.DateObject
                    ElseIf fsi.OldestFileDate.Value.CompareTo(LastWriteDate.DateObject) > 0 Then
                        fsi.OldestFileDate = LastWriteDate.DateObject
                    End If
                    If fsi.NewestFileDate Is Nothing Then
                        fsi.NewestFileDate = LastWriteDate.DateObject
                    ElseIf fsi.NewestFileDate.Value.CompareTo(LastWriteDate.DateObject) < 0 Then
                        fsi.NewestFileDate = LastWriteDate.DateObject
                    End If
                End If
            End If
        Next

        Return fsi
    End Function

    Private Function GetNonStandardTrackList(NonStandardTracks As HashSet(Of UShort), HeadCount As Byte) As String
        Dim TrackList(NonStandardTracks.Count - 1) As UShort
        Dim TrackStartString As String
        Dim TrackEndString As String
        Dim Separator As String

        Dim i As UShort = 0
        For Each Track In NonStandardTracks
            TrackList(i) = Track
            i += 1
        Next

        Array.Sort(TrackList)

        Dim Result As New List(Of String)
        Dim StartRange As UShort = TrackList(0)
        Dim Prev As UShort = TrackList(0)

        For i = 1 To TrackList.Length - 1
            If TrackList(i) = Prev + 1 Then
                Prev = TrackList(i)
            Else
                TrackStartString = (StartRange \ HeadCount) & "." & (StartRange Mod HeadCount)
                If StartRange = Prev Then
                    Result.Add(TrackStartString)
                Else
                    TrackEndString = (Prev \ HeadCount) & "." & (Prev Mod HeadCount)
                    If Prev = StartRange + 1 Then
                        Separator = ", "
                    Else
                        Separator = " - "
                    End If
                    Result.Add(TrackStartString & Separator & TrackEndString)
                End If
                StartRange = TrackList(i)
                Prev = StartRange
            End If
        Next

        TrackStartString = (StartRange \ HeadCount) & "." & (StartRange Mod HeadCount)
        If StartRange = Prev Then
            Result.Add(TrackStartString)
        Else
            TrackEndString = (Prev \ HeadCount) & "." & (Prev Mod HeadCount)
            If Prev = StartRange + 1 Then
                Separator = ", "
            Else
                Separator = " - "
            End If
            Result.Add(TrackStartString & Separator & TrackEndString)
        End If

        Return String.Join(", ", Result)
    End Function

    Private Sub PopulateSummaryPanelBootRecord(ListViewSummary As ListView, Disk As Disk, OEMNameResponse As OEMNameResponse)
        Dim Value As String
        Dim ForeColor As Color

        Dim DiskFormatBySize = GetFloppyDiskFormat(Disk.Image.Length)
        Dim BPBBySize = BuildBPB(DiskFormatBySize)
        Dim DoBPBCompare = Disk.DiskFormat = FloppyDiskFormat.FloppyUnknown And DiskFormatBySize <> FloppyDiskFormat.FloppyUnknown

        With ListViewSummary
            Dim BootRecordGroup = .Groups.Add("BootRecord", "Boot Record")

            If Not OEMNameResponse.Found Then
                ForeColor = SystemColors.WindowText
            ElseIf Not OEMNameResponse.Matched Then
                ForeColor = Color.Red
            ElseIf OEMNameResponse.Verified Then
                ForeColor = Color.Green
            Else
                ForeColor = Color.Blue
            End If

            .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.OEMName), Disk.BootSector.GetOEMNameString.TrimEnd(NULL_CHAR), ForeColor)

            If DoBPBCompare AndAlso Disk.BootSector.BPB.BytesPerSector <> BPBBySize.BytesPerSector Then
                ForeColor = Color.Blue
            Else
                ForeColor = SystemColors.WindowText
            End If

            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.BytesPerSector), Disk.BootSector.BPB.BytesPerSector, ForeColor)

            If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorsPerCluster <> BPBBySize.SectorsPerCluster Then
                ForeColor = Color.Blue
            Else
                ForeColor = SystemColors.WindowText
            End If

            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.SectorsPerCluster), Disk.BootSector.BPB.SectorsPerCluster, ForeColor)

            If DoBPBCompare AndAlso Disk.BootSector.BPB.ReservedSectorCount <> BPBBySize.ReservedSectorCount Then
                ForeColor = Color.Blue
            Else
                ForeColor = SystemColors.WindowText
            End If

            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.ReservedSectorCount), Disk.BootSector.BPB.ReservedSectorCount, ForeColor)

            If DoBPBCompare AndAlso Disk.BootSector.BPB.NumberOfFATs <> BPBBySize.NumberOfFATs Then
                ForeColor = Color.Blue
            Else
                ForeColor = SystemColors.WindowText
            End If

            If IsDiskFormatXDF(Disk.DiskFormat) Then
                Value = "1 + Compatibility Image"
            Else
                Value = Disk.BootSector.BPB.NumberOfFATs
                If Not Disk.FATTables.FATsMatch Then
                    Value &= " (Mismatched)"
                    ForeColor = Color.Red
                End If
            End If

            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.NumberOfFATs), Value, ForeColor)

            If DoBPBCompare AndAlso Disk.BootSector.BPB.RootEntryCount <> BPBBySize.RootEntryCount Then
                ForeColor = Color.Blue
            Else
                ForeColor = SystemColors.WindowText
            End If

            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.RootEntryCount), Disk.BootSector.BPB.RootEntryCount, ForeColor)

            If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorCount <> BPBBySize.SectorCount Then
                ForeColor = Color.Blue
            Else
                ForeColor = SystemColors.WindowText
            End If

            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.SectorCountSmall), Disk.BootSector.BPB.SectorCount, ForeColor)

            If DoBPBCompare AndAlso Disk.BootSector.BPB.MediaDescriptor <> BPBBySize.MediaDescriptor Then
                ForeColor = Color.Blue
            Else
                ForeColor = SystemColors.WindowText
            End If

            Value = Disk.BootSector.BPB.MediaDescriptor.ToString("X2") & " Hex"

            If Disk.BootSector.BPB.IsValid Then
                If Not Disk.BPB.HasValidMediaDescriptor Then
                    Value &= " (Invalid)"
                    ForeColor = Color.Red
                ElseIf Disk.DiskFormat = FloppyDiskFormat.FloppyXDF35 AndAlso Disk.FAT.MediaDescriptor = &HF9 Then
                    'Do Nothing - This is normal for XDF
                ElseIf Disk.DiskFormat <> FloppyDiskFormat.FloppyUnknown AndAlso Disk.BootSector.BPB.MediaDescriptor <> GetFloppyDiskMediaDescriptor(Disk.DiskFormat) Then
                    Value &= " (Mismatched)"
                    ForeColor = Color.Red
                ElseIf Disk.FAT.MediaDescriptor <> Disk.BootSector.BPB.MediaDescriptor Then
                    Value &= " (Mismatched)"
                    ForeColor = Color.Red
                End If
            End If

            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.MediaDescriptor), Value, ForeColor)

            If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorsPerFAT <> BPBBySize.SectorsPerFAT Then
                ForeColor = Color.Blue
            Else
                ForeColor = SystemColors.WindowText
            End If

            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.SectorsPerFAT), Disk.BootSector.BPB.SectorsPerFAT, ForeColor)

            If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorsPerTrack <> BPBBySize.SectorsPerTrack Then
                ForeColor = Color.Blue
            Else
                ForeColor = SystemColors.WindowText
            End If

            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.SectorsPerTrack), Disk.BootSector.BPB.SectorsPerTrack, ForeColor)

            If DoBPBCompare AndAlso Disk.BootSector.BPB.NumberOfHeads <> BPBBySize.NumberOfHeads Then
                ForeColor = Color.Blue
            Else
                ForeColor = SystemColors.WindowText
            End If

            .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.NumberOfHeads), Disk.BootSector.BPB.NumberOfHeads, ForeColor)

            If Disk.BootSector.BPB.HiddenSectors > 0 Then
                .AddItem(BootRecordGroup, BPBDescription(BPBOffsets.HiddenSectors), Disk.BootSector.BPB.HiddenSectors)
            End If

            Dim BootStrapStart = Disk.BootSector.GetBootStrapOffset

            If BootStrapStart >= BootSectorOffsets.BootStrapCode Then
                If Disk.BootSector.DriveNumber > 0 Then
                    .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.DriveNumber), Disk.BootSector.DriveNumber)
                End If

                If Disk.BootSector.HasValidExtendedBootSignature Then
                    .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.VolumeSerialNumber), Disk.BootSector.VolumeSerialNumber.ToString("X8").Insert(4, "-"))
                    If Disk.BootSector.ExtendedBootSignature = BootSector.ValidExtendedBootSignature(1) Then
                        .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.VolumeLabel), Disk.BootSector.GetVolumeLabelString.TrimEnd(NULL_CHAR))
                        .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.FileSystemType), Disk.BootSector.GetFileSystemTypeString)
                    End If
                End If
            End If

            If Not Disk.BootSector.HasValidBootStrapSignature Then
                .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.BootStrapSignature), Disk.BootSector.BootStrapSignature.ToString("X4"))
            End If

            If My.Settings.Debug Then
                If Not Disk.BootSector.CheckJumpInstruction(True, True) Then
                    ForeColor = Color.Red
                Else
                    ForeColor = SystemColors.WindowText
                End If
                .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.JmpBoot), BitConverter.ToString(Disk.BootSector.JmpBoot), ForeColor)
            End If
        End With
    End Sub

    Private Sub PopulateSummaryPanelBootstrap(ListViewSummary As ListView, Disk As Disk, OEMNameResponse As OEMNameResponse)
        Dim ForeColor As Color

        With ListViewSummary
            Dim BootStrapGroup = .Groups.Add("Bootstrap", "Bootstrap")

            If Not OEMNameResponse.NoBootLoader Then
                Dim BootStrapCRC32 = CRC32.ComputeChecksum(Disk.BootSector.BootStrapCode)
                .AddItem(BootStrapGroup, "Bootstrap CRC32", BootStrapCRC32.ToString("X8"))
            End If

            If OEMNameResponse.Found Then
                If OEMNameResponse.Data.Language.Length > 0 Then
                    .AddItem(BootStrapGroup, "Language", OEMNameResponse.Data.Language)
                End If

                Dim OEMName = OEMNameResponse.MatchedOEMName

                If OEMName Is Nothing And OEMNameResponse.Data.OEMNames.Count = 1 Then
                    OEMName = OEMNameResponse.Data.OEMNames(0)
                End If

                If OEMName IsNot Nothing Then
                    If OEMName.Company <> "" Then
                        .AddItem(BootStrapGroup, "Company", OEMName.Company)
                    End If
                    If OEMName.Description <> "" Then
                        .AddItem(BootStrapGroup, "Description", OEMName.Description)
                    End If
                    If OEMName.Note <> "" Then
                        .AddItem(BootStrapGroup, "Note", OEMName.Note, Color.Blue)
                    End If
                End If

                If Disk.BootSector.BPB.IsValid Then
                    If Not OEMNameResponse.Data.ExactMatch Then
                        For Each OEMName In OEMNameResponse.Data.OEMNames
                            If OEMName.Name.Length > 0 AndAlso OEMName.Suggestion AndAlso OEMName IsNot OEMNameResponse.MatchedOEMName Then
                                If OEMName.Verified Then
                                    ForeColor = Color.Green
                                Else
                                    ForeColor = SystemColors.WindowText
                                End If
                                .AddItem(BootStrapGroup, "Alternative OEM Name", OEMName.GetNameAsString, ForeColor)
                            End If
                        Next
                    End If
                End If
            End If
        End With
    End Sub

    Private Function PopulateSummaryPanelDisk(ListViewSummary As ListView, Disk As Disk) As ListViewGroup
        Dim DiskGroup As ListViewGroup
        Dim Value As String
        Dim ForeColor As Color

        With ListViewSummary
            DiskGroup = .Groups.Add("Disk", "Disk")

            If Disk.Image.Data.ImageType = FloppyImageType.BasicSectorImage Then
                If Disk.IsValidImage AndAlso Disk.CheckImageSize <> 0 Then
                    ForeColor = Color.Red
                Else
                    ForeColor = SystemColors.WindowText
                End If
                .AddItem(DiskGroup, "Image Size", Disk.Image.Length.ToString("N0"), ForeColor)
            End If

            If Disk.IsValidImage(False) Then
                Dim DiskFormatString = GetFloppyDiskFormatName(Disk.DiskFormat)
                Dim DiskFormatBySize = GetFloppyDiskFormat(Disk.Image.Length)

                If Disk.DiskFormat <> FloppyDiskFormat.FloppyUnknown Or DiskFormatBySize = FloppyDiskFormat.FloppyUnknown Then
                    .AddItem(DiskGroup, "Disk Type", DiskFormatString & " Floppy")
                Else
                    Dim DiskFormatStringBySize = GetFloppyDiskFormatName(DiskFormatBySize)
                    .AddItem(DiskGroup, "Disk Type", DiskFormatStringBySize & " Floppy (Custom Format)")
                End If

                If IsDiskFormatXDF(Disk.DiskFormat) Then
                    Dim XDFChecksum = CalcXDFChecksum(Disk.Image.GetBytes, Disk.BPB.SectorsPerFAT)
                    If XDFChecksum = Disk.GetXDFChecksum Then
                        ForeColor = Color.Green
                    Else
                        ForeColor = Color.Red
                    End If
                    .AddItem(DiskGroup, "XDF Checksum", XDFChecksum.ToString("X8"), ForeColor)
                End If

                If Disk.BPB.IsValid AndAlso Disk.CheckImageSize > 0 AndAlso Disk.DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
                    .AddItem(DiskGroup, DiskFormatString & " CRC32", HashFunctions.CRC32Hash(Disk.Image.GetBytes(0, Disk.BPB.ReportedImageSize())))
                End If
            End If

            If Disk.Image.Data.ImageType <> FloppyImageType.BasicSectorImage Then
                .AddItem(DiskGroup, "Tracks", Disk.Image.Data.TrackCount)
                .AddItem(DiskGroup, "Heads", Disk.Image.Data.HeadCount)
            End If

            If Disk.Image.Data.ImageType = FloppyImageType.MFMImage Then
                Dim Image As ImageFormats.MFM.MFMImage = DirectCast(Disk.Image.Data, ImageFormats.MFM.MFMByteArray).Image
                If Image.BitRate > 0 Then
                    .AddItem(DiskGroup, "Bitrate", Bitstream.RoundBitRate(Image.BitRate))
                End If
                If Image.RPM > 0 Then
                    .AddItem(DiskGroup, "RPM", Bitstream.RoundRPM(Image.RPM))
                End If

            ElseIf Disk.Image.Data.ImageType = FloppyImageType.HFEImage Then
                Dim Image As ImageFormats.HFE.HFEImage = DirectCast(Disk.Image.Data, ImageFormats.HFE.HFEByteArray).Image
                If Image.BitRate > 0 Then
                    .AddItem(DiskGroup, "Bitrate", Bitstream.RoundBitRate(Image.BitRate))
                End If
                If Image.FloppyRPM > 0 Then
                    .AddItem(DiskGroup, "RPM", Bitstream.RoundRPM(Image.FloppyRPM))
                End If

            ElseIf Disk.Image.Data.ImageType = FloppyImageType._86FImage Then
                Dim Image As ImageFormats._86F._86FImage = DirectCast(Disk.Image.Data, ImageFormats._86F._86FByteArray).Image
                If Image.BitRate = 255 Then
                    Value = "Variable"
                Else
                    Value = ImageFormats._86F.GetBitRate(Image.BitRate, True)
                End If
                .AddItem(DiskGroup, "Bitrate", Value)
                If Image.RPM = 255 Then
                    Value = "Variable"
                Else
                    Value = ImageFormats._86F.GetRPM(Image.RPM)
                End If
                .AddItem(DiskGroup, "RPM", Value)
                If Image.RPMSlowDown <> 0 Then
                    If Image.AlternateBitcellCalculation Then
                        Value = "Speed up by " & (Image.RPMSlowDown * 100) & "%"
                    Else
                        Value = "Slow down by " & (Image.RPMSlowDown * 100) & "%"
                    End If
                    .AddItem(DiskGroup, "RPM Adjustment", Value)
                End If
                .AddItem(DiskGroup, "Has Surface Data", If(Image.HasSurfaceData, "Yes", "No"))

            ElseIf Disk.Image.Data.ImageType = FloppyImageType.TranscopyImage Then
                Dim Image As ImageFormats.TC.TransCopyImage = DirectCast(Disk.Image.Data, ImageFormats.TC.TranscopyByteArray).Image
            End If

            If Disk.Image.Data.NonStandardTracks.Count > 0 Then
                .AddItem(DiskGroup, "Non-Standard Tracks", GetNonStandardTrackList(Disk.Image.Data.NonStandardTracks, Disk.Image.Data.HeadCount))
            End If
        End With

        Return DiskGroup
    End Function

    Private Sub PopulateSummaryPanelFileSystem(ListViewSummary As ListView, Disk As Disk)
        Dim Value As String
        Dim ForeColor As Color

        With ListViewSummary
            Dim FileSystemGroup = .Groups.Add("FileSystem", "File System")

            If Disk.FAT.HasMediaDescriptor Then
                Value = Disk.FAT.MediaDescriptor.ToString("X2") & " Hex"
                ForeColor = SystemColors.WindowText
                Dim Visible As Boolean = False
                If Not Disk.BootSector.BPB.IsValid Then
                    Visible = True
                ElseIf Disk.FAT.MediaDescriptor <> Disk.BootSector.BPB.MediaDescriptor Then
                    Visible = True
                End If
                If Not Disk.FAT.HasValidMediaDescriptor Then
                    Value &= " (Invalid)"
                    ForeColor = Color.Red
                    Visible = True
                ElseIf Disk.DiskFormat = FloppyDiskFormat.FloppyXDF35 AndAlso Disk.FAT.MediaDescriptor = &HF9 Then
                    Visible = False
                ElseIf Disk.DiskFormat <> FloppyDiskFormat.FloppyUnknown AndAlso Disk.FAT.MediaDescriptor <> GetFloppyDiskMediaDescriptor(Disk.DiskFormat) Then
                    Value &= " (Mismatched)"
                    ForeColor = Color.Red
                    Visible = True
                End If

                If Visible Then
                    .AddItem(FileSystemGroup, "Media Descriptor", Value, ForeColor)
                End If
            End If

            Dim fsi = GetFileSystemInfo(Disk)

            If fsi.VolumeLabel IsNot Nothing Then
                .AddItem(FileSystemGroup, "Volume Label", fsi.VolumeLabel.GetVolumeName.TrimEnd(NULL_CHAR))
                Dim VolumeDate = fsi.VolumeLabel.GetLastWriteDate
                .AddItem(FileSystemGroup, "Volume Date", ExpandedDateToString(VolumeDate, True, False, False, False))
            End If

            .AddItem(FileSystemGroup, "Total Space", Format(Disk.SectorToBytes(Disk.BPB.DataRegionSize), "N0") & " bytes")
            .AddItem(FileSystemGroup, "Free Space", Format(Disk.FAT.GetFreeSpace(), "N0") & " bytes")

            If Disk.FAT.BadClusters.Count > 0 Then
                Dim SectorCount = Disk.FAT.BadClusters.Count * Disk.BPB.SectorsPerCluster
                Value = Format(Disk.FAT.BadClusters.Count * Disk.BPB.BytesPerCluster, "N0") & " bytes  (" & SectorCount & ")"
                .AddItem(FileSystemGroup, "Bad Sectors", Value, Color.Red)
            End If

            Dim LostClusters = Disk.RootDirectory.FATAllocation.LostClusters.Count
            If LostClusters > 0 Then
                Value = Format(LostClusters * Disk.BPB.BytesPerCluster, "N0") & " bytes  (" & LostClusters & ")"
                .AddItem(FileSystemGroup, "Lost Clusters", Value, Color.Red)
            End If

            Dim ReservedClusters = Disk.FAT.ReservedClusters.Count
            If ReservedClusters > 0 Then
                Value = Format(ReservedClusters * Disk.BPB.BytesPerCluster, "N0") & " bytes  (" & ReservedClusters & ")"
                .AddItem(FileSystemGroup, "Reserved Clusters", Value)
            End If

            If fsi.OldestFileDate IsNot Nothing Then
                .AddItem(FileSystemGroup, "Oldest Date", fsi.OldestFileDate.Value.ToString("yyyy-MM-dd  hh:mm tt"))
            End If

            If fsi.NewestFileDate IsNot Nothing Then
                .AddItem(FileSystemGroup, "Newest Date", fsi.NewestFileDate.Value.ToString("yyyy-MM-dd  hh:mm tt"))
            End If
        End With
    End Sub

    Private Sub PopulateSummaryPanelTitle(ListViewSummary As ListView, TitleData As FloppyDB.FloppyData)
        Dim MAxOffset As Integer = 40
        Dim Offset As Integer = 0
        Dim Value As String
        Dim ForeColor As Color
        Dim Name = TitleData.GetName
        Dim Variation = TitleData.GetVariation
        Dim Compilation = TitleData.GetCompilation
        Dim Publisher = TitleData.GetPublisher
        Dim Version = TitleData.GetVersion
        'If Variation <> "" Then
        ' Name &= " (" & Variation & ")"
        'End If

        With ListViewSummary
            Dim ColumnWidth As Integer = .Columns.Item(1).Width - 5
            Dim MaxWidth = ColumnWidth + MAxOffset

            Dim TitleGroup = .Groups.Add("Title", "Title")

            If Name <> "" Then
                Dim Status = TitleData.GetStatus
                If Status = FloppyDB.FloppyDBStatus.Verified Then
                    ForeColor = Color.Green
                ElseIf Status = FloppyDB.FloppyDBStatus.Modified Then
                    ForeColor = Color.Red
                Else
                    ForeColor = Color.Blue
                End If

                .AddItem(TitleGroup, "Title", Name, ForeColor, True, MaxWidth)
            End If
            If Variation <> "" Then
                .AddItem(TitleGroup, "Variant", Variation, False)
            End If
            If Compilation <> "" Then
                .AddItem(TitleGroup, "Compilation", Compilation, SystemColors.WindowText, True, MaxWidth)
            End If
            If Publisher <> "" Then
                .AddItem(TitleGroup, "Publisher", Publisher, SystemColors.WindowText, True, MaxWidth)
            End If
            Value = TitleData.GetYear
            If Value <> "" Then
                .AddItem(TitleGroup, "Year", Value, False)
            End If
            Value = TitleData.GetOperatingSystem
            If Value <> "" Then
                .AddItem(TitleGroup, "OS", Value, False)
            End If
            Value = TitleData.GetRegion
            If Value <> "" Then
                .AddItem(TitleGroup, "Region", Value, False)
            End If
            Value = TitleData.GetLanguage
            If Value <> "" Then
                .AddItem(TitleGroup, "Language", Value, False)
            End If
            If Version <> "" Then
                .AddItem(TitleGroup, "Version", Version, SystemColors.WindowText, True, MaxWidth)
            End If
            Value = TitleData.GetDisk
            If Value <> "" Then
                .AddItem(TitleGroup, "Disk", Value, False)
            End If
            If TitleData.CopyProtection <> "" Then
                .AddItem(TitleGroup, "Copy Protection", TitleData.CopyProtection)
            End If

            Dim TextWidth As Integer = ColumnWidth
            For Each Item As ListViewItem In TitleGroup.Items
                Value = Item.SubItems.Item(1).Text
                TextWidth = Math.Max(TextWidth, TextRenderer.MeasureText(Value, .Font).Width)
            Next
            TitleGroup.Tag = TextWidth - ColumnWidth
        End With
    End Sub

End Module