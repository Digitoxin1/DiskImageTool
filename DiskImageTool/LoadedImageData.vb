Imports System.IO

Public Class LoadedImageData
    Private ReadOnly _Filters() As Boolean

    Public Enum LoadedImageType
        SectorImage
        TranscopyImage
        PSIImage
    End Enum

    Public Sub New(SourceFile As String)
        _AppliedFilters = 0
        _BatchUpdated = False
        _BottomIndex = -1
        _Compressed = False
        _CompressedFile = ""
        _DiskType = ""
        _FATIndex = 0
        _SourceFile = SourceFile
        _Modifications = Nothing
        _OEMName = ""
        _ReadOnly = False
        _Scanned = False
        _SortHistory = Nothing
        _TempPath = ""
        _InvalidImage = False
        _Loaded = False
        _Checksum = 0
        _ExternalModified = False
        _XDFMiniDisk = False
        _XDFOffset = 0
        _XDFLength = 0
        _SectorMap = New UInteger(-1) {}
        _ProtectedSectors = New HashSet(Of UInteger)

        Dim FilterCount As Integer = FilterGetCount()
        ReDim _Filters(FilterCount - 1)
        For Counter = 0 To FilterCount - 1
            _Filters(Counter) = False
        Next
    End Sub

    Public Property [ReadOnly] As Boolean
    Public Property AppliedFilters As Integer
    Public Property BatchUpdated As Boolean
    Public Property BottomIndex As Integer
    Public Property CachedRootDir As Byte()
    Public Property Compressed As Boolean
    Public Property CompressedFile As String
    Public Property DiskType As String
    Public Property FATIndex As UShort
    Public Property Modifications As Stack(Of DiskImage.DataChange())
    Public Property OEMName As String
    Public Property Scanned As Boolean
    Public Property SourceFile As String
    Public Property SortHistory As List(Of SortEntity)
    Public Property TempPath As String
    Public Property InvalidImage As Boolean
    Public Property Loaded As Boolean
    Public Property Checksum As UInteger
    Public Property ExternalModified As Boolean
    Public Property XDFMiniDisk As Boolean
    Public Property XDFOffset As UInteger
    Public Property XDFLength As UInteger
    Public Property SectorMap As UInteger()
    Public Property ProtectedSectors As HashSet(Of UInteger)

    Public Shared Property StringOffset As Integer = 0

    Public Property Filter(FilterType As FilterTypes) As Boolean
        Get
            Return _Filters(FilterType)
        End Get

        Set(value As Boolean)
            _Filters(FilterType) = value
        End Set
    End Property

    Public Function DisplayPath() As String
        Dim FullPath = _SourceFile
        If _Compressed Then
            FullPath = Path.Combine(FullPath, Replace(_CompressedFile, "/", "\"))
        End If
        Return FullPath
    End Function
    Public Function FileName() As String
        If _Compressed Then
            Return Path.GetFileName(_CompressedFile)
        Else
            Return Path.GetFileName(_SourceFile)
        End If
    End Function

    Public Sub ClearTempPath()
        If _TempPath <> "" Then
            Try
                File.Delete(_TempPath)
            Catch ex As Exception
                Debug.Print("Caught Exception: LoadedImageData.ClearTempPath")
            End Try
            _TempPath = ""
        End If
    End Sub

    Public Function ImageType() As LoadedImageType
        Dim FileExt = Path.GetExtension(FileName).ToLower

        If FileExt = ".tc" Then
            Return LoadedImageType.TranscopyImage
        ElseIf FileExt = ".psi" Then
            Return LoadedImageType.PSIImage
        Else
            Return LoadedImageType.SectorImage
        End If
    End Function

    Public Function GetSaveFile() As String
        Dim FilePath As String

        If _Compressed Then
            FilePath = Path.Combine(Path.GetDirectoryName(_SourceFile), _CompressedFile)
        Else
            FilePath = _SourceFile
        End If

        If ImageType() = LoadedImageType.TranscopyImage Then
            FilePath = Path.GetFileNameWithoutExtension(FilePath) & ".ima"
        ElseIf ImageType() = LoadedImageType.PSIImage Then
            FilePath = Path.GetFileNameWithoutExtension(FilePath) & ".ima"
        End If

        Return FilePath
    End Function
    Public Overrides Function ToString() As String
        Return Right(DisplayPath, Len(DisplayPath) - _StringOffset).Replace("\", "  >  ") '& IIf(_Modified, " *", "")
    End Function
End Class