Imports System.ComponentModel
Public Class SplitButton
    Inherits Button

    Private Const SplitWidth As Integer = 18

    Private ReadOnly _Items As New List(Of SplitButtonItem)
    Private _Menu As ContextMenuStrip
    Private _SelectedIndex As Integer = -1

    Public Event ItemClicked As EventHandler
    Public Event SelectedIndexChanged As EventHandler

    Public Sub New()
        MyBase.New()

        Me.TextAlign = ContentAlignment.MiddleCenter

        Dim p = Me.Padding
        If p.Right < SplitWidth Then
            Me.Padding = New Padding(p.Left, p.Top, p.Right + SplitWidth, p.Bottom)
        End If

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw, True)
    End Sub

    <Browsable(False)>
    Public ReadOnly Property Items As IList(Of SplitButtonItem)
        Get
            Return _Items
        End Get
    End Property

    <Browsable(False)>
    Public Property SelectedIndex As Integer
        Get
            Return _SelectedIndex
        End Get
        Set(value As Integer)
            Dim NewIndex = value

            If NewIndex < -1 OrElse NewIndex >= _Items.Count Then
                NewIndex = -1
            End If

            If NewIndex = _SelectedIndex Then
                Return
            End If

            _SelectedIndex = NewIndex

            If _SelectedIndex >= 0 AndAlso _SelectedIndex < _Items.Count Then
                Me.Text = _Items(_SelectedIndex).Text
            Else
                Me.Text = String.Empty
            End If

            RaiseEvent SelectedIndexChanged(Me, EventArgs.Empty)
        End Set
    End Property

    <Browsable(False)>
    Public ReadOnly Property SelectedItem As SplitButtonItem
        Get
            If _SelectedIndex < 0 OrElse _SelectedIndex >= _Items.Count Then
                Return Nothing
            End If

            Return _Items(_SelectedIndex)
        End Get
    End Property

    <Browsable(False)>
    Public Property SelectedText As String
        Get
            Dim item = SelectedItem

            If item Is Nothing Then
                Return Nothing
            End If

            Return item.Text
        End Get
        Set(value As String)
            For i = 0 To _Items.Count - 1
                If String.Equals(_Items(i).Text, value, StringComparison.CurrentCulture) Then
                    SelectedIndex = i
                    Exit For
                End If
            Next
        End Set
    End Property

    <Browsable(False)>
    Public Property SelectedValue As Object
        Get
            Dim item = SelectedItem

            If item Is Nothing Then
                Return Nothing
            End If

            Return item.Value
        End Get
        Set(value As Object)
            For i = 0 To _Items.Count - 1
                If Equals(_Items(i).Value, value) Then
                    SelectedIndex = i
                    Exit For
                End If
            Next
        End Set
    End Property

    Public Sub AddItem(Text As String, Optional Value As Object = Nothing)
        _Items.Add(New SplitButtonItem(Text, Value))

        If _SelectedIndex = -1 Then
            SelectedIndex = 0
        End If
    End Sub

    Public Sub ClearItems()
        _Items.Clear()
        _SelectedIndex = -1
        Me.Text = String.Empty
        DisposeMenu()

        RaiseEvent SelectedIndexChanged(Me, EventArgs.Empty)
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            DisposeMenu()
        End If
        MyBase.Dispose(disposing)
    End Sub

    Protected Overrides Sub OnClick(e As EventArgs)
        MyBase.OnClick(e)
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        If (e.Alt AndAlso e.KeyCode = Keys.Down) OrElse e.KeyCode = Keys.F4 Then
            ShowMenu()
            e.Handled = True
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        If e.Button = MouseButtons.Left AndAlso IsOnSplit(e.Location) Then
            ShowMenu()
            Return
        End If

        MyBase.OnMouseUp(e)
    End Sub

    Protected Overrides Sub OnPaddingChanged(e As EventArgs)
        MyBase.OnPaddingChanged(e)

        Dim p = Me.Padding

        If p.Right < SplitWidth Then
            Me.Padding = New Padding(p.Left, p.Top, SplitWidth, p.Bottom)
        End If
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim g = e.Graphics

        Dim RectSplit As New Rectangle(Me.Width - SplitWidth, 0, SplitWidth, Me.Height)

        Dim SplitPen As Pen
        Dim ArrowBrush As Brush

        If Me.Enabled Then
            SplitPen = SystemPens.ControlDark
            ArrowBrush = SystemBrushes.ControlText
        Else
            SplitPen = SystemPens.ControlLight
            ArrowBrush = SystemBrushes.ControlLight
        End If

        g.DrawLine(SplitPen, RectSplit.Left, 4, RectSplit.Left, Me.Height - 4)

        Dim MidX = RectSplit.Left + RectSplit.Width \ 2
        Dim MidY = Me.Height \ 2 + 1

        Dim arrow() As Point = {New Point(MidX - 3, MidY - 1), New Point(MidX + 3, MidY - 1), New Point(MidX, MidY + 2)}

        g.FillPolygon(ArrowBrush, arrow)
    End Sub

    Private Sub BuildMenuIfNeeded()
        If _Menu IsNot Nothing Then
            _Menu.Items.Clear()
        Else
            _Menu = New ContextMenuStrip()
            AddHandler _Menu.ItemClicked, AddressOf Menu_ItemClicked
        End If

        For i = 0 To _Items.Count - 1
            Dim itm = _Items(i)
            Dim menuItem = New ToolStripMenuItem(itm.Text) With {
                .Tag = i
            }
            _Menu.Items.Add(menuItem)
        Next
    End Sub

    Private Sub DisposeMenu()
        If _Menu IsNot Nothing Then
            RemoveHandler _Menu.ItemClicked, AddressOf Menu_ItemClicked
            _Menu.Dispose()
            _Menu = Nothing
        End If
    End Sub

    Private Function IsOnSplit(p As Point) As Boolean
        Return p.X >= Me.Width - SplitWidth
    End Function

    Private Sub Menu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs)
        If TypeOf e.ClickedItem.Tag Is Integer Then
            Dim index As Integer = CInt(e.ClickedItem.Tag)
            SelectedIndex = index
        End If

        RaiseEvent ItemClicked(Me, EventArgs.Empty)
    End Sub

    Private Sub ShowMenu()
        If _Items.Count = 0 Then
            Return
        End If

        BuildMenuIfNeeded()

        Dim pt = New Point(0, Me.Height)
        _Menu.Show(Me, pt)
    End Sub

    Public Class SplitButtonItem
        Public Sub New(Text As String, Optional Value As Object = Nothing)
            Me.Text = Text
            Me.Value = Value
        End Sub

        Public Property Text As String
        Public Property Value As Object
        Public Overrides Function ToString() As String
            Return Text
        End Function
    End Class
End Class
