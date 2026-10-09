Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats.PSI
    Public Class PSIFloppyImage
        Inherits MappedFloppyImage
        Implements IFloppyImage
        Implements IImageFieldSource

        Private Enum PSIImageField As UShort
            Comment = 1
            DataCrcError = 2
        End Enum

        Public Structure PSIDataCrcErrorEdit
            Public SectorIndex As Integer
            Public Value As Boolean

            Public Sub New(SectorIndex As Integer, Value As Boolean)
                Me.SectorIndex = SectorIndex
                Me.Value = Value
            End Sub
        End Structure

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

        Public Sub SetImageField(IsTrackField As Boolean, Track As UShort, Side As Byte, Sector As UShort, FieldId As UShort, Value As Object) Implements IImageFieldSource.SetImageField
            Dim Field = CType(FieldId, PSIImageField)
            Dim RefreshMap As Boolean

            If Field = PSIImageField.DataCrcError Then
                If IsTrackField AndAlso TypeOf Value Is Boolean Then
                    SetDataCrcError(Sector, CBool(Value))
                    RefreshMap = True
                End If
            ElseIf Not IsTrackField AndAlso Field = PSIImageField.Comment AndAlso TypeOf Value Is String Then
                _Image.Comment = CStr(Value)
            End If

            If RefreshMap Then
                BuildSectorMap()
                InitProtectedSectors()
            End If
        End Sub

        Public Function UpdateProperties(Comment As String, DataCrcErrors As IEnumerable(Of PSIDataCrcErrorEdit)) As Boolean
            Dim Changes As New List(Of ImageFieldChange)
            Dim OriginalComment = If(_Image.Comment, "")
            Dim NewComment = If(Comment, "")
            If Not String.Equals(OriginalComment, NewComment, StringComparison.Ordinal) Then
                Changes.Add(New ImageFieldChange(False, 0, 0, 0, PSIImageField.Comment, OriginalComment, NewComment))
            End If

            If DataCrcErrors IsNot Nothing Then
                For Each Edit In DataCrcErrors
                    If Edit.SectorIndex < 0 OrElse Edit.SectorIndex >= _Image.Sectors.Count OrElse Edit.SectorIndex > UShort.MaxValue Then
                        Continue For
                    End If

                    Dim Sector = _Image.Sectors(Edit.SectorIndex)
                    If Sector.HasDataCRCError = Edit.Value Then
                        Continue For
                    End If

                    Changes.Add(New ImageFieldChange(True, Sector.Track, Sector.Side, CUShort(Edit.SectorIndex), PSIImageField.DataCrcError, Sector.HasDataCRCError, Edit.Value))
                Next
            End If

            Return History.CommitImageFields(Changes)
        End Function

        Public Overrides Function SaveToFile(FilePath As String) As Boolean Implements IFloppyImage.SaveToFile
            Return _Image.Export(FilePath)
        End Function

        Private Sub SetDataCrcError(SectorIndex As UShort, Value As Boolean)
            If SectorIndex >= _Image.Sectors.Count Then
                Exit Sub
            End If

            _Image.Sectors(SectorIndex).HasDataCRCError = Value
        End Sub

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
                        TrackData.FirstSectorId = TrackInfo.FirstSector
                        TrackData.LastSectorId = TrackInfo.LastSector
                        TrackData.SectorSize = TrackInfo.SectorSize
                        TrackData.Encoding = BitstreamTrackType.MFM
                        TrackData.SectorCount = TrackInfo.SectorCount
                    End If

                    If TrackData.FirstSectorId = 1 And TrackData.LastSectorId = 4 And TrackData.SectorSize = 1024 Then
                        ProcessSector1024(PSISector)
                    Else
                        ProcessSector(PSISector, MaxSectors)
                    End If

                End If
            Next
        End Sub

        Private Function CalculateHash(HashAlgorithm As HashAlgorithm) As String
            For Each PSISector In _Image.Sectors
                If Not PSISector.IsAlternateSector And Not PSISector.HasDataCRCError Then
                    HashAlgorithm.TransformBlock(PSISector.Data, 0, PSISector.Data.Length, Nothing, 0)
                End If
            Next
            HashAlgorithm.TransformFinalBlock(New Byte(0) {}, 0, 0)

            Return HashBytesToString(HashAlgorithm.Hash)
        End Function

        Private Function IsMappableSector(PSISector As PSISector, MaxSectors As UShort) As Boolean
            If PSISector.Sector < 1 Or PSISector.Sector > MaxSectors Then
                Return False
            End If

            If PSISector.Data Is Nothing Then
                Return False
            End If

            If PSISector.MFMHeader IsNot Nothing Then
                If PSISector.MFMHeader.Cylinder <> PSISector.Track Then
                    Return False
                End If

                If PSISector.MFMHeader.Head <> PSISector.Side Then
                    Return False
                End If

                ' CRC-error sectors are still mapped. The image stores their data.
                If PSISector.MFMHeader.DeletedDAM Or PSISector.MFMHeader.MissingDAM Then
                    Return False
                End If
            End If

            Return True
        End Function

        Private Function IsStandardSector(PSISector As PSISector, MaxSectors As UShort) As Boolean
            If Not IsMappableSector(PSISector, MaxSectors) Then
                Return False
            End If

            If PSISector.HasDataCRCError Then
                Return False
            End If

            If PSISector.MFMHeader IsNot Nothing Then
                If PSISector.MFMHeader.IDFieldCRCError Or PSISector.MFMHeader.DataFieldCRCError Then
                    Return False
                End If
            End If

            Return True
        End Function

        Private Sub ProcessSector(PSISector As PSISector, MaxSectors As UShort)
            Dim BitstreamSector As BitstreamSector

            If IsMappableSector(PSISector, MaxSectors) Then
                Dim IsStandard = IsStandardSector(PSISector, MaxSectors)
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

            If IsMappableSector(PSISector, 4) And PSISector.Size = 1024 Then
                For i = 0 To 1
                    Dim NewSectorId = (PSISector.Sector - 1) * 2 + 1 + i
                    BitstreamSector = GetSector(PSISector.Track, PSISector.Side, NewSectorId)
                    If BitstreamSector Is Nothing Then
                        Dim Buffer = New Byte(511) {}
                        Array.Copy(PSISector.Data, Buffer.Length * i, Buffer, 0, Buffer.Length)
                        BitstreamSector = New BitstreamSector(Buffer, Buffer.Length, False) With {
                            .IsTranslated = True
                        }
                        SetSector(PSISector.Track, PSISector.Side, NewSectorId, BitstreamSector)
                    Else
                        BitstreamSector.IsStandard = False
                    End If
                Next
            End If
        End Sub
    End Class
End Namespace
