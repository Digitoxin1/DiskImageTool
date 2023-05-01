Imports System.IO

Public Class ImageDataScanInfo
    Public Property DirectoryHasAdditionalData As Boolean = False
    Public Property DirectoryHasBootSector As Boolean = False
    Public Property HasBadSectors As Boolean = False
    Public Property HasCreated As Boolean = False
    Public Property HasFATChainingErrors As Boolean = False
    Public Property HasInvalidDirectoryEntries As Boolean = False
    Public Property HasInvalidImageSize As Boolean = False
    Public Property HasLastAccessed As Boolean = False
    Public Property HasLongFileNames As Boolean = False
    Public Property HasMismatchedFATs As Boolean = False
    Public Property HasUnusedClusters As Boolean = False
    Public Property IsValidImage As Boolean = True
    Public Property UnknownDiskType As Boolean = False
    Public Property OEMName As String = ""
    Public Property OEMNameFound As Boolean = False
    Public Property OEMNameMatched As Boolean = False
    Public Property OEMNameWin9x As Boolean = False
End Class

Public Class LoadedImageData
    Public Sub New(SourceFile As String)
        _Compressed = False
        _CompressedFile = ""
        _SourceFile = SourceFile
        _Modifications = Nothing
        _Modified = False
        _ReadOnly = False
        _ScanInfo = New ImageDataScanInfo
        _Scanned = False
    End Sub

    Public Property CachedRootDir As Byte()
    Public Property Compressed As Boolean
    Public Property CompressedFile As String
    Public Property SourceFile As String
    Public Property Modifications As Stack(Of DiskImage.DataChange())
    Public Property Modified As Boolean
    Public Property [ReadOnly] As Boolean
    Public ReadOnly Property ScanInfo As ImageDataScanInfo
    Public Property Scanned As Boolean
    Public Shared Property StringOffset As Integer = 0
    Public Function DisplayPath() As String
        Dim FullPath = _SourceFile
        If _Compressed Then
            FullPath = Path.Combine(FullPath, Replace(_CompressedFile, "/", "\"))
        End If
        Return FullPath
    End Function
    Public Function SaveFile() As String
        If _Compressed Then
            Return Path.Combine(Path.GetDirectoryName(_SourceFile), _CompressedFile)
        Else
            Return _SourceFile
        End If
    End Function
    Public Function FileName() As String
        If _Compressed Then
            Return Path.GetFileName(_CompressedFile)
        Else
            Return Path.GetFileName(_SourceFile)
        End If
    End Function

    Public Overrides Function ToString() As String
        Return Right(DisplayPath, Len(DisplayPath) - _StringOffset) & IIf(_Modified, " *", "")
    End Function
End Class