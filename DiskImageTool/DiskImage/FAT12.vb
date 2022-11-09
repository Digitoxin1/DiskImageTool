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

        Sub New(FileBytes As ImageByteArray, BootSector As BootSector)
            _BootSector = BootSector
            _FileBytes = FileBytes
            _FATChains = New Dictionary(Of UInteger, FATChain)
            _FileAllocation = New Dictionary(Of UInteger, List(Of UInteger))
            _FreeSpace = 0
            _BadClusters = New List(Of UShort)
            _BadSectors = New HashSet(Of UInteger)

            If BootSector.IsValidImage Then
                PopulateFAT12(0)
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

        Public ReadOnly Property TableEntry(Cluster As UShort) As UShort
            Get
                Return _FATTable(Cluster)
            End Get
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

            For Cluster As UShort = 1 To _FATTable.Length - 1
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

            For Cluster As UShort = 1 To _FATTable.Length - 1
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

        Public Sub PopulateFAT12(Index As UShort)
            Erase _FATTable
            _FATChains.Clear()
            _FileAllocation.Clear()
            _FreeSpace = 0
            _BadClusters.Clear()
            _BadSectors.Clear()

            If _BootSector.IsValidImage Then
                Dim Size As UShort = _BootSector.NumberOfFATEntries + 1
                Dim ClusterSize As UInteger = _BootSector.BytesPerCluster
                Dim FATBytes = GetFAT(Index)

                ReDim _FATTable(Size)

                Dim b As UInteger
                Dim Start As Integer = 0
                Dim Cluster As UShort = 0
                Do While Cluster <= Size And Start < FATBytes.Length - 3
                    b = BitConverter.ToUInt32(FATBytes, Start)
                    _FATTable(Cluster) = b Mod 4096
                    If _FATTable(Cluster) = 0 Then
                        _FreeSpace += ClusterSize
                    ElseIf _FATTable(Cluster) = FAT_BADCLUSTER Then
                        _BadClusters.Add(Cluster)
                        For Counter = 0 To _BootSector.SectorsPerCluster - 1
                            _BadSectors.Add(_BootSector.ClusterToSector(Cluster) + Counter)
                        Next Counter
                    End If
                    Cluster += 1
                    If Cluster <= Size And Start < FATBytes.Length - 3 Then
                        b >>= 12
                        _FATTable(Cluster) = b Mod 4096
                        If _FATTable(Cluster) = 0 Then
                            _FreeSpace += ClusterSize
                        ElseIf _FATTable(Cluster) = FAT_BADCLUSTER Then
                            _BadClusters.Add(Cluster)
                            For Counter = 0 To _BootSector.SectorsPerCluster - 1
                                _BadSectors.Add(_BootSector.ClusterToSector(Cluster) + Counter)
                            Next Counter
                        End If
                        Cluster += 1
                    End If
                    Start += 3
                Loop
            End If
        End Sub

        Friend Function InitFATChain(Offset As UInteger, ClusterStart As UShort) As FATChain
            Dim FatChain As New FATChain
            Dim Cluster As UShort = ClusterStart
            Dim AssignedClusters As New HashSet(Of UShort)
            Dim OffsetList As List(Of UInteger)
            Dim ClusterCount = _FATTable.Length - 1

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

            Return FatChain
        End Function

        Private Function GetFAT(Index As UShort) As Byte()
            Dim SectorCount = _BootSector.SectorsPerFAT
            Dim SectorStart = _BootSector.FATRegionStart + (SectorCount * Index)

            Return _FileBytes.GetSectors(SectorStart, SectorCount)
        End Function
    End Class
End Namespace
