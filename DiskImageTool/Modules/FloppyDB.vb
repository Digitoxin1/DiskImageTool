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
    Private _TitleDictionary As Dictionary(Of String, FloppyData)
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

    Public Sub AddTile(FileData As FileNameData, Media As String, MD5 As String)
        If Not FileData.Cracked And FileData.StatusString <> "M" Then
            If _NewXMLDoc Is Nothing Then
                Dim FilePath As String = IO.Path.Combine(My.Application.Info.DirectoryPath, DB_FILE_NAME_NEW)
                _NewXMLDoc = LoadXMLFromFile(DB_FILE_NAME_NEW)
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
        If Result.TitleData IsNot Nothing Then
            If Result.TitleData.GetStatus = FloppyDBStatus.Verified Then
                Return True
            End If
        End If

        Return False
    End Function

    Public Sub Load()
        _TitleDictionary = New Dictionary(Of String, FloppyData)

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
        Dim Response As New TitleFindResult With {
            .TitleData = TitleGet(MD5),
            .MD5 = MD5
        }

        'If Response.TitleData IsNot Nothing Then
        '    MarkDiskAsTdc(_XMLDoc, MD5)
        'End If

        Return Response
    End Function

    Public Function TitleGet(MD5 As String) As FloppyData

        If _TitleDictionary.ContainsKey(MD5) Then
            Return _TitleDictionary.Item(MD5)
        Else
            Return Nothing
        End If
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

        If Node.HasAttribute("name") Then
            TitleData.Name = Node.Attributes("name").Value
        End If
        If Node.HasAttribute("variation") Then
            TitleData.Variation = Node.Attributes("variation").Value
        End If
        If Node.HasAttribute("compilation") Then
            TitleData.Compilation = Node.Attributes("compilation").Value
        End If
        If Node.HasAttribute("year") Then
            TitleData.Year = Node.Attributes("year").Value
        End If
        If Node.HasAttribute("version") Then
            TitleData.Version = Node.Attributes("version").Value
        End If
        If Node.HasAttribute("disk") Then
            TitleData.Disk = Node.Attributes("disk").Value
        End If
        If Node.HasAttribute("media") Then
            Dim Media As String = Node.Attributes("media").Value
            TitleData.Media = FloppyDiskFormatGet(Media)
        End If
        If Node.HasAttribute("publisher") Then
            TitleData.Publisher = Node.Attributes("publisher").Value
        End If
        If Node.HasAttribute("status") Then
            Dim Status = Node.Attributes("status").Value
            TitleData.Status = GetFloppyDBStatus(Status)
        End If
        If Node.HasAttribute("region") Then
            TitleData.Region = Node.Attributes("region").Value
        End If
        If Node.HasAttribute("language") Then
            TitleData.Language = Node.Attributes("language").Value
        End If
        If Node.HasAttribute("cp") Then
            TitleData.CopyProtection = Node.Attributes("cp").Value
        End If
        If Node.HasAttribute("os") Then
            TitleData.OperatingSystem = Node.Attributes("os").Value
        End If
        If Node.HasAttribute("tdc") Then
            If Node.Attributes("tdc").Value = "1" Then
                TitleData.IsTDC = True
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
            BasePath = App.AppSettings.DatabasePath
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
        Dim Count As Integer = 0

        If TypeOf Node Is Xml.XmlElement Then
            Dim FloppyData = GetTitleData(Node, ParentData)
            If Node.Name = "disk" AndAlso DirectCast(Node, Xml.XmlElement).HasAttribute("md5") Then
                Dim md5 As String = Node.Attributes("md5").Value
                If Not _TitleDictionary.ContainsKey(md5) Then
                    _TitleDictionary.Add(md5, FloppyData)
                End If
                Count += 1
            End If
            If Node.Name = "disk" AndAlso DirectCast(Node, Xml.XmlElement).HasAttribute("md5_alt") Then
                Dim md5 As String = Node.Attributes("md5_alt").Value
                If Not _TitleDictionary.ContainsKey(md5) Then
                    _TitleDictionary.Add(md5, FloppyData)
                End If
            End If
            If Node.HasChildNodes Then
                For Each ChildNode As Xml.XmlNode In Node.ChildNodes
                    Count += ParseNode(ChildNode, FloppyData)
                Next
            End If
        End If

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

#If DEBUG Then
        For Each Value In _TitleDictionary.Values
            If Value.GetYear = "" Then
                Debug.Print("FloppyDB: Missing Year: " & Value.GetName)
            End If
        Next
#End If

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
            _FileName = FileName
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
            _Title = Trim(Regex.Match(FileName, "^[^\\(]+").Value)
            _Title = Replace(_Title, " - ", ": ")

            Dim Groups = Regex.Match(FileName, "\((\d{4}-*\d*-*\d*)\)").Groups
            If Groups.Count > 1 Then
                _Year = Groups.Item(1).Value
            End If

            Groups = Regex.Match(FileName, "\(v(.*?)\)").Groups
            If Groups.Count > 1 Then
                _Version = Groups.Item(1).Value
            End If

            Groups = Regex.Match(FileName, "\(.*Disk (.*?)\)|\((\w*? Disk)\)").Groups
            If Groups.Count > 1 Then
                _Disk = Groups.Item(1).Value
            End If

            Dim Captures = Regex.Match(FileName, "\[!\]").Captures
            If Captures.Count > 0 Then
                _Verified = True
            End If

            Captures = Regex.Match(FileName, "\[M.*\]").Captures
            If Captures.Count > 0 Then
                _Modified = True
            End If

            Captures = Regex.Match(FileName, "\[cp\]").Captures
            If Captures.Count > 0 Then
                _CopyProtected = True
            End If

            Captures = Regex.Match(FileName, "\[cr\]").Captures
            If Captures.Count > 0 Then
                _Cracked = True
            End If

            Groups = Regex.Match(FileName, "\((Europe|France|Germany|Spain|Italy)\)").Groups
            If Groups.Count > 1 Then
                _Region = Groups.Item(1).Value
            End If

            Groups = Regex.Match(FileName, "\((\s*[A-Za-z]{2}(?:\s*,\s*[A-Za-z]{2})*\s*)\)").Groups
            If Groups.Count > 1 Then
                _Languages = ParseLanguageList(Groups.Item(1).Value)
            End If

            If Verified Then
                _StatusString = "V"
            ElseIf Modified Then
                _StatusString = "M"
            Else
                _StatusString = "U"
            End If
            _Status = GetFloppyDBStatus(_StatusString)
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
        Public Property IsTDC As Boolean? = Nothing
        Public Property Language As String = ""
        Public Property Media As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown
        Public Property Name As String = ""
        Public Property OperatingSystem As String = ""
        Public Property Parent As FloppyData = Nothing
        Public Property Publisher As String = ""
        Public Property Region As String = ""
        Public Property Status As FloppyDBStatus = FloppyDBStatus.Unknown
        Public Property Variation As String = ""
        Public Property Version As String = ""
        Public Property Year As String = ""

        Public Function GetCompilation() As String
            If _Compilation <> "" Then
                Return _Compilation
            Else
                Dim Parent = _Parent
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
            If _CopyProtection <> "" Then
                Return _CopyProtection
            Else
                Dim Parent = _Parent
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
            If _Disk <> "" Then
                Return _Disk
            Else
                Dim Parent = _Parent
                Do While Parent IsNot Nothing
                    If Parent.Disk <> "" Then
                        Return Parent.Disk
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetIsTDC() As Boolean
            If _IsTDC.HasValue Then
                Return _IsTDC.Value
            Else
                Dim Parent = _Parent
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
            If _Language <> "" Then
                Return _Language
            Else
                Dim Parent = _Parent
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
            If _Media <> FloppyDiskFormat.FloppyUnknown Then
                Return _Media
            Else
                Dim Parent = _Parent
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
            If _Name <> "" Then
                Return _Name
            Else
                Dim Parent = _Parent
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
            If _OperatingSystem <> "" Then
                Return _OperatingSystem
            Else
                Dim Parent = _Parent
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
            If _Publisher <> "" Then
                Return _Publisher
            Else
                Dim Parent = _Parent
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
            If _Region <> "" Then
                Return _Region
            Else
                Dim Parent = _Parent
                Do While Parent IsNot Nothing
                    If Parent.Region <> "" Then
                        Return Parent.Region
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return ""
        End Function

        Public Function GetStatus() As FloppyDBStatus
            If _Status <> FloppyDBStatus.Unknown Then
                Return _Status
            Else
                Dim Parent = _Parent
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
            If _Variation <> "" Then
                Return _Variation
            Else
                Dim Parent = _Parent
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
            If _Version <> "" Then
                Return _Version
            Else
                Dim Parent = _Parent
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
            If _Year <> "" Then
                Return _Year
            Else
                Dim Parent = _Parent
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
        Public Property MD5 As String = ""
        Public Property TitleData As FloppyData = Nothing
    End Class
End Class