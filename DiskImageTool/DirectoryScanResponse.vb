Imports DiskImageTool.DiskImage

Public Class DirectoryScanResponse
    Private ReadOnly _FileNames As HashSet(Of String)

    Private _VolumeNameFound As Boolean

    Public Sub New(Directory As DiskImage.IDirectory)
        If Directory Is Nothing Then
            _HasAdditionalData = False
            _HasBootSector = False
        Else
            _HasAdditionalData = Directory.Data.HasAdditionalData
            _HasBootSector = Directory.Data.HasBootSector
        End If
        _HasCreated = False
        _HasValidCreated = False
        _HasLFN = False
        _HasLastAccessed = False
        _HasValidLastAccessed = False
        _HasInvalidDirectoryEntries = False
        _HasFATChainingErrors = False
        _HasNTReserved = False
        _HasNTUnknownFlags = False
        _HasFAT32Cluster = False
        _ItemCount = 0
        _FileNames = New HashSet(Of String)
        _VolumeNameFound = False
    End Sub

    Public Property HasAdditionalData As Boolean

    Public Property HasBootSector As Boolean

    Public Property HasCreated As Boolean

    Public Property HasFAT32Cluster As Boolean

    Public Property HasFATChainingErrors As Boolean

    Public Property HasInvalidDirectoryEntries As Boolean

    Public Property HasLastAccessed As Boolean

    Public Property HasLFN As Boolean

    Public Property HasNTReserved As Boolean

    Public Property HasNTUnknownFlags As Boolean

    Public Property HasValidCreated As Boolean

    Public Property HasValidLastAccessed As Boolean

    Public Property ItemCount As Integer

    Public Sub Combine(Response As DirectoryScanResponse)
        _HasLastAccessed = _HasLastAccessed Or Response.HasLastAccessed
        _HasValidLastAccessed = _HasValidLastAccessed Or Response.HasValidLastAccessed
        _HasCreated = _HasCreated Or Response.HasCreated
        _HasValidCreated = _HasValidCreated Or Response.HasValidCreated
        _HasLFN = _HasLFN Or Response.HasLFN
        _HasInvalidDirectoryEntries = _HasInvalidDirectoryEntries Or Response.HasInvalidDirectoryEntries
        _HasFATChainingErrors = _HasFATChainingErrors Or Response.HasFATChainingErrors
        _HasAdditionalData = _HasAdditionalData Or Response.HasAdditionalData
        _HasBootSector = _HasBootSector Or Response.HasBootSector
        _HasNTReserved = _HasNTReserved Or Response.HasNTReserved
        _HasNTUnknownFlags = _HasNTUnknownFlags Or Response.HasNTUnknownFlags
        _HasFAT32Cluster = _HasFAT32Cluster Or Response.HasFAT32Cluster
    End Sub

    Public Function ProcessDirectoryEntry(DirectoryEntry As DirectoryEntry, LFNFileName As String, IsRootDirectory As Boolean) As ProcessDirectoryEntryResponse
        Dim Response As ProcessDirectoryEntryResponse
        Response.DuplicateFileName = False
        Response.InvalidVolumeName = False

        Dim IsValid = DirectoryEntry.IsValid
        Dim IsBlank = DirectoryEntry.IsBlank
        Dim IsDeleted = DirectoryEntry.IsDeleted
        Dim HasCreationDate = DirectoryEntry.HasCreationDate
        Dim HasLastAccessDate = DirectoryEntry.HasLastAccessDate
        Dim IsVolumeName = DirectoryEntry.IsVolumeName
        Dim HasInvalidFilename = DirectoryEntry.HasInvalidFilename
        Dim HasInvalidExtension = DirectoryEntry.HasInvalidExtension

        If Not IsDeleted AndAlso Not IsBlank Then
            If IsVolumeName Then
                If IsRootDirectory Then
                    If DirectoryEntry.IsValidVolumeName Then
                        If _VolumeNameFound Then
                            _HasInvalidDirectoryEntries = True
                            Response.InvalidVolumeName = True
                        Else
                            _VolumeNameFound = True
                        End If
                    End If
                Else
                    _HasInvalidDirectoryEntries = True
                    Response.InvalidVolumeName = True
                End If
            ElseIf Not HasInvalidFilename AndAlso Not HasInvalidExtension Then
                Dim FileName = DirectoryEntry.GetFullFileName
                If Not _FileNames.Contains(FileName) Then
                    _FileNames.Add(FileName)
                Else
                    _HasInvalidDirectoryEntries = True
                    Response.DuplicateFileName = True
                End If
            End If
        End If

        Dim HasValidCreationDate = IsValid AndAlso HasCreationDate AndAlso DirectoryEntry.GetCreationDate.IsValidDate
        'AndAlso Not File.HasVendorExceptions

        Dim HasValidLastAccessDate = IsValid AndAlso HasLastAccessDate AndAlso DirectoryEntry.GetLastAccessDate.IsValidDate
        'AndAlso Not File.HasVendorExceptions

        If Not _HasInvalidDirectoryEntries Then
            If Not IsBlank And Not IsDeleted Then
                If Not IsValid _
                    OrElse HasInvalidFilename _
                    OrElse HasInvalidExtension _
                    OrElse DirectoryEntry.HasIncorrectFileSize _
                    OrElse (DirectoryEntry.IsVolumeName And Not DirectoryEntry.IsValidVolumeName) _
                    OrElse Not DirectoryEntry.GetLastWriteDate.IsValidDate Then
                    _HasInvalidDirectoryEntries = True
                End If
            End If
        End If

        If Not _HasInvalidDirectoryEntries Then
            If Not IsBlank AndAlso Not IsDeleted Then
                If (HasCreationDate And Not HasValidCreationDate) Or (HasLastAccessDate And Not HasValidLastAccessDate) Then
                    _HasInvalidDirectoryEntries = True
                End If
            End If
        End If

        If Not IsBlank And HasCreationDate Then
            _HasCreated = True
        End If
        If HasValidCreationDate Then
            _HasValidCreated = True
        End If
        If Not IsBlank And HasLastAccessDate Then
            _HasLastAccessed = True
        End If
        If HasValidLastAccessDate Then
            _HasValidLastAccessed = True
        End If

        If Not _HasFATChainingErrors Then
            If Not IsBlank And (DirectoryEntry.HasCircularChain Or DirectoryEntry.IsCrossLinked) Then
                _HasFATChainingErrors = True
            End If
        End If

        If Not _HasNTReserved Then
            If IsValid And Not IsBlank And DirectoryEntry.ReservedForWinNT <> 0 Then
                _HasNTReserved = True
            End If
        End If

        If Not _HasNTUnknownFlags Then
            If IsValid And Not IsBlank And DirectoryEntry.HasNTUnknownFlags <> 0 Then
                _HasNTUnknownFlags = True
            End If
        End If

        If Not _HasFAT32Cluster Then
            If IsValid And Not IsBlank And DirectoryEntry.ReservedForFAT32 <> 0 Then
                _HasFAT32Cluster = True
            End If
        End If

        If Not _HasLFN Then
            If LFNFileName <> "" Then
                _HasLFN = True
            End If
        End If

        Return Response
    End Function

    Public Structure ProcessDirectoryEntryResponse
        Dim DuplicateFileName As Boolean
        Dim InvalidVolumeName As Boolean
    End Structure

End Class