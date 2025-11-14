Namespace Greaseweazle
    Class CommandLineBuilder
        Private Const DEFAULT_BITRATE As UInteger = 0
        Private Const DEFAULT_DRIVE As String = "A"
        Private Const DEFAULT_HEADS As TrackHeads = TrackHeads.both
        Private Const DEFAULT_NR As UInteger = 1
        Private Const DEFAULT_STEP As Byte = 1
        Public Const DEFAULT_RETRIES As UInteger = 3
        Private ReadOnly _Action As CommandAction
        Private ReadOnly _Cylinders As List(Of (UShort, UShort))

        Public Enum CommandAction
            info
            read
            write
            convert
            [erase]
            clean
            seek
            delays
            update
            pin
            reset
            bandwidth
            rpm
        End Enum

        Public Enum TrackHeads
            head0
            head1
            both
        End Enum

        Public Sub New(Action As CommandAction)
            _Action = Action
            _Cylinders = New List(Of (UShort, UShort))
        End Sub

        Public Property AdjustSpeed As String
        Public Property BitRate As UInteger = DEFAULT_BITRATE
        Public Property Device As String
        Public Property Drive As String = DEFAULT_DRIVE
        Public Property Format As String
        Public Property Heads As TrackHeads = DEFAULT_HEADS
        Public Property HeadStep As Byte = DEFAULT_STEP
        Public Property InFile As String
        Public Property NR As UInteger = DEFAULT_NR
        Public Property OutFile As String
        Public Property Time As Boolean = False
        Public Property Retries As UInteger = DEFAULT_RETRIES
        Public Property PreErase As Boolean = False
        Public Property EraseEmpty As Boolean = False
        Public Property NoVerify As Boolean = False


        Public Sub AddCylinder(Cylinder As UShort)
            _Cylinders.Add((Cylinder, Cylinder))
        End Sub

        Public Sub AddCylinder(CylinderStart As UShort, CylinderEnd As UShort)
            _Cylinders.Add((CylinderStart, CylinderEnd))
        End Sub

        Public Function Arguments() As String
            Dim args = New List(Of String)
            If _Time Then
                args.Add("--time")
            End If

            args.Add(GetActionName(_Action))

            If CheckOptionDevice() AndAlso Not String.IsNullOrEmpty(_Device) Then
                args.Add("--device " & _Device)
            End If

            If CheckOptionDrive() AndAlso _Drive <> DEFAULT_DRIVE Then
                args.Add("--drive " & _Drive)
            End If

            If CheckOptionFormat() AndAlso Not String.IsNullOrEmpty(_Format) Then
                args.Add("--format " & _Format)
            End If

            If CheckOptionNR() AndAlso _NR <> DEFAULT_NR Then
                args.Add("--nr " & _NR)
            End If

            If CheckOptionRetries() AndAlso _Retries <> DEFAULT_RETRIES Then
                args.Add("--retries " & _Retries)
            End If

            If CheckOptionTracks() Then
                Dim TrackString = GetTrackString()
                If Not String.IsNullOrEmpty(TrackString) Then
                    args.Add("--tracks " & Quoted(TrackString))
                End If
            End If

            If CheckOptionPreErase() AndAlso _PreErase Then
                args.Add("--pre-erase")
            End If

            If CheckOptionEraseEmpty() AndAlso _EraseEmpty Then
                args.Add("--erase-empty")
            End If

            If CheckOptionNoVerify() AndAlso _NoVerify Then
                args.Add("--no-verify")
            End If

            If CheckOptionAdjustSpeed() AndAlso Not String.IsNullOrEmpty(_AdjustSpeed) Then
                args.Add("--adjust-speed " & _AdjustSpeed)
            End If

            If CheckOptionInFile() AndAlso Not String.IsNullOrEmpty(_InFile) Then
                args.Add(Quoted(_InFile))

                If CheckOptionOutFile() AndAlso Not String.IsNullOrEmpty(_OutFile) Then
                    Dim FilePath = Quoted(_OutFile)
                    If _BitRate <> DEFAULT_BITRATE Then
                        FilePath &= "::bitrate=" & _BitRate
                    End If
                    args.Add(FilePath)
                End If
            End If

            Return String.Join(" ", args)
        End Function

        Private Function CheckOptionAdjustSpeed() As Boolean
            Select Case _Action
                Case CommandAction.read, CommandAction.write, CommandAction.convert
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function CheckOptionDevice() As Boolean
            Select Case _Action
                Case CommandAction.convert
                    Return False
                Case Else
                    Return True
            End Select
        End Function

        Private Function CheckOptionDrive() As Boolean
            Select Case _Action
                Case CommandAction.read, CommandAction.write, CommandAction.erase, CommandAction.clean, CommandAction.seek, CommandAction.rpm
                    Return True
                Case Else
                    Return False
            End Select
        End Function
        Private Function CheckOptionFormat() As Boolean
            Select Case _Action
                Case CommandAction.read, CommandAction.write, CommandAction.convert
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function CheckOptionInFile() As Boolean
            Select Case _Action
                Case CommandAction.read, CommandAction.write, CommandAction.convert
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function CheckOptionNR() As Boolean
            Select Case _Action
                Case CommandAction.rpm
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function CheckOptionOutFile() As Boolean
            Select Case _Action
                Case CommandAction.convert
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function CheckOptionRetries() As Boolean
            Select Case _Action
                Case CommandAction.read, CommandAction.write
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function CheckOptionPreErase() As Boolean
            Select Case _Action
                Case CommandAction.write
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function CheckOptionEraseEmpty() As Boolean
            Select Case _Action
                Case CommandAction.write
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function CheckOptionNoVerify() As Boolean
            Select Case _Action
                Case CommandAction.write
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function CheckOptionTracks() As Boolean
            Select Case _Action
                Case CommandAction.read, CommandAction.write, CommandAction.convert
                    Return True
                Case Else
                    Return False
            End Select
        End Function

        Private Function GetActionName(Action As CommandAction) As String
            Select Case Action
                Case CommandAction.info
                    Return "info"
                Case CommandAction.read
                    Return "read"
                Case CommandAction.write
                    Return "write"
                Case CommandAction.convert
                    Return "convert"
                Case CommandAction.erase
                    Return "erase"
                Case CommandAction.clean
                    Return "clean"
                Case CommandAction.seek
                    Return "seek"
                Case CommandAction.delays
                    Return "delays"
                Case CommandAction.update
                    Return "update"
                Case CommandAction.pin
                    Return "pin"
                Case CommandAction.reset
                    Return "reset"
                Case CommandAction.bandwidth
                    Return "bandwidth"
                Case CommandAction.rpm
                    Return "rpm"
                Case Else
                    Return ""
            End Select
        End Function

        Private Function GetTrackString() As String
            Dim Tracks = New List(Of String)
            Dim CylinderList = New List(Of String)

            For Each Value In _Cylinders
                Dim Range As String = Value.Item1
                If Value.Item2 <> Value.Item1 Then
                    Range &= "-" & Value.Item2
                End If
                CylinderList.Add(Range)
            Next

            If CylinderList.Count > 0 Then
                Tracks.Add("c=" & String.Join(",", CylinderList))
            End If

            If CylinderList.Count > 0 OrElse _Heads <> DEFAULT_HEADS Then
                Dim HeadList As String
                If _Heads = TrackHeads.head0 Then
                    HeadList = "0"
                ElseIf _Heads = TrackHeads.head1 Then
                    HeadList = "1"
                Else
                    HeadList = "0-1"
                End If
                Tracks.Add("h=" & HeadList)
            End If

            If _HeadStep <> DEFAULT_STEP Then
                Tracks.Add("step=" & _HeadStep)
            End If

            Return String.Join(":", Tracks)
        End Function
    End Class
End Namespace
