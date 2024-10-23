Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace MFM
        Public Class MFMByteArray
            Inherits Bitstream.BitstreamByteArray
            Implements IByteArray

            Private ReadOnly _Image As MFMImage

            Public Sub New(Image As MFMImage, DiskFormat As FloppyDiskFormat)
                MyBase.New(Image)

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
            End Sub

            Private Function CalculateHash(HashAlgorithm As HashAlgorithm) As String
                Dim MFMTrack As MFMTrack
                Dim OutputBuffer() As Byte

                For Track = 0 To _Image.TrackCount - 1 Step _Image.TrackStep
                    For Side = 0 To _Image.SideCount - 1
                        MFMTrack = _Image.GetTrack(Track, Side)
                        For Each MFMSector In MFMTrack.MFMData.Sectors
                            If MFMSector.DAMFound Then
                                If MFMSector.InitialChecksumValid Then
                                    OutputBuffer = New Byte(MFMSector.Data.Length - 1) {}
                                    HashAlgorithm.TransformBlock(MFMSector.Data, 0, MFMSector.Data.Length, OutputBuffer, 0)
                                End If
                            End If
                        Next
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
                Dim MFMTrack As MFMTrack

                Dim TrackCount = Math.Ceiling(_Image.TrackCount / _Image.TrackStep)

                SetTracks(TrackCount, _Image.SideCount)

                For Track = 0 To _Image.TrackCount - 1 Step _Image.TrackStep
                    For Side = 0 To _Image.SideCount - 1
                        Dim MappedTrack = Track \ _Image.TrackStep
                        MFMTrack = _Image.GetTrack(Track, Side)

                        SetTrack(MappedTrack, Side, MFMTrack.MFMData.FirstSector, MFMTrack.MFMData.LastSector, MFMTrack.BitstreamTrackType)

                        For Each MFMSector In MFMTrack.MFMData.Sectors
                            If MFMSector.DAMFound Then
                                If MFMSector.SectorId >= 1 And MFMSector.SectorId <= SECTOR_COUNT Then
                                    Dim BitstreamSector As New BitstreamSector(MFMSector) With {
                                       .IsStandard = IBM_MFM.IsStandardSector(MFMSector, MappedTrack, Side)
                                    }

                                    Dim Sector = GetSector(MappedTrack, Side, MFMSector.SectorId)
                                    If Sector Is Nothing Then
                                        SetSector(MappedTrack, Side, MFMSector.SectorId, BitstreamSector)
                                    Else
                                        Sector.IsStandard = False
                                    End If
                                End If
                            End If
                        Next
                    Next
                Next
            End Sub

            Public ReadOnly Property Image As MFMImage
                Get
                    Return _Image
                End Get
            End Property

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IByteArray.ImageType
                Get
                    Return FloppyImageType.MFMImage
                End Get
            End Property

            Public Overrides Function SaveToFile(FilePath As String) As Boolean Implements IByteArray.SaveToFile
                Return _Image.Export(FilePath)
            End Function
        End Class
    End Namespace
End Namespace
