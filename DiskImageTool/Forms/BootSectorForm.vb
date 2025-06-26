Imports System.Globalization
Imports DiskImageTool.DiskImage
Imports DiskImageTool.DiskImage.BootSector
Imports DiskImageTool.Hb.Windows.Forms

Public Class BootSectorForm
    Const ValidHexChars = "0123456789ABCDEF"
    Private ReadOnly _BootStrap As BootstrapDB
    Private ReadOnly _BootSector As BootSector
    Private ReadOnly _HasExtended As Boolean
    Private ReadOnly _HelpProvider1 As HelpProvider
    Private _SuppressEvent As Boolean = True

    Public Sub New(Data() As Byte, BootStrap As BootstrapDB)

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
        SetHelpString(My.Resources.BootSectorForm_Help_OEMName, LblOEMName, CboOEMName, HexOEMName)
        SetHelpString(My.Resources.BootSectorForm_Help_BytesPerSector, LblBytesPerSector, CboBytesPerSector)
        SetHelpString(My.Resources.BootSectorForm_Help_SectorsPerCluster, LblSectorsPerCluster, CboSectorsPerCluster)
        SetHelpString(My.Resources.BootSectorForm_Help_ReservedSectors, LblReservedSectors, TxtReservedSectors)
        SetHelpString(My.Resources.BootSectorForm_Help_NumberOfFATs, LblNumberOfFATS, TxtNumberOfFATs)
        SetHelpString(My.Resources.BootSectorForm_Help_RootDirectoryEntries, LblRootDirectoryEntries, TxtRootDirectoryEntries)
        SetHelpString(My.Resources.BootSectorForm_Help_SectorCountSmall, LblSectorCountSmall, TxtSectorCountSmall)
        SetHelpString(My.Resources.BootSectorForm_Help_MediaDescriptor, TxtMediaDescriptor, CboMediaDescriptor)
        SetHelpString(My.Resources.BootSectorForm_Help_SectorsPerFAT, LblSectorsPerFAT, TxtSectorsPerFAT)
        SetHelpString(My.Resources.BootSectorForm_Help_SectorsPerTrack, LblSectorsPerTrack, TxtSectorsPerTrack)
        SetHelpString(My.Resources.BootSectorForm_Help_NumberOfHeads, LblNumberOfHeads, TxtNumberOfHeads)
        SetHelpString(My.Resources.BootSectorForm_Help_HiddenSectors, LblHiddenSectors, TxtHiddenSectors)
        SetHelpString(My.Resources.BootSectorForm_Help_SectorCountLarge, LblSectorCountLarge, TxtSectorCountLarge)
        SetHelpString(My.Resources.BootSectorForm_Help_DriveNumber, LblDriveNumber, TxtDriveNumber)
        SetHelpString(My.Resources.BootSectorForm_Help_ExtendedBootSignature, lblExtendedBootSignature, HexExtendedBootSignature)
        SetHelpString(My.Resources.BootSectorForm_Help_VolumeSerialNumber, LblVolumeSerialNumber, HexVolumeSerialNumber)
        SetHelpString(My.Resources.BootSectorForm_Help_VolumeLabel, LblVolumeLabel, TxtVolumeLabel, HexVolumeLabel)
        SetHelpString(My.Resources.BootSectorForm_Help_FileSystemType, LblFileSystemType, TxtFileSystemType, HexFileSystemType)
        SetHelpString(My.Resources.BootSectorForm_Help_DiskType, LblDiskType, CboDiskType)
        SetHelpString(My.Resources.BootSectorForm_Help_VolumeSerialNumberButton, BtnVolumeSerialNumber)
        SetHelpString(My.Resources.BootSectorForm_Help_JumpInstruction, LblJumpInstruction, HexJumpInstruction)
        SetHelpString(My.Resources.BootSectorForm_Help_BootSectorSignature, LblBootSectorSignature, HexBootSectorSignature)
        SetHelpString(My.Resources.BootSectorForm_Help_AdditionalData, HexBox1)
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
        CboDiskType.Items.Add(New BootSectorDiskFormat(FloppyDiskFormat.Floppy2HD))
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
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FE", GetFloppyDiskFormatName(FloppyDiskFormat.Floppy160)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FC", GetFloppyDiskFormatName(FloppyDiskFormat.Floppy180)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FF", GetFloppyDiskFormatName(FloppyDiskFormat.Floppy320)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FD", GetFloppyDiskFormatName(FloppyDiskFormat.Floppy360)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F9", GetFloppyDiskFormatName(FloppyDiskFormat.Floppy720)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F9", GetFloppyDiskFormatName(FloppyDiskFormat.Floppy1200)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", GetFloppyDiskFormatName(FloppyDiskFormat.Floppy1440)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", GetFloppyDiskFormatName(FloppyDiskFormat.Floppy2880)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", GetFloppyDiskFormatName(FloppyDiskFormat.FloppyDMF1024)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F9", GetFloppyDiskFormatName(FloppyDiskFormat.FloppyXDF525)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("F0", GetFloppyDiskFormatName(FloppyDiskFormat.FloppyXDF35)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("ED", GetFloppyDiskFormatName(FloppyDiskFormat.FloppyTandy2000)))
        CboMediaDescriptor.Items.Add(New MediaDescriptorType("FE", GetFloppyDiskFormatName(FloppyDiskFormat.Floppy2HD)))
    End Sub

    Private Sub PopulateOEMName()
        CboOEMName.MaxLength = _BootSector.GetOEMNameSize
        HexOEMName.MaskLength = _BootSector.GetOEMNameSize

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