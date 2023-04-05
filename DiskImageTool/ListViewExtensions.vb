Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Module ListViewExtensions

    <StructLayout(LayoutKind.Sequential)>
    Public Structure HDITEM
        Public theMask As Mask
        Public cxy As Integer
        <MarshalAs(UnmanagedType.LPTStr)>
        Public pszText As String
        Public hbm As IntPtr
        Public cchTextMax As Integer
        Public fmt As Format
        Public lParam As IntPtr
        ' _WIN32_IE >= 0x0300 
        Public iImage As Integer
        Public iOrder As Integer
        ' _WIN32_IE >= 0x0500
        Public type As UInteger
        Public pvFilter As IntPtr
        ' _WIN32_WINNT >= 0x0600
        Public state As UInteger

        <Flags()>
        Public Enum Mask
            Format = &H4        ' HDI_FORMAT
        End Enum

        <Flags()>
        Public Enum Format
            SortDown = &H200    ' HDF_SORTDOWN
            SortUp = &H400      ' HDF_SORTUP
        End Enum
    End Structure

    Public Const LVM_FIRST As Integer = &H1000
    Public Const LVM_GETHEADER As Integer = LVM_FIRST + 31

    Public Const HDM_FIRST As Integer = &H1200
    Public Const HDM_GETITEM As Integer = HDM_FIRST + 11
    Public Const HDM_SETITEM As Integer = HDM_FIRST + 12

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Function SendMessage(hWnd As IntPtr, msg As UInt32, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Function SendMessage(hWnd As IntPtr, msg As UInt32, wParam As IntPtr, ByRef lParam As HDITEM) As IntPtr
    End Function

    <Extension()>
    Public Sub SetSortIcon(listViewControl As ListView, columnIndex As Integer, order As SortOrder)
        Dim columnHeader As IntPtr = SendMessage(listViewControl.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero)
        For columnNumber As Integer = 0 To listViewControl.Columns.Count - 1

            Dim columnPtr As New IntPtr(columnNumber)
            Dim item As New HDITEM With {
                .theMask = HDITEM.Mask.Format
            }

            If SendMessage(columnHeader, HDM_GETITEM, columnPtr, item) = IntPtr.Zero Then Throw New Win32Exception

            If order <> SortOrder.None AndAlso columnNumber = columnIndex Then
                Select Case order
                    Case SortOrder.Ascending
                        item.fmt = item.fmt And Not HDITEM.Format.SortDown
                        item.fmt = item.fmt Or HDITEM.Format.SortUp
                    Case SortOrder.Descending
                        item.fmt = item.fmt And Not HDITEM.Format.SortUp
                        item.fmt = item.fmt Or HDITEM.Format.SortDown
                End Select
            Else
                item.fmt = item.fmt And Not HDITEM.Format.SortDown And Not HDITEM.Format.SortUp
            End If

            If SendMessage(columnHeader, HDM_SETITEM, columnPtr, item) = IntPtr.Zero Then Throw New Win32Exception
        Next
    End Sub
End Module
