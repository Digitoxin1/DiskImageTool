Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Public Class FloppyDB
    Private ReadOnly _NameSpace As String = New StubClass().GetType.Namespace
    Private _BooterDictionary As Dictionary(Of UInteger, List(Of BooterTrack))
    Private _TitleDictionary As Dictionary(Of String, FloppyData)
    Private _NewXMLDoc As XmlDocument

    Public Enum FloppyDBStatus As Byte
        Unknown
        Unverified
        Verified
        Modified
    End Enum

    Public Sub New()
        Dim FilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "FloppyDB.xml")
        ParseXML(FilePath)
    End Sub

    Public Sub AddTile(FileName As String, Media As String, MD5 As String)
        Dim Title As String = Trim(Regex.Match(FileName, "^[^\\(]+").Value)
        Title = Replace(Title, " - ", ": ")

        Dim Year As String = ""
        Dim Groups = Regex.Match(FileName, "\((\d{4}-*\d*-*\d*)\)").Groups
        If Groups.Count > 1 Then
            Year = Groups.Item(1).Value
        End If

        Dim Version As String = ""
        Groups = Regex.Match(FileName, "\(v(.*?)\)").Groups
        If Groups.Count > 1 Then
            Version = Groups.Item(1).Value
        End If

        Dim Disk As String = ""
        Groups = Regex.Match(FileName, "\(Disk (.*?)\)|\((\w*? Disk)\)").Groups
        If Groups.Count > 1 Then
            Disk = Groups.Item(1).Value
        End If

        Dim Verified As Boolean = False
        Dim Captures = Regex.Match(FileName, "\[!\]").Captures
        If Captures.Count > 0 Then
            Verified = True
        End If

        Dim Modified As Boolean = False
        Captures = Regex.Match(FileName, "\[M\]").Captures
        If Captures.Count > 0 Then
            Modified = True
        End If

        Dim Cracked As Boolean = False
        Captures = Regex.Match(FileName, "\[cr\]").Captures
        If Captures.Count > 0 Then
            Cracked = True
        End If

        Dim Status As String
        If Verified Then
            Status = "V"
        ElseIf Modified Then
            Status = "M"
        Else
            Status = "U"
        End If

        If Not _TitleDictionary.ContainsKey(MD5) Then
            If Not Cracked Then
                If _NewXMLDoc Is Nothing Then
                    _NewXMLDoc = LoadXML("NewFloppyDB.xml")
                End If

                Dim rootNode = _NewXMLDoc.SelectSingleNode("/root")

                Dim node = rootNode.SelectSingleNode("//*[@md5=""" & MD5 & """]")
                If node Is Nothing Then
                    Dim titleNode = rootNode.SelectSingleNode("title[@name=""" & Title & """]")
                    If titleNode Is Nothing Then
                        titleNode = _NewXMLDoc.CreateElement("title")
                        titleNode.AppendAttribute("name", Title)
                        titleNode.AppendAttribute("publisher", "")
                        rootNode.AppendChild(titleNode)
                    End If

                    Dim xPath = "release[@media=""" & Media & """"
                    If Year <> "" Then
                        xPath &= " and @year=""" & Year & """"
                    Else
                        xPath &= " and not(@year)"
                    End If
                    If Version <> "" Then
                        xPath &= " and @version=""" & Version & """"
                    Else
                        xPath &= " and not(@version)"
                    End If
                    xPath &= "]"

                    Dim releaseNode = titleNode.SelectSingleNode(xPath)
                    If releaseNode Is Nothing Then
                        releaseNode = _NewXMLDoc.CreateElement("release")
                        If Year <> "" Then
                            releaseNode.AppendAttribute("year", Year)
                        End If
                        If Version <> "" Then
                            releaseNode.AppendAttribute("version", Version)
                        End If
                        releaseNode.AppendAttribute("media", Media)
                        releaseNode.AppendAttribute("status", Status)
                        titleNode.AppendChild(releaseNode)
                    End If

                    Dim ReleaseStatus As String = ""
                    Dim StatusAttribte As XmlAttribute = releaseNode.Attributes.GetNamedItem("status")
                    If StatusAttribte IsNot Nothing Then
                        ReleaseStatus = StatusAttribte.Value
                    End If

                    Dim diskNode = _NewXMLDoc.CreateElement("disk")
                    diskNode.AppendAttribute("md5", MD5)
                    If Disk <> "" Then
                        diskNode.AppendAttribute("disk", Disk)
                    End If
                    If ReleaseStatus <> Status Then
                        diskNode.AppendAttribute("status", Status)
                    End If
                    releaseNode.AppendChild(diskNode)
                End If
            End If
        Else
            Dim FloppyData = _TitleDictionary.Item(MD5)
            If FloppyData.GetStatus <> GetFloppyDBStatus(Status) Then
                Debug.Print("Check Status: " & MD5 & ", " & FileName)
            End If
        End If
    End Sub

    Public Function BooterLookup(BootSector() As Byte) As List(Of BooterTrack)
        Dim Checksum = Crc32.ComputeChecksum(BootSector)

        If _BooterDictionary.ContainsKey(Checksum) Then
            Return _BooterDictionary.Item(Checksum)
        Else
            Return Nothing
        End If
    End Function

    Public Function TitleCount() As Integer
        Return _TitleDictionary.Count
    End Function

    Public Function TitleExists(md5 As String) As Boolean
        Return _TitleDictionary.ContainsKey(md5)
    End Function

    Public Function TitleLookup(md5 As String) As FloppyData

        If _TitleDictionary.ContainsKey(md5) Then
            Return _TitleDictionary.Item(md5)
        Else
            Return Nothing
        End If
    End Function

    Private Function LoadXML(Name As String) As XmlDocument
        Dim XMLDoc As New XmlDocument

        Dim FilePath As String = Path.Combine(My.Application.Info.DirectoryPath, Name)

        Try
            XMLDoc.Load(FilePath)
        Catch
            XMLDoc.LoadXml("<root />")
        End Try

        Return XMLDoc
    End Function

    Private Sub SaveNewXML(Name As String)
        If _NewXMLDoc IsNot Nothing Then
            Dim FilePath As String = Path.Combine(My.Application.Info.DirectoryPath, Name)
            Try
                _NewXMLDoc.Save(Name)
            Catch
            End Try
        End If
    End Sub

    Private Function GetFloppyDBStatus(Status As String) As FloppyDBStatus
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

    Private Function GetTitleData(Node As XmlElement, Parent As FloppyData) As FloppyData
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
            TitleData.Media = GetFloppyDiskType(Media)
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

    Private Sub CheckTitleNodeForErrors(TitleNode As XmlNode)
        If TitleNode.Name <> "title" Then
            Debug.Print("Invalid top level node: " & TitleNode.OuterXml)
        End If
        Dim NameFound As Boolean = False
        Dim PublisherFound As Boolean = False
        For Each Attribute As XmlAttribute In TitleNode.Attributes
            If Attribute.Name = "name" Then
                NameFound = True
            ElseIf Attribute.Name = "publisher" Then
                PublisherFound = True
            ElseIf Attribute.Name = "os" Then
            Else
                Debug.Print("Check Attribute '" & Attribute.Name & "': " & TitleNode.OuterXml)
            End If
        Next
        If Not NameFound Then
            Debug.Print("Name Missing: " & TitleNode.OuterXml)
        End If
        If Not PublisherFound Then
            Debug.Print("Publisher Missing: " & TitleNode.OuterXml)
        End If
    End Sub

    Private Sub ParseXML(Name As String)
        _TitleDictionary = New Dictionary(Of String, FloppyData)
        _BooterDictionary = New Dictionary(Of UInteger, List(Of BooterTrack))

        If Not File.Exists(Name) Then
            Return
        End If

        Dim XMLDoc = LoadXML(Name)

        For Each TitleNode As XmlNode In XMLDoc.SelectNodes("/root/title")
            ParseNode(TitleNode, Nothing)
#If DEBUG Then
            CheckTitleNodeForErrors(TitleNode)
#End If
        Next

#If DEBUG Then
        For Each Value In _TitleDictionary.Values
            If Value.GetYear = "" Then
                Debug.Print("Missing Year: " & Value.GetName)
            End If
        Next
#End If
    End Sub

    Private Sub ParseNode(Node As XmlNode, ParentData As FloppyData)
        If TypeOf Node Is XmlElement Then
            Dim FloppyData = GetTitleData(Node, ParentData)
            If DirectCast(Node, XmlElement).HasAttribute("md5") Then
                Dim md5 As String = Node.Attributes("md5").Value
                If Not _TitleDictionary.ContainsKey(md5) Then
                    _TitleDictionary.Add(md5, FloppyData)
                End If
            End If
            If DirectCast(Node, XmlElement).HasAttribute("bootSectorCRC32") Then
                Dim crc32String As String = Node.Attributes("bootSectorCRC32").Value
                If crc32String <> "" Then
                    Dim crc32 As UInteger = Convert.ToUInt32(crc32String, 16)
                    ProcessCPNodes(Node, crc32)
                End If
            End If
            If Node.HasChildNodes Then
                For Each ChildNode As XmlNode In Node.ChildNodes
                    If ChildNode.Name <> "cp" Then
                        ParseNode(ChildNode, FloppyData)
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub ProcessCPNodes(Node As XmlNode, crc32 As UInteger)
        Dim cp As New List(Of BooterTrack)
        For Each cpNode As XmlElement In Node.SelectNodes("cp")
            If cpNode.HasAttribute("track") And cpNode.HasAttribute("side") Then
                Dim Track As New BooterTrack With {
                    .Track = cpNode.Attributes("track").Value,
                    .Side = cpNode.Attributes("side").Value
                }
                cp.Add(Track)
            End If
        Next
        If Not _BooterDictionary.ContainsKey(crc32) Then
            _BooterDictionary.Add(crc32, cp)
        End If
    End Sub

    Protected Overrides Sub Finalize()
        Dim FilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "NewFloppyDB.xml")
        SaveNewXML(FilePath)
        MyBase.Finalize()
    End Sub

    Public Class FloppyData
        Public Property MD5 As String = ""
        Public Property Name As String = ""
        Public Property Variation As String = ""
        Public Property Compilation As String = ""
        Public Property Year As String = ""
        Public Property Version As String = ""
        Public Property Disk As String = ""
        Public Property Media As FloppyDiskType = FloppyDiskType.FloppyUnknown
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
        Public Function GetMedia() As FloppyDiskType
            If _Media <> FloppyDiskType.FloppyUnknown Then
                Return _Media
            Else
                Dim Parent = _Parent
                Do While Parent IsNot Nothing
                    If Parent.Media <> FloppyDiskType.FloppyUnknown Then
                        Return Parent.Media
                    End If
                    Parent = Parent.Parent
                Loop
            End If

            Return FloppyDiskType.FloppyUnknown
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
End Class
