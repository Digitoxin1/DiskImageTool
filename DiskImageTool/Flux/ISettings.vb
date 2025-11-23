Namespace Flux
    Friend Interface ISettings
        Property AppPath As String
        Property LogFileName As String
        Property LogStripPath As Boolean
        Function IsPathValid() As Boolean
    End Interface
End Namespace