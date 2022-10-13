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
    Private _Path As String
    Private _File As String
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

    Public Property Path As String
        Get
            Return _Path
        End Get
        Set
            _Path = Value
        End Set
    End Property

    Public Property File As String
        Get
            Return _File
        End Get
        Set
            _File = Value
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

    Public Sub New(Path As String, File As String)
        _Path = Path
        _File = File
        _Modified = False
        _Scanned = False
        _Modifications = Nothing
        _ComboIndex = -1
    End Sub
    Public Overrides Function ToString() As String
        Return _File & IIf(_Modified, " *", "")
    End Function
End Class


