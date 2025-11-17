Imports System.Globalization
Imports System.Text.RegularExpressions

Namespace Flux.Kryoflux
    Public Class ConsoleParser
        Public Const REGEX_TRACK As String = "^(?<track>\d+)\.(?<side>\d+)\s*:\s?(?<encoding>[^:]+):\s?<?(?<status>\w+)\*?>?(,\s?trk:\s?(?<MFMTrack>\d+))?(\[(?<physicalTrack>\d+)\])?(,\s?sec:\s?(?<sectors>\d+))?(,\s?bad:\s?(?<bad>\d+))?(,\s?mis:\s?(?<mis>\d+))?(, \*(?<flags>\w+)( \+(?<modifiedSectors>\d+))?)?"
        Public Const REGEX_TRACK_SUMMARY As String = "^(?<track>\d+)\.(?<side>\d+)\s*:\s?(?<details>.+)"
        Public Const REGEX_TRACK_DETAILS As String = "^(?<encoding>[^:]+):\s?<?(?<status>\w+)\*?>?(,\s?trk:\s?(?<MFMTrack>\d+))?(\[(?<physicalTrack>\d+)\])?(,\s?sec:\s?(?<sectors>\d+))?(,\s?bad:\s?(?<bad>\d+))?(,\s?mis:\s?(?<mis>\d+))?(, \*(?<flags>\w+)( \+(?<modifiedSectors>\d+))?)?"

        Private ReadOnly RegExTrackDetails As New Regex(REGEX_TRACK_DETAILS, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExTrackSummary As New Regex(REGEX_TRACK_SUMMARY, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Public Function ParseTrackInfo(line As String) As TrackInfo
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExTrackDetails.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Dim info As New TrackInfo With {
                .Encoding = Match.Groups("encoding").Value.Trim(),
                .Status = Match.Groups("status").Value.Trim(),
                .MFMTrack = GetIntOrDefault(Match, "MFMTrack", -1),
                .PhysicalTrack = GetIntOrDefault(Match, "physicalTrack", -1),
                .SectorCount = GetIntOrDefault(Match, "sectors", 0),
                .BadSectorCount = GetIntOrDefault(Match, "bad", 0),
                .MissingSectorCount = GetIntOrDefault(Match, "mis", 0),
                .ModifiedSectorCount = GetIntOrDefault(Match, "modifiedSectors", 0),
                .Flags = Match.Groups("flags").Value.Trim()
            }

            Return info
        End Function

        Public Function ParseTrackSummary(line As String) As TrackSummary
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExTrackSummary.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Dim info As New TrackSummary With {
                .Track = GetInt(Match, "track"),
                .Side = GetInt(Match, "side"),
                .Details = Match.Groups("details").Value.Trim()
            }

            Return info
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

        Public Class TrackInfo
            Public Property BadSectorCount As Integer
            Public Property Encoding As String
            Public Property Flags As String
            Public Property MissingSectorCount As Integer
            Public Property ModifiedSectorCount As Integer
            Public Property SectorCount As Integer
            Public Property Status As String
            Public Property MFMTrack As Integer
            Public Property PhysicalTrack As Integer
        End Class

        Public Class TrackSummary
            Public Property Details As String
            Public Property Side As Integer
            Public Property Track As Integer
        End Class
    End Class
End Namespace