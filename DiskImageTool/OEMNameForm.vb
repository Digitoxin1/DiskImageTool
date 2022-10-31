Imports System.Text

Public Class OEMNameForm
    Private ReadOnly _Disk As DiskImage.Disk

    Public Sub New(Disk As DiskImage.Disk, OEMNameDictionary As Dictionary(Of UInteger, BootstrapLookup))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk
        Dim BootstrapChecksum = Crc32.ComputeChecksum(Disk.BootSector.BootStrapCode)
        Dim OEMName = Disk.BootSector.OEMName
        Dim OEMNameString = Disk.BootSector.GetOEMNameString

        txtCurrentOEMName.Text = OEMNameString
        If OEMNameDictionary.ContainsKey(BootstrapChecksum) Then
            Dim BootstrapType = OEMNameDictionary.Item(BootstrapChecksum)
            For Each KnownOEMName In BootstrapType.KnownOEMNames
                If KnownOEMName.Name.Length > 0 Then
                    Dim IsMatch = ByteArrayCompare(KnownOEMName.Name, OEMName)
                    If (KnownOEMName.Suggestion And Not BootstrapType.ExactMatch) Or IsMatch Then
                        Dim Index = CboOEMName.Items.Add(KnownOEMName.GetNameAsString)
                        If IsMatch Then
                            CboOEMName.SelectedIndex = Index
                        End If
                    End If
                End If
            Next
        End If
        If CboOEMName.SelectedIndex = -1 Then
            If OEMName.Length > 0 Then
                CboOEMName.Items.Add(OEMNameString)
            End If
            If CboOEMName.Items.Count > 0 Then
                CboOEMName.SelectedIndex = 0
            End If
        End If
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        Dim OEMName = _Disk.BootSector.OEMName
        Dim NewOEMNameString As String = Strings.Left(CboOEMName.Text, 8).PadRight(8)
        Dim NewOEMName = UnicodeToCodePage437(NewOEMNameString)

        If Not ByteArrayCompare(OEMName, NewOEMName) Then
            _Disk.BootSector.OEMName = NewOEMName
        End If
    End Sub

    Private Sub OEMNameForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        ActiveControl = CboOEMName
    End Sub
End Class