Imports DiskImageTool.DiskImage

Public Class FATTables
    Private _BPB As BiosParameterBlock
    Private _FAT12() As FAT12
    Private _FATIndex As UShort
    Private ReadOnly _FileBytes As ImageByteArray
    Private _SyncFATs As Boolean
    Private _FATsMatch As Boolean

    Sub New(BPB As BiosParameterBlock, FileBytes As ImageByteArray, FATIndex As UShort)
        _BPB = BPB
        _FileBytes = FileBytes
        _FATIndex = FATIndex

        Dim FATCount = _BPB.NumberOfFATs
        If FATCount = 0 Then
            FATCount = 1
        ElseIf FATCount > 2 Then
            FATCount = 2
        End If

        ReDim _FAT12(FATCount - 1)
        For Counter = 0 To FATCount - 1
            _FAT12(Counter) = New FAT12(_FileBytes, _BPB, Counter)
        Next
        _FATsMatch = CompareFATTables()
        _SyncFATs = True
    End Sub

    Public Property FATIndex As UShort
        Get
            Return _FATIndex
        End Get
        Set
            _FATIndex = Value
        End Set
    End Property

    Public ReadOnly Property FATsMatch As Boolean
        Get
            Return _FATsMatch
        End Get
    End Property


    Public Property SyncFATs As Boolean
        Get
            Return _SyncFATs
        End Get
        Set
            _SyncFATs = Value
        End Set
    End Property

    Private Function CompareFATTables() As Boolean
        If _BPB.NumberOfFATs < 2 Then
            Return True
        End If

        For Counter As UShort = 1 To _BPB.NumberOfFATs - 1
            Dim FATCopy1 = _FAT12(Counter - 1).GetFAT
            Dim FATCopy2 = _FAT12(Counter).GetFAT

            If Not FATCopy1.CompareTo(FATCopy2) Then
                Return False
            End If
        Next

        Return True
    End Function

    Public Sub UpdateFAT12()
        UpdateFAT12(_SyncFATs)
    End Sub

    Public Function UpdateFAT12(SyncFATs As Boolean) As Boolean
        Return UpdateFAT12(SyncFATs, False, 0)
    End Function

    Public Function UpdateFAT12(SyncFATs As Boolean, MediaDescriptor As Byte) As Boolean
        Return UpdateFAT12(SyncFATs, True, MediaDescriptor)
    End Function

    Private Function UpdateFAT12(SyncFATs As Boolean, UpdateMediaDescriptor As Boolean, MediaDescriptor As Byte) As Boolean
        Dim Start As UShort
        Dim Count As UShort
        Dim Result As Boolean = False

        If SyncFATs Then
            Start = 0
            Count = _FAT12.Length
        Else
            Start = _FATIndex
            Count = 1
        End If

        Dim UseBatchEditMode As Boolean = Not _FileBytes.BatchEditMode And Count > 1

        If UseBatchEditMode Then
            _FileBytes.BatchEditMode = True
        End If

        For Counter = Start To Start + Count - 1
            If _FAT12(Counter).UpdateFAT12() Then
                Result = True
            End If
            If UpdateMediaDescriptor Then
                If _FAT12(Counter).UpdateMedaDescriptor(MediaDescriptor) Then
                    Result = True
                End If
            End If
        Next

        If UseBatchEditMode Then
            _FileBytes.BatchEditMode = False
        End If

        If Result Then
            _FATsMatch = CompareFATTables()
        End If

        Return Result
    End Function

    Public Sub UpdateTableEntry(Cluster As UShort, Value As UShort)
        UpdateTableEntry(Cluster, Value, _SyncFATs)
    End Sub

    Public Sub UpdateTableEntry(Cluster As UShort, Value As UShort, SyncFATs As Boolean)
        Dim Start As UShort
        Dim Count As UShort

        If SyncFATs Then
            Start = 0
            Count = _FAT12.Length
        Else
            Start = _FATIndex
            Count = 1
        End If

        For Counter = Start To Start + Count - 1
            _FAT12(Counter).TableEntry(Cluster) = Value
        Next
    End Sub

    Public Function FAT() As FAT12
        Return _FAT12(_FATIndex)
    End Function

    Public Function FAT(Index As UShort) As FAT12
        Return _FAT12(Index)
    End Function

    Public Sub Reinitialize(BPB As BiosParameterBlock)
        _BPB = BPB

        Dim OldFATCount = _FAT12.Length
        Dim FATCount = _BPB.NumberOfFATs
        If FATCount = 0 Then
            FATCount = 1
        ElseIf FATCount > 2 Then
            FATCount = 2
        End If

        If OldFATCount <> FATCount Then
            ReDim Preserve _FAT12(FATCount - 1)
        End If
        For Counter = 0 To FATCount - 1
            If Counter > OldFATCount - 1 Then
                _FAT12(Counter) = New FAT12(_FileBytes, _BPB, Counter)
            Else
                _FAT12(Counter).PopulateFAT12(_BPB)
            End If
        Next
        _FATsMatch = CompareFATTables()
    End Sub
End Class
