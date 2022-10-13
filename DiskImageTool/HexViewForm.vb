Imports System.Text
Imports System.Runtime.InteropServices

Public Class HexViewForm
    Private ReadOnly InvalidChars() As Byte = {127, 129, 141, 143, 144, 152, 157}
    Private _SuppressEvent As Boolean = False

    Public Sub New(DataBlockList As List(Of DiskImage.DataBlock))
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        Me.Cursor = Cursors.WaitCursor
        ListViewHex.Items.Clear()
        CmbGroups.Items.Clear()
        Dim Group As ListViewGroup
        Dim OffsetStart As UInteger = 0
        Dim Index As Integer = 0
        For Each DataBlock In DataBlockList
            Dim Header As String = "Sector " & DataBlock.Sector
            If DataBlock.Cluster > 0 Then
                Header = "Cluster " & DataBlock.Cluster & ", " & Header
            End If
            Group = New ListViewGroup(Header)

            ListViewHex.Groups.Add(Group)
            ListViewHex.ShowGroups = True
            CmbGroups.Items.Add(Header)

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
        Me.Cursor = Cursors.Default

        _SuppressEvent = True
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

    Private Sub ListViewHex_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListViewHex.ColumnWidthChanging
        e.NewWidth = Me.ListViewHex.Columns(e.ColumnIndex).Width
        e.Cancel = True
    End Sub

    Private Sub ListViewHex_Scroll(sender As Object, e As EventArgs) Handles ListViewHex.Scroll
        SetComboIndex()
    End Sub

    Private Sub CmbGroups_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbGroups.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        ListViewHex.ScrollToGroup(CmbGroups.SelectedIndex, 45)
    End Sub
End Class