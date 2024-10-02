Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace PSI
        Public Class PSISectorImage
            Implements IBitstreamImage

            Private _CurrentSector As PSISector
            Private _CylinderCount As UShort = 0
            Private _HeadCount As Byte = 0
            Private _FirstSector As Integer = -1
            Private _LastSector As Integer = -1

            Public Sub New()
                Initialize()
            End Sub
            Public Property Header As PSIFileHeader
            Public Property Sectors As List(Of PSISector)
            Public Property Comment As String

            Public ReadOnly Property CylinderCount As UShort Implements IBitstreamImage.TrackCount
                Get
                    Return _CylinderCount
                End Get
            End Property

            Public ReadOnly Property HeadCount As Byte Implements IBitstreamImage.SideCount
                Get
                    Return _HeadCount
                End Get
            End Property

            Public ReadOnly Property FirstSector As Integer
                Get
                    Return _FirstSector
                End Get
            End Property

            Public ReadOnly Property LastSector As Integer
                Get
                    Return _LastSector
                End Get
            End Property

            Public Function Export(FilePath As String) As Boolean
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
                    Return False
                End Try


                Return True
            End Function

            Public Function Import(FilePath As String) As Boolean
                Return Import(IO.File.ReadAllBytes(FilePath))
            End Function

            Public Function Import(Buffer() As Byte) As Boolean
                Dim Result As Boolean = False
                Dim Chunk As PSIChunk
                Dim ChunkCount As UInteger = 0
                Dim Response As ReadResponse

                Initialize()

                Response.Offset = 0

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

                Return Result
            End Function

            Private Sub Initialize()
                _Header = New PSIFileHeader
                _Sectors = New List(Of PSISector)
                _CurrentSector = Nothing
                _Comment = ""
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
                    If Sector.Cylinder > _CylinderCount - 1 Then
                        _CylinderCount = Sector.Cylinder + 1
                    End If
                    If Sector.Head > _HeadCount - 1 Then
                        _HeadCount = Sector.Head + 1
                    End If

                    If _FirstSector = -1 Then
                        _FirstSector = Sector.Sector
                    ElseIf Sector.Sector < _FirstSector Then
                        _FirstSector = Sector.Sector
                    End If

                    If _LastSector = -1 Then
                        _LastSector = Sector.Sector
                    ElseIf Sector.Sector > _LastSector Then
                        _LastSector = Sector.Sector
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

            Private ReadOnly Property IBitstreamImage_TrackStep As Byte Implements IBitstreamImage.TrackStep
                Get
                    Return 1
                End Get
            End Property

            Private Function IBitstreamImage_GetTrack(Track As UShort, Site As Byte) As IBitstreamTrack Implements IBitstreamImage.GetTrack
                Return Nothing
            End Function

            Private Function IBitstreamImage_UpdateBitstream() As Boolean Implements IBitstreamImage.UpdateBitstream
                Return False
            End Function
        End Class
    End Namespace
End Namespace
