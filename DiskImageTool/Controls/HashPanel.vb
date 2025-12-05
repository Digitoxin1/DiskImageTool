Imports System.ComponentModel

Public Class HashPanel
    Private WithEvents HashPanelContextMenu As ContextMenuStrip
    Private Const CONTEXT_MENU_HASH_KEY As String = "Hashes"
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        InitContextMenu()
        Clear()
    End Sub

    Public Sub Populate(Disk As DiskImage.Disk, MD5 As String)
        If Disk IsNot Nothing Then
            LabelCRC32.Text = Disk.Image.GetCRC32
            LabelMD5.Text = MD5
            LabelSHA1.Text = Disk.Image.GetSHA1Hash
            SetVisible(True)
        Else
            Clear()
        End If
        FlowLayoutPanelHashes.Refresh()
    End Sub

    Private Sub Clear()
        LabelCRC32.Text = ""
        LabelMD5.Text = ""
        LabelSHA1.Text = ""
        SetVisible(False)
    End Sub

    Private Sub InitContextMenu()
        HashPanelContextMenu = New ContextMenuStrip With {
            .Name = CONTEXT_MENU_HASH_KEY
        }
        HashPanelContextMenu.Items.Add(My.Resources.Menu_CopyValue)
        For Each Control As Control In FlowLayoutPanelHashes.Controls
            Control.ContextMenuStrip = HashPanelContextMenu
        Next
    End Sub

    Private Sub SetVisible(Visible As Boolean)
        For Each Control As Control In FlowLayoutPanelHashes.Controls
            Control.Visible = Visible
        Next
    End Sub
#Region "Events"
    Private Sub HashPanelContextMenu_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles HashPanelContextMenu.ItemClicked
        Dim Label As Label = CType(sender.SourceControl, Label)
        Dim Value As String

        If Label Is LabelCRC32 Or Label Is LabelCRC32Caption Then
            Value = LabelCRC32.Text
        ElseIf Label Is LabelMD5 Or Label Is LabelMD5Caption Then
            Value = LabelMD5.Text
        ElseIf Label Is LabelSHA1 Or Label Is LabelSHA1Caption Then
            Value = LabelSHA1.Text
        Else
            Exit Sub
        End If

        Clipboard.SetText(Value)
    End Sub

    Private Sub HashPanelContextMenu_Opening(sender As Object, e As CancelEventArgs) Handles HashPanelContextMenu.Opening
        Dim CM As ContextMenuStrip = sender
        Dim Label As Label = CType(sender.SourceControl, Label)
        Dim Text As String

        If Label Is LabelCRC32 Or Label Is LabelCRC32Caption Then
            Text = LabelCRC32Caption.Text
        ElseIf Label Is LabelMD5 Or Label Is LabelMD5Caption Then
            Text = LabelMD5Caption.Text
        ElseIf Label Is LabelSHA1 Or Label Is LabelSHA1Caption Then
            Text = LabelSHA1Caption.Text
        Else
            e.Cancel = True
            Exit Sub
        End If

        CM.Items(0).Text = String.Format(My.Resources.Menu_CopyValueByName, Text)
    End Sub
#End Region
End Class
