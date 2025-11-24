Imports CompactJson
Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Flux.Greaseweazle
    Public Class DriveSettings
        Inherits Settings.SettingsGroup

        Public Const MAX_TRACKS As Byte = 84
        Public Const MAX_TRACKS_525DD As Byte = 42
        Public Const MIN_TRACKS As Byte = 80
        Public Const MIN_TRACKS_525DD As Byte = 40

        Private _tracks As Byte = 0

        Private _Type As FloppyMediaType = FloppyMediaType.MediaUnknown

        Public ReadOnly Property Tracks As Byte
            Get
                Return _tracks
            End Get
        End Property

        Public ReadOnly Property Type As FloppyMediaType
            Get
                Return _Type
            End Get
        End Property

        Public Shared Function GetMinMax(Type As FloppyMediaType) As (Min As Byte, Max As Byte)
            Dim Result As (Min As Byte, Max As Byte)

            If Type = FloppyMediaType.MediaUnknown Then
                Return (0, 0)
            ElseIf Type = FloppyMediaType.Media525DoubleDensity Then
                Return (MIN_TRACKS_525DD, MAX_TRACKS_525DD)
            Else
                Return (MIN_TRACKS, MAX_TRACKS)
            End If

            Return Result
        End Function

        Public Sub LoadFromDictionary(dict As Dictionary(Of String, JsonValue))
            If dict Is Nothing Then
                MarkClean()
                Return
            End If

            _Type = GetFloppyTypeFromName(ReadValue(dict, "typeName", GetFloppyTypeName(_Type)))
            Dim Tracks As Integer = ReadValue(dict, "tracks", _tracks)
            _tracks = AdjustedTrackCount(Tracks, _Type)

            MarkClean()
        End Sub

        Public Sub SetDrive(Type As FloppyMediaType, Optional TrackCount As Byte = 0)
            Dim Tracks = AdjustedTrackCount(TrackCount, Type)

            If _Type <> Type Then
                _Type = Type
                MarkDirty()
            End If

            If _tracks <> Tracks Then
                _tracks = Tracks
                MarkDirty()
            End If
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"typeName", GetFloppyTypeName(_Type)},
                {"tracks", CInt(_tracks)}
            }
        End Function

        Private Shared Function AdjustedTrackCount(Value As Integer, DriveType As FloppyMediaType) As Byte
            Dim MinMax = GetMinMax(DriveType)

            If Value = 0 Then
                Value = MinMax.Max
            ElseIf Value < MinMax.Min Then
                Value = MinMax.Min
            ElseIf Value > MinMax.Max Then
                Value = MinMax.Max
            End If

            Return Value
        End Function

        Private Shared Function GetFloppyTypeFromName(Value As String) As FloppyMediaType
            Select Case Value
                Case "360"
                    Return FloppyMediaType.Media525DoubleDensity
                Case "720"
                    Return FloppyMediaType.Media35DoubleDensity
                Case "1200"
                    Return FloppyMediaType.Media525HighDensity
                Case "1440"
                    Return FloppyMediaType.Media35HighDensity
                Case "2880"
                    Return FloppyMediaType.Media35ExtraHighDensity
                Case Else
                    Return FloppyMediaType.MediaUnknown
            End Select
        End Function

        Private Shared Function GetFloppyTypeName(Value As FloppyMediaType) As String
            Select Case Value
                Case FloppyMediaType.Media525DoubleDensity
                    Return "360"
                Case FloppyMediaType.Media35DoubleDensity
                    Return "720"
                Case FloppyMediaType.Media525HighDensity
                    Return "1200"
                Case FloppyMediaType.Media35HighDensity
                    Return "1440"
                Case FloppyMediaType.Media35ExtraHighDensity
                    Return "2880"
                Case Else
                    Return ""
            End Select
        End Function
    End Class
End Namespace