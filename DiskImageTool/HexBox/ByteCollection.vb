Namespace Hb.Windows.Forms
    ''' <summary>
    ''' Represents a collection of bytes.
    ''' </summary>
    Public Class ByteCollection
        Inherits CollectionBase
        ''' <summary>
        ''' Initializes a new instance of ByteCollection class.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Initializes a new instance of ByteCollection class.
        ''' </summary>
        ''' <paramname="bs">an array of bytes to add to collection</param>
        Public Sub New(bs As Byte())
            AddRange(bs)
        End Sub

        ''' <summary>
        ''' Gets or sets the value of a byte
        ''' </summary>
        Default Public Property Item(index As Integer) As Byte
            Get
                Return List(index)
            End Get
            Set(value As Byte)
                List(index) = value
            End Set
        End Property

        ''' <summary>
        ''' Adds a byte into the collection.
        ''' </summary>
        ''' <paramname="b">the byte to add</param>
        Public Sub Add(b As Byte)
            List.Add(b)
        End Sub

        ''' <summary>
        ''' Adds a range of bytes to the collection.
        ''' </summary>
        ''' <paramname="bs">the bytes to add</param>
        Public Sub AddRange(bs As Byte())
            InnerList.AddRange(bs)
        End Sub

        ''' <summary>
        ''' Returns true, if the byte exists in the collection.
        ''' </summary>
        Public Function Contains(b As Byte) As Boolean
            Return InnerList.Contains(b)
        End Function

        ''' <summary>
        ''' Copies the content of the collection into the given array.
        ''' </summary>
        Public Sub CopyTo(bs As Byte(), index As Integer)
            InnerList.CopyTo(bs, index)
        End Sub

        ''' <summary>
        ''' Gets all bytes in the array
        ''' </summary>
        ''' <returns>an array of bytes.</returns>
        Public Function GetBytes() As Byte()
            Dim bytes = New Byte(Count - 1) {}
            InnerList.CopyTo(0, bytes, 0, bytes.Length)
            Return bytes
        End Function

        ''' <summary>
        ''' Returns the index of the given byte.
        ''' </summary>
        Public Function IndexOf(b As Byte) As Integer
            Return InnerList.IndexOf(b)
        End Function

        ''' <summary>
        ''' Inserts a byte to the collection.
        ''' </summary>
        ''' <paramname="index">the index</param>
        ''' <paramname="b">a byte to insert</param>
        Public Sub Insert(index As Integer, b As Byte)
            InnerList.Insert(index, b)
        End Sub

        ''' <summary>
        ''' Inserts a range of bytes to the collection.
        ''' </summary>
        ''' <paramname="index">the index of start byte</param>
        ''' <paramname="bs">an array of bytes to insert</param>
        Public Sub InsertRange(index As Integer, bs As Byte())
            InnerList.InsertRange(index, bs)
        End Sub

        ''' <summary>
        ''' Removes a byte from the collection.
        ''' </summary>
        ''' <paramname="b">the byte to remove</param>
        Public Sub Remove(b As Byte)
            List.Remove(b)
        End Sub

        ''' <summary>
        ''' Removes a range of bytes from the collection.
        ''' </summary>
        ''' <paramname="index">the index of the start byte</param>
        ''' <paramname="count">the count of the bytes to remove</param>
        Public Sub RemoveRange(index As Integer, count As Integer)
            InnerList.RemoveRange(index, count)
        End Sub

        ''' <summary>
        ''' Copies the content of the collection into an array.
        ''' </summary>
        ''' <returns>the array containing all bytes.</returns>
        Public Function ToArray() As Byte()
            Dim data = New Byte(Count - 1) {}
            CopyTo(data, 0)
            Return data
        End Function
    End Class
End Namespace
