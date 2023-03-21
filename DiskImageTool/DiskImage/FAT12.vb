Imports System.Reflection

Namespace DiskImage
    Public Class FAT12
        Private Const FAT_BADCLUSTER As UShort = &HFF7
        Private ReadOnly _BadClusters As List(Of UShort)
        Private ReadOnly _BadSectors As HashSet(Of UInteger)
        Private ReadOnly _BootSector As BootSector
        Private ReadOnly _FATChains As Dictionary(Of UInteger, FATChain)
        Private ReadOnly _FileAllocation As Dictionary(Of UInteger, List(Of UInteger))
        Private ReadOnly _FileBytes As ImageByteArray
        Private _FATTable() As UShort
        Private _FreeSpace As UInteger

        Sub New(FileBytes As ImageByteArray, BootSector As BootSector, Index As UShort, ProcessFATChains As Boolean)
            _BootSector = BootSector
            _FileBytes = FileBytes
            _FATChains = New Dictionary(Of UInteger, FATChain)
            _FileAllocation = New Dictionary(Of UInteger, List(Of UInteger))
            _FreeSpace = 0
            _BadClusters = New List(Of UShort)
            _BadSectors = New HashSet(Of UInteger)

            If BootSector.IsValidImage Then
                PopulateFAT12(Index)
                If ProcessFATChains Then
                    PopulateFATChains()
                End If
            End If
        End Sub

        Public ReadOnly Property BadClusters As List(Of UShort)
            Get
                Return _BadClusters
            End Get
        End Property

        Public ReadOnly Property BadSectors As HashSet(Of UInteger)
            Get
                Return _BadSectors
            End Get
        End Property

        Public ReadOnly Property FATChains As Dictionary(Of UInteger, FATChain)
            Get
                Return _FATChains
            End Get
        End Property

        Public ReadOnly Property FileAllocation As Dictionary(Of UInteger, List(Of UInteger))
            Get
                Return _FileAllocation
            End Get
        End Property

        Public ReadOnly Property FreeSpace As UInteger
            Get
                Return _FreeSpace
            End Get
        End Property

        Public ReadOnly Property TableLength As UShort
            Get
                Return _FATTable.Length
            End Get
        End Property

        Public Property TableEntry(Cluster As UShort) As UShort
            Get
                Return _FATTable(Cluster)
            End Get

            Set(value As UShort)
                If _FATTable(Cluster) <> value Then
                    _FATTable(Cluster) = value

                End If
            End Set
        End Property

        Public Function CompareTables() As Boolean
            If _BootSector.NumberOfFATs < 2 Then
                Return True
            End If

            For Counter As UShort = 1 To _BootSector.NumberOfFATs - 1
                Dim FatCopy1 = GetFAT(Counter - 1)
                Dim FatCopy2 = GetFAT(Counter)

                For Index = FatCopy1.Length - 1 To 0 Step -1
                    If FatCopy1(Index) <> FatCopy2(Index) Then
                        Return False
                    End If
                Next
            Next

            Return True
        End Function

        Public Function GetUnusedSectors(WithData As Boolean) As List(Of UInteger)
            Dim SectorChain As New List(Of UInteger)

            Dim ClusterSize As UInteger = _BootSector.BytesPerCluster
            Dim AddCluster As Boolean

            For Cluster As UShort = 1 To _BootSector.NumberOfFATEntries + 1
                If _FATTable(Cluster) = 0 Then
                    Dim Offset = _BootSector.ClusterToOffset(Cluster)
                    If _FileBytes.Length < Offset + ClusterSize Then
                        ClusterSize = _FileBytes.Length - Offset
                    End If
                    If ClusterSize > 0 Then
                        If WithData Then
                            AddCluster = Not IsDataBlockEmpty(_FileBytes.GetBytes(Offset, ClusterSize))
                        Else
                            AddCluster = True
                        End If
                        If AddCluster Then
                            Dim Sector = _BootSector.ClusterToSector(Cluster)
                            For Index = 0 To _BootSector.SectorsPerCluster - 1
                                SectorChain.Add(Sector + Index)
                            Next
                        End If
                    Else
                        Exit For
                    End If
                End If
            Next

            Return SectorChain
        End Function

        Public Function HasUnusedSectors(WithData As Boolean) As Boolean
            Dim ClusterSize As UInteger = _BootSector.BytesPerCluster

            For Cluster As UShort = 1 To _BootSector.NumberOfFATEntries + 1
                If _FATTable(Cluster) = 0 Then
                    Dim Offset = _BootSector.ClusterToOffset(Cluster)
                    If _FileBytes.Length < Offset + ClusterSize Then
                        ClusterSize = Math.Max(_FileBytes.Length - Offset, 0)
                    End If
                    If ClusterSize > 0 Then
                        If Not WithData Then
                            Return True
                        ElseIf Not IsDataBlockEmpty(_FileBytes.GetBytes(Offset, ClusterSize)) Then
                            Return True
                        End If
                    Else
                        Exit For
                    End If
                End If
            Next

            Return False
        End Function

        Public Function IsFATRegion(Offset As UInteger, Length As UInteger) As Boolean
            Dim FATSectorStart = _BootSector.FATRegionStart
            Dim FATSectorEnd = _BootSector.FATRegionStart + (_BootSector.SectorsPerFAT * _BootSector.NumberOfFATs) - 1

            Dim SectorStart = OffsetToSector(Offset)
            Dim SectorEnd = OffsetToSector(Offset + Length - 1)

            Return (SectorStart >= FATSectorStart And SectorStart <= FATSectorEnd) Or (SectorEnd >= FATSectorStart And SectorEnd <= FATSectorEnd)
        End Function

        Public Function DecodeFAT12(Data() As Byte, Size As UShort) As UShort()
            Dim MaxSize As Integer = Int(Data.Length * 2 / 3)

            If MaxSize Mod 2 > 0 Then
                MaxSize -= 1
            End If

            If Size Mod 2 > 0 Then
                Size += 1
            End If

            If Size > MaxSize Then
                Size = MaxSize
            End If

            Dim FATTable(Size - 1) As UShort

            For i = 0 To Size - 1
                Dim Offset As Integer = Int(i + i / 2)
                Dim b = BitConverter.ToUInt16(Data, Offset)

                If i Mod 2 = 0 Then
                    FATTable(i) = b And &HFFF
                Else
                    FATTable(i) = b >> 4
                End If
            Next

            Return FATTable
        End Function

        Public Function EncodeFAT12(FATTable() As UShort) As Byte()
            Dim Size As Integer = Math.Ceiling(FATTable.Length / 2 * 3)

            Dim FatBytes(Size - 1) As Byte

            For i = 0 To Size - 1
                Dim Offset As Integer = Int(i / 3 * 2)
                If i Mod 3 = 0 Then
                    FatBytes(i) = FATTable(Offset) And &HFF
                ElseIf i Mod 3 = 1 Then
                    FatBytes(i) = (FATTable(Offset) >> 8)
                    If Offset < FATTable.Length - 1 Then
                        FatBytes(i) = FatBytes(i) + (FATTable(Offset + 1) And &HF) * 16
                    End If
                Else
                    FatBytes(i) = FATTable(Offset) >> 4
                End If
            Next

            Return FatBytes
        End Function

        Public Sub PopulateFAT12(Index As UShort)
            Erase _FATTable

            If _BootSector.IsValidImage Then
                Dim FATBytes = GetFAT(Index)
                Dim Size = _BootSector.NumberOfFATEntries + 2
                _FATTable = DecodeFAT12(FATBytes, Size)
            End If

            ProcessFAT12()
        End Sub

        Private Sub PopulateFATChains()
            Dim Directory = New RootDirectory(_FileBytes, _BootSector, Me)
            EnumDirectoryEntries(Directory)
        End Sub

        Private Sub EnumDirectoryEntries(Directory As DiskImage.IDirectory)
            Dim DirectoryEntryCount = Directory.DirectoryEntryCount

            If DirectoryEntryCount > 0 Then
                For Counter = 0 To DirectoryEntryCount - 1
                    Dim File = Directory.GetFile(Counter)

                    If Not File.IsLink Then
                        If File.IsDirectory And File.SubDirectory IsNot Nothing Then
                            If File.SubDirectory.DirectoryEntryCount > 0 Then
                                EnumDirectoryEntries(File.SubDirectory)
                            End If
                        End If
                    End If
                Next
            End If
        End Sub

        Public Sub UpdateFAT12()
            Dim FATBytes = EncodeFAT12(_FATTable)

            For Counter = 0 To _BootSector.NumberOfFATs - 1
                SetFAT(Counter, FATBytes)
            Next

            ProcessFAT12()
        End Sub

        Private Sub ProcessFAT12()
            _FATChains.Clear()
            _FileAllocation.Clear()
            _FreeSpace = 0
            _BadClusters.Clear()
            _BadSectors.Clear()

            If _BootSector.IsValidImage Then
                Dim ClusterSize As UInteger = _BootSector.BytesPerCluster

                For i = 2 To _BootSector.NumberOfFATEntries + 1
                    If _FATTable(i) = 0 Then
                        _FreeSpace += ClusterSize
                    ElseIf _FATTable(i) = FAT_BADCLUSTER Then
                        _BadClusters.Add(i)
                        For j = 0 To _BootSector.SectorsPerCluster - 1
                            _BadSectors.Add(_BootSector.ClusterToSector(i) + j)
                        Next j
                    End If
                Next i
            End If
        End Sub

        Friend Function InitFATChain(Offset As UInteger, ClusterStart As UShort) As FATChain
            Dim FatChain As New FATChain
            Dim Cluster As UShort = ClusterStart
            Dim AssignedClusters As New HashSet(Of UShort)
            Dim OffsetList As List(Of UInteger)

            If _FATTable IsNot Nothing Then
                Dim ClusterCount = _BootSector.NumberOfFATEntries + 1

                Do
                    If Cluster >= 2 And Cluster <= ClusterCount Then
                        Dim Sector = _BootSector.ClusterToSector(Cluster)
                        If AssignedClusters.Contains(Cluster) Then
                            FatChain.HasCircularChain = True
                            Exit Do
                        End If
                        AssignedClusters.Add(Cluster)
                        FatChain.Chain.Add(Cluster)
                        For Index = 0 To _BootSector.SectorsPerCluster - 1
                            FatChain.SectorChain.Add(Sector + Index)
                        Next
                        If Not _FileAllocation.ContainsKey(Cluster) Then
                            OffsetList = New List(Of UInteger) From {
                                Offset
                            }
                            _FileAllocation.Add(Cluster, OffsetList)
                        Else
                            OffsetList = _FileAllocation.Item(Cluster)
                            For Each OffsetItem In OffsetList
                                If Not FatChain.CrossLinks.Contains(OffsetItem) Then
                                    FatChain.CrossLinks.Add(OffsetItem)
                                End If
                                If _FATChains.ContainsKey(OffsetItem) Then
                                    Dim CrossLinks = _FATChains.Item(OffsetItem).CrossLinks
                                    If Not CrossLinks.Contains(Offset) Then
                                        CrossLinks.Add(Offset)
                                    End If
                                End If
                            Next
                            OffsetList.Add(Offset)
                        End If
                        Cluster = _FATTable(Cluster)
                    Else
                        Cluster = 0
                    End If
                Loop Until Cluster < 2 Or Cluster > ClusterCount

                _FATChains.Item(Offset) = FatChain
            End If

            Return FatChain
        End Function

        Private Function GetFAT(Index As UShort) As Byte()
            Dim SectorCount = _BootSector.SectorsPerFAT
            Dim SectorStart = _BootSector.FATRegionStart + (SectorCount * Index)

            Return _FileBytes.GetSectors(SectorStart, SectorCount)
        End Function

        Private Sub SetFAT(Index As UShort, Data() As Byte)
            Dim SectorCount = _BootSector.SectorsPerFAT
            Dim SectorStart = _BootSector.FATRegionStart + (SectorCount * Index)
            Dim Offset = SectorToBytes(SectorStart)

            _FileBytes.SetBytes(Data, Offset)
        End Sub
    End Class
End Namespace
