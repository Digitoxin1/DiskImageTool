Imports System.ComponentModel
Imports System.IO
Imports DiskImageTool.DiskImage
Imports DiskImageTool.DiskImage.BootSector

Public Class BootSectorForm
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _OEMNameDictionary As Dictionary(Of UInteger, BootstrapLookup)
    Private ReadOnly _HasExtended As Boolean
    Private _SuppressEvent As Boolean = True

    Public Sub New(Disk As DiskImage.Disk, OEMNameDictionary As Dictionary(Of UInteger, BootstrapLookup))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk
        _OEMNameDictionary = OEMNameDictionary

        Dim BootStrapStart = _Disk.BootSector.GetBootStrapOffset
        _HasExtended = BootStrapStart >= BootSectorOffsets.FileSystemType + BootSectorSizes.FileSystemType
    End Sub

    Private Sub SetValue(Control As TextBox, Value As String)
        Control.Text = Value
        Control.Tag = Value
    End Sub

    Private Sub SetValue(Control As ComboBox, Value As String)
        Control.Text = Value
        Control.Tag = Value
    End Sub

    Private Sub UpdateTag(Control As TextBox)
        Control.Tag = Control.Text
    End Sub

    Private Sub UpdateTag(Control As ComboBox)
        Control.Tag = Control.Text
    End Sub

    Private Sub RevertValue(Control As TextBox)
        Control.Text = Control.Tag
    End Sub

    Private Sub RevertValue(Control As ComboBox)
        Control.Text = Control.Tag
    End Sub

    Private Sub PopulateBootRecord(BootSector As BootSector)
        SetValue(CboBytesPerSector, BootSector.BytesPerSector)
        SetValue(CboSectorsPerCluster, BootSector.SectorsPerCluster)
        SetValue(TxtReservedSectors, BootSector.ReservedSectorCount)
        SetValue(TxtNumberOfFATs, BootSector.NumberOfFATs)
        SetValue(TxtRootDirectoryEntries, BootSector.RootEntryCount)
        SetValue(TxtSectorCountSmall, BootSector.SectorCountSmall)
        HexMediaDescriptor.SetHex({BootSector.MediaDescriptor})
        HexMediaDescriptor.Tag = BootSector.MediaDescriptor
        SetValue(TxtSectorsPerFAT, BootSector.SectorsPerFAT)
        SetValue(TxtSectorsPerTrack, BootSector.SectorsPerTrack)
        SetValue(TxtNumberOfHeads, BootSector.NumberOfHeads)
        SetValue(TxtHiddenSectors, BootSector.HiddenSectors)
    End Sub

    Private Sub PopulateExtended(BootSector As BootSector)
        SetValue(TxtSectorCountLarge, BootSector.SectorCountLarge)
        SetValue(TxtDriveNumber, BootSector.DriveNumber)
        HexExtendedBootSignature.SetHex({BootSector.ExtendedBootSignature})
        HexVolumeSerialNumber.SetHex(BootSector.VolumeSerialNumber.ToString("X8"))
        SetValue(TxtVolumeLabel, BootSector.GetVolumeLabelString.TrimEnd)
        HexVolumeLabel.SetHex(BootSector.VolumeLabel)
        SetValue(TxtFileSystemType, BootSector.GetFileSystemTypeString.TrimEnd)
        HexFileSystemType.SetHex(BootSector.FileSystemType)
    End Sub

    Private Sub PopulateValues()
        PopulateOEMName()
        PopulateBytesPerSector()
        PopulateSectorsPerCluster()
        PopulateBootRecord(_Disk.BootSector)

        If _HasExtended Then
            GroupBoxExtended.Visible = True
            PopulateExtended(_Disk.BootSector)
        Else
            GroupBoxExtended.Visible = False
        End If
    End Sub

    Private Sub PopulateDiskTypes()
        CboDiskType.Items.Clear()
        CboDiskType.Items.Add(New BootSectorDiskType("160K Floppy", 163840))
        CboDiskType.Items.Add(New BootSectorDiskType("180K Floppy", 184320))
        CboDiskType.Items.Add(New BootSectorDiskType("320K Floppy", 327680))
        CboDiskType.Items.Add(New BootSectorDiskType("360K Floppy", 368640))
        CboDiskType.Items.Add(New BootSectorDiskType("720K Floppy", 737280))
        CboDiskType.Items.Add(New BootSectorDiskType("1.2M Floppy", 1228800))
        CboDiskType.Items.Add(New BootSectorDiskType("1.44M Floppy", 1474560))
        CboDiskType.Items.Add(New BootSectorDiskType("Custom", 0))

        CboDiskType.SelectedIndex = CboDiskType.Items.Count - 1
    End Sub

    Private Sub PopulateBytesPerSector()
        CboBytesPerSector.Items.Clear()
        For Each Value In BootSector.ValidBytesPerSector
            CboBytesPerSector.Items.Add(Value)
        Next
    End Sub

    Private Sub PopulateSectorsPerCluster()
        CboSectorsPerCluster.Items.Clear()
        For Each Value In BootSector.ValidSectorsPerCluster
            CboSectorsPerCluster.Items.Add(Value)
        Next
    End Sub

    Private Sub PopulateOEMName()
        Dim BootstrapChecksum = Crc32.ComputeChecksum(_Disk.BootSector.BootStrapCode)
        Dim OEMName As New KnownOEMName With {
            .Name = _Disk.BootSector.OEMName
        }

        If _OEMNameDictionary.ContainsKey(BootstrapChecksum) Then
            Dim BootstrapType = _OEMNameDictionary.Item(BootstrapChecksum)
            For Each KnownOEMName In BootstrapType.KnownOEMNames
                If KnownOEMName.Name.Length > 0 Then
                    Dim IsMatch = ByteArrayCompare(KnownOEMName.Name, OEMName.Name)
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

        SetValue(CboOEMName, _Disk.BootSector.GetOEMNameString)
        HexOEMName.SetHex(_Disk.BootSector.OEMName)
    End Sub

    Private Sub SetCurrentDiskType()
        Dim SelectedItem As BootSectorDiskType = Nothing

        For Each DiskType As BootSectorDiskType In CboDiskType.Items
            If DiskType.Size > 0 Then
                Dim BootSector = BuildBootSectorFromFileSize(DiskType.Size)
                If BootSector.MediaDescriptor = HexMediaDescriptor.GetHex(0) _
                    And BootSector.NumberOfHeads = TxtNumberOfHeads.Text _
                    And BootSector.RootEntryCount = TxtRootDirectoryEntries.Text _
                    And BootSector.SectorCountSmall = TxtSectorCountSmall.Text _
                    And BootSector.SectorsPerCluster = CboSectorsPerCluster.Text _
                    And BootSector.SectorsPerFAT = TxtSectorsPerFAT.Text _
                    And BootSector.SectorsPerTrack = TxtSectorsPerTrack.Text _
                    And BootSector.BytesPerSector = CboBytesPerSector.Text _
                    And BootSector.NumberOfFATs = TxtNumberOfFATs.Text _
                    And BootSector.ReservedSectorCount = TxtReservedSectors.Text _
                    And BootSector.HiddenSectors = TxtHiddenSectors.Text Then

                    SelectedItem = DiskType
                    Exit For
                End If
            End If
        Next

        If SelectedItem Is Nothing Then
            CboDiskType.SelectedIndex = CboDiskType.Items.Count - 1
        Else
            CboDiskType.SelectedItem = SelectedItem
        End If
    End Sub

    Private Sub UpdateBootSector()
        _Disk.Data.BatchEditMode = True

        Dim UShortValue As UShort
        Dim UintegerValue As UInteger
        Dim ByteValue As Byte

        Dim OEMNameString As String = Strings.Left(CboOEMName.Text, 8).PadRight(8)
        Dim OEMName = DiskImage.UnicodeToCodePage437(OEMNameString)
        _Disk.BootSector.OEMName = OEMName

        If UShort.TryParse(CboBytesPerSector.Text, UShortValue) Then
            _Disk.BootSector.BytesPerSector = UShortValue
        End If

        If Byte.TryParse(CboSectorsPerCluster.Text, ByteValue) Then
            _Disk.BootSector.SectorsPerCluster = ByteValue
        End If

        If UShort.TryParse(TxtReservedSectors.Text, UShortValue) Then
            _Disk.BootSector.ReservedSectorCount = UShortValue
        End If

        If Byte.TryParse(TxtNumberOfFATs.Text, ByteValue) Then
            _Disk.BootSector.NumberOfFATs = ByteValue
        End If

        If UShort.TryParse(TxtRootDirectoryEntries.Text, UShortValue) Then
            _Disk.BootSector.RootEntryCount = UShortValue
        End If

        If UShort.TryParse(TxtSectorCountSmall.Text, UShortValue) Then
            _Disk.BootSector.SectorCountSmall = UShortValue
        End If

        _Disk.BootSector.MediaDescriptor = HexMediaDescriptor.GetHex(0)

        If UShort.TryParse(TxtSectorsPerFAT.Text, UShortValue) Then
            _Disk.BootSector.SectorsPerFAT = UShortValue
        End If

        If UShort.TryParse(TxtSectorsPerTrack.Text, UShortValue) Then
            _Disk.BootSector.SectorsPerTrack = UShortValue
        End If

        If UShort.TryParse(TxtNumberOfHeads.Text, UShortValue) Then
            _Disk.BootSector.NumberOfHeads = UShortValue
        End If

        If UShort.TryParse(TxtHiddenSectors.Text, UShortValue) Then
            _Disk.BootSector.HiddenSectors = UShortValue
        End If

        If _HasExtended Then
            If UInteger.TryParse(TxtSectorCountLarge.Text, UintegerValue) Then
                _Disk.BootSector.SectorCountLarge = UintegerValue
            End If

            _Disk.BootSector.ExtendedBootSignature = HexExtendedBootSignature.GetHex(0)

            If Byte.TryParse(TxtDriveNumber.Text, ByteValue) Then
                _Disk.BootSector.DriveNumber = ByteValue
            End If

            Dim VolumeSerial = HexVolumeSerialNumber.GetHex
            Array.Reverse(VolumeSerial)
            _Disk.BootSector.VolumeSerialNumber = BitConverter.ToUInt32(VolumeSerial, 0)

            Dim VolumeLabelString As String = Strings.Left(TxtVolumeLabel.Text, 11).PadRight(11)
            Dim VolumeLabel = DiskImage.UnicodeToCodePage437(VolumeLabelString)
            _Disk.BootSector.VolumeLabel = VolumeLabel

            Dim FileSystemTypeString As String = Strings.Left(TxtFileSystemType.Text, 8).PadRight(8)
            Dim FileSystemType = DiskImage.UnicodeToCodePage437(FileSystemTypeString)
            _Disk.BootSector.FileSystemType = FileSystemType
        End If

        _Disk.Data.BatchEditMode = False
    End Sub

#Region "Events"

    Private Sub CboOEMName_TextChanged(sender As Object, e As EventArgs) Handles CboOEMName.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim OEMName() As Byte

        If CboOEMName.SelectedIndex = -1 Then
            Dim OEMNameString As String = Strings.Left(CboOEMName.Text, 8).PadRight(8)
            OEMName = DiskImage.UnicodeToCodePage437(OEMNameString)
        Else
            OEMName = CType(CboOEMName.SelectedItem, KnownOEMName).Name
        End If

        HexOEMName.SetHex(OEMName)
    End Sub

    Private Sub MskOEMNameHex_TextChanged(sender As Object, e As EventArgs) Handles HexOEMName.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        _SuppressEvent = True
        CboOEMName.Text = DiskImage.CodePage437ToUnicode(HexOEMName.GetHex)
        _SuppressEvent = False
    End Sub

    Private Sub OEMNameForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        _SuppressEvent = True

        PopulateDiskTypes()
        PopulateValues()
        SetCurrentDiskType()

        _SuppressEvent = False

        ActiveControl = BtnCancel
    End Sub

    Private Sub TxtVolumeLabel_TextChanged(sender As Object, e As EventArgs) Handles TxtVolumeLabel.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim VolumeLabelString As String = Strings.Left(TxtVolumeLabel.Text, 11).PadRight(11)
        Dim VolumeLabel() As Byte = DiskImage.UnicodeToCodePage437(VolumeLabelString)
        HexVolumeLabel.SetHex(VolumeLabel)
    End Sub

    Private Sub HexVolumeLabel_TextChanged(sender As Object, e As EventArgs) Handles HexVolumeLabel.TextChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        _SuppressEvent = True
        TxtVolumeLabel.Text = DiskImage.CodePage437ToUnicode(HexVolumeLabel.GetHex)
        _SuppressEvent = False
    End Sub

    Private Sub TxtFileSystemType_TextChanged(sender As Object, e As EventArgs)
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim FileSystemTypeString As String = Strings.Left(TxtFileSystemType.Text, 8).PadRight(8)
        Dim FileSystemType() As Byte = DiskImage.UnicodeToCodePage437(FileSystemTypeString)
        HexVolumeLabel.SetHex(FileSystemType)
    End Sub

    Private Sub HexFileSystemType_TextChanged(sender As Object, e As EventArgs)
        If _SuppressEvent Then
            Exit Sub
        End If

        _SuppressEvent = True
        TxtFileSystemType.Text = DiskImage.CodePage437ToUnicode(HexFileSystemType.GetHex)
        _SuppressEvent = False
    End Sub

    Private Sub TextBox_KeyPress(sender As Object, e As KeyPressEventArgs) Handles CboBytesPerSector.KeyPress, CboSectorsPerCluster.KeyPress,
                                                                                    TxtReservedSectors.KeyPress, TxtNumberOfFATs.KeyPress,
                                                                                    TxtRootDirectoryEntries.KeyPress, TxtSectorCountSmall.KeyPress,
                                                                                    TxtSectorsPerFAT.KeyPress, TxtSectorsPerTrack.KeyPress,
                                                                                    TxtNumberOfHeads.KeyPress, TxtHiddenSectors.KeyPress,
                                                                                    TxtSectorCountLarge.KeyPress, TxtDriveNumber.KeyPress


        If Not Char.IsNumber(e.KeyChar) AndAlso Not Char.IsControl(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TextBoxMain_LostFocus(sender As Object, e As EventArgs) Handles TxtReservedSectors.LostFocus, TxtNumberOfFATs.LostFocus,
                                                                            TxtRootDirectoryEntries.LostFocus, TxtSectorCountSmall.LostFocus,
                                                                            TxtSectorsPerFAT.LostFocus, TxtSectorsPerTrack.LostFocus,
                                                                            TxtNumberOfHeads.LostFocus, TxtHiddenSectors.LostFocus,
                                                                            TxtSectorCountLarge.LostFocus, TxtDriveNumber.LostFocus
        Dim TB As TextBox = sender
        Dim Result As UInteger
        If Not UInt32.TryParse(TB.Text, Result) Then
            RevertValue(TB)
        Else
            _SuppressEvent = True
            If TB.MaxLength = 3 And Result > 255 Then
                TB.Text = 255
            ElseIf TB.MaxLength = 5 And Result > 65535 Then
                TB.Text = 65535
            End If
            If TB.Text <> TB.Tag Then
                UpdateTag(TB)
                SetCurrentDiskType()
            End If
            _SuppressEvent = False
        End If
    End Sub

    Private Sub ComboBox_LostFocus(sender As Object, e As EventArgs) Handles CboBytesPerSector.LostFocus, CboSectorsPerCluster.LostFocus
        Dim CB As ComboBox = sender
        Dim Result As UInteger
        If Not UInt32.TryParse(CB.Text, Result) Then
            RevertValue(CB)
        Else
            _SuppressEvent = True
            If CB.MaxLength = 3 And Result > 255 Then
                CB.Text = 255
            ElseIf CB.MaxLength = 5 And Result > 65535 Then
                CB.Text = 65535
            End If
            If CB.Text <> CB.Tag Then
                UpdateTag(CB)
                SetCurrentDiskType()
            End If
            _SuppressEvent = False
        End If
    End Sub

    Private Sub CboDiskType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CboDiskType.SelectedIndexChanged
        If _SuppressEvent Then
            Exit Sub
        End If

        Dim DiskType As BootSectorDiskType = CboDiskType.SelectedItem
        If DiskType.Size > 0 Then
            Dim BootSector = BuildBootSectorFromFileSize(DiskType.Size)
            PopulateBootRecord(BootSector)
        End If
    End Sub

    Private Sub HexMediaDescriptor_LostFocus(sender As Object, e As EventArgs) Handles HexMediaDescriptor.LostFocus
        If HexMediaDescriptor.GetHex(0) <> HexMediaDescriptor.Tag Then
            HexMediaDescriptor.Tag = HexMediaDescriptor.GetHex(0)
            _SuppressEvent = True
            SetCurrentDiskType()
            _SuppressEvent = False
        End If
    End Sub

#End Region
    Private Class BootSectorDiskType
        Public Sub New(Name As String, Size As Integer)
            _Name = Name
            _Size = Size
        End Sub

        Public Property Name As String
        Public Property Size As Integer

        Public Overrides Function ToString() As String
            Return _Name
        End Function
    End Class

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs) Handles BtnUpdate.Click
        UpdateBootSector
    End Sub

End Class