Imports DiskImageTool.DiskImage
Imports DiskImageTool.ImageFormats.D86F

Public Class D86FImageForm
    Private Const FILE_SIGNATURE As String = "86BF"
    Private Const GRID_COLUMN_BIT_CELL_COUNT As String = "GridBitCellCount"
    Private Const GRID_COLUMN_BITRATE As String = "GridBitRate"
    Private Const GRID_COLUMN_ENCODING As String = "GridEncoding"
    Private Const GRID_COLUMN_INDEX_HOLE As String = "GridIndexHole"
    Private Const GRID_COLUMN_OFFSET As String = "GridOffset"
    Private Const GRID_COLUMN_RPM As String = "GridRPM"
    Private Const GRID_COLUMN_SIDE As String = "GridSide"
    Private Const GRID_COLUMN_TRACK As String = "GridTrack"

    Private Shared ReadOnly HoleValues() As Byte = {0, 1, 2, 3}
    Private Shared ReadOnly WriteProtectValues() As Byte = {0, 1}

    Private ReadOnly _FloppyImage As D86FFloppyImage
    Private _Updated As Boolean

    Public Sub New(FloppyImage As D86FFloppyImage)

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
    End Sub

    Public Shared Function Display(Disk As Disk) As Boolean
        If Disk Is Nothing OrElse Disk.Image Is Nothing OrElse Disk.Image.ImageType <> FloppyImageType.D86FImage Then
            Return False
        End If

        Dim FloppyImage = DirectCast(Disk.Image, D86FFloppyImage)
        Using dlg As New D86FImageForm(FloppyImage)
            dlg.ShowDialog(App.CurrentFormInstance)
            Return dlg._Updated
        End Using
    End Function

    Private Shared Function BitRateCaption(Rate As BitRate) As String
        Select Case Rate
            Case BitRate.BitRate250
                Return "250"
            Case BitRate.BitRate300
                Return "300"
            Case BitRate.BitRate500
                Return "500"
            Case BitRate.BitRate1000
                Return "1000"
            Case BitRate.BitRate2000
                Return "2000"
            Case Else
                Return CByte(Rate).ToString("X2")
        End Select
    End Function

    Private Shared Function DiskTypeCaption(DiskType As DiskType) As String
        Select Case DiskType
            Case DiskType.FixedRPM
                Return My.Resources.D86F_DiskType_FixedRPM
            Case DiskType.Zoned
                Return My.Resources.D86F_DiskType_Zoned
            Case Else
                Return CByte(DiskType).ToString("X2")
        End Select
    End Function

    Private Shared Function EncodingCaption(TrackEncoding As Encoding) As String
        Select Case TrackEncoding
            Case Encoding.FM
                Return My.Resources.D86F_Encoding_FM
            Case Encoding.MFM
                Return My.Resources.D86F_Encoding_MFM
            Case Encoding.M2MF
                Return My.Resources.D86F_Encoding_M2FM
            Case Encoding.GCR
                Return My.Resources.D86F_Encoding_GCR
            Case Else
                Return CByte(TrackEncoding).ToString("X2")
        End Select
    End Function

    Private Shared Function HoleCaption(Code As Byte) As String
        Select Case CType(Code, DiskHole)
            Case DiskHole.DD
                Return My.Resources.D86F_Hole_DD
            Case DiskHole.HD
                Return My.Resources.D86F_Hole_HD
            Case DiskHole.ED
                Return My.Resources.D86F_Hole_ED
            Case DiskHole.ED2000
                Return My.Resources.D86F_Hole_ED2000
            Case Else
                Return Code.ToString("X2")
        End Select
    End Function

    Private Shared Function RPMCaption(Rate As RPM) As String
        Select Case Rate
            Case RPM.RPM300
                Return "300"
            Case RPM.RPM360
                Return "360"
            Case Else
                Return CByte(Rate).ToString("X2")
        End Select
    End Function

    Private Shared Function RPMSlowdownCaption(Code As Byte) As String
        Select Case Code
            Case 1
                Return "1%"
            Case 2
                Return "1.5%"
            Case 3
                Return "2%"
            Case Else
                Return My.Resources.Label_None
        End Select
    End Function

    Private Shared Function WriteProtectCaption(Value As Byte) As String
        Return ImageForm.YesNo(Value = 1)
    End Function

    Private Shared Function ZoneTypeCaption(ZoneType As ZoneType) As String
        Select Case ZoneType
            Case ZoneType.PreApple1
                Return My.Resources.D86F_Zone_PreApple1
            Case ZoneType.PreApple2
                Return My.Resources.D86F_Zone_PreApple2
            Case ZoneType.Apple
                Return My.Resources.D86F_Zone_Apple
            Case ZoneType.Commodore64
                Return My.Resources.D86F_Zone_Commodore64
            Case Else
                Return CByte(ZoneType).ToString("X2")
        End Select
    End Function

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        Dim WriteProtect = TryCast(CboWriteProtect.SelectedItem, ImageForm.ByteListItem)
        Dim Hole = TryCast(CboHole.SelectedItem, ImageForm.ByteListItem)
        If WriteProtect Is Nothing OrElse Hole Is Nothing Then
            Exit Sub
        End If

        _Updated = _FloppyImage.UpdateHeader(WriteProtect.Value = 1, Hole.Value)
        DialogResult = DialogResult.OK
    End Sub

    Private Function GetTrackTable(Image As D86FImage) As DataTable
        Dim TrackTable As New DataTable("D86FTracks")
        Dim ShowBitCellCount = Image.BitcellMode

        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_TRACK, GetType(UShort))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_SIDE, GetType(Byte))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_OFFSET, GetType(UInteger))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_ENCODING, GetType(String))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_BITRATE, GetType(String))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_RPM, GetType(String))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_INDEX_HOLE, GetType(UInteger))
        If ShowBitCellCount Then
            ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_BIT_CELL_COUNT, GetType(UInteger))
        End If

        If Image.SideCount > 0 Then
            For Track As Integer = 0 To Image.TrackCount - 1
                For Side As Integer = 0 To Image.SideCount - 1
                    Dim Row = TrackTable.NewRow()
                    Row(GRID_COLUMN_TRACK) = CUShort(Track)
                    Row(GRID_COLUMN_SIDE) = CByte(Side)

                    Dim TrackData = Image.GetTrack(CUShort(Track), CByte(Side))
                    If TrackData IsNot Nothing Then
                        Row(GRID_COLUMN_OFFSET) = TrackData.Offset
                        If TrackData.Offset > 0 Then
                            Row(GRID_COLUMN_BITRATE) = BitRateCaption(TrackData.BitRate)
                            Row(GRID_COLUMN_ENCODING) = EncodingCaption(TrackData.Encoding)
                            Row(GRID_COLUMN_RPM) = RPMCaption(TrackData.RPM)
                            Row(GRID_COLUMN_INDEX_HOLE) = TrackData.IndexHolePos
                            If ShowBitCellCount Then
                                Row(GRID_COLUMN_BIT_CELL_COUNT) = TrackData.BitCellCount
                            End If
                        End If
                    End If

                    TrackTable.Rows.Add(Row)
                Next
            Next
        End If

        Return TrackTable
    End Function

    Private Sub InitializeGridColumns(Image As D86FImage)
        ImageForm.PrepareGrid(DataGridViewTracks)

        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_TRACK, My.Resources.Label_Track, 55, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_SIDE, My.Resources.Label_Side, 50, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_OFFSET, My.Resources.Label_OffsetHex, 90, DataGridViewContentAlignment.MiddleRight, "X8")
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_ENCODING, My.Resources.Label_Encoding, 80, DataGridViewContentAlignment.MiddleLeft)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_BITRATE, My.Resources.SummaryPanel_Bitrate, 70, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_RPM, My.Resources.SummaryPanel_RPM, 55, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_INDEX_HOLE, My.Resources.Label_IndexHole, 100, DataGridViewContentAlignment.MiddleRight, "X8")
        If Image.BitcellMode Then
            ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_BIT_CELL_COUNT, My.Resources.Label_BitCellCount, 100, DataGridViewContentAlignment.MiddleRight, "N0")
        End If
    End Sub

    Private Sub LocalizeForm()
        ImageForm.LocalizeButtons(Me, "86F", BtnUpdate, BtnCancel)
        LblSignature.Text = My.Resources.Label_Signature
        LblVersion.Text = My.Resources.Label_Version
        LblSides.Text = My.Resources.Label_Sides
        LblHole.Text = My.Resources.Label_Hole
        LblWriteProtect.Text = My.Resources.Label_WriteProtect
        LblRPMSlowdown.Text = My.Resources.Label_RPMSlowdown
        LblBitCellMode.Text = My.Resources.Label_BitCellMode
        LblAlternateBitCell.Text = My.Resources.Label_AlternateBitCell
        LblReverseEndian.Text = My.Resources.Label_ReverseEndian
        LblSurfaceData.Text = My.Resources.Label_SurfaceData
        LblDiskType.Text = My.Resources.SummaryPanel_DiskType
        LblZoneType.Text = My.Resources.Label_ZoneType
    End Sub

    Private Sub PopulateHeader(Image As D86FImage)
        TxtSignature.Text = FILE_SIGNATURE
        TxtVersion.Text = Image.MajorVersion.ToString() & "." & Image.MinorVersion.ToString()
        TxtSides.Text = Image.SideCount.ToString()
        ImageForm.PopulateByteCombo(CboHole, HoleValues, CByte(Image.Hole), AddressOf HoleCaption)
        ImageForm.PopulateByteCombo(CboWriteProtect, WriteProtectValues, If(Image.WriteProtect, CByte(1), CByte(0)), AddressOf WriteProtectCaption)
        TxtRPMSlowdown.Text = RPMSlowdownCaption(D86FFloppyImage.RPMSlowdownCode(Image))
        TxtBitCellMode.Text = ImageForm.YesNo(Image.BitcellMode)
        TxtAlternateBitCell.Text = ImageForm.YesNo(Image.AlternateBitcellCalculation)
        TxtReverseEndian.Text = ImageForm.YesNo(Image.ReverseEndian)
        TxtSurfaceData.Text = ImageForm.YesNo(Image.HasSurfaceData)
        TxtDiskType.Text = DiskTypeCaption(Image.DiskType)
        TxtZoneType.Text = ZoneTypeCaption(Image.ZoneType)
    End Sub
End Class
