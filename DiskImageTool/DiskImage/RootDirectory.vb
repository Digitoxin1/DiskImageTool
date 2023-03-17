Namespace DiskImage
    Public Class RootDirectory
        Implements IDirectory

        Private ReadOnly _BootSector As BootSector
        Private ReadOnly _FAT As FAT12
        Private ReadOnly _FileBytes As ImageByteArray

        Sub New(FileBytes As ImageByteArray, BootSector As BootSector, FAT As FAT12)
            _BootSector = BootSector
            _FAT = FAT
            _FileBytes = FileBytes
        End Sub

        Public ReadOnly Property SectorChain As List(Of UInteger) Implements IDirectory.SectorChain
            Get
                Dim Chain = New List(Of UInteger)

                For Sector = _BootSector.RootDirectoryRegionStart To _BootSector.DataRegionStart - 1
                    Chain.Add(Sector)
                Next

                Return Chain
            End Get
        End Property

        Public Function DirectoryEntryCount() As UInteger Implements IDirectory.DirectoryEntryCount
            Return GetDirectoryEntryCount(False)
        End Function

        Public Function FileCount() As UInteger Implements IDirectory.FileCount
            Return GetDirectoryEntryCount(True)
        End Function

        Public Function GetContent() As Byte() Implements IDirectory.GetContent
            Dim SectorStart = _BootSector.RootDirectoryRegionStart
            Dim SectorEnd = _BootSector.DataRegionStart
            Dim Length = SectorToBytes(SectorEnd - SectorStart)
            Dim Offset = SectorToBytes(SectorStart)

            Return _FileBytes.GetBytes(Offset, Length)
        End Function

        Public Function GetFile(Index As UInteger) As DirectoryEntry Implements IDirectory.GetFile
            Dim Offset As UInteger = SectorToBytes(_BootSector.RootDirectoryRegionStart) + Index * 32

            Return New DirectoryEntry(_FileBytes, _BootSector, _FAT, Offset)
        End Function

        Public Function HasFile(Filename As String) As Boolean Implements IDirectory.HasFile
            Dim Count = GetDirectoryEntryCount(False)
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

        Private Function GetDirectoryEntryCount(FileCountOnly As Boolean) As UInteger
            Dim OffsetStart As UInteger = SectorToBytes(_BootSector.RootDirectoryRegionStart)
            Dim OffsetEnd As UInteger = SectorToBytes(_BootSector.DataRegionStart)

            Return Functions.GetDirectoryEntryCount(_FileBytes, OffsetStart, OffsetEnd, FileCountOnly)
        End Function
    End Class
End Namespace
