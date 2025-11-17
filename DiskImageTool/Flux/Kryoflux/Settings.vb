Namespace Flux
    Namespace Kryoflux
        Public Class Settings
            Public Property AppPath As String
                Get
                    Return My.Settings.DTC_Path
                End Get
                Set(value As String)
                    My.Settings.DTC_Path = value
                End Set
            End Property

            Public Property LogFileName As String
                Get
                    Return My.Settings.DTC_LogFileName
                End Get
                Set(value As String)
                    My.Settings.DTC_LogFileName = value
                End Set
            End Property

            Public Function IsPathValid() As Boolean
                Return Flux.IsPathValid(AppPath)
            End Function
        End Class
    End Namespace
End Namespace
