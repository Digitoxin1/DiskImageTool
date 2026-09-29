Imports DiskImageTool.DiskImage

Module FloppyDiskIO
    Private Const BYTES_PER_SECTOR As UShort = 512

    Public Function FloppyDiskRead(Drive As FloppyDriveEnum) As String
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
            Dim BPB = FloppyDiskGetReadOptions(FloppyDrive)
            If BPB IsNot Nothing Then
                FileName = FloppyAccessForm.ReadDisk(FloppyDrive, BPB)
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

    Public Sub FloppyDiskWrite(Disk As Disk, Drive As FloppyDriveEnum)
        If Disk Is Nothing Then
            Exit Sub
        End If

        Dim NewDiskFormat = Disk.DiskParams.Format

        If Not FloppyDiskFormatIsStandard(NewDiskFormat) Then
            Dim NewFormatName = String.Format(My.Resources.Label_Floppy, FloppyDiskFormatGetName(NewDiskFormat))

            MsgBox(String.Format(My.Resources.Dialog_FloppyNonstandardFormat, NewFormatName, Environment.NewLine), MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        Dim FloppyDrive As New FloppyInterface

        Dim WriteOptions = FloppyWriteOptionsForm.Display(FloppyDrive, Drive, NewDiskFormat)

        If Not WriteOptions.Cancelled Then
            If FloppyDrive.IsOpen Then
                Dim BPB = BuildBPB(NewDiskFormat)
                FloppyAccessForm.WriteDisk(FloppyDrive, BPB, Disk.Image.GetBytes, WriteOptions.Format, WriteOptions.Verify)
            End If
        End If

        If FloppyDrive.IsOpen Then
            FloppyDrive.Close()
        End If
    End Sub

    Private Function FloppyDiskGetReadOptions(FloppyDrive As FloppyInterface) As BiosParameterBlock
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

        Dim Response = FloppyReadOptionsForm.Display(DetectedFormat)

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