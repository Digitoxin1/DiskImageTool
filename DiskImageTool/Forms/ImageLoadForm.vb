Imports System.Threading

Public Class ImageLoadForm
    Private ReadOnly _CtSource As CancellationTokenSource
    Private ReadOnly _Scanner As ImageScanner
    Private _Counter As Integer = 0
    Private _EndScan As Boolean = False
    Private _ImageCount As Integer = 0
    Private _Visible As Boolean = False

    Public Sub New(scanner As ImageScanner, ctSource As CancellationTokenSource)
        InitializeComponent()
        Me.Text = My.Resources.Caption_ScanFiles
        _Scanner = scanner
        _CtSource = ctSource

        AddHandler _Scanner.FileScanned, AddressOf Scanner_FileScanned
        AddHandler _Scanner.ImageDiscovered, AddressOf Scanner_ImageDiscovered
        AddHandler _Scanner.ScanCompleted, AddressOf Scanner_ScanCompleted
    End Sub

    Private Sub ImageLoadForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not _EndScan Then
            ' Treat user close as cancel
            e.Cancel = True
            _CtSource.Cancel()
        End If
    End Sub

    Private Sub ImageLoadForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Opacity = 0.0
        LblScanning.Text = My.Resources.Label_Scanning
        lblScanning2.Text = String.Format(My.Resources.Label_ImagesLoaded, 0)
    End Sub

    Private Sub Scanner_FileScanned(sender As Object, e As EventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(New EventHandler(AddressOf Scanner_FileScanned), sender, e)
            Return
        End If

        _Counter += 1

        If Not _Visible AndAlso _Counter > 10 Then
            _Visible = True
            Me.Opacity = 1
        End If

        If _Counter Mod 10 = 0 Then
            LblScanning.Text = My.Resources.Label_Scanning & "... " & _Counter & " " & My.Resources.Label_Files
            lblScanning2.Text = String.Format(My.Resources.Label_ImagesLoaded, _ImageCount)
            LblScanning.Refresh()
            lblScanning2.Refresh()
        End If
    End Sub

    Private Sub Scanner_ImageDiscovered(sender As Object, e As ImageDiscoveredEventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(New EventHandler(Of ImageDiscoveredEventArgs)(AddressOf Scanner_ImageDiscovered), sender, e)
            Return
        End If

        _ImageCount += 1
        lblScanning2.Text = String.Format(My.Resources.Label_ImagesLoaded, _ImageCount)
        lblScanning2.Refresh()
    End Sub

    Private Sub Scanner_ScanCompleted(sender As Object, e As ScanCompletedEventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(New EventHandler(Of ScanCompletedEventArgs)(AddressOf Scanner_ScanCompleted), sender, e)
            Return
        End If

        _EndScan = True
        Me.Close()
    End Sub
End Class