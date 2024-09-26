Namespace IBM_MFM
    Public Class MFMTrack
        Private _Gap4A As Byte()
        Private _IAM As MFMAddressMark
        Private _Gap1 As Byte()
        Private _Sectors As List(Of MFMSector)
        Private ReadOnly _Bitstream As BitArray

        Public Sub New(Data() As Byte)
            _Bitstream = MyBitConverter.BytesToBits(Data)
            MFMDecode(_Bitstream)
        End Sub

        Public Sub New(Bitstream As BitArray)
            _Bitstream = Bitstream
            MFMDecode(Bitstream)
        End Sub

        Public ReadOnly Property Bitstream As BitArray
            Get
                Return _Bitstream
            End Get
        End Property

        Public ReadOnly Property Gap4A As Byte()
            Get
                Return _Gap4A
            End Get
        End Property

        Public ReadOnly Property IAM As MFMAddressMark
            Get
                Return _IAM
            End Get
        End Property

        Public ReadOnly Property Gap1 As Byte()
            Get
                Return _Gap1
            End Get
        End Property

        Public ReadOnly Property Sectors As List(Of MFMSector)
            Get
                Return _Sectors
            End Get
        End Property

        Public Function UpdateBitstream() As Boolean
            Dim Updated As Boolean = False

            For Each Sector In Sectors
                If Sector.ValidChecksum Then
                    Dim CurrentChecksum = Sector.CalculateDataChecksum
                    Dim VerifyChecksum = (Sector.DataChecksum = CurrentChecksum)
                    If Not VerifyChecksum Then
                        Sector.DataChecksum = CurrentChecksum
                        Dim Bitstream = Sector.GetDataBitstream
                        Dim DataFieldOffset = Sector.DataOffset
                        For i = 0 To Bitstream.Length - 1
                            _Bitstream(DataFieldOffset + i) = Bitstream(i)
                        Next
                        Updated = True
                    End If
                End If
            Next
            Return Updated
        End Function

        Private Function GetSectorList(BitStream As BitArray) As List(Of UInteger)
            Dim IDAMPattern = MyBitConverter.BytesToBits(IDAM_Sync_Bytes)

            Dim Start As UInteger = 0
            Dim IDFieldSyncIndex As Integer
            Dim SectorList As New List(Of UInteger)
            Do
                IDFieldSyncIndex = FindPattern(BitStream, IDAMPattern, Start)
                If IDFieldSyncIndex > -1 Then
                    SectorList.Add(IDFieldSyncIndex)
                    Start = IDFieldSyncIndex + IDAMPattern.Length
                End If
            Loop Until IDFieldSyncIndex = -1

            Return SectorList
        End Function

        Private Sub MFMDecode(Bitstream As BitArray)
            Dim Start As UInteger = 0

            _Sectors = New List(Of MFMSector)

            Start = ProcessIndexField(Bitstream, Start)

            Dim SectorList = GetSectorList(Bitstream)

            If SectorList.Count > 0 Then
                Dim IDFieldSyncIndex = SectorList.Item(0)
                Dim Offset As UInteger
                If Start = 0 Then
                    Offset = IDFieldSyncIndex Mod MFM_BYTE_SIZE
                Else
                    Offset = 0
                End If
                _Gap1 = GetGapBytes(Bitstream, Start + Offset, IDFieldSyncIndex - SYNC_NULL_LENGTH)

                ProcessSectorList(Bitstream, SectorList)
            Else
                _Gap1 = New Byte(-1) {}
            End If
        End Sub

        Private Function ProcessIndexField(BitStream As BitArray, Start As UInteger) As UInteger
            Dim IAMPattern = MyBitConverter.BytesToBits(IAM_Sync_Bytes)

            Dim IndexFieldSyncIndex = FindPattern(BitStream, IAMPattern, Start)
            If IndexFieldSyncIndex > -1 Then
                Dim Offset = IndexFieldSyncIndex Mod MFM_BYTE_SIZE
                _Gap4A = GetGapBytes(BitStream, Start + Offset, IndexFieldSyncIndex - SYNC_NULL_LENGTH)
                Start = IndexFieldSyncIndex + IAMPattern.Length
                _IAM = MFMGetByte(BitStream, Start)
                Start += MFM_BYTE_SIZE
            Else
                _Gap4A = New Byte(-1) {}
                _IAM = 0
            End If

            Return Start
        End Function

        Private Sub ProcessSectorList(BitStream As BitArray, SectorList As List(Of UInteger))
            Dim SectorIndex As UShort = 0
            Dim NextSectorOffset As UInteger
            Dim DataEnd As UInteger

            For Each SectorOffset In SectorList
                Dim Sector = New MFMSector(BitStream, SectorOffset)

                DataEnd = SectorOffset + 160 + Sector.GetSizeBytes * MFM_BYTE_SIZE

                If SectorIndex < SectorList.Count - 1 Then
                    NextSectorOffset = SectorList.Item(SectorIndex + 1)
                    If DataEnd >= NextSectorOffset Then
                        Sector.Overlaps = True
                    End If
                ElseIf DataEnd > BitStream.Length Then
                    Sector.Overlaps = True
                End If

                _Sectors.Add(Sector)

                SectorIndex += 1
            Next
        End Sub
    End Class
End Namespace
