Imports System.Runtime.InteropServices

Public Class ReadDiskDestination
    <DllImport("user32.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As String) As IntPtr
    End Function

    Private Const EM_REPLACESEL As Integer = &HC2

    Private Sub ReadDiskDestinationFlux_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        If Me.Height <> 153 Then
            Me.Height = 153
        End If
    End Sub

    Public Sub ToggleView(DisplayFlux As Boolean)
        GroupBoxDestination.SuspendLayout()
        TableLayoutPanelDestinationFlux.Visible = DisplayFlux
        TableLayoutPanelDestinationImage.Visible = Not DisplayFlux
        GroupBoxDestination.ResumeLayout()
    End Sub

    Private Sub TextBoxFolderName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFolderName.TextChanged
        ButtonFolderNameClear.Enabled = Not String.IsNullOrEmpty(TextBoxFolderName.Text)
    End Sub

    Private Sub ButtonFolderNameClear_Click(sender As Object, e As EventArgs) Handles ButtonFolderNameClear.Click
        TextBoxFolderName.SelectAll()
        SendMessage(TextBoxFolderName.Handle, EM_REPLACESEL, New IntPtr(1), "")
        TextBoxFolderName.Focus()
    End Sub

    Private Sub TextBoxFileName_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFileName.TextChanged
        ButtonFileNameClear.Enabled = Not String.IsNullOrEmpty(TextBoxFileName.Text)
    End Sub

    Private Sub ButtonFileNameClear_Click(sender As Object, e As EventArgs) Handles ButtonFileNameClear.Click
        TextBoxFileName.SelectAll()
        SendMessage(TextBoxFileName.Handle, EM_REPLACESEL, New IntPtr(1), "")
        TextBoxFileName.Focus()
    End Sub
End Class
