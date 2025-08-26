Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression

Public Class ImageLoadForm
    Private Const MIN_FILE_SIZE As Long = 163840
    Private Const MAX_FILE_SIZE As Long = 3000000
    Private ReadOnly _Files() As String
    Private ReadOnly _LoadedFiles As LoadedFiles
    Private ReadOnly _Parent As MainForm
    Private _Activated As Boolean = False
    Private _Counter As Integer = 0
    Private _ImageCount As Integer = 0
    Private _EndScan As Boolean = False
    Private _NewImage As Boolean = False
    Private _SelectedImageData As ImageData = Nothing
    Private _Visible As Boolean = False

    Public Sub New(Parent As MainForm, Files() As String, LoadedFiles As LoadedFiles, NewImage As Boolean)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Text = My.Resources.Caption_ScanFiles
        _Parent = Parent
        _Files = Files
        _LoadedFiles = LoadedFiles
        _NewImage = NewImage
    End Sub

    Public ReadOnly Property SelectedImageData As ImageData
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
            DebugException(ex)
            Return Nothing
        End Try

        Return Nothing
    End Function

    Private Sub LoadedFileAdd(bw As BackgroundWorker, Key As String, FileName As String, FileType As ImageData.FileTypeEnum, Optional CompressedFile As String = "")
        Dim ImageData = _LoadedFiles.Add(Key, FileName, FileType, CompressedFile)
        If ImageData IsNot Nothing Then
            If _SelectedImageData Is Nothing Then
                _SelectedImageData = ImageData
            End If

            If bw Is Nothing Then
                _Parent.ComboImages.Items.Add(ImageData)
            Else
                bw.ReportProgress(2, ImageData)
            End If

            If FileType = ImageData.FileTypeEnum.NewImage Then
                _Parent.ImageFiltersSetModified(ImageData)
            End If
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
                    Dim EntryFileExt = Path.GetExtension(Entry.Name).ToLower
                    If AllFileExtensions.Contains(EntryFileExt) Then
                        Dim FilePath = Path.Combine(FileName, Entry.FullName)
                        Dim CheckLength = Not BitstreamFileExtensions.Contains(EntryFileExt) And Not AdvancedSectorFileExtensions.Contains(EntryFileExt)
                        If Not CheckLength OrElse (Entry.Length >= MIN_FILE_SIZE And Entry.Length <= MAX_FILE_SIZE) Then
                            LoadedFileAdd(bw, FilePath, FileName, ImageData.FileTypeEnum.Compressed, Entry.FullName)
                        End If
                    End If
                Next
            End If
        Else
            If Not ArchiveFileExtensions.Contains(Extension) Then
                Dim Length As Long = 0
                Try
                    Length = New FileInfo(FileName).Length
                Catch
                    '
                End Try
                Dim CheckLength = Not BitstreamFileExtensions.Contains(Extension) And Not AdvancedSectorFileExtensions.Contains(Extension)
                If Not CheckLength OrElse (Length >= MIN_FILE_SIZE And Length <= MAX_FILE_SIZE) Then
                    LoadedFileAdd(bw, FileName, FileName, If(_NewImage, ImageData.FileTypeEnum.NewImage, ImageData.FileTypeEnum.Standard))
                End If
            End If
        End If
    End Sub

    Public Function ProcessScan(bw As BackgroundWorker) As Boolean
        If _Files.Count = 0 Then
            Return True
        End If

        For Each FilePath In _Files.OrderBy(Function(f) f)
            If bw IsNot Nothing AndAlso bw.CancellationPending Then
                Return True
            End If
            Dim FAttributes = IO.File.GetAttributes(FilePath)
            If (FAttributes And IO.FileAttributes.Directory) > 0 Then
                Dim DirectoryInfo As New IO.DirectoryInfo(FilePath)
                Dim Files = DirectoryInfo.GetFiles("*.*", IO.SearchOption.AllDirectories)
                For Each FileInfo In Files
                    If bw IsNot Nothing AndAlso bw.CancellationPending Then
                        Return True
                    End If
                    Dim Extension = FileInfo.Extension.ToLower
                    If AllFileExtensions.Contains(Extension) OrElse ArchiveFileExtensions.Contains(Extension) Then
                        ProcessFile(bw, FileInfo.FullName, Extension)
                    End If
                    bw?.ReportProgress(1)
                Next
            Else
                Dim Extension = Path.GetExtension(FilePath).ToLower
                ProcessFile(bw, FilePath, Extension)
            End If
            bw?.ReportProgress(1)
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
                LblScanning.Text = My.Resources.Label_Scanning & "... " & _Counter & " " & My.Resources.Label_Files
                lblScanning2.Text = String.Format(My.Resources.Label_ImagesLoaded, _ImageCount)
                LblScanning.Refresh()
                lblScanning2.Refresh()
            End If
        ElseIf e.ProgressPercentage = 2 Then
            _ImageCount += 1
            Dim ImageData As ImageData = e.UserState
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
            LblScanning.Text = My.Resources.Label_Scanning
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