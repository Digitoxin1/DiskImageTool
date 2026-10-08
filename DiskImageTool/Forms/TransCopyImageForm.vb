Imports DiskImageTool.DiskImage
Imports DiskImageTool.ImageFormats.TC

Public Class TransCopyImageForm
    Private Const GRID_COLUMN_TRACK As String = "GridTrack"
    Private Const GRID_COLUMN_SIDE As String = "GridSide"
    Private Const GRID_COLUMN_OFFSET As String = "GridOffset"
    Private Const GRID_COLUMN_SKEW As String = "GridSkew"
    Private Const GRID_COLUMN_LENGTH As String = "GridLength"
    Private Const GRID_COLUMN_TRACK_TYPE As String = "GridTrackType"
    Private Const GRID_COLUMN_COPY_ACROSS_INDEX As String = "GridCopyAcrossIndex"
    Private Const GRID_COLUMN_COPY_WEAK_BITS As String = "GridCopyWeakBits"
    Private Const GRID_COLUMN_KEEP_TRACK_LENGTH As String = "GridKeepTrackLength"
    Private Const GRID_COLUMN_LENGTH_TOLERANCE As String = "GridLengthTolerance"
    Private Const GRID_COLUMN_NO_ADDRESS_MARKS As String = "GridNoAddressMarks"
    Private Const GRID_COLUMN_VERIFY_WRITE As String = "GridVerifyWrite"
    Private Const GRID_COLUMN_BITRATE As String = "GridBitRate"
    Private Const GRID_COLUMN_RPM As String = "GridRPM"

    Public Sub New(Image As TransCopyImage)

        ' This call is required by the designer.
        InitializeComponent()

        GetType(DataGridView).InvokeMember(
            "DoubleBuffered",
            Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.SetProperty,
            Nothing,
            DataGridViewTracks,
            New Object() {True})

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()
        InitializeGridColumns()
        PopulateHeader(Image)
        DataGridViewTracks.DataSource = GetTrackTable(Image)
    End Sub

    Public Shared Sub Display(Disk As Disk)
        If Disk Is Nothing OrElse Disk.Image Is Nothing OrElse Disk.Image.ImageType <> FloppyImageType.TranscopyImage Then
            Exit Sub
        End If

        Dim Image As TransCopyImage = DirectCast(Disk.Image, TranscopyFloppyImage).Image
        Using dlg As New TransCopyImageForm(Image)
            dlg.ShowDialog(App.CurrentFormInstance)
        End Using
    End Sub

    Private Sub LocalizeForm()
        Me.Text = WithoutHotkey(My.Resources.Menu_ImageProperties)
        BtnClose.Text = My.Resources.Menu_Close
        LblComment.Text = My.Resources.Label_Comment
        LblComment2.Text = My.Resources.Label_Comment2
        LblDiskType.Text = My.Resources.SummaryPanel_DiskType
        LblTrackStart.Text = My.Resources.Label_StartingTrack
        LblTrackEnd.Text = My.Resources.Label_EndingTrack
        LblSides.Text = My.Resources.Label_Sides
        LblTrackIncrement.Text = My.Resources.Label_TrackIncrement
    End Sub

    Private Sub PopulateHeader(Image As TransCopyImage)
        TxtComment.Text = Image.Comment
        TxtComment2.Text = Image.Comment2
        TxtDiskType.Text = DiskTypeToString(Image.DiskType)
        TxtTrackStart.Text = Image.TrackStart.ToString()
        TxtTrackEnd.Text = Image.TrackEnd.ToString()
        TxtSides.Text = Image.SideCount.ToString()
        TxtTrackIncrement.Text = Image.TrackIncrement.ToString()
    End Sub

    Private Sub InitializeGridColumns()
        DataGridViewTracks.AutoGenerateColumns = False
        DataGridViewTracks.Columns.Clear()

        AddTextColumn(GRID_COLUMN_TRACK, My.Resources.Label_Track, 55, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(GRID_COLUMN_SIDE, My.Resources.Label_Side, 50, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(GRID_COLUMN_OFFSET, My.Resources.Label_OffsetHex, 80, DataGridViewContentAlignment.MiddleRight, "X8")
        AddTextColumn(GRID_COLUMN_SKEW, My.Resources.Label_Skew, 60, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(GRID_COLUMN_LENGTH, My.Resources.Label_Length, 65, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(GRID_COLUMN_TRACK_TYPE, My.Resources.Label_TrackType, 180, DataGridViewContentAlignment.MiddleLeft)
        AddTextColumn(GRID_COLUMN_BITRATE, My.Resources.SummaryPanel_Bitrate, 65, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(GRID_COLUMN_RPM, My.Resources.SummaryPanel_RPM, 55, DataGridViewContentAlignment.MiddleRight)
        AddCheckColumn(GRID_COLUMN_COPY_ACROSS_INDEX, My.Resources.Label_CopyAcrossIndex, 120)
        AddCheckColumn(GRID_COLUMN_COPY_WEAK_BITS, My.Resources.Label_CopyWeakBits, 115)
        AddCheckColumn(GRID_COLUMN_KEEP_TRACK_LENGTH, My.Resources.Label_KeepTrackLength, 120)
        AddCheckColumn(GRID_COLUMN_LENGTH_TOLERANCE, My.Resources.Label_LengthTolerance, 115)
        AddCheckColumn(GRID_COLUMN_NO_ADDRESS_MARKS, My.Resources.Label_NoAddressMarks, 125)
        AddCheckColumn(GRID_COLUMN_VERIFY_WRITE, My.Resources.Label_VerifyWrites, 95)
    End Sub

    Private Sub AddTextColumn(Name As String, HeaderText As String, Width As Integer, Alignment As DataGridViewContentAlignment, Optional Format As String = "")
        Dim Column As New DataGridViewTextBoxColumn With {
            .Name = Name,
            .HeaderText = HeaderText,
            .ReadOnly = True,
            .DataPropertyName = Name,
            .Width = Width,
            .SortMode = DataGridViewColumnSortMode.NotSortable
        }
        Column.DefaultCellStyle.Alignment = Alignment
        If Format <> "" Then
            Column.DefaultCellStyle.Format = Format
        End If
        DataGridViewTracks.Columns.Add(Column)
    End Sub

    Private Sub AddCheckColumn(Name As String, HeaderText As String, Width As Integer)
        Dim Column As New DataGridViewCheckBoxColumn With {
            .Name = Name,
            .HeaderText = HeaderText,
            .ReadOnly = True,
            .DataPropertyName = Name,
            .Width = Width,
            .SortMode = DataGridViewColumnSortMode.NotSortable,
            .ThreeState = False
        }
        Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridViewTracks.Columns.Add(Column)
    End Sub

    Private Function GetTrackTable(Image As TransCopyImage) As DataTable
        Dim TrackTable As New DataTable("TransCopyTracks")

        AddDataColumn(TrackTable, GRID_COLUMN_TRACK, GetType(UShort))
        AddDataColumn(TrackTable, GRID_COLUMN_SIDE, GetType(Byte))
        AddDataColumn(TrackTable, GRID_COLUMN_OFFSET, GetType(UInteger))
        AddDataColumn(TrackTable, GRID_COLUMN_SKEW, GetType(UShort))
        AddDataColumn(TrackTable, GRID_COLUMN_LENGTH, GetType(UShort))
        AddDataColumn(TrackTable, GRID_COLUMN_TRACK_TYPE, GetType(String))
        AddDataColumn(TrackTable, GRID_COLUMN_BITRATE, GetType(UShort))
        AddDataColumn(TrackTable, GRID_COLUMN_RPM, GetType(UShort))
        AddDataColumn(TrackTable, GRID_COLUMN_COPY_ACROSS_INDEX, GetType(Boolean))
        AddDataColumn(TrackTable, GRID_COLUMN_COPY_WEAK_BITS, GetType(Boolean))
        AddDataColumn(TrackTable, GRID_COLUMN_KEEP_TRACK_LENGTH, GetType(Boolean))
        AddDataColumn(TrackTable, GRID_COLUMN_LENGTH_TOLERANCE, GetType(Boolean))
        AddDataColumn(TrackTable, GRID_COLUMN_NO_ADDRESS_MARKS, GetType(Boolean))
        AddDataColumn(TrackTable, GRID_COLUMN_VERIFY_WRITE, GetType(Boolean))

        If Image.SideCount > 0 Then
            For Track As Integer = Image.TrackStart To Image.TrackEnd
                For Side As Integer = 0 To Image.SideCount - 1
                    Dim Row = TrackTable.NewRow()
                    Dim TrackData = Image.GetTrack(CByte(Track), CByte(Side))
                    If TrackData Is Nothing Then
                        Row(GRID_COLUMN_TRACK) = CUShort(Track)
                        Row(GRID_COLUMN_SIDE) = CByte(Side)
                    Else
                        PopulateTrackRow(Row, TrackData)
                    End If
                    TrackTable.Rows.Add(Row)
                Next
            Next
        End If

        Return TrackTable
    End Function

    Private Sub PopulateTrackRow(Row As DataRow, TrackData As TransCopyTrack)
        Row(GRID_COLUMN_TRACK) = TrackData.Track
        Row(GRID_COLUMN_SIDE) = TrackData.Side
        Row(GRID_COLUMN_OFFSET) = TrackData.Offset
        Row(GRID_COLUMN_SKEW) = TrackData.Skew
        Row(GRID_COLUMN_LENGTH) = TrackData.Length
        Row(GRID_COLUMN_TRACK_TYPE) = DiskTypeToString(TrackData.TrackType)
        Row(GRID_COLUMN_COPY_ACROSS_INDEX) = TrackData.CopyAcrossIndex
        Row(GRID_COLUMN_COPY_WEAK_BITS) = TrackData.CopyWeakBits
        Row(GRID_COLUMN_KEEP_TRACK_LENGTH) = TrackData.KeepTrackLength
        Row(GRID_COLUMN_LENGTH_TOLERANCE) = TrackData.LengthTolerance
        Row(GRID_COLUMN_NO_ADDRESS_MARKS) = TrackData.NoAddressMarks
        Row(GRID_COLUMN_VERIFY_WRITE) = TrackData.VerifyWrite

        If TrackData.Bitstream IsNot Nothing AndAlso TrackData.Bitstream.Length > 0 Then
            Row(GRID_COLUMN_BITRATE) = TrackData.BitRate
            Row(GRID_COLUMN_RPM) = TrackData.RPM
        End If
    End Sub

    Private Sub AddDataColumn(Table As DataTable, Name As String, DataType As Type, Optional Width As Integer = 0)
        Dim Column As New DataColumn(Name, DataType)

        If DataType Is GetType(Boolean) Then
            Column.DefaultValue = False
        ElseIf DataType Is GetType(String) Then
            Column.DefaultValue = ""
        End If

        If Width > 0 Then
            Column.ExtendedProperties("Width") = Width
        End If

        Table.Columns.Add(Column)
    End Sub

    Private Sub ApplyColumnWidths(Grid As DataGridView)
        Dim Table = TryCast(Grid.DataSource, DataTable)

        If Table Is Nothing Then Exit Sub

        For Each Column As DataColumn In Table.Columns
            If Column.ExtendedProperties.ContainsKey("Width") Then
                Dim GridColumn = Grid.Columns(Column.ColumnName)

                If GridColumn IsNot Nothing Then
                    GridColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                    GridColumn.Width = CInt(Column.ExtendedProperties("Width"))
                    GridColumn.Resizable = DataGridViewTriState.False
                End If
            End If
        Next
    End Sub

    Private Sub DataGridViewTracks_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridViewTracks.DataBindingComplete
        DataGridViewTracks.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
    End Sub
End Class
