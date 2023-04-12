Namespace DiskImage
    Public Class SubDirectory
        Implements IDirectory

        Private ReadOnly _BootSector As BootSector
        Private ReadOnly _FAT As FAT12
        Private ReadOnly _FatChain As FATChain
        Private ReadOnly _FileBytes As ImageByteArray
        Private ReadOnly _Length As UInteger

        Sub New(FileBytes As ImageByteArray, BootSector As BootSector, FAT As FAT12, FatChain As FATChain, Length As UInteger)
            _BootSector = BootSector
            _FAT = FAT
            _FileBytes = FileBytes
            _FatChain = FatChain
            _Length = Length
        End Sub

        Public ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain
            Get
                Return _FatChain.Sectors
            End Get
        End Property

        Public Function DirectoryEntryCount() As UInteger Implements IDirectory.DirectoryEntryCount
            Return GetDirectoryEntryCount(False, False)
        End Function

        Public Function FileCount(ExcludeDeleted As Boolean) As UInteger Implements IDirectory.FileCount
            Return GetDirectoryEntryCount(True, ExcludeDeleted)
        End Function

        Public Function GetContent() As Byte() Implements IDirectory.GetContent
            Dim Content = GetDataFromChain(_FileBytes, _FatChain.Sectors)

            If Content.Length <> _Length Then
                Array.Resize(Of Byte)(Content, _Length)
            End If

            Return Content
        End Function

        Public Function GetFile(Index As UInteger) As DirectoryEntry Implements IDirectory.GetFile
            Dim EntriesPerCluster As UInteger = _BootSector.BytesPerCluster / 32
            Dim ChainIndex As UInteger = Index \ EntriesPerCluster
            Dim ClusterIndex As UInteger = Index Mod EntriesPerCluster
            Dim Offset As UInteger = _BootSector.ClusterToOffset(_FatChain.Clusters.Item(ChainIndex)) + ClusterIndex * 32

            Return New DirectoryEntry(_FileBytes, _BootSector, _FAT, Offset)
        End Function

        Public Function HasFile(Filename As String) As Boolean Implements IDirectory.HasFile
            Dim Count = GetDirectoryEntryCount(False, False)
            For Counter As UInteger = 0 To Count - 1
                Dim File = GetFile(Counter)
                If Not File.IsDeleted And Not File.IsVolumeName And Not File.IsDirectory Then
                    If File.GetFullFileName = Filename Then
                        Return True
                    End If
                End If
            Next

            Return False
        End Function

        Private Function GetDirectoryEntryCount(FileCountOnly As Boolean, ExcludeDeleted As Boolean) As UInteger
            Dim Count As UInteger = 0

            For Each Cluster In _FatChain.Clusters
                Dim OffsetStart As UInteger = _BootSector.ClusterToOffset(Cluster)
                Dim OffsetLength As UInteger = _BootSector.BytesPerCluster

                Count += Functions.GetDirectoryEntryCount(_FileBytes, OffsetStart, OffsetStart + OffsetLength, FileCountOnly, ExcludeDeleted)
            Next

            Return Count
        End Function
    End Class
End Namespace
