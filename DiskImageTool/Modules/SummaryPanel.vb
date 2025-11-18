Imports System.Collections.Specialized
Imports System.ComponentModel
Imports DiskImageTool.DiskImage
Imports BootSectorOffsets = DiskImageTool.DiskImage.BootSector.BootSectorOffsets
Imports BPBOffsets = DiskImageTool.DiskImage.BiosParameterBlock.BPBOoffsets

Public Class SummaryPanel
    Private WithEvents ContextMenuCopy As ContextMenuStrip
    Private WithEvents ListViewSummary As ListView
    Private Const COLUMN_WIDTH_NAME As Integer = 124
    Private Const CONTEXT_MENU_SUMMARY_KEY As String = "Summary"
    Private Const GROUP_BOOTRECORD As String = "BootRecord"
    Private Const GROUP_BOOTSTRAP As String = "Bootstrap"
    Private Const GROUP_DISK As String = "Disk"
    Private Const GROUP_FILE_SYSTEM As String = "FileSystem"
    Private Const GROUP_IMAGE As String = "Image"
    Private Const GROUP_TITLE As String = "Title"
    Private Const NULL_CHAR As Char = "�"
    Private ReadOnly _BootStrapDB As BootstrapDB
    Private ReadOnly _TitleDB As FloppyDB
    Private Column_Width_Value As Integer = 0
    Private TitleRows As OrderedDictionary = Nothing

    Public Sub New(ListViewSummary As ListView, TitleDB As FloppyDB, BootStrapDB As BootstrapDB)
        Me.ListViewSummary = ListViewSummary
        _TitleDB = TitleDB
        _BootStrapDB = BootStrapDB

        Initialize()

        ContextMenuCopy = New ContextMenuStrip With {
            .Name = CONTEXT_MENU_SUMMARY_KEY
        }
        ContextMenuCopy.Items.Add(My.Resources.Menu_CopyValue)
        Me.ListViewSummary.ContextMenuStrip = ContextMenuCopy
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        ListViewSummary = Nothing
        ContextMenuCopy = Nothing
    End Sub

    Public Sub Clear()
        ListViewSummary.Items.Clear()
    End Sub

    Public Sub Populate(CurrentImage As DiskImageContainer, MD5 As String)
        With ListViewSummary
            .BeginUpdate()
            .Items.Clear()
            .Groups.Clear()

            .Columns.Item(0).Width = COLUMN_WIDTH_NAME
            .Columns.Item(1).Width = Column_Width_Value

            If CurrentImage.Disk IsNot Nothing Then
                PopulateMain(CurrentImage.Disk, _TitleDB, _BootStrapDB, MD5)

                .HideSelection = False
                .TabStop = True
            Else
                PopulateError(CurrentImage.ImageData.InvalidImage)

                .HideSelection = True
                .TabStop = False
            End If

            .EndUpdate()
            .Refresh()
        End With
    End Sub

    Private Function GetBitRateColor(BitRate As Integer, Format As FloppyDiskFormat) As Color
        Dim ForeColor = SystemColors.WindowText

        BitRate = Bitstream.RoundBitRate(BitRate)

        Select Case Format
            Case FloppyDiskFormat.Floppy160, FloppyDiskFormat.Floppy180, FloppyDiskFormat.Floppy320, FloppyDiskFormat.Floppy360
                If BitRate = 300 Then
                    ForeColor = Color.Blue
                ElseIf BitRate <> 250 Then
                    ForeColor = Color.Red
                End If
            Case FloppyDiskFormat.Floppy720, FloppyDiskFormat.FloppyTandy2000
                If BitRate <> 250 Then
                    ForeColor = Color.Red
                End If
            Case FloppyDiskFormat.Floppy2880
                If BitRate <> 1000 Then
                    ForeColor = Color.Red
                End If
            Case FloppyDiskFormat.FloppyUnknown, FloppyDiskFormat.FloppyNoBPB
                '
            Case Else
                If BitRate <> 500 Then
                    ForeColor = Color.Red
                End If
        End Select

        Return ForeColor
    End Function

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

    Private Function GetFocusedItemName() As String
        If ListViewSummary IsNot Nothing AndAlso ListViewSummary.FocusedItem IsNot Nothing Then
            Dim Item = ListViewSummary.FocusedItem

            If Item.SubItems.Count > 1 Then
                Dim Text As String

                If Item.Tag Is Nothing Then
                    Text = Item.Text
                Else
                    Text = Item.Tag
                End If

                Return String.Format(My.Resources.Menu_CopyValueByName, Text)
            End If
        End If

        Return Nothing
    End Function

    Private Function GetFocusedItemValue() As String
        If ListViewSummary IsNot Nothing AndAlso ListViewSummary.FocusedItem IsNot Nothing Then
            Dim Item = ListViewSummary.FocusedItem.SubItems.Item(1)
            Dim Value As String

            If Item.Tag Is Nothing Then
                Value = Item.Text
            Else
                Value = Item.Tag
            End If

            Return Value
        End If

        Return Nothing
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

    Private Function GetRPMColor(RPM As Integer, Format As FloppyDiskFormat) As Color
        Dim ForeColor = SystemColors.WindowText

        Select Case Format
            Case FloppyDiskFormat.Floppy160, FloppyDiskFormat.Floppy180, FloppyDiskFormat.Floppy320, FloppyDiskFormat.Floppy360
                If RPM = 360 Then
                    ForeColor = Color.Blue
                ElseIf RPM <> 300 Then
                    ForeColor = Color.Red
                End If
            Case FloppyDiskFormat.Floppy1200
                If RPM <> 360 Then
                    ForeColor = Color.Red
                End If
            Case FloppyDiskFormat.FloppyUnknown, FloppyDiskFormat.FloppyNoBPB
                'Return True'
            Case Else
                If RPM <> 300 Then
                    ForeColor = Color.Red
                End If
        End Select

        Return ForeColor
    End Function

    Private Sub Initialize()
        With ListViewSummary
            .FullRowSelect = True
            .View = View.Details
            .HeaderStyle = ColumnHeaderStyle.None
            .HideSelection = False
            .MultiSelect = False
            .OwnerDraw = True
            .DoubleBuffer
            .AutoResizeColumnsContstrained(ColumnHeaderAutoResizeStyle.None)
            .Items.Clear()
            .Columns.Clear()
            .Columns.Add("", COLUMN_WIDTH_NAME, HorizontalAlignment.Left)
            Column_Width_Value = .ClientSize.Width - COLUMN_WIDTH_NAME - SystemInformation.VerticalScrollBarWidth
            .Columns.Add("", Column_Width_Value, HorizontalAlignment.Left)
        End With
    End Sub
    Private Sub InitializeTitleRows()
        If TitleRows Is Nothing Then
            TitleRows = New OrderedDictionary
        End If

        TitleRows.Clear()

        TitleRows.Add("Name", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_Title, True))
        TitleRows.Add("Variation", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_Variant, False))
        TitleRows.Add("Compilation", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_Compilation, True))
        TitleRows.Add("Publisher", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_Publisher, True))
        TitleRows.Add("Year", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_Year, False))
        TitleRows.Add("OperatingSystem", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_OperatingSystem, False))
        TitleRows.Add("Region", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_Region, False))
        TitleRows.Add("Language", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_Language, False))
        TitleRows.Add("Version", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_Version, True))
        TitleRows.Add("Disk", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_Disk, False))
        TitleRows.Add("CopyProtection", New SummaryRow(ListViewSummary.Font, My.Resources.SummaryPanel_CopyProtection, True))
    End Sub

    Private Sub PopulateError(InvalidImage As Boolean)
        With ListViewSummary
            Dim DiskGroup = .Groups.Add(GROUP_DISK, My.Resources.SummaryPanel_Disk)
            Dim Msg As String

            If InvalidImage Then
                Msg = My.Resources.SummaryPanel_InvalidFormat
            Else
                Msg = My.Resources.SummaryPanel_Error
            End If

            Dim Item As New ListViewItem("  " & Msg, DiskGroup) With {
                .ForeColor = Color.Red
            }
            .Items.Add(Item)
        End With
    End Sub

    Private Sub PopulateGroup(Group As ListViewGroup, Rows As OrderedDictionary)
        Dim MaxColumnWidth As Integer = 55

        With Group.ListView
            For Each SummaryRow As SummaryRow In Rows.Values
                If SummaryRow.Value <> "" Then
                    MaxColumnWidth = Math.Max(MaxColumnWidth, SummaryRow.TextWidth)
                End If
            Next

            Dim Diff = .Columns.Item(0).Width - MaxColumnWidth
            Dim ColumnWidth As Integer = .Columns.Item(1).Width - 6
            Dim MaxWidth = ColumnWidth + Diff

            For Each SummaryRow As SummaryRow In Rows.Values
                If SummaryRow.Value <> "" Then
                    If SummaryRow.WrapText Then
                        .AddItem(Group, SummaryRow.Text, SummaryRow.Value, SummaryRow.ForeColor, SummaryRow.WrapText, MaxWidth)
                    Else
                        .AddItem(Group, SummaryRow.Text, SummaryRow.Value, SummaryRow.ForeColor, SummaryRow.WrapText)
                    End If
                End If
            Next

            Dim TextWidth As Integer = ColumnWidth
            For Each Item As ListViewItem In Group.Items
                Dim Value = Item.SubItems.Item(1).Text
                TextWidth = Math.Max(TextWidth, TextRenderer.MeasureText(Value, .Font).Width)
            Next
            Group.Tag = TextWidth - ColumnWidth
        End With
    End Sub

    Private Sub PopulateGroupBootRecord(Disk As Disk, OEMNameResponse As OEMNameResponse)
        Dim Value As String
        Dim ForeColor As Color

        Dim DiskFormatBySize = FloppyDiskFormatGet(Disk.Image.Length)
        Dim BPBBySize = BuildBPB(DiskFormatBySize)
        Dim DoBPBCompare = Disk.DiskParams.Format = FloppyDiskFormat.FloppyUnknown And DiskFormatBySize <> FloppyDiskFormat.FloppyUnknown

        With ListViewSummary
            Dim BootRecordGroup = .Groups.Add(GROUP_BOOTRECORD, My.Resources.SummaryPanel_BootRecord)

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

            If Not Disk.BootSector.BPB.HasValidSectorsPerCluster(True) Then
                ForeColor = Color.Red
            ElseIf DoBPBCompare AndAlso Disk.BootSector.BPB.SectorsPerCluster <> BPBBySize.SectorsPerCluster Then
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

            If Disk.DiskParams.IsXDF Then
                Value = "1 + " & My.Resources.SummaryPanel_CompatibilityImage
            Else
                Value = Disk.BootSector.BPB.NumberOfFATs
                If Not Disk.FATTables.FATsMatch Then
                    Value &= " " & InParens(My.Resources.Label_Mismatched)
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
                    Value &= " " & InParens(My.Resources.Label_Invalid)
                    ForeColor = Color.Red
                ElseIf Disk.DiskParams.Format = FloppyDiskFormat.FloppyXDF35 AndAlso Disk.FAT.MediaDescriptor = &HF9 Then
                    'Do Nothing - This is normal for XDF
                ElseIf Disk.DiskParams.Format <> FloppyDiskFormat.FloppyUnknown AndAlso Disk.BootSector.BPB.MediaDescriptor <> Disk.DiskParams.BPBParams.MediaDescriptor Then
                    Value &= " " & InParens(My.Resources.Label_Mismatched)
                    ForeColor = Color.Red
                ElseIf Disk.FAT.MediaDescriptor <> Disk.BootSector.BPB.MediaDescriptor Then
                    Value &= " " & InParens(My.Resources.Label_Mismatched)
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

            If App.Globals.AppSettings.Debug Then
                If Not Disk.BootSector.CheckJumpInstruction(True, True) Then
                    ForeColor = Color.Red
                Else
                    ForeColor = SystemColors.WindowText
                End If
                .AddItem(BootRecordGroup, BootSectorDescription(BootSectorOffsets.JmpBoot), BitConverter.ToString(Disk.BootSector.JmpBoot), ForeColor)
            End If
        End With
    End Sub

    Private Sub PopulateGroupBootstrap(Disk As Disk, OEMNameResponse As OEMNameResponse)
        Dim ForeColor As Color

        With ListViewSummary
            Dim BootStrapGroup = .Groups.Add(GROUP_BOOTSTRAP, My.Resources.SummaryPanel_Bootstrap)

            If Not OEMNameResponse.NoBootLoader Then
                Dim BootStrapCRC32 = CRC32.ComputeChecksum(Disk.BootSector.BootStrapCode)
                .AddItem(BootStrapGroup, My.Resources.SummaryPanel_Bootstrap & " CRC32", BootStrapCRC32.ToString("X8"))
            End If

            If OEMNameResponse.Found Then
                If OEMNameResponse.Data.Language.Length > 0 Then
                    .AddItem(BootStrapGroup, My.Resources.SummaryPanel_Language, OEMNameResponse.Data.Language)
                End If

                Dim OEMName = OEMNameResponse.MatchedOEMName

                If OEMName Is Nothing And OEMNameResponse.Data.OEMNames.Count = 1 Then
                    OEMName = OEMNameResponse.Data.OEMNames(0)
                End If

                If OEMName IsNot Nothing Then
                    If OEMName.Company <> "" Then
                        .AddItem(BootStrapGroup, My.Resources.SummaryPanel_Company, OEMName.Company)
                    End If
                    If OEMName.Description <> "" Then
                        .AddItem(BootStrapGroup, My.Resources.SummaryPanel_Description, OEMName.Description)
                    End If
                    If OEMName.Note <> "" Then
                        .AddItem(BootStrapGroup, My.Resources.SummaryPanel_Note, OEMName.Note, Color.Blue)
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
                                .AddItem(BootStrapGroup, My.Resources.SummaryPanel_AltOEMName, OEMName.GetNameAsString, ForeColor)
                            End If
                        Next
                    End If
                End If
            End If
        End With
    End Sub

    Private Function PopulateGroupDisk(Disk As Disk) As ListViewGroup
        Dim DiskGroup As ListViewGroup
        Dim Value As String
        Dim ForeColor As Color

        With ListViewSummary
            DiskGroup = .Groups.Add(GROUP_DISK, My.Resources.SummaryPanel_Disk)

            .AddItem(DiskGroup, My.Resources.SummaryPanel_ImageType, GetImageTypeName(Disk.Image.ImageType))

            If Disk.Image.ImageType = FloppyImageType.BasicSectorImage Then
                If Disk.IsValidImage AndAlso Disk.CheckImageSize <> 0 Then
                    ForeColor = Color.Red
                Else
                    ForeColor = SystemColors.WindowText
                End If
                .AddItem(DiskGroup, My.Resources.SummaryPanel_ImageSize, Disk.Image.Length.ToString("N0"), ForeColor)
            End If

            If Disk.IsValidImage(False) Then
                Dim DiskFormatString = FloppyDiskFormatGetName(Disk.DiskParams.Format)
                Dim DiskFormatBySize = FloppyDiskFormatGet(Disk.Image.Length)

                If Disk.DiskParams.Format <> FloppyDiskFormat.FloppyUnknown Or DiskFormatBySize = FloppyDiskFormat.FloppyUnknown Then
                    Value = String.Format(My.Resources.Label_Floppy, DiskFormatString)
                Else
                    Dim DiskFormatStringBySize = FloppyDiskFormatGetName(DiskFormatBySize)
                    Value = String.Format(My.Resources.Label_Floppy, DiskFormatStringBySize) & " " & InParens(My.Resources.SummaryPanel_CustomFormat)
                End If
                .AddItem(DiskGroup, My.Resources.SummaryPanel_DiskFormat, Value)

                If Disk.DiskParams.IsXDF Then
                    Dim XDFChecksum = CalcXDFChecksum(Disk.Image.GetBytes, Disk.BPB.SectorsPerFAT)
                    If XDFChecksum = Disk.GetXDFChecksum Then
                        ForeColor = Color.Green
                    Else
                        ForeColor = Color.Red
                    End If
                    .AddItem(DiskGroup, My.Resources.SummaryPanel_XDFChecksum, XDFChecksum.ToString("X8"), ForeColor)
                End If

                If Disk.BPB.IsValid AndAlso Disk.CheckImageSize > 0 AndAlso Disk.DiskParams.Format <> FloppyDiskFormat.FloppyUnknown Then
                    .AddItem(DiskGroup, DiskFormatString & " CRC32", HashFunctions.CRC32Hash(Disk.Image.GetBytes(0, Disk.BPB.ReportedImageSize())))
                End If
            End If

            If Disk.Image.ImageType = FloppyImageType.TranscopyImage Then
                Dim Image As ImageFormats.TC.TransCopyImage = DirectCast(Disk.Image, ImageFormats.TC.TranscopyFloppyImage).Image
                Value = ImageFormats.TC.DiskTypeToString(Image.DiskType)
                .AddItem(DiskGroup, My.Resources.SummaryPanel_DiskType, Value)
            End If

            If Disk.Image.ImageType <> FloppyImageType.BasicSectorImage Then
                .AddItem(DiskGroup, My.Resources.SummaryPanel_Tracks, Disk.Image.TrackCount)
                .AddItem(DiskGroup, My.Resources.SummaryPanel_Heads, Disk.Image.SideCount)
            End If

            If Disk.Image.IsBitstreamImage Then
                If Disk.Image.BitstreamImage.TrackStep <> 1 Then
                    .AddItem(DiskGroup, My.Resources.SummaryPanel_TrackStep, Disk.Image.BitstreamImage.TrackStep)
                End If
                If Disk.Image.BitstreamImage.VariableBitRate Then
                    .AddItem(DiskGroup, My.Resources.SummaryPanel_Bitrate, My.Resources.Label_Variable)
                ElseIf Disk.Image.BitstreamImage.BitRate <> 0 Then
                    ForeColor = GetBitRateColor(Disk.Image.BitstreamImage.BitRate, Disk.DiskParams.Format)
                    .AddItem(DiskGroup, My.Resources.SummaryPanel_Bitrate, Disk.Image.BitstreamImage.BitRate, ForeColor)
                End If

                If Disk.Image.BitstreamImage.VariableRPM Then
                    .AddItem(DiskGroup, My.Resources.SummaryPanel_RPM, My.Resources.Label_Variable)
                ElseIf Disk.Image.BitstreamImage.RPM <> 0 Then
                    ForeColor = GetRPMColor(Disk.Image.BitstreamImage.RPM, Disk.DiskParams.Format)
                    .AddItem(DiskGroup, My.Resources.SummaryPanel_RPM, Disk.Image.BitstreamImage.RPM, ForeColor)
                End If
            End If

            If Disk.Image.ImageType = FloppyImageType.D86FImage Then
                Dim Image As ImageFormats.D86F.D86FImage = DirectCast(Disk.Image, ImageFormats.D86F.D86FFloppyImage).Image
                If Image.RPMSlowDown <> 0 Then
                    If Image.AlternateBitcellCalculation Then
                        Value = My.Resources.SummaryPanel_SpeedUp & " " & (Image.RPMSlowDown * 100) & "%"
                    Else
                        Value = My.Resources.SummaryPanel_SlowDown & " " & (Image.RPMSlowDown * 100) & "%"
                    End If
                    .AddItem(DiskGroup, My.Resources.SummaryPanel_RPMAdjustment, Value)
                End If
                .AddItem(DiskGroup, My.Resources.SummaryPanel_SurfaceData, If(Image.HasSurfaceData, My.Resources.Label_Yes, My.Resources.Label_No))

            ElseIf Disk.Image.HasWeakBitsSupport Then
                .AddItem(DiskGroup, My.Resources.SummaryPanel_WeakBits, If(Disk.Image.HasWeakBits, My.Resources.Label_Yes, My.Resources.Label_No))
            End If

            If Disk.Image.NonStandardTracks.Count > 0 Or Disk.Image.AdditionalTracks.Count > 0 Then
                Dim TrackList As New HashSet(Of UShort)(Disk.Image.NonStandardTracks.Concat(Disk.Image.AdditionalTracks))
                .AddItem(DiskGroup, My.Resources.SummaryPanel_NonStandardTracks, GetNonStandardTrackList(TrackList, Disk.Image.SideCount))
            End If
        End With

        Return DiskGroup
    End Function
    Private Sub PopulateGroupFileSystem(Disk As Disk)
        Dim Value As String
        Dim ForeColor As Color

        With ListViewSummary
            Dim FileSystemGroup = .Groups.Add(GROUP_FILE_SYSTEM, My.Resources.SummaryPanel_FileSystem)

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
                    Value &= " " & InParens(My.Resources.Label_Invalid)
                    ForeColor = Color.Red
                    Visible = True
                ElseIf Disk.DiskParams.Format = FloppyDiskFormat.FloppyXDF35 AndAlso Disk.FAT.MediaDescriptor = &HF9 Then
                    Visible = False
                ElseIf Disk.DiskParams.Format <> FloppyDiskFormat.FloppyUnknown AndAlso Disk.FAT.MediaDescriptor <> Disk.DiskParams.BPBParams.MediaDescriptor Then
                    Value &= " " & InParens(My.Resources.Label_Mismatched)
                    ForeColor = Color.Red
                    Visible = True
                End If

                If Visible Then
                    .AddItem(FileSystemGroup, My.Resources.SummaryPanel_MediaDescriptor, Value, ForeColor)
                End If
            End If

            Dim fsi = GetFileSystemInfo(Disk)

            If fsi.VolumeLabel IsNot Nothing Then
                .AddItem(FileSystemGroup, My.Resources.SummaryPanel_VolumeLabel, fsi.VolumeLabel.GetVolumeName.TrimEnd(NULL_CHAR))
                Dim VolumeDate = fsi.VolumeLabel.GetLastWriteDate
                .AddItem(FileSystemGroup, My.Resources.SummaryPanel_VolumeDate, VolumeDate.ToString(True, True, False, True))
            End If

            .AddItem(FileSystemGroup, My.Resources.SummaryPanel_TotalSpace, FormatThousands(Disk.BPB.SectorToBytes(Disk.BPB.DataRegionSize)) & " " & My.Resources.Label_Bytes)
            .AddItem(FileSystemGroup, My.Resources.SummaryPanel_FreeSpace, FormatThousands(Disk.FAT.GetFreeSpace()) & " " & My.Resources.Label_Bytes)

            If Disk.FAT.BadClusters.Count > 0 Then
                Dim SectorCount = Disk.FAT.BadClusters.Count * Disk.BPB.SectorsPerCluster
                Value = FormatThousands(Disk.FAT.BadClusters.Count * Disk.BPB.BytesPerCluster) & " " & My.Resources.Label_Bytes & "  " & InParens(SectorCount)
                .AddItem(FileSystemGroup, My.Resources.SummaryPanel_BadSectors, Value, Color.Red)
            End If

            Dim LostClusters = Disk.RootDirectory.FATAllocation.LostClusters.Count
            If LostClusters > 0 Then
                Value = FormatThousands(LostClusters * Disk.BPB.BytesPerCluster) & " " & My.Resources.Label_Bytes & "  " & InParens(LostClusters)
                .AddItem(FileSystemGroup, My.Resources.SummaryPanel_LostClusters, Value, Color.Red)
            End If

            Dim ReservedClusters = Disk.FAT.ReservedClusters.Count
            If ReservedClusters > 0 Then
                Value = FormatThousands(ReservedClusters * Disk.BPB.BytesPerCluster) & " " & My.Resources.Label_Bytes & "  " & InParens(ReservedClusters)
                .AddItem(FileSystemGroup, My.Resources.SummaryPanel_ReservedClusters, Value)
            End If

            If fsi.OldestFileDate IsNot Nothing Then
                .AddItem(FileSystemGroup, My.Resources.SummaryPanel_OldestDate, fsi.OldestFileDate.Value.ToString(My.Resources.IsoDateTimeFormat))
            End If

            If fsi.NewestFileDate IsNot Nothing Then
                .AddItem(FileSystemGroup, My.Resources.SummaryPanel_NewestDate, fsi.NewestFileDate.Value.ToString(My.Resources.IsoDateTimeFormat))
            End If
        End With
    End Sub
    Private Sub PopulateGroupTitle(TitleData As FloppyDB.FloppyData)
        If TitleRows Is Nothing Then
            InitializeTitleRows()
        End If

        Dim Row = Function(key As String) DirectCast(TitleRows(key), SummaryRow)

        Dim Status = TitleData.GetStatus
        Dim ForeColor As Color
        If Status = FloppyDB.FloppyDBStatus.Verified Then
            ForeColor = Color.Green
        ElseIf Status = FloppyDB.FloppyDBStatus.Modified Then
            ForeColor = Color.Red
        Else
            ForeColor = Color.Blue
        End If

        With Row("Name")
            .Value = TitleData.GetName
            .ForeColor = ForeColor
        End With
        Row("Variation").Value = TitleData.GetVariation
        Row("Compilation").Value = TitleData.GetCompilation
        Row("Publisher").Value = TitleData.GetPublisher
        Row("Year").Value = TitleData.GetYear
        Row("OperatingSystem").Value = TitleData.GetOperatingSystem
        Row("Region").Value = TitleData.GetRegion
        Row("Language").Value = TitleData.GetLanguage
        Row("Version").Value = TitleData.GetVersion
        Row("Disk").Value = TitleData.GetDisk
        Row("CopyProtection").Value = TitleData.CopyProtection

        Dim Group = ListViewSummary.Groups.Add(GROUP_TITLE, My.Resources.SummaryPanel_Title)

        PopulateGroup(Group, TitleRows)
    End Sub

    Private Sub PopulateMain(Disk As Disk, TitleDB As FloppyDB, BootStrapDB As BootstrapDB, MD5 As String)
        Dim TitleFound As Boolean = False

        If App.Globals.AppSettings.DisplayTitles AndAlso TitleDB.TitleCount > 0 Then
            Dim TitleFindResult = TitleDB.TitleFind(MD5)
            If TitleFindResult.TitleData IsNot Nothing Then
                TitleFound = True
                PopulateGroupTitle(TitleFindResult.TitleData)
            End If
        End If

        Dim DiskGroup = PopulateGroupDisk(Disk)

        With ListViewSummary
            If Not Disk.IsValidImage Then
                .AddItem(DiskGroup, My.Resources.SummaryPanel_FileSystem, My.Resources.Caption_Unknown, Color.Red)
            Else
                Dim OEMNameResponse = BootStrapDB.CheckOEMName(Disk.BootSector)

                If OEMNameResponse.NoBootLoader Then
                    If Disk.BootSector.CheckJumpInstruction(False, True) Then
                        .AddItem(DiskGroup, My.Resources.SummaryPanel_Bootstrap, My.Resources.SummaryPanel_NoBootLoader, Color.Red)
                    Else
                        .AddItem(DiskGroup, My.Resources.SummaryPanel_Bootstrap, My.Resources.SummaryPanel_CustomBootLoader, Color.Red)
                    End If
                ElseIf Not Disk.BootSector.BPB.IsValid Then
                    .AddItem(DiskGroup, My.Resources.SummaryPanel_BootRecord, My.Resources.SummaryPanel_NoBPB, Color.Red)
                End If

                If Not Disk.BootSector.BPB.IsValid Then
                    If Not Disk.FATTables.FATsMatch Then
                        .AddItem(DiskGroup, My.Resources.SummaryPanel_FAT, My.Resources.Label_Mismatched, Color.Red)
                    End If
                End If

                If Disk.BootSector.BPB.IsValid Then
                    PopulateGroupBootRecord(Disk, OEMNameResponse)
                End If

                PopulateGroupFileSystem(Disk)
                PopulateGroupBootstrap(Disk, OEMNameResponse)
            End If
        End With
    End Sub

#Region "Events"

    Private Sub ContextMenuCopy_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ContextMenuCopy.ItemClicked
        Dim Value = GetFocusedItemValue()

        If Value IsNot Nothing Then
            Clipboard.SetText(Value)
        End If
    End Sub

    Private Sub ContextMenuCopy_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuCopy.Opening
        Dim Name = GetFocusedItemName()

        If Name IsNot Nothing Then
            Dim CM As ContextMenuStrip = sender
            CM.Items(0).Text = Name
        Else
            e.Cancel = True
        End If
    End Sub

    Private Sub ListView_DrawItem(sender As Object, e As DrawListViewItemEventArgs) Handles ListViewSummary.DrawItem
        e.DrawDefault = True
    End Sub

    Private Sub ListView_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles ListViewSummary.DrawSubItem
        If e.Item.Group IsNot Nothing AndAlso e.Item.Group.Tag IsNot Nothing Then
            Dim Offset As Integer = CInt(e.Item.Group.Tag)
            If Offset <> 0 Then
                e.DrawBackground()
                Dim rect As Rectangle = Rectangle.Inflate(e.Bounds, -3, -2)
                If e.ColumnIndex = 0 Then
                    rect.Width -= Offset
                Else
                    rect.X -= Offset
                    rect.Width += Offset
                End If
                TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.SubItem.Font, rect, e.SubItem.ForeColor, TextFormatFlags.Default Or TextFormatFlags.NoPrefix)
            Else
                e.DrawDefault = True
            End If
        Else
            e.DrawDefault = True
        End If
    End Sub

    Private Sub ListView_ItemSelectionChanged(sender As Object, e As ListViewItemSelectionChangedEventArgs) Handles ListViewSummary.ItemSelectionChanged
        e.Item.Selected = False
    End Sub


    'Private Sub ListView_Resize(sender As Object, e As EventArgs) Handles ListView.Resize
    '    If ListView.Columns.Count < 2 Then
    '        Exit Sub
    '    End If

    '    ListView.Columns.Item(1).Width = ListView.ClientSize.Width - ListView.Columns.Item(0).Width
    'End Sub
#End Region

    Private Structure FileSystemInfo
        Dim NewestFileDate As Date?
        Dim OldestFileDate As Date?
        Dim VolumeLabel As DirectoryEntry
    End Structure

    Private Class SummaryRow
        Public Sub New(Font As Font, Text As String, WrapText As Boolean)
            _Text = Text
            _TextWidth = TextRenderer.MeasureText(Text, Font).Width
            _WrapText = WrapText
        End Sub

        Public Property ForeColor As Color = SystemColors.WindowText
        Public Property Text As String
        Public Property TextWidth As Integer
        Public Property Value As String = ""
        Public Property WrapText As Boolean
    End Class
End Class