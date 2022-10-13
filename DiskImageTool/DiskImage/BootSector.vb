Namespace DiskImage
    Public Class BootSector
        Private Enum BootSectorOffset As UInteger
            Code = 0
            OEMID = 3
            BytesPerSector = 11
            SectorsPerCluster = 13
            ReservedSectors = 14
            FatCopies = 16
            RootEntryCount = 17
            SmallNumberOfSectors = 19
            MediaDescriptor = 21
            SectorsPerFAT = 22
            SectorsPerTrack = 24
            NumberOfHeads = 26
            HiddenSectors = 28
            LargeNumberOfSectors = 32
            DriveNumber = 36
            Reserved = 37
            ExtendedBootSignature = 38
            VolumeSerialNumber = 39
            VolumeLabel = 43
            FileSystemType = 54
            BootStrapCode = 62
            BootStrapSignature = 510
        End Enum

        Private Enum BootSectorSize As UInteger
            Code = 3
            OEMID = 8
            VolumeLabel = 11
            FileSystemType = 8
            BootStrapCode = 448
            Data = 512
        End Enum

        ReadOnly _Parent As Disk

        Sub New(Parent As Disk)
            _Parent = Parent
        End Sub

        Public Function BytesPerCluster() As UInteger
            Return SectorsPerCluster * BytesPerSector
        End Function

        Public Function DataRegionSize() As UInteger
            Return SectorCount() - (ReservedSectors + FatRegionSize() + RootDirectoryRegionSize())
        End Function

        Public Function DataRegionStart() As UInteger
            Return RootDirectoryRegionStart() + RootDirectoryRegionSize()
        End Function

        Public Function FatRegionStart() As UInteger
            Return ReservedSectors
        End Function

        Public Function FatRegionSize() As UInteger
            Return FatCopies * SectorsPerFAT
        End Function

        Public Function NumberOfFATEntries() As UInteger
            Return Int(DataRegionSize() / SectorsPerCluster)
        End Function

        Public Function RootDirectoryRegionSize() As UInteger
            Return Math.Ceiling((RootEntryCount * 32) / BytesPerSector)
        End Function

        Public Function RootDirectoryRegionStart() As UInteger
            Return FatRegionStart() + FatRegionSize()
        End Function

        Public Function SectorCount() As UInteger
            If SmallNumberOfSectors > 0 Then
                Return SmallNumberOfSectors
            Else
                Return LargeNumberOfSectors
            End If
        End Function

        Public Property BootStrapCode() As Byte()
            Get
                Return _Parent.GetBytes(BootSectorOffset.BootStrapCode, BootSectorSize.BootStrapCode)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.BootStrapCode, BootSectorSize.BootStrapCode, 0)
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

        Public Property Code() As Byte()
            Get
                Return _Parent.GetBytes(BootSectorOffset.Code, BootSectorSize.Code)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.Code, BootSectorSize.Code, 0)
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

        Public Property FatCopies() As Byte
            Get
                Return _Parent.GetByte(BootSectorOffset.FatCopies)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.FatCopies)
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

        Public Property HiddenSectors() As UInteger
            Get
                Return _Parent.GetBytesInteger(BootSectorOffset.HiddenSectors)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.HiddenSectors)
            End Set
        End Property

        Public Property LargeNumberOfSectors() As UInteger
            Get
                Return _Parent.GetBytesInteger(BootSectorOffset.LargeNumberOfSectors)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.LargeNumberOfSectors)
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

        Public Property NumberOfHeads() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.NumberOfHeads)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.NumberOfHeads)
            End Set
        End Property

        Public Property OEMID() As Byte()
            Get
                Return _Parent.GetBytes(BootSectorOffset.OEMID, BootSectorSize.OEMID)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.OEMID, BootSectorSize.OEMID, 0)
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

        Public Property ReservedSectors() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.ReservedSectors)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.ReservedSectors)
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

        Public Property SmallNumberOfSectors() As UShort
            Get
                Return _Parent.GetBytesShort(BootSectorOffset.SmallNumberOfSectors)
            End Get
            Set
                _Parent.SetBytes(Value, BootSectorOffset.SmallNumberOfSectors)
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
    End Class
End Namespace
