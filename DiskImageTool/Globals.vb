Namespace App
    Module Globals
        Public ReadOnly BootstrapDB As New BootstrapDB
        Public ReadOnly TitleDB As New FloppyDB
        Public ReadOnly GlobalIdentifier As Guid = Guid.NewGuid()
    End Module
End Namespace
