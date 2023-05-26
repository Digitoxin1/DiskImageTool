Imports System.ComponentModel
Imports System.Windows.Forms.Design

Public Class MyToolStripControlHost
    Inherits ToolStripControlHost

    Public Sub New()
        MyBase.New(New Control)
    End Sub

    Public Sub New(c As Control)
        MyBase.New(c)
    End Sub
End Class

<ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip), DebuggerStepThrough()>
Public Class ToolStripNumericUpDown
    Inherits MyToolStripControlHost

    Public Shadows Event KeyDown As KeyEventHandler
    Public Shadows Event KeyPress As KeyPressEventHandler
    Public Shadows Event KeyUp As KeyEventHandler
    Public Event ValueChanged As EventHandler

    Public Sub New()
        MyBase.New(New NumericUpDown)
    End Sub

    <Description("Indicates whether the edit box is read-only."), Category("Behavior")>
    Public Property [ReadOnly] As Boolean
        Get
            Return NumericUpDown.ReadOnly
        End Get
        Set(value As Boolean)
            NumericUpDown.ReadOnly = value
        End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property Accelerations As NumericUpDownAccelerationCollection
        Get
            Return NumericUpDown.Accelerations
        End Get
    End Property

    <Description("Indicates the border style of the up-down control."), Category("Appearance")>
    Public Property BorderStyle As Windows.Forms.BorderStyle
        Get
            Return NumericUpDown.BorderStyle
        End Get
        Set(value As Windows.Forms.BorderStyle)
            NumericUpDown.BorderStyle = value
        End Set
    End Property

    <Description("Indicates how the text should be aligned in the edit box."), Category("Appearance")>
    Public Property ControlTextAlign As HorizontalAlignment
        Get
            Return NumericUpDown.TextAlign
        End Get
        Set(value As HorizontalAlignment)
            NumericUpDown.TextAlign = value
        End Set
    End Property

    <Description("The cursor that appears when the pointer moves over the control."), Category("Appearance")>
    Public Property Cursor As Cursor
        Get
            Return NumericUpDown.Cursor
        End Get
        Set(value As Cursor)
            NumericUpDown.Cursor = Cursor
        End Set
    End Property

    <Description("Indicates the number of decimal places to display"), Category("Data")>
    Public Property DecimalPlaces As Integer
        Get
            Return NumericUpDown.DecimalPlaces
        End Get
        Set(value As Integer)
            NumericUpDown.DecimalPlaces = value
        End Set
    End Property

    <Description("Indicates whether the numeric up-down should display its value in hexadecimal."), Category("Appearance")>
    Public Property Hexadecimal As Boolean
        Get
            Return NumericUpDown.Hexadecimal
        End Get
        Set(value As Boolean)
            NumericUpDown.Hexadecimal = value
        End Set
    End Property

    <Description("Indicates the amount to increment or decrement on each button click"), Category("Data")>
    Public Property Increment As Decimal
        Get
            Return NumericUpDown.Increment
        End Get
        Set(value As Decimal)
            NumericUpDown.Increment = value
        End Set
    End Property

    <Description("Indicates whether the up-down control will increment and decrement the value when UP ARROW and DOWN ARROW keys are pressed."), Category("Behavior")>
    Public Property InterceptArrowKeys As Boolean
        Get
            Return NumericUpDown.InterceptArrowKeys
        End Get
        Set(value As Boolean)
            NumericUpDown.InterceptArrowKeys = value
        End Set
    End Property

    <Description("Indicates the maximum value for the numeric up-down control."), Category("Data")>
    Public Property Maximum As Decimal
        Get
            Return NumericUpDown.Maximum
        End Get
        Set(value As Decimal)
            NumericUpDown.Maximum = value
        End Set
    End Property

    <Description("Indicates the minimum value for the numeric up-down control."), Category("Data")>
    Public Property Minimum As Decimal
        Get
            Return NumericUpDown.Minimum
        End Get
        Set(value As Decimal)
            NumericUpDown.Minimum = value
        End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property NumericUpDown() As NumericUpDown
        Get
            Return CType(Control, NumericUpDown)
        End Get
    End Property
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Overrides Property Text As String
        Get
            Return NumericUpDown.Text
        End Get
        Set(value As String)
            NumericUpDown.Text = value
        End Set
    End Property

    <Description("Indicates whether the thousands separator will be inserted between every three decimal digits."), Category("Data")>
    Public Property ThousandsSeparator As Boolean
        Get
            Return NumericUpDown.ThousandsSeparator
        End Get
        Set(value As Boolean)
            NumericUpDown.ThousandsSeparator = value
        End Set
    End Property

    <Description("Indicates how the up-down control will position the up and down buttons relative to its edit box."), Category("Appearance")>
    Public Property UpDownAlign As LeftRightAlignment
        Get
            Return NumericUpDown.UpDownAlign
        End Get
        Set(value As LeftRightAlignment)
            NumericUpDown.UpDownAlign = value
        End Set
    End Property

    <Description("When this property is true, the Cursor property of the control and its child controls is set to WaitCursor"), Category("Appearance")>
    Public Property UseWaitCursor As Boolean
        Get
            Return NumericUpDown.UseWaitCursor
        End Get
        Set(value As Boolean)
            NumericUpDown.UseWaitCursor = value
        End Set
    End Property
    <Description("The current value of the numeric up-down control."), Category("Appearance")>
    Public Property Value As Decimal
        Get
            Return NumericUpDown.Value
        End Get
        Set(value As Decimal)
            NumericUpDown.Value = value
        End Set
    End Property

    Public Sub BeginInit()
        NumericUpDown.BeginInit()
    End Sub

    Public Sub DownButton()
        NumericUpDown.DownButton()
    End Sub

    Public Sub EndInit()
        NumericUpDown.EndInit()
    End Sub

    Public Overrides Function ToString() As String
        Return NumericUpDown.ToString
    End Function

    Public Sub UpButton()
        NumericUpDown.UpButton()
    End Sub

    Protected Overrides Sub OnSubscribeControlEvents(c As Control)
        MyBase.OnSubscribeControlEvents(c)

        Dim NumericUpDownControl As NumericUpDown = CType(c, NumericUpDown)

        AddHandler NumericUpDownControl.ValueChanged, AddressOf HandleValueChanged
        AddHandler NumericUpDownControl.KeyDown, AddressOf HandleKeyDown
        AddHandler NumericUpDownControl.KeyPress, AddressOf HandleKeyPress
        AddHandler NumericUpDownControl.KeyUp, AddressOf HandleKeyUp
    End Sub

    Protected Overrides Sub OnUnsubscribeControlEvents(c As Control)
        MyBase.OnUnsubscribeControlEvents(c)

        Dim NumericUpDownControl As NumericUpDown = CType(c, NumericUpDown)

        RemoveHandler NumericUpDownControl.ValueChanged, AddressOf HandleValueChanged
        RemoveHandler NumericUpDownControl.KeyDown, AddressOf HandleKeyDown
        RemoveHandler NumericUpDownControl.KeyPress, AddressOf HandleKeyPress
        RemoveHandler NumericUpDownControl.KeyUp, AddressOf HandleKeyUp
    End Sub

    Private Sub HandleKeyDown(sender As Object, e As KeyEventArgs)
        RaiseEvent KeyDown(Me, e)
    End Sub

    Private Sub HandleKeyPress(sender As Object, e As KeyPressEventArgs)
        RaiseEvent KeyPress(Me, e)
    End Sub

    Private Sub HandleKeyUp(sender As Object, e As KeyEventArgs)
        RaiseEvent KeyUp(Me, e)
    End Sub

    Private Sub HandleValueChanged(sender As Object, e As EventArgs)
        RaiseEvent ValueChanged(Me, e)
    End Sub
End Class
