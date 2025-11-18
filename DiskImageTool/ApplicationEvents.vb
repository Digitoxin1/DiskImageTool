Imports Microsoft.VisualBasic.ApplicationServices
Imports System.Globalization
Imports System.Threading

Namespace My
	Partial Friend Class MyApplication
		Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
			If App.Globals.AppSettings.Language <> "" Then
				' Set desired culture here
				Thread.CurrentThread.CurrentUICulture = New CultureInfo(App.Globals.AppSettings.Language)
				Thread.CurrentThread.CurrentCulture = New CultureInfo(App.Globals.AppSettings.Language)
			End If
		End Sub

	End Class
End Namespace
