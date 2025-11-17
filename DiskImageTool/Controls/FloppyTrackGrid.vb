Imports System.ComponentModel
Imports System.Drawing.Text
Imports System.Windows.Forms.VisualStyles

Public Class FloppyTrackGrid
    Inherits Control

    Private Const CELL_HEIGHT As Integer = 21
    Private Const CELL_WIDTH As Integer = 20
    Private Const COLUMNS As Integer = 10
    Private Const FOOTER_MARGIN As Integer = 2

    Private Shared ReadOnly BORDER_PEN As Pen = SystemPens.ControlDark
    Private Shared ReadOnly DEFAULT_BACKCOLOR As Color = Color.White
    Private Shared ReadOnly DEFAULT_FORECOLOR As Color = SystemColors.ControlText
    Private Shared ReadOnly DISABLED_COLOR As Color = Color.LightGray
    Private Shared ReadOnly GRID_FONT As New Font("Segoe UI", 8.0F, FontStyle.Regular)
    Private Shared ReadOnly HEADER_FONT As New Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular)
    Private Shared ReadOnly SELECTED_BACKCOLOR As Color = Color.LightSkyBlue
    Private ReadOnly _SelectedTracks As HashSet(Of UShort)
    Private ReadOnly _ToolTip As ToolTip
    Private _ActiveTrackCount As Integer
    Private _Cells As List(Of CellInfo)

    Private _Disabled As Boolean = False
    Private _FocusedTrack As Integer = -1
    Private _FooterCheckRect As Rectangle
    Private _FooterHasFocus As Boolean = False
    Private _FooterTextRect As Rectangle
    Private _IsChecked As Boolean
    Private _Label As String
    Private _LastSelected As (Track As Integer, Selected As Boolean) = (-1, False)
    Private _NoEventSelectionChanged As Boolean = False
    Private _SelectEnabled As Boolean = False
    Private _Side As Byte
    Private _TrackCount As Integer
    Public Event CellClicked(sender As Object, Track As Integer, Row As Integer, Col As Integer, Shift As Boolean)
    Public Event CheckChanged(sender As Object, Checked As Boolean)
    Public Event SelectionChanged(sender As Object, Track As Integer, Selected As Boolean)

    Public Sub New()
        MyBase.New()

        SetStyle(ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or
                 ControlStyles.Selectable Or
                 ControlStyles.UserPaint, True)

        DoubleBuffered = True
        TabStop = False

        _ToolTip = New ToolTip()
        _SelectedTracks = New HashSet(Of UShort)
        _Cells = New List(Of CellInfo)
    End Sub

    Public Sub New(TrackCount As Integer, Side As Byte)
        Me.New()

        _ActiveTrackCount = TrackCount
        Me.TrackCount = TrackCount
        Me.Side = Side
    End Sub

    <Browsable(True)>
    <DefaultValue(0)>
    Public Property ActiveTrackCount As Integer
        Get
            Return _ActiveTrackCount
        End Get

        Set(value As Integer)
            If value < 0 Then
                value = 0
            ElseIf value > _TrackCount Then
                value = _TrackCount
            End If

            EnsureFocusedTrackValid()

            If _ActiveTrackCount <> value Then
                _ActiveTrackCount = value

                Invalidate()
            End If
        End Set
    End Property

    ' Checkbox checked state
    <Browsable(True)>
    <DefaultValue(False)>
    Public Property Disabled As Boolean
        Get
            Return _Disabled
        End Get
        Set(value As Boolean)
            If _Disabled <> value Then
                _Disabled = value
                Invalidate()
            End If
        End Set
    End Property

    ' Checkbox checked state
    <Browsable(True)>
    <DefaultValue(False)>
    Public Property IsChecked As Boolean
        Get
            Return _IsChecked
        End Get
        Set(value As Boolean)
            If _IsChecked <> value Then
                _IsChecked = value
                InvalidateFooter()

                EventCheckChanged(value)
            End If
        End Set
    End Property

    ' Footer label text (what you pass in the New overload)
    <Browsable(True)>
    <DefaultValue(0)>
    Public Property Side As Byte
        Get
            Return _Side
        End Get
        Set(value As Byte)
            _Side = value
            _Label = My.Resources.Label_Side & " " & _Side
            InvalidateFooter()
        End Set
    End Property

    Public ReadOnly Property SelectedTracks As HashSet(Of UShort)
        Get
            Return _SelectedTracks
        End Get
    End Property

    <Browsable(True)>
    <DefaultValue(False)>
    Public Property SelectEnabled As Boolean
        Get
            Return _SelectEnabled
        End Get
        Set(value As Boolean)
            If _SelectEnabled <> value Then
                _SelectEnabled = value

                ' make the control focusable only when selection is allowed
                Me.TabStop = value

                If Not value Then
                    _IsChecked = False
                    ClearSelected()
                End If

                InvalidateFooter()
            End If
        End Set
    End Property

    <Browsable(True)>
    <DefaultValue(0)>
    Public Property TrackCount As Integer
        Get
            Return _TrackCount
        End Get

        Set(value As Integer)
            If value < 0 Then
                value = 0
            End If

            If _TrackCount <> value Then
                _TrackCount = value

                If _ActiveTrackCount > _TrackCount Then
                    _ActiveTrackCount = _TrackCount
                End If

                EnsureFocusedTrackValid()

                ResizeCellList(_TrackCount)

                UpdateSizeFromTrackCount()

                Invalidate()
            End If
        End Set
    End Property

    Private ReadOnly Property RowCount As Integer
        Get
            If _TrackCount <= 0 Then
                Return 0
            End If

            Return CInt(Math.Ceiling(_TrackCount / COLUMNS))
        End Get
    End Property

    ''' <summary>
    ''' Gets the current state for a cell by track index.
    ''' </summary>
    Public Function GetCell(TrackIndex As Integer) As CellInfo
        If TrackIndex < 0 OrElse TrackIndex >= _TrackCount Then
            Throw New ArgumentOutOfRangeException(NameOf(TrackIndex))
        End If

        Return _Cells(TrackIndex)
    End Function

    Public Sub ResetAll()
        For TrackIndex = 0 To _TrackCount - 1
            ResetCellInternal(TrackIndex)
        Next
    End Sub

    Public Sub ResetCell(TrackIndex As Integer)
        If TrackIndex < 0 OrElse TrackIndex >= _TrackCount Then
            Throw New ArgumentOutOfRangeException(NameOf(TrackIndex))
        End If

        ResetCellInternal(TrackIndex)
    End Sub

    ''' <summary>
    ''' Sets the text/background/foreground for a cell by track index (0-based).
    ''' Pass Nothing for any parameter you don’t want to change.
    ''' </summary>
    Public Sub SetCell(TrackIndex As Integer,
                       Optional Text As String = Nothing,
                       Optional BackColor As Color? = Nothing,
                       Optional ForeColor As Color? = Nothing,
                       Optional Selected As Boolean? = Nothing,
                       Optional Tooltip As String = Nothing)

        If TrackIndex < 0 OrElse TrackIndex >= _TrackCount Then
            Throw New ArgumentOutOfRangeException(NameOf(TrackIndex))
        End If

        Dim Cell = _Cells(TrackIndex)
        Dim Changed As Boolean = False

        If Text IsNot Nothing Then
            If Cell.Text <> Text Then
                Cell.Text = Text
                Changed = True
            End If
        End If

        If Tooltip IsNot Nothing Then
            If Cell.Tooltip <> Tooltip Then
                Cell.Tooltip = Tooltip
            End If
        End If

        If BackColor.HasValue AndAlso Cell.BackColor <> BackColor.Value Then
            Cell.BackColor = BackColor.Value
            Changed = True
        End If

        If ForeColor.HasValue AndAlso Cell.ForeColor <> ForeColor.Value Then
            Cell.ForeColor = ForeColor.Value
            Changed = True
        End If

        If Selected.HasValue Then
            _LastSelected = (TrackIndex, Selected.Value)
            If Cell.Selected <> Selected.Value Then
                Cell.Selected = Selected.Value
                If Selected.Value Then
                    _SelectedTracks.Add(TrackIndex)
                    If Not IsChecked Then
                        IsChecked = True
                    End If
                Else
                    _SelectedTracks.Remove(TrackIndex)
                End If
                Changed = True
                If Not _NoEventSelectionChanged Then
                    EventSelectionChanged(TrackIndex, Selected.Value)
                End If
            End If
        End If

        _Cells(TrackIndex) = Cell

        ' Invalidate just this cell’s rectangle
        If Changed Then
            Invalidate(GetCellRectangle(TrackIndex))
        End If
    End Sub

    Public Sub SetCellBackColor(TrackIndex As Integer, BackColor As Color)
        SetCell(TrackIndex, BackColor:=BackColor)
    End Sub

    Public Sub SetCellSelected(TrackIndex As Integer, Selected As Boolean, Optional NoEvent As Boolean = False)
        If NoEvent Then
            _NoEventSelectionChanged = True
        End If

        SetCell(TrackIndex, Selected:=Selected)

        If NoEvent Then
            _NoEventSelectionChanged = False
        End If
    End Sub

    Public Sub SetCellsSelected(SelectedTracks As HashSet(Of UShort), Selected As Boolean)
        _NoEventSelectionChanged = True

        For Each TrackIndex In SelectedTracks
            SetCell(TrackIndex, Selected:=Selected)
        Next
        _NoEventSelectionChanged = False
    End Sub

    Public Sub SetCellText(TrackIndex As Integer, Text As String)
        SetCell(TrackIndex, Text:=Text)
    End Sub

    Public Sub SetCellTooltip(TrackIndex As Integer, Text As String)
        SetCell(TrackIndex, Tooltip:=Text)
    End Sub

    Public Sub SetCheckStateSilent(Checked As Boolean)
        If _IsChecked <> Checked Then
            _IsChecked = Checked

            InvalidateFooter()
        End If
    End Sub

    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
        Dim key = keyData And Keys.KeyCode

        Select Case key
            Case Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Home, Keys.End, Keys.PageUp, Keys.PageDown, Keys.Tab
                Return True
        End Select

        Return MyBase.IsInputKey(keyData)
    End Function

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        MyBase.OnGotFocus(e)

        _FooterHasFocus = False

        If Not _SelectEnabled Or _Disabled Then
            Return
        End If

        Dim shiftHeld As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

        If shiftHeld Then
            ' Shift+Tab from *next* control -> focus footer checkbox
            _FooterHasFocus = True
        Else
            _FooterHasFocus = False

            EnsureFocusedTrackValid()
        End If

        InvalidateFocusedTrack()

        ' Make sure footer focus visuals are cleared
        InvalidateFooter()
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        If _Disabled OrElse Not _SelectEnabled OrElse _TrackCount = 0 Then
            Exit Sub
        End If

        Dim shiftHeld As Boolean = (e.Modifiers And Keys.Shift) = Keys.Shift

        If e.KeyCode = Keys.Tab Then
            If Not shiftHeld Then
                ' Tab: cells -> footer (internal focus)
                If Not _FooterHasFocus Then
                    _FooterHasFocus = True

                    ' Clear cell focus rectangle
                    InvalidateFocusedTrack()

                    ' Show footer focus
                    InvalidateFooter()
                    e.Handled = True
                    Exit Sub
                Else
                    ' footer -> next control (external)
                    _FooterHasFocus = False
                    InvalidateFooter()

                    If Me.Parent IsNot Nothing Then
                        Me.Parent.SelectNextControl(Me, True, True, True, True)
                    End If
                End If
                ' If already on footer: let Tab move to the next control (do not handle)
            Else
                ' Shift+Tab: footer -> cells
                If _FooterHasFocus Then
                    _FooterHasFocus = False

                    ' Ensure we have a focused track
                    If _FocusedTrack < 0 AndAlso _ActiveTrackCount > 0 Then
                        _FocusedTrack = 0
                    End If

                    InvalidateFocusedTrack()

                    InvalidateFooter()
                    e.Handled = True
                    Exit Sub
                Else
                    ' cells -> previous control (external)
                    If Me.Parent IsNot Nothing Then
                        Me.Parent.SelectNextControl(Me, False, True, True, True)
                    End If
                End If
                ' Shift+Tab from cells: let focus move to previous control
            End If

            ' If we reach here, we didn't consume Tab; let default behavior handle it
            Return
        End If

        ' --- If footer has focus, handle Space here and skip cell logic ---
        If _FooterHasFocus Then
            If e.KeyCode = Keys.Space Then
                If Not _Disabled AndAlso _SelectEnabled Then
                    IsChecked = Not IsChecked
                End If
                e.Handled = True
            End If
            Return
        End If

        EnsureFocusedTrackValid()
        If _FocusedTrack = -1 Then
            Invalidate()
            Exit Sub
        End If

        Dim row As Integer = _FocusedTrack \ COLUMNS
        Dim col As Integer = _FocusedTrack Mod COLUMNS
        Dim newTrack As Integer = _FocusedTrack

        Dim currentSelected As Boolean = False
        If _FocusedTrack > -1 AndAlso _FocusedTrack < _ActiveTrackCount Then
            currentSelected = _Cells(_FocusedTrack).Selected
        End If

        Select Case e.KeyCode
            Case Keys.Left
                If newTrack > 0 Then
                    newTrack -= 1
                End If
                e.Handled = True
            Case Keys.Right
                If newTrack < _ActiveTrackCount - 1 Then
                    newTrack += 1
                End If
                e.Handled = True
            Case Keys.Up
                If row > 0 Then
                    newTrack -= COLUMNS
                End If
                e.Handled = True
            Case Keys.Down
                If row < RowCount - 1 Then
                    newTrack += COLUMNS
                End If
                e.Handled = True
            Case Keys.Home
                newTrack = 0
                e.Handled = True
            Case Keys.End
                newTrack = _ActiveTrackCount - 1
                e.Handled = True
            Case Keys.PageUp
                newTrack = Math.Max(0, _FocusedTrack - COLUMNS * 4)
                e.Handled = True
            Case Keys.PageDown
                newTrack = Math.Min(_ActiveTrackCount - 1, _FocusedTrack + COLUMNS * 4)
                e.Handled = True
            Case Keys.Space
                ' Toggle selection with spacebar
                If newTrack < _ActiveTrackCount Then
                    If shiftHeld AndAlso _LastSelected.Track > -1 AndAlso _LastSelected.Track <> newTrack Then
                        SetRangeSelected(_LastSelected.Track, newTrack, _LastSelected.Selected)
                    Else
                        SetCellSelected(newTrack, Not _Cells(newTrack).Selected)
                    End If
                End If
                e.Handled = True
        End Select

        If newTrack <> _FocusedTrack AndAlso newTrack >= 0 AndAlso newTrack < _ActiveTrackCount Then
            If shiftHeld Then
                SetCellSelected(newTrack, currentSelected)
            End If

            Dim oldFocused = _FocusedTrack
            _FocusedTrack = newTrack
            If oldFocused >= 0 AndAlso oldFocused < _ActiveTrackCount Then
                Invalidate(GetCellRectangle(oldFocused))
            End If
            Invalidate(GetCellRectangle(_FocusedTrack))
        End If
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        MyBase.OnLostFocus(e)

        ' Hide focus rectangle when leaving (by repainting that cell)
        If _FooterHasFocus Then
            InvalidateFooter()
        Else
            InvalidateFocusedTrack()
        End If

        _FooterHasFocus = False
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)

        If _Disabled Then
            Exit Sub
        End If

        '' First, check if click is on footer checkbox or footer text
        If e.Button = MouseButtons.Left AndAlso _SelectEnabled AndAlso Not _Disabled Then
            If _FooterCheckRect.Contains(e.Location) OrElse _FooterTextRect.Contains(e.Location) Then

                If Not Me.Focused Then
                    Me.Focus()
                End If

                _FooterHasFocus = True

                InvalidateFocusedTrack()

                InvalidateFooter()

                IsChecked = Not IsChecked
                Return
            End If
        End If

        Dim Response = HitTestCell(e.Location)

        If Response.Result Then
            If Response.TrackIndex >= _ActiveTrackCount Then
                Exit Sub
            End If

            EventCellClicked(Response.TrackIndex, Response.Row, Response.Col)
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim g = e.Graphics

        g.Clear(Me.BackColor)

        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit

        Dim Rows = RowCount

        Using sf As New StringFormat()
            sf.Alignment = StringAlignment.Center
            sf.LineAlignment = StringAlignment.Center

            '--- Column headers (top, starting at 0) ---
            For Col As Integer = 0 To COLUMNS - 1
                Dim x As Integer = (Col + 1) * CELL_WIDTH    ' skip header column
                Dim rect As New Rectangle(x, 0, CELL_WIDTH, CELL_HEIGHT)
                g.DrawString(Col.ToString(), HEADER_FONT, SystemBrushes.ControlText, rect, sf)
            Next

            '--- Row headers (left, starting at 0) ---
            For Row As Integer = 0 To Math.Max(Rows - 1, 0)
                Dim y As Integer = (Row + 1) * CELL_HEIGHT   ' skip header row
                Dim rect As New Rectangle(0, y, CELL_WIDTH, CELL_HEIGHT)
                g.DrawString(Row.ToString(), HEADER_FONT, SystemBrushes.ControlText, rect, sf)
            Next

            '--- Data cells (background, border, text) ---
            If Rows > 0 Then
                Dim TrackIndex As Integer = 0

                For Row As Integer = 0 To Rows - 1
                    For Col As Integer = 0 To COLUMNS - 1
                        Dim rect = GetCellRectangle(TrackIndex)
                        Dim cell As CellInfo
                        If TrackIndex >= _ActiveTrackCount Then
                            cell = CreateDefaultCell()
                            cell.BackColor = DISABLED_COLOR
                        Else
                            cell = _Cells(TrackIndex)
                        End If

                        ' Fill background
                        Dim BackColor = cell.BackColor
                        If _Disabled Then
                            BackColor = DISABLED_COLOR
                        ElseIf cell.Selected AndAlso cell.BackColor = DEFAULT_BACKCOLOR Then
                            BackColor = SELECTED_BACKCOLOR
                        End If

                        Using b As New SolidBrush(BackColor)
                            g.FillRectangle(b, rect)
                        End Using

                        ' Draw border (full size so borders are shared)
                        g.DrawRectangle(BORDER_PEN, rect)

                        ' Draw centered text, if any
                        If Not String.IsNullOrEmpty(cell.Text) Then
                            Using fb As New SolidBrush(cell.ForeColor)
                                Dim textRect As New Rectangle(rect.X, rect.Y + 1, rect.Width, rect.Height)
                                g.DrawString(cell.Text, GRID_FONT, fb, textRect, sf)
                            End Using
                        End If

                        If _SelectEnabled AndAlso Not _Disabled AndAlso Not _FooterHasFocus AndAlso TrackIndex = _FocusedTrack AndAlso Me.Focused Then
                            Dim focusRect As New Rectangle(
                                rect.X + 2,
                                rect.Y + 2,
                                rect.Width - 3,
                                rect.Height - 3
                            )
                            ControlPaint.DrawFocusRectangle(g, focusRect)
                        End If

                        TrackIndex += 1
                    Next
                Next
            End If
        End Using

        ' Draw footer row (with checkbox + label)
        DrawFooter(g)
    End Sub

    '=== Painting ===
    Protected Overrides Sub OnSizeChanged(e As EventArgs)
        MyBase.OnSizeChanged(e)
        Invalidate()
    End Sub

    Private Sub ClearSelected()
        _NoEventSelectionChanged = True
        For counter = 0 To _TrackCount - 1
            SetCellSelected(counter, False)
        Next
        _NoEventSelectionChanged = False

        _LastSelected = (-1, False)
    End Sub

    Private Function CreateDefaultCell() As CellInfo
        Dim c As New CellInfo With {
            .BackColor = DEFAULT_BACKCOLOR,
            .ForeColor = DEFAULT_FORECOLOR,
            .Text = String.Empty,
            .Selected = False
        }
        Return c
    End Function

    Private Sub DrawFooter(g As Graphics)
        Dim rows = RowCount
        Dim totalCols As Integer = COLUMNS + 1

        Dim footerY As Integer = (rows + 1) * CELL_HEIGHT + 1  ' header + data
        Dim footerRect As New Rectangle(0, footerY, totalCols * CELL_WIDTH, CELL_HEIGHT)

        ' Reset hit-test rects by default
        _FooterCheckRect = Rectangle.Empty
        _FooterTextRect = Rectangle.Empty

        ' Background (just use control BackColor)
        Using b As New SolidBrush(Me.BackColor)
            g.FillRectangle(b, footerRect)
        End Using

        If String.IsNullOrEmpty(_Label) Then
            Return
        End If

        ' Apply top/bottom margin inside footer
        Dim innerFooterRect As New Rectangle(footerRect.X, footerRect.Y + FOOTER_MARGIN, footerRect.Width, footerRect.Height - (FOOTER_MARGIN * 2))

        ' Checkbox glyph size
        Dim glyphSize As Size
        If Application.RenderWithVisualStyles Then
            glyphSize = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal)
        Else
            glyphSize = New Size(13, 13)
        End If

        Dim spacing As Integer = 4

        ' Measure text
        Dim textSize As Size = TextRenderer.MeasureText(_Label, HEADER_FONT)
        Dim contentWidth As Integer = glyphSize.Width + spacing + textSize.Width

        Dim startX As Integer = (innerFooterRect.Width - contentWidth) \ 2

        ' Checkbox rect
        Dim cbY As Integer = innerFooterRect.Y + (innerFooterRect.Height - glyphSize.Height) \ 2
        Dim cbRect As New Rectangle(startX, cbY, glyphSize.Width, glyphSize.Height)

        If Not _Disabled AndAlso _SelectEnabled Then
            ' Draw checkbox
            Dim state As CheckBoxState = If(_IsChecked,
                                        CheckBoxState.CheckedNormal,
                                        CheckBoxState.UncheckedNormal)

            If Application.RenderWithVisualStyles Then
                CheckBoxRenderer.DrawCheckBox(g, cbRect.Location, state)
            Else
                g.DrawRectangle(BORDER_PEN, cbRect)
                If _IsChecked Then
                    g.DrawLine(BORDER_PEN, cbRect.Left + 2, cbRect.Top + glyphSize.Height \ 2,
                           cbRect.Left + glyphSize.Width \ 2, cbRect.Bottom - 2)
                    g.DrawLine(BORDER_PEN, cbRect.Left + glyphSize.Width \ 2, cbRect.Bottom - 2,
                           cbRect.Right - 2, cbRect.Top + 2)
                End If
            End If
        End If

        ' Text rect (vertically centered, left of checkbox)
        Dim textRect As New Rectangle(cbRect.Right + spacing, innerFooterRect.Y, textSize.Width, innerFooterRect.Height)

        TextRenderer.DrawText(g,
                              _Label,
                              HEADER_FONT,
                              textRect,
                              SystemColors.ControlText,
                              TextFormatFlags.VerticalCenter Or TextFormatFlags.Left Or TextFormatFlags.NoPadding)

        ' Store rects for hit-testing
        _FooterCheckRect = cbRect
        _FooterTextRect = textRect

        If _SelectEnabled AndAlso Not _Disabled AndAlso _FooterHasFocus AndAlso Me.Focused Then
            Dim tightSize As Size = TextRenderer.MeasureText(g, _Label, HEADER_FONT, New Size(Integer.MaxValue, Integer.MaxValue), TextFormatFlags.NoPadding Or TextFormatFlags.NoClipping)

            ' Create a tight box around the text only
            Dim fr As New Rectangle(textRect.X, textRect.Y + (textRect.Height - tightSize.Height) \ 2, tightSize.Width, tightSize.Height)
            fr.Inflate(2, 0)
            ControlPaint.DrawFocusRectangle(g, fr)
        End If
    End Sub

    Private Sub EnsureFocusedTrackValid()
        If _ActiveTrackCount <= 0 Then
            _FocusedTrack = -1
        ElseIf _FocusedTrack < 0 Then
            _FocusedTrack = 0
        ElseIf _FocusedTrack >= _ActiveTrackCount Then
            _FocusedTrack = _ActiveTrackCount - 1
        End If
    End Sub

    Private Sub EventCellClicked(Track As Integer, Row As Integer, Col As Integer)
        If _Disabled Then
            Exit Sub
        End If

        Dim Shift As Boolean = (ModifierKeys And Keys.Shift) = Keys.Shift

        If _SelectEnabled Then
            If Shift AndAlso _LastSelected.Track > -1 AndAlso _LastSelected.Track <> Track Then
                SetRangeSelected(_LastSelected.Track, Track, _LastSelected.Selected)
            Else
                SetCellSelected(Track, Not _Cells(Track).Selected)
            End If

            If _FocusedTrack > -1 Then
                Invalidate(GetCellRectangle(_FocusedTrack))
            End If

            _FocusedTrack = Track
            Me.Focus()     ' Give keyboard focus to the control
            Invalidate(GetCellRectangle(Track))
        End If

        Debug.Print("Cell clicked: TrackIndex={0}, Row={1}, Col={2}, Shift={3}", Track, Row, Col, Shift)
        RaiseEvent CellClicked(Me, Track, Row, Col, Shift)
    End Sub

    Private Sub EventCheckChanged(Checked As Boolean)
        If _Disabled Then
            Exit Sub
        End If

        If Not Checked Then
            ClearSelected()
        End If

        Debug.Print("Footer checkbox changed: {0}", Checked)
        RaiseEvent CheckChanged(Me, Checked)
    End Sub

    Private Sub EventSelectionChanged(Track As Integer, Selected As Boolean)
        If _Disabled Then
            Exit Sub
        End If

        Debug.Print("Cell selection changed: TrackIndex={0}, Selected={1}", Track, Selected)
        RaiseEvent SelectionChanged(Me, Track, Selected)
    End Sub

    Private Sub FloppyTrackGrid_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        Dim TooltipText As String = ""

        Dim Response = HitTestCell(e.Location)
        If Response.Result Then
            TooltipText = _Cells(Response.TrackIndex).Tooltip
            If TooltipText = "" Then
                TooltipText = My.Resources.Label_Track & ":  " & Response.TrackIndex.ToString & "." & _Side.ToString
            End If

        End If

            If TooltipText <> _ToolTip.GetToolTip(Me) Then
            _ToolTip.SetToolTip(Me, TooltipText)
        End If
    End Sub

    ' Map track index -> cell rectangle (including header offsets)
    Private Function GetCellRectangle(trackIndex As Integer) As Rectangle
        Dim row As Integer = trackIndex \ COLUMNS
        Dim col As Integer = trackIndex Mod COLUMNS

        Dim x As Integer = (col + 1) * CELL_WIDTH   ' +1 for left header column
        Dim y As Integer = (row + 1) * CELL_HEIGHT  ' +1 for top header row

        Return New Rectangle(x, y, CELL_WIDTH, CELL_HEIGHT)
    End Function

    Private Function HitTestCell(p As Point) As (Result As Boolean, TrackIndex As Integer, Row As Integer, Col As Integer)
        Dim Response As (Result As Boolean, TrackIndex As Integer, Row As Integer, Col As Integer)

        Response.Result = False
        Response.TrackIndex = -1
        Response.Row = -1
        Response.Col = -1

        ' Ignore header row/column
        If p.X < CELL_WIDTH OrElse p.Y < CELL_HEIGHT Then
            Return Response
        End If

        ' Convert to data-cell coordinates (0-based within data area)
        Response.Col = (p.X \ CELL_WIDTH) - 1   ' -1 for left header column
        Response.Row = (p.Y \ CELL_HEIGHT) - 1  ' -1 for top header row

        If Response.Row < 0 OrElse Response.Col < 0 OrElse Response.Col >= COLUMNS Then
            Return Response
        End If

        Response.TrackIndex = Response.Row * COLUMNS + Response.Col

        If Response.TrackIndex < 0 OrElse Response.TrackIndex >= _ActiveTrackCount Then
            Return Response
        End If

        Response.Result = True

        Return Response
    End Function

    Private Sub InvalidateFocusedTrack()
        If _FocusedTrack >= 0 AndAlso _FocusedTrack < _ActiveTrackCount Then
            Invalidate(GetCellRectangle(_FocusedTrack))
        End If
    End Sub
    Private Sub InvalidateFooter()
        Dim rows = RowCount
        Dim footerY As Integer = (rows + 1) * CELL_HEIGHT + 1
        Dim totalCols As Integer = COLUMNS + 1
        Dim rect As New Rectangle(0, footerY, totalCols * CELL_WIDTH + 1, CELL_HEIGHT)
        Invalidate(rect)
    End Sub

    Private Sub ResetCellInternal(trackIndex As Integer)
        SetCell(trackIndex, Text:="", BackColor:=DEFAULT_BACKCOLOR, ForeColor:=DEFAULT_FORECOLOR)
    End Sub
    Private Sub ResizeCellList(NewCount As Integer)
        If _Cells Is Nothing Then
            _Cells = New List(Of CellInfo)()
        End If

        ' Grow: add default cells
        If NewCount > _Cells.Count Then
            For i = _Cells.Count To NewCount - 1
                _Cells.Add(CreateDefaultCell())
            Next
        ElseIf NewCount < _Cells.Count Then
            ' Shrink: remove extra cells
            For i As Integer = NewCount To _Cells.Count - 1
                If _Cells(i).Selected Then
                    _SelectedTracks.Remove(i)
                End If
            Next
            _Cells.RemoveRange(NewCount, _Cells.Count - NewCount)
        End If
    End Sub

    Private Sub SetRangeSelected(LastTrack As Integer, CurrentTrack As Integer, Selected As Boolean)
        Dim StartTrack As Integer = Math.Min(LastTrack, CurrentTrack)
        Dim EndTrack As Integer = Math.Max(LastTrack, CurrentTrack)

        For track = StartTrack To EndTrack
            SetCellSelected(track, Selected)
        Next
    End Sub

    Private Sub UpdateSizeFromTrackCount()
        Dim Rows = RowCount

        Dim TotalRows As Integer
        If Rows > 0 Then
            TotalRows = Rows + 1 ' header + data
        Else
            TotalRows = 1 ' header only
        End If

        TotalRows += 1 ' footer row

        Dim TotalCols As Integer = COLUMNS + 1 ' +1 header column

        ' +1 avoids clipping bottom/right border pixels
        Dim Width As Integer = TotalCols * CELL_WIDTH + 1
        Dim Height As Integer = TotalRows * CELL_HEIGHT + 1

        Me.Size = New Size(Width, Height)
    End Sub
    '=== Per-cell state ===
    Public Structure CellInfo
        Public BackColor As Color
        Public ForeColor As Color
        Public Selected As Boolean
        Public Text As String
        Public Tooltip As String
    End Structure
End Class