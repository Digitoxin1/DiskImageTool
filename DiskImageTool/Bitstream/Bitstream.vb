Namespace Bitstream
    Public Class Bitstream
        Private ReadOnly _BitArray As BitArray
        Private _Index As Integer

        Public Sub New(BitArray As BitArray)
            _BitArray = BitArray
            _Index = 0
        End Sub

        Public ReadOnly Property BitArray As BitArray
            Get
                Return _BitArray
            End Get
        End Property

        Public Property Index As Integer
            Get
                Return _Index
            End Get
            Set(value As Integer)
                _Index = GetIndex(value)
            End Set
        End Property

        Public ReadOnly Property Length As Integer
            Get
                Return _BitArray.Length
            End Get
        End Property

        Public Function CountBytes(Value As Byte, MaxLength As Integer) As Integer
            Return CountBytes(Value, MaxLength, _Index)
        End Function

        Public Function CountBytes(Value As Byte, MaxLength As Integer, Start As Integer) As Integer
            Dim b As Byte
            Dim Count As Integer = 0

            Index = Start

            Do
                b = GetPrevByte()
                If b = Value Then
                    Count += 1
                Else
                    Index += 16
                End If
            Loop Until b <> value Or Count = MaxLength

            Return Count
        End Function

        Public Function FindPattern(Pattern As BitArray) As Integer
            Return FindPattern(Pattern, _Index)
        End Function

        Public Function FindPattern(Pattern As BitArray, Start As Integer) As Integer
            Dim Found As Boolean
            Dim SearchPos As Integer

            Start = GetIndex(Start)
            Dim Pos = Start
            Do
                Found = True
                SearchPos = Pos
                For i = 0 To Pattern.Length - 1
                    If Pattern(i) <> _BitArray(SearchPos) Then
                        Found = False
                        Exit For
                    End If
                    SearchPos = GetIndex(SearchPos + 1)
                Next
                If Not Found Then
                    Pos = GetIndex(Pos + 1)
                End If
            Loop Until Pos = Start Or Found

            If Found Then
                Return Pos
            Else
                Return -1
            End If
        End Function

        Public Function GetByte() As Byte
            Dim Value As Byte = 0
            Dim clockBit As Boolean
            Dim dataBit As Boolean

            For i = 0 To 7
                clockBit = _BitArray(Index)
                Index += 1
                dataBit = _BitArray(Index)
                Index += 1

                Value = (Value << 1) Or (CInt(dataBit) And 1)
            Next

            Return Value
        End Function

        Public Function GetByte(Index As Integer) As Byte
            Me.Index = Index
            Return GetByte()
        End Function

        Public Function GetBytes(Count As UInteger) As Byte()
            Dim Buffer(Count - 1) As Byte

            For i = 0 To Count - 1
                Buffer(i) = GetByte()
            Next

            Return Buffer
        End Function

        Public Function GetBytes(Index As Integer, Count As UInteger) As Byte()
            Me.Index = Index
            Return GetBytes(Count)
        End Function

        Public Function GetPrevByte() As Byte
            Index -= 16
            Dim Value As Byte = GetByte()
            Index -= 16

            Return Value
        End Function

        Public Function GetPrevBytes(Count As UInteger) As Byte()
            Index -= (Count * 16)
            Dim Buffer = GetBytes(Count)
            Index -= (Count * 16)

            Return Buffer
        End Function

        Private Function GetIndex(Value As Integer) As Integer
            If Value < 0 Then
                Value = Value Mod _BitArray.Length
                Value = _BitArray.Length + Value
            ElseIf Value > _BitArray.Length - 1 Then
                Value = Value Mod _BitArray.Length
            End If

            Return Value
        End Function
    End Class
End Namespace
