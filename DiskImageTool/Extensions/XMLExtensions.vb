Imports System.Runtime.CompilerServices
Imports System.Xml

Module XMLExtensions
    <Extension()>
    Public Function AppendAttribute(Node As XmlNode, Name As String, Value As String) As XmlAttribute
        Dim Attribute = Node.OwnerDocument.CreateAttribute(Name)
        Attribute.Value = Value
        Node.Attributes.Append(Attribute)

        Return Attribute
    End Function
End Module
