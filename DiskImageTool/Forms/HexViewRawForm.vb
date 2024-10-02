Imports System.Runtime.InteropServices
Imports DiskImageTool.HexView
Imports DiskImageTool.DiskImage
Imports Hb.Windows.Forms

Public Class HexViewRawForm
    Private _IgnoreEvent As Boolean = False
    Private _Initialized As Boolean = False
    Private _CachedSelectedLength As Long = -1
    Private _CurrentSelectionLength As Long = -1
    Private _CurrentSelectionStart As Long = -1
    Private _DataGridInspector As HexViewDataGridInspector
    Private _Data() As Byte
    Private _StoredCellValue As String
    Private _LastSearch As HexSearch

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function AddClipboardFormatListener(hwnd As IntPtr) As Boolean
    End Function

    Public Sub New(Data() As Byte, Title As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.        
        _Data = Data
        HexBox1.ByteProvider = New DynamicByteProvider(Data)
        HexBox1.ReadOnly = True
        HexBox1.LineInfoOffset = 0
        ToolStripStatusBytes.Text = Format(Data.Length, "N0") & " bytes"
        _LastSearch = New HexSearch

        Me.Text = "Hex Viewer"
        If Title <> "" Then
            Me.Text = Me.Text & " - " & Title
        End If
    End Sub

    Private Sub HexViewRawForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        AddClipboardFormatListener(Me.Handle)

        _DataGridInspector = New HexViewDataGridInspector(DataGridDataInspector)
        _DataGridInspector.SetDataRow(DataRowEnum.File, Nothing, True, True)
        _DataGridInspector.SetDataRow(DataRowEnum.Description, Nothing, True, True)

        HexBox1.VScrollBarVisible = True


        RefreshSelection(True)
        DataInspectorRefresh(True)
    End Sub

    Private Sub CopyHexToClipboard()
        HexBox1.CopyHex()
    End Sub

    Private Sub CopyTextToClipboard()
        HexBox1.Copy()
    End Sub

    Private Sub DataInspectorRefresh(ForceUpdate As Boolean)
        If Not _Initialized Then
            Exit Sub
        End If

        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength As Long
        If _CachedSelectedLength = -1 Then
            SelectionLength = HexBox1.SelectionLength
        Else
            SelectionLength = _CachedSelectedLength
        End If

        If SelectionStart = _CurrentSelectionStart And SelectionLength = _CurrentSelectionLength And Not ForceUpdate Then
            Exit Sub
        End If

        _CurrentSelectionStart = SelectionStart
        _CurrentSelectionLength = SelectionLength

        Dim Bytes() As Byte = Nothing
        Dim SelectedBytes() As Byte = Nothing

        Dim Length As Long
        'Dim BigEndien As Boolean = RadioButtonBigEndien.Checked
        Dim BigEndien As Boolean = False
        Dim HasSelection = SelectionLength > 0
        If SelectionLength = 0 Then
            SelectionLength = 1
        End If
        Dim SelectionEnd = SelectionStart + SelectionLength - 1

        If SelectionStart > -1 Then
            Dim OutOfRange As Boolean = SelectionEnd >= HexBox1.ByteProvider.Length
            If Not OutOfRange Then
                If HasSelection Then
                    Length = Math.Min(SelectionLength, 8)
                Else
                    Length = Math.Min(HexBox1.ByteProvider.Length - SelectionStart, 8)
                End If

                SelectedBytes = New Byte(SelectionLength - 1) {}
                Bytes = New Byte(Length - 1) {}
                Array.Copy(_Data, SelectionStart, SelectedBytes, 0, SelectionLength)
                Array.Copy(_Data, SelectionStart, Bytes, 0, Length)
            End If
        End If

        _DataGridInspector.Refresh(Bytes, SelectedBytes, HexBox1.ReadOnly, BigEndien)

        DataInspectorRefreshButtons()
    End Sub

    Private Sub DataInspectorRefreshButtons()
        Dim Enabled As Boolean = False

        If DataGridDataInspector.SelectedRows.Count > 0 Then
            Dim Row = DataGridDataInspector.SelectedRows.Item(0)
            Dim Invalid As Boolean = Row.Cells.Item("DataGridInvalid").Value
            If Not Invalid Then
                Enabled = True
            End If
        End If

        BtnCopyValue.Enabled = Enabled
    End Sub

    Private Sub ProcessKeyPress(e As KeyEventArgs)
        If e.Control And e.KeyCode = Keys.C Then
            If HexBox1.CanCopy Then
                If e.Shift Then
                    CopyHexFormatted(HexBox1)
                Else
                    CopyHexToClipboard()
                End If
            End If
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub RefreshSelection(ForceUpdate As Boolean)
        If Not _Initialized And Not ForceUpdate Then
            Exit Sub
        End If

        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength = HexBox1.SelectionLength
        Dim SelectionEnd = SelectionStart + SelectionLength - 1

        If SelectionStart = -1 Then
            ToolStripStatusOffset.Visible = False
            ToolStripStatusBlock.Visible = False
            ToolStripStatusLength.Visible = False
        Else
            Dim OffsetStart As UInteger = HexBox1.LineInfoOffset + SelectionStart
            Dim OffsetEnd As Integer = HexBox1.LineInfoOffset + SelectionEnd
            Dim OutOfRange As Boolean = SelectionStart >= HexBox1.ByteProvider.Length

            Dim Sector = Disk.OffsetToSector(OffsetStart)

            ToolStripStatusOffset.Visible = Not OutOfRange
            ToolStripStatusOffset.Text = "Offset(h) :  " & OffsetStart.ToString("X")

            If SelectionLength = 0 Then
                ToolStripStatusBlock.Visible = False
                ToolStripStatusBlock.Text = ""
                ToolStripStatusLength.Visible = False
                ToolStripStatusLength.Text = ""
            Else
                ToolStripStatusBlock.Visible = True
                ToolStripStatusBlock.Text = "Block(h): " & OffsetStart.ToString("X") & "-" & OffsetEnd.ToString("X")
                ToolStripStatusLength.Visible = True
                ToolStripStatusLength.Text = "Length(h): " & SelectionLength.ToString("X") & "  (" & SelectionLength & ")"
            End If
        End If

        BtnCopyText.Enabled = HexBox1.CanCopy
        ToolStripBtnCopyText.Enabled = BtnCopyText.Enabled

        BtnCopyHex.Enabled = HexBox1.CanCopy
        ToolStripBtnCopyHex.Enabled = BtnCopyHex.Enabled

        BtnCopyHexFormatted.Enabled = HexBox1.CanCopy
        ToolStripBtnCopyHexFormatted.Enabled = BtnCopyHexFormatted.Enabled

        _Initialized = True
    End Sub

    Private Sub Search(FindNext As Boolean)
        Dim Result As Boolean = True
        Dim SelectionStart = HexBox1.SelectionStart
        Dim SelectionLength = HexBox1.SelectionLength
        Dim TopLine = HexBox1.StartByte \ HexBox1.BytesPerLine

        _IgnoreEvent = True

        If FindNext And _LastSearch.SearchString <> "" Then
            Result = SearchNext(_LastSearch)
        Else
            Dim frmHexSearchForm As New HexSearchForm(_LastSearch)
            frmHexSearchForm.ShowDialog()

            If frmHexSearchForm.DialogResult = DialogResult.OK Then
                _LastSearch = frmHexSearchForm.Search
                Result = SearchNext(_LastSearch)
            End If
        End If

        _IgnoreEvent = False

        If Not Result Then
            HexBox1.PerformScrollToLine(TopLine)
            HexBox1.SelectionStart = SelectionStart
            HexBox1.SelectionLength = SelectionLength
            MsgBox("Can't Find '" & _LastSearch.SearchString & "'", MsgBoxStyle.Information)
        Else
            RefreshSelection(False)
            DataInspectorRefresh(False)
        End If
    End Sub

    Private Function SearchNext(Search As HexSearch) As Boolean
        Dim Result As Boolean = True
        Dim DoSearch As Boolean = True

        If Search.SearchString <> "" Then
            Dim Options As New FindOptions()
            If Search.SearchHex Then
                Options.Type = FindType.Hex
                Options.Hex = ConvertHexToBytes(Search.SearchString)
                If Options.Hex Is Nothing Then
                    DoSearch = False
                End If
            Else
                Options.Type = FindType.Text
                Options.Text = Search.SearchString
                Options.MatchCase = Search.CaseSensitive
            End If
            If DoSearch Then
                Dim Index = HexBox1.Find(Options)
                If Index = -1 Then
                    If HexBox1.SelectionStart > 0 Then
                        HexBox1.SelectionStart = 0
                        HexBox1.SelectionLength = 0
                        Result = SearchNext(Search)
                    Else
                        Result = False
                    End If
                End If
            End If
        End If

        Return Result
    End Function

    Private Sub BtnCopyHex_Click(sender As Object, e As EventArgs) Handles BtnCopyHex.Click, ToolStripBtnCopyHex.Click
        If HexBox1.CanCopy Then
            CopyHexToClipboard()
        End If
    End Sub

    Private Sub BtnCopyHexFormatted_Click(sender As Object, e As EventArgs) Handles BtnCopyHexFormatted.Click, ToolStripBtnCopyHexFormatted.Click
        If HexBox1.CanCopy Then
            CopyHexFormatted(HexBox1)
        End If
    End Sub

    Private Sub BtnCopyText_Click(sender As Object, e As EventArgs) Handles BtnCopyText.Click, ToolStripBtnCopyText.Click
        If HexBox1.CanCopy Then
            CopyTextToClipboard()
        End If
    End Sub

    Private Sub BtnCopyValue_Click(sender As Object, e As EventArgs) Handles BtnCopyValue.Click
        _DataGridInspector.CopyValueToClipboard()
    End Sub

    Private Sub BtnFind_Click(sender As Object, e As EventArgs) Handles BtnFind.Click, ToolStripBtnFind.Click
        Search(False)
    End Sub

    Private Sub BtnFindNext_Click(sender As Object, e As EventArgs) Handles BtnFindNext.Click, ToolStripBtnFindNext.Click
        Search(True)
    End Sub

    Private Sub BtnSelectAll_Click(sender As Object, e As EventArgs) Handles BtnSelectAll.Click, ToolStripBtnSelectAll.Click
        HexBox1.SelectAll()
    End Sub

    Private Sub DataGridDataInspector_CellBeginEdit(sender As Object, e As DataGridViewCellCancelEventArgs) Handles DataGridDataInspector.CellBeginEdit
        Dim Row = DataGridDataInspector.Rows(e.RowIndex)
        Dim Invalid As Boolean = Row.Cells.Item("DataGridInvalid").Value
        Dim Editable As Boolean = Row.Cells.Item("DataGridEditable").Value
        Dim Length As Integer = Row.Cells.Item("DataGridLength").Value

        If Not Editable Then
            e.Cancel = True
            Exit Sub
        End If

        _CachedSelectedLength = HexBox1.SelectionLength
        HexBox1.SelectionLength = Length

        _StoredCellValue = Row.Cells.Item("DataGridValue").Value
    End Sub

    Private Sub DataGridDataInspector_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridDataInspector.CellEndEdit
        Dim Row = DataGridDataInspector.Rows(e.RowIndex)
        Dim CellValue As String = Row.Cells.Item("DataGridValue").Value
        Dim DataType As DataRowEnum = Row.Cells.Item("DataGridType").Value

        If CellValue <> _StoredCellValue Then
            _CachedSelectedLength = -1
            'DataInspectorUpdateHexBox(CellValue, DataType)
        Else
            HexBox1.SelectionLength = _CachedSelectedLength
            _CachedSelectedLength = -1
        End If
    End Sub

    Private Sub DataGridDataInspector_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridDataInspector.SelectionChanged
        DataInspectorRefreshButtons()
    End Sub

    Private Sub HexBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles HexBox1.KeyDown
        ProcessKeyPress(e)
    End Sub

    Private Sub HexBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles HexBox1.MouseDown
        If e.Button = MouseButtons.Right Then
            If HexBox1.SelectionLength < 2 Then
                HexBox1.SetCaretPosition(New Point(e.X, e.Y))
            End If
        End If
    End Sub

    Private Sub HexBox1_SelectionLengthChanged(sender As Object, e As EventArgs) Handles HexBox1.SelectionLengthChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        RefreshSelection(False)
        DataInspectorRefresh(False)
    End Sub

    Private Sub HexBox1_SelectionStartChanged(sender As Object, e As EventArgs) Handles HexBox1.SelectionStartChanged
        If _IgnoreEvent Then
            Exit Sub
        End If

        RefreshSelection(False)
        DataInspectorRefresh(False)
    End Sub
End Class