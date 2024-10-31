Imports DiskImageTool.Bitstream

Namespace ImageFormats
    Namespace IMD
        Public Class IMDImage
            Implements IBitstreamImage

            Public Sub New()
                _Tracks = New List(Of IMDTrack)
                _TrackCount = 0
                _SideCount = 0
            End Sub

            Public Property Comment As String
            Public ReadOnly Property Header As String

            Public Property Tracks As List(Of IMDTrack)

            Public Property TrackCount As UShort Implements IBitstreamImage.TrackCount

            Public Property SideCount As Byte Implements IBitstreamImage.SideCount

            Public Function Export(FilePath As String) As Boolean Implements IBitstreamImage.Export
                Throw New NotImplementedException()
            End Function

            Public Function Load(FilePath As String) As Boolean Implements IBitstreamImage.Load
                Return Load(IO.File.ReadAllBytes(FilePath))
            End Function

            Public Function Load(Buffer() As Byte) As Boolean Implements IBitstreamImage.Load
                Dim Result As Boolean

                _Header = Text.Encoding.UTF8.GetString(Buffer, 0, 29)
                Result = _Header.Substring(0, 4) = "IMD "

                If Result Then
                    Dim SectorCount As Byte
                    Dim HeadValue As Byte
                    Dim HasCylinderMap As Boolean
                    Dim HasHeadMap As Boolean
                    Dim SectorMap() As Byte
                    Dim CylinderMap() As Byte
                    Dim HeadMap() As Byte
                    Dim Offset As UInteger = 31
                    Dim Length As UInteger = 0

                    Do While Buffer(Offset + Length) <> &H1A
                        Length += 1
                        If Offset + Length >= Buffer.Length Then
                            Exit Do
                        End If
                    Loop

                    _Comment = Text.Encoding.UTF8.GetString(Buffer, Offset, Length)
                    Offset += Length + 1

                    Do While Offset < Buffer.Length
                        HeadValue = Buffer(Offset + 2)
                        SectorCount = Buffer(Offset + 3)
                        HasCylinderMap = (HeadValue And 128) > 0
                        HasHeadMap = (HeadValue And 64) > 0
                        SectorMap = New Byte(SectorCount - 1) {}
                        CylinderMap = New Byte(SectorCount - 1) {}
                        HeadMap = New Byte(SectorCount - 1) {}
                        Dim Track As New IMDTrack With {
                            .Mode = Buffer(Offset),
                            .Cylinder = Buffer(Offset + 1),
                            .Head = HeadValue And &H1,
                            .SectorSize = Buffer(Offset + 4)
                        }
                        Offset += 5

                        For i = 0 To SectorCount - 1
                            SectorMap(i) = Buffer(Offset + i)
                        Next
                        Offset += SectorCount

                        If HasCylinderMap Then
                            For i = 0 To SectorCount - 1
                                CylinderMap(i) = Buffer(Offset + i)
                            Next
                            Offset += SectorCount
                        Else
                            For i = 0 To SectorCount - 1
                                CylinderMap(i) = Track.Cylinder
                            Next
                        End If

                        If HasHeadMap Then
                            For i = 0 To SectorCount - 1
                                HeadMap(i) = Buffer(Offset + i)
                            Next
                            Offset += SectorCount
                        Else
                            For i = 0 To SectorCount - 1
                                HeadMap(i) = Track.Head
                            Next
                        End If

                        For i = 0 To SectorCount - 1
                            Dim SectorSize = Track.GetSizeBytes
                            Dim Format As DataFormat = Buffer(Offset)
                            Dim Sector = New IMDSector(SectorSize) With {
                                .SectorId = SectorMap(i),
                                .Cylinder = CylinderMap(i),
                                .Head = HeadMap(i),
                                .Unavailable = (Format = DataFormat.Unavailable),
                                .Compressed = (Format = DataFormat.DeletedCompressed Or Format = DataFormat.DeletedErrorCompressed Or Format = DataFormat.NormalCompressed Or Format = DataFormat.NormalErrorCompressed),
                                .Deleted = (Format = DataFormat.Deleted Or Format = DataFormat.DeletedCompressed Or Format = DataFormat.DeletedError Or Format = DataFormat.DeletedErrorCompressed),
                                .ChecksumError = (Format = DataFormat.DeletedError Or Format = DataFormat.DeletedErrorCompressed Or Format = DataFormat.NormalError Or Format = DataFormat.NormalErrorCompressed)
                            }
                            Offset += 1

                            If Sector.Unavailable Then
                                'Empty Data
                            ElseIf Sector.Compressed Then
                                Sector.CompressedValue = Buffer(Offset)
                                For j = 0 To Sector.Data.Length - 1
                                    Sector.Data(j) = Sector.CompressedValue
                                Next
                                Offset += 1
                            Else
                                Array.Copy(Buffer, Offset, Sector.Data, 0, Sector.Data.Length)
                                Offset += Sector.Data.Length
                            End If

                            Track.Sectors.Add(Sector)
                        Next

                        _Tracks.Add(Track)

                        If Track.Cylinder >= _TrackCount Then
                            _TrackCount = Track.Cylinder + 1
                        End If

                        If Track.Head >= _SideCount Then
                            _SideCount = Track.Head + 1
                        End If
                    Loop
                End If

                Return Result
            End Function

            Private ReadOnly Property IBitstreamImage_TrackStep As Byte Implements IBitstreamImage.TrackStep
                Get
                    Return 1
                End Get
            End Property

            Private Function IBitstreamImage_GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack Implements IBitstreamImage.GetTrack
                Return Nothing
            End Function

            Private ReadOnly Property IBitstreamImage_HasSurfaceData As Boolean Implements IBitstreamImage.HasSurfaceData
                Get
                    Return False
                End Get
            End Property

        End Class
    End Namespace
End Namespace
