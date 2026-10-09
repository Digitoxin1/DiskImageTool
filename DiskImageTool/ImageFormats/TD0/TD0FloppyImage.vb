Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats.TD0

    Public Class TD0FloppyImage
        Inherits MappedFloppyImage
        Implements IFloppyImage
        Implements IImageFieldSource

        Private Enum TD0ImageField As UShort
            CommentBlock = 1
        End Enum

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

        Public Sub SetImageField(IsTrackField As Boolean, Track As UShort, Side As Byte, FieldId As UShort, Value As Object) Implements IImageFieldSource.SetImageField
            If IsTrackField OrElse CType(FieldId, TD0ImageField) <> TD0ImageField.CommentBlock Then
                Exit Sub
            End If

            Dim CommentBytes = TryCast(Value, Byte())
            If CommentBytes Is Nothing Then
                _Image.SetComment(Nothing)
                _Image.Header.HasCommentBlock = False
            Else
                _Image.SetComment(New TD0Comment(CommentBytes, 0))
                _Image.Header.HasCommentBlock = True
            End If

            _Image.Header.RefreshStoredCrc16()
        End Sub

        Public Function UpdateComment(CommentBytes As Byte()) As Boolean
            Dim Original = CommentSnapshot(_Image.Comment)
            If SameBytes(Original, CommentBytes) Then
                Return False
            End If

            Dim Changes As New List(Of ImageFieldChange) From {
                New ImageFieldChange(False, 0, 0, TD0ImageField.CommentBlock, Original, CommentBytes)
            }
            Return History.CommitImageFields(Changes)
        End Function

        Private Shared Function CommentSnapshot(Comment As TD0Comment) As Byte()
            If Comment Is Nothing Then
                Return Nothing
            End If

            Return Comment.GetBytes()
        End Function

        Private Shared Function SameBytes(Left As Byte(), Right As Byte()) As Boolean
            If Left Is Nothing AndAlso Right Is Nothing Then
                Return True
            End If

            If Left Is Nothing OrElse Right Is Nothing OrElse Left.Length <> Right.Length Then
                Return False
            End If

            For Index = 0 To Left.Length - 1
                If Left(Index) <> Right(Index) Then
                    Return False
                End If
            Next

            Return True
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
                    TrackData.SectorSize = CUInt(Math.Max(0, Track.Sectors(0).Header.GetSectorSizeBytes))
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
                    If s IsNot Nothing AndAlso Not s.Header.NoData AndAlso Not s.Header.HasCrcError AndAlso s.Data IsNot Nothing Then
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

        Private Function IsMappableSector(Track As TD0Track, Sector As TD0Sector, MaxSectors As Byte) As Boolean
            If Sector Is Nothing Then
                Return False
            End If

            If Sector.Header.SectorId < 1 Or Sector.Header.SectorId > MaxSectors Then
                Return False
            End If

            ' CRC-error sectors are still mapped. TeleDisk stores their data.
            If Sector.Header.NoData Or Sector.Header.IsDeletedDataMark Or Sector.Data Is Nothing Then
                Return False
            End If

            If Sector.Header.Cylinder <> Track.Cylinder Or Sector.Header.Head <> Track.Head Then
                Return False
            End If

            Return True
        End Function

        Private Function IsStandardSector(Track As TD0Track, Sector As TD0Sector, MaxSectors As Byte) As Boolean
            Return IsMappableSector(Track, Sector, MaxSectors) AndAlso Not Sector.Header.HasCrcError
        End Function

        Private Sub ProcessSectors(Track As TD0Track)
            Dim BitstreamSector As BitstreamSector
            Dim SectorSize As UInteger
            Dim IsStandard As Boolean
            Dim Buffer() As Byte

            For Each Sector In Track.Sectors
                If IsMappableSector(Track, Sector, SECTOR_COUNT) Then
                    IsStandard = IsStandardSector(Track, Sector, SECTOR_COUNT)
                    BitstreamSector = GetSector(Track.Cylinder, Track.Head, Sector.Header.SectorId)
                    If BitstreamSector Is Nothing Then
                        SectorSize = Sector.Data.Length
                        If SectorSize > 0 And SectorSize < 512 Then
                            Buffer = New Byte(511) {}
                            Array.Copy(Sector.Data, 0, Buffer, 0, SectorSize)
                            BitstreamSector = New BitstreamSector(Buffer, Buffer.Length, False)
                            SetSector(Track.Cylinder, Track.Head, Sector.Header.SectorId, BitstreamSector)
                        ElseIf SectorSize = 512 Then
                            BitstreamSector = New BitstreamSector(Sector.Data, Sector.Data.Length, IsStandard)
                            SetSector(Track.Cylinder, Track.Head, Sector.Header.SectorId, BitstreamSector)
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
            Dim Buffer() As Byte

            For Each Sector In Track.Sectors
                If IsMappableSector(Track, Sector, 4) And Sector.Data.Length = 1024 Then
                    For i = 0 To 1
                        NewSectorId = (Sector.Header.SectorId - 1) * 2 + 1 + i
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
