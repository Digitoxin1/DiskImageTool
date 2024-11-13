Namespace DiskImage
    Public Class FAT12
        Public Enum FreeClusterEmum
            WithData
            WithoutData
            All
        End Enum

        Public Const FAT_BAD_CLUSTER As UShort = &HFF7
        Public Const FAT_FREE_CLUSTER As UShort = &H0
        Public Const FAT_LAST_CLUSTER_END As UShort = &HFFF
        Public Const FAT_LAST_CLUSTER_START As UShort = &HFF8
        Public Const FAT_RESERVED_END As UShort = &HFF6
        Public Const FAT_RESERVED_START As UShort = &HFF0
        Public Shared ReadOnly ValidMediaDescriptor() As Byte = {&HF0, &HF8, &HF9, &HFA, &HFB, &HFC, &HFD, &HFE, &HFF}

        Private ReadOnly _FloppyImage As IFloppyImage
        Private ReadOnly _Index As UShort
        Private ReadOnly _AllocatedClusters As SortedSet(Of UShort)
        Private ReadOnly _BadClusters As SortedSet(Of UShort)
        Private ReadOnly _FreeClusters As SortedSet(Of UShort)
        Private ReadOnly _ReservedClusters As SortedSet(Of UShort)
        Private _BPB As BiosParameterBlock
        Private _Offset As UInteger
        Private _Size As UInteger
        Private _Entries As UInteger
        Private _BytesPerCluster As UInteger
        Private _FATTable() As UShort

        Public Sub New(FloppyImage As IFloppyImage, BPB As BiosParameterBlock, Index As UShort)
            _AllocatedClusters = New SortedSet(Of UShort)
            _BadClusters = New SortedSet(Of UShort)
            _FreeClusters = New SortedSet(Of UShort)
            _ReservedClusters = New SortedSet(Of UShort)

            _FloppyImage = FloppyImage
            _BPB = BPB
            _Index = Index

            InitializeFAT(BPB)
        End Sub

        Public Sub InitializeFAT(BPB As BiosParameterBlock)
            _BPB = BPB
            If BPB.IsValid Then
                _Offset = (BPB.FATRegionStart + (BPB.SectorsPerFAT * _Index)) * Disk.BYTES_PER_SECTOR
                _Size = BPB.SectorsPerFAT * Disk.BYTES_PER_SECTOR
                _Entries = BPB.NumberOfFATEntries + 2
                _BytesPerCluster = BPB.BytesPerCluster

                _FATTable = DecodeFAT12(Data, _Entries)
            Else
                _Offset = 0
                _Size = 0
                _Entries = 1
                _BytesPerCluster = 0
                _FATTable = New UShort(1) {}
            End If

            ProcessFAT()
        End Sub

        Private Sub ProcessFAT()
            _AllocatedClusters.Clear()
            _BadClusters.Clear()
            _FreeClusters.Clear()
            _ReservedClusters.Clear()

            If TableLength > 0 Then
                For Cluster As UShort = 2 To TableLength
                    AddLookup(Cluster)
                Next Cluster
            End If
        End Sub

        Public ReadOnly Property AllocatedClusters As SortedSet(Of UShort)
            Get
                Return _AllocatedClusters
            End Get
        End Property

        Public ReadOnly Property BadClusters As SortedSet(Of UShort)
            Get
                Return _BadClusters
            End Get
        End Property

        Public ReadOnly Property Data As Byte()
            Get
                Return _FloppyImage.GetBytes(_Offset, _Size)
            End Get
        End Property

        Public ReadOnly Property FreeClusters As SortedSet(Of UShort)
            Get
                Return _FreeClusters
            End Get
        End Property

        Public ReadOnly Property HasMediaDescriptor As Boolean
            Get
                Return _FloppyImage.GetBytesShort(_Offset + 1) = &HFFFF
            End Get
        End Property

        Public Property MediaDescriptor As Byte
            Get
                Return _FloppyImage.GetByte(_Offset)
            End Get
            Set(value As Byte)
                _FloppyImage.SetBytes(value, _Offset)
            End Set
        End Property

        Public ReadOnly Property ReservedClusters As SortedSet(Of UShort)
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
                    RemoveLookup(Cluster)
                    _FATTable(Cluster) = value
                    AddLookup(Cluster)
                End If
            End Set
        End Property

        Public ReadOnly Property TableLength As UShort
            Get
                Return _Entries - 1
            End Get
        End Property

        Public Function GetFreeClusters(ClustersRequired As UShort) As SortedSet(Of UShort)
            Dim Clusterlist = GetFreeClusters(FreeClusterEmum.WithoutData)

            If Clusterlist.Count < ClustersRequired Then
                Clusterlist = GetFreeClusters(FreeClusterEmum.All)
            End If

            If Clusterlist.Count < ClustersRequired Then
                Clusterlist = Nothing
            End If

            Return Clusterlist
        End Function

        Public Function GetFreeClusters(FreeClusterType As FreeClusterEmum) As SortedSet(Of UShort)
            Dim ClusterList = New SortedSet(Of UShort)
            For Each Cluster In _FreeClusters
                Dim ProtectedCluster As Boolean = False

                Dim SectorList = ClusterToSectorList(_BPB, Cluster)
                For Each Sector In SectorList
                    If _FloppyImage.ProtectedSectors.Contains(Sector) Then
                        ProtectedCluster = True
                        Exit For
                    End If
                Next

                If Not ProtectedCluster Then
                    If FreeClusterType = FreeClusterEmum.All Then
                        ClusterList.Add(Cluster)
                    Else
                        Dim ClusterEmpty = IsClusterEmpty(Cluster)
                        If FreeClusterType = FreeClusterEmum.WithData And Not ClusterEmpty Then
                            ClusterList.Add(Cluster)
                        ElseIf FreeClusterType = FreeClusterEmum.WithoutData And ClusterEmpty Then
                            ClusterList.Add(Cluster)
                        End If
                    End If
                End If
            Next
            Return ClusterList
        End Function

        Public Function GetFreeSpace() As UInteger
            Return _FreeClusters.Count * _BytesPerCluster
        End Function

        Public Function GetNextFreeCluster(Optional Remove As Boolean = False, Optional StartingCluster As UShort = 2) As UShort
            Return GetNextFreeCluster(_FreeClusters, Remove, StartingCluster)
        End Function

        Public Function GetNextFreeCluster(ClusterList As SortedSet(Of UShort), Optional Remove As Boolean = False, Optional StartingCluster As UShort = 2) As UShort
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

            If Result > 0 And Remove Then
                ClusterList.Remove(Result)
            End If

            Return Result
        End Function

        Public Function HasFreeClusters(FreeClusterType As FreeClusterEmum) As Boolean
            For Each Cluster In _FreeClusters
                Dim ProtectedCluster As Boolean = False

                Dim SectorList = ClusterToSectorList(_BPB, Cluster)
                For Each Sector In SectorList
                    If _FloppyImage.ProtectedSectors.Contains(Sector) Then
                        ProtectedCluster = True
                        Exit For
                    End If
                Next

                If Not ProtectedCluster Then
                    If FreeClusterType = FreeClusterEmum.All Then
                        Return True
                    Else
                        Dim ClusterEmpty = IsClusterEmpty(Cluster)
                        If FreeClusterType = FreeClusterEmum.WithData And Not ClusterEmpty Then
                            Return True
                        ElseIf FreeClusterType = FreeClusterEmum.WithoutData And ClusterEmpty Then
                            Return True
                        End If
                    End If
                End If
            Next

            Return False
        End Function

        Public Function HasValidMediaDescriptor() As Boolean
            Return ValidMediaDescriptor.Contains(MediaDescriptor)
        End Function

        Public Function UpdateFAT() As Boolean
            Dim FATBytes = EncodeFAT12(_FATTable)
            If Not Data.CompareTo(FATBytes, True) Then
                _FloppyImage.SetBytes(FATBytes, _Offset)
                Return True
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

        Private Sub AddLookup(Cluster As UShort)
            Dim Value As UShort = _FATTable(Cluster)
            If Value = 0 Then
                If Not _FreeClusters.Contains(Cluster) Then
                    _FreeClusters.Add(Cluster)
                End If
            ElseIf Value = FAT_BAD_CLUSTER Then
                If Not _BadClusters.Contains(Cluster) Then
                    _BadClusters.Add(Cluster)
                End If
            ElseIf Value >= 2 And Value <= TableLength Then
                If Not _AllocatedClusters.Contains(Cluster) Then
                    _AllocatedClusters.Add(Cluster)
                End If
            ElseIf Value >= FAT12.FAT_LAST_CLUSTER_START And Value <= FAT12.FAT_LAST_CLUSTER_END Then
                If Not _AllocatedClusters.Contains(Cluster) Then
                    _AllocatedClusters.Add(Cluster)
                End If
            ElseIf Value = 1 Or (Value >= FAT12.FAT_RESERVED_START And Value <= FAT12.FAT_RESERVED_END) Then
                If Not _ReservedClusters.Contains(Cluster) Then
                    _ReservedClusters.Add(Cluster)
                End If
            End If
        End Sub

        Private Function IsClusterEmpty(Cluster As UShort) As Boolean
            Dim Offset = _BPB.ClusterToOffset(Cluster)
            Dim ClusterSize As UInteger = _BPB.BytesPerCluster

            Dim Data = _FloppyImage.GetBytes(Offset, ClusterSize)
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

        Private Sub RemoveLookup(Cluster As UShort)
            Dim Value As UShort = _FATTable(Cluster)
            If Value = 0 Then
                If _FreeClusters.Contains(Cluster) Then
                    _FreeClusters.Remove(Cluster)
                End If
            ElseIf Value = FAT_BAD_CLUSTER Then
                If _BadClusters.Contains(Cluster) Then
                    _BadClusters.Remove(Cluster)
                End If
            ElseIf Value >= 2 And Value <= TableLength Then
                If _AllocatedClusters.Contains(Cluster) Then
                    _AllocatedClusters.Remove(Cluster)
                End If
            ElseIf Value >= FAT12.FAT_LAST_CLUSTER_START And Value <= FAT12.FAT_LAST_CLUSTER_END Then
                If _AllocatedClusters.Contains(Cluster) Then
                    _AllocatedClusters.Remove(Cluster)
                End If
            ElseIf Value = 1 Or (Value >= FAT12.FAT_RESERVED_START And Value <= FAT12.FAT_RESERVED_END) Then
                If _ReservedClusters.Contains(Cluster) Then
                    _ReservedClusters.Remove(Cluster)
                End If
            End If
        End Sub
    End Class
End Namespace
