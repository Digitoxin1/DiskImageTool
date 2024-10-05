Namespace DiskImage
    Public Class FAT12
        Inherits FAT12Base
        Private ReadOnly _BadClusters As List(Of UShort)
        Private ReadOnly _CircularChains As HashSet(Of UShort)
        Private ReadOnly _FATChains As Dictionary(Of UInteger, FATChain)
        Private ReadOnly _FileAllocation As Dictionary(Of UShort, List(Of UInteger))
        Private ReadOnly _FileBytes As ImageByteArray
        Private ReadOnly _LostClusters As SortedSet(Of UShort)
        Private _BPB As BiosParameterBlock
        Private _FreeClusters As UInteger
        Private _ReservedClusters As UInteger

        Public Enum FreeClusterEmum
            WithData
            WithoutData
            All
        End Enum

        Sub New(FileBytes As ImageByteArray, BPB As BiosParameterBlock, Index As UShort)
            MyBase.New(FileBytes, BPB, Index)
            _BPB = BPB
            _FileBytes = FileBytes
            _FATChains = New Dictionary(Of UInteger, FATChain)
            _FileAllocation = New Dictionary(Of UShort, List(Of UInteger))
            _FreeClusters = 0
            _ReservedClusters = 0
            _BadClusters = New List(Of UShort)
            _CircularChains = New HashSet(Of UShort)
            _LostClusters = New SortedSet(Of UShort)

            ProcessFAT12()
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

        Public ReadOnly Property ReservedClusters As UInteger
            Get
                Return _ReservedClusters
            End Get
        End Property

        Public Function GetAllocatedClusterCount() As Integer
            Return _FileAllocation.Count
        End Function


        Public Function GetFreeClusters(FreeClusterType As FreeClusterEmum, Optional StartingCluster As UShort = 2) As List(Of UShort)
            Dim ClusterChain As New List(Of UShort)
            Dim Cluster As UShort

            If TableLength > 0 Then
                Dim ClusterSize As UInteger = _BPB.BytesPerCluster
                Dim AddCluster As Boolean

                If StartingCluster < 2 Then
                    StartingCluster = 2
                End If

                For Index As UShort = 0 To TableLength - 2
                    Cluster = (StartingCluster - 2 + Index) Mod (TableLength - 1) + 2
                    If TableEntry(Cluster) = 0 Then
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

        Public Function HasFreeClusters(FreeClusterType As FreeClusterEmum) As Boolean

            If TableLength > 0 Then
                Dim ClusterSize As UInteger = _BPB.BytesPerCluster

                For Cluster As UShort = 2 To TableLength
                    If TableEntry(Cluster) = 0 Then
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

        Public Sub Reinitialize(BPB As BiosParameterBlock)
            _BPB = BPB
            InitializeFAT(BPB)
            ProcessFAT12()
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
                Dim Length As UShort = TableLength
                Dim Value As UShort

                For Cluster As UShort = 2 To Length
                    Value = TableEntry(Cluster)
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
            Dim Result = UpdateFAT()

            If Result Then
                ProcessFAT12()
            End If

            Return Result
        End Function

        Friend Function InitFATChain(Offset As UInteger, ClusterStart As UShort) As FATChain
            Dim FATChain As New FATChain(Offset)
            Dim Cluster As UShort = ClusterStart
            Dim AssignedClusters As New HashSet(Of UShort)
            Dim OffsetList As List(Of UInteger)
            Dim PrevCluster As UShort = 0

            If TableLength > 0 Then
                Dim ClusterCount = TableLength

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
                        Cluster = TableEntry(Cluster)
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
    End Class
End Namespace
