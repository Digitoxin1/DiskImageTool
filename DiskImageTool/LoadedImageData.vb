Public Class ImageDataScanInfo
    Public Property HasCreated As Boolean = False
    Public Property HasInvalidDirectoryEntries As Boolean = False
    Public Property HasLastAccessed As Boolean = False
    Public Property HasLongFileNames As Boolean = False
    Public Property HasUnusedClusters As Boolean = False
    Public Property IsValidImage As Boolean = False
    Public Property OEMIDFound As Boolean = False
    Public Property OEMIDMatched As Boolean = False
End Class

Public Class LoadedImageData
    Public Shared Property StringOffset As Integer = 0
    Public Property CachedRootDir As Byte()
    Public Property ComboIndex As Integer
    Public Property FilePath As String
    Public Property Modifications As Hashtable
    Public Property Modified As Boolean
    Public ReadOnly Property ScanInfo As ImageDataScanInfo
    Public Property Scanned As Boolean
    Public ReadOnly Property SessionModifications As Hashtable

    Public Sub New(FilePath As String)
        _ComboIndex = -1
        _FilePath = FilePath
        _Modifications = Nothing
        _Modified = False
        _ScanInfo = New ImageDataScanInfo
        _Scanned = False
        _SessionModifications = New Hashtable()
    End Sub
    Public Sub UpdateSessionModifications(Modifications As Hashtable)
        For Each Offset As UInteger In Modifications.Keys
            _SessionModifications.Item(Offset) = Modifications.Item(Offset)
        Next
    End Sub
    Public Overrides Function ToString() As String
        Return Right(_FilePath, Len(_FilePath) - _StringOffset) & IIf(_Modified, " *", "")
    End Function
End Class


