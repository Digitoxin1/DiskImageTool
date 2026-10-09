Imports DiskImageTool.DiskImage
Imports DiskImageTool.ImageFormats.IMD

Public Class IMDImageForm
    Private ReadOnly _ChecksumErrorEdits As New Dictionary(Of Long, Boolean)
    Private ReadOnly _Disk As Disk

    Private _LoadingSectors As Boolean
    Private _Updated As Boolean

    Public Sub New(Disk As Disk)
        InitializeComponent()

        _Disk = Disk

        ImageForm.LocalizeButtons(BtnUpdate, BtnCancel)
        Me.Text = String.Format(My.Resources.FloppyImageType_ImageProperties, My.Resources.FloppyImageType_IMD)
        LblHeader.Text = My.Resources.Label_Header
        LblComment.Text = My.Resources.Label_Comment
        LblTracks.Text = My.Resources.Label_Tracks
        LblSectors.Text = My.Resources.Label_Sectors

        TxtComment.AcceptsReturn = True
        TxtComment.ScrollBars = ScrollBars.Vertical

        ImageForm.EnableDoubleBuffer(DataGridViewTracks)
        ImageForm.EnableDoubleBuffer(DataGridViewSectors)
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

    Private Shared Function ModeCaption(Mode As TrackMode) As String
        Select Case Mode
            Case TrackMode.FM500kbps
                Return String.Format(My.Resources.Label_kbps, "FM 500")
            Case TrackMode.FM300kbps
                Return String.Format(My.Resources.Label_kbps, "FM 300")
            Case TrackMode.FM250kbps
                Return String.Format(My.Resources.Label_kbps, "FM 250")
            Case TrackMode.MFM500kbps
                Return String.Format(My.Resources.Label_kbps, "MFM 500")
            Case TrackMode.MFM300kbps
                Return String.Format(My.Resources.Label_kbps, "MFM 300")
            Case TrackMode.MFM250kbps
                Return String.Format(My.Resources.Label_kbps, "MFM 250")
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

    Private Sub AddSectorColumns()
        ImageForm.PrepareGrid(DataGridViewSectors)
        ImageForm.AddTextColumn(DataGridViewSectors, "Cylinder", My.Resources.Label_Cylinder, 70, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        ImageForm.AddTextColumn(DataGridViewSectors, "Head", My.Resources.Label_Head, 55, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        ImageForm.AddTextColumn(DataGridViewSectors, "Sector", My.Resources.Label_Sector, 60, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        ImageForm.AddCheckColumn(DataGridViewSectors, "ChecksumError", My.Resources.Label_ChecksumError, 90, True)
        ImageForm.AddCheckColumn(DataGridViewSectors, "Deleted", My.Resources.Label_Deleted, 90)
        ImageForm.AddCheckColumn(DataGridViewSectors, "Unavailable", My.Resources.Label_Unavailable, 90)
        ImageForm.AddCheckColumn(DataGridViewSectors, "Compressed", My.Resources.Label_Compressed, 90)
    End Sub

    Private Sub AddTrackColumns()
        DataGridViewTracks.DefaultCellStyle.Padding = New Padding(0, 0, 5, 0)
        ImageForm.PrepareGrid(DataGridViewTracks)

        ImageForm.AddTextColumn(DataGridViewTracks, "Cylinder", My.Resources.Label_Cylinder, 70, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, "Head", My.Resources.Label_Head, 50, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, "SectorCount", My.Resources.Label_Sectors, 70, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, "Mode", My.Resources.Label_Mode, 110, DataGridViewContentAlignment.MiddleLeft)
        ImageForm.AddTextColumn(DataGridViewTracks, "SectorSize", My.Resources.Label_Size, 70, DataGridViewContentAlignment.MiddleRight)
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        ImageForm.CommitDirtyCheckBox(DataGridViewSectors)
        _Updated = DirectCast(_Disk.Image, IMDFloppyImage).UpdateProperties(TxtComment.Text, CollectChecksumChanges())
        DialogResult = DialogResult.OK
    End Sub

    Private Function CollectChecksumChanges() As List(Of IMDFloppyImage.IMDChecksumErrorEdit)
        Dim Changes As New List(Of IMDFloppyImage.IMDChecksumErrorEdit)
        For Each Item In _ChecksumErrorEdits
            Dim TrackIndex As Integer
            Dim SectorIndex As Integer
            ImageForm.UnpackSectorEditKey(Item.Key, TrackIndex, SectorIndex)
            Changes.Add(New IMDFloppyImage.IMDChecksumErrorEdit(TrackIndex, SectorIndex, Item.Value))
        Next

        Return Changes
    End Function

    Private Sub DataGridViewSectors_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewSectors.CellValueChanged
        If _LoadingSectors OrElse e.RowIndex < 0 OrElse DataGridViewTracks.CurrentRow Is Nothing Then
            Exit Sub
        End If

        If DataGridViewSectors.Columns(e.ColumnIndex).Name <> "ChecksumError" Then
            Exit Sub
        End If

        _ChecksumErrorEdits(ImageForm.SectorEditKey(DataGridViewTracks.CurrentRow.Index, e.RowIndex)) = CBool(DataGridViewSectors.Rows(e.RowIndex).Cells(e.ColumnIndex).Value)
    End Sub

    Private Sub DataGridViewSectors_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DataGridViewSectors.CurrentCellDirtyStateChanged
        ImageForm.CommitDirtyCheckBox(DataGridViewSectors, "ChecksumError")
    End Sub

    Private Sub DataGridViewSectors_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridViewSectors.DataBindingComplete
        ImageForm.AutoSizeGrid(DataGridViewSectors)
    End Sub

    Private Sub DataGridViewTracks_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridViewTracks.DataBindingComplete
        ImageForm.AutoSizeGrid(DataGridViewTracks, FitWidth:=True)
    End Sub

    Private Sub DataGridViewTracks_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridViewTracks.SelectionChanged
        If DataGridViewTracks.CurrentRow Is Nothing Then
            PopulateSectors(-1)
        Else
            PopulateSectors(DataGridViewTracks.CurrentRow.Index)
        End If
    End Sub

    Private Sub Populate()
        Dim Image = DirectCast(_Disk.Image, IMDFloppyImage).Image

        TxtHeader.Text = If(Image.Header, "")
        TxtComment.Text = If(Image.Comment, "")

        Dim Table = New DataTable
        ImageForm.AddDataColumn(Table, "Cylinder", GetType(String))
        ImageForm.AddDataColumn(Table, "Head", GetType(String))
        ImageForm.AddDataColumn(Table, "SectorCount", GetType(String))
        ImageForm.AddDataColumn(Table, "Mode", GetType(String))
        ImageForm.AddDataColumn(Table, "SectorSize", GetType(String))

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
        ImageForm.AddDataColumn(Table, "Cylinder", GetType(String))
        ImageForm.AddDataColumn(Table, "Head", GetType(String))
        ImageForm.AddDataColumn(Table, "Sector", GetType(String))
        ImageForm.AddDataColumn(Table, "ChecksumError", GetType(Boolean))
        ImageForm.AddDataColumn(Table, "Deleted", GetType(Boolean))
        ImageForm.AddDataColumn(Table, "Unavailable", GetType(Boolean))
        ImageForm.AddDataColumn(Table, "Compressed", GetType(Boolean))

        Dim Image = DirectCast(_Disk.Image, IMDFloppyImage).Image
        If TrackIndex >= 0 AndAlso TrackIndex < Image.Tracks.Count Then
            Dim SectorIndex = 0
            For Each Sector In Image.Tracks(TrackIndex).Sectors
                Dim Row = Table.NewRow()
                Row("Cylinder") = Sector.Track.ToString()
                Row("Head") = Sector.Side.ToString()
                Row("Sector") = Sector.SectorId.ToString()
                Row("ChecksumError") = ImageForm.EditedFlag(_ChecksumErrorEdits, TrackIndex, SectorIndex, Sector.ChecksumError)
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
End Class
