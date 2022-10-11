Imports System.Text

Public Class OEMIDForm
    Private ReadOnly _DiskImage As DiskImage.Disk
    Private _Result As Boolean = False

    Public Sub New(DiskImage As DiskImage.Disk, BootstrapTypes As Dictionary(Of UInteger, BootstrapType))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _DiskImage = DiskImage
        Dim BootstrapChecksum = Crc32.ComputeChecksum(DiskImage.BootSector.BootStrapCode)
        Dim OSNameString As String = Encoding.UTF8.GetString(DiskImage.BootSector.OSName)

        txtCurrentOEMID.Text = OSNameString

        If BootstrapTypes.ContainsKey(BootstrapChecksum) Then
            Dim OSNameList As List(Of String) = BootstrapTypes.Item(BootstrapChecksum).OSNames
            For Each OSName In OSNameList
                Dim Index = CboOEMID.Items.Add(OSName)
                If OSName = OSNameString Then
                    CboOEMID.SelectedIndex = Index
                End If
            Next
        End If
        If CboOEMID.SelectedIndex = -1 Then
            If OSNameString <> "" Then
                CboOEMID.Items.Add(OSNameString)
            End If
            If CboOEMID.Items.Count > 0 Then
                CboOEMID.SelectedIndex = 0
            End If
        End If
    End Sub

    Public ReadOnly Property Result As Boolean
        Get
            Return _Result
        End Get
    End Property

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        Dim OSNameString As String = Encoding.UTF8.GetString(_DiskImage.BootSector.OSName)
        Dim NewOSName As String = Strings.Left(CboOEMID.Text, 8).PadRight(8)

        If OSNameString <> NewOSName Then
            _DiskImage.BootSector.OSName = Encoding.UTF8.GetBytes(NewOSName)
            _Result = True
        End If

        Me.Close()
    End Sub
End Class