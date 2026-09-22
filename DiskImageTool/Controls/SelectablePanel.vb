Imports System.ComponentModel
Imports System.Drawing.Drawing2D

Public Class SelectablePanel
    Inherits Panel

    Private Const PADDING_COLS As Integer = 8
    Private Const PADDING_ROWS As Integer = 6
    Private Const SECTOR_HEIGHT As Integer = 16
    Private Const SECTOR_WIDTH As Integer = 24

    Public Event SelectedIndexChanged(sender As Object, Index As Integer)

    Private ReadOnly _ToolTip As TwoColumnToolTip
    Private _Items As IList(Of SectorCell) = Array.Empty(Of SectorCell)()
    Private _SelectedIndex As Integer = -1
    Private _UpdatingLayout As Boolean

    Public Sub New()
        SetStyle(ControlStyles.Selectable Or
                 ControlStyles.UserPaint Or
                 ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw, True)
        DoubleBuffered = True
        TabStop = True
        _ToolTip = New TwoColumnToolTip()
    End Sub

    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Items As IList(Of SectorCell)
        Get
            Return _Items
        End Get
        Set(value As IList(Of SectorCell))
            If value Is Nothing Then
                _Items = Array.Empty(Of SectorCell)()
            Else
                _Items = value
            End If

            If _SelectedIndex >= _Items.Count Then
                _SelectedIndex = -1
            End If

            UpdateLayout()
            Invalidate()
        End Set
    End Property

    <Browsable(False)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SelectedIndex As Integer
        Get
            Return _SelectedIndex
        End Get
        Set(value As Integer)
            If value < -1 OrElse value >= _Items.Count Then
                value = -1
            End If

            If _SelectedIndex = value Then
                Exit Property
            End If

            _SelectedIndex = value
            Invalidate()
        End Set
    End Property

    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
        If keyData = Keys.Up OrElse keyData = Keys.Down Then Return True
        If keyData = Keys.Left OrElse keyData = Keys.Right Then Return True
        Return MyBase.IsInputKey(keyData)
    End Function

    Protected Overrides Sub OnEnter(e As EventArgs)
        Invalidate()
        MyBase.OnEnter(e)
    End Sub

    Protected Overrides Sub OnLeave(e As EventArgs)
        Invalidate()
        MyBase.OnLeave(e)
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        Focus()
        MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Sub OnMouseClick(e As MouseEventArgs)
        MyBase.OnMouseClick(e)

        If (e.Button And MouseButtons.Left) = 0 Then
            Exit Sub
        End If

        Dim Index = GetSectorIndex(e.Location)
        If Index > -1 Then
            SelectFromUser(Index)
        End If
    End Sub

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)

        Dim TooltipText As String = ""
        Dim Index = GetSectorIndex(e.Location)
        If Index > -1 Then
            TooltipText = If(_Items(Index).ToolTipText, "")
        End If

        If TooltipText <> _ToolTip.GetToolTip(Me) Then
            _ToolTip.SetToolTip(Me, TooltipText)
        End If
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        If _Items.Count = 0 Then
            Exit Sub
        End If

        Dim Index As Integer = -1

        If e.KeyCode = Keys.Left Then
            Index = _SelectedIndex
            If Index = -1 Then
                Index = _Items.Count - 1
            Else
                Index -= 1
            End If
        ElseIf e.KeyCode = Keys.Right Then
            Index = _SelectedIndex
            If Index = -1 Then
                Index = 0
            Else
                Index += 1
            End If
        End If

        If Index > -1 AndAlso Index < _Items.Count Then
            SelectFromUser(Index)
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        e.Graphics.Clear(SystemColors.Control)

        If _Items.Count = 0 Then
            Exit Sub
        End If

        Using SelectedPen As New Pen(Color.Blue, 2)
            Using SectorFont As New Font("Microsoft Sans Serif", 7)
                Dim LeftPos = Padding.Left
                Dim TopPos = Padding.Top

                For i = 0 To _Items.Count - 1
                    Dim Cell = _Items(i)
                    Dim SectorBrush As Brush = New SolidBrush(Cell.FillColor)
                    Dim DisposeBrush = True

                    If Cell.HasWeakBits Then
                        Dim Hatch As New HatchBrush(HatchStyle.ForwardDiagonal, Color.Gray, Cell.FillColor)
                        SectorBrush.Dispose()
                        SectorBrush = Hatch
                    End If

                    e.Graphics.FillRectangle(SectorBrush, LeftPos, TopPos, SECTOR_WIDTH, SECTOR_HEIGHT)

                    If DisposeBrush Then
                        SectorBrush.Dispose()
                    End If

                    If i = _SelectedIndex Then
                        e.Graphics.DrawRectangle(SelectedPen, LeftPos + 1, TopPos + 1, SECTOR_WIDTH - 1, SECTOR_HEIGHT - 1)
                    Else
                        e.Graphics.DrawRectangle(SystemPens.WindowFrame, LeftPos, TopPos, SECTOR_WIDTH, SECTOR_HEIGHT)
                    End If

                    If Cell.WriteSplice Then
                        Const DotSize As Integer = 4
                        e.Graphics.FillEllipse(Brushes.Blue, LeftPos + SECTOR_WIDTH - DotSize - 2, TopPos + 1, DotSize, DotSize)
                    End If

                    Dim TextSize = e.Graphics.MeasureString(Cell.Text, SectorFont)
                    e.Graphics.DrawString(Cell.Text, SectorFont, SystemBrushes.WindowText, LeftPos + (SECTOR_WIDTH - TextSize.Width) / 2, TopPos + (SECTOR_HEIGHT - TextSize.Height) / 2)

                    LeftPos += SECTOR_WIDTH + PADDING_COLS
                    If LeftPos + SECTOR_WIDTH > Width - Padding.Right Then
                        LeftPos = Padding.Left
                        TopPos += SECTOR_HEIGHT + PADDING_ROWS
                    End If
                Next
            End Using
        End Using
    End Sub

    Protected Overrides Sub OnResize(eventargs As EventArgs)
        MyBase.OnResize(eventargs)
        If DesignMode Then
            Exit Sub
        End If
        UpdateLayout()
    End Sub

    Private Function GetSectorsPerRow() As Integer
        Dim MaxWidth As Integer = Width - Padding.Horizontal + PADDING_COLS
        Dim SectorWidth As Integer = SECTOR_WIDTH + PADDING_COLS
        If SectorWidth <= 0 Then
            Return 1
        End If

        Dim Count = MaxWidth \ SectorWidth
        If Count < 1 Then
            Return 1
        End If

        Return Count
    End Function

    Private Function GetSectorIndex(MousePos As Point) As Integer
        If MousePos.X >= Width - Padding.Right Then
            Return -1
        End If

        MousePos.Offset(-Padding.Left, -Padding.Top)
        Dim SectorWidth = SECTOR_WIDTH + PADDING_COLS
        Dim SectorHeight = SECTOR_HEIGHT + PADDING_ROWS
        If SectorWidth <= 0 OrElse SectorHeight <= 0 Then
            Return -1
        End If

        Dim ColIndex = MousePos.X \ SectorWidth
        Dim RowIndex = MousePos.Y \ SectorHeight
        Dim SectorRect As New Rectangle(ColIndex * SectorWidth, RowIndex * SectorHeight, SECTOR_WIDTH, SECTOR_HEIGHT)
        If SectorRect.Contains(MousePos) Then
            Dim Index = RowIndex * GetSectorsPerRow() + ColIndex
            If Index >= 0 AndAlso Index < _Items.Count Then
                Return Index
            End If
        End If

        Return -1
    End Function

    Private Sub SelectFromUser(Index As Integer)
        SelectedIndex = Index
        RaiseEvent SelectedIndexChanged(Me, Index)
    End Sub

    Private Sub UpdateLayout()
        If _UpdatingLayout Then
            Exit Sub
        End If

        _UpdatingLayout = True
        Try
            If _Items.Count = 0 Then
                Height = 0
                Visible = False
            Else
                Dim MaxSectors = GetSectorsPerRow()
                Dim NumRows As Integer = CInt(Math.Ceiling(_Items.Count / CDbl(MaxSectors)))
                Dim PanelHeight = SECTOR_HEIGHT * NumRows + PADDING_ROWS * (NumRows - 1) + Padding.Top + Padding.Bottom
                Height = PanelHeight
                Visible = True
            End If
        Finally
            _UpdatingLayout = False
        End Try
    End Sub

    Public Class SectorCell
        Public Property Text As String
        Public Property FillColor As Color
        Public Property HasWeakBits As Boolean
        Public Property WriteSplice As Boolean
        Public Property ToolTipText As String
    End Class
End Class
