Imports System.IO

Namespace Hb.Windows.Forms
    ''' <summary>
    ''' Byte provider for (big) files.
    ''' </summary>
    Public Class FileByteProvider
        Implements Forms.IByteProvider, IDisposable
#Region "WriteCollection class"
        ''' <summary>
        ''' Represents the write buffer class
        ''' </summary>
        Friend Class WriteCollection
            Inherits DictionaryBase
            ''' <summary>
            ''' Gets or sets a byte in the collection
            ''' </summary>
            Default Public Property Item(index As Long) As Byte
                Get
                    Return Dictionary(index)
                End Get
                Set(value As Byte)
                    Dictionary(index) = value
                End Set
            End Property

            ''' <summary>
            ''' Adds a byte into the collection
            ''' </summary>
            ''' <paramname="index">the index of the byte</param>
            ''' <paramname="value">the value of the byte</param>
            Public Sub Add(index As Long, value As Byte)
                Dictionary.Add(index, value)
            End Sub

            ''' <summary>
            ''' Determines if a byte with the given index exists.
            ''' </summary>
            ''' <paramname="index">the index of the byte</param>
            ''' <returns>true, if the is in the collection</returns>
            Public Function Contains(index As Long) As Boolean
                Return Dictionary.Contains(index)
            End Function

        End Class
#End Region

        ''' <summary>
        ''' Read-only access.
        ''' </summary>
        Private ReadOnly _readOnly As Boolean

        ''' <summary>
        ''' Contains all changes
        ''' </summary>
        Private ReadOnly _writes As New WriteCollection()

        ''' <summary>
        ''' Contains the file name.
        ''' </summary>
        Private _fileName As String

        ''' <summary>
        ''' Contains the file stream.
        ''' </summary>
        Private _fileStream As FileStream

        ''' <summary>
        ''' Occurs, when the write buffer contains new changes.
        ''' </summary>
        Public Event Changed As EventHandler Implements Forms.IByteProvider.Changed

        ''' <summary>
        ''' Initializes a new instance of the FileByteProvider class.
        ''' </summary>
        ''' <paramname="fileName"></param>
        Public Sub New(fileName As String)
            _fileName = fileName

            Try
                ' try to open in write mode
                _fileStream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read)
            Catch
                ' write mode failed, try to open in read-only and fileshare friendly mode.
                Try
                    _fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    _readOnly = True
                Catch
                    Throw
                End Try
            End Try
        End Sub

        ''' <summary>
        ''' Terminates the instance of the FileByteProvider class.
        ''' </summary>
        Protected Overrides Sub Finalize()
            Dispose()
        End Sub

        ''' <summary>
        ''' Gets the name of the file the byte provider is using.
        ''' </summary>
        Public ReadOnly Property FileName As String
            Get
                Return _fileName
            End Get
        End Property

        ''' <summary>
        ''' Updates the file with all changes the write buffer contains.
        ''' </summary>
        Public Sub ApplyChanges() Implements Forms.IByteProvider.ApplyChanges
            If _readOnly Then
                Throw New Exception("File is in read-only mode.")
            End If

            If Not HasChanges() Then Return

            Dim en As IDictionaryEnumerator = _writes.GetEnumerator()
            While en.MoveNext()
                Dim index As Long = en.Key
                Dim value As Byte = en.Value
                If _fileStream.Position <> index Then _fileStream.Position = index
                _fileStream.Write(New Byte() {value}, 0, 1)
            End While
            _writes.Clear()
        End Sub

        ''' <summary>
        ''' Returns a value if there are some changes.
        ''' </summary>
        ''' <returns>true, if there are some changes</returns>
        Public Function HasChanges() As Boolean Implements Forms.IByteProvider.HasChanges
            Return _writes.Count > 0
        End Function

        ''' <summary>
        ''' Clears the write buffer and reject all changes made.
        ''' </summary>
        Public Sub RejectChanges()
            _writes.Clear()
        End Sub

        ''' <summary>
        ''' Raises the Changed event.
        ''' </summary>
        ''' <remarks>Never used.</remarks>
        Private Sub OnChanged(e As EventArgs)
            RaiseEvent Changed(Me, e)
        End Sub
#Region "IByteProvider Members"
        ''' <summary>
        ''' Never used.
        ''' </summary>
        Public Event LengthChanged As EventHandler Implements Forms.IByteProvider.LengthChanged

        ''' <summary>
        ''' Gets the length of the file.
        ''' </summary>
        Public ReadOnly Property Length As Long Implements Forms.IByteProvider.Length
            Get
                Return _fileStream.Length
            End Get
        End Property

        ''' <summary>
        ''' Not supported
        ''' </summary>
        Public Sub DeleteBytes(index As Long, length As Long) Implements Forms.IByteProvider.DeleteBytes
            Throw New NotSupportedException("FileByteProvider.DeleteBytes")
        End Sub

        ''' <summary>
        ''' Not supported
        ''' </summary>
        Public Sub InsertBytes(index As Long, bs As Byte()) Implements Forms.IByteProvider.InsertBytes
            Throw New NotSupportedException("FileByteProvider.InsertBytes")
        End Sub

        ''' <summary>
        ''' Reads a byte from the file.
        ''' </summary>
        ''' <paramname="index">the index of the byte to read</param>
        ''' <returns>the byte</returns>
        Public Function ReadByte(index As Long) As Byte Implements Forms.IByteProvider.ReadByte
            If _writes.Contains(index) Then Return _writes(index)

            If _fileStream.Position <> index Then _fileStream.Position = index

            Dim res As Byte = CByte(_fileStream.ReadByte())
            Return res
        End Function
        ''' <summary>
        ''' Returns false
        ''' </summary>
        Public Function SupportsDeleteBytes() As Boolean Implements Forms.IByteProvider.SupportsDeleteBytes
            Return False
        End Function

        ''' <summary>
        ''' Returns false
        ''' </summary>
        Public Function SupportsInsertBytes() As Boolean Implements Forms.IByteProvider.SupportsInsertBytes
            Return False
        End Function

        ''' <summary>
        ''' Returns true
        ''' </summary>
        Public Function SupportsWriteByte() As Boolean Implements Forms.IByteProvider.SupportsWriteByte
            Return Not _readOnly
        End Function

        ''' <summary>
        ''' Writes a byte into write buffer
        ''' </summary>
        Public Sub WriteByte(index As Long, value As Byte) Implements Forms.IByteProvider.WriteByte
            If _writes.Contains(index) Then
                _writes(index) = value
            Else
                _writes.Add(index, value)
            End If

            OnChanged(EventArgs.Empty)
        End Sub
#End Region

#Region "IDisposable Members"
        ''' <summary>
        ''' Releases the file handle used by the FileByteProvider.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            If _fileStream IsNot Nothing Then
                _fileName = Nothing

                _fileStream.Close()
                _fileStream = Nothing
            End If

            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
