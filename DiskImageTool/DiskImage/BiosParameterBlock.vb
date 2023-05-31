
Namespace DiskImage
    Public Class BiosParameterBlock
        Public Const BPB_SIZE As UShort = 36
        Public Shared ReadOnly ValidBytesPerSector() As UShort = {512, 1024, 2048, 4096}
        Public Shared ReadOnly ValidMediaDescriptor() As Byte = {&HF0, &HF8, &HF9, &HFA, &HFB, &HFC, &HFD, &HFE, &HFF}
        Public Shared ReadOnly ValidNumberOfFATS() As Byte = {1, 2}
        Public Shared ReadOnly ValidNumberOfHeads() As UShort = {1, 2}
        Public Shared ReadOnly ValidSectorsPerCluster() As Byte = {1, 2, 4, 8, 16, 32, 64, 128}
        Public Shared ReadOnly ValidSectorsPerTrack() As UShort = {8, 9, 15, 18, 21, 36}
        Private ReadOnly _FileBytes As ImageByteArray

        Public Enum BPBOoffsets As UInteger
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
        End Enum

        Public Enum BPBSizes As UInteger
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
            HiddenSectorsFAT16 = 4
            SectorCountLarge = 4
        End Enum

        Sub New(FileBytes As ImageByteArray)
            _FileBytes = FileBytes
        End Sub

        Public Property BytesPerSector() As UShort
            Get
                Return _FileBytes.GetBytesShort(BPBOoffsets.BytesPerSector)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.BytesPerSector)
            End Set
        End Property

        Public Property HiddenSectors() As UShort
            Get
                Return _FileBytes.GetBytesShort(BPBOoffsets.HiddenSectors)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.HiddenSectors)
            End Set
        End Property

        Public Property HiddenSectorsFAT16() As UInteger
            Get
                Return _FileBytes.GetBytesInteger(BPBOoffsets.HiddenSectors)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.HiddenSectors)
            End Set
        End Property

        Public Property MediaDescriptor() As Byte
            Get
                Return _FileBytes.GetByte(BPBOoffsets.MediaDescriptor)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.MediaDescriptor)
            End Set
        End Property

        Public Property NumberOfFATs() As Byte
            Get
                Return _FileBytes.GetByte(BPBOoffsets.NumberOfFATs)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.NumberOfFATs)
            End Set
        End Property

        Public Property NumberOfHeads() As UShort
            Get
                Return _FileBytes.GetBytesShort(BPBOoffsets.NumberOfHeads)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.NumberOfHeads)
            End Set
        End Property

        Public Property ReservedSectorCount() As UShort
            Get
                Return _FileBytes.GetBytesShort(BPBOoffsets.ReservedSectorCount)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.ReservedSectorCount)
            End Set
        End Property

        Public Property RootEntryCount() As UShort
            Get
                Return _FileBytes.GetBytesShort(BPBOoffsets.RootEntryCount)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.RootEntryCount)
            End Set
        End Property

        Public Property SectorCountLarge() As UInteger
            Get
                Return _FileBytes.GetBytesInteger(BPBOoffsets.SectorCountLarge)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.SectorCountLarge)
            End Set
        End Property

        Public Property SectorCountSmall() As UShort
            Get
                Return _FileBytes.GetBytesShort(BPBOoffsets.SectorCountSmall)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.SectorCountSmall)
            End Set
        End Property

        Public Property SectorsPerCluster() As Byte
            Get
                Return _FileBytes.GetByte(BPBOoffsets.SectorsPerCluster)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.SectorsPerCluster)
            End Set
        End Property

        Public Property SectorsPerFAT() As UShort
            Get
                Return _FileBytes.GetBytesShort(BPBOoffsets.SectorsPerFAT)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.SectorsPerFAT)
            End Set
        End Property

        Public Property SectorsPerTrack() As UShort
            Get
                Return _FileBytes.GetBytesShort(BPBOoffsets.SectorsPerTrack)
            End Get
            Set
                _FileBytes.SetBytes(Value, BPBOoffsets.SectorsPerTrack)
            End Set
        End Property

        Public Function BytesPerCluster() As UInteger
            Return Disk.SectorToBytes(SectorsPerCluster)
        End Function

        Public Function ClusterToOffset(Cluster As UShort) As UInteger
            Return Disk.SectorToBytes(ClusterToSector(Cluster))
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
            Return NumberOfFATs * CUInt(SectorsPerFAT)
        End Function

        Public Function FATRegionStart() As UInteger
            Return ReservedSectorCount
        End Function

        Public Function HasValidBytesPerSector() As Boolean
            Return ValidBytesPerSector.Contains(BytesPerSector)
        End Function

        Public Function HasValidMediaDescriptor() As Boolean
            Return ValidMediaDescriptor.Contains(MediaDescriptor)
        End Function

        Public Function HasValidNumberOfFATs() As Boolean
            Return ValidNumberOfFATS.Contains(NumberOfFATs)
        End Function

        Public Function HasValidNumberOfHeadss() As Boolean
            Return ValidNumberOfHeads.Contains(NumberOfHeads)
        End Function

        Public Function HasValidReservedSectorCount() As Boolean
            Return ReservedSectorCount > 0
        End Function

        Public Function HasValidRootEntryCount() As Boolean
            Return (RootEntryCount * 32) Mod Disk.BYTES_PER_SECTOR = 0
        End Function

        Public Function HasValidSectorsPerCluster() As Boolean
            Return ValidSectorsPerCluster.Contains(SectorsPerCluster)
        End Function

        Public Function HasValidSectorsPerTrack() As Boolean
            Return ValidSectorsPerTrack.Contains(SectorsPerTrack)
        End Function

        Public Function ImageSize() As UInteger
            Return Disk.SectorToBytes(SectorCount())
        End Function

        Public Function IsFATRegion(Offset As UInteger, Length As UInteger) As Boolean
            Dim FATSectorStart = FATRegionStart()
            Dim FATSectorEnd = FATSectorStart + (SectorsPerFAT * NumberOfFATs) - 1

            Dim SectorStart = Disk.OffsetToSector(Offset)
            Dim SectorEnd = Disk.OffsetToSector(Offset + Length - 1)

            Return (SectorStart >= FATSectorStart And SectorStart <= FATSectorEnd) Or (SectorEnd >= FATSectorStart And SectorEnd <= FATSectorEnd)
        End Function

        Public Function IsValid() As Boolean
            Return HasValidSectorsPerCluster() _
                AndAlso HasValidReservedSectorCount() _
                AndAlso HasValidNumberOfFATs() _
                AndAlso RootEntryCount > 0 _
                AndAlso SectorCount() > 0 _
                AndAlso HasValidMediaDescriptor() _
                AndAlso SectorsPerFAT > 0 _
                AndAlso HasValidSectorsPerTrack() _
                AndAlso HasValidNumberOfHeadss()
        End Function

        Public Function NumberOfFATEntries() As UShort
            Return DataRegionSize() \ SectorsPerCluster
        End Function

        Public Function OffsetToCluster(Offset As UInteger) As UShort
            Return SectorToCluster(Disk.OffsetToSector(Offset))
        End Function

        Public Function ReportedImageSize() As UInteger
            Return Disk.SectorToBytes(DataRegionStart() + DataRegionSize())
        End Function

        Public Function RootDirectoryRegionSize() As UInteger
            Return Disk.BytesToSector(RootEntryCount * 32)
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

        Public Function SectorToSide(Sector As UInteger) As UShort
            Return Int(Sector / SectorsPerTrack) Mod NumberOfHeads
        End Function

        Public Function SectorToTrack(Sector As UInteger) As UShort
            Return Int(Sector / SectorsPerTrack / NumberOfHeads)
        End Function
        Public Function TrackToSector(Track As UShort, Side As UShort) As UInteger
            Return Track * NumberOfHeads * SectorsPerTrack + SectorsPerTrack * Side
        End Function
    End Class
End Namespace
