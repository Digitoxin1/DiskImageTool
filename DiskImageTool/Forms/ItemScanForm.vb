Imports System.Threading

Public Class ItemScanForm
    Private ReadOnly _ctSource As CancellationTokenSource
    Private ReadOnly _progressLabel As String
    Private ReadOnly _scanner As IImageScanner
    Private _endScan As Boolean = False

    Public Sub New(scanner As IImageScanner, windowTitle As String, progressLabel As String, ctSource As CancellationTokenSource)

        InitializeComponent()
        _scanner = scanner
        _ctSource = ctSource
        Me.Text = windowTitle
        _progressLabel = progressLabel

        AddHandler _scanner.ProgressChanged, AddressOf Scanner_ProgressChanged
        AddHandler _scanner.ScanCompleted, AddressOf Scanner_ScanCompleted
    End Sub

    Public ReadOnly Property ItemsRemaining As UInteger
        Get
            Return _scanner.ItemsRemaining
        End Get
    End Property

    Public Shared Sub Display(scanner As IImageScanner, windowTitle As String, progessLabel As String, ctSource As CancellationTokenSource, owner As IWin32Window)
        Using dlg As New ItemScanForm(scanner, windowTitle, progessLabel, ctSource)
            dlg.ShowDialog(owner)
        End Using
    End Sub

    Private Sub ItemScanForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not _endScan Then
            e.Cancel = True
            _ctSource.Cancel()
        End If
    End Sub

    Private Sub ItemScanForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LblScanning.Text = _progressLabel & "... 0%"
    End Sub

    Private Sub Scanner_ProgressChanged(percent As Integer)
        If InvokeRequired Then
            Invoke(New Action(Of Integer)(AddressOf Scanner_ProgressChanged), percent)
            Return
        End If

        LblScanning.Text = $"{_progressLabel}... {percent}%"
    End Sub

    Private Sub Scanner_ScanCompleted(cancelled As Boolean, [error] As Exception)
        If InvokeRequired Then
            Invoke(New Action(Of Boolean, Exception)(AddressOf Scanner_ScanCompleted), cancelled, [error])
            Return
        End If

        _endScan = True
        Close()
    End Sub
End Class
