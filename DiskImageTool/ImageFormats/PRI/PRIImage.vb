Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace PRI
        Public Class PRIImage
            Inherits BitStreamImageBase
            Implements IBitstreamImage

            Private _BitRate As UShort
            Private _CurrentTrack As PRITrack
            Private _HasWeakBits As Boolean
            Private _SideCount As Byte = 0
            Private _TrackCount As UShort = 0
            Private _VariableBitRate As Boolean
            Public Sub New()
                Initialize()
            End Sub

            Public Overloads ReadOnly Property BitRate As UShort Implements IBitstreamImage.BitRate
                Get
                    Return _BitRate
                End Get
            End Property

            Public Property Comment As String

            Public ReadOnly Property HasWeakBits As Boolean Implements IBitstreamImage.HasSurfaceData
                Get
                    Return _HasWeakBits
                End Get
            End Property

            Public Property Header As PRIFileHeader

            Public Overloads ReadOnly Property SideCount As Byte Implements IBitstreamImage.SideCount
                Get
                    Return _SideCount
                End Get
            End Property

            Public Overloads ReadOnly Property TrackCount As UShort Implements IBitstreamImage.TrackCount
                Get
                    Return _TrackCount
                End Get
            End Property

            Public Property Tracks As Dictionary(Of String, PRITrack)

            Public Overloads ReadOnly Property VariableBitRate As Boolean Implements IBitstreamImage.VariableBitRate
                Get
                    Return _VariableBitRate
                End Get
            End Property

            Public Overrides Function Export(FilePath As String) As Boolean Implements IBitstreamImage.Export
                Return False
            End Function

            Public Function GetTrack(Track As Byte, Side As Byte) As PRITrack
                If Track > _TrackCount - 1 Or Side > _SideCount - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Key = Track & "." & Side

                If _Tracks.ContainsKey(Key) Then
                    Return _Tracks.Item(Key)
                Else
                    Return Nothing
                End If
            End Function
            Public Overrides Function Load(FilePath As String) As Boolean Implements IBitstreamImage.Load
                Return Load(IO.File.ReadAllBytes(FilePath))
            End Function

            Public Overrides Function Load(Buffer() As Byte) As Boolean Implements IBitstreamImage.Load
                Dim Result As Boolean = False
                Dim Chunk As PRIChunk
                Dim ChunkCount As UInteger = 0
                Dim Response As ReadResponse

                Initialize()

                Response.Offset = 0

                Try
                    Do While Response.Offset + 8 < Buffer.Length
                        Chunk = New PRIChunk()
                        Response = Chunk.ReadFromBuffer(Buffer, Response.Offset)
                        If Response.ChecksumVerified Then
                            If ChunkCount = 0 AndAlso Chunk.ChunkID <> "PRI " Then
                                Exit Do
                            ElseIf Chunk.ChunkID = "END " Then
                                Result = True
                                Exit Do
                            Else
                                ProcessChunk(Chunk)
                            End If
                        Else
                            Exit Do
                        End If
                        ChunkCount += 1
                    Loop
                Catch ex As Exception
                    DebugException(ex)
                    Return False
                End Try

                Return Result
            End Function

            Public Sub SetTrack(Track As Byte, Side As Byte, Value As PRITrack)
                If Track > _TrackCount - 1 Or Side > _SideCount - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Key = Track & "." & Side

                _Tracks.Item(Key) = Value
            End Sub
            Private Function IBitstreamImage_GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack Implements IBitstreamImage.GetTrack
                Return GetTrack(Track, Side)
            End Function

            Private Sub Initialize()
                _Header = New PRIFileHeader
                _Tracks = New Dictionary(Of String, PRITrack)
                _CurrentTrack = Nothing
                _Comment = ""
                _BitRate = 0
                _VariableBitRate = False
                _HasWeakBits = False
            End Sub


            Private Sub ProcessChunk(Chunk As PRIChunk)
                If Chunk.ChunkID = "PRI " Then
                    _Header = New PRIFileHeader(Chunk.ChunkData)

                ElseIf Chunk.ChunkID = "TEXT" Then
                    _Comment &= Text.Encoding.UTF8.GetString(Chunk.ChunkData)

                ElseIf Chunk.ChunkID = "TRAK" Then
                    Dim Track = New PRITrack(Chunk.ChunkData)
                    Dim Key = Track.Track & "." & Track.Side
                    _Tracks.Item(Key) = Track
                    _CurrentTrack = Track
                    If Track.Track > _TrackCount - 1 Then
                        _TrackCount = Track.Track + 1
                    End If
                    If Track.Side > _SideCount - 1 Then
                        _SideCount = Track.Side + 1
                    End If

                ElseIf Chunk.ChunkID = "DATA" Then
                    If _CurrentTrack IsNot Nothing Then
                        _CurrentTrack.Bitstream = IBM_MFM.BytesToBits(Chunk.ChunkData, 0, _CurrentTrack.Length)
                        _CurrentTrack.MFMData = New IBM_MFM.IBM_MFM_Track(_CurrentTrack.Bitstream)
                        If _CurrentTrack.TrackType = BitstreamTrackType.MFM Then
                            If Not _VariableBitRate Then
                                If _BitRate = 0 Then
                                    _BitRate = _CurrentTrack.BitRate
                                ElseIf _BitRate <> _CurrentTrack.BitRate Then
                                    _BitRate = 0
                                    _VariableBitRate = True
                                End If
                            End If
                        End If
                    End If

                ElseIf Chunk.ChunkID = "WEAK" Then
                    If _CurrentTrack IsNot Nothing Then
                        _HasWeakBits = True
                        For Offset As UInteger = 0 To Chunk.ChunkSize - 1 Step 8
                            Dim BitOffset = MyBitConverter.ToUInt32(Chunk.ChunkData, True, Offset)
                            Dim WeakBitMask = MyBitConverter.ToUInt32(Chunk.ChunkData, True, Offset + 4)
                            _CurrentTrack.AddWeakBits(BitOffset, WeakBitMask)
                        Next
                    End If

                ElseIf Chunk.ChunkID = "BCLK" Then
                    If _CurrentTrack IsNot Nothing Then
                        For Offset As UInteger = 0 To Chunk.ChunkSize - 1 Step 8
                            Dim BitOffset = MyBitConverter.ToUInt32(Chunk.ChunkData, True, Offset)
                            Dim NewBitClock = MyBitConverter.ToUInt32(Chunk.ChunkData, True, Offset + 4)
                            _CurrentTrack.AddAltBitClock(BitOffset, NewBitClock)
                        Next
                    End If
                End If
            End Sub
        End Class
    End Namespace
End Namespace
