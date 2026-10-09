Imports DiskImageTool.DiskImage
Imports DiskImageTool.ImageFormats.PRI

Public Class PRIImageForm
    Private Const GRID_COLUMN_BIT_CLOCK As String = "GridBitClock"
    Private Const GRID_COLUMN_LENGTH As String = "GridLength"
    Private Const GRID_COLUMN_SIDE As String = "GridSide"
    Private Const GRID_COLUMN_TRACK As String = "GridTrack"
    Private Const GRID_COLUMN_WEAK_BITS As String = "GridWeakBits"

    Private ReadOnly _FloppyImage As PRIFloppyImage
    Private _Updated As Boolean

    Public Sub New(FloppyImage As PRIFloppyImage)

        ' This call is required by the designer.
        InitializeComponent()

        ImageForm.EnableDoubleBuffer(DataGridViewTracks)

        ' Add any initialization after the InitializeComponent() call.
        _FloppyImage = FloppyImage
        Dim Image = FloppyImage.Image
        LocalizeForm()
        InitializeGridColumns()
        PopulateHeader(Image)
        DataGridViewTracks.DataSource = GetTrackTable(Image)
    End Sub

    Public Shared Function Display(Disk As Disk) As Boolean
        If Disk Is Nothing OrElse Disk.Image Is Nothing OrElse Disk.Image.ImageType <> FloppyImageType.PRIImage Then
            Return False
        End If

        Dim FloppyImage = DirectCast(Disk.Image, PRIFloppyImage)
        Using dlg As New PRIImageForm(FloppyImage)
            dlg.ShowDialog(App.CurrentFormInstance)
            Return dlg._Updated
        End Using
    End Function

    Private Sub LocalizeForm()
        ImageForm.LocalizeButtons(BtnUpdate, BtnCancel)
        Me.Text = String.Format(My.Resources.FloppyImageType_BitstreamImageProperties, My.Resources.FloppyImageType_PCE)
        LblVersion.Text = My.Resources.Label_Version
        LblReserved.Text = My.Resources.Label_Reserved
        LblComment.Text = My.Resources.Label_Comment
    End Sub

    Private Sub PopulateHeader(Image As PRIImage)
        TxtVersion.Text = Image.Header.Version.ToString()
        TxtReserved.Text = Image.Header.Reserved.ToString("X4")
        TxtComment.Text = If(Image.Comment, "")
    End Sub

    Private Sub InitializeGridColumns()
        ImageForm.PrepareGrid(DataGridViewTracks)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_TRACK, My.Resources.Label_Track, 55, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_SIDE, My.Resources.Label_Side, 50, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_LENGTH, My.Resources.Label_Length, 90, DataGridViewContentAlignment.MiddleRight, "N0")
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_BIT_CLOCK, My.Resources.Label_BitClock, 100, DataGridViewContentAlignment.MiddleRight, "N0")
        ImageForm.AddCheckColumn(DataGridViewTracks, GRID_COLUMN_WEAK_BITS, My.Resources.Label_WeakBits)
    End Sub

    Private Function GetTrackTable(Image As PRIImage) As DataTable
        Dim TrackTable As New DataTable("PRITracks")
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_TRACK, GetType(UInteger))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_SIDE, GetType(UInteger))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_LENGTH, GetType(UInteger))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_BIT_CLOCK, GetType(UInteger))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_WEAK_BITS, GetType(Boolean))

        Dim Tracks As New List(Of PRITrack)(Image.Tracks.Values)
        Tracks.Sort(Function(Left, Right)
                        Dim TrackCompare = Left.Track.CompareTo(Right.Track)
                        If TrackCompare <> 0 Then
                            Return TrackCompare
                        End If

                        Return Left.Side.CompareTo(Right.Side)
                    End Function)

        For Each Track In Tracks
            Dim Row = TrackTable.NewRow()
            Row(GRID_COLUMN_TRACK) = Track.Track
            Row(GRID_COLUMN_SIDE) = Track.Side
            Row(GRID_COLUMN_LENGTH) = Track.Length
            Row(GRID_COLUMN_BIT_CLOCK) = Track.BitClockRate
            Row(GRID_COLUMN_WEAK_BITS) = Track.SurfaceData IsNot Nothing
            TrackTable.Rows.Add(Row)
        Next

        Return TrackTable
    End Function

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        _Updated = _FloppyImage.UpdateProperties(TxtComment.Text)
        DialogResult = DialogResult.OK
    End Sub

End Class
