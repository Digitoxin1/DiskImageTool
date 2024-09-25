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
        Private ReadOnly _FATTables As FATTables
        Private _DiskType As FloppyDiskType

        Sub New(Data() As Byte, FatIndex As UShort, Optional Modifications As Stack(Of DataChange()) = Nothing)
            FileBytes = New ImageByteArray(New ByteArray(Data))
            _DirectoryCache = New Dictionary(Of UInteger, DirectoryCacheEntry)
            _BootSector = New BootSector(FileBytes.Data, BootSector.BOOT_SECTOR_OFFSET)
            SetBPB()

            _FATTables = New FATTables(_BPB, FileBytes, FatIndex)
            _DiskType = InferFloppyDiskType()
            _FATTables.SyncFATs = Not IsDiskTypeXDF(_DiskType)

            _Directory = New RootDirectory(FileBytes, _BPB, _FATTables, _DirectoryCache, False)

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

        Public ReadOnly Property Image As ImageByteArray
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

        Public ReadOnly Property DiskType As FloppyDiskType
            Get
                Return _DiskType
            End Get
        End Property

        Public ReadOnly Property FATTables As FATTables
            Get
                Return _FATTables
            End Get
        End Property

        Public ReadOnly Property FAT As FAT12
            Get
                Return _FATTables.FAT
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
            Return FileBytes.Length.CompareTo(ReportedSize)
        End Function

        Public Function CheckSize() As Boolean
            Return (FileBytes.Length > 0 And FileBytes.Length < 4423680)
        End Function

        Public Function FixImageSize() As Boolean
            Dim Result As Boolean = False

            Dim ReportedSize = _BPB.ReportedImageSize()

            If ReportedSize <> FileBytes.Length Then
                FileBytes.BatchEditMode = True
                If ReportedSize < FileBytes.Length Then
                    Dim b(FileBytes.Length - ReportedSize - 1) As Byte
                    For i = 0 To b.Length - 1
                        b(i) = 0
                    Next
                    FileBytes.SetBytes(b, FileBytes.Length - b.Length)
                End If
                FileBytes.Resize(ReportedSize)
                FileBytes.BatchEditMode = False
                Result = True
            End If

            Return Result
        End Function

        Public Function GetDirectoryEntryByOffset(Offset As UInteger) As DirectoryEntry
            Return New DirectoryEntry(FileBytes, _BPB, _FATTables, _DirectoryCache, Offset)
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
                    If File.IsValidVolumeName Then
                        VolumeLabel = File
                        Exit For
                    End If
                Next
            End If

            Return VolumeLabel
        End Function

        Public Function GetXDFChecksum() As UInteger
            Return FileBytes.GetBytesInteger(&H13C)
        End Function

        Public Function IsValidImage(Optional CheckBPB As Boolean = True) As Boolean
            Return _FileBytes.Length >= 512 And (Not CheckBPB OrElse _BPB.IsValid)
        End Function

        Public Sub Reinitialize()
            SetBPB()

            _FATTables.Reinitialize(_BPB)
            _DiskType = InferFloppyDiskType()
            _FATTables.SyncFATs = Not IsDiskTypeXDF(_DiskType)
            _Directory.RefreshData(_BPB)
        End Sub

        Public Sub SaveFile(FilePath As String)
            IO.File.WriteAllBytes(FilePath, FileBytes.GetBytes)
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

        Private Function InferFloppyDiskType() As FloppyDiskType
            Dim DiskType As FloppyDiskType

            If _BPB.IsValid Then
                DiskType = GetFloppyDiskType(_BPB, False)
            Else
                DiskType = GetFloppyDiskType(_FATTables.FAT.MediaDescriptor)
                If DiskType = FloppyDiskType.FloppyUnknown Then
                    DiskType = GetFloppyDiskType(_FileBytes.Length)
                End If
            End If

            Return DiskType
        End Function

        Private Sub SetBPB()
            If _BootSector.BPB.IsValid Then
                _BPB = _BootSector.BPB
            Else
                Dim FATMediaDescriptor = GetFATMediaDescriptor()

                Dim DiskTypeFAT = GetFloppyDiskType(FATMediaDescriptor)
                Dim DiskTypeSize = GetFloppyDiskType(_FileBytes.Length)

                If DiskTypeFAT = FloppyDiskType.Floppy360 And DiskTypeSize = FloppyDiskType.Floppy180 Then
                    FATMediaDescriptor = GetFloppyDiskMediaDescriptor(DiskTypeSize)
                ElseIf DiskTypeFAT = FloppyDiskType.Floppy320 And DiskTypeSize = FloppyDiskType.Floppy160 Then
                    FATMediaDescriptor = GetFloppyDiskMediaDescriptor(DiskTypeSize)
                End If

                _BPB = BuildBPB(FATMediaDescriptor)
            End If
        End Sub
    End Class

End Namespace