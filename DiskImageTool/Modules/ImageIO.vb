Imports System.IO
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Module ImageIO
    Public Const BASIC_SECTOR_FILE_EXTENSIONS As String = ".ima,.img,.vfd,.flp"
    Public ReadOnly AdvancedSectorFileExtensions As New List(Of String) From {".imd", ".psi", ".td0"}
    Public ReadOnly AllFileExtensions As New List(Of String)
    Public ReadOnly ArchiveFileExtensions As New List(Of String) From {".zip"}
    Public ReadOnly BasicSectorFileExtensions As New List(Of String) From {".ima", ".img", ".imz", ".vfd", ".flp"}
    Public ReadOnly BitstreamFileExtensions As New List(Of String) From {".86f", ".hfe", ".mfm", ".pri", ".tc"}

    Public Enum SaveImageResponse
        Success
        Failed
        Unsupported
        Unknown
        Cancelled
    End Enum

    Private Enum FloppyImageGroup
        BasicSectorImage
        AdvancedSectorImage
        BitstreamImage
    End Enum
    Public Function CreateBackupIfExists(FilePath As String) As Boolean
        If Not IO.File.Exists(FilePath) Then
            Return True
        End If

        Try
            Dim BackupPath As String = FilePath & ".bak"
            IO.File.Copy(FilePath, BackupPath, True)
        Catch ex As Exception
            DebugException(ex)
            Return False
        End Try

        Return True
    End Function

    Public Function DeleteTempFileIfExists(FilePath As String) As Boolean
        If String.IsNullOrWhiteSpace(FilePath) Then
            Return False
        End If

        Dim TempRoot As String = GetTempPath()

        If String.IsNullOrWhiteSpace(TempRoot) Then
            Return False
        End If

        Try
            Dim FullFilePath As String = IO.Path.GetFullPath(FilePath)
            Dim FullTempRoot As String = IO.Path.GetFullPath(TempRoot)

            If Not FullTempRoot.EndsWith(IO.Path.DirectorySeparatorChar) AndAlso Not FullTempRoot.EndsWith(IO.Path.AltDirectorySeparatorChar) Then
                FullTempRoot &= IO.Path.DirectorySeparatorChar
            End If

            If Not FullFilePath.StartsWith(FullTempRoot, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If

            If IO.File.Exists(FilePath) Then
                IO.File.Delete(FilePath)
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub DirectoryEntryExport(FileData As FileData)
        Dim DirectoryEntry = FileData.DirectoryEntry

        Dim FileName = DirectoryEntry.GetFullFileName

        Using Dialog As New SaveFileDialog With {
                .FileName = CleanFileName(FileName),
                .InitialDirectory = App.UserState.LastExportFilePath,
                .RestoreDirectory = True
            }

            If Dialog.ShowDialog <> DialogResult.OK Then
                Exit Sub
            End If

            App.UserState.LastExportFilePath = IO.Path.GetDirectoryName(Dialog.FileName)

            Dim Result = DirectoryEntrySaveToFile(Dialog.FileName, DirectoryEntry)

            If Not Result Then
                Dim Msg = String.Format(My.Resources.Dialog_SaveFileError, IO.Path.GetFileName(Dialog.FileName))
                MsgBox(Msg, MsgBoxStyle.Critical + MsgBoxStyle.OkOnly)
            End If
        End Using
    End Sub

    Public Function DirectoryEntrySaveToFile(FilePath As String, DirectoryEntry As DiskImage.DirectoryEntry) As Boolean
        Try
            IO.File.WriteAllBytes(FilePath, DirectoryEntry.GetContent)

            Dim FileInfo As New IO.FileInfo(FilePath)
            DirectoryEntrySetFileDates(FileInfo, DirectoryEntry)
        Catch ex As Exception
            DebugException(ex)
            Return False
        End Try

        Return True
    End Function

    Public Sub DirectoryEntrySetFileDates(DirectoryInfo As DirectoryInfo, DirectoryEntry As DiskImage.DirectoryEntry)
        DirectoryInfo.LastWriteTime = DirectoryEntry.GetLastWriteDate.DateObject

        If DirectoryEntry.HasCreationDate Then
            DirectoryInfo.CreationTime = DirectoryEntry.GetCreationDate.DateObject
        End If

        If DirectoryEntry.HasLastAccessDate Then
            DirectoryInfo.LastAccessTime = DirectoryEntry.GetLastAccessDate.DateObject
        End If
    End Sub

    Public Sub DirectoryEntrySetFileDates(FileInfo As IO.FileInfo, DirectoryEntry As DiskImage.DirectoryEntry)
        FileInfo.LastWriteTime = DirectoryEntry.GetLastWriteDate.DateObject

        If DirectoryEntry.HasCreationDate Then
            FileInfo.CreationTime = DirectoryEntry.GetCreationDate.DateObject
        End If

        If DirectoryEntry.HasLastAccessDate Then
            FileInfo.LastAccessTime = DirectoryEntry.GetLastAccessDate.DateObject
        End If
    End Sub

    Public Function DiskImageLoadFromImageData(ImageData As ImageData, Optional SetChecksum As Boolean = False) As DiskImage.Disk
        Dim Data() As Byte
        Dim LastChecksum As UInteger
        Dim FloppyImage As IFloppyImage = Nothing

        ImageData.InvalidImage = False

        If FloppyImage Is Nothing AndAlso IO.File.Exists(ImageData.SourceFile) Then
            Try
                If ImageData.FileType = ImageData.FileTypeEnum.Compressed Then
                    ImageData.ReadOnly = True
                    Data = OpenFileFromZIP(ImageData.SourceFile, ImageData.CompressedFile)
                Else
                    ImageData.ReadOnly = IsFileReadOnly(ImageData.SourceFile)
                    Data = IO.File.ReadAllBytes(ImageData.SourceFile)
                End If

                Dim FloppyImageType = GetImageTypeFromHeader(Data)
                'Dim FloppyImageType = GetImageTypeFromFileName(ImageData.FileName)

                LastChecksum = ImageData.Checksum
                If SetChecksum Then
                    ImageData.Checksum = CRC32.ComputeChecksum(Data)
                End If

                If FloppyImageType = FloppyImageType.TranscopyImage Then
                    Dim TCImage = ImageFormats.TC.ImageLoad(Data)
                    If TCImage IsNot Nothing Then
                        FloppyImage = TCImage
                    Else
                        ImageData.InvalidImage = True
                    End If

                ElseIf FloppyImageType = FloppyImageType.PSIImage Then
                    Dim PSIImage = ImageFormats.PSI.ImageLoad(Data)
                    If PSIImage IsNot Nothing Then
                        FloppyImage = PSIImage
                    Else
                        ImageData.InvalidImage = True
                    End If

                ElseIf FloppyImageType = FloppyImageType.PRIImage Then
                    Dim PRIImage = ImageFormats.PRI.ImageLoad(Data)
                    If PRIImage IsNot Nothing Then
                        FloppyImage = PRIImage
                    Else
                        ImageData.InvalidImage = True
                    End If

                ElseIf FloppyImageType = FloppyImageType.IMDImage Then
                    Dim IMDImage = ImageFormats.IMD.ImageLoad(Data)
                    If IMDImage IsNot Nothing Then
                        FloppyImage = IMDImage
                    Else
                        ImageData.InvalidImage = True
                    End If

                ElseIf FloppyImageType = FloppyImageType.MFMImage Then
                    Dim MFMImage = ImageFormats.MFM.ImageLoad(Data)
                    If MFMImage IsNot Nothing Then
                        FloppyImage = MFMImage
                    Else
                        ImageData.InvalidImage = True
                    End If

                ElseIf FloppyImageType = FloppyImageType.D86FImage Then
                    Dim D86FImage = ImageFormats.D86F.ImageLoad(Data)
                    If D86FImage IsNot Nothing Then
                        FloppyImage = D86FImage
                    Else
                        ImageData.InvalidImage = True
                    End If

                ElseIf FloppyImageType = FloppyImageType.HFEImage Then
                    Dim HFEImage = ImageFormats.HFE.ImageLoad(Data)
                    If HFEImage IsNot Nothing Then
                        FloppyImage = HFEImage
                    Else
                        ImageData.InvalidImage = True
                    End If

                ElseIf FloppyImageType = FloppyImageType.TD0Image Then
                    Dim TD0Image = ImageFormats.TD0.ImageLoad(Data)
                    If TD0Image IsNot Nothing Then
                        FloppyImage = TD0Image
                    Else
                        ImageData.InvalidImage = True
                    End If

                Else
                    FloppyImage = New BasicSectorImage(Data)
                End If
                'If ImageData.XDFMiniDisk Then
                '    ImageData.ReadOnly = True
                '    Dim NewData(ImageData.XDFLength - 1) As Byte
                '    Array.Copy(Data, ImageData.XDFOffset, NewData, 0, ImageData.XDFLength)
                '    Data = NewData
                'End If
            Catch ex As Exception
                DebugException(ex)
                FloppyImage = Nothing
            End Try
        End If

        If FloppyImage Is Nothing Then
            Return Nothing
        Else
            If ImageData.Loaded AndAlso ImageData.Checksum <> LastChecksum Then
                If ImageData.Modifications IsNot Nothing AndAlso ImageData.Modifications.Count > 0 Then
                    ImageData.Modifications.Clear()
                    ImageData.ExternalModified = True
                ElseIf SetChecksum Then
                    ImageData.ExternalModified = False
                End If
            End If
            If SetChecksum Then
                ImageData.Loaded = True
            End If

            Dim Disk As New DiskImage.Disk(FloppyImage, ImageData.FATIndex, ImageData.Modifications)
            ImageData.Modifications = Disk.Image.History.Changes

            Return Disk
        End If
    End Function

    Public Sub EmptyTempImagePath()
        Dim TempPath = IO.Path.Combine(GetTempPath, App.Globals.GlobalIdentifier.ToString)

        If IO.Directory.Exists(TempPath) Then
            Try
                Dim Files = IO.Directory.EnumerateFiles(TempPath, "*.*")
                For Each File In Files
                    Try
                        IO.File.Delete(File)
                    Catch ex As Exception
                        DebugException(ex)
                    End Try
                Next
                IO.Directory.Delete(TempPath)
            Catch ex As Exception
                DebugException(ex)
            End Try
        End If
    End Sub

    Public Function GenerateOutputFile(Extension As String) As String
        Dim TempPath = InitTempImagePath()

        If TempPath = "" Then
            MsgBox(My.Resources.Dialog_TempPathError, MsgBoxStyle.Critical)
            Return ""
        End If

        Dim FileName = Guid.NewGuid.ToString & Extension
        Return GenerateUniqueFileName(TempPath, FileName)
    End Function

    Public Function GetAppDataPath() As String
        Dim BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        Dim AppName = My.Application.Info.ProductName

        Return Path.Combine(BaseFolder, AppName)
    End Function

    Public Function GetAppPath() As String
        Return IO.Path.GetDirectoryName(Application.ExecutablePath)
    End Function
    Public Function GetImageTypeNameFromExtension(Extension As String) As String
        Extension = Extension.ToLower()

        Select Case Extension
            Case ".imz"
                Return My.Resources.FileType_IMZ
            Case ".vfd", ".flp"
                Return My.Resources.FileType_VFD
            Case ".imd"
                Return GetImageTypeName(FloppyImageType.IMDImage)
            Case ".pri"
                Return GetImageTypeName(FloppyImageType.PRIImage)
            Case ".psi"
                Return GetImageTypeName(FloppyImageType.PSIImage)
            Case ".86f"
                Return GetImageTypeName(FloppyImageType.D86FImage)
            Case ".hfe"
                Return GetImageTypeName(FloppyImageType.HFEImage)
            Case ".mfm"
                Return GetImageTypeName(FloppyImageType.MFMImage)
            Case ".tc"
                Return GetImageTypeName(FloppyImageType.TranscopyImage)
            Case ".td0"
                Return GetImageTypeName(FloppyImageType.TD0Image)
        End Select

        If BasicSectorFileExtensions.Contains(Extension) Then
            Return My.Resources.FloppyImageType_BasicSectorImage

        ElseIf AdvancedSectorFileExtensions.Contains(Extension) Then
            Return My.Resources.FileType_AdvancedSectorImage

        ElseIf BitstreamFileExtensions.Contains(Extension) Then
            Return My.Resources.FileType_BitstreamImage
        End If

        Return Extension.ToUpper
    End Function

    Public Function GetLoadDialogFilters() As String
        Dim FileFilter As String
        Dim ExtensionList As List(Of String)

        FileFilter = FileDialogGetFilter(My.Resources.FileType_AllDiskImages, AllFileExtensions)

        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FloppyImageType_BasicSectorImage, BasicSectorFileExtensions)

        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_AdvancedSectorImage, AdvancedSectorFileExtensions)

        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_BitstreamImage, BitstreamFileExtensions)

        ExtensionList = New List(Of String) From {".imz"}
        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_IMZ, ExtensionList)

        ExtensionList = New List(Of String) From {".vfd", ".flp"}
        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_VFD, ExtensionList)

        ExtensionList = New List(Of String) From {".imd"}
        FileFilter = FileDialogAppendFilter(FileFilter, GetImageTypeName(FloppyImageType.IMDImage), ExtensionList)

        ExtensionList = New List(Of String) From {".td0"}
        FileFilter = FileDialogAppendFilter(FileFilter, GetImageTypeName(FloppyImageType.TD0Image), ExtensionList)

        ExtensionList = New List(Of String) From {".pri"}
        FileFilter = FileDialogAppendFilter(FileFilter, GetImageTypeName(FloppyImageType.PRIImage), ExtensionList)

        ExtensionList = New List(Of String) From {".psi"}
        FileFilter = FileDialogAppendFilter(FileFilter, GetImageTypeName(FloppyImageType.PSIImage), ExtensionList)

        ExtensionList = New List(Of String) From {".86f"}
        FileFilter = FileDialogAppendFilter(FileFilter, GetImageTypeName(FloppyImageType.D86FImage), ExtensionList)

        ExtensionList = New List(Of String) From {".hfe"}
        FileFilter = FileDialogAppendFilter(FileFilter, GetImageTypeName(FloppyImageType.HFEImage), ExtensionList)

        ExtensionList = New List(Of String) From {".mfm"}
        FileFilter = FileDialogAppendFilter(FileFilter, GetImageTypeName(FloppyImageType.MFMImage), ExtensionList)

        ExtensionList = New List(Of String) From {".tc"}
        FileFilter = FileDialogAppendFilter(FileFilter, GetImageTypeName(FloppyImageType.TranscopyImage), ExtensionList)

        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_ZipArchive, ".zip")
        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_All, ".*")

        Return FileFilter
    End Function

    Public Function GetSaveDialogFilters(DiskFormat As FloppyDiskFormat, ImageType As FloppyImageType, FileExt As String) As (Filter As String, FilterIndex As Integer)
        Dim Response As (Filter As String, FilterIndex As Integer)
        Dim CurrentIndex As Integer = 1
        Dim Extension As String
        Dim ExtensionList As List(Of String)
        Dim ImageGroup = GetImageGroup(ImageType)

        Response.FilterIndex = 0

        ExtensionList = New List(Of String) From {".ima", ".img"}
        For Each Extension In ExtensionList
            If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                Response.FilterIndex = CurrentIndex
            End If
        Next
        Response.Filter = FileDialogGetFilter(My.Resources.FileType_FloppyDiskImage, ExtensionList)
        CurrentIndex += 1

        ExtensionList = New List(Of String) From {".vfd", ".flp"}
        For Each Extension In ExtensionList
            If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                Response.FilterIndex = CurrentIndex
            End If
        Next
        Response.Filter = FileDialogAppendFilter(Response.Filter, My.Resources.FileType_VFD, ExtensionList)
        CurrentIndex += 1

        If ImageType = FloppyImageType.IMDImage Or ImageGroup <> FloppyImageGroup.AdvancedSectorImage Then
            ExtensionList = New List(Of String) From {".imd"}
            For Each Extension In ExtensionList
                If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                    Response.FilterIndex = CurrentIndex
                End If
            Next
            Response.Filter = FileDialogAppendFilter(Response.Filter, GetImageTypeName(FloppyImageType.IMDImage), ExtensionList)
            CurrentIndex += 1
        End If

        If ImageType = FloppyImageType.PSIImage Or ImageGroup <> FloppyImageGroup.AdvancedSectorImage Then
            ExtensionList = New List(Of String) From {".psi"}
            For Each Extension In ExtensionList
                If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                    Response.FilterIndex = CurrentIndex
                End If
            Next
            Response.Filter = FileDialogAppendFilter(Response.Filter, GetImageTypeName(FloppyImageType.PSIImage), ExtensionList)
            CurrentIndex += 1
        End If

        If ImageGroup <> FloppyImageGroup.AdvancedSectorImage Then
            ExtensionList = New List(Of String) From {".86f"}
            For Each Extension In ExtensionList
                If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                    Response.FilterIndex = CurrentIndex
                End If
            Next
            Response.Filter = FileDialogAppendFilter(Response.Filter, GetImageTypeName(FloppyImageType.D86FImage), ExtensionList)
            CurrentIndex += 1

            ExtensionList = New List(Of String) From {".hfe"}
            For Each Extension In ExtensionList
                If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                    Response.FilterIndex = CurrentIndex
                End If
            Next
            Response.Filter = FileDialogAppendFilter(Response.Filter, GetImageTypeName(FloppyImageType.HFEImage), ExtensionList)
            CurrentIndex += 1

            ExtensionList = New List(Of String) From {".mfm"}
            For Each Extension In ExtensionList
                If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                    Response.FilterIndex = CurrentIndex
                End If
            Next
            Response.Filter = FileDialogAppendFilter(Response.Filter, GetImageTypeName(FloppyImageType.MFMImage), ExtensionList)
            CurrentIndex += 1

            ExtensionList = New List(Of String) From {".pri"}
            For Each Extension In ExtensionList
                If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                    Response.FilterIndex = CurrentIndex
                End If
            Next
            Response.Filter = FileDialogAppendFilter(Response.Filter, GetImageTypeName(FloppyImageType.PRIImage), ExtensionList)
            CurrentIndex += 1

            ExtensionList = New List(Of String) From {".tc"}
            For Each Extension In ExtensionList
                If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                    Response.FilterIndex = CurrentIndex
                End If
            Next
            Response.Filter = FileDialogAppendFilter(Response.Filter, GetImageTypeName(FloppyImageType.TranscopyImage), ExtensionList)
            CurrentIndex += 1
        End If

        Dim Items = System.Enum.GetValues(GetType(FloppyDiskFormat))
        For Each Item As Integer In Items
            Extension = FloppyDiskFormatGetParams(Item).FileExtension
            If Extension <> "" Then
                Dim Description = FloppyDiskFormatGetFileFilterDescription(Item)
                If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                    Response.Filter = FileDialogAppendFilter(Response.Filter, Description, Extension)
                    Response.FilterIndex = CurrentIndex
                    CurrentIndex += 1
                ElseIf Item = DiskFormat Then
                    Response.Filter = FileDialogAppendFilter(Response.Filter, Description, Extension)
                    CurrentIndex += 1
                End If
            End If
        Next

        Response.Filter = FileDialogAppendFilter(Response.Filter, My.Resources.FileType_All, ".*")
        If Response.FilterIndex = 0 Then
            Response.FilterIndex = CurrentIndex
        End If

        Return Response
    End Function

    Public Function GetTempPath() As String
        Dim BaseFolder = IO.Path.GetTempPath()
        Dim AppName = My.Application.Info.ProductName

        Return Path.Combine(BaseFolder, AppName)
    End Function

    Public Sub InitAllFileExtensions()
        Dim Items = System.Enum.GetValues(GetType(FloppyDiskFormat))
        For Each Item As Integer In Items
            Dim FileExt = FloppyDiskFormatGetParams(Item).FileExtension
            If FileExt <> "" Then
                If Not BasicSectorFileExtensions.Contains(FileExt) Then
                    BasicSectorFileExtensions.Add(FileExt)
                End If
            End If
        Next

        For Each Item In BasicSectorFileExtensions
            If Not AllFileExtensions.Contains(Item) Then
                AllFileExtensions.Add(Item)
            End If
        Next

        For Each Item In AdvancedSectorFileExtensions
            If Not AllFileExtensions.Contains(Item) Then
                AllFileExtensions.Add(Item)
            End If
        Next

        For Each Item In BitstreamFileExtensions
            If Not AllFileExtensions.Contains(Item) Then
                AllFileExtensions.Add(Item)
            End If
        Next
    End Sub

    Public Function InitDataPath() As String
        Dim DataPath = GetAppDataPath()

        If Not IO.Directory.Exists(DataPath) Then
            Try
                IO.Directory.CreateDirectory(DataPath)
            Catch ex As Exception
                DebugException(ex)
                Return ""
            End Try
        End If

        Return DataPath
    End Function

    Public Function InitTempImagePath() As String
        Dim TempPath = IO.Path.Combine(GetTempPath, App.Globals.GlobalIdentifier.ToString)

        If Not IO.Directory.Exists(TempPath) Then
            Try
                IO.Directory.CreateDirectory(TempPath)
            Catch ex As Exception
                DebugException(ex)
                Return ""
            End Try
        End If

        Return TempPath
    End Function
    Public Function InitTempPath() As String
        Dim TempPath = GetTempPath()

        If Not IO.Directory.Exists(TempPath) Then
            Try
                IO.Directory.CreateDirectory(TempPath)
            Catch ex As Exception
                DebugException(ex)
            End Try
        End If

        Return TempPath
    End Function

    Public Function OpenFileFromZIP(ZipFileName As String, FileName As String) As Byte()
        Dim Data As New IO.MemoryStream()
        Dim Archive As IO.Compression.ZipArchive = IO.Compression.ZipFile.OpenRead(ZipFileName)
        Dim Entry = Archive.GetEntry(FileName)
        If Entry IsNot Nothing Then
            Entry.Open.CopyTo(Data)
            Return Data.ToArray
        Else
            Return Nothing
        End If
    End Function

    Public Function SaveDiskImageToFile(Disk As DiskImage.Disk, FilePath As String, DoBackup As Boolean) As SaveImageResponse
        Dim FileImageType = GetImageTypeFromFileName(FilePath)
        Dim DiskImageType = Disk.Image.ImageType
        Dim Response As SaveImageResponse = SaveImageResponse.Failed
        Dim Result As Boolean = False

        If Not CheckCompatibility(FileImageType, Disk.Image, Disk.DiskParams.Format) Then
            Return SaveImageResponse.Cancelled
        End If

        If FileImageType = DiskImageType Then
            If Not DoBackup OrElse CreateBackupIfExists(FilePath) Then
                Result = Disk.Image.SaveToFile(FilePath)
            End If

        ElseIf FileImageType = FloppyImageType.BasicSectorImage Then
            Dim Data As Byte()
            If Disk.Image.IsBitstreamImage AndAlso Disk.DiskParams.Format = FloppyDiskFormat.FloppyXDFMicro Then
                Dim XDFResponse = ImageFormats.XDFImageToBasicSectorImaage(Disk.Image.BitstreamImage)
                If XDFResponse.Result Then
                    Data = XDFResponse.Data
                Else
                    Return SaveImageResponse.Unknown
                End If
            Else
                Data = Disk.Image.GetBytes()
            End If

            If Not DoBackup OrElse CreateBackupIfExists(FilePath) Then
                Result = SaveByteArrayToFile(FilePath, Data)
            End If
        Else
            Dim NewImage As IBitstreamImage = Nothing

            If DiskImageType = FloppyImageType.BasicSectorImage Then
                Dim Format = Disk.DiskParams.Format

                If FloppyDiskFormatIsStandard(Format) Then
                    Dim Data = Disk.Image.GetBytes()

                    If FileImageType = FloppyImageType.TranscopyImage Then
                        NewImage = ImageFormats.BasicSectorToTranscopyImage(Data, Format)
                    ElseIf FileImageType = FloppyImageType.PSIImage Then
                        NewImage = ImageFormats.BasicSectorToPSIImage(Data, Format)
                    ElseIf FileImageType = FloppyImageType.PRIImage Then
                        NewImage = ImageFormats.BasicSectorToPRIImage(Data, Format)
                    ElseIf FileImageType = FloppyImageType.MFMImage Then
                        NewImage = ImageFormats.BasicSectorToMFMImage(Data, Format)
                    ElseIf FileImageType = FloppyImageType.HFEImage Then
                        NewImage = ImageFormats.BasicSectorToHFEImage(Data, Format)
                    ElseIf FileImageType = FloppyImageType.D86FImage Then
                        NewImage = ImageFormats.BasicSectorTo86FImage(Data, Format)
                    ElseIf FileImageType = FloppyImageType.IMDImage Then
                        NewImage = ImageFormats.BasicSectorToIMDImage(Data, Format)
                    End If
                Else
                    Return SaveImageResponse.Unknown
                End If

            ElseIf FileImageType = FloppyImageType.TranscopyImage Then
                Dim Image As IBitstreamImage = Disk.Image.BitstreamImage
                If Image IsNot Nothing Then
                    NewImage = ImageFormats.BitstreamToTranscopyImage(Image)
                End If

            ElseIf FileImageType = FloppyImageType.MFMImage Then
                Dim Image As IBitstreamImage = Disk.Image.BitstreamImage
                If Image IsNot Nothing Then
                    NewImage = ImageFormats.BitstreamToMFMImage(Image)
                End If

            ElseIf FileImageType = FloppyImageType.HFEImage Then
                Dim Image As IBitstreamImage = Disk.Image.BitstreamImage
                If Image IsNot Nothing Then
                    NewImage = ImageFormats.BitstreamToHFEImage(Image)
                End If

            ElseIf FileImageType = FloppyImageType.PSIImage Then
                Dim Image As IBitstreamImage = Disk.Image.BitstreamImage
                If Image IsNot Nothing Then
                    NewImage = ImageFormats.BitstreamToPSIImage(Image)
                End If

            ElseIf FileImageType = FloppyImageType.PRIImage Then
                Dim Image As IBitstreamImage = Disk.Image.BitstreamImage
                If Image IsNot Nothing Then
                    NewImage = ImageFormats.BitstreamToPRIImage(Image)
                End If

            ElseIf FileImageType = FloppyImageType.IMDImage Then
                Dim Image As IBitstreamImage = Disk.Image.BitstreamImage
                If Image IsNot Nothing Then
                    NewImage = ImageFormats.BitstreamToIMDImage(Image)
                End If

            ElseIf FileImageType = FloppyImageType.D86FImage Then
                Dim Image As IBitstreamImage = Disk.Image.BitstreamImage
                If Image IsNot Nothing Then
                    NewImage = ImageFormats.BitstreamTo86FImage(Image)
                End If
            End If

            If NewImage IsNot Nothing Then
                If Not DoBackup OrElse CreateBackupIfExists(FilePath) Then
                    Result = NewImage.Export(FilePath)
                End If
            Else
                Return SaveImageResponse.Unsupported
            End If
        End If

        If Result Then
            Response = SaveImageResponse.Success
        End If

        Return Response
    End Function

    Private Function CheckCompatibility(FileImageType As FloppyImageType, FloppyImage As IFloppyImage, Format As FloppyDiskFormat) As Boolean
        Dim Msg As String

        If FileImageType = FloppyImage.ImageType Then
            Return True
        End If

        If FileImageType = FloppyImageType.IMDImage Then
            If TypeOf FloppyImage Is MappedFloppyImage Then
                Dim BitstreamData = DirectCast(FloppyImage, MappedFloppyImage)
                Dim CompatibleSectors As Boolean = True
                For i = 0 To BitstreamData.TrackCount - 1
                    For j = 0 To BitstreamData.SideCount - 1
                        Dim TrackData = BitstreamData.GetTrack(i, j)
                        If TrackData IsNot Nothing Then
                            If TrackData.FirstSectorId > -1 Then
                                If TrackData.SectorSize = -1 Or TrackData.OverlappingSectors Or TrackData.DuplicateSectors Then
                                    CompatibleSectors = False
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                    If Not CompatibleSectors Then
                        Exit For
                    End If
                Next
                If Not CompatibleSectors Then
                    Msg = My.Resources.Dialog_Image_NonCompatibleSectors
                    MsgBox(Msg, MsgBoxStyle.Exclamation)
                    Return False
                End If
            End If
        End If

        If FileImageType = FloppyImageType.BasicSectorImage Then
            If Format = FloppyDiskFormat.FloppyXDFMicro Then
                Return True
            End If

            If FloppyImage.NonStandardTracks.Count > 0 Then
                Msg = String.Format(My.Resources.Dialog_Image_NonStandardTracks, Environment.NewLine)
                If MsgBox(Msg, MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.No Then
                    Return False
                End If
            End If

        ElseIf FloppyImage.HasWeakBitsSupport Then
            If Not HasWeakBitsSupport(FileImageType) Then
                If FloppyImage.NonStandardTracks.Count > 0 Then
                    If FloppyImage.HasWeakBits Then
                        Msg = String.Format(My.Resources.Dialog_Image_SurfaceData, Environment.NewLine)
                        If MsgBox(Msg, MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2) = MsgBoxResult.No Then
                            Return False
                        End If
                    End If
                End If
            End If
        End If

        Return True
    End Function

    Private Function GetImageGroup(ImageType As FloppyImageType) As FloppyImageGroup
        Select Case ImageType
            Case FloppyImageType.HFEImage, FloppyImageType.MFMImage, FloppyImageType.TranscopyImage, FloppyImageType.D86FImage, FloppyImageType.PRIImage
                Return FloppyImageGroup.BitstreamImage
            Case FloppyImageType.IMDImage, FloppyImageType.PSIImage, FloppyImageType.TD0Image
                Return FloppyImageGroup.AdvancedSectorImage
            Case Else
                Return FloppyImageGroup.BasicSectorImage
        End Select
    End Function
    Private Function HasWeakBitsSupport(ImageType As FloppyImageType) As Boolean
        Return (ImageType = FloppyImageType.PSIImage Or ImageType = FloppyImageType.PRIImage Or ImageType = FloppyImageType.D86FImage)
    End Function
End Module