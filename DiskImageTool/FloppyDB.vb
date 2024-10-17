Imports System.Text.RegularExpressions
Imports DiskImageTool.DiskImage

Public Class FloppyDB
    Private Const DB_FILE_NAME As String = "FloppyDB.xml"
    Private Const DB_FILE_NAME_NEW As String = "NewFloppyDB.xml"
    Private ReadOnly _NameSpace As String = New StubClass().GetType.Namespace
    Private _TitleDictionary As Dictionary(Of String, FloppyData)
    Private _NewXMLDoc As Xml.XmlDocument

    Public Enum FloppyDBStatus As Byte
        Unknown
        Unverified
        Verified
        Modified
    End Enum

    Public Sub New()
        Dim FilePath = IO.Path.Combine(IO.Path.GetDirectoryName(Application.ExecutablePath), DB_FILE_NAME)
        ParseXML(FilePath)
    End Sub

    Public Sub AddTile(FileData As FileNameData, Media As String, MD5 As String)
        If Not FileData.Cracked And FileData.StatusString <> "MM" Then
            If _NewXMLDoc Is Nothing Then
                _NewXMLDoc = LoadXML(DB_FILE_NAME_NEW)
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

                Dim xPath = "release[@media=""" & Media & """"
                If FileData.Year <> "" Then
                    xPath &= " and @year=""" & FileData.Year & """"
                Else
                    xPath &= " and not(@year)"
                End If
                If FileData.Version <> "" Then
                    xPath &= " and @version=""" & FileData.Version & """"
                Else
                    xPath &= " and not(@version)"
                End If
                xPath &= "]"

                Dim releaseNode = titleNode.SelectSingleNode(xPath)
                If releaseNode Is Nothing Then
                    releaseNode = _NewXMLDoc.CreateElement("release")
                    If FileData.Year <> "" Then
                        releaseNode.AppendAttribute("year", FileData.Year)
                    End If
                    If FileData.Version <> "" Then
                        releaseNode.AppendAttribute("version", FileData.Version)
                    End If
                    releaseNode.AppendAttribute("media", Media)
                    releaseNode.AppendAttribute("status", FileData.StatusString)
                    titleNode.AppendChild(releaseNode)
                End If

                Dim ReleaseStatus As String = ""
                Dim StatusAttribte As Xml.XmlAttribute = releaseNode.Attributes.GetNamedItem("status")
                If StatusAttribte IsNot Nothing Then
                    ReleaseStatus = StatusAttribte.Value
                End If

                Dim diskNode = _NewXMLDoc.CreateElement("disk")
                diskNode.AppendAttribute("md5", MD5)
                If FileData.Disk <> "" Then
                    diskNode.AppendAttribute("disk", FileData.Disk)
                End If
                If ReleaseStatus <> FileData.StatusString Then
                    diskNode.AppendAttribute("status", FileData.StatusString)
                End If
                diskNode.AppendAttribute("fileName", FileData.FileName)
                releaseNode.AppendChild(diskNode)
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

    Public Function TitleCount() As Integer
        Return _TitleDictionary.Count
    End Function

    Public Function TitleExists(MD5 As String) As Boolean
        Return _TitleDictionary.ContainsKey(MD5)
    End Function

    Public Function TitleFind(Disk As Disk) As TitleFindResult
        Dim MD5 As String = Disk.Image.Data.GetMD5Hash
        Return TitleFind(MD5)
    End Function

    Public Function TitleFind(MD5 As String) As TitleFindResult
        Dim Response As New TitleFindResult With {
            .TitleData = TitleGet(MD5),
            .MD5 = MD5
        }

        Return Response
    End Function

    Public Function TitleGet(MD5 As String) As FloppyData

        If _TitleDictionary.ContainsKey(MD5) Then
            Return _TitleDictionary.Item(MD5)
        Else
            Return Nothing
        End If
    End Function

    Private Function LoadXML(Name As String) As Xml.XmlDocument
        Dim XMLDoc As New Xml.XmlDocument

        Dim FilePath As String = IO.Path.Combine(My.Application.Info.DirectoryPath, Name)

        Try
            XMLDoc.Load(FilePath)
        Catch
            XMLDoc.LoadXml("<root />")
        End Try

        Return XMLDoc
    End Function

    Public Sub SaveNewXML()
        If _NewXMLDoc IsNot Nothing Then
            Dim FilePath As String = IO.Path.Combine(My.Application.Info.DirectoryPath, DB_FILE_NAME_NEW)
            Try
                _NewXMLDoc.Save(FilePath)
            Catch
            End Try
        End If
    End Sub

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

    Private Function GetNormalizedDataByProtectedSectors(Disk As Disk) As Byte()
        Dim Data(Disk.Image.Length - 1) As Byte
        Disk.Image.CopyTo(Data, 0)
        Dim Buffer(Disk.BYTES_PER_SECTOR - 1) As Byte
        For Each Sector In Disk.Image.Data.ProtectedSectors
            Dim Offset = Disk.SectorToBytes(Sector)
            If Offset + Disk.BYTES_PER_SECTOR <= Data.Length Then
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
            Dim Offset = Disk.SectorToBytes(BPB.TrackToSector(Track.Track, Track.Side))
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
            TitleData.Media = GetFloppyDiskFormat(Media)
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

        Return TitleData
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

    Private Sub ParseXML(Name As String)
        _TitleDictionary = New Dictionary(Of String, FloppyData)

        If Not IO.File.Exists(Name) Then
            Return
        End If

        Dim XMLDoc = LoadXML(Name)

        For Each TitleNode As Xml.XmlNode In XMLDoc.SelectNodes("/root/title")
            ParseNode(TitleNode, Nothing)
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
    End Sub

    Private Sub ParseNode(Node As Xml.XmlNode, ParentData As FloppyData)
        If TypeOf Node Is Xml.XmlElement Then
            Dim FloppyData = GetTitleData(Node, ParentData)
            If DirectCast(Node, Xml.XmlElement).HasAttribute("md5") Then
                Dim md5 As String = Node.Attributes("md5").Value
                If Not _TitleDictionary.ContainsKey(md5) Then
                    _TitleDictionary.Add(md5, FloppyData)
                End If
            End If
            If Node.HasChildNodes Then
                For Each ChildNode As Xml.XmlNode In Node.ChildNodes
                    ParseNode(ChildNode, FloppyData)
                Next
            End If
        End If
    End Sub

    Public Class FileNameData
        Public Property Cracked As Boolean = False
        Public Property Disk As String = ""
        Public Property FileName As String = ""
        Public Property Modified As Boolean = False
        Public Property Status As FloppyDBStatus = FloppyDBStatus.Unknown
        Public Property StatusString As String = ""
        Public Property Title As String = ""
        Public Property Verified As Boolean = False
        Public Property Version As String = ""
        Public Property Year As String = ""

        Public Sub New(FileName As String)
            _FileName = FileName
            ParseFileName()
        End Sub

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

            Groups = Regex.Match(FileName, "\(Disk (.*?)\)|\((\w*? Disk)\)").Groups
            If Groups.Count > 1 Then
                _Disk = Groups.Item(1).Value
            End If

            Dim Captures = Regex.Match(FileName, "\[!\]").Captures
            If Captures.Count > 0 Then
                _Verified = True
            End If

            Captures = Regex.Match(FileName, "\[M\]").Captures
            If Captures.Count > 0 Then
                _Modified = True
            End If

            Captures = Regex.Match(FileName, "\[cr\]").Captures
            If Captures.Count > 0 Then
                _Cracked = True
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
    End Class

    Public Class FloppyData
        Public Property Name As String = ""
        Public Property Variation As String = ""
        Public Property Compilation As String = ""
        Public Property Year As String = ""
        Public Property Version As String = ""
        Public Property Disk As String = ""
        Public Property Media As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown
        Public Property Publisher As String = ""
        Public Property Status As FloppyDBStatus = FloppyDBStatus.Unknown
        Public Property CopyProtection As String = ""
        Public Property Region As String = ""
        Public Property Language As String = ""
        Public Property OperatingSystem As String = ""
        Public Property Parent As FloppyData = Nothing

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
    End Class

    Public Class BooterTrack
        Public Property Track As UShort
        Public Property Side As UShort
    End Class

    Public Class TitleFindResult
        Public Property TitleData As FloppyData = Nothing
        Public Property MD5 As String = ""
    End Class
End Class
