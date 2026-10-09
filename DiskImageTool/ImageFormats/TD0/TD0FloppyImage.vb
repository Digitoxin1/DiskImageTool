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
            CrcError = 2
        End Enum

        Public Structure TD0CrcErrorEdit
            Public TrackIndex As Integer
            Public SectorIndex As Integer
            Public Value As Boolean

            Public Sub New(TrackIndex As Integer, SectorIndex As Integer, Value As Boolean)
                Me.TrackIndex = TrackIndex
                Me.SectorIndex = SectorIndex
                Me.Value = Value
            End Sub
        End Structure

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

        Public Sub SetImageField(IsTrackField As Boolean, Track As UShort, Side As Byte, Sector As UShort, FieldId As UShort, Value As Object) Implements IImageFieldSource.SetImageField
            Dim Field = CType(FieldId, TD0ImageField)
            Dim RefreshMap As Boolean

            If Field = TD0ImageField.CrcError Then
                If IsTrackField AndAlso TypeOf Value Is Boolean Then
                    SetCrcError(Track, Sector, CBool(Value))
                    RefreshMap = True
                End If
            ElseIf Not IsTrackField AndAlso Field = TD0ImageField.CommentBlock Then
                Dim CommentBytes = TryCast(Value, Byte())
                If CommentBytes Is Nothing Then
                    _Image.SetComment(Nothing)
                    _Image.Header.HasCommentBlock = False
                Else
                    _Image.SetComment(New TD0Comment(CommentBytes, 0))
                    _Image.Header.HasCommentBlock = True
                End If

                _Image.Header.RefreshStoredCrc16()
            End If

            If RefreshMap Then
                BuildSectorMap()
                InitProtectedSectors()
            End If
        End Sub

        Public Function UpdateProperties(CommentBytes As Byte(), CrcErrors As IEnumerable(Of TD0CrcErrorEdit)) As Boolean
            Dim Changes As New List(Of ImageFieldChange)
            Dim OriginalComment = CommentSnapshot(_Image.Comment)
            If Not SameBytes(OriginalComment, CommentBytes) Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, TD0ImageField.CommentBlock, OriginalComment, CommentBytes))
            End If

            If CrcErrors IsNot Nothing Then
                For Each Edit In CrcErrors
                    If Edit.TrackIndex < 0 OrElse Edit.TrackIndex >= _Image.Tracks.Count OrElse Edit.TrackIndex > UShort.MaxValue Then
                        Continue For
                    End If

                    Dim Track = _Image.Tracks(Edit.TrackIndex)
                    If Edit.SectorIndex < 0 OrElse Edit.SectorIndex >= Track.Sectors.Count OrElse Edit.SectorIndex > UShort.MaxValue Then
                        Continue For
                    End If

                    Dim Original = (Track.Sectors(Edit.SectorIndex).Header.Flags And TD0SectorFlags.CrcError) <> 0
                    If Original = Edit.Value Then
                        Continue For
                    End If

                    Changes.Add(New ImageFieldChange(True, CUShort(Edit.TrackIndex), Track.Head, CUShort(Edit.SectorIndex), TD0ImageField.CrcError, Original, Edit.Value))
                Next
            End If

            Return History.CommitImageFields(Changes)
        End Function

        Private Sub SetCrcError(TrackIndex As UShort, SectorIndex As UShort, Value As Boolean)
            If TrackIndex >= _Image.Tracks.Count Then
                Exit Sub
            End If

            Dim Sectors = _Image.Tracks(TrackIndex).Sectors
            If SectorIndex >= Sectors.Count Then
                Exit Sub
            End If

            Sectors(SectorIndex).Header.HasCrcError = Value
        End Sub

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
