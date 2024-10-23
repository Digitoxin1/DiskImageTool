Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace _86F
        Public Class _86FByteArray
            Inherits Bitstream.BitstreamByteArray
            Implements IByteArray

            Private ReadOnly _Image As _86FImage

            Public Sub New(Image As _86FImage, DiskFormat As FloppyDiskFormat)
                MyBase.New(Image)

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
            End Sub

            Private Function CalculateHash(HashAlgorithm As HashAlgorithm) As String
                Dim F86Track As _86FTrack
                Dim OutputBuffer() As Byte

                For Track = 0 To _Image.TrackCount - 1
                    For Side = 0 To _Image.Sides - 1
                        F86Track = _Image.GetTrack(Track, Side)
                        If F86Track IsNot Nothing AndAlso F86Track.MFMData IsNot Nothing Then
                            For Each MFMSector In F86Track.MFMData.Sectors
                                If MFMSector.DAMFound Then
                                    If MFMSector.InitialChecksumValid Then
                                        OutputBuffer = New Byte(MFMSector.Data.Length - 1) {}
                                        HashAlgorithm.TransformBlock(MFMSector.Data, 0, MFMSector.Data.Length, OutputBuffer, 0)
                                    End If
                                End If
                            Next
                        End If
                    Next
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
                Dim F86Track As _86FTrack

                SetTracks(_Image.TrackCount, _Image.Sides)

                For Track = 0 To _Image.TrackCount - 1
                    For Side = 0 To _Image.Sides - 1
                        F86Track = _Image.GetTrack(Track, Side)
                        If F86Track IsNot Nothing Then
                            Dim FirstSector As Integer = -1
                            Dim LastSector As Integer = -1
                            Dim TrackType As BitstreamTrackType = BitstreamTrackType.Other

                            If F86Track.MFMData IsNot Nothing Then
                                FirstSector = F86Track.MFMData.FirstSector
                                LastSector = F86Track.MFMData.LastSector
                            End If

                            If F86Track.Bitstream.Length > 0 Then
                                TrackType = F86Track.BitstreamTrackType
                            End If

                            SetTrack(Track, Side, FirstSector, LastSector, TrackType)

                            If F86Track.MFMData IsNot Nothing Then
                                For Each MFMSector In F86Track.MFMData.Sectors
                                    If MFMSector.DAMFound Then
                                        If MFMSector.SectorId >= 1 And MFMSector.SectorId <= SECTOR_COUNT Then
                                            Dim BitstreamSector As New BitstreamSector(MFMSector) With {
                                               .IsStandard = IBM_MFM.IsStandardSector(MFMSector, Track, Side)
                                            }

                                            Dim Sector = GetSector(Track, Side, MFMSector.SectorId)
                                            If Sector Is Nothing Then
                                                SetSector(Track, Side, MFMSector.SectorId, BitstreamSector)
                                            Else
                                                Sector.IsStandard = False
                                            End If
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    Next
                Next
            End Sub

            Public ReadOnly Property Image As _86FImage
                Get
                    Return _Image
                End Get
            End Property

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IByteArray.ImageType
                Get
                    Return FloppyImageType._86FImage
                End Get
            End Property

            Public Overrides Function SaveToFile(FilePath As String) As Boolean Implements IByteArray.SaveToFile
                Return _Image.Export(FilePath)
            End Function
        End Class
    End Namespace
End Namespace
