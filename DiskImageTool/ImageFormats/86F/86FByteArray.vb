Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace _86F
        Public Class _86FByteArray
            Inherits Bitstream.BitstreamByteArray
            Implements IByteArray

            Private ReadOnly _Image As _86FImage

            Public Sub New(Image As _86FImage, DiskFormat As FloppyDiskFormat)
                MyBase.New()

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
            End Sub

            Private Sub BuildSectorMap()
                Dim F86Track As _86FTrack

                SetTracks(_Image.TrackCount, _Image.Sides)

                For Track = 0 To _Image.TrackCount - 1 Step _Image.TrackStep
                    For Side = 0 To _Image.Sides - 1
                        F86Track = _Image.GetTrack(Track, Side)
                        If F86Track IsNot Nothing AndAlso F86Track.MFMData IsNot Nothing Then

                            SetTrack(Track, Side, F86Track.MFMData.FirstSector, F86Track.MFMData.LastSector)

                            For Each MFMSector In F86Track.MFMData.Sectors
                                If MFMSector.DAMFound Then
                                    If MFMSector.SectorId >= 1 And MFMSector.SectorId <= SECTOR_COUNT Then
                                        Dim BitstreamSector As New BitstreamSector With {
                                           .Data = MFMSector.Data,
                                           .Size = MFMSector.GetSizeBytes,
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
                Return _Image.Export(FilePath, True)
            End Function
        End Class
    End Namespace
End Namespace
