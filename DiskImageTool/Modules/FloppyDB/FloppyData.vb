Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace FloppyDB
    Public Class FloppyData
        Private ReadOnly Prefixes = {"The", "A", "An"}

        Public Property Compilation As String = Nothing
        Public Property CopyProtection As String = Nothing
        Public Property Disk As String = Nothing
        Public Property DiskCount As Integer? = Nothing
        Public Property Fixed As Boolean? = Nothing
        Public Property Int As String = Nothing
        Public Property IsTDC As Boolean? = Nothing
        Public Property Language As String = Nothing
        Public Property Media As FloppyDiskFormat? = Nothing
        Public Property MediaString As String
        Public Property MobyGamesId As String = Nothing
        Public Property Name As String = Nothing
        Public Property OperatingSystem As String = Nothing
        Public Property Owner As FloppyDB = Nothing
        Public Property Parent As FloppyData = Nothing
        Public Property Publisher As String = Nothing
        Public Property Region As String = Nothing
        Public Property Serial As String = Nothing
        Public Property Status As FloppyDBStatus? = Nothing
        Public Property Variation As String = Nothing
        Public Property Version As String = Nothing
        Public Property Year As String = Nothing

        Public Function GetCompilation() As String
            Return ResolveString(Me, Function(d) d.Compilation)
        End Function

        Public Function GetCopyProtection() As String
            Return ResolveString(Me, Function(d) d.CopyProtection)
        End Function

        Public Function GetDisk() As String
            Return ResolveString(Me, Function(d) d.Disk)
        End Function

        Public Function GetDiskCount() As Integer
            Return ResolveNullable(Of Integer)(Me, Function(d) d.DiskCount, 0)
        End Function

        Public Function GetFixed() As Boolean
            Return ResolveBoolean(Me, Function(d) d.Fixed)
        End Function

        Public Function GetInt() As String
            Return ResolveString(Me, Function(d) d.Int)
        End Function

        Public Function GetIsTDC() As Boolean
            Return ResolveBoolean(Me, Function(d) d.IsTDC)
        End Function

        Public Function GetLanguage() As String
            Dim raw = GetLanguageCodes()
            If Owner Is Nothing Then
                Return If(raw, "")
            End If
            Return ResolveCodeList(raw, Owner.Languages, Owner.LanguageNameCache)
        End Function

        Public Function GetLanguageCodes() As String
            Return ResolveString(Me, Function(d) d.Language)
        End Function

        Public Function GetMedia() As FloppyDiskFormat
            Return ResolveNullable(Of FloppyDiskFormat)(Me, Function(d) d.Media, FloppyDiskFormat.FloppyUnknown)
        End Function

        Public Function GetMediaString() As String
            Return ResolveString(Me, Function(d) d.MediaString)
        End Function

        Public Function GetMobyGamesId() As String
            Return ResolveString(Me, Function(d) d.MobyGamesId)
        End Function

        Public Function GetName() As String
            Return ResolveString(Me, Function(d) d.Name)
        End Function

        Public Function GetOperatingSystem() As String
            Return ResolveString(Me, Function(d) d.OperatingSystem, "MS-DOS")
        End Function

        Public Function GetPublisher() As String
            Return ResolveString(Me, Function(d) d.Publisher)
        End Function

        Public Function GetRegion() As String
            Dim raw = GetRegionCodes()
            If Owner Is Nothing Then
                Return If(raw, "")
            End If
            Return ResolveCodeList(raw, Owner.Regions, Owner.RegionNameCache)
        End Function

        Public Function GetRegionCodes() As String
            Return ResolveString(Me, Function(d) d.Region)
        End Function

        Public Function GetSerial() As String
            Return ResolveString(Me, Function(d) d.Serial)
        End Function

        Public Function GetStatus() As FloppyDBStatus
            Return ResolveNullable(Of FloppyDBStatus)(Me, Function(d) d.Status, FloppyDBStatus.Unverified)
        End Function

        Public Function GetSuggestedFileName() As String
            Dim fileName As String

            Dim name = GetName()
            Dim variation = GetVariation()
            Dim year = GetYear()
            Dim publisher = GetPublisher()
            Dim media = GetMediaString()
            Dim version = GetVersion()
            Dim serial = GetSerial()
            Dim int = GetInt()
            Dim status = GetStatus()
            Dim region = GetRegion()
            Dim languageCodes = GetLanguageCodes()
            Dim disk = GetDisk()
            Dim fixed = GetFixed()
            Dim os = GetOperatingSystem()
            Dim cp = GetCopyProtection()

            fileName = SafeString(MoveLeadingPrefix(name, prefixes))

            If Not String.IsNullOrEmpty(variation) Then
                fileName &= " (" & SafeString(variation) & ")"
            End If

            If Not String.IsNullOrEmpty(year) Then
                fileName &= " (" & year & ")"
            End If

            Dim versionList As New List(Of String)
            If Not String.IsNullOrEmpty(version) Then
                If IsNumeric(Left(version, 1)) Then
                    versionList.Add("v" & version)
                Else
                    versionList.Add(SafeString(version))
                End If
            End If
            If Not String.IsNullOrEmpty(serial) Then
                versionList.Add("Serial " & serial)
            End If
            If Not String.IsNullOrEmpty(int) Then
                versionList.Add("Int. " & int)
            End If

            If versionList.Count > 0 Then
                fileName &= " (" & String.Join(", ", versionList) & ")"
            End If

            If Not String.IsNullOrEmpty(os) AndAlso os <> "MS-DOS" AndAlso os <> "PC Booter" Then
                If os = "Windows 3.1" Then
                    os = "Win3.1"
                End If
                fileName &= " (" & SafeString(os) & ")"
            End If

            If Not String.IsNullOrEmpty(region) Then
                If region <> "USA" Then
                    region = Replace(region, "United Kingdom", "UK")
                    fileName &= " (" & region & ")"
                End If
            End If

            If Not String.IsNullOrEmpty(languageCodes) Then
                fileName &= " (" & Owner.SortLanguageCodes(languageCodes) & ")"
            End If

            If Not String.IsNullOrEmpty(publisher) Then
                fileName &= " (" & SafeString(publisher) & ")"
            End If

            If Not String.IsNullOrEmpty(media) Then
                fileName &= " (" & media & ")"
            End If

            If os = "PC Booter" Then
                fileName &= " (Booter)"
            End If

            If Not String.IsNullOrEmpty(disk) Then
                If IsNumeric(Left(disk, 1)) OrElse disk.Length = 1 Then
                    disk = "Disk " & disk
                End If
                fileName &= " (" & SafeString(disk) & ")"
            End If

            If cp.Length > 0 Then
                fileName &= " [cp]"
            End If

            If status = FloppyDBStatus.Verified Then
                fileName &= " [!]"
            ElseIf status = FloppyDBStatus.Modified Then
                fileName &= " [M]"
            ElseIf fixed Then
                fileName &= " [F]"
            End If

            Return fileName
        End Function

        Public Function GetVariation() As String
            Return ResolveString(Me, Function(d) d.Variation)
        End Function

        Public Function GetVersion() As String
            Return ResolveString(Me, Function(d) d.Version)
        End Function

        Public Function GetYear() As String
            Return ResolveString(Me, Function(d) d.Year)
        End Function

        Private Function MoveLeadingPrefix(value As String, prefixes As IEnumerable(Of String)) As String
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

        Private Shared Function ResolveBoolean(Start As FloppyData, Selector As Func(Of FloppyData, Boolean?), Optional DefaultValue As Boolean = False) As Boolean
            Return ResolveNullable(Start, Selector, DefaultValue)
        End Function

        Private Shared Function ResolveCodeList(codeCsv As String, lookup As List(Of KeyValuePair(Of String, String)), cache As Dictionary(Of String, String)) As String
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

        Private Shared Function ResolveNullable(Of T As Structure)(Start As FloppyData, Selector As Func(Of FloppyData, Nullable(Of T)), DefaultValue As T) As T
            Dim Current = Start

            Do While Current IsNot Nothing
                Dim Value = Selector(Current)

                If Value.HasValue Then
                    Return Value.Value
                End If

                Current = Current.Parent
            Loop

            Return DefaultValue
        End Function

        Private Shared Function ResolveString(Start As FloppyData, Selector As Func(Of FloppyData, String), Optional DefaultValue As String = "") As String
            Dim Current = Start

            Do While Current IsNot Nothing
                Dim Value = Selector(Current)

                If Value IsNot Nothing Then
                    Return Value
                End If

                Current = Current.Parent
            Loop

            Return DefaultValue
        End Function

        Private Function SafeString(s As String) As String
            s = Replace(s, ":", " -")
            s = Replace(s, "/", "-")

            Return s
        End Function
    End Class
End Namespace