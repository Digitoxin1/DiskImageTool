Imports System.Runtime.InteropServices

Friend Class AdvancedListView
    Inherits ListView

    Public Event Scroll As EventHandler

    Public Sub ScrollToGroup(key As String, height As Integer)
        ScrollToGroup(Groups.Item(key), height)
    End Sub

    Public Sub ScrollToGroup(index As Integer, height As Integer)
        ScrollToGroup(Groups.Item(index), height)
    End Sub

    Private Sub ScrollToGroup(lvg As ListViewGroup, height As Integer)
        If lvg.Items.Count > 0 Then
            ScrollV(lvg.Items(0).Position.Y, height)
        End If
    End Sub

    Protected Sub ScrollV(scrollPos As Integer, height As Integer)
        scrollPos -= height

        Dim prevScrollPos As Integer = (Me.Items(0).Position.Y - height) * -1
        Dim WhereToScroll As Integer = prevScrollPos + scrollPos

        Call AdvancedListView.SendMessage(New HandleRef(Nothing, Me.Handle), ListViewMessages.LVM_SCROLL, CType(0, IntPtr), CType(scrollPos, IntPtr))

        prevScrollPos = (Me.Items(0).Position.Y - height) * -1

        If WhereToScroll <> prevScrollPos Then
            scrollPos = WhereToScroll - prevScrollPos
            Call AdvancedListView.SendMessage(New HandleRef(Nothing, Me.Handle), ListViewMessages.LVM_SCROLL, CType(0, IntPtr), CType(scrollPos, IntPtr))
        End If
    End Sub
    Protected Overridable Sub OnScroll()
        Dim handler As EventHandler = ScrollEvent
        If handler IsNot Nothing Then
            handler(Me, EventArgs.Empty)
        End If
    End Sub
    Protected Overrides Sub WndProc(ByRef m As Message)
        MyBase.WndProc(m)

        If m.Msg = &H115 Or m.Msg = &H20A Then
            OnScroll()
        ElseIf m.Msg = &H100 Then
            If m.WParam = 33 Or m.WParam = 34 Or m.WParam = 35 Or m.WParam = 36 Or m.WParam = 38 Or m.WParam = 40 Then
                OnScroll()
            End If
        End If
    End Sub

    Private Enum ListViewMessages As UInteger
        LVM_FIRST = &H1000
        LVM_SCROLL = LVM_FIRST + 20
    End Enum

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=False)>
    Private Shared Function SendMessage(ByVal hWnd As HandleRef, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function
End Class