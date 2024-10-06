Namespace DiskImage
    Public MustInherit Class DirectoryBase
        Implements IDirectory
        Private ReadOnly _DirectoryData As DirectoryData
        Private ReadOnly _DirectoryEntries As List(Of DirectoryEntry)
        Private ReadOnly _Disk As Disk
        Private ReadOnly _ParentEntry As DirectoryEntry

        Public Sub New(Disk As Disk, ParentEntry As DirectoryEntry)
            Dim Level As UInteger
            If ParentEntry Is Nothing Then
                Level = 0
            Else
                Level = ParentEntry.ParentDirectory.Data.Level + 1
            End If
            _DirectoryData = New DirectoryData(Level)
            _DirectoryEntries = New List(Of DirectoryEntry)
            _Disk = Disk
            _ParentEntry = ParentEntry
        End Sub

        Public MustOverride ReadOnly Property ClusterChain As List(Of UShort) Implements IDirectory.ClusterChain

        Public ReadOnly Property Data As DirectoryData Implements IDirectory.Data
            Get
                Return _DirectoryData
            End Get
        End Property

        Public ReadOnly Property DirectoryEntries As List(Of DirectoryEntry) Implements IDirectory.DirectoryEntries
            Get
                Return _DirectoryEntries
            End Get
        End Property

        Public ReadOnly Property Disk As Disk Implements IDirectory.Disk
            Get
                Return _Disk
            End Get
        End Property

        Public ReadOnly Property IsRootDirectory As Boolean Implements IDirectory.IsRootDirectory
            Get
                Return _ParentEntry Is Nothing
            End Get
        End Property

        Public ReadOnly Property ParentEntry As DirectoryEntry Implements IDirectory.ParentEntry
            Get
                Return _ParentEntry
            End Get
        End Property

        Public MustOverride ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain

        Public MustOverride Function GetContent() As Byte() Implements IDirectory.GetContent

        Public Function HasFile(Filename As String) As Integer Implements IDirectory.HasFile
            If _DirectoryData.EntryCount > 0 Then
                For Counter As UInteger = 0 To _DirectoryData.EntryCount - 1
                    Dim File = _DirectoryEntries.Item(Counter)
                    If Not File.IsDeleted And Not File.IsVolumeName And Not File.IsDirectory Then
                        If File.GetFullFileName = Filename Then
                            Return Counter
                        End If
                    End If
                Next
            End If

            Return -1
        End Function

        Public Function GetFile(Index As UInteger) As DirectoryEntry Implements IDirectory.GetFile
            Return _DirectoryEntries.Item(Index)
        End Function

        Public Function GetNextAvailableEntry() As DirectoryEntry Implements IDirectory.GetNextAvailableEntry
            Dim Buffer(10) As Byte

            For Each DirectoryEntry In _DirectoryEntries
                If DirectoryEntry.Data(0) = 0 Then
                    Return DirectoryEntry
                End If

                Array.Copy(DirectoryEntry.Data, 0, Buffer, 0, Buffer.Length)
                If Buffer.CompareTo(DirectoryEntry.EmptyDirectoryEntry) Then
                    Return DirectoryEntry
                End If
            Next

            Return Nothing
        End Function

        Public Shared Sub GetDirectoryData(RootDirectory As RootDirectory, ParentDirectory As IDirectory, OffsetStart As UInteger, OffsetEnd As UInteger, CheckBootSector As Boolean)
            Dim EntryCount = (OffsetEnd - OffsetStart) \ DirectoryEntry.DIRECTORY_ENTRY_SIZE

            ParentDirectory.Data.MaxEntries += EntryCount

            If EntryCount > 0 Then
                For Entry As UInteger = 0 To EntryCount - 1
                    Dim Offset = OffsetStart + (Entry * DirectoryEntry.DIRECTORY_ENTRY_SIZE)

                    Dim FirstByte = RootDirectory.Disk.Image.Data.GetByte(Offset)
                    If FirstByte = 0 Then
                        ParentDirectory.Data.EndOfDirectory = True
                    End If
                    If Not ParentDirectory.Data.HasBootSector And CheckBootSector Then
                        If BootSector.ValidJumpInstructuon.Contains(FirstByte) Then
                            If OffsetEnd - Offset >= BootSector.BOOT_SECTOR_SIZE Then
                                Dim BootSectorData = RootDirectory.Disk.Image.Data.GetBytes(Offset, DiskImage.BootSector.BOOT_SECTOR_SIZE)
                                Dim BootSector = New BootSector(BootSectorData)
                                If BootSector.BPB.IsValid Then
                                    ParentDirectory.Data.HasBootSector = True
                                    ParentDirectory.Data.BootSectorOffset = Offset
                                    ParentDirectory.Data.EndOfDirectory = True
                                End If
                            End If
                        End If
                    End If
                    If ParentDirectory.Data.EndOfDirectory Then
                        If Not ParentDirectory.Data.HasAdditionalData Then
                            If Not ParentDirectory.Data.HasBootSector Or Offset < ParentDirectory.Data.BootSectorOffset Or Offset > ParentDirectory.Data.BootSectorOffset + DiskImage.BootSector.BOOT_SECTOR_SIZE Then
                                If DirectoryEntryHasData(RootDirectory.Disk.Image.Data, Offset) Then
                                    ParentDirectory.Data.HasAdditionalData = True
                                End If
                            End If
                        End If
                    Else
                        'If Not RootDirectory.DirectoryEntries.ContainsKey(Offset) Then
                        '    RootDirectory.DirectoryEntries.Add(Offset, New DirectoryEntry(RootDirectory, ParentDirectory, Offset, Entry))
                        'End If
                        ParentDirectory.Data.EntryCount += 1
                        If RootDirectory.Disk.Image.Data.GetByte(Offset + 11) <> &HF Then 'Exclude LFN entries
                            Dim FilePart = RootDirectory.Disk.Image.Data.ToUInt16(Offset)
                            If FilePart <> &H202E And FilePart <> &H2E2E Then 'Exclude '.' and '..' entries
                                ParentDirectory.Data.FileCount += 1
                                If FirstByte = DirectoryEntry.CHAR_DELETED Then
                                    ParentDirectory.Data.DeletedFileCount += 1
                                End If
                            End If
                        End If
                    End If

                    Dim NewDirectoryEntry = New DirectoryEntry(RootDirectory, ParentDirectory, Offset, Entry, ParentDirectory.Data.EndOfDirectory)

                    ParentDirectory.DirectoryEntries.Add(NewDirectoryEntry)

                    If Not RootDirectory.AllDirectoryEntries.ContainsKey(Offset) Then
                        RootDirectory.AllDirectoryEntries.Item(Offset) = NewDirectoryEntry
                    End If
                Next
            End If
        End Sub
    End Class
End Namespace
