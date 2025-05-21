Imports System.ComponentModel
Imports DiskImageTool.DiskImage

Module FloppyDiskIO
    Private Const BYTES_PER_SECTOR As UShort = 512
    Public Function FloppyDiskRead(Owner As IWin32Window, Drive As FloppyDriveEnum, LoadedFileNames As Dictionary(Of String, ImageData)) As String
        Dim FloppyDrive = New FloppyInterface
        Dim DriveLetter = FloppyInterface.GetDriveLetter(Drive)
        Dim DriveName = DriveLetter & ":\"
        Dim DriveInfo = New IO.DriveInfo(DriveName)
        Dim Result = DriveInfo.IsReady
        Dim FileName As String = ""

        If Result Then
            Result = FloppyDrive.OpenRead(Drive)
        End If

        If Result Then
            Dim BPB = FloppyDiskGetReadOptions(Owner, FloppyDrive)
            If BPB IsNot Nothing Then
                Dim FloppyAccessForm As New FloppyAccessForm(FloppyDrive, BPB, FloppyAccessForm.FloppyAccessType.Read) With {
                    .LoadedFileNames = LoadedFileNames
                }
                FloppyAccessForm.ShowDialog(Owner)
                FileName = FloppyAccessForm.FileName
                FloppyAccessForm.Close()
            End If
            FloppyDrive.Close()
        Else
            MsgBox($"Floppy drive {DriveLetter} is not ready.{vbCrLf}{vbCrLf}Please verify that a formatted disk is inserted into drive {DriveLetter}.", MsgBoxStyle.Exclamation)
        End If

        Return FileName
    End Function

    Public Function FloppyDiskSaveFile(Buffer() As Byte, DiskFormat As FloppyDiskFormat, LoadedFileNames As Dictionary(Of String, ImageData)) As String
        Dim FileExt = ".ima"
        Dim FileFilter = GetSaveDialogFilters(DiskFormat, FloppyImageType.BasicSectorImage, FileExt)

        Dim Dialog = New SaveFileDialog With {
            .Filter = FileFilter.Filter,
            .FilterIndex = FileFilter.FilterIndex,
            .DefaultExt = FileExt
        }

        AddHandler Dialog.FileOk, Sub(sender As Object, e As CancelEventArgs)
                                      If LoadedFileNames.ContainsKey(Dialog.FileName) Then
                                          Dim Msg As String = IO.Path.GetFileName(Dialog.FileName) &
                                            $"{vbCrLf}{vbCrLf}This file is currently open in {Application.ProductName}. " &
                                            $"Try again with a different file name."
                                          MsgBox(Msg, MsgBoxStyle.Exclamation, "Save As")
                                          e.Cancel = True
                                      End If
                                  End Sub

        If Dialog.ShowDialog = DialogResult.OK Then
            Dim Success As Boolean
            Try
                Dim FloppyImage = New BasicSectorImage(Buffer)
                Dim Disk = New DiskImage.Disk(FloppyImage, 0)
                Dim Response = SaveDiskImageToFile(Disk, Dialog.FileName)
                Success = (Response = SaveImageResponse.Success)
            Catch ex As Exception
                DebugException(ex)
                Success = False
            End Try

            If Success Then
                Return Dialog.FileName
            Else
                MsgBox("An error has occurred while attempting to save the file.", MsgBoxStyle.Exclamation)
                Return ""
            End If
        Else
            Return ""
        End If
    End Function

    Public Sub FloppyDiskWrite(Owner As IWin32Window, Disk As Disk, Drive As FloppyDriveEnum)
        If Disk Is Nothing Then
            Exit Sub
        End If

        Dim FloppyDrive = New FloppyInterface
        Dim DriveLetter = FloppyInterface.GetDriveLetter(Drive)
        Dim DriveName = DriveLetter & ":\"
        Dim DriveInfo = New IO.DriveInfo(DriveName)
        Dim IsReady = DriveInfo.IsReady
        Dim NewDiskFormat = GetFloppyDiskFormat(Disk.BPB, False)
        Dim NewFormatName = GetFloppyDiskFormatName(NewDiskFormat) & " Floppy"
        Dim DetectedFormat As FloppyDiskFormat = 255
        Dim DoFormat = Not IsReady

        Dim MsgBoxResult As MsgBoxResult
        If IsReady Then
            Dim Result = FloppyDrive.OpenRead(Drive)
            If Result Then
                Dim Buffer(BYTES_PER_SECTOR - 1) As Byte
                Dim BytesRead = FloppyDrive.ReadSector(0, Buffer)
                If BytesRead = Buffer.Length Then
                    Dim BootSector = New BootSector(Buffer)
                    DetectedFormat = GetFloppyDiskFormat(BootSector.BPB, False)
                Else
                    DetectedFormat = FloppyDiskFormat.FloppyUnknown
                End If
                FloppyDrive.Close()
            Else
                DetectedFormat = FloppyDiskFormat.FloppyUnknown
            End If
            Dim Msg As String
            If DetectedFormat = NewDiskFormat Then
                Msg = $"Warning: The disk in drive {DriveLetter} is not empty.{vbCrLf}{vbCrLf}If you continue, the disk will be overwritten.{vbCrLf}{vbCrLf}Do you wish to continue?"
            ElseIf DetectedFormat = -1 Then
                DoFormat = True
                Msg = $"Warning: The disk in drive {DriveLetter} is not empty, but I am unable to determine the format type.{vbCrLf}{vbCrLf}The image you are attempting to write is a {NewFormatName} image.{vbCrLf}{vbCrLf}If you continue, the disk will be overwritten and may be unreadable.{vbCrLf}{vbCrLf}Do you wish to continue?"
            Else
                DoFormat = True
                Dim DetectedFormatName = GetFloppyDiskFormatName(DetectedFormat) & " Floppy"
                Msg = $"Warning: The disk in drive {DriveLetter} is not empty and is formatted as a {DetectedFormatName}.{vbCrLf}{vbCrLf}The image you are attempting to write is a {NewFormatName} image.{vbCrLf}{vbCrLf}If you continue, the disk will be overwritten and may be unreadable.{vbCrLf}{vbCrLf}Do you wish to continue?"
            End If
            MsgBoxResult = MsgBox(Msg, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkCancel Or MsgBoxStyle.DefaultButton2)
        Else
            MsgBoxResult = MsgBoxResult.Ok
        End If

        If MsgBoxResult = MsgBoxResult.Ok Then
            Dim Result = FloppyDrive.OpenWrite(Drive)
            If Result Then
                Dim WriteOptions = FloppyDiskWriteOptions(Owner, DoFormat, DetectedFormat, NewDiskFormat)
                If Not WriteOptions.Cancelled Then
                    Dim FloppyAccessForm As New FloppyAccessForm(FloppyDrive, Disk.BPB, FloppyAccessForm.FloppyAccessType.Write) With {
                        .DiskBuffer = Disk.Image.GetBytes,
                        .DoFormat = WriteOptions.Format,
                        .DoVerify = WriteOptions.Verify
                    }
                    FloppyAccessForm.ShowDialog(Owner)
                    FloppyAccessForm.Close()
                End If
                FloppyDrive.Close()
            Else
                MsgBox($"Error: Unable to write to the drive at this time.", MsgBoxStyle.Exclamation)
            End If
        End If
    End Sub

    Private Function FloppyDiskGetReadOptions(Owner As IWin32Window, FloppyDrive As FloppyInterface) As BiosParameterBlock
        Dim DetectedFormat As FloppyDiskFormat
        Dim ReturnedFormat As FloppyDiskFormat
        Dim BootSector As BootSector = Nothing

        Dim Buffer(BYTES_PER_SECTOR - 1) As Byte
        Dim BytesRead = FloppyDrive.ReadSector(0, Buffer)
        If BytesRead = Buffer.Length Then
            BootSector = New BootSector(Buffer)
            DetectedFormat = GetFloppyDiskFormat(BootSector.BPB, False)
        Else
            DetectedFormat = FloppyDiskFormat.FloppyUnknown
        End If

        Dim FloppyReadOptionsForm As New FloppyReadOptionsForm(DetectedFormat)
        FloppyReadOptionsForm.ShowDialog(Owner)
        ReturnedFormat = FloppyReadOptionsForm.DiskFormat
        FloppyReadOptionsForm.Close()

        If FloppyReadOptionsForm.DialogResult = DialogResult.Cancel Then
            Return Nothing
        ElseIf ReturnedFormat = -1 Then
            Return Nothing
        ElseIf BootSector IsNot Nothing AndAlso DetectedFormat = ReturnedFormat Then
            Return BootSector.BPB
        Else
            Return BuildBPB(ReturnedFormat)
        End If
    End Function
    Private Function FloppyDiskWriteOptions(Owner As IWin32Window, DoFormat As Boolean, DetectedFormat As FloppyDiskFormat, DiskFormat As FloppyDiskFormat) As FloppyWriteOptionsForm.FloppyWriteOptions
        Dim WriteOptions As FloppyWriteOptionsForm.FloppyWriteOptions

        Dim FloppyWriteOptioonsForm As New FloppyWriteOptionsForm(DoFormat, DetectedFormat, DiskFormat)
        FloppyWriteOptioonsForm.ShowDialog(Owner)
        WriteOptions = FloppyWriteOptioonsForm.WriteOptions
        FloppyWriteOptioonsForm.Close()

        Return WriteOptions
    End Function

End Module