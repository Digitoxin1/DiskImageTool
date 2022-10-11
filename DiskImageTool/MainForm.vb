Imports System.Security.Cryptography
Imports System.Text
Public Enum FilterTypes
    UnknownOEMID = 1
    MismatchedOEMID = 2
    HasCreated = 4
    HasLastAccessed = 8
    HasInvalidImage = 16
    HasLongFileNames = 32
End Enum

Public Class MainForm
    Private ReadOnly _FileHashTable As Dictionary(Of UInteger, FileData)
    Private _Disk As DiskImage.Disk
    Private _VolumeLabel As String = ""
    Private _InternalDrop As Boolean
    Private ReadOnly _BootstrapTypes As Dictionary(Of UInteger, BootstrapType)
    Private _SuppressEvent As Boolean = False
    Private _CachedListItems As List(Of ComboFileItem)

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _FileHashTable = New Dictionary(Of UInteger, FileData)
        _InternalDrop = False
        _BootstrapTypes = InitBootstrapTypes()
        FlowLayoutPanel1.Visible = False
        BtnClearCreated.Enabled = False
        BtnClearCreated.Visible = False
        BtnClearLastAccessed.Enabled = False
        BtnClearLastAccessed.Visible = False
        BtnScan.Enabled = False
        BtnFilters.Enabled = False
        CBCheckAll.Visible = False
        LblFilterMessage.Visible = False
    End Sub

    Private Function InitBootstrapTypes() As Dictionary(Of UInteger, BootstrapType)
        Dim Result As New Dictionary(Of UInteger, BootstrapType)
        With Result
            .Add(&HC6CE8A87&, GetBootstrapType("..DOS4.0", "English"))
            .Add(&H62084C99&, GetBootstrapType("688AS EA", "English"))
            .Add(&H231A83E&, GetBootstrapType("ALF  2.0", "English"))
            .Add(&HB4140C43&, GetBootstrapType("ALF  3.0", "English"))
            .Add(&H7096EE00&, GetBootstrapType("ALF  3.0", "English"))
            .Add(&HE981E850&, GetBootstrapType("ALF  3.0", "English"))
            .Add(&H20AE7ABE&, GetBootstrapType("C ZDS3.3", "English"))
            .Add(&HD7FB8823&, GetBootstrapType("CH-FOR15", "English"))
            .Add(&HC91B0027&, GetBootstrapType("CH-FOR16", "English"))
            .Add(&H69C11AA5&, GetBootstrapType("CH-FOR18", "English"))
            .Add(&HCE2D6ECF&, GetBootstrapType("CMAST2.0", "English"))
            .Add(&H6E55822D&, GetBootstrapType("FORMATQM", "English"))
            .Add(&H5EB98F7B&, GetBootstrapType("IBM  2.0", "English"))
            .Add(&H58BB4185&, GetBootstrapType("IBM  3.0", "English"))
            .Add(&H440CF965&, GetBootstrapType("IBM  3.1", "English"))
            .Add(&H148EBE9E&, GetBootstrapType("IBM  3.1", "English"))
            .Add(&H3CD5390D&, GetBootstrapType("IBM  3.1", "English"))
            .Add(&H7AD69436&, GetBootstrapType("IBM  3.2,IBMMS3.2", "English"))
            .Add(&H66660092&, GetBootstrapType("IBM  3.2", "English"))
            .Add(&HA6003920&, GetBootstrapType("IBM  3.2", "English"))
            .Add(&H96B7BEAA&, GetBootstrapType("IBM  3.2", "German"))
            .Add(&H18BA4260&, GetBootstrapType("IBM  3.3", "English"))
            .Add(&HA813EDA9&, GetBootstrapType("IBM  3.3", "English"))
            .Add(&HFC1650BE&, GetBootstrapType("IBM  3.3", "English"))
            .Add(&HE7E28CB3&, GetBootstrapType("IBM  3.3", "English"))
            .Add(&HBBB8D363&, GetBootstrapType("IBM  3.3", "English"))
            .Add(&HD5A08E3B&, GetBootstrapType("IBM  3.3,KEY DISK,MSDOS3.3", "English"))
            .Add(&H50EC1B86&, GetBootstrapType("IBM  3.3", "French"))
            .Add(&HC63EA411&, GetBootstrapType("IBM  3.3", "French"))
            .Add(&H55957DBE&, GetBootstrapType("IBM  3.3", "German"))
            .Add(&H90E549B9&, GetBootstrapType("IBM  3.3", "German"))
            .Add(&HFD8B56C&, GetBootstrapType("IBM  3.3", "German"))
            .Add(&HCA5FCC71&, GetBootstrapType("IBM  4.0,IBMMS4.0", "English"))
            .Add(&H47E20088&, GetBootstrapType("IBM  4.1,IBM 4.01", "English"))
            .Add(&H3F6176DF&, GetBootstrapType("IBM  5.0", "English"))
            .Add(&HA157CE&, GetBootstrapType("IBM  5.0", "English"))
            .Add(&H42705487&, GetBootstrapType("IBM  5.0", "French"))
            .Add(&H1E9133C3&, GetBootstrapType("IBM  5.0", "German"))
            .Add(&HC5584353&, GetBootstrapType("IBM 10.2", "English"))
            .Add(&H19410C4&, GetBootstrapType("IBM 20.0", "English"))
            .Add(&HDC004D1A&, GetBootstrapType("IBM 20.0", "English"))
            .Add(&H4495837A&, GetBootstrapType("IBM PNCI", "English"))
            .Add(&H4E1A74D4&, GetBootstrapType("IBM PNCI", "English"))
            .Add(&HFA938DDA&, GetBootstrapType("IBM PNCI", "English"))
            .Add(&HC53FAC5A&, GetBootstrapType("KEY DISK", "English"))
            .Add(&HF47AF861&, GetBootstrapType("LHX TWO ", "English"))
            .Add(&H8CD8F888&, GetBootstrapType("MagicISO", "English"))
            .Add(&H9B0E679E&, GetBootstrapType("MSC 3.1 ", "English"))
            .Add(&HEB85220D&, GetBootstrapType("MSDMF3.2", "English"))
            .Add(&H2D34342E&, GetBootstrapType("MSDOS3.2", "English"))
            .Add(&H63DCC6E5&, GetBootstrapType("MSDOS3.2", "English"))
            .Add(&H58FAE0BA&, GetBootstrapType("MSDOS3.2", "English"))
            .Add(&HD7CDA53E&, GetBootstrapType("MSDOS3.2", "English"))
            .Add(&H7186F853&, GetBootstrapType("MSDOS3.2", "French"))
            .Add(&H461FE7DE&, GetBootstrapType("MSDOS3.2", "French"))
            .Add(&H6EA1666A&, GetBootstrapType("MSDOS3.2", "German"))
            .Add(&HB3C42CAD&, GetBootstrapType("MSDOS3.3,INSIGNIA,I93 1.00", "English"))
            .Add(&H17120C24&, GetBootstrapType("MSDOS3.3", "English"))
            .Add(&H5045D72B&, GetBootstrapType("MSDOS3.3", "English"))
            .Add(&HF8BD5277&, GetBootstrapType("MSDOS3.3", "French"))
            .Add(&HBA7919EB&, GetBootstrapType("MSDOS3.3", "German"))
            .Add(&HDB11BCE5&, GetBootstrapType("MSDOS4.0", "English"))
            .Add(&H70386805&, GetBootstrapType("MSDOS4.0", "English"))
            .Add(&H4767DBA2&, GetBootstrapType("MSDOS4.0", "French"))
            .Add(&HFE944220&, GetBootstrapType("MSDOS5.0,MSDBL6.0,MSDSP6.0", "English"))
            .Add(&H93282B6B&, GetBootstrapType("MSDOS5.0", "English"))
            .Add(&HD3C0FBEF&, GetBootstrapType("MSDOS5.0", "French"))
            .Add(&H7E3C7397&, GetBootstrapType("MSDOS5.0", "French"))
            .Add(&H5D696A06&, GetBootstrapType("MSDOS5.0", "German"))
            .Add(&HDC98D56D&, GetBootstrapType("MSDOS5.0", "Italian"))
            .Add(&HEEB2E10D&, GetBootstrapType("MSDOS5.0", "Spanish"))
            .Add(&H6A18BCFC&, GetBootstrapType("MSDOS5.0", "Spanish"))
            .Add(&H1D1B49D&, GetBootstrapType("MSDOS6.0", "English"))
            .Add(&HA975B7F6&, GetBootstrapType("MSWIN4.0", "English"))
            .Add(&H342A7190&, GetBootstrapType("NECIS3.3", "English"))
            .Add(&H3E12D96&, GetBootstrapType("NWDOS7.0", "German"))
            .Add(&H74259F76&, GetBootstrapType("Olivetti", "English"))
            .Add(&HABF90357&, GetBootstrapType("PC Tools,UBI-SOFT", "English"))
            .Add(&H373BC7C4&, GetBootstrapType("PC Tools", "English"))
            .Add(&H4CEA4F0B&, GetBootstrapType("PC Tools", "English"))
            .Add(&H61AEADE2&, GetBootstrapType("PC Tools", "French"))
            .Add(&H23318A0D&, GetBootstrapType("PCFormat", "English"))
            .Add(&HDE92B409&, GetBootstrapType("PCFormat", "English"))
            .Add(&H10739392&, GetBootstrapType("PCFormat", "English"))
            .Add(&H8BE98DEC&, GetBootstrapType("ProCopy ", "English"))
            .Add(&H6D055CC4&, GetBootstrapType("PSA  3.1", "English"))
            .Add(&HD2318C04&, GetBootstrapType("PSA  3.2", "English"))
            .Add(&HFBF4FA19&, GetBootstrapType("PSA  3.2", "English"))
            .Add(&HE32BB64C&, GetBootstrapType("S.BERMAN", "English"))
            .Add(&H7BC949F3&, GetBootstrapType("T V2.11 ", "English"))
            .Add(&HC1C6ECB8&, GetBootstrapType("T V4.00 ", "English"))
            .Add(&H577983D2&, GetBootstrapType("TAN  3.2", "English"))
            .Add(&H847631E8&, GetBootstrapType("TAN  3.3", "English"))
            .Add(&HC82880D8&, GetBootstrapType("TAN  3.3", "English"))
            .Add(&H69B1B489&, GetBootstrapType("Tandy557", "English"))
            .Add(&HA26247F1&, GetBootstrapType("TracerST", "English"))
            .Add(&HCA33CAD&, GetBootstrapType("VGACPY51", "English"))
            .Add(&H1F29F8C6&, GetBootstrapType("VGACP625", "English"))
            .Add(&HB9067457&, GetBootstrapType("WINIMAGE", "English"))
            .Add(&H78E72263&, GetBootstrapType("WINIMAGE", "English"))
            .Add(&HCB438B3C&, GetBootstrapType("XTF-BOOT", "English"))
            .Add(&H7E03A60A&, GetBootstrapType("XTREEWIN", "English"))
        End With

        Return Result
    End Function

    Private Function IsFiltered(File As ComboFileItem, AppliedFilters As FilterTypes) As Boolean
        If (AppliedFilters And FilterTypes.HasInvalidImage) > 0 Then
            If Not File.IsValidImage Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.HasCreated) > 0 Then
            If File.IsValidImage And File.HasCreated Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.HasLastAccessed) > 0 Then
            If File.IsValidImage And File.HasLastAccessed Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.MismatchedOEMID) > 0 Then
            If File.IsValidImage And File.OEMIDFound And Not File.OEMIDMatched Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.UnknownOEMID) > 0 Then
            If File.IsValidImage And Not File.OEMIDFound Then
                Return False
            End If
        End If

        If (AppliedFilters And FilterTypes.HasLongFileNames) > 0 Then
            If File.IsValidImage And File.HasLongFileNames Then
                Return False
            End If
        End If

        Return True
    End Function

    Private Sub ApplyFilters(e As ItemCheckEventArgs)
        Dim Count As Integer = ListFilters.CheckedItems.Count
        Dim IsChecked As Boolean

        If e.NewValue = CheckState.Checked And e.CurrentValue = CheckState.Unchecked Then
            Count += 1
        ElseIf e.NewValue = CheckState.Unchecked And e.CurrentValue = CheckState.Checked Then
            Count -= 1
        End If

        If Count > 0 Then
            Me.Cursor = Cursors.WaitCursor

            If _CachedListItems Is Nothing Then
                CacheComboGroups()
            End If
            ComboGroups.Items.Clear()
            Dim AppliedFilters As FilterTypes = 0
            For Counter = 0 To ListFilters.Items.Count - 1
                If Counter = e.Index Then
                    IsChecked = (e.NewValue = CheckState.Checked)
                Else
                    IsChecked = ListFilters.GetItemChecked(Counter)
                End If
                If IsChecked Then
                    Dim Item As ComboFileType = ListFilters.Items(Counter)
                    AppliedFilters += Item.ID
                End If
            Next
            For Each File In _CachedListItems
                If Not IsFiltered(File, AppliedFilters) Then
                    ComboGroups.Items.Add(File)
                End If
            Next
            If ComboGroups.Items.Count > 0 Then
                ComboGroups.SelectedIndex = 0
            End If
            BtnFilters.BackColor = Color.LightGreen

            LblFilterMessage.Text = ComboGroups.Items.Count & " of " & _CachedListItems.Count & " files"
            LblFilterMessage.Visible = True

            Me.Cursor = Cursors.Default
        Else
            If _CachedListItems IsNot Nothing Then
                RestoreComboGroups()
            End If
            BtnFilters.BackColor = SystemColors.Control
            BtnFilters.UseVisualStyleBackColor = True
        End If
    End Sub

    Private Sub CacheComboGroups()
        _CachedListItems = New List(Of ComboFileItem)
        For Counter = 0 To ComboGroups.Items.Count - 1
            _CachedListItems.Add(ComboGroups.Items(Counter))
        Next
    End Sub

    Private Sub RestoreComboGroups()
        Me.Cursor = Cursors.WaitCursor

        ComboGroups.Items.Clear()
        For Each File In _CachedListItems
            ComboGroups.Items.Add(File)
        Next
        _CachedListItems = Nothing
        If ComboGroups.Items.Count > 0 Then
            ComboGroups.SelectedIndex = 0
        End If
        LblFilterMessage.Visible = False

        Me.Cursor = Cursors.Default
    End Sub

    Private Function ChangeOEMID() As Boolean
        Dim frmOEMID As New OEMIDForm(_Disk, _BootstrapTypes)
        Dim Result As Boolean

        frmOEMID.ShowDialog()

        Result = frmOEMID.Result

        If Result Then
            ComboItemSetModified(False)
        End If

        Return Result
    End Function

    Private Sub ComboItemSetModified(FullRefresh As Boolean)
        Dim Item As ComboFileItem = ComboGroups.SelectedItem
        Item.Modified = True
        Item.Disk = _Disk
        _SuppressEvent = True
        ComboGroups.Items(ComboGroups.SelectedIndex) = ComboGroups.Items(ComboGroups.SelectedIndex)
        _SuppressEvent = False
        If FullRefresh Then
            PopulateDetails()
        End If
        PopulateSummary()
        BtnSave.Enabled = True
    End Sub

    Private Function ClearCreatedDate() As Boolean
        Dim Result As Boolean = False

        For Each Item As ListViewItem In ListViewFiles.CheckedItems
            Dim FileData = _FileHashTable(Item.GetHashCode)
            If FileData.HasCreated Then
                Dim File = _Disk.GetDirectoryEntryByOffset(FileData.Offset)
                File.ClearCreationDate()
                Item.SubItems.Item("FileCreateDate").Text = ""
                Result = True
            End If
        Next

        If Result Then
            ComboItemSetModified(False)
        End If

        Return Result
    End Function

    Private Function ClearLastAccessedDate() As Boolean
        Dim Result As Boolean = False

        For Each Item As ListViewItem In ListViewFiles.CheckedItems
            Dim FileData = _FileHashTable(Item.GetHashCode)
            If FileData.HasLastAccessed Then
                Dim File = _Disk.GetDirectoryEntryByOffset(FileData.Offset)
                File.ClearLastAccessDate()
                Item.SubItems.Item("FileLastAccessDate").Text = ""
                Result = True
            End If
        Next

        If Result Then
            ComboItemSetModified(False)
        End If

        Return Result
    End Function

    Private Function GetBootstrapType(OSName As String, Language As String) As BootstrapType
        Dim BootstrapType As New BootstrapType
        With BootstrapType
            .OSNames = Split(OSName, ",").ToList
            .Language = Language
        End With

        Return BootstrapType
    End Function

    Private Sub DragDropSelectedFiles()
        If ListViewFiles.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        Dim TempPath As String = System.IO.Path.GetTempPath() & Guid.NewGuid().ToString() & "\"
        For Each Item As ListViewItem In ListViewFiles.SelectedItems
            Dim FileData = _FileHashTable(Item.GetHashCode)
            Dim File = _Disk.GetDirectoryEntryByOffset(FileData.Offset)
            If Not File.IsDeleted And Not File.IsDirectory And Not File.IsVolumeName And Not File.HasInvalidFileSize And Not File.HasInvalidFilename And Not File.HasInvalidExtension Then
                Dim FilePath = System.IO.Path.Combine(TempPath, FileData.FilePath, File.GetFileName)
                If Not System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(FilePath)) Then
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath))
                End If
                System.IO.File.WriteAllBytes(FilePath, File.GetContent)
                Dim D = File.GetLastWriteDate
                If D.IsValidDate Then
                    System.IO.File.SetLastWriteTime(FilePath, D.DateObject)
                End If
            End If
        Next

        If System.IO.Directory.Exists(TempPath) Then
            Dim FileList = System.IO.Directory.EnumerateDirectories(TempPath)
            For Each FilePath In System.IO.Directory.GetFiles(TempPath)
                FileList = FileList.Append(FilePath)
            Next
            If FileList.Count > 0 Then
                Dim Data = New DataObject(DataFormats.FileDrop, FileList.ToArray)
                ListViewFiles.DoDragDrop(Data, DragDropEffects.Copy)
            End If
            System.IO.Directory.Delete(TempPath, True)
        End If
    End Sub

    Private Function ExpandedDateToString(D As DiskImage.ExpandedDate, IncludeTime As Boolean) As String
        Dim Response As String = Format(D.Year, "0000") & "-" & Format(D.Month, "00") & "-" & Format(D.Day, "00")
        If IncludeTime Then
            Response &= "  " & Format(D.Hour12, "00") _
                & ":" & Format(D.Minute, "00") _
                & ":" & Format(D.Second, "00") _
                & " " & IIf(D.IsPM, "PM", "AM")
        End If

        Return Response
    End Function

    Private Function GetListViewFileItem(File As DiskImage.DirectoryEntry, Group As ListViewGroup, LFNFileName As String) As ListViewItem
        Dim SI As ListViewItem.ListViewSubItem

        Dim Attrib As String = IIf(File.IsArchive, "A ", "- ") _
            & IIf(File.IsReadOnly, "R ", "- ") _
            & IIf(File.IsSystem, "S ", "- ") _
            & IIf(File.IsHidden, "H ", "- ") _
            & IIf(File.IsDirectory, "D ", "- ") _
            & IIf(File.IsVolumeName, "V ", "- ")

        Dim Item = New ListViewItem("", Group)
        Item.SubItems.Add(Encoding.UTF8.GetString(File.FileName).Trim)

        If File.IsDeleted Then
            Item.ForeColor = Color.Gray
        ElseIf File.IsVolumeName Then
            Item.ForeColor = Color.Green
        ElseIf File.HasInvalidFilename Or File.HasInvalidExtension Or File.HasInvalidFileSize Then
            Item.ForeColor = Color.Red
        ElseIf File.IsDirectory Then
            Item.ForeColor = Color.Blue
        End If
        Item.SubItems.Add(Encoding.UTF8.GetString(File.Extension).Trim)
        If File.HasInvalidFileSize Then
            Item.SubItems.Add("Invalid")
        Else
            Item.SubItems.Add(Format(File.FileSize, "N0"))
        End If

        Item.SubItems.Add(ExpandedDateToString(File.GetLastWriteDate, True))

        Item.SubItems.Add(Format(File.StartingCluster, "N0"))
        Item.SubItems.Add(Attrib)

        If Not File.IsDeleted And Not File.IsDirectory And Not File.IsVolumeName And Not File.HasInvalidFileSize Then
            Item.SubItems.Add(Crc32.ComputeChecksum(File.GetContent).ToString("X8"))
        Else
            Item.SubItems.Add("")
        End If

        If File.HasCreationDate Then
            SI = Item.SubItems.Add(ExpandedDateToString(File.GetCreationDate, True))
        Else
            SI = Item.SubItems.Add("")
        End If
        SI.Name = "FileCreateDate"

        If File.HasLastAccessDate Then
            SI = Item.SubItems.Add(ExpandedDateToString(File.GetLastAccessDate, False))
        Else
            SI = Item.SubItems.Add("")
        End If
        SI.Name = "FileLastAccessDate"

        SI = Item.SubItems.Add(LFNFileName)
        SI.Name = "FileLFN"

        Return Item
    End Function

    Private Function GetListViewSummaryItem(Name As String, Value As String) As ListViewItem
        Dim Item = New ListViewItem(Name) With {
                .UseItemStyleForSubItems = False
            }
        Item.SubItems.Add(Value)

        Return Item
    End Function

    Private Function GetListViewSummaryItem(Name As String, Value As String, ForeColor As Color) As ListViewItem
        Dim Item = New ListViewItem(Name) With {
                .UseItemStyleForSubItems = False
            }
        Dim SubItem = Item.SubItems.Add(Value)
        SubItem.ForeColor = ForeColor

        Return Item
    End Function

    Private Function GetMediaType(MediaDescriptor As Byte, SectorsPerTrack As UShort) As String
        Select Case MediaDescriptor
            Case &HF0
                If SectorsPerTrack = 36 Then
                    Return "2.88M Floppy"
                Else
                    Return "1.44M Floppy"
                End If
            Case &HF8
                Return "Fixed Disk"
            Case &HF9
                If SectorsPerTrack = 15 Then
                    Return "1.2M Floppy"
                Else
                    Return "720KB Floppy"
                End If
            Case &HFC
                Return "180KB Floppy"
            Case &HFD
                Return "360KB Floppy"
            Case &HFE
                Return "160KB Floppy"
            Case &HFF
                Return "320KB Floppy"
            Case Else
                Return "Unknown"
        End Select
    End Function

    Private Sub InitializeButtons()
        FlowLayoutPanel1.Visible = True
        BtnClearCreated.Enabled = False
        BtnClearLastAccessed.Enabled = False
    End Sub

    Private Sub InitializeColumns()
        If Not ListViewFiles.Columns.ContainsKey("FileCreateDate") Then
            ListViewAddColumn("FileCreateDate", "Created", 0, 8)
        End If
        If Not ListViewFiles.Columns.ContainsKey("FileLastAccessDate") Then
            ListViewAddColumn("FileLastAccessDate", "Last Accessed", 0, 9)
        End If
        If Not ListViewFiles.Columns.ContainsKey("FileLFN") Then
            ListViewAddColumn("FileLFN", "Long File Name", 0, 10)
        End If
    End Sub
    Private Sub ListViewAddColumn(Name As String, Text As String, Width As Integer, Index As Integer)
        Dim Column As New ColumnHeader With {
            .Name = Name,
            .Width = Width,
            .Text = Text
        }
        ListViewFiles.Columns.Insert(Index, Column)
    End Sub

    Private Function MD5Hash(Data() As Byte) As String
        Using hasher As MD5 = MD5.Create()
            Dim dbytes As Byte() = hasher.ComputeHash(Data)
            Dim sBuilder As New StringBuilder()

            For n As Integer = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("X2"))
            Next n

            Return sBuilder.ToString
        End Using
    End Function

    Private Function SHA1Hash(Data() As Byte) As String
        Using hasher As SHA1 = SHA1.Create()
            Dim dbytes As Byte() = hasher.ComputeHash(Data)
            Dim sBuilder As New StringBuilder()

            For n As Integer = 0 To dbytes.Length - 1
                sBuilder.Append(dbytes(n).ToString("X2"))
            Next n

            Return sBuilder.ToString
        End Using
    End Function

    Private Sub PopulateDetails()
        ListViewFiles.Items.Clear()

        Dim Response As ProcessFilesResponse = ProcessFiles(_Disk.Directory, "", False)

        If Not Response.HasCreated Then
            ListViewFiles.Columns.RemoveByKey("FileCreateDate")
        Else
            ListViewFiles.Columns.Item("FileCreateDate").Width = 140
        End If
        If Not Response.HasLastAccessed Then
            ListViewFiles.Columns.RemoveByKey("FileLastAccessDate")
        Else
            ListViewFiles.Columns.Item("FileLastAccessDate").Width = 90
        End If
        If Not Response.HasLFN Then
            ListViewFiles.Columns.RemoveByKey("FileLFN")
        Else
            ListViewFiles.Columns.Item("FileLFN").Width = 200
        End If
        _VolumeLabel = Response.VolumeLabel
        BtnClearCreated.Visible = (Response.HasCreated)
        BtnClearLastAccessed.Visible = (Response.HasLastAccessed)
        CBCheckAll.Checked = False
    End Sub

    Private Function FindOEMMatch(Checksum As UInteger) As BootstrapType
        If _BootstrapTypes.ContainsKey(Checksum) Then
            Return _BootstrapTypes.Item(Checksum)
        Else
            Return Nothing
        End If
    End Function

    Private Sub PopulateSummary()
        Dim BootstrapChecksum = Crc32.ComputeChecksum(_Disk.BootSector.BootStrapCode)
        Dim ForeColor As Color
        Dim OSNameString As String = Encoding.UTF8.GetString(_Disk.BootSector.OSName)
        Dim OSNameMatched As Boolean = False

        Me.Text = "Disk Image Tool - " & System.IO.Path.GetFileName(_Disk.FilePath)

        Dim BootstrapType = FindOEMMatch(BootstrapChecksum)
        If BootstrapType IsNot Nothing Then
            OSNameMatched = BootstrapType.OSNames.Contains(OSNameString)
        End If

        With ListViewSummary.Items
            .Clear()
            .Add(GetListViewSummaryItem("Modified:", IIf(_Disk.Modified, "Yes", "No"), IIf(_Disk.Modified, Color.Blue, SystemColors.WindowText)))
            If BootstrapType IsNot Nothing Then
                If Not OSNameMatched Then
                    ForeColor = Color.Red
                Else
                    ForeColor = Color.Green
                End If
            Else
                ForeColor = SystemColors.WindowText
            End If
            .Add(GetListViewSummaryItem("OEM ID:", OSNameString, ForeColor))
            If BootstrapType IsNot Nothing Then
                .Add(GetListViewSummaryItem("Language:", _BootstrapTypes.Item(BootstrapChecksum).Language))
            End If
            .Add(GetListViewSummaryItem("Media Type:", GetMediaType(_Disk.BootSector.MediaDescriptor, _Disk.BootSector.SectorsPerTrack)))
            If _VolumeLabel <> "" Then
                .Add(GetListViewSummaryItem("Volume Label:", _VolumeLabel))
            End If
            If _Disk.BootSector.ExtendedBootSignature = &H29 Then
                .Add(GetListViewSummaryItem("Volume Serial Number:", _Disk.BootSector.VolumeSerialNumber.ToString("X8").Insert(4, "-")))
                .Add(GetListViewSummaryItem("File System Type:", Encoding.UTF8.GetString(_Disk.BootSector.FileSystemType)))
            End If
            .Add(GetListViewSummaryItem("Bytes Per Sector:", _Disk.BootSector.BytesPerSector))
            .Add(GetListViewSummaryItem("Sectors Per Cluster:", _Disk.BootSector.SectorsPerCluster))
            .Add(GetListViewSummaryItem("Sectors Per Track:", _Disk.BootSector.SectorsPerTrack))
            If BootstrapType IsNot Nothing Then
                If Not OSNameMatched Then
                    For Each OSName In BootstrapType.OSNames
                        .Add(GetListViewSummaryItem("Detected OEM ID:", OSName))
                    Next
                    'If Strings.Right(OSNameString, 3) <> "IHC" Then
                    'System.IO.File.AppendAllText("D:\exceptions.txt", BootstrapChecksum.ToString("X8") & "  " & OSNameString & "  " & _BootstrapTypes.Item(BootstrapChecksum).OSName & "  " & DiskImage.FilePath & vbCrLf)
                    'End If
                End If
                'Else
                'System.IO.File.AppendAllText("D:\missing.txt", BootstrapChecksum.ToString("X8") & "  " & OSNameString & "  " & DiskImage.FilePath & vbCrLf)
            End If
        End With
        With ListViewHashes.Items
            .Clear()
            .Add(GetListViewSummaryItem("CRC32", Crc32.ComputeChecksum(_Disk.Data).ToString("X8")))
            .Add(GetListViewSummaryItem("MD5", MD5Hash(_Disk.Data)))
            .Add(GetListViewSummaryItem("SHA-1", SHA1Hash(_Disk.Data)))
        End With
    End Sub
    Private Sub ProcessFileDrop(Files() As String)
        Dim FilePath As String
        Dim FileInfo As System.IO.FileInfo
        Dim FileList As New List(Of String)
        Dim Index As UInteger
        Dim PathName As String
        Dim FileName As String
        Dim ComboCleared As Boolean = False

        BtnSave.Enabled = False

        Me.Cursor = Cursors.WaitCursor

        For Each FilePath In Files
            Dim FAttributes = System.IO.File.GetAttributes(FilePath)
            If (FAttributes And System.IO.FileAttributes.Directory) > 0 Then
                Dim DirectoryInfo As New System.IO.DirectoryInfo(FilePath)
                For Each FileInfo In DirectoryInfo.GetFiles("*.im*", System.IO.SearchOption.AllDirectories)
                    PathName = System.IO.Path.GetDirectoryName(FilePath)
                    If Not PathName.EndsWith("\") Then
                        PathName &= "\"
                    End If
                    FileName = Strings.Right(FileInfo.FullName, Len(FileInfo.FullName) - Len(PathName))
                    If Not ComboCleared Then
                        ComboGroups.Items.Clear()
                        ComboCleared = True
                    End If
                    Index = ComboGroups.Items.Add(New ComboFileItem(PathName, FileName))
                Next
            Else
                Dim Ext As String = UCase(System.IO.Path.GetExtension(FilePath))
                If Ext = ".IMA" Or Ext = ".IMG" Then
                    PathName = System.IO.Path.GetDirectoryName(FilePath)
                    FileName = System.IO.Path.GetFileName(FilePath)
                    If Not ComboCleared Then
                        ComboGroups.Items.Clear()
                        ComboCleared = True
                    End If
                    Index = ComboGroups.Items.Add(New ComboFileItem(PathName, FileName))
                End If
            End If
        Next

        If ComboCleared And ComboGroups.Items.Count > 0 Then
            ComboGroups.SelectedIndex = 0
            LabelDropMessage.Visible = False
            BtnScan.Enabled = True
            BtnFilters.BackColor = SystemColors.Control
            BtnFilters.UseVisualStyleBackColor = True
            BtnFilters.Enabled = False
            ListFilters.Items.Clear()
            _CachedListItems = Nothing
        End If

        Me.Cursor = Cursors.Default
    End Sub
    Private Function GetFileDataFromFile(File As DiskImage.DirectoryEntry, FilePath As String) As FileData
        Dim Response As FileData
        With Response
            .FilePath = FilePath
            .Offset = File.Offset
            .HasCreated = File.HasCreationDate
            .HasLastAccessed = File.HasLastAccessDate
        End With

        Return Response
    End Function
    Private Function ProcessFiles(Directory As DiskImage.Directory, Path As String, ScanOnly As Boolean) As ProcessFilesResponse
        Dim Group As ListViewGroup = Nothing
        Dim Counter As UInteger
        Dim FileCount As UInteger = Directory.FileCount
        Dim LFNFileName As String = ""
        Dim Response As ProcessFilesResponse
        With Response
            .VolumeLabel = ""
            .HasCreated = False
            .HasLFN = False
            .HasLastAccessed = False
        End With

        If Not ScanOnly Then
            Dim GroupName As String = IIf(Path = "", "(Root)", Path)
            GroupName = GroupName & "  (" & FileCount & IIf(FileCount <> 1, " entries", " entry") & ")"
            Group = New ListViewGroup(GroupName)
            ListViewFiles.Groups.Add(Group)
        End If

        For Counter = 1 To Directory.DirectoryLength
            Dim File = Directory.GetFile(Counter)
            Dim FullFileName = File.GetFileName
            Dim FileData = GetFileDataFromFile(File, Path)

            If FullFileName <> "." And FullFileName <> ".." Then
                If Path = "" And Response.VolumeLabel = "" Then
                    If Not File.IsDeleted And File.IsVolumeName Then
                        Response.VolumeLabel = Encoding.UTF8.GetString(File.FileName) & Encoding.UTF8.GetString(File.Extension)
                    End If
                End If
                If File.IsLFN Then
                    LFNFileName = File.GetLFNFileName & LFNFileName
                Else
                    If Not ScanOnly Then
                        Dim item = GetListViewFileItem(File, Group, LFNFileName)
                        _FileHashTable.Add(item.GetHashCode, FileData)
                        ListViewFiles.Items.Add(item)
                    End If

                    If Not Response.HasCreated Then
                        If File.HasCreationDate Then
                            Response.HasCreated = True
                        End If
                    End If
                    If Not Response.HasLastAccessed Then
                        If File.HasLastAccessDate Then
                            Response.HasLastAccessed = True
                        End If
                    End If

                    If Not Response.HasLFN Then
                        If LFNFileName <> "" Then
                            Response.HasLFN = True
                        End If
                    End If
                    LFNFileName = ""
                End If

                If File.IsDirectory And File.SubDirectory IsNot Nothing Then
                    If FullFileName <> "." And FullFileName <> ".." And File.SubDirectory.DirectoryLength > 0 Then
                        Dim NewPath = FullFileName
                        If Path <> "" Then
                            NewPath = Path & "\" & NewPath
                        End If
                        Dim SubResponse = ProcessFiles(File.SubDirectory, NewPath, ScanOnly)
                        Response.HasLastAccessed = Response.HasLastAccessed Or SubResponse.HasLastAccessed
                        Response.HasCreated = Response.HasCreated Or SubResponse.HasCreated
                        Response.HasLFN = Response.HasLFN Or SubResponse.HasLFN
                    End If
                End If
            End If
        Next

        Return Response
    End Function
    Private Sub ProcessImage(Item As ComboFileItem)
        _FileHashTable.Clear()
        InitializeColumns()
        InitializeButtons()

        If Item.Disk IsNot Nothing Then
            _Disk = Item.Disk
        Else
            Dim FilePath As String = System.IO.Path.Combine(Item.Path, Item.File)
            _Disk = New DiskImage.Disk(FilePath)
        End If
        If _Disk.IsValidImage Then
            PopulateDetails()
            PopulateSummary()
            LblInvalidImage.Visible = False
            CBCheckAll.Visible = True
        Else
            _VolumeLabel = ""
            ListViewSummary.Items.Clear()
            ListViewFiles.Items.Clear()
            LblInvalidImage.Visible = True
            BtnClearCreated.Visible = False
            BtnClearLastAccessed.Visible = False
            CBCheckAll.Visible = False
        End If
    End Sub
    Private Sub RefreshButtons()
        Dim CreatedEnabled As Boolean = False
        Dim LastAccessEnabled As Boolean = False

        For Each Item As ListViewItem In ListViewFiles.CheckedItems
            Dim FileData = _FileHashTable(Item.GetHashCode)
            If FileData.HasCreated Then
                CreatedEnabled = True
            End If
            If FileData.HasLastAccessed Then
                LastAccessEnabled = True
            End If
            If CreatedEnabled And LastAccessEnabled Then
                Exit For
            End If
        Next
        BtnClearCreated.Enabled = CreatedEnabled
        BtnClearLastAccessed.Enabled = LastAccessEnabled
    End Sub
    Private Sub SaveChanges()
        _SuppressEvent = True
        For Counter = 0 To ComboGroups.Items.Count - 1
            Dim Item As ComboFileItem = ComboGroups.Items(Counter)
            If Item.Modified Then
                If Item.Disk IsNot Nothing Then
                    Dim BackupPath As String = Item.Disk.FilePath & ".bak"
                    System.IO.File.Copy(Item.Disk.FilePath, BackupPath, True)
                    Item.Disk.SaveFile(Item.Disk.FilePath)
                    Item.Disk = Nothing
                    Item.Modified = False
                    ComboGroups.Items(Counter) = ComboGroups.Items(Counter)
                End If
            End If
        Next
        _SuppressEvent = False
        PopulateSummary()
        BtnSave.Enabled = False
    End Sub
    Private Sub ScanImages()
        Dim HasMismatchedOEMID As Boolean = False
        Dim HasunknownOEMID As Boolean = False
        Dim HasCreated As Boolean = False
        Dim HasLastAccessed As Boolean = False
        Dim HasInvalidImage As Boolean = False
        Dim HasLongFileNames As Boolean = False

        Me.Cursor = Cursors.WaitCursor

        BtnScan.Enabled = False
        BtnFilters.Enabled = False
        ListFilters.Items.Clear()
        BtnFilters.BackColor = SystemColors.Control
        BtnFilters.UseVisualStyleBackColor = True
        If _CachedListItems IsNot Nothing Then
            RestoreComboGroups()
        End If

        LblFilterMessage.Visible = True
        For Counter = 0 To ComboGroups.Items.Count - 1
            Dim Percentage = Counter / ComboGroups.Items.Count * 100
            If Counter Mod 100 = 0 Then
                LblFilterMessage.Text = "Scanning... " & Int(Percentage) & "%"
                Application.DoEvents()
            End If
            Dim Item As ComboFileItem = ComboGroups.Items(Counter)
            Dim FilePath As String = System.IO.Path.Combine(Item.Path, Item.File)
            Dim Disk = New DiskImage.Disk(FilePath)
            If Disk.IsValidImage Then
                Item.IsValidImage = True
                Dim BootstrapChecksum = Crc32.ComputeChecksum(Disk.BootSector.BootStrapCode)
                Dim OSNameString As String = Encoding.UTF8.GetString(Disk.BootSector.OSName)

                Dim BootstrapType = FindOEMMatch(BootstrapChecksum)
                If BootstrapType IsNot Nothing Then
                    Item.OEMIDFound = True
                    Item.OEMIDMatched = BootstrapType.OSNames.Contains(OSNameString)
                    If Not Item.OEMIDMatched Then
                        HasMismatchedOEMID = True
                    End If
                Else
                    Item.OEMIDFound = False
                    Item.OEMIDMatched = False
                    HasunknownOEMID = True
                End If

                Dim Response As ProcessFilesResponse = ProcessFiles(Disk.Directory, "", True)
                Item.HasCreated = Response.HasCreated
                If Item.HasCreated Then
                    HasCreated = True
                End If
                Item.HasLastAccessed = Response.HasLastAccessed
                If Item.HasLastAccessed Then
                    HasLastAccessed = True
                End If
                Item.HasLongFileNames = Response.HasLFN
                If Item.HasLongFileNames Then
                    HasLongFileNames = True
                End If
            Else
                Item.IsValidImage = False
                HasInvalidImage = True
            End If

            Item.Scanned = True
        Next
        LblFilterMessage.Visible = False

        If HasunknownOEMID Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.UnknownOEMID, "Unknown OEM ID"))
        End If
        If HasMismatchedOEMID Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.MismatchedOEMID, "Mismatched OEM ID"))
        End If
        If HasCreated Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.HasCreated, "Has Creation Date"))
        End If
        If HasLastAccessed Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.HasLastAccessed, "Has Last Access Date"))
        End If
        If HasLongFileNames Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.HasLongFileNames, "Has Long File Names"))
        End If
        If HasInvalidImage Then
            ListFilters.Items.Add(New ComboFileType(FilterTypes.HasInvalidImage, "Invalid Image"))
        End If
        BtnFilters.Enabled = (ListFilters.Items.Count > 0)
        BtnScan.Enabled = True

        Me.Cursor = Cursors.Default
    End Sub
    Private Sub StartFileDrop(e As DragEventArgs)
        If _InternalDrop Then
            Exit Sub
        End If
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs)
        For Counter = 0 To ComboGroups.Items.Count - 1
            ComboGroups.SelectedIndex = Counter
            Debug.Print(ComboGroups.Text)
            Application.DoEvents()
        Next
    End Sub
    Private Sub ComboGroups_DragDrop(sender As Object, e As DragEventArgs) Handles ComboGroups.DragDrop
        ProcessFileDrop(e.Data.GetData(DataFormats.FileDrop))
    End Sub
    Private Sub ComboGroups_DragEnter(sender As Object, e As DragEventArgs) Handles ComboGroups.DragEnter
        StartFileDrop(e)
    End Sub
    Private Sub ComboGroups_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboGroups.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        ProcessImage(ComboGroups.SelectedItem)
    End Sub
    Private Sub ListViewFiles_DragDrop(sender As Object, e As DragEventArgs) Handles ListViewFiles.DragDrop
        ProcessFileDrop(e.Data.GetData(DataFormats.FileDrop))
    End Sub
    Private Sub ListViewFiles_DragEnter(sender As Object, e As DragEventArgs) Handles ListViewFiles.DragEnter
        StartFileDrop(e)
    End Sub
    Private Sub ListViewSummary_DragDrop(sender As Object, e As DragEventArgs) Handles ListViewSummary.DragDrop
        ProcessFileDrop(e.Data.GetData(DataFormats.FileDrop))
    End Sub
    Private Sub ListViewSummary_DragEnter(sender As Object, e As DragEventArgs) Handles ListViewSummary.DragEnter
        StartFileDrop(e)
    End Sub

    Private Sub ListViewHashes_DragDrop(sender As Object, e As DragEventArgs) Handles ListViewHashes.DragDrop
        ProcessFileDrop(e.Data.GetData(DataFormats.FileDrop))
    End Sub

    Private Sub ListViewHashes_DragEnter(sender As Object, e As DragEventArgs) Handles ListViewHashes.DragEnter
        StartFileDrop(e)
    End Sub

    Private Sub ListViewFiles_ItemDrag(sender As Object, e As ItemDragEventArgs) Handles ListViewFiles.ItemDrag
        _InternalDrop = True
        DragDropSelectedFiles()
        _InternalDrop = False
    End Sub

    Private Sub ButtonDisplayBootSector_Click(sender As Object, e As EventArgs) Handles ButtonDisplayBootSector.Click
        Dim frmHexView As New HexViewForm(_Disk.BootSector.Data)
        frmHexView.ShowDialog()
    End Sub

    Private Sub ButtonOEMID_Click(sender As Object, e As EventArgs) Handles ButtonOEMID.Click
        ChangeOEMID()
    End Sub

    Private Sub LabelDropMessage_DragDrop(sender As Object, e As DragEventArgs) Handles LabelDropMessage.DragDrop
        ProcessFileDrop(e.Data.GetData(DataFormats.FileDrop))
    End Sub

    Private Sub LabelDropMessage_DragEnter(sender As Object, e As DragEventArgs) Handles LabelDropMessage.DragEnter
        StartFileDrop(e)
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        SaveChanges()
    End Sub

    Private Sub ListViewFiles_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListViewFiles.ColumnWidthChanging
        If e.ColumnIndex = 0 Then
            e.NewWidth = Me.ListViewFiles.Columns(e.ColumnIndex).Width
            e.Cancel = True
        End If
    End Sub

    Private Sub CBCheckAll_CheckedChanged(sender As Object, e As EventArgs) Handles CBCheckAll.CheckedChanged
        _SuppressEvent = True
        For Each Item As ListViewItem In ListViewFiles.Items
            Item.Checked = CBCheckAll.Checked
        Next
        _SuppressEvent = False
        RefreshButtons()
    End Sub

    Private Sub ListViewFiles_ItemChecked(sender As Object, e As ItemCheckedEventArgs) Handles ListViewFiles.ItemChecked
        If _SuppressEvent Then
            Exit Sub
        End If

        RefreshButtons()
    End Sub

    Private Sub BtnClearCreated_Click(sender As Object, e As EventArgs) Handles BtnClearCreated.Click
        Dim Msg As String = "Are you sure you wish to clear the Creation Date for all selected files?"
        If MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            ClearCreatedDate()
            BtnClearCreated.Enabled = False
        End If
    End Sub

    Private Sub BtnClearLastAccessed_Click(sender As Object, e As EventArgs) Handles BtnClearLastAccessed.Click
        Dim Msg As String = "Are you sure you wish to clear the Last Access Date for all selected files?"
        If MsgBox(Msg, MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.Yes Then
            ClearLastAccessedDate()
            BtnClearLastAccessed.Enabled = False
        End If
    End Sub

    Private Sub BtnScan_Click(sender As Object, e As EventArgs) Handles BtnScan.Click
        ScanImages()
    End Sub

    Private Sub BtnFilters_Click(sender As Object, e As EventArgs) Handles BtnFilters.Click
        ListFilters.Visible = Not ListFilters.Visible
        If ListFilters.Visible Then
            ListFilters.Focus()
        End If
    End Sub

    Private Sub ListFilters_LostFocus(sender As Object, e As EventArgs) Handles ListFilters.LostFocus
        ListFilters.Visible = False
    End Sub

    Private Sub ListFilters_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles ListFilters.ItemCheck
        ApplyFilters(e)
    End Sub

End Class

Public Structure ProcessFilesResponse
    Dim VolumeLabel As String
    Dim HasCreated As Boolean
    Dim HasLastAccessed As Boolean
    Dim HasLFN As Boolean
End Structure

Public Structure FileData
    Dim FilePath As String
    Dim Offset As UInteger
    Dim HasCreated As Boolean
    Dim HasLastAccessed As Boolean
End Structure

Public Class BootstrapType
    Public OSNames As List(Of String)
    Public Language As String
End Class

Public Class ComboFileType
    Private _ID As FilterTypes
    Private _Text As String

    Public Property ID As FilterTypes
        Get
            Return _ID
        End Get
        Set
            _ID = Value
        End Set
    End Property

    Public Property Text As String
        Get
            Return _Text
        End Get
        Set
            _Text = Value
        End Set
    End Property
    Public Sub New(ID As FilterTypes, Text As String)
        _ID = ID
        _Text = Text
    End Sub
    Public Overrides Function ToString() As String
        Return _Text
    End Function
End Class


