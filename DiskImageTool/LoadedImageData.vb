Public Class LoadedImageData
    Private _Path As String
    Private _File As String
    Private _Modified As Boolean
    Private _Modifications As Hashtable
    Private _Scanned As Boolean
    Private _OEMIDFound As Boolean
    Private _OEMIDMatched As Boolean
    Private _IsValidImage As Boolean
    Private _HasCreated As Boolean
    Private _HasLastAccessed As Boolean
    Private _HasLongFileNames As Boolean
    Public Property HasInvalidDirectoryEntries As Boolean
        Get
            Return _HasInvalidDirectoryEntries
        End Get
        Set
            _HasInvalidDirectoryEntries = Value
        End Set
    End Property

    Private _ComboIndex As Integer
    Private _HasInvalidDirectoryEntries As Boolean

    Public Property ComboIndex As Integer
        Get
            Return _ComboIndex
        End Get
        Set
            _ComboIndex = Value
        End Set
    End Property

    Public Property HasLongFileNames As Boolean
        Get
            Return _HasLongFileNames
        End Get
        Set
            _HasLongFileNames = Value
        End Set
    End Property

    Public Property HasCreated As Boolean
        Get
            Return _HasCreated
        End Get
        Set
            _HasCreated = Value
        End Set
    End Property

    Public Property HasLastAccessed As Boolean
        Get
            Return _HasLastAccessed
        End Get
        Set
            _HasLastAccessed = Value
        End Set
    End Property

    Public Property IsValidImage As Boolean
        Get
            Return _IsValidImage
        End Get
        Set
            _IsValidImage = Value
        End Set
    End Property

    Public Property OEMIDFound As Boolean
        Get
            Return _OEMIDFound
        End Get
        Set
            _OEMIDFound = Value
        End Set
    End Property

    Public Property OEMIDMatched As Boolean
        Get
            Return _OEMIDMatched
        End Get
        Set
            _OEMIDMatched = Value
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

    Public Sub New(Path As String, File As String)
        _Path = Path
        _File = File
        _Modified = False
        _Scanned = False
        _OEMIDFound = False
        _OEMIDMatched = False
        _IsValidImage = False
        _HasCreated = False
        _HasLastAccessed = False
        _HasLongFileNames = False
        _Modifications = Nothing
        _HasInvalidDirectoryEntries = False
        _ComboIndex = -1
    End Sub
    Public Overrides Function ToString() As String
        Return _File & IIf(_Modified, " *", "")
    End Function
End Class


