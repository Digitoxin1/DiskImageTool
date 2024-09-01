Public Module FilterMethods
    Public Delegate Sub FilterChangedEventHandler()

    Public Enum FilterTypes
        ModifiedFiles
        Disk_UnknownFormat
        Disk_CustomFormat
        Disk_NOBPB
        DIsk_CustomBootLoader
        Disk_MismatchedImageSize
        Disk_MismatchedMediaDescriptor
        Disk_FreeClustersWithData
        Bootstrap_Unknown
        OEMName_Unknown
        OEMName_Mismatched
        OEMName_Windows9x
        OEMName_Verified
        OEMName_Unverified
        FileSystem_HasCreationDate
        FileSystem_HasLastAccessDate
        FileSystem_HasReservedBytesSet
        FileSystem_HasLongFileNames
        FileSystem_InvalidDirectoryEntries
        FileSystem_DirectoryHasAdditionalData
        FileSystem_DirectoryHasBootSector
        FAT_BadSectors
        FAT_LostClusters
        FAT_MismatchedFATs
        FAT_ChainingErrors
        Image_InDatabase
        Image_NotInDatabase
        Image_Verified
        Image_Unverified
        Database_MismatchedStatus
    End Enum

    Public Function FilterGetCount() As Integer
        Return [Enum].GetNames(GetType(FilterTypes)).Length
    End Function

    Public Class FilterCounts
        Public Sub New()
            _Total = 0
            _Available = 0
        End Sub
        Public Property Total As Integer
        Public Property Available As Integer
    End Class

    Public Class FilterTag
        Public Sub New(Value As Integer)
            _Value = Value
            _Visible = False
        End Sub
        Public ReadOnly Property Value As Integer
        Public Property Visible As Boolean
    End Class

    Public Class ImageFilters
        Private ReadOnly _ContextMenuFilters As ContextMenuStrip
        Private _SuppressEvent As Boolean = False

        Public Event FilterChanged As FilterChangedEventHandler

        Public Sub New(ContextMenuFilters As ContextMenuStrip)
            _ContextMenuFilters = ContextMenuFilters
            _FiltersApplied = False
            Initialize()
        End Sub

        Public Property FilterCounts As FilterCounts()
        Public Property FiltersApplied As Boolean

        Private Shared Function FilterGetCaption(ID As FilterTypes, Count As Integer) As String
            Dim Caption As String

            Select Case ID
                Case FilterTypes.ModifiedFiles
                    Caption = "Modified Files"
                Case FilterTypes.Disk_UnknownFormat
                    Caption = "Disk - Unknown Format"
                Case FilterTypes.Disk_CustomFormat
                    Caption = "Disk - Custom Format"
                Case FilterTypes.Disk_NOBPB
                    Caption = "Disk - No BPB"
                Case FilterTypes.DIsk_CustomBootLoader
                    Caption = "Disk - Custom Boot Loader"
                Case FilterTypes.Disk_MismatchedImageSize
                    Caption = "Disk - Mismatched Image Size"
                Case FilterTypes.Disk_MismatchedMediaDescriptor
                    Caption = "Disk - Mismatched Media Descriptor"
                Case FilterTypes.Disk_FreeClustersWithData
                    Caption = "Disk - Free Clusters with Data"
                Case FilterTypes.Bootstrap_Unknown
                    Caption = "Bootstrap - Unknown"
                Case FilterTypes.OEMName_Unknown
                    Caption = "OEM Name - Unknown"
                Case FilterTypes.OEMName_Mismatched
                    Caption = "OEM Name - Mismatched"
                Case FilterTypes.OEMName_Windows9x
                    Caption = "OEM Name - Windows 9x"
                Case FilterTypes.OEMName_Verified
                    Caption = "OEM Name - Verified"
                Case FilterTypes.OEMName_Unverified
                    Caption = "OEM Name - Unverified"
                Case FilterTypes.FileSystem_HasCreationDate
                    Caption = "File System - Has Creation Date"
                Case FilterTypes.FileSystem_HasLastAccessDate
                    Caption = "File System - Has Last Access Date"
                Case FilterTypes.FileSystem_HasReservedBytesSet
                    Caption = "File System - Has Reserved Bytes Set"
                Case FilterTypes.FileSystem_HasLongFileNames
                    Caption = "File System - Has Long File Names"
                Case FilterTypes.FileSystem_InvalidDirectoryEntries
                    Caption = "File System - Invalid Directory Entries"
                Case FilterTypes.FileSystem_DirectoryHasAdditionalData
                    Caption = "File System - Directory has Additional Data"
                Case FilterTypes.FileSystem_DirectoryHasBootSector
                    Caption = "File System - Directory has Boot Sector"
                Case FilterTypes.FAT_BadSectors
                    Caption = "FAT - Bad Sectors"
                Case FilterTypes.FAT_LostClusters
                    Caption = "FAT - Lost Clusters"
                Case FilterTypes.FAT_MismatchedFATs
                    Caption = "FAT - Mismatched FATs"
                Case FilterTypes.FAT_ChainingErrors
                    Caption = "FAT - Chaining Errors"
                Case FilterTypes.Image_InDatabase
                    Caption = "Image - In Database"
                Case FilterTypes.Image_NotInDatabase
                    Caption = "Image - Not in Database"
                Case FilterTypes.Image_Verified
                    Caption = "Image - Verified"
                Case FilterTypes.Image_Unverified
                    Caption = "Image - Unverified"
                Case FilterTypes.Database_MismatchedStatus
                    Caption = "Database - Mismatched Status"
                Case Else
                    Caption = ""
            End Select
            If Count > 0 Then
                Caption &= "  [" & Count & "]"
            End If

            Return Caption
        End Function

        Private Shared Function FilterCheck(FilterType As FilterTypes, AppliedFilters As Integer) As Boolean
            Return (AppliedFilters And (2 ^ FilterType)) > 0
        End Function

        Public Shared Function IsFiltered(ImageData As LoadedImageData, AppliedFilters As Integer, ByRef FilterCounts() As FilterCounts) As Boolean
            Dim Result = False
            ImageData.AppliedFilters = 0

            Dim FilterCount As Integer = FilterGetCount()

            For FilterType As FilterTypes = 0 To FilterCount - 1
                If Not ImageData.Filter(FilterType) Then
                    ImageData.AppliedFilters += (2 ^ FilterType)
                    If FilterCheck(FilterType, AppliedFilters) Then
                        Result = True
                    End If
                End If
            Next

            If Not Result Then
                For FilterType As FilterTypes = 0 To FilterCount - 1
                    If Not FilterCheck(FilterType, ImageData.AppliedFilters) Then
                        FilterCounts(FilterType).Available += 1
                    End If
                Next
            End If

            Return Result
        End Function

        Public Function AreFiltersApplied() As Boolean
            Dim FilterCount As Integer = FilterGetCount()

            For Counter = 0 To FilterCount - 1
                Dim Item As ToolStripMenuItem = _ContextMenuFilters.Items("key_" & Counter)
                If Item.Checked Then
                    Return True
                    Exit For
                End If
            Next

            Return False
        End Function

        Public Sub Clear()
            _SuppressEvent = True
            For Counter = 0 To FilterGetCount() - 1
                Dim Item As ToolStripMenuItem = _ContextMenuFilters.Items("key_" & Counter)
                If Item.CheckState = CheckState.Checked Then
                    Item.CheckState = CheckState.Unchecked
                End If
                _FilterCounts(Counter).Available = _FilterCounts(Counter).Total
            Next
            _SuppressEvent = False

            _FiltersApplied = False
        End Sub

        Public Sub FilterUpdate(ImageData As LoadedImageData, UpdateFilters As Boolean, FilterType As FilterTypes, Value As Boolean)
            FilterUpdate(ImageData, UpdateFilters, FilterType, Value, ImageData.Scanned)
        End Sub

        Public Sub FilterUpdate(ImageData As LoadedImageData, UpdateFilters As Boolean, FilterType As FilterTypes, Value As Boolean, Scanned As Boolean)
            If Not Scanned Or Value <> ImageData.Filter(FilterType) Then
                ImageData.Filter(FilterType) = Value
                If Value Then
                    _FilterCounts(FilterType).Total += 1
                    _FilterCounts(FilterType).Available += 1
                ElseIf Scanned Then
                    _FilterCounts(FilterType).Total -= 1
                    _FilterCounts(FilterType).Available -= 1
                End If
                If UpdateFilters Then
                    UpdateMenuItem(FilterType)
                End If
            End If
        End Sub

        Public Function GetAppliedFilters(ClearAvailable As Boolean) As Integer
            Dim AppliedFilters As Integer = 0
            Dim FilterCount As Integer = FilterGetCount()

            For Counter = 0 To FilterCount - 1
                Dim Item As ToolStripMenuItem = _ContextMenuFilters.Items("key_" & Counter)
                If Item.Checked Then
                    AppliedFilters += CType(Item.Tag, FilterTag).Value
                End If
                If ClearAvailable Then
                    _FilterCounts(Counter).Available = 0
                End If
            Next

            Return AppliedFilters
        End Function

        Private Sub Initialize()
            Dim Separator = New ToolStripSeparator With {
                .Name = "FilterSeparator",
                .Visible = False,
                .Tag = 0
            }
            _ContextMenuFilters.Items.Add(Separator)

            Dim FilterCount As Integer = FilterGetCount()
            ReDim _FilterCounts(FilterCount - 1)
            For Counter = 0 To FilterCount - 1
                Dim Item = New ToolStripMenuItem With {
                    .Text = FilterGetCaption(Counter, 0),
                    .CheckOnClick = True,
                    .Name = "key_" & Counter,
                    .Visible = False,
                    .Enabled = False,
                    .Tag = New FilterTag(2 ^ Counter)
                }
                AddHandler Item.CheckStateChanged, AddressOf ContextMenuFilters_CheckStateChanged
                _ContextMenuFilters.Items.Add(Item)
            Next
        End Sub

        Public Sub Reset()
            For Counter = 0 To _FilterCounts.Length - 1
                If _FilterCounts(Counter) Is Nothing Then
                    _FilterCounts(Counter) = New FilterCounts
                Else
                    _FilterCounts(Counter).Total = 0
                    _FilterCounts(Counter).Available = 0
                End If
                UpdateMenuItem(Counter)
            Next
        End Sub

        Public Sub UpdateAllMenuItems()
            For Counter = 0 To FilterGetCount() - 1
                UpdateMenuItem(Counter)
            Next
        End Sub

        Public Function UpdateMenuItem(ID As FilterTypes) As Boolean
            Dim Count As Integer = _FilterCounts(ID).Total
            Dim Available As Integer = _FilterCounts(ID).Available
            Dim Item As ToolStripMenuItem = _ContextMenuFilters.Items("key_" & ID)
            Dim ItemTag = CType(Item.Tag, FilterTag)
            Dim Enabled As Boolean = (Available > 0 And Count > 0)
            Dim Visible As Boolean = (Count > 0)
            Dim CheckstateChanged As Boolean = False

            Item.Text = FilterGetCaption(ID, Available)

            If Visible <> ItemTag.Visible Then
                Item.Visible = Visible
                ItemTag.Visible = Visible

                With _ContextMenuFilters.Items.Item("FilterSeparator")
                    .Tag = .Tag + IIf(Visible, 1, -1)
                    .Visible = (.Tag > 0)
                End With
            End If

            If Enabled <> Item.Enabled Then
                Item.Enabled = Enabled
                If Not Enabled And Item.CheckState = CheckState.Checked Then
                    Item.CheckState = CheckState.Unchecked
                    CheckstateChanged = True
                End If
            End If

            Return CheckstateChanged
        End Function

        Private Sub ContextMenuFilters_CheckStateChanged(sender As Object, e As EventArgs)
            If _SuppressEvent Then
                Exit Sub
            End If

            RaiseEvent FilterChanged()
        End Sub
    End Class
End Module