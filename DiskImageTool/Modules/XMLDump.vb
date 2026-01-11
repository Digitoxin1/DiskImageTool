Imports System.IO
Imports System.Text
Imports System.Xml
Imports DiskImageTool.DiskImage

Module XMLDump
    Private Const NULL_CHAR As Char = "�"

    Public Sub BuildImageNode(xw As XmlWriter, Disk As Disk, ImageData As ImageData, BootStrapDB As BootstrapDB, TitleDB As FloppyDB)
        xw.WriteStartElement("image")

        xw.WriteAttributeString("name", ImageData.FileName)

        Dim PathName As String
        If ImageData.FileType = ImageData.FileTypeEnum.Compressed Then
            PathName = IO.Path.GetDirectoryName(ImageData.CompressedFile)
        Else
            PathName = IO.Path.GetDirectoryName(ImageData.SourceFile)
        End If

        WriteElementStringIFNotEmpty(xw, "path", PathName)

        If Disk IsNot Nothing Then
            Dim MD5 = Disk.Image.GetMD5Hash()
            BuildHashes(xw, Disk, MD5)
            BuildSummaryNode(xw, Disk, BootStrapDB, TitleDB, MD5)
            BuildDirectoryNode(xw, Disk)
        End If

        xw.WriteEndElement()
    End Sub

    Public Function BuildXMLDump(Image As DiskImageContainer, BootStrapDB As BootstrapDB, TitleDB As FloppyDB) As String
        Dim settings As New XmlWriterSettings With {
            .Indent = True,
            .Encoding = New System.Text.UTF8Encoding(False),
            .OmitXmlDeclaration = False
        }

        Using ms As New MemoryStream()
            Using xw As XmlWriter = XmlWriter.Create(ms, settings)
                xw.WriteStartDocument()

                xw.WriteStartElement("images")

                If Image.ImageData.FileType = ImageData.FileTypeEnum.Compressed Then
                    xw.WriteStartElement("archive")
                    xw.WriteAttributeString("name", IO.Path.GetFileName(Image.ImageData.SourceFile))
                    xw.WriteAttributeString("path", IO.Path.GetDirectoryName(Image.ImageData.SourceFile))
                End If

                BuildImageNode(xw, Image.Disk, Image.ImageData, BootStrapDB, TitleDB)

                If Image.ImageData.FileType = ImageData.FileTypeEnum.Compressed Then
                    xw.WriteEndElement()
                End If

                xw.WriteEndElement()

                xw.WriteEndDocument()
            End Using

            ms.Position = 0

            Return Encoding.UTF8.GetString(ms.ToArray())
        End Using
    End Function

    Private Sub BuildDirectoryEntryNode(xw As XmlWriter, DirectoryEntry As DirectoryEntry, Path As String, LFNFileName As String)
        Dim Attributes = ""
        If DirectoryEntry.IsArchive Then
            Attributes &= "A"
        End If
        If DirectoryEntry.IsReadOnly Then
            Attributes &= "R"
        End If
        If DirectoryEntry.IsSystem Then
            Attributes &= "S"
        End If
        If DirectoryEntry.IsHidden Then
            Attributes &= "H"
        End If
        If DirectoryEntry.IsDirectory Then
            Attributes &= "D"
        End If
        If DirectoryEntry.IsVolumeName Then
            Attributes &= "V"
        End If

        Dim ShortFileName As String = DirectoryEntry.GetShortFileName
        Dim NewPath As String = Path & "\" & ShortFileName

        If DirectoryEntry.IsValidDirectory Then
            xw.WriteStartElement("directory")
        ElseIf DirectoryEntry.IsValidVolumeName Then
            xw.WriteStartElement("volume")
        Else
            xw.WriteStartElement("file")
        End If

        If DirectoryEntry.IsValidVolumeName Then
            xw.WriteAttributeString("name", SanitizeXmlText(DirectoryEntry.GetVolumeName))
        Else
            xw.WriteAttributeString("name", SanitizeXmlText(ShortFileName))
        End If

        If Not DirectoryEntry.IsValidVolumeName Then
            xw.WriteAttributeString("path", SanitizeXmlText(NewPath))
        End If

        If Not DirectoryEntry.IsValidDirectory And Not DirectoryEntry.IsValidVolumeName And Not DirectoryEntry.HasInvalidFileSize Then
            xw.WriteAttributeString("sizeBytes", DirectoryEntry.FileSize)
        End If

        If DirectoryEntry.IsValidFile Then
            xw.WriteAttributeString("crc32", DirectoryEntry.GetChecksum().ToString("X8"))
        End If

        xw.WriteAttributeString("lastWritten", DirectoryEntry.GetLastWriteDate.DateObject.ToString("yyyy-MM-ddTHH:mm:ss"))
        If DirectoryEntry.HasCreationDate Then
            xw.WriteAttributeString("created", DirectoryEntry.GetCreationDate.DateObject.ToString("yyyy-MM-ddTHH:mm:ss.fff"))
        End If
        If DirectoryEntry.HasLastAccessDate Then
            xw.WriteAttributeString("lastAccessed", DirectoryEntry.GetLastAccessDate.DateObject.ToString("yyyy-MM-dd"))
        End If
        xw.WriteAttributeString("attr", Attributes)

        If Not DirectoryEntry.HasInvalidStartingCluster And DirectoryEntry.StartingCluster > 1 Then
            xw.WriteAttributeString("cluster", DirectoryEntry.StartingCluster)
        End If

        If DirectoryEntry.IsDeleted Then
            xw.WriteAttributeString("deleted", "true")
        End If

        If LFNFileName.Length > 0 Then
            xw.WriteAttributeString("lfn", LFNFileName)
        End If
    End Sub

    Private Sub BuildDirectoryNode(xw As XmlWriter, Disk As Disk)
        xw.WriteStartElement("directory")

        xw.WriteAttributeString("name", "")
        xw.WriteAttributeString("path", "\")

        ProcessDirectoryEntries(xw, Disk.RootDirectory, "")

        xw.WriteEndElement()
    End Sub

    Private Sub BuildHashes(xw As XmlWriter, Disk As Disk, MD5 As String)
        xw.WriteStartElement("hashes")
        xw.WriteAttributeString("crc32", Disk.Image.GetCRC32())
        xw.WriteAttributeString("md5", MD5)
        xw.WriteAttributeString("sha1", Disk.Image.GetSHA1Hash())
        xw.WriteEndElement()
    End Sub

    Private Sub BuildNonStandardTracks(xw As XmlWriter, NonStandardTracks As HashSet(Of UShort), HeadCount As Byte)
        Dim TrackList(NonStandardTracks.Count - 1) As UShort

        Dim i As UShort = 0
        For Each Track In NonStandardTracks
            TrackList(i) = Track
            i += 1
        Next

        Array.Sort(TrackList)

        xw.WriteStartElement("nonStandardTracks")

        For i = 0 To TrackList.Length - 1
            Dim TrackString = (TrackList(i) \ HeadCount) & "." & (TrackList(i) Mod HeadCount)
            xw.WriteElementString("track", TrackString)
        Next

        xw.WriteEndElement()
    End Sub

    Private Sub BuildSummaryBootRecordNode(xw As XmlWriter, Disk As Disk, OEMNameResponse As OEMNameResponse)
        Dim Value As String

        xw.WriteStartElement("bootRecord")

        If Disk.BootSector.BPB.IsValid Then
            Dim DiskFormatBySize = FloppyDiskFormatGet(Disk.Image.Length)
            Dim BPBBySize = BuildBPB(DiskFormatBySize)
            Dim DoBPBCompare = Disk.DiskParams.Format = FloppyDiskFormat.FloppyUnknown And DiskFormatBySize <> FloppyDiskFormat.FloppyUnknown

            If Not OEMNameResponse.Found Then
                Value = "Not Found"
            ElseIf Not OEMNameResponse.Matched Then
                Value = "Mismatched"
            ElseIf OEMNameResponse.Verified Then
                Value = "Verified"
            Else
                Value = "Matched"
            End If

            xw.WriteStartElement("oemName")
            xw.WriteAttributeString("dbStatus", Value)
            xw.WriteValue(Disk.BootSector.GetOEMNameString.TrimEnd(NULL_CHAR))
            xw.WriteEndElement()

            xw.WriteStartElement("bytesPerSector")
            If DoBPBCompare AndAlso Disk.BootSector.BPB.BytesPerSector <> BPBBySize.BytesPerSector Then
                xw.WriteAttributeString("custom", "true")
            End If
            xw.WriteValue(Disk.BootSector.BPB.BytesPerSector)
            xw.WriteEndElement()

            xw.WriteStartElement("sectorsPerCluster")
            If Not Disk.BootSector.BPB.HasValidSectorsPerCluster(True) Then
                xw.WriteAttributeString("invalid", "true")
            ElseIf DoBPBCompare AndAlso Disk.BootSector.BPB.SectorsPerCluster <> BPBBySize.SectorsPerCluster Then
                xw.WriteAttributeString("custom", "true")
            End If
            xw.WriteValue(Disk.BootSector.BPB.SectorsPerCluster)
            xw.WriteEndElement()

            xw.WriteStartElement("reservedSectorCount")
            If DoBPBCompare AndAlso Disk.BootSector.BPB.ReservedSectorCount <> BPBBySize.ReservedSectorCount Then
                xw.WriteAttributeString("custom", "true")
            End If
            xw.WriteValue(Disk.BootSector.BPB.ReservedSectorCount)
            xw.WriteEndElement()

            xw.WriteStartElement("numberOfFATs")
            If DoBPBCompare AndAlso Disk.BootSector.BPB.NumberOfFATs <> BPBBySize.NumberOfFATs Then
                xw.WriteAttributeString("custom", "true")
            End If
            If Disk.DiskParams.IsXDF Then
                xw.WriteAttributeString("xdfCompabilityImage", "true")
            End If
            xw.WriteValue(Disk.BootSector.BPB.NumberOfFATs)
            xw.WriteEndElement()

            xw.WriteStartElement("rootDirectoryEntries")
            If DoBPBCompare AndAlso Disk.BootSector.BPB.RootEntryCount <> BPBBySize.RootEntryCount Then
                xw.WriteAttributeString("custom", "true")
            End If
            xw.WriteValue(Disk.BootSector.BPB.RootEntryCount)
            xw.WriteEndElement()

            xw.WriteStartElement("totalSectorCount")
            If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorCount <> BPBBySize.SectorCount Then
                xw.WriteAttributeString("custom", "true")
            End If
            xw.WriteValue(Disk.BootSector.BPB.SectorCount)
            xw.WriteEndElement()

            xw.WriteStartElement("mediaDescriptor")
            If DoBPBCompare AndAlso Disk.BootSector.BPB.MediaDescriptor <> BPBBySize.MediaDescriptor Then
                xw.WriteAttributeString("custom", "true")
            End If
            If Disk.BootSector.BPB.IsValid Then
                If Not Disk.BPB.HasValidMediaDescriptor Then
                    xw.WriteAttributeString("invalid", "true")
                ElseIf Disk.DiskParams.Format = FloppyDiskFormat.FloppyXDF35 AndAlso Disk.FAT.MediaDescriptor = &HF9 Then
                    'Do Nothing - This is normal for XDF
                ElseIf Disk.DiskParams.Format <> FloppyDiskFormat.FloppyUnknown AndAlso Disk.BootSector.BPB.MediaDescriptor <> Disk.DiskParams.BPBParams.MediaDescriptor Then
                    xw.WriteAttributeString("mismatched", "true")
                ElseIf Disk.FAT.MediaDescriptor <> Disk.BootSector.BPB.MediaDescriptor Then
                    xw.WriteAttributeString("mismatched", "true")
                End If
            End If
            xw.WriteValue(Disk.BootSector.BPB.MediaDescriptor.ToString("X2"))
            xw.WriteEndElement()

            xw.WriteStartElement("sectorsPerFAT")
            If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorsPerFAT <> BPBBySize.SectorsPerFAT Then
                xw.WriteAttributeString("custom", "true")
            End If
            xw.WriteValue(Disk.BootSector.BPB.SectorsPerFAT)
            xw.WriteEndElement()

            xw.WriteStartElement("sectorsPerTrack")
            If DoBPBCompare AndAlso Disk.BootSector.BPB.SectorsPerTrack <> BPBBySize.SectorsPerTrack Then
                xw.WriteAttributeString("custom", "true")
            End If
            xw.WriteValue(Disk.BootSector.BPB.SectorsPerTrack)
            xw.WriteEndElement()

            xw.WriteStartElement("numberOfHeads")
            If DoBPBCompare AndAlso Disk.BootSector.BPB.NumberOfHeads <> BPBBySize.NumberOfHeads Then
                xw.WriteAttributeString("custom", "true")
            End If
            xw.WriteValue(Disk.BootSector.BPB.NumberOfHeads)
            xw.WriteEndElement()

            If Disk.BootSector.BPB.HiddenSectors > 0 Then
                xw.WriteElementString("hiddenSectors", Disk.BootSector.BPB.HiddenSectors)
            End If

            Dim BootStrapStart = Disk.BootSector.GetBootStrapOffset

            If BootStrapStart >= BootSector.BootSectorOffsets.BootStrapCode Then
                If Disk.BootSector.DriveNumber > 0 Then
                    xw.WriteElementString("driveNumber", Disk.BootSector.DriveNumber)
                End If

                If Disk.BootSector.HasValidExtendedBootSignature Then
                    xw.WriteElementString("volumeSerialNumber", Disk.BootSector.VolumeSerialNumber.ToString("X8").Insert(4, "-"))
                    If Disk.BootSector.ExtendedBootSignature = BootSector.ValidExtendedBootSignature(1) Then
                        xw.WriteElementString("volumeLabel", Disk.BootSector.GetVolumeLabelString.TrimEnd(NULL_CHAR))
                        xw.WriteElementString("fileSystemType", Disk.BootSector.GetFileSystemTypeString)
                    End If
                End If
            End If

            If Not Disk.BootSector.HasValidBootStrapSignature Then
                xw.WriteElementString("bootStrapSignature", Disk.BootSector.BootStrapSignature.ToString("X4"))
            End If

            xw.WriteStartElement("bootstrapJmp")
            If Not Disk.BootSector.CheckJumpInstruction(True, True) Then
                xw.WriteAttributeString("invalid", "true")
            End If
            xw.WriteValue(BitConverter.ToString(Disk.BootSector.JmpBoot).Replace("-", ""))
            xw.WriteEndElement()
        End If

        xw.WriteEndElement()
    End Sub

    Private Sub BuildSummaryBootstrapNode(xw As XmlWriter, Disk As Disk, OEMNameResponse As OEMNameResponse)
        xw.WriteStartElement("bootstrap")

        If Not OEMNameResponse.NoBootLoader Then
            Dim BootStrapCRC32 = CRC32.ComputeChecksum(Disk.BootSector.BootStrapCode)
            xw.WriteAttributeString("crc32", BootStrapCRC32.ToString("X8"))
        Else
            If Disk.BootSector.CheckJumpInstruction(False, True) Then
                xw.WriteAttributeString("bootLoader", "None")
            Else
                xw.WriteAttributeString("bootLoader", "Custom")
            End If
        End If

        If OEMNameResponse.Found Then
            If OEMNameResponse.Data.Language.Length > 0 Then
                xw.WriteElementString("language", OEMNameResponse.Data.Language)
            End If

            Dim OEMName = OEMNameResponse.MatchedOEMName

            If OEMName Is Nothing And OEMNameResponse.Data.OEMNames.Count = 1 Then
                OEMName = OEMNameResponse.Data.OEMNames(0)
            End If

            If OEMName IsNot Nothing Then
                If OEMName.Company <> "" Then
                    xw.WriteElementString("company", OEMName.Company)
                End If
                If OEMName.Description <> "" Then
                    xw.WriteElementString("description", OEMName.Description)
                End If
                If OEMName.Note <> "" Then
                    xw.WriteElementString("note", OEMName.Note)
                End If
            End If

            If Disk.BootSector.BPB.IsValid Then
                If Not OEMNameResponse.Data.ExactMatch Then
                    Dim wroteAlternate As Boolean = False

                    For Each OEMName In OEMNameResponse.Data.OEMNames
                        If OEMName.Name.Length > 0 AndAlso OEMName.Suggestion AndAlso OEMName IsNot OEMNameResponse.MatchedOEMName Then
                            If Not wroteAlternate Then
                                xw.WriteStartElement("alternateNames")
                                wroteAlternate = True
                            End If

                            xw.WriteStartElement("oemName")
                            If OEMName.Verified Then
                                xw.WriteAttributeString("verified", "true")
                            End If
                            xw.WriteValue(OEMName.GetNameAsString)
                            xw.WriteEndElement()
                        End If
                    Next

                    If wroteAlternate Then
                        xw.WriteEndElement()
                    End If
                End If
            End If
        End If

        xw.WriteEndElement()
    End Sub

    Private Sub BuildSummaryDiskNode(xw As XmlWriter, Disk As Disk, BootStrapDB As BootstrapDB)
        Dim Value As String

        xw.WriteStartElement("disk")

        xw.WriteElementString("type", GetImageTypeName(Disk.Image.ImageType))

        If Disk.Image.ImageType = FloppyImageType.BasicSectorImage Then
            xw.WriteStartElement("sizeBytes")
            If Disk.IsValidImage AndAlso Disk.CheckImageSize <> 0 Then
                xw.WriteAttributeString("valid", "false")
            End If
            xw.WriteValue(Disk.Image.Length)
            xw.WriteEndElement()
        End If

        If Disk.IsValidImage(False) Then
            Dim DiskFormatString = FloppyDiskFormatGetName(Disk.DiskParams.Format)
            Dim DiskFormatBySize = FloppyDiskFormatGet(Disk.Image.Length)

            If Disk.DiskParams.Format <> FloppyDiskFormat.FloppyUnknown Or DiskFormatBySize = FloppyDiskFormat.FloppyUnknown Then
                xw.WriteElementString("format", DiskFormatString)
            Else
                Dim DiskFormatStringBySize = FloppyDiskFormatGetName(DiskFormatBySize)
                xw.WriteStartElement("format")
                xw.WriteAttributeString("custom", "true")
                xw.WriteValue(DiskFormatStringBySize)
                xw.WriteEndElement()
            End If

            If Disk.DiskParams.IsXDF Then
                Dim XDFChecksum = CalcXDFChecksum(Disk.Image.GetBytes, Disk.BPB.SectorsPerFAT)

                xw.WriteStartElement("xdfChecksum")
                xw.WriteAttributeString("valid", If(XDFChecksum = Disk.GetXDFChecksum, "true", "false"))
                xw.WriteValue(XDFChecksum.ToString("X8"))
                xw.WriteEndElement()
            End If

            If Disk.BPB.IsValid AndAlso Disk.CheckImageSize > 0 AndAlso Disk.DiskParams.Format <> FloppyDiskFormat.FloppyUnknown Then
                xw.WriteElementString("truncatedCRC32", HashFunctions.CRC32Hash(Disk.Image.GetBytes(0, Disk.BPB.ReportedImageSize())))
            End If
        End If

        If Disk.Image.ImageType = FloppyImageType.TranscopyImage Then
            Dim Image As ImageFormats.TC.TransCopyImage = DirectCast(Disk.Image, ImageFormats.TC.TranscopyFloppyImage).Image
            Value = ImageFormats.TC.DiskTypeToString(Image.DiskType)
            xw.WriteElementString("diskType", Value)
        End If

        If Disk.Image.ImageType <> FloppyImageType.BasicSectorImage Then
            xw.WriteElementString("tracks", Disk.Image.TrackCount)
            xw.WriteElementString("heads", Disk.Image.SideCount)
        End If

        If Disk.Image.IsBitstreamImage Then
            If Disk.Image.BitstreamImage.TrackStep <> 1 Then
                xw.WriteElementString("trackStep", Disk.Image.BitstreamImage.TrackStep)
            End If
            If Disk.Image.BitstreamImage.VariableBitRate Then
                xw.WriteElementString("bitRate", "Variable")
            ElseIf Disk.Image.BitstreamImage.BitRate <> 0 Then
                xw.WriteElementString("bitRate", Disk.Image.BitstreamImage.BitRate)
            End If

            If Disk.Image.BitstreamImage.VariableRPM Then
                xw.WriteElementString("rpm", "Variable")
            ElseIf Disk.Image.BitstreamImage.RPM <> 0 Then
                xw.WriteElementString("rpm", Disk.Image.BitstreamImage.RPM)
            End If
        End If

        If Disk.Image.ImageType = FloppyImageType.D86FImage Then
            Dim Image As ImageFormats.D86F.D86FImage = DirectCast(Disk.Image, ImageFormats.D86F.D86FFloppyImage).Image
            If Image.RPMSlowDown <> 0 Then
                If Image.AlternateBitcellCalculation Then
                    xw.WriteElementString("rpmPercentUp", Image.RPMSlowDown * 100)
                Else
                    xw.WriteElementString("rpmPercentDown", Image.RPMSlowDown * 100)
                End If
            End If
            xw.WriteElementString("hasSurfaceData", If(Image.HasSurfaceData, "true", "false"))

        ElseIf Disk.Image.HasWeakBitsSupport Then
            xw.WriteElementString("hasWeakBits", If(Disk.Image.HasWeakBits, "true", "false"))
        End If

        If Disk.Image.NonStandardTracks.Count > 0 Or Disk.Image.AdditionalTracks.Count > 0 Then
            Dim TrackList As New HashSet(Of UShort)(Disk.Image.NonStandardTracks.Concat(Disk.Image.AdditionalTracks))
            BuildNonStandardTracks(xw, TrackList, Disk.Image.SideCount)
        End If

        If Not Disk.IsValidImage Then
            xw.WriteElementString("fileSystem", "Unknown")
        End If

        xw.WriteEndElement()
    End Sub

    Private Sub BuildSummaryFileSystemNode(xw As XmlWriter, Disk As Disk)
        xw.WriteStartElement("fileSystem")

        If Disk.FAT.HasMediaDescriptor Then
            xw.WriteStartElement("mediaDescriptor")
            If Not Disk.FAT.HasValidMediaDescriptor Then
                xw.WriteAttributeString("invalid", "true")
            ElseIf Disk.DiskParams.Format = FloppyDiskFormat.FloppyXDF35 AndAlso Disk.FAT.MediaDescriptor = &HF9 Then
                'Do Nothing - This is normal for XDF
            ElseIf Disk.DiskParams.Format <> FloppyDiskFormat.FloppyUnknown AndAlso Disk.FAT.MediaDescriptor <> Disk.DiskParams.BPBParams.MediaDescriptor Then
                xw.WriteAttributeString("mismatched", "true")
            End If
            xw.WriteValue(Disk.FAT.MediaDescriptor.ToString("X2"))
            xw.WriteEndElement()
        End If

        Dim fsi = SummaryPanel.GetFileSystemInfo(Disk)

        If fsi.VolumeLabel IsNot Nothing Then
            xw.WriteElementString("volumeLabel", fsi.VolumeLabel.GetVolumeName.TrimEnd(NULL_CHAR))
            Dim VolumeDate = fsi.VolumeLabel.GetLastWriteDate
            xw.WriteElementString("volumeDate", VolumeDate.DateObject.ToString("yyyy-MM-ddTHH:mm:ss"))
        End If

        xw.WriteElementString("totalSpaceBytes", Disk.BPB.SectorToBytes(Disk.BPB.DataRegionSize))
        xw.WriteElementString("freeSpaceBytes", Disk.FAT.GetFreeSpace())

        If Disk.FAT.BadClusters.Count > 0 Then
            Dim SectorCount = Disk.FAT.BadClusters.Count * Disk.BPB.SectorsPerCluster
            Dim BadSectorBytes As Integer = Disk.FAT.BadClusters.Count * Disk.BPB.BytesPerCluster
            xw.WriteElementString("badSectorCount", SectorCount)
            xw.WriteElementString("badSectorBytes", BadSectorBytes)
        End If

        Dim LostClusters = Disk.RootDirectory.FATAllocation.LostClusters.Count
        If LostClusters > 0 Then
            Dim LostClusterBytes As Integer = LostClusters * Disk.BPB.BytesPerCluster
            xw.WriteElementString("lostClusterCount", LostClusters)
            xw.WriteElementString("lostClusterBytes", LostClusterBytes)
        End If

        Dim ReservedClusters = Disk.FAT.ReservedClusters.Count
        If ReservedClusters > 0 Then
            Dim ReservedClusterBytes As Integer = ReservedClusters * Disk.BPB.BytesPerCluster
            xw.WriteElementString("reservedClusterCount", ReservedClusters)
            xw.WriteElementString("reservedClusterBytes", ReservedClusterBytes)
        End If

        If fsi.OldestFileDate.HasValue Then
            xw.WriteElementString("oldestFileDate", fsi.OldestFileDate.Value.ToString("yyyy-MM-ddTHH:mm:ss"))
        End If

        If fsi.NewestFileDate.HasValue Then
            xw.WriteElementString("newestFileDate", fsi.NewestFileDate.Value.ToString("yyyy-MM-ddTHH:mm:ss"))
        End If

        If Disk.DiskParams.IsXDF Then
            'Do Nothing
        ElseIf Not Disk.FATTables.FATsMatch Then
            xw.WriteElementString("fatsMatch", "false")
        End If

        xw.WriteEndElement()
    End Sub

    Private Sub BuildSummaryNode(xw As XmlWriter, Disk As Disk, BootStrapDB As BootstrapDB, TitleDB As FloppyDB, MD5 As String)
        xw.WriteStartElement("summary")

        If TitleDB IsNot Nothing AndAlso TitleDB.TitleCount > 0 Then
            Dim TitleFindResult = TitleDB.TitleFind(MD5)
            If TitleFindResult.Matches IsNot Nothing Then
                BuildSummaryTitleNode(xw, TitleFindResult)
            End If
        End If

        BuildSummaryDiskNode(xw, Disk, BootStrapDB)

        If Disk.IsValidImage Then
            Dim OEMNameResponse = BootStrapDB.CheckOEMName(Disk.BootSector)

            BuildSummaryBootRecordNode(xw, Disk, OEMNameResponse)

            BuildSummaryFileSystemNode(xw, Disk)
            BuildSummaryBootstrapNode(xw, Disk, OEMNameResponse)
        End If

        xw.WriteEndElement()
    End Sub

    Private Sub BuildSummaryTitleNode(xw As XmlWriter, Result As FloppyDB.TitleFindResult)
        For Each Title In Result.Matches
            xw.WriteStartElement("title")

            WriteElementStringIFNotEmpty(xw, "name", Title.GetName)
            WriteElementStringIFNotEmpty(xw, "variant", Title.GetVariation)
            WriteElementStringIFNotEmpty(xw, "compilation", Title.GetCompilation)
            WriteElementStringIFNotEmpty(xw, "publisher", Title.GetPublisher)
            WriteElementStringIFNotEmpty(xw, "year", Title.GetYear)
            WriteElementStringIFNotEmpty(xw, "os", Title.GetOperatingSystem)
            WriteElementStringIFNotEmpty(xw, "region", Title.GetRegion)
            WriteElementStringIFNotEmpty(xw, "language", Title.GetLanguage)
            WriteElementStringIFNotEmpty(xw, "version", Title.GetVersion)
            WriteElementStringIFNotEmpty(xw, "disk", Title.GetDisk)
            WriteElementStringIFNotEmpty(xw, "protection", Title.GetCopyProtection)

            Dim Status = Title.GetStatus
            Dim StatusString As String = ""
            If Status = FloppyDB.FloppyDBStatus.Verified Then
                StatusString = "Verified"
            ElseIf Status = FloppyDB.FloppyDBStatus.Unverified Then
                If Title.GetFixed Then
                    StatusString = "Fixed"
                Else
                    StatusString = "Unverified"
                End If
            ElseIf Status = FloppyDB.FloppyDBStatus.Modified Then
                StatusString = "Modified"
            End If

            WriteElementStringIFNotEmpty(xw, "status", StatusString)

            xw.WriteEndElement()
        Next
    End Sub

    Private Sub ProcessDirectoryEntries(xw As XmlWriter, Directory As DiskImage.IDirectory, Path As String)
        Dim DirectoryEntryCount = Directory.Data.EntryCount

        If DirectoryEntryCount > 0 Then
            Dim HasLFN As Boolean = False
            Dim LFNFileName As String = ""
            Dim LFNIndex As Byte = 0
            Dim LFNChecksum As Byte = 0

            Dim EntryCount = 0
            For Counter = 0 To DirectoryEntryCount - 1
                Dim DirectoryEntry = Directory.GetFile(Counter)

                If Not DirectoryEntry.IsLink And Not DirectoryEntry.IsFileEmpty Then
                    If DirectoryEntry.IsLFN Then
                        If LFNIndex > 0 Then
                            LFNIndex -= 1
                        End If
                        LFNIndex = DirectoryEntry.LFNGetNextSequence(LFNIndex, LFNChecksum, True)
                        If LFNIndex > 0 Then
                            LFNChecksum = DirectoryEntry.LFNChecksum
                            If (LFNIndex And &H40) > 0 Then
                                LFNIndex = LFNIndex And Not &H40
                                LFNFileName = DirectoryEntry.GetLFNFileName
                            Else
                                LFNFileName = DirectoryEntry.GetLFNFileName & LFNFileName
                            End If
                            HasLFN = True
                        Else
                            LFNFileName = ""
                            HasLFN = False
                        End If
                    Else
                        If HasLFN Then
                            If LFNIndex <> 1 Or LFNChecksum <> DirectoryEntry.CalculateLFNChecksum Then
                                LFNFileName = ""
                                HasLFN = False
                            End If
                        End If
                        If Not HasLFN Then
                            If Not DirectoryEntry.HasNTUnknownFlags And (DirectoryEntry.HasNTLowerCaseFileName Or DirectoryEntry.HasNTLowerCaseExtension) Then
                                LFNFileName = DirectoryEntry.GetNTFileName
                            End If
                        End If

                        If DirectoryEntry.IsDirectory AndAlso DirectoryEntry.SubDirectory IsNot Nothing Then
                            If DirectoryEntry.SubDirectory.Data.EntryCount > 0 Then
                                BuildDirectoryEntryNode(xw, DirectoryEntry, Path, LFNFileName)

                                Dim NewPath As String = Path & "\" & DirectoryEntry.GetShortFileName
                                ProcessDirectoryEntries(xw, DirectoryEntry.SubDirectory, NewPath)

                                xw.WriteEndElement()

                            End If
                        Else
                            BuildDirectoryEntryNode(xw, DirectoryEntry, Path, LFNFileName)
                            xw.WriteEndElement()
                        End If


                        LFNFileName = ""
                        LFNIndex = 0
                        LFNChecksum = 0
                        HasLFN = False
                    End If
                    EntryCount += 1
                End If
            Next
        End If
    End Sub
    Private Function SanitizeXmlText(s As String) As String
        If s Is Nothing Then
            Return Nothing
        End If

        Dim sb As New StringBuilder(s.Length)
        For Each ch As Char In s
            If XmlConvert.IsXmlChar(ch) Then
                sb.Append(ch)
            End If
        Next
        Return sb.ToString()
    End Function
    Private Sub WriteElementStringIFNotEmpty(xw As XmlWriter, ElementName As String, Value As String)
        If Not String.IsNullOrEmpty(Value) Then
            xw.WriteElementString(ElementName, Value)
        End If
    End Sub
End Module

