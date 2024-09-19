Imports DiskImageTool.DiskImage.FloppyDiskFunctions

Public Class FloppyReadOptionsForm
    Private _DiskType As FloppyDiskType = FloppyDiskType.FloppyUnknown

    Public Sub New(DetectedType As FloppyDiskType)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        PopulateCombo(DetectedType)
    End Sub

    Public ReadOnly Property DiskType As FloppyDiskType
        Get
            Return _DiskType
        End Get
    End Property

    Private Sub PopulateCombo(DetectedType As FloppyDiskType)
        Dim DiskTypeItem As ComboDiskTypeItem
        ComboDiskType.BeginUpdate()
        ComboDiskType.Items.Clear()

        If DetectedType = FloppyDiskType.FloppyUnknown Then
            lblMessage.Text = "Warning: Unable to determine the floppy disk type."
            DiskTypeItem = New ComboDiskTypeItem(DetectedType, False)
            Dim Index = ComboDiskType.Items.Add(DiskTypeItem)
            ComboDiskType.SelectedIndex = Index
        Else
            lblMessage.Text = ""
        End If

        Dim Items = System.Enum.GetValues(GetType(FloppyDiskType))
        For Each Type As FloppyDiskType In Items
            If (Type <> FloppyDiskType.FloppyUnknown And IsDiskTypeValidForRead(Type)) Or DetectedType = Type Then
                DiskTypeItem = New ComboDiskTypeItem(Type, Type = DetectedType)
                Dim Index = ComboDiskType.Items.Add(DiskTypeItem)
                If Type = DetectedType Then
                    ComboDiskType.SelectedIndex = Index
                End If
            End If
        Next

        ComboDiskType.EndUpdate()
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        If ComboDiskType.SelectedItem Is Nothing Then
            _DiskType = FloppyDiskType.FloppyUnknown
        Else
            Dim DiskTypeItem As ComboDiskTypeItem = ComboDiskType.SelectedItem
            _DiskType = DiskTypeItem.Type
        End If
    End Sub

    Private Sub ComboDiskType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboDiskType.SelectedIndexChanged
        If ComboDiskType.SelectedItem Is Nothing Then
            BtnOK.Enabled = False
        Else
            Dim DiskTypeItem As ComboDiskTypeItem = ComboDiskType.SelectedItem
            BtnOK.Enabled = DiskTypeItem.Type <> FloppyDiskType.FloppyUnknown
        End If
    End Sub

    Private Class ComboDiskTypeItem
        Public Sub New(Type As FloppyDiskType, Detected As Boolean)
            _Detected = Detected
            _Type = Type
        End Sub

        Public Property Detected As Boolean

        Public Property Type As FloppyDiskType

        Public Overrides Function ToString() As String
            Dim TypeName As String

            If _Type = FloppyDiskType.FloppyUnknown Then
                TypeName = "Unknown"
            Else
                TypeName = GetFloppyDiskTypeName(_Type) & " Floppy"
            End If

            Return TypeName & IIf(_Detected, " (Detected)", "")
        End Function
    End Class

End Class