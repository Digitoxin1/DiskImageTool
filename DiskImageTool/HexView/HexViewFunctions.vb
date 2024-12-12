Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Hb.Windows.Forms

Namespace HexView
    Module HexViewFunctions
        Public Function ClipboardHasHex() As Boolean
            Dim DataObject = Clipboard.GetDataObject()

            If DataObject.GetDataPresent(GetType(String)) Then
                Dim Hex = CStr(DataObject.GetData(GetType(String)))
                Return ConvertHexToBytes(Hex) IsNot Nothing
            End If

            Return False
        End Function
        Private Function ConvertHexToByte(Hex As String, <Out> ByRef b As Byte) As Boolean
            Return Byte.TryParse(Hex, NumberStyles.HexNumber, Thread.CurrentThread.CurrentCulture, b)
        End Function

        Public Function ConvertHexToBytes(Hex As String) As Byte()
            If String.IsNullOrEmpty(Hex) Then
                Return Nothing
            End If

            Hex = Hex.Trim()
            Hex = Hex.Replace(" ", "")
            Hex = Hex.Replace(Chr(13), "")
            Hex = Hex.Replace(Chr(10), "")
            Hex = Hex.Replace(Chr(9), "")

            Dim regex = New Regex("^[0-9A-F]*$", RegexOptions.IgnoreCase)

            Dim HexArray As String()
            If regex.IsMatch(Hex) Then
                If Hex.Length Mod 2 = 1 Then
                    Hex = "0" & Hex
                End If
                HexArray = New String(Hex.Length / 2 - 1) {}
                For i As Integer = 0 To Hex.Length / 2 - 1
                    HexArray(i) = Hex.Substring(i * 2, 2)
                Next
            Else
                Return Nothing
            End If

            Dim ByteArray = New Byte(HexArray.Length - 1) {}
            Dim b As Byte = Nothing
            For j = 0 To HexArray.Length - 1
                Dim HexValue = HexArray(j)

                If Not ConvertHexToByte(HexValue, b) Then
                    Return Nothing
                End If

                ByteArray(j) = b
            Next

            Return ByteArray
        End Function

        Public Sub CopyHex(HexBox As HexBox, Formatted As Boolean)
            Dim Capacity As Integer = HexBox.SelectionLength * 2 + HexBox.SelectionLength
            If Formatted Then
                Capacity += HexBox.SelectionLength \ 16
            End If
            Dim SB = New System.Text.StringBuilder(Capacity)
            For Counter = 0 To HexBox.SelectionLength - 1
                Dim B = HexBox.ByteProvider.ReadByte(HexBox.SelectionStart + Counter)
                SB.Append(B.ToString("X2"))
                If Formatted AndAlso (Counter + 1) Mod 16 = 0 Then
                    SB.Append(vbNewLine)
                Else
                    SB.Append(" ")
                End If
            Next
            Clipboard.SetText(SB.ToString)
        End Sub
    End Module
End Namespace
