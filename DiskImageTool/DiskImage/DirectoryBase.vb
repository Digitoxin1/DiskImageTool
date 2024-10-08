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

        Public Shared Sub GetDirectoryData(RootDirectory As RootDirectory, ParentDirectory As IDirectory, OffsetStart As UInteger, OffsetEnd As UInteger, CheckBootSector As Boolean)
            Dim EntryCount = (OffsetEnd - OffsetStart) \ DirectoryEntry.DIRECTORY_ENTRY_SIZE

            If EntryCount > 0 Then
                Dim Buffer() As Byte
                For Entry As UInteger = 0 To EntryCount - 1
                    Dim Offset = OffsetStart + (Entry * DirectoryEntry.DIRECTORY_ENTRY_SIZE)

                    Buffer = RootDirectory.Disk.Image.Data.GetBytes(Offset, 11)
                    If Buffer(0) = 0 Then
                        ParentDirectory.Data.EndOfDirectory = True
                    End If
                    If Not ParentDirectory.Data.HasBootSector And CheckBootSector Then
                        If BootSector.ValidJumpInstructuon.Contains(Buffer(0)) Then
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
                        ParentDirectory.Data.AvailableEntries += 1

                        If Not ParentDirectory.Data.HasAdditionalData Then
                            If Not ParentDirectory.Data.HasBootSector Or Offset < ParentDirectory.Data.BootSectorOffset Or Offset > ParentDirectory.Data.BootSectorOffset + DiskImage.BootSector.BOOT_SECTOR_SIZE Then
                                If DirectoryEntryHasData(RootDirectory.Disk.Image.Data, Offset) Then
                                    ParentDirectory.Data.HasAdditionalData = True
                                End If
                            End If
                        End If
                    Else
                        If Buffer.CompareTo(DirectoryEntry.EmptyDirectoryEntry) Then
                            ParentDirectory.Data.AvailableEntries += 1
                        End If
                        ParentDirectory.Data.EntryCount += 1
                        If RootDirectory.Disk.Image.Data.GetByte(Offset + 11) <> &HF Then 'Exclude LFN entries
                            Dim FilePart = RootDirectory.Disk.Image.Data.ToUInt16(Offset)
                            If FilePart <> &H202E And FilePart <> &H2E2E Then 'Exclude '.' and '..' entries
                                ParentDirectory.Data.FileCount += 1
                                If Buffer(0) = DirectoryEntry.CHAR_DELETED Then
                                    ParentDirectory.Data.DeletedFileCount += 1
                                End If
                            End If
                        End If
                    End If

                    Dim NewDirectoryEntry = New DirectoryEntry(RootDirectory, ParentDirectory, Offset, ParentDirectory.Data.EndOfDirectory)

                    ParentDirectory.DirectoryEntries.Add(NewDirectoryEntry)
                Next
            End If
        End Sub

        Public MustOverride Function AddFile(FilePath As String, LFN As Boolean, Optional Index As Integer = -1) As Boolean Implements IDirectory.AddFile

        Public MustOverride Function AddFile(FilePath As String, LFN As Boolean, ClusterList As SortedSet(Of UShort), Optional Index As Integer = -1) As Boolean Implements IDirectory.AddFile

        Public Function AdjustIndexForLFN(Index As Integer) As Integer
            If Index < 1 Then
                Return Index
            End If

            Dim PrevEntry = GetFile(Index - 1)
            Do While PrevEntry.IsLFN
                Index -= 1
                If Index = 0 Then
                    Exit Do
                End If
                PrevEntry = GetFile(Index - 1)
            Loop

            Return Index
        End Function

        Public Function GetAvailableEntries(Count As UInteger) As List(Of DirectoryEntry) Implements IDirectory.GetAvailableEntries
            Dim Entries As New List(Of DirectoryEntry)

            Dim Buffer(10) As Byte


            Dim Index As UInteger = 0

            For Each DirectoryEntry In _DirectoryEntries
                Dim Found As Boolean = False

                If Index > Data.EntryCount - 1 Then
                    Entries.Add(DirectoryEntry)
                    Found = True
                Else
                    Array.Copy(DirectoryEntry.Data, 0, Buffer, 0, Buffer.Length)
                    If Buffer.CompareTo(DirectoryEntry.EmptyDirectoryEntry) Then
                        Entries.Add(DirectoryEntry)
                        Found = True
                    End If
                End If

                If Not Found Then
                    Entries.Clear()
                ElseIf Entries.Count = Count Then
                    Return Entries
                End If

                Index += 1
            Next

            Return Nothing
        End Function

        Public Function GetAvailableEntry() As DirectoryEntry Implements IDirectory.GetAvailableEntry
            Dim Buffer(10) As Byte

            Dim Index As UInteger = 0
            For Each DirectoryEntry In _DirectoryEntries
                If Index > Data.EntryCount - 1 Then
                    Return DirectoryEntry
                End If

                Array.Copy(DirectoryEntry.Data, 0, Buffer, 0, Buffer.Length)
                If Buffer.CompareTo(DirectoryEntry.EmptyDirectoryEntry) Then
                    Return DirectoryEntry
                End If

                Index += 1
            Next

            Return Nothing
        End Function

        Public Function GetAvailableFileName(FileName As String) As String Implements IDirectory.GetAvailableFileName
            FileName = DOSCleanFileName(FileName)

            Dim FilePart As String
            Dim Extension As String = IO.Path.GetExtension(FileName)
            FileName = IO.Path.GetFileNameWithoutExtension(FileName)

            If Extension.Length > 4 Then
                Extension = Extension.Substring(0, 4)
            End If

            Dim Index As UInteger = 1

            If FileName.Length > 8 Then
                FilePart = TruncateFileName(FileName, Index)
                Index += 1
            Else
                FilePart = FileName
            End If

            Dim NewFileName = FilePart & Extension

            Do While GetFileIndex(NewFileName, True) > -1
                FilePart = TruncateFileName(FileName, Index)
                Index += 1
                NewFileName = FilePart & Extension
            Loop

            Return NewFileName
        End Function

        Public MustOverride Function GetContent() As Byte() Implements IDirectory.GetContent
        Public Function GetEntries(Index As UInteger, Count As UInteger) As List(Of DirectoryEntry)
            Dim Entries As New List(Of DirectoryEntry)

            For Counter = Index To Index + Count - 1
                Entries.Add(_DirectoryEntries.Item(Counter))
            Next

            Return Entries
        End Function

        Public Function GetFile(Index As UInteger) As DirectoryEntry Implements IDirectory.GetFile
            Return _DirectoryEntries.Item(Index)
        End Function

        Public Function GetFileIndex(Filename As String, IncludeDirectories As Boolean) As Integer Implements IDirectory.GetFileIndex
            If _DirectoryData.EntryCount > 0 Then
                For Counter As UInteger = 0 To _DirectoryData.EntryCount - 1
                    Dim File = _DirectoryEntries.Item(Counter)
                    If Not File.IsDeleted And Not File.IsVolumeName And (IncludeDirectories Or Not File.IsDirectory) Then
                        If File.GetFullFileName = Filename Then
                            Return Counter
                        End If
                    End If
                Next
            End If

            Return -1
        End Function

        Public Function GetIndex(DirectoryEntry As DirectoryEntry) As Integer Implements IDirectory.GetIndex
            Return _DirectoryEntries.IndexOf(DirectoryEntry)
        End Function

        Public Overridable Function RemoveEntry(Index As UInteger) As Boolean Implements IDirectory.RemoveEntry
            Dim DirectoryEntry = _DirectoryEntries.Item(Index)

            If Not DirectoryEntry.IsDeleted Then
                Return False
            End If

            Dim NewIndex = AdjustIndexForLFN(Index)
            Dim EntryCount = Index - NewIndex + 1

            ShiftEntries(NewIndex, EntryCount * -1)

            _DirectoryData.AvailableEntries += EntryCount
            _DirectoryData.EntryCount -= EntryCount

            Return True
        End Function

        Public Sub ShiftEntries(StartIndex As UInteger, Offset As Integer)
            Dim UseTransaction As Boolean = _Disk.BeginTransaction

            If Offset > 0 Then
                For Counter = _DirectoryData.EntryCount - 1 To StartIndex Step -1
                    Dim DirectoryEntry = _DirectoryEntries(Counter)
                    Dim NewDirectoryEntry = _DirectoryEntries(Counter + Offset)
                    NewDirectoryEntry.Data = DirectoryEntry.Data
                Next
            ElseIf Offset < 0 Then
                For Counter = StartIndex To (_DirectoryData.EntryCount + Offset) - 1
                    Dim DirectoryEntry = _DirectoryEntries(Counter - Offset)
                    Dim NewDirectoryEntry = _DirectoryEntries(Counter)
                    NewDirectoryEntry.Data = DirectoryEntry.Data
                Next
                For Counter = _DirectoryData.EntryCount + Offset To _DirectoryData.EntryCount - 1
                    Dim buffer = New Byte(31) {}
                    Dim DirectoryEntry = _DirectoryEntries(Counter)
                    DirectoryEntry.Data = buffer
                Next
            End If

            If UseTransaction Then
                _Disk.EndTransaction()
            End If
        End Sub

        Private Function TruncateFileName(FilePart As String, Index As UInteger) As String
            Dim Suffix = "~" & Index
            Return FilePart.Substring(0, 8 - Suffix.Length) & Suffix
        End Function
    End Class
End Namespace
