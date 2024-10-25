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
        Dim LastWritten = New Date(1980, 1, 1, 0, 0, 0)
        Dim Created As Date? = Nothing
        Dim LastAccessed As Date? = Nothing
        Dim SetArchived As Boolean = True
        Dim SetReadOnly As Boolean = True
        Dim SetSystem As Boolean = True
        Dim SetHidden As Boolean = True

        IsVolumeLabel = False
        Deleted = False

        GroupFileName.Text = "Multiple Files"
        LblMultipleFiles.Text = "(" & _DirectoryEntries.Count & " Files Selected)"

        For Each DirectoryEntry In _DirectoryEntries
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
