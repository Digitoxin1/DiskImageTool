Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace HFE
        Public Class HFEByteArray
            Inherits Bitstream.BitstreamByteArray
            Implements IByteArray

            Private ReadOnly _Image As HFEImage

            Public Sub New(Image As HFEImage, DiskFormat As FloppyDiskFormat)
                MyBase.New(Image)

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
            End Sub

            Private Function CalculateHash(HashAlgorithm As HashAlgorithm) As String
                Dim HFETrack As HFETrack
                Dim OutputBuffer() As Byte

                For Track = 0 To _Image.TrackCount - 1 Step _Image.TrackStep
                    For Side = 0 To _Image.SideCount - 1
                        HFETrack = _Image.GetTrack(Track, Side)
                        For Each MFMSector In HFETrack.MFMData.Sectors
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
                Dim HFETrack As HFETrack

                Dim TrackCount = Math.Ceiling(_Image.TrackCount / _Image.TrackStep)

                SetTracks(TrackCount, _Image.SideCount)

                For Track = 0 To _Image.TrackCount - 1 Step _Image.TrackStep
                    For Side = 0 To _Image.SideCount - 1
                        Dim MappedTrack = Track \ _Image.TrackStep
                        HFETrack = _Image.GetTrack(Track, Side)

                        SetTrack(MappedTrack, Side, HFETrack.MFMData.FirstSector, HFETrack.MFMData.LastSector, HFETrack.BitstreamTrackType)

                        For Each MFMSector In HFETrack.MFMData.Sectors
                            If MFMSector.DAMFound Then
                                If MFMSector.SectorId >= 1 And MFMSector.SectorId <= SECTOR_COUNT Then
                                    Dim IsStandard = IBM_MFM.IsStandardSector(MFMSector, MappedTrack, Side)
                                    If IsStandard Then
                                        Dim BitstreamSector As New BitstreamSector(MFMSector) With {
                                            .IsStandard = IsStandard
                                        }
                                        Dim Sector = GetSector(MappedTrack, Side, MFMSector.SectorId)
                                        If Sector Is Nothing Then
                                            SetSector(MappedTrack, Side, MFMSector.SectorId, BitstreamSector)
                                        Else
                                            Sector.IsStandard = False
                                        End If
                                    End If
                                End If
                            End If
                        Next
                    Next
                Next
            End Sub

            Public ReadOnly Property Image As HFEImage
                Get
                    Return _Image
                End Get
            End Property

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IByteArray.ImageType
                Get
                    Return FloppyImageType.HFEImage
                End Get
            End Property

            Public Overrides ReadOnly Property IsBitstreamImage As Boolean Implements IByteArray.IsBitstreamImage
                Get
                    Return True
                End Get
            End Property

            Public Overrides Function SaveToFile(FilePath As String) As Boolean Implements IByteArray.SaveToFile
                Return _Image.Export(FilePath)
            End Function
        End Class
    End Namespace
End Namespace


