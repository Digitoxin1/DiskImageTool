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

        Private _Type As FloppyDriveType = FloppyDriveType.DriveUnknown

        Public ReadOnly Property Tracks As Byte
            Get
                Return _tracks
            End Get
        End Property

        Public ReadOnly Property Type As FloppyDriveType
            Get
                Return _Type
            End Get
        End Property

        Public Shared Function GetMinMax(Type As FloppyDriveType) As (Min As Byte, Max As Byte)
            Dim Result As (Min As Byte, Max As Byte)

            If Type = FloppyDriveType.DriveUnknown Then
                Return (0, 0)
            ElseIf Type = FloppyDriveType.Drive525DoubleDensity Then
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

            _Type = GetDriveTypeFromName(ReadValue(dict, "typeName", GetDriveTypeName(_Type)))
            Dim Tracks As Integer = ReadValue(dict, "tracks", _tracks)
            _tracks = AdjustedTrackCount(Tracks, _Type)

            MarkClean()
        End Sub

        Public Sub SetDrive(Type As FloppyDriveType, Optional TrackCount As Byte = 0)
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
                {"typeName", GetDriveTypeName(_Type)},
                {"tracks", CInt(_tracks)}
            }
        End Function

        Private Shared Function AdjustedTrackCount(Value As Integer, DriveType As FloppyDriveType) As Byte
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

        Private Shared Function GetDriveTypeFromName(Value As String) As FloppyDriveType
            Select Case Value
                Case "360"
                    Return FloppyDriveType.Drive525DoubleDensity
                Case "720"
                    Return FloppyDriveType.Drive35DoubleDensity
                Case "1200"
                    Return FloppyDriveType.Drive525HighDensity
                Case "1440"
                    Return FloppyDriveType.Drive35HighDensity
                Case "2880"
                    Return FloppyDriveType.Drive35ExtraHighDensity
                Case Else
                    Return FloppyDriveType.DriveUnknown
            End Select
        End Function

        Private Shared Function GetDriveTypeName(Value As FloppyDriveType) As String
            Select Case Value
                Case FloppyDriveType.Drive525DoubleDensity
                    Return "360"
                Case FloppyDriveType.Drive35DoubleDensity
                    Return "720"
                Case FloppyDriveType.Drive525HighDensity
                    Return "1200"
                Case FloppyDriveType.Drive35HighDensity
                    Return "1440"
                Case FloppyDriveType.Drive35ExtraHighDensity
                    Return "2880"
                Case Else
                    Return ""
            End Select
        End Function
    End Class
End Namespace