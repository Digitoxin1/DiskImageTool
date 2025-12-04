Public Class LoadedImageList
    Private WithEvents ComboAll As ComboBox
    Private WithEvents ComboFiltered As ComboBox
    Private _Filtered As Boolean = False
    Private _LastSelected As ImageData = Nothing

    Public Event DragDrop(sender As Object, e As DragEventArgs)
    Public Event DragEnter(sender As Object, e As DragEventArgs)
    Public Event SelectedIndexChanged()

    Public Sub New(Combo As ComboBox)
        Me.ComboAll = Combo

        Initialize()
    End Sub

    Public ReadOnly Property AllItems As ComboBox.ObjectCollection
        Get
            Return ComboAll.Items
        End Get
    End Property

    Public ReadOnly Property FilteredItemCount As Integer
        Get
            Return ComboFiltered.Items.Count
        End Get
    End Property

    Public ReadOnly Property SelectedItem As ImageData
        Get
            Return ComboAll.SelectedItem
        End Get
    End Property

    Public Sub AddFilteredItem(ImageData As ImageData)
        Dim Index = ComboFiltered.Items.Add(ImageData)
        If ImageData Is ComboAll.SelectedItem Then
            ComboFiltered.SelectedIndex = Index
        End If
    End Sub

    Public Sub AddItem(Image As ImageData)
        ComboAll.Items.Add(Image)
    End Sub

    Public Sub BeginUpdate()
        ComboAll.BeginUpdate()
    End Sub

    Public Sub BeginUpdateFiltered()
        ComboFiltered.BeginUpdate()
        ComboFiltered.Items.Clear()
    End Sub

    Public Sub ClearAll()
        ComboImagesClear(ComboAll)
        HideFiltered()
        _LastSelected = Nothing
    End Sub

    Public Sub EndUpdate()
        ComboAll.EndUpdate()
    End Sub

    Public Sub EndUpdateFiltered()
        EnsureItemSelected(ComboFiltered)
        ComboFiltered.EndUpdate()
        DisplayFiltered()
    End Sub

    Public Function GetPathOffset() As Integer
        ' No images → no common path.
        If ComboAll.Items.Count = 0 Then
            Return 0
        End If

        Dim First As Boolean = True
        Dim CommonParts() As String = Nothing

        For Each img As ImageData In ComboAll.Items
            Dim dir = IO.Path.GetDirectoryName(img.DisplayPath)

            If String.IsNullOrEmpty(dir) Then
                ' Any image without a directory kills the common prefix
                Return 0
            End If

            Dim parts = dir.Split("\")

            If First Then
                CommonParts = parts
                First = False
            Else
                Dim maxLen = Math.Min(CommonParts.Length, parts.Length)
                Dim j As Integer = 0

                While j < maxLen AndAlso String.Equals(CommonParts(j), parts(j), StringComparison.OrdinalIgnoreCase)
                    j += 1
                End While

                If j = 0 Then
                    ' No common prefix at all
                    Return 0
                End If

                If j < CommonParts.Length Then
                    Dim newCommon(j - 1) As String
                    Array.Copy(CommonParts, newCommon, j)
                    CommonParts = newCommon
                End If
            End If
        Next

        If CommonParts Is Nothing OrElse CommonParts.Length = 0 Then
            Return 0
        End If

        Dim BasePath = String.Join("\", CommonParts)
        BasePath = PathAddBackslash(BasePath)

        Return BasePath.Length
    End Function

    Public Sub HideFiltered()
        _Filtered = False

        ComboFiltered.Visible = False
        ComboAll.Visible = True

        ComboImagesClear(ComboFiltered)
        EnsureItemSelected(ComboAll)
    End Sub

    Public Sub RefreshItemText()
        ComboAll.Invalidate()
        ComboFiltered.Invalidate()
    End Sub

    Public Sub RefreshPaths()
        ComboAll.BeginUpdate()
        ImageData.StringOffset = GetPathOffset()
        RefreshItemText()
        ComboAll.EndUpdate()
    End Sub

    Public Sub RemoveItem(Value As ImageData)
        Dim ActiveComboBox As ComboBox = If(_Filtered, ComboFiltered, ComboAll)

        Dim SelectedIndex = ActiveComboBox.SelectedIndex

        ComboAll.Items.Remove(Value)
        ComboFiltered.Items.Remove(Value)

        If ActiveComboBox.SelectedIndex = -1 Then
            If SelectedIndex > ActiveComboBox.Items.Count - 1 Then
                SelectedIndex = ActiveComboBox.Items.Count - 1
            End If
            ActiveComboBox.SelectedIndex = SelectedIndex
        End If

        CheckComboCount(ActiveComboBox)
    End Sub

    Public Sub SetSelectedItem(ImageData As ImageData)
        RefreshComboEnabled(ComboAll)
        RefreshItemText()
        ComboAll.SelectedItem = ImageData
        EnsureItemSelected(ComboAll)
    End Sub

    Private Sub CheckComboCount(Combo As ComboBox)
        If Combo.Items.Count = 0 Then
            RefreshComboEnabled(Combo)
            DoSelectedIndexChanged()
        End If
    End Sub

    Private Sub ComboImagesClear(Combo As ComboBox)
        Combo.Items.Clear()
        RefreshComboEnabled(Combo)
    End Sub

    Private Sub DisplayFiltered()
        _Filtered = True

        ComboAll.Visible = False
        ComboFiltered.Visible = True

        RefreshComboEnabled(ComboFiltered)
        If ComboFiltered.Items.Count = 0 Then
            ComboAll.SelectedIndex = -1
            DoSelectedIndexChanged()
        End If
    End Sub

    Private Sub DoSelectedIndexChanged()
        If _LastSelected Is ComboAll.SelectedItem Then
            Exit Sub
        End If

        _LastSelected = ComboAll.SelectedItem

        Debug.Print("Selected item changed: " & If(_LastSelected?.DisplayPath, "(none)"))

        RaiseEvent SelectedIndexChanged()
    End Sub

    Private Sub EnsureItemSelected(Combo As ComboBox)
        If Combo.SelectedIndex = -1 AndAlso Combo.Items.Count > 0 Then
            Combo.SelectedIndex = 0
        End If
    End Sub

    Private Sub Initialize()
        With ComboAll
            .DropDownStyle = ComboBoxStyle.DropDownList
            .Sorted = True
            .Visible = True
            .AllowDrop = True
        End With

        Dim Parent = ComboAll.Parent
        If Parent Is Nothing Then
            Throw New InvalidOperationException("Main ComboBox must have a parent before LoadedImageList is created.")
        End If

        Parent.SuspendLayout()

        ComboFiltered = New ComboBox() With {
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Dock = DockStyle.Fill,
            .Visible = False,
            .Sorted = ComboAll.Sorted,
            .Font = ComboAll.Font,
            .IntegralHeight = ComboAll.IntegralHeight,
            .MaxDropDownItems = ComboAll.MaxDropDownItems,
            .DrawMode = ComboAll.DrawMode,
            .AllowDrop = ComboAll.AllowDrop
        }

        Parent.Controls.Add(ComboFiltered)

        Dim MainIndex = Parent.Controls.GetChildIndex(ComboAll)
        Parent.Controls.SetChildIndex(ComboFiltered, MainIndex)

        Parent.ResumeLayout()
    End Sub

    Private Sub RefreshComboEnabled(Combo As ComboBox)
        Dim Enabled As Boolean = Combo.Items.Count > 0
        Combo.Enabled = Enabled
        Combo.DrawMode = If(Enabled, DrawMode.OwnerDrawFixed, DrawMode.Normal)
    End Sub
#Region "Events"
    Public Sub Combo_DragDrop(sender As Object, e As DragEventArgs) Handles ComboAll.DragDrop, ComboFiltered.DragDrop
        RaiseEvent DragDrop(sender, e)
    End Sub

    Private Sub Combo_DragEnter(sender As Object, e As DragEventArgs) Handles ComboAll.DragEnter, ComboFiltered.DragEnter
        RaiseEvent DragEnter(sender, e)
    End Sub

    Private Sub Combo_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ComboAll.DrawItem, ComboFiltered.DrawItem
        If e.Index >= -1 Then
            e.DrawBackground()

            If e.Index > -1 Then
                Dim CB As ComboBox = sender
                Dim tBrush As Brush

                If e.State And DrawItemState.Selected Then
                    tBrush = SystemBrushes.HighlightText
                Else
                    Dim ImageData As ImageData = CB.Items(e.Index)
                    If ImageData IsNot Nothing AndAlso ImageData.IsModified Then
                        tBrush = Brushes.Blue
                    Else
                        tBrush = SystemBrushes.WindowText
                    End If
                End If

                Dim Format As New StringFormat With {
                    .Trimming = StringTrimming.None,
                    .FormatFlags = StringFormatFlags.NoWrap
                }
                e.Graphics.DrawString(CB.Items(e.Index).ToString, e.Font, tBrush, e.Bounds, Format)
            End If
        End If

        e.DrawFocusRectangle()
    End Sub

    Private Sub Combo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboAll.SelectedIndexChanged
        DoSelectedIndexChanged()
    End Sub

    Private Sub ComboFiltered_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboFiltered.SelectedIndexChanged
        ComboAll.SelectedItem = ComboFiltered.SelectedItem
    End Sub
#End Region
End Class
