Namespace DiskImage
    Public Class SubDirectory
        Implements IDirectory

        Private ReadOnly _BootSector As BootSector
        Private ReadOnly _FAT As FAT12
        Private ReadOnly _FatChain As FATChain
        Private ReadOnly _FileBytes As ImageByteArray
        Private ReadOnly _Length As UInteger
        Private _DirectoryData As DirectoryData

        Sub New(FileBytes As ImageByteArray, BootSector As BootSector, FAT As FAT12, FatChain As FATChain, Length As UInteger)
            _BootSector = BootSector
            _FAT = FAT
            _FileBytes = FileBytes
            _FatChain = FatChain
            _Length = Length
            _DirectoryData = GetDirectoryData()
        End Sub

        Public ReadOnly Property Data As DirectoryData Implements IDirectory.Data
            Get
                Return _DirectoryData
            End Get
        End Property

        Public ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain
            Get
                Return _FatChain.Sectors
            End Get
        End Property

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
            Dim Offset As UInteger = _BootSector.ClusterToOffset(_FatChain.Clusters.Item(ChainIndex)) + ClusterIndex * DirectoryEntry.DIRECTORY_ENTRY_SIZE

            Return New DirectoryEntry(_FileBytes, _BootSector, _FAT, Offset)
        End Function

        Public Function HasFile(Filename As String) As Boolean Implements IDirectory.HasFile
            Dim Count = _DirectoryData.EntryCount
            If Count > 0 Then
                For Counter As UInteger = 0 To Count - 1
                    Dim File = GetFile(Counter)
                    If Not File.IsDeleted And Not File.IsVolumeName And Not File.IsDirectory Then
                        If File.GetFullFileName = Filename Then
                            Return True
                        End If
                    End If
                Next
            End If

            Return False
        End Function

        Public Sub RefreshData() Implements IDirectory.RefreshData
            If _BootSector.IsValidImage Then
                _DirectoryData = GetDirectoryData()
            Else
                _DirectoryData = New DirectoryData
            End If
        End Sub

        Private Function GetDirectoryData() As DirectoryData
            Dim Data As New DirectoryData
            Dim EndOfDirectory As Boolean = False

            For Each Cluster In _FatChain.Clusters
                Dim OffsetStart As UInteger = _BootSector.ClusterToOffset(Cluster)
                Dim OffsetLength As UInteger = _BootSector.BytesPerCluster

                EndOfDirectory = Functions.GetDirectoryData(Data, _FileBytes, OffsetStart, OffsetStart + OffsetLength, EndOfDirectory, False)
            Next

            Return Data
        End Function
    End Class
End Namespace
