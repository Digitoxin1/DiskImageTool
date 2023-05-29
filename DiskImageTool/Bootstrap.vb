Imports System.Xml

Public Class Bootstrap
    Private ReadOnly _NameSpace As String = New StubClass().GetType.Namespace
    Private _OEMNameDictionary As Dictionary(Of UInteger, BootstrapLookup)

    Public Sub New()
        InitOEMNames()
    End Sub

    Public Function FindMatch(BootstrapCode() As Byte) As BootstrapLookup
        Dim Checksum = Crc32.ComputeChecksum(BootstrapCode)

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
            Dim TextStreamReader = New IO.StreamReader(Stream)
            Value = TextStreamReader.ReadToEnd()
            TextStreamReader.Close()
        End If

        Return Value
    End Function

    Private Sub InitOEMNames()
        _OEMNameDictionary = New Dictionary(Of UInteger, BootstrapLookup)

        Dim XMLDoc As New XmlDocument()
        XMLDoc.LoadXml(GetResource("bootstrap.xml"))

        For Each bootstrapNode As XmlElement In XMLDoc.SelectNodes("/root/bootstrap")
            Dim crc32string As String = bootstrapNode.Attributes("crc32").Value
            Dim crc32 As UInteger = Convert.ToUInt32(crc32string, 16)
            Dim BootstrapType As New BootstrapLookup
            If bootstrapNode.HasAttribute("language") Then
                BootstrapType.Language = bootstrapNode.Attributes("language").Value
            End If
            If bootstrapNode.HasAttribute("exactmatch") Then
                BootstrapType.ExactMatch = bootstrapNode.Attributes("exactmatch").Value
            End If
            For Each oemNameNode As XmlElement In bootstrapNode.SelectNodes("oemname")
                Dim KnownOEMName As New KnownOEMName

                If oemNameNode.HasAttribute("namehex") Then
                    KnownOEMName.Name = HexStringToBytes(oemNameNode.Attributes("namehex").Value)
                ElseIf oemNameNode.HasAttribute("name") Then
                    KnownOEMName.Name = Text.Encoding.UTF8.GetBytes(oemNameNode.Attributes("name").Value)
                End If
                If oemNameNode.HasAttribute("company") Then
                    KnownOEMName.Company = oemNameNode.Attributes("company").Value
                End If
                If oemNameNode.HasAttribute("description") Then
                    KnownOEMName.Description = oemNameNode.Attributes("description").Value
                End If
                If oemNameNode.HasAttribute("note") Then
                    KnownOEMName.Note = oemNameNode.Attributes("note").Value
                End If
                If oemNameNode.HasAttribute("win9xid") Then
                    KnownOEMName.Win9xId = oemNameNode.Attributes("win9xid").Value
                End If
                If oemNameNode.HasAttribute("suggestion") Then
                    KnownOEMName.Suggestion = oemNameNode.Attributes("suggestion").Value
                End If
                If oemNameNode.HasAttribute("verified") Then
                    KnownOEMName.Verified = oemNameNode.Attributes("verified").Value
                End If
                BootstrapType.KnownOEMNames.Add(KnownOEMName)
            Next
            _OEMNameDictionary.Add(crc32, BootstrapType)
        Next
    End Sub
End Class

Public Class BootstrapLookup
    Public Property ExactMatch As Boolean = False
    Public Property KnownOEMNames As New List(Of KnownOEMName)
    Public Property Language As String = "English"
End Class

Public Class KnownOEMName
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

Public Class StubClass
End Class