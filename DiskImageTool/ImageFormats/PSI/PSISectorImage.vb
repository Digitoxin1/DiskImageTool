Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace PSI
        Public Class PSISectorImage
            Inherits BitStreamImageBase
            Implements IBitstreamImage

            Private _CurrentSector As PSISector
            Private _HasWeakBits As Boolean
            Private _SideCount As Byte = 0
            Private _TrackCount As UShort = 0
            Private _TrackInfo As Dictionary(Of String, PSITrackInfo)

            Public Sub New()
                Initialize()
            End Sub

            Public Property Comment As String

            Public ReadOnly Property HasWeakBits As Boolean Implements IBitstreamImage.HasSurfaceData
                Get
                    Return _HasWeakBits
                End Get
            End Property

            Public Property Header As PSIFileHeader

            Public Property Sectors As List(Of PSISector)

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

            Public Overrides Function Export(FilePath As String) As Boolean Implements IBitstreamImage.Export
                Dim Buffer() As Byte

                Try
                    If IO.File.Exists(FilePath) Then
                        IO.File.Delete(FilePath)
                    End If

                    Using fs As IO.FileStream = IO.File.OpenWrite(FilePath)
                        Buffer = New PSIChunk("PSI ", _Header.ChunkData).ToBytes
                        fs.Write(Buffer, 0, Buffer.Length)

                        If _Comment.Length > 0 Then
                            Buffer = New PSIChunk("TEXT", Text.Encoding.UTF8.GetBytes(_Comment)).ToBytes
                            fs.Write(Buffer, 0, Buffer.Length)
                        End If

                        For Each Sector In _Sectors
                            Buffer = New PSIChunk("SECT", Sector.ChunkData).ToBytes
                            fs.Write(Buffer, 0, Buffer.Length)

                            If Sector.FMHeader IsNot Nothing Then
                                Buffer = New PSIChunk("IBMF", Sector.FMHeader.ChunkData).ToBytes
                                fs.Write(Buffer, 0, Buffer.Length)
                            End If

                            If Sector.MFMHeader IsNot Nothing Then
                                Buffer = New PSIChunk("IBMM", Sector.MFMHeader.ChunkData).ToBytes
                                fs.Write(Buffer, 0, Buffer.Length)
                            End If

                            If Sector.GCRHeader IsNot Nothing Then
                                Buffer = New PSIChunk("MACG", Sector.GCRHeader.ChunkData).ToBytes
                                fs.Write(Buffer, 0, Buffer.Length)
                            End If

                            If Sector.Offset > 0 Then
                                Dim OffsetBuffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(Sector.Offset))
                                Buffer = New PSIChunk("OFFS", OffsetBuffer).ToBytes
                                fs.Write(Buffer, 0, Buffer.Length)
                            End If

                            If Sector.SectorReadTime > 0 Then
                                Dim OffsetBuffer = BitConverter.GetBytes(MyBitConverter.SwapEndian(Sector.SectorReadTime))
                                Buffer = New PSIChunk("TIME", OffsetBuffer).ToBytes
                                fs.Write(Buffer, 0, Buffer.Length)
                            End If

                            If Not Sector.IsCompressed Then
                                Buffer = New PSIChunk("DATA", Sector.Data).ToBytes
                                fs.Write(Buffer, 0, Buffer.Length)
                            End If

                            If Sector.HasWeakBits Then
                                Buffer = New PSIChunk("WEAK", Sector.Weak).ToBytes
                                fs.Write(Buffer, 0, Buffer.Length)
                            End If
                        Next

                        Buffer = New PSIChunk("END ").ToBytes
                        fs.Write(Buffer, 0, Buffer.Length)
                    End Using
                Catch ex As Exception
                    DebugException(ex)
                    Return False
                End Try

                Return True
            End Function

            Public Function GetTrackInfo(Track As UShort, Side As Byte) As PSITrackInfo
                Dim Key = Track & "." & Side

                If _TrackInfo.ContainsKey(Key) Then
                    Return _TrackInfo.Item(Key)
                Else
                    Dim TrackInfo = New PSITrackInfo
                    _TrackInfo.Add(Key, TrackInfo)
                    Return TrackInfo
                End If
            End Function

            Public Overrides Function Load(FilePath As String) As Boolean Implements IBitstreamImage.Load
                Return Load(IO.File.ReadAllBytes(FilePath))
            End Function

            Public Overrides Function Load(Buffer() As Byte) As Boolean Implements IBitstreamImage.Load
                Dim Result As Boolean = False
                Dim Chunk As PSIChunk
                Dim ChunkCount As UInteger = 0
                Dim Response As ReadResponse

                Initialize()

                Response.Offset = 0

                Try
                    Do While Response.Offset + 8 < Buffer.Length
                        Chunk = New PSIChunk()
                        Response = Chunk.ReadFromBuffer(Buffer, Response.Offset)
                        If Response.ChecksumVerified Then
                            If ChunkCount = 0 AndAlso Chunk.ChunkID <> "PSI " Then
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

            Private Sub Initialize()
                _Header = New PSIFileHeader
                _Sectors = New List(Of PSISector)
                _CurrentSector = Nothing
                _Comment = ""
                _HasWeakBits = False
                _TrackInfo = New Dictionary(Of String, PSITrackInfo)
            End Sub

            Private Sub ProcessChunk(Chunk As PSIChunk)
                If Chunk.ChunkID = "PSI " Then
                    _Header = New PSIFileHeader(Chunk.ChunkData)

                ElseIf Chunk.ChunkID = "TEXT" Then
                    _Comment &= Text.Encoding.UTF8.GetString(Chunk.ChunkData)

                ElseIf Chunk.ChunkID = "SECT" Then
                    Dim Sector = New PSISector(Chunk.ChunkData)
                    _Sectors.Add(Sector)
                    _CurrentSector = Sector
                    If Sector.Track > _TrackCount - 1 Then
                        _TrackCount = Sector.Track + 1
                    End If
                    If Sector.Side > _SideCount - 1 Then
                        _SideCount = Sector.Side + 1
                    End If

                    If Not Sector.IsAlternateSector Then
                        UpdateTrackInfo(Sector)
                    End If

                ElseIf Chunk.ChunkID = "DATA" Then
                    If _CurrentSector IsNot Nothing Then
                        Dim Size = Math.Min(_CurrentSector.Size, Chunk.ChunkData.Length)
                        Array.Copy(Chunk.ChunkData, 0, _CurrentSector.Data, 0, Size)
                        If _CurrentSector.IsCompressed Then
                            _CurrentSector.IsCompressed = False
                        End If
                    End If

                ElseIf Chunk.ChunkID = "WEAK" Then
                    If _CurrentSector IsNot Nothing Then
                        _HasWeakBits = True
                        _CurrentSector.Weak = Chunk.ChunkData
                    End If

                ElseIf Chunk.ChunkID = "OFFS" Then
                    If _CurrentSector IsNot Nothing Then
                        _CurrentSector.Offset = MyBitConverter.ToUInt32(Chunk.ChunkData, True)
                    End If

                ElseIf Chunk.ChunkID = "TIME" Then
                    If _CurrentSector IsNot Nothing Then
                        _CurrentSector.SectorReadTime = MyBitConverter.ToUInt32(Chunk.ChunkData, True)
                    End If

                ElseIf Chunk.ChunkID = "IBMM" Then
                    If _CurrentSector IsNot Nothing Then
                        _CurrentSector.MFMHeader = New IBMSectorHeader(Chunk.ChunkData)
                    End If

                ElseIf Chunk.ChunkID = "IBMF" Then
                    If _CurrentSector IsNot Nothing Then
                        _CurrentSector.FMHeader = New IBMSectorHeader(Chunk.ChunkData)
                    End If

                ElseIf Chunk.ChunkID = "MACG" Then
                    If _CurrentSector IsNot Nothing Then
                        _CurrentSector.GCRHeader = New GCRSectorHeader(Chunk.ChunkData)
                    End If
                End If
            End Sub

            Private Sub UpdateTrackInfo(Sector As PSISector)
                Dim TrackInfo = GetTrackInfo(Sector.Track, Sector.Side)

                If TrackInfo.FirstSector = -1 Then
                    TrackInfo.FirstSector = Sector.Sector
                    TrackInfo.SectorSize = Sector.Size
                Else
                    If Sector.Sector < TrackInfo.FirstSector Then
                        TrackInfo.FirstSector = Sector.Sector
                    End If

                    If Sector.Size <> TrackInfo.SectorSize Then
                        TrackInfo.SectorSize = -1
                    End If
                End If

                If TrackInfo.LastSector = -1 Then
                    TrackInfo.LastSector = Sector.Sector
                ElseIf Sector.Sector > TrackInfo.LastSector Then
                    TrackInfo.LastSector = Sector.Sector
                End If
            End Sub
        End Class
    End Namespace
End Namespace
