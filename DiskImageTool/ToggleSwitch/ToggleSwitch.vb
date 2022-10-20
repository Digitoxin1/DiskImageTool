Imports System
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports DiskImageTool.ToggleSwitch

' *******************************************************************************
' ToggleSwitch - Version  1.1                           
' *******************************************************************************
' http://www.codeproject.com/Articles/1029499/ToggleSwitch-Winforms-Control    
' *******************************************************************************


Namespace JCS
    <DefaultValue("Checked"), DefaultEvent("CheckedChanged"), ToolboxBitmap(GetType(CheckBox))>
    Public Class ToggleSwitch
        Inherits Control
#Region "Delegate and Event declarations"

        Public Delegate Sub CheckedChangedDelegate(ByVal sender As Object, ByVal e As EventArgs)
        <Description("Raised when the ToggleSwitch has changed state")>
        Public Event CheckedChanged As CheckedChangedDelegate

        Public Delegate Sub BeforeRenderingDelegate(ByVal sender As Object, ByVal e As BeforeRenderingEventArgs)
        <Description("Raised when the ToggleSwitch renderer is changed")>
        Public Event BeforeRendering As BeforeRenderingDelegate

#End Region

#Region "Enums"

        Public Enum ToggleSwitchStyle
            PlainAndSimpel
        End Enum

        Public Enum ToggleSwitchAlignment
            Near
            Center
            Far
        End Enum

        Public Enum ToggleSwitchButtonAlignment
            Left
            Center
            Right
        End Enum

#End Region

#Region "Private Members"

        Private ReadOnly _animationTimer As Timer = New Timer()
        Private _renderer As ToggleSwitchRendererBase

        Private _style As ToggleSwitchStyle = ToggleSwitchStyle.PlainAndSimpel
        Private _checked As Boolean = False
        Private _moving As Boolean = False
        Private _animating As Boolean = False
        Private _animationResult As Boolean = False
        Private _animationTarget As Integer = 0
        Private _useAnimation As Boolean = True
        Private _animationInterval As Integer = 1
        Private _animationStep As Integer = 10
        Private _allowUserChange As Boolean = True

        Private _isLeftFieldHovered As Boolean = False
        Private _isButtonHovered As Boolean = False
        Private _isRightFieldHovered As Boolean = False
        Private _isLeftFieldPressed As Boolean = False
        Private _isButtonPressed As Boolean = False
        Private _isRightFieldPressed As Boolean = False

        Private _buttonValue As Integer = 0
        Private _savedButtonValue As Integer = 0
        Private _xOffset As Integer = 0
        Private _xValue As Integer = 0
        Private _thresholdPercentage As Integer = 50
        Private _grayWhenDisabled As Boolean = True
        Private _toggleOnButtonClick As Boolean = True
        Private _toggleOnSideClick As Boolean = True

        Private _lastMouseEventArgs As MouseEventArgs = Nothing

        Private _buttonScaleImage As Boolean
        Private _buttonAlignment As ToggleSwitchButtonAlignment = ToggleSwitchButtonAlignment.Center
        Private _buttonImage As Image = Nothing

        Private _offText As String = ""
        Private _offForeColor As Color = Color.Black
        Private _offFont As Font
        Private _offSideImage As Image = Nothing
        Private _offSideScaleImage As Boolean
        Private _offSideAlignment As ToggleSwitchAlignment = ToggleSwitchAlignment.Center
        Private _offButtonImage As Image = Nothing
        Private _offButtonScaleImage As Boolean
        Private _offButtonAlignment As ToggleSwitchButtonAlignment = ToggleSwitchButtonAlignment.Center

        Private _onText As String = ""
        Private _onForeColor As Color = Color.Black
        Private _onFont As Font
        Private _onSideImage As Image = Nothing
        Private _onSideScaleImage As Boolean
        Private _onSideAlignment As ToggleSwitchAlignment = ToggleSwitchAlignment.Center
        Private _onButtonImage As Image = Nothing
        Private _onButtonScaleImage As Boolean
        Private _onButtonAlignment As ToggleSwitchButtonAlignment = ToggleSwitchButtonAlignment.Center

#End Region

#Region "Constructor Etc."

        Public Sub New()
            SetStyle(ControlStyles.ResizeRedraw Or ControlStyles.SupportsTransparentBackColor Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.DoubleBuffer, True)

            OnFont = MyBase.Font
            OffFont = MyBase.Font

            SetRenderer(New ToggleSwitchPlainAndSimpleRenderer())

            _animationTimer.Enabled = False
            _animationTimer.Interval = _animationInterval
            AddHandler _animationTimer.Tick, AddressOf AnimationTimer_Tick
        End Sub

        Public Sub SetRenderer(ByVal renderer As ToggleSwitchRendererBase)
            renderer.SetToggleSwitch(Me)
            _renderer = renderer

            If _renderer IsNot Nothing Then Refresh()
        End Sub

#End Region

#Region "Public Properties"

        <Bindable(False)>
        <DefaultValue(GetType(ToggleSwitchStyle), "Metro")>
        <Category("Appearance")>
        <Description("Gets or sets the style of the ToggleSwitch")>
        Public Property Style As ToggleSwitchStyle
            Get
                Return _style
            End Get
            Set(ByVal value As ToggleSwitchStyle)
                If value <> _style Then
                    _style = value

                    Select Case _style
                        Case ToggleSwitchStyle.PlainAndSimpel
                            SetRenderer(New ToggleSwitchPlainAndSimpleRenderer())
                    End Select
                End If

                Refresh()
            End Set
        End Property

        <Bindable(True)>
        <DefaultValue(False)>
        <Category("Data")>
        <Description("Gets or sets the Checked value of the ToggleSwitch")>
        Public Property Checked As Boolean
            Get
                Return _checked
            End Get
            Set(ByVal value As Boolean)
                If value <> _checked Then
                    While _animating
                        Application.DoEvents()
                    End While

                    If value = True Then
                        Dim buttonWidth As Integer = _renderer.GetButtonWidth()
                        _animationTarget = Width - buttonWidth
                        BeginAnimation(True)
                    Else
                        _animationTarget = 0
                        BeginAnimation(False)
                    End If
                End If
            End Set
        End Property

        <Bindable(True)>
        <DefaultValue(True)>
        <Category("Behavior")>
        <Description("Gets or sets whether the user can change the value of the button or not")>
        Public Property AllowUserChange As Boolean
            Get
                Return _allowUserChange
            End Get
            Set(ByVal value As Boolean)
                _allowUserChange = value
            End Set
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property CheckedString As String
            Get
                Return If(Checked, If(String.IsNullOrEmpty(OnText), "ON", OnText), If(String.IsNullOrEmpty(OffText), "OFF", OffText))
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public ReadOnly Property ButtonRectangle As Rectangle
            Get
                Return _renderer.GetButtonRectangle()
            End Get
        End Property

        <Bindable(False)>
        <DefaultValue(True)>
        <Category("Appearance")>
        <Description("Gets or sets if the ToggleSwitch should be grayed out when disabled")>
        Public Property GrayWhenDisabled As Boolean
            Get
                Return _grayWhenDisabled
            End Get
            Set(ByVal value As Boolean)
                If value <> _grayWhenDisabled Then
                    _grayWhenDisabled = value

                    If Not Enabled Then Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(True)>
        <Category("Behavior")>
        <Description("Gets or sets if the ToggleSwitch should toggle when the button is clicked")>
        Public Property ToggleOnButtonClick As Boolean
            Get
                Return _toggleOnButtonClick
            End Get
            Set(ByVal value As Boolean)
                _toggleOnButtonClick = value
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(True)>
        <Category("Behavior")>
        <Description("Gets or sets if the ToggleSwitch should toggle when the track besides the button is clicked")>
        Public Property ToggleOnSideClick As Boolean
            Get
                Return _toggleOnSideClick
            End Get
            Set(ByVal value As Boolean)
                _toggleOnSideClick = value
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(50)>
        <Category("Behavior")>
        <Description("Gets or sets how much the button need to be on the other side (in peercept) before it snaps")>
        Public Property ThresholdPercentage As Integer
            Get
                Return _thresholdPercentage
            End Get
            Set(ByVal value As Integer)
                _thresholdPercentage = value
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(GetType(Color), "Black")>
        <Category("Appearance")>
        <Description("Gets or sets the forecolor of the text when Checked=false")>
        Public Property OffForeColor As Color
            Get
                Return _offForeColor
            End Get
            Set(ByVal value As Color)
                If value <> _offForeColor Then
                    _offForeColor = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <ComponentModel.DefaultValueAttribute(GetType(Font), "Microsoft Sans Serif, 8.25pt")>
        <Category("Appearance")>
        <Description("Gets or sets the font of the text when Checked=false")>
        Public Property OffFont As Font
            Get
                Return _offFont
            End Get
            Set(ByVal value As Font)
                If Not value.Equals(_offFont) Then
                    _offFont = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue("")>
        <Category("Appearance")>
        <Description("Gets or sets the text when Checked=true")>
        Public Property OffText As String
            Get
                Return _offText
            End Get
            Set(ByVal value As String)
                If Not Equals(value, _offText) Then
                    _offText = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <Category("Appearance")>
        <Description("Gets or sets the side image when Checked=false - Note: Settings the OffSideImage overrules the OffText property. Only the image will be shown")>
        Public Property OffSideImage As Image
            Get
                Return _offSideImage
            End Get
            Set(ByVal value As Image)
                If value IsNot _offSideImage Then
                    _offSideImage = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(False)>
        <Category("Behavior")>
        <Description("Gets or sets whether the side image visible when Checked=false should be scaled to fit")>
        Public Property OffSideScaleImageToFit As Boolean
            Get
                Return _offSideScaleImage
            End Get
            Set(ByVal value As Boolean)
                If value <> _offSideScaleImage Then
                    _offSideScaleImage = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(GetType(ToggleSwitchAlignment), "Center")>
        <Category("Appearance")>
        <Description("Gets or sets how the text or side image visible when Checked=false should be aligned")>
        Public Property OffSideAlignment As ToggleSwitchAlignment
            Get
                Return _offSideAlignment
            End Get
            Set(ByVal value As ToggleSwitchAlignment)
                If value <> _offSideAlignment Then
                    _offSideAlignment = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <Category("Appearance")>
        <Description("Gets or sets the button image when Checked=false and ButtonImage is not set")>
        Public Property OffButtonImage As Image
            Get
                Return _offButtonImage
            End Get
            Set(ByVal value As Image)
                If value IsNot _offButtonImage Then
                    _offButtonImage = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(False)>
        <Category("Behavior")>
        <Description("Gets or sets whether the button image visible when Checked=false should be scaled to fit")>
        Public Property OffButtonScaleImageToFit As Boolean
            Get
                Return _offButtonScaleImage
            End Get
            Set(ByVal value As Boolean)
                If value <> _offButtonScaleImage Then
                    _offButtonScaleImage = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(GetType(ToggleSwitchButtonAlignment), "Center")>
        <Category("Appearance")>
        <Description("Gets or sets how the button image visible when Checked=false should be aligned")>
        Public Property OffButtonAlignment As ToggleSwitchButtonAlignment
            Get
                Return _offButtonAlignment
            End Get
            Set(ByVal value As ToggleSwitchButtonAlignment)
                If value <> _offButtonAlignment Then
                    _offButtonAlignment = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(GetType(Color), "Black")>
        <Category("Appearance")>
        <Description("Gets or sets the forecolor of the text when Checked=true")>
        Public Property OnForeColor As Color
            Get
                Return _onForeColor
            End Get
            Set(ByVal value As Color)
                If value <> _onForeColor Then
                    _onForeColor = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <ComponentModel.DefaultValueAttribute(GetType(Font), "Microsoft Sans Serif, 8,25pt")>
        <Category("Appearance")>
        <Description("Gets or sets the font of the text when Checked=true")>
        Public Property OnFont As Font
            Get
                Return _onFont
            End Get
            Set(ByVal value As Font)
                If Not value.Equals(_onFont) Then
                    _onFont = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue("")>
        <Category("Appearance")>
        <Description("Gets or sets the text when Checked=true")>
        Public Property OnText As String
            Get
                Return _onText
            End Get
            Set(ByVal value As String)
                If Not Equals(value, _onText) Then
                    _onText = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <Category("Appearance")>
        <Description("Gets or sets the side image visible when Checked=true - Note: Settings the OnSideImage overrules the OnText property. Only the image will be shown.")>
        Public Property OnSideImage As Image
            Get
                Return _onSideImage
            End Get
            Set(ByVal value As Image)
                If value IsNot _onSideImage Then
                    _onSideImage = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(False)>
        <Category("Behavior")>
        <Description("Gets or sets whether the side image visible when Checked=true should be scaled to fit")>
        Public Property OnSideScaleImageToFit As Boolean
            Get
                Return _onSideScaleImage
            End Get
            Set(ByVal value As Boolean)
                If value <> _onSideScaleImage Then
                    _onSideScaleImage = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <Category("Appearance")>
        <Description("Gets or sets the button image")>
        Public Property ButtonImage As Image
            Get
                Return _buttonImage
            End Get
            Set(ByVal value As Image)
                If value IsNot _buttonImage Then
                    _buttonImage = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(False)>
        <Category("Behavior")>
        <Description("Gets or sets whether the button image should be scaled to fit")>
        Public Property ButtonScaleImageToFit As Boolean
            Get
                Return _buttonScaleImage
            End Get
            Set(ByVal value As Boolean)
                If value <> _buttonScaleImage Then
                    _buttonScaleImage = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(GetType(ToggleSwitchButtonAlignment), "Center")>
        <Category("Appearance")>
        <Description("Gets or sets how the button image should be aligned")>
        Public Property ButtonAlignment As ToggleSwitchButtonAlignment
            Get
                Return _buttonAlignment
            End Get
            Set(ByVal value As ToggleSwitchButtonAlignment)
                If value <> _buttonAlignment Then
                    _buttonAlignment = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(GetType(ToggleSwitchAlignment), "Center")>
        <Category("Appearance")>
        <Description("Gets or sets how the text or side image visible when Checked=true should be aligned")>
        Public Property OnSideAlignment As ToggleSwitchAlignment
            Get
                Return _onSideAlignment
            End Get
            Set(ByVal value As ToggleSwitchAlignment)
                If value <> _onSideAlignment Then
                    _onSideAlignment = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <Category("Appearance")>
        <Description("Gets or sets the button image visible when Checked=true and ButtonImage is not set")>
        Public Property OnButtonImage As Image
            Get
                Return _onButtonImage
            End Get
            Set(ByVal value As Image)
                If value IsNot _onButtonImage Then
                    _onButtonImage = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(False)>
        <Category("Behavior")>
        <Description("Gets or sets whether the button image visible when Checked=true should be scaled to fit")>
        Public Property OnButtonScaleImageToFit As Boolean
            Get
                Return _onButtonScaleImage
            End Get
            Set(ByVal value As Boolean)
                If value <> _onButtonScaleImage Then
                    _onButtonScaleImage = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(GetType(ToggleSwitchButtonAlignment), "Center")>
        <Category("Appearance")>
        <Description("Gets or sets how the button image visible when Checked=true should be aligned")>
        Public Property OnButtonAlignment As ToggleSwitchButtonAlignment
            Get
                Return _onButtonAlignment
            End Get
            Set(ByVal value As ToggleSwitchButtonAlignment)
                If value <> _onButtonAlignment Then
                    _onButtonAlignment = value
                    Refresh()
                End If
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(True)>
        <Category("Behavior")>
        <Description("Gets or sets whether the toggle change should be animated or not")>
        Public Property UseAnimation As Boolean
            Get
                Return _useAnimation
            End Get
            Set(ByVal value As Boolean)
                _useAnimation = value
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(1)>
        <Category("Behavior")>
        <Description("Gets or sets the interval in ms between animation frames")>
        Public Property AnimationInterval As Integer
            Get
                Return _animationInterval
            End Get
            Set(ByVal value As Integer)
                If value <= 0 Then
                    Throw New ArgumentOutOfRangeException("AnimationInterval must larger than zero!")
                End If

                _animationInterval = value
            End Set
        End Property

        <Bindable(False)>
        <DefaultValue(10)>
        <Category("Behavior")>
        <Description("Gets or sets the step in pixes the button shouldbe moved between each animation interval")>
        Public Property AnimationStep As Integer
            Get
                Return _animationStep
            End Get
            Set(ByVal value As Integer)
                If value <= 0 Then
                    Throw New ArgumentOutOfRangeException("AnimationStep must larger than zero!")
                End If

                _animationStep = value
            End Set
        End Property

#Region "Hidden Base Properties"

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Overloads Property Text As String
            Get
                Return ""
            End Get
            Set(ByVal value As String)
                MyBase.Text = ""
            End Set
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Overloads Property ForeColor As Color
            Get
                Return Color.Black
            End Get
            Set(ByVal value As Color)
                MyBase.ForeColor = Color.Black
            End Set
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Public Overloads Property Font As Font
            Get
                Return MyBase.Font
            End Get
            Set(ByVal value As Font)
                MyBase.Font = New Font(MyBase.Font, FontStyle.Regular)
            End Set
        End Property

#End Region

#End Region

#Region "Internal Properties"

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property IsButtonHovered As Boolean
            Get
                Return _isButtonHovered AndAlso Not _isButtonPressed
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property IsButtonPressed As Boolean
            Get
                Return _isButtonPressed
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property IsLeftSideHovered As Boolean
            Get
                Return _isLeftFieldHovered AndAlso Not _isLeftFieldPressed
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property IsLeftSidePressed As Boolean
            Get
                Return _isLeftFieldPressed
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property IsRightSideHovered As Boolean
            Get
                Return _isRightFieldHovered AndAlso Not _isRightFieldPressed
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property IsRightSidePressed As Boolean
            Get
                Return _isRightFieldPressed
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend Property ButtonValue As Integer
            Get
                If _animating OrElse _moving Then
                    Return _buttonValue
                ElseIf _checked Then
                    Return Width - _renderer.GetButtonWidth()
                Else
                    Return 0
                End If
            End Get
            Set(ByVal value As Integer)
                If value <> _buttonValue Then
                    _buttonValue = value
                    Refresh()
                End If
            End Set
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property IsButtonOnLeftSide As Boolean
            Get
                Return ButtonValue <= 0
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property IsButtonOnRightSide As Boolean
            Get
                Return (ButtonValue >= (Width - _renderer.GetButtonWidth()))
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property IsButtonMovingLeft As Boolean
            Get
                Return _animating AndAlso Not IsButtonOnLeftSide AndAlso _animationResult = False
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property IsButtonMovingRight As Boolean
            Get
                Return _animating AndAlso Not IsButtonOnRightSide AndAlso _animationResult = True
            End Get
        End Property

        <Browsable(False)>
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
        Friend ReadOnly Property AnimationResult As Boolean
            Get
                Return _animationResult
            End Get
        End Property

#End Region

#Region "Overridden Control Methods"

        Protected Overrides ReadOnly Property DefaultSize As Size
            Get
                Return New Size(50, 19)
            End Get
        End Property

        Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
            pevent.Graphics.ResetClip()

            MyBase.OnPaintBackground(pevent)

            If _renderer IsNot Nothing Then _renderer.RenderBackground(pevent)
        End Sub

        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
            e.Graphics.ResetClip()

            MyBase.OnPaint(e)

            If _renderer IsNot Nothing Then
                RaiseEvent BeforeRendering(Me, New BeforeRenderingEventArgs(_renderer))

                _renderer.RenderControl(e)
            End If
        End Sub

        Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
            _lastMouseEventArgs = e

            Dim buttonWidth As Integer = _renderer.GetButtonWidth()
            Dim buttonRectangle As Rectangle = _renderer.GetButtonRectangle(buttonWidth)

            If _moving Then
                Dim val As Integer = _xValue + (e.Location.X - _xOffset)

                If val < 0 Then val = 0

                If val > Width - buttonWidth Then val = Width - buttonWidth

                ButtonValue = val
                Refresh()
                Return
            End If

            If buttonRectangle.Contains(e.Location) Then
                _isButtonHovered = True
                _isLeftFieldHovered = False
                _isRightFieldHovered = False
            Else
                If e.Location.X > buttonRectangle.X + buttonRectangle.Width Then
                    _isButtonHovered = False
                    _isLeftFieldHovered = False
                    _isRightFieldHovered = True
                ElseIf e.Location.X < buttonRectangle.X Then
                    _isButtonHovered = False
                    _isLeftFieldHovered = True
                    _isRightFieldHovered = False
                End If
            End If

            Refresh()
        End Sub

        Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
            If _animating OrElse Not AllowUserChange Then Return

            Dim buttonWidth As Integer = _renderer.GetButtonWidth()
            Dim buttonRectangle As Rectangle = _renderer.GetButtonRectangle(buttonWidth)

            _savedButtonValue = ButtonValue

            If buttonRectangle.Contains(e.Location) Then
                _isButtonPressed = True
                _isLeftFieldPressed = False
                _isRightFieldPressed = False

                _moving = True
                _xOffset = e.Location.X
                _buttonValue = buttonRectangle.X
                _xValue = ButtonValue
            Else
                If e.Location.X > buttonRectangle.X + buttonRectangle.Width Then
                    _isButtonPressed = False
                    _isLeftFieldPressed = False
                    _isRightFieldPressed = True
                ElseIf e.Location.X < buttonRectangle.X Then
                    _isButtonPressed = False
                    _isLeftFieldPressed = True
                    _isRightFieldPressed = False
                End If
            End If

            Refresh()
        End Sub

        Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
            If _animating OrElse Not AllowUserChange Then Return

            Dim buttonWidth As Integer = _renderer.GetButtonWidth()

            Dim wasLeftSidePressed = IsLeftSidePressed
            Dim wasRightSidePressed = IsRightSidePressed

            _isButtonPressed = False
            _isLeftFieldPressed = False
            _isRightFieldPressed = False

            If _moving Then
                Dim percentage As Integer = 100 * CDbl(ButtonValue) / (CDbl(Width) - buttonWidth)

                If _checked Then
                    If percentage <= 100 - _thresholdPercentage Then
                        _animationTarget = 0
                        BeginAnimation(False)
                    ElseIf ToggleOnButtonClick AndAlso _savedButtonValue = ButtonValue Then
                        _animationTarget = 0
                        BeginAnimation(False)
                    Else
                        _animationTarget = Width - buttonWidth
                        BeginAnimation(True)
                    End If
                Else
                    If percentage >= _thresholdPercentage Then
                        _animationTarget = Width - buttonWidth
                        BeginAnimation(True)
                    ElseIf ToggleOnButtonClick AndAlso _savedButtonValue = ButtonValue Then
                        _animationTarget = Width - buttonWidth
                        BeginAnimation(True)
                    Else
                        _animationTarget = 0
                        BeginAnimation(False)
                    End If
                End If

                _moving = False
                Return
            End If

            If IsButtonOnRightSide Then
                _buttonValue = Width - buttonWidth
                _animationTarget = 0
            Else
                _buttonValue = 0
                _animationTarget = Width - buttonWidth
            End If

            If wasLeftSidePressed AndAlso ToggleOnSideClick Then
                SetValueInternal(False)
            ElseIf wasRightSidePressed AndAlso ToggleOnSideClick Then
                SetValueInternal(True)
            End If

            Refresh()
        End Sub

        Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
            _isButtonHovered = False
            _isLeftFieldHovered = False
            _isRightFieldHovered = False
            _isButtonPressed = False
            _isLeftFieldPressed = False
            _isRightFieldPressed = False

            Refresh()
        End Sub

        Protected Overrides Sub OnEnabledChanged(ByVal e As EventArgs)
            MyBase.OnEnabledChanged(e)
            Refresh()
        End Sub

        Protected Overrides Sub OnRegionChanged(ByVal e As EventArgs)
            MyBase.OnRegionChanged(e)
            Refresh()
        End Sub

        Protected Overrides Sub OnSizeChanged(ByVal e As EventArgs)
            If _animationTarget > 0 Then
                Dim buttonWidth As Integer = _renderer.GetButtonWidth()
                _animationTarget = Width - buttonWidth
            End If

            MyBase.OnSizeChanged(e)
        End Sub

#End Region

#Region "Private Methods"

        Private Sub SetValueInternal(ByVal checkedValue As Boolean)
            If checkedValue = _checked Then Return

            While _animating
                Application.DoEvents()
            End While

            BeginAnimation(checkedValue)
        End Sub

        Private Sub BeginAnimation(ByVal checkedValue As Boolean)
            _animating = True
            _animationResult = checkedValue

            If _animationTimer IsNot Nothing AndAlso _useAnimation Then
                _animationTimer.Interval = _animationInterval
                _animationTimer.Enabled = True
            Else
                AnimationComplete()
            End If
        End Sub

        Private Sub AnimationTimer_Tick(ByVal sender As Object, ByVal e As EventArgs)
            _animationTimer.Enabled = False

            Dim animationDone As Boolean
            Dim newButtonValue As Integer

            If IsButtonMovingRight Then
                newButtonValue = ButtonValue + _animationStep

                If newButtonValue > _animationTarget Then newButtonValue = _animationTarget

                ButtonValue = newButtonValue

                animationDone = ButtonValue >= _animationTarget
            Else
                newButtonValue = ButtonValue - _animationStep

                If newButtonValue < _animationTarget Then newButtonValue = _animationTarget

                ButtonValue = newButtonValue

                animationDone = ButtonValue <= _animationTarget
            End If

            If animationDone Then
                AnimationComplete()
            Else
                _animationTimer.Enabled = True
            End If
        End Sub

        Private Sub AnimationComplete()
            _animating = False
            _moving = False
            _checked = _animationResult

            _isButtonHovered = False
            _isButtonPressed = False
            _isLeftFieldHovered = False
            _isLeftFieldPressed = False
            _isRightFieldHovered = False
            _isRightFieldPressed = False

            Refresh()

            RaiseEvent CheckedChanged(Me, New EventArgs())

            If _lastMouseEventArgs IsNot Nothing Then Me.OnMouseMove(_lastMouseEventArgs)

            _lastMouseEventArgs = Nothing
        End Sub

#End Region
    End Class
End Namespace
