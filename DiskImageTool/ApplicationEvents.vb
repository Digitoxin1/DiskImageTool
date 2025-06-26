Imports Microsoft.VisualBasic.ApplicationServices
Imports System.Globalization
Imports System.Threading

Namespace My
	Partial Friend Class MyApplication
		Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
			If My.Settings.Language <> "" Then
				' Set desired culture here
				Thread.CurrentThread.CurrentUICulture = New CultureInfo(My.Settings.Language)
				Thread.CurrentThread.CurrentCulture = New CultureInfo(My.Settings.Language)
			End If
		End Sub

	End Class
End Namespace
