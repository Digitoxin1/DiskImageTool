Namespace Bitstream
    Namespace IBM_MFM
        Public Class IBM_MFM_Track
            Private _AddressMarkIndexes As List(Of UInteger)
            Private _DuplicateSectors As Boolean
            Private _FirstSectorId As Short
            Private _LastSectorId As Short
            Private _OverlappingSectors As Boolean
            Private _Sectors As List(Of IBM_MFM_Sector)
            Private _SectorSize As Integer
            Private _SectorStartIndex As UInteger
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

            Public ReadOnly Property FirstSectorId As Short
                Get
                    Return _FirstSectorId
                End Get
            End Property

            Public ReadOnly Property LastSectorId As Short
                Get
                    Return _LastSectorId
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

            Public ReadOnly Property SectorStartIndex As UInteger
                Get
                    Return _SectorStartIndex
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
                _FirstSectorId = -1
                _LastSectorId = -1
                _SectorSize = -1
                _SectorStartIndex = 0
                _OverlappingSectors = False
                _DuplicateSectors = False

                ProcessIndexField(Bitstream, Start)

                Dim IDAMPattern = BytesToBits(MFM_IDAM_SYNC_PATTERN_BYTES)
                Dim SectorList = MFMGetSectorList(Bitstream, IDAMPattern)

                If SectorList.Count > 0 Then
                    ProcessSectorList(Bitstream, SectorList)
                End If
            End Sub

            Private Function ProcessIndexField(BitStream As BitArray, Start As UInteger) As UInteger
                Dim IAMPattern = BytesToBits(MFM_IAM_SYNC_PATTERN_BYTES)

                Dim IndexFieldSyncIndex = FindPattern(BitStream, IAMPattern, Start)
                If IndexFieldSyncIndex > -1 Then
                    AddAddressMarkIndex(IndexFieldSyncIndex + MFM_SYNC_MARK_BITS)
                End If

                Return Start
            End Function

            Private Sub ProcessSectorList(BitStream As BitArray, SectorList As List(Of UInteger))
                Dim SectorIndex As UShort = 0
                Dim SectorIds As New HashSet(Of Byte)
                Dim SectorStartIndex As Integer = -1
                Dim HasSequentialIds As Boolean = True
                Dim SequenceWrapped As Boolean = False
                Dim CurrentSectorId As Short = -1
                Dim SectorIdStart As Short = -1

                For Each SectorOffset In SectorList
                    AddAddressMarkIndex(SectorOffset + MFM_SYNC_MARK_BITS)

                    Dim Sector As New IBM_MFM_Sector(BitStream, SectorOffset)
                    Dim SectorSize = Sector.GetSizeBytes

                    If SectorIndex < SectorList.Count - 1 Then
                        Dim NextSectorOffset = SectorList.Item(SectorIndex + 1)
                        If Sector.OffsetEnd >= NextSectorOffset Or Sector.OffsetEnd < Sector.Offset Then
                            Sector.Overlaps = True
                            _OverlappingSectors = True
                        End If
                    ElseIf Sector.OffsetEnd < Sector.Offset Then
                        Sector.Overlaps = True
                        _OverlappingSectors = True
                    End If

                    _Sectors.Add(Sector)

                    If _FirstSectorId = -1 Then
                        _FirstSectorId = Sector.SectorId
                        _SectorSize = SectorSize
                    Else
                        If Sector.SectorId < _FirstSectorId Then
                            _FirstSectorId = Sector.SectorId
                        End If

                        If SectorSize <> _SectorSize Then
                            _SectorSize = -1
                        End If
                    End If

                    If _LastSectorId = -1 Then
                        _LastSectorId = Sector.SectorId
                    ElseIf Sector.SectorId > _LastSectorId Then
                        _LastSectorId = Sector.SectorId
                    End If

                    If Not SectorIds.Contains(Sector.SectorId) Then
                        SectorIds.Add(Sector.SectorId)
                    Else
                        _DuplicateSectors = True
                    End If

                    If SectorStartIndex = -1 AndAlso Sector.SectorId < 2 Then
                        SectorStartIndex = SectorIndex
                    End If

                    If HasSequentialIds AndAlso CurrentSectorId <> -1 Then
                        If Not SequenceWrapped AndAlso Sector.SectorId < SectorIdStart Then
                            SequenceWrapped = True
                        ElseIf Sector.SectorId <> CurrentSectorId + 1 Then
                            HasSequentialIds = False
                        End If
                    Else
                        SectorIdStart = Sector.SectorId
                    End If
                    CurrentSectorId = Sector.SectorId

                    SectorIndex += 1
                Next

                If SectorStartIndex = -1 Then
                    SectorStartIndex = 0
                End If

                If HasSequentialIds Then
                    _SectorStartIndex = SectorStartIndex
                End If
            End Sub
        End Class
    End Namespace
End Namespace
