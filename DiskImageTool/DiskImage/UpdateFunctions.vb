Imports System.Runtime.ConstrainedExecution

Namespace DiskImage
    Module UpdateFunctions
        Public Sub BootSectorRemoveFromDirectory(Disk As Disk)
            If Not Disk.Directory.Data.HasBootSector Then
                Exit Sub
            End If

            Dim BootSectorBytes(BootSector.BOOT_SECTOR_SIZE - 1) As Byte

            For Counter = 0 To BootSectorBytes.Length - 1
                BootSectorBytes(Counter) = 0
            Next

            Disk.Image.SetBytes(BootSectorBytes, Disk.Directory.Data.BootSectorOffset)
        End Sub

        Public Sub BootSectorRestoreFromDirectory(Disk As Disk)
            If Not Disk.Directory.Data.HasBootSector Then
                Exit Sub
            End If

            Dim BootSectorBytes = Disk.Image.GetBytes(Disk.Directory.Data.BootSectorOffset, BootSector.BOOT_SECTOR_SIZE)
            Disk.Image.SetBytes(BootSectorBytes, 0)
        End Sub

        Public Function ClearReservedBytes(Disk As Disk) As Boolean
            Dim Result As Boolean = False

            Disk.Image.BatchEditMode = True

            Dim FileList = Disk.GetFileList()

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

            Disk.Image.BatchEditMode = False

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

            Return DirectoryEntry.CanRestore
        End Function

        Public Function DirectoryEntryDelete(DirectoryEntry As DirectoryEntry, FillChar As Byte, Clear As Boolean) As Boolean
            Dim Result As Boolean = False

            If DirectoryEntryCanDelete(DirectoryEntry, Clear) Then
                DirectoryEntry.Remove(Clear, FillChar)
                Result = True
            ElseIf Clear And DirectoryEntryCanDelete(DirectoryEntry, False) Then
                DirectoryEntry.Remove(False, FillChar)
                Result = True
            End If

            Return Result
        End Function

        Public Function FixImageSize(Disk As Disk) As Boolean
            Dim Result As Boolean = False

            Dim ReportedSize = Disk.BPB.ReportedImageSize()

            If ReportedSize <> Disk.Image.Length Then
                Disk.Image.BatchEditMode = True
                If ReportedSize < Disk.Image.Length Then
                    Dim b(Disk.Image.Length - ReportedSize - 1) As Byte
                    For i = 0 To b.Length - 1
                        b(i) = 0
                    Next
                    Disk.Image.SetBytes(b, Disk.Image.Length - b.Length)
                End If
                Disk.Image.Resize(ReportedSize)
                Disk.Image.BatchEditMode = False
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
            Disk.Image.BatchEditMode = True
            Disk.Image.SetBytes(Data, 0, Disk.Image.Length, 0)
            Disk.Image.Resize(Data.Length)
            Disk.Image.BatchEditMode = False
        End Sub

    End Module
End Namespace
