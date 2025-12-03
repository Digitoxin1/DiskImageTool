Public Class ImagePreview
    Private ReadOnly _Disk As DiskImage.Disk
    Private ReadOnly _SummaryPanel As SummaryPanel
    Private WithEvents FilePanelMain As FilePanel
    Public Sub New(Disk As DiskImage.Disk)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Disk = Disk
        _SummaryPanel = New SummaryPanel(ListViewSummary)
        FilePanelMain = New FilePanel(ListViewFiles, True)

        Dim CurrentImage = New DiskImageContainer(_Disk, New ImageData(""))

        _SummaryPanel.Populate(CurrentImage, App.BootstrapDB)
        FilePanelMain.Load(CurrentImage, False)
    End Sub

    Public Shared Sub Display(Disk As DiskImage.Disk)
        With New ImagePreview(Disk)
            .ShowDialog()
        End With
    End Sub
End Class