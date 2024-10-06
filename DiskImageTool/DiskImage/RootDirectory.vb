Namespace DiskImage
    Public Class RootDirectory
        Implements IDirectory

        Private ReadOnly _Disk As Disk
        Private ReadOnly _FATTables As FATTables
        Private _DirectoryData As DirectoryData

        Sub New(Disk As Disk, FATTables As FATTables, EnumerateEntries As Boolean)
            _Disk = Disk
            _FATTables = FATTables
            If _Disk.BPB.IsValid Then
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

        Public ReadOnly Property Disk As Disk
            Get
                Return _Disk
            End Get
        End Property

        Public ReadOnly Property ClusterChain As List(Of UShort) Implements IDirectory.ClusterChain
            Get
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain
            Get
                Dim Chain = New List(Of UInteger)

                For Sector = _Disk.BPB.RootDirectoryRegionStart To _Disk.BPB.DataRegionStart - 1
                    Chain.Add(Sector)
                Next

                Return Chain
            End Get
        End Property

        Public ReadOnly Property IsRootDirectory As Boolean Implements IDirectory.IsRootDirectory
            Get
                Return True
            End Get
        End Property

        Public Function GetContent() As Byte() Implements IDirectory.GetContent
            Dim SectorStart = _Disk.BPB.RootDirectoryRegionStart
            Dim SectorEnd = _Disk.BPB.DataRegionStart
            Dim Length = Disk.SectorToBytes(SectorEnd - SectorStart)
            Dim Offset = Disk.SectorToBytes(SectorStart)

            Return _Disk.Image.GetBytes(Offset, Length)
        End Function

        Public Function GetFile(Index As UInteger) As DirectoryEntry Implements IDirectory.GetFile
            Return New DirectoryEntry(_Disk, _FATTables, GetOffset(Index))
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
            Dim OffsetStart As UInteger = Disk.SectorToBytes(_Disk.BPB.RootDirectoryRegionStart)
            Dim OffsetEnd As UInteger = Disk.SectorToBytes(_Disk.BPB.DataRegionStart)
            Dim Data As New DirectoryData

            Functions.GetDirectoryData(Data, _Disk.Image.Data, OffsetStart, OffsetEnd, True)

            Return Data
        End Function

        Private Function GetOffset(Index As UInteger) As UInteger
            Return Disk.SectorToBytes(_Disk.BPB.RootDirectoryRegionStart) + Index * DirectoryEntry.DIRECTORY_ENTRY_SIZE
        End Function
    End Class
End Namespace
