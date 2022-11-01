Public Class OEMNameForm
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _OEMNameDictionary As Dictionary(Of UInteger, BootstrapLookup)
    Private _SuppressEvent As Boolean = True

    Public Sub New(Disk As DiskImage.Disk, OEMNameDictionary As Dictionary(Of UInteger, BootstrapLookup))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk
        _OEMNameDictionary = OEMNameDictionary
    End Sub

#Region "Events"

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        Dim OEMName = _Disk.BootSector.OEMName
        Dim NewOEMNameString As String = Strings.Left(CboOEMName.Text, 8).PadRight(8)
        Dim NewOEMName = UnicodeToCodePage437(NewOEMNameString)

        If Not ByteArrayCompare(OEMName, NewOEMName) Then
            _Disk.BootSector.OEMName = NewOEMName
        End If
    End Sub

    Private Sub CboOEMName_TextChanged(sender As Object, e As EventArgs) Handles CboOEMName.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim OEMName() As Byte

        If CboOEMName.SelectedIndex = -1 Then
            Dim OEMNameString As String = Strings.Left(CboOEMName.Text, 8).PadRight(8)
            OEMName = UnicodeToCodePage437(OEMNameString)
        Else
            OEMName = CType(CboOEMName.SelectedItem, KnownOEMName).Name
        End If

        MskOEMNameHex.SetHex(OEMName)
    End Sub

    Private Sub MskOEMNameHex_TextChanged(sender As Object, e As EventArgs) Handles MskOEMNameHex.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        _SuppressEvent = True
        CboOEMName.Text = CodePage437ToUnicode(MskOEMNameHex.GetHex)
        _SuppressEvent = False
    End Sub

    Private Sub OEMNameForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        _SuppressEvent = True

        Dim BootstrapChecksum = Crc32.ComputeChecksum(_Disk.BootSector.BootStrapCode)
        Dim OEMName As New KnownOEMName With {
            .Name = _Disk.BootSector.OEMName
        }

        TxtCurrentOEMName.Text = OEMName.GetNameAsString
        TxtCurrentOEMHex.Text = BitConverter.ToString(OEMName.Name).Replace("-", " ")

        If _OEMNameDictionary.ContainsKey(BootstrapChecksum) Then
            Dim BootstrapType = _OEMNameDictionary.Item(BootstrapChecksum)
            For Each KnownOEMName In BootstrapType.KnownOEMNames
                If KnownOEMName.Name.Length > 0 Then
                    Dim IsMatch = ByteArrayCompare(KnownOEMName.Name, OEMName.Name)
                    If (KnownOEMName.Suggestion And Not BootstrapType.ExactMatch) Or IsMatch Then
                        Dim Index = CboOEMName.Items.Add(KnownOEMName)
                        If IsMatch Then
                            CboOEMName.SelectedIndex = Index
                        End If
                    End If
                End If
            Next
        End If

        If CboOEMName.SelectedIndex = -1 Then
            If OEMName.Name.Length > 0 Then
                CboOEMName.Items.Add(OEMName)
            End If
            If CboOEMName.Items.Count > 0 Then
                CboOEMName.SelectedIndex = 0
            End If
        End If

        If CboOEMName.SelectedIndex > -1 Then
            OEMName = CType(CboOEMName.SelectedItem, KnownOEMName)
            MskOEMNameHex.SetHex(OEMName.Name)
        End If

        _SuppressEvent = False

        ActiveControl = CboOEMName
    End Sub

#End Region

End Class