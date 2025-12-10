Imports DiskImageTool.Settings

Namespace Flux
    Public Class GeneralSettingsPanel
        Private _Initialized As Boolean = False
        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            LocalizeForm()
        End Sub

        Private Sub LocalizeForm()
            GroupBoxImageSaveLocation.Text = My.Resources.Caption_Settings_ConvertedImages
            RadioImageConvertLastUsed.Text = My.Resources.Radio_Settings_ConvertLastUsed
            RadioImageConvertParentFolder.Text = My.Resources.Radio_Settings_ConvertParentFolder
            RadioImageConvertSameFolder.Text = My.Resources.Radio_Settings_ConvertSameFolder
        End Sub

        Public ReadOnly Property Initialized As Boolean
            Get
                Return _Initialized
            End Get
        End Property

        Public Sub Initialize()
            InitializeFields()
            _Initialized = True
        End Sub

        Public Sub UpdateSettings()
            If Not _Initialized Then
                Exit Sub
            End If

            If RadioImageConvertParentFolder.Checked Then
                App.AppSettings.ImageConvertStartPathMode = AppSettings.ImageConvertPathMode.ParentOfFlux
            ElseIf RadioImageConvertSameFolder.Checked Then
                App.AppSettings.ImageConvertStartPathMode = AppSettings.ImageConvertPathMode.SameAsFlux
            Else
                App.AppSettings.ImageConvertStartPathMode = AppSettings.ImageConvertPathMode.LastSavedImage
            End If
        End Sub

        Private Sub InitializeFields()
            Select Case App.AppSettings.ImageConvertStartPathMode
                Case AppSettings.ImageConvertPathMode.ParentOfFlux
                    RadioImageConvertParentFolder.Checked = True
                Case AppSettings.ImageConvertPathMode.SameAsFlux
                    RadioImageConvertSameFolder.Checked = True
                Case Else
                    RadioImageConvertLastUsed.Checked = True
            End Select
        End Sub
    End Class
End Namespace
