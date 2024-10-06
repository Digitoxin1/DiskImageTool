Namespace DiskImage
    Public Class SubDirectory
        Inherits DirectoryBase
        Implements IDirectory

        Private ReadOnly _RootDirectory As RootDirectory

        Sub New(RootDirectory As RootDirectory, ParentEntry As DirectoryEntry)
            MyBase.New(RootDirectory.Disk, ParentEntry)

            _RootDirectory = RootDirectory

            InitializeDirectoryData()
        End Sub

        Public Overrides ReadOnly Property ClusterChain As List(Of UShort) Implements IDirectory.ClusterChain
            Get
                Return ParentEntry.FATChain.Clusters
            End Get
        End Property

        Public Overrides ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain
            Get
                Return ClusterListToSectorList(Disk.BPB, ParentEntry.FATChain.Clusters)
            End Get
        End Property

        Public Overrides Function GetContent() As Byte() Implements IDirectory.GetContent
            Return GetDataFromChain(Disk.Image.Data, SectorChain)
        End Function

        Private Sub InitializeDirectoryData()
            DirectoryEntries.Clear()

            If Data.Level <= 32 Then
                For Each Cluster In ParentEntry.FATChain.Clusters
                    Dim OffsetStart As UInteger = Disk.BPB.ClusterToOffset(Cluster)
                    Dim OffsetLength As UInteger = Disk.BPB.BytesPerCluster

                    DirectoryBase.GetDirectoryData(_RootDirectory, Me, OffsetStart, OffsetStart + OffsetLength, False)
                Next
            End If
        End Sub
    End Class
End Namespace
