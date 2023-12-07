Imports DiskImageTool.DiskImage

Public Class DirectoryScanResponse
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
        _HasReserved = False
    End Sub

    Public Property HasAdditionalData As Boolean
    Public Property HasBootSector As Boolean
    Public Property HasCreated As Boolean
    Public Property HasFATChainingErrors As Boolean
    Public Property HasInvalidDirectoryEntries As Boolean
    Public Property HasLastAccessed As Boolean
    Public Property HasLFN As Boolean
    Public Property HasValidCreated As Boolean
    Public Property HasValidLastAccessed As Boolean
    Public Property HasReserved As Boolean
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
        _HasReserved = _HasReserved Or Response.HasReserved
    End Sub

    Public Sub ProcessDirectoryEntry(DirectoryEntry As DirectoryEntry, LFNFileName As String)
        Dim IsValid = DirectoryEntry.IsValid
        Dim IsBlank = DirectoryEntry.IsBlank
        Dim HasCreationDate = DirectoryEntry.HasCreationDate
        Dim HasLastAccessDate = DirectoryEntry.HasLastAccessDate

        Dim HasValidCreationDate = IsValid AndAlso HasCreationDate AndAlso DirectoryEntry.GetCreationDate.IsValidDate
        'AndAlso Not File.HasVendorExceptions

        Dim HasValidLastAccessDate = IsValid AndAlso HasLastAccessDate AndAlso DirectoryEntry.GetLastAccessDate.IsValidDate
        'AndAlso Not File.HasVendorExceptions

        If Not _HasInvalidDirectoryEntries Then
            If Not IsBlank And Not DirectoryEntry.IsDeleted Then
                If Not IsValid _
                    OrElse DirectoryEntry.HasInvalidFilename _
                    OrElse DirectoryEntry.HasInvalidExtension _
                    OrElse DirectoryEntry.HasIncorrectFileSize _
                    OrElse (DirectoryEntry.IsVolumeName And Not DirectoryEntry.IsValidValumeName) _
                    OrElse Not DirectoryEntry.GetLastWriteDate.IsValidDate Then
                    _HasInvalidDirectoryEntries = True
                End If
            End If
        End If

        If Not _HasInvalidDirectoryEntries Then
            If Not IsBlank AndAlso Not DirectoryEntry.IsDeleted Then
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

        If Not _HasReserved Then
            If Not IsBlank And (DirectoryEntry.ReservedForWinNT <> 0 Or DirectoryEntry.ReservedForFAT32 <> 0) Then
                _HasReserved = True
            End If
        End If

        If Not _HasLFN Then
            If LFNFileName <> "" Then
                _HasLFN = True
            End If
        End If
    End Sub
End Class
