Namespace DiskImage

    Public Class BootSector
        Public Const BOOT_SECTOR_SIZE As UShort = 512
        Public Const BOOT_SECTOR_OFFSET As UInteger = 0
        Public Shared ReadOnly ValidBootStrapSignature() As UShort = {&HAA55, &H0, &H4254}
        Public Shared ReadOnly ValidDriveNumber() As Byte = {&H0, &H80}
        Public Shared ReadOnly ValidExtendedBootSignature() As Byte = {&H28, &H29}
        Public Shared ReadOnly ValidJumpInstructuon() As Byte = {&HEB, &HE9}
        Private ReadOnly _BPB As BiosParameterBlock
        Private ReadOnly _FileBytes As ByteArray
        Private ReadOnly _Offset As UInteger
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

        Sub New(buffer() As Byte)
            _FileBytes = New ByteArray(buffer)
            _Offset = 0
            _BPB = New BiosParameterBlock(_FileBytes, _Offset)
        End Sub

        Sub New(FileBytes As ByteArray, Offset As UInteger)
            _FileBytes = FileBytes
            _Offset = Offset
            _BPB = New BiosParameterBlock(FileBytes, _Offset)
        End Sub

        Public Property BootStrapCode() As Byte()
            Get
                Return GetBootStrapCode(GetBootStrapOffset())
            End Get
            Set
                Dim BootStrapStart = GetBootStrapOffset()
                Dim BootStrapLength = BootSectorOffsets.BootStrapSignature - BootStrapStart
                If BootStrapStart > 2 And BootStrapLength > 0 Then
                    _FileBytes.SetBytes(Value, BootStrapStart + _Offset, BootSectorOffsets.BootStrapSignature - BootStrapStart, 0)
                End If
            End Set
        End Property

        Public Property BootStrapSignature() As UShort
            Get
                Return _FileBytes.GetBytesShort(BootSectorOffsets.BootStrapSignature + _Offset)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.BootStrapSignature + _Offset)
            End Set
        End Property

        Public ReadOnly Property BPB As BiosParameterBlock
            Get
                Return _BPB
            End Get
        End Property

        Public Property Data As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset, BOOT_SECTOR_SIZE)
            End Get
            Set(value As Byte())
                _FileBytes.SetBytes(value, _Offset, BOOT_SECTOR_SIZE, 0)
            End Set
        End Property

        Public Property DriveNumber() As Byte
            Get
                Return _FileBytes.GetByte(BootSectorOffsets.DriveNumber + _Offset)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.DriveNumber + _Offset)
            End Set
        End Property

        Public Property ExtendedBootSignature() As Byte
            Get
                Return _FileBytes.GetByte(BootSectorOffsets.ExtendedBootSignature + _Offset)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.ExtendedBootSignature + _Offset)
            End Set
        End Property

        Public Property FileSystemType() As Byte()
            Get
                Return _FileBytes.GetBytes(BootSectorOffsets.FileSystemType + _Offset, BootSectorSizes.FileSystemType)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.FileSystemType + _Offset, BootSectorSizes.FileSystemType, 0)
            End Set
        End Property

        Public Property JmpBoot() As Byte()
            Get
                Return _FileBytes.GetBytes(BootSectorOffsets.JmpBoot + _Offset, BootSectorSizes.JmpBoot)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.JmpBoot + _Offset, BootSectorSizes.JmpBoot, 0)
            End Set
        End Property

        Public Property OEMName() As Byte()
            Get
                Return _FileBytes.GetBytes(BootSectorOffsets.OEMName + _Offset, BootSectorSizes.OEMName)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.OEMName + _Offset, BootSectorSizes.OEMName, 0)
            End Set
        End Property

        Public Property Reserved() As Byte
            Get
                Return _FileBytes.GetByte(BootSectorOffsets.Reserved + _Offset)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.Reserved + _Offset)
            End Set
        End Property

        Public Property VolumeLabel() As Byte()
            Get
                Return _FileBytes.GetBytes(BootSectorOffsets.VolumeLabel + _Offset, BootSectorSizes.VolumeLabel)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.VolumeLabel + _Offset, BootSectorSizes.VolumeLabel, 0)
            End Set
        End Property

        Public Property VolumeSerialNumber() As UInteger
            Get
                Return _FileBytes.GetBytesInteger(BootSectorOffsets.VolumeSerialNumber + _Offset)
            End Get
            Set
                _FileBytes.SetBytes(Value, BootSectorOffsets.VolumeSerialNumber + _Offset)
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

        Private Function GetBootStrapCode(Offset As UShort) As Byte()
            Dim BootStrapLength = BootSectorOffsets.BootStrapSignature - Offset
            If Offset > 2 And BootStrapLength > 0 Then
                Return _FileBytes.GetBytes(Offset + _Offset, BootStrapLength)
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

        Public Function IsWin9xOEMName() As Boolean
            Dim OEMNameLocal = OEMName

            Return OEMNameLocal(5) = &H49 And OEMNameLocal(6) = &H48 And OEMNameLocal(7) = &H43
        End Function
    End Class

End Namespace