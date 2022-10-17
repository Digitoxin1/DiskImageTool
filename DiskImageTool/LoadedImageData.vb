Public Class ImageDataScanInfo
    Public Property OEMIDFound As Boolean = False
    Public Property OEMIDMatched As Boolean = False
    Public Property IsValidImage As Boolean = False
    Public Property HasCreated As Boolean = False
    Public Property HasLastAccessed As Boolean = False
    Public Property HasLongFileNames As Boolean = False
    Public Property HasInvalidDirectoryEntries As Boolean = False
    Public Property HasUnusedClusters As Boolean = False
End Class
Public Class LoadedImageData
    Public Shared Property StringOffset As Integer = 0
    Private _FilePath As String
    Private _Modified As Boolean
    Private _Modifications As Hashtable
    Private _Scanned As Boolean
    Private ReadOnly _ScanInfo As New ImageDataScanInfo
    Private _ComboIndex As Integer

    Public Property ComboIndex As Integer
        Get
            Return _ComboIndex
        End Get
        Set
            _ComboIndex = Value
        End Set
    End Property

    Public Property FilePath As String
        Get
            Return _FilePath
        End Get
        Set
            _FilePath = Value
        End Set
    End Property

    Public Property Modified As Boolean
        Get
            Return _Modified
        End Get
        Set
            _Modified = Value
        End Set
    End Property

    Public Property Modifications As Hashtable
        Get
            Return _Modifications
        End Get
        Set
            _Modifications = Value
        End Set
    End Property

    Public Property Scanned As Boolean
        Get
            Return _Scanned
        End Get
        Set
            _Scanned = Value
        End Set
    End Property

    Public ReadOnly Property ScanInfo As ImageDataScanInfo
        Get
            Return _ScanInfo
        End Get
    End Property

    Public Sub New(FilePath As String)
        _FilePath = FilePath
        _Modified = False
        _Scanned = False
        _Modifications = Nothing
        _ComboIndex = -1
    End Sub
    Public Overrides Function ToString() As String
        Return Right(_FilePath, Len(_FilePath) - _StringOffset) & IIf(_Modified, " *", "")
    End Function
End Class


