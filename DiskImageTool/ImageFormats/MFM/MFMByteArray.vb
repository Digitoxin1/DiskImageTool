Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace MFM
        Public Class MFMByteArray
            Inherits Bitstream.BitstreamByteArray
            Implements IByteArray

            Private ReadOnly _Image As MFMImage

            Public Sub New(Image As MFMImage, DiskFormat As FloppyDiskFormat)
                MyBase.New()

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
            End Sub

            Private Sub BuildSectorMap()
                Dim MFMTrack As MFMTrack

                SetTracks(_Image.TrackCount, _Image.SideCount)

                For Track = 0 To _Image.TrackCount - 1 Step _Image.TrackStep
                    For Side = 0 To _Image.SideCount - 1
                        Dim MappedTrack = Track \ _Image.TrackStep
                        MFMTrack = _Image.GetTrack(Track, Side)

                        SetTrack(MappedTrack, Side, MFMTrack.MFMData.FirstSector, MFMTrack.MFMData.LastSector)

                        For Each MFMSector In MFMTrack.MFMData.Sectors
                            If MFMSector.DAMFound Then
                                If MFMSector.SectorId >= 1 And MFMSector.SectorId <= SECTOR_COUNT Then
                                    Dim BitstreamSector As New BitstreamSector With {
                                       .Data = MFMSector.Data,
                                       .Size = MFMSector.GetSizeBytes,
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
                Return _Image.Export(FilePath, True)
            End Function
        End Class
    End Namespace
End Namespace
