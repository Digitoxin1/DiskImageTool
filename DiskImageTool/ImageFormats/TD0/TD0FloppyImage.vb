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
            SetTracks(_Image.TrackCount, _Image.SideCount)

            For Each trk In _Image.Tracks
                Dim td As TrackData = SetTrack(trk.Cylinder, trk.Head)

                td.Encoding = If(trk.IsFM, BitstreamTrackType.FM, BitstreamTrackType.MFM)

                If trk.Sectors.Count > 0 AndAlso trk.Sectors(0) IsNot Nothing Then
                    td.SectorSize = CUInt(Math.Max(0, trk.Sectors(0).GetSizeBytes()))
                End If

                td.FirstSectorId = If(trk.FirstSectorId < 0, 0, trk.FirstSectorId)
                td.LastSectorId = If(trk.LastSectorId < 0, 0, trk.LastSectorId)
                td.SectorCount = trk.Sectors.Count

                For Each s In trk.Sectors
                    If s Is Nothing Then
                        Continue For
                    End If

                    If s.SectorId < 1 OrElse s.SectorId > SECTOR_COUNT Then
                        Continue For
                    End If

                    If s.Unavailable OrElse s.Data Is Nothing Then
                        Continue For
                    End If

                    Dim bs = GetSector(trk.Cylinder, trk.Head, s.SectorId)

                    If bs Is Nothing Then
                        Dim buf = s.Data

                        If buf.Length < 512 Then
                            Dim tmp(511) As Byte
                            Array.Copy(buf, 0, tmp, 0, buf.Length)
                            bs = New BitstreamSector(tmp, tmp.Length, True)
                        ElseIf buf.Length = 512 Then
                            bs = New BitstreamSector(buf, buf.Length, True)
                        Else
                            bs = New BitstreamSector(buf, buf.Length, False)
                        End If

                        SetSector(trk.Cylinder, trk.Head, s.SectorId, bs)
                    Else
                        bs.IsStandard = False
                    End If
                Next
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
    End Class
End Namespace
