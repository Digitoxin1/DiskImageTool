Public Class LoadedImageList
    Private WithEvents Combo As ComboBox
    Private ReadOnly _Filtered As List(Of ImageData)
    Private ReadOnly _Main As List(Of ImageData)

    Private _CurrentSelected As ImageData = Nothing
    Private _IsFiltered As Boolean = False
    Private _SuppressEvent As Boolean = False

    Public Event SelectedImageChanged()

    Public Sub New(Combo As ComboBox)
        Me.Combo = Combo

        _Main = New List(Of ImageData)
        _Filtered = New List(Of ImageData)

        Initialize()
    End Sub

    Public ReadOnly Property Filtered As List(Of ImageData)
        Get
            Return _Filtered
        End Get
    End Property

    Public ReadOnly Property Main As List(Of ImageData)
        Get
            Return _Main
        End Get
    End Property

    Public ReadOnly Property SelectedImage As ImageData
        Get
            Return _CurrentSelected
        End Get
    End Property

    Public Sub ClearAll()
        _Main.Clear()
        _Filtered.Clear()
        _IsFiltered = False

        Combo.Items.Clear()
        RefreshComboEnabled(False)
        SetCurrentSelected(Nothing)
    End Sub

    Public Function GetModifiedImageList() As List(Of ImageData)
        Dim ModifyImageList As New List(Of ImageData)

        For Each ImageData In _Main
            If ImageData.IsModified Then
                ModifyImageList.Add(ImageData)
            End If
        Next

        Return ModifyImageList
    End Function

    Public Function GetPathOffset() As Integer
        Dim PathName As String = ""
        Dim CheckPath As Boolean = False

        For Each ImageData In _Main
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

    Public Sub Refresh(IsFiltered As Boolean)
        _IsFiltered = IsFiltered

        Dim Source As List(Of ImageData) = If(_IsFiltered, _Filtered, _Main)

        RefreshComboEnabled(Source.Count > 0)
        PopulateCombo(Source)

        SetSelectedImage(_CurrentSelected)
    End Sub

    Public Sub RefreshCurrentItemText()
        Combo.Invalidate()
    End Sub

    Public Sub RefreshPaths()
        ImageData.StringOffset = GetPathOffset()

        Combo.BeginUpdate()
        _SuppressEvent = True

        For Index = 0 To Combo.Items.Count - 1
            Combo.Items(Index) = Combo.Items(Index)
        Next

        _SuppressEvent = False
        Combo.EndUpdate()
    End Sub

    Public Sub RemoveImage(Image As ImageData)
        Dim SelectedIndex = Combo.SelectedIndex

        _Main.Remove(Image)
        _Filtered.Remove(Image)
        Combo.Items.Remove(Image)

        If Combo.Items.Count > 0 Then
            If Combo.SelectedIndex = -1 Then
                If SelectedIndex > Combo.Items.Count - 1 Then
                    SelectedIndex = Combo.Items.Count - 1
                End If

                If SelectedIndex >= 0 Then
                    Combo.SelectedIndex = SelectedIndex
                End If
            End If
        Else
            RefreshComboEnabled(False)
            SetCurrentSelected(Nothing)
        End If
    End Sub

    Public Sub SetSelectedImage(ImageData As ImageData)
        If ImageData IsNot Nothing AndAlso Combo.Items.Contains(ImageData) Then
            Combo.SelectedItem = ImageData

        ElseIf Combo.Items.Count > 0 AndAlso Combo.SelectedIndex = -1 Then
            Combo.SelectedIndex = 0

        Else
            SetCurrentSelected(Nothing)
        End If
    End Sub

    Private Sub Initialize()
        With Combo
            .DropDownStyle = ComboBoxStyle.DropDownList
            .Sorted = True
            .Visible = True
        End With
    End Sub

    Private Sub PopulateCombo(list As List(Of ImageData))
        _SuppressEvent = True

        Combo.BeginUpdate()
        Combo.Items.Clear()
        For Each img In list
            Combo.Items.Add(img)
        Next
        Combo.EndUpdate()

        _SuppressEvent = False
    End Sub

    Private Sub RefreshComboEnabled(Enabled As Boolean)
        Combo.Enabled = Enabled
        Combo.DrawMode = If(Enabled, DrawMode.OwnerDrawFixed, DrawMode.Normal)
    End Sub

    Private Sub SetCurrentSelected(Image As ImageData)
        If _CurrentSelected IsNot Image Then
            _CurrentSelected = Image
            RaiseEvent SelectedImageChanged()
        End If
    End Sub
#Region "Events"
    Private Sub Combo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Combo.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Debug.Print("LoadedImageList.Combo_SelectedIndexChanged fired")
        SetCurrentSelected(TryCast(Combo.SelectedItem, ImageData))
    End Sub

    Private Sub ComboImages_DrawItem(sender As Object, e As DrawItemEventArgs) Handles Combo.DrawItem
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
