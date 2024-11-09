Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace PSI
        Public Class PSIFloppyImage
            Inherits MappedFloppyImage
            Implements IFloppyImage

            Private ReadOnly _Image As PSISectorImage

            Public Sub New(Image As PSISectorImage, DiskFormat As FloppyDiskFormat)
                MyBase.New(Nothing)

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
                        Dim Track = GetTrack(PSISector.Track, PSISector.Side)

                        If Track Is Nothing Then
                            Track = SetTrack(PSISector.Track, PSISector.Side)
                            Track.FirstSector = PSISector.Sector
                            Track.LastSector = PSISector.Sector
                            Track.SectorSize = PSISector.Size
                            Track.Encoding = BitstreamTrackType.MFM
                        Else
                            If PSISector.Sector < Track.FirstSector Then
                                Track.FirstSector = PSISector.Sector
                            End If

                            If PSISector.Sector > Track.LastSector Then
                                Track.LastSector = PSISector.Sector
                            End If
                            If PSISector.Size <> Track.SectorSize Then
                                Track.SectorSize = -1
                            End If
                        End If

                        If PSISector.Sector >= 1 And PSISector.Sector <= SECTOR_COUNT Then
                            Dim IsStandard = (PSISector.Size = 512 And Not PSISector.HasDataCRCError And PSISector.Sector <= MaxSectors)
                            If IsStandard Then
                                Dim BitstreamSector As New BitstreamSector(PSISector.Data, PSISector.Size) With {
                                    .IsStandard = IsStandard
                                }
                                Dim Sector = GetSector(PSISector.Track, PSISector.Side, PSISector.Sector)
                                If Sector Is Nothing Then
                                    SetSector(PSISector.Track, PSISector.Side, PSISector.Sector, BitstreamSector)
                                Else
                                    Sector.IsStandard = False
                                End If
                            End If
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
        End Class
    End Namespace
End Namespace
