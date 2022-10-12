Imports System.Text
Imports System.Runtime.InteropServices

Public Class HexViewForm
    Private ReadOnly InvalidChars() As Byte = {127, 129, 141, 143, 144, 152, 157}

    Public Sub New(DataBlockList As List(Of DiskImage.DataBlock))
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        ListViewHex.Items.Clear()
        Dim OffsetStart As UInteger = 0
        Dim Index As Integer = 0
        For Each DataBlock In DataBlockList
            If Index > 0 Then
                If DataBlock.Offset > OffsetStart Then
                    ListViewHex.Items.Add(New ListViewItem(""))
                End If
            End If

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
                Dim Item As New ListViewItem("")
                Dim SI = Item.SubItems.Add((DataBlock.Offset + OffsetStart).ToString("X8"))
                Item.SubItems.Add(HexValues)
                Item.SubItems.Add(DecodedText)
                ListViewHex.Items.Add(Item)
            Next
            OffsetStart += 16
            Index += 1
        Next
    End Sub

    Private Sub ListViewHex_ColumnWidthChanging(sender As Object, e As ColumnWidthChangingEventArgs) Handles ListViewHex.ColumnWidthChanging
        e.NewWidth = Me.ListViewHex.Columns(e.ColumnIndex).Width
        e.Cancel = True
    End Sub
End Class