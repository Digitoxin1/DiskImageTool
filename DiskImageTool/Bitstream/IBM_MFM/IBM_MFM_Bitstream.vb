Namespace Bitstream
    Namespace IBM_MFM
        Public Class IBM_MFM_Bitstream
            Private ReadOnly _AddressMarkIndexes As List(Of UInteger)
            Private ReadOnly _Bitstream As BitArray
            Private _DataRate As UShort
            Private _RPM As UShort

            Public Enum MFMFloppyDiskDataRate As UShort
                FloppyDiskDD = 250
                FloppyDiskHD = 500
            End Enum

            Public Enum MFMFloppyDiskRPM As UShort
                FloppyDisk35 = 300
                FloppyDisk525DD = 300
                FloppyDisk525HD = 360
            End Enum

            Public Enum MFMSectorSize As Byte
                SectorSize_128 = 0
                SectorSize_256 = 1
                SectorSize_512 = 2
                SectorSize_1024 = 3
                SectorSize_2048 = 4
                SectorSize_4096 = 5
                SectorSize_8192 = 6
                SectorSize_16384 = 7
            End Enum

            Public Sub New(Gap4A As UInteger, Gap1 As UInteger)
                _Bitstream = New BitArray(0)
                _AddressMarkIndexes = New List(Of UInteger)
                _RPM = 0
                _DataRate = 0
                If Gap4A > 0 Then
                    AddGap(Gap4A)
                    AddSync(MFM_SYNC_NULL_SIZE)
                    AddIAM()
                    AddAddressMarkIndex()
                End If
                If Gap1 > 0 Then
                    AddGap(Gap1)
                End If
            End Sub

            Public ReadOnly Property AddressMarkIndexes As List(Of UInteger)
                Get
                    Return _AddressMarkIndexes
                End Get
            End Property

            Public ReadOnly Property Bitstream As BitArray
                Get
                    Return _Bitstream
                End Get
            End Property

            Public ReadOnly Property DataRate As UShort
                Get
                    Return _DataRate
                End Get
            End Property

            Public ReadOnly Property RPM As UShort
                Get
                    Return _RPM
                End Get
            End Property

            Public Sub AddData(Data() As Byte, Gap3 As UInteger)
                AddSync(MFM_SYNC_NULL_SIZE)
                Dim Start = _Bitstream.Length
                AddMFM(MFMAddressMark.Data)
                AppendBytes(Data)
                Dim Checksum = CalculateChecksum(Start, _Bitstream.Length - Start)
                AppendBytes(BitConverter.GetBytes(Checksum))
                If Gap3 > 0 Then
                    AddGap(Gap3)
                End If
            End Sub

            Public Sub AddSectorId(Cylinder As Byte, Head As Byte, SectorId As Byte, Size As MFMSectorSize)
                AddSync(MFM_SYNC_NULL_SIZE)
                Dim Start = _Bitstream.Length
                AddMFM(MFMAddressMark.ID)
                AddAddressMarkIndex()
                AppendByte(Cylinder)
                AppendByte(Head)
                AppendByte(SectorId)
                AppendByte(Size)
                Dim Checksum = CalculateChecksum(Start, _Bitstream.Length - Start)
                AppendBytes(BitConverter.GetBytes(Checksum))
                AddGap(MFM_GAP2_SIZE)
            End Sub

            Public Function CalculateChecksum(Start As UInteger, Length As UInteger) As UShort
                Dim Buffer = MFMGetBytes(_Bitstream, Start, Length \ 16)

                Return MFMCRC16(Buffer)
            End Function

            Public Function Finish(RPM As UShort, DataRate As UShort) As Boolean
                _RPM = RPM
                _DataRate = DataRate

                Dim MaxSize = MFMGetSize(RPM, DataRate)
                Dim Diff = MaxSize - _Bitstream.Length
                If Diff > 0 Then
                    AddGap(Diff \ 16)
                End If

                Return (Diff >= 0)
            End Function

            Public Function GetBytes() As Byte()
                Return BitsToBytes(_Bitstream)
            End Function

            Private Sub AddAddressMarkIndex()
                _AddressMarkIndexes.Add(_Bitstream.Length)
            End Sub

            Private Sub AddGap(Length As UInteger)
                Dim Buffer = FillBytes(MFM_GAP_BYTE, Length)
                AppendBytes(Buffer)
            End Sub

            Private Sub AddIAM()
                Dim Newbits = BytesToBits(MFM_IAM_Sync_Bytes)
                AppendBits(Newbits)
                AppendByte(MFMAddressMark.Index)
            End Sub

            Private Sub AddMFM(AddressMArk As MFMAddressMark)
                Dim Newbits = BytesToBits(MFM_Sync_Bytes)
                AppendBits(Newbits)
                AppendByte(AddressMArk)
            End Sub

            Private Sub AddSync(Length As UInteger)
                Dim Buffer = FillBytes(0, Length)
                AppendBytes(Buffer)
            End Sub

            Private Sub AppendBits(NewBits As BitArray)
                Dim Start = _Bitstream.Length
                _Bitstream.Length += NewBits.Length

                For i = 0 To NewBits.Length - 1
                    _Bitstream(Start + i) = NewBits(i)
                Next
            End Sub

            Private Sub AppendByte(Value As Byte)
                Dim Buffer = New Byte(0) {Value}
                AppendBytes(Buffer)
            End Sub

            Private Sub AppendBytes(Buffer() As Byte)
                Dim EncodedBits = MFMEncodeBytes(Buffer, GetLastBit)
                AppendBits(EncodedBits)
            End Sub

            Private Function BitsToBytes(Bitstream As BitArray) As Byte()
                Dim Length = Bitstream.Length \ 8
                Dim PaddedLength = Math.Ceiling(Length / 256) * 256

                Dim buffer(Length - 1) As Byte

                Bitstream.CopyTo(buffer, 0)

                For i As Integer = 0 To buffer.Length - 1
                    buffer(i) = ReverseBits(buffer(i))
                Next

                Return buffer
            End Function
            Private Function FillBytes(Value As Byte, Length As UInteger) As Byte()
                Dim buffer(Length - 1) As Byte
                For i = 0 To Length - 1
                    buffer(i) = Value
                Next

                Return buffer
            End Function

            Private Function GetLastBit() As Boolean
                If _Bitstream.Length = 0 Then
                    Return False
                Else
                    Return _Bitstream.Get(_Bitstream.Length - 1)
                End If
            End Function
        End Class
    End Namespace
End Namespace