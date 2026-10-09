Imports DiskImageTool.DiskImage
Imports DiskImageTool.ImageFormats.HFE

Public Class HFEImageForm
    Private Const ALT_ENCODING_UNUSED As Byte = &HFF
    Private Const ALT_ENCODING_USE As Byte = &H0
    Private Const DOUBLE_STEP As Byte = &H0
    Private Const GRID_COLUMN_LENGTH As String = "GridLength"
    Private Const GRID_COLUMN_OFFSET As String = "GridOffset"
    Private Const GRID_COLUMN_TRACK As String = "GridTrack"
    Private Const SINGLE_STEP As Byte = &HFF
    Private Const WRITE_ALLOWED As Byte = &HFF
    Private Const WRITE_PROTECTED As Byte = &H0

    Private Shared ReadOnly WriteAllowedValues() As Byte = {WRITE_ALLOWED, WRITE_PROTECTED}

    Private ReadOnly _FloppyImage As HFEFloppyImage
    Private _Updated As Boolean

    Public Sub New(FloppyImage As HFEFloppyImage)

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
        ImageForm.AttachNumericTextBox(TxtRPM)
        ImageForm.AttachNumericTextBox(TxtBitRate)
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

    Private Function ApplyUpdates() As Boolean
        Dim RPM As UShort
        Dim BitRate As UShort
        If Not ImageForm.TryReadUShort(TxtRPM, RPM) OrElse Not ImageForm.TryReadUShort(TxtBitRate, BitRate) Then
            Return False
        End If

        Dim InterfaceItem = TryCast(CboInterfaceType.SelectedItem, ImageForm.ByteListItem)
        Dim WriteAllowedItem = TryCast(CboWriteAllowed.SelectedItem, ImageForm.ByteListItem)
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

    Private Function GetTrackTable(Image As HFEImage) As DataTable
        Dim TrackTable As New DataTable("HFETracks")

        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_TRACK, GetType(Byte))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_OFFSET, GetType(UShort))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_LENGTH, GetType(UInteger))

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

    Private Sub InitializeGridColumns()
        ImageForm.PrepareGrid(DataGridViewTracks)

        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_TRACK, My.Resources.Label_Track, 55, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_OFFSET, My.Resources.Label_OffsetHex, 80, DataGridViewContentAlignment.MiddleRight, "X4")
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_LENGTH, My.Resources.Label_Length, 80, DataGridViewContentAlignment.MiddleRight, "N0")
    End Sub

    Private Sub LocalizeForm()
        ImageForm.LocalizeButtons(BtnUpdate, BtnCancel)
        Me.Text = String.Format(My.Resources.FloppyImageType_SectorImageProperties, My.Resources.FloppyImageType_HFE)
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
        ImageForm.PopulateByteCombo(CboInterfaceType, ImageForm.InterfaceModes, CByte(Image.FloppyInterfaceMode), AddressOf ImageForm.InterfaceModeCaption)
        ImageForm.PopulateByteCombo(CboWriteAllowed, WriteAllowedValues, Image.WriteAllowed, AddressOf WriteAllowedCaption)
    End Sub
End Class
