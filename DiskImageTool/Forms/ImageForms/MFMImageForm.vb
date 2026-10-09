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
    Private Const INTERFACE_MODE_DISABLED As Byte = &HFE

    Private Shared ReadOnly InterfaceModes() As Byte = {
        &H0, &H1, &H8, &H2, &H3, &H4, &H5, &H6, &H7, &H9, &HA, &HB, &HC, &HD, &HE, &HF, &H10, INTERFACE_MODE_DISABLED
    }

    Private ReadOnly _FloppyImage As MFMFloppyImage
    Private _Updated As Boolean

    Public Sub New(FloppyImage As MFMFloppyImage)

        ' This call is required by the designer.
        InitializeComponent()

        GetType(DataGridView).InvokeMember(
            "DoubleBuffered",
            Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.SetProperty,
            Nothing,
            DataGridViewTracks,
            New Object() {True})

        ' Add any initialization after the InitializeComponent() call.
        _FloppyImage = FloppyImage
        Dim Image = FloppyImage.Image
        LocalizeForm()
        InitializeGridColumns(Image)
        PopulateHeader(Image)
        DataGridViewTracks.DataSource = GetTrackTable(Image)
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
        PopulateInterfaceTypes(Image)
    End Sub

    Private Sub PopulateInterfaceTypes(Image As MFMImage)
        CboInterfaceType.BeginUpdate()
        CboInterfaceType.Items.Clear()

        Dim ModeToSelect = InterfaceModeToSelect(Image.IFType)
        Dim Selected As InterfaceTypeItem = Nothing
        For Each InterfaceMode In InterfaceModes
            Dim Item As New InterfaceTypeItem(InterfaceMode, InterfaceModeCaption(InterfaceMode))
            CboInterfaceType.Items.Add(Item)
            If InterfaceMode = ModeToSelect Then
                Selected = Item
            End If
        Next

        If Selected Is Nothing Then
            Selected = New InterfaceTypeItem(ModeToSelect, ModeToSelect.ToString("X2"))
            CboInterfaceType.Items.Insert(0, Selected)
        End If

        CboInterfaceType.SelectedItem = Selected
        CboInterfaceType.EndUpdate()
    End Sub

    Private Shared Function InterfaceModeToSelect(InterfaceType As Byte) As Byte
        If InterfaceType = INTERFACE_MODE_DISABLED Then
            Return INTERFACE_MODE_DISABLED
        End If

        Return InterfaceType And &H7F
    End Function

    Private Shared Function InterfaceModeCaption(Mode As Byte) As String
        Select Case Mode
            Case &H0
                Return My.Resources.FloppyInterface_IbmPcDd
            Case &H1
                Return My.Resources.FloppyInterface_IbmPcHd
            Case &H2
                Return My.Resources.FloppyInterface_AtariStDd
            Case &H3
                Return My.Resources.FloppyInterface_AtariStHd
            Case &H4
                Return My.Resources.FloppyInterface_AmigaDd
            Case &H5
                Return My.Resources.FloppyInterface_AmigaHd
            Case &H6
                Return My.Resources.FloppyInterface_CpcDd
            Case &H7
                Return My.Resources.FloppyInterface_ShugartDd
            Case &H8
                Return My.Resources.FloppyInterface_IbmPcEd
            Case &H9
                Return My.Resources.FloppyInterface_Msx2Dd
            Case &HA
                Return My.Resources.FloppyInterface_C64Dd
            Case &HB
                Return My.Resources.FloppyInterface_EmuShugart
            Case &HC
                Return My.Resources.FloppyInterface_S950Dd
            Case &HD
                Return My.Resources.FloppyInterface_S950Hd
            Case &HE
                Return My.Resources.FloppyInterface_S950Auto
            Case &HF
                Return My.Resources.FloppyInterface_IbmPcAuto
            Case &H10
                Return My.Resources.FloppyInterface_QuickDisk
            Case INTERFACE_MODE_DISABLED
                Return My.Resources.FloppyInterface_Disabled
            Case Else
                Return Mode.ToString("X2")
        End Select
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

        Dim Item = TryCast(CboInterfaceType.SelectedItem, InterfaceTypeItem)
        If Item Is Nothing Then
            Return False
        End If

        _Updated = _FloppyImage.UpdateHeader(RPM, BitRate, Item.Mode)
        Return True
    End Function

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        If Not ApplyUpdates() Then
            Exit Sub
        End If

        DialogResult = DialogResult.OK
    End Sub

    Private Sub NumericTextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtRPM.KeyPress, TxtBitRate.KeyPress
        If Not Char.IsDigit(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub NumericTextBox_TextChanged(sender As Object, e As EventArgs) Handles TxtRPM.TextChanged, TxtBitRate.TextChanged
        Dim Box = DirectCast(sender, TextBox)
        Dim Digits = New String(Box.Text.Where(Function(Character) Char.IsDigit(Character)).ToArray())
        If Digits = Box.Text Then
            Exit Sub
        End If

        Dim SelectionStart = Math.Min(Box.SelectionStart, Digits.Length)
        Box.Text = Digits
        Box.SelectionStart = SelectionStart
    End Sub

    Private Sub NumericTextBox_LostFocus(sender As Object, e As EventArgs) Handles TxtRPM.LostFocus, TxtBitRate.LostFocus
        Dim Box = DirectCast(sender, TextBox)
        Dim Value As ULong
        If Not ULong.TryParse(Box.Text, Value) Then
            Exit Sub
        End If

        If Value > UShort.MaxValue Then
            Box.Text = UShort.MaxValue.ToString()
        End If
    End Sub

    Private Sub InitializeGridColumns(Image As MFMImage)
        DataGridViewTracks.AutoGenerateColumns = False
        DataGridViewTracks.Columns.Clear()

        AddTextColumn(GRID_COLUMN_TRACK, My.Resources.Label_Track, 55, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(GRID_COLUMN_SIDE, My.Resources.Label_Side, 50, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(GRID_COLUMN_OFFSET, My.Resources.Label_OffsetHex, 80, DataGridViewContentAlignment.MiddleRight, "X8")
        AddTextColumn(GRID_COLUMN_LENGTH, My.Resources.Label_Length, 80, DataGridViewContentAlignment.MiddleRight, "N0")
        If (Image.IFType And ADVANCED_TRACK_LIST) <> 0 Then
            AddTextColumn(GRID_COLUMN_BITRATE, My.Resources.SummaryPanel_Bitrate, 65, DataGridViewContentAlignment.MiddleRight, "N0")
            AddTextColumn(GRID_COLUMN_RPM, My.Resources.SummaryPanel_RPM, 55, DataGridViewContentAlignment.MiddleRight, "N0")
        End If
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

    Private Function GetTrackTable(Image As MFMImage) As DataTable
        Dim TrackTable As New DataTable("MFMTracks")
        Dim Advanced = (Image.IFType And ADVANCED_TRACK_LIST) <> 0

        AddDataColumn(TrackTable, GRID_COLUMN_TRACK, GetType(UShort))
        AddDataColumn(TrackTable, GRID_COLUMN_SIDE, GetType(Byte))
        AddDataColumn(TrackTable, GRID_COLUMN_OFFSET, GetType(UInteger))
        AddDataColumn(TrackTable, GRID_COLUMN_LENGTH, GetType(UInteger))
        If Advanced Then
            AddDataColumn(TrackTable, GRID_COLUMN_BITRATE, GetType(UShort))
            AddDataColumn(TrackTable, GRID_COLUMN_RPM, GetType(UShort))
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

    Private Sub AddDataColumn(Table As DataTable, Name As String, DataType As Type)
        Table.Columns.Add(New DataColumn(Name, DataType))
    End Sub

    Private Sub DataGridViewTracks_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridViewTracks.DataBindingComplete
        'DataGridViewTracks.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
    End Sub

    Private Class InterfaceTypeItem
        Public Sub New(Mode As Byte, Caption As String)
            Me.Mode = Mode
            _Caption = Caption
        End Sub

        Public ReadOnly Property Mode As Byte

        Public Overrides Function ToString() As String
            Return _Caption
        End Function

        Private ReadOnly _Caption As String
    End Class
End Class
