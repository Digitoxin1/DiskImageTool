Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace IMD
        Public Class IMDByteArray
            Inherits Bitstream.BitstreamByteArray
            Implements IByteArray

            Private ReadOnly _Image As IMDImage

            Public Sub New(Image As IMDImage, DiskFormat As FloppyDiskFormat)
                MyBase.New(Nothing)

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
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

            Public Overrides Function GetCRC32() As String Implements IByteArray.GetCRC32
                Using Hasher As CRC32Hash = CRC32Hash.Create()
                    Return CalculateHash(Hasher)
                End Using
            End Function

            Public Overrides Function GetMD5Hash() As String Implements IByteArray.GetMD5Hash
                Using Hasher As MD5 = MD5.Create()
                    Return CalculateHash(Hasher)
                End Using
            End Function

            Public Overrides Function GetSHA1Hash() As String Implements IByteArray.GetSHA1Hash
                Using Hasher As SHA1 = SHA1.Create()
                    Return CalculateHash(Hasher)
                End Using
            End Function

            Private Sub BuildSectorMap()
                SetTracks(_Image.TrackCount, _Image.SideCount)

                For Each Track In _Image.Tracks
                    Dim TrackData = SetTrack(Track.Cylinder, Track.Head, -1, -1, GetEncoding(Track.Mode))
                    For Each Sector In Track.Sectors
                        If TrackData.FirstSector = -1 Or Sector.SectorId < TrackData.FirstSector Then
                            TrackData.FirstSector = Sector.SectorId
                        End If

                        If Sector.SectorId > TrackData.LastSector Then
                            TrackData.LastSector = Sector.SectorId
                        End If

                        If Sector.SectorId >= 1 And Sector.SectorId <= SECTOR_COUNT Then
                            Dim IsStandard = IsStandardSector(Track, Sector)
                            If IsStandard Then
                                Dim BitstreamSector = GetSector(Track.Cylinder, Track.Head, Sector.SectorId)
                                If BitstreamSector Is Nothing Then
                                    BitstreamSector = New BitstreamSector(Sector.Data, Sector.Data.Length) With {
                                        .IsStandard = IsStandard
                                    }
                                    SetSector(Track.Cylinder, Track.Head, Sector.SectorId, BitstreamSector)
                                Else
                                    BitstreamSector.IsStandard = False
                                End If
                            End If
                        End If
                    Next
                Next
            End Sub

            Private Function GetEncoding(Mode As TrackMode) As BitstreamTrackType
                If Mode = TrackMode.FM250kbps Or Mode = TrackMode.FM300kbps Or Mode = TrackMode.FM500kbps Then
                    Return BitstreamTrackType.FM
                ElseIf Mode = TrackMode.MFM250kbps Or Mode = TrackMode.MFM300kbps Or Mode = TrackMode.MFM500kbps Then
                    Return BitstreamTrackType.MFM
                Else
                    Return BitstreamTrackType.Other
                End If
            End Function

            Private Function IsStandardSector(Track As IMDTrack, Sector As IMDSector) As Boolean
                If Sector.Unavailable Or Sector.Deleted Or Sector.ChecksumError Then
                    Return False
                End If

                If Sector.Data.Length <> 512 Then
                    Return False
                End If

                If Sector.Cylinder <> Track.Cylinder Or Sector.Head <> Track.Head Then
                    Return False
                End If

                Return True
            End Function

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IByteArray.ImageType
                Get
                    Return FloppyImageType.IMDImage
                End Get
            End Property

            Public Overrides ReadOnly Property IsBitstreamImage As Boolean Implements IByteArray.IsBitstreamImage
                Get
                    Return False
                End Get
            End Property

            Public Overrides Function SaveToFile(FilePath As String) As Boolean Implements IByteArray.SaveToFile
                Return _Image.Export(FilePath)
            End Function

        End Class
    End Namespace
End Namespace
