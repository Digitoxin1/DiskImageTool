Imports System.ComponentModel
Imports System.Runtime.InteropServices

Public Class PlaceholderTextBox
    Inherits TextBox

    Private _placeholderText As String = ""
    Private _showCueWhenFocused As Boolean = False

    Private Const EM_SETCUEBANNER As Integer = &H1501

    <DllImport("user32.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function SendMessage(
        hWnd As IntPtr,
        msg As Integer,
        wParam As IntPtr,
        lParam As String
    ) As IntPtr
    End Function

    <Category("Appearance")>
    <Description("The placeholder text shown when the textbox is empty.")>
    Public Property PlaceholderText As String
        Get
            Return _placeholderText
        End Get
        Set(value As String)
            If value Is Nothing Then value = ""
            If _placeholderText <> value Then
                _placeholderText = value
                UpdateCueBanner()
            End If
        End Set
    End Property

    <Category("Behavior")>
    <DefaultValue(False)>
    <Description("If True, the placeholder remains visible while the textbox has focus until the user types.")>
    Public Property ShowCueWhenFocused As Boolean
        Get
            Return _showCueWhenFocused
        End Get
        Set(value As Boolean)
            If _showCueWhenFocused <> value Then
                _showCueWhenFocused = value
                UpdateCueBanner()
            End If
        End Set
    End Property

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        UpdateCueBanner()
    End Sub

    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)
        UpdateCueBanner()
    End Sub

    Protected Overrides Sub OnFontChanged(e As EventArgs)
        MyBase.OnFontChanged(e)
        UpdateCueBanner()
    End Sub

    Private Sub UpdateCueBanner()
        If IsHandleCreated Then
            SendMessage(Handle, EM_SETCUEBANNER, If(_showCueWhenFocused, CType(1, IntPtr), IntPtr.Zero), _placeholderText)
        End If
    End Sub

End Class