﻿
Namespace DiskImage
    Public Class BiosParameterBlock
        Public Const BPB_SIZE As UShort = 36
        Public Shared ReadOnly ValidBytesPerSector() As UShort = {512, 1024, 2048, 4096}
        Public Shared ReadOnly ValidMediaDescriptor() As Byte = {&HF0, &HF8, &HF9, &HFA, &HFB, &HFC, &HFD, &HFE, &HFF, &HED}
        Public Shared ReadOnly ValidNumberOfFATS() As Byte = {1, 2}
        Public Shared ReadOnly ValidNumberOfHeads() As UShort = {1, 2}
        Public Shared ReadOnly ValidSectorsPerCluster() As Byte = {1, 2, 4, 8, 16, 32, 64, 128}
        Public Shared ReadOnly ValidSectorsPerTrack() As UShort = {8, 9, 10, 15, 18, 19, 21, 23, 36}
        Private ReadOnly _FloppyImage As IFloppyImage
        Private ReadOnly _Offset As UInteger

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

        Sub New()
            Dim Data(BPB_SIZE - 1) As Byte
            _FloppyImage = New BasicSectorImage(Data)
            _Offset = 0
        End Sub

        Sub New(FloppyImage As IFloppyImage, Offset As UInteger)
            _FloppyImage = FloppyImage
            _Offset = Offset
        End Sub

        Sub New(Data() As Byte)
            _FloppyImage = New BasicSectorImage(Data)
            _Offset = 0
        End Sub

        Public Property BytesPerSector() As UShort
            Get
                Return _FloppyImage.GetBytesShort(BPBOoffsets.BytesPerSector + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.BytesPerSector + _Offset)
            End Set
        End Property

        Public Property HiddenSectors() As UShort
            Get
                Return _FloppyImage.GetBytesShort(BPBOoffsets.HiddenSectors + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.HiddenSectors + _Offset)
            End Set
        End Property

        Public Property HiddenSectorsFAT16() As UInteger
            Get
                Return _FloppyImage.GetBytesInteger(BPBOoffsets.HiddenSectors + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.HiddenSectors + _Offset)
            End Set
        End Property

        Public Property MediaDescriptor() As Byte
            Get
                Return _FloppyImage.GetByte(BPBOoffsets.MediaDescriptor + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.MediaDescriptor + _Offset)
            End Set
        End Property

        Public Property NumberOfFATs() As Byte
            Get
                Return _FloppyImage.GetByte(BPBOoffsets.NumberOfFATs + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.NumberOfFATs + _Offset)
            End Set
        End Property

        Public Property NumberOfHeads() As UShort
            Get
                Return _FloppyImage.GetBytesShort(BPBOoffsets.NumberOfHeads + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.NumberOfHeads + _Offset)
            End Set
        End Property

        Public Property ReservedSectorCount() As UShort
            Get
                Return _FloppyImage.GetBytesShort(BPBOoffsets.ReservedSectorCount + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.ReservedSectorCount + _Offset)
            End Set
        End Property

        Public Property RootEntryCount() As UShort
            Get
                Return _FloppyImage.GetBytesShort(BPBOoffsets.RootEntryCount + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.RootEntryCount + _Offset)
            End Set
        End Property

        Public Property SectorCountLarge() As UInteger
            Get
                Return _FloppyImage.GetBytesInteger(BPBOoffsets.SectorCountLarge + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.SectorCountLarge + _Offset)
            End Set
        End Property

        Public Property SectorCountSmall() As UShort
            Get
                Return _FloppyImage.GetBytesShort(BPBOoffsets.SectorCountSmall + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.SectorCountSmall + _Offset)
            End Set
        End Property

        Public Property SectorsPerCluster() As Byte
            Get
                Return _FloppyImage.GetByte(BPBOoffsets.SectorsPerCluster + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.SectorsPerCluster + _Offset)
            End Set
        End Property

        Public Property SectorsPerFAT() As UShort
            Get
                Return _FloppyImage.GetBytesShort(BPBOoffsets.SectorsPerFAT + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.SectorsPerFAT + _Offset)
            End Set
        End Property

        Public Property SectorsPerTrack() As UShort
            Get
                Return _FloppyImage.GetBytesShort(BPBOoffsets.SectorsPerTrack + _Offset)
            End Get
            Set
                _FloppyImage.SetBytes(Value, BPBOoffsets.SectorsPerTrack + _Offset)
            End Set
        End Property

        Public Function BytesPerCluster() As UInteger
            Return SectorToBytes(SectorsPerCluster)
        End Function

        Public Function BytesToSector(Bytes As UInteger) As UInteger
            Return Math.Ceiling(Bytes / BytesPerSector)
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
            Return (RootEntryCount * 32) Mod BytesPerSector = 0
        End Function

        Public Function HasValidSectorsPerCluster(CheckMediaDescriptor As Boolean) As Boolean
            If CheckMediaDescriptor Then
                If MediaDescriptor = &HFC Or MediaDescriptor = &HFE Then
                    If SectorsPerCluster <> 1 Then
                        Return False
                    End If
                End If
            End If
            Return ValidSectorsPerCluster.Contains(SectorsPerCluster)
        End Function

        Public Function HasValidSectorsPerFAT() As Boolean
            Return SectorsPerFAT > 0 And SectorsPerFAT <= 11
        End Function

        Public Function HasValidSectorsPerTrack() As Boolean
            Return ValidSectorsPerTrack.Contains(SectorsPerTrack)
        End Function

        Public Function ImageSize() As UInteger
            Return SectorToBytes(SectorCount())
        End Function

        Public Function IsFATRegion(Offset As UInteger, Length As UInteger) As Boolean
            Dim FATSectorStart = FATRegionStart()
            Dim FATSectorEnd = FATSectorStart + (SectorsPerFAT * NumberOfFATs) - 1

            Dim SectorStart = OffsetToSector(Offset)
            Dim SectorEnd = OffsetToSector(Offset + Length - 1)

            Return (SectorStart >= FATSectorStart And SectorStart <= FATSectorEnd) Or (SectorEnd >= FATSectorStart And SectorEnd <= FATSectorEnd)
        End Function

        Public Function IsValid() As Boolean
            Return _FloppyImage.Length >= BPB_SIZE _
                AndAlso HasValidSectorsPerCluster(False) _
                AndAlso HasValidReservedSectorCount() _
                AndAlso HasValidNumberOfFATs() _
                AndAlso RootEntryCount > 0 _
                AndAlso RootEntryCount <= 512 _
                AndAlso SectorCount() > 0 _
                AndAlso HasValidMediaDescriptor() _
                AndAlso HasValidSectorsPerFAT() _
                AndAlso HasValidSectorsPerTrack() _
                AndAlso HasValidNumberOfHeadss()
        End Function

        Public Function NumberOfFATEntries() As UShort
            Return DataRegionSize() \ SectorsPerCluster
        End Function

        Public Function OffsetToCluster(Offset As UInteger) As UShort
            Return SectorToCluster(OffsetToSector(Offset))
        End Function

        Public Function OffsetToSector(Offset As UInteger) As UInteger
            Return Offset \ BytesPerSector
        End Function

        Public Function ReportedImageSize() As UInteger
            Return SectorToBytes(DataRegionStart() + DataRegionSize())
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

        Public Function SectorToBytes(Sector As UInteger) As UInteger
            Return Sector * BytesPerSector()
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

        Public Function SectorToTrackSector(Sector As UInteger) As UShort
            Return Sector Mod SectorsPerTrack
        End Function

        Public Function TrackToSector(Track As UShort, Side As UShort) As UInteger
            Return Track * NumberOfHeads * SectorsPerTrack + SectorsPerTrack * Side
        End Function
    End Class
End Namespace
