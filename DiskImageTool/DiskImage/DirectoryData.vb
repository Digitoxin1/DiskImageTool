Namespace DiskImage
    Public Class DirectoryData
        Public Sub New(Level As UInteger)
            _Level = Level
            Clear()
        End Sub

        Public Property AvailableEntryCount As UInteger
        Public Property BootSectorOffset As UInteger
        Public Property DeletedFileCount As UInteger
        Public Property EndOfDirectory As Boolean
        Public Property EntryCount As UInteger
        Public Property FileCount As UInteger
        Public Property HasAdditionalData As Boolean
        Public Property HasBootSector As Boolean
        Public ReadOnly Property Level As UInteger

        Public Sub Clear()
            _AvailableEntryCount = 0
            _BootSectorOffset = 0
            _DeletedFileCount = 0
            _EntryCount = 0
            _FileCount = 0
            _HasAdditionalData = False
            _HasBootSector = False
            _EndOfDirectory = False
        End Sub
    End Class
End Namespace
