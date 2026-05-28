Imports System.Text.RegularExpressions
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace FloppyDB
    Public Class FloppyData
        Private ReadOnly _Compilation As String
        Private ReadOnly _CopyProtection As String
        Private ReadOnly _Disk As String
        Private ReadOnly _DiskCount As Integer?
        Private ReadOnly _Fixed As Boolean?
        Private ReadOnly _Int As String
        Private ReadOnly _IsTDC As Boolean?
        Private ReadOnly _Language As String
        Private ReadOnly _Media As FloppyDiskFormat?
        Private ReadOnly _MediaString As String
        Private ReadOnly _MobyGamesId As String
        Private ReadOnly _Name As String
        Private ReadOnly _OperatingSystem As String
        Private ReadOnly _Owner As FloppyDB
        Private ReadOnly _Publisher As String
        Private ReadOnly _Region As String
        Private ReadOnly _Serial As String
        Private ReadOnly _Status As FloppyDBStatus?
        Private ReadOnly _Series As String
        Private ReadOnly _System As String
        Private ReadOnly _Variation As String
        Private ReadOnly _Version As String
        Private ReadOnly _Year As String

        Private ReadOnly Prefixes = {"The", "A", "An"}
        Private ReadOnly DefLang As New Dictionary(Of String, String) From {
            {"FR", "Fr"},
            {"DE", "De"},
            {"ES", "Es"},
            {"IT", "It"}
        }
        Private ReadOnly OSAbbrev As New Dictionary(Of String, String) From {
            {"Windows 3.1", "Win3.1"},
            {"Windows 2.1", "Win2.1"},
            {"MS-DOS", ""},
            {"PC Booter", ""}
        }

        Friend Sub New(Owner As FloppyDB, Inherited As FloppyData, Node As Xml.XmlElement)
            _Owner = Owner

            If Inherited IsNot Nothing Then
                _Compilation = Inherited._Compilation
                _CopyProtection = Inherited._CopyProtection
                _Disk = Inherited._Disk
                _DiskCount = Inherited._DiskCount
                _Fixed = Inherited._Fixed
                _Int = Inherited._Int
                _IsTDC = Inherited._IsTDC
                _Language = Inherited._Language
                _Media = Inherited._Media
                _MediaString = Inherited._MediaString
                _MobyGamesId = Inherited._MobyGamesId
                _Name = Inherited._Name
                _OperatingSystem = Inherited._OperatingSystem
                _Publisher = Inherited._Publisher
                _Region = Inherited._Region
                _Serial = Inherited._Serial
                _Status = Inherited._Status
                _Series = Inherited._Series
                _System = Inherited._System
                _Variation = Inherited._Variation
                _Version = Inherited._Version
                _Year = Inherited._Year
            End If

            If Node IsNot Nothing Then
                If Node.HasAttribute("name") Then _Name = Node.GetAttribute("name")
                If Node.HasAttribute("variation") Then _Variation = Node.GetAttribute("variation")
                If Node.HasAttribute("compilation") Then _Compilation = Node.GetAttribute("compilation")
                If Node.HasAttribute("year") Then _Year = Node.GetAttribute("year")
                If Node.HasAttribute("version") Then _Version = Node.GetAttribute("version")
                If Node.HasAttribute("int") Then _Int = Node.GetAttribute("int")
                If Node.HasAttribute("serial") Then _Serial = Node.GetAttribute("serial")
                If Node.HasAttribute("disk") Then _Disk = Node.GetAttribute("disk")
                If Node.HasAttribute("publisher") Then _Publisher = Node.GetAttribute("publisher")
                If Node.HasAttribute("region") Then _Region = Node.GetAttribute("region")
                If Node.HasAttribute("language") Then _Language = Node.GetAttribute("language")
                If Node.HasAttribute("cp") Then _CopyProtection = Node.GetAttribute("cp")
                If Node.HasAttribute("os") Then _OperatingSystem = Node.GetAttribute("os")
                If Node.HasAttribute("mobyGamesId") Then _MobyGamesId = Node.GetAttribute("mobyGamesId")
                If Node.HasAttribute("system") Then _System = Node.GetAttribute("system")
                If Node.HasAttribute("series") Then _Series = Node.GetAttribute("series")

                If Node.HasAttribute("media") Then
                    _MediaString = Node.GetAttribute("media")
                    _Media = FloppyDiskFormatGet(_MediaString)
                End If

                If Node.HasAttribute("status") Then _Status = GetFloppyDBStatus(Node.GetAttribute("status"))
                If Node.HasAttribute("tdc") Then _IsTDC = (Node.GetAttribute("tdc") = "1")
                If Node.HasAttribute("fixed") Then _Fixed = (Node.GetAttribute("fixed") = "1")

                If Node.Name = "media" Then
                    Dim mediaDiskCount As Integer = 0
                    For Each childNode As Xml.XmlNode In Node.ChildNodes
                        Dim childElem = TryCast(childNode, Xml.XmlElement)
                        If childElem IsNot Nothing AndAlso childElem.Name = "disk" Then
                            mediaDiskCount += 1
                        End If
                    Next
                    _DiskCount = mediaDiskCount
                End If
            End If
        End Sub

        Public ReadOnly Property Compilation As String
            Get
                Return If(_Compilation, "")
            End Get
        End Property

        Public ReadOnly Property CopyProtection As String
            Get
                Return If(_CopyProtection, "")
            End Get
        End Property

        Public ReadOnly Property Disk As String
            Get
                Return If(_Disk, "")
            End Get
        End Property

        Public ReadOnly Property DiskCount As Integer
            Get
                Return If(_DiskCount, 0)
            End Get
        End Property

        Public ReadOnly Property Fixed As Boolean
            Get
                Return If(_Fixed, False)
            End Get
        End Property

        Public ReadOnly Property Int As String
            Get
                Return If(_Int, "")
            End Get
        End Property

        Public ReadOnly Property IsTDC As Boolean
            Get
                Return If(_IsTDC, False)
            End Get
        End Property

        Public ReadOnly Property Language As String
            Get
                If _Owner Is Nothing Then Return If(_Language, "")
                Return ResolveCodeList(If(_Language, ""), _Owner.Languages, _Owner.LanguageNameCache)
            End Get
        End Property

        Public ReadOnly Property LanguageCodes As String
            Get
                Return If(_Language, "")
            End Get
        End Property

        Public ReadOnly Property Media As FloppyDiskFormat
            Get
                Return If(_Media, FloppyDiskFormat.FloppyUnknown)
            End Get
        End Property

        Public ReadOnly Property MediaString As String
            Get
                Return If(_MediaString, "")
            End Get
        End Property

        Public ReadOnly Property MobyGamesId As String
            Get
                Return If(_MobyGamesId, "")
            End Get
        End Property

        Public ReadOnly Property Name As String
            Get
                Return If(_Name, "")
            End Get
        End Property

        Public ReadOnly Property OperatingSystem As String
            Get
                Return If(_OperatingSystem, "MS-DOS")
            End Get
        End Property

        Public ReadOnly Property Owner As FloppyDB
            Get
                Return _Owner
            End Get
        End Property
        Public ReadOnly Property Publisher As String
            Get
                Return If(_Publisher, "")
            End Get
        End Property

        Public ReadOnly Property Region As String
            Get
                If _Owner Is Nothing Then Return If(_Region, "")
                Return ResolveCodeList(If(_Region, ""), _Owner.Regions, _Owner.RegionNameCache)
            End Get
        End Property

        Public ReadOnly Property RegionCodes As String
            Get
                Return If(_Region, "")
            End Get
        End Property

        Public ReadOnly Property Serial As String
            Get
                Return If(_Serial, "")
            End Get
        End Property

        Public ReadOnly Property Status As FloppyDBStatus
            Get
                Return If(_Status, FloppyDBStatus.Unverified)
            End Get
        End Property

        Public ReadOnly Property Series As String
            Get
                Return If(_Series, "")
            End Get
        End Property

        Public ReadOnly Property System As String
            Get
                Return If(_System, "")
            End Get
        End Property

        Public ReadOnly Property Variation As String
            Get
                Return If(_Variation, "")
            End Get
        End Property

        Public ReadOnly Property Version As String
            Get
                Return If(_Version, "")
            End Get
        End Property

        Public ReadOnly Property Year As String
            Get
                Return If(_Year, "")
            End Get
        End Property

        Public Function GetSuggestedFileName(Optional IncludeAltToken As Boolean = False) As String
            Dim fileName As String

            fileName = SafeString(MoveLeadingPrefix(Me.Name, Prefixes))

            If Not String.IsNullOrEmpty(Me.Variation) Then
                fileName &= " (" & SafeString(Me.Variation) & ")"
            End If

            If Not String.IsNullOrEmpty(Me.Year) Then
                fileName &= " (" & Me.Year & ")"
            End If

            If Not String.IsNullOrEmpty(Me.System) Then
                fileName &= " (" & SafeString(Me.System) & ")"
            End If

            Dim versionList As New List(Of String)
            If Not String.IsNullOrEmpty(Me.Version) Then
                If IsRevision(Me.Version) Then
                    versionList.Add(SafeString(Me.Version))
                Else
                    versionList.Add("v" & Me.Version)
                End If
            End If
            If Not String.IsNullOrEmpty(Me.Serial) Then
                versionList.Add("Serial " & Me.Serial)
            End If
            If Not String.IsNullOrEmpty(Me.Int) Then
                versionList.Add("Int. " & Me.Int)
            End If

            If versionList.Count > 0 Then
                fileName &= " (" & String.Join(", ", versionList) & ")"
            End If

            Dim TitleOs = Me.OperatingSystem

            If Not String.IsNullOrEmpty(TitleOs) Then
                Dim abbrev As String = Nothing
                If OSAbbrev.TryGetValue(TitleOs, abbrev) Then
                    TitleOs = abbrev
                End If
            End If

            If Not String.IsNullOrEmpty(TitleOs) Then
                fileName &= " (" & SafeString(TitleOs) & ")"
            End If

            If IncludeAltToken Then
                fileName &= "{ALT}"
            End If

            Dim LanguageCodes = Me.LanguageCodes

            If Not String.IsNullOrEmpty(Me.RegionCodes) Then
                Dim LangFound As Boolean = False

                If String.IsNullOrEmpty(LanguageCodes) Then
                    LangFound = DefLang.TryGetValue(Me.RegionCodes, LanguageCodes)
                End If

                If Not LangFound Then
                    If Me.RegionCodes <> "US" Then
                        Dim TitleRegion = Replace(Me.Region, "United Kingdom", "UK")
                        fileName &= " (" & TitleRegion & ")"
                    End If
                End If
            End If

            If String.IsNullOrEmpty(Me.RegionCodes) OrElse Me.RegionCodes = "US" Then
                If LanguageCodes = "En" Then
                    LanguageCodes = ""
                End If
            End If

            If Not String.IsNullOrEmpty(LanguageCodes) Then
                fileName &= " (" & _Owner.SortLanguageCodes(LanguageCodes) & ")"
            End If

            If Not String.IsNullOrEmpty(Me.Publisher) Then
                fileName &= " (" & SafeString(Me.Publisher) & ")"
            End If

            If Not String.IsNullOrEmpty(Me.Series) Then
                fileName &= " (" & SafeString(Me.Series) & ")"
            End If

            If Not String.IsNullOrEmpty(Me.MediaString) Then
                fileName &= " (" & Me.MediaString & ")"
            End If

            If Me.OperatingSystem = "PC Booter" Then
                fileName &= " (Booter)"
            End If

            Dim TitleDisk = Me.Disk

            If Not String.IsNullOrEmpty(TitleDisk) Then
                Dim parts As String() = TitleDisk.Split({"&"c, "+"c}, StringSplitOptions.RemoveEmptyEntries).Select(Function(s) s.Trim()).ToArray()
                Dim allValid As Boolean = parts.All(Function(p) IsIntegerOrSingleCharacter(p))

                If allValid Then
                    TitleDisk = "Disk " & TitleDisk
                End If
                fileName &= " (" & SafeString(TitleDisk) & ")"
            End If

            If Me.CopyProtection.Length > 0 Then
                fileName &= " [cp]"
            End If

            If Me.Status = FloppyDBStatus.Verified Then
                fileName &= " [!]"
            ElseIf Me.Status = FloppyDBStatus.Modified Then
                fileName &= " [M]"
            ElseIf Me.Fixed Then
                fileName &= " [F]"
            End If

            Return fileName
        End Function

        Private Shared Function IsRevision(value As String) As Boolean
            If value Is Nothing Then Return False

            Return Regex.IsMatch(value, "^(?:Rev\..*|Rel\..*|r\d+)$", RegexOptions.IgnoreCase)
        End Function

        Private Shared Function IsIntegerOrSingleCharacter(value As String) As Boolean
            If String.IsNullOrWhiteSpace(value) Then
                Return False
            End If

            value = value.Trim()

            Dim n As Integer
            If Integer.TryParse(value, n) Then
                Return True
            End If

            Return value.Length = 1
        End Function

        Private Shared Function MoveLeadingPrefix(value As String, prefixes As IEnumerable(Of String)) As String
            If String.IsNullOrWhiteSpace(value) Then Return value
            If prefixes Is Nothing Then Return value

            For Each prefix As String In prefixes
                If String.IsNullOrWhiteSpace(prefix) Then Continue For

                Dim cleanPrefix As String = prefix.Trim()

                ' Match prefix as a leading word followed by a space.
                Dim matchText As String = cleanPrefix & " "

                If value.StartsWith(matchText, StringComparison.OrdinalIgnoreCase) Then
                    Dim rest As String = value.Substring(matchText.Length)

                    ' If there is a subtitle separator, only move the prefix within the main title.
                    Dim colonIndex As Integer = rest.IndexOf(":"c)

                    If colonIndex >= 0 Then
                        Dim mainTitle As String = rest.Substring(0, colonIndex).TrimEnd()
                        Dim subtitle As String = rest.Substring(colonIndex)

                        Return $"{mainTitle}, {cleanPrefix}{subtitle}"
                    Else
                        Return $"{rest}, {cleanPrefix}"
                    End If
                End If
            Next

            Return value
        End Function

        Private Shared Function ResolveCodeList(codeCsv As String, lookup As IReadOnlyList(Of KeyValuePair(Of String, String)), cache As Dictionary(Of String, String)) As String
            If String.IsNullOrWhiteSpace(codeCsv) Then
                Return ""
            End If

            If lookup Is Nothing OrElse lookup.Count = 0 Then
                Return codeCsv
            End If

            Dim cached As String = Nothing
            If cache IsNot Nothing AndAlso cache.TryGetValue(codeCsv, cached) Then
                Return cached
            End If

            Dim codes As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each part In codeCsv.Split(","c)
                Dim c = part.Trim()
                If c <> "" Then codes.Add(c)
            Next

            Dim names As New List(Of String)(codes.Count)
            For Each kvp In lookup
                If codes.Remove(kvp.Key) Then
                    names.Add(kvp.Value)
                End If
            Next

            For Each c In codes
                names.Add(c)
            Next

            Dim result = String.Join(", ", names)
            If cache IsNot Nothing Then
                cache(codeCsv) = result
            End If

            Return result
        End Function

        Private Shared Function SafeString(s As String) As String
            s = Replace(s, ":", " -")
            s = Replace(s, "/", "-")
            s = Replace(s, "?", "")
            s = Replace(s, """", "'")

            Return s
        End Function
    End Class
End Namespace
