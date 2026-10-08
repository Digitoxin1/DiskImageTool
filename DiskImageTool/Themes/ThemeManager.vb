Module ThemeManager
    Public Enum ThemeMode
        System
        Light
        Dark
    End Enum

    Public Event ThemeChanged As EventHandler

    Private _Mode As ThemeMode = ThemeMode.System
    Private _CurrentTheme As AppTheme = Themes.Light

    Public ReadOnly Property CurrentTheme As AppTheme
        Get
            Return _CurrentTheme
        End Get
    End Property

    Public Property Mode As ThemeMode
        Get
            Return _Mode
        End Get
        Set(value As ThemeMode)
            If _Mode = value Then Return

            _Mode = value
            UpdateTheme()
        End Set
    End Property

    Private Sub UpdateTheme()
        Dim newTheme As AppTheme

        Select Case _Mode
            Case ThemeMode.Dark
                newTheme = Themes.Dark

            Case ThemeMode.Light
                newTheme = Themes.Light

            Case Else
                newTheme = If(IsWindowsDarkMode(), Themes.Dark, Themes.Light)
        End Select

        If Object.ReferenceEquals(_CurrentTheme, newTheme) Then
            Return
        End If

        _CurrentTheme = newTheme

        RaiseEvent ThemeChanged(Nothing, EventArgs.Empty)
    End Sub

    Private Function IsWindowsDarkMode() As Boolean
        Using key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")

            If key Is Nothing Then Return False

            Dim value = key.GetValue("AppsUseLightTheme", 1)

            Return Convert.ToInt32(value) = 0
        End Using
    End Function

    Public Sub Refresh()
        UpdateTheme()
    End Sub
End Module
