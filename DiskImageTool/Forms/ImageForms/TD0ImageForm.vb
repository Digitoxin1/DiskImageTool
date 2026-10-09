Imports DiskImageTool.DiskImage
Imports DiskImageTool.ImageFormats.TD0

Public Class TD0ImageForm
    Private Const GRID_COLUMN_CYLINDER As String = "GridCylinder"
    Private Const GRID_COLUMN_HEAD As String = "GridHead"
    Private Const GRID_COLUMN_SECTORS As String = "GridSectors"
    Private Const GRID_COLUMN_FM As String = "GridFM"
    Private Const GRID_COLUMN_SECTOR As String = "GridSector"
    Private Const GRID_COLUMN_SIZE As String = "GridSize"
    Private Const GRID_COLUMN_DUPLICATE As String = "GridDuplicate"
    Private Const GRID_COLUMN_CRC_ERROR As String = "GridCrcError"
    Private Const GRID_COLUMN_DELETED As String = "GridDeleted"
    Private Const GRID_COLUMN_SKIPPED As String = "GridSkipped"
    Private Const GRID_COLUMN_NO_DATA As String = "GridNoData"
    Private Const GRID_COLUMN_NO_ID As String = "GridNoId"

    Private ReadOnly _FloppyImage As TD0FloppyImage
    Private ReadOnly _CrcErrorEdits As New Dictionary(Of Long, Boolean)
    Private _LoadingSectors As Boolean
    Private _Updated As Boolean

    Public Sub New(FloppyImage As TD0FloppyImage)

        ' This call is required by the designer.
        InitializeComponent()

        EnableDoubleBuffer(DataGridViewTracks)
        EnableDoubleBuffer(DataGridViewSectors)
        DtpTimestamp.MinDate = New DateTime(1900, 1, 1)
        DtpTimestamp.MaxDate = New DateTime(2155, 12, 31, 23, 59, 59)

        ' Add any initialization after the InitializeComponent() call.
        _FloppyImage = FloppyImage
        Dim Image = FloppyImage.Image
        LocalizeForm()
        InitializeGridColumns()
        PopulateHeader(Image)
        DataGridViewTracks.DataSource = GetTrackTable(Image)
        ShowSelectedSectors()
    End Sub

    Public Shared Function Display(Disk As Disk) As Boolean
        If Disk Is Nothing OrElse Disk.Image Is Nothing OrElse Disk.Image.ImageType <> FloppyImageType.TD0Image Then
            Return False
        End If

        Dim FloppyImage = DirectCast(Disk.Image, TD0FloppyImage)
        Using dlg As New TD0ImageForm(FloppyImage)
            dlg.ShowDialog(App.CurrentFormInstance)
            Return dlg._Updated
        End Using
    End Function

    Private Sub LocalizeForm()
        Me.Text = "TD0 " & WithoutHotkey(My.Resources.Menu_ImageProperties)
        BtnCancel.Text = My.Resources.Menu_Cancel
        BtnUpdate.Text = My.Resources.Menu_Update
        LblCompression.Text = My.Resources.Label_Compression
        LblVersion.Text = My.Resources.Label_Version
        LblSequence.Text = My.Resources.Label_Sequence
        LblCheckSequence.Text = My.Resources.Label_CheckSequence
        LblDataRate.Text = My.Resources.Label_DataRate
        LblSingleDensity.Text = My.Resources.Label_SingleDensity
        LblDriveType.Text = My.Resources.Label_HeaderDriveType
        LblStepping.Text = My.Resources.Label_Stepping
        LblDosAllocation.Text = My.Resources.Label_DosAllocation
        LblSides.Text = My.Resources.Label_Sides
        LblComment.Text = My.Resources.Label_Comment
        LblTimestamp.Text = My.Resources.Label_Timestamp
        LblTracks.Text = My.Resources.Label_Tracks
        LblSectors.Text = My.Resources.Label_Sectors
    End Sub

    Private Sub PopulateHeader(Image As TD0Image)
        Dim Header = Image.Header
        TxtCompression.Text = CompressionCaption(Header)
        TxtVersion.Text = Header.VersionString
        TxtSequence.Text = Header.Sequence.ToString()
        TxtCheckSequence.Text = Header.CheckSequence.ToString()
        TxtDataRate.Text = DataRateCaption(Header.DataRate)
        TxtSingleDensity.Text = YesNo(Header.IsSingleDensity)
        TxtDriveType.Text = DriveTypeCaption(Header.DriveType)
        TxtStepping.Text = SteppingCaption(Header.Stepping)
        TxtDosAllocation.Text = Header.DosAllocationFlag.ToString("X2")
        TxtSides.Text = If(Header.Sides = 1, My.Resources.StepMode_Single, My.Resources.StepMode_Double)
        PopulateComment(Image.Comment)
    End Sub

    Private Sub PopulateComment(Comment As TD0Comment)
        TxtComment.Text = ""
        DtpTimestamp.Checked = False

        If Comment Is Nothing Then
            Exit Sub
        End If

        TxtComment.Text = Comment.Text
        Dim Timestamp = Comment.GetTimestamp()
        If Timestamp.HasValue AndAlso Timestamp.Value >= DtpTimestamp.MinDate AndAlso Timestamp.Value <= DtpTimestamp.MaxDate Then
            DtpTimestamp.Value = Timestamp.Value
            DtpTimestamp.Checked = True
        End If
    End Sub

    Private Shared Function YesNo(Value As Boolean) As String
        If Value Then
            Return My.Resources.Label_Yes
        Else
            Return My.Resources.Label_No
        End If
    End Function

    Private Shared Function CompressionCaption(Header As TD0Header) As String
        If Not Header.IsCompressed Then
            Return My.Resources.TD0_Compression_None
        End If

        If Header.VersionMajor >= 2 Then
            Return My.Resources.TD0_Compression_LZHUF
        End If

        Return My.Resources.TD0_Compression_LZW
    End Function

    Private Shared Function DataRateCaption(Rate As TD0DataRate) As String
        Select Case Rate
            Case TD0DataRate.Rate250Kbps
                Return My.Resources.TD0_DataRate_250
            Case TD0DataRate.Rate300Kbps
                Return My.Resources.TD0_DataRate_300
            Case TD0DataRate.Rate500Kbps
                Return My.Resources.TD0_DataRate_500
            Case Else
                Return CByte(Rate).ToString("X2")
        End Select
    End Function

    Private Shared Function DriveTypeCaption(DriveType As TD0DriveType) As String
        Select Case DriveType
            Case TD0DriveType.Drive5_25_96TPI
                Return My.Resources.TD0_Drive_52596Tpi
            Case TD0DriveType.Drive360K
                Return My.Resources.TD0_Drive_360K
            Case TD0DriveType.Drive1200K
                Return My.Resources.TD0_Drive_1200K
            Case TD0DriveType.Drive720K
                Return My.Resources.TD0_Drive_720K
            Case TD0DriveType.Drive1440K
                Return My.Resources.TD0_Drive_1440K
            Case TD0DriveType.Drive8_Inch
                Return My.Resources.TD0_Drive_8Inch
            Case TD0DriveType.Drive3_5_Unknown
                Return My.Resources.TD0_Drive_35Unknown
            Case Else
                Return CByte(DriveType).ToString("X2")
        End Select
    End Function

    Private Shared Function SteppingCaption(Stepping As TD0Stepping) As String
        Select Case Stepping
            Case TD0Stepping.SteppingSingle
                Return My.Resources.StepMode_Single
            Case TD0Stepping.SteppingDouble
                Return My.Resources.StepMode_Double
            Case TD0Stepping.SteppingEvenOnly
                Return My.Resources.TD0_Stepping_EvenOnly
            Case Else
                Return CByte(Stepping).ToString("X2")
        End Select
    End Function

    Private Function BuildCommentSnapshot() As Byte()
        Dim Text = TxtComment.Text
        If Text.Length = 0 Then
            Return Nothing
        End If

        Dim Existing = _FloppyImage.Image.Comment
        If Existing IsNot Nothing AndAlso Text = Existing.Text AndAlso Not TimestampChanged(Existing) Then
            Return Existing.GetBytes()
        End If

        Dim Comment As TD0Comment
        If Existing Is Nothing Then
            Comment = New TD0Comment()
        Else
            Comment = New TD0Comment(Existing.GetBytes(), 0)
        End If

        Comment.Text = Text
        If DtpTimestamp.Checked Then
            Comment.SetTimestamp(DtpTimestamp.Value)
        ElseIf Existing Is Nothing Then
            Comment.SetTimestamp(DateTime.Now)
        End If

        Comment.RefreshStoredCrc16()
        Return Comment.GetBytes()
    End Function

    Private Function TimestampChanged(Existing As TD0Comment) As Boolean
        If Not DtpTimestamp.Checked Then
            Return False
        End If

        Dim Stored = Existing.GetTimestamp()
        If Not Stored.HasValue Then
            Return True
        End If

        Dim Picked = DtpTimestamp.Value
        Return Stored.Value.Year <> Picked.Year OrElse
            Stored.Value.Month <> Picked.Month OrElse
            Stored.Value.Day <> Picked.Day OrElse
            Stored.Value.Hour <> Picked.Hour OrElse
            Stored.Value.Minute <> Picked.Minute OrElse
            Stored.Value.Second <> Picked.Second
    End Function

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        If DataGridViewSectors.IsCurrentCellDirty Then
            DataGridViewSectors.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

        _Updated = _FloppyImage.UpdateProperties(BuildCommentSnapshot(), CollectCrcChanges())
        DialogResult = DialogResult.OK
    End Sub

    Private Sub InitializeGridColumns()
        DataGridViewTracks.DefaultCellStyle.Padding = New Padding(0, 0, 5, 0)
        DataGridViewTracks.AutoGenerateColumns = False
        DataGridViewTracks.Columns.Clear()

        AddTextColumn(DataGridViewTracks, GRID_COLUMN_CYLINDER, My.Resources.Label_Cylinder, 70, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(DataGridViewTracks, GRID_COLUMN_HEAD, My.Resources.Label_Head, 55, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(DataGridViewTracks, GRID_COLUMN_SECTORS, My.Resources.Label_Sectors, 70, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(DataGridViewTracks, GRID_COLUMN_FM, My.Resources.Label_FM, 50, DataGridViewContentAlignment.MiddleLeft)

        DataGridViewSectors.AutoGenerateColumns = False
        DataGridViewSectors.Columns.Clear()
        AddTextColumn(DataGridViewSectors, GRID_COLUMN_CYLINDER, My.Resources.Label_Cylinder, 70, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        AddTextColumn(DataGridViewSectors, GRID_COLUMN_HEAD, My.Resources.Label_Head, 55, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        AddTextColumn(DataGridViewSectors, GRID_COLUMN_SECTOR, My.Resources.Label_Sector, 60, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        AddTextColumn(DataGridViewSectors, GRID_COLUMN_SIZE, My.Resources.Label_Size, 70, DataGridViewContentAlignment.MiddleRight, "N0", Padding:=5)
        AddCheckColumn(DataGridViewSectors, GRID_COLUMN_DUPLICATE, My.Resources.TD0_SectorFlag_Duplicated)
        AddCheckColumn(DataGridViewSectors, GRID_COLUMN_CRC_ERROR, My.Resources.TD0_SectorFlag_CrcError, True)
        AddCheckColumn(DataGridViewSectors, GRID_COLUMN_DELETED, My.Resources.Label_Deleted)
        AddCheckColumn(DataGridViewSectors, GRID_COLUMN_SKIPPED, My.Resources.TD0_SectorFlag_DosSkipped)
        AddCheckColumn(DataGridViewSectors, GRID_COLUMN_NO_DATA, My.Resources.TD0_SectorFlag_NoData)
        AddCheckColumn(DataGridViewSectors, GRID_COLUMN_NO_ID, My.Resources.TD0_SectorFlag_DataNoId)
    End Sub

    Private Shared Sub AddCheckColumn(Grid As DataGridView, Name As String, HeaderText As String, Optional Editable As Boolean = False)
        Dim Column As New DataGridViewCheckBoxColumn With {
            .Name = Name,
            .HeaderText = HeaderText,
            .ReadOnly = Not Editable,
            .DataPropertyName = Name,
            .SortMode = DataGridViewColumnSortMode.NotSortable,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
            .MinimumWidth = 60
        }
        Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Grid.Columns.Add(Column)
    End Sub

    Private Function GetTrackTable(Image As TD0Image) As DataTable
        Dim TrackTable As New DataTable("TD0Tracks")
        AddDataColumn(TrackTable, GRID_COLUMN_CYLINDER, GetType(Byte))
        AddDataColumn(TrackTable, GRID_COLUMN_HEAD, GetType(Byte))
        AddDataColumn(TrackTable, GRID_COLUMN_SECTORS, GetType(Byte))
        AddDataColumn(TrackTable, GRID_COLUMN_FM, GetType(String))

        For Each Track In Image.Tracks
            Dim Row = TrackTable.NewRow()
            Row(GRID_COLUMN_CYLINDER) = Track.Cylinder
            Row(GRID_COLUMN_HEAD) = Track.Head
            Row(GRID_COLUMN_SECTORS) = Track.SectorCount
            Row(GRID_COLUMN_FM) = YesNo(Track.IsFMTrack)
            TrackTable.Rows.Add(Row)
        Next

        Return TrackTable
    End Function

    Private Sub DataGridViewTracks_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridViewTracks.SelectionChanged
        If _FloppyImage Is Nothing Then
            Exit Sub
        End If

        ShowSelectedSectors()
    End Sub

    Private Sub ShowSelectedSectors()
        Dim Tracks = _FloppyImage.Image.Tracks
        Dim Row = DataGridViewTracks.CurrentRow
        Dim Track As TD0Track = Nothing
        If Row IsNot Nothing AndAlso Row.Index >= 0 AndAlso Row.Index < Tracks.Count Then
            Track = Tracks(Row.Index)
        End If

        _LoadingSectors = True
        DataGridViewSectors.DataSource = GetSectorTable(Track, TrackIndex())
        _LoadingSectors = False
    End Sub

    Private Function TrackIndex() As Integer
        Dim Row = DataGridViewTracks.CurrentRow
        If Row Is Nothing OrElse Row.Index < 0 Then
            Return -1
        End If

        Return Row.Index
    End Function

    Private Sub DataGridViewSectors_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DataGridViewSectors.CurrentCellDirtyStateChanged
        If Not DataGridViewSectors.IsCurrentCellDirty Then
            Exit Sub
        End If

        If DataGridViewSectors.CurrentCell Is Nothing OrElse DataGridViewSectors.CurrentCell.OwningColumn.Name <> GRID_COLUMN_CRC_ERROR Then
            Exit Sub
        End If

        DataGridViewSectors.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub

    Private Sub DataGridViewSectors_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewSectors.CellValueChanged
        If _LoadingSectors OrElse e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            Exit Sub
        End If

        If DataGridViewSectors.Columns(e.ColumnIndex).Name <> GRID_COLUMN_CRC_ERROR Then
            Exit Sub
        End If

        Dim Index = TrackIndex()
        If Index < 0 Then
            Exit Sub
        End If

        Dim Value = DataGridViewSectors.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
        If TypeOf Value Is Boolean Then
            _CrcErrorEdits(CrcEditKey(Index, e.RowIndex)) = CBool(Value)
        End If
    End Sub

    Private Function CollectCrcChanges() As List(Of TD0FloppyImage.TD0CrcErrorEdit)
        Dim Changes As New List(Of TD0FloppyImage.TD0CrcErrorEdit)
        For Each Entry In _CrcErrorEdits
            Changes.Add(New TD0FloppyImage.TD0CrcErrorEdit(CInt(Entry.Key >> 16), CInt(Entry.Key And &HFFFF), Entry.Value))
        Next

        Return Changes
    End Function

    Private Shared Function CrcEditKey(TrackIndex As Integer, SectorIndex As Integer) As Long
        Return (CLng(TrackIndex) << 16) Or SectorIndex
    End Function

    Private Function GetSectorTable(Track As TD0Track, TrackIndex As Integer) As DataTable
        Dim SectorTable As New DataTable("TD0Sectors")
        AddDataColumn(SectorTable, GRID_COLUMN_CYLINDER, GetType(Byte))
        AddDataColumn(SectorTable, GRID_COLUMN_HEAD, GetType(Byte))
        AddDataColumn(SectorTable, GRID_COLUMN_SECTOR, GetType(Byte))
        AddDataColumn(SectorTable, GRID_COLUMN_SIZE, GetType(Integer))
        AddDataColumn(SectorTable, GRID_COLUMN_DUPLICATE, GetType(Boolean))
        AddDataColumn(SectorTable, GRID_COLUMN_CRC_ERROR, GetType(Boolean))
        AddDataColumn(SectorTable, GRID_COLUMN_DELETED, GetType(Boolean))
        AddDataColumn(SectorTable, GRID_COLUMN_SKIPPED, GetType(Boolean))
        AddDataColumn(SectorTable, GRID_COLUMN_NO_DATA, GetType(Boolean))
        AddDataColumn(SectorTable, GRID_COLUMN_NO_ID, GetType(Boolean))

        If Track Is Nothing Then
            Return SectorTable
        End If

        For SectorIndex = 0 To Track.Sectors.Count - 1
            Dim Sector = Track.Sectors(SectorIndex)
            Dim Row = SectorTable.NewRow()
            Row(GRID_COLUMN_CYLINDER) = Sector.Header.Cylinder
            Row(GRID_COLUMN_HEAD) = Sector.Header.Head
            Row(GRID_COLUMN_SECTOR) = Sector.Header.SectorId
            Dim Flags = Sector.Header.Flags
            Dim CrcError = (Flags And TD0SectorFlags.CrcError) <> 0
            Dim EditedCrcError As Boolean
            If TrackIndex >= 0 AndAlso _CrcErrorEdits.TryGetValue(CrcEditKey(TrackIndex, SectorIndex), EditedCrcError) Then
                CrcError = EditedCrcError
            End If
            Row(GRID_COLUMN_SIZE) = Sector.Header.GetSectorSizeBytes()
            Row(GRID_COLUMN_DUPLICATE) = (Flags And TD0SectorFlags.Duplicated) <> 0
            Row(GRID_COLUMN_CRC_ERROR) = CrcError
            Row(GRID_COLUMN_DELETED) = (Flags And TD0SectorFlags.DeletedData) <> 0
            Row(GRID_COLUMN_SKIPPED) = (Flags And TD0SectorFlags.DosSkipped) <> 0
            Row(GRID_COLUMN_NO_DATA) = (Flags And TD0SectorFlags.NoData) <> 0
            Row(GRID_COLUMN_NO_ID) = (Flags And TD0SectorFlags.DataNoId) <> 0
            SectorTable.Rows.Add(Row)
        Next

        Return SectorTable
    End Function

    Private Sub ResizeGridWidth(Grid As DataGridView)
        Dim NewWidth As Integer = Grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible)

        If Grid.RowHeadersVisible Then
            NewWidth += Grid.RowHeadersWidth
        End If

        If Grid.Controls.OfType(Of VScrollBar)().Any(Function(s) s.Visible) Then
            NewWidth += SystemInformation.VerticalScrollBarWidth
        End If

        NewWidth += Grid.Width - Grid.ClientSize.Width

        Grid.Width = NewWidth + 4
    End Sub

    Private Sub DataGridViewTracks_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridViewTracks.DataBindingComplete
        DataGridViewTracks.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
        ResizeGridWidth(DataGridViewTracks)
    End Sub

    Private Sub DataGridViewSectors_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridViewSectors.DataBindingComplete
        DataGridViewSectors.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
    End Sub
End Class
