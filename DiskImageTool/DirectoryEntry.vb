Imports System.Text

Namespace DiskImage
    Public Structure ExpandedDate
        Dim Year As UShort
        Dim Month As Byte
        Dim Day As Byte
        Dim Hour As Byte
        Dim Hour12 As Byte
        Dim Minute As Byte
        Dim Second As Byte
        Dim Milliseconds As UShort
        Dim IsValidDate As Boolean
        Dim DateObject As Date
        Dim IsPM As Boolean
    End Structure

    Public Enum PropertyType
        inByte = 0
        inString = 1
    End Enum

    Public Class DirectoryEntry
        Public Enum AttributeFlags
            [ReadOnly] = 1
            Hidden = 2
            System = 4
            VolumeName = 8
            Directory = 16
            ArchiveFlag = 32
            LongFileName = 15
        End Enum

        Private Enum DirectoryEntryOffset As UInteger
            FileName = 0
            Extension = 8
            Attributes = 11
            ReservedForWinNT = 12
            CreationMillisecond = 13
            CreationTime = 14
            CreationDate = 16
            LastAccessDate = 18
            ReservedForFAT32 = 20
            LastWriteTime = 22
            LastWriteDate = 24
            StartingCluster = 26
            FileSize = 28
            LFNSequence = 0
            LFNFilePart1 = 1
            LFNFilePart2 = 14
            LFNFilePart3 = 28
        End Enum

        Private Enum DirectoryEntrySize As UInteger
            FileName = 8
            Extension = 3
            LFNFilePart1 = 10
            LFNFilePart2 = 12
            LFNFilePart3 = 4
        End Enum

        Private ReadOnly InvalidChars() As Byte = {&H22, &H2B, &H2C, &H2F, &H3A, &H3B, &H3C, &H3D, &H3E, &H3F, &H5B, &H5C, &H5D, &H7C}
        Private ReadOnly _FatChain As List(Of UInteger)
        Private ReadOnly _Offset As UInteger
        Private ReadOnly _Parent As Disk
        Private _SubDirectory As DiskImage.Directory

        Sub New(Parent As Disk, Offset As UInteger)
            _Parent = Parent
            _Offset = Offset

            If IsDeleted() Then
                _FatChain = New List(Of UInteger)
            Else
                _FatChain = GetFATChain()
            End If

            InitSubDirectory()
        End Sub

        Public Shared Function DateToFATDate(D As Date) As UShort
            Dim FATDate As UShort = D.Year - 1980

            FATDate <<= 4
            FATDate += D.Month
            FATDate <<= 5
            FATDate += D.Day

            Return FATDate
        End Function

        Public Shared Function DateToFATTime(D As Date) As UShort
            Dim DTTime As UShort = D.Hour

            DTTime <<= 6
            DTTime += D.Minute
            DTTime <<= 5
            DTTime += D.Second / 2

            Return DTTime
        End Function

        Public Shared Function DateToFATMilliseconds(D As Date) As Byte
            Return D.Millisecond / 10
        End Function

        Public Shared Function ExpandDate(FATDate As UShort) As ExpandedDate
            Return ExpandDate(FATDate, 0, 0)
        End Function

        Public Shared Function ExpandDate(FATDate As UShort, FATTime As UShort) As ExpandedDate
            Return ExpandDate(FATDate, FATTime, 0)
        End Function

        Public Shared Function ExpandDate(FATDate As UShort, FATTime As UShort, Milliseconds As Byte) As ExpandedDate
            Dim DT As ExpandedDate

            DT.Day = FATDate Mod 32
            FATDate >>= 5
            DT.Month = FATDate Mod 16
            FATDate >>= 4
            DT.Year = 1980 + FATDate Mod 128

            DT.Second = (FATTime Mod 32) * 2
            FATTime >>= 5
            DT.Minute = FATTime Mod 64
            FATTime >>= 6
            DT.Hour = FATTime Mod 32

            If Milliseconds < 100 Then
                DT.Milliseconds = Milliseconds * 10
            Else
                DT.Milliseconds = 0
            End If

            Dim DateString As String = DT.Year & "-" & Format(DT.Month, "00") & "-" & Format(DT.Day, "00") & " " & DT.Hour & ":" & Format(DT.Minute, "00") & ":" & Format(DT.Second, "00")

            DT.IsValidDate = IsDate(DateString)

            DT.IsPM = (DT.Hour > 11)

            If DT.Hour > 12 Then
                DT.Hour12 = DT.Hour - 12
            ElseIf DT.Hour = 0 Then
                DT.Hour12 = 12
            Else
                DT.Hour12 = DT.Hour
            End If

            If DT.IsValidDate Then
                DT.DateObject = New Date(DT.Year, DT.Month, DT.Day, DT.Hour, DT.Minute, DT.Second, DT.Milliseconds)
            End If

            Return DT
        End Function

        Private Function GetByte(Offset As UInteger) As Byte
            Return _Parent.GetByte(Offset + _Offset)
        End Function

        Private Function GetBytes(Offset As UInteger, Size As UInteger) As Byte()
            Return _Parent.GetBytes(Offset + _Offset, Size)
        End Function

        Private Function GetBytesShort(Offset As UInteger) As UShort
            Return _Parent.GetBytesShort(Offset + _Offset)
        End Function

        Private Function GetBytesInteger(Offset As UInteger) As UInteger
            Return _Parent.GetBytesInteger(Offset + _Offset)
        End Function

        Private Sub SetBytes(Value As UShort, Offset As UInteger)
            _Parent.SetBytes(Value, Offset + _Offset)
        End Sub

        Private Sub SetBytes(Value As UInteger, Offset As UInteger)
            _Parent.SetBytes(Value, Offset + _Offset)
        End Sub

        Private Sub SetBytes(Value As Byte, Offset As UInteger)
            _Parent.SetBytes(Value, Offset + _Offset)
        End Sub

        Private Sub SetBytes(Value() As Byte, Offset As UInteger, Size As UInteger, Padding As Byte)
            _Parent.SetBytes(Value, Offset + _Offset, Size, Padding)
        End Sub

        Public Sub ClearCreationDate()
            CreationDate = 0
            CreationTime = 0
            CreationMillisecond = 0
        End Sub

        Public Sub ClearLastAccessDate()
            LastAccessDate = 0
        End Sub

        Public Sub ClearLastWriteDate()
            LastWriteDate = 0
            LastWriteTime = 0
        End Sub

        Public Function GetCreationDate() As ExpandedDate
            Return ExpandDate(CreationDate, CreationTime, CreationMillisecond)
        End Function
        Public Function GetContent() As Byte()
            Dim Content(FileSize - 1) As Byte
            Dim ContentOffset As UInteger = 0
            Dim BytesRemaining As UInteger = FileSize
            Dim BytesToCopy As UInteger
            Dim FileBytes = _Parent.Data

            For Each Cluster In _FatChain
                Dim Offset As UInteger = _Parent.BootSector.DataRegionStart + ((Cluster - 2) * _Parent.BootSector.SectorsPerCluster)
                Offset *= _Parent.BootSector.BytesPerSector
                BytesToCopy = _Parent.BootSector.BytesPerSector * _Parent.BootSector.SectorsPerCluster
                If BytesToCopy > BytesRemaining Then
                    BytesToCopy = BytesRemaining
                End If
                If FileBytes.Length - Offset < BytesToCopy Then
                    BytesToCopy = Math.Max(FileBytes.Length - Offset, 0)
                End If
                If BytesToCopy = 0 Then
                    Exit For
                End If
                Array.Copy(FileBytes, Offset, Content, ContentOffset, BytesToCopy)
                BytesRemaining -= BytesToCopy
                If BytesRemaining = 0 Then
                    Exit For
                End If
                ContentOffset += BytesToCopy
            Next

            Return Content
        End Function

        Private Function GetFATChain() As List(Of UInteger)
            Dim FatChain = New List(Of UInteger)
            Dim Cluster As UShort = StartingCluster
            Dim AssignedClusters As New Hashtable
            Do
                If Cluster > 1 And Cluster <= UBound(_Parent.FAT12) Then
                    If AssignedClusters.ContainsKey(Cluster) Then
                        Exit Do
                    End If
                    AssignedClusters.Add(Cluster, Cluster)
                    FatChain.Add(Cluster)
                    Cluster = _Parent.FAT12(Cluster)
                Else
                    Cluster = 0
                End If
            Loop Until Cluster < 2 Or Cluster > 4079

            Return FatChain
        End Function

        Public Function GetFileName() As String
            Dim File = Encoding.UTF8.GetString(FileName).Trim
            Dim Ext = Encoding.UTF8.GetString(Extension).Trim
            If Ext <> "" Then
                File = File & "." & Ext
            End If

            Return File
        End Function

        Public Function GetLastAccessDate() As ExpandedDate
            Return ExpandDate(LastAccessDate)
        End Function

        Public Function GetLastWriteDate() As ExpandedDate
            Return ExpandDate(LastWriteDate, LastWriteTime)
        End Function

        Public Function GetLFNFileName() As String
            If IsLFN() Then
                Dim Size As Integer = DirectoryEntrySize.LFNFilePart1 + DirectoryEntrySize.LFNFilePart2 + DirectoryEntrySize.LFNFilePart3
                Dim LFN(Size - 1) As Byte

                Array.Copy(_Parent.Data, _Offset + DirectoryEntryOffset.LFNFilePart1, LFN, 0, DirectoryEntrySize.LFNFilePart1)
                Array.Copy(_Parent.Data, _Offset + DirectoryEntryOffset.LFNFilePart2, LFN, DirectoryEntrySize.LFNFilePart1, DirectoryEntrySize.LFNFilePart2)
                Array.Copy(_Parent.Data, _Offset + DirectoryEntryOffset.LFNFilePart3, LFN, DirectoryEntrySize.LFNFilePart1 + DirectoryEntrySize.LFNFilePart2, DirectoryEntrySize.LFNFilePart3)

                Return Encoding.Unicode.GetString(LFN)
            Else
                Dim LFN(1) As Byte

                Return Encoding.Unicode.GetString(LFN)
            End If
        End Function

        Public Function HasCreationDate() As Boolean
            Return CreationDate <> 0 Or CreationTime <> 0 Or CreationMillisecond <> 0
        End Function

        Public Function HasInvalidExtension() As Boolean
            Dim Result As Boolean = False
            Dim LocalExtension = Extension

            For Counter = 0 To UBound(LocalExtension)
                If LocalExtension(Counter) < &H20 Then
                    Result = True
                    Exit For
                ElseIf InvalidChars.Contains(LocalExtension(Counter)) Then
                    Result = True
                    Exit For
                End If
            Next

            Return Result
        End Function

        Public Function HasInvalidFilename() As Boolean
            Dim Result As Boolean = False
            Dim LocalFileName = FileName

            If LocalFileName(0) = &H20 Then
                Result = True
            Else
                For Counter = 0 To UBound(LocalFileName)
                    If LocalFileName(Counter) < &H20 Then
                        Result = True
                        Exit For
                    ElseIf InvalidChars.Contains(LocalFileName(Counter)) Then
                        Result = True
                        Exit For
                    End If
                Next
            End If

            Return Result
        End Function

        Public Function HasInvalidFileSize() As Boolean
            Dim Result As Boolean

            Select Case _Parent.BootSector.MediaDescriptor
                Case &HF0
                    If _Parent.BootSector.SectorsPerTrack = 36 Then
                        Result = (FileSize > 2949120)
                    Else
                        Result = (FileSize > 1474560)
                    End If
                Case &HF9
                    If _Parent.BootSector.SectorsPerTrack = 15 Then
                        Result = (FileSize > 1228800)
                    Else
                        Result = (FileSize > 737280)
                    End If
                Case &HFC
                    Result = (FileSize > 184320)
                Case &HFD
                    Result = (FileSize > 368640)
                Case &HFE
                    Result = (FileSize > 163840)
                Case &HFF
                    Result = (FileSize > 327680)
                Case Else
                    Result = (FileSize > 2949120)
            End Select

            Return Result
        End Function

        Public Function HasLastAccessDate() As Boolean
            Return LastAccessDate <> 0
        End Function

        Private Sub InitSubDirectory()
            If IsDirectory() And Not IsDeleted() Then
                _SubDirectory = New DiskImage.Directory(_Parent, _FatChain)
            Else
                _SubDirectory = Nothing
            End If
        End Sub

        Public Function IsArchive() As Boolean
            Return (Attributes And AttributeFlags.ArchiveFlag) > 0
        End Function

        Public Function IsDeleted() As Boolean
            Return FileName(0) = &HE5
        End Function

        Public Function IsDirectory() As Boolean
            Return (Attributes And AttributeFlags.Directory) > 0
        End Function

        Public Function IsHidden() As Boolean
            Return (Attributes And AttributeFlags.Hidden) > 0
        End Function

        Public Function IsLFN() As Boolean
            Return (Attributes And AttributeFlags.LongFileName) = AttributeFlags.LongFileName
        End Function

        Public Function IsReadOnly() As Boolean
            Return (Attributes And AttributeFlags.ReadOnly) > 0
        End Function

        Public Function IsSystem() As Boolean
            Return (Attributes And AttributeFlags.System) > 0
        End Function

        Public Function IsVolumeName() As Boolean
            Return (Attributes And AttributeFlags.VolumeName) > 0
        End Function

        Public Sub SetCreationDate(Value As Date)
            CreationDate = DateToFATDate(Value)
            CreationTime = DateToFATTime(Value)
            CreationMillisecond = DateToFATMilliseconds(Value)
        End Sub

        Public Sub SetLastAccessDate(Value As Date)
            LastAccessDate = DateToFATDate(Value)
        End Sub

        Public Sub SetLastWriteDate(Value As Date)
            LastWriteDate = DateToFATDate(Value)
            LastWriteTime = DateToFATTime(Value)
        End Sub

        Public Property Attributes() As Byte
            Get
                Return GetByte(DirectoryEntryOffset.Attributes)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.Attributes)
            End Set
        End Property

        Public Property CreationDate() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.CreationDate)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.CreationDate)
            End Set
        End Property

        Public Property CreationTime() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.CreationTime)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.CreationTime)
            End Set
        End Property

        Public Property CreationMillisecond() As Byte
            Get
                Return GetByte(DirectoryEntryOffset.CreationMillisecond)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.CreationMillisecond)
            End Set
        End Property

        Public Property Extension() As Byte()
            Get
                Return GetBytes(DirectoryEntryOffset.Extension, DirectoryEntrySize.Extension)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.Extension, DirectoryEntrySize.Extension, 32)
            End Set
        End Property

        Public Property FileName() As Byte()
            Get
                Return GetBytes(DirectoryEntryOffset.FileName, DirectoryEntrySize.FileName)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.FileName, DirectoryEntrySize.FileName, 32)
            End Set
        End Property

        Public Property FileSize() As UInteger
            Get
                Return GetBytesInteger(DirectoryEntryOffset.FileSize)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.FileSize)
            End Set
        End Property

        Public Property LastAccessDate() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.LastAccessDate)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.LastAccessDate)
            End Set
        End Property

        Public Property LastWriteDate() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.LastWriteDate)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.LastWriteDate)
            End Set
        End Property

        Public Property LastWriteTime() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.LastWriteTime)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.LastWriteTime)
            End Set
        End Property

        Public ReadOnly Property LFNSequence As Byte
            Get
                Return GetByte(DirectoryEntryOffset.LFNSequence)
            End Get
        End Property

        Public ReadOnly Property Offset As Integer
            Get
                Return _Offset
            End Get
        End Property

        Public Property ReservedForWinNT() As Byte
            Get
                Return GetByte(DirectoryEntryOffset.ReservedForWinNT)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.ReservedForWinNT)
            End Set
        End Property

        Public Property ReservedForFAT32() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.ReservedForFAT32)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.ReservedForFAT32)
            End Set
        End Property

        Public Property StartingCluster() As UShort
            Get
                Return GetBytesShort(DirectoryEntryOffset.StartingCluster)
            End Get
            Set
                SetBytes(Value, DirectoryEntryOffset.StartingCluster)
            End Set
        End Property

        Public ReadOnly Property SubDirectory As DiskImage.Directory
            Get
                Return _SubDirectory
            End Get
        End Property
    End Class
End Namespace