Namespace DiskImage
    Public Class FATAllocation
        Private ReadOnly _FAT As FAT12
        Private ReadOnly _CircularChains As HashSet(Of UShort)
        Private ReadOnly _FATChains As Dictionary(Of DirectoryEntry, FATChain)
        Private ReadOnly _FileAllocation As Dictionary(Of UShort, List(Of DirectoryEntry))
        Private _LostClusters As SortedSet(Of UShort)

        Public Sub New(FAT As FAT12)
            _FAT = FAT
            _FATChains = New Dictionary(Of DirectoryEntry, FATChain)
            _FileAllocation = New Dictionary(Of UShort, List(Of DirectoryEntry))
            _CircularChains = New HashSet(Of UShort)
            _LostClusters = New SortedSet(Of UShort)(FAT.AllocatedClusters)
        End Sub

        Public ReadOnly Property CircularChains As HashSet(Of UShort)
            Get
                Return _CircularChains
            End Get
        End Property

        Public ReadOnly Property FATChains As Dictionary(Of DirectoryEntry, FATChain)
            Get
                Return _FATChains
            End Get
        End Property

        Public ReadOnly Property FileAllocation As Dictionary(Of UShort, List(Of DirectoryEntry))
            Get
                Return _FileAllocation
            End Get
        End Property

        Public ReadOnly Property LostClusters As SortedSet(Of UShort)
            Get
                Return _LostClusters
            End Get
        End Property

        Public Sub Clear()
            _FATChains.Clear()
            _FileAllocation.Clear()
            _CircularChains.Clear()
            _LostClusters = New SortedSet(Of UShort)(_FAT.AllocatedClusters)
        End Sub

        Public Function InitFATChain(DirectoryEntry As DirectoryEntry) As FATChain
            Dim FATChain As New FATChain(DirectoryEntry)
            Dim Cluster As UShort
            Dim AssignedClusters As New HashSet(Of UShort)
            Dim DirectoryEntryList As List(Of DirectoryEntry)
            Dim PrevCluster As UShort = 0

            If _FAT.TableLength > 0 Then
                If DirectoryEntry.IsDeleted() Then
                    Cluster = 0
                ElseIf DirectoryEntry.IsDirectory() AndAlso DirectoryEntry.IsLink() Then
                    Cluster = 0
                Else
                    Cluster = DirectoryEntry.StartingCluster
                End If

                Dim ClusterCount = _FAT.TableLength

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
                            DirectoryEntryList = New List(Of DirectoryEntry) From {
                                DirectoryEntry
                            }
                            _FileAllocation.Add(Cluster, DirectoryEntryList)
                            If _LostClusters.Contains(Cluster) Then
                                _LostClusters.Remove(Cluster)
                            End If
                        Else
                            DirectoryEntryList = _FileAllocation.Item(Cluster)
                            For Each Entry In DirectoryEntryList
                                If Not FATChain.CrossLinks.Contains(Entry) Then
                                    FATChain.CrossLinks.Add(Entry)
                                End If
                                If _FATChains.ContainsKey(Entry) Then
                                    Dim CrossLinks = _FATChains.Item(Entry).CrossLinks
                                    If Not CrossLinks.Contains(DirectoryEntry) Then
                                        CrossLinks.Add(DirectoryEntry)
                                    End If
                                End If
                            Next
                            DirectoryEntryList.Add(DirectoryEntry)
                        End If
                        PrevCluster = Cluster
                        Cluster = _FAT.TableEntry(Cluster)
                        If Cluster >= FAT12.FAT_LAST_CLUSTER_START And Cluster <= FAT12.FAT_LAST_CLUSTER_END Then
                            FATChain.OpenChain = False
                        End If
                    Else
                        Cluster = 0
                    End If
                Loop Until Cluster < 2 Or Cluster > ClusterCount
            End If

            FATChains.Item(DirectoryEntry) = FATChain

            Return FATChain
        End Function
    End Class
End Namespace
