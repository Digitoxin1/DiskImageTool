Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats.TD0

    Public Class TD0FloppyImage
        Inherits MappedFloppyImage
        Implements IFloppyImage

        Private ReadOnly _Image As TD0Image

        Public Sub New(image As TD0Image, diskFormat As FloppyDiskFormat, bytesPerSector As UInteger)
            MyBase.New(Nothing, bytesPerSector)
            _Image = image
            BuildSectorMap()
            InitDiskFormat(diskFormat)
        End Sub

        Public ReadOnly Property Image As TD0Image
            Get
                Return _Image
            End Get
        End Property

        Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IFloppyImage.ImageType
            Get
                Return FloppyImageType.TD0Image
            End Get
        End Property

        Public Overrides Function GetCRC32() As String Implements IFloppyImage.GetCRC32
            Using h As CRC32Hash = CRC32Hash.Create()
                Return CalculateHash(h)
            End Using
        End Function

        Public Overrides Function GetMD5Hash() As String Implements IFloppyImage.GetMD5Hash
            Using h As MD5 = MD5.Create()
                Return CalculateHash(h)
            End Using
        End Function

        Public Overrides Function GetSHA1Hash() As String Implements IFloppyImage.GetSHA1Hash
            Using h As SHA1 = SHA1.Create()
                Return CalculateHash(h)
            End Using
        End Function

        Public Overrides Function SaveToFile(FilePath As String) As Boolean Implements IFloppyImage.SaveToFile
            Return _Image.Export(FilePath)
        End Function

        Private Sub BuildSectorMap()
            Dim TrackData As TrackData

            SetTracks(_Image.TrackCount, _Image.SideCount)

            For Each Track In _Image.Tracks
                TrackData = SetTrack(Track.Cylinder, Track.Head)
                If Track.Sectors.Count > 0 AndAlso Track.Sectors(0) IsNot Nothing Then
                    TrackData.SectorSize = CUInt(Math.Max(0, Track.Sectors(0).GetSizeBytes()))
                End If
                TrackData.Encoding = GetEncoding(Track.IsFM)
                TrackData.FirstSectorId = Track.FirstSectorId
                TrackData.LastSectorId = Track.LastSectorId
                TrackData.SectorCount = Track.Sectors.Count

                If TrackData.FirstSectorId = 1 And TrackData.LastSectorId = 4 And TrackData.SectorSize = 1024 Then
                    ProcessSectors1024(Track)
                Else
                    ProcessSectors(Track)
                End If
            Next
        End Sub

        Private Function CalculateHash(hashAlg As HashAlgorithm) As String
            For Each trk In _Image.Tracks
                For Each s In trk.Sectors
                    If s IsNot Nothing AndAlso Not s.Unavailable AndAlso Not s.ChecksumError AndAlso s.Data IsNot Nothing Then
                        hashAlg.TransformBlock(s.Data, 0, s.Data.Length, Nothing, 0)
                    End If
                Next
            Next
            hashAlg.TransformFinalBlock(New Byte(0) {}, 0, 0)
            Return HashBytesToString(hashAlg.Hash)
        End Function

        Private Function GetEncoding(IsFM As Boolean) As BitstreamTrackType
            Return If(IsFM, BitstreamTrackType.FM, BitstreamTrackType.MFM)
        End Function

        Private Function IsStandardSector(Track As TD0Track, Sector As TD0Sector, MaxSectors As Byte) As Boolean
            If Sector Is Nothing Then
                Return False
            End If

            If Sector.SectorId < 1 Or Sector.SectorId > MaxSectors Then
                Return False
            End If

            If Sector.Unavailable Or Sector.Deleted Or Sector.ChecksumError Then
                Return False
            End If

            If Sector.Cylinder <> Track.Cylinder Or Sector.Head <> Track.Head Then
                Return False
            End If

            Return True
        End Function

        Private Sub ProcessSectors(Track As TD0Track)
            Dim BitstreamSector As BitstreamSector
            Dim SectorSize As UInteger
            Dim IsStandard As Boolean
            Dim Buffer() As Byte

            For Each Sector In Track.Sectors
                IsStandard = IsStandardSector(Track, Sector, SECTOR_COUNT)
                If IsStandard Then
                    BitstreamSector = GetSector(Track.Cylinder, Track.Head, Sector.SectorId)
                    If BitstreamSector Is Nothing Then
                        SectorSize = Sector.Data.Length
                        If SectorSize > 0 And SectorSize < 512 Then
                            Buffer = New Byte(511) {}
                            Array.Copy(Sector.Data, 0, Buffer, 0, SectorSize)
                            BitstreamSector = New BitstreamSector(Buffer, Buffer.Length, False)
                            SetSector(Track.Cylinder, Track.Head, Sector.SectorId, BitstreamSector)
                        ElseIf SectorSize = 512 Then
                            BitstreamSector = New BitstreamSector(Sector.Data, Sector.Data.Length, IsStandard)
                            SetSector(Track.Cylinder, Track.Head, Sector.SectorId, BitstreamSector)
                        End If
                    Else
                        BitstreamSector.IsStandard = False
                    End If
                End If
            Next
        End Sub

        Private Sub ProcessSectors1024(Track As TD0Track)
            Dim BitstreamSector As BitstreamSector
            Dim NewSectorId As Integer
            Dim IsStandard As Boolean
            Dim Buffer() As Byte

            For Each Sector In Track.Sectors
                IsStandard = IsStandardSector(Track, Sector, 4)
                If IsStandard And Sector.Data.Length = 1024 Then
                    For i = 0 To 1
                        NewSectorId = (Sector.SectorId - 1) * 2 + 1 + i
                        BitstreamSector = GetSector(Track.Cylinder, Track.Head, NewSectorId)
                        If BitstreamSector Is Nothing Then
                            Buffer = New Byte(511) {}
                            Array.Copy(Sector.Data, Buffer.Length * i, Buffer, 0, Buffer.Length)
                            BitstreamSector = New BitstreamSector(Buffer, Buffer.Length, False) With {
                                .IsTranslated = True
                            }
                            SetSector(Track.Cylinder, Track.Head, NewSectorId, BitstreamSector)
                        Else
                            BitstreamSector.IsStandard = False
                        End If
                    Next
                End If
            Next
        End Sub
    End Class
End Namespace
