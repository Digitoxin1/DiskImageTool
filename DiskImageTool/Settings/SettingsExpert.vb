Imports CompactJson

Namespace Settings
    Public Class SettingsExpert
        Inherits SettingsGroup

        Private _writeSpliceDisplayDetails As Boolean = False
        Private _writeSpliceDisplayFilename As Boolean = True
        Private _writeSpliceFileName As String = "WriteSplices.txt"
        Private _writeSpliceHeader As String = ""

        Public Property WriteSpliceDisplayDetails As Boolean
            Get
                Return _writeSpliceDisplayDetails
            End Get
            Set(Value As Boolean)
                If _writeSpliceDisplayDetails <> Value Then
                    _writeSpliceDisplayDetails = Value
                    MarkDirty()
                End If
            End Set
        End Property
        Public Property WriteSpliceDisplayFilename As Boolean
            Get
                Return _writeSpliceDisplayFilename
            End Get
            Set(Value As Boolean)
                If _writeSpliceDisplayFilename <> Value Then
                    _writeSpliceDisplayFilename = Value
                    MarkDirty()
                End If
            End Set
        End Property

        Public Property WriteSpliceFileName As String
            Get
                Return _writeSpliceFileName
            End Get
            Set(value As String)
                If _writeSpliceFileName <> value Then
                    _writeSpliceFileName = value
                    MarkDirty()
                End If
            End Set
        End Property

        Public Property WriteSpliceHeader As String
            Get
                Return _writeSpliceHeader
            End Get
            Set(value As String)
                If _writeSpliceHeader <> value Then
                    _writeSpliceHeader = value
                    MarkDirty()
                End If
            End Set
        End Property

        Public Sub LoadFromDictionary(dict As Dictionary(Of String, JsonValue))
            If dict Is Nothing Then
                MarkClean()
                Return
            End If

            _writeSpliceFileName = ReadValue(dict, "writeSpliceFileName", _writeSpliceFileName)
            _writeSpliceHeader = ReadValue(dict, "writeSpliceHeader", _writeSpliceHeader)
            _writeSpliceDisplayFilename = ReadValue(dict, "writeSpliceDisplayFilename", _writeSpliceDisplayFilename)
            _writeSpliceDisplayDetails = ReadValue(dict, "writeSpliceDisplayDetails", _writeSpliceDisplayDetails)

            MarkClean()
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Dim result As New Dictionary(Of String, Object)

            result("writeSpliceFileName") = _writeSpliceFileName
            result("writeSpliceDisplayFilename") = _writeSpliceDisplayFilename
            result("writeSpliceDisplayDetails") = _writeSpliceDisplayDetails
            If Not String.IsNullOrEmpty(_writeSpliceHeader) Then
                result("writeSpliceHeader") = _writeSpliceHeader
            End If

            Return result
        End Function
    End Class
End Namespace
