Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace TC

        Public Class TranscopyByteArray
            Inherits Bitstream.BitstreamByteArray
            Implements IByteArray

            Private ReadOnly _Image As TransCopyImage

            Public Sub New(Image As TransCopyImage, DiskFormat As FloppyDiskFormat)
                MyBase.New()

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
            End Sub

            Private Sub BuildSectorMap()
                Dim TranscopyCylinder As TransCopyCylinder

                SetTracks(_Image.CylinderEnd + 1, _Image.Sides)

                For Cylinder = 0 To _Image.CylinderEnd
                    For Side = 0 To _Image.Sides - 1
                        TranscopyCylinder = _Image.GetCylinder(Cylinder, Side)
                        If TranscopyCylinder.MFMData IsNot Nothing Then
                            For Each MFMSector In TranscopyCylinder.MFMData.Sectors
                                If MFMSector.DAMFound Then
                                    If MFMSector.SectorId >= 1 And MFMSector.SectorId <= SECTOR_COUNT Then
                                        Dim BitstreamSector As New BitstreamSector With {
                                   .Data = MFMSector.Data,
                                   .Overlaps = MFMSector.Overlaps,
                                   .IsValid = MFMSector.IsValid,
                                   .Size = MFMSector.GetSizeBytes
                                }
                                        SetSector(Cylinder, Side, MFMSector.SectorId, BitstreamSector)
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

            Public Overrides Function SaveToFile(FilePath As String) As Boolean Implements IByteArray.SaveToFile
                Return _Image.Export(FilePath, True)
            End Function
        End Class
    End Namespace
End Namespace