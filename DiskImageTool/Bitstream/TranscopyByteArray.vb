Imports DiskImageTool.Bitstream
Imports DiskImageTool.DiskImage
Imports DiskImageTool.IBM_MFM
Imports DiskImageTool.PSI_Image
Imports DiskImageTool.Transcopy

Public Class TranscopyByteArray
    Inherits Bitstream.BitstreamByteArray
    Implements IByteArray

    Private ReadOnly _Image As TransCopyImage

    Public Sub New(Image As TransCopyImage, DiskFormat As FloppyDiskFormat)
        MyBase.New(DiskFormat)

        _Image = Image

        BuildSectorMap()
    End Sub

    Private Sub BuildSectorMap()
        Dim TrackCount = GetTrackCount()
        Dim MaxSize As UInteger = ImageParams.BytesPerSector
        Dim MaxSectors As UShort = ImageParams.SectorsPerTrack

        For Each Cylinder In _Image.Cylinders
            If Cylinder.MFMData IsNot Nothing Then
                If Cylinder.Track < TrackCount And Cylinder.Side < ImageParams.NumberOfHeads Then
                    For Each MFMSector In Cylinder.MFMData.Sectors
                        If MFMSector.DAMFound Then
                            If MFMSector.SectorId >= 1 And MFMSector.SectorId <= MaxSectors Then
                                Dim BitstreamSector As New BitstreamSector With {
                                .Data = MFMSector.Data,
                                .Overlaps = MFMSector.Overlaps,
                                .HasValidChecksum = MFMSector.ValidChecksum,
                                .Size = MFMSector.GetSizeBytes
                                }
                                Dim Offset = GetOffset(Cylinder.Track, Cylinder.Side, MFMSector.SectorId)
                                Dim Sector = GetSector(Offset)
                                If IsProtectedSector(BitstreamSector) AndAlso Not ProtectedSectors.Contains(Sector) Then
                                    ProtectedSectors.Add(Sector)
                                End If
                                SectorMap(Sector) = BitstreamSector
                            End If
                        End If
                    Next
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
            Return FloppyImageType.TranscopyImage
        End Get
    End Property

    Public Overrides Function SaveToFile(FilePath As String) As Boolean Implements IByteArray.SaveToFile
        Return _Image.Export(FilePath)
    End Function
End Class
