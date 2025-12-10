Imports System.ComponentModel
Imports DiskImageTool.DiskImage

Module FloppyDiskIO
    Private Const BYTES_PER_SECTOR As UShort = 512
    Public Function FloppyDiskRead(Owner As IWin32Window, Drive As FloppyDriveEnum) As String
        Dim FloppyDrive As New FloppyInterface
        Dim DriveLetter = FloppyInterface.GetDriveLetter(Drive)
        Dim DriveName = DriveLetter & ":\"
        Dim DriveInfo As New IO.DriveInfo(DriveName)
        Dim Result = DriveInfo.IsReady
        Dim FileName As String = ""

        If Result Then
            Result = FloppyDrive.OpenRead(Drive)
        End If

        If Result Then
            Dim BPB = FloppyDiskGetReadOptions(Owner, FloppyDrive)
            If BPB IsNot Nothing Then
                FileName = FloppyAccessForm.ReadDisk(FloppyDrive, BPB, Owner)
            End If
            FloppyDrive.Close()
        Else
            MsgBox(String.Format(My.Resources.Dialog_FloppyDriveNotReady, DriveLetter, Environment.NewLine), MsgBoxStyle.Exclamation)
        End If

        Return FileName
    End Function

    Public Function FloppyDiskNewImage(Buffer() As Byte, DiskFormat As FloppyDiskFormat) As String
        Dim FileName = GenerateOutputFile(".ima")

        If FileName = "" Then
            Return ""
        End If

        Dim Success As Boolean
        Try
            Dim FloppyImage As New BasicSectorImage(Buffer)
            Dim Disk As New DiskImage.Disk(FloppyImage, 0)
            Dim Response = SaveDiskImageToFile(Disk, FileName, False)
            Success = (Response = SaveImageResponse.Success)

        Catch ex As Exception
            DebugException(ex)
            Success = False
        End Try

        If Success Then
            Return FileName
        Else
            MsgBox(My.Resources.Dialog_SaveFileError2, MsgBoxStyle.Exclamation)
            Return ""
        End If
    End Function

    Public Sub FloppyDiskWrite(Owner As IWin32Window, Disk As Disk, Drive As FloppyDriveEnum)
        If Disk Is Nothing Then
            Exit Sub
        End If

        Dim FloppyDrive As New FloppyInterface
        Dim DriveLetter = FloppyInterface.GetDriveLetter(Drive)
        Dim DriveName = DriveLetter & ":\"
        Dim DriveInfo As New IO.DriveInfo(DriveName)
        Dim IsReady = DriveInfo.IsReady
        Dim NewDiskFormat = FloppyDiskFormatGet(Disk.BPB)
        Dim NewFormatName = String.Format(My.Resources.Label_Floppy, FloppyDiskFormatGetName(NewDiskFormat))
        Dim DetectedFormat As FloppyDiskFormat = 255
        Dim DoFormat = Not IsReady

        Dim MsgBoxResult As MsgBoxResult
        If IsReady Then
            Dim Result = FloppyDrive.OpenRead(Drive)
            If Result Then
                Dim Buffer(BYTES_PER_SECTOR - 1) As Byte
                Dim BytesRead = FloppyDrive.ReadSector(0, Buffer)
                If BytesRead = Buffer.Length Then
                    DetectedFormat = FloppyDiskFormatGet(Buffer)
                Else
                    DetectedFormat = FloppyDiskFormat.FloppyUnknown
                End If
                FloppyDrive.Close()
            Else
                DetectedFormat = FloppyDiskFormat.FloppyUnknown
            End If
            Dim Msg As String
            If DetectedFormat = NewDiskFormat Then
                Msg = String.Format(My.Resources.Dialog_DiskNotEmptyWarning, DriveLetter, Environment.NewLine)
            ElseIf DetectedFormat = -1 Then
                DoFormat = True
                Msg = String.Format(My.Resources.Dialog_DiskNotEmptyWarning_UnknownFormat, DriveLetter, Environment.NewLine, NewFormatName)
            Else
                DoFormat = True
                Dim DetectedFormatName = String.Format(My.Resources.Label_Floppy, FloppyDiskFormatGetName(DetectedFormat))
                Msg = String.Format(My.Resources.Dialog_DiskNotEmptyWarning_Mismatched, DriveLetter, DetectedFormatName, Environment.NewLine, NewFormatName)
            End If
            MsgBoxResult = MsgBox(Msg, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkCancel Or MsgBoxStyle.DefaultButton2)
        Else
            MsgBoxResult = MsgBoxResult.Ok
        End If

        If MsgBoxResult = MsgBoxResult.Ok Then
            Dim Result = FloppyDrive.OpenWrite(Drive)
            If Result Then
                Dim WriteOptions = FloppyWriteOptionsForm.Display(Owner, DoFormat, DetectedFormat, NewDiskFormat)
                If Not WriteOptions.Cancelled Then
                    FloppyAccessForm.WriteDisk(FloppyDrive, Disk.BPB, Disk.Image.GetBytes, WriteOptions.Format, WriteOptions.Verify, Owner)
                End If
                FloppyDrive.Close()
            Else
                MsgBox(My.Resources.Dialog_DiskWriteError, MsgBoxStyle.Exclamation)
            End If
        End If
    End Sub

    Private Function FloppyDiskGetReadOptions(Owner As IWin32Window, FloppyDrive As FloppyInterface) As BiosParameterBlock
        Dim DetectedFormat As FloppyDiskFormat
        Dim BootSector As BootSector = Nothing

        Dim Buffer(BYTES_PER_SECTOR - 1) As Byte
        Dim BytesRead = FloppyDrive.ReadSector(0, Buffer)
        If BytesRead = Buffer.Length Then
            BootSector = New BootSector(Buffer)
            DetectedFormat = FloppyDiskFormatGet(BootSector.BPB)
        Else
            DetectedFormat = FloppyDiskFormat.FloppyUnknown
        End If

        Dim Response = FloppyReadOptionsForm.Display(DetectedFormat, Owner)

        If Not Response.Result Then
            Return Nothing
        ElseIf Response.Format = -1 Then
            Return Nothing
        ElseIf BootSector IsNot Nothing AndAlso DetectedFormat = Response.Format Then
            Return BootSector.BPB
        Else
            Return BuildBPB(Response.Format)
        End If
    End Function
End Module