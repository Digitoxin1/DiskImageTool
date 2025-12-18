Namespace ImageFormats.TD0
    Public Class TD0Track
        Private ReadOnly _IsSingleDensityGlobal As Boolean
        Private ReadOnly _TrackHeader As TD0TrackHeader
        Public Sub New(TrackHeader As TD0TrackHeader, IsSingleDensityGlobal As Boolean)
            _TrackHeader = TrackHeader
            _IsSingleDensityGlobal = IsSingleDensityGlobal

            Sectors = New List(Of TD0Sector)()
            FirstSectorId = -1
            LastSectorId = -1
        End Sub

        Public ReadOnly Property Cylinder As Byte
            Get
                Return _TrackHeader.PhysicalCylinder
            End Get
        End Property

        Public Property FirstSectorId As Short

        Public ReadOnly Property Head As Byte
            Get
                Return _TrackHeader.PhysicalHead
            End Get
        End Property

        Public ReadOnly Property IsFM As Boolean
            Get
                Return _TrackHeader.IsFMTrack OrElse _IsSingleDensityGlobal
            End Get
        End Property

        Public Property LastSectorId As Short
        Public Property Sectors As List(Of TD0Sector)

        Public Sub AddSector(sec As TD0Sector)
            Sectors.Add(sec)

            If FirstSectorId = -1 OrElse sec.SectorId < FirstSectorId Then
                FirstSectorId = sec.SectorId
            End If

            If sec.SectorId > LastSectorId Then
                LastSectorId = sec.SectorId
            End If
        End Sub

        Public Function GetHeaderBytes() As Byte()
            Return _TrackHeader.GetBytes()
        End Function
    End Class
End Namespace
