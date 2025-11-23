Imports System.Globalization
Imports System.Text.RegularExpressions

Namespace Flux.PcImgCnv
    Public Class ConsoleParser
        Public Const REGEX_SECTOR As String = "^(?<mark>FB|FE):\s?(?<offset>\d+)\s?\((?<bits>\d+) bits\s?=\s?(?<bytes>\d+(\.\d+)?) bytes\)(\s?(?<message>.+))?$"

        Private ReadOnly RegExSector As New Regex(REGEX_SECTOR, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Public Function ParseSector(line As String) As SectorInfo?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Mark As String = line.Substring(0, 2)

            If Mark <> "FE" AndAlso Mark <> "FB" Then
                Return Nothing
            End If

            Dim Match = RegExSector.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Return New SectorInfo With {
                .Mark = Match.Groups("mark").Value,
                .Offset = GetInt(Match, "offset"),
                .Bits = GetInt(Match, "bits"),
                .Bytes = GetDouble(Match, "bytes"),
                .Message = If(Match.Groups("message").Success, Match.Groups("message").Value.Trim(), String.Empty)
            }
        End Function

        Public Function ParseSectorData(input As String) As SectorData?
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

            Return New SectorData With {
                .Cylinder = v1,
                .Head = v2,
                .SectorId = v3,
                .SizeId = v4
            }
        End Function

        Public Function ParseMessage(Input As String) As String
            If String.IsNullOrWhiteSpace(Input) Then
                Return Nothing
            End If

            Input = Input.Trim()

            If Input.StartsWith("(") AndAlso Input.EndsWith(")") Then
                Return Input.Substring(1, Input.Length - 2)
            End If

            Return Nothing
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

        Public Structure SectorData
            Public Cylinder As Byte
            Public Head As Byte
            Public SectorId As Byte
            Public SizeId As Byte
        End Structure

        Public Structure SectorInfo
            Public Mark As String
            Public Offset As Integer
            Public Bits As Integer
            Public Bytes As Double
            Public Message As String
        End Structure
    End Class
End Namespace
