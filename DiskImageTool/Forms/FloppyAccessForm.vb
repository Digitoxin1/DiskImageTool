Imports DiskImageTool.DiskImage
Imports System.ComponentModel

Public Class FloppyAccessForm
    Private ReadOnly _BPB As BiosParameterBlock
    Private ReadOnly _FloppyDrive As FloppyInterface
    Private _Activated As Boolean = False
    Private _Complete As Boolean = False
    Private _DiskBuffer() As Byte = Nothing
    Private _EndProcess As Boolean = False
    Private _SectorData As SectorData = Nothing
    Private _SectorError As Boolean = False
    Private _TotalBadSectors As UInteger = 0

    Private Enum SectorReadType
        BySector
        ByTrack
    End Enum

    Private Enum TrackStatus
        Pending
        Success
        Failure
    End Enum

    Public Sub New(FloppyDrive As FloppyInterface, BPB As BiosParameterBlock)
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call
        TableSide0.GetType() _
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic) _
            .SetValue(TableSide0, True, Nothing)
        TableSide1.GetType() _
            .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic) _
            .SetValue(TableSide1, True, Nothing)

        _FloppyDrive = FloppyDrive
        _BPB = BPB
        ReDim _DiskBuffer(_BPB.ImageSize - 1)

        Me.Text = "Reading " & GetFloppyDiskTypeName(_BPB) & " Floppy"

        InitTables()
    End Sub

    Public ReadOnly Property Complete As Boolean
        Get
            Return _Complete
        End Get
    End Property

    Public ReadOnly Property DiskBuffer As Byte()
        Get
            Return _DiskBuffer
        End Get
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

    Private Sub FloppyDiskRead(bw As BackgroundWorker)
        _Complete = False
        _EndProcess = False
        _SectorError = False
        Dim TrackInfo As TrackInfo
        Dim SectorData As SectorData
        Dim SectorStart As UInteger

        If _SectorData Is Nothing Then
            SectorStart = 0
            SectorData = Nothing
        Else
            SectorStart = _SectorData.SectorStart
            SectorData = _SectorData
        End If

        For Sector As UInteger = SectorStart To _BPB.SectorCount - 1 Step _BPB.SectorsPerTrack
            If SectorData Is Nothing Then
                SectorData = New SectorData(Sector, _BPB.SectorsPerTrack)
            End If
            If bw.CancellationPending Then
                _SectorData = SectorData
                Exit Sub
            End If
            TrackInfo.Sector = Sector
            TrackInfo.Status = TrackStatus.Pending
            TrackInfo.BadSectors = 0
            bw.ReportProgress(0, TrackInfo)
            If SectorData.SectorsRead = 0 Then
                ReadSectors(_DiskBuffer, SectorData, SectorReadType.ByTrack)
            End If
            If SectorData.SectorsRead < SectorData.SectorCount Then
                ReadSectors(_DiskBuffer, SectorData, SectorReadType.BySector)
            End If
            If SectorData.SectorCount = SectorData.SectorsRead Then
                TrackInfo.Status = TrackStatus.Success
                TrackInfo.BadSectors = 0
            Else
                TrackInfo.Status = TrackStatus.Failure
                TrackInfo.BadSectors = SectorData.SectorCount - SectorData.SectorsRead
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

    Private Function HandleError() As Boolean
        Dim Track = _BPB.SectorToTrack(_SectorData.SectorStart)
        Dim Side = _BPB.SectorToSide(_SectorData.SectorStart)
        Dim BadSectors = _SectorData.SectorCount - _SectorData.SectorsRead
        Dim Msg = "Error Reading " & BadSectors & " Sector".Pluralize(BadSectors) & " in Track " & Track & ", Side " & Side & "."
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
    Private Sub ReadSectors(DiskBuffer() As Byte, SectorData As SectorData, ReadType As SectorReadType)
        If ReadType = SectorReadType.ByTrack Then
            Dim BufferSize = Disk.BYTES_PER_SECTOR * SectorData.SectorCount
            Dim Buffer(BufferSize - 1) As Byte
            Dim BytesRead = _FloppyDrive.ReadSector(SectorData.SectorStart, Buffer)
            If BytesRead = Buffer.Length Then
                Buffer.CopyTo(DiskBuffer, Disk.SectorToBytes(SectorData.SectorStart))
                SectorData.SectorsRead = SectorData.SectorCount
                For Count = 0 To SectorData.SectorCount - 1
                    SectorData.SectorStatus(Count) = True
                Next
            Else
                SectorData.SectorsRead = 0
                For Count = 0 To SectorData.SectorCount - 1
                    SectorData.SectorStatus(Count) = False
                Next
            End If
        Else
            Dim BufferSize = Disk.BYTES_PER_SECTOR
            Dim Buffer(BufferSize - 1) As Byte
            For Count = 0 To SectorData.SectorCount - 1
                If Not SectorData.SectorStatus(Count) Then
                    Dim Sector = SectorData.SectorStart + Count
                    Dim BytesRead = _FloppyDrive.ReadSector(Sector, Buffer)
                    If BytesRead = Buffer.Length Then
                        Buffer.CopyTo(DiskBuffer, Disk.SectorToBytes(Sector))
                        SectorData.SectorsRead += 1
                        SectorData.SectorStatus(Count) = True
                    End If
                End If
            Next
        End If
    End Sub

    Private Structure TrackInfo
        Dim BadSectors As UShort
        Dim Sector As UInteger
        Dim Status As TrackStatus
    End Structure
#Region "Events"

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim bw As BackgroundWorker = CType(sender, BackgroundWorker)

        FloppyDiskRead(bw)
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
            Dim BackColor As Color
            Dim objLabel As Label = Table.GetControlFromPosition(Col, Row)
            If TrackInfo.Status = TrackStatus.Pending Then
                BackColor = Color.Blue
            ElseIf TrackInfo.Status = TrackStatus.Success Then
                BackColor = Color.LightGreen
            Else
                BackColor = Color.Red
            End If
            objLabel.BackColor = BackColor
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
            If Not ConfirmAbort Then
                Exit Sub
            End If
        End If
        Me.Hide()
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
            _SectorsRead = 0
            ReDim _SectorStatus(SectorCount - 1)
        End Sub

        Public ReadOnly Property SectorCount As UShort
            Get
                Return _SectorCount
            End Get
        End Property

        Public Property SectorsRead As UShort

        Public ReadOnly Property SectorStart As UInteger
            Get
                Return _SectorStart
            End Get
        End Property
        Public Property SectorStatus As Boolean()
    End Class

End Class