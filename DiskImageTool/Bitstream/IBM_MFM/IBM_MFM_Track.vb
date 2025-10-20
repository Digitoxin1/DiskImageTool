Namespace Bitstream
    Namespace IBM_MFM
        Public Class IBM_MFM_Track
            Private _AddressMarkIndexes As List(Of UInteger)
            Private _DuplicateSectors As Boolean
            Private _FirstSector As Integer
            Private _Gap1 As Byte()
            Private _Gap4A As Byte()
            Private _IAM As MFMAddressMark
            Private _LastSector As Integer
            Private _OverlappingSectors As Boolean
            Private _Sectors As List(Of IBM_MFM_Sector)
            Private _SectorSize As Integer
            Private _SectorStart As UInteger
            Private _Size As UInteger

            Public Sub New(Data() As Byte)
                MFMDecode(BytesToBits(Data))
            End Sub

            Public Sub New(Bitstream As BitArray)
                MFMDecode(Bitstream)
            End Sub

            Public ReadOnly Property AddressMarkIndexes As List(Of UInteger)
                Get
                    Return _AddressMarkIndexes
                End Get
            End Property

            Public ReadOnly Property DuplicateSectors As Boolean
                Get
                    Return _DuplicateSectors
                End Get
            End Property

            Public ReadOnly Property FirstSector As Integer
                Get
                    Return _FirstSector
                End Get
            End Property

            Public ReadOnly Property Gap1 As Byte()
                Get
                    Return _Gap1
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

            Public ReadOnly Property LastSector As Integer
                Get
                    Return _LastSector
                End Get
            End Property

            Public ReadOnly Property OverlappingSectors As Boolean
                Get
                    Return _OverlappingSectors
                End Get
            End Property

            Public ReadOnly Property Sectors As List(Of IBM_MFM_Sector)
                Get
                    Return _Sectors
                End Get
            End Property

            Public ReadOnly Property SectorSize As Integer
                Get
                    Return _SectorSize
                End Get
            End Property

            Public ReadOnly Property SectorStart As UInteger
                Get
                    Return _SectorStart
                End Get
            End Property

            Public ReadOnly Property Size As UInteger
                Get
                    Return _Size
                End Get
            End Property

            Private Sub AddAddressMarkIndex(Offset As UInteger)
                _AddressMarkIndexes.Add(Offset)
            End Sub

            Private Sub MFMDecode(Bitstream As BitArray)
                Dim Start As UInteger = 0

                _Size = Bitstream.Length
                _Sectors = New List(Of IBM_MFM_Sector)
                _AddressMarkIndexes = New List(Of UInteger)
                _FirstSector = -1
                _LastSector = -1
                _SectorSize = -1
                _SectorStart = 0
                _OverlappingSectors = False
                _DuplicateSectors = False

                Start = ProcessIndexField(Bitstream, Start)

                Dim IDAMPattern = BytesToBits(MFM_IDAM_SYNC_PATTERN_BYTES)
                Dim SectorList = MFMGetSectorList(Bitstream, IDAMPattern)

                If SectorList.Count > 0 Then
                    Dim IDFieldSyncIndex = SectorList.Item(0)
                    Dim Offset As UInteger
                    If Start = 0 Then
                        Offset = MFMBitOffset(IDFieldSyncIndex)
                    Else
                        Offset = 0
                    End If
                    _Gap1 = MFMGetBytesByRange(Bitstream, Start + Offset, IDFieldSyncIndex - MFM_SYNC_NULL_BITS)

                    ProcessSectorList(Bitstream, SectorList)
                Else
                    _Gap1 = New Byte(-1) {}
                End If
            End Sub

            Private Function ProcessIndexField(BitStream As BitArray, Start As UInteger) As UInteger
                Dim IAMPattern = BytesToBits(MFM_IAM_SYNC_PATTERN_BYTES)

                Dim IndexFieldSyncIndex = FindPattern(BitStream, IAMPattern, Start)
                If IndexFieldSyncIndex > -1 Then
                    Dim Offset = MFMBitOffset(IndexFieldSyncIndex)
                    _Gap4A = MFMGetBytesByRange(BitStream, Start + Offset, IndexFieldSyncIndex - MFM_SYNC_NULL_BITS)
                    Start = IndexFieldSyncIndex + IAMPattern.Length
                    _IAM = MFMGetByte(BitStream, Start)
                    Start += MFM_BYTE_BYTES
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
                Dim DataStart As UInteger
                Dim DataEnd As UInteger
                Dim SectorIds As New HashSet(Of Byte)
                Dim SectorStart As Integer = -1

                For Each SectorOffset In SectorList
                    Dim Start = SectorOffset + MFM_IDAM_SYNC_PATTERN_BYTES.Length * 8
                    AddAddressMarkIndex(Start)

                    Dim Sector = New IBM_MFM_Sector(BitStream, SectorOffset)
                    Dim SectorSize = Sector.GetSizeBytes

                    DataStart = SectorOffset + 160
                    DataEnd = DataStart + MFMBytesToBits(SectorSize)

                    If SectorIndex < SectorList.Count - 1 Then
                        NextSectorOffset = SectorList.Item(SectorIndex + 1)
                        If DataEnd >= NextSectorOffset Then
                            Sector.Overlaps = True
                            _OverlappingSectors = True
                        End If
                    ElseIf DataEnd > BitStream.Length Then
                        Sector.Overlaps = True
                        _OverlappingSectors = True
                    End If

                    _Sectors.Add(Sector)

                    If _FirstSector = -1 Then
                        _FirstSector = Sector.SectorId
                        _SectorSize = SectorSize
                    Else
                        If Sector.SectorId < _FirstSector Then
                            _FirstSector = Sector.SectorId
                        End If

                        If SectorSize <> _SectorSize Then
                            _SectorSize = -1
                        End If
                    End If

                    If _LastSector = -1 Then
                        _LastSector = Sector.SectorId
                    ElseIf Sector.SectorId > _LastSector Then
                        _LastSector = Sector.SectorId
                    End If

                    If Not SectorIds.Contains(Sector.SectorId) Then
                        SectorIds.Add(Sector.SectorId)
                    Else
                        _DuplicateSectors = True
                    End If

                    If SectorStart = -1 AndAlso Sector.SectorId < 2 Then
                        SectorStart = SectorIndex
                    End If

                    SectorIndex += 1
                Next

                If SectorStart = -1 Then
                    SectorStart = 0
                End If

                _SectorStart = SectorStart
            End Sub
        End Class
    End Namespace
End Namespace
