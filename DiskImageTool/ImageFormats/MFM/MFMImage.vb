Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace MFM
        Public Class MFMImage
            Implements IBitstreamImage

            Private Const FILE_SIGNATURE = "HXCMFM"
            Private _Tracks() As MFMTrack
            Private _TrackCount As UShort
            Private _SideCount As Byte
            Private _TrackStep As Byte

            Public Sub New()
                _TrackCount = 0
                _SideCount = 0
                _TrackStep = 1
                _RPM = 0
                _BitRate = 0
            End Sub

            Public Sub Initialize(TrackCount As UShort, SideCount As Byte, TrackStep As Byte)
                _TrackCount = TrackCount
                _SideCount = SideCount
                _TrackStep = TrackStep

                _Tracks = New MFMTrack((_TrackCount) * _SideCount - 1) {}
            End Sub

            Public Function Load(Buffer() As Byte) As Boolean
                Dim Result As Boolean

                Dim Signature = Text.Encoding.UTF8.GetString(Buffer, 0, 6)
                Result = (Signature = FILE_SIGNATURE)

                If Result Then
                    _TrackCount = BitConverter.ToUInt16(Buffer, 7)
                    _SideCount = Buffer(9)

                    _Tracks = New MFMTrack(_TrackCount * _SideCount - 1) {}

                    _RPM = BitConverter.ToUInt16(Buffer, 10)
                    _BitRate = BitConverter.ToUInt16(Buffer, 12)
                    _IFType = Buffer(14)

                    Dim TrackListOffset = BitConverter.ToUInt32(Buffer, 15)
                    For i = 0 To _TrackCount - 1
                        For j = 0 To _SideCount - 1
                            Dim Track = BitConverter.ToUInt16(Buffer, TrackListOffset)
                            Dim Side = Buffer(TrackListOffset + 2)
                            Dim MFMTrack = New MFMTrack(Track, Side)
                            Dim Size As UInteger
                            If _IFType And &H80 Then
                                MFMTrack.RPM = BitConverter.ToUInt16(Buffer, TrackListOffset + 3)
                                MFMTrack.BitRate = BitConverter.ToUInt16(Buffer, TrackListOffset + 5)
                                Size = BitConverter.ToUInt32(Buffer, TrackListOffset + 7)
                                MFMTrack.Offset = BitConverter.ToUInt32(Buffer, TrackListOffset + 11)
                            Else
                                Size = BitConverter.ToUInt32(Buffer, TrackListOffset + 3)
                                MFMTrack.Offset = BitConverter.ToUInt32(Buffer, TrackListOffset + 7)
                            End If
                            MFMTrack.Bitstream = Bitstream.IBM_MFM.BytesToBits(Buffer, MFMTrack.Offset, Size)
                            MFMTrack.MFMData = New Bitstream.IBM_MFM.IBM_MFM_Track(MFMTrack.Bitstream)
                            SetTrack(Track, Side, MFMTrack)
                            TrackListOffset += 11
                        Next j
                    Next i
                    Dim BitRate = Math.Round(_BitRate / 50) * 50
                    If BitRate = 300 And _TrackCount > 79 Then
                        _TrackStep = 2
                    End If
                End If

                Return Result
            End Function

            Public ReadOnly Property TrackCount As UShort Implements IBitstreamImage.TrackCount
                Get
                    Return _TrackCount
                End Get
            End Property

            Public ReadOnly Property SideCount As Byte Implements IBitstreamImage.SideCount
                Get
                    Return _SideCount
                End Get
            End Property

            Public ReadOnly Property TrackStep As Byte Implements IBitstreamImage.TrackStep
                Get
                    Return _TrackStep
                End Get
            End Property

            Public Property RPM As UShort
            Public Property BitRate As UShort
            Public Property IFType As Byte

            Public Function GetTrack(Track As UShort, Side As Byte) As MFMTrack
                If Track > _TrackCount - 1 Or Side > _SideCount - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Index = Track * _SideCount + Side

                Return _Tracks(Index)
            End Function

            Public Sub SetTrack(Track As UShort, Side As Byte, Value As MFMTrack)
                If Track > _TrackCount - 1 Or Side > _SideCount - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Index = Track * _SideCount + Side

                _Tracks(Index) = Value
            End Sub

            Public Function Load(FilePath As String) As Boolean
                Return Load(IO.File.ReadAllBytes(FilePath))
            End Function

            Public Function Export(FilePath As String, RefreshBitstream As Boolean) As Boolean
                Dim Buffer() As Byte

                If RefreshBitstream Then
                    UpdateBitstream()
                End If

                Try
                    If IO.File.Exists(FilePath) Then
                        IO.File.Delete(FilePath)
                    End If

                    Using fs As IO.FileStream = IO.File.OpenWrite(FilePath)
                        Buffer = Text.Encoding.UTF8.GetBytes(FILE_SIGNATURE)
                        fs.Write(Buffer, 0, Buffer.Length)

                        fs.WriteByte(0)

                        Buffer = BitConverter.GetBytes(_TrackCount)
                        fs.Write(Buffer, 0, Buffer.Length)

                        fs.WriteByte(_SideCount)

                        Buffer = BitConverter.GetBytes(_RPM)
                        fs.Write(Buffer, 0, Buffer.Length)

                        Buffer = BitConverter.GetBytes(_BitRate)
                        fs.Write(Buffer, 0, Buffer.Length)

                        fs.WriteByte(_IFType)

                        Dim TrackListOffset As UInteger = fs.Position + 4
                        Buffer = BitConverter.GetBytes(TrackListOffset)
                        fs.Write(Buffer, 0, Buffer.Length)

                        Dim OffsetPos(_TrackCount * _SideCount - 1) As UInteger
                        For i = 0 To _TrackCount - 1
                            For j = 0 To _SideCount - 1
                                Dim MFMTrack = GetTrack(i, j)

                                Buffer = BitConverter.GetBytes(MFMTrack.Track)
                                fs.Write(Buffer, 0, Buffer.Length)

                                fs.WriteByte(MFMTrack.Side)

                                If _IFType And &H80 Then
                                    Buffer = BitConverter.GetBytes(MFMTrack.RPM)
                                    fs.Write(Buffer, 0, Buffer.Length)

                                    Buffer = BitConverter.GetBytes(MFMTrack.BitRate)
                                    fs.Write(Buffer, 0, Buffer.Length)
                                End If

                                Buffer = BitConverter.GetBytes(CUInt(MFMTrack.Length))
                                fs.Write(Buffer, 0, Buffer.Length)

                                OffsetPos(i * _SideCount + j) = fs.Position
                                fs.WriteByte(0)
                                fs.WriteByte(0)
                                fs.WriteByte(0)
                                fs.WriteByte(0)
                            Next
                        Next
                        Dim Offset As UInteger = Math.Ceiling(fs.Position / 1024) * 1024
                        For i = 0 To _TrackCount - 1
                            For j = 0 To _SideCount - 1
                                fs.Seek(Offset, IO.SeekOrigin.Begin)
                                Dim Pos = fs.Position
                                fs.Seek(OffsetPos(i * _SideCount + j), IO.SeekOrigin.Begin)
                                Buffer = BitConverter.GetBytes(Offset)
                                fs.Write(Buffer, 0, Buffer.Length)
                                fs.Seek(Pos, IO.SeekOrigin.Begin)
                                Dim MFMTrack = GetTrack(i, j)
                                Dim BitLength = Math.Ceiling(MFMTrack.Bitstream.Length / 4096) * 4096
                                Dim Padding = BitLength - MFMTrack.Bitstream.Length
                                Buffer = Bitstream.IBM_MFM.BitsToBytes(MFMTrack.Bitstream, Padding)
                                fs.Write(Buffer, 0, Buffer.Length)
                                Offset = fs.Position
                            Next
                        Next
                    End Using
                Catch ex As Exception
                    Return False
                End Try

                Return True
            End Function

            Public Function UpdateBitstream() As Boolean Implements IBitstreamImage.UpdateBitstream
                Dim Updated As Boolean = False
                Dim Track As MFMTrack

                For i = 0 To _Tracks.Length - 1
                    Track = _Tracks(i)
                    For Each Sector In Track.MFMData.Sectors
                        If Sector.IsValid Then
                            Sector.UpdateChecksum()
                            If Sector.IsModified Then
                                Dim Bitstream = Sector.GetDataBitstream
                                Dim DataFieldOffset = Sector.DataOffset
                                For j = 0 To Bitstream.Length - 1
                                    Track.Bitstream(DataFieldOffset + j) = Bitstream(j)
                                Next
                                Updated = True
                            End If
                        End If
                    Next
                Next i

                Return Updated
            End Function

            Private Function IBitstreamImage_GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack Implements IBitstreamImage.GetTrack
                Return GetTrack(Track, Side)
            End Function
        End Class
    End Namespace
End Namespace
