Imports System.ComponentModel

Public Class ItemScanForm
    Private _Activated As Boolean = False
    Private ReadOnly _Parent As MainForm
    Private ReadOnly _LoadedImageList As List(Of LoadedImageData)
    Private ReadOnly _NewOnly As Boolean
    Private _EndScan As Boolean = False
    Private _ScanComplete As Boolean = False

    Public Sub New(Parent As MainForm, LoadedImageList As List(Of LoadedImageData), NewOnly As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Parent = Parent
        _LoadedImageList = LoadedImageList
        _NewOnly = NewOnly
    End Sub

    Public ReadOnly Property ScanComplete As Boolean
        Get
            Return _ScanComplete
        End Get
    End Property

    Private Sub ItemScanForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If Not _Activated Then
            _EndScan = False
            ProcessScan()
            _EndScan = True
            Me.Close()
        End If
        _Activated = True
    End Sub

    Private Sub ProcessScan()
        LblScanning.Text = "Scanning"
        _ScanComplete = False

        Dim ItemCount As Integer = 0
        If _NewOnly Then
            For Each ImageData In _LoadedImageList
                If Not ImageData.Scanned Then
                    ItemCount += 1
                End If
            Next
        Else
            ItemCount = _LoadedImageList.Count
        End If

        Dim Counter As Integer = 0
        For Each ImageData In _LoadedImageList
            If _EndScan Then
                Exit For
            End If
            Dim Percentage = Counter / ItemCount * 100
            If Counter Mod 100 = 0 Then
                LblScanning.Text = "Scanning... " & Int(Percentage) & "%"
                Application.DoEvents()
            End If
            If Not _NewOnly Or Not ImageData.Scanned Then
                Dim Disk = _Parent.DiskImageLoad(ImageData)

                If Not Disk.LoadError Then
                    _Parent.ItemScanModified(Disk, ImageData)
                    _Parent.ItemScanValidImage(Disk, ImageData)
                    _Parent.ItemScanOEMID(Disk, ImageData)
                    _Parent.ItemScanUnusedClusters(Disk, ImageData)
                    _Parent.ItemScanDirectory(Disk, ImageData)

                    ImageData.Scanned = True
                End If
                Counter += 1
            End If
        Next

        If Not _EndScan Then
            _ScanComplete = True
        End If
    End Sub

    Private Sub ItemScanForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not _EndScan Then
            e.Cancel = True
            _EndScan = True
        End If
    End Sub
End Class