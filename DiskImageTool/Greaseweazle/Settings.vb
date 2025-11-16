Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Namespace Greaseweazle
    Public Class Settings
        Public Const MAX_TRACKS As Byte = 84
        Public Const MAX_TRACKS_525DD As Byte = 42
        Public Const MIN_TRACKS As Byte = 80
        Public Const MIN_TRACKS_525DD As Byte = 40

        Public Enum GreaseweazleInterface
            IBM
            Shugart
        End Enum

        Public Property [Interface] As GreaseweazleInterface
            Get
                Return GetInterfaceFromName(My.Settings.GW_Interface)
            End Get

            Set(value As GreaseweazleInterface)
                My.Settings.GW_Interface = GetInterfaceName(value)
            End Set
        End Property

        Public Property AppPath As String
            Get
                Return My.Settings.GW_Path
            End Get
            Set(value As String)
                My.Settings.GW_Path = value
            End Set
        End Property

        Public ReadOnly Property AvailableDriveTypes As FloppyMediaType
            Get
                Dim AvailableTypes As FloppyMediaType = GetDriveType(0) Or GetDriveType(1)
                If [Interface] = GreaseweazleInterface.Shugart Then
                    AvailableTypes = AvailableTypes Or GetDriveType(2)
                End If

                Return AvailableTypes
            End Get
        End Property

        Public Property COMPort As String
            Get
                Return My.Settings.GW_COMPort
            End Get
            Set(value As String)
                My.Settings.GW_COMPort = value
            End Set
        End Property

        Public Property DefaultRevs As Byte
            Get
                Dim value As Byte = My.Settings.GW_DefaultRevs
                If value < CommandLineBuilder.MIN_REVS Then
                    value = CommandLineBuilder.MIN_REVS
                ElseIf value > CommandLineBuilder.MAX_REVS Then
                    value = CommandLineBuilder.MAX_REVS
                End If
                Return value
            End Get
            Set(value As Byte)
                If value < CommandLineBuilder.MIN_REVS Then
                    value = CommandLineBuilder.MIN_REVS
                ElseIf value > CommandLineBuilder.MAX_REVS Then
                    value = CommandLineBuilder.MAX_REVS
                End If
                My.Settings.GW_DefaultRevs = value
            End Set
        End Property

        Public Property LogFileName As String
            Get
                Return My.Settings.GW_LogFileName
            End Get
            Set(value As String)
                My.Settings.GW_LogFileName = value
            End Set
        End Property
        Public ReadOnly Property DriveType(index As Byte) As FloppyMediaType
            Get
                Return GetDriveType(index)
            End Get
        End Property

        Public ReadOnly Property TrackCount(index As Byte) As Byte
            Get
                Return AdjustedTrackCount(GetTrackCount(index), GetDriveType(index))
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

        Public Shared Function IsPathValid(Path As String) As Boolean
            Dim IsValid As Boolean

            If Path = "" Then
                IsValid = False
            ElseIf Not IsExecutable(Path) Then
                IsValid = False
            Else
                IsValid = True
            End If

            Return IsValid
        End Function

        Public Function IsPathValid() As Boolean
            Return IsPathValid(AppPath)
        End Function

        Public Sub SetDrive(index As Byte, Type As FloppyMediaType, Optional TrackCount As Byte = 0)
            Dim TypeName = GetFloppyTypeName(Type)
            Dim AdjustedCount = AdjustedTrackCount(TrackCount, Type)

            Select Case index
                Case 0
                    My.Settings.GW_DriveType0 = TypeName
                    My.Settings.GW_Tracks0 = AdjustedCount
                Case 1
                    My.Settings.GW_DriveType1 = TypeName
                    My.Settings.GW_Tracks1 = AdjustedCount
                Case Else
                    My.Settings.GW_DriveType2 = TypeName
                    My.Settings.GW_Tracks2 = AdjustedCount
            End Select
        End Sub

        Private Shared Function AdjustedTrackCount(Value As Byte, DriveType As FloppyMediaType) As Byte
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

        Private Shared Function GetDriveType(Index As Byte) As FloppyMediaType
            Select Case Index
                Case 0
                    Return GetFloppyTypeFromName(My.Settings.GW_DriveType0)
                Case 1
                    Return GetFloppyTypeFromName(My.Settings.GW_DriveType1)
                Case Else
                    Return GetFloppyTypeFromName(My.Settings.GW_DriveType2)
            End Select
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

        Private Shared Function GetInterfaceFromName(Value As String) As GreaseweazleInterface
            Select Case Value
                Case "Shugart"
                    Return GreaseweazleInterface.Shugart
                Case Else
                    Return GreaseweazleInterface.IBM
            End Select
        End Function

        Private Shared Function GetInterfaceName(Value As GreaseweazleInterface) As String
            Select Case Value
                Case GreaseweazleInterface.Shugart
                    Return "Shugart"
                Case Else
                    Return "IBM"
            End Select
        End Function

        Private Shared Function GetTrackCount(Index As Byte) As Byte
            Select Case Index
                Case 0
                    Return My.Settings.GW_Tracks0
                Case 1
                    Return My.Settings.GW_Tracks1
                Case Else
                    Return My.Settings.GW_Tracks2
            End Select
        End Function

        Private Shared Function IsExecutable(path As String) As Boolean
            Return Not String.IsNullOrWhiteSpace(path) AndAlso
                IO.File.Exists(path) AndAlso
                IO.Path.GetExtension(path).Equals(".exe", StringComparison.OrdinalIgnoreCase)
        End Function
    End Class
End Namespace