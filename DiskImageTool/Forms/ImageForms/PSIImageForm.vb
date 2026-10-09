Imports DiskImageTool.DiskImage
Imports DiskImageTool.ImageFormats.PSI

Public Class PSIImageForm
    Private Const GRID_COLUMN_ALTERNATE As String = "GridAlternate"
    Private Const GRID_COLUMN_COMPRESSED As String = "GridCompressed"
    Private Const GRID_COLUMN_CYLINDER As String = "GridCylinder"
    Private Const GRID_COLUMN_DATA_CRC_ERROR As String = "GridDataCrcError"
    Private Const GRID_COLUMN_HEAD As String = "GridHead"
    Private Const GRID_COLUMN_SECTOR As String = "GridSector"
    Private Const GRID_COLUMN_SECTORS As String = "GridSectors"
    Private Const GRID_COLUMN_SIZE As String = "GridSize"
    Private Const GRID_COLUMN_WEAK_BITS As String = "GridWeakBits"
    Private Const GRID_COLUMN_HEADER As String = "GridHeader"
    Private Const GRID_COLUMN_HEADER_CYLINDER As String = "GridHeaderCylinder"
    Private Const GRID_COLUMN_HEADER_HEAD As String = "GridHeaderHead"
    Private Const GRID_COLUMN_HEADER_SECTOR As String = "GridHeaderSector"
    Private Const GRID_COLUMN_HEADER_SIZE As String = "GridHeaderSize"
    Private Const GRID_COLUMN_HEADER_ENCODING As String = "GridHeaderEncoding"
    Private Const GRID_COLUMN_HEADER_ID_CRC As String = "GridHeaderIdCrc"
    Private Const GRID_COLUMN_HEADER_DATA_CRC As String = "GridHeaderDataCrc"
    Private Const GRID_COLUMN_HEADER_DELETED_DAM As String = "GridHeaderDeletedDam"
    Private Const GRID_COLUMN_HEADER_MISSING_DAM As String = "GridHeaderMissingDam"

    Private ReadOnly _DataCrcErrorEdits As New Dictionary(Of Integer, Boolean)
    Private ReadOnly _FloppyImage As PSIFloppyImage
    Private ReadOnly _Groups As New List(Of TrackGroup)
    Private _LoadingSectors As Boolean
    Private _Updated As Boolean

    Public Sub New(FloppyImage As PSIFloppyImage)

        ' This call is required by the designer.
        InitializeComponent()

        ImageForm.EnableDoubleBuffer(DataGridViewTracks)
        ImageForm.EnableDoubleBuffer(DataGridViewSectors)

        ' Add any initialization after the InitializeComponent() call.
        _FloppyImage = FloppyImage
        Dim Image = FloppyImage.Image
        LocalizeForm()
        InitializeGridColumns()
        PopulateHeader(Image)
        BuildGroups(Image)
        DataGridViewTracks.DataSource = GetTrackTable(Image)
        ShowSelectedSectors()
    End Sub

    Public Shared Function Display(Disk As Disk) As Boolean
        If Disk Is Nothing OrElse Disk.Image Is Nothing OrElse Disk.Image.ImageType <> FloppyImageType.PSIImage Then
            Return False
        End If

        Dim FloppyImage = DirectCast(Disk.Image, PSIFloppyImage)
        Using dlg As New PSIImageForm(FloppyImage)
            dlg.ShowDialog(App.CurrentFormInstance)
            Return dlg._Updated
        End Using
    End Function

    Private Shared Function FormatCaption(Format As DefaultSectorFormat) As String
        Select Case Format
            Case DefaultSectorFormat.IBM_FM
                Return My.Resources.PSI_Format_IbmFm
            Case DefaultSectorFormat.IBM_MFM_DD
                Return My.Resources.PSI_Format_IbmMfmDd
            Case DefaultSectorFormat.IBM_MFM_HD
                Return My.Resources.PSI_Format_IbmMfmHd
            Case DefaultSectorFormat.IBM_MFM_ED
                Return My.Resources.PSI_Format_IbmMfmEd
            Case DefaultSectorFormat.MAG_GCR
                Return My.Resources.PSI_Format_MacintoshGcr
            Case Else
                Return CUShort(Format).ToString("X4")
        End Select
    End Function

    Private Shared Function GroupSizeCaption(Image As PSISectorImage, Group As TrackGroup) As String
        If Group.SectorIndexes.Count = 0 Then
            Return ""
        End If

        Dim Size = Image.Sectors(Group.SectorIndexes(0)).Size
        For Index = 1 To Group.SectorIndexes.Count - 1
            If Image.Sectors(Group.SectorIndexes(Index)).Size <> Size Then
                Return ""
            End If
        Next

        Return Size.ToString("N0")
    End Function

    Private Shared Function EncodingCaption(Encoding As MFMEncodingSubtype) As String
        Select Case Encoding
            Case MFMEncodingSubtype.DoubleDensity
                Return My.Resources.PSI_Encoding_DoubleDensity
            Case MFMEncodingSubtype.HighDensity
                Return My.Resources.PSI_Encoding_HighDensity
            Case MFMEncodingSubtype.ExtraDensity
                Return My.Resources.PSI_Encoding_ExtraDensity
            Case Else
                Return CByte(Encoding).ToString("X2")
        End Select
    End Function

    Private Shared Sub ApplyAdditionalHeader(Row As DataRow, Sector As PSISector)
        If Sector.FMHeader IsNot Nothing Then
            Row(GRID_COLUMN_HEADER) = "IBMF"
            ApplyIbmHeader(Row, Sector.FMHeader)
        ElseIf Sector.MFMHeader IsNot Nothing Then
            Row(GRID_COLUMN_HEADER) = "IBMM"
            ApplyIbmHeader(Row, Sector.MFMHeader)
        ElseIf Sector.GCRHeader IsNot Nothing Then
            Row(GRID_COLUMN_HEADER) = "MACG"
            ApplyGcrHeader(Row, Sector.GCRHeader)
        End If
    End Sub

    Private Shared Sub ApplyIbmHeader(Row As DataRow, Header As IBMSectorHeader)
        Row(GRID_COLUMN_HEADER_CYLINDER) = Header.Cylinder.ToString()
        Row(GRID_COLUMN_HEADER_HEAD) = Header.Head.ToString()
        Row(GRID_COLUMN_HEADER_SECTOR) = Header.Sector.ToString()
        Row(GRID_COLUMN_HEADER_SIZE) = Header.Size.ToString()
        Row(GRID_COLUMN_HEADER_ENCODING) = EncodingCaption(Header.EncodingSubType)
        Row(GRID_COLUMN_HEADER_ID_CRC) = Header.IDFieldCRCError
        Row(GRID_COLUMN_HEADER_DATA_CRC) = Header.DataFieldCRCError
        Row(GRID_COLUMN_HEADER_DELETED_DAM) = Header.DeletedDAM
        Row(GRID_COLUMN_HEADER_MISSING_DAM) = Header.MissingDAM
    End Sub

    Private Shared Sub ApplyGcrHeader(Row As DataRow, Header As GCRSectorHeader)
        Row(GRID_COLUMN_HEADER_CYLINDER) = Header.Cylinder.ToString()
        Row(GRID_COLUMN_HEADER_HEAD) = Header.Head.ToString()
        Row(GRID_COLUMN_HEADER_SECTOR) = Header.Sector.ToString()
        Row(GRID_COLUMN_HEADER_ID_CRC) = (Header.Flags And GCRSectorFlags.IDFieldChecksumError) <> 0
        Row(GRID_COLUMN_HEADER_DATA_CRC) = (Header.Flags And GCRSectorFlags.DataFieldCChecksumError) <> 0
        Row(GRID_COLUMN_HEADER_MISSING_DAM) = (Header.Flags And GCRSectorFlags.MissingDataMark) <> 0
    End Sub

    Private Sub LocalizeForm()
        Me.Text = String.Format(My.Resources.FloppyImageType_SectorImageProperties, My.Resources.FloppyImageType_PCE)
        LblVersion.Text = My.Resources.Label_Version
        LblFormat.Text = My.Resources.Label_Format
        LblComment.Text = My.Resources.Label_Comment
        LblTracks.Text = My.Resources.Label_Tracks
        LblSectors.Text = My.Resources.Label_Sectors
    End Sub

    Private Sub PopulateHeader(Image As PSISectorImage)
        TxtVersion.Text = Image.Header.FormatVersion.ToString()
        TxtFormat.Text = FormatCaption(Image.Header.DefaultSectorFormat)
        TxtComment.Text = If(Image.Comment, "")
    End Sub

    Private Sub BuildGroups(Image As PSISectorImage)
        Dim Lookup As New Dictionary(Of String, TrackGroup)
        For SectorIndex = 0 To Image.Sectors.Count - 1
            Dim Sector = Image.Sectors(SectorIndex)
            Dim Key = Sector.Track.ToString() & "." & Sector.Side.ToString()
            Dim Group As TrackGroup = Nothing
            If Not Lookup.TryGetValue(Key, Group) Then
                Group = New TrackGroup(Sector.Track, Sector.Side)
                Lookup.Add(Key, Group)
                _Groups.Add(Group)
            End If

            Group.SectorIndexes.Add(SectorIndex)
        Next
    End Sub

    Private Sub InitializeGridColumns()
        DataGridViewTracks.DefaultCellStyle.Padding = New Padding(0, 0, 5, 0)
        ImageForm.PrepareGrid(DataGridViewTracks)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_CYLINDER, My.Resources.Label_Cylinder, 70, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_HEAD, My.Resources.Label_Head, 55, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_SECTORS, My.Resources.Label_Sectors, 70, DataGridViewContentAlignment.MiddleRight)
        ImageForm.AddTextColumn(DataGridViewTracks, GRID_COLUMN_SIZE, My.Resources.Label_Size, 70, DataGridViewContentAlignment.MiddleRight)

        ImageForm.PrepareGrid(DataGridViewSectors)
        ImageForm.AddTextColumn(DataGridViewSectors, GRID_COLUMN_CYLINDER, My.Resources.Label_Cylinder, 70, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        ImageForm.AddTextColumn(DataGridViewSectors, GRID_COLUMN_HEAD, My.Resources.Label_Head, 55, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        ImageForm.AddTextColumn(DataGridViewSectors, GRID_COLUMN_SECTOR, My.Resources.Label_Sector, 60, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        ImageForm.AddTextColumn(DataGridViewSectors, GRID_COLUMN_SIZE, My.Resources.Label_Size, 70, DataGridViewContentAlignment.MiddleRight, "N0", Padding:=5)
        ImageForm.AddCheckColumn(DataGridViewSectors, GRID_COLUMN_DATA_CRC_ERROR, My.Resources.Label_DataCrcError, Editable:=True)
        ImageForm.AddCheckColumn(DataGridViewSectors, GRID_COLUMN_COMPRESSED, My.Resources.Label_Compressed)
        ImageForm.AddCheckColumn(DataGridViewSectors, GRID_COLUMN_ALTERNATE, My.Resources.Label_Alternate)
        ImageForm.AddCheckColumn(DataGridViewSectors, GRID_COLUMN_WEAK_BITS, My.Resources.Label_WeakBits)
        ImageForm.AddTextColumn(DataGridViewSectors, GRID_COLUMN_HEADER, My.Resources.Label_AdditionalHeader, 110, DataGridViewContentAlignment.MiddleLeft, Padding:=5)
        ImageForm.AddTextColumn(DataGridViewSectors, GRID_COLUMN_HEADER_CYLINDER, My.Resources.Label_Cylinder, 70, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        ImageForm.AddTextColumn(DataGridViewSectors, GRID_COLUMN_HEADER_HEAD, My.Resources.Label_Head, 55, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        ImageForm.AddTextColumn(DataGridViewSectors, GRID_COLUMN_HEADER_SECTOR, My.Resources.Label_Sector, 60, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        ImageForm.AddTextColumn(DataGridViewSectors, GRID_COLUMN_HEADER_SIZE, My.Resources.Label_Size, 70, DataGridViewContentAlignment.MiddleRight, Padding:=5)
        ImageForm.AddTextColumn(DataGridViewSectors, GRID_COLUMN_HEADER_ENCODING, My.Resources.Label_Encoding, 110, DataGridViewContentAlignment.MiddleLeft, Padding:=5)
        ImageForm.AddCheckColumn(DataGridViewSectors, GRID_COLUMN_HEADER_ID_CRC, My.Resources.Label_IdCrcError)
        ImageForm.AddCheckColumn(DataGridViewSectors, GRID_COLUMN_HEADER_DATA_CRC, My.Resources.Label_DataCrcError)
        ImageForm.AddCheckColumn(DataGridViewSectors, GRID_COLUMN_HEADER_DELETED_DAM, My.Resources.Label_DeletedDam)
        ImageForm.AddCheckColumn(DataGridViewSectors, GRID_COLUMN_HEADER_MISSING_DAM, My.Resources.Label_MissingDam)
    End Sub

    Private Sub SetAdditionalHeaderColumnsVisible(Group As TrackGroup)
        Dim Visible = GroupHasAdditionalHeader(Group)
        Dim ColumnNames = {
            GRID_COLUMN_HEADER,
            GRID_COLUMN_HEADER_CYLINDER,
            GRID_COLUMN_HEADER_HEAD,
            GRID_COLUMN_HEADER_SECTOR,
            GRID_COLUMN_HEADER_SIZE,
            GRID_COLUMN_HEADER_ENCODING,
            GRID_COLUMN_HEADER_ID_CRC,
            GRID_COLUMN_HEADER_DATA_CRC,
            GRID_COLUMN_HEADER_DELETED_DAM,
            GRID_COLUMN_HEADER_MISSING_DAM
        }
        For Each ColumnName In ColumnNames
            DataGridViewSectors.Columns(ColumnName).Visible = Visible
        Next
    End Sub

    Private Function GroupHasAdditionalHeader(Group As TrackGroup) As Boolean
        If Group Is Nothing Then
            Return False
        End If

        Dim Image = _FloppyImage.Image
        For Each SectorIndex In Group.SectorIndexes
            Dim Sector = Image.Sectors(SectorIndex)
            If Sector.FMHeader IsNot Nothing OrElse Sector.MFMHeader IsNot Nothing OrElse Sector.GCRHeader IsNot Nothing Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Function GetTrackTable(Image As PSISectorImage) As DataTable
        Dim TrackTable As New DataTable("PSITracks")
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_CYLINDER, GetType(UShort))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_HEAD, GetType(Byte))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_SECTORS, GetType(Integer))
        ImageForm.AddDataColumn(TrackTable, GRID_COLUMN_SIZE, GetType(String))

        For Each Group In _Groups
            Dim Row = TrackTable.NewRow()
            Row(GRID_COLUMN_CYLINDER) = Group.Cylinder
            Row(GRID_COLUMN_HEAD) = Group.Head
            Row(GRID_COLUMN_SECTORS) = Group.SectorIndexes.Count
            Row(GRID_COLUMN_SIZE) = GroupSizeCaption(Image, Group)
            TrackTable.Rows.Add(Row)
        Next

        Return TrackTable
    End Function

    Private Sub ShowSelectedSectors()
        Dim Group As TrackGroup = Nothing
        Dim Row = DataGridViewTracks.CurrentRow
        If Row IsNot Nothing AndAlso Row.Index >= 0 AndAlso Row.Index < _Groups.Count Then
            Group = _Groups(Row.Index)
        End If

        _LoadingSectors = True
        SetAdditionalHeaderColumnsVisible(Group)
        DataGridViewSectors.DataSource = GetSectorTable(Group)
        _LoadingSectors = False
    End Sub

    Private Function GetSectorTable(Group As TrackGroup) As DataTable
        Dim SectorTable As New DataTable("PSISectors")
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_CYLINDER, GetType(UShort))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEAD, GetType(Byte))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_SECTOR, GetType(Byte))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_SIZE, GetType(UShort))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_DATA_CRC_ERROR, GetType(Boolean))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_COMPRESSED, GetType(Boolean))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_ALTERNATE, GetType(Boolean))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_WEAK_BITS, GetType(Boolean))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEADER, GetType(String))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEADER_CYLINDER, GetType(String))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEADER_HEAD, GetType(String))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEADER_SECTOR, GetType(String))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEADER_SIZE, GetType(String))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEADER_ENCODING, GetType(String))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEADER_ID_CRC, GetType(Boolean))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEADER_DATA_CRC, GetType(Boolean))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEADER_DELETED_DAM, GetType(Boolean))
        ImageForm.AddDataColumn(SectorTable, GRID_COLUMN_HEADER_MISSING_DAM, GetType(Boolean))

        If Group Is Nothing Then
            Return SectorTable
        End If

        Dim Image = _FloppyImage.Image
        For Each SectorIndex In Group.SectorIndexes
            Dim Sector = Image.Sectors(SectorIndex)
            Dim DataCrcError As Boolean
            If Not _DataCrcErrorEdits.TryGetValue(SectorIndex, DataCrcError) Then
                DataCrcError = Sector.HasDataCRCError
            End If

            Dim Row = SectorTable.NewRow()
            Row(GRID_COLUMN_CYLINDER) = Sector.Track
            Row(GRID_COLUMN_HEAD) = Sector.Side
            Row(GRID_COLUMN_SECTOR) = Sector.Sector
            Row(GRID_COLUMN_SIZE) = Sector.Size
            Row(GRID_COLUMN_DATA_CRC_ERROR) = DataCrcError
            Row(GRID_COLUMN_COMPRESSED) = Sector.IsCompressed
            Row(GRID_COLUMN_ALTERNATE) = Sector.IsAlternateSector
            Row(GRID_COLUMN_WEAK_BITS) = Sector.HasWeakBits
            Row(GRID_COLUMN_HEADER_ID_CRC) = False
            Row(GRID_COLUMN_HEADER_DATA_CRC) = False
            Row(GRID_COLUMN_HEADER_DELETED_DAM) = False
            Row(GRID_COLUMN_HEADER_MISSING_DAM) = False
            ApplyAdditionalHeader(Row, Sector)
            SectorTable.Rows.Add(Row)
        Next

        Return SectorTable
    End Function

    Private Function CollectDataCrcChanges() As List(Of PSIFloppyImage.PSIDataCrcErrorEdit)
        Dim Changes As New List(Of PSIFloppyImage.PSIDataCrcErrorEdit)
        For Each Entry In _DataCrcErrorEdits
            Changes.Add(New PSIFloppyImage.PSIDataCrcErrorEdit(Entry.Key, Entry.Value))
        Next

        Return Changes
    End Function

    Private Sub DataGridViewTracks_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridViewTracks.SelectionChanged
        If _FloppyImage Is Nothing Then
            Exit Sub
        End If

        ShowSelectedSectors()
        ImageForm.AutoSizeGrid(DataGridViewSectors)
    End Sub

    Private Sub DataGridViewSectors_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles DataGridViewSectors.CurrentCellDirtyStateChanged
        ImageForm.CommitDirtyCheckBox(DataGridViewSectors, GRID_COLUMN_DATA_CRC_ERROR)
    End Sub

    Private Sub DataGridViewSectors_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewSectors.CellValueChanged
        If _LoadingSectors OrElse e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            Exit Sub
        End If

        If DataGridViewSectors.Columns(e.ColumnIndex).Name <> GRID_COLUMN_DATA_CRC_ERROR Then
            Exit Sub
        End If

        Dim Row = DataGridViewTracks.CurrentRow
        If Row Is Nothing OrElse Row.Index < 0 OrElse Row.Index >= _Groups.Count OrElse e.RowIndex >= _Groups(Row.Index).SectorIndexes.Count Then
            Exit Sub
        End If

        Dim Value = DataGridViewSectors.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
        If TypeOf Value Is Boolean Then
            _DataCrcErrorEdits(_Groups(Row.Index).SectorIndexes(e.RowIndex)) = CBool(Value)
        End If
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        ImageForm.CommitDirtyCheckBox(DataGridViewSectors)
        _Updated = _FloppyImage.UpdateProperties(TxtComment.Text, CollectDataCrcChanges())
        DialogResult = DialogResult.OK
    End Sub

    Private Sub DataGridViewTracks_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridViewTracks.DataBindingComplete
        ImageForm.AutoSizeGrid(DataGridViewTracks, FitWidth:=True)
    End Sub

    Private Sub DataGridViewSectors_DataBindingComplete(sender As Object, e As DataGridViewBindingCompleteEventArgs) Handles DataGridViewSectors.DataBindingComplete
        ImageForm.AutoSizeGrid(DataGridViewSectors)
    End Sub

    Private Class TrackGroup
        Public Sub New(Cylinder As UShort, Head As Byte)
            Me.Cylinder = Cylinder
            Me.Head = Head
            SectorIndexes = New List(Of Integer)
        End Sub

        Public ReadOnly Property Cylinder As UShort
        Public ReadOnly Property Head As Byte
        Public ReadOnly Property SectorIndexes As List(Of Integer)
    End Class
End Class
