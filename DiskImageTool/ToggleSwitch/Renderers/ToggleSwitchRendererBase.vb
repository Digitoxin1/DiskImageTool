Imports System.Drawing
Imports System.Windows.Forms

Namespace JCS
    Public MustInherit Class ToggleSwitchRendererBase
#Region "Private Members"

        Private _toggleSwitch As ToggleSwitch

#End Region

#Region "Constructor"

        Protected Sub New()
        End Sub

        Friend Sub SetToggleSwitch(ByVal toggleSwitch As ToggleSwitch)
            _toggleSwitch = toggleSwitch
        End Sub

        Friend ReadOnly Property ToggleSwitch As ToggleSwitch
            Get
                Return _toggleSwitch
            End Get
        End Property

#End Region

#Region "Render Methods"

        Public Sub RenderBackground(ByVal e As PaintEventArgs)
            If _toggleSwitch Is Nothing Then Return

            e.Graphics.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias

            Dim controlRectangle = New Rectangle(0, 0, _toggleSwitch.Width, _toggleSwitch.Height)

            FillBackground(e.Graphics, controlRectangle)

            RenderBorder(e.Graphics, controlRectangle)
        End Sub

        Public Sub RenderControl(ByVal e As PaintEventArgs)
            If _toggleSwitch Is Nothing Then Return

            e.Graphics.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias

            Dim buttonRectangle As Rectangle = GetButtonRectangle()
            Dim totalToggleFieldWidth As Integer = ToggleSwitch.Width - buttonRectangle.Width

            If buttonRectangle.X > 0 Then
                Dim leftRectangle = New Rectangle(0, 0, buttonRectangle.X, ToggleSwitch.Height)

                If leftRectangle.Width > 0 Then RenderLeftToggleField(e.Graphics, leftRectangle, totalToggleFieldWidth)
            End If

            If buttonRectangle.X + buttonRectangle.Width < e.ClipRectangle.Width Then
                Dim rightRectangle = New Rectangle(buttonRectangle.X + buttonRectangle.Width, 0, ToggleSwitch.Width - buttonRectangle.X - buttonRectangle.Width, ToggleSwitch.Height)

                If rightRectangle.Width > 0 Then RenderRightToggleField(e.Graphics, rightRectangle, totalToggleFieldWidth)
            End If

            RenderButton(e.Graphics, buttonRectangle)
        End Sub

        Public Sub FillBackground(ByVal g As Graphics, ByVal controlRectangle As Rectangle)
            Dim backColor As Color = If(Not ToggleSwitch.Enabled AndAlso ToggleSwitch.GrayWhenDisabled, ToggleSwitch.BackColor.ToGrayScale(), ToggleSwitch.BackColor)

            Using backBrush As Brush = New SolidBrush(backColor)
                g.FillRectangle(backBrush, controlRectangle)
            End Using
        End Sub

        Public MustOverride Sub RenderBorder(ByVal g As Graphics, ByVal borderRectangle As Rectangle)
        Public MustOverride Sub RenderLeftToggleField(ByVal g As Graphics, ByVal leftRectangle As Rectangle, ByVal totalToggleFieldWidth As Integer)
        Public MustOverride Sub RenderRightToggleField(ByVal g As Graphics, ByVal rightRectangle As Rectangle, ByVal totalToggleFieldWidth As Integer)
        Public MustOverride Sub RenderButton(ByVal g As Graphics, ByVal buttonRectangle As Rectangle)

#End Region

#Region "Helper Methods"

        Public MustOverride Function GetButtonWidth() As Integer
        Public MustOverride Function GetButtonRectangle() As Rectangle
        Public MustOverride Function GetButtonRectangle(ByVal buttonWidth As Integer) As Rectangle

#End Region
    End Class
End Namespace
