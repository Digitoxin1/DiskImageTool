Namespace DiskImage

    Public Class BootSector
        Private ReadOnly _Parent As Disk
        Private ReadOnly _ValidBootStrapSignature As UShort = &HAA55
        Private ReadOnly _ValidBytesPerSector() As UShort = {512, 1024, 2048, 4096}
        Private ReadOnly _ValidDriveNumber() As Byte = {&H0, &H80}
        Private ReadOnly _ValidExtendedBootSignature() As Byte = {&H29}
        Private ReadOnly _ValidJumpInstructuon() As Byte = {&HEB, &HE9}
        Private ReadOnly _ValidMediaDescriptor() As Byte = {&HF0, &HF8, &HF9, &HFA, &HFB, &HFC, &HFD, &HFE, &HFF}
        Private ReadOnly _ValidSectorsPerSector() As Byte = {1, 2, 4, 8, 16, 32, 64, 128}

        Public Enum BootSectorOffset As UInteger
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
            BiosParameterBlock = 11
            ExtendedParameterBlock = 36
        End Enum

        Public Enum BootSectorSize As UInteger
            JmpBoot = 3
            OEMName = 8
            VolumeLabel = 11
            FileSystemType = 8
            BootStrapCode = 448
            Data = 512
            BiosParameterBlockFat12 = 19
            BiosParameterBlockFat16 = 25
            ExtendedParameterBlock = 26
        End Enum

        Sub New(Parent As Disk)
            _Parent = Parent
        End Sub
        Public Property BootStrapCode() As Byte()
            Get
                Dim BootStrapStart = GetBootStrapOffset()
                Dim BootStrapLength = BootSectorOffset.BootStrapSignature - BootStrapStart
                If BootStrapStart > 2 And BootStrapLength > 0 Then
                    Return _Parent.GetBytes(BootStrapStart, BootStrapLength)
                Else
                    Return {}
                End If
            End Get
            Set
                Dim BootStrapStart = GetBootStrapOffset()
                Dim BootStrapLength = BootSectorOffset.BootStrapSignature - BootStrapStart
                If BootStrapStart > 2 And BootStrapLength > 0 Then
                    _Parent.SetBytes(Value, BootStrapStart, BootSectorOffset.BootStrapSignature - BootStrapStart, 0)
                End If
            End Set
        End Property

        Public Property BootStrapSignature() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.BootStrapSignature)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.BootStrapSignature)
            End Set
        End Property

        Public Property BytesPerSector() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.BytesPerSector)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.BytesPerSector)
            End Set
        End Property

        Public ReadOnly Property Data As Byte()
            Get
                Return _Parent.GetBytes(0, BootSectorSize.Data)
            End Get
        End Property

        Public Property DriveNumber() As Byte
            Get
                Return _Parent.GetByte(BootSectorOffset.DriveNumber)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.DriveNumber)
            End Set
        End Property

        Public Property ExtendedBootSignature() As Byte
            Get
                Return _Parent.GetByte(BootSectorOffset.ExtendedBootSignature)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.ExtendedBootSignature)
            End Set
        End Property

        Public Property FileSystemType() As Byte()
            Get
                Return _Parent.GetBytes(BootSectorOffset.FileSystemType, BootSectorSize.FileSystemType)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.FileSystemType, BootSectorSize.FileSystemType, 0)
            End Set
        End Property

        Public Property HiddenSectors() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.HiddenSectors)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.HiddenSectors)
            End Set
        End Property

        Public Property JmpBoot() As Byte()
            Get
                Return _Parent.GetBytes(BootSectorOffset.JmpBoot, BootSectorSize.JmpBoot)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.JmpBoot, BootSectorSize.JmpBoot, 0)
            End Set
        End Property

        Public Property MediaDescriptor() As Byte
            Get
                Return _Parent.GetByte(BootSectorOffset.MediaDescriptor)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.MediaDescriptor)
            End Set
        End Property

        Public Property NumberOfFATs() As Byte
            Get
                Return _Parent.GetByte(BootSectorOffset.NumberOfFATs)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.NumberOfFATs)
            End Set
        End Property

        Public Property NumberOfHeads() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.NumberOfHeads)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.NumberOfHeads)
            End Set
        End Property

        Public Property OEMName() As Byte()
            Get
                Return _Parent.GetBytes(BootSectorOffset.OEMName, BootSectorSize.OEMName)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.OEMName, BootSectorSize.OEMName, 0)
            End Set
        End Property

        Public Property Reserved() As Byte
            Get
                Return _Parent.GetByte(BootSectorOffset.Reserved)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.Reserved)
            End Set
        End Property

        Public Property ReservedSectorCount() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.ReservedSectorCount)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.ReservedSectorCount)
            End Set
        End Property

        Public Property RootEntryCount() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.RootEntryCount)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.RootEntryCount)
            End Set
        End Property

        Public Property SectorCountLarge() As UInteger
            Get
                Return _Parent.GetBytesInteger(BootSectorOffset.SectorCountLarge)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.SectorCountLarge)
            End Set
        End Property

        Public Property SectorCountSmall() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.SectorCountSmall)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.SectorCountSmall)
            End Set
        End Property

        Public Property SectorsPerCluster() As Byte
            Get
                Return _Parent.GetByte(BootSectorOffset.SectorsPerCluster)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.SectorsPerCluster)
            End Set
        End Property

        Public Property SectorsPerFAT() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.SectorsPerFAT)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.SectorsPerFAT)
            End Set
        End Property

        Public Property SectorsPerTrack() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.SectorsPerTrack)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.SectorsPerTrack)
            End Set
        End Property

        Public Property VolumeLabel() As Byte()
            Get
                Return _Parent.GetBytes(BootSectorOffset.VolumeLabel, BootSectorSize.VolumeLabel)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.VolumeLabel, BootSectorSize.VolumeLabel, 0)
            End Set
        End Property

        Public Property VolumeSerialNumber() As UInteger
            Get
                Return _Parent.GetBytesInteger(BootSectorOffset.VolumeSerialNumber)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.VolumeSerialNumber)
            End Set
        End Property

        Public Function BytesPerCluster() As UInteger
            Return SectorsPerCluster * BytesPerSector
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

            If Offset >= BootSectorOffset.BootStrapSignature Then
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
            Return (RootEntryCount * 32) Mod BytesPerSector = 0
        End Function

        Public Function HasValidSectorsPerCluster() As Boolean
            Return _ValidSectorsPerSector.Contains(SectorsPerCluster)
        End Function

        Public Function ImageSize() As UInteger
            Return SectorCount() * BytesPerSector
        End Function

        Public Function IsWin9xOEMName() As Boolean
            Dim OEMNameLocal = OEMName

            Return OEMNameLocal(5) = &H49 And OEMNameLocal(6) = &H48 And OEMNameLocal(7) = &H43
        End Function
        Public Function NumberOfFATEntries() As UShort
            Return DataRegionSize() \ SectorsPerCluster
        End Function

        Public Function RootDirectoryRegionSize() As UInteger
            Return Math.Ceiling((RootEntryCount * 32) / BytesPerSector)
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
    End Class

End Namespace