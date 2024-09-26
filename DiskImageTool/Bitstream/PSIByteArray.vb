Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage
Imports DiskImageTool.PSI_Image

Public Class PSIByteArray
    Inherits Bitstream.BitstreamByteArray
    Implements IByteArray

    Private ReadOnly _Image As PSISectorImage

    Public Sub New(Image As PSISectorImage, DiskFormat As FloppyDiskFormat)
        MyBase.New(DiskFormat)

        _Image = Image

        BuildSectorMap()
    End Sub

    Private Sub BuildSectorMap()
        Dim TrackCount = GetTrackCount()
        Dim MaxSize As UInteger = ImageParams.BytesPerSector
        Dim MaxSectors As UShort = ImageParams.SectorsPerTrack

        For Each PSISector In _Image.Sectors
            If Not PSISector.IsAlternateSector Then
                If PSISector.Cylinder < TrackCount And PSISector.Head < ImageParams.NumberOfHeads Then
                    If PSISector.Sector >= 1 And PSISector.Sector <= MaxSectors Then
                        Dim BitstreamSector As New BitstreamSector With {
                            .Data = PSISector.Data,
                            .Overlaps = False,
                            .HasValidChecksum = Not PSISector.HasDataCRCError,
                            .Size = PSISector.Size
                        }

                        Dim Offset = GetOffset(PSISector.Cylinder, PSISector.Head, PSISector.Sector)
                        Dim Sector = GetSector(Offset)
                        If IsProtectedSector(BitstreamSector) AndAlso Not ProtectedSectors.Contains(Sector) Then
                            ProtectedSectors.Add(Sector)
                        End If
                        SectorMap(Sector) = BitstreamSector
                    End If
                End If
            End If
        Next

        For Sector = 0 To SectorMap.Length - 1
            If SectorMap(Sector) Is Nothing Then
                ProtectedSectors.Add(Sector)
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
