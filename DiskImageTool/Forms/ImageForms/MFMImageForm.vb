Imports DiskImageTool.DiskImage
Imports DiskImageTool.ImageFormats.MFM

Public Class MFMImageForm
    Private Const GRID_COLUMN_TRACK As String = "GridTrack"
    Private Const GRID_COLUMN_SIDE As String = "GridSide"
    Private Const GRID_COLUMN_OFFSET As String = "GridOffset"
    Private Const GRID_COLUMN_LENGTH As String = "GridLength"
    Private Const GRID_COLUMN_BITRATE As String = "GridBitRate"
    Private Const GRID_COLUMN_RPM As String = "GridRPM"
    Private Const ADVANCED_TRACK_LIST As Byte = &H80

    Private ReadOnly _FloppyImage As MFMFloppyImage
    Private _Updated As Boolean

    Public Sub New(FloppyImage As MFMFloppyImage)

        ' This call is required by the designer.
        InitializeComponent()

        ImageForm.EnableDoubleBuffer(DataGridViewTracks)

        ' Add any initialization after the InitializeComponent() call.
        _FloppyImage = FloppyImage
        Dim Image = FloppyImage.Image
        LocalizeForm()
        InitializeGridColumns(Image)
        PopulateHeader(Image)
        DataGridViewTracks.DataSource = GetTrackTable(Image)
        ImageForm.AttachNumericTextBox(TxtRPM)
        ImageForm.AttachNumericTextBox(TxtBitRate)
    End Sub

    Public Shared Function Display(Disk As Disk) As Boolean
        If Disk Is Nothing OrElse Disk.Image Is Nothing OrElse Disk.Image.ImageType <> FloppyImageType.MFMImage Then
            Return False
        End If

        Dim FloppyImage = DirectCast(Disk.Image, MFMFloppyImage)
        Using dlg As New MFMImageForm(FloppyImage)
            dlg.ShowDialog(App.CurrentFormInstance)
            Return dlg._Updated
        End Using
    End Function

    Private Sub LocalizeForm()
        Me.Text = "MFM " & WithoutHotkey(My.Resources.Menu_ImageProperties)
        BtnCancel.Text = My.Resources.Menu_Cancel
        BtnUpdate.Text = My.Resources.Menu_Update
        LblTracks.Text = My.Resources.Label_Tracks
        LblSides.Text = My.Resources.Label_Sides
        LblRPM.Text = My.Resources.SummaryPanel_RPM
        LblBitRate.Text = My.Resources.SummaryPanel_Bitrate
        LblInterfaceType.Text = My.Resources.Label_InterfaceType
        ChkPerTrackRates.Text = My.Resources.Label_PerTrackRates
    End Sub

    Private Sub PopulateHeader(Image As MFMImage)
        TxtTrackCount.Text = Image.TrackCount.ToString()
        TxtSides.Text = Image.SideCount.ToString()
        TxtRPM.Text = Image.RPM.ToString()
        TxtBitRate.Text = Image.BitRate.ToString()
        ChkPerTrackRates.Checked = (Image.IFType And ADVANCED_TRACK_LIST) <> 0
        ImageForm.PopulateByteCombo(CboInterfaceType, ImageForm.InterfaceModes, InterfaceModeToSelect(Image.IFType), AddressOf ImageForm.InterfaceModeCaption)
    End Sub

    Private Shared Function InterfaceModeToSelect(InterfaceType As Byte) As Byte
        If InterfaceType = ImageForm.InterfaceModeDisabled Then
            Return ImageForm.InterfaceModeDisabled
        End If

        Return InterfaceType And &H7F
    End Function

    Private Function ApplyUpdates() As Boolean
        Dim RPM As UShort
        Dim BitRate As UShort
        If Not UShort.TryParse(TxtRPM.Text, RPM) Then
            TxtRPM.Focus()
            Return False
        End If

        If Not UShort.TryParse(TxtBitRate.Text, BitRate) Then
            TxtBitRate.Focus()
            Return False
        End If

        Dim Item = TryCast(CboInterfaceType.SelectedItem, ImageForm.ByteListItem)
        If Item Is Nothing Then
            Return False
        End If

        _Updated = _FloppyImage.UpdateHeader(RPM, BitRate, Item.Value)
        Return True
    End Function

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        If Not ApplyUpdates() Then
            Exit Sub
        End If

        DialogResult = DialogResult.OK
    End Sub

    Private Sub InitializeGridColumns(Image As MFMImage)
        DataGridViewTracks.AutoGenerateColumns = False
        DataGridViewTracks.Columns.Clear()

        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_TRACK, My.Resources.Label_Track, 55, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_SIDE, My.Resources.Label_Side, 50, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_OFFSET, My.Resources.Label_OffsetHex, 80, DataGridViewContentAlignment.MiddleRight, "X8")
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_LENGTH, My.Resources.Label_Length, 80, DataGridViewContentAlignment.MiddleRight, "N0")
        If (Image.IFType And ADVANCED_TRACK_LIST) <> 0 Then
            ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_BITRATE, My.Resources.SummaryPanel_Bitrate, 65, DataGridViewContentAlignment.MiddleRight, "N0")
            ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_RPM, My.Resources.SummaryPanel_RPM, 55, DataGridViewContentAlignment.MiddleRight, "N0")
        End If
    End Sub

    Private Function GetTrackTable(Image As MFMImage) As DataTable
        Dim TrackTable As New DataTable("MFMTracks")
        Dim Advanced = (Image.IFType And ADVANCED_TRACK_LIST) <> 0

        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_TRACK, GetType(UShort))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_SIDE, GetType(Byte))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_OFFSET, GetType(UInteger))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_LENGTH, GetType(UInteger))
        If Advanced Then
            ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_BITRATE, GetType(UShort))
            ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_RPM, GetType(UShort))
        End If

        If Image.SideCount > 0 Then
            For Track As Integer = 0 To Image.TrackCount - 1
                For Side As Integer = 0 To Image.SideCount - 1
                    Dim Row = TrackTable.NewRow()
                    Dim TrackData = Image.GetTrack(CUShort(Track), CByte(Side))
                    If TrackData Is Nothing Then
                        Row(GRID_COLUMN_TRACK) = CUShort(Track)
                        Row(GRID_COLUMN_SIDE) = CByte(Side)
                    Else
                        Row(GRID_COLUMN_TRACK) = TrackData.Track
                        Row(GRID_COLUMN_SIDE) = TrackData.Side
                        Row(GRID_COLUMN_OFFSET) = TrackData.Offset
                        Row(GRID_COLUMN_LENGTH) = CUInt(TrackData.Length)
                        If Advanced Then
                            Row(GRID_COLUMN_BITRATE) = TrackData.BitRate
                            Row(GRID_COLUMN_RPM) = TrackData.RPM
                        End If
                    End If
                    TrackTable.Rows.Add(Row)
                Next
            Next
        End If

        Return TrackTable
    End Function
End Class
