Imports DiskImageTool.DiskImage
Imports DiskImageTool.Bitstream
Imports DiskImageTool.Bitstream.IBM_MFM
Imports System.Collections.Specialized

Module Reports
    Private Const H_SEPARATOR As String = "-"
    Private Const V_SEPARATOR As String = " | "

    Public Sub DisplayReportModifications(Disk As Disk, FileName As String, Optional FilePath As String = Nothing)
        Const OUTPUT_FILENAME As String = "modifications.txt"
        Dim TrackLength As Integer = 6
        Dim SideLength As Integer = 6
        Dim SectorIdLength As Integer = 14
        Dim RegionLength As Integer = 14
        Dim FileLength As Integer = 12
        Dim RegionFlags As FloppyDiskRegionEnum = FloppyDiskRegionEnum.None
        Dim Rows As New List(Of String)
        Dim ScanResponse As DirectoryScanResponse = Nothing
        Dim ModifiedFiles As New OrderedDictionary()

        If Disk.Image.IsBitstreamImage Then
            Dim SectorList As New List(Of WriteSpliceSector)
            Dim HasFile As Boolean = False
            Dim RowLength As Integer

            Dim BPB = Disk.BPB
            If Not BPB.IsValid Then
                BPB = BuildBPB(CInt(Disk.Image.Length))
            End If

            If Disk.IsValidImage Then
                ScanResponse = ProcessDirectoryEntries(Disk.RootDirectory, Nothing)
            End If

            Dim Image = Disk.Image.BitstreamImage

            For Track = 0 To Image.TrackCount - 1 Step Image.TrackStep
                For Side = 0 To Image.SideCount - 1
                    Dim BitstreamTrack = Image.GetTrack(Track, Side)
                    If BitstreamTrack.TrackType = BitstreamTrackType.MFM Or BitstreamTrack.TrackType = BitstreamTrackType.FM Then
                        Dim MappedTrack = Track \ Image.TrackStep
                        Dim RegionData = MFMGetRegionList(BitstreamTrack.Bitstream, BitstreamTrack.TrackType)
                        For Each RegionSector In RegionData.Sectors
                            If RegionSector.WriteSplice Then
                                Dim AreaName As String = ""
                                Dim DirectoryEntry As DirectoryEntry = Nothing
                                Dim IsDataArea As Boolean = False
                                Dim IsEmpty As Boolean = False

                                If BPB.IsValid Then
                                    If RegionSector.SectorId >= 1 And RegionSector.SectorId <= BPB.SectorsPerTrack Then
                                        Dim Sector = BPB.TrackToSector(MappedTrack, Side) + (CInt(RegionSector.SectorId) - 1)
                                        Dim Cluster = BPB.SectorToCluster(Sector)
                                        Dim Region = GetFloppyDiskRegionName(Disk, Sector, Cluster)
                                        AreaName = Region.AreaName
                                        RegionFlags = RegionFlags Or Region.Region

                                        If AreaName.Length = 0 Then
                                            AreaName = My.Resources.Label_DataArea
                                        End If

                                        RegionLength = Math.Max(RegionLength, AreaName.Length)

                                        If Cluster > 1 Then
                                            DirectoryEntry = GetDirectoryEntryFromCluster(Disk, Cluster)
                                            IsDataArea = True
                                            IsEmpty = IsClusterEmpty(Disk.Image, BPB, Cluster)
                                        End If
                                    End If
                                End If

                                Dim WriteSpliceSector As New WriteSpliceSector With {
                                    .Track = MappedTrack,
                                    .Side = Side,
                                    .SectorIndex = RegionSector.SectorIndex,
                                    .SectorId = RegionSector.SectorId,
                                    .Region = AreaName,
                                    .DirectoryEntry = DirectoryEntry
                                }

                                If DirectoryEntry IsNot Nothing Then
                                    HasFile = True
                                    WriteSpliceSector.FileName = DirectoryEntry.GetFullFileName
                                    WriteSpliceSector.IsDirectory = DirectoryEntry.IsDirectory
                                    FileLength = Math.Max(FileLength, WriteSpliceSector.FileName.Length)

                                    If Not ModifiedFiles.Contains(WriteSpliceSector.FileName) Then
                                        ModifiedFiles.Add(WriteSpliceSector.FileName, WriteSpliceSector.IsDirectory)
                                    End If
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

            If App.AppSettings.Expert.WriteSpliceDisplayFilename Then
                Rows.Add(FileName)
                Rows.Add("")
            End If


            Rows.Add(My.Resources.Label_Modifications)
            Rows.Add(StrDup(RowLength, H_SEPARATOR))

            Dim ContentRow As New List(Of String) From {
                My.Resources.Label_Track.PadRight(TrackLength),
                My.Resources.Label_Side.PadRight(SideLength),
                My.Resources.Label_SectorId.PadRight(SectorIdLength),
                My.Resources.Label_Region.PadRight(RegionLength)
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

            If App.AppSettings.Expert.WriteSpliceDisplayDetails AndAlso SectorList.Count > 0 Then
                Dim Details As New List(Of String)
                Dim SeparatorLength As Integer = 7

                Dim RootMissing As Boolean = (RegionFlags And FloppyDiskRegionEnum.RootDirectory) = 0
                Dim DataMissing As Boolean = (RegionFlags And FloppyDiskRegionEnum.DataArea) = 0

                If (RegionFlags And FloppyDiskRegionEnum.BootSector) <> 0 Then
                    If Disk.BootSector.IsWin9xOEMName Then
                        Details.Add(My.Resources.Modifications_OEMName)
                    Else
                        Details.Add(My.Resources.Modifications_BootSector)
                    End If
                End If

                If RootMissing And DataMissing Then
                    If (RegionFlags And FloppyDiskRegionEnum.FAT1) <> 0 Then
                        Details.Add(String.Format(My.Resources.Modifications_FAT, "1"))
                    End If
                    If (RegionFlags And FloppyDiskRegionEnum.FAT2) <> 0 Then
                        Details.Add(String.Format(My.Resources.Modifications_FAT, "2"))
                    End If
                End If

                If (RegionFlags And FloppyDiskRegionEnum.RootDirectory) <> 0 Then
                    If ScanResponse IsNot Nothing Then
                        If ScanResponse.HasCreated Then
                            Details.Add(My.Resources.Modifications_CreatedDateAdded)
                        End If
                        If ScanResponse.HasLastAccessed Then
                            Details.Add(My.Resources.Modifcations_LastAccessDateAdded)
                        End If
                        If ScanResponse.HasBootSector Then
                            Details.Add(My.Resources.Modifications_BootSectorCopy)
                        End If
                    End If
                End If

                For Each Key As Object In ModifiedFiles.Keys
                    Dim IsDirectory As Boolean = ModifiedFiles.Item(Key)
                    If IsDirectory Then
                        Details.Add($"Directory {Quoted(CStr(Key))} modified")
                    Else
                        Details.Add($"File {Quoted(CStr(Key))} modified")
                    End If
                Next

                If Details.Count > 0 Then
                    SeparatorLength = Details(0).Length
                End If
                Rows.Add("")
                Rows.Add(My.Resources.Label_Details)
                Rows.Add(StrDup(SeparatorLength, H_SEPARATOR))
                Rows.Add(String.Join(Environment.NewLine, Details))
            End If
        End If

        Dim Content = String.Join(vbCrLf, Rows) & If(Rows.Count > 0, vbCrLf, String.Empty)

        Dim SaveFileName = OUTPUT_FILENAME
        If Not String.IsNullOrEmpty(FilePath) Then
            SaveFileName = IO.Path.Combine(FilePath, SaveFileName)
        End If

        Dim Caption = My.Resources.Label_Modifications
        If Not String.IsNullOrEmpty(FileName) Then
            Caption &= " - " & FileName
        End If

        TextViewForm.Display(Caption, Content, True, True, SaveFileName)
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
