Imports DiskImageTool.DiskImage
Imports System.ComponentModel

Public Class FloppyAccessForm
    Private Const BYTES_PER_SECTOR As UShort = 512
    Private ReadOnly _AccessType As FloppyAccessType
    Private ReadOnly _BPB As BiosParameterBlock
    Private ReadOnly _FloppyDrive As FloppyInterface
    Private _Activated As Boolean = False
    Private _Complete As Boolean = False
    Private _DiskBuffer() As Byte = Nothing
    Private _DoFormat As Boolean
    Private _DoVerify As Boolean
    Private _EndProcess As Boolean = False
    Private _LastStatus As TrackStatus
    Private _SectorData As SectorData = Nothing
    Private _SectorError As Boolean = False
    Private _TotalBadSectors As UInteger = 0
    Private _LoadedFileNames As New Dictionary(Of String, ImageData)
    Private _FileName As String = ""

    Public Enum FloppyAccessType
        Read
        Write
    End Enum

    Private Enum TrackStatus
        Reading
        Writing
        Verifying
        Formatting
        Success
        Failure
    End Enum

    Public Sub New(FloppyDrive As FloppyInterface, BPB As BiosParameterBlock, AccessType As FloppyAccessType)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call
        TableSide0.GetType() _
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic) _
            .SetValue(TableSide0, True, Nothing)
        TableSide1.GetType() _
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic) _
            .SetValue(TableSide1, True, Nothing)

        _AccessType = AccessType
        _FloppyDrive = FloppyDrive
        _BPB = BPB
        ReDim _DiskBuffer(_BPB.ImageSize - 1)
        Dim StatusTypeString = IIf(AccessType = FloppyAccessType.Read, "Reading", "Writing")

        Me.Text = StatusTypeString & " " & GetFloppyDiskFormatName(_BPB, False) & " Floppy"
        StatusType.Text = StatusTypeString

        btnAbort.Text = "Abort"

        InitTables()
    End Sub

    Public ReadOnly Property Complete As Boolean
        Get
            Return _Complete
        End Get
    End Property

    Public Property DiskBuffer As Byte()
        Get
            Return _DiskBuffer
        End Get
        Set(value As Byte())
            _DiskBuffer = value
        End Set
    End Property

    Public Property DoFormat As Boolean
        Get
            Return _DoFormat
        End Get
        Set(value As Boolean)
            _DoFormat = value
        End Set
    End Property

    Public Property DoVerify As Boolean
        Get
            Return _DoVerify
        End Get
        Set(value As Boolean)
            _DoVerify = value
        End Set
    End Property

    Public ReadOnly Property FileName As String
        Get
            Return _FileName
        End Get
    End Property

    Public Property LoadedFileNames As Dictionary(Of String, ImageData)
        Get
            Return _LoadedFileNames
        End Get
        Set(value As Dictionary(Of String, ImageData))
            _LoadedFileNames = value
        End Set
    End Property

    Public ReadOnly Property TotalBadSectors As UInteger
        Get
            Return _TotalBadSectors
        End Get
    End Property

    Private Function AddLabel(Text As String, BackColor As Color) As Label
        Dim objLabel As New Label With {
            .BorderStyle = BorderStyle.None,
            .Margin = New Padding(0),
            .TextAlign = ContentAlignment.MiddleCenter,
            .UseMnemonic = False,
            .AutoSize = False,
            .Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom,
            .BackColor = BackColor,
            .Text = Text
        }

        Return objLabel
    End Function

    Private Sub ClearFlags()
        _Complete = False
        _EndProcess = False
        _LastStatus = TrackStatus.Reading
        _SectorError = False
    End Sub

    Private Function ConfirmAbort() As Boolean
        Dim Msg = "Are you sure you wish to abort?"
        Dim MsgBoxResult = MsgBox(Msg, MsgBoxStyle.YesNo Or MsgBoxStyle.DefaultButton2)

        If MsgBoxResult = MsgBoxResult.No Then
            BackgroundWorker1.RunWorkerAsync()
            Return False
        Else
            Return True
        End If
    End Function

    Private Function FloppyDiskFormatTrack(bw As BackgroundWorker, TrackInfo As TrackInfo, SectorData As SectorData) As Boolean
        Dim Result As Boolean

        TrackInfo.Status = TrackStatus.Formatting
        _LastStatus = TrackInfo.Status
        bw.ReportProgress(0, TrackInfo)
        Result = FormatTrack(SectorData.SectorStart)

        If Not Result Then
            TrackInfo.Status = TrackStatus.Failure
            TrackInfo.BadSectors = 0
            bw.ReportProgress(0, TrackInfo)
            _SectorError = True
            _SectorData = SectorData
        End If

        Return Result
    End Function

    Private Sub FloppyDiskRead(bw As BackgroundWorker)
        Dim TrackInfo As TrackInfo
        Dim SectorData As SectorData
        Dim SectorStart As UInteger

        ClearFlags()

        If _SectorData Is Nothing Then
            SectorStart = 0
            SectorData = Nothing
        Else
            SectorStart = _SectorData.SectorStart
            SectorData = _SectorData
        End If

        For Sector As UInteger = SectorStart To _BPB.SectorCount - 1 Step _BPB.SectorsPerTrack
            TrackInfo = New TrackInfo(Sector)
            If SectorData Is Nothing Then
                SectorData = New SectorData(Sector, _BPB.SectorsPerTrack)
            End If
            If bw.CancellationPending Then
                _SectorData = SectorData
                Exit Sub
            End If

            _LastStatus = TrackInfo.Status
            bw.ReportProgress(0, TrackInfo)
            If SectorData.SectorsProcessed = 0 Then
                ReadSectorsByTrack(_DiskBuffer, SectorData)
            End If
            If SectorData.SectorsProcessed < SectorData.SectorCount Then
                ReadSectorsBySector(_DiskBuffer, SectorData)
            End If
            If SectorData.SectorCount = SectorData.SectorsProcessed Then
                TrackInfo.Status = TrackStatus.Success
                TrackInfo.BadSectors = 0
            Else
                TrackInfo.Status = TrackStatus.Failure
                TrackInfo.BadSectors = SectorData.SectorCount - SectorData.SectorsProcessed
                _SectorError = True
            End If
            bw.ReportProgress(0, TrackInfo)

            If _SectorError Then
                _SectorData = SectorData
                Exit Sub
            End If

            SectorData = Nothing
        Next

        _Complete = True
    End Sub

    Private Function FloppyDiskVerifyTrack(bw As BackgroundWorker, TrackInfo As TrackInfo, SectorData As SectorData) As Boolean
        Dim Result As Boolean

        TrackInfo.Status = TrackStatus.Verifying
        _LastStatus = TrackInfo.Status
        bw.ReportProgress(0, TrackInfo)
        TrackInfo.BytesRead = ReadSectorsByTrack(Nothing, SectorData)

        If SectorData.SectorCount <> SectorData.SectorsProcessed Then
            TrackInfo.BadSectors = SectorData.SectorCount - SectorData.SectorsProcessed
            Result = False
        ElseIf Not TrackInfo.BytesWritten.CompareTo(TrackInfo.BytesRead) Then
            TrackInfo.BadSectors = 0
            Result = False
        Else
            Result = True
        End If

        If Not Result Then
            TrackInfo.Status = TrackStatus.Failure
            bw.ReportProgress(0, TrackInfo)
            _SectorError = True
            _SectorData = SectorData
        End If

        Return Result
    End Function

    Private Sub FloppyDiskWrite(bw As BackgroundWorker, ByVal Format As Boolean, ByVal Verify As Boolean)
        Dim TrackInfo As TrackInfo
        Dim SectorData As SectorData
        Dim SectorStart As UInteger

        ClearFlags()

        If _SectorData Is Nothing Then
            SectorStart = 0
            SectorData = Nothing
        Else
            SectorStart = _SectorData.SectorStart
            SectorData = _SectorData
        End If

        For Sector As UInteger = SectorStart To _BPB.SectorCount - 1 Step _BPB.SectorsPerTrack
            TrackInfo = New TrackInfo(Sector)
            If SectorData Is Nothing Then
                SectorData = New SectorData(Sector, _BPB.SectorsPerTrack)
            End If
            If bw.CancellationPending Then
                _SectorData = SectorData
                Exit Sub
            End If

            If Format Then
                If Not FloppyDiskFormatTrack(bw, TrackInfo, SectorData) Then
                    Exit Sub
                End If
            End If

            If Not FloppyDiskWriteTrack(bw, TrackInfo, SectorData) Then
                Exit Sub
            End If

            If Verify Then
                If Not FloppyDiskVerifyTrack(bw, TrackInfo, SectorData) Then
                    Exit Sub
                End If
            End If

            TrackInfo.Status = TrackStatus.Success
            bw.ReportProgress(0, TrackInfo)

            SectorData = Nothing
        Next

        _Complete = True
    End Sub

    Private Function FloppyDiskWriteTrack(bw As BackgroundWorker, TrackInfo As TrackInfo, SectorData As SectorData) As Boolean
        Dim Result As Boolean

        TrackInfo.Status = TrackStatus.Writing
        _LastStatus = TrackInfo.Status
        bw.ReportProgress(0, TrackInfo)
        TrackInfo.BytesWritten = WriteSectorsByTrack(_DiskBuffer, SectorData)
        Result = (SectorData.SectorCount = SectorData.SectorsProcessed)

        If Not Result Then
            TrackInfo.Status = TrackStatus.Failure
            TrackInfo.BadSectors = SectorData.SectorCount - SectorData.SectorsProcessed
            bw.ReportProgress(0, TrackInfo)
            _SectorError = True
            _SectorData = SectorData
        End If

        Return Result
    End Function

    Private Function FormatTrack(Sector As UInteger) As Boolean
        Dim Track = _BPB.SectorToTrack(Sector)
        Dim Side = _BPB.SectorToSide(Sector)
        Dim MediaType As FloppyInterface.MEDIA_TYPE = GetMediaTypeFromDiskType(GetFloppyDiskFormat(_BPB, False))

        If MediaType = FloppyInterface.MEDIA_TYPE.Unknown Then
            Return False
        Else
            Return _FloppyDrive.FormatTrack(MediaType, Track, Side)
        End If
    End Function

    Private Function GetMediaTypeFromDiskType(DiskFormat As FloppyDiskFormat) As FloppyInterface.MEDIA_TYPE
        Select Case DiskFormat
            Case FloppyDiskFormat.Floppy160
                Return FloppyInterface.MEDIA_TYPE.F5_160_512
            Case FloppyDiskFormat.Floppy180
                Return FloppyInterface.MEDIA_TYPE.F5_180_512
            Case FloppyDiskFormat.Floppy320
                Return FloppyInterface.MEDIA_TYPE.F5_320_512
            Case FloppyDiskFormat.Floppy360
                Return FloppyInterface.MEDIA_TYPE.F5_360_512
            Case FloppyDiskFormat.Floppy720
                Return FloppyInterface.MEDIA_TYPE.F3_720_512
            Case FloppyDiskFormat.Floppy1200
                Return FloppyInterface.MEDIA_TYPE.F5_1Pt2_512
            Case FloppyDiskFormat.Floppy1440
                Return FloppyInterface.MEDIA_TYPE.F3_1Pt44_512
            Case FloppyDiskFormat.Floppy2880
                Return FloppyInterface.MEDIA_TYPE.F3_2Pt88_512
            Case FloppyDiskFormat.FloppyProCopy
                Return FloppyInterface.MEDIA_TYPE.F3_1Pt44_512
            Case Else
                Return FloppyInterface.MEDIA_TYPE.Unknown
        End Select
    End Function

    Private Function GetStatusColor(Status As TrackStatus) As Color
        Select Case Status
            Case TrackStatus.Reading
                Return Color.Blue
            Case TrackStatus.Writing
                Return Color.Blue
            Case TrackStatus.Verifying
                Return Color.Purple
            Case TrackStatus.Formatting
                Return Color.Yellow
            Case TrackStatus.Success
                Return Color.LightGreen
            Case TrackStatus.Failure
                Return Color.Red
            Case Else
                Return Color.Red
        End Select
    End Function

    Private Function GetStatusText(Status As TrackStatus) As String
        Select Case Status
            Case TrackStatus.Reading
                Return "Reading"
            Case TrackStatus.Writing
                Return "Writing"
            Case TrackStatus.Verifying
                Return "Verifying"
            Case TrackStatus.Formatting
                Return "Formatting"
            Case Else
                Return ""
        End Select
    End Function
    Private Function HandleError() As Boolean
        Dim Track = _BPB.SectorToTrack(_SectorData.SectorStart)
        Dim Side = _BPB.SectorToSide(_SectorData.SectorStart)
        Dim BadSectors = _SectorData.SectorCount - _SectorData.SectorsProcessed
        Dim Msg As String
        If _AccessType = FloppyAccessType.Read Then
            Msg = "Error Reading " & BadSectors & " Sector".Pluralize(BadSectors) & " in Track " & Track & ", Side " & Side & "."
        Else
            Msg = "Error " & GetStatusText(_LastStatus) & " Track " & Track & ", Side " & Side & "."
        End If
        Dim MsgBoxResult = MsgBox(Msg, MsgBoxStyle.AbortRetryIgnore Or MsgBoxStyle.DefaultButton2)

        If MsgBoxResult = MsgBoxResult.Abort Then
            _TotalBadSectors += BadSectors
            StatusBadSectors.Text = _TotalBadSectors & " Bad Sector".Pluralize(_TotalBadSectors)
            Return False
        ElseIf MsgBoxResult = MsgBoxResult.Ignore Then
            _TotalBadSectors += BadSectors
            StatusBadSectors.Text = _TotalBadSectors & " Bad Sector".Pluralize(_TotalBadSectors)
            Dim SectorStart = _SectorData.SectorStart + _BPB.SectorsPerTrack
            If SectorStart < _BPB.SectorCount Then
                _SectorData = New SectorData(SectorStart, _BPB.SectorsPerTrack)
                BackgroundWorker1.RunWorkerAsync()
                Return True
            Else
                _Complete = True
                Return False
            End If
        Else
            BackgroundWorker1.RunWorkerAsync()
            Return True
        End If
    End Function

    Private Sub InitTables()
        Dim objLabel As Label
        Dim Tracks = _BPB.SectorToTrack(_BPB.SectorCount)
        Dim TrackColor As Color

        For Col = 1 To 10
            objLabel = AddLabel(Col - 1, SystemColors.Control)
            TableSide0Outer.Controls.Add(objLabel, Col, 0)

            objLabel = AddLabel(Col - 1, SystemColors.Control)
            TableSide1Outer.Controls.Add(objLabel, Col, 0)
        Next
        For Row = 1 To 8
            objLabel = AddLabel(Row - 1, SystemColors.Control)
            TableSide0Outer.Controls.Add(objLabel, 0, Row)

            objLabel = AddLabel(Row - 1, SystemColors.Control)
            TableSide1Outer.Controls.Add(objLabel, 0, Row)
        Next
        For Row = 0 To TableSide0.RowCount - 1
            For Col = 0 To TableSide0.ColumnCount - 1
                Dim CurrentTrack As UShort = Row * 10 + Col

                If CurrentTrack < Tracks Then
                    TrackColor = Color.White
                Else
                    TrackColor = Color.LightGray
                End If
                objLabel = AddLabel("", TrackColor)
                TableSide0.Controls.Add(objLabel, Col, Row)

                If CurrentTrack < Tracks And _BPB.NumberOfHeads > 1 Then
                    TrackColor = Color.White
                Else
                    TrackColor = Color.LightGray
                End If
                objLabel = AddLabel("", TrackColor)
                TableSide1.Controls.Add(objLabel, Col, Row)
            Next
        Next
    End Sub
    Private Sub ReadSectorsBySector(DiskBuffer() As Byte, SectorData As SectorData)
        Dim BufferSize = BYTES_PER_SECTOR
        Dim Buffer(BufferSize - 1) As Byte
        For Count = 0 To SectorData.SectorCount - 1
            If Not SectorData.SectorStatus(Count) Then
                Dim Sector = SectorData.SectorStart + Count
                Dim BytesRead = _FloppyDrive.ReadSector(Sector, Buffer)
                If BytesRead = Buffer.Length Then
                    Buffer.CopyTo(DiskBuffer, SectorToBytes(Sector))
                    SectorData.SectorsProcessed += 1
                    SectorData.SectorStatus(Count) = True
                End If
            End If
        Next
    End Sub

    Private Function ReadSectorsByTrack(DiskBuffer() As Byte, SectorData As SectorData) As Byte()
        Dim BufferSize = BYTES_PER_SECTOR * SectorData.SectorCount
        Dim Buffer(BufferSize - 1) As Byte
        Dim BytesRead = _FloppyDrive.ReadSector(SectorData.SectorStart, Buffer)
        If BytesRead = Buffer.Length Then
            If DiskBuffer IsNot Nothing Then
                Buffer.CopyTo(DiskBuffer, SectorToBytes(SectorData.SectorStart))
            End If
            SectorData.SectorsProcessed = SectorData.SectorCount
            For Count = 0 To SectorData.SectorCount - 1
                SectorData.SectorStatus(Count) = True
            Next
        Else
            SectorData.SectorsProcessed = 0
            For Count = 0 To SectorData.SectorCount - 1
                SectorData.SectorStatus(Count) = False
            Next
        End If

        Return Buffer
    End Function

    Public Function SectorToBytes(Sector As UInteger) As UInteger
        Return Sector * BYTES_PER_SECTOR
    End Function

    Private Sub WriteSectorsBySector(DiskBuffer() As Byte, SectorData As SectorData)
        Dim BufferSize = BYTES_PER_SECTOR
        Dim Buffer(BufferSize - 1) As Byte
        For Count = 0 To SectorData.SectorCount - 1
            If Not SectorData.SectorStatus(Count) Then
                Dim Sector = SectorData.SectorStart + Count
                Array.Copy(_DiskBuffer, SectorToBytes(Sector), Buffer, 0, Buffer.Length)
                Dim BytesWritten = _FloppyDrive.WriteSector(Sector, Buffer)
                If BytesWritten = Buffer.Length Then
                    SectorData.SectorsProcessed += 1
                    SectorData.SectorStatus(Count) = True
                End If
            End If
        Next
    End Sub

    Private Function WriteSectorsByTrack(DiskBuffer() As Byte, SectorData As SectorData) As Byte()
        Dim BufferSize = BYTES_PER_SECTOR * SectorData.SectorCount
        Dim Buffer(BufferSize - 1) As Byte
        Array.Copy(_DiskBuffer, SectorToBytes(SectorData.SectorStart), Buffer, 0, Buffer.Length)
        Dim BytesWritten = _FloppyDrive.WriteSector(SectorData.SectorStart, Buffer)
        If BytesWritten = Buffer.Length Then
            SectorData.SectorsProcessed = SectorData.SectorCount
            For Count = 0 To SectorData.SectorCount - 1
                SectorData.SectorStatus(Count) = True
            Next
        Else
            SectorData.SectorsProcessed = 0
            For Count = 0 To SectorData.SectorCount - 1
                SectorData.SectorStatus(Count) = False
            Next
        End If

        Return Buffer
    End Function
#Region "Events"

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim bw As BackgroundWorker = CType(sender, BackgroundWorker)

        If _AccessType = FloppyAccessType.Read Then
            FloppyDiskRead(bw)
        Else
            FloppyDiskWrite(bw, _DoFormat, _DoVerify)
        End If
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        Dim TrackInfo As TrackInfo = e.UserState
        Dim Track = _BPB.SectorToTrack(TrackInfo.Sector)
        Dim Side = _BPB.SectorToSide(TrackInfo.Sector)
        Dim Table As TableLayoutPanel

        If Side = 0 Then
            Table = TableSide0
        ElseIf Side = 1 Then
            Table = TableSide1
        Else
            Table = Nothing
        End If

        If Table IsNot Nothing Then
            Dim Row As Integer = Track \ 10
            Dim Col As Integer = Track Mod 10
            Dim objLabel As Label = Table.GetControlFromPosition(Col, Row)
            Dim StatusText = GetStatusText(TrackInfo.Status)
            If StatusText <> "" Then
                StatusType.Text = StatusText
            End If
            objLabel.BackColor = GetStatusColor(TrackInfo.Status)
            If TrackInfo.BadSectors > 0 Then
                objLabel.Text = TrackInfo.BadSectors
            Else
                objLabel.Text = ""
            End If
        End If

        StatusTrack.Text = "Track " & Track
        StatusSide.Text = "Side " & Side
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        _EndProcess = True
        If _SectorError Then
            _SectorError = False
            If HandleError() Then
                Exit Sub
            End If
        End If

        If Not _Complete Then
            If Not ConfirmAbort() Then
                Exit Sub
            End If
            StatusType.Text = "Aborted"
        Else
            StatusType.Text = "Complete"
        End If

        If _AccessType = FloppyAccessType.Write Then
            If _Complete Then
                Dim Msg As String = $"Your disk has been written successfully.{vbCrLf}{vbCrLf}Please remove the disk from the drive and write protect it now.{vbCrLf}{vbCrLf}Warning: If you continue without write protecting the disk, The operating system may make modifications to the disk as soon as you close this window."
                MsgBox(Msg, MsgBoxStyle.Information)
            End If
        Else
            If _Complete Then
                Dim DiskFormat = GetFloppyDiskFormat(_BPB, False)
                _FileName = FloppyDiskSaveFile(_DiskBuffer, DiskFormat, _LoadedFileNames)
            End If
        End If

        btnAbort.Text = "Close"

        'Me.Hide()
    End Sub

    Private Sub FloppyAccessForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If Not _Activated Then
            BackgroundWorker1.RunWorkerAsync()
        End If
        _Activated = True
    End Sub

    Private Sub FloppyAccessForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not _EndProcess Then
            e.Cancel = True
            If Not BackgroundWorker1.CancellationPending Then
                BackgroundWorker1.CancelAsync()
            End If
        End If
    End Sub


#End Region

    Private Class SectorData
        Private ReadOnly _SectorCount As UShort
        Private ReadOnly _SectorStart As UInteger

        Public Sub New(SectorStart As UInteger, SectorCount As UShort)
            _SectorCount = SectorCount
            _SectorStart = SectorStart
            _SectorsProcessed = 0
            ReDim _SectorStatus(SectorCount - 1)
        End Sub

        Public ReadOnly Property SectorCount As UShort
            Get
                Return _SectorCount
            End Get
        End Property

        Public Property SectorsProcessed As UShort

        Public ReadOnly Property SectorStart As UInteger
            Get
                Return _SectorStart
            End Get
        End Property
        Public Property SectorStatus As Boolean()
    End Class

    Private Class TrackInfo
        Public Sub New(Sector As UInteger)
            _Sector = Sector
        End Sub
        Public Property BadSectors As UShort = 0
        Public Property BytesRead As Byte() = Nothing
        Public Property BytesWritten As Byte() = Nothing
        Public ReadOnly Property Sector As UInteger
        Public Property Status As TrackStatus
    End Class
End Class