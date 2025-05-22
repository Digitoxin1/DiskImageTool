Imports System.IO
Imports System.Text

Namespace DiskImage
    Module Functions
        Public Function CalcXDFChecksum(Data() As Byte, SectorsPerFAT As UInteger) As UInteger
            Dim Checksum As UInteger = &H12345678

            Checksum = (Checksum << 1) + CalcXDFChecksumBlock(Data, 1, &HA00)
            Checksum = (Checksum << 1) + CalcXDFChecksumBlock(Data, (SectorsPerFAT << 1) + 1, &HA00)
            Checksum = (Checksum << 1) + CalcXDFChecksumBlock(Data, (SectorsPerFAT << 1) + 6, &HA00)

            For i = 0 To &HB6 - 1
                Dim Start = ((SectorsPerFAT << 1) + &H15) + Data(&H80 + i) + (Checksum And &H7FF)
                Checksum = (Checksum << 1) + CalcXDFChecksumBlock(Data, Start, &H200)
            Next

            Return Checksum
        End Function

        Public Function ClusterListToSectorList(BPB As BiosParameterBlock, ClusterList As List(Of UShort)) As List(Of UInteger)
            Dim SectorList As New List(Of UInteger)

            For Each Cluster In ClusterList
                Dim Sector = BPB.ClusterToSector(Cluster)
                For Index = 0 To BPB.SectorsPerCluster - 1
                    SectorList.Add(Sector + Index)
                Next
            Next

            Return SectorList
        End Function

        Public Function ClusterToSectorList(BPB As BiosParameterBlock, Cluster As UShort) As List(Of UInteger)
            Dim SectorList As New List(Of UInteger)

            Dim Sector = BPB.ClusterToSector(Cluster)
            For Index = 0 To BPB.SectorsPerCluster - 1
                SectorList.Add(Sector + Index)
            Next

            Return SectorList
        End Function

        Public Function CombineFileParts(Filename As String, Extension As String) As String
            Return Filename & If(Extension.Length > 0, ".", "") & Extension
        End Function

        Public Function DateToFATDate(D As Date) As UShort
            Dim FATDate As UShort = D.Year - 1980

            FATDate <<= 4
            FATDate += D.Month
            FATDate <<= 5
            FATDate += D.Day

            Return FATDate
        End Function

        Public Function DateToFATMilliseconds(D As Date) As Byte
            Return D.Millisecond \ 10 + (D.Second Mod 2) * 100
        End Function

        Public Function DateToFATTime(D As Date) As UShort
            Dim DTTime As UShort = D.Hour

            DTTime <<= 6
            DTTime += D.Minute
            DTTime <<= 5
            DTTime += D.Second \ 2

            Return DTTime
        End Function

        Public Function DirectoryEntryHasData(FloppyImage As IFloppyImage, Offset As UInteger) As Boolean
            Dim Result As Boolean = False

            Dim FirstByte = FloppyImage.GetByte(Offset)

            If FirstByte = &HE5 Then
                For Offset2 As UInteger = Offset + 1 To Offset + DirectoryEntry.DIRECTORY_ENTRY_SIZE - 1
                    If FloppyImage.GetByte(Offset2) <> 0 Then
                        Result = True
                        Exit For
                    End If
                Next
            ElseIf FirstByte <> 0 Then
                Result = True
            Else
                Dim NextByte As Byte = FloppyImage.GetByte(Offset + 1)

                If Not Disk.FreeClusterBytes.Contains(NextByte) Then
                    Result = True
                Else
                    For Offset2 As UInteger = Offset + 2 To Offset + DirectoryEntry.DIRECTORY_ENTRY_SIZE - 1
                        If FloppyImage.GetByte(Offset2) <> NextByte Then
                            Result = True
                        End If
                    Next
                End If
            End If

            Return Result
        End Function

        Public Function DirectoryExpand(Disk As Disk, Directory As SubDirectory, FreeClusters As SortedSet(Of UShort)) As Boolean
            Dim Cluster = Disk.FAT.GetNextFreeCluster(FreeClusters, True)

            If Cluster = 0 Then
                Return False
            End If

            Directory.ExpandDirectorySize(Cluster)

            Return True
        End Function

        Public Function DOSCleanFileName(FileName As String, Optional MaxLength As Integer = -1) As String
            FileName = RemoveDiacritics(FileName)

            Dim FileBytes = Encoding.UTF8.GetBytes(FileName).ToList

            For i = FileBytes.Count - 1 To 0 Step -1
                If FileBytes(i) = &H20 Then 'Remove spaces
                    FileBytes.RemoveAt(i)
                ElseIf FileBytes(i) = &H2E Then 'Remove periods
                    FileBytes.RemoveAt(i)
                ElseIf DirectoryEntry.InvalidFileChars.Contains(FileBytes(i)) Then 'Replace invalid characters with underscores
                    FileBytes(i) = &H95
                ElseIf FileBytes(i) > 96 And FileBytes(i) < 123 Then 'Convert lowercase to uppercase
                    FileBytes(i) = FileBytes(i) - 32
                End If
            Next

            If MaxLength > -1 And FileBytes.Count > MaxLength Then
                Return Encoding.UTF8.GetString(FileBytes.ToArray, 0, MaxLength)
            Else
                Return Encoding.UTF8.GetString(FileBytes.ToArray)
            End If
        End Function

        Public Function GetBadSectors(BPB As BiosParameterBlock, BadClusters As List(Of UShort)) As HashSet(Of UInteger)
            Dim BadSectors As New HashSet(Of UInteger)

            For Each Cluster In BadClusters
                Dim Sector = BPB.ClusterToSector(Cluster)
                For Index = 0 To BPB.SectorsPerCluster - 1
                    BadSectors.Add(Sector + Index)
                Next
            Next

            Return BadSectors
        End Function

        Public Function GetDataFromChain(FloppyImage As IFloppyImage, BPB As BiosParameterBlock, SectorChain As List(Of UInteger)) As Byte()
            Dim SectorSize As UInteger = BPB.BytesPerSector
            Dim Content(SectorChain.Count * SectorSize - 1) As Byte
            Dim ContentOffset As UInteger = 0

            For Each Sector In SectorChain
                Dim Offset As UInteger = BPB.SectorToBytes(Sector)
                If FloppyImage.Length < Offset + SectorSize Then
                    SectorSize = Math.Max(FloppyImage.Length - Offset, 0)
                End If
                If SectorSize > 0 Then
                    FloppyImage.CopyTo(Offset, Content, ContentOffset, SectorSize)
                    ContentOffset += SectorSize
                Else
                    Exit For
                End If
            Next

            Return Content
        End Function

        Public Function GetLFNDirectoryEntries(FileName As String, ShortName As String) As List(Of Byte())
            Dim Entries = New List(Of Byte())

            If FileName = ShortName Then
                Return Entries
            End If

            FileName = Left(FileName, 255)

            Dim Buffer() As Byte
            Dim LFNBuffer() As Byte

            Dim FileBytes = System.Text.Encoding.Unicode.GetBytes(FileName)
            Dim Count = Math.Ceiling(FileBytes.Length / 26)

            For i = 0 To Count - 1
                Dim Offset As Long = i * 26
                Dim Length As Long = Math.Min(FileBytes.Length - Offset, 26)

                Buffer = New Byte(25) {}
                For j = 0 To Buffer.Length - 1
                    Buffer(j) = &HFF
                Next
                Array.Copy(FileBytes, Offset, Buffer, 0, Length)
                If Length < 26 Then
                    Buffer(Length) = 0
                    Buffer(Length + 1) = 0
                End If

                LFNBuffer = New Byte(31) {}
                If i = Count - 1 Then
                    LFNBuffer(0) = (i + 1) Or &H40
                Else
                    LFNBuffer(0) = i + 1
                End If
                Array.Copy(Buffer, 0, LFNBuffer, 1, 10)
                LFNBuffer(11) = &HF
                LFNBuffer(12) = &H0
                LFNBuffer(13) = 0
                Array.Copy(Buffer, 10, LFNBuffer, 14, 12)
                LFNBuffer(26) = 0
                LFNBuffer(27) = 0
                Array.Copy(Buffer, 22, LFNBuffer, 28, 4)

                Entries.Add(LFNBuffer)
            Next

            Entries.Reverse()

            Return Entries
        End Function

        Public Function GetShortFileChecksum(Filename As String) As UShort
            Dim Checksum As UShort = 0

            For i As Integer = 0 To Filename.Length - 1
                Checksum = (Checksum * &H25 + AscW(Filename(i))) And &HFFFF&
            Next

            Dim temp As UInteger = CLng(Checksum) * 314159269 And &HFFFFFFFF&

            Dim temp2 As Integer

            If temp > Integer.MaxValue Then
                temp2 = (UInteger.MaxValue - temp + 1)
            Else
                temp2 = temp
            End If

            temp2 -= (CType((CLng(temp2) * 1152921497) >> 60, ULong) * 1000000007)

            Checksum = temp2 And &HFFFF&

            ' Reverse nibble order
            Checksum = CUShort(
                ((Checksum And &HF000) >> 12) Or
                ((Checksum And &HF00) >> 4) Or
                ((Checksum And &HF0) << 4) Or
                ((Checksum And &HF) << 12)
            )

            Return Checksum
        End Function

        Public Function GetShortFileChecksumString(Filename As String) As String
            Return GetShortFileChecksum(Filename).ToString("X4")
        End Function

        Public Function InitializeAddDirectory(Directory As DirectoryBase, Options As AddFileOptions, Index As Integer, FileName As String) As AddFileData
            Return InitializeAddFile(Directory, Options, Index, FileName, 1)
        End Function

        Public Function InitializeAddFile(Directory As DirectoryBase, Options As AddFileOptions, Index As Integer, FileInfo As IO.FileInfo) As AddFileData
            Dim ClustersRequired As UShort = Math.Ceiling(FileInfo.Length / Directory.Disk.BPB.BytesPerCluster)

            Dim AddFileData = InitializeAddFile(Directory, Options, Index, FileInfo.Name, ClustersRequired)
            AddFileData.FileInfo = FileInfo

            Return AddFileData
        End Function

        Public Function InitializeUpdateLFN(Directory As DirectoryBase, FileName As String, Index As Integer, UseNTExtensions As Boolean) As AddFileData
            Dim AddFileData As New AddFileData With {
                .RequiresExpansion = False,
                .Entry = Directory.DirectoryEntries.Item(Index),
                .Index = Directory.AdjustIndexForLFN(Index)
            }

            InitializeFileNames(AddFileData, Directory, FileName, UseNTExtensions, True, Index)

            Dim CurrentLFNEntryCount = Index - AddFileData.Index
            AddFileData.EntriesNeeded = AddFileData.LFNEntries.Count - CurrentLFNEntryCount

            If AddFileData.EntriesNeeded > 0 Then
                If Directory.Data.AvailableEntryCount < AddFileData.EntriesNeeded Then
                    AddFileData.RequiresExpansion = True
                End If
            End If

            Return AddFileData
        End Function

        Public Sub ProcessAddDirectory(Directory As DirectoryBase, Data As AddFileData, EntryData() As Byte)
            Dim Entries = GetEntryList(Directory, Data.EntriesNeeded, Data.Index)

            Data.Entry = Entries(Entries.Count - 1)

            DirectoryEntrySetDirectory(Data.Entry, Data, EntryData)

            If Data.Options.UseLFN Then
                ProcessLFNEntries(Entries, Data.LFNEntries)
            End If

            Directory.UpdateEntryCounts()
        End Sub

        Public Sub ProcessAddFile(Directory As DirectoryBase, Data As AddFileData)
            Dim Entries = GetEntryList(Directory, Data.EntriesNeeded, Data.Index)

            Data.Entry = Entries(Entries.Count - 1)

            DirectoryEntrySetFile(Data.Entry, Data)

            If Data.Options.UseLFN Then
                ProcessLFNEntries(Entries, Data.LFNEntries)
            End If

            Directory.UpdateEntryCounts()
        End Sub

        Public Sub ProcessLFNEntries(DirectoryEntries As List(Of DirectoryEntry), LFNEntries As List(Of Byte()))
            Dim DirectoryEntry = DirectoryEntries(DirectoryEntries.Count - 1)
            Dim Checksum = DirectoryEntry.CalculateLFNChecksum
            For Counter = 0 To LFNEntries.Count - 1
                Dim Buffer = LFNEntries(Counter)
                Buffer(13) = Checksum
                DirectoryEntries(Counter).Data = Buffer
            Next
        End Sub

        Public Sub ProcessUpdateLFN(Directory As DirectoryBase, Data As AddFileData)
            If Data.EntriesNeeded <> 0 Then
                Dim EntryCount = Directory.DirectoryEntries.Count - Directory.Data.AvailableEntryCount
                Directory.ShiftEntries(Data.Index, EntryCount, Data.EntriesNeeded)
            End If

            Dim NewEntry = Data.Entry.Clone
            NewEntry.SetFileName(Data.ShortFileName)
            NewEntry.HasNTLowerCaseFileName = Data.HasNTLowerCaseFileName
            NewEntry.HasNTLowerCaseExtension = Data.HasNTLowerCaseExtension

            Data.Entry.Data = NewEntry.Data

            If Data.LFNEntries.Count > 0 Then
                Dim Entries = Directory.GetEntries(Data.Index, Data.LFNEntries.Count + 1)
                ProcessLFNEntries(Entries, Data.LFNEntries)
            End If

            Directory.UpdateEntryCounts()
        End Sub

        Public Function ReadFileIntoBuffer(FileInfo As IO.FileInfo, FileSize As UInteger, FillChar As Byte) As Byte()
            Dim FileBuffer(FileSize - 1) As Byte
            Dim n As Integer
            Using fs = FileInfo.OpenRead()
                n = fs.Read(FileBuffer, 0, Math.Min(FileInfo.Length, FileBuffer.Length))
            End Using
            For Counter As Integer = n To FileBuffer.Length - 1
                FileBuffer(Counter) = FillChar
            Next

            Return FileBuffer
        End Function

        Public Function TruncateFileName(Filename As String, Extension As String, Checksum As String, Index As UInteger, UseNTExtensions As Boolean) As String
            Dim Suffix = ""
            Dim UseChecksum As Boolean = False

            If UseNTExtensions Then
                If Filename.Length < 3 Or Index > 4 Then
                    UseChecksum = True
                End If
                If Index > 4 Then
                    Index -= 4
                End If
            End If

            If UseChecksum Then
                Suffix &= Checksum
            End If
            Suffix = Suffix & "~" & Index
            Dim Length = 8 - Suffix.Length
            If Length > Filename.Length Then
                Length = Filename.Length
            End If

            Filename = Filename.Substring(0, Length) & Suffix

            Return CombineFileParts(Filename, Extension)
        End Function

        Private Function CalcXDFChecksumBlock(Data() As Byte, Start As UInteger, Length As UShort) As UInteger
            Dim Checksum As UInt32 = &HABDC
            Dim Loc2 As UInt16

            Start <<= 9

            For i = 0 To Length - 1
                Loc2 = Data((Data(i + Start) * &H13) Mod Length + Start)
                Checksum = (Checksum + (Loc2 >> 5) + ((Loc2 And &H1F) << 4)) And &HFFFF&
            Next

            Return Checksum
        End Function

        Private Sub DirectoryEntrySetDirectory(DirectoryEntry As DirectoryEntry, DirectoryData As AddFileData, EntryData() As Byte)
            Dim UseTransaction As Boolean = DirectoryEntry.Disk.BeginTransaction

            Dim Cluster = DirectoryEntry.Disk.FAT.GetNextFreeCluster(DirectoryData.ClusterList, True)
            DirectoryEntry.Disk.FATTables.UpdateTableEntry(Cluster, FAT12.FAT_LAST_CLUSTER_END)

            Dim NewEntry = New DirectoryEntryBase(EntryData) With {
                .StartingCluster = Cluster,
                .HasNTLowerCaseExtension = DirectoryData.HasNTLowerCaseExtension,
                .HasNTLowerCaseFileName = DirectoryData.HasNTLowerCaseFileName
            }
            NewEntry.SetFileName(DirectoryData.ShortFileName)

            DirectoryEntry.Data = NewEntry.Data

            DirectoryEntry.InitFatChain()
            DirectoryEntry.InitSubDirectory()
            DirectoryEntry.SubDirectory.Initialize()

            If UseTransaction Then
                DirectoryEntry.Disk.EndTransaction()
            End If
        End Sub

        Private Function DirectoryEntrySetFile(DirectoryEntry As DirectoryEntry, FileData As AddFileData) As Boolean
            Dim ClusterSize = DirectoryEntry.Disk.BPB.BytesPerCluster
            Dim StartingCluster As UShort = 2

            If FileData.FileInfo.Length > FileData.ClusterList.Count * ClusterSize Then
                Return False
            End If

            Dim UseTransaction As Boolean = DirectoryEntry.Disk.BeginTransaction

            Dim FirstCluster As UShort = 0

            If FileData.FileInfo.Length > 0 Then
                'Load file into buffer, padding with empty space if needed            
                Dim FileSize = Math.Ceiling(FileData.FileInfo.Length / ClusterSize) * ClusterSize
                Dim FileBuffer = ReadFileIntoBuffer(FileData.FileInfo, FileSize, 0)

                Dim LastCluster As UShort = 0

                For Counter As Integer = 0 To FileBuffer.Length - 1 Step ClusterSize
                    Dim Cluster = DirectoryEntry.Disk.FAT.GetNextFreeCluster(FileData.ClusterList, True, StartingCluster)
                    If Cluster > 0 Then
                        If Counter = 0 Then
                            FirstCluster = Cluster
                        Else
                            DirectoryEntry.Disk.FATTables.UpdateTableEntry(LastCluster, Cluster)
                        End If
                        Dim ClusterOffset = DirectoryEntry.Disk.BPB.ClusterToOffset(Cluster)
                        Dim Buffer = DirectoryEntry.Disk.Image.GetBytes(ClusterOffset, ClusterSize)
                        Array.Copy(FileBuffer, Counter, Buffer, 0, ClusterSize)
                        DirectoryEntry.Disk.Image.SetBytes(Buffer, ClusterOffset)
                        LastCluster = Cluster
                        StartingCluster = Cluster + 1
                    End If
                Next

                If LastCluster > 0 Then
                    DirectoryEntry.Disk.FATTables.UpdateTableEntry(LastCluster, FAT12.FAT_LAST_CLUSTER_END)
                End If
            End If

            Dim NewEntry = New DirectoryEntryBase With {
                .StartingCluster = FirstCluster,
                .HasNTLowerCaseExtension = FileData.HasNTLowerCaseExtension,
                .HasNTLowerCaseFileName = FileData.HasNTLowerCaseFileName
            }
            NewEntry.SetFileInfo(FileData.FileInfo, FileData.ShortFileName, FileData.Options.UseCreatedDate, FileData.Options.UseLastAccessedDate)

            DirectoryEntry.Data = NewEntry.Data

            DirectoryEntry.InitFatChain()

            If UseTransaction Then
                DirectoryEntry.Disk.EndTransaction()
            End If

            Return True
        End Function

        Private Function GetEntryList(Directory As DirectoryBase, EntriesNeeded As Integer, Index As Integer) As List(Of DirectoryEntry)
            Dim EntryCount = Directory.DirectoryEntries.Count - Directory.Data.AvailableEntryCount
            Dim Entries As List(Of DirectoryEntry)

            If Index > -1 Then
                Index = Directory.AdjustIndexForLFN(Index)
                Directory.ShiftEntries(Index, EntryCount, EntriesNeeded)
                Entries = Directory.GetEntries(Index, EntriesNeeded)
            Else
                Entries = Directory.GetEntries(EntryCount, EntriesNeeded)
            End If

            Return Entries
        End Function

        Private Function InitializeAddFile(Directory As DirectoryBase, Options As AddFileOptions, Index As Integer, FileName As String, ClustersRequired As UShort) As AddFileData
            Dim AddFileData As New AddFileData With {
                .Options = Options,
                .Index = Index
            }

            InitializeFileNames(AddFileData, Directory, FileName, Options.UseNTExtensions, Options.UseLFN)

            AddFileData.RequiresExpansion = Directory.Data.AvailableEntryCount < AddFileData.EntriesNeeded

            If AddFileData.RequiresExpansion Then
                ClustersRequired += 1
            End If

            If AddFileData.RequiresExpansion And Directory.IsRootDirectory Then
                AddFileData.ClusterList = Nothing
            Else
                AddFileData.ClusterList = Directory.Disk.FAT.GetFreeClusters(ClustersRequired)
            End If

            Return AddFileData
        End Function

        Private Sub InitializeFileNames(AddFileData As AddFileData, Directory As DirectoryBase, FileName As String, UseNTExtensions As Boolean, UseLFN As Boolean, Optional CurrentIndex As Integer = -1)
            FileName = Directory.GetAvailableFileName(FileName, CurrentIndex)

            AddFileData.ShortFileName = Directory.GetAvailableShortFileName(FileName, UseNTExtensions, CurrentIndex)

            If UseNTExtensions And FileName.ToUpper = AddFileData.ShortFileName Then
                Dim ShortFileParts = SplitFilename(AddFileData.ShortFileName)
                Dim LongFileParts = SplitFilename(FileName)

                AddFileData.HasNTLowerCaseFileName = LongFileParts.Name.Length > 0 AndAlso ShortFileParts.Name.ToLower = LongFileParts.Name
                AddFileData.HasNTLowerCaseExtension = LongFileParts.Extension.Length > 0 AndAlso ShortFileParts.Extension.ToLower = LongFileParts.Extension
            Else
                AddFileData.HasNTLowerCaseFileName = False
                AddFileData.HasNTLowerCaseExtension = False
            End If

            If UseLFN Then
                If AddFileData.HasNTLowerCaseFileName Or AddFileData.HasNTLowerCaseExtension Then
                    AddFileData.LFNEntries = New List(Of Byte())
                Else
                    AddFileData.LFNEntries = GetLFNDirectoryEntries(FileName, AddFileData.ShortFileName)
                End If
                AddFileData.EntriesNeeded = AddFileData.LFNEntries.Count + 1
            Else
                AddFileData.LFNEntries = Nothing
                AddFileData.EntriesNeeded = 1
            End If
        End Sub

        Private Function RemoveDiacritics(value As String) As String
            Dim NormalizedString = value.Normalize(NormalizationForm.FormD)
            Dim SB = New StringBuilder(NormalizedString.Length)

            For i = 0 To NormalizedString.Length - 1
                Dim c = NormalizedString(i)
                Dim UnicodeCategory = Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                If UnicodeCategory <> Globalization.UnicodeCategory.NonSpacingMark Then
                    SB.Append(c)
                End If
            Next

            Return SB.ToString.Normalize(NormalizationForm.FormC)
        End Function
    End Module
End Namespace
