Namespace Greaseweazle
    Module GreaseweazleLib
        Public Enum GreaseweazleFloppyType
            None
            F525_DD_360K
            F525_HD_12M
            F35_DD_720K
            F35_HD_144M
            F35_ED_288M
        End Enum

        Public Enum GreaseweazleInterface
            IBM
            Shugart
        End Enum
        Public Function GetGreaseweazleFloppyTypeDescription(Value As GreaseweazleFloppyType) As String
            Select Case Value
                Case GreaseweazleFloppyType.F525_DD_360K
                    Return "5.25"" 360 KB (Double Density)"
                Case GreaseweazleFloppyType.F35_DD_720K
                    Return "3.5"" 720 KB (Double Density)"
                Case GreaseweazleFloppyType.F525_HD_12M
                    Return "5.25"" 1.2 MB (High Density)"
                Case GreaseweazleFloppyType.F35_HD_144M
                    Return "3.5"" 1.44 MB (High Density)"
                Case GreaseweazleFloppyType.F35_ED_288M
                    Return "3.5"" 2.88 MB (Extra Density)"
                Case Else
                    Return "None"
            End Select
        End Function

        Public Function GetGreaseweazleFloppyTypeName(Value As GreaseweazleFloppyType) As String
            Select Case Value
                Case GreaseweazleFloppyType.F525_DD_360K
                    Return "360"
                Case GreaseweazleFloppyType.F35_DD_720K
                    Return "720"
                Case GreaseweazleFloppyType.F525_HD_12M
                    Return "1200"
                Case GreaseweazleFloppyType.F35_HD_144M
                    Return "1440"
                Case GreaseweazleFloppyType.F35_ED_288M
                    Return "2880"
                Case Else
                    Return ""
            End Select
        End Function

        Public Function GetGreaseweazleFoppyTypeFromName(Value As String) As GreaseweazleFloppyType
            Select Case Value
                Case "360"
                    Return GreaseweazleFloppyType.F525_DD_360K
                Case "720"
                    Return GreaseweazleFloppyType.F35_DD_720K
                Case "1200"
                    Return GreaseweazleFloppyType.F525_HD_12M
                Case "1440"
                    Return GreaseweazleFloppyType.F35_HD_144M
                Case "2880"
                    Return GreaseweazleFloppyType.F35_ED_288M
                Case Else
                    Return GreaseweazleFloppyType.None
            End Select
        End Function

        Public Function GetGreaseweazleInterfaceName(Value As GreaseweazleInterface) As String
            Select Case Value
                Case GreaseweazleInterface.Shugart
                    Return "Shugart"
                Case Else
                    Return "IBM"
            End Select
        End Function

        Public Function GetGreaseweazleInterfacFromName(Value As String) As GreaseweazleInterface
            Select Case Value
                Case "Shugart"
                    Return GreaseweazleInterface.Shugart
                Case Else
                    Return GreaseweazleInterface.IBM
            End Select
        End Function
    End Module
End Namespace