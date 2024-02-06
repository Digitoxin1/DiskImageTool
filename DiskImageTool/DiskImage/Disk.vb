Namespace DiskImage
    Public Structure DirectoryCacheEntry
        Dim Checksum As UInteger
        Dim Data() As Byte
    End Structure

    Public Class Disk
        Private WithEvents FileBytes As ImageByteArray
        Public Const BYTES_PER_SECTOR As UShort = 512
        Private ReadOnly _BootSector As BootSector
        Private _BPB As BiosParameterBlock
        Private ReadOnly _Directory As RootDirectory
        Private ReadOnly _DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry)
        Private ReadOnly _FAT12 As FAT12

        Sub New(Data() As Byte, FatIndex As UShort, Optional Modifications As Stack(Of DataChange()) = Nothing)
            FileBytes = New ImageByteArray(Data)
            _DirectoryCache = New Dictionary(Of UInteger, DirectoryCacheEntry)
            _BootSector = New BootSector(FileBytes)
            If _BootSector.BPB.IsValid Then
                _BPB = _BootSector.BPB
            Else
                _BPB = BuildBPB(GetFATMediaDescriptor)
            End If
            _FAT12 = New FAT12(FileBytes, _BPB, FatIndex)
            _Directory = New RootDirectory(FileBytes, _BPB, _FAT12, _DirectoryCache, False)

            CacheDirectoryEntries()

            If Modifications IsNot Nothing Then
                FileBytes.ApplyModifications(Modifications)
                Reinitialize()
            End If
        End Sub

        Public ReadOnly Property BPB As BiosParameterBlock
            Get
                Return _BPB
            End Get
        End Property

        Public ReadOnly Property BootSector As BootSector
            Get
                Return _BootSector
            End Get
        End Property

        Public ReadOnly Property Data As ImageByteArray
            Get
                Return FileBytes
            End Get
        End Property

        Public ReadOnly Property Directory As RootDirectory
            Get
                Return _Directory
            End Get
        End Property

        Public ReadOnly Property DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry)
            Get
                Return _DirectoryCache
            End Get
        End Property

        Public ReadOnly Property FAT As FAT12
            Get
                Return _FAT12
            End Get
        End Property

        Public Shared Function BytesToSector(Bytes As UInteger) As UInteger
            Return Math.Ceiling(Bytes / BYTES_PER_SECTOR)
        End Function

        Public Shared Function OffsetToSector(Offset As UInteger) As UInteger
            Return Offset \ BYTES_PER_SECTOR
        End Function

        Public Shared Function SectorToBytes(Sector As UInteger) As UInteger
            Return Sector * BYTES_PER_SECTOR
        End Function
        Public Function CheckImageSize() As Integer
            Dim ReportedSize As Integer = _BPB.ReportedImageSize()
            Return Data.Length.CompareTo(ReportedSize)
        End Function

        Public Function CheckSize() As Boolean
            Return (FileBytes.Length > 0 And FileBytes.Length < 4423680)
        End Function

        Public Function CompareFATTables() As Boolean
            If _BPB.NumberOfFATs < 2 Then
                Return True
            End If

            For Counter As UShort = 1 To _BPB.NumberOfFATs - 1
                Dim FATCopy1 = _FileBytes.GetSectors(FAT12.GetFATSectors(_BPB.FATRegionStart, _BPB.SectorsPerFAT, Counter - 1))
                Dim FATCopy2 = _FileBytes.GetSectors(FAT12.GetFATSectors(_BPB.FATRegionStart, _BPB.SectorsPerFAT, Counter))

                If Not FATCopy1.CompareTo(FATCopy2) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Public Function FixImageSize() As Boolean
            Dim Result As Boolean = False

            Dim ReportedSize = _BPB.ReportedImageSize()

            If ReportedSize <> Data.Length Then
                Data.BatchEditMode = True
                If ReportedSize < Data.Length Then
                    Dim b(Data.Length - ReportedSize - 1) As Byte
                    For i = 0 To b.Length - 1
                        b(i) = 0
                    Next
                    Data.SetBytes(b, Data.Length - b.Length)
                End If
                Data.Resize(ReportedSize)
                Data.BatchEditMode = False
                Result = True
            End If

            Return Result
        End Function

        Public Function GetDirectoryEntryByOffset(Offset As UInteger) As DirectoryEntry
            Return New DirectoryEntry(FileBytes, _BPB, _FAT12, _DirectoryCache, Offset)
        End Function

        Private Function GetFATMediaDescriptor() As Byte
            Dim Result As Byte = 0

            If FileBytes.Length >= 515 Then
                Dim b = FileBytes.GetBytes(512, 3)
                If b(1) = &HFF And b(2) = &HFF Then
                    Result = b(0)
                End If
            End If

            Return Result
        End Function

        Public Function GetFileList() As List(Of DirectoryEntry)
            Dim DirectoryList As New List(Of DirectoryEntry)

            EnumerateDirectoryEntries(_Directory, DirectoryList)

            Return DirectoryList
        End Function

        Public Function GetVolumeLabel() As DirectoryEntry
            Dim VolumeLabel As DirectoryEntry = Nothing
            Dim DirectoryEntryCount = _Directory.Data.EntryCount

            If DirectoryEntryCount > 0 Then
                For Counter As UInteger = 0 To DirectoryEntryCount - 1
                    Dim File = _Directory.GetFile(Counter)
                    If File.IsValidValumeName Then
                        VolumeLabel = File
                        Exit For
                    End If
                Next
            End If

            Return VolumeLabel
        End Function

        Public Function IsValidImage(Optional CheckBPB As Boolean = True) As Boolean
            Return _FileBytes.Length >= 512 And (Not CheckBPB OrElse _BPB.IsValid)
        End Function

        Public Sub Reinitialize()
            If _BootSector.BPB.IsValid Then
                _BPB = _BootSector.BPB
            Else
                _BPB = BuildBPB(GetFATMediaDescriptor)
            End If

            _FAT12.PopulateFAT12(_BPB)
            _Directory.RefreshData(_BPB)
        End Sub

        Public Sub SaveFile(FilePath As String)
            IO.File.WriteAllBytes(FilePath, FileBytes.Data)
            FileBytes.ClearChanges()
            CacheDirectoryEntries()
        End Sub

        Private Sub CacheDirectoryEntries()
            _DirectoryCache.Clear()

            If IsValidImage() Then
                CacheDirectoryEntries(_Directory)
            End If
        End Sub

        Private Sub CacheDirectoryEntries(Directory As DiskImage.IDirectory)
            Dim DirectoryEntryCount = Directory.Data.EntryCount

            If DirectoryEntryCount > 0 Then
                For Counter = 0 To DirectoryEntryCount - 1
                    Dim DirectoryEntry = Directory.GetFile(Counter)

                    If Not DirectoryEntry.IsLink Then
                        _DirectoryCache.Item(DirectoryEntry.Offset) = GetDirectoryCacheEntry(DirectoryEntry)
                        If DirectoryEntry.IsDirectory And DirectoryEntry.SubDirectory IsNot Nothing Then
                            If DirectoryEntry.SubDirectory.Data.EntryCount > 0 Then
                                CacheDirectoryEntries(DirectoryEntry.SubDirectory)
                            End If
                        End If
                    End If
                Next
            End If
        End Sub

        Private Function GetDirectoryCacheEntry(DirectoryEntry As DirectoryEntry) As DirectoryCacheEntry
            Dim CacheEntry As DirectoryCacheEntry

            CacheEntry.Data = DirectoryEntry.Data
            If DirectoryEntry.IsValidFile Then
                CacheEntry.Checksum = DirectoryEntry.GetChecksum()
            Else
                CacheEntry.Checksum = 0
            End If

            Return CacheEntry
        End Function

        Private Sub EnumerateDirectoryEntries(Directory As DiskImage.IDirectory, DirectoryList As List(Of DirectoryEntry))
            Dim DirectoryEntryCount = Directory.Data.EntryCount

            If DirectoryEntryCount > 0 Then
                For Counter = 0 To DirectoryEntryCount - 1
                    Dim DirectoryEntry = Directory.GetFile(Counter)

                    If Not DirectoryEntry.IsLink Then
                        DirectoryList.Add(DirectoryEntry)
                        If DirectoryEntry.IsDirectory And DirectoryEntry.SubDirectory IsNot Nothing Then
                            If DirectoryEntry.SubDirectory.Data.EntryCount > 0 Then
                                EnumerateDirectoryEntries(DirectoryEntry.SubDirectory, DirectoryList)
                            End If
                        End If
                    End If
                Next
            End If
        End Sub
    End Class

End Namespace