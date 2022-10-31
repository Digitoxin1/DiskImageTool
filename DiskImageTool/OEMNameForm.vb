Imports System.Text

Public Class OEMNameForm
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _ValidChars() As Char = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F"}
    Private _SuppressEvent As Boolean = True

    Public Sub New(Disk As DiskImage.Disk, OEMNameDictionary As Dictionary(Of UInteger, BootstrapLookup))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk
        Dim BootstrapChecksum = Crc32.ComputeChecksum(Disk.BootSector.BootStrapCode)
        Dim OEMName As New KnownOEMName With {
            .Name = Disk.BootSector.OEMName
        }


        TxtCurrentOEMName.Text = OEMName.GetNameAsString
        TxtCurrentOEMHex.Text = BitConverter.ToString(OEMName.Name).Replace("-", " ")

        If OEMNameDictionary.ContainsKey(BootstrapChecksum) Then
            Dim BootstrapType = OEMNameDictionary.Item(BootstrapChecksum)
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
            MskOEMNameHex.Text = BitConverter.ToString(OEMName.Name).Replace("-", " ")
        End If
    End Sub

    Private Function GetHexString() As String
        Dim HexString = ""
        Dim TmpString = MskOEMNameHex.Text
        For Counter = 0 To TmpString.Length - 1
            Dim C As Char = TmpString.Substring(Counter, 1)
            If _ValidChars.Contains(C) Then
                HexString &= C
            Else
                HexString &= 0
            End If
        Next

        Return HexString.Replace(" ", "0").PadRight(16, "0")
    End Function

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
        _SuppressEvent = False
    End Sub

    Private Sub MskOEMNameHex_KeyPress(sender As Object, e As KeyPressEventArgs) Handles MskOEMNameHex.KeyPress
        If (e.KeyChar >= "a" And e.KeyChar <= "f") Then
            e.KeyChar = Chr(Asc(e.KeyChar) - 32)
        ElseIf Not _ValidChars.Contains(e.KeyChar) Then
            e.KeyChar = ""
            e.Handled = True
        End If
    End Sub

    Private Sub MskOEMNameHex_LostFocus(sender As Object, e As EventArgs) Handles MskOEMNameHex.LostFocus
        MskOEMNameHex.Text = GetHexString()
    End Sub

    Private Sub MskOEMNameHex_Click(sender As Object, e As EventArgs) Handles MskOEMNameHex.Click
        MskOEMNameHex.SelectionStart = (MskOEMNameHex.SelectionStart \ 3) * 3
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

        _SuppressEvent = True
        MskOEMNameHex.Text = BitConverter.ToString(OEMName).Replace("-", " ")
        _SuppressEvent = False
    End Sub

    Private Sub MskOEMNameHex_TextChanged(sender As Object, e As EventArgs) Handles MskOEMNameHex.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim OEMName = HexStringToBytes(GetHexString())

        _SuppressEvent = True
        CboOEMName.Text = CodePage437ToUnicode(OEMName)
        _SuppressEvent = False
    End Sub
End Class