Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression

Public Class ImageLoadForm
    Private Const MIN_FILE_SIZE As Long = 163840
    Private Const MAX_FILE_SIZE As Long = 3000000
    Private ReadOnly _ArchiveFilterExt As List(Of String)
    Private ReadOnly _FileFilterExt As List(Of String)
    Private ReadOnly _Files() As String
    Private ReadOnly _LoadedFileNames As Dictionary(Of String, LoadedImageData)
    Private ReadOnly _Parent As MainForm
    Private _Activated As Boolean = False
    Private _Counter As Integer = 0
    Private _ImageCount As Integer = 0
    Private _EndScan As Boolean = False
    Private _SelectedImageData As LoadedImageData = Nothing
    Private _Visible As Boolean = False

    Public Sub New(Parent As MainForm, Files() As String, LoadedFileNames As Dictionary(Of String, LoadedImageData), FileFilterExt As List(Of String), ArchiveFilterExt As List(Of String))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Parent = Parent
        _Files = Files
        _LoadedFileNames = LoadedFileNames
        _FileFilterExt = FileFilterExt
        _ArchiveFilterExt = ArchiveFilterExt
    End Sub

    Public ReadOnly Property SelectedImageData As LoadedImageData
        Get
            Return _SelectedImageData
        End Get
    End Property

    Private Shared Function IsZipArchive(FileName As String) As ZipArchive
        Try
            Dim Buffer(1) As Byte
            Using fs = New FileStream(FileName, FileMode.Open, FileAccess.Read)
                Dim BytesRead = fs.Read(Buffer, 0, Buffer.Length)
                fs.Close()
            End Using

            If Buffer(0) = &H50 And Buffer(1) = &H4B Then
                Return ZipFile.OpenRead(FileName)
            End If
        Catch ex As Exception
            Return Nothing
        End Try

        Return Nothing
    End Function

    Private Sub LoadedFileAdd(bw As BackgroundWorker, Key As String, FileName As String, Compressed As Boolean, Optional CompressedFile As String = "")

        If Not _LoadedFileNames.ContainsKey(Key) Then
            Dim ImageData = New LoadedImageData(FileName)
            If Compressed Then
                ImageData.Compressed = True
                ImageData.CompressedFile = CompressedFile
            End If
            _LoadedFileNames.Add(Key, ImageData)
            If _SelectedImageData Is Nothing Then
                _SelectedImageData = ImageData
            End If
            bw.ReportProgress(2, ImageData)
        End If
    End Sub

    Private Sub ProcessFile(bw As BackgroundWorker, FileName As String, Extension As String)
        If Not File.Exists(FileName) Then
            Return
        End If

        Dim Archive = IsZipArchive(FileName)

        If Archive IsNot Nothing Then
            Dim Entries As ReadOnlyCollection(Of ZipArchiveEntry) = Nothing
            Try
                Entries = Archive.Entries
            Catch
                '
            End Try
            If Entries IsNot Nothing Then
                For Each Entry In Entries.OrderBy(Function(e) e.FullName)
                    If _FileFilterExt.Contains(Path.GetExtension(Entry.Name).ToLower) Then
                        Dim FilePath = Path.Combine(FileName, Entry.FullName)
                        If Entry.Length >= MIN_FILE_SIZE And Entry.Length <= MAX_FILE_SIZE Then
                            LoadedFileAdd(bw, FilePath, FileName, True, Entry.FullName)
                        End If
                    End If
                Next
            End If
        Else
            If Not _ArchiveFilterExt.Contains(Extension) Then
                Dim Length As Long = 0
                Try
                    Length = New FileInfo(FileName).Length
                Catch
                    '
                End Try
                If Length >= MIN_FILE_SIZE And Length <= MAX_FILE_SIZE Then
                    LoadedFileAdd(bw, FileName, FileName, False)
                End If
            End If
        End If
    End Sub

    Private Function ProcessScan(bw As BackgroundWorker) As Boolean
        If _Files.Count = 0 Then
            Return True
        End If

        For Each FilePath In _Files.OrderBy(Function(f) f)
            If bw.CancellationPending Then
                Return True
            End If
            Dim FAttributes = IO.File.GetAttributes(FilePath)
            If (FAttributes And IO.FileAttributes.Directory) > 0 Then
                Dim DirectoryInfo As New IO.DirectoryInfo(FilePath)
                Dim Files = DirectoryInfo.GetFiles("*.*", IO.SearchOption.AllDirectories)
                For Each FileInfo In Files
                    If bw.CancellationPending Then
                        Return True
                    End If
                    Dim Extension = FileInfo.Extension.ToLower
                    If _FileFilterExt.Contains(Extension) OrElse _ArchiveFilterExt.Contains(Extension) Then
                        ProcessFile(bw, FileInfo.FullName, Extension)
                    End If
                    bw.ReportProgress(1)
                Next
            Else
                Dim Extension = Path.GetExtension(FilePath).ToLower
                ProcessFile(bw, FilePath, Extension)
            End If
            bw.ReportProgress(1)
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
        If e.ProgressPercentage = 1 Then
            _Counter += 1
            If Not _Visible AndAlso _Counter > 10 Then
                _Visible = True
                Me.Opacity = 1
            End If
            If _Counter Mod 10 = 0 Then
                LblScanning.Text = "Scanning... " & _Counter & " files"
                lblScanning2.Text = _ImageCount & " images loaded"
                LblScanning.Refresh()
                lblScanning2.Refresh()
            End If
        ElseIf e.ProgressPercentage = 2 Then
            _ImageCount += 1
            Dim ImageData As LoadedImageData = e.UserState
            _Parent.ComboImages.Items.Add(ImageData)
        End If
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        _EndScan = True

        Me.Hide()
    End Sub

    Private Sub ImageLoadForm_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If Not _Activated Then
            _EndScan = False
            LblScanning.Text = "Scanning"
            BackgroundWorker1.RunWorkerAsync()
        End If
        _Activated = True
    End Sub

    Private Sub ImageLoadForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not _EndScan Then
            e.Cancel = True
            If Not BackgroundWorker1.CancellationPending Then
                BackgroundWorker1.CancelAsync()
            End If
        End If
    End Sub

    Private Sub ImageLoadForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Opacity = 0.0
    End Sub

#End Region

End Class