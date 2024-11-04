Imports System.Text
Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace HFE
        Public Class HFEImage
            Inherits BitStreamImageBase
            Implements IBitstreamImage

            Private Const FILE_SIGNATURE_V1 = "HXCPICFE"
            Private Const FILE_SIGNATURE_V3 = "HXCHFEV3"
            Private _Header() As Byte
            Private _Tracks() As HFETrack

            Private Enum HFEHeaderOffsets
                Signature = &H0
                FormatRevision = &H8
                TrackCount = &H9
                SideCount = &HA
                TrackEncoding = &HB
                BitRate = &HC
                FloppyRPM = &HE
                FloppyInterfaceMode = &H10
                DNU = &H11
                TrackListOffset = &H12
                WriteAllowed = &H14
                SingleStep = &H15
                Track0S0_AltEncoding = &H16
                Track0S0_Encoding = &H17
                Track0S1_AltEncoding = &H18
                Track0S1_Encoding = &H19
            End Enum

            Private Enum HFEHeadeSizes
                Signature = 8
            End Enum

            Public Sub New()
                _Header = New Byte(25) {}
                Encoding.UTF8.GetBytes(FILE_SIGNATURE_V1).CopyTo(_Header, 0)
                FormatRevision = 0
                TrackCount = 0
                SideCount = 0
                TrackEncoding = HFETrackEncoding.ISOIBM_MFM_ENCODING
                BitRate = 0
                RPM = 0
                FloppyInterfaceMode = HFEFloppyinterfaceMode.DISABLE_FLOPPYMODE
                DNU = &HFF
                TrackListOffset = 1
                WriteAllowed = &HFF
                SingleStep = &HFF
                Track0S0_AltEncoding = &HFF
                Track0S0_Encoding = &HFF
                Track0S1_AltEncoding = &HFF
                Track0S1_Encoding = &HFF
            End Sub

            Public Sub New(TrackCount As Byte, SideCount As Byte)
                _Header = New Byte(25) {}
                Encoding.UTF8.GetBytes(FILE_SIGNATURE_V1).CopyTo(_Header, 0)
                FormatRevision = 0
                Me.TrackCount = TrackCount
                Me.SideCount = SideCount
                TrackEncoding = HFETrackEncoding.ISOIBM_MFM_ENCODING
                BitRate = 0
                RPM = 0
                FloppyInterfaceMode = HFEFloppyinterfaceMode.DISABLE_FLOPPYMODE
                DNU = &HFF
                TrackListOffset = 1
                WriteAllowed = &HFF
                SingleStep = &HFF
                Track0S0_AltEncoding = &HFF
                Track0S0_Encoding = &HFF
                Track0S1_AltEncoding = &HFF
                Track0S1_Encoding = &HFF

                _Tracks = New HFETrack((TrackCount) * SideCount - 1) {}
            End Sub

            Public Overloads Property BitRate As UShort Implements IBitstreamImage.BitRate
                Get
                    Return BitConverter.ToUInt16(_Header, HFEHeaderOffsets.BitRate)
                End Get
                Set(value As UShort)
                    Array.Copy(BitConverter.GetBytes(value), 0, _Header, HFEHeaderOffsets.BitRate, 2)
                End Set
            End Property

            Public Property DNU As Byte
                Get
                    Return _Header(HFEHeaderOffsets.DNU)
                End Get
                Set(value As Byte)
                    _Header(HFEHeaderOffsets.DNU) = value
                End Set
            End Property

            Public Property FloppyInterfaceMode As HFEFloppyinterfaceMode
                Get
                    Return _Header(HFEHeaderOffsets.FloppyInterfaceMode)
                End Get
                Set(value As HFEFloppyinterfaceMode)
                    _Header(HFEHeaderOffsets.FloppyInterfaceMode) = value
                End Set
            End Property

            Public Property FormatRevision As Byte
                Get
                    Return _Header(HFEHeaderOffsets.FormatRevision)
                End Get
                Set(value As Byte)
                    _Header(HFEHeaderOffsets.FormatRevision) = value
                End Set
            End Property

            Public Overloads Property RPM As UShort Implements IBitstreamImage.RPM
                Get
                    Return BitConverter.ToUInt16(_Header, HFEHeaderOffsets.FloppyRPM)
                End Get
                Set(value As UShort)
                    Array.Copy(BitConverter.GetBytes(value), 0, _Header, HFEHeaderOffsets.FloppyRPM, 2)
                End Set
            End Property

            Public Overloads Property SideCount As Byte Implements IBitstreamImage.SideCount
                Get
                    Return _Header(HFEHeaderOffsets.SideCount)
                End Get
                Set(value As Byte)
                    _Header(HFEHeaderOffsets.SideCount) = value
                End Set
            End Property

            Public ReadOnly Property Signature As String
                Get
                    Dim Buffer(HFEHeadeSizes.Signature - 1) As Byte
                    Array.Copy(_Header, HFEHeaderOffsets.Signature, Buffer, 0, Buffer.Length)
                    Return Encoding.UTF8.GetString(Buffer)
                End Get
            End Property

            Public Property SingleStep As Byte
                Get
                    Return _Header(HFEHeaderOffsets.SingleStep)
                End Get
                Set(value As Byte)
                    _Header(HFEHeaderOffsets.SingleStep) = value
                End Set
            End Property

            Public Property Track0S0_AltEncoding As Byte
                Get
                    Return _Header(HFEHeaderOffsets.Track0S0_AltEncoding)
                End Get
                Set(value As Byte)
                    _Header(HFEHeaderOffsets.Track0S0_AltEncoding) = value
                End Set
            End Property

            Public Property Track0S0_Encoding As Byte
                Get
                    Return _Header(HFEHeaderOffsets.Track0S0_Encoding)
                End Get
                Set(value As Byte)
                    _Header(HFEHeaderOffsets.Track0S0_Encoding) = value
                End Set
            End Property

            Public Property Track0S1_AltEncoding As Byte
                Get
                    Return _Header(HFEHeaderOffsets.Track0S1_AltEncoding)
                End Get
                Set(value As Byte)
                    _Header(HFEHeaderOffsets.Track0S1_AltEncoding) = value
                End Set
            End Property

            Public Property Track0S1_Encoding As Byte
                Get
                    Return _Header(HFEHeaderOffsets.Track0S1_Encoding)
                End Get
                Set(value As Byte)
                    _Header(HFEHeaderOffsets.Track0S1_Encoding) = value
                End Set
            End Property

            Public Overloads Property TrackCount As Byte
                Get
                    Return _Header(HFEHeaderOffsets.TrackCount)
                End Get
                Set(value As Byte)
                    _Header(HFEHeaderOffsets.TrackCount) = value
                End Set
            End Property

            Public Property TrackEncoding As HFETrackEncoding
                Get
                    Return _Header(HFEHeaderOffsets.TrackEncoding)
                End Get
                Set(value As HFETrackEncoding)
                    _Header(HFEHeaderOffsets.TrackEncoding) = value
                End Set
            End Property

            Public Property TrackListOffset As UShort
                Get
                    Return BitConverter.ToUInt16(_Header, HFEHeaderOffsets.TrackListOffset)
                End Get
                Set(value As UShort)
                    Array.Copy(BitConverter.GetBytes(value), 0, _Header, HFEHeaderOffsets.TrackListOffset, 2)
                End Set
            End Property

            Public Property Version As HFEVersion
                Get
                    If Signature = FILE_SIGNATURE_V3 Then
                        Return HFEVersion.HFE_V3
                    ElseIf FormatRevision = 1 Then
                        Return HFEVersion.HFE_V2
                    Else
                        Return HFEVersion.HFE_V1
                    End If
                End Get
                Set(value As HFEVersion)
                    If value = HFEVersion.HFE_V2 Then
                        Encoding.UTF8.GetBytes(FILE_SIGNATURE_V1).CopyTo(_Header, 0)
                        FormatRevision = 1
                    ElseIf value = HFEVersion.HFE_V3 Then
                        Encoding.UTF8.GetBytes(FILE_SIGNATURE_V3).CopyTo(_Header, 0)
                        FormatRevision = 0
                    Else
                        Encoding.UTF8.GetBytes(FILE_SIGNATURE_V1).CopyTo(_Header, 0)
                        FormatRevision = 0
                    End If
                End Set
            End Property

            Public Property WriteAllowed As Byte
                Get
                    Return _Header(HFEHeaderOffsets.WriteAllowed)
                End Get
                Set(value As Byte)
                    _Header(HFEHeaderOffsets.WriteAllowed) = value
                End Set
            End Property

            Private ReadOnly Property IBitstreamImage_TrackCount As UShort Implements IBitstreamImage.TrackCount
                Get
                    Return TrackCount
                End Get
            End Property

            Public Overrides Function Export(FilePath As String) As Boolean Implements IBitstreamImage.Export
                Dim Buffer() As Byte

                Try
                    If IO.File.Exists(FilePath) Then
                        IO.File.Delete(FilePath)
                    End If

                    Using fs As IO.FileStream = IO.File.OpenWrite(FilePath)
                        fs.Write(_Header, 0, _Header.Length)

                        Dim Offset = TrackListOffset * 512

                        Buffer = New Byte(Offset - _Header.Length - 1) {}
                        FillArray(Buffer, &HFF)
                        fs.Write(Buffer, 0, Buffer.Length)

                        Dim OffsetBlockSize = Math.Ceiling(TrackCount * 4 / 512) * 512
                        Dim TrackDataOffset As UShort = (fs.Position + OffsetBlockSize) \ 512
                        Dim TrackOffsets(TrackCount - 1) As UShort
                        Dim BlockCount As UShort
                        Dim TrackDataLength As UShort

                        For i As Byte = 0 To TrackCount - 1
                            Dim Track0 = GetTrack(i, 0)
                            Dim TrackLength = Track0.Length
                            If SideCount > 1 Then
                                Dim Track1 = GetTrack(i, 1)
                                If Track1.Length > TrackLength Then
                                    TrackLength = Track1.Length
                                End If
                            End If

                            TrackDataLength = TrackLength * 2

                            TrackOffsets(i) = TrackDataOffset
                            BlockCount = Math.Ceiling(TrackDataLength / 512)

                            Buffer = BitConverter.GetBytes(TrackDataOffset)
                            fs.Write(Buffer, 0, Buffer.Length)

                            Buffer = BitConverter.GetBytes(TrackDataLength)
                            fs.Write(Buffer, 0, Buffer.Length)

                            TrackDataOffset += BlockCount
                            OffsetBlockSize -= 4
                        Next

                        Buffer = New Byte(OffsetBlockSize - 1) {}
                        FillArray(Buffer, &HFF)
                        fs.Write(Buffer, 0, Buffer.Length)

                        For i As Byte = 0 To TrackCount - 1
                            Dim DataSide0() As Byte
                            Dim DataSide1() As Byte

                            Dim Track0 = GetTrack(i, 0)
                            Dim TrackLength As UShort = Track0.Length

                            If SideCount > 1 Then
                                Dim Track1 = GetTrack(i, 1)
                                If Track1.Length > TrackLength Then
                                    TrackLength = Track1.Length
                                End If
                                DataSide1 = IBM_MFM.BitsToBytes(IBM_MFM.ResizeBitstream(Track1.Bitstream, TrackLength * 8), 0, False)
                            Else
                                DataSide1 = New Byte(TrackLength - 1) {}
                            End If

                            DataSide0 = IBM_MFM.BitsToBytes(IBM_MFM.ResizeBitstream(Track0.Bitstream, TrackLength * 8), 0, False)

                            TrackDataLength = TrackLength * 2
                            BlockCount = Math.Ceiling(TrackDataLength / 512)

                            For j = 0 To BlockCount - 1
                                Dim DataOffset = j * 256
                                Dim BlockSize As Integer = Math.Min(512, TrackDataLength)
                                Dim DataSize = BlockSize \ 2

                                fs.Write(DataSide0, DataOffset, DataSize)
                                If DataSize < 256 Then
                                    Buffer = New Byte(256 - DataSize - 1) {}
                                    fs.Write(Buffer, 0, Buffer.Length)
                                End If

                                fs.Write(DataSide1, DataOffset, DataSize)
                                If DataSize < 256 Then
                                    Buffer = New Byte(256 - DataSize - 1) {}
                                    fs.Write(Buffer, 0, Buffer.Length)
                                End If

                                TrackDataLength -= BlockSize
                            Next
                        Next
                    End Using
                Catch ex As Exception
                    DebugException(ex)
                    Return False
                End Try

                Return True
            End Function


            Public Function GetTrack(Track As Byte, Side As Byte) As HFETrack
                If Track > TrackCount - 1 Or Side > SideCount - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Index = Track * SideCount + Side

                Return _Tracks(Index)
            End Function


            Public Overrides Function Load(FilePath As String) As Boolean Implements IBitstreamImage.Load
                Return Load(IO.File.ReadAllBytes(FilePath))
            End Function


            Public Overrides Function Load(Buffer() As Byte) As Boolean Implements IBitstreamImage.Load
                Dim Result As Boolean

                If Buffer.Length < HFEHeadeSizes.Signature Then
                    Return False
                End If

                Dim Signature = Text.Encoding.UTF8.GetString(Buffer, HFEHeaderOffsets.Signature, HFEHeadeSizes.Signature)
                Result = (Signature = FILE_SIGNATURE_V1 Or Signature = FILE_SIGNATURE_V3)

                If Result Then
                    Try
                        _Header = New Byte(25) {}
                        Array.Copy(Buffer, 0, _Header, 0, _Header.Length)

                        If Version <> HFEVersion.HFE_V1 Then
                            'Unsupported Version
                            Return False
                        End If

                        _Tracks = New HFETrack(TrackCount * SideCount - 1) {}

                        Dim Offset = TrackListOffset * 512
                        Dim DataSide0() As Byte
                        Dim DataSide1() As Byte = Nothing

                        For i = 0 To TrackCount - 1
                            Dim TrackDataOffset = BitConverter.ToUInt16(Buffer, Offset)
                            Dim TrackDataLength = BitConverter.ToUInt16(Buffer, Offset + 2)
                            DataSide0 = New Byte(TrackDataLength / 2 - 1) {}
                            If SideCount > 1 Then
                                DataSide1 = New Byte(TrackDataLength / 2 - 1) {}
                            End If

                            Dim BlockCount As UInteger = Math.Ceiling(TrackDataLength / 512)

                            For j = 0 To BlockCount - 1
                                Dim BlockOffset = j * 512
                                Dim DataOffset = j * 256
                                Dim BlockSize As Integer = Math.Min(512, TrackDataLength - BlockOffset)
                                Dim DataSide0Offset = BlockOffset
                                Dim DataSide1Offset = BlockOffset + 256
                                Dim DataSize = BlockSize \ 2
                                Array.Copy(Buffer, TrackDataOffset * 512 + DataSide0Offset, DataSide0, DataOffset, DataSize)
                                If SideCount > 1 Then
                                    Array.Copy(Buffer, TrackDataOffset * 512 + DataSide1Offset, DataSide1, DataOffset, DataSize)
                                End If
                            Next

                            Dim HFETrack = New HFETrack(i, 0) With {
                                .Bitstream = IBM_MFM.BytesToBits(DataSide0, 0, DataSide0.Length * 8, False),
                                .BitRate = BitRate,
                                .RPM = RPM
                            }
                            HFETrack.MFMData = New IBM_MFM.IBM_MFM_Track(HFETrack.Bitstream)
                            SetTrack(i, 0, HFETrack)

                            If SideCount > 1 Then
                                HFETrack = New HFETrack(i, 1) With {
                                    .Bitstream = IBM_MFM.BytesToBits(DataSide1, 0, DataSide1.Length * 8, False),
                                    .BitRate = BitRate,
                                    .RPM = RPM
                                }
                                HFETrack.MFMData = New IBM_MFM.IBM_MFM_Track(HFETrack.Bitstream)
                                SetTrack(i, 1, HFETrack)
                            End If

                            Offset += 4
                        Next
                    Catch ex As Exception
                        DebugException(ex)
                        Return False
                    End Try
                End If

                Return Result
            End Function

            Public Sub SetTrack(Track As Byte, Side As Byte, Value As HFETrack)
                If Track > TrackCount - 1 Or Side > SideCount - 1 Then
                    Throw New System.IndexOutOfRangeException
                End If

                Dim Index = Track * SideCount + Side

                _Tracks(Index) = Value
            End Sub

            Private Function IBitstreamImage_GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack Implements IBitstreamImage.GetTrack
                Return GetTrack(Track, Side)
            End Function
        End Class
    End Namespace
End Namespace
