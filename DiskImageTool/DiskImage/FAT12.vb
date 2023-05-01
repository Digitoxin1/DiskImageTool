Namespace DiskImage
    Public Class FAT12
        Public Const FAT_BAD_CLUSTER As UShort = &HFF7
        Public Const FAT_FREE_CLUSTER As UShort = &H0
        Public Const FAT_LAST_CLUSTER_START As UShort = &HFF8
        Public Const FAT_LAST_CLUSTER_END As UShort = &HFFF
        Public Const FAT_RESERVED_START As UShort = &HFF0
        Public Const FAT_RESERVED_END As UShort = &HFF6
        Private ReadOnly _BadClusters As List(Of UShort)
        Private ReadOnly _BootSector As BootSector
        Private ReadOnly _CircularChains As HashSet(Of UShort)
        Private ReadOnly _FATChains As Dictionary(Of UInteger, FATChain)
        Private ReadOnly _FileAllocation As Dictionary(Of UShort, List(Of UInteger))
        Private ReadOnly _FileBytes As ImageByteArray
        Private ReadOnly _Index As UShort
        Private _FATTable() As UShort
        Private _FreeClusters As UInteger
        Private _MediaDescriptor As Byte

        Sub New(FileBytes As ImageByteArray, BootSector As BootSector, Index As UShort)
            _BootSector = BootSector
            _FileBytes = FileBytes
            _Index = Index
            _FATChains = New Dictionary(Of UInteger, FATChain)
            _FileAllocation = New Dictionary(Of UShort, List(Of UInteger))
            _FreeClusters = 0
            _BadClusters = New List(Of UShort)
            _CircularChains = New HashSet(Of UShort)

            PopulateFAT12()
        End Sub

        Public ReadOnly Property BadClusters As List(Of UShort)
            Get
                Return _BadClusters
            End Get
        End Property

        Public ReadOnly Property CircularChains As HashSet(Of UShort)
            Get
                Return _CircularChains
            End Get
        End Property

        Public ReadOnly Property FATChains As Dictionary(Of UInteger, FATChain)
            Get
                Return _FATChains
            End Get
        End Property

        Public ReadOnly Property FileAllocation As Dictionary(Of UShort, List(Of UInteger))
            Get
                Return _FileAllocation
            End Get
        End Property

        Public ReadOnly Property FreeClusters As UInteger
            Get
                Return _FreeClusters
            End Get
        End Property

        Public ReadOnly Property MediaDescriptor As Byte
            Get
                Return _MediaDescriptor
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

        Public Function GetFreeSpace(ClusterSize As UInteger) As UInteger
            Return _FreeClusters * ClusterSize
        End Function

        Public Function GetUnusedClusters(WithData As Boolean) As List(Of UShort)
            Dim ClusterChain As New List(Of UShort)

            If _FATTable IsNot Nothing Then
                Dim ClusterSize As UInteger = _BootSector.BytesPerCluster
                Dim AddCluster As Boolean
                Dim Length = GetFATTableLength()

                For Cluster As UShort = 1 To Length
                    If _FATTable(Cluster) = 0 Then
                        Dim Offset = _BootSector.ClusterToOffset(Cluster)
                        If _FileBytes.Length < Offset + ClusterSize Then
                            ClusterSize = Math.Max(_FileBytes.Length - Offset, 0)
                        End If
                        If ClusterSize > 0 Then
                            If WithData Then
                                AddCluster = Not IsDataBlockEmpty(_FileBytes.GetBytes(Offset, ClusterSize))
                            Else
                                AddCluster = True
                            End If
                            If AddCluster Then
                                ClusterChain.Add(Cluster)
                            End If
                        Else
                            Exit For
                        End If
                    End If
                Next
            End If

            Return ClusterChain
        End Function

        Public Function HasUnusedClusters(WithData As Boolean) As Boolean

            If _FATTable IsNot Nothing Then
                Dim ClusterSize As UInteger = _BootSector.BytesPerCluster
                Dim Length = GetFATTableLength()

                For Cluster As UShort = 1 To Length
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
            End If

            Return False
        End Function

        Public Shared Function DecodeFAT12(Data() As Byte, Size As UShort) As UShort()
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

        Public Shared Function EncodeFAT12(FATTable() As UShort) As Byte()
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

        Public Sub PopulateFAT12()
            Erase _FATTable

            If _BootSector.IsValidImage Then
                Dim FATBytes = GetFAT(_Index)
                Dim Size = _BootSector.NumberOfFATEntries + 2
                _FATTable = DecodeFAT12(FATBytes, Size)
                If FATBytes(1) = &HFF And FATBytes(2) = &HFF Then
                    _MediaDescriptor = FATBytes(0)
                End If
            End If

            ProcessFAT12()
        End Sub

        Public Function UpdateFAT12(SyncAll As Boolean) As Boolean
            Dim Updated As Boolean = False
            Dim UseBatchEditMode As Boolean = Not _FileBytes.BatchEditMode

            Dim FATBytes = EncodeFAT12(_FATTable)

            If UseBatchEditMode Then
                _FileBytes.BatchEditMode = True
            End If

            If SyncAll Then
                For Counter = 0 To _BootSector.NumberOfFATs - 1
                    Dim OldBytes = GetFAT(Counter)
                    If Not OldBytes.CompareTo(FATBytes, True) Then
                        SetFAT(Counter, FATBytes)
                        Updated = True
                    End If
                Next
            Else
                Dim OldBytes = GetFAT(_Index)
                If Not OldBytes.CompareTo(FATBytes, True) Then
                    SetFAT(_Index, FATBytes)
                    Updated = True
                End If
            End If

            If UseBatchEditMode Then
                _FileBytes.BatchEditMode = False
            End If

            If Updated Then
                ProcessFAT12()
            End If

            Return Updated
        End Function

        Private Function GetFATTableLength() As UShort
            Return Math.Min(_FATTable.Length - 1, _BootSector.NumberOfFATEntries + 1)
        End Function

        Public Sub ProcessFAT12()
            _FATChains.Clear()
            _FileAllocation.Clear()
            _FreeClusters = 0
            _BadClusters.Clear()
            _CircularChains.Clear()

            If _BootSector.IsValidImage Then
                Dim Length As UShort = GetFATTableLength()

                For Cluster = 2 To Length
                    If _FATTable(Cluster) = 0 Then
                        _FreeClusters += 1
                    ElseIf _FATTable(Cluster) = FAT_BAD_CLUSTER Then
                        _BadClusters.Add(Cluster)
                    End If
                Next Cluster
            End If
        End Sub

        Friend Function InitFATChain(Offset As UInteger, ClusterStart As UShort) As FATChain
            Dim FatChain As New FATChain(Offset)
            Dim Cluster As UShort = ClusterStart
            Dim AssignedClusters As New HashSet(Of UShort)
            Dim OffsetList As List(Of UInteger)
            Dim PrevCluster As UShort = 0

            If _FATTable IsNot Nothing Then
                Dim ClusterCount = GetFATTableLength()

                Do
                    If Cluster >= 2 And Cluster <= ClusterCount Then
                        If AssignedClusters.Contains(Cluster) Then
                            FatChain.HasCircularChain = True
                            _CircularChains.Add(PrevCluster)
                            Exit Do
                        End If
                        AssignedClusters.Add(Cluster)
                        FatChain.Clusters.Add(Cluster)
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
                        PrevCluster = Cluster
                        Cluster = _FATTable(Cluster)
                        If Cluster >= FAT_LAST_CLUSTER_START And Cluster <= FAT_LAST_CLUSTER_END Then
                            FatChain.OpenChain = False
                        End If
                    Else
                        Cluster = 0
                    End If
                Loop Until Cluster < 2 Or Cluster > ClusterCount

                _FATChains.Item(Offset) = FatChain
            End If

            Return FatChain
        End Function

        Public Shared Function GetFATSectors(FATRegionStart As UInteger, SectorsPerFAT As UShort, Index As UShort) As SectorRange
            Dim Sectors As SectorRange

            Sectors.Count = SectorsPerFAT
            Sectors.Start = FATRegionStart + (SectorsPerFAT * Index)

            Return Sectors
        End Function

        Private Function GetFAT(Index As UShort) As Byte()
            Dim Sectors = GetFATSectors(_BootSector.FATRegionStart, _BootSector.SectorsPerFAT, Index)

            Return _FileBytes.GetSectors(Sectors)
        End Function

        Private Function IsDataBlockEmpty(Data() As Byte) As Boolean
            Dim EmptyByte As Byte = Data(0)
            If EmptyByte <> &HF6 And EmptyByte <> &H0 Then
                Return False
            Else
                For Each B In Data
                    If B <> EmptyByte Then
                        Return False
                        Exit For
                    End If
                Next
            End If

            Return True
        End Function

        Private Sub SetFAT(Index As UShort, Data() As Byte)
            Dim Range = GetFATSectors(_BootSector.FATRegionStart, _BootSector.SectorsPerFAT, Index)
            Dim Offset = Disk.SectorToBytes(Range.Start)

            _FileBytes.SetBytes(Data, Offset)
        End Sub
    End Class
End Namespace
