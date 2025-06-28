Public Class TwoColumnToolTip
	Inherits ToolTip

	Private Const PADDING As Integer = 4
	Private Const GAP As Integer = 8
	Private _CachedMaxLeftWidth As Integer = 0

	Public Sub New()
		MyBase.New()
		Me.OwnerDraw = True
		AddHandler Me.Popup, AddressOf OnPopup
		AddHandler Me.Draw, AddressOf OnDraw
	End Sub

	Private Sub OnPopup(sender As Object, e As PopupEventArgs)
		Dim tooltipText As String = Me.GetToolTip(e.AssociatedControl)
		Dim font As Font = SystemFonts.StatusFont

		Dim lines = tooltipText.Split(New String() {Environment.NewLine}, StringSplitOptions.None)

		Dim maxLeftWidth = 0
		Dim maxRightWidth = 0
		Dim totalHeight = 0

		Using g = e.AssociatedControl.CreateGraphics()
			For Each line In lines
				Dim parts = line.Split({vbTab}, 2, StringSplitOptions.None)
				Dim left = If(parts.Length > 0, parts(0), "")
				Dim right = If(parts.Length > 1, parts(1), "")
				Dim szLeft = TextRenderer.MeasureText(g, left, font)
				Dim szRight = TextRenderer.MeasureText(g, right, font)

				maxLeftWidth = Math.Max(maxLeftWidth, szLeft.Width)
				maxRightWidth = Math.Max(maxRightWidth, szRight.Width)
				totalHeight += Math.Max(szLeft.Height, szRight.Height)
			Next
		End Using

		e.ToolTipSize = New Size(maxLeftWidth + maxRightWidth + PADDING * 2 + GAP, totalHeight + PADDING * 2)

		_CachedMaxLeftWidth = maxLeftWidth
	End Sub

	Private Sub OnDraw(sender As Object, e As DrawToolTipEventArgs)
		Dim font = e.Font
		Dim g = e.Graphics
		Dim tooltipText = e.ToolTipText

		e.Graphics.Clear(SystemColors.Window)

		Using borderPen As New Pen(Color.Gray)
			g.DrawRectangle(borderPen, New Rectangle(0, 0, e.Bounds.Width - 1, e.Bounds.Height - 1))
		End Using

		Dim y = PADDING

		Dim lines = tooltipText.Split(New String() {Environment.NewLine}, StringSplitOptions.None)

		For Each line In lines
			Dim parts = line.Split({vbTab}, 2, StringSplitOptions.None)
			Dim left = If(parts.Length > 0, parts(0), "")
			Dim right = If(parts.Length > 1, parts(1), "")

			Dim sz = TextRenderer.MeasureText(g, left, font)
			TextRenderer.DrawText(g, left, font, New Point(PADDING, y), SystemColors.InfoText)
			TextRenderer.DrawText(g, right, font, New Point(PADDING + _CachedMaxLeftWidth + GAP, y), SystemColors.InfoText)
			y += sz.Height
		Next
	End Sub
End Class
