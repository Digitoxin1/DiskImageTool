Imports DiskImageTool.Bitstream

Namespace ImageFormats.MFM
    Public Class MFMImage
        Inherits BitStreamImageBase
        Implements IBitstreamImage

        Private Const FILE_SIGNATURE = "HXCMFM"
        Private _SideCount As Byte
        Private _TrackCount As UShort
        Private _Tracks() As MFMTrack
        Private _TrackStep As Byte

        Private Enum MFMImageOffsets
            Signature = &H0
            TrackCount = &H7
            SideCount = &H9
            RPM = &HA
            BitRate = &HC
            IFType = &HE
            TrackList = &HF
        End Enum

        Private Enum MFMImageSizes
            Signature = &H6
            TrackList = &HB
            Advanced_TrackList = &HF
        End Enum

        Private Enum MFMTrackListOffsets
            Track = &H0
            Side = &H2
            Size = &H3
            Offset = &H7
            Advanced_RPM = &H3
            Advanced_BitRate = &H5
            Advanced_Size = &H7
            Advanced_Offset = &HB
        End Enum

        Public Sub New()
            _TrackCount = 0
            _SideCount = 0
            _TrackStep = 1
            _RPM = 0
            _BitRate = 0
        End Sub

        Public Sub New(TrackCount As UShort, SideCount As Byte, TrackStep As Byte)
            _TrackCount = TrackCount
            _SideCount = SideCount
            _TrackStep = TrackStep
            _RPM = 0
            _BitRate = 0

            _Tracks = New MFMTrack((_TrackCount) * _SideCount - 1) {}
        End Sub

        Public Overloads Property BitRate As UShort Implements IBitstreamImage.BitRate

        Public Property IFType As Byte

        Public Overloads Property RPM As UShort Implements IBitstreamImage.RPM

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

        Public Overloads ReadOnly Property TrackStep As Byte Implements IBitstreamImage.TrackStep
            Get
                Return _TrackStep
            End Get
        End Property

        Public Overrides Function Export(FilePath As String) As Boolean Implements IBitstreamImage.Export
            Dim Buffer() As Byte

            Try
                DeleteFileIfExists(FilePath)

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
                    Dim Offset As UInteger = AlignUp(CULng(fs.Position), 1024UL)
                    For i = 0 To _TrackCount - 1
                        For j = 0 To _SideCount - 1
                            fs.Seek(Offset, IO.SeekOrigin.Begin)
                            Dim Pos = fs.Position
                            fs.Seek(OffsetPos(i * _SideCount + j), IO.SeekOrigin.Begin)
                            Buffer = BitConverter.GetBytes(Offset)
                            fs.Write(Buffer, 0, Buffer.Length)
                            fs.Seek(Pos, IO.SeekOrigin.Begin)
                            Dim MFMTrack = GetTrack(i, j)
                            Dim BitLength = AlignUp(CUInt(MFMTrack.Bitstream.Length), 4096)
                            'Dim Padding = BitLength - MFMTrack.Bitstream.Length
                            'Buffer = IBM_MFM.BitsToBytes(MFMTrack.Bitstream, Padding)
                            Buffer = IBM_MFM.BitsToBytes(IBM_MFM.ResizeBitstream(MFMTrack.Bitstream, BitLength), 0)
                            fs.Write(Buffer, 0, Buffer.Length)
                            Offset = fs.Position
                        Next
                    Next
                End Using
            Catch ex As Exception
                DebugException(ex)
                Return False
            End Try

            Return True
        End Function

        Public Function GetTrack(Track As UShort, Side As Byte) As MFMTrack
            If Track > _TrackCount - 1 Or Side > _SideCount - 1 Then
                Throw New System.IndexOutOfRangeException
            End If

            Dim Index = Track * _SideCount + Side

            Return _Tracks(Index)
        End Function

        Public Overrides Function Load(FilePath As String) As Boolean Implements IBitstreamImage.Load
            Return Load(IO.File.ReadAllBytes(FilePath))
        End Function

        Public Overrides Function Load(Buffer() As Byte) As Boolean Implements IBitstreamImage.Load
            Dim Result As Boolean

            If Buffer.Length < MFMImageSizes.Signature Then
                Return False
            End If

            Dim Signature = Text.Encoding.UTF8.GetString(Buffer, MFMImageOffsets.Signature, MFMImageSizes.Signature)
            Result = (Signature = FILE_SIGNATURE)

            If Result Then
                Try
                    _TrackCount = BitConverter.ToUInt16(Buffer, MFMImageOffsets.TrackCount)
                    _SideCount = Buffer(MFMImageOffsets.SideCount)

                    _Tracks = New MFMTrack(_TrackCount * _SideCount - 1) {}

                    _RPM = BitConverter.ToUInt16(Buffer, MFMImageOffsets.RPM)
                    _BitRate = BitConverter.ToUInt16(Buffer, MFMImageOffsets.BitRate)
                    _IFType = Buffer(MFMImageOffsets.IFType)

                    Dim TrackListOffset = BitConverter.ToUInt32(Buffer, MFMImageOffsets.TrackList)

                    For i = 0 To _TrackCount - 1
                        For j = 0 To _SideCount - 1
                            Dim Track = BitConverter.ToUInt16(Buffer, TrackListOffset + MFMTrackListOffsets.Track)
                            Dim Side = Buffer(TrackListOffset + MFMTrackListOffsets.Side)
                            Dim MFMTrack As New MFMTrack(Track, Side)
                            Dim Size As UInteger
                            If _IFType And &H80 Then
                                MFMTrack.RPM = BitConverter.ToUInt16(Buffer, TrackListOffset + MFMTrackListOffsets.Advanced_RPM)
                                MFMTrack.BitRate = BitConverter.ToUInt16(Buffer, TrackListOffset + MFMTrackListOffsets.Advanced_BitRate)
                                Size = BitConverter.ToUInt32(Buffer, TrackListOffset + MFMTrackListOffsets.Advanced_Size)
                                MFMTrack.Offset = BitConverter.ToUInt32(Buffer, TrackListOffset + MFMTrackListOffsets.Advanced_Offset)
                                TrackListOffset += MFMImageSizes.Advanced_TrackList
                            Else
                                MFMTrack.RPM = RoundRPM(_RPM)
                                MFMTrack.BitRate = RoundBitRate(_BitRate)
                                Size = BitConverter.ToUInt32(Buffer, TrackListOffset + MFMTrackListOffsets.Size)
                                MFMTrack.Offset = BitConverter.ToUInt32(Buffer, TrackListOffset + MFMTrackListOffsets.Offset)
                                TrackListOffset += MFMImageSizes.TrackList

                                'Infer RPM
                                If MFMTrack.RPM = 0 Then
                                    If MFMTrack.BitRate = 250 Then
                                        MFMTrack.RPM = 300
                                    Else
                                        MFMTrack.RPM = CalculatetRPM(MFMTrack.BitRate, Size * 8)

                                        If MFMTrack.BitRate = 300 And MFMTrack.RPM = 240 Then
                                            MFMTrack.BitRate = 500
                                            MFMTrack.RPM = 360
                                        End If

                                        If MFMTrack.BitRate = 300 And MFMTrack.RPM <> 360 Then
                                            MFMTrack.RPM = 360
                                        End If

                                        If MFMTrack.RPM <> 300 And MFMTrack.RPM <> 360 Then
                                            MFMTrack.RPM = 300
                                        End If
                                    End If
                                End If
                            End If

                            MFMTrack.Bitstream = IBM_MFM.BytesToBits(Buffer, MFMTrack.Offset, Size * 8)
                            MFMTrack.MFMData = New IBM_MFM.IBM_MFM_Track(MFMTrack.Bitstream)

                            SetTrack(Track, Side, MFMTrack)
                        Next j
                    Next i

                    Dim BitRate = RoundBitRate(_BitRate)
                    If BitRate = 300 And _TrackCount > 79 Then
                        _TrackStep = 2
                    End If
                Catch ex As Exception
                    DebugException(ex)
                    Return False
                End Try
            End If

            Return Result
        End Function

        Public Sub SetTrack(Track As UShort, Side As Byte, Value As MFMTrack)
            If Track > _TrackCount - 1 Or Side > _SideCount - 1 Then
                Throw New System.IndexOutOfRangeException
            End If

            Dim Index = Track * _SideCount + Side

            _Tracks(Index) = Value
        End Sub

        Private Function IBitstreamImage_GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack Implements IBitstreamImage.GetTrack
            Return GetTrack(Track, Side)
        End Function
    End Class
End Namespace
