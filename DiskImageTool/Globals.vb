Namespace App
    Module Globals
        Public ReadOnly BootstrapDB As New BootstrapDB
        Public ReadOnly TitleDB As New FloppyDB
        Public ReadOnly GlobalIdentifier As Guid = Guid.NewGuid()
        Public ReadOnly AppSettings As Settings.AppSettings = Settings.AppSettings.Load()
        Public ReadOnly UserState As Settings.UserState = Settings.UserState.Load()
    End Module
End Namespace
