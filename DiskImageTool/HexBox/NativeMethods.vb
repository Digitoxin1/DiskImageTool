Imports System.Runtime.InteropServices

Namespace Hb.Windows.Forms
    Friend Module NativeMethods
        ' Key definitions
        Public Const WM_CHAR As Integer = &H102
        Public Const WM_KEYDOWN As Integer = &H100
        Public Const WM_KEYUP As Integer = &H101

        ' Caret definitions
        <DllImport("user32.dll", SetLastError:=True)>
        Public Function CreateCaret(hWnd As IntPtr, hBitmap As IntPtr, nWidth As Integer, nHeight As Integer) As Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Function DestroyCaret() As Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Function SetCaretPos(X As Integer, Y As Integer) As Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Public Function ShowCaret(hWnd As IntPtr) As Boolean
        End Function
    End Module
End Namespace