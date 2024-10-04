Namespace DiskImage
    Public Class FAT12
        Public Const FAT_BAD_CLUSTER As UShort = &HFF7
        Public Const FAT_FREE_CLUSTER As UShort = &H0
        Public Const FAT_LAST_CLUSTER_END As UShort = &HFFF
        Public Const FAT_LAST_CLUSTER_START As UShort = &HFF8
        Public Const FAT_RESERVED_END As UShort = &HFF6
        Public Const FAT_RESERVED_START As UShort = &HFF0
        Public Shared ReadOnly ValidMediaDescriptor() As Byte = {&HF0, &HF8, &HF9, &HFA, &HFB, &HFC, &HFD, &HFE, &HFF}
        Private ReadOnly _BadClusters As List(Of UShort)
        Private ReadOnly _CircularChains As HashSet(Of UShort)
        Private ReadOnly _FATChains As Dictionary(Of UInteger, FATChain)
        Private ReadOnly _FileAllocation As Dictionary(Of UShort, List(Of UInteger))
        Private ReadOnly _FileBytes As ImageByteArray
        Private ReadOnly _Index As UShort
        Private ReadOnly _LostClusters As SortedSet(Of UShort)
        Private _BPB As BiosParameterBlock
        Private _FATTable() As UShort
        Private _FreeClusters As UInteger
        Private _HasMediaDescriptor As Boolean
        Private _MediaDescriptor As Byte
        Private _ReservedClusters As UInteger

        Public Enum FreeClusterEmum
            WithData
            WithoutData
            All
        End Enum

        Sub New(FileBytes As ImageByteArray, BPB As BiosParameterBlock, Index As UShort)
            _BPB = BPB
            _FileBytes = FileBytes
            _Index = Index
            _FATChains = New Dictionary(Of UInteger, FATChain)
            _FileAllocation = New Dictionary(Of UShort, List(Of UInteger))
            _FreeClusters = 0
            _ReservedClusters = 0
            _BadClusters = New List(Of UShort)
            _CircularChains = New HashSet(Of UShort)
            _LostClusters = New SortedSet(Of UShort)

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

        Public ReadOnly Property LostClusters As SortedSet(Of UShort)
            Get
                Return _LostClusters
            End Get
        End Property

        Public ReadOnly Property MediaDescriptor As Byte
            Get
                Return _MediaDescriptor
            End Get
        End Property

        Public ReadOnly Property ReservedClusters As UInteger
            Get
                Return _ReservedClusters
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

        Public Function GetAllocatedClusterCount() As Integer
            Return _FileAllocation.Count
        End Function

        Public Function GetFAT() As Byte()
            Dim SectorStart = _BPB.FATRegionStart + (_BPB.SectorsPerFAT * _Index)

            Return _FileBytes.GetSectors(SectorStart, _BPB.SectorsPerFAT)
        End Function

        Public Function GetFATTableLength() As UShort
            Return Math.Min(_FATTable.Length - 1, _BPB.NumberOfFATEntries + 1)
        End Function

        Public Function GetFreeClusters(FreeClusterType As FreeClusterEmum, Optional StartingCluster As UShort = 2) As List(Of UShort)
            Dim ClusterChain As New List(Of UShort)
            Dim Cluster As UShort

            If _FATTable IsNot Nothing Then
                Dim ClusterSize As UInteger = _BPB.BytesPerCluster
                Dim AddCluster As Boolean
                Dim Length = GetFATTableLength()

                If StartingCluster < 2 Then
                    StartingCluster = 2
                End If

                For Index As UShort = 0 To Length - 2
                    Cluster = (StartingCluster - 2 + Index) Mod (Length - 1) + 2
                    If _FATTable(Cluster) = 0 Then
                        Dim Offset = _BPB.ClusterToOffset(Cluster)
                        If _FileBytes.Length < Offset + ClusterSize Then
                            ClusterSize = Math.Max(_FileBytes.Length - Offset, 0)
                        End If
                        If ClusterSize > 0 Then
                            If FreeClusterType = FreeClusterEmum.All Then
                                AddCluster = True
                            Else
                                Dim Empty = IsDataBlockEmpty(_FileBytes.GetBytes(Offset, ClusterSize))
                                If FreeClusterType = FreeClusterEmum.WithData Then
                                    AddCluster = Not Empty
                                Else
                                    AddCluster = Empty
                                End If
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

        Public Function GetFreeSpace(ClusterSize As UInteger) As UInteger
            Return _FreeClusters * ClusterSize
        End Function

        Public Function GetLostClusterCount() As UShort
            Return _LostClusters.Count
        End Function

        Public Function GetOffset() As UInteger
            Dim SectorStart = _BPB.FATRegionStart + (_BPB.SectorsPerFAT * _Index)
            Return Disk.SectorToBytes(SectorStart)
        End Function

        Public Function HasFreeClusters(FreeClusterType As FreeClusterEmum) As Boolean

            If _FATTable IsNot Nothing Then
                Dim ClusterSize As UInteger = _BPB.BytesPerCluster
                Dim Length = GetFATTableLength()

                For Cluster As UShort = 2 To Length
                    If _FATTable(Cluster) = 0 Then
                        Dim Offset = _BPB.ClusterToOffset(Cluster)
                        If _FileBytes.Length < Offset + ClusterSize Then
                            ClusterSize = Math.Max(_FileBytes.Length - Offset, 0)
                        End If
                        If ClusterSize > 0 Then
                            If FreeClusterType = FreeClusterEmum.All Then
                                Return True
                            Else
                                Dim Empty = IsDataBlockEmpty(_FileBytes.GetBytes(Offset, ClusterSize))
                                If FreeClusterType = FreeClusterEmum.WithData Then
                                    If Not Empty Then
                                        Return True
                                    End If
                                Else
                                    If Empty Then
                                        Return True
                                    End If
                                End If
                            End If
                        Else
                            Exit For
                        End If
                    End If
                Next
            End If

            Return False
        End Function

        Public Function HasMediaDescriptor() As Boolean
            Return _HasMediaDescriptor
        End Function

        Public Function HasValidMediaDescriptor() As Boolean
            Return ValidMediaDescriptor.Contains(MediaDescriptor)
        End Function

        Public Sub PopulateFAT12()
            Erase _FATTable

            If _BPB.IsValid Then
                Dim FATBytes = GetFAT()
                Dim Size = _BPB.NumberOfFATEntries + 2
                _FATTable = DecodeFAT12(FATBytes, Size)
                _HasMediaDescriptor = FATBytes(1) = &HFF And FATBytes(2) = &HFF
                _MediaDescriptor = FATBytes(0)
            End If

            ProcessFAT12()
        End Sub

        Public Sub PopulateFAT12(BPB As BiosParameterBlock)
            _BPB = BPB
            PopulateFAT12()
        End Sub

        Public Sub ProcessFAT12()
            _FATChains.Clear()
            _FileAllocation.Clear()
            _FreeClusters = 0
            _ReservedClusters = 0
            _BadClusters.Clear()
            _CircularChains.Clear()
            _LostClusters.Clear()

            If _BPB.IsValid Then
                Dim Length As UShort = GetFATTableLength()
                Dim Value As UShort

                For Cluster As UShort = 2 To Length
                    Value = _FATTable(Cluster)
                    If Value = 0 Then
                        _FreeClusters += 1
                    ElseIf Value = FAT_BAD_CLUSTER Then
                        _BadClusters.Add(Cluster)
                    ElseIf Value >= 2 And Value <= Length Then
                        _LostClusters.Add(Cluster)
                    ElseIf Value >= FAT12.FAT_LAST_CLUSTER_START And Value <= FAT12.FAT_LAST_CLUSTER_END Then
                        _LostClusters.Add(Cluster)
                    ElseIf Value = 1 Or (Value >= FAT12.FAT_RESERVED_START And Value <= FAT12.FAT_RESERVED_END) Then
                        _ReservedClusters += 1
                    End If
                Next Cluster
            End If
        End Sub

        Public Function UpdateFAT12() As Boolean
            Dim Updated As Boolean = False
            Dim UseBatchEditMode As Boolean = Not _FileBytes.BatchEditMode

            Dim FATBytes = EncodeFAT12(_FATTable)

            If UseBatchEditMode Then
                _FileBytes.BatchEditMode = True
            End If

            Dim OldBytes = GetFAT()
            If Not OldBytes.CompareTo(FATBytes, True) Then
                SetFAT(FATBytes)
                Updated = True
            End If

            If UseBatchEditMode Then
                _FileBytes.BatchEditMode = False
            End If

            If Updated Then
                ProcessFAT12()
            End If

            Return Updated
        End Function

        Public Function UpdateMedaDescriptor(Value As Byte) As Boolean
            If Value <> _MediaDescriptor Then
                _MediaDescriptor = Value
                Dim Offset = GetOffset()
                _FileBytes.SetBytes(Value, Offset)
                Return True
            End If

            Return False
        End Function

        Friend Function InitFATChain(Offset As UInteger, ClusterStart As UShort) As FATChain
            Dim FATChain As New FATChain(Offset)
            Dim Cluster As UShort = ClusterStart
            Dim AssignedClusters As New HashSet(Of UShort)
            Dim OffsetList As List(Of UInteger)
            Dim PrevCluster As UShort = 0

            If _FATTable IsNot Nothing Then
                Dim ClusterCount = GetFATTableLength()

                Do
                    If Cluster >= 2 And Cluster <= ClusterCount Then
                        If AssignedClusters.Contains(Cluster) Then
                            FATChain.HasCircularChain = True
                            _CircularChains.Add(PrevCluster)
                            Exit Do
                        End If
                        AssignedClusters.Add(Cluster)
                        FATChain.Clusters.Add(Cluster)
                        If Not _FileAllocation.ContainsKey(Cluster) Then
                            OffsetList = New List(Of UInteger) From {
                                Offset
                            }
                            _FileAllocation.Add(Cluster, OffsetList)
                            If _LostClusters.Contains(Cluster) Then
                                _LostClusters.Remove(Cluster)
                            End If
                        Else
                            OffsetList = _FileAllocation.Item(Cluster)
                            For Each OffsetItem In OffsetList
                                If Not FATChain.CrossLinks.Contains(OffsetItem) Then
                                    FATChain.CrossLinks.Add(OffsetItem)
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
                            FATChain.OpenChain = False
                        End If
                    Else
                        Cluster = 0
                    End If
                Loop Until Cluster < 2 Or Cluster > ClusterCount

                _FATChains.Item(Offset) = FATChain
            End If

            Return FATChain
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

        Private Sub SetFAT(Data() As Byte)
            Dim Offset = GetOffset()

            _FileBytes.SetBytes(Data, Offset)
        End Sub
    End Class
End Namespace
