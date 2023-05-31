Imports System.IO
Imports System.Text
Imports DiskImageTool.DiskImage

Module Copy_Protection
    Public Function GetBroderbundCopyright(Disk As DiskImage.Disk, BadSectors As HashSet(Of UInteger)) As String
        If BadSectors.Count >= 2 AndAlso CheckBadSectors(BadSectors, {186, 187}) AndAlso Not CheckBadSectors(BadSectors, {185, 188}) Then
            Dim b = Disk.Data.GetBytes(Disk.SectorToBytes(186), Disk.BYTES_PER_SECTOR)
            Dim Counter As Integer
            For Counter = 0 To b.Length - 1
                If {0, &HF6}.Contains(b(Counter)) Then
                    Exit For
                End If
            Next
            Return Encoding.UTF8.GetString(b, 0, Counter)
        End If

        Return ""
    End Function

    Public Function GetCopyProtection(Disk As DiskImage.Disk, BadSectors As HashSet(Of UInteger)) As String
        Dim ProtectionFound As Boolean = False
        Dim ProtectionName As String = ""
        Dim DataLength = Disk.Data.Length
        Dim Offset As UInteger

        'H.L.S. Duplication
        If Not ProtectionFound AndAlso BadSectors.Count >= 2 Then
            If CheckBadSectors(BadSectors, {708, 709}) Then
                Offset = Disk.SectorToBytes(709)
                If Offset + 8 <= DataLength Then
                    Dim b1 = Disk.Data.GetBytes(Offset, 8)
                    Offset = Disk.SectorToBytes(708)
                    Dim b2 = Disk.Data.GetBytes(Offset, 8)

                    If b1.CompareTo(b2) AndAlso Integer.TryParse(Encoding.UTF8.GetString(b1, 3, 4), 0) Then
                        ProtectionFound = True
                        ProtectionName = "H.L.S. Duplication"
                    End If
                End If
            End If
        End If

        'Softguard 2.0.3 (Sierra)
        If Not ProtectionFound AndAlso BadSectors.Count >= 10 Then
            If CheckBadSectors(BadSectors, {108, 109, 110, 111, 112, 113, 114, 115, 116, 117}) AndAlso Disk.Directory.HasFile("CPC.COM") > -1 Then
                ProtectionFound = True
                ProtectionName = "Softguard 2.0.3 (Sierra)"
            End If
        End If

        'Origin Systems OSI-1
        If Not ProtectionFound AndAlso BadSectors.Count >= 10 Then
            If CheckBadSectors(BadSectors, {108, 109, 110, 111, 112, 113, 114, 115, 116, 117}) _
                AndAlso CheckFileList(Disk, {"2400AD.EXE", "ULTIMA.COM", "ULTIMA.EXE", "ULTIMAII.EXE", "LORE.EXE"}) Then

                ProtectionFound = True
                ProtectionName = "Origin Systems OSI-1"
            End If
        End If

        'KBI
        If Not ProtectionFound AndAlso BadSectors.Count >= 10 Then
            For Sector As UInteger = 710 To 729
                If BadSectors.Contains(Sector) Then
                    Offset = Disk.SectorToBytes(Sector)
                    If Offset + 31 <= DataLength Then
                        Dim b = Disk.Data.GetBytes(Offset, 31)
                        If Encoding.UTF8.GetString(b) = "(c) 1986 for KBI by L. TOURNIER" Then
                            ProtectionFound = True
                            ProtectionName = "KBI"
                            Exit For
                        End If
                    End If
                End If
            Next
        End If

        'Microprose Protection 2
        If Not ProtectionFound AndAlso BadSectors.Count >= 8 AndAlso Disk.BPB.MediaDescriptor = &HFD Then
            If CheckBadSectors(BadSectors, {684, 685, 686, 687, 702, 703, 704, 705}) AndAlso Not CheckBadSectors(BadSectors, {683, 706}) Then
                ProtectionFound = True
                ProtectionName = "Microprose Protection 2"
            End If
        End If

        If Not ProtectionFound AndAlso BadSectors.Count >= 8 AndAlso Disk.BPB.MediaDescriptor = &HF9 Then
            If CheckBadSectors(BadSectors, {1406, 1407, 1408, 1409, 1424, 1425, 1426, 1427}) AndAlso Not CheckBadSectors(BadSectors, {1405, 1428}) Then
                ProtectionFound = True
                ProtectionName = "Microprose Protection 2"
            End If
            If Not ProtectionFound Then
                If CheckBadSectors(BadSectors, {1404, 1405, 1406, 1407, 1422, 1423, 1424, 1425}) AndAlso Not CheckBadSectors(BadSectors, {1403, 1426}) Then
                    ProtectionFound = True
                    ProtectionName = "Microprose Protection 2"
                End If
            End If
        End If

        'Softguard 2.x/3.x
        If Not ProtectionFound AndAlso Disk.Directory.HasFile("CML0300.FCL") > -1 Then
            ProtectionFound = True
            ProtectionName = "Softguard 2.x/3.x"
        End If

        'Xidex Magnetics XEMAG / XELOK
        If Not ProtectionFound Then
            Dim Fileindex = Disk.Directory.HasFile("XEMAG.SYS")
            If Fileindex > -1 Then
                Dim DirectoryEntry = Disk.Directory.GetFile(Fileindex)
                If Disk.BPB.ClusterToSector(DirectoryEntry.StartingCluster) = 162 Then
                    ProtectionFound = True
                    ProtectionName = "Xidex Magnetics XEMAG / XELOK"
                End If
            End If
        End If

        If ProtectionFound Then
            Return ProtectionName
        Else
            Return ""
        End If
    End Function

    Private Function CheckBadSectors(BadSectors As HashSet(Of UInteger), SectorList() As UShort) As Boolean
        For Each Sector In SectorList
            If Not BadSectors.Contains(Sector) Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Function CheckFileList(Disk As DiskImage.Disk, FileList() As String) As Boolean
        For Each File In FileList
            If Disk.Directory.HasFile(File) > -1 Then
                Return True
            End If
        Next
        Return False
    End Function
End Module
