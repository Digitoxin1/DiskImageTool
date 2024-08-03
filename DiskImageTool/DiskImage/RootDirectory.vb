Namespace DiskImage
    Public Class RootDirectory
        Implements IDirectory

        Private _BPB As BiosParameterBlock
        Private ReadOnly _DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry)
        Private ReadOnly _FATTables As FATTables
        Private ReadOnly _FileBytes As ImageByteArray
        Private _DirectoryData As DirectoryData

        Sub New(FileBytes As ImageByteArray, BPB As BiosParameterBlock, FATTables As FATTables, DirectoryCache As Dictionary(Of UInteger, DirectoryCacheEntry), EnumerateEntries As Boolean)
            _BPB = BPB
            _FATTables = FATTables
            _FileBytes = FileBytes
            _DirectoryCache = DirectoryCache
            If BPB.IsValid Then
                _DirectoryData = GetDirectoryData()
                If EnumerateEntries Then
                    EnumDirectoryEntries(Me)
                End If
            Else
                _DirectoryData = New DirectoryData
            End If
        End Sub

        Public ReadOnly Property Data As DirectoryData Implements IDirectory.Data
            Get
                Return _DirectoryData
            End Get
        End Property

        Public ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain
            Get
                Dim Chain = New List(Of UInteger)

                For Sector = _BPB.RootDirectoryRegionStart To _BPB.DataRegionStart - 1
                    Chain.Add(Sector)
                Next

                Return Chain
            End Get
        End Property

        Public Function GetContent() As Byte() Implements IDirectory.GetContent
            Dim SectorStart = _BPB.RootDirectoryRegionStart
            Dim SectorEnd = _BPB.DataRegionStart
            Dim Length = Disk.SectorToBytes(SectorEnd - SectorStart)
            Dim Offset = Disk.SectorToBytes(SectorStart)

            Return _FileBytes.GetBytes(Offset, Length)
        End Function

        Public Function GetFile(Index As UInteger) As DirectoryEntry Implements IDirectory.GetFile
            Dim Offset As UInteger = Disk.SectorToBytes(_BPB.RootDirectoryRegionStart) + Index * DirectoryEntry.DIRECTORY_ENTRY_SIZE

            Return New DirectoryEntry(_FileBytes, _BPB, _FATTables, _DirectoryCache, Offset)
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
            If _BPB.IsValid Then
                _DirectoryData = GetDirectoryData()
            Else
                _DirectoryData = New DirectoryData
            End If
        End Sub

        Public Sub RefreshData(BPB As BiosParameterBlock)
            _BPB = BPB
            RefreshData()
        End Sub

        Private Shared Sub EnumDirectoryEntries(Directory As DiskImage.IDirectory)
            Dim DirectoryEntryCount = Directory.Data.EntryCount

            If DirectoryEntryCount > 0 Then
                For Counter = 0 To DirectoryEntryCount - 1
                    Dim File = Directory.GetFile(Counter)

                    If Not File.IsLink And Not File.IsVolumeName Then
                        If File.IsDirectory And File.SubDirectory IsNot Nothing Then
                            If File.SubDirectory.Data.EntryCount > 0 Then
                                EnumDirectoryEntries(File.SubDirectory)
                            End If
                        End If
                    End If
                Next
            End If
        End Sub

        Private Function GetDirectoryData() As DirectoryData
            Dim OffsetStart As UInteger = Disk.SectorToBytes(_BPB.RootDirectoryRegionStart)
            Dim OffsetEnd As UInteger = Disk.SectorToBytes(_BPB.DataRegionStart)
            Dim Data As New DirectoryData

            Functions.GetDirectoryData(Data, _FileBytes, OffsetStart, OffsetEnd, False, True)

            Return Data
        End Function
    End Class
End Namespace
