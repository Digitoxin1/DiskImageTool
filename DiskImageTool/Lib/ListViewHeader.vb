Public Class ListViewHeader
    Inherits System.Windows.Forms.NativeWindow

    Private Declare Function GetWindow Lib "user32" Alias "GetWindow" (hwnd As IntPtr, wCmd As Integer) As IntPtr
    Private Const GW_CHILD As Integer = 5
    Private ReadOnly ptrHWnd As IntPtr

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        Select Case m.Msg
            Case Is = &H20  ' WM_SETCURSOR
                m.Msg = 0
        End Select

        MyBase.WndProc(m)
    End Sub

    Protected Overrides Sub Finalize()
        Me.ReleaseHandle()
        MyBase.Finalize()
    End Sub

    Public Sub New(Handle As IntPtr)
        ptrHWnd = GetWindow(Handle, GW_CHILD)
        Me.AssignHandle(ptrHWnd)
    End Sub
End Class
