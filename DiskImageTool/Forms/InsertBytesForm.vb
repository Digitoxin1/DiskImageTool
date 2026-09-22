Imports System.Globalization

Public Class InsertBytesForm
    Public Sub New(DefaultFill As Byte)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        If DefaultFill = &H4E Then
            RadBtn4E.Checked = True
        Else
            RadBtn00.Checked = True
        End If

        RefreshOtherState()
    End Sub

    Public Shared Function Display(DefaultFill As Byte) As (Result As Boolean, Count As Integer, FillByte As Byte)
        Using dlg As New InsertBytesForm(DefaultFill)
            dlg.ShowDialog(App.CurrentFormInstance)

            If dlg.DialogResult <> DialogResult.OK Then
                Return (False, 0, 0)
            End If

            Return (True, dlg.ByteCount, dlg.FillByte)
        End Using
    End Function

    Public ReadOnly Property ByteCount As Integer
        Get
            Return CInt(NumCount.Value)
        End Get
    End Property

    Public ReadOnly Property FillByte As Byte
        Get
            If RadBtn4E.Checked Then
                Return &H4E
            ElseIf RadBtn00.Checked Then
                Return 0
            End If

            Dim Value As Byte
            If Byte.TryParse(TextOther.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, Value) Then
                Return Value
            End If

            Return 0
        End Get
    End Property

    Private Sub LocalizeForm()
        BtnCancel.Text = My.Resources.Menu_Cancel
        BtnOK.Text = My.Resources.Menu_Ok
        LabelCount.Text = My.Resources.Label_Bytes
        RadBtnOther.Text = My.Resources.Label_Other
        Me.Text = My.Resources.Menu_InsertBytes
    End Sub

    Private Sub RefreshOtherState()
        TextOther.Enabled = RadBtnOther.Checked
        If RadBtnOther.Checked Then
            TextOther.Focus()
            TextOther.SelectAll()
        End If
        RefreshOkEnabled()
    End Sub

    Private Sub RefreshOkEnabled()
        If Not RadBtnOther.Checked Then
            BtnOK.Enabled = True
            Exit Sub
        End If

        Dim Value As Byte
        BtnOK.Enabled = Byte.TryParse(TextOther.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, Value)
    End Sub

    Private Function IsHexChar(Value As Char) As Boolean
        Return (Value >= "0"c AndAlso Value <= "9"c) OrElse (Value >= "A"c AndAlso Value <= "F"c) OrElse (Value >= "a"c AndAlso Value <= "f"c)
    End Function

    Private Sub FilterOtherText()
        Dim Filtered As String = ""
        For Each C As Char In TextOther.Text
            If IsHexChar(C) Then
                Filtered &= Char.ToUpperInvariant(C)
            End If
        Next
        If Filtered.Length > TextOther.MaxLength Then
            Filtered = Filtered.Substring(0, TextOther.MaxLength)
        End If
        If Filtered <> TextOther.Text Then
            Dim Start = TextOther.SelectionStart
            TextOther.Text = Filtered
            TextOther.SelectionStart = Math.Min(Start, TextOther.Text.Length)
        End If
    End Sub

    Private Sub RadBtn_CheckedChanged(sender As Object, e As EventArgs) Handles RadBtn4E.CheckedChanged, RadBtn00.CheckedChanged, RadBtnOther.CheckedChanged
        RefreshOtherState()
    End Sub

    Private Sub TextOther_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextOther.KeyPress
        If Char.IsControl(e.KeyChar) Then
            Exit Sub
        End If
        If Not IsHexChar(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextOther_TextChanged(sender As Object, e As EventArgs) Handles TextOther.TextChanged
        FilterOtherText()
        RefreshOkEnabled()
    End Sub
End Class
