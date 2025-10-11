Imports DiskImageTool.DiskImage
Imports DiskImageTool.Bitstream
Imports DiskImageTool.Bitstream.IBM_MFM

Module Reports
    Private Const H_SEPARATOR As String = "-"
    Private Const V_SEPARATOR As String = " | "
    Public Sub DisplayReportWriteSplices(CurrentImage As CurrentImage)
        Dim TrackLength As Integer = 6
        Dim SideLength As Integer = 6
        Dim SectorIdLength As Integer = 14
        Dim RegionLength As Integer = 14
        Dim FileLength As Integer = 12
        Dim Rows As New List(Of String)

        If CurrentImage.Disk.Image.IsBitstreamImage Then
            Dim SectorList = New List(Of WriteSpliceSector)
            Dim HasFile As Boolean = False
            Dim RowLength As Integer

            Dim BPB = CurrentImage.Disk.BPB
            If Not BPB.IsValid Then
                BPB = BuildBPB(CInt(CurrentImage.Disk.Image.Length))
            End If

            Dim Image = CurrentImage.Disk.Image.BitstreamImage

            For Track = 0 To Image.TrackCount - 1 Step Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    Dim BitstreamTrack = Image.GetTrack(Track, Side)
                    If BitstreamTrack.TrackType = BitstreamTrackType.MFM Or BitstreamTrack.TrackType = BitstreamTrackType.FM Then
                        Dim MappedTrack = Track \ Image.TrackStep
                        Dim RegionData = MFMGetRegionList(BitstreamTrack.Bitstream, BitstreamTrack.TrackType)
                        For Each RegionSector In RegionData.Sectors
                            If RegionSector.WriteSplice Then
                                Dim Region As String = ""
                                Dim DirectoryEntry As DirectoryEntry = Nothing
                                Dim IsDataArea As Boolean = False
                                Dim IsEmpty As Boolean = False

                                If BPB.IsValid Then
                                    Dim Sector = BPB.TrackToSector(MappedTrack, Side) + RegionSector.SectorIndex
                                    Dim Cluster = BPB.SectorToCluster(Sector)
                                    Region = GetFloppyDiskRegionName(CurrentImage.Disk, Sector, Cluster)
                                    If Region.Length = 0 Then
                                        Region = My.Resources.DataInspector_Label_DataArea
                                    End If

                                    RegionLength = Math.Max(RegionLength, Region.Length)

                                    If Cluster > 1 Then
                                        DirectoryEntry = GetDirectoryEntryFromCluster(CurrentImage.Disk, Cluster)
                                        IsDataArea = True
                                        IsEmpty = IsClusterEmpty(CurrentImage.Disk.Image, BPB, Cluster)
                                    End If
                                End If

                                Dim WriteSpliceSector = New WriteSpliceSector With {
                                    .Track = MappedTrack,
                                    .Side = Side,
                                    .SectorIndex = RegionSector.SectorIndex,
                                    .SectorId = RegionSector.SectorId,
                                    .Region = Region,
                                    .DirectoryEntry = DirectoryEntry
                                }

                                If DirectoryEntry IsNot Nothing Then
                                    HasFile = True
                                    WriteSpliceSector.FileName = DirectoryEntry.GetFullFileName
                                    WriteSpliceSector.IsDirectory = DirectoryEntry.IsDirectory
                                    FileLength = Math.Max(FileLength, WriteSpliceSector.FileName.Length)
                                Else
                                    If IsDataArea And Not IsEmpty Then
                                        HasFile = True
                                        WriteSpliceSector.FileName = InParens(My.Resources.Label_Deleted)
                                        WriteSpliceSector.DeletedData = True
                                    End If
                                End If

                                SectorList.Add(WriteSpliceSector)
                            End If
                        Next
                    End If
                Next
            Next

            RowLength = TrackLength + SideLength + SectorIdLength + RegionLength + (V_SEPARATOR.Length * 3) + 2
            If HasFile Then
                RowLength += V_SEPARATOR.Length + FileLength
            End If

            Rows.Add(CurrentImage.ImageData.FileName)
            Rows.Add("")
            If My.Settings.Debug Then
                Rows.Add("Modifications")
            Else
                Rows.Add(My.Resources.Label_WriteSplices)
            End If
            Rows.Add(StrDup(RowLength, H_SEPARATOR))

            Dim ContentRow As New List(Of String) From {
                My.Resources.Label_Track.PadRight(TrackLength),
                My.Resources.Label_Side.PadRight(SideLength),
                My.Resources.Label_SectorId.PadRight(SectorIdLength),
                My.Resources.DataInspector_Label_Region.PadRight(RegionLength)
            }
            If HasFile Then
                ContentRow.Add(My.Resources.Label_File.PadRight(FileLength))
            End If

            Rows.Add(GetRowString(ContentRow))
            Rows.Add(StrDup(RowLength, H_SEPARATOR))

            Dim LastWriteSpliceSector As WriteSpliceSector = Nothing
            Dim SectorIdList As New List(Of Integer)
            Dim SectorIdString As String

            For Each WriteSpliceSector In SectorList
                If LastWriteSpliceSector IsNot Nothing Then
                    Dim GroupChanged = LastWriteSpliceSector.Track <> WriteSpliceSector.Track _
                        Or LastWriteSpliceSector.Side <> WriteSpliceSector.Side _
                        Or LastWriteSpliceSector.Region <> WriteSpliceSector.Region _
                        Or LastWriteSpliceSector.FileName <> WriteSpliceSector.FileName _
                        Or LastWriteSpliceSector.IsDirectory <> WriteSpliceSector.IsDirectory _
                        Or LastWriteSpliceSector.DeletedData <> WriteSpliceSector.DeletedData

                    If GroupChanged Then
                        SectorIdString = CompressIntegerList(SectorIdList)

                        ContentRow = New List(Of String) From {
                            LastWriteSpliceSector.Track.ToString.PadRight(TrackLength),
                            LastWriteSpliceSector.Side.ToString.PadRight(SideLength),
                            SectorIdString.PadRight(SectorIdLength),
                            LastWriteSpliceSector.Region.PadRight(RegionLength)
                        }
                        If HasFile Then
                            ContentRow.Add(LastWriteSpliceSector.FileName.PadRight(FileLength))
                        End If

                        Rows.Add(GetRowString(ContentRow))
                        SectorIdList.Clear()
                    End If
                End If

                SectorIdList.Add(WriteSpliceSector.SectorId)
                LastWriteSpliceSector = WriteSpliceSector
            Next

            If LastWriteSpliceSector IsNot Nothing Then
                SectorIdString = CompressIntegerList(SectorIdList)

                ContentRow = New List(Of String) From {
                        LastWriteSpliceSector.Track.ToString.PadRight(TrackLength),
                        LastWriteSpliceSector.Side.ToString.PadRight(SideLength),
                        SectorIdString.PadRight(SectorIdLength),
                        LastWriteSpliceSector.Region.PadRight(RegionLength)
                    }
                If HasFile Then
                    ContentRow.Add(LastWriteSpliceSector.FileName.PadRight(FileLength))
                End If

                Rows.Add(GetRowString(ContentRow))
            End If

            If My.Settings.Debug Then
                Rows.Add("")
                Rows.Add("Details")
                Rows.Add(StrDup(7, H_SEPARATOR))
            End If
        End If

        Dim Content = String.Join(vbCrLf, Rows) & If(Rows.Count > 0, vbCrLf, String.Empty)

        Dim frmTextView = New TextViewForm(My.Resources.Label_WriteSplices & " - " & CurrentImage.ImageData.FileName, Content, True, True)
        frmTextView.ShowDialog()
    End Sub

    Private Function CompressIntegerList(values As List(Of Integer)) As String
        If values Is Nothing OrElse values.Count = 0 Then Return String.Empty

        ' Sort the list in ascending order
        values.Sort()

        Dim result As New List(Of String)
        Dim start As Integer = values(0)
        Dim last As Integer = values(0)

        For i As Integer = 1 To values.Count - 1
            If values(i) = last + 1 Then
                ' Still in a sequence
                last = values(i)
            Else
                ' Sequence ended, append range or single number
                If start = last Then
                    result.Add(start.ToString())
                Else
                    result.Add($"{start}-{last}")
                End If
                start = values(i)
                last = values(i)
            End If
        Next

        ' Add the final range
        If start = last Then
            result.Add(start.ToString())
        Else
            result.Add($"{start}-{last}")
        End If

        ' Join all parts with commas
        Return String.Join(",", result)
    End Function

    Private Function GetRowString(Row As List(Of String))
        Return " " & String.Join(V_SEPARATOR, Row).TrimEnd
    End Function

    Private Class WriteSpliceSector
        Property Track As Integer
        Property Side As Integer
        Property SectorIndex As UShort
        Property SectorId As Byte
        Property Region As String = ""
        Property DirectoryEntry As DirectoryEntry = Nothing
        Property FileName As String = ""
        Property IsDirectory As Boolean
        Property DeletedData As Boolean = False
    End Class
End Module
