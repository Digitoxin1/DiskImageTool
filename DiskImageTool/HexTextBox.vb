Imports System.ComponentModel

Public Class HexTextBox
    Inherits System.Windows.Forms.MaskedTextBox

    Private ReadOnly _ValidChars() As Char = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F"}
    Private _MaskLength As Integer
    Private _SuppressEvent As Boolean = False
    Private ReadOnly _Menu As ContextMenuStrip
    Private WithEvents ButtonUndo As ToolStripMenuItem
    Private _LastValue As String = ""
    Private _UndoValue As String = ""

    Public Sub New()
        _Menu = New ContextMenuStrip
        ButtonUndo = _Menu.Items.Add("&Undo")
        ButtonUndo.Enabled = False
        Me.ContextMenuStrip = _Menu
        MyBase.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals
        MyBase.CutCopyMaskFormat = MaskFormat.ExcludePromptAndLiterals
    End Sub

    Public Sub SetHex(Value() As Byte)
        Dim NewText = BitConverter.ToString(Value).Replace("-", " ")

        If Me.Text <> NewText Then
            _SuppressEvent = True
            Me.Text = NewText
            _SuppressEvent = False
            ButtonUndo.Enabled = False
        End If
    End Sub

    Public Function GetHex() As Byte()
        Return HexStringToBytes(GetHexString())
    End Function

    <Description("Length of mask"), Category("Behavior")>
    Public Property MaskLength As Integer
        Get
            Return _MaskLength
        End Get
        Set
            _MaskLength = Value
            Dim Mask As String = ""
            For Counter = 1 To MaskLength
                Mask &= "AA"
                If Counter < MaskLength Then
                    Mask &= " "
                End If
            Next
            MyBase.Mask = Mask
        End Set
    End Property

    <Browsable(False)>
    Public Overloads Property Mask As String

    <Browsable(False)>
    Public Overloads Property TextMaskFormat As MaskFormat

    <Browsable(False)>
    Public Overloads Property CutCopyMaskFormat As MaskFormat

    Protected Overrides Sub OnClick(e As EventArgs)
        Me.SelectionStart = (Me.SelectionStart \ 3) * 3
        MyBase.OnClick(e)
    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        _LastValue = Me.Text

        MyBase.OnGotFocus(e)
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        If e.KeyCode = Keys.Back Or e.KeyCode = Keys.Delete Then
            e.SuppressKeyPress = True
            If e.KeyCode = Keys.Back And Me.SelectionStart > 0 Then
                Me.SelectionStart -= 1
            End If
        Else
            MyBase.OnKeyDown(e)
        End If
    End Sub

    Protected Overrides Sub OnDoubleClick(e As EventArgs)
        Me.SelectAll()

        MyBase.OnDoubleClick(e)
    End Sub

    Protected Overrides Sub OnKeyPress(e As KeyPressEventArgs)
        If (e.KeyChar >= "a" And e.KeyChar <= "f") Then
            e.KeyChar = Chr(Asc(e.KeyChar) - 32)
        ElseIf Not _ValidChars.Contains(e.KeyChar) Then
            e.KeyChar = ""
            e.Handled = True
        End If

        Me.SelectionLength = 0

        MyBase.OnKeyPress(e)
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        Dim NewValue = GetHexString()
        If Me.Text <> NewValue Then
            Me.Text = NewValue
        End If

        MyBase.OnLostFocus(e)
    End Sub

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        If _SuppressEvent Then
            Exit Sub
        End If

        _UndoValue = _LastValue
        ButtonUndo.Enabled = True

        MyBase.OnTextChanged(e)
    End Sub

    Private Function GetHexString() As String
        Dim Size = (Me.Mask.Length + 1) / 3 * 2
        Dim HexString = ""
        Dim TmpString = Me.Text
        For Counter = 0 To TmpString.Length - 1
            Dim C As Char = TmpString.Substring(Counter, 1)
            If _ValidChars.Contains(C) Then
                HexString &= C
            Else
                HexString &= 0
            End If
        Next

        Return HexString.Replace(" ", "0").PadRight(Size, "0")
    End Function

    Private Sub ButtonUndo_Click(sender As Object, e As EventArgs) Handles ButtonUndo.Click
        Dim CurrentValue = Me.Text
        Me.Text = _UndoValue
        _UndoValue = CurrentValue
    End Sub
End Class
