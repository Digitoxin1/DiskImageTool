Imports System.Text
Imports DiskImageTool.DiskImage

Module MenuTools
    Public Sub GenerateTrackLayout(CurrentImage As DiskImageContainer)
        If CurrentImage Is Nothing Then Exit Sub

        If Not CurrentImage.Disk.Image.IsBitstreamImage Then
            Exit Sub
        End If

        Dim BitstreamImage = CurrentImage.Disk.Image.BitstreamImage
        Dim Gap4A As UShort
        Dim Gap1 As UShort
        Dim Gap3List() As UShort
        Dim Gap3 As UShort
        Dim Gap3Match As Boolean
        Dim SectorCount As UShort
        Dim GapString As String
        Dim TrackString As String
        Dim PrevTrackString As String = ""
        Dim FirstTrack As UShort
        Dim Track As UShort
        Dim Side As UShort
        Dim TrackLayout As New StringBuilder

        For Track = 0 To BitstreamImage.TrackCount - 1 Step BitstreamImage.TrackStep
            TrackString = ""
            For Side = 0 To BitstreamImage.SideCount - 1
                Gap4A = 80
                Gap1 = 50
                Gap3List = New UShort(0) {80}
                Gap3Match = True
                Dim BitstreamTrack = BitstreamImage.GetTrack(Track, Side)
                Dim RegionList = Bitstream.IBM_MFM.MFMGetRegionList(BitstreamTrack.Bitstream, BitstreamTrack.TrackType)
                SectorCount = RegionList.Sectors.Count
                Gap4A = RegionList.Gap4A
                Gap1 = RegionList.Gap1
                If SectorCount > 1 Then
                    Gap3List = New UShort(SectorCount - 1) {}
                    For k = 0 To SectorCount - 2
                        Dim Sector = RegionList.Sectors.Item(k)
                        Gap3List(k) = Sector.Gap3
                        If k > 0 Then
                            If Gap3 <> Gap3List(k) Then
                                Gap3Match = False
                            End If
                        End If
                        Gap3 = Gap3List(k)
                    Next
                    Gap3List(SectorCount - 1) = Gap3List(SectorCount - 2)
                End If
                GapString = Gap4A & "," & Gap1 & ","
                If Gap3Match Then
                    GapString &= Gap3List(0)
                Else
                    GapString &= "["
                    For k = 0 To Gap3List.Length - 1
                        If k > 0 Then
                            GapString &= ","
                        End If
                        GapString &= Gap3List(k)
                    Next
                    GapString &= "]"
                End If

                If Side = 0 Then
                    TrackString = GapString
                ElseIf TrackString <> GapString Then
                    TrackString &= "." & GapString
                End If
            Next

            If Track = 0 Then
                FirstTrack = Track
            ElseIf TrackString <> PrevTrackString Then
                TrackLayout.AppendLine(FirstTrack & "-" & Track - 1 & ":" & PrevTrackString)
                FirstTrack = Track
            End If
            PrevTrackString = TrackString
        Next
        TrackLayout.AppendLine(FirstTrack & "-" & Track - 1 & ":" & PrevTrackString)

        Dim frmTextView = New TextViewForm(My.Resources.Caption_TrackLayout, TrackLayout.ToString, True, True, "tracklayout.txt")
        frmTextView.ShowDialog()
    End Sub

    Public Function ImageClearReservedBytes(CurrentImage As DiskImageContainer) As Boolean
        If App.Globals.TitleDB.IsVerifiedImage(CurrentImage.Disk) Then
            If Not MsgBoxQuestion(My.Resources.Dialog_ClearReservedBytes) Then
                Return False
            End If
        End If

        Return DiskImage.ClearReservedBytes(CurrentImage.Disk)
    End Function

    Public Function ImageFixImageSize(CurrentImage As DiskImageContainer) As Boolean
        Dim Result As Boolean = True

        If App.Globals.TitleDB.IsVerifiedImage(CurrentImage.Disk) Then
            If Not MsgBoxQuestion(My.Resources.Dialog_AdjustImageSize) Then
                Return False
            End If
        End If

        Dim Compare = CurrentImage.Disk.CheckImageSize

        If Compare = 0 Then
            Result = False
        ElseIf Compare < 0 Then
            Result = MsgBoxQuestion(String.Format(My.Resources.Dialog_IncreaseImageSize, Environment.NewLine))
        Else
            Dim ReportedSize = CurrentImage.Disk.BPB.ReportedImageSize
            Dim Data = CurrentImage.Disk.Image.GetBytes(ReportedSize, CurrentImage.Disk.Image.Length - ReportedSize)
            Dim HasData As Boolean = False
            For Each b In Data
                If b <> 0 Then
                    HasData = True
                    Exit For
                End If
            Next
            If HasData Then
                Result = MsgBoxQuestion(String.Format(My.Resources.Dialog_TruncateImage, Environment.NewLine))
            End If
        End If

        If Result Then
            Result = DiskImage.FixImageSize(CurrentImage.Disk)
        End If

        Return Result
    End Function

    Public Function ImageRemoveWindowsModifications(Disk As Disk, Batch As Boolean) As Boolean
        If App.Globals.TitleDB.IsVerifiedImage(Disk) Then
            If Batch Then
                Return False
            Else
                If Not MsgBoxQuestion(My.Resources.Dialog_Win9xClean) Then
                    Return False
                End If
            End If
        End If

        Dim Result As Boolean = False
        Dim UseTransaction = Disk.BeginTransaction

        If Disk.BootSector.IsWin9xOEMName Then
            Dim BootstrapType = App.Globals.BootstrapDB.FindMatch(Disk.BootSector.BootStrapCode)
            If BootstrapType IsNot Nothing Then
                If BootstrapType.OEMNames.Count > 0 Then
                    Disk.BootSector.OEMName = BootstrapType.OEMNames.Item(0).Name
                    Result = True
                End If
            End If
        End If

        Dim FileList = Disk.RootDirectory.GetFileList()

        For Each DirectoryEntry In FileList
            If DirectoryEntry.IsValid Then
                If DirectoryEntry.HasLastAccessDate Then
                    If DirectoryEntry.GetLastAccessDate.IsValidDate Then
                        DirectoryEntry.ClearLastAccessDate()
                        Result = True
                    End If
                End If

                If DirectoryEntry.HasCreationDate Then
                    If DirectoryEntry.GetCreationDate.IsValidDate Then
                        DirectoryEntry.ClearCreationDate()
                        Result = True
                    End If
                End If
            End If
        Next

        If UseTransaction Then
            Disk.EndTransaction()
        End If

        Return Result
    End Function

    Public Function ImageRestructure(Disk As Disk) As Boolean
        If App.Globals.TitleDB.IsVerifiedImage(Disk) Then
            If Not MsgBoxQuestion(My.Resources.Dialog_RestructureImage) Then
                Return False
            End If
        End If

        DiskImage.RestructureImage(Disk)

        Return True
    End Function
End Module
