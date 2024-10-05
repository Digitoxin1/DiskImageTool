Namespace DiskImage
    Public Class FAT12
        Inherits FAT12Base
        Private ReadOnly _CircularChains As HashSet(Of UShort)
        Private ReadOnly _FATChains As Dictionary(Of UInteger, FATChain)
        Private ReadOnly _FileAllocation As Dictionary(Of UShort, List(Of UInteger))
        Private ReadOnly _FileBytes As ImageByteArray
        Private _LostClusters As SortedSet(Of UShort)
        Private _BPB As BiosParameterBlock

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
            _CircularChains = New HashSet(Of UShort)
            _LostClusters = New SortedSet(Of UShort)
            ProcessFAT12()
            GetFreeClusters(FreeClusterEmum.WithData)
        End Sub

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

        Public ReadOnly Property LostClusters As SortedSet(Of UShort)
            Get
                Return _LostClusters
            End Get
        End Property

        Public Function GetAllocatedClusterCount() As Integer
            Return _FileAllocation.Count
        End Function

        Public Function GetFreeClusters(FreeClusterType As FreeClusterEmum) As SortedSet(Of UShort)
            If FreeClusterType = FreeClusterEmum.All Then
                Return New SortedSet(Of UShort)(FreeClusters)
            Else
                Dim ClusterList = New SortedSet(Of UShort)
                For Each Cluster In FreeClusters
                    Dim ClusterEmpty = IsClusterEmpty(Cluster)
                    If FreeClusterType = FreeClusterEmum.WithData And Not ClusterEmpty Then
                        ClusterList.Add(Cluster)
                    ElseIf FreeClusterType = FreeClusterEmum.WithoutData And ClusterEmpty Then
                        ClusterList.Add(Cluster)
                    End If
                Next
                Return ClusterList
            End If
        End Function

        Public Function HasFreeClusters(FreeClusterType As FreeClusterEmum) As Boolean
            If FreeClusterType = FreeClusterEmum.All Then
                Return FreeClusters.Count > 0
            Else
                For Each Cluster In FreeClusters
                    Dim ClusterEmpty = IsClusterEmpty(Cluster)
                    If FreeClusterType = FreeClusterEmum.WithData And Not ClusterEmpty Then
                        Return True
                    ElseIf FreeClusterType = FreeClusterEmum.WithoutData And ClusterEmpty Then
                        Return True
                    End If
                Next

                Return False
            End If
        End Function

        Public Function GetNextFreeCluster(ClusterList As SortedSet(Of UShort), Optional StartingCluster As UShort = 2) As UShort
            Dim Result As UShort = 0

            Dim NextCluster = StartingCluster
            If NextCluster < 2 Or NextCluster > TableLength Then
                NextCluster = 2
            End If

            Do
                If ClusterList.Contains(NextCluster) Then
                    Result = NextCluster
                    Exit Do
                End If
                NextCluster += 1
                If NextCluster > TableLength Then
                    NextCluster = 2
                End If
            Loop Until NextCluster = StartingCluster

            Return Result
        End Function

        Public Sub Reinitialize(BPB As BiosParameterBlock)
            _BPB = BPB
            InitializeFAT(BPB)
            ProcessFAT12()
        End Sub

        Public Sub ProcessFAT12()
            _FATChains.Clear()
            _FileAllocation.Clear()
            _CircularChains.Clear()
            _LostClusters = New SortedSet(Of UShort)(AllocatedClusters)
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

        Private Function IsClusterEmpty(Cluster As UShort) As Boolean
            Dim Offset = _BPB.ClusterToOffset(Cluster)
            Dim ClusterSize As UInteger = _BPB.BytesPerCluster

            Dim Data = _FileBytes.GetBytes(Offset, ClusterSize)
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
