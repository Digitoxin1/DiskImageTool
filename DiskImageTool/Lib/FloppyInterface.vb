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
    Private Shared Function DeviceIoControl(hFile As SafeHandle, dwIoControlCode As Int32, lpInBuffer As IntPtr, nInBufferSize As Int32, lpOutBuffer As IntPtr, nOutBufferSize As Int32, ByRef lpBytesReturned As Int32, lpOverlapped As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Private Shared Function ReadFile(hFile As SafeFileHandle, lpBuffer As Byte(), nNumberOfBytesToRead As Int32, ByRef lpNumberOfBytesRead As Int32, lpOverlapped As IntPtr) As Int32
    End Function

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Private Shared Function SetFilePointer(hFile As SafeFileHandle, lDistanceToMove As Int32, lpDistanceToMoveHigh As IntPtr, dwMoveMethod As Integer) As Integer
    End Function

    <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Ansi)>
    Private Shared Function WriteFile(hFile As SafeHandle, lpBuffer As Byte(), nNumberOfBytesToWrite As Int32, ByRef lpNumberOfBytesWritten As Int32, lpOverlapped As IntPtr) As Int32
    End Function

    Public Enum MEDIA_TYPE
        Unknown = 0
        F5_1Pt2_512
        F3_1Pt44_512
        F3_2Pt88_512
        F3_20Pt8_512
        F3_720_512
        F5_360_512
        F5_320_512
        F5_320_1024
        F5_180_512
        F5_160_512
        RemovableMedia
        FixedMedia
        F3_120M_512
        F3_640_512
        F5_640_512
        F5_720_512
        F3_1Pt2_512
        F3_1Pt23_1024
        F5_1Pt23_1024
        F3_128Mb_512
        F3_230Mb_512
        F8_256_128
        F3_200Mb_512
        F3_240M_512
        F3_32M_512
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Private Structure FORMAT_PARAMETERS
        Dim MediaType As MEDIA_TYPE
        Dim StartCylinderNumber As Integer
        Dim EndCylinderNumber As Integer
        Dim StartHeadNumber As Integer
        Dim EndHeadNumber As Integer
    End Structure

    Private Const FILE_ATTRIBUTE_NORMAL As Integer = &H80
    Private Const FILE_FLAG_NO_BUFFERING As Integer = &H20000000
    Private Const FILE_READ_ACCESS As Integer = 1
    Private Const FILE_SHARE_READ As Integer = &H1
    Private Const FILE_WRITE_ACCESS As Integer = 2
    Private Const GENERIC_READ As Integer = &H80000000
    Private Const GENERIC_WRITE As Integer = &H40000000
    Private Const IOCTL_DISK_BASE As Integer = &H7
    Private Const METHOD_BUFFERED As Integer = 0
    Private Const OPEN_EXISTING As Integer = 3
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

    Public Function FormatTrack(MediaType As MEDIA_TYPE, Track As Integer, Head As Integer) As Boolean
        Dim IOCTL_DISK_FORMAT_TRACKS As Integer = CTL_CODE(IOCTL_DISK_BASE, 6, METHOD_BUFFERED, FILE_READ_ACCESS Or FILE_WRITE_ACCESS)
        Dim lpBytesReturned As Integer
        Dim Params As FORMAT_PARAMETERS
        Params.MediaType = MediaType
        Params.StartCylinderNumber = Track
        Params.EndCylinderNumber = Track
        Params.StartHeadNumber = Head
        Params.EndHeadNumber = Head

        Dim buffer As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(Params))
        Marshal.StructureToPtr(Params, buffer, False)
        Dim Result = DeviceIoControl(_DriveHandle, IOCTL_DISK_FORMAT_TRACKS, buffer, Marshal.SizeOf(Params), IntPtr.Zero, 0, lpBytesReturned, IntPtr.Zero)
        Marshal.FreeHGlobal(buffer)

        Return Result
    End Function

    Public Function OpenRead(Drive As FloppyDriveEnum) As Boolean
        Dim DriveLetter = FloppyInterface.GetDriveLetter(Drive) & ":"

        _DriveHandle = CreateFile("\\.\" & DriveLetter, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL Or FILE_FLAG_NO_BUFFERING, IntPtr.Zero)

        Return Not _DriveHandle.IsInvalid
    End Function

    Public Function OpenWrite(Drive As FloppyDriveEnum) As Boolean
        Dim DriveLetter = FloppyInterface.GetDriveLetter(Drive) & ":"

        _DriveHandle = CreateFile("\\.\" & DriveLetter, GENERIC_READ Or GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL Or FILE_FLAG_NO_BUFFERING, IntPtr.Zero)

        Return Not _DriveHandle.IsInvalid
    End Function

    Public Function ReadSector(Sector As Integer, ByRef Buffer() As Byte) As Integer
        Dim Offset As Integer = Sector * SECTOR_SIZE
        Dim BytesRead As Integer

        SetFilePointer(_DriveHandle, Offset, 0, 0)
        ReadFile(_DriveHandle, Buffer, Buffer.Length, BytesRead, IntPtr.Zero)

        Return BytesRead
    End Function

    Public Function WriteSector(Sector As Integer, Buffer() As Byte) As Integer
        Dim Offset As Integer = Sector * SECTOR_SIZE
        Dim BytesWritten As Integer

        SetFilePointer(_DriveHandle, Offset, 0, 0)
        WriteFile(_DriveHandle, Buffer, Buffer.Length, BytesWritten, IntPtr.Zero)

        Return BytesWritten
    End Function

    Public Sub Close()
        If _DriveHandle IsNot Nothing Then
            _DriveHandle.Dispose()
            _DriveHandle = Nothing
        End If
    End Sub

    Private Function CTL_CODE(DeviceType As UInteger, FunctionCode As UInteger, Method As UInteger, Access As UInteger) As UInteger
        Return (DeviceType << 16) Or (Access << 14) Or (FunctionCode << 2) Or Method
    End Function

    Protected Overrides Sub Finalize()
        Close()
        MyBase.Finalize()
    End Sub
End Class
