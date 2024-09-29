Namespace WindowsAPI
    Public Enum FlashWindowFlags As UInteger
        ' Stop flashing. The system restores the window to its original state.
        FLASHW_STOP = 0
        ' Flash the window caption.
        FLASHW_CAPTION = 1
        ' Flash the taskbar button.
        FLASHW_TRAY = 2
        ' Flash both the window caption and taskbar button.
        ' This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
        FLASHW_ALL = 3
        ' Flash continuously, until the FLASHW_STOP flag is set.
        FLASHW_TIMER = 4
        ' Flash continuously until the window comes to the foreground.
        FLASHW_TIMERNOFG = 12
    End Enum
    Public Structure FLASHWINFO
        Public cbSize As UInteger
        Public hwnd As IntPtr
        Public dwFlags As FlashWindowFlags
        Public uCount As UInteger
        Public dwTimeout As UInteger
    End Structure
    Module WindowsAPI
        <Runtime.InteropServices.DllImport("User32.dll")>
        Private Function FlashWindowEx(ByRef fwInfo As FLASHWINFO) As Boolean
        End Function

        <Runtime.InteropServices.DllImport("User32.dll")>
        Public Function GetForegroundWindow() As Integer
        End Function

        <Runtime.InteropServices.DllImport("User32.dll")>
        Public Function SetForegroundWindow(hWnd As Integer) As Integer
        End Function

        Public Function FlashWindow(ByRef handle As IntPtr, FlashTitleBar As Boolean, FlashTray As Boolean, FlashCount As Integer, Continuous As Boolean) As Boolean
            If handle = Nothing Then
                Return False
            End If

            Try
                Dim fwi As New FLASHWINFO
                With fwi
                    .hwnd = handle
                    If FlashTitleBar Then
                        .dwFlags = .dwFlags Or FlashWindowFlags.FLASHW_CAPTION
                    End If
                    If FlashTray Then
                        .dwFlags = .dwFlags Or FlashWindowFlags.FLASHW_TRAY
                    End If
                    .uCount = CUInt(FlashCount)
                    If Continuous Then
                        .dwFlags = .dwFlags Or FlashWindowFlags.FLASHW_TIMERNOFG
                    End If
                    .dwTimeout = 0 ' Use the default cursor blink rate.
                    .cbSize = CUInt(System.Runtime.InteropServices.Marshal.SizeOf(fwi))
                End With

                Return FlashWindowEx(fwi)
            Catch
                Return False
            End Try
        End Function

        Public Function FlashWindowStop(ByRef handle As IntPtr) As Boolean
            If handle = Nothing Then
                Return False
            End If

            Try
                Dim fwi As New FLASHWINFO
                With fwi
                    .hwnd = handle
                    .dwFlags = .dwFlags Or FlashWindowFlags.FLASHW_STOP
                    .uCount = 0
                    .dwTimeout = 0 ' Use the default cursor blink rate.
                    .cbSize = CUInt(System.Runtime.InteropServices.Marshal.SizeOf(fwi))
                End With

                Return FlashWindowEx(fwi)
            Catch
                Return False
            End Try
        End Function
    End Module
End Namespace
