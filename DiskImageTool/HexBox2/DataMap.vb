Namespace Hb.Windows.Forms
    Friend Class DataMap
        Implements ICollection, IEnumerable

        Friend _count As Integer
        Friend _firstBlock As Forms.DataBlock
        Friend _version As Integer
        Private ReadOnly _syncRoot As New Object()

        Public Sub New()
        End Sub

        Public Sub New(collection As IEnumerable)
            If collection Is Nothing Then
                Throw New ArgumentNullException("collection")
            End If

            For Each item As Forms.DataBlock In collection
                Me.AddLast(item)
            Next
        End Sub

        Public ReadOnly Property FirstBlock As Forms.DataBlock
            Get
                Return _firstBlock
            End Get
        End Property

        Public Sub AddAfter(block As Forms.DataBlock, newBlock As Forms.DataBlock)
            Me.AddAfterInternal(block, newBlock)
        End Sub

        Public Sub AddBefore(block As Forms.DataBlock, newBlock As Forms.DataBlock)
            Me.AddBeforeInternal(block, newBlock)
        End Sub

        Public Sub AddFirst(block As Forms.DataBlock)
            If _firstBlock Is Nothing Then
                Me.AddBlockToEmptyMap(block)
            Else
                Me.AddBeforeInternal(_firstBlock, block)
            End If
        End Sub

        Public Sub AddLast(block As Forms.DataBlock)
            If _firstBlock Is Nothing Then
                Me.AddBlockToEmptyMap(block)
            Else
                Me.AddAfterInternal(GetLastBlock(), block)
            End If
        End Sub

        Public Sub Clear()
            Dim block As Forms.DataBlock = FirstBlock
            While block IsNot Nothing
                Dim nextBlock As Forms.DataBlock = block.NextBlock
                Me.InvalidateBlock(block)
                block = nextBlock
            End While
            _firstBlock = Nothing
            _count = 0
            _version += 1
        End Sub

        Public Sub Remove(block As Forms.DataBlock)
            Me.RemoveInternal(block)
        End Sub

        Public Sub RemoveFirst()
            If _firstBlock Is Nothing Then
                Throw New InvalidOperationException("The collection is empty.")
            End If
            Me.RemoveInternal(_firstBlock)
        End Sub

        Public Sub RemoveLast()
            If _firstBlock Is Nothing Then
                Throw New InvalidOperationException("The collection is empty.")
            End If
            Me.RemoveInternal(GetLastBlock())
        End Sub

        Public Function Replace(block As Forms.DataBlock, newBlock As Forms.DataBlock) As Forms.DataBlock
            Me.AddAfterInternal(block, newBlock)
            Me.RemoveInternal(block)
            Return newBlock
        End Function

        Private Sub AddAfterInternal(block As Forms.DataBlock, newBlock As Forms.DataBlock)
            newBlock._previousBlock = block
            newBlock._nextBlock = block._nextBlock
            newBlock._map = Me

            If block._nextBlock IsNot Nothing Then
                block._nextBlock._previousBlock = newBlock
            End If
            block._nextBlock = newBlock

            _version += 1
            _count += 1
        End Sub

        Private Sub AddBeforeInternal(block As Forms.DataBlock, newBlock As Forms.DataBlock)
            newBlock._nextBlock = block
            newBlock._previousBlock = block._previousBlock
            newBlock._map = Me

            If block._previousBlock IsNot Nothing Then
                block._previousBlock._nextBlock = newBlock
            End If
            block._previousBlock = newBlock

            If _firstBlock Is block Then
                _firstBlock = newBlock
            End If
            _version += 1
            _count += 1
        End Sub

        Private Sub AddBlockToEmptyMap(block As Forms.DataBlock)
            block._map = Me
            block._nextBlock = Nothing
            block._previousBlock = Nothing

            _firstBlock = block
            _version += 1
            _count += 1
        End Sub

        Private Function GetLastBlock() As Forms.DataBlock
            Dim lastBlock As Forms.DataBlock = Nothing
            Dim block As Forms.DataBlock = FirstBlock

            While block IsNot Nothing
                lastBlock = block
                block = block.NextBlock
            End While
            Return lastBlock
        End Function

        Private Sub InvalidateBlock(block As Forms.DataBlock)
            block._map = Nothing
            block._nextBlock = Nothing
            block._previousBlock = Nothing
        End Sub

        Private Sub RemoveInternal(block As Forms.DataBlock)
            Dim previousBlock As Forms.DataBlock = block._previousBlock
            Dim nextBlock As Forms.DataBlock = block._nextBlock

            If previousBlock IsNot Nothing Then
                previousBlock._nextBlock = nextBlock
            End If

            If nextBlock IsNot Nothing Then
                nextBlock._previousBlock = previousBlock
            End If

            If _firstBlock Is block Then
                _firstBlock = nextBlock
            End If

            Me.InvalidateBlock(block)

            _count -= 1
            _version += 1
        End Sub
#Region "ICollection Members"
        Public ReadOnly Property Count As Integer Implements ICollection.Count
            Get
                Return _count
            End Get
        End Property

        Public ReadOnly Property IsSynchronized As Boolean Implements ICollection.IsSynchronized
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property SyncRoot As Object Implements ICollection.SyncRoot
            Get
                Return _syncRoot
            End Get
        End Property

        Public Sub CopyTo(array As Array, index As Integer) Implements ICollection.CopyTo
            Dim blockArray As Hb.Windows.Forms.DataBlock() = TryCast(array, Hb.Windows.Forms.DataBlock())
            Dim block As Forms.DataBlock = FirstBlock

            While block IsNot Nothing
                blockArray(Math.Min(Threading.Interlocked.Increment(index), index - 1)) = block
                block = block.NextBlock
            End While
        End Sub
#End Region

#Region "IEnumerable Members"
        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return New Enumerator(Me)
        End Function
#End Region

#Region "Enumerator Nested Type"
        Friend Class Enumerator
            Implements IEnumerator, IDisposable

            Private ReadOnly _map As DataMap
            Private ReadOnly _version As Integer
            Private _current As Forms.DataBlock
            Private _index As Integer

            Friend Sub New(map As DataMap)
                _map = map
                _version = map._version
                _current = Nothing
                _index = -1
            End Sub

            Private ReadOnly Property Current As Object Implements IEnumerator.Current
                Get
                    If _index < 0 OrElse _index > _map.Count Then
                        Throw New InvalidOperationException("Enumerator is positioned before the first element or after the last element of the collection.")
                    End If
                    Return _current
                End Get
            End Property

            Public Sub Dispose() Implements IDisposable.Dispose
            End Sub

            Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
                If _version <> _map._version Then
                    Throw New InvalidOperationException("Collection was modified after the enumerator was instantiated.")
                End If

                If _index >= _map.Count Then
                    Return False
                End If

                If Threading.Interlocked.Increment(_index) = 0 Then
                    _current = _map.FirstBlock
                Else
                    _current = _current.NextBlock
                End If

                Return _index < _map.Count
            End Function

            Private Sub Reset() Implements IEnumerator.Reset
                If _version <> _map._version Then
                    Throw New InvalidOperationException("Collection was modified after the enumerator was instantiated.")
                End If

                _index = -1
                _current = Nothing
            End Sub
        End Class
#End Region
    End Class
End Namespace
