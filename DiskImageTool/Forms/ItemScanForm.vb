Imports System.ComponentModel

Public Enum ScanType
    ScanTypeFilters
    ScanTypeWin9xClean
End Enum

Public Class ItemScanForm
    Private ReadOnly _CurrentImage As CurrentImage
    Private ReadOnly _ImageList As ComboBox.ObjectCollection
    Private ReadOnly _NewOnly As Boolean
    Private ReadOnly _Parent As MainForm
    Private ReadOnly _ProgressLabel As String
    Private ReadOnly _ScanType As ScanType
    Private _Activated As Boolean = False
    Private _EndScan As Boolean = False
    Private _ItemsRemaining As UInteger
    Private _ScanComplete As Boolean = False
    Public Sub New(Parent As MainForm, ImageList As ComboBox.ObjectCollection, CurrentImage As CurrentImage, NewOnly As Boolean, ScanType As ScanType)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Parent = Parent
        _ImageList = ImageList
        _CurrentImage = CurrentImage
        _NewOnly = NewOnly
        _ScanType = ScanType
        _ItemsRemaining = ImageList.Count

        If ScanType = ScanType.ScanTypeFilters Then
            Me.Text = My.Resources.Caption_ScanImages
            _ProgressLabel = My.Resources.Label_Scanning
        Else
            Me.Text = My.Resources.Caption_CleanImages
            _ProgressLabel = My.Resources.Label_Processing
        End If

        If ScanType = ScanType.ScanTypeFilters And _NewOnly Then
            For Each ImageData As ImageData In ImageList
                If ImageData.Scanned Then
                    _ItemsRemaining -= 1
                End If
            Next
        End If
    End Sub

    Public ReadOnly Property ItemsRemaining As Boolean
        Get
            Return _ItemsRemaining
        End Get
    End Property

    Public ReadOnly Property ScanComplete As Boolean
        Get
            Return _ScanComplete
        End Get
    End Property

    Private Function ProcessFilters(ImageData As ImageData) As Boolean
        Dim Result As Boolean = False

        If Not _NewOnly Or Not ImageData.Scanned Then
            Dim Disk As DiskImage.Disk

            If ImageData Is _CurrentImage.ImageData Then
                Disk = _CurrentImage.Disk
            Else
                Disk = DiskImageLoad(ImageData, True)
            End If

            If Disk IsNot Nothing Then
                _Parent.ImageFiltersScanModified(Disk, ImageData)
                _Parent.ImageFiltersScanDisk(Disk, ImageData)
                _Parent.ImageFiltersScanOEMName(Disk, ImageData)
                _Parent.ImageSubFilterOEMNameUpdate(Disk, ImageData)
                _Parent.ImageSubFilterDiskTypeUpdate(Disk, ImageData)
                _Parent.ImageFiltersScanFreeClusters(Disk, ImageData)
                _Parent.ImageFiltersScanDirectory(Disk, ImageData)

                ImageData.Scanned = True
            End If

            Result = True
        End If

        Return Result
    End Function

    Private Function ProcessScan(bw As BackgroundWorker) As Boolean
        Dim ItemCount As Integer = _ItemsRemaining

        If ItemCount = 0 Then
            Return True
        End If

        Dim PrevPercentage As Integer = 0
        Dim Counter As Integer = 0
        Dim Result As Boolean = True
        For Each ImageData As ImageData In _ImageList
            If bw.CancellationPending Then
                Return False
            End If
            Dim Percentage As Integer = Counter / ItemCount * 100
            If Percentage <> PrevPercentage Then
                bw.ReportProgress(Percentage)
                PrevPercentage = Percentage
            End If

            If _ScanType = ScanType.ScanTypeFilters Then
                Result = ProcessFilters(ImageData)
            ElseIf _ScanType = ScanType.ScanTypeWin9xClean Then
                Result = ProcessWin9XClean(ImageData)
            End If

            If Result Then
                Counter += 1
                _ItemsRemaining -= 1
            End If
        Next

        Return True
    End Function

    Private Function ProcessWin9XClean(ImageData As ImageData) As Boolean
        Dim Disk As DiskImage.Disk

        If ImageData Is _CurrentImage.ImageData Then
            Disk = _CurrentImage.Disk
        Else
            Disk = DiskImageLoad(ImageData, True)
        End If

        If Disk IsNot Nothing Then
            Dim Result = _Parent.ImageWin9xClean(Disk, True)
            ImageData.BatchUpdated = Result
            If Result Then
                _Parent.ImageFiltersScanModified(Disk, ImageData)
                If ImageData.Scanned Then
                    _Parent.ImageFiltersScanOEMName(Disk, ImageData)
                    _Parent.ImageSubFilterOEMNameUpdate(Disk, ImageData)
                    _Parent.ImageFiltersScanDirectory(Disk, ImageData)
                End If
            End If
        End If

        Return True
    End Function
#Region "Events"

    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Dim bw As BackgroundWorker = CType(sender, BackgroundWorker)

        If Not ProcessScan(bw) Then
            e.Cancel = True
        End If
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        LblScanning.Text = _ProgressLabel & "... " & e.ProgressPercentage & "%"
        LblScanning.Refresh()
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        _EndScan = True
        If Not e.Cancelled Then
            _ScanComplete = True
        End If
        Me.Close()
    End Sub

    Private Sub ItemScanForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If Not _Activated Then
            _EndScan = False
            _ScanComplete = False
            LblScanning.Text = My.Resources.Label_Scanning
            BackgroundWorker1.RunWorkerAsync()
        End If
        _Activated = True
    End Sub

    Private Sub ItemScanForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not _EndScan Then
            e.Cancel = True
            If Not BackgroundWorker1.CancellationPending Then
                BackgroundWorker1.CancelAsync()
            End If
        End If
    End Sub

#End Region

End Class