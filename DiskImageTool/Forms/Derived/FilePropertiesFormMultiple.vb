Imports DiskImageTool.DiskImage
Public Class FilePropertiesFormMultiple
    Inherits FilePropertiesForm

    Private ReadOnly _Disk As Disk
    Private ReadOnly _DirectoryEntries As List(Of DirectoryEntry)
    Private _Updated As Boolean = False

    Public Sub New(Disk As DiskImage.Disk, DirectoryEntries As List(Of DirectoryEntry))
        MyBase.New()

        _Disk = Disk
        _DirectoryEntries = DirectoryEntries

        MyBase.Text = WithoutHotkey(My.Resources.Menu_FileProperties)
        BtnUpdate.Text = My.Resources.Menu_Update
    End Sub

    Public ReadOnly Property Updated As Boolean
        Get
            Return _Updated
        End Get
    End Property

    Public Shared Function Display(Disk As Disk, DirectoryEntries As List(Of DirectoryEntry)) As Boolean
        Using dlg As New FilePropertiesFormMultiple(Disk, DirectoryEntries)
            dlg.ShowDialog(App.CurrentFormInstance)

            If dlg.DialogResult = DialogResult.OK Then
                Return dlg.Updated
            Else
                Return False
            End If
        End Using
    End Function

    Private Sub ApplyUpdate()
        _Updated = False

        Dim UseTransaction = _Disk.BeginTransaction

        For Each DirectoryEntry In _DirectoryEntries
            Dim NewDirectoryEntry = DirectoryEntry.Clone

            ApplyFileDatesUpdate(NewDirectoryEntry)
            ApplyAttributesUpdate(NewDirectoryEntry)

            If Not DirectoryEntry.Data.CompareTo(NewDirectoryEntry.Data) Then
                DirectoryEntry.Data = NewDirectoryEntry.Data
                _Updated = True
            End If

            If DirectoryEntry.IsDirectory And Not DirectoryEntry.IsDeleted And Not DirectoryEntry.IsVolumeName Then
                DirectoryEntry.SubDirectory.UpdateLinkDates()
            End If
        Next

        If UseTransaction Then
            _Disk.EndTransaction()
        End If
    End Sub

    Private Sub PopulateForm()
        Dim DT As DiskImage.ExpandedDate
        Dim LastWritten As New Date(1980, 1, 1, 0, 0, 0)
        Dim Created As Date? = Nothing
        Dim LastAccessed As Date? = Nothing
        Dim SetArchived As Boolean = True
        Dim SetReadOnly As Boolean = True
        Dim SetSystem As Boolean = True
        Dim SetHidden As Boolean = True

        IsVolumeLabel = False
        Deleted = False

        GroupFileName.Text = My.Resources.Label_MultipleFiles
        LblMultipleFiles.Text = InParens(_DirectoryEntries.Count & " " & My.Resources.Label_FilesSelected)

        For Each DirectoryEntry In _DirectoryEntries
            DT = DirectoryEntry.GetLastWriteDate
            If DT.DateObject > LastWritten Then
                LastWritten = DT.DateObject
            End If
            DT = DirectoryEntry.GetCreationDate
            If Created Is Nothing Or DT.DateObject > Created Then
                Created = DT.DateObject
            End If
            DT = DirectoryEntry.GetLastAccessDate
            If LastAccessed Is Nothing Or DT.DateObject > LastAccessed Then
                LastAccessed = DT.DateObject
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

        InitMultiple(True)

        PopulateForm()

        SuppressEvent(False)
    End Sub
End Class
