Imports System.Collections.ObjectModel
Imports System.IO
Imports System.IO.Compression
Imports System.Threading

Public Class ImageScanner
    Private Const MAX_FILE_SIZE As Long = 3000000
    Private Const MIN_FILE_SIZE As Long = 163840

    Public Event FileScanned As EventHandler
    Public Event ImageDiscovered As EventHandler(Of ImageDiscoveredEventArgs)
    Public Event ScanCompleted As EventHandler(Of ScanCompletedEventArgs)

    Public Sub Scan(Files() As String, NewImage As Boolean, Optional NewFileName As String = "", Optional ct As CancellationToken = Nothing)

        Dim Cancelled As Boolean = False
        Dim ErrorEx As Exception = Nothing

        Try
            If Files Is Nothing OrElse Files.Length = 0 Then
                Exit Try
            End If

            For Each FilePath In Files.OrderBy(Function(f) f)
                If ct.IsCancellationRequested Then
                    Cancelled = True
                    Exit For
                End If

                Dim FileParts = SplitFilePath(FilePath)

                Dim FAttributes As FileAttributes
                Try
                    FAttributes = IO.File.GetAttributes(FileParts.FilePath)
                Catch
                    Continue For
                End Try

                If (FAttributes And IO.FileAttributes.Directory) > 0 Then
                    Dim DirectoryInfo As New IO.DirectoryInfo(FileParts.FilePath)
                    Dim AllFiles = DirectoryInfo.GetFiles("*.*", IO.SearchOption.AllDirectories)

                    For Each FileInfo In AllFiles
                        If ct.IsCancellationRequested Then
                            Cancelled = True
                            Exit For
                        End If

                        Dim Extension = FileInfo.Extension.ToLower()

                        If AllFileExtensions.Contains(Extension) OrElse ArchiveFileExtensions.Contains(Extension) Then
                            ProcessFile(FileInfo.FullName, Extension, NewImage, NewFileName, ct)
                        End If

                        OnFileScanned()
                    Next

                    If Cancelled Then Exit For
                Else
                    Dim Extension = Path.GetExtension(FileParts.FilePath).ToLower()
                    ProcessFile(FileParts.FilePath, Extension, NewImage, NewFileName, ct, FileParts.InnerPath)

                    OnFileScanned()
                End If
            Next

        Catch ex As Exception
            ErrorEx = ex
        Finally
            RaiseEvent ScanCompleted(Me, New ScanCompletedEventArgs(Cancelled, ErrorEx))
        End Try
    End Sub

    Private Function IsValidFileLength(Length As Long, Extension As String) As Boolean
        Dim CheckLength = Not BitstreamFileExtensions.Contains(Extension) And Not AdvancedSectorFileExtensions.Contains(Extension)
        Return Not CheckLength OrElse (Length >= MIN_FILE_SIZE And Length <= MAX_FILE_SIZE)
    End Function

    Private Sub OnFileScanned()
        RaiseEvent FileScanned(Me, EventArgs.Empty)
    End Sub

    Private Sub OnImageFound(Key As String, FileName As String, FileType As ImageData.FileTypeEnum, Optional CompressedFile As String = "", Optional NewFileName As String = "")
        RaiseEvent ImageDiscovered(Me, New ImageDiscoveredEventArgs(Key, FileName, FileType, CompressedFile, NewFileName))
    End Sub

    Private Sub ProcessFile(FileName As String, Extension As String, NewImage As Boolean, NewFileName As String, ct As CancellationToken, Optional InnerPath As String = Nothing)
        If ct.IsCancellationRequested Then
            Return
        End If

        If Not File.Exists(FileName) Then
            Return
        End If

        Dim Archive = IsZipArchive(FileName)

        If Archive IsNot Nothing Then
            ProcessZipArchive(Archive, FileName, ct, InnerPath)
        Else
            If Not ArchiveFileExtensions.Contains(Extension) Then
                Dim Length As Long = 0
                Try : Length = New FileInfo(FileName).Length : Catch : End Try

                If IsValidFileLength(Length, Extension) Then
                    Dim ft = If(NewImage, ImageData.FileTypeEnum.NewImage, ImageData.FileTypeEnum.Standard)
                    OnImageFound(FileName, FileName, ft, NewFileName:=NewFileName)
                End If
            End If
        End If
    End Sub

    Private Sub ProcessZipArchive(Archive As ZipArchive, Filename As String, ct As CancellationToken, Optional InnerPath As String = Nothing)
        Dim Entries As ReadOnlyCollection(Of ZipArchiveEntry) = Nothing
        Try : Entries = Archive.Entries : Catch : End Try

        If Entries Is Nothing Then
            Return
        End If

        Dim HasFilter = Not String.IsNullOrEmpty(InnerPath)

        For Each Entry In Entries.OrderBy(Function(e) e.FullName)
            If ct.IsCancellationRequested Then
                Exit For
            End If

            If Entry.Name.Length = 0 Then
                Continue For
            End If

            Dim EntryFileExt = Path.GetExtension(Entry.Name).ToLower()
            If AllFileExtensions.Contains(EntryFileExt) Then
                Dim FilePath = Path.Combine(Filename, Entry.FullName)

                If HasFilter AndAlso Entry.FullName <> InnerPath Then
                    Continue For
                End If

                If IsValidFileLength(Entry.Length, EntryFileExt) Then
                    OnImageFound(FilePath, Filename, ImageData.FileTypeEnum.Compressed, CompressedFile:=Entry.FullName)
                End If
            End If
        Next
    End Sub

    Private Function SplitFilePath(FilePath As String) As (FilePath As String, InnerPath As String)
        Dim p = FilePath.IndexOf("|"c)
        If p < 0 Then
            Return (FilePath, Nothing)
        End If

        Return (FilePath.Substring(0, p), FilePath.Substring(p + 1))
    End Function
End Class

#Region "Event Classes"
Public Class ImageDiscoveredEventArgs
    Inherits EventArgs

    Public Sub New(Key As String, FileName As String, FileType As ImageData.FileTypeEnum, Optional CompressedFile As String = "", Optional NewFileName As String = "")
        Me.Key = Key
        Me.FileName = FileName
        Me.FileType = FileType
        Me.CompressedFile = CompressedFile
        Me.NewFileName = NewFileName
    End Sub

    Public Property CompressedFile As String
    Public Property FileName As String
    Public Property FileType As ImageData.FileTypeEnum
    Public Property Key As String
    Public Property NewFileName As String
End Class

Public Class ScanCompletedEventArgs
    Inherits EventArgs

    Public Sub New(Cancelled As Boolean, [Error] As Exception)
        Me.Cancelled = Cancelled
        Me.[Error] = [Error]
    End Sub

    Public Property [Error] As Exception
    Public Property Cancelled As Boolean
End Class
#End Region

