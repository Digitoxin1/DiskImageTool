Namespace DiskImage
    Public Class Directory
        Private ReadOnly _FatChain As List(Of UInteger)
        Private ReadOnly _Parent As Disk

        Sub New(Parent As Disk, Optional FatChain As List(Of UInteger) = Nothing)
            _Parent = Parent
            _FatChain = FatChain
        End Sub

        Private Function GetDataRoot() As DataBlock
            Dim Result As DataBlock

            Result.Offset = _Parent.BootSector.RootDirectoryRegionStart * _Parent.BootSector.BytesPerSector
            Dim OffsetEnd As UInteger = _Parent.BootSector.DataRegionStart * _Parent.BootSector.BytesPerSector

            Result.Data = _Parent.GetBytes(Result.Offset, OffsetEnd - Result.Offset)

            Return Result
        End Function

        Private Function GetDataSubDirectory() As List(Of DataBlock)
            Dim Result As New List(Of DataBlock)

            For Each Cluster In _FatChain
                Dim Block As DataBlock

                Block.Offset = _Parent.BootSector.DataRegionStart + ((Cluster - 2) * _Parent.BootSector.SectorsPerCluster)
                Block.Offset *= _Parent.BootSector.BytesPerSector

                Dim Length As UInteger = _Parent.BootSector.SectorsPerCluster * _Parent.BootSector.BytesPerSector

                Block.Data = _Parent.GetBytes(Block.Offset, Length)
                Result.Add(Block)
            Next

            Return Result
        End Function

        Private Function GetDirectoryLengthSubDirectory(FileCountOnly As Boolean) As Integer
            Dim Count As UInteger = 0

            For Each Cluster In _FatChain
                Dim OffsetStart As UInteger = _Parent.BootSector.DataRegionStart + ((Cluster - 2) * _Parent.BootSector.SectorsPerCluster)
                Dim OffsetEnd As UInteger = OffsetStart + _Parent.BootSector.SectorsPerCluster

                OffsetStart *= _Parent.BootSector.BytesPerSector
                OffsetEnd *= _Parent.BootSector.BytesPerSector

                Count += _Parent.GetDirectoryLength(OffsetStart, OffsetEnd, FileCountOnly)
            Next

            Return Count
        End Function

        Private Function GetDirectoryLengthRoot(FileCountOnly As Boolean) As UInteger
            Dim OffsetStart As UInteger = _Parent.BootSector.RootDirectoryRegionStart * _Parent.BootSector.BytesPerSector
            Dim OffsetEnd As UInteger = _Parent.BootSector.DataRegionStart * _Parent.BootSector.BytesPerSector

            Return _Parent.GetDirectoryLength(OffsetStart, OffsetEnd, FileCountOnly)
        End Function

        Private Function GetFileSubDirectory(Index As UInteger) As DiskImage.DirectoryEntry
            Dim EntriesPerCluster As UInteger = _Parent.BootSector.BytesPerSector * _Parent.BootSector.SectorsPerCluster / 32
            Dim ChainIndex As UInteger = (Index - 1) \ EntriesPerCluster
            Dim ClusterIndex As UInteger = (Index - 1) Mod EntriesPerCluster
            Dim Offset As UInteger = _Parent.BootSector.DataRegionStart + ((_FatChain.Item(ChainIndex) - 2) * _Parent.BootSector.SectorsPerCluster)
            Offset *= _Parent.BootSector.BytesPerSector
            Offset += ClusterIndex * 32

            Return New DiskImage.DirectoryEntry(_Parent, Offset)
        End Function

        Private Function GetFileRoot(Index As Integer) As DiskImage.DirectoryEntry
            Dim Offset As UInteger = _Parent.BootSector.RootDirectoryRegionStart * _Parent.BootSector.BytesPerSector
            Offset += (Index - 1) * 32

            Return New DiskImage.DirectoryEntry(_Parent, Offset)
        End Function

        Public Function DirectoryLength() As UInteger
            If _FatChain Is Nothing Then
                Return GetDirectoryLengthRoot(False)
            Else
                Return GetDirectoryLengthSubDirectory(False)
            End If
        End Function

        Public Function FileCount() As UInteger
            If _FatChain Is Nothing Then
                Return GetDirectoryLengthRoot(True)
            Else
                Return GetDirectoryLengthSubDirectory(True)
            End If
        End Function

        Public Function GetFile(Index As UInteger) As DiskImage.DirectoryEntry
            If _FatChain Is Nothing Then
                Return GetFileRoot(Index)
            Else
                Return GetFileSubDirectory(Index)
            End If
        End Function

        Public Function GetData() As List(Of DataBlock)
            If _FatChain Is Nothing Then
                Return New List(Of DataBlock) From {
                    GetDataRoot()
                }
            Else
                Return GetDataSubDirectory()
            End If
        End Function
    End Class
End Namespace
