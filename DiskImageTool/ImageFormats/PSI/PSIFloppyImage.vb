Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace PSI
        Public Class PSIFloppyImage
            Inherits MappedFloppyImage
            Implements IFloppyImage

            Private ReadOnly _Image As PSISectorImage

            Public Sub New(Image As PSISectorImage, DiskFormat As FloppyDiskFormat, BytesPerSector As UInteger)
                MyBase.New(Nothing, BytesPerSector)

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
            End Sub

            Public Overrides ReadOnly Property HasWeakBits As Boolean Implements IFloppyImage.HasWeakBits
                Get
                    Return _Image.HasWeakBits
                End Get
            End Property

            Public Overrides ReadOnly Property HasWeakBitsSupport As Boolean Implements IFloppyImage.HasWeakBitsSupport
                Get
                    Return True
                End Get
            End Property

            Public ReadOnly Property Image As PSISectorImage
                Get
                    Return _Image
                End Get
            End Property

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IFloppyImage.ImageType
                Get
                    Return FloppyImageType.PSIImage
                End Get
            End Property

            Public Overrides Function GetCRC32() As String Implements IFloppyImage.GetCRC32
                Using Hasher As CRC32Hash = CRC32Hash.Create()
                    Return CalculateHash(Hasher)
                End Using
            End Function

            Public Overrides Function GetMD5Hash() As String Implements IFloppyImage.GetMD5Hash
                Using Hasher As MD5 = MD5.Create()
                    Return CalculateHash(Hasher)
                End Using
            End Function

            Public Overrides Function GetSHA1Hash() As String Implements IFloppyImage.GetSHA1Hash
                Using Hasher As SHA1 = SHA1.Create()
                    Return CalculateHash(Hasher)
                End Using
            End Function

            Private Sub BuildSectorMap()
                Dim TrackInfo As PSITrackInfo
                Dim TrackData As TrackData
                Dim MaxSectors As UShort

                SetTracks(_Image.TrackCount, _Image.SideCount)

                If _Image.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_DD Then
                    MaxSectors = 9
                ElseIf _Image.Header.DefaultSectorFormat = DefaultSectorFormat.IBM_MFM_HD Then
                    MaxSectors = 18
                Else
                    MaxSectors = 36
                End If

                For Each PSISector In _Image.Sectors
                    If Not PSISector.IsAlternateSector Then
                        TrackData = GetTrack(PSISector.Track, PSISector.Side)

                        If TrackData Is Nothing Then
                            TrackInfo = _Image.GetTrackInfo(PSISector.Track, PSISector.Side)
                            TrackData = SetTrack(PSISector.Track, PSISector.Side)
                            TrackData.FirstSector = TrackInfo.FirstSector
                            TrackData.LastSector = TrackInfo.LastSector
                            TrackData.SectorSize = TrackInfo.SectorSize
                            TrackData.Encoding = BitstreamTrackType.MFM
                        End If

                        If TrackData.FirstSector = 1 And TrackData.LastSector = 4 And TrackData.SectorSize = 1024 Then
                            ProcessSector1024(PSISector)
                        Else
                            ProcessSector(PSISector, MaxSectors)
                        End If

                    End If
                Next
            End Sub

            Private Function CalculateHash(HashAlgorithm As HashAlgorithm) As String
                Dim OutputBuffer() As Byte

                For Each PSISector In _Image.Sectors
                    If Not PSISector.IsAlternateSector And Not PSISector.HasDataCRCError Then
                        OutputBuffer = New Byte(PSISector.Data.Length - 1) {}
                        HashAlgorithm.TransformBlock(PSISector.Data, 0, PSISector.Data.Length, OutputBuffer, 0)
                    End If
                Next
                HashAlgorithm.TransformFinalBlock(New Byte(0) {}, 0, 0)

                Return HashBytesToString(HashAlgorithm.Hash)
            End Function

            Private Function IsStandardSector(PSISector As PSISector, MaxSectors As UShort) As Boolean
                If PSISector.Sector < 1 Or PSISector.Sector > MaxSectors Then
                    Return False
                End If

                If PSISector.HasDataCRCError Then
                    Return False
                End If

                If PSISector.MFMHeader IsNot Nothing Then
                    If PSISector.MFMHeader.Cylinder <> PSISector.Track Then
                        Return False
                    End If

                    If PSISector.MFMHeader.Head <> PSISector.Side Then
                        Return False
                    End If

                    If PSISector.MFMHeader.DeletedDAM Or PSISector.MFMHeader.MissingDAM Or PSISector.MFMHeader.IDFieldCRCError Or PSISector.MFMHeader.DataFieldCRCError Then
                        Return False
                    End If
                End If

                Return True
            End Function

            Private Sub ProcessSector(PSISector As PSISector, MaxSectors As UShort)
                Dim BitstreamSector As BitstreamSector

                Dim IsStandard = IsStandardSector(PSISector, MaxSectors)
                If IsStandard Then
                    BitstreamSector = GetSector(PSISector.Track, PSISector.Side, PSISector.Sector)
                    If BitstreamSector Is Nothing Then
                        If PSISector.Size > 0 And PSISector.Size < 512 Then
                            Dim Buffer = New Byte(511) {}
                            Array.Copy(PSISector.Data, 0, Buffer, 0, PSISector.Size)
                            BitstreamSector = New BitstreamSector(Buffer, Buffer.Length, False)
                            SetSector(PSISector.Track, PSISector.Side, PSISector.Sector, BitstreamSector)
                        ElseIf PSISector.Size = 512 Then
                            BitstreamSector = New BitstreamSector(PSISector.Data, PSISector.Size, IsStandard)
                            SetSector(PSISector.Track, PSISector.Side, PSISector.Sector, BitstreamSector)
                        End If
                    Else
                        BitstreamSector.IsStandard = False
                    End If
                End If
            End Sub

            Private Sub ProcessSector1024(PSISector As PSISector)
                Dim BitstreamSector As BitstreamSector

                Dim IsStandard = IsStandardSector(PSISector, 4)
                If IsStandard And PSISector.Size = 1024 Then
                    For i = 0 To 1
                        Dim NewSectorId = (PSISector.Sector - 1) * 2 + 1 + i
                        BitstreamSector = GetSector(PSISector.Track, PSISector.Side, NewSectorId)
                        If BitstreamSector Is Nothing Then
                            Dim Buffer = New Byte(511) {}
                            Array.Copy(PSISector.Data, Buffer.Length * i, Buffer, 0, Buffer.Length)
                            BitstreamSector = New BitstreamSector(Buffer, Buffer.Length, False)
                            SetSector(PSISector.Track, PSISector.Side, NewSectorId, BitstreamSector)
                        Else
                            BitstreamSector.IsStandard = False
                        End If
                    Next
                End If
            End Sub
        End Class
    End Namespace
End Namespace
