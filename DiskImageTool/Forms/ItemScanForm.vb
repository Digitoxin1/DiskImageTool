Imports System.ComponentModel

Public Class ItemScanForm
    Private ReadOnly _ImageList As ComboBox.ObjectCollection
    Private ReadOnly _NewOnly As Boolean
    Private ReadOnly _Parent As MainForm
    Private _Activated As Boolean = False
    Private _EndScan As Boolean = False
    Private _ScanComplete As Boolean = False
    Private _ItemsRemaining As UInteger

    Public Sub New(Parent As MainForm, ImageList As ComboBox.ObjectCollection, NewOnly As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Parent = Parent
        _ImageList = ImageList
        _NewOnly = NewOnly
        _ItemsRemaining = ImageList.Count
        If _NewOnly Then
            For Each ImageData As LoadedImageData In ImageList
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

    Private Function ProcessScan(bw As BackgroundWorker) As Boolean
        Dim ItemCount As Integer = _ItemsRemaining

        If ItemCount = 0 Then
            Return True
        End If

        Dim PrevPercentage As Integer = 0
        Dim Counter As Integer = 0
        For Each ImageData As LoadedImageData In _ImageList
            If bw.CancellationPending Then
                Return False
            End If
            Dim Percentage As Integer = Counter / ItemCount * 100
            If Percentage <> PrevPercentage Then
                bw.ReportProgress(Percentage)
                PrevPercentage = Percentage
            End If
            If Not _NewOnly Or Not ImageData.Scanned Then
                Dim Disk = _Parent.DiskImageLoad(ImageData)

                If Disk IsNot Nothing Then
                    _Parent.ItemScanModified(Disk, ImageData)
                    _Parent.ItemScanDisk(Disk, ImageData)
                    _Parent.ItemScanOEMName(Disk, ImageData)
                    _Parent.OEMNameFilterUpdate(Disk, ImageData)
                    _Parent.DiskTypeFilterUpdate(Disk, ImageData)
                    _Parent.ItemScanUnusedClusters(Disk, ImageData)
                    _Parent.ItemScanDirectory(Disk, ImageData)

                    ImageData.Scanned = True
                End If
                Counter += 1
                _ItemsRemaining -= 1
            End If
        Next

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
        LblScanning.Text = "Scanning... " & e.ProgressPercentage & "%"
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
            LblScanning.Text = "Scanning"
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