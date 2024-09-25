Imports System.Globalization
Imports DiskImageTool.DiskImage
Imports DiskImageTool.DiskImage.BootSector
Imports Hb.Windows.Forms

Public Class BootSectorForm
    Const ValidHexChars = "0123456789ABCDEF"
    Private ReadOnly _BootStrap As BoootstrapDB
    Private ReadOnly _BootSector As BootSector
    Private ReadOnly _HasExtended As Boolean
    Private ReadOnly _HelpProvider1 As HelpProvider
    Private _SuppressEvent As Boolean = True

    Public Sub New(Data() As Byte, BootStrap As BoootstrapDB)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _BootSector = New BootSector(Data)
        _BootStrap = BootStrap
        _HelpProvider1 = New HelpProvider

        Dim BootStrapStart = _BootSector.GetBootStrapOffset
        _HasExtended = BootStrapStart >= BootSectorOffsets.FileSystemType + BootSectorSizes.FileSystemType

        Dim DiskFormat = GetFloppyDiskFormat(_BootSector.BPB, True, False)

        IntitializeHelp()
        PopulateDiskTypes(DiskFormat)
        SetCurrentDiskFormat(DiskFormat)
        PopulateValues()

        _SuppressEvent = False
    End Sub

    Public ReadOnly Property Data As Byte()
        Get
            Return _BootSector.Data
        End Get
    End Property

    Private Sub ChangeVolumeSerialNumber()
        Dim VolumeSerialNumberForm As New VolumeSerialNumberForm()

        VolumeSerialNumberForm.ShowDialog()

        Dim Result As Boolean = VolumeSerialNumberForm.DialogResult = DialogResult.OK

        If Result Then
            _SuppressEvent = True
            Dim Value = VolumeSerialNumberForm.GetValue()
            ControlSetValue(HexVolumeSerialNumber, BootSector.GenerateVolumeSerialNumber(Value).ToString("X8"), False)
            _SuppressEvent = False
        End If
    End Sub

    Private Sub IntitializeHelp()
        Dim Msg As String

        Msg = "OEM Name Identifier" &
            $"{vbCrLf}{vbCrLf}Can be set by a FAT implementation to any desired value" &
            $"{vbCrLf}{vbCrLf}Typically this is some indication of what system formatted the volume."
        SetHelpString(Msg, LblOEMName, CboOEMName, HexOEMName)

        Msg = "Number of bytes in each physical sector" &
             $"{vbCrLf}{vbCrLf}Allowed Values: 512, 1024, 2048, 4096" &
             $"{vbCrLf}{vbCrLf}Note: This value should be 512 for all floppy disks."
        SetHelpString(Msg, LblBytesPerSector, CboBytesPerSector)

        Msg = "Number of sectors per cluster" &
            $"{vbCrLf}{vbCrLf}Allowed Values: 1, 2, 4, 8, 16, 32, 128" &
            $"{vbCrLf}{vbCrLf}Typical Values:" &
            $"{vbCrLf}160K Floppy{vbTab}1" &
            $"{vbCrLf}180K Floppy{vbTab}1" &
            $"{vbCrLf}320K Floppy{vbTab}2" &
            $"{vbCrLf}360K Floppy{vbTab}2" &
            $"{vbCrLf}720K Floppy{vbTab}2" &
            $"{vbCrLf}1.2M Floppy{vbTab}1" &
            $"{vbCrLf}1.44M Floppy{vbTab}1" &
            $"{vbCrLf}2.88M Floppy{vbTab}2" &
            $"{vbCrLf}DMF Floppy{vbTab}2 or 4" &
            $"{vbCrLf}XDF Floppy{vbTab}1"
        SetHelpString(Msg, LblSectorsPerCluster, CboSectorsPerCluster)

        Msg = "Number of reserved sectors in the reserved region of the volume starting at the first sector of the volume" &
            $"{vbCrLf}{vbCrLf}Allowed Values: Any non-zero value" &
            $"{vbCrLf}{vbCrLf}Note: This value should typically be set to 1."
        SetHelpString(Msg, LblReservedSectors, TxtReservedSectors)

        Msg = "Number of File Allocation Table (FAT) copies on the volume" &
            $"{vbCrLf}{vbCrLf}Allowed Values: Any non-zero value" &
            $"{vbCrLf}{vbCrLf}Note: This value should typically be set to 2."
        SetHelpString(Msg, LblNumberOfFATS, TxtNumberOfFATs)

        Msg = "Number of entries in the root directory" &
            $"{vbCrLf}{vbCrLf}Allowed Values: Value multiplied by 32 should be an even multiple of Bytes Per Sector" &
            $"{vbCrLf}{vbCrLf}Typical Values:" &
            $"{vbCrLf}160K Floppy{vbTab}64" &
            $"{vbCrLf}180K Floppy{vbTab}64" &
            $"{vbCrLf}320K Floppy{vbTab}112" &
            $"{vbCrLf}360K Floppy{vbTab}112" &
            $"{vbCrLf}720K Floppy{vbTab}112" &
            $"{vbCrLf}1.2M Floppy{vbTab}224" &
            $"{vbCrLf}1.44M Floppy{vbTab}224" &
            $"{vbCrLf}2.88M Floppy{vbTab}240" &
            $"{vbCrLf}DMF Floppy{vbTab}16" &
            $"{vbCrLf}XDF Floppy{vbTab}224"
        SetHelpString(Msg, LblRootDirectoryEntries, TxtRootDirectoryEntries)

        Msg = "Total number of sectors in the volume" &
            $"{vbCrLf}{vbCrLf}Typical Values:" &
            $"{vbCrLf}160K Floppy{vbTab}{vbTab}320" &
            $"{vbCrLf}180K Floppy{vbTab}{vbTab}360" &
            $"{vbCrLf}320K Floppy{vbTab}{vbTab}640" &
            $"{vbCrLf}360K Floppy{vbTab}{vbTab}720" &
            $"{vbCrLf}720K Floppy{vbTab}{vbTab}1440" &
            $"{vbCrLf}1.2M Floppy{vbTab}{vbTab}2400" &
            $"{vbCrLf}1.44M Floppy{vbTab}{vbTab}2880" &
            $"{vbCrLf}2.88M Floppy{vbTab}{vbTab}5760" &
            $"{vbCrLf}DMF Floppy{vbTab}{vbTab}3360" &
            $"{vbCrLf}5.25"" XDF Floppy{vbTab}3040" &
            $"{vbCrLf}3.5"" XDF Floppy{vbTab}{vbTab}3680"
        SetHelpString(Msg, LblSectorCountSmall, TxtSectorCountSmall)

        Msg = "Media Descriptor" &
            $"{vbCrLf}{vbCrLf}Allowed Values:" &
            $"{vbCrLf}F0{vbTab}1.44M, 2.88M, DMF, 3.5"" XDF Floppy" &
            $"{vbCrLf}F8{vbTab}Fixed Disk" &
            $"{vbCrLf}F9{vbTab}720K, 1.2M, 5.25"" XDF Floppy" &
            $"{vbCrLf}FA{vbTab}Unused" &
            $"{vbCrLf}FB{vbTab}Unused" &
            $"{vbCrLf}FC{vbTab}180K Floppy" &
            $"{vbCrLf}FD{vbTab}360K Floppy" &
            $"{vbCrLf}FE{vbTab}160K Floppy" &
            $"{vbCrLf}FF{vbTab}320K Floppy" &
            $"{vbCrLf}ED{vbTab}Tandy 2000 Floppy"
        SetHelpString(Msg, TxtMediaDescriptor, CboMediaDescriptor)

        Msg = "Number of sectors allocated to each copy of the File Allocation Table (FAT)" &
            $"{vbCrLf}{vbCrLf}Typical Values:" &
            $"{vbCrLf}160K Floppy{vbTab}{vbTab}1" &
            $"{vbCrLf}180K Floppy{vbTab}{vbTab}2" &
            $"{vbCrLf}320K Floppy{vbTab}{vbTab}1" &
            $"{vbCrLf}360K Floppy{vbTab}{vbTab}2" &
            $"{vbCrLf}720K Floppy{vbTab}{vbTab}3" &
            $"{vbCrLf}1.2M Floppy{vbTab}{vbTab}7" &
            $"{vbCrLf}1.44M Floppy{vbTab}{vbTab}9" &
            $"{vbCrLf}2.88M Floppy{vbTab}{vbTab}9" &
            $"{vbCrLf}DMF Floppy{vbTab}{vbTab}3 or 5" &
            $"{vbCrLf}5.25"" XDF Floppy{vbTab}9" &
            $"{vbCrLf}3.5"" XDF Floppy{vbTab}{vbTab}11"
        SetHelpString(Msg, LblSectorsPerFAT, TxtSectorsPerFAT)

        Msg = "Number of sectors per track on the disk" &
            $"{vbCrLf}{vbCrLf}Typical Values:" &
            $"{vbCrLf}160K Floppy{vbTab}{vbTab}8" &
            $"{vbCrLf}180K Floppy{vbTab}{vbTab}9" &
            $"{vbCrLf}320K Floppy{vbTab}{vbTab}8" &
            $"{vbCrLf}360K Floppy{vbTab}{vbTab}9" &
            $"{vbCrLf}720K Floppy{vbTab}{vbTab}9" &
            $"{vbCrLf}1.2M Floppy{vbTab}{vbTab}15" &
            $"{vbCrLf}1.44M Floppy{vbTab}{vbTab}18" &
            $"{vbCrLf}2.88M Floppy{vbTab}{vbTab}36" &
            $"{vbCrLf}DMF Floppy{vbTab}{vbTab}21" &
            $"{vbCrLf}5.25"" XDF Floppy{vbTab}19" &
            $"{vbCrLf}3.5"" XDF Floppy{vbTab}{vbTab}23"
        SetHelpString(Msg, LblSectorsPerTrack, TxtSectorsPerTrack)

        Msg = "Number of physical heads (sides) on the disk" &
            $"{vbCrLf}{vbCrLf}Typical Values:" &
            $"{vbCrLf}160K Floppy{vbTab}1" &
            $"{vbCrLf}180K Floppy{vbTab}1" &
            $"{vbCrLf}320K Floppy{vbTab}2" &
            $"{vbCrLf}360K Floppy{vbTab}2" &
            $"{vbCrLf}720K Floppy{vbTab}2" &
            $"{vbCrLf}1.2M Floppy{vbTab}2" &
            $"{vbCrLf}1.44M Floppy{vbTab}2" &
            $"{vbCrLf}2.88M Floppy{vbTab}2" &
            $"{vbCrLf}DMF Floppy{vbTab}2" &
            $"{vbCrLf}XDF Floppy{vbTab}2"
        SetHelpString(Msg, LblNumberOfHeads, TxtNumberOfHeads)

        Msg = "Number of sectors preceeding the first sector of a partitioned volume" &
             $"{vbCrLf}{vbCrLf}Note: This value should be 0 for all floppy disks"
        SetHelpString(Msg, LblHiddenSectors, TxtHiddenSectors)

        Msg = "Total number of sectors in a FAT16 volume larger than 65535 sectors" &
             $"{vbCrLf}{vbCrLf}Note: This value should be 0 for all floppy disks"
        SetHelpString(Msg, LblSectorCountLarge, TxtSectorCountLarge)

        Msg = "Interrupt 13h drive number" &
            $"{vbCrLf}{vbCrLf}Allowed Values: 0, 128"
        SetHelpString(Msg, LblDriveNumber, TxtDriveNumber)

        Msg = "Extended Boot Signature" &
            $"{vbCrLf}{vbCrLf}Typical Values:" &
            $"{vbCrLf}28h{vbTab}Volume Serial Number is present" &
            $"{vbCrLf}29h{vbTab}Volume Serial Number, Volume Label, and File System ID are present"
        SetHelpString(Msg, lblExtendedBootSignature, HexExtendedBootSignature)

        Msg = "The Volume Serial Number is a 32-bit random number used in conjunction with the Volume Label for removable media tracking" &
            $"{vbCrLf}{vbCrLf}Note: This id is typically generated by converting the current date and time into a 32-bit value"
        SetHelpString(Msg, LblVolumeSerialNumber, HexVolumeSerialNumber)

        Msg = "This field typically matches the 11-byte volume label in the root directory of the disk or has the value ""NO NAME    "" if the volume label does not exist."
        SetHelpString(Msg, LblVolumeLabel, TxtVolumeLabel, HexVolumeLabel)

        Msg = "The File System ID is informational only" &
            $"{vbCrLf}{vbCrLf}Typical Values: FAT12, FAT16, FAT"
        SetHelpString(Msg, LblFileSystemType, TxtFileSystemType, HexFileSystemType)

        Msg = "The disk type detected based on the current values in the Boot Record.  Changing this will set the values in the boot record to those of the selected disk type."
        SetHelpString(Msg, LblDiskType, CboDiskType)

        Msg = "Generate a new volume serial number based on a user supplied date and time"
        SetHelpString(Msg, BtnVolumeSerialNumber)

        Msg = "This instruction indicates where the bootstrap code starts." &
            $"{vbCrLf}{vbCrLf}Allowed Values: EB xx 90, E9 xx xx"
        SetHelpString(Msg, LblJumpInstruction, HexJumpInstruction)

        Msg = "Indicated to the BIOS that the sector is executable." &
            $"{vbCrLf}{vbCrLf}Allowed Values:" &
            $"{vbCrLf}AA 55" &
            $"{vbCrLf}00 00 (PC DOS 1.x)" &
            $"{vbCrLf}54 42 (Tandy 2000)"
        SetHelpString(Msg, LblBootSectorSignature, HexBootSectorSignature)

        Msg = $"This Is additional data found in the Boot Sector.{vbCrLf}{vbCrLf}Note: Data highlighted in green is the bootstrap code."
        SetHelpString(Msg, HexBox1)
    End Sub

    Private Function IsByteArrayNull(b() As Byte) As Boolean
        Dim Result As Boolean = True

        For Each value In b
            If value <> 0 Then
                Result = False
                Exit For
            End If
        Next

        Return Result
    End Function

    Private Function IsDataValid(Control As Control) As Boolean
        If Control Is TxtReservedSectors Then
            Return Control.Text <> "0"
        ElseIf Control Is TxtNumberOfFATs Then
            Return Control.Text <> "0"
        ElseIf Control Is TxtRootDirectoryEntries Then
            Return Control.Text <> "0"
        ElseIf Control Is TxtSectorCountSmall Then
            Return Control.Text <> "0"
        ElseIf Control Is TxtSectorsPerFAT Then
            Return Control.Text <> "0"
        ElseIf Control Is TxtHiddenSectors Then
            Return Control.Text = "0"
        ElseIf Control Is HexJumpInstruction Then
            Return BootSector.CheckJumpInstruction(HexJumpInstruction.GetHex, True, True)
        Else
            Return True
        End If
    End Function

    Private Sub PopulateAdditionalData(BootSector As BootSector)
        Dim DataStart As UShort
        Dim DataLength As UShort = 0
        Dim BootStrapStart = BootSector.GetBootStrapOffset

        If BootSector.HasValidExtendedBootSignature And BootStrapStart >= BootSectorOffsets.FileSystemType + BootSectorSizes.FileSystemType Then
            DataStart = BootSector.BootSectorOffsets.FileSystemType + BootSector.BootSectorSizes.FileSystemType
        Else
            DataStart = BiosParameterBlock.BPBOoffsets.HiddenSectors + BiosParameterBlock.BPBSizes.HiddenSectors
            'If BootSector.HiddenSectors >> 4 > 0 Then
            'DataStart -= 2
            'End If
        End If

        If BootSectorOffsets.BootStrapSignature > DataStart Then
            DataLength = BootSectorOffsets.BootStrapSignature - DataStart
        End If

        If DataLength > 0 Then
            Dim Data(DataLength - 1) As Byte
            Array.Copy(BootSector.Data, DataStart, Data, 0, DataLength)

            HexBox1.ByteProvider = New DynamicByteProvider(Data)
            Dim LineCount As Integer = Math.Ceiling(DataLength / HexBox1.BytesPerLine)
            If LineCount = 1 Then
                LineCount = 2
            ElseIf LineCount > 10 Then
                LineCount = 10
                HexBox1.VScrollBarVisible = True
            End If
            HexBox1.Height = LineCount * 13 + 4

            If BootSector.CheckJumpInstruction(False, True) Then
                If BootStrapStart >= DataStart Then
                    Dim Start = BootStrapStart - DataStart
                    HexBox1.Highlight(Start, DataLength - Start, Color.Green, Color.White)
                End If
            End If

            GroupBoxAdditionalData.Visible = True
        Else
            GroupBoxAdditionalData.Visible = False
        End If
    End Sub

    Private Sub PopulateBPB(BPB As BiosParameterBlock)
        ControlSetValue(CboBytesPerSector, BPB.BytesPerSector, Array.ConvertAll(BiosParameterBlock.ValidBytesPerSector, Function(x) x.ToString()), True)
        ControlSetValue(CboSectorsPerCluster, BPB.SectorsPerCluster, Array.ConvertAll(BiosParameterBlock.ValidSectorsPerCluster, Function(x) x.ToString()), True)
        ControlSetValue(TxtReservedSectors, BPB.ReservedSectorCount, True)
        ControlSetValue(TxtNumberOfFATs, BPB.NumberOfFATs, True)
        ControlSetValue(TxtRootDirectoryEntries, BPB.RootEntryCount, True)
        ControlSetValue(TxtSectorCountSmall, BPB.SectorCountSmall, True)
        ControlSetValue(CboMediaDescriptor, BPB.MediaDescriptor.ToString("X2"), Array.ConvertAll(BiosParameterBlock.ValidMediaDescriptor, Function(x) x.ToString("X2")), True)
        ControlSetValue(TxtSectorsPerFAT, BPB.SectorsPerFAT, True)
        ControlSetValue(TxtSectorsPerTrack, BPB.SectorsPerTrack, {"8", "9", "15", "18", "21", "23", "36"}, True)
        ControlSetValue(TxtNumberOfHeads, BPB.NumberOfHeads, {"1", "2"}, True)
    End Sub

    Private Sub PopulateBytesPerSector()
        CboBytesPerSector.Items.Clear()
        For Each Value In BiosParameterBlock.ValidBytesPerSector
            CboBytesPerSector.Items.Add(Value)
        Next
    End Sub

    Private Sub PopulateDiskTypes(DiskFormat As FloppyDiskFormat)
        CboDiskType.Items.Clear()
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.Floppy160))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.Floppy180))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.Floppy320))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.Floppy360))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.Floppy720))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.Floppy1200))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.Floppy1440))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.Floppy2880))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.FloppyDMF1024))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.FloppyDMF2048))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.FloppyXDF525))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.FloppyXDF35))
        If DiskFormat = FloppyDiskFormat.FloppyXDFMicro Then
            CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.FloppyXDFMicro))
        End If
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.FloppyProCopy))
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.FloppyTandy2000))
        If DiskFormat = FloppyDiskFormat.FloppyNoBPB Then
            CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.FloppyNoBPB))
        End If
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.FloppyUnknown))

        CboDiskType.SelectedIndex = CboDiskType.Items.Count - 1
    End Sub

    Private Sub PopulateExtended(BootSector As BootSector)
        ControlSetValue(TxtSectorCountLarge, BootSector.BPB.SectorCountLarge, False)
        ControlSetValue(TxtDriveNumber, BootSector.DriveNumber, False)
        ControlSetValue(HexExtendedBootSignature, BootSector.ExtendedBootSignature.ToString("X2"), False)
        ControlSetValue(HexVolumeSerialNumber, BootSector.VolumeSerialNumber.ToString("X8"), False)
        ControlSetValue(TxtVolumeLabel, BootSector.GetVolumeLabelString.TrimEnd, False)
        HexVolumeLabel.SetHex(BootSector.VolumeLabel)
        ControlSetValue(TxtFileSystemType, BootSector.GetFileSystemTypeString.TrimEnd, False)
        HexFileSystemType.SetHex(BootSector.FileSystemType)

        Dim ErrorCount As Integer = 0
        If BootSector.BPB.SectorCountLarge <> 0 Then
            ErrorCount += 1
        End If
        If BootSector.ExtendedBootSignature <> 0 And BootSector.ExtendedBootSignature <> &H28 And BootSector.ExtendedBootSignature <> &H29 Then
            ErrorCount += 1
        End If
        If BootSector.DriveNumber <> 0 And BootSector.DriveNumber <> 128 Then
            ErrorCount += 1
        End If
        LblExtendedMsg.Visible = (ErrorCount > 1)
    End Sub

    Private Sub PopulateMediaDescriptors()
        CboMediaDescriptor.Items.Clear()
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FE", "160K"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FC", "180K"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FF", "320K"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FD", "360K"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F9", "720K"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F9", "1.2M"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", "1.44M"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", "2.88M"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", "DNF"))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F9", "XDF 5.25"""))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", "XDF 3.5"""))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("ED", "Tandy 2000"))
    End Sub

    Private Sub PopulateOEMName()
        Dim OEMName As New OEMNameData With {
            .Name = _BootSector.OEMName
        }

        Dim BootstrapType As BootstrapLookup = Nothing
        If _BootStrap IsNot Nothing Then
            BootstrapType = _BootStrap.FindMatch(_BootSector.BootStrapCode)
        End If
        If BootstrapType IsNot Nothing Then
            For Each KnownOEMName In BootstrapType.OEMNames
                If KnownOEMName.Name.Length > 0 Then
                    Dim IsMatch = KnownOEMName.Name.CompareTo(OEMName.Name)
                    If (KnownOEMName.Suggestion And Not BootstrapType.ExactMatch) Or IsMatch Then
                        Dim Index = CboOEMName.Items.Add(KnownOEMName)
                        If IsMatch Then
                            CboOEMName.SelectedIndex = Index
                        End If
                    End If
                End If
            Next
        End If

        If CboOEMName.SelectedIndex = -1 Then
            If OEMName.Name.Length > 0 Then
                CboOEMName.Items.Add(OEMName)
            End If
        End If

        ControlSetValue(CboOEMName, _BootSector.GetOEMNameString, False)
        HexOEMName.SetHex(_BootSector.OEMName)
    End Sub

    Private Sub PopulateSectorsPerCluster()
        CboSectorsPerCluster.Items.Clear()
        For Each Value In BiosParameterBlock.ValidSectorsPerCluster
            CboSectorsPerCluster.Items.Add(Value)
        Next
    End Sub

    Private Sub PopulateValues()
        PopulateOEMName()
        PopulateBytesPerSector()
        PopulateSectorsPerCluster()
        PopulateMediaDescriptors()
        PopulateBPB(_BootSector.BPB)

        ControlSetValue(TxtHiddenSectors, _BootSector.BPB.HiddenSectors, False)
        ControlSetValue(HexJumpInstruction, _BootSector.JmpBoot, False)
        ControlSetValue(HexBootSectorSignature, _BootSector.BootStrapSignature.ToString("X4"), Array.ConvertAll(BootSector.ValidBootStrapSignature, Function(x) x.ToString("X4")), False)

        PopulateAdditionalData(_BootSector)

        If _HasExtended Then
            GroupBoxExtended.Visible = True
            PopulateExtended(_BootSector)
            SetExtendedState()
        Else
            GroupBoxExtended.Visible = False
        End If

        UpdateBackColors(GetFloppyDiskFormat(GetBPB(), True, False))
    End Sub

    Private Sub RefreshFileSystemType()
        Dim MaskNulls As Boolean = (HexExtendedBootSignature.GetHex(0) <> &H29)
        Dim b = HexFileSystemType.GetHex
        Dim Value As String
        If MaskNulls AndAlso IsByteArrayNull(b) Then
            Value = ""
        Else
            Value = DiskImage.CodePage437ToUnicode(HexFileSystemType.GetHex).TrimEnd
        End If

        ControlSetValue(TxtFileSystemType, Value, False)
    End Sub

    Private Sub RefreshVolumeLabel()
        Dim BootSignature = HexExtendedBootSignature.GetHex(0)
        Dim MaskNulls As Boolean = (BootSignature <> &H28 And BootSignature <> &H29)
        Dim b = HexVolumeLabel.GetHex
        Dim Value As String
        If MaskNulls AndAlso IsByteArrayNull(b) Then
            Value = ""
        Else
            Value = DiskImage.CodePage437ToUnicode(HexVolumeLabel.GetHex).TrimEnd
        End If

        ControlSetValue(TxtVolumeLabel, Value, False)
    End Sub

    Private Sub AddDiskTypeUnknown()
        Dim LastDiskFormat As BootSectorDiskFormat = CboDiskType.Items.Item(CboDiskType.Items.Count - 1)

        If LastDiskFormat.Format <> FloppyDiskFormat.FloppyUnknown Then
            CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.FloppyUnknown))
        End If
    End Sub

    Private Sub RemoveDiskTypeUnknown()
        Dim LastDiskFormat As BootSectorDiskFormat = CboDiskType.Items.Item(CboDiskType.Items.Count - 1)

        If LastDiskFormat.Format = FloppyDiskFormat.FloppyUnknown Then
            CboDiskType.Items.RemoveAt(CboDiskType.Items.Count - 1)
        End If
    End Sub

    Private Sub SetCurrentDiskFormat(CurrentDiskFormat As FloppyDiskFormat)
        Dim SelectedItem As BootSectorDiskFormat = Nothing

        If CurrentDiskFormat = FloppyDiskFormat.FloppyUnknown Then
            AddDiskTypeUnknown()
        Else
            RemoveDiskTypeUnknown()
        End If

        For Each DiskFormat As BootSectorDiskFormat In CboDiskType.Items
            If DiskFormat.Format = CurrentDiskFormat Then
                SelectedItem = DiskFormat
                Exit For
            End If
        Next

        CboDiskType.SelectedItem = SelectedItem
    End Sub

    Private Sub SetExtendedState()
        Dim Enabled As Boolean

        Dim BootSignature = HexExtendedBootSignature.GetHex(0)

        Enabled = (BootSignature = &H28 Or BootSignature = &H29)
        HexVolumeSerialNumber.Enabled = Enabled
        BtnVolumeSerialNumber.Enabled = Enabled

        Enabled = (BootSignature = &H29)
        TxtVolumeLabel.Enabled = Enabled
        HexVolumeLabel.Enabled = Enabled
        TxtFileSystemType.Enabled = Enabled
        HexFileSystemType.Enabled = Enabled

        RefreshVolumeLabel()
        RefreshFileSystemType()
    End Sub

    Private Sub SetHelpString(HelpString As String, ParamArray ControlArray() As Control)
        For Each Control In ControlArray
            _HelpProvider1.SetHelpString(Control, HelpString)
            _HelpProvider1.SetShowHelp(Control, True)
        Next
    End Sub

    Private Function GetBPB() As BiosParameterBlock
        Dim BPB = New BiosParameterBlock()

        UpdateBPB(BPB)

        Return BPB
    End Function

    Private Sub UpdateBPB(BPB As BiosParameterBlock)
        Dim UShortValue As UShort
        Dim UintegerValue As UInteger
        Dim ByteValue As Byte
        Dim Result As Boolean

        If UShort.TryParse(CboBytesPerSector.Text, UShortValue) Then
            BPB.BytesPerSector = UShortValue
        End If

        If Byte.TryParse(CboSectorsPerCluster.Text, ByteValue) Then
            BPB.SectorsPerCluster = ByteValue
        End If

        If UShort.TryParse(TxtReservedSectors.Text, UShortValue) Then
            BPB.ReservedSectorCount = UShortValue
        End If

        If Byte.TryParse(TxtNumberOfFATs.Text, ByteValue) Then
            BPB.NumberOfFATs = ByteValue
        End If

        If UShort.TryParse(TxtRootDirectoryEntries.Text, UShortValue) Then
            BPB.RootEntryCount = UShortValue
        End If

        If UShort.TryParse(TxtSectorCountSmall.Text, UShortValue) Then
            BPB.SectorCountSmall = UShortValue
        End If

        If UShort.TryParse(CboMediaDescriptor.Text, NumberStyles.HexNumber, CultureInfo.CurrentCulture, Result) Then
            BPB.MediaDescriptor = Convert.ToByte(CboMediaDescriptor.Text, 16)
        End If

        If UShort.TryParse(TxtSectorsPerFAT.Text, UShortValue) Then
            BPB.SectorsPerFAT = UShortValue
        End If

        If UShort.TryParse(TxtSectorsPerTrack.Text, UShortValue) Then
            BPB.SectorsPerTrack = UShortValue
        End If

        If UShort.TryParse(TxtNumberOfHeads.Text, UShortValue) Then
            BPB.NumberOfHeads = UShortValue
        End If

        If UShort.TryParse(TxtHiddenSectors.Text, UShortValue) Then
            BPB.HiddenSectors = UShortValue
        End If

        If _HasExtended Then
            If UInteger.TryParse(TxtSectorCountLarge.Text, UintegerValue) Then
                BPB.SectorCountLarge = UintegerValue
            End If
        End If
    End Sub

    Private Sub UpdateBootSector()
        Dim ByteValue As Byte

        Dim OEMNameString As String = Strings.Left(CboOEMName.Text, 8).PadRight(8)
        Dim OEMName = DiskImage.UnicodeToCodePage437(OEMNameString)
        _BootSector.OEMName = OEMName

        UpdateBPB(_BootSector.BPB)

        _BootSector.JmpBoot = HexJumpInstruction.GetHex

        Dim BootStrapSignature = HexBootSectorSignature.GetHex
        Array.Reverse(BootStrapSignature)
        _BootSector.BootStrapSignature = BitConverter.ToUInt16(BootStrapSignature, 0)

        If _HasExtended Then
            _BootSector.ExtendedBootSignature = HexExtendedBootSignature.GetHex(0)

            If Byte.TryParse(TxtDriveNumber.Text, ByteValue) Then
                _BootSector.DriveNumber = ByteValue
            End If

            Dim VolumeSerial = HexVolumeSerialNumber.GetHex
            Array.Reverse(VolumeSerial)
            _BootSector.VolumeSerialNumber = BitConverter.ToUInt32(VolumeSerial, 0)

            Dim VolumeLabelString As String = Strings.Left(TxtVolumeLabel.Text, 11).PadRight(11)
            Dim VolumeLabel = DiskImage.UnicodeToCodePage437(VolumeLabelString)
            _BootSector.VolumeLabel = VolumeLabel

            Dim FileSystemTypeString As String = Strings.Left(TxtFileSystemType.Text, 8).PadRight(8)
            Dim FileSystemType = DiskImage.UnicodeToCodePage437(FileSystemTypeString)
            _BootSector.FileSystemType = FileSystemType
        End If
    End Sub

    Private Sub UpdateBackColor(Control As Control, KnownDiskType As Boolean)
        ControlUpdateBackColor(Control, KnownDiskType, IsDataValid(Control))
    End Sub

    Private Sub UpdateBackColors(DiskFormat As FloppyDiskFormat)
        Dim KnownDiskFormat = DiskFormat <> FloppyDiskFormat.FloppyUnknown

        UpdateBackColor(CboBytesPerSector, KnownDiskFormat)
        UpdateBackColor(CboSectorsPerCluster, KnownDiskFormat)
        UpdateBackColor(TxtReservedSectors, KnownDiskFormat)
        UpdateBackColor(TxtNumberOfFATs, KnownDiskFormat)
        UpdateBackColor(TxtRootDirectoryEntries, KnownDiskFormat)
        UpdateBackColor(TxtSectorCountSmall, KnownDiskFormat)
        UpdateBackColor(CboMediaDescriptor, KnownDiskFormat)
        UpdateBackColor(TxtSectorsPerFAT, KnownDiskFormat)
        UpdateBackColor(TxtSectorsPerTrack, KnownDiskFormat)
        UpdateBackColor(TxtNumberOfHeads, KnownDiskFormat)
        UpdateBackColor(TxtHiddenSectors, KnownDiskFormat)
        UpdateBackColor(HexJumpInstruction, KnownDiskFormat)
        UpdateBackColor(HexBootSectorSignature, KnownDiskFormat)
    End Sub

    Private Sub UpdateTag(Control As Control, UpdateDiskType As Boolean)
        ControlSetLastValue(Control, Control.Text)

        Dim DiskFormat = GetFloppyDiskFormat(GetBPB(), True, False)

        If UpdateDiskType Then
            SetCurrentDiskFormat(DiskFormat)
        End If

        UpdateBackColors(DiskFormat)
        ControlUpdateColor(Control)
    End Sub

#Region "Events"
    Private Sub BtnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        SetCurrentDiskFormat(GetFloppyDiskFormat(_BootSector.BPB, True, False))
        PopulateValues()
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        UpdateBootSector()
    End Sub

    Private Sub BtnVolumeSerialNumber_Click(sender As Object, e As EventArgs) Handles BtnVolumeSerialNumber.Click
        ChangeVolumeSerialNumber()
    End Sub

    Private Sub CboDiskType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboDiskType.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        _SuppressEvent = True
        Dim DiskFormat = CType(CboDiskType.SelectedItem, BootSectorDiskFormat).Format
        If DiskFormat <> FloppyDiskFormat.FloppyUnknown Then
            RemoveDiskTypeUnknown()
            Dim BPB = BuildBPB(DiskFormat)
            PopulateBPB(BPB)
        Else
            DiskFormat = GetFloppyDiskFormat(GetBPB, True, False)
        End If
        UpdateBackColors(DiskFormat)
        _SuppressEvent = False
    End Sub

    Private Sub CboMediaDescriptor_DrawItem(sender As Object, e As DrawItemEventArgs) Handles CboMediaDescriptor.DrawItem
        Dim CB As ComboBox = sender

        e.DrawBackground()

        If e.Index >= 0 Then
            Dim Item As MediaDescriptorType = CB.Items(e.Index)

            Dim Brush As Brush
            Dim tBrush As Brush
            Dim nBrush As Brush

            If e.State And DrawItemState.Selected Then
                Brush = SystemBrushes.Highlight
                tBrush = SystemBrushes.HighlightText
                nBrush = SystemBrushes.HighlightText
            Else
                Brush = SystemBrushes.Window
                tBrush = SystemBrushes.WindowText
                nBrush = Brushes.Blue
            End If

            e.Graphics.FillRectangle(Brush, e.Bounds)
            Dim r1 As Rectangle = e.Bounds

            Dim Width = TextRenderer.MeasureText(Item.MediaDescriptor, e.Font).Width + 2
            r1.Width -= Width
            Dim r2 As Rectangle = e.Bounds
            r2.X = r2.Width - Width
            e.Graphics.DrawString(Item.MediaDescriptor, e.Font, nBrush, r2, StringFormat.GenericDefault)

            e.Graphics.DrawString(Item.Description, e.Font, tBrush, r1, StringFormat.GenericDefault)
        End If

        e.DrawFocusRectangle()
    End Sub

    Private Sub CboMediaDescriptor_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CboMediaDescriptor.KeyPress
        e.KeyChar = UCase(e.KeyChar)
        If Not ValidHexChars.Contains(e.KeyChar) And Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub CboMediaDescriptor_LostFocus(sender As Object, e As EventArgs) Handles CboMediaDescriptor.LostFocus
        Dim CB As ComboBox = sender
        Dim Result As UShort

        If Not UShort.TryParse(CB.Text, NumberStyles.HexNumber, CultureInfo.CurrentCulture, Result) Then
            ControlRevertValue(CB)
        Else
            _SuppressEvent = True
            If CB.Text.Length = 1 Then
                CB.Text = "0" & CB.Text
            End If
            If CB.Text <> CType(CB.Tag, FormControlData).LastValue Then
                UpdateTag(CB, True)
            End If
            _SuppressEvent = False
        End If
    End Sub
    Private Sub CboMediaDescriptor_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboMediaDescriptor.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim CB As ComboBox = sender
        _SuppressEvent = True
        If CB.Text <> CType(CB.Tag, FormControlData).LastValue Then
            UpdateTag(CB, True)
        End If
        _SuppressEvent = False
    End Sub

    Private Sub CboOEMName_TextChanged(sender As Object, e As EventArgs) Handles CboOEMName.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim OEMName() As Byte

        If CboOEMName.SelectedIndex = -1 Then
            Dim OEMNameString As String = Strings.Left(CboOEMName.Text, 8).PadRight(8)
            OEMName = DiskImage.UnicodeToCodePage437(OEMNameString)
        Else
            OEMName = CType(CboOEMName.SelectedItem, OEMNameData).Name
        End If

        HexOEMName.SetHex(OEMName)
    End Sub

    Private Sub ComboBox_DrawItem(sender As Object, e As DrawItemEventArgs) Handles CboBytesPerSector.DrawItem, CboSectorsPerCluster.DrawItem
        Dim CB As ComboBox = sender

        e.DrawBackground()

        If e.Index >= 0 Then
            Dim Brush As Brush
            Dim tBrush As Brush

            If e.State And DrawItemState.Selected Then
                Brush = SystemBrushes.Highlight
                tBrush = SystemBrushes.HighlightText
            Else
                Brush = SystemBrushes.Window
                tBrush = SystemBrushes.WindowText
            End If

            e.Graphics.FillRectangle(Brush, e.Bounds)
            e.Graphics.DrawString(CB.Items(e.Index).ToString, e.Font, tBrush, e.Bounds, StringFormat.GenericDefault)
        End If

        e.DrawFocusRectangle()
    End Sub

    Private Sub ComboBoxNumeric_LostFocus(sender As Object, e As EventArgs) Handles CboBytesPerSector.LostFocus, CboSectorsPerCluster.LostFocus
        Dim CB As ComboBox = sender
        Dim Result As UInteger
        If Not UInt32.TryParse(CB.Text, Result) Then
            ControlRevertValue(CB)
        Else
            _SuppressEvent = True
            If CB.MaxLength = 3 And Result > 255 Then
                CB.Text = 255
            ElseIf CB.MaxLength = 5 And Result > 65535 Then
                CB.Text = 65535
            Else
                CB.Text = CUInt(CB.Text)
            End If
            If CB.Text <> CType(CB.Tag, FormControlData).LastValue Then
                UpdateTag(CB, True)
            End If
            _SuppressEvent = False
        End If
    End Sub

    Private Sub ComboBoxNumeric_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboBytesPerSector.SelectedIndexChanged, CboSectorsPerCluster.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim CB As ComboBox = sender
        _SuppressEvent = True
        If CB.Text <> CType(CB.Tag, FormControlData).LastValue Then
            UpdateTag(CB, True)
        End If
        _SuppressEvent = False
    End Sub

    Private Sub ComboBoxText_LostFocus(sender As Object, e As EventArgs) Handles CboOEMName.LostFocus
        UpdateTag(CType(sender, ComboBox), False)
    End Sub

    Private Sub HexExtendedBootSignature_TextChanged(sender As Object, e As EventArgs) Handles HexExtendedBootSignature.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        _SuppressEvent = True
        Dim HB As HexTextBox = sender
        If HB.Text <> CType(HB.Tag, FormControlData).LastValue Then
            UpdateTag(HB, False)
            SetExtendedState()
        End If
        _SuppressEvent = False
    End Sub

    Private Sub HexFileSystemType_TextChanged(sender As Object, e As EventArgs) Handles HexFileSystemType.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        _SuppressEvent = True
        RefreshFileSystemType()
        _SuppressEvent = False
    End Sub

    Private Sub HexJumpInstruction_TextChanged(sender As Object, e As EventArgs) Handles HexJumpInstruction.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim HB As HexTextBox = sender
        If HB.Text <> CType(HB.Tag, FormControlData).LastValue Then
            UpdateTag(HB, False)
        End If
    End Sub

    Private Sub HexOEMName_TextChanged(sender As Object, e As EventArgs) Handles HexOEMName.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        _SuppressEvent = True
        ControlSetValue(CboOEMName, DiskImage.CodePage437ToUnicode(HexOEMName.GetHex).TrimEnd, False)
        _SuppressEvent = False
    End Sub

    Private Sub HexTextBox_TextChanged(sender As Object, e As EventArgs) Handles HexVolumeSerialNumber.TextChanged, HexBootSectorSignature.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim HB As HexTextBox = sender
        If HB.Text <> CType(HB.Tag, FormControlData).LastValue Then
            UpdateTag(HB, False)
        End If
    End Sub

    Private Sub HexVolumeLabel_TextChanged(sender As Object, e As EventArgs) Handles HexVolumeLabel.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        _SuppressEvent = True
        RefreshVolumeLabel()
        _SuppressEvent = False
    End Sub

    Private Sub OEMNameForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        ActiveControl = BtnCancel
    End Sub

    Private Sub TextBoxNumeric_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CboBytesPerSector.KeyPress, CboSectorsPerCluster.KeyPress,
                                                                                    TxtReservedSectors.KeyPress, TxtNumberOfFATs.KeyPress,
                                                                                    TxtRootDirectoryEntries.KeyPress, TxtSectorCountSmall.KeyPress,
                                                                                    TxtSectorsPerFAT.KeyPress, TxtSectorsPerTrack.KeyPress,
                                                                                    TxtNumberOfHeads.KeyPress, TxtHiddenSectors.KeyPress,
                                                                                    TxtSectorCountLarge.KeyPress, TxtDriveNumber.KeyPress

        If Not Char.IsNumber(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextBoxNumeric_LostFocus(sender As Object, e As EventArgs) Handles TxtReservedSectors.LostFocus, TxtNumberOfFATs.LostFocus,
                                                                            TxtRootDirectoryEntries.LostFocus, TxtSectorCountSmall.LostFocus,
                                                                            TxtSectorsPerFAT.LostFocus, TxtSectorsPerTrack.LostFocus,
                                                                            TxtNumberOfHeads.LostFocus, TxtHiddenSectors.LostFocus,
                                                                            TxtSectorCountLarge.LostFocus, TxtDriveNumber.LostFocus
        Dim TB As TextBox = sender
        Dim Result As UInteger
        If Not UInt32.TryParse(TB.Text, Result) Then
            ControlRevertValue(TB)
        Else
            _SuppressEvent = True
            If TB.MaxLength = 3 And Result > 255 Then
                TB.Text = 255
            ElseIf TB.MaxLength = 5 And Result > 65535 Then
                TB.Text = 65535
            Else
                TB.Text = CUInt(TB.Text)
            End If
            If TB.Text <> CType(TB.Tag, FormControlData).LastValue Then
                UpdateTag(TB, True)
            End If
            _SuppressEvent = False
        End If
    End Sub

    Private Sub TextBoxText_LostFocus(sender As Object, e As EventArgs) Handles TxtVolumeLabel.LostFocus, TxtFileSystemType.LostFocus
        Dim TB = CType(sender, TextBox)
        TB.Text = TB.Text.TrimEnd
        UpdateTag(TB, False)
    End Sub

    Private Sub TxtFileSystemType_TextChanged(sender As Object, e As EventArgs)
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim FileSystemTypeString As String = Strings.Left(TxtFileSystemType.Text, 8).PadRight(8)
        Dim FileSystemType() As Byte = DiskImage.UnicodeToCodePage437(FileSystemTypeString)
        HexVolumeLabel.SetHex(FileSystemType)
    End Sub

    Private Sub TxtVolumeLabel_TextChanged(sender As Object, e As EventArgs) Handles TxtVolumeLabel.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim VolumeLabelString As String = Strings.Left(TxtVolumeLabel.Text, 11).PadRight(11)
        Dim VolumeLabel() As Byte = DiskImage.UnicodeToCodePage437(VolumeLabelString)
        HexVolumeLabel.SetHex(VolumeLabel)
    End Sub

#End Region
    Private Class BootSectorDiskFormat
        Public Sub New(Format As FloppyDiskFormat)
            _Format = Format
        End Sub

        Public Property Format As FloppyDiskFormat

        Public Overrides Function ToString() As String
            Return GetFloppyDiskFormatName(_Format, True)
        End Function
    End Class
End Class