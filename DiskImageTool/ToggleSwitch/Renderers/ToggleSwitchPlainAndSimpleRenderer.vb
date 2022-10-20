Imports System.Drawing
Imports System.Drawing.Drawing2D

Namespace JCS
    Public Class ToggleSwitchPlainAndSimpleRenderer
        Inherits ToggleSwitchRendererBase
#Region "Constructor"

        Public Sub New()
            BorderColorChecked = Color.Black
            BorderColorUnchecked = Color.Black
            BorderWidth = 2
            ButtonMargin = 1
            InnerBackgroundColor = Color.White
            ButtonColor = Color.Black
        End Sub

#End Region

#Region "Public Properties"

        Public Property BorderColorChecked As Color
        Public Property BorderColorUnchecked As Color
        Public Property BorderWidth As Integer
        Public Property ButtonMargin As Integer
        Public Property InnerBackgroundColor As Color
        Public Property ButtonColor As Color

#End Region

#Region "Render Method Implementations"

        Public Overrides Sub RenderBorder(ByVal g As Graphics, ByVal borderRectangle As Rectangle)
            'Draw this one AFTER the button is drawn in the RenderButton method
        End Sub

        Public Overrides Sub RenderLeftToggleField(ByVal g As Graphics, ByVal leftRectangle As Rectangle, ByVal totalToggleFieldWidth As Integer)
            g.SmoothingMode = SmoothingMode.HighQuality
            g.PixelOffsetMode = PixelOffsetMode.HighQuality

            Dim buttonWidth As Integer = GetButtonWidth()

            Dim controlRectangle = New Rectangle(0, 0, ToggleSwitch.Width, ToggleSwitch.Height)
            Dim innerBorderClipPath As GraphicsPath = GetInnerBorderClipPath(controlRectangle)

            g.SetClip(innerBorderClipPath)
            g.IntersectClip(leftRectangle)

            Using innerBackgroundBrush As Brush = New SolidBrush(InnerBackgroundColor)
                g.FillPath(innerBackgroundBrush, innerBorderClipPath)
            End Using

            g.ResetClip()
        End Sub

        Public Overrides Sub RenderRightToggleField(ByVal g As Graphics, ByVal rightRectangle As Rectangle, ByVal totalToggleFieldWidth As Integer)
            g.SmoothingMode = SmoothingMode.HighQuality
            g.PixelOffsetMode = PixelOffsetMode.HighQuality

            Dim buttonWidth As Integer = GetButtonWidth()

            Dim controlRectangle = New Rectangle(0, 0, ToggleSwitch.Width, ToggleSwitch.Height)
            Dim innerBorderClipPath As GraphicsPath = GetInnerBorderClipPath(controlRectangle)

            g.SetClip(innerBorderClipPath)
            g.IntersectClip(rightRectangle)

            Using innerBackgroundBrush As Brush = New SolidBrush(InnerBackgroundColor)
                g.FillPath(innerBackgroundBrush, innerBorderClipPath)
            End Using

            g.ResetClip()
        End Sub

        Public Overrides Sub RenderButton(ByVal g As Graphics, ByVal buttonRectangle As Rectangle)
            g.SmoothingMode = SmoothingMode.HighQuality
            g.PixelOffsetMode = PixelOffsetMode.HighQuality

            Dim buttonClipPath As GraphicsPath = GetButtonClipPath()

            Dim controlRectangle = New Rectangle(0, 0, ToggleSwitch.Width, ToggleSwitch.Height)
            Dim outerBorderClipPath As GraphicsPath = GetControlClipPath(controlRectangle)
            Dim innerBorderClipPath As GraphicsPath = GetInnerBorderClipPath(controlRectangle)

            g.SetClip(innerBorderClipPath)
            g.IntersectClip(buttonRectangle)

            'Fill the button surface with the background color

            Using innerBackgroundBrush As Brush = New SolidBrush(InnerBackgroundColor)
                g.FillRectangle(innerBackgroundBrush, buttonRectangle)
            End Using

            g.ResetClip()

            g.SetClip(GetButtonClipPath())
            g.IntersectClip(controlRectangle)

            'Render the button

            Using buttonBrush As Brush = New SolidBrush(ButtonColor)
                g.FillPath(buttonBrush, buttonClipPath)
            End Using

            g.ResetClip()

            'Render the border

            g.SetClip(outerBorderClipPath)
            g.ExcludeClip(New Region(innerBorderClipPath))

            Dim borderColor = If(ToggleSwitch.Checked, BorderColorChecked, BorderColorUnchecked)

            Using borderBrush As Brush = New SolidBrush(borderColor)
                g.FillPath(borderBrush, outerBorderClipPath)
            End Using

            g.ResetClip()
        End Sub

#End Region

#Region "Helper Method Implementations"

        Public Function GetControlClipPath(ByVal controlRectangle As Rectangle) As GraphicsPath
            Dim borderPath = New GraphicsPath()
            borderPath.AddArc(controlRectangle.X, controlRectangle.Y, controlRectangle.Height, controlRectangle.Height, 90, 180)
            borderPath.AddArc(controlRectangle.X + controlRectangle.Width - controlRectangle.Height, controlRectangle.Y, controlRectangle.Height, controlRectangle.Height, 270, 180)
            borderPath.CloseFigure()

            Return borderPath
        End Function

        Public Function GetInnerBorderClipPath(ByVal controlRectangle As Rectangle) As GraphicsPath
            Dim innerBorderRectangle = New Rectangle(controlRectangle.X + BorderWidth, controlRectangle.Y + BorderWidth, controlRectangle.Width - 2 * BorderWidth, controlRectangle.Height - 2 * BorderWidth)

            Dim borderPath = New GraphicsPath()
            borderPath.AddArc(innerBorderRectangle.X, innerBorderRectangle.Y, innerBorderRectangle.Height, innerBorderRectangle.Height, 90, 180)
            borderPath.AddArc(innerBorderRectangle.X + innerBorderRectangle.Width - innerBorderRectangle.Height, innerBorderRectangle.Y, innerBorderRectangle.Height, innerBorderRectangle.Height, 270, 180)
            borderPath.CloseFigure()

            Return borderPath
        End Function

        Public Function GetButtonClipPath() As GraphicsPath
            Dim buttonRectangle As Rectangle = GetButtonRectangle()

            Dim buttonPath = New GraphicsPath()

            buttonPath.AddArc(buttonRectangle.X + ButtonMargin, buttonRectangle.Y + ButtonMargin, buttonRectangle.Height - 2 * ButtonMargin, buttonRectangle.Height - 2 * ButtonMargin, 0, 360)

            Return buttonPath
        End Function

        Public Overrides Function GetButtonWidth() As Integer
            Dim buttonWidth As Integer = ToggleSwitch.Height - 2 * BorderWidth
            Return If(buttonWidth > 0, buttonWidth, 0)
        End Function

        Public Overloads Overrides Function GetButtonRectangle() As Rectangle
            Dim buttonWidth As Integer = GetButtonWidth()
            Return GetButtonRectangle(buttonWidth)
        End Function

        Public Overloads Overrides Function GetButtonRectangle(ByVal buttonWidth As Integer) As Rectangle
            If buttonWidth <= 0 Then
                Return Rectangle.Empty
            End If

            Dim buttonRect = New Rectangle(ToggleSwitch.ButtonValue, BorderWidth, buttonWidth, buttonWidth)

            If buttonRect.X < BorderWidth + ButtonMargin - 1 Then buttonRect.X = BorderWidth + ButtonMargin - 1

            If buttonRect.X + buttonRect.Width > ToggleSwitch.Width - BorderWidth - ButtonMargin + 1 Then buttonRect.X = ToggleSwitch.Width - buttonRect.Width - BorderWidth - ButtonMargin + 1

            Return buttonRect
        End Function

#End Region
    End Class
End Namespace
