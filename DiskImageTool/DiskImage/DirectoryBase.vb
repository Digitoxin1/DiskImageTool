﻿Namespace DiskImage
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

                    Buffer = RootDirectory.Disk.Image.GetBytes(Offset, 11)
                    If Buffer(0) = 0 Then
                        ParentDirectory.Data.EndOfDirectory = True
                    End If

                    If Not ParentDirectory.Data.HasBootSector And CheckBootSector Then
                        If BootSector.ValidJumpInstructuon.Contains(Buffer(0)) Then
                            If OffsetEnd - Offset >= BootSector.BOOT_SECTOR_SIZE Then
                                Dim BootSectorData = RootDirectory.Disk.Image.GetBytes(Offset, DiskImage.BootSector.BOOT_SECTOR_SIZE)
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
                        ParentDirectory.Data.AvailableEntryCount += 1
                        If Not ParentDirectory.Data.HasAdditionalData Then
                            If Not ParentDirectory.Data.HasBootSector Or Offset < ParentDirectory.Data.BootSectorOffset Or Offset > ParentDirectory.Data.BootSectorOffset + DiskImage.BootSector.BOOT_SECTOR_SIZE Then
                                If DirectoryEntryHasData(RootDirectory.Disk.Image, Offset) Then
                                    ParentDirectory.Data.HasAdditionalData = True
                                End If
                            End If
                        End If
                    Else
                        ParentDirectory.Data.EntryCount += 1
                        If Buffer.CompareTo(DirectoryEntry.EmptyDirectoryEntry) Then
                            ParentDirectory.Data.AvailableEntryCount += 1
                        Else
                            ParentDirectory.Data.AvailableEntryCount = 0
                        End If
                        If RootDirectory.Disk.Image.GetByte(Offset + 11) <> &HF Then 'Exclude LFN entries
                            Dim FilePart = RootDirectory.Disk.Image.ToUInt16(Offset)
                            If FilePart <> &H202E And FilePart <> &H2E2E Then 'Exclude '.' and '..' entries
                                ParentDirectory.Data.FileCount += 1
                                If Buffer(0) = DirectoryEntry.CHAR_DELETED Then
                                    ParentDirectory.Data.DeletedFileCount += 1
                                End If
                            End If
                        End If
                    End If

                    Dim NewDirectoryEntry = New DirectoryEntry(RootDirectory, ParentDirectory, Offset, ParentDirectory.DirectoryEntries.Count, ParentDirectory.Data.EndOfDirectory)

                    ParentDirectory.DirectoryEntries.Add(NewDirectoryEntry)
                Next
            End If
        End Sub

        Public MustOverride Function AddDirectory(EntryData() As Byte, Options As AddFileOptions, Filename As String, Optional Index As Integer = -1) As AddFileData Implements IDirectory.AddDirectory

        Public MustOverride Function AddFile(FilePath As String, Options As AddFileOptions, Optional Index As Integer = -1) As Integer Implements IDirectory.AddFile

        Public MustOverride Function AddFile(FileInfo As IO.FileInfo, Options As AddFileOptions, Optional Index As Integer = -1) As Integer Implements IDirectory.AddFile

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

        Public Function FindFileName(Filename As String, IncludeDirectories As Boolean, Optional SkipIndex As Integer = -1) As Integer Implements IDirectory.FindFileName
            If _DirectoryData.EntryCount > 0 Then
                For Counter As UInteger = 0 To _DirectoryData.EntryCount - 1
                    If Counter <> SkipIndex Then
                        Dim File = _DirectoryEntries.Item(Counter)
                        If Not File.IsDeleted And Not File.IsVolumeName And (IncludeDirectories Or Not File.IsDirectory) Then
                            If File.GetFullFileName = Filename Then
                                Return Counter
                            End If
                        End If
                    End If
                Next
            End If

            Return -1
        End Function

        Public Function FindShortFileName(FileBytes() As Byte, IncludeDirectories As Boolean, Optional SkipIndex As Integer = -1) As Integer Implements IDirectory.FindShortFileName
            If _DirectoryData.EntryCount > 0 Then
                For Counter As UInteger = 0 To _DirectoryData.EntryCount - 1
                    If Counter <> SkipIndex Then
                        Dim File = _DirectoryEntries.Item(Counter)
                        If Not File.IsDeleted And Not File.IsVolumeName And (IncludeDirectories Or Not File.IsDirectory) Then
                            If FileBytes.CompareTo(File.FileNameWithExtension) Then
                                Return Counter
                            End If
                        End If
                    End If
                Next
            End If

            Return -1
        End Function

        Public Function FindShortFileName(Filename As String, IncludeDirectories As Boolean, Optional SkipIndex As Integer = -1) As Integer Implements IDirectory.FindShortFileName
            If _DirectoryData.EntryCount > 0 Then
                For Counter As UInteger = 0 To _DirectoryData.EntryCount - 1
                    If Counter <> SkipIndex Then
                        Dim File = _DirectoryEntries.Item(Counter)
                        If Not File.IsDeleted And Not File.IsVolumeName And (IncludeDirectories Or Not File.IsDirectory) Then
                            If File.GetShortFileName = Filename Then
                                Return Counter
                            End If
                        End If
                    End If
                Next
            End If

            Return -1
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

        Public Function GetAvailableFileName(FileName As String, Optional CurrentIndex As Integer = -1) As String Implements IDirectory.GetAvailableFileName
            Dim FileParts = SplitFilename(FileName)

            Dim NewFileName As String = FileName
            Dim Index As UInteger = 1

            Do While FindFileName(NewFileName, True, CurrentIndex) > -1
                NewFileName = CombineFileParts(FileParts.Name & " " & InParens(Index), FileParts.Extension)
                Index += 1
            Loop

            Return NewFileName
        End Function

        Public Function GetAvailableShortFileName(FileName As String, UseNTExtensions As Boolean, Optional CurrentIndex As Integer = -1) As String Implements IDirectory.GetAvailableShortFileName
            Dim FileParts = SplitFilename(FileName)

            Dim CleanFileName = DOSCleanFileName(FileParts.Name)
            Dim CleanExtension = DOSCleanFileName(FileParts.Extension, 3)

            Dim Checksum As String = ""

            If UseNTExtensions Then
                Checksum = GetShortFileChecksumString(FileName)
            End If

            Dim Index As UInteger = 1
            Dim NewFileName As String

            If CleanFileName.Length > 8 Or CleanFileName.Length <> FileParts.Name.Length Then
                NewFileName = TruncateFileName(CleanFileName, CleanExtension, Checksum, Index, UseNTExtensions)
                Index += 1
            Else
                NewFileName = CombineFileParts(CleanFileName, CleanExtension)
            End If

            Do While FindShortFileName(NewFileName, True, CurrentIndex) > -1
                NewFileName = TruncateFileName(CleanFileName, CleanExtension, Checksum, Index, UseNTExtensions)
                Index += 1
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
        Public Function GetIndex(DirectoryEntry As DirectoryEntry) As Integer Implements IDirectory.GetIndex
            Return _DirectoryEntries.IndexOf(DirectoryEntry)
        End Function

        Public Overridable Function RemoveEntry(Index As UInteger) As Boolean Implements IDirectory.RemoveEntry
            Dim DirectoryEntry = _DirectoryEntries.Item(Index)

            If Not DirectoryEntry.IsDeleted Then
                Return False
            End If

            Dim NewIndex = AdjustIndexForLFN(Index)
            Dim EntriesToRemove = Index - NewIndex + 1

            ShiftEntries(NewIndex, _DirectoryData.EntryCount, EntriesToRemove * -1)

            UpdateEntryCounts()

            Return True
        End Function

        Public Overridable Function RemoveLFN(Index As UInteger) As Boolean Implements IDirectory.RemoveLFN
            Dim DirectoryEntry = _DirectoryEntries.Item(Index)

            Dim NewIndex = AdjustIndexForLFN(Index)

            If NewIndex = Index Then
                Return False
            End If

            Dim EntriesToRemove = Index - NewIndex

            ShiftEntries(NewIndex, _DirectoryData.EntryCount, EntriesToRemove * -1)

            UpdateEntryCounts()

            Return True
        End Function

        Public Sub ShiftEntries(StartIndex As UInteger, EntryCount As UInteger, Offset As Integer)
            Dim DirectoryEntry As DirectoryEntry
            Dim TempOffsets As New List(Of UInteger)
            Dim Counter As Integer

            Dim UseTransaction As Boolean = _Disk.BeginTransaction

            If Offset > 0 Then
                For Counter = EntryCount + Offset - 1 To StartIndex Step -1
                    TempOffsets.Add(DirectoryEntries(Counter).Offset)
                Next

                Dim TempIndex As UInteger = 0
                For Counter = EntryCount - 1 To StartIndex Step -1
                    DirectoryEntry = _DirectoryEntries(Counter)
                    DirectoryEntry.Offset = TempOffsets.Item(TempIndex)
                    TempIndex += 1
                Next

                For Counter = 0 To Offset - 1
                    _DirectoryEntries.RemoveAt(EntryCount)
                Next

                For Counter = TempIndex To TempOffsets.Count - 1
                    Dim NewDirectoryEntry = New DirectoryEntry(_Disk.RootDirectory, Me, TempOffsets.Item(Counter), StartIndex, True) With {
                      .Data = New Byte(31) {}
                    }
                    _DirectoryEntries.Insert(StartIndex, NewDirectoryEntry)
                Next

            ElseIf Offset < 0 Then
                For Counter = StartIndex To EntryCount - 1
                    TempOffsets.Add(DirectoryEntries(Counter).Offset)
                Next

                Dim TempIndex As UInteger = 0
                For Counter = StartIndex - Offset To EntryCount - 1
                    DirectoryEntry = _DirectoryEntries(Counter)
                    DirectoryEntry.Offset = TempOffsets.Item(TempIndex)
                    TempIndex += 1
                Next

                Dim EntryIndex = EntryCount
                For Counter = TempIndex To TempOffsets.Count - 1
                    Dim NewDirectoryEntry = New DirectoryEntry(_Disk.RootDirectory, Me, TempOffsets.Item(Counter), EntryIndex, True) With {
                      .Data = New Byte(31) {}
                    }
                    _DirectoryEntries.Insert(EntryIndex, NewDirectoryEntry)
                    EntryIndex += 1
                Next

                For Counter = 0 To -Offset - 1
                    _DirectoryEntries.RemoveAt(StartIndex)
                Next
            End If

            If UseTransaction Then
                _Disk.EndTransaction()
            End If
        End Sub

        Public Sub UpdateEntryCounts() Implements IDirectory.UpdateEntryCounts
            Dim EntryCount As UInteger
            Dim Counter As Integer
            Dim Buffer(10) As Byte

            EntryCount = 0
            For Counter = 0 To _DirectoryEntries.Count - 1
                Dim DirectoryEntry = _DirectoryEntries(Counter)
                DirectoryEntry.Index = Counter
                Dim FirstChar = DirectoryEntry.Data(0)
                If FirstChar = 0 Then
                    Exit For
                End If
                EntryCount += 1
            Next

            _DirectoryData.EntryCount = EntryCount

            For Counter = _DirectoryData.EntryCount - 1 To 0 Step -1
                Dim DirectoryEntry = _DirectoryEntries(Counter)
                Array.Copy(DirectoryEntry.Data, 0, Buffer, 0, Buffer.Length)
                If Buffer.CompareTo(DirectoryEntry.EmptyDirectoryEntry) Then
                    EntryCount = Counter
                Else
                    Exit For
                End If
            Next

            Data.AvailableEntryCount = _DirectoryEntries.Count - EntryCount
        End Sub

        Public MustOverride Function UpdateLFN(FileName As String, Index As Integer, UseNTExtensions As Boolean) As Boolean Implements IDirectory.UpdateLFN
    End Class
End Namespace
