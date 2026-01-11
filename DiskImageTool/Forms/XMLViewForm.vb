Imports System.Xml

Public Class XMLViewForm
    Private m_SaveFileName As String
    Private m_xmlContent As XmlDocument

    Public Sub New(Caption As String, Content As XmlDocument, EnableSave As Boolean, Optional SaveFileName As String = "")

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        Me.Text = Caption

        m_SaveFileName = SaveFileName
        m_xmlContent = Content

        PopulateTreeViewFromXml(TreeView1, Content)

        If Not EnableSave Then
            PanelBottom.Visible = False
        End If
    End Sub

    Public Shared Sub Display(Caption As String, Content As XmlDocument, EnableSave As Boolean, Optional SaveFileName As String = "")
        Using dlg As New XMLViewForm(Caption, Content, EnableSave, SaveFileName)
            dlg.ShowDialog(App.CurrentFormInstance)
        End Using
    End Sub

    Private Sub AddXmlNodeChildren(xmlNode As XmlNode, treeNode As TreeNode)

        ' Attributes
        If xmlNode.Attributes IsNot Nothing AndAlso xmlNode.Attributes.Count > 0 Then
            Dim attrGroup As New TreeNode("@attributes") With {
                .ForeColor = Color.DarkGray
            }

            treeNode.Nodes.Add(attrGroup)

            For Each attr As XmlAttribute In xmlNode.Attributes
                Dim attrNode As New TreeNode($"{attr.Name}=""{attr.Value}""") With {
                    .Tag = attr,
                    .ForeColor = Color.Brown
                }
                attrGroup.Nodes.Add(attrNode)
            Next
        End If

        ' Determine if this is a simple text-only element
        Dim elementChildren = xmlNode.ChildNodes.
            Cast(Of XmlNode)().
            Where(Function(n) n.NodeType = XmlNodeType.Element).
            ToList()

        Dim textChildren = xmlNode.ChildNodes.
            Cast(Of XmlNode)().
            Where(Function(n) n.NodeType = XmlNodeType.Text OrElse n.NodeType = XmlNodeType.CDATA).
            ToList()

        Dim isSimpleValueNode = elementChildren.Count = 0 AndAlso textChildren.Count = 1 AndAlso textChildren(0).Value.Trim().Length > 0

        ' Only add child nodes if it's NOT a simple value node
        If Not isSimpleValueNode Then
            For Each child As XmlNode In xmlNode.ChildNodes
                Select Case child.NodeType

                    Case XmlNodeType.Element
                        Dim childTree As New TreeNode(GetElementLabel(child)) With {
                            .Tag = child,
                            .ForeColor = Color.Blue
                        }
                        treeNode.Nodes.Add(childTree)
                        AddXmlNodeChildren(child, childTree)

                    Case XmlNodeType.Text, XmlNodeType.CDATA
                        Dim text = child.Value.Trim()
                        If text.Length > 0 Then
                            Dim textNode As New TreeNode(text) With {
                                .Tag = child,
                                .ForeColor = Color.Black
                            }
                            treeNode.Nodes.Add(textNode)
                        End If

                    Case XmlNodeType.Comment
                        Dim commentNode As New TreeNode($"<!-- {child.Value} -->") With {
                            .Tag = child,
                            .ForeColor = Color.Green
                        }
                        treeNode.Nodes.Add(commentNode)
                End Select
            Next
        End If
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        Dim Extension = IO.Path.GetExtension(m_SaveFileName).ToLower
        Dim FilterIndex As Integer = 1
        If Extension <> ".txt" Then
            FilterIndex = 2
        End If

        Using Dialog As New SaveFileDialog With {
               .FileName = m_SaveFileName,
               .DefaultExt = "txt",
               .Filter = My.Resources.FileType_Text & " (*.txt)|*.txt|" & My.Resources.FileType_All & " (*.*)|*.*",
               .FilterIndex = FilterIndex
            }

            If Not String.IsNullOrEmpty(m_SaveFileName) Then
                Dim Path = IO.Path.GetDirectoryName(m_SaveFileName)

                Dialog.FileName = IO.Path.GetFileName(m_SaveFileName)

                If Not String.IsNullOrEmpty(Path) AndAlso IO.Directory.Exists(Path) Then
                    Dialog.InitialDirectory = Path
                    Dialog.RestoreDirectory = True
                End If
            End If

            If Dialog.ShowDialog = DialogResult.OK Then
                m_xmlContent.Save(Dialog.FileName)
            End If
        End Using
    End Sub

    Private Sub DrawAttributeNodeText(g As Graphics,
                                      font As Font,
                                      attr As XmlAttribute,
                                      displayedText As String,
                                      defaultFore As Color,
                                      x As Integer,
                                      y As Integer,
                                      selected As Boolean)

        Dim nameColor = If(selected, defaultFore, Color.Brown)
        Dim valueColor = If(selected, defaultFore, Color.Purple)
        Dim punctColor = If(selected, defaultFore, Color.Gray)

        x = DrawToken(g, attr.Name, font, nameColor, x, y)
        x = DrawToken(g, "=", font, punctColor, x, y)
        DrawToken(g, $"""{attr.Value}""", font, valueColor, x, y)
    End Sub

    Private Sub DrawElementNodeText(g As Graphics,
                                    font As Font,
                                    element As XmlNode,
                                    displayedText As String,
                                    defaultFore As Color,
                                    x As Integer,
                                    y As Integer,
                                    selected As Boolean)

        Dim name = element.Name

        x = DrawToken(g, "<", font, If(selected, defaultFore, Color.Gray), x, y)
        x = DrawToken(g, name, font, If(selected, defaultFore, Color.Blue), x, y)
        x = DrawToken(g, ">", font, If(selected, defaultFore, Color.Gray), x, y)


        Dim prefix = $"<{name}>"

        If displayedText.StartsWith(prefix, StringComparison.Ordinal) AndAlso displayedText.Length > prefix.Length Then
            Dim rest = displayedText.Substring(prefix.Length) ' includes leading space
            DrawToken(g, rest, font, defaultFore, x, y)
        End If
    End Sub

    Private Function DrawToken(g As Graphics, text As String, font As Font, color As Color, x As Integer, y As Integer) As Integer
        TextRenderer.DrawText(g, text, font, New Point(x, y), color, TextFormatFlags.NoPadding)

        Dim sz = TextRenderer.MeasureText(g, text, font, New Size(Integer.MaxValue, Integer.MaxValue), TextFormatFlags.NoPadding)

        Return x + sz.Width
    End Function

    Private Function GetElementLabel(node As XmlNode) As String
        If node.NodeType <> XmlNodeType.Element Then
            Return node.Name
        End If

        ' Show <tag>value</tag> inline if it's a simple leaf
        Dim hasChildElements = node.ChildNodes.Cast(Of XmlNode)().Any(Function(n) n.NodeType = XmlNodeType.Element)

        If Not hasChildElements Then
            Dim text = node.InnerText.Trim()
            If text.Length > 0 AndAlso text.Length <= 60 Then
                Return $"<{node.Name}> {text}"
            End If
        End If

        Return $"<{node.Name}>"
    End Function

    Private Sub LocalizeForm()
        BtnClose.Text = WithoutHotkey(My.Resources.Menu_Close)
        BtnSave.Text = WithoutHotkey(My.Resources.Menu_Save)
    End Sub

    Private Sub PopulateTreeViewFromXml(tv As TreeView, doc As XmlDocument)
        tv.BeginUpdate()
        tv.Nodes.Clear()

        If doc.DocumentElement Is Nothing Then
            tv.EndUpdate()
            Return
        End If

        Dim rootNode As New TreeNode(GetElementLabel(doc.DocumentElement)) With {
            .Tag = doc.DocumentElement,
            .ForeColor = Color.Blue
        }

        tv.Nodes.Add(rootNode)
        AddXmlNodeChildren(doc.DocumentElement, rootNode)

        tv.ExpandAll()

        tv.EndUpdate()
    End Sub

    Private Sub TreeView1_DrawNode(sender As Object, e As DrawTreeNodeEventArgs) Handles TreeView1.DrawNode
        If e.Node Is Nothing Then
            Return
        End If

        e.DrawDefault = False

        Dim tv = DirectCast(sender, TreeView)
        Dim g = e.Graphics
        Dim bounds = e.Bounds
        Dim font = tv.Font

        Dim selected = (e.State And TreeNodeStates.Selected) <> 0

        ' Background
        If selected Then
            g.FillRectangle(SystemBrushes.Highlight, bounds)
        Else
            g.FillRectangle(SystemBrushes.Window, bounds)
        End If

        Dim defaultFore = If(selected, SystemColors.HighlightText, tv.ForeColor)
        Dim x = bounds.Left
        Dim y = bounds.Top

        Dim tagObj = e.Node.Tag

        Dim xn = TryCast(tagObj, XmlNode)
        If xn IsNot Nothing AndAlso xn.NodeType = XmlNodeType.Element Then
            DrawElementNodeText(g, font, xn, e.Node.Text, defaultFore, x, y, selected)
            Return
        End If

        Dim xa = TryCast(tagObj, XmlAttribute)
        If xa IsNot Nothing Then
            DrawAttributeNodeText(g, font, xa, e.Node.Text, defaultFore, x, y, selected)
            Return
        End If

        If String.Equals(e.Node.Text, "@attributes", StringComparison.OrdinalIgnoreCase) Then
            DrawToken(g, "@attributes", font, If(selected, defaultFore, Color.DarkGray), x, y)
            Return
        End If

        TextRenderer.DrawText(g, e.Node.Text, font, New Point(x, y), defaultFore, TextFormatFlags.NoPadding)
    End Sub

    Private Sub XMLViewForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub
End Class