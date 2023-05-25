Public Class FilePropertiesForm
    Private Const EMPTY_FORMAT As String = "'Empty'"
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _Items As ICollection
    Private _Deleted As Boolean = False
    Private _IsDirectory As Boolean = False
    Private _IsVolumeLabel As Boolean = False
    Private _SuppressEvent As Boolean = True
    Private _Updated As Boolean = False

    Public Sub New(Disk As DiskImage.Disk, Items As ICollection)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk
        _Items = Items
    End Sub

    Public ReadOnly Property Updated As Boolean
        Get
            Return _Updated
        End Get
    End Property

    Private Sub ApplyAttributesUpdate(DirectoryEntry As DiskImage.DirectoryEntry)
        Dim Attributes As Byte = DirectoryEntry.Attributes
        Dim BitArray = New BitArray(Attributes)

        If BtnArchive.Tag Then
            Attributes = ToggleBit(Attributes, DiskImage.DirectoryEntry.AttributeFlags.ArchiveFlag, ChkArchive.Checked)
        End If
        If BtnReadOnly.Tag Then
            Attributes = ToggleBit(Attributes, DiskImage.DirectoryEntry.AttributeFlags.ReadOnly, ChkReadOnly.Checked)
        End If
        If BtnHidden.Tag Then
            Attributes = ToggleBit(Attributes, DiskImage.DirectoryEntry.AttributeFlags.Hidden, ChkHidden.Checked)
        End If
        If BtnSystem.Tag Then
            Attributes = ToggleBit(Attributes, DiskImage.DirectoryEntry.AttributeFlags.System, ChkSystem.Checked)
        End If

        If Attributes <> DirectoryEntry.Attributes Then
            DirectoryEntry.Attributes = Attributes
            _Updated = True
        End If
    End Sub
    Private Sub ApplyFileDatesUpdate(DirectoryEntry As DiskImage.DirectoryEntry)
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

    Private Sub ApplyFileNameUpdate(DirectoryEntry As DiskImage.DirectoryEntry)
        Dim FileName() As Byte
        Dim Extension() As Byte

        If _IsVolumeLabel Then
            Dim VolumeLabel() As Byte
            ReDim FileName(7)
            ReDim Extension(2)
            VolumeLabel = DiskImage.UnicodeToCodePage437(TxtFile.Text)
            DiskImage.ResizeArray(VolumeLabel, 11, 32)
            Array.Copy(VolumeLabel, 0, FileName, 0, 8)
            Array.Copy(VolumeLabel, 8, Extension, 0, 3)
        Else
            FileName = DiskImage.UnicodeToCodePage437(TxtFile.Text)
            DiskImage.ResizeArray(FileName, 8, 32)
            Extension = DiskImage.UnicodeToCodePage437(TxtExtension.Text)
            DiskImage.ResizeArray(Extension, 3, 32)
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

    Private Sub ApplyUpdates()
        _Updated = False

        _Disk.Data.BatchEditMode = True

        If _Items.Count = 1 Then
            Dim Item As ListViewItem = _Items(0)
            Dim FileData As FileData = Item.Tag
            Dim DirectoryEntry = FileData.DirectoryEntry
            ApplyFileNameUpdate(DirectoryEntry)
        End If

        For Each Item As ListViewItem In _Items
            Dim FileData As FileData = Item.Tag
            Dim DirectoryEntry = FileData.DirectoryEntry

            ApplyFileDatesUpdate(DirectoryEntry)
            ApplyAttributesUpdate(DirectoryEntry)
        Next

        _Disk.Data.BatchEditMode = False
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

        _IsDirectory = False
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
    End Sub

    Private Sub PopulateFormSingle()
        Dim Item As ListViewItem = _Items(0)
        Dim FileData As FileData = Item.Tag
        Dim DirectoryEntry = FileData.DirectoryEntry
        Dim DT As DiskImage.ExpandedDate

        _IsDirectory = DirectoryEntry.IsDirectory And Not DirectoryEntry.IsVolumeName
        _IsVolumeLabel = DirectoryEntry.IsValidValumeName
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
        Else
            Maxlength = 8
            TxtExtension.Visible = True
            MskExtensionHex.Visible = True
            FileName = DirectoryEntry.GetFileName
            FileNameHex = DirectoryEntry.FileName
            TxtExtension.Text = DirectoryEntry.GetFileExtension
            MskExtensionHex.SetHex(DirectoryEntry.Extension)
            If _IsDirectory Then
                Caption = "Directory Name"
            Else
                Caption = "File Name"
            End If
        End If

        If _Deleted Then
            Caption &= " (Deleted)"
            Maxlength -= 1
            FileName = FileName.Substring(1)
            ReDim FileNameHex(Maxlength - 1)
            Array.Copy(DirectoryEntry.FileName, 1, FileNameHex, 0, Maxlength)
        End If

        GroupFileName.Text = Caption
        TxtFile.MaxLength = Maxlength
        TxtFile.Text = FileName
        MskFileHex.MaskLength = Maxlength
        MskFileHex.Width = (Maxlength * 3 - 1) * 7 + 8
        MskFileHex.SetHex(FileNameHex)
        TxtFile.Width = MskFileHex.Width

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

    Private Function ToggleBit(Attributes As Byte, Flag As DiskImage.DirectoryEntry.AttributeFlags, Value As Boolean) As Byte
        If Value Then
            Return Attributes Or Flag
        Else
            Return Attributes And Not Flag
        End If
    End Function

    Private Sub ToggleButton(Button As Button, Enabled As Boolean, Optional Visible As Boolean = True)
        Button.Visible = Visible
        Button.Tag = Enabled
        Button.BackColor = IIf(Enabled, Color.LightGreen, SystemColors.Control)
        Button.UseVisualStyleBackColor = Not Enabled
        ToggleRelatedControls(Button)
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
        ApplyUpdates()
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

        DTLastWritten.Value = New Date(1980, 1, 1)
        DTLastWrittenTime.Value = New Date(1980, 1, 1, 0, 0, 0)
        SetCreatedDateValue(Nothing)
        SetLastAccessedDateValue(Nothing)

        If _Items.Count = 1 Then
            PopulateFormSingle()
        Else
            PopulateFormMultiple()
        End If
        InitMultiple(_Items.Count > 1)

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

    Private Sub MskFileHex_TextChanged(sender As Object, e As EventArgs) Handles MskFileHex.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim NewValue = DiskImage.CodePage437ToUnicode(MskFileHex.GetHex)

        _SuppressEvent = True
        If TxtFile.Text <> NewValue Then
            TxtFile.Text = NewValue
        End If
        _SuppressEvent = False
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
            If Value > 96 And Value < 123 Then
                Value -= 32
                e.KeyChar = Chr(Value)
            ElseIf Value > 255 Then
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

#End Region

End Class