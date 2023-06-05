Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Module ListViewExtensions

    Private Const GWL_STYLE As Integer = -16
    Private Const HDM_FIRST As Integer = &H1200
    Private Const HDM_GETITEM As Integer = HDM_FIRST + 11
    Private Const HDM_SETITEM As Integer = HDM_FIRST + 12
    Private Const LVHT_EX_GROUP_HEADER As Integer = &H10000000
    Private Const LVM_FIRST As Integer = &H1000
    Private Const LVM_GETHEADER As Integer = LVM_FIRST + 31
    Private Const LVM_HITTEST As Integer = &H1000 + 18
    Private Const SB_HORZ As Integer = 0
    Private Const SB_VERT As Integer = 1
    Private Const WS_HSCROLL As Integer = &H100000

    <Extension()>
    Public Sub AddColumn(listViewControl As ListView, Name As String, Text As String, Width As Integer, Index As Integer)
        Dim Column As New ColumnHeader With {
            .Name = Name,
            .Width = Width,
            .Text = Text
        }
        listViewControl.Columns.Insert(Index, Column)
    End Sub

    <Extension()>
    Public Function AddItem(listViewControl As ListView, Text As String, Value As String, WrapText As Boolean) As ListViewItem
        Return AddItem(listViewControl, Nothing, Text, Value, Color.Empty, WrapText)
    End Function

    <Extension()>
    Public Function AddItem(listViewControl As ListView, Text As String, Value As String, ForeColor As Color) As ListViewItem
        Return AddItem(listViewControl, Nothing, Text, Value, ForeColor)
    End Function

    <Extension()>
    Public Function AddItem(listViewControl As ListView, Group As ListViewGroup, Text As String, Value As String) As ListViewItem
        Return AddItem(listViewControl, Group, Text, Value, Color.Empty)
    End Function

    <Extension()>
    Public Function AddItem(listViewControl As ListView, Group As ListViewGroup, Text As String, Value As String, ForeColor As Color) As ListViewItem
        Return AddItem(listViewControl, Group, Text, Value, ForeColor, True)
    End Function

    <Extension()>
    Public Function AddItem(listViewControl As ListView, Group As ListViewGroup, Text As String, Value As String, ForeColor As Color, WrapText As Boolean) As ListViewItem
        Dim Item As ListViewItem = Nothing
        Dim SubItem As ListViewItem.ListViewSubItem
        Dim StringList As List(Of String) = Nothing
        Dim AddTags As Boolean = False

        If WrapText Then
            Dim Width = listViewControl.Columns.Item(1).Width - 5
            If TextRenderer.MeasureText(Value, listViewControl.Font).Width > Width Then
                StringList = Value.WordWrap(Width, listViewControl.Font)
                AddTags = True
            End If
        End If

        If StringList Is Nothing Then
            StringList = New List(Of String) From {
                Value
            }
        End If

        For Counter = 0 To StringList.Count - 1
            Dim ItemText As String
            If Counter = 0 Then
                ItemText = Text
            Else
                ItemText = ""
            End If
            Dim NewItem = New ListViewItem(ItemText, Group) With {
                    .UseItemStyleForSubItems = False
                }
            If AddTags Then
                NewItem.Tag = Text
            End If
            SubItem = NewItem.SubItems.Add(StringList(Counter))
            SubItem.ForeColor = ForeColor
            If AddTags Then
                SubItem.Tag = Value
            End If
            listViewControl.Items.Add(NewItem)

            If Item Is Nothing Then
                Item = NewItem
            End If
        Next

        Return Item
    End Function
    <Extension()>
    Public Sub AutoResizeColumnsContstrained(listViewControl As ListView, headerAutoResize As ColumnHeaderAutoResizeStyle)
        listViewControl.AutoResizeColumns(headerAutoResize)

        If listViewControl.Columns.Count = 0 Then
            Exit Sub
        End If

        Dim LastColumnn = listViewControl.Columns(listViewControl.Columns.Count - 1)
        Dim TotalWidth As Integer = 0
        For Counter = 0 To listViewControl.Columns.Count - 2
            Dim Column = listViewControl.Columns(Counter)
            TotalWidth += Column.Width
        Next
        Dim ClientWidth = listViewControl.ClientSize.Width - SystemInformation.VerticalScrollBarWidth
        'If TotalWidth + LastColumnn.Width > listViewControl.ClientSize.Width Then
        LastColumnn.Width = ClientWidth - TotalWidth
        'End If
    End Sub

    <Extension()>
    Public Sub DoubleBuffer(listViewControl As ListView)
        listViewControl.GetType() _
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic) _
            .SetValue(listViewControl, True, Nothing)
    End Sub

    <Extension()>
    Public Function GetBottomIndex(ListViewControl As ListView) As Integer
        Dim BottomItem As ListViewItem = Nothing
        Dim LastPosY As Integer = 0
        Dim Bottom = ListViewControl.Bounds.Height

        If ListViewControl.IsHorizontalScrollBarVisible Then
            Bottom -= SystemInformation.HorizontalScrollBarHeight
        End If

        For Each Item As ListViewItem In ListViewControl.Items
            If Item.Position.Y > LastPosY Then
                LastPosY = Item.Position.Y
                If Item.Position.Y + Item.Bounds.Height < Bottom Then
                    BottomItem = Item
                End If
            End If
        Next

        If BottomItem Is Nothing Then
            Return -1
        Else
            Return BottomItem.Index
        End If
    End Function

    <Extension()>
    Public Function GetGroupAtPoint(ListViewControl As ListView, pt As Point) As ListViewGroup
        Dim Response As ListViewGroup = Nothing

        Dim ht = New LVHITTESTINFO() With {
            .pt_x = pt.X,
            .pt_y = pt.Y
        }
        Dim Id = SendMessage(ListViewControl.Handle, LVM_HITTEST, -1, ht)
        If Id <> -1 AndAlso (ht.flags And LVHT_EX_GROUP_HEADER) = 0 Then
            Id = -1
        End If

        If Id > -1 Then
            For Each Group As ListViewGroup In ListViewControl.Groups
                If Id = ExtractID(Group) Then
                    Response = Group
                    Exit For
                End If
            Next
        End If

        Return Response
    End Function

    <Extension()>
    Public Function GetVerticalScrollPos(ListViewControl As ListView) As Integer
        Return GetScrollPos(ListViewControl.Handle, SB_VERT)

    End Function

    <Extension()>
    Public Function IsHorizontalScrollBarVisible(ListViewControl As ListView) As Boolean
        Dim wndStyle = GetWindowLong(ListViewControl.Handle, GWL_STYLE)

        Return (wndStyle And WS_HSCROLL) <> 0
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

    Private Function ExtractID(Group As ListViewGroup) As Integer
        Try
            Return Group.GetType().GetProperty("ID", Reflection.BindingFlags.NonPublic Or Reflection.BindingFlags.Instance).GetValue(Group, New Object(-1) {})
        Catch ex As Exception
            Return -1
        End Try
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Function GetScrollPos(hWnd As IntPtr, nBar As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Function GetWindowLong(hWnd As IntPtr, nIndex As Integer) As Integer
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Function SendMessage(hWnd As IntPtr, msg As UInt32, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Function SendMessage(hWnd As IntPtr, msg As UInt32, wParam As IntPtr, ByRef lParam As HDITEM) As IntPtr
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As Integer, ByRef ht As LVHITTESTINFO) As Integer
    End Function

    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Function SetScrollPos(hWnd As IntPtr, nBar As Integer, nPos As Integer, bRedraw As Boolean) As Integer
    End Function
    <StructLayout(LayoutKind.Sequential)>
    Private Structure HDITEM
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

    <StructLayout(LayoutKind.Sequential)>
    Private Structure LVHITTESTINFO
        Public pt_x As Integer
        Public pt_y As Integer
        Public flags As Integer
        Public iItem As Integer
        Public iSubItem As Integer
        Public iGroup As Integer
    End Structure
End Module
