Namespace DiskImage
    Public Class Disk
        Private WithEvents FileBytes As ImageByteArray
        Public Const BYTES_PER_SECTOR As UShort = 512
        Private ReadOnly _BootSector As BootSector
        Private _BPB As BiosParameterBlock
        Private ReadOnly _RootDirectory As RootDirectory
        Private ReadOnly _FATTables As FATTables
        Private _DiskFormat As FloppyDiskFormat

        Sub New(Image As IByteArray, FatIndex As UShort, Optional Modifications As Stack(Of DataChange()) = Nothing)
            FileBytes = New ImageByteArray(Image)
            _BootSector = New BootSector(FileBytes.Data, BootSector.BOOT_SECTOR_OFFSET)
            _BPB = GetBPB()

            _FATTables = New FATTables(_BPB, FileBytes, FatIndex)
            _DiskFormat = InferFloppyDiskFormat()
            _FATTables.SyncFATs = Not IsDiskFormatXDF(_DiskFormat)

            _RootDirectory = New RootDirectory(Me, _FATTables)

            If Modifications IsNot Nothing Then
                FileBytes.ApplyModifications(Modifications)
                Reinitialize()
            End If
        End Sub

        Public ReadOnly Property BPB As BiosParameterBlock
            Get
                Return _BPB
            End Get
        End Property

        Public ReadOnly Property BootSector As BootSector
            Get
                Return _BootSector
            End Get
        End Property

        Public ReadOnly Property Image As ImageByteArray
            Get
                Return FileBytes
            End Get
        End Property

        Public ReadOnly Property RootDirectory As RootDirectory
            Get
                Return _RootDirectory
            End Get
        End Property

        Public ReadOnly Property DiskFormat As FloppyDiskFormat
            Get
                Return _DiskFormat
            End Get
        End Property

        Public ReadOnly Property FATTables As FATTables
            Get
                Return _FATTables
            End Get
        End Property

        Public ReadOnly Property FAT As FAT12
            Get
                Return _FATTables.FAT
            End Get
        End Property

        Public Shared Function BytesToSector(Bytes As UInteger) As UInteger
            Return Math.Ceiling(Bytes / BYTES_PER_SECTOR)
        End Function

        Public Shared Function OffsetToSector(Offset As UInteger) As UInteger
            Return Offset \ BYTES_PER_SECTOR
        End Function

        Public Shared Function SectorToBytes(Sector As UInteger) As UInteger
            Return Sector * BYTES_PER_SECTOR
        End Function

        Public Function CheckImageSize() As Integer
            Dim ReportedSize As Integer = _BPB.ReportedImageSize()
            Return FileBytes.Length.CompareTo(ReportedSize)
        End Function

        Public Function CheckSize() As Boolean
            Return (FileBytes.Length > 0 And FileBytes.Length < 4423680)
        End Function

        Private Function GetFATMediaDescriptor() As Byte
            Dim Result As Byte = 0

            If FileBytes.Length >= 515 Then
                Dim b = FileBytes.GetBytes(512, 3)
                If b(1) = &HFF And b(2) = &HFF Then
                    Result = b(0)
                End If
            End If

            Return Result
        End Function

        Public Function GetXDFChecksum() As UInteger
            Return FileBytes.GetBytesInteger(&H13C)
        End Function

        Public Function IsValidImage(Optional CheckBPB As Boolean = True) As Boolean
            Return FileBytes.Length >= 512 And (Not CheckBPB OrElse _BPB.IsValid)
        End Function

        Public Sub Reinitialize()
            _BPB = GetBPB()
            _FATTables.Reinitialize(_BPB)
            _DiskFormat = InferFloppyDiskFormat()
            _FATTables.SyncFATs = Not IsDiskFormatXDF(_DiskFormat)
            _RootDirectory.RefreshData()
        End Sub

        Public Sub ClearChanges()
            FileBytes.ClearChanges()
            _RootDirectory.RefreshCache()
        End Sub

        Private Function InferFloppyDiskFormat() As FloppyDiskFormat
            Dim DiskFormat As FloppyDiskFormat

            If _BPB.IsValid Then
                DiskFormat = GetFloppyDiskFormat(_BPB, False)
            Else
                DiskFormat = GetFloppyDiskFomat(_FATTables.FAT.MediaDescriptor)
                If DiskFormat = FloppyDiskFormat.FloppyUnknown Then
                    DiskFormat = GetFloppyDiskFormat(FileBytes.Length)
                End If
            End If

            Return DiskFormat
        End Function

        Private Function GetBPB() As BiosParameterBlock
            Dim BPB As BiosParameterBlock

            If _BootSector.BPB.IsValid Then
                BPB = _BootSector.BPB
            Else
                Dim FATMediaDescriptor = GetFATMediaDescriptor()

                Dim DiskFormatFAT = GetFloppyDiskFomat(FATMediaDescriptor)
                Dim DiskFormatSize = GetFloppyDiskFormat(FileBytes.Length)

                If DiskFormatFAT = FloppyDiskFormat.Floppy360 And DiskFormatSize = FloppyDiskFormat.Floppy180 Then
                    FATMediaDescriptor = GetFloppyDiskMediaDescriptor(DiskFormatSize)
                ElseIf DiskFormatFAT = FloppyDiskFormat.Floppy320 And DiskFormatSize = FloppyDiskFormat.Floppy160 Then
                    FATMediaDescriptor = GetFloppyDiskMediaDescriptor(DiskFormatSize)
                End If

                BPB = BuildBPB(FATMediaDescriptor)
            End If

            Return BPB
        End Function
    End Class

End Namespace