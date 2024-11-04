Namespace Bitstream
    Public MustInherit Class BitStreamImageBase
        Implements IBitstreamImage

        Public ReadOnly Property BitRate As UShort Implements IBitstreamImage.BitRate
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property HasSurfaceData As Boolean Implements IBitstreamImage.HasSurfaceData
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property RPM As UShort Implements IBitstreamImage.RPM
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property SideCount As Byte Implements IBitstreamImage.SideCount
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property TrackCount As UShort Implements IBitstreamImage.TrackCount
            Get
                Return 0
            End Get
        End Property

        Public ReadOnly Property TrackStep As Byte Implements IBitstreamImage.TrackStep
            Get
                Return 1
            End Get
        End Property

        Public ReadOnly Property VariableBitRate As Boolean Implements IBitstreamImage.VariableBitRate
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property VariableRPM As Boolean Implements IBitstreamImage.VariableRPM
            Get
                Return False
            End Get
        End Property

        Public MustOverride Function Export(FilePath As String) As Boolean Implements IBitstreamImage.Export

        Public MustOverride Function Load(FilePath As String) As Boolean Implements IBitstreamImage.Load

        Public MustOverride Function Load(Buffer() As Byte) As Boolean Implements IBitstreamImage.Load

        Private Function IBitstreamTrack_GetTrack(Track As UShort, Side As Byte) As IBitstreamTrack Implements IBitstreamImage.GetTrack
            Return Nothing
        End Function
    End Class
End Namespace
