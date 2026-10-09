Imports DiskImageTool.DiskImage
Imports DiskImageTool.ImageFormats.HFE

Public Class HFEImageForm
    Private Const GRID_COLUMN_TRACK As String = "GridTrack"
    Private Const GRID_COLUMN_OFFSET As String = "GridOffset"
    Private Const GRID_COLUMN_LENGTH As String = "GridLength"
    Private Const INTERFACE_MODE_DISABLED As Byte = &HFE
    Private Const WRITE_ALLOWED As Byte = &HFF
    Private Const WRITE_PROTECTED As Byte = &H0
    Private Const SINGLE_STEP As Byte = &HFF
    Private Const DOUBLE_STEP As Byte = &H0
    Private Const ALT_ENCODING_USE As Byte = &H0
    Private Const ALT_ENCODING_UNUSED As Byte = &HFF

    Private Shared ReadOnly InterfaceModes() As Byte = {
        &H0, &H1, &H8, &H2, &H3, &H4, &H5, &H6, &H7, &H9, &HA, &HB, &HC, &HD, &HE, &HF, &H10, INTERFACE_MODE_DISABLED
    }

    Private Shared ReadOnly WriteAllowedValues() As Byte = {WRITE_ALLOWED, WRITE_PROTECTED}

    Private ReadOnly _FloppyImage As HFEFloppyImage
    Private _Updated As Boolean

    Public Sub New(FloppyImage As HFEFloppyImage)

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
        InitializeGridColumns()
        PopulateHeader(Image)
        DataGridViewTracks.DataSource = GetTrackTable(Image)
    End Sub

    Public Shared Function Display(Disk As Disk) As Boolean
        If Disk Is Nothing OrElse Disk.Image Is Nothing OrElse Disk.Image.ImageType <> FloppyImageType.HFEImage Then
            Return False
        End If

        Dim FloppyImage = DirectCast(Disk.Image, HFEFloppyImage)
        Using dlg As New HFEImageForm(FloppyImage)
            dlg.ShowDialog(App.CurrentFormInstance)
            Return dlg._Updated
        End Using
    End Function

    Private Sub LocalizeForm()
        Me.Text = "HFE " & WithoutHotkey(My.Resources.Menu_ImageProperties)
        BtnCancel.Text = My.Resources.Menu_Cancel
        BtnUpdate.Text = My.Resources.Menu_Update
        LblSignature.Text = My.Resources.Label_Signature
        LblFormatRevision.Text = My.Resources.Label_FormatRevision
        LblTracks.Text = My.Resources.Label_Tracks
        LblSides.Text = My.Resources.Label_Sides
        LblTrackEncoding.Text = My.Resources.Label_TrackEncoding
        LblRPM.Text = My.Resources.SummaryPanel_RPM
        LblBitRate.Text = My.Resources.SummaryPanel_Bitrate
        LblInterfaceType.Text = My.Resources.Label_InterfaceType
        LblReserved.Text = My.Resources.Label_Reserved
        LblTrackListOffset.Text = My.Resources.Label_TrackListOffset
        LblWriteAllowed.Text = My.Resources.Label_WriteAllowed
        LblSingleStep.Text = My.Resources.Label_SingleStep
        LblTrack0Side0Alt.Text = My.Resources.Label_Track0Side0Alt
        LblTrack0Side0Encoding.Text = My.Resources.Label_Track0Side0Encoding
        LblTrack0Side1Alt.Text = My.Resources.Label_Track0Side1Alt
        LblTrack0Side1Encoding.Text = My.Resources.Label_Track0Side1Encoding
    End Sub

    Private Sub PopulateHeader(Image As HFEImage)
        TxtSignature.Text = Image.Signature
        TxtFormatRevision.Text = Image.FormatRevision.ToString()
        TxtTrackCount.Text = Image.TrackCount.ToString()
        TxtSides.Text = Image.SideCount.ToString()
        TxtTrackEncoding.Text = TrackEncodingCaption(CByte(Image.TrackEncoding))
        TxtRPM.Text = Image.RPM.ToString()
        TxtBitRate.Text = Image.BitRate.ToString()
        TxtReserved.Text = Image.DNU.ToString("X2")
        TxtTrackListOffset.Text = Image.TrackListOffset.ToString("X4")
        TxtSingleStep.Text = SingleStepCaption(Image.SingleStep)
        TxtTrack0Side0Alt.Text = AlternateEncodingCaption(Image.Track0S0_AltEncoding)
        TxtTrack0Side0Encoding.Text = TrackEncodingCaption(Image.Track0S0_Encoding)
        TxtTrack0Side1Alt.Text = AlternateEncodingCaption(Image.Track0S1_AltEncoding)
        TxtTrack0Side1Encoding.Text = TrackEncodingCaption(Image.Track0S1_Encoding)
        PopulateInterfaceTypes(CByte(Image.FloppyInterfaceMode))
        PopulateWriteAllowed(Image.WriteAllowed)
    End Sub

    Private Sub PopulateInterfaceTypes(ModeToSelect As Byte)
        CboInterfaceType.BeginUpdate()
        CboInterfaceType.Items.Clear()

        Dim Selected As ByteItem = Nothing
        For Each InterfaceMode In InterfaceModes
            Dim Item As New ByteItem(InterfaceMode, InterfaceModeCaption(InterfaceMode))
            CboInterfaceType.Items.Add(Item)
            If InterfaceMode = ModeToSelect Then
                Selected = Item
            End If
        Next

        If Selected Is Nothing Then
            Selected = New ByteItem(ModeToSelect, ModeToSelect.ToString("X2"))
            CboInterfaceType.Items.Insert(0, Selected)
        End If

        CboInterfaceType.SelectedItem = Selected
        CboInterfaceType.EndUpdate()
    End Sub

    Private Sub PopulateWriteAllowed(ValueToSelect As Byte)
        CboWriteAllowed.BeginUpdate()
        CboWriteAllowed.Items.Clear()

        Dim Selected As ByteItem = Nothing
        For Each WriteAllowedValue In WriteAllowedValues
            Dim Item As New ByteItem(WriteAllowedValue, WriteAllowedCaption(WriteAllowedValue))
            CboWriteAllowed.Items.Add(Item)
            If WriteAllowedValue = ValueToSelect Then
                Selected = Item
            End If
        Next

        If Selected Is Nothing Then
            Selected = New ByteItem(ValueToSelect, ValueToSelect.ToString("X2"))
            CboWriteAllowed.Items.Insert(0, Selected)
        End If

        CboWriteAllowed.SelectedItem = Selected
        CboWriteAllowed.EndUpdate()
    End Sub

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

    Private Shared Function TrackEncodingCaption(TrackEncoding As Byte) As String
        Select Case TrackEncoding
            Case &H0
                Return My.Resources.TrackEncoding_IbmMfm
            Case &H1
                Return My.Resources.TrackEncoding_AmigaMfm
            Case &H2
                Return My.Resources.TrackEncoding_IbmFm
            Case &H3
                Return My.Resources.TrackEncoding_EmuFm
            Case &HFF
                Return My.Resources.Label_Unknown
            Case Else
                Return TrackEncoding.ToString("X2")
        End Select
    End Function

    Private Shared Function WriteAllowedCaption(Value As Byte) As String
        Select Case Value
            Case WRITE_ALLOWED
                Return My.Resources.WriteAllowed_Unprotected
            Case WRITE_PROTECTED
                Return My.Resources.SummaryPanel_WriteProtected
            Case Else
                Return Value.ToString("X2")
        End Select
    End Function

    Private Shared Function SingleStepCaption(Value As Byte) As String
        Select Case Value
            Case SINGLE_STEP
                Return My.Resources.StepMode_Single
            Case DOUBLE_STEP
                Return My.Resources.StepMode_Double
            Case Else
                Return Value.ToString("X2")
        End Select
    End Function

    Private Shared Function AlternateEncodingCaption(Value As Byte) As String
        Select Case Value
            Case ALT_ENCODING_USE
                Return My.Resources.AltEncoding_Use
            Case ALT_ENCODING_UNUSED
                Return My.Resources.AltEncoding_Unused
            Case Else
                Return Value.ToString("X2")
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

        Dim InterfaceItem = TryCast(CboInterfaceType.SelectedItem, ByteItem)
        Dim WriteAllowedItem = TryCast(CboWriteAllowed.SelectedItem, ByteItem)
        If InterfaceItem Is Nothing OrElse WriteAllowedItem Is Nothing Then
            Return False
        End If

        _Updated = _FloppyImage.UpdateHeader(RPM, BitRate, InterfaceItem.Value, WriteAllowedItem.Value)
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

    Private Sub InitializeGridColumns()
        DataGridViewTracks.AutoGenerateColumns = False
        DataGridViewTracks.Columns.Clear()

        AddTextColumn(GRID_COLUMN_TRACK, My.Resources.Label_Track, 55, DataGridViewContentAlignment.MiddleRight)
        AddTextColumn(GRID_COLUMN_OFFSET, My.Resources.Label_OffsetHex, 80, DataGridViewContentAlignment.MiddleRight, "X4")
        AddTextColumn(GRID_COLUMN_LENGTH, My.Resources.Label_Length, 80, DataGridViewContentAlignment.MiddleRight, "N0")
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

    Private Function GetTrackTable(Image As HFEImage) As DataTable
        Dim TrackTable As New DataTable("HFETracks")

        AddDataColumn(TrackTable, GRID_COLUMN_TRACK, GetType(Byte))
        AddDataColumn(TrackTable, GRID_COLUMN_OFFSET, GetType(UShort))
        AddDataColumn(TrackTable, GRID_COLUMN_LENGTH, GetType(UInteger))

        If Image.SideCount > 0 Then
            For Track As Integer = 0 To Image.TrackCount - 1
                Dim Row = TrackTable.NewRow()
                Dim TrackData = Image.GetTrack(CByte(Track), 0)
                Row(GRID_COLUMN_TRACK) = CByte(Track)
                If TrackData IsNot Nothing Then
                    Row(GRID_COLUMN_OFFSET) = TrackData.TrackListOffset
                    Row(GRID_COLUMN_LENGTH) = CUInt(TrackData.TrackListLength)
                End If
                TrackTable.Rows.Add(Row)
            Next
        End If

        Return TrackTable
    End Function

    Private Sub AddDataColumn(Table As DataTable, Name As String, DataType As Type)
        Table.Columns.Add(New DataColumn(Name, DataType))
    End Sub

    Private Class ByteItem
        Public Sub New(Value As Byte, Caption As String)
            Me.Value = Value
            _Caption = Caption
        End Sub

        Public ReadOnly Property Value As Byte

        Public Overrides Function ToString() As String
            Return _Caption
        End Function

        Private ReadOnly _Caption As String
    End Class
End Class
