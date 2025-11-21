Imports System.Globalization
Imports System.Text.RegularExpressions

Namespace Flux.Greaseweazle
    Public Class ConsoleParser
        Public Const REGEX_COMMAND_FAILED As String = "^Command Failed:\s*(?<message>[^:]+):\s*(?<details>.+)$"
        Public Const REGEX_DISK_RANGE As String = "^(?<action>Reading|Writing)\b c=(?<trkStart>\d+)(-(?<trkEnd>\d+))?:h=(?<headStart>\d+)(-(?<headEnd>\d+))?"
        Public Const REGEX_SECTOR_GRID_END As String = "^Found (?<foundSectors>\d+) sectors of (?<totalSectors>\d+) \((?<percentage>\d+)%\)"
        Public Const REGEX_SECTOR_GRID_LINE As String = "^(?<side>\d+)\.\s?(?<sector>\d+):\s?(?<tracks>[\.X]+)"
        Public Const REGEX_TRACK_CONVERTING As String = "^\s*Converting\s+c=(?<cs>\d+)(?:-(?<ce>\d+))?\s*:\s*h=(?<hs>\d+)(?:-(?<he>\d+))?\s*->\s*c=(?<ds>\d+)(?:-(?<de>\d+))?\s*:\s*h=(?<dhs>\d+)(?:-(?<dhe>\d+))?\s*$"
        Public Const REGEX_TRACK_READ_DETAILS As String = "^(?<system>\w+) (?<encoding>\w+)( \((?<sectorsFound>\d+)\/(?<sectorsTotal>\d+) sectors\))?(?: from (?<srcFormat>[^()]+))? \((?<fluxCount>\d+) flux in (?<fluxTimeMS>\d+(?:\.?\d+)?)ms\)( \(Retry #(?<seek>\d+)\.(?<retry>\d+)\))?"
        Public Const REGEX_TRACK_READ_FAILED As String = "^Giving up: (?<sectors>\d+) sectors missing"
        Public Const REGEX_TRACK_READ_OUTOFRANGE As String = "^WARNING: Out of range for format '(?<format>(\w|\.)+)': No format conversion applied: Raw Flux \((?<fluxCount>\d+) flux in (?<fluxTimeMS>\d+(?:\.?\d+)?)ms\)"
        Public Const REGEX_TRACK_READ_SUMMARY As String = "^T(?<destTrack>\d+)\.(?<destSide>\d+)( <- (?<srcType>\w+) (?<srcTrack>\d+)\.(?<srcSide>\d+))?: (?<details>.+)"
        Public Const REGEX_TRACK_UNEXPECTED As String = "^T(?<track>\d+)\.(?<side>\d+): Ignoring unexpected sector C:(?<cylinder>\d+) H:(?<head>\d+) R:(?<sectorId>\d+) N:(?<sizeId>\d+)"
        Public Const REGEX_TRACK_WRITE As String = "^T(?<srcTrack>\d+)\.(?<srcSide>\d+)( -> Drive (?<destTrack>\d+).(?<destHead>\d+))?: (?<action>Writing|Erasing) Track\b( \((?<details>.+)\))?"
        Public Const REGEX_TRACK_WRITE_FAILED As String = "^Failed to verify Track (?<srcTrack>\d+)\.(?<srcSide>\d+)"
        Public Const REGEX_TRACK_WRITE_RETRY As String = "Retry #(?<retry>\d+)"

        Private ReadOnly RegExCommandFailed As New Regex(REGEX_COMMAND_FAILED, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExDiskRange As New Regex(REGEX_DISK_RANGE, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExSectorGridEnd As New Regex(REGEX_SECTOR_GRID_END, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExSectorGridLine As New Regex(REGEX_SECTOR_GRID_LINE, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExTrackConverting As New Regex(REGEX_TRACK_CONVERTING, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExTrackReadDetails As New Regex(REGEX_TRACK_READ_DETAILS, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExTrackReadFailed As New Regex(REGEX_TRACK_READ_FAILED, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExTrackReadOutOfRange As New Regex(REGEX_TRACK_READ_OUTOFRANGE, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExTrackReadSummary As New Regex(REGEX_TRACK_READ_SUMMARY, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExTrackUnexpected As New Regex(REGEX_TRACK_UNEXPECTED, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExTrackWrite As New Regex(REGEX_TRACK_WRITE, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExTrackWriteFailed As New Regex(REGEX_TRACK_WRITE_FAILED, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Private ReadOnly RegExTrackWriteRetry As New Regex(REGEX_TRACK_WRITE_RETRY, RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant Or RegexOptions.Compiled)
        Public Function ParseCommandFailed(line As String) As (Message As String, Details As String)?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExCommandFailed.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Dim message As String = Match.Groups("message").Value.Trim()
            Dim details As String = Match.Groups("details").Value.Trim()

            Return (message, details)
        End Function

        Public Function ParseDiskRange(line As String) As TrackRange?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExDiskRange.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Return New TrackRange With {
                .Action = Match.Groups("action").Value,
                .TrackStart = GetInt(Match, "trkStart"),
                .TrackEnd = GetIntOrDefault(Match, "trkEnd", .TrackStart),
                .HeadStart = GetInt(Match, "headStart"),
                .HeadEnd = GetIntOrDefault(Match, "headEnd", .HeadStart)
            }
        End Function

        Public Function ParseSectorGridEnd(line As String) As (FoundSectors As Integer, TotalSectors As Integer, Percentage As Integer)?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExSectorGridEnd.Match(line)

            If Not Match.Success Then
                Return Nothing
            End If

            Dim foundSectors As Integer = GetInt(Match, "foundSectors")
            Dim totalSectors As Integer = GetInt(Match, "totalSectors")
            Dim percentage As Integer = GetInt(Match, "percentage")

            Return (foundSectors, totalSectors, percentage)
        End Function

        Public Function ParseSectorGridLine(line As String) As (Side As Integer, Sector As Integer, Tracks As List(Of Boolean))?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExSectorGridLine.Match(line)

            If Not Match.Success Then
                Return Nothing
            End If

            Dim side As Integer = GetInt(Match, "side")
            Dim sector As Integer = GetInt(Match, "sector")
            Dim trackList As String = Match.Groups("tracks").Value.Trim()

            Dim tracks As New List(Of Boolean)
            For i = 0 To trackList.Length - 1
                Dim IsBad = (trackList.Substring(i, 1) = "X")
                tracks.Add(IsBad)
            Next

            Return (side, sector, tracks)
        End Function

        Public Function ParseTrackConverting(line As String) As ConvertingRange?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim m = RegExTrackConverting.Match(line)
            If Not m.Success Then
                Return Nothing
            End If

            Return New ConvertingRange With {
                .SrcCylStart = GetInt(m, "cs"),
                .SrcCylEnd = GetIntOrDefault(m, "ce", GetInt(m, "cs")),
                .SrcHeadStart = GetInt(m, "hs"),
                .SrcHeadEnd = GetIntOrDefault(m, "he", GetInt(m, "hs")),
                .DstCylStart = GetInt(m, "ds"),
                .DstCylEnd = GetIntOrDefault(m, "de", GetInt(m, "ds")),
                .DstHeadStart = GetInt(m, "dhs"),
                .DstHeadEnd = GetIntOrDefault(m, "dhe", GetInt(m, "dhs"))
            }
        End Function

        Public Function ParseTrackReadDetails(line As String) As TrackReadDetails?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExTrackReadDetails.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Return New TrackReadDetails With {
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
        End Function

        Public Function ParseTrackReadFailed(line As String) As Integer?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExTrackReadFailed.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Return GetInt(Match, "sectors")
        End Function

        Public Function ParseTrackReadOutOfRange(line As String) As TrackReadOutOfRange?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExTrackReadOutOfRange.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Return New TrackReadOutOfRange With {
                .Format = Match.Groups("format").Value,
                .FluxCount = GetInt(Match, "fluxCount"),
                .FluxTimeMs = GetDouble(Match, "fluxTimeMS")
            }
        End Function

        Public Function ParseTrackReadSummary(line As String) As TrackReadSummary?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExTrackReadSummary.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Return New TrackReadSummary With {
                .DestTrack = GetInt(Match, "destTrack"),
                .DestSide = GetInt(Match, "destSide"),
                .SrcType = Match.Groups("srcType").Value.Trim(),
                .SrcTrack = GetIntOrDefault(Match, "srcTrack", .DestTrack),
                .SrcSide = GetIntOrDefault(Match, "srcSide", .DestSide),
                .Details = Match.Groups("details").Value.Trim()
            }
        End Function
        Public Function ParseTrackUnexpected(line As String) As UnexpectedSector?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExTrackUnexpected.Match(line)
            If Not Match.Success Then
                Return Nothing
            End If

            Return New UnexpectedSector With {
                .Track = GetInt(Match, "track"),
                .Side = GetInt(Match, "side"),
                .Cylinder = GetInt(Match, "cylinder"),
                .Head = GetInt(Match, "head"),
                .SectorId = GetInt(Match, "sectorId"),
                .SizeId = GetInt(Match, "sizeId")
            }
        End Function

        Public Function ParseTrackWrite(line As String) As TrackWrite?
            If String.IsNullOrWhiteSpace(line) Then
                Return Nothing
            End If

            Dim Match = RegExTrackWrite.Match(line)
            If Match.Success Then
                Dim info As New TrackWrite With {
                    .Action = Match.Groups("action").Value,
                    .SrcTrack = GetInt(Match, "srcTrack"),
                    .SrcSide = GetInt(Match, "srcSide")
                }

                info.DestTrack = GetIntOrDefault(Match, "destTrack", info.SrcTrack)
                info.DestSide = GetIntOrDefault(Match, "destSide", info.SrcSide)

                Dim details = Match.Groups("details").Value
                Dim RetryMatch = RegExTrackWriteRetry.Match(details)
                If RetryMatch.Success Then
                    info.Retry = GetInt(RetryMatch, "retry")
                End If

                Return info
            End If

            Dim MatchFailed = RegExTrackWriteFailed.Match(line)
            If MatchFailed.Success Then
                Dim info As New TrackWrite With {
                    .SrcTrack = GetInt(MatchFailed, "srcTrack"),
                    .SrcSide = GetInt(MatchFailed, "srcSide"),
                    .Failed = True
                }

                Return info
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

        Public Structure ConvertingRange
            Public Property DstCylEnd As Integer
            Public Property DstCylStart As Integer
            Public Property DstHeadEnd As Integer
            Public Property DstHeadStart As Integer
            Public Property SrcCylEnd As Integer
            Public Property SrcCylStart As Integer
            Public Property SrcHeadEnd As Integer
            Public Property SrcHeadStart As Integer
        End Structure

        Public Structure TrackRange
            Public Property Action As String
            Public Property HeadEnd As Integer
            Public Property HeadStart As Integer
            Public Property TrackEnd As Integer
            Public Property TrackStart As Integer
        End Structure

        Public Structure TrackReadDetails
            Public ReadOnly Property BadSectors As Integer
                Get
                    Return _SectorsTotal - _SectorsFound
                End Get
            End Property

            Public Property Encoding As String
            Public Property FluxCount As Integer
            Public Property FluxTimeMs As Double
            Public Property Retry As Integer
            Public Property SectorsFound As Integer
            Public Property SectorsTotal As Integer
            Public Property Seek As Integer
            Public Property SrcFormat As String
            Public Property System As String
        End Structure

        Public Structure TrackReadOutOfRange
            Public Property FluxCount As Integer
            Public Property FluxTimeMs As Double
            Public Property Format As String
        End Structure

        Public Structure TrackReadSummary
            Public Property DestSide As Integer
            Public Property DestTrack As Integer
            Public Property Details As String
            Public ReadOnly Property IsRemapped As Boolean
                Get
                    Return _SrcTrack <> _DestTrack OrElse _SrcSide <> _DestSide
                End Get
            End Property

            Public Property SrcSide As Integer
            Public Property SrcTrack As Integer
            Public Property SrcType As String
        End Structure

        Public Structure TrackWrite
            Public Property Action As String
            Public Property DestSide As Integer
            Public Property DestTrack As Integer
            Public Property Failed As Boolean
            Public Property Retry As Integer
            Public Property SrcSide As Integer
            Public Property SrcTrack As Integer
        End Structure

        Public Structure UnexpectedSector
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
        End Structure
    End Class
End Namespace