Namespace Hb.Windows.Forms
    ''' <summary>
    ''' Represents a position in the HexBox control
    ''' </summary>
    Friend Structure BytePositionInfo
        Private ReadOnly _characterPosition As Integer
        Private ReadOnly _index As Long

        Public Sub New(index As Long, characterPosition As Integer)
            _index = index
            _characterPosition = characterPosition
        End Sub

        Public ReadOnly Property CharacterPosition As Integer
            Get
                Return _characterPosition
            End Get
        End Property

        Public ReadOnly Property Index As Long
            Get
                Return _index
            End Get
        End Property
    End Structure
End Namespace