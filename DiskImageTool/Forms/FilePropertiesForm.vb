﻿Imports System.ComponentModel
Imports DiskImageTool.DiskImage

Public Class FilePropertiesForm
    Private Const EMPTY_FORMAT As String = "'Empty'"
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _Items As ICollection
    Private _DeferredChange() As Byte
    Private _Deleted As Boolean = False
    Private _HasDeferredChange As Boolean = False
    Private _IsVolumeLabel As Boolean = False
    Private _SuppressEvent As Boolean = True
    Private _Updated As Boolean = False
    Private _NewDirectoryEntry As DirectoryEntryBase

    Public Sub New(Disk As DiskImage.Disk, Items As ICollection)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk
        _Items = Items

        Me.Text = "File Properties"
        BtnUpdate.Text = "Update"
    End Sub

    Public Sub New(Disk As DiskImage.Disk)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk
        _Items = Nothing

        Me.Text = "New Directory"
        BtnUpdate.Text = "Add"
    End Sub

    Public ReadOnly Property HasLFN As Boolean
        Get
            Return RadioFileLong.Checked
        End Get
    End Property

    Public ReadOnly Property LFN As String
        Get
            Return TxtLFN.Text
        End Get
    End Property

    Public ReadOnly Property NewDirectoryData As Byte()
        Get
            Return _NewDirectoryEntry.Data
        End Get
    End Property

    Public ReadOnly Property Updated As Boolean
        Get
            Return _Updated
        End Get
    End Property

    Private Sub ApplyAttributesUpdate(DirectoryEntry As DiskImage.DirectoryEntryBase)
        Dim Attributes As Byte = DirectoryEntry.Attributes
        Dim BitArray = New BitArray(Attributes)

        If BtnArchive.Tag Then
            Attributes = MyBitConverter.ToggleBit(Attributes, DiskImage.DirectoryEntry.AttributeFlags.ArchiveFlag, ChkArchive.Checked)
        End If
        If BtnReadOnly.Tag Then
            Attributes = MyBitConverter.ToggleBit(Attributes, DiskImage.DirectoryEntry.AttributeFlags.ReadOnly, ChkReadOnly.Checked)
        End If
        If BtnHidden.Tag Then
            Attributes = MyBitConverter.ToggleBit(Attributes, DiskImage.DirectoryEntry.AttributeFlags.Hidden, ChkHidden.Checked)
        End If
        If BtnSystem.Tag Then
            Attributes = MyBitConverter.ToggleBit(Attributes, DiskImage.DirectoryEntry.AttributeFlags.System, ChkSystem.Checked)
        End If

        If Attributes <> DirectoryEntry.Attributes Then
            DirectoryEntry.Attributes = Attributes
            _Updated = True
        End If
    End Sub

    Private Sub ApplyFileDatesUpdate(DirectoryEntry As DiskImage.DirectoryEntryBase)
        Dim LastWritten = GetDateFromPicker(DTLastWritten, DTLastWrittenTime).Value
        Dim Created As Date? = GetDateFromPicker(DTCreated, DTCreatedTime, NumCreatedMS)
        Dim LastAccessed As Date? = GetDateFromPicker(DTLastAccessed)
        Dim DT As DiskImage.ExpandedDate

        If BtnLastWritten.Tag Then
            DT = DirectoryEntry.GetLastWriteDate
            If Not DT.IsValidDate Or Date.Compare(DT.DateObject, LastWritten) <> 0 Then
                DirectoryEntry.SetLastWriteDate(LastWritten)
                _Updated = True
            End If
        End If

        If BtnCreated.Tag Then
            If DirectoryEntry.HasCreationDate() Then
                DT = DirectoryEntry.GetCreationDate
                If Created Is Nothing Then
                    DirectoryEntry.ClearCreationDate()
                    _Updated = True
                ElseIf Not DT.IsValidDate Or Date.Compare(DT.DateObject, Created) <> 0 Then
                    DirectoryEntry.SetCreationDate(Created)
                    _Updated = True
                End If
            ElseIf Created IsNot Nothing Then
                DirectoryEntry.SetCreationDate(Created)
                _Updated = True
            End If
        End If

        If BtnLastAccessed.Tag Then
            If DirectoryEntry.HasLastAccessDate() Then
                DT = DirectoryEntry.GetLastAccessDate
                If LastAccessed Is Nothing Then
                    DirectoryEntry.ClearLastAccessDate()
                    _Updated = True
                ElseIf Not DT.IsValidDate Or Date.Compare(DT.DateObject, LastAccessed) <> 0 Then
                    DirectoryEntry.SetLastAccessDate(LastAccessed)
                    _Updated = True
                End If
            ElseIf LastAccessed IsNot Nothing Then
                DirectoryEntry.SetLastAccessDate(LastAccessed)
                _Updated = True
            End If
        End If
    End Sub

    Private Sub ApplyFileNameUpdateShort(DirectoryEntry As DiskImage.DirectoryEntryBase)
        Dim FileName() As Byte
        Dim Extension() As Byte

        If DirectoryEntry.IsValidVolumeName Then
            Dim VolumeLabel() As Byte = MskFileHex.GetHex
            ReDim FileName(7)
            ReDim Extension(2)
            ResizeArray(VolumeLabel, 11, 32)
            Array.Copy(VolumeLabel, 0, FileName, 0, 8)
            Array.Copy(VolumeLabel, 8, Extension, 0, 3)
        Else
            FileName = MskFileHex.GetHex
            ResizeArray(FileName, 8, 32)
            Extension = MskExtensionHex.GetHex
            ResizeArray(Extension, 3, 32)
        End If

        If Not FileName.CompareTo(DirectoryEntry.FileName) Then
            DirectoryEntry.FileName = FileName
            _Updated = True
        End If

        If Not Extension.CompareTo(DirectoryEntry.Extension) Then
            DirectoryEntry.Extension = Extension
            _Updated = True
        End If
    End Sub

    Private Sub ApplyFilePropertiesUpdate()
        _Updated = False

        Dim UseTransaction = _Disk.BeginTransaction

        Dim UpdateFileName = _Items.Count = 1

        For Each Item As ListViewItem In _Items
            Dim FileData As FileData = Item.Tag
            Dim DirectoryEntry = FileData.DirectoryEntry
            Dim NewDirectoryEntry = DirectoryEntry.Clone

            If UpdateFileName Then
                If RadioFileShort.Checked Then
                    ApplyFileNameUpdateShort(NewDirectoryEntry)
                End If
            End If
            ApplyFileDatesUpdate(NewDirectoryEntry)
            ApplyAttributesUpdate(NewDirectoryEntry)

            DirectoryEntry.Data = NewDirectoryEntry.Data

            If DirectoryEntry.IsDirectory And Not DirectoryEntry.IsDeleted And Not DirectoryEntry.IsVolumeName Then
                DirectoryEntry.SubDirectory.UpdateLinkDates()
            End If

            If UpdateFileName Then
                If RadioFileShort.Checked Then
                    If Not DirectoryEntry.IsValidVolumeName And Not DirectoryEntry.IsDeleted And DirectoryEntry.HasLFN Then
                        If DirectoryEntry.RemoveLFN() Then
                            _Updated = True
                        End If
                    End If
                Else
                    If DirectoryEntry.ParentDirectory.UpdateLFN(TxtLFN.Text, DirectoryEntry.Index) Then
                        _Updated = True
                    End If
                End If
            End If
        Next

        If UseTransaction Then
            _Disk.EndTransaction()
        End If
    End Sub

    Private Sub ApplyNewDirectoryUpdate()
        _Updated = True

        _NewDirectoryEntry = New DirectoryEntryBase With {
            .FileSize = 0
        }

        If RadioFileShort.Checked Then
            ApplyFileNameUpdateShort(_NewDirectoryEntry)
        End If

        ApplyFileDatesUpdate(_NewDirectoryEntry)
        ApplyAttributesUpdate(_NewDirectoryEntry)

        _NewDirectoryEntry.Attributes = MyBitConverter.ToggleBit(_NewDirectoryEntry.Attributes, DiskImage.DirectoryEntry.AttributeFlags.Directory, True)
    End Sub

    Private Function GetDateFromPicker(DatePicker As DateTimePicker, TimePicker As DateTimePicker, MS As NumericUpDown) As Date?
        If Not DatePicker.ShowCheckBox OrElse DatePicker.Checked Then
            Dim Milliseecond As Integer = 0
            If MS IsNot Nothing Then
                Milliseecond = MS.Value
            End If
            Return New Date(DatePicker.Value.Year, DatePicker.Value.Month, DatePicker.Value.Day, TimePicker.Value.Hour, TimePicker.Value.Minute, TimePicker.Value.Second, Milliseecond)
        Else
            Return Nothing
        End If
    End Function

    Private Function GetDateFromPicker(DatePicker As DateTimePicker, TimePicker As DateTimePicker) As Date?
        Return GetDateFromPicker(DatePicker, TimePicker, Nothing)
    End Function

    Private Function GetDateFromPicker(DatePicker As DateTimePicker) As Date?
        If DatePicker.Checked Then
            Return DatePicker.Value
        Else
            Return Nothing
        End If
    End Function

    Private Sub InitButtons(Visible As Boolean)
        ToggleButton(BtnLastAccessed, Not Visible, Visible)
        ToggleButton(BtnCreated, Not Visible, Visible)
        ToggleButton(BtnLastWritten, Not Visible, Visible)
        ToggleButton(BtnArchive, Not Visible, Visible)
        ToggleButton(BtnReadOnly, Not Visible, Visible)
        ToggleButton(BtnSystem, Not Visible, Visible)
        ToggleButton(BtnHidden, Not Visible, Visible)
    End Sub

    Private Sub InitMultiple(Multiple As Boolean)
        TxtFile.Visible = Not Multiple
        TxtExtension.Visible = Not Multiple
        LblMultipleFiles.Visible = Multiple
        MskFileHex.Visible = Not Multiple
        MskExtensionHex.Visible = Not Multiple
        FlowLayoutFileNameType.Visible = Not Multiple
        InitButtons(Multiple)
    End Sub

    Private Sub PopulateFormMultiple()
        Dim DT As DiskImage.ExpandedDate
        Dim LastWritten = New Date(1980, 1, 1, 0, 0, 0)
        Dim Created As Date? = Nothing
        Dim LastAccessed As Date? = Nothing
        Dim SetArchived As Boolean = True
        Dim SetReadOnly As Boolean = True
        Dim SetSystem As Boolean = True
        Dim SetHidden As Boolean = True

        _IsVolumeLabel = False
        _Deleted = False

        GroupFileName.Text = "Multiple Files"
        LblMultipleFiles.Text = "(" & _Items.Count & " Files Selected)"

        For Each Item As ListViewItem In _Items
            Dim FileData As FileData = Item.Tag
            Dim DirectoryEntry = FileData.DirectoryEntry
            DT = DirectoryEntry.GetLastWriteDate
            If DT.IsValidDate Then
                If DT.DateObject > LastWritten Then
                    LastWritten = DT.DateObject
                End If
            End If
            DT = DirectoryEntry.GetCreationDate
            If DT.IsValidDate Then
                If Created Is Nothing Or DT.DateObject > Created Then
                    Created = DT.DateObject
                End If
            End If
            DT = DirectoryEntry.GetLastAccessDate
            If DT.IsValidDate Then
                If LastAccessed Is Nothing Or DT.DateObject > LastAccessed Then
                    LastAccessed = DT.DateObject
                End If
            End If

            If Not DirectoryEntry.IsArchive Then
                SetArchived = False
            End If
            If Not DirectoryEntry.IsReadOnly Then
                SetReadOnly = False
            End If
            If Not DirectoryEntry.IsHidden Then
                SetHidden = False
            End If
            If Not DirectoryEntry.IsSystem Then
                SetSystem = False
            End If
        Next

        DTLastWritten.Value = LastWritten.Date
        DTLastWrittenTime.Value = LastWritten
        SetCreatedDateValue(Created)
        SetLastAccessedDateValue(LastAccessed)

        ChkArchive.Checked = SetArchived
        ChkReadOnly.Checked = SetReadOnly
        ChkHidden.Checked = SetHidden
        ChkSystem.Checked = SetSystem

        TxtLFN.Visible = False
    End Sub

    Private Sub PopulateFormNewDirectory()
        Dim Maxlength As Integer = 8
        _IsVolumeLabel = False
        _Deleted = False

        TxtExtension.Visible = True
        MskExtensionHex.Visible = True
        TxtExtension.Text = ""
        MskExtensionHex.SetHex(New Byte(2) {32, 32, 32})

        FlowLayoutFileNameType.Visible = True
        RadioFileShort.Checked = True

        MskFileHex.MaskLength = Maxlength
        MskFileHex.Width = (Maxlength * 3 - 1) * 7 + 8
        MskFileHex.SetHex(New Byte(7) {32, 32, 32, 32, 32, 32, 32, 32})
        TxtFile.PromptChar = " "
        TxtFile.Width = MskFileHex.Width

        TxtFile.Mask = ">" & Strings.StrDup(8, "C")
        TxtFile.Text = ""

        TxtLFN.Text = ""

        GroupFileName.Text = "Directory Name"

        ChkArchive.Checked = False
        ChkReadOnly.Checked = False
        ChkHidden.Checked = False
        ChkSystem.Checked = False

        ToggleFileType(True)
    End Sub

    Private Sub PopulateFormSingle()
        Dim Item As ListViewItem = _Items(0)
        Dim FileData As FileData = Item.Tag
        Dim DirectoryEntry = FileData.DirectoryEntry
        Dim DT As DiskImage.ExpandedDate

        Dim IsDirectory = DirectoryEntry.IsDirectory And Not DirectoryEntry.IsVolumeName
        Dim HasLFN = DirectoryEntry.HasLFN

        _IsVolumeLabel = DirectoryEntry.IsValidVolumeName
        _Deleted = DirectoryEntry.IsDeleted

        Dim Caption As String
        Dim Maxlength As Integer
        Dim FileName As String
        Dim FileNameHex() As Byte

        If _IsVolumeLabel Then
            Caption = "Volume Label"
            Maxlength = 11
            TxtExtension.Visible = False
            MskExtensionHex.Visible = False
            FileName = DirectoryEntry.GetVolumeName
            ReDim FileNameHex(Maxlength - 1)
            DirectoryEntry.FileName.CopyTo(FileNameHex, 0)
            DirectoryEntry.Extension.CopyTo(FileNameHex, 8)
            FlowLayoutFileNameType.Visible = False
            RadioFileShort.Checked = True
        Else
            Maxlength = 8
            TxtExtension.Visible = True
            MskExtensionHex.Visible = True
            FileName = DirectoryEntry.GetFileName
            FileNameHex = DirectoryEntry.FileName
            TxtExtension.Text = DirectoryEntry.GetFileExtension
            MskExtensionHex.SetHex(DirectoryEntry.Extension)
            If IsDirectory Then
                Caption = "Directory Name"
            Else
                Caption = "File Name"
            End If
            FlowLayoutFileNameType.Visible = Not _Deleted
            If Not _Deleted And HasLFN Then
                RadioFileLong.Checked = True
            Else
                RadioFileShort.Checked = True
            End If
        End If

        MskFileHex.MaskLength = Maxlength
        MskFileHex.Width = (Maxlength * 3 - 1) * 7 + 8
        MskFileHex.SetHex(FileNameHex)
        TxtFile.PromptChar = " "
        TxtFile.Width = MskFileHex.Width

        If _Deleted Then
            Caption &= " (Deleted)"
            TxtFile.Mask = "\" & FileName.Substring(0, 1) & ">" & Strings.StrDup(Maxlength - 1, "C")
            TxtFile.Text = FileName.Substring(1)
        Else
            TxtFile.Mask = ">" & Strings.StrDup(Maxlength, "C")
            TxtFile.Text = FileName
        End If

        If HasLFN Then
            TxtLFN.Text = DirectoryEntry.GetLongFileName
        Else
            TxtLFN.Text = DirectoryEntry.GetFullFileName
        End If

        GroupFileName.Text = Caption

        DT = DirectoryEntry.GetLastWriteDate
        If DT.IsValidDate Then
            DTLastWritten.Value = DT.DateObject.Date
            DTLastWrittenTime.Value = DT.DateObject
        Else
            DTLastWritten.Value = New Date(1980, 1, 1)
            DTLastWrittenTime.Value = New Date(1980, 1, 1, 0, 0, 0)
        End If

        If DirectoryEntry.HasCreationDate() Then
            DT = DirectoryEntry.GetCreationDate
            If DT.IsValidDate Then
                SetCreatedDateValue(DT.DateObject)
            End If
        End If

        If DirectoryEntry.HasLastAccessDate() Then
            DT = DirectoryEntry.GetLastAccessDate
            If DT.IsValidDate Then
                SetLastAccessedDateValue(DT.DateObject)
            End If
        End If

        ChkArchive.Checked = DirectoryEntry.IsArchive
        ChkReadOnly.Checked = DirectoryEntry.IsReadOnly
        ChkHidden.Checked = DirectoryEntry.IsHidden
        ChkSystem.Checked = DirectoryEntry.IsSystem

        ToggleFileType(True)
    End Sub

    Private Sub SetCreatedDateValue(Value? As Date)
        If Value Is Nothing Then
            DTCreated.Format = DateTimePickerFormat.Custom
            DTCreated.CustomFormat = EMPTY_FORMAT
            DTCreated.Checked = False

            DTCreatedTime.Visible = False

            NumCreatedMS.Visible = False
            NumCreatedMS.Value = 0

            PanelCreatedTime.BorderStyle = BorderStyle.FixedSingle
        Else
            DTCreated.Format = DateTimePickerFormat.Short
            DTCreated.CustomFormat = ""
            DTCreated.Checked = True
            DTCreated.Value = Value.Value.Date

            DTCreatedTime.Visible = True
            DTCreatedTime.Value = Value.Value

            NumCreatedMS.Visible = True
            NumCreatedMS.Value = Value.Value.Millisecond

            PanelCreatedTime.BorderStyle = BorderStyle.None
        End If
    End Sub

    Private Sub SetLastAccessedDateValue(Value? As Date)
        If Value Is Nothing Then
            DTLastAccessed.Format = DateTimePickerFormat.Custom
            DTLastAccessed.CustomFormat = EMPTY_FORMAT
            DTLastAccessed.Checked = False
        Else
            DTLastAccessed.Format = DateTimePickerFormat.Short
            DTLastAccessed.CustomFormat = ""
            DTLastAccessed.Checked = True
            DTLastAccessed.Value = Value
        End If
    End Sub

    Private Sub ToggleButton(Button As Button, Enabled As Boolean, Optional Visible As Boolean = True)
        Button.Visible = Visible
        Button.Tag = Enabled
        Button.BackColor = IIf(Enabled, Color.LightGreen, SystemColors.Control)
        Button.UseVisualStyleBackColor = Not Enabled
        ToggleRelatedControls(Button)
    End Sub

    Private Sub ToggleFileType(SetFocus As Boolean)
        If RadioFileShort.Checked Then
            TableLayoutPanelFile.Visible = True
            TxtLFN.Visible = False
            If SetFocus Then
                TxtFile.Select()
            End If
        Else
            TxtLFN.Visible = True
            TableLayoutPanelFile.Visible = False
            If SetFocus Then
                TxtLFN.SelectionStart = TxtLFN.Text.Length
                TxtLFN.Select()
            End If
        End If
    End Sub

    Private Sub ToggleRelatedControls(Button As Button)
        If Button Is BtnLastWritten Then
            DTLastWritten.Enabled = Button.Tag
            DTLastWrittenTime.Enabled = Button.Tag
            LblLastWritten.Enabled = Button.Tag
        ElseIf Button Is BtnLastAccessed Then
            DTLastAccessed.Enabled = Button.Tag
            LblLastAccessed.Enabled = Button.Tag
        ElseIf Button Is BtnCreated Then
            DTCreated.Enabled = Button.Tag
            DTCreatedTime.Enabled = Button.Tag
            NumCreatedMS.Enabled = Button.Tag
            LblCreated.Enabled = Button.Tag
        ElseIf Button Is BtnArchive Then
            ChkArchive.Enabled = Button.Tag
        ElseIf Button Is BtnReadOnly Then
            ChkReadOnly.Enabled = Button.Tag
        ElseIf Button Is BtnSystem Then
            ChkSystem.Enabled = Button.Tag
        ElseIf Button Is BtnHidden Then
            ChkHidden.Enabled = Button.Tag
        End If
    End Sub

#Region "Events"

    Private Sub BtnEnable_Click(sender As Object, e As EventArgs) Handles BtnLastWritten.Click, BtnCreated.Click, BtnLastAccessed.Click, BtnReadOnly.Click, BtnHidden.Click, BtnArchive.Click, BtnSystem.Click
        ToggleButton(sender, Not sender.tag)
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        If _Items IsNot Nothing Then
            ApplyFilePropertiesUpdate()
        Else
            ApplyNewDirectoryUpdate()
        End If
    End Sub

    Private Sub DTCreated_ValueChanged(sender As Object, e As EventArgs) Handles DTCreated.ValueChanged
        If DTCreated.Checked Then
            DTCreated.Format = DateTimePickerFormat.Short
            DTCreated.CustomFormat = ""
            DTCreatedTime.Visible = True
            NumCreatedMS.Visible = True
            PanelCreatedTime.BorderStyle = BorderStyle.None
        Else
            DTCreated.Format = DateTimePickerFormat.Custom
            DTCreated.CustomFormat = EMPTY_FORMAT
            DTCreatedTime.Visible = False
            NumCreatedMS.Visible = False
            PanelCreatedTime.BorderStyle = BorderStyle.FixedSingle
        End If
    End Sub

    Private Sub DTLastAccessed_ValueChanged(sender As Object, e As EventArgs) Handles DTLastAccessed.ValueChanged
        If DTLastAccessed.Checked Then
            DTLastAccessed.Format = DateTimePickerFormat.Short
            DTLastAccessed.CustomFormat = ""
        Else
            DTLastAccessed.Format = DateTimePickerFormat.Custom
            DTLastAccessed.CustomFormat = EMPTY_FORMAT
        End If
    End Sub

    Private Sub FilePropertiesForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        _SuppressEvent = True

        SetCreatedDateValue(Nothing)
        SetLastAccessedDateValue(Nothing)

        If _Items IsNot Nothing Then
            DTLastWritten.Value = New Date(1980, 1, 1)
            DTLastWrittenTime.Value = New Date(1980, 1, 1, 0, 0, 0)

            InitMultiple(_Items.Count > 1)

            If _Items.Count = 1 Then
                PopulateFormSingle()
            Else
                PopulateFormMultiple()
            End If
        Else
            DTLastWritten.Value = Now.Date
            DTLastWrittenTime.Value = Now

            InitMultiple(False)

            PopulateFormNewDirectory()
        End If

        _SuppressEvent = False
    End Sub

    Private Sub MskExtensionHex_TextChanged(sender As Object, e As EventArgs) Handles MskExtensionHex.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim NewValue = DiskImage.CodePage437ToUnicode(MskExtensionHex.GetHex)

        _SuppressEvent = True
        If TxtExtension.Text <> NewValue Then
            TxtExtension.Text = NewValue
        End If
        _SuppressEvent = False
    End Sub

    Private Sub MskFileHex_KeyUp(sender As Object, e As KeyEventArgs) Handles MskFileHex.KeyUp
        If _HasDeferredChange Then
            Dim SelectionStart = MskFileHex.SelectionStart
            MskFileHex.SetHex(_DeferredChange, False)
            MskFileHex.Select(SelectionStart, 0)
            _DeferredChange = Nothing
            _HasDeferredChange = False
        End If
    End Sub

    Private Sub MskFileHex_TextChanged(sender As Object, e As EventArgs) Handles MskFileHex.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim Hex = MskFileHex.GetHex

        If _Deleted And Hex(0) <> &HE5 Then
            Hex(0) = &HE5
            _DeferredChange = Hex
            _HasDeferredChange = True
        End If

        Dim NewValue = DiskImage.CodePage437ToUnicode(Hex)

        _SuppressEvent = True
        If TxtFile.Text <> NewValue Then
            TxtFile.Text = NewValue
        End If
        _SuppressEvent = False
    End Sub
    Private Sub RadioFile_CheckedChanged(sender As Object, e As EventArgs) Handles RadioFileShort.CheckedChanged, RadioFileLong.CheckedChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        ToggleFileType(False)
    End Sub

    Private Sub TxtExtension_TextChanged(sender As Object, e As EventArgs) Handles TxtExtension.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim Value As String = Strings.Left(TxtExtension.Text, TxtExtension.MaxLength).PadRight(TxtExtension.MaxLength)
        Dim b = DiskImage.UnicodeToCodePage437(Value)

        MskExtensionHex.SetHex(b)
    End Sub

    Private Sub TxtFile_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtFile.KeyPress, TxtExtension.KeyPress
        Dim Value = Asc(e.KeyChar)

        If _IsVolumeLabel Then
            If Value < 32 And Value <> 8 Then
                e.KeyChar = Chr(0)
            End If
        Else
            If Value > 255 Then
                e.KeyChar = Chr(0)
            ElseIf Value < 33 And Value <> 8 Then
                e.KeyChar = Chr(0)
            ElseIf DiskImage.InvalidFileChars.Contains(Value) Then
                e.KeyChar = Chr(0)
            End If
        End If
    End Sub

    Private Sub TxtFile_TextChanged(sender As Object, e As EventArgs) Handles TxtFile.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim Value As String = Strings.Left(TxtFile.Text, TxtFile.MaxLength).PadRight(TxtFile.MaxLength)
        Dim b = DiskImage.UnicodeToCodePage437(Value)

        MskFileHex.SetHex(b)
    End Sub
    Private Sub TxtLFN_LostFocus(sender As Object, e As EventArgs) Handles TxtLFN.LostFocus
        TxtLFN.Text = Trim(TxtLFN.Text)
    End Sub

    Private Sub TxtLFN_Validating(sender As Object, e As CancelEventArgs) Handles TxtLFN.Validating
        Dim InvalidChars = IO.Path.GetInvalidFileNameChars
        Dim Value = TxtLFN.Text
        For i = 0 To Value.Length - 1
            If InvalidChars.Contains(Value.Substring(i, 1)) Then
                e.Cancel = True
                Dim msg As String = "A file name can't contain any of the following characters:" & vbCrLf & StrDup(16, " ") & "\ / :  * ? "" < > |"
                MsgBox(msg, MsgBoxStyle.Exclamation)
                Exit For
            End If
        Next
    End Sub
#End Region

End Class