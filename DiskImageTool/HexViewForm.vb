
Public Class HexViewForm
    Private ReadOnly InvalidChars() As Byte = {127, 129, 141, 143, 144, 152, 157}
    Private _SuppressEvent As Boolean = False
    Private ReadOnly _Disk As DiskImage.Disk
    Private _Modified As Boolean = False

    Public Sub New(Disk As DiskImage.Disk, DataBlockList As List(Of DiskImage.DataBlock), AllowModifications As Boolean)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk

        If AllowModifications Then
            BtnClear.Visible = True
            ComboBytes.Visible = True
            ComboBytes.SelectedIndex = 0
        Else
            BtnClear.Visible = False
            ComboBytes.Visible = False
        End If

        FlowLayoutHeader.Left = (Me.ClientSize.Width - FlowLayoutHeader.Width) / 2

        Initialize(DataBlockList)
    End Sub

    Public ReadOnly Property Modified As Boolean
        Get
            Return _Modified
        End Get
    End Property

    Private Sub Initialize(DataBlockList As List(Of DiskImage.DataBlock))
        _SuppressEvent = True

        ListViewHex.BeginUpdate()
        ListViewHex.Items.Clear()
        CmbGroups.Items.Clear()
        Dim Group As ListViewGroup
        Dim OffsetStart As UInteger = 0
        Dim Index As Integer = 0
        For Each DataBlock In DataBlockList
            Dim ComboGroup = New ComboGroups(DataBlock.Cluster, DataBlock.Sector, DataBlock.Offset)
            Group = New ListViewGroup(ComboGroup.ToString)
            ListViewHex.Groups.Add(Group)
            ListViewHex.ShowGroups = True
            CmbGroups.Items.Add(ComboGroup)


            Dim Rows As Integer = Math.Ceiling(DataBlock.Data.Length / 16)
            For Row = 1 To Rows
                OffsetStart = (Row - 1) * 16
                Dim HexValues As String = ""
                Dim DecodedText As String = ""
                For Col = 0 To 15
                    Dim Offset As Integer = OffsetStart + Col
                    If Offset < DataBlock.Data.Length Then
                        If HexValues <> "" Then
                            HexValues &= " "
                        End If
                        HexValues &= DataBlock.Data(Offset).ToString("X2")
                        Dim Value = DataBlock.Data(Offset)
                        If Value < 32 Or InvalidChars.Contains(Value) Then
                            DecodedText &= "."
                        Else
                            DecodedText &= Chr(Value)
                        End If
                    End If
                Next
                Dim Item As New ListViewItem("", Group)
                Dim SI = Item.SubItems.Add((DataBlock.Offset + OffsetStart).ToString("X8"))
                Item.SubItems.Add(HexValues)
                Item.SubItems.Add(DecodedText)
                ListViewHex.Items.Add(Item)
            Next
            OffsetStart += 16
            Index += 1
        Next
        ListViewHex.EndUpdate()

        CmbGroups.SelectedIndex = 0

        _SuppressEvent = False
    End Sub

    Private Sub SetComboIndex()
        Dim Item = ListViewHex.HitTest(0, 45).Item
        If Item IsNot Nothing Then
            Dim NewIndex = ListViewHex.Groups.IndexOf(Item.Group)
            If CmbGroups.SelectedIndex <> NewIndex Then
                _SuppressEvent = True
                CmbGroups.SelectedIndex = NewIndex
                _SuppressEvent = False
            End If
        End If
    End Sub

    Private Sub ClearSector()
        Dim ComboGroup As ComboGroups = CmbGroups.SelectedItem()
        Dim Value = Convert.ToByte(ComboBytes.Text, 16)
        Dim Data(_Disk.BootSector.BytesPerSector - 1) As Byte
        For Index = 0 To Data.Length - 1
            Data(Index) = Value
        Next

        Dim HexValues As String = ""
        Dim DecodedText As String = ""
        For Col = 0 To 15
            If Col > 0 Then
                HexValues &= " "
            End If
            HexValues &= Value.ToString("X2")
            If Value < 32 Or InvalidChars.Contains(Value) Then
                DecodedText &= "."
            Else
                DecodedText &= Chr(Value)
            End If
        Next
        Dim Group = ListViewHex.Groups.Item(CmbGroups.SelectedIndex)
        For Each Item As ListViewItem In Group.Items
            Item.SubItems.Item(2).Text = HexValues
            Item.SubItems.Item(3).Text = DecodedText
        Next
        _Disk.SetBytes(Data, ComboGroup.Offset)
        _Modified = True
    End Sub

    Private Sub ListViewHex_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListViewHex.ColumnWidthChanging
        If _SuppressEvent Then
            Exit Sub
        End If

        e.NewWidth = Me.ListViewHex.Columns(e.ColumnIndex).Width
        e.Cancel = True
    End Sub

    Private Sub ListViewHex_Scroll(sender As Object, e As EventArgs) Handles ListViewHex.Scroll
        If _SuppressEvent Then
            Exit Sub
        End If

        SetComboIndex()
    End Sub

    Private Sub CmbGroups_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbGroups.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        ListViewHex.ScrollToGroup(CmbGroups.SelectedIndex, 45)
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs) Handles BtnClear.Click
        ClearSector
    End Sub

    Private Class ComboGroups
        Public Property Cluster As UInteger
        Public Property Sector As UInteger
        Public Property Offset As UInteger

        Public Sub New(Cluster As UInteger, Sector As UInteger, Offset As UInteger)
            Me.Cluster = Cluster
            Me.Sector = Sector
            Me.Offset = Offset
        End Sub
        Public Overrides Function ToString() As String
            Dim Header As String = "Sector " & Sector
            If Cluster > 0 Then
                Header = "Cluster " & Cluster & ", " & Header
            End If

            Return Header
        End Function
    End Class
End Class