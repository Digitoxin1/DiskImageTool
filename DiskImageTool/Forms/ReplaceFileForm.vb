Imports DiskImageTool.DiskImage

Public Class ReplaceFileForm
    Private ReadOnly _AvailableSpace As UInteger
    Private ReadOnly _Directory As IDirectory
    Private _FileName As String
    Private _FileNameOriginal As String
    Private _FileDateOriginal As Date
    Private _FileSizeOriginal As UInteger
    Private _FileNameNew As String
    Private _FileDateNew As Date
    Private _FileSizeNew As UInteger
    Private _IgnoreEvent As Boolean = False
    Private _Result As ReplaceFileFormResult

    Public Structure ReplaceFileFormResult
        Dim Cancelled As Boolean
        Dim FileName As String
        Dim FileNameChanged As Boolean
        Dim FileDate As Date
        Dim FileDateChanged As Boolean
        Dim FileSize As UInteger
        Dim FileSizeChanged As Boolean
        Dim FillChar As Byte
    End Structure

    Public Sub New(AvailableSpace As UInteger, Directory As IDirectory)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Result.Cancelled = True
        _AvailableSpace = AvailableSpace
        _Directory = Directory

        _IgnoreEvent = True
        ChkFilenameOriginal.Checked = False
        ChkFileDateOriginal.Checked = False
        ChkFileSizeOriginal.Checked = False

        ChkFilenameNew.Checked = True
        ChkFileDateNew.Checked = True
        ChkFileSizeNew.Checked = True
        _IgnoreEvent = False
    End Sub

    Public ReadOnly Property Result As ReplaceFileFormResult
        Get
            Return _Result
        End Get
    End Property

    Public Sub GetFileNameForm()
        _FileNameNew = TxtFilenameNew.Text
        If TxtFileExtNew.Text.Length > 0 Then
            _FileNameNew &= "." & TxtFileExtNew.Text
        End If
    End Sub

    Public Sub SetFileNameForm()
        _IgnoreEvent = True

        Dim FileParts = SplitFilename(_FileNameNew)

        TxtFilenameNew.Text = FileParts.Name
        TxtFileExtNew.Text = FileParts.Extension

        _IgnoreEvent = False
    End Sub

    Public Sub SetOriginalFile(Filename As String, FileDate As Date, FileSize As UInteger)
        _FileNameOriginal = Filename
        _FileDateOriginal = FileDate
        _FileSizeOriginal = FileSize

        ChkFilenameOriginal.Text = Filename
        ChkFileDateOriginal.Text = FileDate
        ChkFileSizeOriginal.Text = Format(FileSize, "N0") & " bytes"
    End Sub

    Public Sub SetNewFile(Filename As String, FileDate As Date, FileSize As UInteger)
        _FileName = Filename
        _FileNameNew = Filename
        _FileDateNew = FileDate
        _FileSizeNew = FileSize

        ChkFileDateNew.Text = FileDate
        ChkFileSizeNew.Text = Format(FileSize, "N0") & " bytes"

        SetFileNameForm()
        BtnUndo.Visible = False
    End Sub

    Public Sub RefreshText()
        Dim AdjustmentType As String

        If ChkFilenameOriginal.Checked Then
            LblFileName.Text = _FileNameOriginal
        Else
            LblFileName.Text = _FileNameNew
        End If

        If ChkFileDateOriginal.Checked Then
            LblFileDate.Text = _FileDateOriginal
        Else
            LblFileDate.Text = _FileDateNew
        End If

        FlowLayoutPad.Visible = False
        LblFileSizeError.Text = ""
        LblFileSizeError.Visible = False
        BtnOK.Enabled = True

        If ChkFileSizeOriginal.Checked Then
            LblFileSize.Text = Format(_FileSizeOriginal, "N0") & " bytes"
            If _FileSizeNew = _FileSizeOriginal Then
                AdjustmentType = "set"
            ElseIf _FileSizeNew > _FileSizeOriginal Then
                AdjustmentType = "truncated"
            Else
                AdjustmentType = "padded"
                FlowLayoutPad.Visible = True
                LblPadCaption.Text = "Pad file with:"
            End If
        Else
            LblFileSize.Text = Format(_FileSizeNew, "N0") & " bytes"
            If _FileSizeOriginal = _FileSizeNew Then
                AdjustmentType = "set"
            ElseIf _FileSizeOriginal > _FileSizeNew Then
                AdjustmentType = "reduced"
                FlowLayoutPad.Visible = True
                LblPadCaption.Text = "Fill free space with:"
            Else
                AdjustmentType = "expanded"
                If _FileSizeNew > _AvailableSpace Then
                    LblFileSizeError.Text = "(Not enough available space on disk)"
                    LblFileSizeError.Visible = True
                    BtnOK.Enabled = False
                End If
            End If
        End If
        LblFileSizeCaption.Text = "File size will be " & AdjustmentType & " to:"
    End Sub

    Private Sub ToggleCheckBox(Current As CheckBox, Linked As CheckBox)
        _IgnoreEvent = True

        If Not Current.Checked Then
            Current.Checked = True
        Else
            Linked.Checked = False
            RefreshText()
        End If

        _IgnoreEvent = False
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        If ChkFilenameNew.Checked Then
            If TxtFilenameNew.Text.Length = 0 Then
                MsgBox("Filename cannot be blank.", MsgBoxStyle.Exclamation)
                TxtFilenameNew.Focus()
                Exit Sub
            End If

            If _FileNameNew <> _FileNameOriginal AndAlso _Directory.FindShortFileName(_FileNameNew, True) > -1 Then
                MsgBox("A file with this name already exists in this directory.", MsgBoxStyle.Exclamation)
                TxtFilenameNew.Focus()
                Exit Sub
            End If
        End If

        _Result.Cancelled = False
        If ChkFilenameNew.Checked Then
            _Result.FileName = _FileNameNew
            _Result.FileNameChanged = (_FileNameNew <> _FileNameOriginal)
        Else
            _Result.FileName = _FileNameOriginal
            _Result.FileNameChanged = False
        End If

        If ChkFileDateNew.Checked Then
            _Result.FileDate = _FileDateNew
            _Result.FileDateChanged = (DateDiff(DateInterval.Second, _FileDateOriginal, _FileDateNew) <> 0)
        Else
            _Result.FileDate = _FileDateOriginal
            _Result.FileDateChanged = False
        End If

        If ChkFileSizeNew.Checked Then
            _Result.FileSize = _FileSizeNew
            _Result.FileSizeChanged = (_FileSizeNew <> _FileSizeOriginal)
        Else
            _Result.FileSize = _FileSizeOriginal
            _Result.FileSizeChanged = False
        End If

        If RadioFillF6.Checked Then
            _Result.FillChar = &HF6
        Else
            _Result.FillChar = &H0
        End If

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        _Result.Cancelled = True
    End Sub

    Private Sub BtnUndo_Click(sender As Object, e As EventArgs) Handles BtnUndo.Click
        _FileNameNew = _FileName
        SetFileNameForm()
        BtnUndo.Visible = False
        If ChkFilenameNew.Checked Then
            LblFileName.Text = _FileNameNew
        End If
    End Sub

    Private Sub ChkFilenameOriginal_CheckedChanged(sender As Object, e As EventArgs) Handles ChkFilenameOriginal.CheckedChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        ToggleCheckBox(ChkFilenameOriginal, ChkFilenameNew)
    End Sub

    Private Sub ChkFilenameNew_CheckedChanged(sender As Object, e As EventArgs) Handles ChkFilenameNew.CheckedChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        ToggleCheckBox(ChkFilenameNew, ChkFilenameOriginal)
    End Sub

    Private Sub ChkFileSizeNew_CheckedChanged(sender As Object, e As EventArgs) Handles ChkFileSizeNew.CheckedChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        ToggleCheckBox(ChkFileSizeNew, ChkFileSizeOriginal)
    End Sub

    Private Sub ChkFileSizeOriginal_CheckedChanged(sender As Object, e As EventArgs) Handles ChkFileSizeOriginal.CheckedChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        ToggleCheckBox(ChkFileSizeOriginal, ChkFileSizeNew)
    End Sub

    Private Sub ChkFileDateNew_CheckedChanged(sender As Object, e As EventArgs) Handles ChkFileDateNew.CheckedChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        ToggleCheckBox(ChkFileDateNew, ChkFileDateOriginal)
    End Sub

    Private Sub ChkFileDateOriginal_CheckedChanged(sender As Object, e As EventArgs) Handles ChkFileDateOriginal.CheckedChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        ToggleCheckBox(ChkFileDateOriginal, ChkFileDateNew)
    End Sub

    Private Sub TxtFilenameNew_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtFilenameNew.KeyPress, TxtFileExtNew.KeyPress
        Dim Value = Asc(e.KeyChar)

        If Value > 255 Then
            e.KeyChar = Chr(0)
        ElseIf Value < 33 And Value <> 8 Then
            e.KeyChar = Chr(0)
        ElseIf DirectoryEntry.InvalidFileChars.Contains(Value) Then
            e.KeyChar = Chr(0)
        End If
    End Sub

    Private Sub TxtFilenameNew_TextChanged(sender As Object, e As EventArgs) Handles TxtFilenameNew.TextChanged, TxtFileExtNew.TextChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        GetFileNameForm()

        BtnUndo.Visible = _FileNameNew <> _FileName

        If ChkFilenameNew.Checked Then
            LblFileName.Text = _FileNameNew
        End If
    End Sub
End Class