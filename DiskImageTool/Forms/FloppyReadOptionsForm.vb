Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Public Class FloppyReadOptionsForm
    Private _DiskFormat As FloppyDiskFormat = FloppyDiskFormat.FloppyUnknown

    Public Sub New(DetectedType As FloppyDiskFormat)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        PopulateCombo(DetectedType)
    End Sub

    Private Sub LocalizeForm()
        BtnCancel.Text = My.Resources.Menu_Cancel
        BtnOK.Text = My.Resources.Menu_Ok
        Label1.Text = My.Resources.Label_FloppyDiskType
        Me.Text = My.Resources.Label_ChooseDiskType
    End Sub

    Public ReadOnly Property DiskFormat As FloppyDiskFormat
        Get
            Return _DiskFormat
        End Get
    End Property

    Private Sub PopulateCombo(DetectedType As FloppyDiskFormat)
        Dim DiskTypeItem As ComboDiskTypeItem
        ComboDiskType.BeginUpdate()
        ComboDiskType.Items.Clear()

        If DetectedType = FloppyDiskFormat.FloppyUnknown Then
            lblMessage.Text = My.Resources.Dialog_DiskTypeWarning
            DiskTypeItem = New ComboDiskTypeItem(DetectedType, False)
            Dim Index = ComboDiskType.Items.Add(DiskTypeItem)
            ComboDiskType.SelectedIndex = Index
        Else
            lblMessage.Text = ""
        End If

        Dim Items = System.Enum.GetValues(GetType(FloppyDiskFormat))
        For Each DiskFormat As FloppyDiskFormat In Items
            If (DiskFormat <> FloppyDiskFormat.FloppyUnknown And FloppyDiskFormatIsStandard(DiskFormat)) Or DetectedType = DiskFormat Then
                DiskTypeItem = New ComboDiskTypeItem(DiskFormat, DiskFormat = DetectedType)
                Dim Index = ComboDiskType.Items.Add(DiskTypeItem)
                If DiskFormat = DetectedType Then
                    ComboDiskType.SelectedIndex = Index
                End If
            End If
        Next

        ComboDiskType.EndUpdate()
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        If ComboDiskType.SelectedItem Is Nothing Then
            _DiskFormat = FloppyDiskFormat.FloppyUnknown
        Else
            Dim DiskTypeItem As ComboDiskTypeItem = ComboDiskType.SelectedItem
            _DiskFormat = DiskTypeItem.Format
        End If
    End Sub

    Private Sub ComboDiskType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDiskType.SelectedIndexChanged
        If ComboDiskType.SelectedItem Is Nothing Then
            BtnOK.Enabled = False
        Else
            Dim DiskTypeItem As ComboDiskTypeItem = ComboDiskType.SelectedItem
            BtnOK.Enabled = DiskTypeItem.Format <> FloppyDiskFormat.FloppyUnknown
        End If
    End Sub

    Private Class ComboDiskTypeItem
        Public Sub New(Format As FloppyDiskFormat, Detected As Boolean)
            _Detected = Detected
            _Format = Format
        End Sub

        Public Property Detected As Boolean

        Public Property Format As FloppyDiskFormat

        Public Overrides Function ToString() As String
            Dim FormatName As String

            If _Format = FloppyDiskFormat.FloppyUnknown Then
                FormatName = My.Resources.Label_Unknown
            Else
                FormatName = String.Format(My.Resources.Label_Floppy, FloppyDiskFormatGetName(_Format))
            End If

            Return FormatName & If(_Detected, " " & InParens(My.Resources.Label_Detected), "")
        End Function
    End Class

End Class