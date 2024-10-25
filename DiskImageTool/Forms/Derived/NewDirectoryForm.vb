Imports DiskImageTool.DiskImage
Public Class NewDirectoryForm
    Inherits FilePropertiesForm

    Private _NewDirectoryEntry As DirectoryEntryBase
    Private _Updated As Boolean = False

    Public Sub New()
        MyBase.New()

        MyBase.Text = "New Directory"
        BtnUpdate.Text = "Add"
    End Sub

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

    Private Sub ApplyUpdate()
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

    Private Sub PopulateForm()
        Dim Maxlength As Integer = 8
        IsVolumeLabel = False
        Deleted = False

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
        ChkNTExtensions.Checked = False

        ToggleFileType(True)
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        ApplyUpdate()

        DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub NewDirectoryForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        SuppressEvent(True)

        SetCreatedDateValue(Nothing)
        SetLastAccessedDateValue(Nothing)
        SetLastWrittenDateValue(Now)

        InitMultiple(False)

        PopulateForm()

        SuppressEvent(False)
    End Sub
End Class
