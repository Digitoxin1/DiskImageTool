Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage

Namespace ImageFormats
    Namespace PSI
        Public Class PSIByteArray
            Inherits Bitstream.BitstreamByteArray
            Implements IByteArray

            Private ReadOnly _Image As PSISectorImage

            Public Sub New(Image As PSISectorImage, DiskFormat As FloppyDiskFormat)
                MyBase.New()

                _Image = Image

                BuildSectorMap()
                InitDiskFormat(DiskFormat)
            End Sub

            Private Sub BuildSectorMap()
                SetTracks(_Image.CylinderCount, _Image.HeadCount)

                For Each PSISector In _Image.Sectors
                    If Not PSISector.IsAlternateSector Then
                        If PSISector.Sector >= 1 And PSISector.Sector <= SECTOR_COUNT Then
                            Dim BitstreamSector As New BitstreamSector With {
                                .Data = PSISector.Data,
                                .Overlaps = False,
                                .IsValid = Not PSISector.HasDataCRCError,
                                .Size = PSISector.Size
                            }
                            SetSector(PSISector.Cylinder, PSISector.Head, PSISector.Sector, BitstreamSector)
                        End If
                    End If
                Next
            End Sub

            Public Overrides ReadOnly Property ImageType As FloppyImageType Implements IByteArray.ImageType
                Get
                    Return FloppyImageType.PSIImage
                End Get
            End Property

            Public Overrides Function SaveToFile(FilePath As String) As Boolean Implements IByteArray.SaveToFile
                Return _Image.Export(FilePath)
            End Function
        End Class
    End Namespace
End Namespace
