Imports System.ComponentModel

Public Class HexTextBox
    Inherits MaskedTextBox
    Private ReadOnly _ValidChars() As Char = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F"}
    Private _MaskLength As Integer
    Private _SuppressEvent As Boolean = False
    Private WithEvents ButtonUndo As ToolStripMenuItem
    Private WithEvents ButtonCopy As ToolStripMenuItem
    Private _LastValue As String = ""
    Private _UndoValue As String = ""

    Public Sub New()
        MyBase.TextMaskFormat = MaskFormat.IncludePrompt
        MyBase.CutCopyMaskFormat = MaskFormat.IncludePrompt
        MyBase.InsertKeyMode = InsertKeyMode.Overwrite
        MyBase.ShortcutsEnabled = False
        MyBase.AsciiOnly = False
        MyBase.PromptChar = "0"
        MyBase.HidePromptOnLeave = False
        MyBase.AllowPromptAsInput = True

        InitMenu()
    End Sub

    Private Sub InitMenu()
        MyBase.ContextMenuStrip = New ContextMenuStrip
        ButtonUndo = MyBase.ContextMenuStrip.Items.Add("&Undo")
        MyBase.ContextMenuStrip.Items.Add(New ToolStripSeparator)
        ButtonCopy = MyBase.ContextMenuStrip.Items.Add("&Copy")
        ButtonUndo.Enabled = False
    End Sub

    Public Sub SetHex(Value() As Byte)
        Dim NewText = BitConverter.ToString(Value).Replace("-", " ")

        If MyBase.Text <> NewText Then
            _SuppressEvent = True
            MyBase.Text = NewText
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

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads ReadOnly Property Mask As String
        Get
            Return MyBase.Mask
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads ReadOnly Property TextMaskFormat As MaskFormat
        Get
            Return MyBase.TextMaskFormat
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads ReadOnly Property CutCopyMaskFormat As MaskFormat
        Get
            Return MyBase.CutCopyMaskFormat
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads ReadOnly Property InsertKeyMode As InsertKeyMode
        Get
            Return MyBase.InsertKeyMode
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads ReadOnly Property ShortcutsEnabled As Boolean
        Get
            Return MyBase.ShortcutsEnabled
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads ReadOnly Property AsciiOnly As Boolean
        Get
            Return MyBase.AsciiOnly
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads ReadOnly Property PromptChar As Char
        Get
            Return MyBase.PromptChar
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads ReadOnly Property HidePromptOnLeave As Boolean
        Get
            Return MyBase.HidePromptOnLeave
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads ReadOnly Property Text As String
        Get
            Return GetHexString()
        End Get
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overloads ReadOnly Property AllowPromptAsInput As Boolean
        Get
            Return MyBase.AllowPromptAsInput
        End Get
    End Property

    Protected Overrides Sub OnClick(e As EventArgs)
        MyBase.SelectionStart = (MyBase.SelectionStart \ 3) * 3
        MyBase.OnClick(e)
    End Sub

    Protected Overrides Sub OnGotFocus(e As EventArgs)
        _LastValue = MyBase.Text

        MyBase.OnGotFocus(e)
    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        If e.KeyCode = Keys.Back Or e.KeyCode = Keys.Delete Then
            e.SuppressKeyPress = True
            If e.KeyCode = Keys.Back And MyBase.SelectionStart > 0 Then
                MyBase.SelectionStart -= 1
            End If
        Else
            MyBase.OnKeyDown(e)
        End If
    End Sub

    Protected Overrides Sub OnDoubleClick(e As EventArgs)
        MyBase.SelectAll()

        MyBase.OnDoubleClick(e)
    End Sub

    Protected Overrides Sub OnKeyPress(e As KeyPressEventArgs)
        If (e.KeyChar >= "a" And e.KeyChar <= "f") Then
            e.KeyChar = Chr(Asc(e.KeyChar) - 32)
        ElseIf Not _ValidChars.Contains(e.KeyChar) Then
            e.KeyChar = ""
            e.Handled = True
        End If

        MyBase.SelectionLength = 0

        MyBase.OnKeyPress(e)
    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)
        Dim NewValue = GetHexString()
        If MyBase.Text <> NewValue Then
            MyBase.Text = NewValue
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
        Dim Size = (MyBase.Mask.Length + 1) / 3 * 2
        Dim HexString = ""
        Dim TmpString = MyBase.Text
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
        Dim CurrentValue = MyBase.Text
        MyBase.Text = _UndoValue
        _UndoValue = CurrentValue
    End Sub

    Private Sub ButtonCopy_Click(sender As Object, e As EventArgs) Handles ButtonCopy.Click
        Clipboard.SetText(GetHexString)
    End Sub
End Class
