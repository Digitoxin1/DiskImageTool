Imports System.Text.RegularExpressions
Imports DiskImageTool.DiskImage

Public Class FloppyDB
    Public Const MAIN_DB_FILE_NAME As String = "FloppyDB.xml"
    Public Const USER_DB_FILE_NAME As String = "UserDB.xml"
    Private Const DB_FILE_NAME_NEW As String = "NewFloppyDB.xml"
    Private ReadOnly _NameSpace As String = New StubClass().GetType.Namespace
    Private _MainCount As Integer = 0
    Private _MainPath As String
    Private _NewXMLDoc As Xml.XmlDocument
    Private _TitleDictionary As Dictionary(Of String, List(Of FloppyData))
    Private _userCount As Integer = 0
    Private _UserPath As String
    Private _Version As String
    'Private ReadOnly _XMLDoc As Xml.XmlDocument

    Public Enum FloppyDBStatus As Byte
        Unknown
        Unverified
        Verified
        Modified
    End Enum

    Public Sub New()
        Load()
    End Sub

    Public ReadOnly Property MainCount As Integer
        Get
            Return _MainCount
        End Get
    End Property

    Public ReadOnly Property MainPath As String
        Get
            Return _MainPath
        End Get
    End Property

    Public ReadOnly Property UserCount As Integer
        Get
            Return _userCount
        End Get
    End Property

    Public ReadOnly Property UserPath As String
        Get
            Return _UserPath
        End Get
    End Property

    Public ReadOnly Property Version As String
        Get
            Return _Version
        End Get
    End Property

    Public Sub AddTitle(FileData As FileNameData, Media As String, MD5 As String)
        If Not FileData.Cracked And FileData.StatusString <> "M" Then
            If _NewXMLDoc Is Nothing Then
                Dim FilePath As String = IO.Path.Combine(My.Application.Info.DirectoryPath, DB_FILE_NAME_NEW)
                _NewXMLDoc = LoadXMLFromFile(FilePath)
            End If

            Dim rootNode = _NewXMLDoc.SelectSingleNode("/root")

            Dim node = rootNode.SelectSingleNode("//*[@md5=""" & MD5 & """]")
            If node Is Nothing Then
                Dim titleNode = rootNode.SelectSingleNode("title[@name=""" & FileData.Title & """]")
                If titleNode Is Nothing Then
                    titleNode = _NewXMLDoc.CreateElement("title")
                    titleNode.AppendAttribute("name", FileData.Title)
                    titleNode.AppendAttribute("publisher", "")
                    rootNode.AppendChild(titleNode)
                End If

                Dim xPathRelease = "release["

                Dim parts As New List(Of String)

                If FileData.Year <> "" Then
                    parts.Add("@year=""" & FileData.Year & """")
                Else
                    parts.Add("not(@year)")
                End If

                If FileData.Version <> "" Then
                    parts.Add("@version=""" & FileData.Version & """")
                Else
                    parts.Add("not(@version)")
                End If

                If FileData.Region <> "" Then
                    parts.Add("@region=""" & FileData.Region & """")
                Else
                    parts.Add("not(@region)")
                End If

                If FileData.Languages <> "" Then
                    parts.Add("@language=""" & FileData.Languages & """")
                Else
                    parts.Add("not(@language)")
                End If

                xPathRelease &= String.Join(" and ", parts) & "]"

                Dim releaseNode = titleNode.SelectSingleNode(xPathRelease)
                If releaseNode Is Nothing Then
                    releaseNode = _NewXMLDoc.CreateElement("release")
                    If FileData.Year <> "" Then
                        releaseNode.AppendAttribute("year", FileData.Year)
                    End If
                    If FileData.Version <> "" Then
                        releaseNode.AppendAttribute("version", FileData.Version)
                    End If
                    If FileData.Region <> "" Then
                        releaseNode.AppendAttribute("region", FileData.Region)
                    End If
                    If FileData.Languages <> "" Then
                        releaseNode.AppendAttribute("language", FileData.Languages)
                    End If
                    titleNode.AppendChild(releaseNode)
                End If

                Dim xPathMedia = "media[@media=""" & Media & """]"
                Dim mediaNode = releaseNode.SelectSingleNode(xPathMedia)
                If mediaNode Is Nothing Then
                    mediaNode = _NewXMLDoc.CreateElement("media")
                    mediaNode.AppendAttribute("media", Media)
                    mediaNode.AppendAttribute("status", FileData.StatusString)
                    releaseNode.AppendChild(mediaNode)
                End If

                Dim MediaStatus As String = ""
                Dim StatusAttribte As Xml.XmlAttribute = mediaNode.Attributes.GetNamedItem("status")
                If StatusAttribte IsNot Nothing Then
                    MediaStatus = StatusAttribte.Value
                End If

                Dim diskNode = _NewXMLDoc.CreateElement("disk")
                diskNode.AppendAttribute("md5", MD5)
                If FileData.Disk <> "" Then
                    diskNode.AppendAttribute("disk", FileData.Disk)
                End If
                If FileData.CopyProtected Then
                    diskNode.AppendAttribute("cp", "yes")
                End If
                If MediaStatus <> FileData.StatusString Then
                    diskNode.AppendAttribute("status", FileData.StatusString)
                End If
                diskNode.AppendAttribute("fileName", FileData.FileName)
                mediaNode.AppendChild(diskNode)
            End If
        End If
    End Sub

    Public Function IsVerifiedImage(Disk As Disk) As Boolean
        Dim Result = TitleFind(Disk)
        If Result.Primary IsNot Nothing Then
            If Result.Primary.GetStatus = FloppyDBStatus.Verified Then
                Return True
            End If
        End If

        Return False
    End Function

    Public Sub Load()
        _TitleDictionary = New Dictionary(Of String, List(Of FloppyData))(StringComparer.OrdinalIgnoreCase)

        Dim UserResponse = LoadXMLDoc(USER_DB_FILE_NAME)
        _UserPath = UserResponse.Path
        _userCount = ParseXML(UserResponse.XMLDoc)

        Dim MainResponse = LoadXMLDoc(MAIN_DB_FILE_NAME, App.AppSettings.DatabasePath)
        _MainPath = MainResponse.Path
        _Version = MainResponse.Version
        _MainCount = ParseXML(MainResponse.XMLDoc)
    End Sub

    'Public Function MarkDiskAsTdc(doc As Xml.XmlDocument, md5Hash As String) As Boolean
    '    If doc Is Nothing OrElse String.IsNullOrWhiteSpace(md5Hash) Then
    '        Return False
    '    End If

    '    ' XPath: find any <disk> node with matching md5
    '    Dim xpath As String = $"//disk[@md5='{md5Hash}']"
    '    Dim node As Xml.XmlNode = doc.SelectSingleNode(xpath)

    '    ' If not found, try case-insensitive search
    '    If node Is Nothing Then
    '        Dim nodes = doc.SelectNodes("//disk[@md5]")
    '        For Each n As Xml.XmlElement In nodes
    '            If String.Equals(n.GetAttribute("md5"), md5Hash, StringComparison.OrdinalIgnoreCase) Then
    '                node = n
    '                Exit For
    '            End If
    '        Next
    '    End If

    '    If node Is Nothing Then
    '        Return False ' No match found
    '    End If

    '    ' Add or update attribute tdc="1"
    '    Dim elem As Xml.XmlElement = CType(node, Xml.XmlElement)
    '    elem.SetAttribute("tdc", "1")

    '    Return True
    'End Function

    'Public Sub SaveXML()
    '    If _XMLDoc IsNot Nothing Then
    '        Dim FilePath As String = IO.Path.Combine(My.Application.Info.DirectoryPath, "FloppyDBNew.xml")
    '        Try
    '            _XMLDoc.Save(FilePath)
    '        Catch
    '        End Try
    '    End If
    'End Sub    

    Public Sub SaveNewXML()
        If _NewXMLDoc IsNot Nothing Then
            Dim FilePath As String = IO.Path.Combine(My.Application.Info.DirectoryPath, DB_FILE_NAME_NEW)

            Dim settings As New Xml.XmlWriterSettings With {
                .Indent = True,
                .IndentChars = vbTab,
                .NewLineChars = vbCrLf,
                .NewLineHandling = Xml.NewLineHandling.Replace,
                .OmitXmlDeclaration = False
            }

            Try
                Using writer = Xml.XmlWriter.Create(FilePath, settings)
                    _NewXMLDoc.Save(writer)
                End Using
            Catch
            End Try
        End If
    End Sub

    Public Function TitleCount() As Integer
        Return _TitleDictionary.Count
    End Function

    Public Function TitleExists(MD5 As String) As Boolean
        Return _TitleDictionary.ContainsKey(MD5)
    End Function

    Public Function TitleFind(Disk As Disk) As TitleFindResult
        Dim MD5 As String = Disk.Image.GetMD5Hash
        Return TitleFind(MD5)
    End Function

    Public Function TitleFind(MD5 As String) As TitleFindResult
        Return New TitleFindResult With {
            .MD5 = MD5,
            .Matches = TitleGet(MD5)
        }
    End Function

    Public Function TitleGet(MD5 As String) As List(Of FloppyData)
        Dim List As List(Of FloppyData) = Nothing

        If _TitleDictionary.TryGetValue(MD5, List) Then
            Return List
        End If

        Return Nothing
    End Function

    Private Shared Function GetFloppyDBStatus(Status As String) As FloppyDBStatus
        Select Case Status
            Case "V"
                Return FloppyDBStatus.Verified
            Case "U"
                Return FloppyDBStatus.Unverified
            Case "M"
                Return FloppyDBStatus.Modified
            Case Else
                Return FloppyDBStatus.Unknown
        End Select
    End Function

    Private Sub AddMd5Mapping(MD5 As String, Data As FloppyData)
        If String.IsNullOrWhiteSpace(MD5) Then
            Return
        End If

        Dim List As List(Of FloppyData) = Nothing
        If Not _TitleDictionary.TryGetValue(MD5, List) Then
            List = New List(Of FloppyData)(2)
            _TitleDictionary(MD5) = List
        End If

        List.Add(Data)
    End Sub

    Private Sub CheckTitleNodeForErrors(TitleNode As Xml.XmlNode)
        If TitleNode.Name <> "title" Then
            Debug.Print("FloppyDB: Invalid top level node: " & TitleNode.OuterXml)
        End If
        Dim NameFound As Boolean = False
        Dim PublisherFound As Boolean = False
        For Each Attribute As Xml.XmlAttribute In TitleNode.Attributes
            If Attribute.Name = "name" Then
                NameFound = True
            ElseIf Attribute.Name = "publisher" Then
                PublisherFound = True
            ElseIf Attribute.Name = "os" Then
            Else
                Debug.Print("FloppyDB: Check Attribute '" & Attribute.Name & "': " & TitleNode.OuterXml)
            End If
        Next
        If Not NameFound Then
            Debug.Print("FloppyDB: Name Missing: " & TitleNode.OuterXml)
        End If
        If Not PublisherFound Then
            Debug.Print("FloppyDB: Publisher Missing: " & TitleNode.OuterXml)
        End If
    End Sub

    Private Function GetNormalizedDataByBadSectors(Disk As Disk) As Byte()
        Dim Data(Disk.Image.Length - 1) As Byte
        Disk.Image.CopyTo(Data, 0)
        Dim BytesPerCluster = Disk.BPB.BytesPerCluster()
        Dim Buffer(BytesPerCluster - 1) As Byte
        For Each Cluster In Disk.FATTables.FAT(0).BadClusters
            Dim Offset = Disk.BPB.ClusterToOffset(Cluster)
            If Offset + BytesPerCluster <= Data.Length Then
                Buffer.CopyTo(Data, Offset)
            End If
        Next
        Return Data
    End Function

    Private Function GetNormalizedDataByTrackList(Disk As Disk, TrackList As List(Of FloppyDB.BooterTrack)) As Byte()
        Dim BPB As BiosParameterBlock = BuildBPB(Disk.Image.Length)

        Dim Data(Disk.Image.Length - 1) As Byte
        Disk.Image.CopyTo(Data, 0)
        Dim BytesPerTrack = BPB.BytesPerSector * BPB.SectorsPerTrack
        Dim Buffer(BytesPerTrack - 1) As Byte
        For Each Track In TrackList
            Dim Offset = BPB.SectorToBytes(BPB.TrackToSector(Track.Track, Track.Side))
            If Offset + BytesPerTrack <= Data.Length Then
                Buffer.CopyTo(Data, Offset)
            End If
        Next
        Return Data
    End Function

    Private Function GetTitleData(Node As Xml.XmlElement, Parent As FloppyData) As FloppyData
        Dim TitleData As New FloppyData With {
            .Parent = Parent
        }

        Dim Value As String

        Value = Node.GetAttribute("name")
        If Value <> "" Then
            TitleData.Name = Value
        End If

        Value = Node.GetAttribute("variation")
        If Value <> "" Then
            TitleData.Variation = Value
        End If

        Value = Node.GetAttribute("compilation")
        If Value <> "" Then
            TitleData.Compilation = Value
        End If

        Value = Node.GetAttribute("year")
        If Value <> "" Then
            TitleData.Year = Value
        End If

        Value = Node.GetAttribute("version")
        If Value <> "" Then
            TitleData.Version = Value
        End If

        Value = Node.GetAttribute("int")
        If Value <> "" Then
            TitleData.Int = Value
        End If

        Value = Node.GetAttribute("serial")
        If Value <> "" Then
            TitleData.Serial = Value
        End If

        Value = Node.GetAttribute("disk")
        If Value <> "" Then
            TitleData.Disk = Value
        End If

        Value = Node.GetAttribute("media")
        If Value <> "" Then
            TitleData.Media = FloppyDiskFormatGet(Value)
        End If

        Value = Node.GetAttribute("publisher")
        If Value <> "" Then
            TitleData.Publisher = Value
        End If

        Value = Node.GetAttribute("status")
        If Value <> "" Then
            TitleData.Status = GetFloppyDBStatus(Value)
        End If

        Value = Node.GetAttribute("region")
        If Value <> "" Then
            TitleData.Region = Value
        End If

        Value = Node.GetAttribute("language")
        If Value <> "" Then
            TitleData.Language = Value
        End If

        Value = Node.GetAttribute("cp")
        If Value <> "" Then
            TitleData.CopyProtection = Value
        End If

        Value = Node.GetAttribute("os")
        If Value <> "" Then
            TitleData.OperatingSystem = Value
        End If

        Value = Node.GetAttribute("tdc")
        If Value <> "" Then
            If Value = "1" Then
                TitleData.IsTDC = True
            End If
        End If

        Value = Node.GetAttribute("fixed")
        If Value <> "" Then
            If Value = "1" Then
                TitleData.Fixed = True
            End If
        End If

        Return TitleData
    End Function

    Private Function GetVersion(XMLDoc As Xml.XmlDocument) As String
        Return If(XMLDoc.SelectSingleNode("/root/@version")?.Value, "")
    End Function

    'Private Function GetNormalizedDataByProtectedSectors(Disk As Disk) As Byte()
    '    Dim Data(Disk.Image.Length - 1) As Byte
    '    Disk.Image.CopyTo(Data, 0)
    '    Dim Buffer(Disk.BPB.BytesPerSector - 1) As Byte
    '    For Each Sector In Disk.Image.ProtectedSectors
    '        Dim Offset = Disk.BPB.SectorToBytes(Sector)
    '        If Offset + Disk.BPB.BytesPerSector <= Data.Length Then
    '            Buffer.CopyTo(Data, Offset)
    '        End If
    '    Next
    '    Return Data
    'End Function

    Private Function LoadXMLDoc(FileName As String, Optional OverridePath As String = "") As (XMLDoc As Xml.XmlDocument, Path As String, Version As String)
        Dim BasePath As String
        Dim SourcePath As String = ""
        Dim Version As String = ""

        If OverridePath <> "" Then
            BasePath = OverridePath
        Else
            BasePath = GetAppPath()
        End If

        Dim MainFile = IO.Path.Combine(BasePath, FileName)
        Dim AppDataFile = IO.Path.Combine(GetAppDataPath(), FileName)

        Dim MainXML = TryLoadXmlWithVersion(MainFile)
        Dim AppDataXML = TryLoadXmlWithVersion(AppDataFile)

        Dim xmlDoc As Xml.XmlDocument = Nothing

        If AppDataXML.Doc IsNot Nothing AndAlso MainXML.Doc IsNot Nothing Then
            If String.Compare(MainXML.Version, AppDataXML.Version, StringComparison.Ordinal) >= 0 Then
                xmlDoc = MainXML.Doc
                SourcePath = MainFile
                Version = MainXML.Version
            Else
                xmlDoc = AppDataXML.Doc
                SourcePath = AppDataFile
                Version = AppDataXML.Version
            End If

        ElseIf AppDataXML.Doc IsNot Nothing Then
            xmlDoc = AppDataXML.Doc
            SourcePath = AppDataFile
            Version = AppDataXML.Version

        ElseIf MainXML.Doc IsNot Nothing Then
            xmlDoc = MainXML.Doc
            SourcePath = MainFile
            Version = MainXML.Version
        End If

        If xmlDoc Is Nothing Then
            xmlDoc = New Xml.XmlDocument()
            xmlDoc.LoadXml("<root />")

            Dim Saved As Boolean = False
            If MainXML.NotFound Then
                If TrySave(xmlDoc, MainFile) Then
                    SourcePath = MainFile
                    Saved = True
                End If
            End If

            If Not Saved AndAlso AppDataXML.NotFound Then
                If TrySave(xmlDoc, AppDataFile) Then
                    SourcePath = AppDataFile
                End If
            End If
        End If

        Return (xmlDoc, SourcePath, Version)
    End Function

    Private Function LoadXMLFromFile(FilePath As String) As Xml.XmlDocument
        Dim XMLDoc As New Xml.XmlDocument

        Try
            XMLDoc.Load(FilePath)
        Catch
            XMLDoc.LoadXml("<root />")
        End Try

        Return XMLDoc
    End Function

    Private Function ParseNode(Node As Xml.XmlNode, ParentData As FloppyData) As Integer
        Dim Elem = TryCast(Node, Xml.XmlElement)

        If Elem Is Nothing Then
            Return 0
        End If

        Dim Count As Integer = 0
        Dim FloppyData = GetTitleData(Elem, ParentData)

        If Elem.Name = "disk" Then
            Dim md5 = Elem.GetAttribute("md5")
            If md5 <> "" Then
                AddMd5Mapping(md5, FloppyData)
                Count += 1
            End If

            Dim md5Alt = Elem.GetAttribute("md5_alt")
            If md5Alt <> "" Then
                AddMd5Mapping(md5Alt, FloppyData)
            End If
        End If

        For Each ChildNode As Xml.XmlNode In Elem.ChildNodes
            Count += ParseNode(ChildNode, FloppyData)
        Next

        Return Count
    End Function

    Private Function ParseXML(XMLDoc As Xml.XmlDocument) As Integer
        Dim Count As Integer = 0
        For Each TitleNode As Xml.XmlNode In XMLDoc.SelectNodes("/root/title")
            Count += ParseNode(TitleNode, Nothing)

#If DEBUG Then
            CheckTitleNodeForErrors(TitleNode)
#End If
        Next

        Return Count
    End Function

    Private Function TryLoadXmlWithVersion(Path As String) As (Doc As Xml.XmlDocument, Version As String, NotFound As Boolean)
        If Not IO.File.Exists(Path) Then
            Return (Nothing, "", True)
        End If

        Try
            Dim Doc As New Xml.XmlDocument()
            Doc.Load(Path)
            Return (Doc, GetVersion(Doc), False)
        Catch
            Return (Nothing, "", False)
        End Try
    End Function

    Private Function TrySave(XMLDoc As Xml.XmlDocument, Path As String) As Boolean
        Try
            XMLDoc.Save(Path)
            Return True
        Catch ex As Exception
        End Try

        Return False
    End Function

    Public Class BooterTrack
        Public Property Side As UShort
        Public Property Track As UShort
    End Class

    Public Class FileNameData
        Public Sub New(FileName As String)
            Me.FileName = FileName
            ParseFileName()
        End Sub

        Public Property CopyProtected As Boolean = False
        Public Property Cracked As Boolean = False
        Public Property Disk As String = ""
        Public Property FileName As String = ""
        Public Property Languages As String = ""
        Public Property Modified As Boolean = False
        Public Property Region As String = ""
        Public Property Status As FloppyDBStatus = FloppyDBStatus.Unknown
        Public Property StatusString As String = ""
        Public Property Title As String = ""
        Public Property Verified As Boolean = False
        Public Property Version As String = ""
        Public Property Year As String = ""

        Private Sub ParseFileName()
            Me.Title = Trim(Regex.Match(FileName, "^[^\\(]+").Value)
            Me.Title = Replace(Me.Title, " - ", ": ")

            Dim Groups = Regex.Match(FileName, "\((\d{4}-*\d*-*\d*)\)").Groups
            If Groups.Count > 1 Then
                Me.Year = Groups.Item(1).Value
            End If

            Groups = Regex.Match(FileName, "\(v(.*?)\)").Groups
            If Groups.Count > 1 Then
                Me.Version = Groups.Item(1).Value
            End If

            Groups = Regex.Match(FileName, "\(.*Disk (.*?)\)|\((\w*? Disk)\)").Groups
            If Groups.Count > 1 Then
                Me.Disk = Groups.Item(1).Value
            End If

            Dim Captures = Regex.Match(FileName, "\[!\]").Captures
            If Captures.Count > 0 Then
                Me.Verified = True
            End If

            Captures = Regex.Match(FileName, "\[M.*\]").Captures
            If Captures.Count > 0 Then
                Me.Modified = True
            End If

            Captures = Regex.Match(FileName, "\[cp\]").Captures
            If Captures.Count > 0 Then
                Me.CopyProtected = True
            End If

            Captures = Regex.Match(FileName, "\[cr\]").Captures
            If Captures.Count > 0 Then
                Me.Cracked = True
            End If

            Groups = Regex.Match(FileName, "\((Europe|France|Germany|Spain|Italy)\)").Groups
            If Groups.Count > 1 Then
                Me.Region = Groups.Item(1).Value
            End If

            Groups = Regex.Match(FileName, "\((\s*[A-Za-z]{2}(?:\s*,\s*[A-Za-z]{2})*\s*)\)").Groups
            If Groups.Count > 1 Then
                Me.Languages = ParseLanguageList(Groups.Item(1).Value)
            End If

            If Verified Then
                Me.StatusString = "V"
            ElseIf Modified Then
                Me.StatusString = "M"
            Else
                Me.StatusString = "U"
            End If
            Me.Status = GetFloppyDBStatus(Me.StatusString)
        End Sub

        Private Function ParseLanguageList(codesList As String) As String
            If String.IsNullOrWhiteSpace(codesList) Then
                Return ""
            End If

            Dim parts = codesList.Split(","c)
            Dim result As New List(Of String)

            For Each p In parts
                Dim code = p.Trim().ToLowerInvariant()

                Try
                    ' Convert ISO-639-1 code into language name
                    Dim culture = New Globalization.CultureInfo(code)
                    ' Remove parenthetical extra info like "German (Germany)"
                    Dim name = culture.EnglishName.Split("("c)(0).Trim()
                    result.Add(name)
                Catch
                    ' Ignore invalid codes
                End Try
            Next

            Return String.Join(", ", result)
        End Function
    End Class

    Public Class FloppyData
        Public Property Compilation As String = ""
        Public Property CopyProtection As String = ""
        Public Property Disk As String = ""
        Public Property Fixed As Boolean? = Nothing
        Public Property Int As String = ""
        Public Property IsTDC As Boolean? = Nothing
        Public Property Language As String = ""
        Public Property Media As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown
        Public Property Name As String = ""
        Public Property OperatingSystem As String = ""
        Public Property Parent As FloppyData = Nothing
        Public Property Publisher As String = ""
        Public Property Region As String = ""
        Public Property Serial As String = ""
        Public Property Status As FloppyDBStatus = FloppyDBStatus.Unknown
        Public Property Variation As String = ""
        Public Property Version As String = ""
        Public Property Year As String = ""

        Public Function GetCompilation() As String
            If Me.Compilation <> "" Then
                Return Me.Compilation
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Compilation <> "" Then
                        Return Parent.Compilation
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetCopyProtection() As String
            If Me.CopyProtection <> "" Then
                Return Me.CopyProtection
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.CopyProtection <> "" Then
                        Return Parent.CopyProtection
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetDisk() As String
            If Me.Disk <> "" Then
                Return Me.Disk
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Disk <> "" Then
                        Return Parent.Disk
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetFixed() As Boolean
            If Me.Fixed.HasValue Then
                Return Me.Fixed.Value
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Fixed.HasValue Then
                        Return Parent.Fixed.Value
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return False
        End Function

        Public Function GetInt() As String
            If Me.Int <> "" Then
                Return Me.Int
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Int <> "" Then
                        Return Parent.Int
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetIsTDC() As Boolean
            If Me.IsTDC.HasValue Then
                Return Me.IsTDC.Value
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.IsTDC.HasValue Then
                        Return Parent.IsTDC.Value
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return False
        End Function

        Public Function GetLanguage() As String
            If Me.Language <> "" Then
                Return Me.Language
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Language <> "" Then
                        Return Parent.Language
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetMedia() As FloppyDiskFormat
            If Me.Media <> FloppyDiskFormat.FloppyUnknown Then
                Return Me.Media
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Media <> FloppyDiskFormat.FloppyUnknown Then
                        Return Parent.Media
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return FloppyDiskFormat.FloppyUnknown
        End Function

        Public Function GetName() As String
            If Me.Name <> "" Then
                Return Me.Name
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Name <> "" Then
                        Return Parent.Name
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetOperatingSystem() As String
            If Me.OperatingSystem <> "" Then
                Return Me.OperatingSystem
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.OperatingSystem <> "" Then
                        Return Parent.OperatingSystem
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return "MS-DOS"
        End Function

        Public Function GetPublisher() As String
            If Me.Publisher <> "" Then
                Return Me.Publisher
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Publisher <> "" Then
                        Return Parent.Publisher
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetRegion() As String
            If Me.Region <> "" Then
                Return Me.Region
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Region <> "" Then
                        Return Parent.Region
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetSerial() As String
            If Me.Serial <> "" Then
                Return Me.Serial
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Serial <> "" Then
                        Return Parent.Serial
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetStatus() As FloppyDBStatus
            If Me.Status <> FloppyDBStatus.Unknown Then
                Return Me.Status
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Status <> FloppyDBStatus.Unknown Then
                        Return Parent.Status
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return FloppyDBStatus.Unverified
        End Function

        Public Function GetVariation() As String
            If Me.Variation <> "" Then
                Return Me.Variation
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Variation <> "" Then
                        Return Parent.Variation
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetVersion() As String
            If Me.Version <> "" Then
                Return Me.Version
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Version <> "" Then
                        Return Parent.Version
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function
        Public Function GetYear() As String
            If Me.Year <> "" Then
                Return Me.Year
            Else
                Dim Parent = Me.Parent
                Do While Parent IsNot Nothing
                    If Parent.Year <> "" Then
                        Return Parent.Year
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function
    End Class

    Public Class TitleFindResult
        Public Property Matches As IReadOnlyList(Of FloppyData) = Nothing
        Public Property MD5 As String = ""
        Public ReadOnly Property Primary As FloppyData
            Get
                Return If(Matches IsNot Nothing AndAlso Matches.Count > 0, Matches(0), Nothing)
            End Get
        End Property

        Public Function GetIntList() As String
            Return JoinDistinct(Function(d) d.GetInt())
        End Function

        Public Function GetLanguageList() As String
            Return JoinDistinct(Function(d) d.GetLanguage())
        End Function

        Public Function GetNameList() As String
            Return JoinDistinct(Function(d) d.GetName(), vbNewLine)
        End Function

        Public Function GetPublisherList() As String
            Return JoinDistinct(Function(d) d.GetPublisher())
        End Function

        Public Function GetRegionList() As String
            Return JoinDistinct(Function(d) d.GetRegion())
        End Function

        Public Function GetSerialList() As String
            Return JoinDistinct(Function(d) d.GetSerial())
        End Function

        Public Function GetVersionDisplay() As String
            If Matches Is Nothing OrElse Matches.Count = 0 Then
                Return ""
            End If

            If Matches.Count = 1 Then
                Return FormatOne(Matches(0))
            End If

            Dim firstKey = MakeKey(Matches(0))
            Dim allSame As Boolean = True
            For i = 1 To Matches.Count - 1
                If MakeKey(Matches(i)) <> firstKey Then
                    allSame = False
                    Exit For
                End If
            Next

            If allSame Then
                Return FormatOne(Matches(0))
            End If

            Dim rows = Matches.Select(Function(d) New With {
                .Ver = Normalize(d.GetVersion()),
                .Ser = Normalize(d.GetSerial()),
                .Int = Normalize(d.GetInt())
            }).
            GroupBy(Function(r) r.Ver & vbNullChar & r.Ser & vbNullChar & r.Int).
            Select(Function(g) g.First()).
            ToList


            Dim groups = rows.GroupBy(Function(r) (r.Ver, r.Ser)).ToList()

            Dim parts As New List(Of String)

            For Each g In groups
                Dim seg As New List(Of String)

                If g.Key.Ver <> "" Then
                    seg.Add(g.Key.Ver)
                End If
                If g.Key.Ser <> "" Then
                    seg.Add("Serial " & g.Key.Ser)
                End If

                Dim ints = g.Select(Function(x) x.Int).Where(Function(s) s <> "").Distinct(StringComparer.OrdinalIgnoreCase).ToList()

                If ints.Count > 0 Then
                    seg.Add("Int. " & String.Join(", ", ints))
                End If

                If seg.Count > 0 Then
                    parts.Add(String.Join(", ", seg))
                End If
            Next

            Return String.Join(", ", parts)
        End Function

        Public Function GetVersionList() As String
            Return JoinDistinct(Function(d) d.GetVersion())
        End Function
        Public Function GetYearList() As String
            Return JoinDistinct(Function(d) d.GetYear())
        End Function

        Private Shared Function Normalize(s As String) As String
            Return If(s, "").Trim()
        End Function

        Private Function DistinctStrings(selector As Func(Of FloppyData, String)) As List(Of String)
            Dim result As New List(Of String)
            Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each d In Matches
                Dim s = selector(d)
                If String.IsNullOrWhiteSpace(s) Then
                    Continue For
                End If
                s = s.Trim()
                If seen.Add(s) Then
                    result.Add(s)
                End If
            Next

            Return result
        End Function

        Private Function FormatOne(d As FloppyData) As String
            Dim ver = Normalize(d.GetVersion())
            Dim ser = Normalize(d.GetSerial())
            Dim intl = Normalize(d.GetInt())

            Dim seg As New List(Of String)
            If ver <> "" Then
                seg.Add(ver)
            End If
            If ser <> "" Then
                seg.Add("Serial " & ser)
            End If
            If intl <> "" Then
                seg.Add("Int. " & intl)
            End If

            Return String.Join(", ", seg)
        End Function

        Private Function JoinDistinct(selector As Func(Of FloppyData, String), Optional Separator As String = ", ") As String
            Return String.Join(Separator, DistinctStrings(selector))
        End Function

        Private Function MakeKey(d As FloppyData) As String
            Dim ver = Normalize(d.GetVersion())
            Dim ser = Normalize(d.GetSerial())
            Dim intl = Normalize(d.GetInt())

            Return ver & vbNullChar & ser & vbNullChar & intl
        End Function
    End Class
End Class