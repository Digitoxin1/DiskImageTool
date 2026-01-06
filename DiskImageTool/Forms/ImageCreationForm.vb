Imports System.Xml
Imports DiskImageTool.DiskImage

Public Class ImageCreationForm
    Private Const USER_DB_FILE_NAME As String = "BootSector.xml"
    Private Const XPATH_BOOT_SECTORS As String = "/root/bootsectors/bootsector"
    Private Const XPATH_FORMATS As String = "/root/formats/format"
    Private ReadOnly _BootSectorTypes As Dictionary(Of String, List(Of BootSectorType))
    Private ReadOnly _NameSpace As String = New StubClass().GetType.Namespace
    Private ReadOnly _UserDB As List(Of BootSectorDefinition)
    Private _Data As Byte() = Nothing
    Private _SelectedFormat As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        GroupBoxStandard.Height = PanelFormats.Height
        GroupBoxSpecial.Height = PanelFormats.Height

        _BootSectorTypes = New Dictionary(Of String, List(Of BootSectorType))
        _UserDB = New List(Of BootSectorDefinition)

        ParseXML()
        ParseUserXML()
        InitializeEvents()
        RefreshCombo()
    End Sub

    Private Sub ParseUserXML()
        Dim UserXMLPath As String = IO.Path.Combine(GetAppPath, USER_DB_FILE_NAME)

        If Not IO.File.Exists(UserXMLPath) Then
            Return
        End If

        Dim XMLDoc As New XmlDocument()
        XMLDoc.Load(UserXMLPath)

        For Each BootSectorNode As XmlElement In XMLDoc.SelectNodes(XPATH_BOOT_SECTORS)
            Dim BootSector = ParseBootSectorNode(BootSectorNode)
            If BootSector IsNot Nothing Then
                _UserDB.Add(BootSector)
            End If
        Next
    End Sub

    Public ReadOnly Property Data As Byte()
        Get
            Return _Data
        End Get
    End Property

    Public ReadOnly Property DiskFormat As FloppyDiskFormat
        Get
            Return _SelectedFormat
        End Get
    End Property

    Public Shared Function Display() As (Data As Byte(), DiskFormat As FloppyDiskFormat)
        Using dlg As New ImageCreationForm()
            dlg.ShowDialog(App.CurrentFormInstance)
            Return (dlg.Data, dlg.DiskFormat)
        End Using
    End Function

    Private Shared Sub AddRootEntries(Data() As Byte, Params As FloppyDiskBPBParams)
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

    Private Shared Sub AddTandy2000RootEntries(Data() As Byte, Params As FloppyDiskBPBParams)
        Dim Buffer = FillArray(32, &HF6)
        Buffer(0) = &H0

        Dim Offset As UInteger = (Params.ReservedSectorCount + (Params.SectorsPerFAT * Params.NumberOfFATs)) * Params.BytesPerSector
        For Counter = 0 To Params.RootEntryCount - 1
            Buffer.CopyTo(Data, Offset)
            Offset += Buffer.Length
        Next
    End Sub

    Private Shared Function CreateFloppy(Format As FloppyDiskFormat, BootSectorType As BootSectorType) As Byte()
        Dim Params = FloppyDiskFormatGetParams(Format)
        Dim BPBParams = Params.BPBParams
        Dim Size = BPBParams.SizeInBytes

        If BootSectorType Is Nothing OrElse Size = 0 Then
            Return Nothing
        End If

        Dim BootSector As New BootSector(BootSectorType.BootSector.Data)
        Dim FATMediaDescriptor As Byte

        If BootSectorType.OEMName <> "" Then
            BootSector.OEMName = HexStringToBytes(BootSectorType.OEMName)
        End If

        If BootSectorType.BootSector.BPB Then
            SetBPB(BootSector.BPB, BPBParams)
        End If

        If BootSectorType.BootSector.VolumeSerialNumber Then
            BootSector.VolumeSerialNumber = DiskImage.BootSector.GenerateVolumeSerialNumber(Now)
        End If

        Dim Buffer = New Byte(Size - 1) {}
        BootSector.Data.CopyTo(Buffer, 0)

        If BootSectorType.FATMediaDescriptor = "" Then
            FATMediaDescriptor = BPBParams.MediaDescriptor
        Else
            FATMediaDescriptor = Convert.ToByte(BootSectorType.FATMediaDescriptor, 16)
        End If

        SetFAT(Buffer, BPBParams, FATMediaDescriptor)

        If BootSectorType.BootSector.RootFill Then
            If Format = FloppyDiskFormat.FloppyTandy2000 Then
                AddTandy2000RootEntries(Buffer, BPBParams)
            Else
                AddRootEntries(Buffer, BPBParams)
            End If
        End If

        Dim Offset As UInteger = (BPBParams.ReservedSectorCount + (BPBParams.SectorsPerFAT * BPBParams.NumberOfFATs)) * BPBParams.BytesPerSector + BPBParams.RootEntryCount * 32
        For Counter As UInteger = Offset To Buffer.Length - 1
            Buffer(Counter) = &HF6
        Next

        Return Buffer
    End Function

    Private Shared Function ParseBootSectorHexString(HexString As String) As (Result As Boolean, Data As Byte())
        If HexString Is Nothing Then
            Return (False, Array.Empty(Of Byte)())
        End If

        Dim sb As New Text.StringBuilder(HexString.Length)
        For Each ch As Char In HexString
            If Not Char.IsWhiteSpace(ch) Then
                sb.Append(ch)
            End If
        Next
        Dim Cleaned As String = sb.ToString()

        If (Cleaned.Length Mod 2) <> 0 Then
            Return (False, Array.Empty(Of Byte)())
        End If

        For Each ch As Char In Cleaned
            Dim IsDigit As Boolean = (ch >= "0"c AndAlso ch <= "9"c)
            Dim IsLower As Boolean = (ch >= "a"c AndAlso ch <= "f"c)
            Dim IsUpper As Boolean = (ch >= "A"c AndAlso ch <= "F"c)

            If Not (IsDigit OrElse IsLower OrElse IsUpper) Then
                Return (False, Array.Empty(Of Byte)())
            End If
        Next

        Dim RawBytes As Byte()
        Try
            RawBytes = HexStringToBytes(Cleaned)
        Catch ex As Exception
            Return (False, Array.Empty(Of Byte)())
        End Try

        Dim Data(512 - 1) As Byte
        If RawBytes IsNot Nothing AndAlso RawBytes.Length > 0 Then
            Dim CopyLength As Integer = Math.Min(512, RawBytes.Length)
            Array.Copy(RawBytes, Data, CopyLength)
        End If

        Return (True, Data)
    End Function

    Private Shared Function ParseBootSectorNode(Node As XmlElement) As BootSectorDefinition
        Dim BootSectorName As String = Node.GetAttribute("name")

        If String.IsNullOrWhiteSpace(BootSectorName) Then
            Return Nothing
        End If

        Dim BootSectorDefinition As New BootSectorDefinition With {
            .Id = Node.GetAttribute("id"),
            .Name = BootSectorName,
            .BPB = (Node.HasAttribute("BPB") AndAlso Node.GetAttribute("BPB") = "1"),
            .VolumeSerialNumber = (Node.HasAttribute("volumeSerialNumber") AndAlso Node.GetAttribute("volumeSerialNumber") = "1"),
            .RootFill = (Node.HasAttribute("rootFill") AndAlso Node.GetAttribute("rootFill") = "1"),
            .Empty = (Node.HasAttribute("empty") AndAlso Node.GetAttribute("empty") = "1")
        }

        If BootSectorDefinition.Empty Then
            BootSectorDefinition.Data = New Byte(511) {}
        Else
            If String.IsNullOrWhiteSpace(Node.InnerText) Then
                Return Nothing
            End If

            Dim Response = ParseBootSectorHexString(Node.InnerText)

            If Not Response.Result Then
                Return Nothing
            End If

            BootSectorDefinition.Data = Response.Data
        End If

        Return BootSectorDefinition
    End Function

    Private Shared Function ParseBootSectors(XMLDoc As XmlDocument) As Dictionary(Of String, BootSectorDefinition)
        Dim BootSectors As New Dictionary(Of String, BootSectorDefinition)

        For Each BootSectorNode As XmlElement In XMLDoc.SelectNodes(XPATH_BOOT_SECTORS)
            Dim BootSector = ParseBootSectorNode(BootSectorNode)

            If BootSector IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(BootSector.Id) AndAlso Not BootSectors.ContainsKey(BootSector.Id) Then
                BootSectors.Add(BootSector.Id, BootSector)
            End If
        Next

        Return BootSectors
    End Function

    Private Shared Sub SetBPB(BPB As BiosParameterBlock, Params As FloppyDiskBPBParams)
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

    Private Shared Sub SetFAT(Data() As Byte, Params As FloppyDiskBPBParams, MediaDescriptor As Byte)

        Dim Offset As UInteger = Params.ReservedSectorCount * Params.BytesPerSector
        For Counter = 0 To Params.NumberOfFATs - 1
            Data(Offset) = MediaDescriptor
            Data(Offset + 1) = &HFF
            Data(Offset + 2) = &HFF
            Offset += Params.SectorsPerFAT * Params.BytesPerSector
        Next
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        If ComboBootSector.SelectedIndex = -1 OrElse _SelectedFormat = FloppyDiskFormat.FloppyUnknown Then
            Me.DialogResult = DialogResult.None
            Exit Sub
        End If

        _Data = CreateFloppy(_SelectedFormat, ComboBootSector.SelectedItem)

        If _Data Is Nothing Then
            Me.DialogResult = DialogResult.None
        End If
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

    Private Function GetResource(Name As String) As String
        Dim Value As String

        Dim Assembly As Reflection.[Assembly] = Reflection.[Assembly].GetExecutingAssembly()
        Dim Stream = Assembly.GetManifestResourceStream(_NameSpace & "." & Name)
        If Stream Is Nothing Then
            Throw New Exception("Unable to load resource " & Name)
        Else
            Dim TextStreamReader As New IO.StreamReader(Stream)
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

    Private Sub LocalizeForm()
        LabelBootSector.Text = My.Resources.Label_BootSector
        BtnCancel.Text = WithoutHotkey(My.Resources.Menu_Cancel)
        BtnOK.Text = WithoutHotkey(My.Resources.Menu_Ok)
        GroupBoxSpecial.Text = My.Resources.Label_SpecialFormats
        GroupBoxStandard.Text = My.Resources.Label_StandardFormats
        Me.Text = My.Resources.Label_NewDiskImage
    End Sub

    Private Sub ParseXML()
        Dim XMLDoc As New XmlDocument()
        XMLDoc.LoadXml(GetResource("bootsector.xml"))

        Dim BootSectors = ParseBootSectors(XMLDoc)

        For Each FormatNode As XmlElement In XMLDoc.SelectNodes(XPATH_FORMATS)
            Dim DiskFormat As FloppyDiskFormat = FormatNode.GetAttribute("id")

            Dim BootSectorList As New List(Of BootSectorType)

            For Each BootSectorNode As XmlElement In FormatNode.SelectNodes("bootsector")
                Dim BootSectorId = BootSectorNode.GetAttribute("id")

                If Not BootSectors.ContainsKey(BootSectorId) Then
                    Continue For
                End If

                Dim BootSectorType As New BootSectorType With {
                    .BootSector = BootSectors.Item(BootSectorId),
                    .OEMName = BootSectorNode.GetAttribute("oemName"),
                    .FATMediaDescriptor = BootSectorNode.GetAttribute("fatMediaDescriptor")
                }

                BootSectorList.Add(BootSectorType)
            Next

            _BootSectorTypes.Add(DiskFormat, BootSectorList)
        Next
    End Sub

    Private Sub RefreshCombo()
        Dim SelectdBootSectorType As BootSectorType = ComboBootSector.SelectedItem
        Dim SelectedName As String = ""

        If SelectdBootSectorType IsNot Nothing Then
            SelectedName = SelectdBootSectorType.BootSector.Name
        End If

        ComboBootSector.Items.Clear()

        If _BootSectorTypes.ContainsKey(_SelectedFormat) Then
            Dim BootSectorList = _BootSectorTypes.Item(_SelectedFormat)
            For Each BootSectorType In BootSectorList
                ComboBootSector.Items.Add(BootSectorType)

                If BootSectorType.BootSector.Name = SelectedName Then
                    ComboBootSector.SelectedIndex = ComboBootSector.Items.Count - 1
                End If
            Next
        End If

        For Each BootSector In _UserDB
            Dim BootSectorType As New BootSectorType With {
                .BootSector = BootSector,
                .OEMName = "",
                .FATMediaDescriptor = ""
            }
            ComboBootSector.Items.Add(BootSectorType)

            If BootSector.Name = SelectedName Then
                ComboBootSector.SelectedIndex = ComboBootSector.Items.Count - 1
            End If
        Next

        If ComboBootSector.Items.Count > 0 Then
            If ComboBootSector.SelectedIndex = -1 Then
                ComboBootSector.SelectedIndex = 0
            End If
        End If

        BtnOK.Enabled = ComboBootSector.Items.Count > 0
    End Sub

    Private Class BootSectorDefinition
        Public Property BPB As Boolean
        Public Property Data As Byte()
        Public Property Empty As Boolean
        Public Property Id As String
        Public Property Name As String
        Public Property RootFill As Boolean
        Public Property VolumeSerialNumber As Boolean
    End Class

    Private Class BootSectorType
        Public Property BootSector As BootSectorDefinition
        Public Property FATMediaDescriptor As String
        Public Property OEMName As String

        Public Overrides Function ToString() As String
            Return BootSector.Name
        End Function
    End Class
End Class