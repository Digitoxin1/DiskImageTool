Imports DiskImageTool.DiskImage

Public Class BootstrapDB
    Private ReadOnly _NameSpace As String = New StubClass().GetType.Namespace
    Private _JmpInst As HashSet(Of String)
    Private _OEMNameDictionary As Dictionary(Of UInteger, BootstrapLookup)

    Public Sub New()
        ParseXML()
    End Sub

    Public Function CheckOEMName(BootSector As BootSector) As OEMNameResponse
        Dim Response As New OEMNameResponse

        Dim BootStrapCode = BootSector.BootStrapCode

        Response.IsWin9x = BootSector.IsWin9xOEMName
        Response.NoBootLoader = BootStrapCode.Length = 0
        Response.OEMName = BootSector.OEMName

        If Not Response.NoBootLoader Then
            Response.Data = FindMatch(BootStrapCode)
            Response.Found = Response.Data IsNot Nothing
            If Response.Found Then
                Dim Win9xOEMName As OEMNameData = Nothing

                For Each KnownOEMName In Response.Data.OEMNames
                    If Response.IsWin9x And KnownOEMName.Win9xId Then
                        Win9xOEMName = KnownOEMName
                    End If
                    If KnownOEMName.Name.CompareTo(Response.OEMName) Then
                        Response.MatchedOEMName = KnownOEMName
                        Exit For
                    End If
                Next

                If Response.MatchedOEMName Is Nothing And Win9xOEMName IsNot Nothing Then
                    Response.MatchedOEMName = Win9xOEMName
                End If

                If Response.MatchedOEMName Is Nothing Then
                    If Response.Data.ExactMatch Then
                        Response.Found = False
                        Response.Data = Nothing
                    End If
                Else
                    Response.Matched = True
                    Response.Verified = Response.MatchedOEMName.Verified
                End If
            End If
        End If

        Return Response
    End Function

    Public Function FindMatch(BootstrapCode() As Byte) As BootstrapLookup
        If BootstrapCode.Length = 0 Then
            Return Nothing
        End If

        Dim Checksum = CRC32.ComputeChecksum(BootstrapCode)

        If _OEMNameDictionary.ContainsKey(Checksum) Then
            Return _OEMNameDictionary.Item(Checksum)
        Else
            If BootstrapCode.Length = &H1AD Then
                Return FindXDFMatch(BootstrapCode)
            Else
                Return Nothing
            End If
        End If
    End Function

    Public Function SearchBootSector(BootSector As BootSector) As BootstrapLookup
        For Each JmpString In _JmpInst
            Dim Jmp = HexStringToBytes(JmpString)
            Dim BootstrapCode = BootSector.GetBootStrapCode(Jmp)
            Dim Lookup = FindMatch(BootstrapCode)
            If Lookup IsNot Nothing Then
                Return Lookup
            End If
        Next

        Return Nothing
    End Function

    Private Function FindXDFMatch(BootstrapCode() As Byte) As BootstrapLookup
        For Counter = &HE7 To &HEE
            BootstrapCode(Counter) = 0
        Next
        Dim Checksum = CRC32.ComputeChecksum(BootstrapCode)

        If _OEMNameDictionary.ContainsKey(Checksum) Then
            Return _OEMNameDictionary.Item(Checksum)
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
            Dim TextStreamReader As New IO.StreamReader(Stream)
            Value = TextStreamReader.ReadToEnd()
            TextStreamReader.Close()
        End If

        Return Value
    End Function

    Private Sub ParseXML()
        _OEMNameDictionary = New Dictionary(Of UInteger, BootstrapLookup)
        _JmpInst = New HashSet(Of String)

        Dim XMLDoc As New Xml.XmlDocument()
        XMLDoc.LoadXml(GetResource("bootstrap.xml"))

        For Each bootstrapNode As Xml.XmlElement In XMLDoc.SelectNodes("/root/bootstrap")
            Dim crc32string As String = bootstrapNode.Attributes("crc32").Value
            Dim crc32 As UInteger = Convert.ToUInt32(crc32string, 16)
            Dim BootstrapType As New BootstrapLookup
            If bootstrapNode.HasAttribute("language") Then
                BootstrapType.Language = bootstrapNode.Attributes("language").Value
            End If
            If bootstrapNode.HasAttribute("exactmatch") Then
                BootstrapType.ExactMatch = bootstrapNode.Attributes("exactmatch").Value
            End If
            If bootstrapNode.HasAttribute("jmp") Then
                BootstrapType.Jmp = bootstrapNode.Attributes("jmp").Value
            End If
            For Each oemNameNode As Xml.XmlElement In bootstrapNode.SelectNodes("oemname")
                Dim OEMName As New OEMNameData

                If oemNameNode.HasAttribute("namehex") Then
                    OEMName.Name = HexStringToBytes(oemNameNode.Attributes("namehex").Value)
                ElseIf oemNameNode.HasAttribute("name") Then
                    OEMName.Name = Text.Encoding.UTF8.GetBytes(oemNameNode.Attributes("name").Value)
                End If
                If oemNameNode.HasAttribute("company") Then
                    OEMName.Company = oemNameNode.Attributes("company").Value
                End If
                If oemNameNode.HasAttribute("description") Then
                    OEMName.Description = oemNameNode.Attributes("description").Value
                End If
                If oemNameNode.HasAttribute("note") Then
                    OEMName.Note = oemNameNode.Attributes("note").Value
                End If
                If oemNameNode.HasAttribute("win9xid") Then
                    OEMName.Win9xId = oemNameNode.Attributes("win9xid").Value
                End If
                If oemNameNode.HasAttribute("suggestion") Then
                    OEMName.Suggestion = oemNameNode.Attributes("suggestion").Value
                End If
                If oemNameNode.HasAttribute("verified") Then
                    OEMName.Verified = oemNameNode.Attributes("verified").Value
                End If
                BootstrapType.OEMNames.Add(OEMName)
            Next
            _OEMNameDictionary.Add(crc32, BootstrapType)

            If BootstrapType.Jmp <> "" Then
                If Not _JmpInst.Contains(BootstrapType.Jmp) Then
                    _JmpInst.Add(BootstrapType.Jmp)
                End If
            End If
        Next
    End Sub

End Class

Public Class BootstrapLookup
    Public Property ExactMatch As Boolean = False
    Public Property Jmp As String
    Public Property Language As String = "English"
    Public Property OEMNames As New List(Of OEMNameData)
End Class

Public Class OEMNameData
    Public Property Company As String = ""
    Public Property Description As String = ""
    Public Property Name As Byte()
    Public Property Note As String = ""
    Public Property Suggestion As Boolean = True
    Public Property Verified As Boolean = False
    Public Property Win9xId As Boolean = False

    Public Function GetNameAsString() As String
        Return DiskImage.CodePage437ToUnicode(_Name)
    End Function

    Public Overrides Function ToString() As String
        Return GetNameAsString()
    End Function

End Class

Public Class OEMNameResponse
    Public Property Data As BootstrapLookup = Nothing
    Public Property Found As Boolean = False
    Public Property IsWin9x As Boolean = False
    Public Property Matched As Boolean = False
    Public Property MatchedOEMName As OEMNameData = Nothing
    Public Property NoBootLoader As Boolean = False
    Public Property OEMName As Byte()
    Public Property Verified As Boolean = False
End Class

Public Class StubClass
End Class