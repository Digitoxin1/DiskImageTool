Imports DiskImageTool.DiskImage
Imports DiskImageTool.ImageFormats.IMD

Public Class IMDImageForm
    Private ReadOnly _Disk As Disk
    Private ReadOnly _ChecksumErrorEdits As New Dictionary(Of Long, Boolean)
    Private _LoadingSectors As Boolean
    Private _Updated As Boolean

    Public Sub New(Disk As Disk)
        InitializeComponent()

        _Disk = Disk

        Text = "IMD " & WithoutHotkey(My.Resources.Menu_ImageProperties)

        LblHeader.Text = My.Resources.Label_Header
        LblComment.Text = My.Resources.Label_Comment
        LblTracks.Text = My.Resources.Label_Tracks
        LblSectors.Text = My.Resources.Label_Sectors
        BtnUpdate.Text = My.Resources.Menu_Update
        BtnCancel.Text = My.Resources.Menu_Cancel

        TxtComment.AcceptsReturn = True
        TxtComment.ScrollBars = ScrollBars.Vertical

        EnableDoubleBuffer(DataGridViewTracks)
        EnableDoubleBuffer(DataGridViewSectors)
        AddTrackColumns()
        AddSectorColumns()
        Populate()
    End Sub

    Public ReadOnly Property Updated As Boolean
        Get
            Return _Updated
        End Get
    End Property

    Public Shared Function Display(Disk As Disk) As Boolean
        Using Form As New IMDImageForm(Disk)
            Form.ShowDialog()
            Return Form.Updated
        End Using
    End Function

    Private Sub AddTrackColumns()
        DataGridViewTracks.DefaultCellStyle.Padding = New Padding(0, 0, 5, 0)

        AddTextColumn(DataGridViewTracks, "Cylinder", My.Resources.Label_Cylinder, 70, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(DataGridViewTracks, "Head", My.Resources.Label_Head, 50, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(DataGridViewTracks, "SectorCount", My.Resources.Label_Sectors, 70, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(DataGridViewTracks, "Mode", My.Resources.Label_Mode, 110, DataGridViewContentAlignment.MiddleLeft)
        AddTextColumn(DataGridViewTracks, "SectorSize", My.Resources.Label_Size, 70, DataGridViewContentAlignment.MiddleRight)
    End Sub

    Private Sub AddSectorColumns()
        AddTextColumn(DataGridViewSectors, "Cylinder", My.Resources.Label_Cylinder, 70, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        AddTextColumn(DataGridViewSectors, "Head", My.Resources.Label_Head, 55, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        AddTextColumn(DataGridViewSectors, "Sector", My.Resources.Label_Sector, 60, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        AddFlagColumn("ChecksumError", My.Resources.Label_ChecksumError, False)
        AddFlagColumn("Deleted", My.Resources.Label_Deleted, True)
        AddFlagColumn("Unavailable", My.Resources.Label_Unavailable, True)
        AddFlagColumn("Compressed", My.Resources.Label_Compressed, True)
    End Sub

    Private Sub AddFlagColumn(Name As String, HeaderText As String, IsReadOnly As Boolean)
        Dim Column As New DataGridViewCheckBoxColumn With {
            .Name = Name,
            .HeaderText = HeaderText,
            .DataPropertyName = Name,
            .ReadOnly = IsReadOnly,
            .Width = 90,
            .MinimumWidth = 60,
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
            .SortMode = DataGridViewColumnSortMode.NotSortable
        }
        Column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        DataGridViewSectors.Columns.Add(Column)
    End Sub

    Private Sub Populate()
        Dim Image = DirectCast(_Disk.Image, IMDFloppyImage).Image

        TxtHeader.Text = If(Image.Header, "")
        TxtComment.Text = If(Image.Comment, "")

        Dim Table = New DataTable
        AddDataColumn(Table, "Cylinder", GetType(String))
        AddDataColumn(Table, "Head", GetType(String))
        AddDataColumn(Table, "SectorCount", GetType(String))
        AddDataColumn(Table, "Mode", GetType(String))
        AddDataColumn(Table, "SectorSize", GetType(String))

        For Each Track In Image.Tracks
            Dim Row = Table.NewRow()
            Row("Cylinder") = Track.Track.ToString()
            Row("Head") = Track.Side.ToString()
            Row("SectorCount") = Track.Sectors.Count.ToString()
            Row("Mode") = ModeCaption(Track.Mode)
            Row("SectorSize") = SectorSizeCaption(Track.GetSizeBytes(), Track.SectorSize)
            Table.Rows.Add(Row)
        Next

        DataGridViewTracks.DataSource = Table
        If DataGridViewTracks.Rows.Count > 0 Then
            DataGridViewTracks.Rows(0).Selected = True
            PopulateSectors(0)
        Else
            PopulateSectors(-1)
        End If
    End Sub

    Private Sub PopulateSectors(TrackIndex As Integer)
        _LoadingSectors = True

        Dim Table = New DataTable
        AddDataColumn(Table, "Cylinder", GetType(String))
        AddDataColumn(Table, "Head", GetType(String))
        AddDataColumn(Table, "Sector", GetType(String))
        AddDataColumn(Table, "ChecksumError", GetType(Boolean))
        AddDataColumn(Table, "Deleted", GetType(Boolean))
        AddDataColumn(Table, "Unavailable", GetType(Boolean))
        AddDataColumn(Table, "Compressed", GetType(Boolean))

        Dim Image = DirectCast(_Disk.Image, IMDFloppyImage).Image
        If TrackIndex >= 0 AndAlso TrackIndex < Image.Tracks.Count Then
            Dim SectorIndex = 0
            For Each Sector In Image.Tracks(TrackIndex).Sectors
                Dim Row = Table.NewRow()
                Row("Cylinder") = Sector.Track.ToString()
                Row("Head") = Sector.Side.ToString()
                Row("Sector") = Sector.SectorId.ToString()
                Row("ChecksumError") = ChecksumErrorValue(TrackIndex, SectorIndex, Sector.ChecksumError)
                Row("Deleted") = Sector.Deleted
                Row("Unavailable") = Sector.Unavailable
                Row("Compressed") = Sector.Compressed
                Table.Rows.Add(Row)
                SectorIndex += 1
            Next
        End If

        DataGridViewSectors.DataSource = Table
        _LoadingSectors = False
    End Sub

    Private Function ChecksumErrorValue(TrackIndex As Integer, SectorIndex As Integer, Stored As Boolean) As Boolean
        Dim Edited As Boolean
        If _ChecksumErrorEdits.TryGetValue(EditKey(TrackIndex, SectorIndex), Edited) Then
            Return Edited
        End If

        Return Stored
    End Function

    Private Shared Function EditKey(TrackIndex As Integer, SectorIndex As Integer) As Long
        Return (CLng(TrackIndex) << 16) Or CUInt(SectorIndex)
    End Function

    Private Shared Function ModeCaption(Mode As TrackMode) As String
        Select Case Mode
            Case TrackMode.FM500kbps
                Return My.Resources.IMD_Mode_FM500
            Case TrackMode.FM300kbps
                Return My.Resources.IMD_Mode_FM300
            Case TrackMode.FM250kbps
                Return My.Resources.IMD_Mode_FM250
            Case TrackMode.MFM500kbps
                Return My.Resources.IMD_Mode_MFM500
            Case TrackMode.MFM300kbps
                Return My.Resources.IMD_Mode_MFM300
            Case TrackMode.MFM250kbps
                Return My.Resources.IMD_Mode_MFM250
            Case Else
                Return CByte(Mode).ToString("X2")
        End Select
    End Function

    Private Shared Function SectorSizeCaption(Bytes As UShort, Size As SectorSize) As String
        If Bytes = 0 Then
            Return CByte(Size).ToString("X2")
        End If

        Return Bytes.ToString("N0")
    End Function

    Private Function CollectChecksumChanges() As List(Of IMDFloppyImage.IMDChecksumErrorEdit)
        Dim Changes As New List(Of IMDFloppyImage.IMDChecksumErrorEdit)
        For Each Item In _ChecksumErrorEdits
            Dim TrackIndex = CInt(Item.Key >> 16)
            Dim SectorIndex = CInt(Item.Key And &HFFFF)
            Changes.Add(New IMDFloppyImage.IMDChecksumErrorEdit(TrackIndex, SectorIndex, Item.Value))
        Next

        Return Changes
    End Function

    Private Sub DataGridViewSectors_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DataGridViewSectors.CurrentCellDirtyStateChanged
        If DataGridViewSectors.IsCurrentCellDirty Then
            DataGridViewSectors.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub DataGridViewSectors_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewSectors.CellValueChanged
        If _LoadingSectors OrElse e.RowIndex < 0 OrElse DataGridViewTracks.CurrentRow Is Nothing Then
            Exit Sub
        End If

        If DataGridViewSectors.Columns(e.ColumnIndex).Name <> "ChecksumError" Then
            Exit Sub
        End If

        _ChecksumErrorEdits(EditKey(DataGridViewTracks.CurrentRow.Index, e.RowIndex)) = CBool(DataGridViewSectors.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)
    End Sub

    Private Sub DataGridViewTracks_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridViewTracks.SelectionChanged
        If DataGridViewTracks.CurrentRow Is Nothing Then
            PopulateSectors(-1)
        Else
            PopulateSectors(DataGridViewTracks.CurrentRow.Index)
        End If
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        _Updated = DirectCast(_Disk.Image, IMDFloppyImage).UpdateProperties(TxtComment.Text, CollectChecksumChanges())
        DialogResult = DialogResult.OK
    End Sub

    Private Sub DataGridViewTracks_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridViewTracks.DataBindingComplete
        DataGridViewTracks.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
        ResizeGridWidth(DataGridViewTracks)
    End Sub

    Private Sub DataGridViewSectors_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridViewSectors.DataBindingComplete
        DataGridViewSectors.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
    End Sub
End Class
