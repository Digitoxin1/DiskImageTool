Namespace DiskImage
    Module UpdateFunctions
        Public Sub BootSectorRemoveFromDirectory(Disk As Disk)
            If Not Disk.RootDirectory.Data.HasBootSector Then
                Exit Sub
            End If

            Dim BootSectorBytes(BootSector.BOOT_SECTOR_SIZE - 1) As Byte

            For Counter = 0 To BootSectorBytes.Length - 1
                BootSectorBytes(Counter) = 0
            Next

            Disk.Image.SetBytes(BootSectorBytes, Disk.RootDirectory.Data.BootSectorOffset)
        End Sub

        Public Sub BootSectorRestoreFromDirectory(Disk As Disk)
            If Not Disk.RootDirectory.Data.HasBootSector Then
                Exit Sub
            End If

            Dim BootSectorBytes = Disk.Image.GetBytes(Disk.RootDirectory.Data.BootSectorOffset, BootSector.BOOT_SECTOR_SIZE)
            Disk.Image.SetBytes(BootSectorBytes, 0)
        End Sub

        Public Function ClearReservedBytes(Disk As Disk) As Boolean
            Dim Result As Boolean = False

            Dim UseTransaction = Disk.BeginTransaction

            Dim FileList = Disk.RootDirectory.GetFileList()

            For Each DirectoryEntry In FileList
                If DirectoryEntry.IsValid Then
                    If DirectoryEntry.ReservedForWinNT <> 0 Then
                        DirectoryEntry.ReservedForWinNT = 0
                        Result = True
                    End If
                    If DirectoryEntry.ReservedForFAT32 <> 0 Then
                        DirectoryEntry.ReservedForFAT32 = 0
                        Result = True
                    End If
                End If
            Next

            If UseTransaction Then
                Disk.EndTransaction()
            End If

            Return Result
        End Function

        Public Function DirectoryEntryCanDelete(DirectoryEntry As DiskImage.DirectoryEntry, Clear As Boolean) As Boolean
            If DirectoryEntry.IsDeleted Then
                Return False
            ElseIf Clear And Not DirectoryEntry.IsValidFile Then
                Return False
            ElseIf DirectoryEntry.IsValidDirectory Then
                Return DirectoryEntry.SubDirectory.Data.FileCount - DirectoryEntry.SubDirectory.Data.DeletedFileCount = 0
            Else
                Return True
            End If
        End Function

        Public Function DirectoryEntryCanExport(DirectoryEntry As DiskImage.DirectoryEntry) As Boolean
            Return (DirectoryEntry.IsValidFile Or DirectoryEntry.IsValidVolumeName) _
            And Not DirectoryEntry.IsDeleted _
            And Not DirectoryEntry.HasInvalidFilename _
            And Not DirectoryEntry.HasInvalidExtension
        End Function

        Public Function DirectoryEntryCanUndelete(DirectoryEntry As DiskImage.DirectoryEntry) As Boolean
            If Not DirectoryEntry.IsDeleted Then
                Return False
            End If

            Return DirectoryEntry.CanUnDelete
        End Function

        Public Function DirectoryEntryDelete(DirectoryEntry As DirectoryEntry, FillChar As Byte, Clear As Boolean) As Boolean
            Dim Result As Boolean = False

            If DirectoryEntryCanDelete(DirectoryEntry, Clear) Then
                DirectoryEntry.Delete(Clear, FillChar)
                Result = True
            ElseIf Clear And DirectoryEntryCanDelete(DirectoryEntry, False) Then
                DirectoryEntry.Delete(False, FillChar)
                Result = True
            End If

            Return Result
        End Function

        Public Function FixImageSize(Disk As Disk) As Boolean
            Dim Result As Boolean = False

            Dim ReportedSize = Disk.BPB.ReportedImageSize()

            If ReportedSize <> Disk.Image.Length Then
                Dim UseTransaction = Disk.BeginTransaction

                If ReportedSize < Disk.Image.Length Then
                    Dim b(Disk.Image.Length - ReportedSize - 1) As Byte
                    For i = 0 To b.Length - 1
                        b(i) = 0
                    Next
                    Disk.Image.SetBytes(b, Disk.Image.Length - b.Length)
                End If
                Disk.Image.Resize(ReportedSize)

                If UseTransaction Then
                    Disk.EndTransaction()
                End If

                Result = True
            End If

            Return Result
        End Function

        Public Sub RestructureImage(Disk As Disk)

            Dim Size = GetFloppyDiskSize(Disk.DiskFormat)
            Dim Data(Size - 1) As Byte
            Dim Params = GetFloppyDiskParams(Disk.DiskFormat)
            Dim ParamsBySize = GetFloppyDiskParams(Disk.Image.Length)

            Dim DataOffset As UInteger = 0
            For Offset As UInteger = 0 To Disk.Image.Length - 1 Step Disk.BYTES_PER_SECTOR
                Dim Sector As UInteger = Offset \ Disk.BYTES_PER_SECTOR
                Dim TrackSector As UShort = Sector Mod ParamsBySize.SectorsPerTrack
                Dim Track As UShort = Sector \ ParamsBySize.SectorsPerTrack
                Dim Side As UShort = Track Mod ParamsBySize.NumberOfHeads
                If TrackSector < Params.SectorsPerTrack And Side < Params.NumberOfHeads Then
                    Disk.Image.CopyTo(Offset, Data, DataOffset, Disk.BYTES_PER_SECTOR)
                    DataOffset += Disk.BYTES_PER_SECTOR
                End If
            Next

            DiskUpdateBytes(Disk, Data)
        End Sub

        Private Sub DiskUpdateBytes(Disk As Disk, Data() As Byte)
            Dim UseTransaction = Disk.BeginTransaction

            Disk.Image.SetBytes(Data, 0, Disk.Image.Length, 0)
            Disk.Image.Resize(Data.Length)

            If UseTransaction Then
                Disk.EndTransaction()
            End If
        End Sub

    End Module
End Namespace
