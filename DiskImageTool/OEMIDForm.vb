Imports System.Text

Public Class OEMIDForm
    Private ReadOnly _DiskImage As DiskImage.Disk

    Public Sub New(DiskImage As DiskImage.Disk, OEMIDDictionary As Dictionary(Of UInteger, List(Of OEMID)))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _DiskImage = DiskImage
        Dim BootstrapChecksum = Crc32.ComputeChecksum(DiskImage.BootSector.BootStrapCode)
        Dim OEMIDString As String = Encoding.UTF8.GetString(DiskImage.BootSector.OEMID)

        txtCurrentOEMID.Text = OEMIDString

        If OEMIDDictionary.ContainsKey(BootstrapChecksum) Then
            Dim OEMIDList As List(Of OEMID) = OEMIDDictionary.Item(BootstrapChecksum)
            For Each OEMID In OEMIDList
                Dim Index = CboOEMID.Items.Add(OEMID.ID)
                If OEMID.ID = OEMIDString Then
                    CboOEMID.SelectedIndex = Index
                End If
            Next
        End If
        If CboOEMID.SelectedIndex = -1 Then
            If OEMIDString <> "" Then
                CboOEMID.Items.Add(OEMIDString)
            End If
            If CboOEMID.Items.Count > 0 Then
                CboOEMID.SelectedIndex = 0
            End If
        End If
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        Dim OEMIDString As String = Encoding.UTF8.GetString(_DiskImage.BootSector.OEMID)
        Dim NewOEMID As String = Strings.Left(CboOEMID.Text, 8).PadRight(8)

        If OEMIDString <> NewOEMID Then
            _DiskImage.BootSector.OEMID = Encoding.UTF8.GetBytes(NewOEMID)
        End If
    End Sub

    Private Sub OEMIDForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        ActiveControl = CboOEMID
    End Sub
End Class