Namespace DiskImage

    Public Class BootSector
        Public Const BOOT_SECTOR_SIZE As UShort = 512
        Public Shared ReadOnly ValidBootStrapSignature() As UShort = {&HAA55, &H0, &H4254}
        Public Shared ReadOnly ValidDriveNumber() As Byte = {&H0, &H80}
        Public Shared ReadOnly ValidExtendedBootSignature() As Byte = {&H28, &H29}
        Public Shared ReadOnly ValidJumpInstructuon() As Byte = {&HEB, &HE9}
        Private Const BOOT_SECTOR As UInteger = 0
        Private ReadOnly _BPB As BiosParameterBlock
        Private ReadOnly _FileBytes As ImageByteArray
        Public Enum BootSectorOffsets As UInteger
            JmpBoot = 0
            OEMName = 3
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
            _BPB = New BiosParameterBlock(FileBytes)
        End Sub

        Public Property BootStrapCode() As Byte()
            Get
                Return GetBootStrapCode(GetBootStrapOffset())
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

        Public ReadOnly Property BPB As BiosParameterBlock
            Get
                Return _BPB
            End Get
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

        Public Property JmpBoot() As Byte()
            Get
                Return _FileBytes.GetBytes(BootSectorOffsets.JmpBoot, BootSectorSizes.JmpBoot)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.JmpBoot, BootSectorSizes.JmpBoot, 0)
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

        Public Function CheckJumpInstruction(CheckNOP As Boolean) As Boolean
            Return CheckJumpInstruction(JmpBoot, CheckNOP)
        End Function

        Public Function CheckJumpInstruction(CheckNOP As Boolean, CheckDestination As Boolean) As Boolean
            Return CheckJumpInstruction(JmpBoot, CheckNOP, CheckDestination)
        End Function

        Public Shared Function CheckJumpInstruction(Jmp() As Byte, CheckNOP As Boolean) As Boolean
            Return CheckJumpInstruction(Jmp, CheckNOP, False)
        End Function

        Public Shared Function CheckJumpInstruction(Jmp() As Byte, CheckNOP As Boolean, CheckDestination As Boolean) As Boolean
            Dim Result As Boolean = False

            If Jmp(0) = &HEB And (Not CheckNOP Or Jmp(2) = &H90) Then
                If CheckDestination Then
                    Dim Offset As UShort = Jmp(1) + 2
                    Result = (Offset < BootSectorOffsets.BootStrapSignature)
                Else
                    Result = True
                End If
            ElseIf Jmp(0) = &HE9 Then
                If CheckDestination Then
                    Dim Offset As UShort = BitConverter.ToUInt16(Jmp, 1) + 3
                    Result = (Offset < BootSectorOffsets.BootStrapSignature)
                Else
                    Result = True
                End If
            End If

            Return Result
        End Function

        Public Shared Function GenerateVolumeSerialNumber(Value As Date) As UInteger
            Dim Lo As UShort = (Value.Day + Value.Month * 256) + (Value.Millisecond \ 10 + Value.Second * 256)
            Dim Hi As UShort = (Value.Minute + Value.Hour * 256) + Value.Year

            Return Hi + Lo * 65536
        End Function

        Public Function GetBootStrapCode(Jmp() As Byte) As Byte()
            Dim Offset = GetBootStrapOffset(Jmp)

            Return GetBootStrapCode(Offset)
        End Function

        Public Function GetBootStrapCode(Offset As UShort) As Byte()
            Dim BootStrapLength = BootSectorOffsets.BootStrapSignature - Offset
            If Offset > 2 And BootStrapLength > 0 Then
                Return _FileBytes.GetBytes(Offset, BootStrapLength)
            Else
                Return {}
            End If
        End Function

        Public Shared Function GetBootStrapOffset(Jmp() As Byte) As UShort
            Dim Offset As UShort

            If Jmp(0) = &HEB Then
                Offset = Jmp(1) + 2
            ElseIf Jmp(0) = &HE9 Then
                Offset = BitConverter.ToUInt16(Jmp, 1) + 3
            Else
                Offset = 0
            End If

            If Offset >= BootSectorOffsets.BootStrapSignature Then
                Offset = 0
            End If

            Return Offset
        End Function

        Public Function GetBootStrapOffset() As UShort
            Return GetBootStrapOffset(JmpBoot)
        End Function

        Public Function GetFileSystemTypeString() As String
            Return CodePage437ToUnicode(FileSystemType)
        End Function

        Public Function GetOEMNameString() As String
            Return CodePage437ToUnicode(OEMName)
        End Function

        Public Function GetVolumeLabelString() As String
            Return CodePage437ToUnicode(VolumeLabel)
        End Function

        Public Function HasValidBootStrapSignature() As Boolean
            Return ValidBootStrapSignature.Contains(BootStrapSignature)
        End Function

        Public Function HasValidDriveNumber() As Boolean
            Return ValidDriveNumber.Contains(DriveNumber)
        End Function

        Public Function HasValidExtendedBootSignature() As Boolean
            Return ValidExtendedBootSignature.Contains(ExtendedBootSignature)
        End Function

        Public Function IsBootSectorRegion(Offset As UInteger) As Boolean
            Return Disk.OffsetToSector(Offset) = BOOT_SECTOR
        End Function

        Public Function IsValidImage() As Boolean
            Return _FileBytes.Length >= 512 AndAlso _BPB.IsValid
        End Function

        Public Function IsWin9xOEMName() As Boolean
            Dim OEMNameLocal = OEMName

            Return OEMNameLocal(5) = &H49 And OEMNameLocal(6) = &H48 And OEMNameLocal(7) = &H43
        End Function
    End Class

End Namespace