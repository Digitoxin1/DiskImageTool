Imports System.IO
Imports System.Text
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Module ImageIO
    Public Const BASIC_SECTOR_FILE_EXTENSIONS As String = ".ima,.img,.vfd,.flp"
    Public ReadOnly AdvancedSectorFileExtensions As New List(Of String) From {".imd", ".psi", ".td0"}
    Public ReadOnly AllFileExtensions As New List(Of String)
    Public ReadOnly ArchiveFileExtensions As New List(Of String) From {".zip"}
    Public ReadOnly BasicSectorFileExtensions As New List(Of String) From {".ima", ".img", ".imz", ".vfd", ".flp"}
    Public ReadOnly BitstreamFileExtensions As New List(Of String) From {".86f", ".hfe", ".mfm", ".pri", ".tc"}

    Private ReadOnly ImageLoaders As New Dictionary(Of FloppyImageType, Func(Of Byte(), IFloppyImage)) From {
        {FloppyImageType.TranscopyImage, Function(d) ImageFormats.TC.ImageLoad(d)},
        {FloppyImageType.PSIImage, Function(d) ImageFormats.PSI.ImageLoad(d)},
        {FloppyImageType.PRIImage, Function(d) ImageFormats.PRI.ImageLoad(d)},
        {FloppyImageType.IMDImage, Function(d) ImageFormats.IMD.ImageLoad(d)},
        {FloppyImageType.MFMImage, Function(d) ImageFormats.MFM.ImageLoad(d)},
        {FloppyImageType.D86FImage, Function(d) ImageFormats.D86F.ImageLoad(d)},
        {FloppyImageType.HFEImage, Function(d) ImageFormats.HFE.ImageLoad(d)},
        {FloppyImageType.TD0Image, Function(d) ImageFormats.TD0.ImageLoad(d)}
    }

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
        Dim LastChecksum As UInteger = ImageData.Checksum
        Dim FloppyImage As IFloppyImage = Nothing

        ImageData.InvalidImage = False

        If Not IO.File.Exists(ImageData.SourceFile) Then
            Return Nothing
        End If

        Try
            If ImageData.FileType = ImageData.FileTypeEnum.Compressed Then
                ImageData.ReadOnly = True
                Data = OpenFileFromZIP(ImageData.SourceFile, ImageData.CompressedFile)
            Else
                ImageData.ReadOnly = IsFileReadOnly(ImageData.SourceFile)
                Data = IO.File.ReadAllBytes(ImageData.SourceFile)
            End If

            Dim FloppyImageType = GetImageTypeFromHeader(Data)

            If SetChecksum Then
                ImageData.Checksum = CRC32.ComputeChecksum(Data)
            End If

            FloppyImage = TryLoadTypedImage(FloppyImageType, Data)
            If FloppyImage Is Nothing Then
                If ImageLoaders.ContainsKey(FloppyImageType) Then
                    ImageData.InvalidImage = True
                    Return Nothing
                End If
                FloppyImage = New BasicSectorImage(Data)
            End If
        Catch ex As Exception
            DebugException(ex)
            Return Nothing
        End Try

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

    Public Function GetImageExtensionFromType(ImageType As FloppyImageType) As String
        Select Case ImageType
            Case FloppyImageType.TD0Image
                Return ".td0"
            Case FloppyImageType.PSIImage
                Return ".psi"
            Case FloppyImageType.PRIImage
                Return ".pri"
            Case FloppyImageType.IMDImage
                Return ".imd"
            Case FloppyImageType.D86FImage
                Return ".86f"
            Case FloppyImageType.HFEImage
                Return ".hfe"
            Case FloppyImageType.MFMImage
                Return ".mfm"
            Case FloppyImageType.TranscopyImage
                Return ".tc"
            Case Else
                Return ".ima"
        End Select
    End Function

    Public Function GetImageTypeFromFileName(FileName As String) As FloppyImageType
        Dim FileExt = IO.Path.GetExtension(FileName).ToLower

        If FileExt = ".tc" Then
            Return FloppyImageType.TranscopyImage
        ElseIf FileExt = ".psi" Then
            Return FloppyImageType.PSIImage
        ElseIf FileExt = ".pri" Then
            Return FloppyImageType.PRIImage
        ElseIf FileExt = ".mfm" Then
            Return FloppyImageType.MFMImage
        ElseIf FileExt = ".hfe" Then
            Return FloppyImageType.HFEImage
        ElseIf FileExt = ".86f" Then
            Return FloppyImageType.D86FImage
        ElseIf FileExt = ".imd" Then
            Return FloppyImageType.IMDImage
        ElseIf FileExt = ".td0" Then
            Return FloppyImageType.TD0Image
        Else
            Return FloppyImageType.BasicSectorImage
        End If
    End Function

    Public Function GetImageTypeFromHeader(Data() As Byte) As FloppyImageType
        If Encoding.UTF8.GetString(Data, 0, 8) = "HXCPICFE" Then
            Return FloppyImageType.HFEImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 8) = "HXCHFEV3" Then
            Return FloppyImageType.HFEImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 4) = "86BF" Then
            Return FloppyImageType.D86FImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 6) = "HXCMFM" Then
            Return FloppyImageType.MFMImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 4) = "PSI " Then
            Return FloppyImageType.PSIImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 4) = "PRI " Then
            Return FloppyImageType.PRIImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 4) = "IMD " Then
            Return FloppyImageType.IMDImage
        ElseIf Encoding.UTF8.GetString(Data, 0, 2).ToUpper = "TD" Then
            Return FloppyImageType.TD0Image
        ElseIf BitConverter.ToUInt16(Data, 0) = &HA55A Then
            Return FloppyImageType.TranscopyImage
        Else
            Return FloppyImageType.BasicSectorImage
        End If
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

        FileFilter = FileDialogGetFilter(My.Resources.FileType_AllDiskImages, AllFileExtensions)
        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FloppyImageType_BasicSectorImage, BasicSectorFileExtensions)
        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_AdvancedSectorImage, AdvancedSectorFileExtensions)
        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_BitstreamImage, BitstreamFileExtensions)
        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_IMZ, ".imz")
        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_VFD, ".vfd", ".flp")

        Dim ImageTypes = {
            FloppyImageType.IMDImage,
            FloppyImageType.TD0Image,
            FloppyImageType.PRIImage,
            FloppyImageType.PSIImage,
            FloppyImageType.D86FImage,
            FloppyImageType.HFEImage,
            FloppyImageType.MFMImage,
            FloppyImageType.TranscopyImage
        }

        For Each t In ImageTypes
            FileFilter = FileDialogAppendFilter(FileFilter, GetImageTypeName(t), GetImageExtensionFromType(t))
        Next

        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_ZipArchive, ".zip")
        FileFilter = FileDialogAppendFilter(FileFilter, My.Resources.FileType_All, ".*")

        Return FileFilter
    End Function

    Public Function GetSaveDialogFilters(DiskFormat As FloppyDiskFormat, ImageType As FloppyImageType, FileExt As String) As (Filter As String, FilterIndex As Integer)
        Dim Filter As String = ""
        Dim FilterIndex As Integer = 0
        Dim CurrentIndex As Integer = 1

        Dim imageGroup = GetImageGroup(ImageType)

        AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, My.Resources.FileType_FloppyDiskImage, ".ima", ".img")
        AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, My.Resources.FileType_VFD, ".vfd", ".flp")

        If ImageType = FloppyImageType.IMDImage OrElse imageGroup <> FloppyImageGroup.AdvancedSectorImage Then
            AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, FloppyImageType.IMDImage)
        End If

        If ImageType = FloppyImageType.PSIImage OrElse imageGroup <> FloppyImageGroup.AdvancedSectorImage Then
            AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, FloppyImageType.PSIImage)
        End If

        If ImageType = FloppyImageType.TD0Image Then
            AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, FloppyImageType.TD0Image)
        End If

        If imageGroup <> FloppyImageGroup.AdvancedSectorImage Then
            AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, FloppyImageType.D86FImage)
            AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, FloppyImageType.HFEImage)
            AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, FloppyImageType.MFMImage)
            AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, FloppyImageType.PRIImage)
            AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, FloppyImageType.TranscopyImage)
        End If


        For Each Item As FloppyDiskFormat In System.Enum.GetValues(GetType(FloppyDiskFormat))
            Dim Extension = FloppyDiskFormatGetParams(Item).FileExtension
            If Extension <> "" Then
                Dim Description = FloppyDiskFormatGetFileFilterDescription(Item)
                If FileExt.Equals(Extension, StringComparison.OrdinalIgnoreCase) Then
                    AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, Description, Extension)
                ElseIf Item = DiskFormat Then
                    AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, Description, Extension)
                End If
            End If
        Next

        AppendFilterAndTrackIndex(Filter, FilterIndex, CurrentIndex, FileExt, My.Resources.FileType_All, ".*")

        If FilterIndex = 0 Then
            FilterIndex = CurrentIndex - 1
        End If

        Return (Filter, FilterIndex)
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

    Private Sub AppendFilterAndTrackIndex(ByRef filter As String, ByRef filterIndex As Integer, ByRef currentIndex As Integer, fileExt As String, ImageType As FloppyImageType)
        AppendFilterAndTrackIndex(filter, filterIndex, currentIndex, fileExt, GetImageTypeName(ImageType), GetImageExtensionFromType(ImageType))
    End Sub

    Private Sub AppendFilterAndTrackIndex(ByRef filter As String, ByRef filterIndex As Integer, ByRef currentIndex As Integer, fileExt As String, description As String, ParamArray exts() As String)
        If exts Is Nothing OrElse exts.Length = 0 Then
            Return
        End If

        If filterIndex = 0 Then
            For Each e In exts
                If fileExt.Equals(e, StringComparison.OrdinalIgnoreCase) Then
                    filterIndex = currentIndex
                    Exit For
                End If
            Next
        End If

        If String.IsNullOrEmpty(filter) Then
            filter = FileDialogGetFilter(description, exts.ToList())
        Else
            filter = FileDialogAppendFilter(filter, description, exts)
        End If

        currentIndex += 1
    End Sub

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

    Private Function TryLoadTypedImage(ImgType As FloppyImageType, Data As Byte()) As IFloppyImage
        Dim Loader As Func(Of Byte(), IFloppyImage) = Nothing

        If ImageLoaders.TryGetValue(ImgType, Loader) Then
            Return Loader(Data)
        End If

        Return Nothing
    End Function
End Module