Namespace Hb.Windows.Forms
    Friend MustInherit Class DataBlock
        Friend _map As Forms.DataMap
        Friend _nextBlock As DataBlock
        Friend _previousBlock As DataBlock

        Public MustOverride ReadOnly Property Length As Long

        Public ReadOnly Property Map As Forms.DataMap
            Get
                Return _map
            End Get
        End Property

        Public ReadOnly Property NextBlock As DataBlock
            Get
                Return _nextBlock
            End Get
        End Property

        Public ReadOnly Property PreviousBlock As DataBlock
            Get
                Return _previousBlock
            End Get
        End Property

        Public MustOverride Sub RemoveBytes(position As Long, count As Long)
    End Class
End Namespace
