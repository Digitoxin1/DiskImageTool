Imports System.IO

Namespace DiskImage
    Public Class Disk
        Private WithEvents FileBytes As ImageByteArray
        Private ReadOnly _BootSector As BootSector
        Private ReadOnly _Directory As RootDirectory
        Private ReadOnly _FAT12 As FAT12
        Private ReadOnly _FilePath As String
        Private ReadOnly _LoadError As Boolean = False
        Private ReadOnly _ReadOnly As Boolean = False
        Private _ReinitializeRequired As Boolean = False
        Sub New(FilePath As String, Optional Modifications As Stack(Of DataChange()) = Nothing)
            _LoadError = False
            _FilePath = FilePath
            Try
                Dim fInfo As New FileInfo(FilePath)
                _ReadOnly = fInfo.IsReadOnly
                FileBytes = New ImageByteArray(IO.File.ReadAllBytes(FilePath))
            Catch ex As Exception
                _LoadError = True
            End Try

            If Not _LoadError Then
                _BootSector = New BootSector(FileBytes)
                _FAT12 = New FAT12(FileBytes, _BootSector, 0)
                _Directory = New RootDirectory(FileBytes, _BootSector, _FAT12, False)

                CacheDirectoryEntries()

                If Modifications IsNot Nothing Then
                    FileBytes.ApplyModifications(Modifications)

                    If ReinitializeRequired Then
                        Reinitialize()
                    End If
                End If

            End If
        End Sub

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

        Public ReadOnly Property FAT As FAT12
            Get
                Return _FAT12
            End Get
        End Property

        Public ReadOnly Property FilePath As String
            Get
                Return _FilePath
            End Get
        End Property

        Public ReadOnly Property LoadError As Boolean
            Get
                Return _LoadError
            End Get
        End Property

        Public ReadOnly Property [ReadOnly] As Boolean
            Get
                Return _ReadOnly
            End Get
        End Property

        Public ReadOnly Property ReinitializeRequired As Boolean
            Get
                Return _ReinitializeRequired
            End Get
        End Property

        Public Function CompareFATTables() As Boolean
            If _BootSector.NumberOfFATs < 2 Then
                Return True
            End If

            For Counter As UShort = 1 To _BootSector.NumberOfFATs - 1
                Dim FATCopy1 = _FileBytes.GetSectors(FAT12.GetFATSectors(_BootSector.FATRegionStart, _BootSector.SectorsPerFAT, Counter - 1))
                Dim FATCopy2 = _FileBytes.GetSectors(FAT12.GetFATSectors(_BootSector.FATRegionStart, _BootSector.SectorsPerFAT, Counter))

                If Not ByteArrayCompare(FATCopy1, FATCopy2) Then
                    Return False
                End If
            Next

            Return True
        End Function

        Public Function GetDirectoryEntryByOffset(Offset As UInteger) As DirectoryEntry
            Return New DirectoryEntry(FileBytes, _BootSector, _FAT12, Offset)
        End Function

        Public Function GetVolumeLabel() As DirectoryEntry
            Dim VolumeLabel As DirectoryEntry = Nothing
            Dim DirectoryEntryCount = _Directory.Data.EntryCount

            If DirectoryEntryCount > 0 Then
                For Counter As UInteger = 0 To DirectoryEntryCount - 1
                    Dim File = _Directory.GetFile(Counter)
                    If Not File.IsDeleted And File.IsVolumeName Then
                        VolumeLabel = File
                        Exit For
                    End If
                Next
            End If

            Return VolumeLabel
        End Function

        Public Function HasInvalidSize() As Boolean
            Dim ReportedSize = SectorToBytes(_BootSector.DataRegionStart + _BootSector.DataRegionSize)
            Return ReportedSize <> Data.Length
        End Function

        Public Function FixImageSize() As Boolean
            Dim Result As Boolean = False

            Dim ReportedSize = SectorToBytes(_BootSector.DataRegionStart + _BootSector.DataRegionSize)

            If ReportedSize <> Data.Length Then
                Result = Data.Resize(ReportedSize)
            End If

            Return Result
        End Function

        Private Function IsFATRegion(Offset As UInteger, Length As UInteger) As Boolean
            Dim FATSectorStart = _BootSector.FATRegionStart
            Dim FATSectorEnd = _BootSector.FATRegionStart + (_BootSector.SectorsPerFAT * _BootSector.NumberOfFATs) - 1

            Dim SectorStart = OffsetToSector(Offset)
            Dim SectorEnd = OffsetToSector(Offset + Length - 1)

            Return (SectorStart >= FATSectorStart And SectorStart <= FATSectorEnd) Or (SectorEnd >= FATSectorStart And SectorEnd <= FATSectorEnd)
        End Function

        Public Function IsValidImage() As Boolean
            Return Not _LoadError AndAlso _BootSector.IsValidImage
        End Function

        Public Function CheckSize() As Boolean
            Return Not _LoadError AndAlso (FileBytes.Length > 0 And FileBytes.Length < 4423680)
        End Function

        Public Sub Reinitialize()
            _FAT12.PopulateFAT12()
            _Directory.RefreshData()

            _ReinitializeRequired = False
        End Sub
        Public Sub SaveFile(FilePath As String)
            IO.File.WriteAllBytes(FilePath, FileBytes.Data)
            FileBytes.ClearChanges()
            CacheDirectoryEntries()
        End Sub

        Private Sub CacheDirectoryEntries()
            FileBytes.DirectoryCache.Clear()
            If _BootSector.IsValidImage Then
                CacheDirectoryEntries(_Directory)
            End If
        End Sub

        Private Sub CacheDirectoryEntries(Directory As DiskImage.IDirectory)
            Dim DirectoryEntryCount = Directory.Data.EntryCount

            If DirectoryEntryCount > 0 Then
                For Counter = 0 To DirectoryEntryCount - 1
                    Dim File = Directory.GetFile(Counter)

                    If Not File.IsLink And Not File.IsVolumeName Then
                        FileBytes.DirectoryCache.Item(File.Offset) = File.Data
                        If File.IsDirectory And File.SubDirectory IsNot Nothing Then
                            If File.SubDirectory.Data.EntryCount > 0 Then
                                CacheDirectoryEntries(File.SubDirectory)
                            End If
                        End If
                    End If
                Next
            End If
        End Sub

        Private Sub FileBytes_DataChanged(Offset As UInteger, OriginalValue As Object, NewValue As Object) Handles FileBytes.DataChanged
            If BootSector.IsBootSectorRegion(Offset) Then
                _ReinitializeRequired = True
            ElseIf _BootSector.IsValidImage AndAlso IsFATRegion(Offset, GetObjectSize(NewValue)) Then
                _ReinitializeRequired = True
            End If
        End Sub
    End Class

End Namespace