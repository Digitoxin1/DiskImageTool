﻿Imports System.IO
Imports System.Runtime.InteropServices

Module Utility
    Public Function DuplicateHashTable(Table As Hashtable) As Hashtable
        Dim NewTable As New Hashtable
        For Each Key In Table.Keys
            NewTable.Item(Key) = Table.Item(Key)
        Next

        Return NewTable
    End Function

    Public Function FileDialogAppendFilter(FileFilter As String, Description As String, Extension As String) As String
        Return FileFilter & IIf(FileFilter = "", "", "|") & FileDialogGetFilter(Description, Extension)
    End Function

    Public Function FileDialogAppendFilter(FileFilter As String, Description As String, ExtensionList As List(Of String)) As String
        Return FileFilter & IIf(FileFilter = "", "", "|") & FileDialogGetFilter(Description, ExtensionList)
    End Function

    Public Function FileDialogGetFilter(Description As String, Extension As String) As String
        Return Description & " (*" & Extension & ")|" & "*" & Extension
    End Function

    Public Function FileDialogGetFilter(Description As String, ExtensionList As List(Of String)) As String
        Dim Extensions = ExtensionList.ToArray

        For Counter = 0 To Extensions.Length - 1
            Extensions(Counter) = "*" & Extensions(Counter)
        Next

        Return Description & " (" & String.Join("; ", Extensions) & ")|" & String.Join(";", Extensions)
    End Function

    Public Function GetDownloadsFolder() As String

        Dim Result As String = ""
        Dim ppszPath As IntPtr
        Dim rfid = New Guid("{374DE290-123F-4565-9164-39C4925E467B}")

        If SHGetKnownFolderPath(rfid, 0, 0, ppszPath) = 0 Then
            Result = Marshal.PtrToStringUni(ppszPath)
            Marshal.FreeCoTaskMem(ppszPath)
        End If

        Return Result
    End Function

    Public Function GetVersionString() As String
        Dim Version = FileVersionInfo.GetVersionInfo(Application.ExecutablePath)
        Return Version.FileMajorPart & "." & Version.FileMinorPart & "." & Version.FilePrivatePart
    End Function

    Public Function HexStringToBytes(ByVal HexString As String) As Byte()
        Dim b(HexString.Length / 2 - 1) As Byte

        For i As Integer = 0 To HexString.Length - 1 Step 2
            b(i / 2) = Convert.ToByte(HexString.Substring(i, 2), 16)
        Next

        Return b
    End Function

    Public Function IsFileReadOnly(fileName As String) As Boolean
        Dim fInfo As New FileInfo(fileName)
        Return fInfo.IsReadOnly
    End Function

    Public Function PathAddBackslash(Path As String) As String
        If Len(Path) > 0 Then
            If Not Path.EndsWith("\") Then
                Path &= "\"
            End If
        End If
        Return Path
    End Function

    <DllImport("shell32.dll")>
    Private Function SHGetKnownFolderPath(<MarshalAs(UnmanagedType.LPStruct)> ByVal rfid As Guid,
        ByVal dwFlags As UInt32,
        ByVal hToken As IntPtr,
        ByRef pszPath As IntPtr) As Int32
    End Function
End Module