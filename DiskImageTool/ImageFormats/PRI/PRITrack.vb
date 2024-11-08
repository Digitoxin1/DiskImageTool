Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace PRI
        Public Structure AltBitClock
            Dim BitOffset As UInteger
            Dim NewBitClock As UInteger
        End Structure

        Public Class PRITrack
            Implements IBitstreamTrack

            Private ReadOnly _ChunkData() As Byte
            Private _AltBitClockList As List(Of AltBitClock)
            Private _Bitstream As BitArray
            Private _SurfaceData As BitArray

            Public Sub New()
                _ChunkData = New Byte(15) {}
                Initialize()
            End Sub

            Public Sub New(ChunkData() As Byte)
                _ChunkData = ChunkData
                Initialize()
            End Sub

            Public ReadOnly Property AltBitClockList As List(Of AltBitClock)
                Get
                    Return _AltBitClockList
                End Get
            End Property

            Public Property BitClockRate As UInteger
                Get
                    Return MyBitConverter.ToUInt32(_ChunkData, True, 12)
                End Get
                Set(value As UInteger)
                    Dim Buffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(value))
                    Array.Copy(Buffer, 0, _ChunkData, 12, 4)
                End Set
            End Property

            Public ReadOnly Property BitRate As UShort Implements IBitstreamTrack.BitRate
                Get
                    Return BitClockRate / 2000
                End Get
            End Property

            Public Property Bitstream As BitArray Implements IBitstreamTrack.Bitstream
                Get
                    Return _Bitstream
                End Get
                Set(value As BitArray)
                    _Bitstream = value
                End Set
            End Property

            Public ReadOnly Property Decoded As Boolean Implements IBitstreamTrack.Decoded
                Get
                    Return _MFMData IsNot Nothing AndAlso _MFMData.Sectors.Count > 0
                End Get
            End Property

            Public Property Length As UInteger
                Get
                    Return MyBitConverter.ToUInt32(_ChunkData, True, 8)
                End Get
                Set(value As UInteger)
                    Dim Buffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(value))
                    Array.Copy(Buffer, 0, _ChunkData, 8, 4)
                End Set
            End Property

            Public Property MFMData As IBM_MFM.IBM_MFM_Track Implements IBitstreamTrack.MFMData

            Public ReadOnly Property RPM As UShort Implements IBitstreamTrack.RPM
                Get
                    Return InferRPM(_Bitstream.Length)
                End Get
            End Property

            Public Property Side As UInteger
                Get
                    Return MyBitConverter.ToUInt32(_ChunkData, True, 4)
                End Get
                Set(value As UInteger)
                    Dim Buffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(value))
                    Array.Copy(Buffer, 0, _ChunkData, 4, 4)
                End Set
            End Property

            Public ReadOnly Property SurfaceData As BitArray Implements IBitstreamTrack.SurfaceData
                Get
                    Return _SurfaceData
                End Get
            End Property

            Public Property Track As UInteger
                Get
                    Return MyBitConverter.ToUInt32(_ChunkData, True, 0)
                End Get
                Set(value As UInteger)
                    Dim Buffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(value))
                    Array.Copy(Buffer, 0, _ChunkData, 0, 4)
                End Set
            End Property

            Public ReadOnly Property TrackType As BitstreamTrackType Implements IBitstreamTrack.TrackType
                Get
                    If _MFMData IsNot Nothing AndAlso _MFMData.Sectors.Count > 0 Then
                        Return BitstreamTrackType.MFM
                    Else
                        Return BitstreamTrackType.Other
                    End If
                End Get
            End Property

            Private ReadOnly Property IBitstreamTrack_Side As Byte Implements IBitstreamTrack.Side
                Get
                    Return Side
                End Get
            End Property

            Private ReadOnly Property IBitstreamTrack_Track As UShort Implements IBitstreamTrack.Track
                Get
                    Return Track
                End Get
            End Property

            Public Function AddAltBitClock(BitOffset As UInteger, NewBitClock As UInteger) As AltBitClock
                Dim AltBitClock As AltBitClock
                AltBitClock.BitOffset = BitOffset
                AltBitClock.NewBitClock = NewBitClock

                _AltBitClockList.Add(AltBitClock)

                Return AltBitClock
            End Function

            Public Sub AddWeakBits(BitOffset As UInteger, WeakBitMask As UInteger)
                If _SurfaceData Is Nothing Then
                    _SurfaceData = New BitArray(Length)
                End If

                Dim Buffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(WeakBitMask))
                Dim WeakBits = IBM_MFM.BytesToBits(Buffer)
                For i = 0 To WeakBits.Length - 1
                    _SurfaceData(BitOffset + i) = WeakBits(i)
                Next
            End Sub

            Private Sub Initialize()
                _AltBitClockList = New List(Of AltBitClock)
                _Bitstream = New BitArray(Length)
                _MFMData = Nothing
                _SurfaceData = Nothing
            End Sub
        End Class
    End Namespace
End Namespace
