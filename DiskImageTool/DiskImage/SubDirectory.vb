Namespace DiskImage
    Public Class SubDirectory
        Implements IDirectory

        Private ReadOnly _Disk As Disk
        Private ReadOnly _FatTables As FATTables
        Private ReadOnly _FatChain As FATChain
        Private _DirectoryData As DirectoryData

        Sub New(Disk As Disk, FatTables As FATTables, FatChain As FATChain)
            _Disk = Disk
            _FatTables = FatTables
            _FatChain = FatChain
            _DirectoryData = GetDirectoryData()
        End Sub

        Public ReadOnly Property Data As DirectoryData Implements IDirectory.Data
            Get
                Return _DirectoryData
            End Get
        End Property

        Public ReadOnly Property Disk As Disk
            Get
                Return _Disk
            End Get
        End Property

        Public ReadOnly Property ClusterChain As List(Of UShort) Implements IDirectory.ClusterChain
            Get
                Return _FatChain.Clusters
            End Get
        End Property

        Public ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain
            Get
                Return ClusterListToSectorList(_Disk.BPB, _FatChain.Clusters)
            End Get
        End Property

        Public ReadOnly Property IsRootDirectory As Boolean Implements IDirectory.IsRootDirectory
            Get
                Return False
            End Get
        End Property

        Public Function GetContent() As Byte() Implements IDirectory.GetContent
            Return GetDataFromChain(_Disk.Image.Data, SectorChain)
        End Function

        Public Function GetFile(Index As UInteger) As DirectoryEntry Implements IDirectory.GetFile
            Dim EntriesPerCluster As UInteger = _Disk.BPB.BytesPerCluster / DirectoryEntry.DIRECTORY_ENTRY_SIZE
            Dim ChainIndex As UInteger = Index \ EntriesPerCluster
            Dim ClusterIndex As UInteger = Index Mod EntriesPerCluster
            Dim Offset As UInteger = _Disk.BPB.ClusterToOffset(_FatChain.Clusters.Item(ChainIndex)) + ClusterIndex * DirectoryEntry.DIRECTORY_ENTRY_SIZE

            Return New DirectoryEntry(_Disk, _FatTables, Offset)
        End Function

        Public Function GetNextAvailableEntry() As DirectoryEntry Implements IDirectory.GetNextAvailableEntry
            Dim Buffer(10) As Byte

            For Counter As UInteger = 0 To _DirectoryData.MaxEntries - 1
                Dim DirectoryEntry = GetFile(Counter)
                If DirectoryEntry.Data(0) = 0 Then
                    Return DirectoryEntry
                End If
                Array.Copy(DirectoryEntry.Data, 0, Buffer, 0, Buffer.Length)
                If Buffer.CompareTo(DirectoryEntry.EmptyDirectoryEntry) Then
                    Return DirectoryEntry
                End If
            Next

            Return Nothing
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
            If _Disk.BPB.IsValid Then
                _DirectoryData = GetDirectoryData()
            Else
                _DirectoryData = New DirectoryData
            End If
        End Sub

        Private Function GetDirectoryData() As DirectoryData
            Dim Data As New DirectoryData

            For Each Cluster In _FatChain.Clusters
                Dim OffsetStart As UInteger = Disk.BPB.ClusterToOffset(Cluster)
                Dim OffsetLength As UInteger = Disk.BPB.BytesPerCluster

                Functions.GetDirectoryData(Data, _Disk.Image.Data, OffsetStart, OffsetStart + OffsetLength, False)
            Next

            Return Data
        End Function
    End Class
End Namespace
