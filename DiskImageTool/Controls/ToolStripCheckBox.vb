Imports System.ComponentModel
Imports System.Windows.Forms.Design

<ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip), DebuggerStepThrough()>
Public Class ToolStripCheckBox
    Inherits MyToolStripControlHost

    Public Event CheckedChanged As EventHandler

    Public Sub New()
        MyBase.New(New System.Windows.Forms.CheckBox())
        CheckBox.BackColor = Color.Transparent
    End Sub

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property CheckBox() As CheckBox
        Get
            Return CType(Control, CheckBox)
        End Get
    End Property

    <Description("Indicates whether the control is enabled."), Category("Behavior")>
    Public Overrides Property Enabled() As Boolean
        Get
            Return CheckBox.Enabled
        End Get
        Set(ByVal value As Boolean)
            CheckBox.Enabled = value
        End Set
    End Property

    <Description("Indicates whether the component is in the checked state."), Category("Appearance")>
    Public Property Checked() As Boolean
        Get
            Return CheckBox.Checked
        End Get
        Set(ByVal value As Boolean)
            CheckBox.Checked = value
        End Set
    End Property

    <Description("The text associated with the control."), Category("Appearance")>
    Public Overrides Property Text As String
        Get
            Return CheckBox.Text
        End Get
        Set(value As String)
            CheckBox.Text = value
        End Set
    End Property

    Protected Overrides Sub OnSubscribeControlEvents(ByVal c As Control)
        MyBase.OnSubscribeControlEvents(c)

        Dim CheckBoxControl As CheckBox = CType(c, CheckBox)

        AddHandler CheckBoxControl.CheckedChanged, AddressOf HandleCheckedChanged
    End Sub

    Protected Overrides Sub OnUnsubscribeControlEvents(ByVal c As Control)
        MyBase.OnUnsubscribeControlEvents(c)

        Dim CheckBoxControl As CheckBox = CType(c, CheckBox)

        RemoveHandler CheckBoxControl.CheckedChanged, AddressOf HandleCheckedChanged
    End Sub

    Private Sub HandleCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent CheckedChanged(Me, e)
    End Sub
End Class