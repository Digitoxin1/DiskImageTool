Imports DiskImageTool.DiskImage

Public Class ReplaceFileForm
    Private ReadOnly _AvailableSpace As UInteger
    Private ReadOnly _Directory As IDirectory
    Private _FileDateNew As Date
    Private _FileDateOriginal As Date
    Private _FileName As String
    Private _FileNameNew As String
    Private _FileNameOriginal As String
    Private _FileSizeNew As UInteger
    Private _FileSizeOriginal As UInteger
    Private _IgnoreEvent As Boolean = False
    Private _Result As ReplaceFileFormResult

    Public Sub New(AvailableSpace As UInteger, Directory As IDirectory)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

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

    Public Shared Function Display(
                                  AvailableSpace As UInteger,
                                  Directory As IDirectory,
                                  OriginalFilename As String,
                                  OriginalFileDate As Date,
                                  OriginalFileSize As UInteger,
                                  NewFilename As String,
                                  NewFileDate As Date,
                                  NewFileSize As UInteger
                                  ) As ReplaceFileFormResult

        Using Form As New ReplaceFileForm(AvailableSpace, Directory)
            With Form
                .SetOriginalFile(OriginalFilename, OriginalFileDate, OriginalFileSize)
                .SetNewFile(NewFilename, NewFileDate, NewFileSize)
                .RefreshText()
                .ShowDialog()
                Return .Result
            End With
        End Using
    End Function

    Public Sub GetFileNameForm()
        _FileNameNew = TxtFilenameNew.Text
        If TxtFileExtNew.Text.Length > 0 Then
            _FileNameNew &= "." & TxtFileExtNew.Text
        End If
    End Sub

    Public Sub RefreshText()
        Dim FileSizeCaption As String

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
            LblFileSize.Text = FormatThousands(_FileSizeOriginal) & " " & My.Resources.Label_Bytes
            If _FileSizeNew = _FileSizeOriginal Then
                FileSizeCaption = My.Resources.Label_FileSizeSet
            ElseIf _FileSizeNew > _FileSizeOriginal Then
                FileSizeCaption = My.Resources.Label_FileSizeTruncated
            Else
                FileSizeCaption = My.Resources.Label_FileSizePadded
                FlowLayoutPad.Visible = True
                LblPadCaption.Text = My.Resources.Label_PadFile & ":"
            End If
        Else
            LblFileSize.Text = FormatThousands(_FileSizeNew) & " " & My.Resources.Label_Bytes
            If _FileSizeOriginal = _FileSizeNew Then
                FileSizeCaption = My.Resources.Label_FileSizeSet
            ElseIf _FileSizeOriginal > _FileSizeNew Then
                FileSizeCaption = My.Resources.Label_FileSizeReduced
                FlowLayoutPad.Visible = True
                LblPadCaption.Text = My.Resources.Label_FillFreeSpace & ":"
            Else
                FileSizeCaption = My.Resources.Label_FileSizeExpanded
                If _FileSizeNew > _AvailableSpace Then
                    LblFileSizeError.Text = InParens(My.Resources.Label_NotEnoughSpace)
                    LblFileSizeError.Visible = True
                    BtnOK.Enabled = False
                End If
            End If
        End If
        LblFileSizeCaption.Text = FileSizeCaption & ":"
    End Sub

    Public Sub SetFileNameForm()
        _IgnoreEvent = True

        Dim FileParts = SplitFilename(_FileNameNew)

        TxtFilenameNew.Text = FileParts.Name
        TxtFileExtNew.Text = FileParts.Extension

        _IgnoreEvent = False
    End Sub

    Public Sub SetNewFile(Filename As String, FileDate As Date, FileSize As UInteger)
        _FileName = Filename
        _FileNameNew = Filename
        _FileDateNew = FileDate
        _FileSizeNew = FileSize

        ChkFileDateNew.Text = FileDate
        ChkFileSizeNew.Text = FormatThousands(FileSize) & " " & My.Resources.Label_Bytes

        SetFileNameForm()
        BtnUndo.Visible = False
    End Sub

    Public Sub SetOriginalFile(Filename As String, FileDate As Date, FileSize As UInteger)
        _FileNameOriginal = Filename
        _FileDateOriginal = FileDate
        _FileSizeOriginal = FileSize

        ChkFilenameOriginal.Text = Filename
        ChkFileDateOriginal.Text = FileDate
        ChkFileSizeOriginal.Text = FormatThousands(FileSize) & " " & My.Resources.Label_Bytes
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        _Result.Cancelled = True
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        If ChkFilenameNew.Checked Then
            If TxtFilenameNew.Text.Length = 0 Then
                MsgBox(My.Resources.Dialog_FileBlankWarning, MsgBoxStyle.Exclamation)
                TxtFilenameNew.Focus()
                Exit Sub
            End If

            If _FileNameNew <> _FileNameOriginal AndAlso _Directory.FindShortFileName(_FileNameNew, True) > -1 Then
                MsgBox(My.Resources.Dialog_FileExists, MsgBoxStyle.Exclamation)
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

    Private Sub BtnUndo_Click(sender As Object, e As EventArgs) Handles BtnUndo.Click
        _FileNameNew = _FileName
        SetFileNameForm()
        BtnUndo.Visible = False
        If ChkFilenameNew.Checked Then
            LblFileName.Text = _FileNameNew
        End If
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

    Private Sub ChkFilenameNew_CheckedChanged(sender As Object, e As EventArgs) Handles ChkFilenameNew.CheckedChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        ToggleCheckBox(ChkFilenameNew, ChkFilenameOriginal)
    End Sub

    Private Sub ChkFilenameOriginal_CheckedChanged(sender As Object, e As EventArgs) Handles ChkFilenameOriginal.CheckedChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        ToggleCheckBox(ChkFilenameOriginal, ChkFilenameNew)
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

    Private Sub LocalizeForm()
        BtnCancel.Text = WithoutHotkey(My.Resources.Menu_Cancel)
        BtnOK.Text = My.Resources.Label_Replace
        GroupBox1.Text = My.Resources.Label_NewFile
        GroupBoxOriginal.Text = My.Resources.Label_OriginalFile
        LblFileDateCaption.Text = My.Resources.Caption_LastWrittenDateSetTo & ":"
        LblFileNameCaption.Text = My.Resources.Caption_FileNameSetTo & ":"
        LblFileSizeCaption.Text = My.Resources.Label_FileSizeSet & ":"
        LblFileSizeError.Text = "(" & My.Resources.Label_Error & ")"
        LblPadCaption.Text = My.Resources.Label_PadFile & ":"
        Me.Text = My.Resources.Label_ReplaceFile
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

    Public Structure ReplaceFileFormResult
        Dim Cancelled As Boolean
        Dim FileDate As Date
        Dim FileDateChanged As Boolean
        Dim FileName As String
        Dim FileNameChanged As Boolean
        Dim FileSize As UInteger
        Dim FileSizeChanged As Boolean
        Dim FillChar As Byte
    End Structure
End Class