Namespace DiskImage

    Public Class BootSector
        Private Const BOOT_SECTOR As UInteger = 0
        Private ReadOnly _FileBytes As ImageByteArray
        Private Shared ReadOnly _ValidBootStrapSignature As UShort = &HAA55
        Private Shared ReadOnly _ValidBytesPerSector() As UShort = {512, 1024, 2048, 4096}
        Private Shared ReadOnly _ValidDriveNumber() As Byte = {&H0, &H80}
        Private Shared ReadOnly _ValidExtendedBootSignature() As Byte = {&H29}
        Private Shared ReadOnly _ValidJumpInstructuon() As Byte = {&HEB, &HE9}
        Private Shared ReadOnly _ValidMediaDescriptor() As Byte = {&HF0, &HF8, &HF9, &HFA, &HFB, &HFC, &HFD, &HFE, &HFF}
        Private Shared ReadOnly _ValidSectorsPerSector() As Byte = {1, 2, 4, 8, 16, 32, 64, 128}

        Public Enum BootSectorOffsets As UInteger
            JmpBoot = 0
            OEMName = 3
            BytesPerSector = 11
            SectorsPerCluster = 13
            ReservedSectorCount = 14
            NumberOfFATs = 16
            RootEntryCount = 17
            SectorCountSmall = 19
            MediaDescriptor = 21
            SectorsPerFAT = 22
            SectorsPerTrack = 24
            NumberOfHeads = 26
            HiddenSectors = 28
            SectorCountLarge = 32
            DriveNumber = 36
            Reserved = 37
            ExtendedBootSignature = 38
            VolumeSerialNumber = 39
            VolumeLabel = 43
            FileSystemType = 54
            BootStrapCode = 62
            BootStrapSignature = 510
        End Enum

        Public Enum BootSectorSizes As UInteger
            JmpBoot = 3
            OEMName = 8
            BytesPerSector = 2
            SectorsPerCluster = 1
            ReservedSectorCount = 2
            NumberOfFATs = 1
            RootEntryCount = 2
            SectorCountSmall = 2
            MediaDescriptor = 1
            SectorsPerFAT = 2
            SectorsPerTrack = 2
            NumberOfHeads = 2
            HiddenSectors = 2
            SectorCountLarge = 4
            DriveNumber = 1
            Reserved = 1
            ExtendedBootSignature = 1
            VolumeSerialNumber = 4
            VolumeLabel = 11
            FileSystemType = 8
            BootStrapCode = 448
            BootStrapSignature = 2
        End Enum

        Sub New(FileBytes As ImageByteArray)
            _FileBytes = FileBytes
        End Sub

        Public Property BootStrapCode() As Byte()
            Get
                Dim BootStrapStart = GetBootStrapOffset()
                Dim BootStrapLength = BootSectorOffsets.BootStrapSignature - BootStrapStart
                If BootStrapStart > 2 And BootStrapLength > 0 Then
                    Return _FileBytes.GetBytes(BootStrapStart, BootStrapLength)
                Else
                    Return {}
                End If
            End Get
            Set
                Dim BootStrapStart = GetBootStrapOffset()
                Dim BootStrapLength = BootSectorOffsets.BootStrapSignature - BootStrapStart
                If BootStrapStart > 2 And BootStrapLength > 0 Then
                    _FileBytes.SetBytes(Value, BootStrapStart, BootSectorOffsets.BootStrapSignature - BootStrapStart, 0)
                End If
            End Set
        End Property

        Public Property BootStrapSignature() As UShort
            Get
                Return _FileBytes.GetBytesShort(BootSectorOffsets.BootStrapSignature)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.BootStrapSignature)
            End Set
        End Property

        Public Property BytesPerSector() As UShort
            Get
                Return _FileBytes.GetBytesShort(BootSectorOffsets.BytesPerSector)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.BytesPerSector)
            End Set
        End Property

        Public ReadOnly Property Data As Byte()
            Get
                Return _FileBytes.GetSector(BOOT_SECTOR)
            End Get
        End Property

        Public Property DriveNumber() As Byte
            Get
                Return _FileBytes.GetByte(BootSectorOffsets.DriveNumber)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.DriveNumber)
            End Set
        End Property

        Public Property ExtendedBootSignature() As Byte
            Get
                Return _FileBytes.GetByte(BootSectorOffsets.ExtendedBootSignature)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.ExtendedBootSignature)
            End Set
        End Property

        Public Property FileSystemType() As Byte()
            Get
                Return _FileBytes.GetBytes(BootSectorOffsets.FileSystemType, BootSectorSizes.FileSystemType)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.FileSystemType, BootSectorSizes.FileSystemType, 0)
            End Set
        End Property

        Public Property HiddenSectors() As UShort
            Get
                Return _FileBytes.GetBytesShort(BootSectorOffsets.HiddenSectors)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.HiddenSectors)
            End Set
        End Property

        Public Property JmpBoot() As Byte()
            Get
                Return _FileBytes.GetBytes(BootSectorOffsets.JmpBoot, BootSectorSizes.JmpBoot)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.JmpBoot, BootSectorSizes.JmpBoot, 0)
            End Set
        End Property

        Public Property MediaDescriptor() As Byte
            Get
                Return _FileBytes.GetByte(BootSectorOffsets.MediaDescriptor)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.MediaDescriptor)
            End Set
        End Property

        Public Property NumberOfFATs() As Byte
            Get
                Return _FileBytes.GetByte(BootSectorOffsets.NumberOfFATs)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.NumberOfFATs)
            End Set
        End Property

        Public Property NumberOfHeads() As UShort
            Get
                Return _FileBytes.GetBytesShort(BootSectorOffsets.NumberOfHeads)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.NumberOfHeads)
            End Set
        End Property

        Public Property OEMName() As Byte()
            Get
                Return _FileBytes.GetBytes(BootSectorOffsets.OEMName, BootSectorSizes.OEMName)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.OEMName, BootSectorSizes.OEMName, 0)
            End Set
        End Property

        Public Property Reserved() As Byte
            Get
                Return _FileBytes.GetByte(BootSectorOffsets.Reserved)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.Reserved)
            End Set
        End Property

        Public Property ReservedSectorCount() As UShort
            Get
                Return _FileBytes.GetBytesShort(BootSectorOffsets.ReservedSectorCount)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.ReservedSectorCount)
            End Set
        End Property

        Public Property RootEntryCount() As UShort
            Get
                Return _FileBytes.GetBytesShort(BootSectorOffsets.RootEntryCount)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.RootEntryCount)
            End Set
        End Property

        Public Property SectorCountLarge() As UInteger
            Get
                Return _FileBytes.GetBytesInteger(BootSectorOffsets.SectorCountLarge)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.SectorCountLarge)
            End Set
        End Property

        Public Property SectorCountSmall() As UShort
            Get
                Return _FileBytes.GetBytesShort(BootSectorOffsets.SectorCountSmall)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.SectorCountSmall)
            End Set
        End Property

        Public Property SectorsPerCluster() As Byte
            Get
                Return _FileBytes.GetByte(BootSectorOffsets.SectorsPerCluster)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.SectorsPerCluster)
            End Set
        End Property

        Public Property SectorsPerFAT() As UShort
            Get
                Return _FileBytes.GetBytesShort(BootSectorOffsets.SectorsPerFAT)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.SectorsPerFAT)
            End Set
        End Property

        Public Property SectorsPerTrack() As UShort
            Get
                Return _FileBytes.GetBytesShort(BootSectorOffsets.SectorsPerTrack)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.SectorsPerTrack)
            End Set
        End Property

        Public Property VolumeLabel() As Byte()
            Get
                Return _FileBytes.GetBytes(BootSectorOffsets.VolumeLabel, BootSectorSizes.VolumeLabel)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.VolumeLabel, BootSectorSizes.VolumeLabel, 0)
            End Set
        End Property

        Public Property VolumeSerialNumber() As UInteger
            Get
                Return _FileBytes.GetBytesInteger(BootSectorOffsets.VolumeSerialNumber)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.VolumeSerialNumber)
            End Set
        End Property

        Public Function BytesPerCluster() As UInteger
            Return SectorToBytes(SectorsPerCluster)
        End Function

        Public Function ClusterToOffset(Cluster As UShort) As UInteger
            Return SectorToBytes(ClusterToSector(Cluster))
        End Function

        Public Function ClusterToSector(Cluster As UShort) As UInteger
            Return (DataRegionStart() + ((Cluster - 2) * SectorsPerCluster))
        End Function

        Public Function DataRegionSize() As UInteger
            Return SectorCount() - (ReservedSectorCount + FATRegionSize() + RootDirectoryRegionSize())
        End Function

        Public Function DataRegionStart() As UInteger
            Return RootDirectoryRegionStart() + RootDirectoryRegionSize()
        End Function

        Public Function FATRegionSize() As UInteger
            Return NumberOfFATs * SectorsPerFAT
        End Function

        Public Function FATRegionStart() As UInteger
            Return ReservedSectorCount
        End Function

        Public Function GetBootStrapOffset() As UShort
            Dim JmpBootLocal = JmpBoot
            Dim JumpInstruction As Byte = JmpBootLocal(0)
            Dim Offset As UShort

            If JumpInstruction = &HEB Then
                Offset = JmpBootLocal(1) + 2
            ElseIf JumpInstruction = &HE9 Then
                Offset = BitConverter.ToUInt16(JmpBootLocal, 1) + 3
            Else
                Offset = 0
            End If

            If Offset >= BootSectorOffsets.BootStrapSignature Then
                Offset = 0
            End If

            Return Offset
        End Function

        Public Function GetOEMNameString() As String
            Return CodePage437ToUnicode(OEMName)
        End Function

        Public Function GetVolumeLabelString() As String
            Return CodePage437ToUnicode(VolumeLabel)
        End Function

        Public Function HasValidBootStrapSignature() As Boolean
            Return BootStrapSignature = _ValidBootStrapSignature
        End Function

        Public Function HasValidBytesPerSector() As Boolean
            Return _ValidBytesPerSector.Contains(BytesPerSector)
        End Function

        Public Function HasValidDriveNumber() As Boolean
            Return _ValidDriveNumber.Contains(DriveNumber)
        End Function

        Public Function HasValidExtendedBootSignature() As Boolean
            Return _ValidExtendedBootSignature.Contains(ExtendedBootSignature)
        End Function

        Public Function HasValidJumpInstruction() As Boolean
            Return _ValidJumpInstructuon.Contains(JmpBoot(0))
        End Function

        Public Function HasValidMediaDescriptor() As Boolean
            Return _ValidMediaDescriptor.Contains(MediaDescriptor)
        End Function

        Public Function HasValidNumberOfFATs() As Boolean
            Return NumberOfFATs > 0
        End Function

        Public Function HasValidReservedSectorCount() As Boolean
            Return ReservedSectorCount > 0
        End Function

        Public Function HasValidRootEntryCount() As Boolean
            Return (RootEntryCount * 32) Mod BYTES_PER_SECTOR = 0
        End Function

        Public Function HasValidSectorsPerCluster() As Boolean
            Return _ValidSectorsPerSector.Contains(SectorsPerCluster)
        End Function

        Public Function ImageSize() As UInteger
            Return SectorToBytes(SectorCount())
        End Function

        Public Function IsBootSectorRegion(Offset As UInteger) As Boolean
            Return OffsetToSector(Offset) = BOOT_SECTOR
        End Function

        Public Function IsValidImage() As Boolean
            Return HasValidSectorsPerCluster() AndAlso HasValidMediaDescriptor()
        End Function

        Public Function IsWin9xOEMName() As Boolean
            Dim OEMNameLocal = OEMName

            Return OEMNameLocal(5) = &H49 And OEMNameLocal(6) = &H48 And OEMNameLocal(7) = &H43
        End Function

        Public Function NumberOfFATEntries() As UShort
            Return DataRegionSize() \ SectorsPerCluster
        End Function

        Public Function OffsetToCluster(Offset As UInteger) As UShort
            Return SectorToCluster(OffsetToSector(Offset))
        End Function

        Public Function RootDirectoryRegionSize() As UInteger
            Return BytesToSector(RootEntryCount * 32)
        End Function

        Public Function RootDirectoryRegionStart() As UInteger
            Return FATRegionStart() + FATRegionSize()
        End Function

        Public Function SectorCount() As UInteger
            If SectorCountSmall > 0 Then
                Return SectorCountSmall
            Else
                Return SectorCountLarge
            End If
        End Function

        Public Function SectorToCluster(Sector As UInteger) As UShort
            If Sector < DataRegionStart() Then
                Return 0
            End If
            Dim Result = (Sector - DataRegionStart()) \ SectorsPerCluster + 2
            If Result < 2 Then
                Return 0
            Else
                Return Result
            End If
        End Function
    End Class

End Namespace