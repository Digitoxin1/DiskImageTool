Public Class ImageData
    Public Enum FileTypeEnum
        Standard
        Compressed
        NewImage
    End Enum

    Private ReadOnly _Filters() As Boolean

    Public Sub New(SourceFile As String)
        _AppliedFilters = 0
        _BatchUpdated = False
        _BottomIndex = -1
        _CompressedFile = ""
        _DiskType = ""
        _FATIndex = 0
        _FileType = FileTypeEnum.Standard
        _SourceFile = SourceFile
        _Modifications = Nothing
        _NewFileName = ""
        _OEMName = ""
        _ReadOnly = False
        _Scanned = False
        _SortHistory = Nothing
        _InvalidImage = False
        _Loaded = False
        _Checksum = 0
        _ExternalModified = False
        _XDFMiniDisk = False
        _XDFOffset = 0
        _XDFLength = 0

        Dim FilterCount As Integer = Filters.FilterGetCount()
        ReDim _Filters(FilterCount - 1)
        For Counter = 0 To FilterCount - 1
            _Filters(Counter) = False
        Next
    End Sub

    Public Property [ReadOnly] As Boolean
    Public Property AppliedFilters As Long
    Public Property BatchUpdated As Boolean
    Public Property BottomIndex As Integer
    Public Property Checksum As UInteger
    Public Property CompressedFile As String
    Public Property DiskType As String
    Public Property ExternalModified As Boolean
    Public Property FATIndex As UShort
    Public Property FileType As FileTypeEnum
    Public Property InvalidImage As Boolean
    Public Property Loaded As Boolean
    Public Property Modifications As Stack(Of DiskImage.DataChange())
    Public Property NewFileName As String
    Public Property OEMName As String
    Public Property Scanned As Boolean
    Public Property SortHistory As List(Of SortEntity)
    Public Property SourceFile As String
    Public Shared Property StringOffset As Integer = 0
    Public Property XDFLength As UInteger
    Public Property XDFMiniDisk As Boolean
    Public Property XDFOffset As UInteger

    Public Property Filter(FilterType As Filters.FilterTypes) As Boolean
        Get
            Return _Filters(FilterType)
        End Get

        Set(value As Boolean)
            _Filters(FilterType) = value
        End Set
    End Property

    Public Function DisplayPath() As String
        Dim FullPath = _SourceFile
        If _FileType = FileTypeEnum.Compressed Then
            FullPath = IO.Path.Combine(FullPath, Replace(_CompressedFile, "/", "\"))
        ElseIf _filetype = FileTypeEnum.NewImage Then
            If _NewFileName = "" Then
                FullPath = IO.Path.GetFileName(FullPath)
            Else
                FullPath = _NewFileName
            End If
        End If
            Return FullPath
    End Function

    Public Function FileName() As String
        If _FileType = FileTypeEnum.Compressed Then
            Return IO.Path.GetFileName(_CompressedFile)
        ElseIf _filetype = FileTypeEnum.NewImage AndAlso _newfilename <> "" Then
            Return _NewFileName
        Else
            Return IO.Path.GetFileName(_SourceFile)
        End If
    End Function

    Public Function IsModified() As Boolean
        Return Filter(Filters.FilterTypes.ModifiedFiles)
    End Function

    Public Function GetSaveFile() As String
        Dim FilePath As String

        If _FileType = FileTypeEnum.Compressed Then
            FilePath = IO.Path.Combine(IO.Path.GetDirectoryName(_SourceFile), _CompressedFile)
        ElseIf _filetype = FileTypeEnum.NewImage AndAlso _NewFileName <> "" Then
            FilePath = _NewFileName
        Else
            FilePath = _SourceFile
        End If

        'Dim ImageType = GetImageTypeFromFileName(FileName)

        'If ImageType = FloppyImageType.TranscopyImage Then
        '    FilePath = Path.GetFileNameWithoutExtension(FilePath) & ".ima"
        'End If

        Return FilePath
    End Function

    Public Overrides Function ToString() As String
        Return Right(DisplayPath, Len(DisplayPath) - _StringOffset).Replace("\", "  >  ") '& If(_Modified, " *", "")
    End Function

End Class