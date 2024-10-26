Imports DiskImageTool.DiskImage
Public Class FilePropertiesFormSingle
    Inherits FilePropertiesForm

    Private ReadOnly _DirectoryEntry As DirectoryEntry
    Private _Updated As Boolean = False

    Public Sub New(DirectoryEntry As DirectoryEntry)
        MyBase.New()

        _DirectoryEntry = DirectoryEntry

        MyBase.Text = "File Properties"
        BtnUpdate.Text = "Update"
    End Sub

    Public ReadOnly Property Updated As Boolean
        Get
            Return _Updated
        End Get
    End Property

    Private Sub ApplyUpdate()
        _Updated = False

        Dim UseTransaction = _DirectoryEntry.Disk.BeginTransaction

        Dim NewDirectoryEntry = _DirectoryEntry.Clone

        If RadioFileShort.Checked Then
            ApplyFileNameUpdateShort(NewDirectoryEntry)
            NewDirectoryEntry.RemoveNTExtensions()
        End If

        ApplyFileDatesUpdate(NewDirectoryEntry)
        ApplyAttributesUpdate(NewDirectoryEntry)

        If Not _DirectoryEntry.Data.CompareTo(NewDirectoryEntry.Data) Then
            _DirectoryEntry.Data = NewDirectoryEntry.Data
            _Updated = True
        End If

        If _DirectoryEntry.IsDirectory And Not _DirectoryEntry.IsDeleted And Not _DirectoryEntry.IsVolumeName Then
            _DirectoryEntry.SubDirectory.UpdateLinkDates()
        End If

        If RadioFileShort.Checked Then
            If Not _DirectoryEntry.IsVolumeName And Not _DirectoryEntry.IsDeleted And _DirectoryEntry.HasLFN Then
                If _DirectoryEntry.RemoveLFN() Then
                    _Updated = True
                End If
            End If
        Else
            Dim UseNTExtensions = ChkNTExtensions.Checked
            If _DirectoryEntry.ParentDirectory.UpdateLFN(TxtLFN.Text, _DirectoryEntry.Index, UseNTExtensions) Then
                _Updated = True
            End If
        End If

        If UseTransaction Then
            _DirectoryEntry.Disk.EndTransaction()
        End If
    End Sub

    Private Sub PopulateForm()
        Dim DT As DiskImage.ExpandedDate

        IsVolumeLabel = _DirectoryEntry.IsVolumeName
        Deleted = _DirectoryEntry.IsDeleted

        Dim Caption As String
        Dim Maxlength As Integer
        Dim FileName As String
        Dim FileNameHex() As Byte

        RadioFileShort.Checked = True

        If IsVolumeLabel Then
            Caption = "Volume Label"
            Maxlength = 11
            TxtExtension.Visible = False
            MskExtensionHex.Visible = False
            FileName = _DirectoryEntry.GetVolumeName
            ReDim FileNameHex(Maxlength - 1)
            _DirectoryEntry.FileName.CopyTo(FileNameHex, 0)
            _DirectoryEntry.Extension.CopyTo(FileNameHex, 8)
            FlowLayoutFileNameType.Visible = False
        Else
            Dim IsDirectory = _DirectoryEntry.IsDirectory And Not _DirectoryEntry.IsVolumeName

            If Not Deleted Then
                If _DirectoryEntry.HasLFN Then
                    TxtLFN.Text = _DirectoryEntry.GetLongFileName
                    RadioFileLong.Checked = True
                    ChkNTExtensions.Checked = False
                    ChkNTExtensions.Enabled = True
                ElseIf Not _DirectoryEntry.HasNTUnknownFlags And (_DirectoryEntry.HasNTLowerCaseExtension Or _DirectoryEntry.HasNTLowerCaseFileName) Then
                    TxtLFN.Text = _DirectoryEntry.GetNTFileName
                    RadioFileLong.Checked = True
                    ChkNTExtensions.Checked = True
                    ChkNTExtensions.Enabled = True
                Else
                    TxtLFN.Text = _DirectoryEntry.GetFullFileName
                    ChkNTExtensions.Checked = False
                    ChkNTExtensions.Enabled = False
                End If
            End If

            Maxlength = 8
            TxtExtension.Visible = True
            MskExtensionHex.Visible = True
            FileName = _DirectoryEntry.GetFileName(True)
            FileNameHex = _DirectoryEntry.FileName
            TxtExtension.Text = _DirectoryEntry.GetFileExtension(True)
            MskExtensionHex.SetHex(_DirectoryEntry.Extension)
            If IsDirectory Then
                Caption = "Directory Name"
            Else
                Caption = "File Name"
            End If
            FlowLayoutFileNameType.Visible = Not Deleted
        End If

        MskFileHex.MaskLength = Maxlength
        MskFileHex.Width = (Maxlength * 3 - 1) * 7 + 8
        MskFileHex.SetHex(FileNameHex)
        TxtFile.PromptChar = " "
        TxtFile.Width = MskFileHex.Width

        If Deleted Then
            Caption &= " (Deleted)"
            TxtFile.Mask = "\" & FileName.Substring(0, 1) & Strings.StrDup(Maxlength - 1, "C")
            TxtFile.Text = FileName.Substring(1)
        Else
            TxtFile.Mask = Strings.StrDup(Maxlength, "C")
            TxtFile.Text = FileName
        End If

        GroupFileName.Text = Caption

        DT = _DirectoryEntry.GetLastWriteDate
        If DT.IsValidDate Then
            DTLastWritten.Value = DT.DateObject.Date
            DTLastWrittenTime.Value = DT.DateObject
        Else
            DTLastWritten.Value = New Date(1980, 1, 1)
            DTLastWrittenTime.Value = New Date(1980, 1, 1, 0, 0, 0)
        End If

        If _DirectoryEntry.HasCreationDate() Then
            DT = _DirectoryEntry.GetCreationDate
            If DT.IsValidDate Then
                SetCreatedDateValue(DT.DateObject)
            End If
        End If

        If _DirectoryEntry.HasLastAccessDate() Then
            DT = _DirectoryEntry.GetLastAccessDate
            If DT.IsValidDate Then
                SetLastAccessedDateValue(DT.DateObject)
            End If
        End If

        ChkArchive.Checked = _DirectoryEntry.IsArchive
        ChkReadOnly.Checked = _DirectoryEntry.IsReadOnly
        ChkHidden.Checked = _DirectoryEntry.IsHidden
        ChkSystem.Checked = _DirectoryEntry.IsSystem

        ToggleFileType(True)
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        ApplyUpdate()

        DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub FilePropertiesEditForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        SuppressEvent(True)

        SetCreatedDateValue(Nothing)
        SetLastAccessedDateValue(Nothing)
        SetLastWrittenDateValue(New Date(1980, 1, 1, 0, 0, 0))

        InitMultiple(False)

        PopulateForm()

        SuppressEvent(False)
    End Sub
End Class
