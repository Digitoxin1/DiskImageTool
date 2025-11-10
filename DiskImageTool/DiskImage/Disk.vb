Namespace DiskImage
    Public Class Disk
        Public Shared ReadOnly FreeClusterBytes() As Byte = {&HF6, &H0, &HE5}
        Private ReadOnly _BootSector As BootSector
        Private ReadOnly _FATTables As FATTables
        Private ReadOnly _FloppyImage As IFloppyImage
        Private ReadOnly _RootDirectory As RootDirectory
        Private _BPB As BiosParameterBlock
        Private _DiskFormat As FloppyDiskFormat

        Sub New(FloppyImage As IFloppyImage, FatIndex As UShort, Optional Modifications As Stack(Of DataChange()) = Nothing)
            _FloppyImage = FloppyImage
            _FloppyImage.History.Enabled = True

            _BootSector = New BootSector(_FloppyImage, BootSector.BOOT_SECTOR_OFFSET)
            _BPB = GetBPB(FloppyImage.BytesPerSector)

            _FATTables = New FATTables(_BPB, _FloppyImage, FatIndex)
            _DiskFormat = InferFloppyDiskFormat()
            _FATTables.SyncFATs = Not IsDiskFormatXDF(_DiskFormat)

            _RootDirectory = New RootDirectory(Me, _FATTables.FAT)

            If Modifications IsNot Nothing Then
                _FloppyImage.History.ApplyModifications(Modifications)
                Reinitialize()
            End If
        End Sub

        Public ReadOnly Property BootSector As BootSector
            Get
                Return _BootSector
            End Get
        End Property

        Public ReadOnly Property BPB As BiosParameterBlock
            Get
                Return _BPB
            End Get
        End Property

        Public ReadOnly Property DiskFormat As FloppyDiskFormat
            Get
                Return _DiskFormat
            End Get
        End Property

        Public ReadOnly Property FAT As FAT12
            Get
                Return _FATTables.FAT
            End Get
        End Property

        Public ReadOnly Property FATTables As FATTables
            Get
                Return _FATTables
            End Get
        End Property

        Public ReadOnly Property Image As IFloppyImage
            Get
                Return _FloppyImage
            End Get
        End Property

        Public ReadOnly Property RootDirectory As RootDirectory
            Get
                Return _RootDirectory
            End Get
        End Property

        Public Function BeginTransaction() As Boolean
            If _FloppyImage.History.BatchEditMode Then
                Return False
            End If

            _FATTables.BatchUpdates = False
            _FloppyImage.History.BatchEditMode = True

            Return True
        End Function

        Public Function CheckImageSize() As Integer
            Dim ReportedSize As Integer = _BPB.ReportedImageSize()
            Return _FloppyImage.Length.CompareTo(ReportedSize)
        End Function

        Public Function CheckSize() As Boolean
            Return (_FloppyImage.Length > 0 And _FloppyImage.Length < 4423680)
        End Function

        Public Sub ClearChanges()
            _FloppyImage.History.ClearChanges()
            _RootDirectory.RefreshCache()
        End Sub

        Public Sub EndTransaction()
            If _FloppyImage.History.BatchEditMode Then
                If _FATTables.BatchUpdates Then
                    _FATTables.UpdateFAT12()
                End If
                _FloppyImage.History.BatchEditMode = False
            End If
        End Sub

        Public Function GetSector(Sector As UInteger) As Byte()
            Dim Offset = _BPB.SectorToBytes(Sector)

            Return GetSectors(Sector, 1)
        End Function

        Public Function GetSectors(SectorStart As UInteger, Count As UShort) As Byte()
            Dim Offset = _BPB.SectorToBytes(SectorStart)
            Dim Size = _BPB.SectorToBytes(Count)

            If Size + Offset > _FloppyImage.Length Then
                Size = _FloppyImage.Length - Offset
            End If

            Return _FloppyImage.GetBytes(Offset, Size)
        End Function

        Public Function GetXDFChecksum() As UInteger
            Return _FloppyImage.GetBytesInteger(&H13C)
        End Function

        Public Function IsValidImage(Optional CheckBPB As Boolean = True) As Boolean
            Return _FloppyImage.Length >= 512 And (Not CheckBPB OrElse _BPB.IsValid)
        End Function

        Public Sub Reinitialize()
            _BPB = GetBPB(_FloppyImage.BytesPerSector)
            _FATTables.Reinitialize(_BPB)
            _DiskFormat = InferFloppyDiskFormat()
            _FATTables.SyncFATs = Not IsDiskFormatXDF(_DiskFormat)
            _RootDirectory.RefreshData()
        End Sub

        Public Function SetSector(Value() As Byte, Sector As UInteger) As Boolean
            Dim Offset = _BPB.SectorToBytes(Sector)

            Return _FloppyImage.SetBytes(Value, Offset, _BPB.BytesPerSector, 0)
        End Function

        Private Function GetBPB(BytesPerSector As UInteger) As BiosParameterBlock
            Dim BPB As BiosParameterBlock

            If _BootSector.BPB.IsValid Then
                'BPB = _BootSector.BPB
                BPB = New BiosParameterBlock(_BootSector.Data)
                Dim DiskFormat = GetFloppyDiskFomat(BPB.MediaDescriptor)
                If DiskFormat = FloppyDiskFormat.Floppy160 Or DiskFormat = FloppyDiskFormat.Floppy180 Then
                    BPB.SectorsPerCluster = 1
                End If
            Else
                Dim FATMediaDescriptor = GetFATMediaDescriptor()

                Dim DiskFormatSize = GetFloppyDiskFormat(_FloppyImage.Length)
                Dim DiskFormatFAT = GetFloppyDiskFomat(FATMediaDescriptor)

                If DiskFormatFAT = FloppyDiskFormat.Floppy360 And DiskFormatSize = FloppyDiskFormat.Floppy180 Then
                    FATMediaDescriptor = GetFloppyDiskMediaDescriptor(DiskFormatSize)
                ElseIf DiskFormatFAT = FloppyDiskFormat.Floppy320 And DiskFormatSize = FloppyDiskFormat.Floppy160 Then
                    FATMediaDescriptor = GetFloppyDiskMediaDescriptor(DiskFormatSize)
                End If

                BPB = BuildBPB(FATMediaDescriptor)

                If BPB.BytesPerSector = 0 Then
                    BPB.BytesPerSector = BuildBPB(DiskFormatSize).BytesPerSector
                End If

                If BPB.BytesPerSector = 0 Then
                    BPB.BytesPerSector = BytesPerSector
                End If
            End If

            Return BPB
        End Function

        Private Function GetFATMediaDescriptor() As Byte
            Dim Result As Byte = 0

            If _FloppyImage.Length >= _FloppyImage.BytesPerSector + 3 Then
                Dim b = _FloppyImage.GetBytes(_FloppyImage.BytesPerSector, 3)
                If b(1) = &HFF And b(2) = &HFF Then
                    Result = b(0)
                End If
            End If

            Return Result
        End Function
        Private Function InferFloppyDiskFormat() As FloppyDiskFormat
            Dim DiskFormat As FloppyDiskFormat

            If _BPB.IsValid Then
                DiskFormat = GetFloppyDiskFormat(_BPB)
            Else
                DiskFormat = GetFloppyDiskFomat(_FATTables.FAT.MediaDescriptor)
                If DiskFormat = FloppyDiskFormat.FloppyUnknown Then
                    DiskFormat = GetFloppyDiskFormat(_FloppyImage.Length)
                End If
            End If

            Return DiskFormat
        End Function
    End Class

End Namespace