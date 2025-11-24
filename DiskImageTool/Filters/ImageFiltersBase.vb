Namespace Filters
    Public Delegate Sub FilterChangedEventHandler(ResetSubFilters As Boolean)
    Public Class ImageFiltersBase
        Implements IDisposable

        Private ReadOnly _ContextMenuFilters As ContextMenuStrip
        Private ReadOnly _eventHandlers As New List(Of ToolStripMenuItem)
        Private _disposed As Boolean = False
        Private _SuppressEvent As Boolean = False
        Public Event FilterChanged As FilterChangedEventHandler
        Public Sub New(ContextMenuFilters As ContextMenuStrip)
            _ContextMenuFilters = ContextMenuFilters
            _FiltersApplied = False
            Initialize()
        End Sub

        Public Property FilterCounts As FilterCounts()
        Public Property FiltersApplied As Boolean

        Public Shared Function IsFiltered(ImageData As ImageData, AppliedFilters As Long, ByRef FilterCounts() As FilterCounts) As Boolean
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

        Public Overridable Sub Clear()
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

        Public Overridable Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then
                Return
            End If

            _disposed = True

            ' Detach handlers safely
            For Each item In _eventHandlers
                Try
                    RemoveHandler item.CheckStateChanged, AddressOf ContextMenuFilters_CheckStateChanged
                Catch
                    ' Swallow in Dispose
                End Try
            Next

            GC.SuppressFinalize(Me)
        End Sub

        Public Sub FilterUpdate(ImageData As ImageData, UpdateFilters As Boolean, FilterType As FilterTypes, Value As Boolean)
            FilterUpdate(ImageData, UpdateFilters, FilterType, Value, ImageData.Scanned)
        End Sub

        Public Sub FilterUpdate(ImageData As ImageData, UpdateFilters As Boolean, FilterType As FilterTypes, Value As Boolean, Scanned As Boolean)
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

        Public Function GetAppliedFilters(ClearAvailable As Boolean) As Long
            Dim AppliedFilters As Long = 0
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

        Public Overridable Sub Reset()
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

            If Visible Then
                Item.Text = FilterGetCaption(ID, Available)
            Else
                Item.Text = ""
            End If

            If Visible <> ItemTag.Visible Then
                Item.Visible = Visible
                ItemTag.Visible = Visible

                With _ContextMenuFilters.Items.Item("FilterSeparator")
                    .Tag = .Tag + If(Visible, 1, -1)
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

        Protected Overridable Sub OnFilterChanged(ResetSubFilters As Boolean)
            RaiseEvent FilterChanged(ResetSubFilters)
        End Sub

        Private Shared Function FilterCheck(FilterType As FilterTypes, AppliedFilters As Long) As Boolean
            Return (AppliedFilters And (2 ^ FilterType)) > 0
        End Function

        Private Shared Function FilterGetCaption(ID As FilterTypes, Count As Integer) As String
            Dim Caption As String

            Select Case ID
                Case FilterTypes.ModifiedFiles
                    Caption = My.Resources.Filter_ModifiedFiles
                Case FilterTypes.Disk_UnknownFormat
                    Caption = My.Resources.Label_Disk & " - " & My.Resources.Filter_Disk_UnknownFormat
                Case FilterTypes.Disk_CustomFormat
                    Caption = My.Resources.Label_Disk & " - " & My.Resources.Filter_Disk_CustomFormat
                Case FilterTypes.Disk_NOBPB
                    Caption = My.Resources.Label_Disk & " - " & My.Resources.Filter_Disk_NOBPB
                Case FilterTypes.Disk_NoBootLoader
                    Caption = My.Resources.Label_Disk & " - " & My.Resources.Filter_Disk_NoBootLoader
                Case FilterTypes.Disk_CustomBootLoader
                    Caption = My.Resources.Label_Disk & " - " & My.Resources.Filter_Disk_CustomBootLoader
                Case FilterTypes.Disk_MismatchedImageSize
                    Caption = My.Resources.Label_Disk & " - " & My.Resources.Filter_Disk_MismatchedImageSize
                Case FilterTypes.Disk_MismatchedMediaDescriptor
                    Caption = My.Resources.Label_Disk & " - " & My.Resources.Filter_Disk_MismatchedMediaDescriptor
                Case FilterTypes.Disk_FreeClustersWithData
                    Caption = My.Resources.Label_Disk & " - " & My.Resources.Filter_Disk_FreeClustersWithData
                Case FilterTypes.Disk_HasWriteSplices
                    Caption = My.Resources.Label_Disk & " - " & My.Resources.Filter_Disk_HasWriteSplices
                Case FilterTypes.Bootstrap_Unknown
                    Caption = My.Resources.Filter_Bootstrap_Unknown
                Case FilterTypes.OEMName_Unknown
                    Caption = My.Resources.Label_OEMName & " - " & My.Resources.Filter_OEMName_Unknown
                Case FilterTypes.OEMName_Mismatched
                    Caption = My.Resources.Label_OEMName & " - " & My.Resources.Filter_OEMName_Mismatched
                Case FilterTypes.OEMName_Windows9x
                    Caption = My.Resources.Label_OEMName & " - " & My.Resources.Filter_OEMName_Windows9x
                Case FilterTypes.OEMName_Verified
                    Caption = My.Resources.Label_OEMName & " - " & My.Resources.Filter_OEMName_Verified
                Case FilterTypes.OEMName_Unverified
                    Caption = My.Resources.Label_OEMName & " - " & My.Resources.Filter_OEMName_Unverified
                Case FilterTypes.FileSystem_HasCreationDate
                    Caption = My.Resources.Filter_FileSystem_HasCreationDate
                Case FilterTypes.FileSystem_HasLastAccessDate
                    Caption = My.Resources.Filter_FileSystem_HasLastAccessDate
                Case FilterTypes.FileSystem_HasReservedBytesSet
                    Caption = My.Resources.Filter_FileSystem_HasReservedBytesSet
                Case FilterTypes.FileSystem_HasLongFileNames
                    Caption = My.Resources.Filter_FileSystem_HasLongFileNames
                Case FilterTypes.FileSystem_InvalidDirectoryEntries
                    Caption = My.Resources.Filter_FileSystem_InvalidDirectoryEntries
                Case FilterTypes.FileSystem_DirectoryHasAdditionalData
                    Caption = My.Resources.Filter_FileSystem_DirectoryHasAdditionalData
                Case FilterTypes.FileSystem_DirectoryHasBootSector
                    Caption = My.Resources.Filter_FileSystem_DirectoryHasBootSector
                Case FilterTypes.FAT_BadSectors
                    Caption = My.Resources.Filter_FAT_BadSectors
                Case FilterTypes.FAT_LostClusters
                    Caption = My.Resources.Filter_FAT_LostClusters
                Case FilterTypes.FAT_MismatchedFATs
                    Caption = My.Resources.Filter_FAT_MismatchedFATs
                Case FilterTypes.FAT_ChainingErrors
                    Caption = My.Resources.Filter_FAT_ChainingErrors
                Case FilterTypes.Image_InDatabase
                    Caption = My.Resources.Filter_Image_InDatabase
                Case FilterTypes.Image_NotInDatabase
                    Caption = My.Resources.Filter_Image_NotInDatabase
                Case FilterTypes.Image_Verified
                    Caption = My.Resources.Filter_Image_Verified
                Case FilterTypes.Image_Unverified
                    Caption = My.Resources.Filter_Image_Unverified
                Case FilterTypes.Database_MismatchedStatus
                    Caption = My.Resources.Filter_Database_MismatchedStatus
                Case Else
                    Caption = ""
            End Select
            If Count > 0 Then
                Caption &= "  [" & Count & "]"
            End If

            Return Caption
        End Function
        Private Sub ContextMenuFilters_CheckStateChanged(sender As Object, e As EventArgs)
            If _SuppressEvent Then
                Exit Sub
            End If

            RaiseEvent FilterChanged(True)
        End Sub

        Private Sub Initialize()
            Dim Separator As New ToolStripSeparator With {
                .Name = "FilterSeparator",
                .Visible = False,
                .Tag = 0
            }
            _ContextMenuFilters.Items.Add(Separator)

            Dim FilterCount As Integer = FilterGetCount()
            ReDim _FilterCounts(FilterCount - 1)
            For Counter = 0 To FilterCount - 1
                Dim Item As New ToolStripMenuItem With {
                    .Text = "",
                    .CheckOnClick = True,
                    .Name = "key_" & Counter,
                    .Visible = False,
                    .Enabled = False,
                    .Tag = New FilterTag(2 ^ Counter)
                }
                AddHandler Item.CheckStateChanged, AddressOf ContextMenuFilters_CheckStateChanged
                _ContextMenuFilters.Items.Add(Item)
                _eventHandlers.Add(Item)
            Next
        End Sub
    End Class
End Namespace
