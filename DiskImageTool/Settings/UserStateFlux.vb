Imports CompactJson

Namespace Settings
    Public Class UserStateFlux
        Inherits SettingsGroup

        Private ReadOnly _Convert As UserStateFluxConvert
        Private ReadOnly _Read As UserStateFluxRead

        Public ReadOnly Property Convert As UserStateFluxConvert
            Get
                Return _Convert
            End Get
        End Property

        Public ReadOnly Property Read As UserStateFluxRead
            Get
                Return _Read
            End Get
        End Property

        Public Sub New()
            _Convert = New UserStateFluxConvert
            _Read = New UserStateFluxRead
        End Sub

        Public Overrides ReadOnly Property IsDirty As Boolean
            Get
                Return MyBase.IsDirty OrElse Convert.IsDirty OrElse Read.IsDirty
            End Get
        End Property

        Public Sub LoadFromDictionary(dict As Dictionary(Of String, JsonValue))
            If dict Is Nothing Then
                MarkClean()
                Return
            End If

            _Convert.LoadFromDictionary(ReadSection(dict, "convert"))
            _Read.LoadFromDictionary(ReadSection(dict, "read"))

            MarkClean()
        End Sub

        Public Function ToJsonObject() As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"convert", _Convert.ToJsonObject},
                {"read", _Read.ToJsonObject}
            }
        End Function

        Public Overrides Sub MarkClean()
            MyBase.MarkClean()
            Convert.MarkClean()
            Read.MarkClean()
        End Sub
    End Class
End Namespace
