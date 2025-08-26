Imports System.Xml
Imports DiskImageTool.DiskImage

Public Class ImageCreationForm
    Private ReadOnly _NameSpace As String = New StubClass().GetType.Namespace
    Private _SelectedFormat As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown
    Private _Data As Byte() = Nothing
    Private _BootSectorTypes As Dictionary(Of String, List(Of BootSectorType))

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        GroupBoxStandard.Height = PanelFormats.Height
        GroupBoxSpecial.Height = PanelFormats.Height

        _BootSectorTypes = New Dictionary(Of String, List(Of BootSectorType))

        ParseXML()
        InitializeEvents()
        RefreshCombo()
    End Sub

    Public ReadOnly Property DiskFormat As FloppyDiskFormat
        Get
            Return _SelectedFormat
        End Get
    End Property

    Public ReadOnly Property Data As Byte()
        Get
            Return _Data
        End Get
    End Property

    Public ReadOnly Property ImportFiles As Boolean
        Get
            Return CheckImportFiles.Checked
        End Get
    End Property

    Private Sub AddTandy2000RootEntries(Data() As Byte, Params As FloppyDiskParams)
        Dim Buffer = FillArray(32, &HF6)
        Buffer(0) = &H0

        Dim Offset As UInteger = (Params.ReservedSectorCount + (Params.SectorsPerFAT * Params.NumberOfFATs)) * Params.BytesPerSector
        For Counter = 0 To Params.RootEntryCount - 1
            Buffer.CopyTo(Data, Offset)
            Offset += Buffer.Length
        Next
    End Sub

    Private Sub AddRootEntries(Data() As Byte, Params As FloppyDiskParams)
        Dim Buffer = New Byte(31) {}
        Buffer(0) = &HE5

        Dim Offset As UInteger = (Params.ReservedSectorCount + (Params.SectorsPerFAT * Params.NumberOfFATs)) * Params.BytesPerSector
        For Counter = 0 To Params.RootEntryCount - 1
            Buffer.CopyTo(Data, Offset)
            If Counter < 2 Then
                Data(Offset + 11) = &H6
            End If
            Offset += Buffer.Length
        Next
    End Sub

    Private Sub CreateFloppy()
        Dim Size = GetFloppyDiskSize(_SelectedFormat)
        Dim BootSectorType As BootSectorType = ComboBootSector.SelectedItem
        Dim Buffer() As Byte

        If BootSectorType IsNot Nothing And Size > 0 Then
            If BootSectorType.Empty Then
                Buffer = New Byte(511) {}
            Else
                Buffer = GetBootSector(BootSectorType.Id)
            End If
            If Buffer IsNot Nothing Then
                Dim BootSector = New BootSector(Buffer)
                Dim FATMediaDescriptor As Byte

                If BootSectorType.OEMName <> "" Then
                    BootSector.OEMName = HexStringToBytes(BootSectorType.OEMName)
                End If

                Dim Params = GetFloppyDiskParams(_SelectedFormat)

                If BootSectorType.BPB Then
                    SetBPB(BootSector.BPB, Params)
                End If

                If BootSectorType.VolumeSerialNumber Then
                    BootSector.VolumeSerialNumber = DiskImage.BootSector.GenerateVolumeSerialNumber(Now)
                End If

                Buffer = New Byte(Size - 1) {}
                BootSector.Data.CopyTo(Buffer, 0)

                If BootSectorType.FATMediaDescriptor = "" Then
                    FATMediaDescriptor = Params.MediaDescriptor
                Else
                    FATMediaDescriptor = Convert.ToByte(BootSectorType.FATMediaDescriptor, 16)
                End If

                SetFAT(Buffer, Params, FATMediaDescriptor)

                If BootSectorType.RootFill Then
                    If _SelectedFormat = FloppyDiskFormat.FloppyTandy2000 Then
                        AddTandy2000RootEntries(Buffer, Params)
                    Else
                        AddRootEntries(Buffer, Params)
                    End If
                End If

                Dim Offset As UInteger = (Params.ReservedSectorCount + (Params.SectorsPerFAT * Params.NumberOfFATs)) * Params.BytesPerSector + Params.RootEntryCount * 32
                For Counter As UInteger = Offset To Buffer.Length - 1
                    Buffer(Counter) = &HF6
                Next

                _Data = Buffer
            End If
        End If
    End Sub

    Private Function GetBootSector(Id As String) As Byte()
        Dim XMLDoc As New XmlDocument()
        XMLDoc.LoadXml(GetResource("bootsector.xml"))

        Dim BootSectorData As XmlElement = XMLDoc.SelectSingleNode("/root/bootsectors/bootsector[@id='" & Id & "']")
        If BootSectorData IsNot Nothing Then
            Dim HexString = BootSectorData.InnerText
            HexString = Replace(HexString, " ", "")
            HexString = Replace(HexString, Chr(13), "")
            HexString = Replace(HexString, Chr(10), "")
            Return HexStringToBytes(HexString)
        Else
            Return Nothing
        End If
    End Function

    Private Function GetResource(Name As String) As String
        Dim Value As String

        Dim Assembly As Reflection.[Assembly] = Reflection.[Assembly].GetExecutingAssembly()
        Dim Stream = Assembly.GetManifestResourceStream(_NameSpace & "." & Name)
        If Stream Is Nothing Then
            Throw New Exception("Unable to load resource " & Name)
        Else
            Dim TextStreamReader = New IO.StreamReader(Stream)
            Value = TextStreamReader.ReadToEnd()
            TextStreamReader.Close()
        End If

        Return Value
    End Function


    Private Sub InitializeEvents()
        For Each Control In PanelFormats.Controls
            If TypeOf Control Is RadioButton Then
                Dim Radiobutton = DirectCast(Control, RadioButton)
                AddHandler Radiobutton.CheckedChanged, AddressOf Format_CheckChanged
                If Radiobutton.Checked Then
                    _SelectedFormat = Radiobutton.Tag
                End If
            End If
        Next
    End Sub

    Private Sub ParseXML()
        Dim XMLDoc As New XmlDocument()
        XMLDoc.LoadXml(GetResource("bootsector.xml"))

        For Each FormatNode As XmlElement In XMLDoc.SelectNodes("/root/formats/format")
            Dim DiskFormat As FloppyDiskFormat = FormatNode.GetAttribute("id")
            Dim BootSectorList = New List(Of BootSectorType)
            For Each BootSectorNode As XmlElement In FormatNode.SelectNodes("bootsector")
                Dim BootSectorId = BootSectorNode.GetAttribute("id")
                Dim OEMName = ""
                Dim FATMediaDescriptor = ""
                If BootSectorNode.HasAttribute("oemName") Then
                    OEMName = BootSectorNode.GetAttribute("oemName")
                End If
                If BootSectorNode.HasAttribute("fatMediaDescriptor") Then
                    FATMediaDescriptor = BootSectorNode.GetAttribute("fatMediaDescriptor")
                End If
                Dim BootSectorData As XmlElement = XMLDoc.SelectSingleNode("/root/bootsectors/bootsector[@id='" & BootSectorId & "']")
                If BootSectorData IsNot Nothing Then
                    Dim BootSectorType = New BootSectorType With {
                        .Id = BootSectorId,
                        .Name = BootSectorData.GetAttribute("name"),
                        .VolumeSerialNumber = (BootSectorData.HasAttribute("volumeSerialNumber") AndAlso BootSectorData.GetAttribute("volumeSerialNumber") = "1"),
                        .BPB = (BootSectorData.HasAttribute("BPB") AndAlso BootSectorData.GetAttribute("BPB") = "1"),
                        .OEMName = OEMName,
                        .RootFill = (BootSectorData.HasAttribute("rootFill") AndAlso BootSectorData.GetAttribute("rootFill") = "1"),
                        .Empty = (BootSectorData.HasAttribute("empty") AndAlso BootSectorData.GetAttribute("empty") = "1"),
                        .FATMediaDescriptor = FATMediaDescriptor
                    }
                    BootSectorList.Add(BootSectorType)
                End If
            Next
            _BootSectorTypes.Add(DiskFormat, BootSectorList)
        Next
    End Sub

    Private Sub RefreshCombo()
        Dim SelectedId As String = ""
        Dim SelectdBootSectorType As BootSectorType = ComboBootSector.SelectedItem
        If SelectdBootSectorType IsNot Nothing Then
            SelectedId = SelectdBootSectorType.Id
        End If

        ComboBootSector.Items.Clear()

        If _BootSectorTypes.ContainsKey(_SelectedFormat) Then
            Dim BootSectorList = _BootSectorTypes.Item(_SelectedFormat)
            For Each BootSectorType In BootSectorList
                ComboBootSector.Items.Add(BootSectorType)
                If BootSectorType.Id = SelectedId Then
                    ComboBootSector.SelectedIndex = ComboBootSector.Items.Count - 1
                End If
            Next
            If ComboBootSector.Items.Count > 0 Then
                If ComboBootSector.SelectedIndex = -1 Then
                    ComboBootSector.SelectedIndex = 0
                End If
            End If
        End If

        BtnOK.Enabled = ComboBootSector.Items.Count > 0

    End Sub

    Private Sub SetBPB(BPB As BiosParameterBlock, Params As FloppyDiskParams)
        With BPB
            .BytesPerSector = Params.BytesPerSector
            .MediaDescriptor = Params.MediaDescriptor
            .NumberOfFATs = Params.NumberOfFATs
            .NumberOfHeads = Params.NumberOfHeads
            .ReservedSectorCount = Params.ReservedSectorCount
            .RootEntryCount = Params.RootEntryCount
            .SectorCountSmall = Params.SectorCountSmall
            .SectorsPerCluster = Params.SectorsPerCluster
            .SectorsPerFAT = Params.SectorsPerFAT
            .SectorsPerTrack = Params.SectorsPerTrack
        End With
    End Sub

    Private Sub SetFAT(Data() As Byte, Params As FloppyDiskParams, MediaDescriptor As Byte)

        Dim Offset As UInteger = Params.ReservedSectorCount * Params.BytesPerSector
        For Counter = 0 To Params.NumberOfFATs - 1
            Data(Offset) = MediaDescriptor
            Data(Offset + 1) = &HFF
            Data(Offset + 2) = &HFF
            Offset += Params.SectorsPerFAT * Params.BytesPerSector
        Next
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        CreateFloppy()
    End Sub

    Private Sub ComboBootSector_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBootSector.SelectedIndexChanged
        BtnOK.Enabled = True
    End Sub

    Private Sub Format_CheckChanged(sender As Object, e As EventArgs)
        Dim Radiobutton = DirectCast(sender, RadioButton)
        If Radiobutton.Checked Then
            _SelectedFormat = Radiobutton.Tag
            RefreshCombo()
        End If
    End Sub

    Private Class BootSectorType
        Public Sub New()
        End Sub

        Public Property Id As String
        Public Property Name As String
        Public Property VolumeSerialNumber As Boolean
        Public Property BPB As Boolean
        Public Property OEMName As String
        Public Property RootFill As Boolean
        Public Property Empty As Boolean
        Public Property FATMediaDescriptor As String


        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

End Class