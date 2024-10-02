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
                            Dim IsStandard = (PSISector.Size = 512 And Not PSISector.HasDataCRCError)
                            Dim BitstreamSector As New BitstreamSector With {
                                .Data = PSISector.Data,
                                .Size = PSISector.Size,
                                .IsStandard = IsStandard
                            }
                            Dim Sector = GetSector(PSISector.Cylinder, PSISector.Head, PSISector.Sector)
                            If Sector Is Nothing Then
                                SetSector(PSISector.Cylinder, PSISector.Head, PSISector.Sector, BitstreamSector)
                            Else
                                Sector.IsStandard = False
                            End If
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
