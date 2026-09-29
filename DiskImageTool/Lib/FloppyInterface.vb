Imports System.Runtime.InteropServices
Imports Microsoft.Win32.SafeHandles

Public Enum FloppyDriveEnum
    FloppyDriveA
    FloppyDriveB
End Enum

Public Enum FloppyMediaState
    NoDisk
    Blank
    Formatted
    DriveUnavailable
End Enum

Public Structure FloppyMediaInfo
    Public State As FloppyMediaState
    Public BootSector() As Byte
End Structure

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

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function ReadFile(hFile As SafeFileHandle, lpBuffer As IntPtr, nNumberOfBytesToRead As Int32, ByRef lpNumberOfBytesRead As Int32, lpOverlapped As IntPtr) As Boolean
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

    Private Const ERROR_ACCESS_DENIED As Integer = 5
    Private Const ERROR_CRC As Integer = 23
    Private Const ERROR_DISK_CORRUPT As Integer = 1393
    Private Const ERROR_GEN_FAILURE As Integer = 31
    Private Const ERROR_INVALID_FUNCTION As Integer = 1
    Private Const ERROR_INVALID_PARAMETER As Integer = 87
    Private Const ERROR_MEDIA_CHANGED As Integer = 1110
    Private Const ERROR_NOT_READY As Integer = 21
    Private Const ERROR_NOT_SUPPORTED As Integer = 50
    Private Const ERROR_NO_MEDIA_IN_DRIVE As Integer = 1112
    Private Const ERROR_SECTOR_NOT_FOUND As Integer = 27
    Private Const ERROR_SHARING_VIOLATION As Integer = 32
    Private Const ERROR_UNRECOGNIZED_VOLUME As Integer = 1005
    Private Const ERROR_WRONG_DISK As Integer = 34
    Private Const FILE_ANY_ACCESS As Integer = 0
    Private Const FILE_ATTRIBUTE_NORMAL As Integer = &H80
    Private Const FILE_DEVICE_FILE_SYSTEM As Integer = &H9
    Private Const FILE_DEVICE_MASS_STORAGE As Integer = &H2D
    Private Const FILE_FLAG_NO_BUFFERING As Integer = &H20000000
    Private Const FILE_READ_ACCESS As Integer = 1
    Private Const FILE_SHARE_READ As Integer = &H1
    Private Const FILE_SHARE_WRITE As Integer = &H2
    Private Const FILE_WRITE_ACCESS As Integer = 2
    Private Const GENERIC_READ As Integer = &H80000000
    Private Const GENERIC_WRITE As Integer = &H40000000
    Private Const IOCTL_DISK_BASE As Integer = &H7
    Private Const METHOD_BUFFERED As Integer = 0
    Private Const OPEN_EXISTING As Integer = 3
    Private Const SECTOR_SIZE As Integer = 512

    Private _DriveHandle As SafeFileHandle = Nothing
    Private _VolumeLocked As Boolean = False

    Public ReadOnly Property IsOpen As Boolean
        Get
            Return _DriveHandle IsNot Nothing AndAlso Not _DriveHandle.IsInvalid AndAlso Not _DriveHandle.IsClosed
        End Get
    End Property

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

    Public Function ProbeMedia() As FloppyMediaInfo
        Dim Info As New FloppyMediaInfo With {
            .State = FloppyMediaState.DriveUnavailable,
            .BootSector = Nothing
        }

        Try
            ' Read first. On an unformatted disk the verify ioctl and the sector read each wait out
            ' the driver's full retry timeout, so only call verify when the read result is ambiguous.
            Dim Buffer(SECTOR_SIZE - 1) As Byte
            Dim ReadError = ReadBootSector(_DriveHandle, Buffer)

            If ReadError = ERROR_MEDIA_CHANGED Then
                ReadError = ReadBootSector(_DriveHandle, Buffer)
            End If

            If ReadError = 0 Then
                Info.State = FloppyMediaState.Formatted
                Info.BootSector = Buffer
            ElseIf IsUnformattedError(ReadError) Then
                Info.State = FloppyMediaState.Blank
            ElseIf IsNoMediaError(ReadError) Then
                Info.State = FloppyMediaState.NoDisk
            ElseIf ReadError = ERROR_ACCESS_DENIED OrElse ReadError = ERROR_SHARING_VIOLATION OrElse ReadError = ERROR_INVALID_PARAMETER Then
                Info.State = FloppyMediaState.DriveUnavailable
            Else
                Dim VerifyError = TryCheckVerify(_DriveHandle)

                If IsNoMediaError(VerifyError) Then
                    Info.State = FloppyMediaState.NoDisk
                Else
                    Info.State = FloppyMediaState.Blank
                End If
            End If
        Catch
        End Try

        Return Info
    End Function

    Public Function FormatTrack(MediaType As MEDIA_TYPE, Track As Integer, Head As Integer) As Boolean
        Dim IOCTL_DISK_FORMAT_TRACKS As Integer = CTL_CODE(IOCTL_DISK_BASE, &H6, METHOD_BUFFERED, FILE_READ_ACCESS Or FILE_WRITE_ACCESS)

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
        Dim DriveLetter = GetDriveLetter(Drive) & ":"

        _DriveHandle = CreateFile("\\.\" & DriveLetter, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL Or FILE_FLAG_NO_BUFFERING, IntPtr.Zero)

        If _DriveHandle.IsInvalid Then
            _DriveHandle.Dispose()
            _DriveHandle = Nothing
            Return False
        End If

        Return True
    End Function

    Public Function OpenWrite(Drive As FloppyDriveEnum) As Boolean
        Dim DriveLetter = GetDriveLetter(Drive) & ":"

        _DriveHandle = CreateFile("\\.\" & DriveLetter, GENERIC_READ Or GENERIC_WRITE, FILE_SHARE_READ Or FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL Or FILE_FLAG_NO_BUFFERING, IntPtr.Zero)

        If _DriveHandle.IsInvalid Then
            _DriveHandle.Dispose()
            _DriveHandle = Nothing
            Return False
        End If

        LockAndDismountVolume()
        Return True
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
            If _VolumeLocked AndAlso Not _DriveHandle.IsInvalid AndAlso Not _DriveHandle.IsClosed Then
                Dim FSCTL_UNLOCK_VOLUME As Integer = CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 7, METHOD_BUFFERED, FILE_ANY_ACCESS)
                Dim BytesReturned As Integer
                DeviceIoControl(_DriveHandle, FSCTL_UNLOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, BytesReturned, IntPtr.Zero)
                _VolumeLocked = False
            End If
            _DriveHandle.Dispose()
            _DriveHandle = Nothing
        End If
    End Sub

    Private Sub LockAndDismountVolume()
        Dim FSCTL_LOCK_VOLUME As Integer = CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 6, METHOD_BUFFERED, FILE_ANY_ACCESS)
        Dim FSCTL_DISMOUNT_VOLUME As Integer = CTL_CODE(FILE_DEVICE_FILE_SYSTEM, 8, METHOD_BUFFERED, FILE_ANY_ACCESS)
        Dim BytesReturned As Integer

        ' A successful dismount locks the volume on this handle. Lock only if the dismount did not.
        _VolumeLocked = DeviceIoControl(_DriveHandle, FSCTL_DISMOUNT_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, BytesReturned, IntPtr.Zero)

        If Not _VolumeLocked Then
            _VolumeLocked = DeviceIoControl(_DriveHandle, FSCTL_LOCK_VOLUME, IntPtr.Zero, 0, IntPtr.Zero, 0, BytesReturned, IntPtr.Zero)
        End If
    End Sub

    Private Shared Function IsNoMediaError(Win32Error As Integer) As Boolean
        Return Win32Error = ERROR_NOT_READY OrElse Win32Error = ERROR_NO_MEDIA_IN_DRIVE
    End Function

    Private Shared Function IsUnformattedError(Win32Error As Integer) As Boolean
        Select Case Win32Error
            Case ERROR_CRC, ERROR_SECTOR_NOT_FOUND, ERROR_WRONG_DISK, ERROR_UNRECOGNIZED_VOLUME, ERROR_DISK_CORRUPT
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Shared Function ReadBootSector(Handle As SafeFileHandle, Buffer() As Byte) As Integer
        ' FILE_FLAG_NO_BUFFERING rejects buffers that are not sector aligned.
        ' A managed array often fails that check with ERROR_INVALID_PARAMETER before the driver looks for a disk.
        Dim Raw As IntPtr = Marshal.AllocHGlobal(SECTOR_SIZE * 2)

        Try
            Dim Address = Raw.ToInt64()
            Dim Mask = CLng(SECTOR_SIZE - 1)
            Dim Aligned = New IntPtr((Address + Mask) And Not Mask)
            Dim BytesRead As Integer
            Dim ReadOk = ReadFile(Handle, Aligned, SECTOR_SIZE, BytesRead, IntPtr.Zero)
            Dim ReadError = Marshal.GetLastWin32Error()

            If ReadOk AndAlso BytesRead = SECTOR_SIZE Then
                Marshal.Copy(Aligned, Buffer, 0, SECTOR_SIZE)
                Return 0
            End If

            If ReadError = 0 Then
                Return ERROR_GEN_FAILURE
            End If

            Return ReadError
        Finally
            Marshal.FreeHGlobal(Raw)
        End Try
    End Function

    Private Shared Function RunCheckVerify(Handle As SafeFileHandle, Ioctl As Integer, ByRef Win32Error As Integer) As Boolean
        Dim BytesReturned As Integer
        Dim MediaChanges As IntPtr = Marshal.AllocHGlobal(4)
        Try
            Dim Ok = DeviceIoControl(Handle, Ioctl, IntPtr.Zero, 0, MediaChanges, 4, BytesReturned, IntPtr.Zero)

            If Ok Then
                Win32Error = 0
                Return True
            End If

            Win32Error = Marshal.GetLastWin32Error()

            If Win32Error <> ERROR_MEDIA_CHANGED Then
                Return False
            End If

            Ok = DeviceIoControl(Handle, Ioctl, IntPtr.Zero, 0, MediaChanges, 4, BytesReturned, IntPtr.Zero)

            If Ok Then
                Win32Error = 0
                Return True
            End If

            Win32Error = Marshal.GetLastWin32Error()

            Return False
        Finally
            Marshal.FreeHGlobal(MediaChanges)
        End Try
    End Function

    Private Shared Function TryCheckVerify(Handle As SafeFileHandle) As Integer
        Dim IOCTL_STORAGE_CHECK_VERIFY2 As Integer = CTL_CODE(FILE_DEVICE_MASS_STORAGE, &H200, METHOD_BUFFERED, FILE_ANY_ACCESS)
        Dim IOCTL_DISK_CHECK_VERIFY As Integer = CTL_CODE(IOCTL_DISK_BASE, &H200, METHOD_BUFFERED, FILE_READ_ACCESS)
        Dim Win32Error As Integer

        If RunCheckVerify(Handle, IOCTL_STORAGE_CHECK_VERIFY2, Win32Error) Then
            Return Win32Error
        End If

        If Win32Error = ERROR_INVALID_FUNCTION OrElse Win32Error = ERROR_NOT_SUPPORTED Then
            RunCheckVerify(Handle, IOCTL_DISK_CHECK_VERIFY, Win32Error)
        End If

        Return Win32Error
    End Function

    Private Shared Function CTL_CODE(DeviceType As UInteger, FunctionCode As UInteger, Method As UInteger, Access As UInteger) As UInteger
        Return (DeviceType << 16) Or (Access << 14) Or (FunctionCode << 2) Or Method
    End Function

    Protected Overrides Sub Finalize()
        Close()
        MyBase.Finalize()
    End Sub
End Class
