Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace IMD
        Public Class IMDFloppyImage
            Inherits MappedFloppyImage
            Implements IFloppyImage

            Private ReadOnly _Image As IMDImage

            Public Sub New(Image As IMDImage, DiskFormat As FloppyDiskFormat)
                MyBase.New(Nothing)

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
            End Sub

            Public ReadOnly Property Image As IMDImage
                Get
                    Return _Image
                End Get
            End Property

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IFloppyImage.ImageType
                Get
                    Return FloppyImageType.IMDImage
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
                Dim TrackData As TrackData

                SetTracks(_Image.TrackCount, _Image.SideCount)

                For Each Track In _Image.Tracks
                    TrackData = SetTrack(Track.Track, Track.Side)
                    TrackData.SectorSize = Track.GetSizeBytes
                    TrackData.Encoding = GetEncoding(Track.Mode)
                    TrackData.FirstSector = Track.FirstSector
                    TrackData.LastSector = Track.LastSector

                    If TrackData.FirstSector = 1 And TrackData.LastSector = 4 And TrackData.SectorSize = 1024 Then
                        ProcessSectors1024(Track)
                    Else
                        ProcessSectors(Track)
                    End If
                Next
            End Sub

            Private Function CalculateHash(HashAlgorithm As HashAlgorithm) As String
                Dim OutputBuffer() As Byte

                For Each Track In _Image.Tracks
                    If Track.IsMFM Then
                        For Each Sector In Track.Sectors
                            If Not Sector.Unavailable And Not Sector.ChecksumError Then
                                OutputBuffer = New Byte(Sector.Data.Length - 1) {}
                                HashAlgorithm.TransformBlock(Sector.Data, 0, Sector.Data.Length, OutputBuffer, 0)
                            End If
                        Next
                    End If
                Next
                HashAlgorithm.TransformFinalBlock(New Byte(0) {}, 0, 0)

                Return HashBytesToString(HashAlgorithm.Hash)
            End Function

            Private Function GetEncoding(Mode As TrackMode) As BitstreamTrackType
                If Mode = TrackMode.FM250kbps Or Mode = TrackMode.FM300kbps Or Mode = TrackMode.FM500kbps Then
                    Return BitstreamTrackType.FM
                ElseIf Mode = TrackMode.MFM250kbps Or Mode = TrackMode.MFM300kbps Or Mode = TrackMode.MFM500kbps Then
                    Return BitstreamTrackType.MFM
                Else
                    Return BitstreamTrackType.Other
                End If
            End Function

            Private Function IsStandardSector(Track As IMDTrack, Sector As IMDSector, Optional Size As UInteger = 512) As Boolean
                If Sector.Unavailable Or Sector.Deleted Or Sector.ChecksumError Then
                    Return False
                End If

                If Sector.Data.Length <> Size Then
                    Return False
                End If

                If Sector.Track <> Track.Track Or Sector.Side <> Track.Side Then
                    Return False
                End If

                Return True
            End Function

            Private Sub ProcessSectors(Track As IMDTrack)
                Dim BitstreamSector As BitstreamSector
                Dim IsStandard As Boolean

                For Each Sector In Track.Sectors
                    If Sector.SectorId >= 1 And Sector.SectorId <= SECTOR_COUNT Then
                        IsStandard = IsStandardSector(Track, Sector)
                        If IsStandard Then
                            BitstreamSector = GetSector(Track.Track, Track.Side, Sector.SectorId)
                            If BitstreamSector Is Nothing Then
                                BitstreamSector = New BitstreamSector(Sector.Data, Sector.Data.Length) With {
                                    .IsStandard = IsStandard
                                }
                                SetSector(Track.Track, Track.Side, Sector.SectorId, BitstreamSector)
                            Else
                                BitstreamSector.IsStandard = False
                            End If
                        End If
                    End If
                Next
            End Sub

            Private Sub ProcessSectors1024(Track As IMDTrack)
                Dim BitstreamSector As BitstreamSector
                Dim NewSectorId As Integer
                Dim IsStandard As Boolean
                Dim Buffer() As Byte

                For Each Sector In Track.Sectors
                    If Sector.SectorId >= 1 And Sector.SectorId <= 4 Then
                        IsStandard = IsStandardSector(Track, Sector, 1024)
                        If IsStandard Then
                            For i = 0 To 1
                                NewSectorId = (Sector.SectorId - 1) * 2 + 1 + i
                                BitstreamSector = GetSector(Track.Track, Track.Side, NewSectorId)
                                If BitstreamSector Is Nothing Then
                                    Buffer = New Byte(511) {}
                                    Array.Copy(Sector.Data, 512 * i, Buffer, 0, 512)
                                    BitstreamSector = New BitstreamSector(Buffer, 512) With {
                                        .IsStandard = False
                                    }
                                    SetSector(Track.Track, Track.Side, NewSectorId, BitstreamSector)
                                Else
                                    BitstreamSector.IsStandard = False
                                End If
                            Next
                        End If
                    End If
                Next
            End Sub
        End Class
    End Namespace
End Namespace
