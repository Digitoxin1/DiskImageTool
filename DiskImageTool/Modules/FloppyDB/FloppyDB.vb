Imports DiskImageTool.DiskImage

Namespace FloppyDB
    Public Class FloppyDB
        Public Const MAIN_DB_FILE_NAME As String = "FloppyDB.xml"
        Public Const USER_DB_FILE_NAME As String = "UserDB.xml"
        Private Const DB_FILE_NAME_NEW As String = "NewFloppyDB.xml"
        Private ReadOnly _NameSpace As String = New StubClass().GetType.Namespace
        Private _LanguageNameCache As Dictionary(Of String, String)
        Private _Languages As List(Of KeyValuePair(Of String, String))
        Private _MainCount As Integer = 0
        Private _MainPath As String
        Private _NewXMLDoc As Xml.XmlDocument
        Private _RegionNameCache As Dictionary(Of String, String)
        Private _Regions As List(Of KeyValuePair(Of String, String))
        Private _TitleDictionary As Dictionary(Of String, List(Of FloppyData))
        Private _userCount As Integer = 0
        Private _UserPath As String
        Private _Version As String
        'Private ReadOnly _XMLDoc As Xml.XmlDocument

        Public Sub New()
            Load()
        End Sub

        Public ReadOnly Property LanguageNameCache As Dictionary(Of String, String)
            Get
                Return _LanguageNameCache
            End Get
        End Property

        Public ReadOnly Property Languages As IReadOnlyList(Of KeyValuePair(Of String, String))
            Get
                Return _Languages
            End Get
        End Property

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

        Public ReadOnly Property RegionNameCache As Dictionary(Of String, String)
            Get
                Return _RegionNameCache
            End Get
        End Property

        Public ReadOnly Property Regions As IReadOnlyList(Of KeyValuePair(Of String, String))
            Get
                Return _Regions
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
                If Result.Primary.Status = FloppyDBStatus.Verified Then
                    Return True
                End If
            End If

            Return False
        End Function

        Public Sub Load()
            _TitleDictionary = New Dictionary(Of String, List(Of FloppyData))(StringComparer.OrdinalIgnoreCase)
            _Languages = New List(Of KeyValuePair(Of String, String))()
            _Regions = New List(Of KeyValuePair(Of String, String))()
            _LanguageNameCache = New Dictionary(Of String, String)()
            _RegionNameCache = New Dictionary(Of String, String)()

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

        Public Function SortLanguageCodes(codesCsv As String) As String
            Return SortCodes(codesCsv, _Languages)
        End Function

        Public Function SortRegionCodes(codesCsv As String) As String
            Return SortCodes(codesCsv, _Regions)
        End Function

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

        Private Shared Sub ParseLookupList(XMLDoc As Xml.XmlDocument, XPath As String, Target As List(Of KeyValuePair(Of String, String)))
            Dim Nodes = XMLDoc.SelectNodes(XPath)
            If Nodes Is Nothing Then Return

            For Each Node As Xml.XmlNode In Nodes
                Dim Elem = TryCast(Node, Xml.XmlElement)
                If Elem Is Nothing Then Continue For

                Dim Code = If(Elem.GetAttribute("code"), "")
                Dim Name = If(Elem.GetAttribute("name"), "")
                If Code = "" Then Continue For

                Dim Exists As Boolean = False
                For Each Existing In Target
                    If String.Equals(Existing.Key, Code, StringComparison.OrdinalIgnoreCase) Then
                        Exists = True
                        Exit For
                    End If
                Next

                If Not Exists Then
                    Target.Add(New KeyValuePair(Of String, String)(Code, Name))
                End If
            Next
        End Sub

        Private Shared Function SortCodes(codesCsv As String, lookup As List(Of KeyValuePair(Of String, String))) As String
            If String.IsNullOrWhiteSpace(codesCsv) Then
                Return ""
            End If

            If lookup Is Nothing OrElse lookup.Count = 0 Then
                Return codesCsv
            End If

            Dim codes As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each part In codesCsv.Split(","c)
                Dim c = part.Trim()
                If c <> "" Then codes.Add(c)
            Next

            Dim ordered As New List(Of String)(codes.Count)
            For Each kvp In lookup ' master XML order
                If codes.Remove(kvp.Key) Then
                    ordered.Add(kvp.Key) ' canonical casing from the master list
                End If
            Next
            ' Unknown codes preserved at the end so data isn't lost

            For Each c In codes
                ordered.Add(c)
            Next

            Return String.Join(",", ordered)
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
                ElseIf Attribute.Name = "mobyGamesId" Then
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

        Private Sub ParseLookupTables(XMLDoc As Xml.XmlDocument)
            ParseLookupList(XMLDoc, "/root/languages/language", _Languages)
            ParseLookupList(XMLDoc, "/root/regions/region", _Regions)
        End Sub

        Private Function ParseNode(Node As Xml.XmlNode, Inherited As FloppyData) As Integer
            Dim Elem = TryCast(Node, Xml.XmlElement)

            If Elem Is Nothing Then
                Return 0
            End If

            Dim Count As Integer = 0
            Dim Merged As New FloppyData(Me, Inherited, Elem)

            If Elem.Name = "disk" Then
                Dim md5 = Elem.GetAttribute("md5")
                If md5 <> "" Then
                    AddMd5Mapping(md5, Merged)
                    Count += 1
                End If

                Dim md5Alt = Elem.GetAttribute("md5_alt")
                If md5Alt <> "" Then
                    AddMd5Mapping(md5Alt, Merged)
                End If

                Return Count
            End If

            For Each ChildNode As Xml.XmlNode In Elem.ChildNodes
                Count += ParseNode(ChildNode, Merged)
            Next

            Return Count
        End Function

        Private Function ParseXML(XMLDoc As Xml.XmlDocument) As Integer
            ParseLookupTables(XMLDoc)

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

        Private Class BooterTrack
            Public Property Side As UShort
            Public Property Track As UShort
        End Class
    End Class
End Namespace