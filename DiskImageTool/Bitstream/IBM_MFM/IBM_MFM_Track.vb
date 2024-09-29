
Namespace Bitstream
    Namespace IBM_MFM
        Public Class IBM_MFM_Track
            Private _AddressMarkIndexes As List(Of UInteger)
            Private _Gap4A As Byte()
            Private _IAM As MFMAddressMark
            Private _Gap1 As Byte()
            Private _Sectors As List(Of IBM_MFM_Sector)
            Private _Size As UInteger

            Public Sub New(Data() As Byte)
                MFMDecode(BytesToBits(Data))
            End Sub

            Public Sub New(Bitstream As BitArray)
                MFMDecode(Bitstream)
            End Sub

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

            Public ReadOnly Property Sectors As List(Of IBM_MFM_Sector)
                Get
                    Return _Sectors
                End Get
            End Property

            Public ReadOnly Property AddressMarkIndexes As List(Of UInteger)
                Get
                    Return _AddressMarkIndexes
                End Get
            End Property

            Public Function GetTrackFormat() As MFMTrackFormat
                Dim Size = Math.Round(_Size / 10000) * 10000
                If Size = 100000 Then
                    Return MFMTrackFormat.TrackFormatDD
                ElseIf Size = 170000 Then
                    Return MFMTrackFormat.TrackFormatHD1200
                ElseIf Size = 200000 Then
                    Return MFMTrackFormat.TrackFormatHD
                ElseIf Size = 400000 Then
                    Return MFMTrackFormat.TrackFormatED
                Else
                    Return MFMTrackFormat.TrackFormatUnknown
                End If
            End Function

            Private Sub AddAddressMarkIndex(Offset As UInteger)
                _AddressMarkIndexes.Add(Offset)
            End Sub

            Private Function GetSectorList(BitStream As BitArray) As List(Of UInteger)
                Dim IDAMPattern = BytesToBits(IDAM_Sync_Bytes)

                Dim Start As UInteger = 0
                Dim IDFieldSyncIndex As Integer
                Dim SectorList As New List(Of UInteger)
                Do
                    IDFieldSyncIndex = FindPattern(BitStream, IDAMPattern, Start)
                    If IDFieldSyncIndex > -1 Then
                        SectorList.Add(IDFieldSyncIndex)
                        Start = IDFieldSyncIndex + IDAMPattern.Length
                        AddAddressMarkIndex(Start)
                    End If
                Loop Until IDFieldSyncIndex = -1

                Return SectorList
            End Function

            Private Sub MFMDecode(Bitstream As BitArray)
                Dim Start As UInteger = 0

                _Size = Bitstream.Length
                _Sectors = New List(Of IBM_MFM_Sector)
                _AddressMarkIndexes = New List(Of UInteger)

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
                    _Gap1 = GetGapBytes(Bitstream, Start + Offset, IDFieldSyncIndex - MFM_SYNC_SIZE_BITS)

                    ProcessSectorList(Bitstream, SectorList)
                Else
                    _Gap1 = New Byte(-1) {}
                End If
            End Sub

            Private Function ProcessIndexField(BitStream As BitArray, Start As UInteger) As UInteger
                Dim IAMPattern = BytesToBits(IAM_Sync_Bytes)

                Dim IndexFieldSyncIndex = FindPattern(BitStream, IAMPattern, Start)
                If IndexFieldSyncIndex > -1 Then
                    Dim Offset = IndexFieldSyncIndex Mod MFM_BYTE_SIZE
                    _Gap4A = GetGapBytes(BitStream, Start + Offset, IndexFieldSyncIndex - MFM_SYNC_SIZE_BITS)
                    Start = IndexFieldSyncIndex + IAMPattern.Length
                    _IAM = MFMGetByte(BitStream, Start)
                    Start += MFM_BYTE_SIZE
                    AddAddressMarkIndex(Start)
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
                    Dim Sector = New IBM_MFM_Sector(BitStream, SectorOffset)

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
End Namespace
