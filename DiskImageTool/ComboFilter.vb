Public Class ComboFilter
    Private ReadOnly _Combo As ToolStripComboBox
    Private ReadOnly _Filter As Dictionary(Of String, ComboFilterItem)
    Public Sub New(ComboBox As ToolStripComboBox)
        _Combo = ComboBox
        _Combo.ComboBox.DrawMode = DrawMode.OwnerDrawFixed
        AddHandler _Combo.ComboBox.DrawItem, AddressOf DrawItem

        _Filter = New Dictionary(Of String, ComboFilterItem)
    End Sub

    Public Sub Add(Name As String, UpdateFilters As Boolean)
        If Name = "" Then
            Return
        End If

        If _Filter.ContainsKey(Name) Then
            Dim Item = _Filter.Item(Name)
            Item.Count += 1

            If UpdateFilters Then
                Dim Index = _Combo.Items.IndexOf(Item)
                _Combo.Items.Item(Index) = Item
            End If
        Else
            Dim Item = New ComboFilterItem With {
                .Name = Name,
                .Count = 1
            }
            _Filter.Add(Item.Name, Item)

            If UpdateFilters Then
                _Combo.Items.Add(Item)
            End If
        End If
    End Sub

    Public Sub Clear()
        _Filter.Clear()
        _Combo.Items.Clear()
    End Sub

    Public Sub ClearFilter()
        If _Combo.Items.Count > 0 Then
            _Combo.SelectedIndex = 0
        End If
    End Sub

    Public Sub Populate()
        Dim Item As ComboFilterItem

        _Combo.BeginUpdate()
        _Combo.Items.Clear()
        _Combo.Sorted = True
        For Each Item In _Filter.Values
            _Combo.Items.Add(Item)
        Next
        Item = New ComboFilterItem With {
            .AllItems = True
        }
        _Combo.Sorted = False
        _Combo.Items.Insert(0, Item)
        _Combo.SelectedIndex = 0
        _Combo.EndUpdate()
    End Sub

    Public Sub Remove(Name As String, UpdateFilters As Boolean)
        If Name = "" Then
            Return
        End If

        If _Filter.ContainsKey(Name) Then
            Dim Item = _Filter.Item(Name)
            Item.Count -= 1
            If Item.Count = 0 Then
                _Filter.Remove(Item.Name)

                If UpdateFilters Then
                    _Combo.Items.Remove(Item)
                    If _Combo.SelectedIndex = -1 Then
                        _Combo.SelectedIndex = 0
                    End If
                End If
            Else
                If UpdateFilters Then
                    Dim Index = _Combo.Items.IndexOf(Item)
                    _Combo.Items.Item(Index) = Item
                End If
            End If
        End If
    End Sub

    Private Sub DrawItem(ByVal sender As Object, ByVal e As DrawItemEventArgs)
        e.DrawBackground()

        If e.Index >= 0 Then
            Dim Item As ComboFilterItem = _Combo.Items(e.Index)

            Dim Brush As Brush
            Dim tBrush As Brush
            Dim nBrush As Brush

            If e.State And DrawItemState.Selected Then
                Brush = New SolidBrush(SystemColors.Highlight)
                tBrush = New SolidBrush(SystemColors.HighlightText)
                nBrush = New SolidBrush(SystemColors.HighlightText)
            Else
                Brush = New SolidBrush(SystemColors.Window)
                tBrush = New SolidBrush(SystemColors.WindowText)
                nBrush = New SolidBrush(Color.Blue)
            End If

            e.Graphics.FillRectangle(Brush, e.Bounds)

            Dim Name As String
            Dim Count As String
            If Item.AllItems Then
                Name = "(ALL)"
                Count = ""
            Else
                Name = Item.Name
                Count = Item.Count
            End If

            Dim r1 As Rectangle = e.Bounds

            If (e.State And DrawItemState.ComboBoxEdit) <> DrawItemState.ComboBoxEdit Then
                If Count <> "" Then
                    Dim Width = TextRenderer.MeasureText(Count, e.Font).Width + 2
                    r1.Width -= Width
                    Dim r2 As Rectangle = e.Bounds
                    r2.X = r2.Width - Width
                    e.Graphics.DrawString(Count, e.Font, nBrush, r2, StringFormat.GenericDefault)
                End If
            End If

            e.Graphics.DrawString(Name, e.Font, tBrush, r1, StringFormat.GenericDefault)

            nBrush.Dispose()
            tBrush.Dispose()
            Brush.Dispose()
        End If

        e.DrawFocusRectangle()
    End Sub
End Class

Public Class ComboFilterItem
    Public Property AllItems As Boolean = False
    Public Property Count As Integer
    Public Property Name As String

    Public Overrides Function ToString() As String
        If AllItems Then
            Return "(ALL)"
        Else
            Return Name & "  [" & _Count & "]"
        End If
    End Function

End Class
