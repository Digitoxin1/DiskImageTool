Imports System.ComponentModel
Imports System.Text
Imports DiskImageTool.DiskImage

Public Class FilePropertiesForm
    Private Const EMPTY_FORMAT As String = "'Empty'"
    Private _DeferredChange() As Byte
    Private _Deleted As Boolean = False
    Private _HasDeferredChange As Boolean = False
    Private _IsVolumeLabel As Boolean = False
    Private _NewDirectoryEntry As DirectoryEntryBase
    Private _SuppressEvent As Boolean = True

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Property Deleted As Boolean
        Get
            Return _Deleted
        End Get
        Set(value As Boolean)
            _Deleted = value
        End Set
    End Property

    Public ReadOnly Property HasLFN As Boolean
        Get
            Return RadioFileLong.Checked
        End Get
    End Property


    Public Property IsVolumeLabel As Boolean
        Get
            Return _IsVolumeLabel
        End Get
        Set(value As Boolean)
            _IsVolumeLabel = value
        End Set
    End Property

    Public ReadOnly Property LFN As String
        Get
            Return TxtLFN.Text
        End Get
    End Property

    Public Sub ApplyAttributesUpdate(DirectoryEntry As DiskImage.DirectoryEntryBase)
        If BtnArchive.Tag Then
            DirectoryEntry.IsArchive = ChkArchive.Checked
        End If
        If BtnReadOnly.Tag Then
            DirectoryEntry.IsReadOnly = ChkReadOnly.Checked
        End If
        If BtnHidden.Tag Then
            DirectoryEntry.IsHidden = ChkHidden.Checked
        End If
        If BtnSystem.Tag Then
            DirectoryEntry.IsSystem = ChkSystem.Checked
        End If
    End Sub

    Public Sub ApplyFileDatesUpdate(DirectoryEntry As DiskImage.DirectoryEntryBase)
        Dim LastWritten = GetDateFromPicker(DTLastWritten, DTLastWrittenTime).Value
        Dim Created As Date? = GetDateFromPicker(DTCreated, DTCreatedTime, NumCreatedMS)
        Dim LastAccessed As Date? = GetDateFromPicker(DTLastAccessed)
        Dim DT As DiskImage.ExpandedDate

        If BtnLastWritten.Tag Then
            DT = DirectoryEntry.GetLastWriteDate
            If Not DT.IsValidDate Or Date.Compare(DT.DateObject, LastWritten) <> 0 Then
                DirectoryEntry.SetLastWriteDate(LastWritten)
            End If
        End If

        If BtnCreated.Tag Then
            If DirectoryEntry.HasCreationDate() Then
                DT = DirectoryEntry.GetCreationDate
                If Created Is Nothing Then
                    DirectoryEntry.ClearCreationDate()
                ElseIf Not DT.IsValidDate Or Date.Compare(DT.DateObject, Created) <> 0 Then
                    DirectoryEntry.SetCreationDate(Created)
                End If
            ElseIf Created IsNot Nothing Then
                DirectoryEntry.SetCreationDate(Created)
            End If
        End If

        If BtnLastAccessed.Tag Then
            If DirectoryEntry.HasLastAccessDate() Then
                DT = DirectoryEntry.GetLastAccessDate
                If LastAccessed Is Nothing Then
                    DirectoryEntry.ClearLastAccessDate()
                ElseIf Not DT.IsValidDate Or Date.Compare(DT.DateObject, LastAccessed) <> 0 Then
                    DirectoryEntry.SetLastAccessDate(LastAccessed)
                End If
            ElseIf LastAccessed IsNot Nothing Then
                DirectoryEntry.SetLastAccessDate(LastAccessed)
            End If
        End If
    End Sub

    Public Sub ApplyFileNameUpdateShort(DirectoryEntry As DiskImage.DirectoryEntryBase)
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
        End If

        If Not Extension.CompareTo(DirectoryEntry.Extension) Then
            DirectoryEntry.Extension = Extension
        End If
    End Sub

    Public Sub InitMultiple(Multiple As Boolean)
        TxtFile.Visible = Not Multiple
        TxtExtension.Visible = Not Multiple
        LblMultipleFiles.Visible = Multiple
        MskFileHex.Visible = Not Multiple
        MskExtensionHex.Visible = Not Multiple
        FlowLayoutFileNameType.Visible = Not Multiple
        InitButtons(Multiple)
    End Sub

    Public Sub SetCreatedDateValue(Value? As Date)
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

    Public Sub SetLastAccessedDateValue(Value? As Date)
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

    Public Sub SetLastWrittenDateValue(Value As Date)
        DTLastWritten.Value = Value.Date
        DTLastWrittenTime.Value = Value
    End Sub

    Public Sub SuppressEvent(Value As Boolean)
        _SuppressEvent = Value
    End Sub

    Public Sub ToggleFileType(SetFocus As Boolean)
        If RadioFileShort.Checked Then
            TableLayoutPanelFile.Visible = True
            TxtLFN.Visible = False
            ChkNTExtensions.Enabled = False
            If SetFocus Then
                TxtFile.Select()
            End If
        Else
            TxtLFN.Visible = True
            TableLayoutPanelFile.Visible = False
            ChkNTExtensions.Enabled = True
            If SetFocus Then
                TxtLFN.SelectionStart = TxtLFN.Text.Length
                TxtLFN.Select()
            End If
        End If
    End Sub

    'Private Function CheckIfFileExists() As Boolean
    '    Dim Result = False
    '    Dim ParentDirectory As IDirectory
    '    Dim SkipIndex As Integer = -1

    '    If _Items Is Nothing Then
    '        ParentDirectory = _ParentDirectory
    '    ElseIf _Items.Count > 1 Then
    '        Return False
    '    Else
    '        Dim FileData As FileData = CType(_Items(0), ListViewItem).Tag
    '        Dim DirectoryEntry = FileData.DirectoryEntry

    '        If DirectoryEntry.IsValidVolumeName Or DirectoryEntry.IsDeleted Then
    '            Return False
    '        End If

    '        ParentDirectory = DirectoryEntry.ParentDirectory
    '        SkipIndex = DirectoryEntry.Index
    '    End If

    '    If RadioFileShort.Checked Then
    '        Dim FileBytes = New Byte(10) {32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32}
    '        MskFileHex.GetHex.CopyTo(FileBytes, 0)
    '        MskExtensionHex.GetHex.CopyTo(FileBytes, 8)
    '        Result = ParentDirectory.GetFileIndex(FileBytes, True, SkipIndex) > -1
    '    End If

    '    Return Result
    'End Function

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

    Private Function GetShortFilename() As String
        Dim FileNameBytes = MskFileHex.GetHex
        ResizeArray(FileNameBytes, 8, 32)
        Dim ExtensionBytes = MskExtensionHex.GetHex
        ResizeArray(ExtensionBytes, 3, 32)

        Dim FileName = Encoding.GetEncoding(437).GetString(FileNameBytes).TrimEnd(" ")
        Dim Extension = Encoding.GetEncoding(437).GetString(ExtensionBytes).TrimEnd(" ")

        If Extension <> "" Then
            FileName = FileName & "." & Extension
        End If

        Return FileName
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
            If Value > 255 Or (Value < 32 And Value <> 8) Then
                e.KeyChar = Chr(0)
            End If
        Else
            If Value > 96 And Value < 123 Then
                e.KeyChar = Chr(Value - 32)
            ElseIf Value > 255 Then
                e.KeyChar = Chr(0)
            ElseIf Value < 33 And Value <> 8 Then
                e.KeyChar = Chr(0)
            ElseIf DiskImage.DirectoryEntry.InvalidFileChars.Contains(Value) Then
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