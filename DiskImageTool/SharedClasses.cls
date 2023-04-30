Imports DiskImageTool.DiskImage
Imports Hb.Windows.Forms

Public Class BootstrapLookup
    Public Property ExactMatch As Boolean = False
    Public Property KnownOEMNames As New List(Of KnownOEMName)
    Public Property Language As String = "English"
End Class

Public Class HexViewData
    Private ReadOnly _HexViewSectorData As HexViewSectorData
    Public Sub New(HexViewSectorData As HexViewSectorData, Index As Integer)
        _HexViewSectorData = HexViewSectorData
        _Index = Index
        _SectorBlock = _HexViewSectorData.SectorData.GetBlock(Index)
        _ByteProvider = New MyByteProvider(_HexViewSectorData.SectorData.Data, _SectorBlock.Offset, _SectorBlock.Size)
    End Sub
    Public ReadOnly Property Disk As DiskImage.Disk
        Get
            Return _HexViewSectorData.Disk
        End Get
    End Property
    Public ReadOnly Property ByteProvider As IByteProvider
    Public ReadOnly Property Index As Integer
    Public ReadOnly Property SectorBlock As SectorBlock

    Public Overrides Function ToString() As String
        Dim Sector = _SectorBlock.SectorStart
        Dim SectorEnd = _SectorBlock.SectorStart + _SectorBlock.SectorCount - 1

        Dim Cluster As UShort
        Dim ClusterEnd As UShort
        If _HexViewSectorData.Disk.IsValidImage Then
            Cluster = _HexViewSectorData.Disk.BootSector.SectorToCluster(Sector)
            ClusterEnd = _HexViewSectorData.Disk.BootSector.SectorToCluster(SectorEnd)
        Else
            Cluster = 0
            ClusterEnd = 0
        End If

        Dim Header As String = "Sector " & Sector & IIf(SectorEnd > Sector, "-" & SectorEnd, "")
        If Cluster > 0 Then
            Header = "Cluster " & Cluster & IIf(ClusterEnd > Cluster, "-" & ClusterEnd, "") & "; " & Header
        End If
        If _SectorBlock.Description <> "" Then
            Header = _SectorBlock.Description & "  (" & Header & ")"
        End If

        Return Header
    End Function
End Class

Public Class HexViewHighlightRegion
    Implements IEquatable(Of HexViewHighlightRegion)
    Implements IComparable(Of HexViewHighlightRegion)

    Public Sub New(Start As Long, Size As Long, ForeColor As Color)
        _Start = Start
        _Size = Size
        _ForeColor = ForeColor
        _BackColor = Color.White
        _Description = ""
    End Sub

    Public Sub New(Start As Long, Size As Long, ForeColor As Color, Description As String)
        _Start = Start
        _Size = Size
        _ForeColor = ForeColor
        _BackColor = Color.White
        _Description = Description
    End Sub

    Public Sub New(Start As Long, Size As Long, ForeColor As Color, BackColor As Color)
        _Start = Start
        _Size = Size
        _ForeColor = ForeColor
        _BackColor = BackColor
        _Description = ""
    End Sub

    Public Sub New(Start As Long, Size As Long, ForeColor As Color, BackColor As Color, Description As String)
        _Start = Start
        _Size = Size
        _ForeColor = ForeColor
        _BackColor = BackColor
        _Description = Description
    End Sub

    Public ReadOnly Property BackColor As Color
    Public ReadOnly Property Description As String
    Public ReadOnly Property ForeColor As Color
    Public ReadOnly Property Size As Long
    Public ReadOnly Property Start As Long

    Public Function CompareTo(comparePart As HexViewHighlightRegion) As Integer Implements IComparable(Of HexViewHighlightRegion).CompareTo
        If comparePart Is Nothing Then
            Return 1
        Else

            Return Me.Start.CompareTo(comparePart.Start)
        End If
    End Function

    Public Overloads Function Equals(other As HexViewHighlightRegion) As Boolean Implements IEquatable(Of HexViewHighlightRegion).Equals
        If other Is Nothing Then
            Return False
        End If
        Return (Me.Start.Equals(other.Start))
    End Function
End Class

Public Class HexViewRegionDescription
    Public Sub New(Start As Long, Size As Long, Text As String)
        _Start = Start
        _Size = Size
        _Text = Text
    End Sub
    Public ReadOnly Property Size As Long
    Public ReadOnly Property Start As Long
    Public ReadOnly Property Text As String
End Class

Public Class HexViewSectorData
    Public Sub New(Disk As DiskImage.Disk)
        _Disk = Disk
        _SectorData = New SectorData(Disk.Data)
        _HighlightedRegionList = New List(Of HighlightedRegions)
    End Sub

    Public Sub New(Disk As DiskImage.Disk, ClusterChain As List(Of UShort))
        _Disk = Disk
        _SectorData = New SectorData(Disk.Data)
        _SectorData.AddBlocksByChain(ClusterListToSectorList(_Disk.BootSector, ClusterChain))
        _HighlightedRegionList = New List(Of HighlightedRegions)
    End Sub

    Public Sub New(Disk As DiskImage.Disk, SectorChain As List(Of UInteger))
        _Disk = Disk
        _SectorData = New SectorData(Disk.Data)
        _SectorData.AddBlocksByChain(SectorChain)
        _HighlightedRegionList = New List(Of HighlightedRegions)
    End Sub

    Public Sub New(Disk As DiskImage.Disk, Offset As UInteger, Length As UInteger)
        _Disk = Disk
        _SectorData = New SectorData(Disk.Data)
        _SectorData.AddBlockByOffset(Offset, Length)
        _HighlightedRegionList = New List(Of HighlightedRegions)
    End Sub


    Public ReadOnly Property Disk As DiskImage.Disk
    Public ReadOnly Property HighlightedRegionList As List(Of HighlightedRegions)
    Public ReadOnly Property SectorData As SectorData
End Class

Public Class HighlightedRegions
    Inherits List(Of HexViewHighlightRegion)

    Public Function AddItem(Start As Long, Size As Long, ForeColor As Color) As HexViewHighlightRegion
        Return AddItem(Start, Size, ForeColor, Color.White, "")
    End Function

    Public Function AddItem(Start As Long, Size As Long, ForeColor As Color, Description As String) As HexViewHighlightRegion
        Return AddItem(Start, Size, ForeColor, Color.White, Description)
    End Function

    Public Function AddItem(Start As Long, Size As Long, ForeColor As Color, BackColor As Color) As HexViewHighlightRegion
        Return AddItem(Start, Size, ForeColor, BackColor, "")
    End Function

    Public Function AddItem(Start As Long, Size As Long, ForeColor As Color, BackColor As Color, Description As String) As HexViewHighlightRegion
        Dim HexViewHighlightRegion As New HexViewHighlightRegion(Start, Size, ForeColor, BackColor, Description)
        Me.Add(HexViewHighlightRegion)

        Return HexViewHighlightRegion
    End Function

    Public Sub AddOffset(Offset As DiskImage.BootSector.BootSectorOffsets, ForeColor As Color)
        Dim Name As String = [Enum].GetName(GetType(DiskImage.BootSector.BootSectorOffsets), Offset)
        Dim Size As DiskImage.BootSector.BootSectorSizes

        If Not [Enum].TryParse(Of DiskImage.BootSector.BootSectorSizes)(Name, Size) Then
            Size = 0
        End If

        Me.Add(New HexViewHighlightRegion(Offset, Size, ForeColor, DiskImage.BootSectorDescription(Offset)))
    End Sub
End Class
Public Class KnownOEMName
    Public Property Company As String = ""
    Public Property Description As String = ""
    Public Property Name As Byte()
    Public Property Suggestion As Boolean = True
    Public Property Win9xId As Boolean = False
    Public Property Note As String = ""

    Public Function GetNameAsString() As String
        Return DiskImage.CodePage437ToUnicode(_Name)
    End Function

    Public Overrides Function ToString() As String
        Return GetNameAsString()
    End Function

End Class