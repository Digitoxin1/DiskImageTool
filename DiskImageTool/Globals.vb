Namespace App
    Module Globals
        Public ReadOnly GlobalIdentifier As Guid = Guid.NewGuid()
        Public ReadOnly AppSettings As Settings.AppSettings = Settings.AppSettings.Load()
        Public ReadOnly UserState As Settings.UserState = Settings.UserState.Load()
        Public ReadOnly BootstrapDB As New BootstrapDB
        Public ReadOnly TitleDB As New FloppyDB

        Public Function CurrentFormInstance() As Form
            Dim forms = Application.OpenForms
            Dim main As MainForm = Nothing

            For i As Integer = forms.Count - 1 To 0 Step -1
                Dim f As Form = forms(i)

                If f.Modal Then
                    Return f
                End If

                If main Is Nothing Then
                    main = TryCast(f, MainForm)
                End If
            Next

            Return main
        End Function
    End Module
End Namespace
