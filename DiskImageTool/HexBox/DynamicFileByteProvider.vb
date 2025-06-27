Imports System.IO
Imports System.Runtime.InteropServices

Namespace Hb.Windows.Forms
    ''' <summary>
    ''' Implements a fully editable byte provider for file data of any size.
    ''' </summary>
    ''' <remarks>
    ''' Only changes to the file are stored in memory with reads from the
    ''' original data occurring as required.
    ''' </remarks>
    Public NotInheritable Class DynamicFileByteProvider
        Implements Forms.IByteProvider, IDisposable

        Const COPY_BLOCK_SIZE As Integer = 4096

        Private ReadOnly _readOnly As Boolean
        Private _dataMap As Forms.DataMap
        Private _fileName As String
        Private _stream As Stream
        Private _totalLength As Long

        ''' <summary>
        ''' Constructs a new <seecref="DynamicFileByteProvider"/> instance.
        ''' </summary>
        ''' <paramname="fileName">The name of the file from which bytes should be provided.</param>
        Public Sub New(fileName As String)
            Me.New(fileName, False)
        End Sub

        ''' <summary>
        ''' Constructs a new <seecref="DynamicFileByteProvider"/> instance.
        ''' </summary>
        ''' <paramname="fileName">The name of the file from which bytes should be provided.</param>
        ''' <paramname="readOnly">True, opens the file in read-only mode.</param>
        Public Sub New(fileName As String, [readOnly] As Boolean)
            _fileName = fileName

            If Not [readOnly] Then
                _stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read)
            Else
                _stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            End If

            _readOnly = [readOnly]

            ReInitialize()
        End Sub

        ''' <summary>
        ''' Constructs a new <seecref="DynamicFileByteProvider"/> instance.
        ''' </summary>
        ''' <paramname="stream">the stream containing the data.</param>
        ''' <remarks>
        ''' The stream must supported seek operations.
        ''' </remarks>
        Public Sub New(stream As Stream)
            If stream Is Nothing Then Throw New ArgumentNullException("stream")
            If Not stream.CanSeek Then Throw New ArgumentException("stream must supported seek operations(CanSeek)")

            _stream = stream
            _readOnly = Not stream.CanWrite
            ReInitialize()
        End Sub

#Region "IByteProvider Members"
        ''' <summary>
        ''' See <seecref="IByteProvider.Changed"/> for more information.
        ''' </summary>
        Public Event Changed As EventHandler Implements Forms.IByteProvider.Changed

        ''' <summary>
        ''' See <seecref="IByteProvider.LengthChanged"/> for more information.
        ''' </summary>
        Public Event LengthChanged As EventHandler Implements Forms.IByteProvider.LengthChanged

        ''' <summary>
        ''' See <seecref="IByteProvider.Length"/> for more information.
        ''' </summary>
        Public ReadOnly Property Length As Long Implements Forms.IByteProvider.Length
            Get
                Return _totalLength
            End Get
        End Property

        ''' <summary>
        ''' See <seecref="IByteProvider.ApplyChanges"/> for more information.
        ''' </summary>
        Public Sub ApplyChanges() Implements Forms.IByteProvider.ApplyChanges
            If _readOnly Then Throw New OperationCanceledException("File is in read-only mode")

            ' This method is implemented to efficiently save the changes to the same file stream opened for reading.
            ' Saving to a separate file would be a much simpler implementation.

            ' Firstly, extend the file length (if necessary) to ensure that there is enough disk space.
            If _totalLength > _stream.Length Then
                _stream.SetLength(_totalLength)
            End If

            ' Secondly, shift around any file sections that have moved.
            Dim dataOffset As Long = 0
            Dim block As Forms.DataBlock = _dataMap.FirstBlock

            While block IsNot Nothing
                Dim fileBlock As Forms.FileDataBlock = TryCast(block, Forms.FileDataBlock)
                If fileBlock IsNot Nothing AndAlso fileBlock.FileOffset <> dataOffset Then
                    Me.MoveFileBlock(fileBlock, dataOffset)
                End If
                dataOffset += block.Length
                block = block.NextBlock
            End While

            ' Next, write in-memory changes.
            dataOffset = 0
            block = _dataMap.FirstBlock

            While block IsNot Nothing
                Dim memoryBlock As Forms.MemoryDataBlock = TryCast(block, Forms.MemoryDataBlock)
                If memoryBlock IsNot Nothing Then
                    _stream.Position = dataOffset
                    Dim memoryOffset = 0

                    While memoryOffset < memoryBlock.Length
                        _stream.Write(memoryBlock.Data, memoryOffset, CInt(Math.Min(COPY_BLOCK_SIZE, memoryBlock.Length - memoryOffset)))
                        memoryOffset += COPY_BLOCK_SIZE
                    End While
                End If
                dataOffset += block.Length
                block = block.NextBlock
            End While

            ' Finally, if the file has shortened, truncate the stream.
            _stream.SetLength(_totalLength)
            ReInitialize()
        End Sub

        ''' <summary>
        ''' See <seecref="IByteProvider.DeleteBytes"/> for more information.
        ''' </summary>
        Public Sub DeleteBytes(index As Long, length As Long) Implements Forms.IByteProvider.DeleteBytes
            Try
                Dim bytesToDelete = length

                ' Find the first block affected.
                Dim blockOffset As Long
                Dim block As Forms.DataBlock = GetDataBlock(index, blockOffset)

                ' Truncate or remove each block as necessary.
                While bytesToDelete > 0 AndAlso block IsNot Nothing
                    Dim blockLength As Long = block.Length
                    Dim nextBlock As Forms.DataBlock = block.NextBlock

                    ' Delete the appropriate section from the block (this may result in two blocks or a zero length block).
                    Dim count = Math.Min(bytesToDelete, blockLength - (index - blockOffset))
                    block.RemoveBytes(index - blockOffset, count)

                    If block.Length = 0 Then
                        _dataMap.Remove(block)
                        If _dataMap.FirstBlock Is Nothing Then
                            _dataMap.AddFirst(New Forms.MemoryDataBlock(New Byte(-1) {}))
                        End If
                    End If

                    bytesToDelete -= count
                    blockOffset += block.Length
                    block = If(bytesToDelete > 0, nextBlock, Nothing)
                End While

            Finally
                _totalLength -= length
                OnLengthChanged(EventArgs.Empty)
                OnChanged(EventArgs.Empty)
            End Try
        End Sub

        ''' <summary>
        ''' See <seecref="IByteProvider.HasChanges"/> for more information.
        ''' </summary>
        Public Function HasChanges() As Boolean Implements Forms.IByteProvider.HasChanges
            If _readOnly Then Return False

            If _totalLength <> _stream.Length Then
                Return True
            End If

            Dim offset As Long = 0
            Dim block As Forms.DataBlock = _dataMap.FirstBlock

            While block IsNot Nothing
                Dim fileBlock As Forms.FileDataBlock = TryCast(block, Forms.FileDataBlock)
                If fileBlock Is Nothing Then
                    Return True
                End If

                If fileBlock.FileOffset <> offset Then
                    Return True
                End If

                offset += fileBlock.Length
                block = block.NextBlock
            End While
            Return offset <> _stream.Length
        End Function

        ''' <summary>
        ''' See <seecref="IByteProvider.InsertBytes"/> for more information.
        ''' </summary>
        Public Sub InsertBytes(index As Long, bs As Byte()) Implements Forms.IByteProvider.InsertBytes
            Try
                ' Find the block affected.
                Dim blockOffset As Long
                Dim block As Forms.DataBlock = GetDataBlock(index, blockOffset)

                ' If the insertion point is in a memory block, just insert it.
                Dim memoryBlock As Forms.MemoryDataBlock = TryCast(block, Forms.MemoryDataBlock)
                If memoryBlock IsNot Nothing Then
                    memoryBlock.InsertBytes(index - blockOffset, bs)
                    Return
                End If

                Dim fileBlock As Forms.FileDataBlock = CType(block, Forms.FileDataBlock)

                ' If the insertion point is at the start of a file block, and the previous block is a memory block, append it to that block.
                If blockOffset = index AndAlso block.PreviousBlock IsNot Nothing Then
                    Dim previousMemoryBlock As Forms.MemoryDataBlock = TryCast(block.PreviousBlock, Forms.MemoryDataBlock)
                    If previousMemoryBlock IsNot Nothing Then
                        previousMemoryBlock.InsertBytes(previousMemoryBlock.Length, bs)
                        Return
                    End If
                End If

                ' Split the block into a prefix and a suffix and place a memory block in-between.
                Dim prefixBlock As Forms.FileDataBlock = Nothing
                If index > blockOffset Then
                    prefixBlock = New Forms.FileDataBlock(fileBlock.FileOffset, index - blockOffset)
                End If

                Dim suffixBlock As Forms.FileDataBlock = Nothing
                If index < blockOffset + fileBlock.Length Then
                    suffixBlock = New Forms.FileDataBlock(fileBlock.FileOffset + index - blockOffset, fileBlock.Length - (index - blockOffset))
                End If

                block = _dataMap.Replace(block, New Forms.MemoryDataBlock(bs))

                If prefixBlock IsNot Nothing Then
                    _dataMap.AddBefore(block, prefixBlock)
                End If

                If suffixBlock IsNot Nothing Then
                    _dataMap.AddAfter(block, suffixBlock)
                End If

            Finally
                _totalLength += bs.Length
                OnLengthChanged(EventArgs.Empty)
                OnChanged(EventArgs.Empty)
            End Try
        End Sub

        ''' <summary>
        ''' See <seecref="IByteProvider.ReadByte"/> for more information.
        ''' </summary>
        Public Function ReadByte(index As Long) As Byte Implements Forms.IByteProvider.ReadByte
            Dim blockOffset As Long
            Dim block As Forms.DataBlock = GetDataBlock(index, blockOffset)
            Dim fileBlock As Forms.FileDataBlock = TryCast(block, Forms.FileDataBlock)

            If fileBlock IsNot Nothing Then
                Return Me.ReadByteFromFile(fileBlock.FileOffset + index - blockOffset)
            Else
                Dim memoryBlock As Forms.MemoryDataBlock = CType(block, Forms.MemoryDataBlock)
                Return memoryBlock.Data(index - blockOffset)
            End If
        End Function

        ''' <summary>
        ''' Saves file as another file, it can save over 2 GB size. Changes nust below 2 GB size.
        ''' </summary>
        ''' <paramname="outfile"></param>
        Public Sub SaveAsChanges(outfile As String)
            ' Firstly, create the file.
            Dim t As Stream = New FileStream(outfile, FileMode.Create)

            _stream.Seek(0, SeekOrigin.Begin)

            Dim buf = New Byte(131071) {} '128 KB buffer
            Dim ReadedBytes As Long
            Dim block As Forms.DataBlock = _dataMap.FirstBlock

            While block IsNot Nothing
                Dim memoryBlock As Forms.MemoryDataBlock = TryCast(block, Forms.MemoryDataBlock)
                If memoryBlock IsNot Nothing Then 'read from memory changed parts
                    t.Write(memoryBlock.Data, 0, CInt(memoryBlock.Length))
                Else
                    ReadedBytes = 0

                    While ReadedBytes < block.Length
                        If block.Length - ReadedBytes < buf.Length Then
                            _stream.Read(buf, 0, CInt(block.Length - ReadedBytes))
                            t.Write(buf, 0, CInt(block.Length - ReadedBytes))
                            Exit While
                        End If
                        _stream.Read(buf, 0, buf.Length)
                        t.Write(buf, 0, buf.Length)
                        ReadedBytes += buf.Length
                    End While
                End If

                block = block.NextBlock
            End While
            _stream.Dispose()
            t.Dispose()

            ' Finally, change file name and stream.
            _fileName = outfile
            If Not _readOnly Then
                _stream = File.Open(_fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read)
            Else
                _stream = File.Open(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            End If

            ReInitialize()
        End Sub

        ''' <summary>
        ''' See <seecref="IByteProvider.SupportsDeleteBytes"/> for more information.
        ''' </summary>
        Public Function SupportsDeleteBytes() As Boolean Implements Forms.IByteProvider.SupportsDeleteBytes
            Return Not _readOnly
        End Function

        ''' <summary>
        ''' See <seecref="IByteProvider.SupportsInsertBytes"/> for more information.
        ''' </summary>
        Public Function SupportsInsertBytes() As Boolean Implements Forms.IByteProvider.SupportsInsertBytes
            Return Not _readOnly
        End Function

        ''' <summary>
        ''' See <seecref="IByteProvider.SupportsWriteByte"/> for more information.
        ''' </summary>
        Public Function SupportsWriteByte() As Boolean Implements Forms.IByteProvider.SupportsWriteByte
            Return Not _readOnly
        End Function

        ''' <summary>
        ''' See <seecref="IByteProvider.WriteByte"/> for more information.
        ''' </summary>
        Public Sub WriteByte(index As Long, value As Byte) Implements Forms.IByteProvider.WriteByte
            Try
                ' Find the block affected.
                Dim blockOffset As Long
                Dim block As Forms.DataBlock = GetDataBlock(index, blockOffset)

                ' If the byte is already in a memory block, modify it.
                Dim memoryBlock As Forms.MemoryDataBlock = TryCast(block, Forms.MemoryDataBlock)
                If memoryBlock IsNot Nothing Then
                    memoryBlock.Data(index - blockOffset) = value
                    Return
                End If

                Dim fileBlock As Forms.FileDataBlock = CType(block, Forms.FileDataBlock)

                ' If the byte changing is the first byte in the block and the previous block is a memory block, extend that.
                If blockOffset = index AndAlso block.PreviousBlock IsNot Nothing Then
                    Dim previousMemoryBlock As Forms.MemoryDataBlock = TryCast(block.PreviousBlock, Forms.MemoryDataBlock)
                    If previousMemoryBlock IsNot Nothing Then
                        previousMemoryBlock.AddByteToEnd(value)
                        fileBlock.RemoveBytesFromStart(1)
                        If fileBlock.Length = 0 Then
                            _dataMap.Remove(fileBlock)
                        End If
                        Return
                    End If
                End If

                ' If the byte changing is the last byte in the block and the next block is a memory block, extend that.
                If blockOffset + fileBlock.Length - 1 = index AndAlso block.NextBlock IsNot Nothing Then
                    Dim nextMemoryBlock As Forms.MemoryDataBlock = TryCast(block.NextBlock, Forms.MemoryDataBlock)
                    If nextMemoryBlock IsNot Nothing Then
                        nextMemoryBlock.AddByteToStart(value)
                        fileBlock.RemoveBytesFromEnd(1)
                        If fileBlock.Length = 0 Then
                            _dataMap.Remove(fileBlock)
                        End If
                        Return
                    End If
                End If

                ' Split the block into a prefix and a suffix and place a memory block in-between.
                Dim prefixBlock As Forms.FileDataBlock = Nothing
                If index > blockOffset Then
                    prefixBlock = New Forms.FileDataBlock(fileBlock.FileOffset, index - blockOffset)
                End If

                Dim suffixBlock As Forms.FileDataBlock = Nothing
                If index < blockOffset + fileBlock.Length - 1 Then
                    suffixBlock = New Forms.FileDataBlock(fileBlock.FileOffset + index - blockOffset + 1, fileBlock.Length - (index - blockOffset + 1))
                End If

                block = _dataMap.Replace(block, New Forms.MemoryDataBlock(value))

                If prefixBlock IsNot Nothing Then
                    _dataMap.AddBefore(block, prefixBlock)
                End If

                If suffixBlock IsNot Nothing Then
                    _dataMap.AddAfter(block, suffixBlock)
                End If

            Finally
                OnChanged(EventArgs.Empty)
            End Try
        End Sub
#End Region

#Region "IDisposable Members"
        ''' <summary>
        ''' See <seecref="Object.Finalize"/> for more information.
        ''' </summary>
        Protected Overrides Sub Finalize()
            Dispose()
        End Sub

        ''' <summary>
        ''' See <seecref="IDisposable.Dispose"/> for more information.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            If _stream IsNot Nothing Then
                _stream.Close()
                _stream = Nothing
            End If
            _fileName = Nothing
            _dataMap = Nothing
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        ''' <summary>
        ''' Gets a value, if the file is opened in read-only mode.
        ''' </summary>
        Public ReadOnly Property [ReadOnly] As Boolean
            Get
                Return _readOnly
            End Get
        End Property

        Private Function GetDataBlock(findOffset As Long, <Out> ByRef blockOffset As Long) As Forms.DataBlock
            If findOffset < 0 OrElse findOffset > _totalLength Then
                Throw New ArgumentOutOfRangeException("findOffset")
            End If

            ' Iterate over the blocks until the block containing the required offset is encountered.
            blockOffset = 0
            Dim block As Forms.DataBlock = _dataMap.FirstBlock

            While block IsNot Nothing
                If blockOffset <= findOffset AndAlso blockOffset + block.Length > findOffset OrElse block.NextBlock Is Nothing Then
                    Return block
                End If
                blockOffset += block.Length
                block = block.NextBlock
            End While
            Return Nothing
        End Function

        Private Function GetNextFileDataBlock(block As Forms.DataBlock, dataOffset As Long, <Out> ByRef nextDataOffset As Long) As Forms.FileDataBlock
            ' Iterate over the remaining blocks until a file block is encountered.
            nextDataOffset = dataOffset + block.Length
            block = block.NextBlock
            While block IsNot Nothing
                Dim fileBlock As Forms.FileDataBlock = TryCast(block, Forms.FileDataBlock)
                If fileBlock IsNot Nothing Then
                    Return fileBlock
                End If
                nextDataOffset += block.Length
                block = block.NextBlock
            End While
            Return Nothing
        End Function

        Private Sub MoveFileBlock(fileBlock As Forms.FileDataBlock, dataOffset As Long)
            ' First, determine whether the next file block needs to move before this one.
            Dim nextDataOffset As Long
            Dim nextFileBlock As Forms.FileDataBlock = Me.GetNextFileDataBlock(fileBlock, dataOffset, nextDataOffset)
            If nextFileBlock IsNot Nothing AndAlso dataOffset + fileBlock.Length > nextFileBlock.FileOffset Then
                ' The next block needs to move first, so do that now.
                Me.MoveFileBlock(nextFileBlock, nextDataOffset)
            End If

            ' Now, move the block.
            If fileBlock.FileOffset > dataOffset Then
                ' Move the section to earlier in the file stream (done in chunks starting at the beginning of the section).
                Dim buffer = New Byte(4095) {}
                Dim relativeOffset As Long = 0

                While relativeOffset < fileBlock.Length
                    Dim readOffset As Long = fileBlock.FileOffset + relativeOffset
                    Dim bytesToRead = CInt(Math.Min(buffer.Length, fileBlock.Length - relativeOffset))
                    _stream.Position = readOffset
                    _stream.Read(buffer, 0, bytesToRead)

                    Dim writeOffset = dataOffset + relativeOffset
                    _stream.Position = writeOffset
                    _stream.Write(buffer, 0, bytesToRead)
                    relativeOffset += buffer.Length
                End While
            Else
                ' Move the section to later in the file stream (done in chunks starting at the end of the section).
                Dim buffer = New Byte(4095) {}
                Dim relativeOffset As Long = 0

                While relativeOffset < fileBlock.Length
                    Dim bytesToRead = CInt(Math.Min(buffer.Length, fileBlock.Length - relativeOffset))
                    Dim readOffset As Long = fileBlock.FileOffset + fileBlock.Length - relativeOffset - bytesToRead
                    _stream.Position = readOffset
                    _stream.Read(buffer, 0, bytesToRead)

                    Dim writeOffset As Long = dataOffset + fileBlock.Length - relativeOffset - bytesToRead
                    _stream.Position = writeOffset
                    _stream.Write(buffer, 0, bytesToRead)
                    relativeOffset += buffer.Length
                End While
            End If

            ' This block now points to a different position in the file.
            fileBlock.SetFileOffset(dataOffset)
        End Sub

        Private Sub OnChanged(e As EventArgs)
            RaiseEvent Changed(Me, e)
        End Sub

        Private Sub OnLengthChanged(e As EventArgs)
            RaiseEvent LengthChanged(Me, e)
        End Sub
        Private Function ReadByteFromFile(fileOffset As Long) As Byte
            ' Move to the correct position and read the byte.
            If _stream.Position <> fileOffset Then
                _stream.Position = fileOffset
            End If
            Return CByte(_stream.ReadByte())
        End Function
        Private Sub ReInitialize()
            _dataMap = New Forms.DataMap()
            _dataMap.AddFirst(New Forms.FileDataBlock(0, _stream.Length))
            _totalLength = _stream.Length
        End Sub
    End Class
End Namespace
