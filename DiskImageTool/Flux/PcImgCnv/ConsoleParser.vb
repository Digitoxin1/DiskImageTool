Imports System.Globalization
Imports System.Text.RegularExpressions

Namespace Flux.PcImgCnv
    Public Class ConsoleParser
        Public Const REGEX_SECTOR_RAW As String = "^(?<mark>FB|FE):\s?(?<offset>\d+)\s?\((?<bits>\d+) bits\s?=\s?(?<bytes>\d+(\.\d+)?) bytes\)(\s?(?<message>.+))?$"
        Public Const REGEX_SECTOR_REMASTER As String = "^(?<track>\d+)\.(?<side>\d):\s?(?<modified>\*)?(?<format>.+?)(?:,\s?(?<sectors>\d+)\s+sectors)?\s*(?<messages>\(.+?\))?$"
        Public Const REGEX_SECTOR_TEXT As String = "\((?<text>[^()]*)\)"

        Private ReadOnly RegExSectorRaw As New Regex(REGEX_SECTOR_RAW, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExSectorRemaster As New Regex(REGEX_SECTOR_REMASTER, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExSectorText As New Regex(REGEX_SECTOR_TEXT, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Public Function ParseRawMessage(Input As String) As String
            If String.IsNullOrWhiteSpace(Input) Then
                Return Nothing
            End If

            Input = Input.Trim()

            If Input.StartsWith("(") AndAlso Input.EndsWith(")") Then
                Return Input.Substring(1, Input.Length - 2)
            End If

            Return Nothing
        End Function

        Public Function ParseRawSector(line As String) As RawSectorInfo?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExSectorRaw.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Return New RawSectorInfo With {
                .Mark = Match.Groups("mark").Value,
                .Offset = GetInt(Match, "offset"),
                .Bits = GetInt(Match, "bits"),
                .Bytes = GetDouble(Match, "bytes"),
                .Message = If(Match.Groups("message").Success, Match.Groups("message").Value.Trim(), String.Empty)
            }
        End Function

        Public Function ParseRawSectorData(input As String) As RawSectorData?
            If String.IsNullOrWhiteSpace(input) Then
                Return Nothing
            End If

            input = input.Trim()

            Dim parts = input.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
            If parts.Length <> 4 Then
                Return Nothing
            End If

            Dim v1, v2, v3, v4 As Byte
            If Not Byte.TryParse(parts(0), Globalization.NumberStyles.HexNumber, Nothing, v1) Then
                Return Nothing
            End If
            If Not Byte.TryParse(parts(1), Globalization.NumberStyles.HexNumber, Nothing, v2) Then
                Return Nothing
            End If
            If Not Byte.TryParse(parts(2), Globalization.NumberStyles.HexNumber, Nothing, v3) Then
                Return Nothing
            End If
            If Not Byte.TryParse(parts(3), Globalization.NumberStyles.HexNumber, Nothing, v4) Then
                Return Nothing
            End If

            Return New RawSectorData With {
                .Cylinder = v1,
                .Head = v2,
                .SectorId = v3,
                .SizeId = v4
            }
        End Function

        Public Function ParseRemasteredSector(line As String) As RemasteredSectorInfo?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExSectorRemaster.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Dim Messages = New List(Of String)

            Dim MessageList = Match.Groups("messages").Value.Trim()
            For Each MsgMatch As Match In RegExSectorText.Matches(MessageList)
                Messages.Add(MsgMatch.Groups("text").Value.Trim())
            Next


            Return New RemasteredSectorInfo With {
                .Track = GetInt(Match, "track"),
                .Side = GetInt(Match, "side"),
                .Format = Match.Groups("format").Value.Trim(),
                .Sectors = GetInt(Match, "sectors"),
                .Modified = (Match.Groups("modified").Value.Trim() = "*"),
                .Messages = Messages
            }
        End Function
        Private Shared Function GetDouble(m As Match, name As String) As Double
            Dim v As Double = 0
            Double.TryParse(m.Groups(name).Value, NumberStyles.Float, CultureInfo.InvariantCulture, v)
            Return v
        End Function

        Private Shared Function GetInt(m As Match, name As String) As Integer
            Dim v As Integer = 0
            Integer.TryParse(m.Groups(name).Value, NumberStyles.None, CultureInfo.InvariantCulture, v)
            Return v
        End Function

        Private Shared Function GetIntOrDefault(m As Match, name As String, def As Integer) As Integer
            If Not m.Groups(name).Success Then Return def
            Return GetInt(m, name)
        End Function

        Public Structure RawSectorData
            Public Cylinder As Byte
            Public Head As Byte
            Public SectorId As Byte
            Public SizeId As Byte
        End Structure

        Public Structure RawSectorInfo
            Public Bits As Integer
            Public Bytes As Double
            Public Mark As String
            Public Message As String
            Public Offset As Integer
        End Structure

        Public Structure RemasteredSectorInfo
            Public Format As String
            Public Messages As List(Of String)
            Public Modified As Boolean
            Public Sectors As Integer
            Public Side As Integer
            Public Track As Integer
        End Structure
    End Class
End Namespace
