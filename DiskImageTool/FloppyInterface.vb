Imports System.Runtime.InteropServices
Imports Microsoft.Win32.SafeHandles

Public Enum FloppyDriveEnum
    FloppyDriveA
    FloppyDriveB
End Enum

Public Class FloppyInterface

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Private Shared Function CreateFile(lpFileName As String, dwDesiredAccess As Int32, dwShareMode As Int32, lpSecurityAttributes As IntPtr, dwCreationDisposition As Int32, dwFlagsAndAttributes As Int32, hTemplateFile As IntPtr) As SafeFileHandle
    End Function

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Private Shared Function SetFilePointer(hFile As SafeFileHandle, lDistanceToMove As Int32, lpDistanceToMoveHigh As IntPtr, dwMoveMethod As Integer) As Integer
    End Function

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Private Shared Function ReadFile(hFile As SafeFileHandle, lpBuffer As Byte(), nNumberOfBytesToRead As Int32, ByRef lpNumberOfBytesRead As Int32, lpOverlapped As IntPtr) As Int32
    End Function

    Private Const OPEN_EXISTING As Integer = 3
    Private Const FILE_ATTRIBUTE_NORMAL As Integer = &H80
    Private Const FILE_BEGIN As Integer = 0
    Private Const FILE_FLAG_NO_BUFFERING As Integer = &H20000000
    Private Const FILE_SHARE_READ As Integer = &H1
    Private Const GENERIC_READ As Integer = &H80000000
    Private Const GENERIC_WRITE As Integer = &H40000000
    Private Const SECTOR_SIZE As Integer = 512

    Private _DriveHandle As SafeFileHandle = Nothing

    Public Shared Function GetDriveLetter(Drive As FloppyDriveEnum) As String
        Select Case Drive
            Case FloppyDriveEnum.FloppyDriveA
                Return "A"
            Case FloppyDriveEnum.FloppyDriveB
                Return "B"
            Case Else
                Return ""
        End Select
    End Function

    Public Function Open(Drive As FloppyDriveEnum) As Boolean
        Dim DriveLetter As String = "A:"

        If Drive = FloppyDriveEnum.FloppyDriveA Then
            DriveLetter = "A:"
        ElseIf Drive = FloppyDriveEnum.FloppyDriveB Then
            DriveLetter = "B:"
        End If

        _DriveHandle = CreateFile("\\.\" & DriveLetter, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL Or FILE_FLAG_NO_BUFFERING, IntPtr.Zero)

        Return Not _DriveHandle.IsInvalid
    End Function

    Public Function ReadSector(Sector As Integer, ByRef Buffer() As Byte) As Integer
        Dim Offset As Integer = Sector * SECTOR_SIZE
        Dim BytesRead As Integer

        SetFilePointer(_DriveHandle, Offset, 0, 0)
        ReadFile(_DriveHandle, Buffer, Buffer.Length, BytesRead, IntPtr.Zero)

        Return BytesRead
    End Function


    Public Sub Close()
        If _DriveHandle IsNot Nothing Then
            _DriveHandle.Dispose()
            _DriveHandle = Nothing
        End If
    End Sub

    Protected Overrides Sub Finalize()
        Close()
        MyBase.Finalize()
    End Sub
End Class
