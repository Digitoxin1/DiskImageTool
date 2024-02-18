Imports System.IO
Imports System.Xml

Public Class FloppyDB
    Private ReadOnly _NameSpace As String = New StubClass().GetType.Namespace
    Private _TitleDictionary As Dictionary(Of UInteger, FloppyData)

    Public Sub New()
        ParseXML()
    End Sub

    Public Function TitleLookup(crc32 As UInteger) As FloppyData

        If _TitleDictionary.ContainsKey(crc32) Then
            Return _TitleDictionary.Item(crc32)
        Else
            Return Nothing
        End If
    End Function

    Private Function GetResource(Name As String) As String
        Dim Value As String

        Dim ResourcePath As String = Path.Combine(My.Application.Info.DirectoryPath, Name)

        Try
            Value = File.ReadAllText(ResourcePath)
        Catch
            Value = "<root />"
        End Try

        Return Value
    End Function

    Private Function GetTitleData(node As XmlElement) As FloppyData
        Dim TitleData As New FloppyData

        If node.HasAttribute("name") Then
            TitleData.Name = node.Attributes("name").Value
        End If
        If node.HasAttribute("year") Then
            TitleData.Year = node.Attributes("year").Value
        End If
        If node.HasAttribute("version") Then
            TitleData.Version = node.Attributes("version").Value
        End If
        If node.HasAttribute("disk") Then
            TitleData.Disk = node.Attributes("disk").Value
        End If
        If node.HasAttribute("media") Then
            TitleData.Media = node.Attributes("media").Value
        End If
        If node.HasAttribute("publisher") Then
            TitleData.Publisher = node.Attributes("publisher").Value
        End If
        If node.HasAttribute("verified") Then
            TitleData.Verified = node.Attributes("verified").Value
        End If
        If node.HasAttribute("region") Then
            TitleData.Region = node.Attributes("region").Value
        End If
        If node.HasAttribute("language") Then
            TitleData.Language = node.Attributes("language").Value
        End If
        If node.HasAttribute("cp") Then
            TitleData.CopyProtection = node.Attributes("cp").Value
        End If

        Return TitleData
    End Function

    Private Sub ParseXML()
        _TitleDictionary = New Dictionary(Of UInteger, FloppyData)

        Dim XMLDoc As New XmlDocument()
        XMLDoc.LoadXml(GetResource("FloppyDB.xml"))

        For Each diskNode As XmlElement In XMLDoc.SelectNodes("/root/title/disk")
            Dim parentNode = diskNode.ParentNode
            Dim crc32string As String = diskNode.Attributes("crc32").Value
            Dim crc32 As UInteger = Convert.ToUInt32(crc32string, 16)
            If Not _TitleDictionary.ContainsKey(crc32) Then
                Dim DiskData As FloppyData = GetTitleData(diskNode)
                DiskData.Parent = GetTitleData(parentNode)
                _TitleDictionary.Add(crc32, DiskData)
            End If
        Next
    End Sub


    Public Class FloppyData
        Public Property Name As String = ""
        Public Property Year As String = ""
        Public Property Version As String = ""
        Public Property Disk As String = ""
        Public Property Media As String = ""
        Public Property Publisher As String = ""
        Public Property Verified As Boolean = False
        Public Property CopyProtection As String = ""
        Public Property Region As String = ""
        Public Property Language As String = ""
        Public Property Parent As FloppyData = Nothing
        Public Function GetName() As String
            If _Name <> "" Then
                Return _Name
            ElseIf _Parent IsNot Nothing AndAlso _Parent.Name <> "" Then
                Return _Parent.Name
            Else
                Return ""
            End If
        End Function
        Public Function GetYear() As String
            If _Year <> "" Then
                Return _Year
            ElseIf _Parent IsNot Nothing AndAlso _Parent.Year <> "" Then
                Return _Parent.Year
            Else
                Return ""
            End If
        End Function
        Public Function GetVersion() As String
            If _Version <> "" Then
                Return _Version
            ElseIf _Parent IsNot Nothing AndAlso _Parent.Version <> "" Then
                Return _Parent.Version
            Else
                Return ""
            End If
        End Function
        Public Function GetDisk() As String
            If _Disk <> "" Then
                Return _Disk
            ElseIf _Parent IsNot Nothing AndAlso _Parent.Disk <> "" Then
                Return _Parent.Disk
            Else
                Return ""
            End If
        End Function
        Public Function GetMedia() As String
            If _Media <> "" Then
                Return _Media
            ElseIf _Parent IsNot Nothing AndAlso _Parent.Media <> "" Then
                Return _Parent.Media
            Else
                Return ""
            End If
        End Function
        Public Function GetPublisher() As String
            If _Publisher <> "" Then
                Return _Publisher
            ElseIf _Parent IsNot Nothing AndAlso _Parent.Publisher <> "" Then
                Return _Parent.Publisher
            Else
                Return ""
            End If
        End Function
        Public Function GetVerified() As String
            If _Verified Then
                Return _Verified
            ElseIf _Parent IsNot Nothing AndAlso _Parent.Verified Then
                Return _Parent.Verified
            Else
                Return False
            End If
        End Function
        Public Function GetRegion() As String
            If _Region <> "" Then
                Return _Region
            ElseIf _Parent IsNot Nothing AndAlso _Parent.Region <> "" Then
                Return _Parent.Region
            Else
                Return ""
            End If
        End Function
        Public Function GetLanguage() As String
            If _Language <> "" Then
                Return _Language
            ElseIf _Parent IsNot Nothing AndAlso _Parent.Language <> "" Then
                Return _Parent.Language
            Else
                Return ""
            End If
        End Function
        Public Function GetCopyProtection() As String
            If _CopyProtection <> "" Then
                Return _CopyProtection
            ElseIf _Parent IsNot Nothing AndAlso _Parent.CopyProtection <> "" Then
                Return _Parent.CopyProtection
            Else
                Return ""
            End If
        End Function
    End Class
End Class
