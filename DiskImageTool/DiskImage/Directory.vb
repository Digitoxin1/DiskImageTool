Namespace DiskImage
    Public Class Directory
        Private ReadOnly _FatClusterList As List(Of UShort)
        Private ReadOnly _Parent As Disk

        Sub New(Parent As Disk, Optional FatClusterList As List(Of UShort) = Nothing)
            _Parent = Parent
            _FatClusterList = FatClusterList
        End Sub

        Private Function GetDataBlockRoot() As DataBlock
            Dim SectorStart = _Parent.BootSector.RootDirectoryRegionStart
            Dim SectorEnd = _Parent.BootSector.DataRegionStart
            Dim Length = (SectorEnd - SectorStart) * _Parent.BootSector.BytesPerSector

            Return _Parent.NewDataBlock(_Parent.SectorToOffset(SectorStart), Length)
        End Function

        Private Function GetDataBlocksSubDirectory() As List(Of DataBlock)
            Return _Parent.GetDataBlocksFromFATClusterList(_FatClusterList)
        End Function

        Private Function GetDirectoryLengthSubDirectory(FileCountOnly As Boolean) As UInteger
            Dim Count As UInteger = 0

            For Each Cluster In _FatClusterList
                Dim OffsetStart As UInteger = _Parent.ClusterToOffset(Cluster)
                Dim OffsetLength As UInteger = _Parent.BootSector.BytesPerCluster


                Count += _Parent.GetDirectoryLength(OffsetStart, OffsetStart + OffsetLength, FileCountOnly)
            Next

            Return Count
        End Function

        Private Function GetDirectoryLengthRoot(FileCountOnly As Boolean) As UInteger
            Dim OffsetStart As UInteger = _Parent.SectorToOffset(_Parent.BootSector.RootDirectoryRegionStart)
            Dim OffsetEnd As UInteger = _Parent.SectorToOffset(_Parent.BootSector.DataRegionStart)

            Return _Parent.GetDirectoryLength(OffsetStart, OffsetEnd, FileCountOnly)
        End Function

        Private Function GetFileSubDirectory(Index As UInteger) As DiskImage.DirectoryEntry
            Dim EntriesPerCluster As UInteger = _Parent.BootSector.BytesPerCluster / 32
            Dim ChainIndex As UInteger = (Index - 1) \ EntriesPerCluster
            Dim ClusterIndex As UInteger = (Index - 1) Mod EntriesPerCluster
            Dim Offset As UInteger = _Parent.ClusterToOffset(_FatClusterList.Item(ChainIndex)) + ClusterIndex * 32

            Return New DiskImage.DirectoryEntry(_Parent, Offset)
        End Function

        Private Function GetFileRoot(Index As Integer) As DiskImage.DirectoryEntry
            Dim Offset As UInteger = _Parent.SectorToOffset(_Parent.BootSector.RootDirectoryRegionStart) + (Index - 1) * 32

            Return New DiskImage.DirectoryEntry(_Parent, Offset)
        End Function

        Public Function DirectoryLength() As UInteger
            If _FatClusterList Is Nothing Then
                Return GetDirectoryLengthRoot(False)
            Else
                Return GetDirectoryLengthSubDirectory(False)
            End If
        End Function

        Public Function FileCount() As UInteger
            If _FatClusterList Is Nothing Then
                Return GetDirectoryLengthRoot(True)
            Else
                Return GetDirectoryLengthSubDirectory(True)
            End If
        End Function

        Public Function GetFile(Index As UInteger) As DiskImage.DirectoryEntry
            If _FatClusterList Is Nothing Then
                Return GetFileRoot(Index)
            Else
                Return GetFileSubDirectory(Index)
            End If
        End Function

        Public Function GetDataBlocks() As List(Of DataBlock)
            If _FatClusterList Is Nothing Then
                Return New List(Of DataBlock) From {
                    GetDataBlockRoot()
                }
            Else
                Return GetDataBlocksSubDirectory()
            End If
        End Function
    End Class
End Namespace
