Namespace DiskImage
    Public Class Directory
        Private ReadOnly _FatChain As List(Of UInteger)
        Private ReadOnly _Parent As Disk

        Sub New(Parent As Disk, Optional FatChain As List(Of UInteger) = Nothing)
            _Parent = Parent
            _FatChain = FatChain
        End Sub

        Private Function GetDataRoot() As DataBlock

            Dim SectorStart = _Parent.BootSector.RootDirectoryRegionStart
            Dim SectorEnd = _Parent.BootSector.DataRegionStart
            Dim Length = (SectorEnd - SectorStart) * _Parent.BootSector.BytesPerSector

            Dim Block As DataBlock
            With Block
                .Offset = _Parent.SectorToOffset(SectorStart)
                .Length = Length
            End With

            Return Block
        End Function

        Private Function GetDataSubDirectory() As List(Of DataBlock)
            Dim Result As New List(Of DataBlock)
            Dim Block As DataBlock
            Dim CurrentOffset As UInteger = 0
            Dim Length As UInteger = _Parent.BootSector.BytesPerCluster
            Dim TotalLength As UInteger = 0

            Dim LastCluster As UInteger = 0
            For Each Cluster In _FatChain
                If LastCluster = 0 Or Cluster <> LastCluster + 1 Then
                    If LastCluster > 0 Then
                        With Block
                            .Offset = CurrentOffset
                            .Length = TotalLength
                        End With
                        Result.Add(Block)
                        TotalLength = 0
                    End If
                    CurrentOffset = _Parent.ClusterToOffset(Cluster)
                End If
                TotalLength += Length
                LastCluster = Cluster
            Next

            If Length > 0 Then
                With Block
                    .Offset = CurrentOffset
                    .Length = TotalLength
                End With
                Result.Add(Block)
            End If

            Return Result
        End Function

        Private Function GetDirectoryLengthSubDirectory(FileCountOnly As Boolean) As Integer
            Dim Count As UInteger = 0

            For Each Cluster In _FatChain
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
            Dim Offset As UInteger = _Parent.ClusterToOffset(_FatChain.Item(ChainIndex)) + ClusterIndex * 32

            Return New DiskImage.DirectoryEntry(_Parent, Offset)
        End Function

        Private Function GetFileRoot(Index As Integer) As DiskImage.DirectoryEntry
            Dim Offset As UInteger = _Parent.SectorToOffset(_Parent.BootSector.RootDirectoryRegionStart) + (Index - 1) * 32

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
                Dim DataBlock = New List(Of DataBlock) From {
                    GetDataRoot()
                }
                Return DataBlock
            Else
                Return GetDataSubDirectory()
            End If
        End Function
    End Class
End Namespace
