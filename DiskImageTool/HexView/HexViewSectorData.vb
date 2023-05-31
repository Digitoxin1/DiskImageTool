Imports DiskImageTool.DiskImage

Public Class HexViewSectorData
    Public Sub New(Disk As Disk)
        _Disk = Disk
        _SectorData = New SectorData(Disk.Data)
        _HighlightedRegionList = New List(Of HighlightedRegions)
    End Sub

    Public Sub New(Disk As Disk, ClusterChain As List(Of UShort))
        _Disk = Disk
        _SectorData = New SectorData(Disk.Data)
        _SectorData.AddBlocksByChain(ClusterListToSectorList(_Disk.BPB, ClusterChain))
        _HighlightedRegionList = New List(Of HighlightedRegions)
    End Sub

    Public Sub New(Disk As Disk, SectorChain As List(Of UInteger))
        _Disk = Disk
        _SectorData = New SectorData(Disk.Data)
        _SectorData.AddBlocksByChain(SectorChain)
        _HighlightedRegionList = New List(Of HighlightedRegions)
    End Sub

    Public Sub New(Disk As Disk, Offset As UInteger, Length As UInteger)
        _Disk = Disk
        _SectorData = New SectorData(Disk.Data)
        _SectorData.AddBlockByOffset(Offset, Length)
        _HighlightedRegionList = New List(Of HighlightedRegions)
    End Sub

    Public Property Description As String
    Public ReadOnly Property Disk As Disk
    Public ReadOnly Property HighlightedRegionList As List(Of HighlightedRegions)
    Public ReadOnly Property SectorData As SectorData
End Class
