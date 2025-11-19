Public Class LoadedImageList
    Private WithEvents Combo As ComboBox
    Private WithEvents ComboFiltered As ComboBox
    Private _Filtered As Boolean = False
    Private _SuppressEvent As Boolean = False

    Public Event SelectedIndexChanged As EventHandler

    Public Sub New(Combo As ComboBox, ComboFiltered As ComboBox)
        Me.Combo = Combo
        Me.ComboFiltered = ComboFiltered

        Initialize()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()

        Combo = Nothing
        ComboFiltered = Nothing
    End Sub

    Public ReadOnly Property Filtered As ComboBox
        Get
            Return ComboFiltered
        End Get
    End Property

    Public ReadOnly Property Main As ComboBox
        Get
            Return Combo
        End Get
    End Property

    Public ReadOnly Property SelectedImage As ImageData
        Get
            Return Combo.SelectedItem
        End Get
    End Property

    Public Sub AddFilteredItem(ImageData As ImageData)
        Dim Index = ComboFiltered.Items.Add(ImageData)
        If ImageData Is Combo.SelectedItem Then
            ComboFiltered.SelectedIndex = Index
        End If
    End Sub

    Public Sub ClearAll()
        ClearMain()
        ToggleFiltered(False)
    End Sub

    Public Sub ClearMain()
        ComboImagesClear(Combo)
    End Sub

    Public Sub EnsureFilteredImageSelected()
        EnsureImageSelected(ComboFiltered)
    End Sub

    Public Sub EnsureMainImageSelected()
        EnsureImageSelected(Combo)
    End Sub

    Public Function GetModifiedImageList() As List(Of ImageData)
        Dim ModifyImageList As New List(Of ImageData)

        For Each ImageData As ImageData In Combo.Items
            If ImageData.IsModified Then
                ModifyImageList.Add(ImageData)
            End If
        Next

        Return ModifyImageList
    End Function

    Public Function GetPathOffset() As Integer
        Dim PathName As String = ""
        Dim CheckPath As Boolean = False

        For Each ImageData As ImageData In Combo.Items
            Dim CurrentPathName As String = IO.Path.GetDirectoryName(ImageData.DisplayPath)
            If CheckPath Then
                Do While CurrentPathName.Split("\").Count > PathName.Split("\").Count
                    If CurrentPathName <> "" Then
                        CurrentPathName = IO.Path.GetDirectoryName(CurrentPathName)
                    End If
                Loop
                Do While PathName <> CurrentPathName
                    PathName = IO.Path.GetDirectoryName(PathName)
                    If CurrentPathName <> "" Then
                        CurrentPathName = IO.Path.GetDirectoryName(CurrentPathName)
                    End If
                Loop
            Else
                PathName = CurrentPathName
            End If
            If PathName = "" Then
                Exit For
            End If
            CheckPath = True
        Next
        PathName = PathAddBackslash(PathName)

        Return Len(PathName)
    End Function

    Public Sub RefreshCurrentItemText()
        Combo.Invalidate()
        ComboFiltered.Invalidate()
    End Sub

    Public Sub RefreshItemText()
        _SuppressEvent = True

        For Index = 0 To Combo.Items.Count - 1
            Combo.Items(Index) = Combo.Items(Index)
        Next

        For Index = 0 To ComboFiltered.Items.Count - 1
            ComboFiltered.Items(Index) = ComboFiltered.Items(Index)
        Next

        _SuppressEvent = False
    End Sub

    Public Sub RefreshMain()
        RefreshComboEnabled(Combo)
    End Sub

    Public Sub RefreshPaths()
        Combo.BeginUpdate()
        ImageData.StringOffset = GetPathOffset()
        RefreshItemText()
        Combo.EndUpdate()
    End Sub

    Public Sub RemoveImage(Value As ImageData)
        Dim ActiveComboBox As ComboBox = If(_Filtered, ComboFiltered, Combo)

        Dim SelectedIndex = ActiveComboBox.SelectedIndex

        Combo.Items.Remove(Value)
        ComboFiltered.Items.Remove(Value)

        If ActiveComboBox.SelectedIndex = -1 Then
            If SelectedIndex > ActiveComboBox.Items.Count - 1 Then
                SelectedIndex = ActiveComboBox.Items.Count - 1
            End If
            ActiveComboBox.SelectedIndex = SelectedIndex
        End If

        CheckComboCount(ActiveComboBox)
    End Sub

    Public Sub SetSelectedImage(ImageData As ImageData)
        RefreshMain()
        RefreshItemText()
        Combo.SelectedItem = ImageData
        EnsureMainImageSelected()
    End Sub

    Public Sub ToggleFiltered(Filtered As Boolean)
        _Filtered = Filtered

        Combo.Visible = Not Filtered
        ComboFiltered.Visible = Filtered

        If Filtered Then
            RefreshComboEnabled(ComboFiltered)
            If ComboFiltered.Items.Count = 0 Then
                Combo.SelectedIndex = -1
                RaiseEvent SelectedIndexChanged(Me, New EventArgs())
            End If
        Else
            ComboImagesClear(ComboFiltered)
            EnsureImageSelected(Combo)
        End If
    End Sub

    Private Sub CheckComboCount(Combo As ComboBox)
        If Combo.Items.Count = 0 Then
            RefreshComboEnabled(Combo)
            RaiseEvent SelectedIndexChanged(Me, New EventArgs())
        End If
    End Sub

    Private Sub ComboImagesClear(Combo As ComboBox)
        Combo.Items.Clear()
        RefreshComboEnabled(Combo)
    End Sub

    Private Sub EnsureImageSelected(Combo As ComboBox)
        If Combo.SelectedIndex = -1 AndAlso Combo.Items.Count > 0 Then
            Combo.SelectedIndex = 0
        End If
    End Sub
    Private Sub Initialize()
        With Combo
            .DropDownStyle = ComboBoxStyle.DropDownList
            .Sorted = True
            .Visible = False
        End With

        With ComboFiltered
            .DropDownStyle = ComboBoxStyle.DropDownList
            .Sorted = True
            .Visible = True
        End With
    End Sub
    Private Sub RefreshComboEnabled(Combo As ComboBox)
        Dim Enabled As Boolean = Combo.Items.Count > 0
        Combo.Enabled = Enabled
        If Enabled Then
            Combo.DrawMode = DrawMode.OwnerDrawFixed
        Else
            Combo.DrawMode = DrawMode.Normal
        End If
    End Sub
#Region "Events"
    Private Sub Combo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Combo.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Debug.Print("LoadedImageList.Combo_SelectedIndexChanged fired")

        RaiseEvent SelectedIndexChanged(sender, e)
    End Sub

    Private Sub ComboFiltered_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboFiltered.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Debug.Print("LoadedImageList.ComboFiltered_SelectedIndexChanged fired")

        Combo.SelectedItem = ComboFiltered.SelectedItem
    End Sub

    Private Sub ComboImages_DrawItem(sender As Object, e As DrawItemEventArgs) Handles Combo.DrawItem, ComboFiltered.DrawItem
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
#End Region
End Class
