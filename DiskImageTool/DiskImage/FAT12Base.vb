Namespace DiskImage

    Public Class FAT12Base
        Public Const FAT_BAD_CLUSTER As UShort = &HFF7
        Public Const FAT_FREE_CLUSTER As UShort = &H0
        Public Const FAT_LAST_CLUSTER_END As UShort = &HFFF
        Public Const FAT_LAST_CLUSTER_START As UShort = &HFF8
        Public Const FAT_RESERVED_END As UShort = &HFF6
        Public Const FAT_RESERVED_START As UShort = &HFF0
        Public Shared ReadOnly ValidMediaDescriptor() As Byte = {&HF0, &HF8, &HF9, &HFA, &HFB, &HFC, &HFD, &HFE, &HFF}

        Private ReadOnly _FileBytes As ImageByteArray
        Private ReadOnly _Index As UShort
        Private _Offset As UInteger
        Private _Size As UInteger
        Private _Entries As UInteger
        Private _FATTable() As UShort

        Public Sub New(FileBytes As ImageByteArray, BPB As BiosParameterBlock, Index As UShort)
            _FileBytes = FileBytes
            _Index = Index

            InitializeFAT(BPB)
        End Sub

        Public Sub InitializeFAT(BPB As BiosParameterBlock)
            If BPB.IsValid Then
                _Offset = (BPB.FATRegionStart + (BPB.SectorsPerFAT * _Index)) * Disk.BYTES_PER_SECTOR
                _Size = BPB.SectorsPerFAT * Disk.BYTES_PER_SECTOR
                _Entries = BPB.NumberOfFATEntries + 2

                _FATTable = DecodeFAT12(Data, _Entries)
            Else
                _Offset = 0
                _Size = 0
                _Entries = 1
                _FATTable = New UShort(1) {}
            End If
        End Sub

        Public ReadOnly Property Data As Byte()
            Get
                Return _FileBytes.GetBytes(_Offset, _Size)
            End Get
        End Property

        Public ReadOnly Property HasMediaDescriptor As Boolean
            Get
                Return _FileBytes.GetBytesShort(_Offset + 1) = &HFFFF
            End Get
        End Property


        Public Property MediaDescriptor As Byte
            Get
                Return _FileBytes.GetByte(_Offset)
            End Get
            Set(value As Byte)
                _FileBytes.SetBytes(value, _Offset)
            End Set
        End Property

        Public Property TableEntry(Cluster As UShort) As UShort
            Get
                Return _FATTable(Cluster)
            End Get

            Set(value As UShort)
                If _FATTable(Cluster) <> value Then
                    _FATTable(Cluster) = value
                End If
            End Set
        End Property

        Public ReadOnly Property TableLength As UShort
            Get
                Return _Entries - 1
            End Get
        End Property

        Public Function HasValidMediaDescriptor() As Boolean
            Return ValidMediaDescriptor.Contains(MediaDescriptor)
        End Function

        Public Function UpdateFAT() As Boolean
            Dim FATBytes = EncodeFAT12(_FATTable)
            If Not Data.CompareTo(FATBytes, True) Then
                _FileBytes.SetBytes(FATBytes, _Offset)
                Return True
            End If

            Return False
        End Function

        Public Shared Function DecodeFAT12(Data() As Byte, Size As UShort) As UShort()
            Dim MaxSize As Integer = Int(Data.Length * 2 / 3)

            If MaxSize Mod 2 > 0 Then
                MaxSize -= 1
            End If

            If Size Mod 2 > 0 Then
                Size += 1
            End If

            If Size > MaxSize Then
                Size = MaxSize
            End If

            Dim FATTable(Size - 1) As UShort

            For i = 0 To Size - 1
                Dim Offset As Integer = Int(i + i / 2)
                Dim b = BitConverter.ToUInt16(Data, Offset)

                If i Mod 2 = 0 Then
                    FATTable(i) = b And &HFFF
                Else
                    FATTable(i) = b >> 4
                End If
            Next

            Return FATTable
        End Function

        Public Shared Function EncodeFAT12(FATTable() As UShort) As Byte()
            Dim Size As Integer = Math.Ceiling(FATTable.Length / 2 * 3)

            Dim FatBytes(Size - 1) As Byte

            For i = 0 To Size - 1
                Dim Offset As Integer = Int(i / 3 * 2)
                If i Mod 3 = 0 Then
                    FatBytes(i) = FATTable(Offset) And &HFF
                ElseIf i Mod 3 = 1 Then
                    FatBytes(i) = (FATTable(Offset) >> 8)
                    If Offset < FATTable.Length - 1 Then
                        FatBytes(i) = FatBytes(i) + (FATTable(Offset + 1) And &HF) * 16
                    End If
                Else
                    FatBytes(i) = FATTable(Offset) >> 4
                End If
            Next

            Return FatBytes
        End Function
    End Class
End Namespace
