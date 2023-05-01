Namespace DiskImage
    Public Class RootDirectory
        Implements IDirectory

        Private ReadOnly _BootSector As BootSector
        Private ReadOnly _FAT As FAT12
        Private ReadOnly _FileBytes As ImageByteArray
        Private _DirectoryData As DirectoryData

        Sub New(FileBytes As ImageByteArray, BootSector As BootSector, FAT As FAT12, EnumerateEntries As Boolean)
            _BootSector = BootSector
            _FAT = FAT
            _FileBytes = FileBytes
            If BootSector.IsValidImage Then
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

                For Sector = _BootSector.RootDirectoryRegionStart To _BootSector.DataRegionStart - 1
                    Chain.Add(Sector)
                Next

                Return Chain
            End Get
        End Property

        Public Function GetContent() As Byte() Implements IDirectory.GetContent
            Dim SectorStart = _BootSector.RootDirectoryRegionStart
            Dim SectorEnd = _BootSector.DataRegionStart
            Dim Length = Disk.SectorToBytes(SectorEnd - SectorStart)
            Dim Offset = Disk.SectorToBytes(SectorStart)

            Return _FileBytes.GetBytes(Offset, Length)
        End Function

        Public Function GetFile(Index As UInteger) As DirectoryEntry Implements IDirectory.GetFile
            Dim Offset As UInteger = Disk.SectorToBytes(_BootSector.RootDirectoryRegionStart) + Index * DirectoryEntry.DIRECTORY_ENTRY_SIZE

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
            Dim OffsetStart As UInteger = Disk.SectorToBytes(_BootSector.RootDirectoryRegionStart)
            Dim OffsetEnd As UInteger = Disk.SectorToBytes(_BootSector.DataRegionStart)
            Dim Data As New DirectoryData

            Functions.GetDirectoryData(Data, _FileBytes, OffsetStart, OffsetEnd, False, True)

            Return Data
        End Function

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
    End Class
End Namespace
