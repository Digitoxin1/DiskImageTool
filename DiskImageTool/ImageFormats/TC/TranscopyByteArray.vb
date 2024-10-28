Imports System.Security.Cryptography
Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace TC

        Public Class TranscopyByteArray
            Inherits Bitstream.BitstreamByteArray
            Implements IByteArray

            Private ReadOnly _Image As TransCopyImage

            Public Sub New(Image As TransCopyImage, DiskFormat As FloppyDiskFormat)
                MyBase.New(Image)

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
            End Sub

            Private Function CalculateHash(HashAlgorithm As HashAlgorithm) As String
                Dim TranscopyCylinder As TransCopyCylinder
                Dim OutputBuffer() As Byte

                For Cylinder = 0 To _Image.CylinderEnd
                    For Side = 0 To _Image.Sides - 1
                        TranscopyCylinder = _Image.GetCylinder(Cylinder, Side)
                        If TranscopyCylinder.MFMData IsNot Nothing Then
                            For Each MFMSector In TranscopyCylinder.MFMData.Sectors
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
                Dim TranscopyCylinder As TransCopyCylinder

                SetTracks(_Image.CylinderEnd + 1, _Image.Sides)

                For Cylinder = 0 To _Image.CylinderEnd
                    For Side = 0 To _Image.Sides - 1
                        TranscopyCylinder = _Image.GetCylinder(Cylinder, Side)

                        Dim FirstSector As Integer = -1
                        Dim LastSector As Integer = -1
                        Dim TrackType As BitstreamTrackType = BitstreamTrackType.Other

                        If TranscopyCylinder.MFMData IsNot Nothing Then
                            FirstSector = TranscopyCylinder.MFMData.FirstSector
                            LastSector = TranscopyCylinder.MFMData.LastSector
                        End If

                        If TranscopyCylinder.Bitstream.Length > 0 Then
                            TrackType = TranscopyCylinder.BitstreamTrackType
                        End If

                        SetTrack(Cylinder, Side, FirstSector, LastSector, TrackType)

                        If TranscopyCylinder.MFMData IsNot Nothing Then
                            For Each MFMSector In TranscopyCylinder.MFMData.Sectors
                                If MFMSector.DAMFound Then
                                    If MFMSector.SectorId >= 1 And MFMSector.SectorId <= SECTOR_COUNT Then
                                        Dim IsStandard = IBM_MFM.IsStandardSector(MFMSector, Cylinder, Side)
                                        If IsStandard Then
                                            Dim BitstreamSector As New BitstreamSector(MFMSector) With {
                                                .IsStandard = IsStandard
                                            }
                                            Dim Sector = GetSector(Cylinder, Side, MFMSector.SectorId)
                                            If Sector Is Nothing Then
                                                SetSector(Cylinder, Side, MFMSector.SectorId, BitstreamSector)
                                            Else
                                                Sector.IsStandard = False
                                            End If
                                        End If
                                    End If
                                End If
                            Next
                        End If
                    Next
                Next
            End Sub

            Public ReadOnly Property Image As TransCopyImage
                Get
                    Return _Image
                End Get
            End Property

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IByteArray.ImageType
                Get
                    Return FloppyImageType.TranscopyImage
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