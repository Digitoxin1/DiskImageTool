Imports System.Threading

Public Class ImageLoadForm
    Private WithEvents DelayTimer As Windows.Forms.Timer
    Private Const DelayMs As Integer = 500
    Private ReadOnly _CtSource As CancellationTokenSource
    Private ReadOnly _Files As String()
    Private ReadOnly _NewFileName As String
    Private ReadOnly _NewImage As Boolean
    Private ReadOnly _Scanner As ImageScanner
    Private _Counter As Integer = 0
    Private _EndScan As Boolean = False
    Private _ImageCount As Integer = 0

    Public Sub New(scanner As ImageScanner, Files() As String, NewImage As Boolean, NewFileName As String)
        InitializeComponent()
        Me.Opacity = 0.0

        Me.Text = My.Resources.Caption_ScanFiles
        _Scanner = scanner
        _CtSource = New Threading.CancellationTokenSource()
        _Files = Files
        _NewImage = NewImage
        _NewFileName = NewFileName

        AddHandler _Scanner.FileScanned, AddressOf Scanner_FileScanned
        AddHandler _Scanner.ImageDiscovered, AddressOf Scanner_ImageDiscovered
        AddHandler _Scanner.ScanCompleted, AddressOf Scanner_ScanCompleted
    End Sub

    Public ReadOnly Property EndScan As Boolean
        Get
            Return _EndScan
        End Get
    End Property

    Public Shared Sub Display(scanner As ImageScanner, Files() As String, NewImage As Boolean, NewFileName As String)
        Using dlg As New ImageLoadForm(scanner, Files, NewImage, NewFileName)
            dlg.ShowDialog(App.CurrentFormInstance)
        End Using
    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        MyBase.OnFormClosed(e)

        If DelayTimer IsNot Nothing Then
            DelayTimer.Stop()
            DelayTimer.Dispose()
            DelayTimer = Nothing
        End If

        _CtSource.Cancel()
        _CtSource.Dispose()
    End Sub

    Private Sub DelayTimer_Tick(sender As Object, e As EventArgs) Handles DelayTimer.Tick
        DelayTimer.Stop()

        ' If the scan already finished or the form is closing, do nothing
        If _EndScan OrElse Me.IsDisposed OrElse Me.Disposing Then
            Return
        End If

        ' Scan is still running → bring the dialog on-screen and show it
        Me.Opacity = 1.0
    End Sub

    Private Sub ImageLoadForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not _EndScan Then
            ' Treat user close as cancel
            e.Cancel = True
            _CtSource.Cancel()
        End If
    End Sub
    Private Sub ImageLoadForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LblScanning.Text = My.Resources.Label_Scanning
        lblScanning2.Text = String.Format(My.Resources.Label_ImagesLoaded, 0)

        Task.Run(Sub() _Scanner.Scan(_Files, _NewImage, _NewFileName, _CtSource.Token))

        DelayTimer = New Windows.Forms.Timer With {
            .Interval = DelayMs
        }
        DelayTimer.Start()
    End Sub

    Private Sub Scanner_FileScanned(sender As Object, e As EventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(New EventHandler(AddressOf Scanner_FileScanned), sender, e)
            Return
        End If

        _Counter += 1

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

        DelayTimer?.Stop()

        Me.Close()
    End Sub
End Class