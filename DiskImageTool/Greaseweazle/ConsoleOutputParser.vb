Imports System.Globalization
Imports System.Text.RegularExpressions

Namespace Greaseweazle
    Public Class ConsoleOutputParser
        Private ReadOnly RegExConverting As New Regex(
            "^\s*Converting\s+c=(?<cs>\d+)(?:-(?<ce>\d+))?\s*:\s*h=(?<hs>\d+)(?:-(?<he>\d+))?\s*->\s*c=(?<ds>\d+)(?:-(?<de>\d+))?\s*:\s*h=(?<dhs>\d+)(?:-(?<dhe>\d+))?\s*$",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExLine As New Regex(
            "^T(?<srcTrack>\d+)\.(?<srcSide>\d+)( <- Image (?<imgTrack>\d+)\.(?<imgSide>\d+))?: (?<system>\w+) (?<encoding>\w+)( \((?<sectorsFound>\d+)\/(?<sectorsTotal>\d+) sectors\))?( from (?<sourceType>.+))? \((?<fluxCount>\d+) flux in (?<fluxTimeMS>\d+(\.?\d+))?ms\)",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExUnexpected As New Regex(
            "^T(?<srcTrack>\d+)\.(?<srcSide>\d+): Ignoring unexpected sector C:(?<cylinder>\d+) H:(?<head>\d+) R:(?<sectorId>\d+) N:(?<sizeId>\d+)",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExWriting As New Regex(
            "^T(?<srcTrack>\d+)\.(?<srcSide>\d+): (?<action>Writing|Erasing) Track\b( \((?<details>.+)\))?",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExWritingRetries As New Regex(
            "Retry #(?<retry>\d+)",
            RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)

        Private ReadOnly RegExWritingFailed As New Regex(
            "^Failed to verify Track (?<srcTrack>\d+)\.(?<srcSide>\d+)",
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

        Public Function ParseTrackInfo(line As String) As TrackInfo
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExLine.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If


            Dim info As New TrackInfo With {
                .SourceTrack = GetInt(Match, "srcTrack"),
                .SourceSide = GetInt(Match, "srcSide"),
                .ImageTrack = GetIntOrDefault(Match, "imgTrack", .SourceTrack),
                .ImageSide = GetIntOrDefault(Match, "imgSide", .SourceSide),
                .System = Match.Groups("syatem").Value.Trim(),
                .Encoding = Match.Groups("encoding").Value.Trim(),
                .SectorsFound = GetInt(Match, "sectorsFound"),
                .SectorsTotal = GetInt(Match, "sectorsTotal"),
                .SourceType = Match.Groups("sourceType").Value.Trim(),
                .FluxCount = GetInt(Match, "fluxCount"),
                .FluxTimeMs = GetDouble(Match, "fluxTimeMS")
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
                .SourceTrack = GetInt(Match, "srcTrack"),
                .SourceSide = GetInt(Match, "srcSide"),
                .Cylinder = GetInt(Match, "cylinder"),
                .Head = GetInt(Match, "head"),
                .SectorId = GetInt(Match, "sectorId"),
                .SizeId = GetInt(Match, "sizeId")
            }

            Return info
        End Function

        Public Function ParseWriteTrackInfo(line As String) As WriteTrackInfo
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim MatchFound As Boolean = False
            Dim info As New WriteTrackInfo

            Dim Match = RegExWriting.Match(line)
            If Match.Success Then
                MatchFound = True

                Dim Details = Match.Groups("details").Value
                Dim RetryMatch = RegExWritingRetries.Match(Details)
                If RetryMatch.Success Then
                    info.Retry = GetInt(RetryMatch, "retry")
                End If
                info.SourceTrack = GetInt(Match, "srcTrack")
                info.SourceSide = GetInt(Match, "srcSide")
            End If

            If Not MatchFound Then
                Match = RegExWritingFailed.Match(line)
                If Match.Success Then
                    MatchFound = True
                    info.SourceTrack = GetInt(Match, "srcTrack")
                    info.SourceSide = GetInt(Match, "srcSide")
                    info.Failed = True
                End If
            End If

            If MatchFound Then
                Return info
            Else
                Return Nothing
            End If
        End Function

        Private Shared Function GetDouble(m As Match, name As String) As Double
            Dim v As Double
            Double.TryParse(m.Groups(name).Value, NumberStyles.Float, CultureInfo.InvariantCulture, v)
            Return v
        End Function

        Private Shared Function GetInt(m As Match, name As String) As Integer
            Dim v As Integer
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

        Public Class TrackInfo
            Public ReadOnly Property BadSectors As Integer
                Get
                    Return _SectorsTotal - _SectorsFound
                End Get
            End Property

            Public Property Encoding As String
            Public Property FluxCount As Integer
            Public Property FluxTimeMs As Double
            Public Property ImageSide As Integer
            Public Property ImageTrack As Integer
            Public ReadOnly Property IsRemapped As Boolean
                Get
                    Return _ImageTrack <> _SourceTrack OrElse _ImageSide <> _SourceSide
                End Get
            End Property
            Public Property SectorsFound As Integer
            Public Property SectorsTotal As Integer
            Public Property SourceSide As Integer
            Public Property SourceTrack As Integer
            Public Property SourceType As String
            Public Property System As String
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
            Public Property SizeId As Integer
            Public Property SourceSide As Integer
            Public Property SourceTrack As Integer
        End Class

        Public Class WriteTrackInfo
            Public Property SourceSide As Integer = 0
            Public Property SourceTrack As Integer = 0
            Public Property Retry As Integer = 0
            Public Property Failed As Boolean = False
        End Class
    End Class
End Namespace