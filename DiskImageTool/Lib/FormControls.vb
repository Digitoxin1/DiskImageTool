Module FormControls

    Public Sub ControlRevertValue(Control As Control)
        Control.Text = CType(Control.Tag, FormControlData).LastValue
        ControlUpdateColor(Control)
    End Sub

    Public Sub ControlSetLastValue(Control As Control, Value As String)
        CType(Control.Tag, FormControlData).LastValue = Value
    End Sub

    Public Sub ControlSetValue(Control As Control, Value As String, CheckDiskType As Boolean)
        Control.Text = Value
        Dim Data = ControlSetTagValue(Control, Value)
        Data.CheckDiskType = CheckDiskType
        ControlUpdateColor(Control)
    End Sub

    Public Sub ControlSetValue(Control As Control, Value As String, AllowedValues() As String, CheckDiskType As Boolean)
        Control.Text = Value
        Dim Data = ControlSetTagValue(Control, Value)
        Data.AllowedValues = AllowedValues
        Data.CheckDiskType = CheckDiskType
        ControlUpdateColor(Control)
    End Sub

    Public Sub ControlSetValue(Control As HexTextBox, Value As String, CheckDiskType As Boolean)
        Control.SetHex(Value)
        Dim Data = ControlSetTagValue(Control, Value)
        Data.CheckDiskType = CheckDiskType
        ControlUpdateColor(Control)
    End Sub

    Public Sub ControlSetValue(Control As HexTextBox, Value() As Byte, CheckDiskType As Boolean)
        Control.SetHex(Value)
        Dim Data = ControlSetTagValue(Control, BitConverter.ToString(Value).Replace("-", ""))
        Data.CheckDiskType = CheckDiskType
        ControlUpdateColor(Control)
    End Sub

    Public Sub ControlUpdateBackColor(Control As Control)
        ControlUpdateBackColor(Control, False, True)
    End Sub

    Public Sub ControlUpdateBackColor(Control As Control, KnownDiskType As Boolean, DefaultValid As Boolean)
        Dim IsValid As Boolean

        Dim Data = CType(Control.Tag, FormControlData)

        If Data.CheckDiskType And KnownDiskType Then
            IsValid = True
        ElseIf Data.AllowedValues IsNot Nothing Then
            IsValid = Data.AllowedValues.Contains(Control.Text)
        Else
            IsValid = DefaultValid
        End If

        If IsValid Then
            Control.BackColor = SystemColors.Window
        Else
            Control.BackColor = Color.LightPink
        End If
    End Sub

    Public Sub ControlUpdateColor(Control As Control)
        Dim Data = CType(Control.Tag, FormControlData)

        If Control.Text <> Data.OriginalValue Then
            Control.ForeColor = Color.Blue
        Else
            Control.ForeColor = SystemColors.WindowText
        End If
    End Sub

    Private Function ControlSetTagValue(Control As Control, Value As String) As FormControlData
        If Control.Tag Is Nothing Then
            Control.Tag = New FormControlData(Value)
        Else
            ControlSetLastValue(Control, Value)
        End If

        Return Control.Tag
    End Function

    Public Function TryShowHelpAtPoint(Owner As Form, Provider As HelpProvider, hlpevent As HelpEventArgs) As Boolean
        If hlpevent.Handled OrElse Owner Is Nothing OrElse Provider Is Nothing Then
            Return False
        End If

        Dim Target = FindControlAtPoint(Owner, Owner.PointToClient(hlpevent.MousePos))

        While Target IsNot Nothing
            Dim HelpString = Provider.GetHelpString(Target)
            If Not String.IsNullOrEmpty(HelpString) Then
                Help.ShowPopup(Owner, HelpString, hlpevent.MousePos)
                hlpevent.Handled = True
                Return True
            End If
            Target = Target.Parent
        End While

        Return False
    End Function

    Private Function FindControlAtPoint(Container As Control, ClientPt As Point) As Control
        Dim Child = Container.GetChildAtPoint(ClientPt, GetChildAtPointSkip.Invisible)
        If Child Is Nothing Then
            Return Container
        End If

        Return FindControlAtPoint(Child, Child.PointToClient(Container.PointToScreen(ClientPt)))
    End Function

    Public Class FormControlData

        Public Sub New(Value As String)
            _OriginalValue = Value
            _LastValue = Value
            _AllowedValues = Nothing
            _CheckDiskType = False
        End Sub

        Public Property AllowedValues As String()
        Public Property CheckDiskType As Boolean
        Public Property LastValue As String
        Public Property OriginalValue As String
    End Class

    Public Class MediaDescriptorType

        Public Sub New(MediaDescriptor As String, Description As String)
            _MediaDescriptor = MediaDescriptor
            _Description = Description
        End Sub

        Public Property Description As String
        Public Property MediaDescriptor As String

        Public Overrides Function ToString() As String
            Return MediaDescriptor
        End Function

    End Class

End Module