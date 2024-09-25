Namespace DiskImage
    Public Class SubDirectory
        Implements IDirectory

        Private ReadOnly _BPB As BiosParameterBlock
        Private ReadOnly _DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry)
        Private ReadOnly _FatTables As FATTables
        Private ReadOnly _FatChain As FATChain
        Private ReadOnly _FileBytes As ImageByteArray
        Private _DirectoryData As DirectoryData

        Sub New(FileBytes As ImageByteArray, BPB As BiosParameterBlock, FatTables As FATTables, FatChain As FATChain, DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry))
            _BPB = BPB
            _FatTables = FatTables
            _FileBytes = FileBytes
            _FatChain = FatChain
            _DirectoryCache = DirectoryCache
            _DirectoryData = GetDirectoryData()
        End Sub

        Public ReadOnly Property Data As DirectoryData Implements IDirectory.Data
            Get
                Return _DirectoryData
            End Get
        End Property

        Public ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain
            Get
                Return ClusterListToSectorList(_BPB, _FatChain.Clusters)
            End Get
        End Property

        Public Function GetContent() As Byte() Implements IDirectory.GetContent
            Return GetDataFromChain(_FileBytes.Data, SectorChain)
        End Function

        Public Function GetFile(Index As UInteger) As DirectoryEntry Implements IDirectory.GetFile
            Dim EntriesPerCluster As UInteger = _BPB.BytesPerCluster / DirectoryEntry.DIRECTORY_ENTRY_SIZE
            Dim ChainIndex As UInteger = Index \ EntriesPerCluster
            Dim ClusterIndex As UInteger = Index Mod EntriesPerCluster
            Dim Offset As UInteger = _BPB.ClusterToOffset(_FatChain.Clusters.Item(ChainIndex)) + ClusterIndex * DirectoryEntry.DIRECTORY_ENTRY_SIZE

            Return New DirectoryEntry(_FileBytes, _BPB, _FatTables, _DirectoryCache, Offset)
        End Function

        Public Function HasFile(Filename As String) As Integer Implements IDirectory.HasFile
            Dim Count = _DirectoryData.EntryCount
            If Count > 0 Then
                For Counter As UInteger = 0 To Count - 1
                    Dim File = GetFile(Counter)
                    If Not File.IsDeleted And Not File.IsVolumeName And Not File.IsDirectory Then
                        If File.GetFullFileName = Filename Then
                            Return Counter
                        End If
                    End If
                Next
            End If

            Return -1
        End Function

        Public Sub RefreshData() Implements IDirectory.RefreshData
            If _BPB.isvalid Then
                _DirectoryData = GetDirectoryData()
            Else
                _DirectoryData = New DirectoryData
            End If
        End Sub

        Private Function GetDirectoryData() As DirectoryData
            Dim Data As New DirectoryData

            For Each Cluster In _FatChain.Clusters
                Dim OffsetStart As UInteger = _BPB.ClusterToOffset(Cluster)
                Dim OffsetLength As UInteger = _BPB.BytesPerCluster

                Functions.GetDirectoryData(Data, _FileBytes.Data, OffsetStart, OffsetStart + OffsetLength, False)
            Next

            Return Data
        End Function
    End Class
End Namespace
