Imports System.Globalization
Imports System.Text.RegularExpressions

Namespace Greaseweazle
    Public Class ConsoleOutputParser
        Private ReadOnly RegExConverting As New Regex(
            "^\s*Converting\s+c=(?<cs>\d+)(?:-(?<ce>\d+))?\s*:\s*h=(?<hs>\d+)(?:-(?<he>\d+))?\s*->\s*c=(?<ds>\d+)(?:-(?<de>\d+))?\s*:\s*h=(?<dhs>\d+)(?:-(?<dhe>\d+))?\s*$",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExRead As New Regex(
            "^T(?<destTrack>\d+)\.(?<destSide>\d+)( <- (?<srcType>\w+) (?<srcTrack>\d+)\.(?<srcSide>\d+))?: (?<system>\w+) (?<encoding>\w+)( \((?<sectorsFound>\d+)\/(?<sectorsTotal>\d+) sectors\))?(?: from (?<srcFormat>[^()]+))? \((?<fluxCount>\d+) flux in (?<fluxTimeMS>\d+(?:\.?\d+)?)ms\)( \(Retry #(?<seek>\d+)\.(?<retry>\d+)\))?",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExReadingFailed As New Regex(
            "^T(?<destTrack>\d+)\.(?<destSide>\d+)( <- (?<srcType>\w+) (?<srcTrack>\d+)\.(?<srcSide>\d+))?: Giving up: (?<sectors>\d+) sectors missing",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExTrackRange As New Regex(
                    "^(?<action>Reading|Writing)\b c=(?<trkStart>\d+)(-(?<trkEnd>\d+))?:h=(?<headStart>\d+)(-(?<headEnd>\d+))?",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExUnexpected As New Regex(
                    "^T(?<track>\d+)\.(?<side>\d+): Ignoring unexpected sector C:(?<cylinder>\d+) H:(?<head>\d+) R:(?<sectorId>\d+) N:(?<sizeId>\d+)",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExWrite As New Regex(
            "^T(?<srcTrack>\d+)\.(?<srcSide>\d+)( -> Drive (?<destTrack>\d+).(?<destHead>\d+))?: (?<action>Writing|Erasing) Track\b( \((?<details>.+)\))?",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExWritingFailed As New Regex(
            "^Failed to verify Track (?<srcTrack>\d+)\.(?<srcSide>\d+)",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExWritingRetries As New Regex(
                    "Retry #(?<retry>\d+)",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Public Function ParseConvertingLine(line As String) As ConvertingRange
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim m = RegExConverting.Match(line)
            If Not m.Success Then
                Return Nothing
            End If

            Dim r As New ConvertingRange With {
            .SrcCylStart = GetInt(m, "cs"),
            .SrcCylEnd = GetIntOrDefault(m, "ce", GetInt(m, "cs")),
            .SrcHeadStart = GetInt(m, "hs"),
            .SrcHeadEnd = GetIntOrDefault(m, "he", GetInt(m, "hs")),
            .DstCylStart = GetInt(m, "ds"),
            .DstCylEnd = GetIntOrDefault(m, "de", GetInt(m, "ds")),
            .DstHeadStart = GetInt(m, "dhs"),
            .DstHeadEnd = GetIntOrDefault(m, "dhe", GetInt(m, "dhs"))
        }

            Return r
        End Function

        Public Function ParseTrackInfoRead(line As String) As TrackInfoRead
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExRead.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If


            Dim info As New TrackInfoRead With {
                .DestTrack = GetInt(Match, "destTrack"),
                .DestSide = GetInt(Match, "destSide"),
                .SrcType = Match.Groups("srcType").Value.Trim(),
                .SrcTrack = GetIntOrDefault(Match, "srcTrack", .DestTrack),
                .SrcSide = GetIntOrDefault(Match, "srcSide", .DestSide),
                .System = Match.Groups("syatem").Value.Trim(),
                .Encoding = Match.Groups("encoding").Value.Trim(),
                .SectorsFound = GetInt(Match, "sectorsFound"),
                .SectorsTotal = GetInt(Match, "sectorsTotal"),
                .SrcFormat = Match.Groups("srcFormat").Value.Trim(),
                .FluxCount = GetInt(Match, "fluxCount"),
                .FluxTimeMs = GetDouble(Match, "fluxTimeMS"),
                .Seek = GetInt(Match, "seek"),
                .Retry = GetInt(Match, "retry")
            }

            Return info
        End Function

        Public Function ParseTrackInfoReadFailed(line As String) As TrackInfoReadFailed
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExReadingFailed.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Dim info As New TrackInfoReadFailed With {
                .DestTrack = GetInt(Match, "destTrack"),
                .DestSide = GetInt(Match, "destSide"),
                .SrcType = Match.Groups("srcType").Value.Trim(),
                .SrcTrack = GetIntOrDefault(Match, "srcTrack", .DestTrack),
                .SrcSide = GetIntOrDefault(Match, "srcSide", .DestSide),
                .Sectors = GetInt(Match, "sectors")
            }
            Return info
        End Function
        Public Function ParseTrackInfoWrite(line As String) As TrackInfoWrite
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim MatchFound As Boolean = False
            Dim info As New TrackInfoWrite

            Dim Match = RegExWrite.Match(line)
            If Match.Success Then
                MatchFound = True

                info.Action = Match.Groups("action").Value
                Dim Details = Match.Groups("details").Value
                Dim RetryMatch = RegExWritingRetries.Match(Details)
                If RetryMatch.Success Then
                    info.Retry = GetInt(RetryMatch, "retry")
                End If
                info.SrcTrack = GetInt(Match, "srcTrack")
                info.SrcSide = GetInt(Match, "srcSide")
                info.DestTrack = GetIntOrDefault(Match, "destTrack", info.SrcTrack)
                info.DestSide = GetIntOrDefault(Match, "destSide", info.SrcSide)
            End If

            If Not MatchFound Then
                Match = RegExWritingFailed.Match(line)
                If Match.Success Then
                    MatchFound = True
                    info.SrcTrack = GetInt(Match, "srcTrack")
                    info.SrcSide = GetInt(Match, "srcSide")
                    info.Failed = True
                End If
            End If

            If MatchFound Then
                Return info
            Else
                Return Nothing
            End If
        End Function

        Public Function ParseTrackRange(line As String) As TrackRange
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExTrackRange.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Dim info As New TrackRange With {
                .Action = Match.Groups("action").Value,
                .TrackStart = GetInt(Match, "trkStart"),
                .TrackEnd = GetIntOrDefault(Match, "trkEnd", .TrackStart),
                .HeadStart = GetInt(Match, "headStart"),
                .HeadEnd = GetIntOrDefault(Match, "headEnd", .HeadStart)
            }

            Return info
        End Function
        Public Function ParseUnexpectedSector(line As String) As UnexpectedSector
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExUnexpected.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Dim info As New UnexpectedSector With {
                .Track = GetInt(Match, "track"),
                .Side = GetInt(Match, "side"),
                .Cylinder = GetInt(Match, "cylinder"),
                .Head = GetInt(Match, "head"),
                .SectorId = GetInt(Match, "sectorId"),
                .SizeId = GetInt(Match, "sizeId")
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

        Public Class ConvertingRange
            Public Property DstCylEnd As Integer
            Public Property DstCylStart As Integer
            Public Property DstHeadEnd As Integer
            Public Property DstHeadStart As Integer
            Public Property SrcCylEnd As Integer
            Public Property SrcCylStart As Integer
            Public Property SrcHeadEnd As Integer
            Public Property SrcHeadStart As Integer
        End Class

        Public Class TrackInfoRead
            Public ReadOnly Property BadSectors As Integer
                Get
                    Return _SectorsTotal - _SectorsFound
                End Get
            End Property

            Public Property DestSide As Integer
            Public Property DestTrack As Integer
            Public Property Encoding As String
            Public Property FluxCount As Integer
            Public Property FluxTimeMs As Double
            Public ReadOnly Property IsRemapped As Boolean
                Get
                    Return _SrcTrack <> _DestTrack OrElse _SrcSide <> _DestSide
                End Get
            End Property

            Public Property Retry As Integer
            Public Property SectorsFound As Integer
            Public Property SectorsTotal As Integer
            Public Property Seek As Integer
            Public Property SrcFormat As String
            Public Property SrcSide As Integer
            Public Property SrcTrack As Integer
            Public Property SrcType As String
            Public Property System As String
        End Class

        Public Class TrackInfoReadFailed
            Public Property DestSide As Integer
            Public Property DestTrack As Integer
            Public Property Sectors As Integer
            Public Property SrcSide As Integer
            Public Property SrcTrack As Integer
            Public Property SrcType As String
        End Class
        Public Class TrackInfoWrite
            Public Property Action As String = ""
            Public Property DestSide As Integer = 0
            Public Property DestTrack As Integer = 0
            Public Property Failed As Boolean = False
            Public Property Retry As Integer = 0
            Public Property SrcSide As Integer = 0
            Public Property SrcTrack As Integer = 0
        End Class

        Public Class TrackRange
            Public Property Action As String = ""
            Public Property HeadEnd As Integer = 0
            Public Property HeadStart As Integer = 0
            Public Property TrackEnd As Integer = 0
            Public Property TrackStart As Integer = 0
        End Class

        Public Class UnexpectedSector
            Public Property Cylinder As Integer
            Public Property Head As Integer
            Public ReadOnly Property Key As String
                Get
                    Return String.Format("{0},{1},{2},{3}", _Cylinder, _Head, SectorId, _SizeId)
                End Get
            End Property

            Public Property SectorId As Integer
            Public Property Side As Integer
            Public Property SizeId As Integer
            Public Property Track As Integer
        End Class
    End Class
End Namespace