Namespace DiskImage
    Public Class DirectoryData
        Public Property MaxEntries As UInteger = 0
        Public Property EntryCount As UInteger = 0
        Public Property FileCount As UInteger = 0
        Public Property DeletedFileCount As UInteger = 0
        Public Property HasAdditionalData As Boolean = False
        Public Property HasBootSector As Boolean = False
        Public Property BootSectorOffset As UInteger = 0
    End Class
End Namespace
