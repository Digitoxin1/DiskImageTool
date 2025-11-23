Namespace Flux.PcImgCnv
    Public Class CommandLineBuilder
        Public Property F86SurfaceData As Boolean = False
        Public Property FindSplice As Boolean = True
        Public Property FluxRate As UShort? = Nothing
        Public Property FluxRPM As UShort? = Nothing
        Public Property InFile As String
        Public Property OutFile As String
        Public Property Remaster As Boolean = True
        Public Property Rev As UShort? = Nothing
        Public Property TrackLayout As String = ""

        Public Function Arguments() As String
            Dim args As New List(Of String) From {
                Quoted(_InFile),
                Quoted(_OutFile)
            }

            If _F86SurfaceData Then
                args.Add("--86fsurfacedata")
            End If

            If Not _Remaster Then
                args.Add("--noremaster")
            End If

            If Not _FindSplice Then
                args.Add("--findsplice")
            End If

            If _FluxRate.HasValue Then
                args.Add("--fluxrate " & _FluxRate.Value)
            End If

            If _FluxRPM.HasValue Then
                args.Add("--fluxrpm " & _FluxRPM.Value)
            End If

            If Not String.IsNullOrEmpty(_TrackLayout) Then
                args.Add("--tracklayout " & Quoted(_TrackLayout))
            End If

            If _Rev.HasValue Then
                args.Add("--rev " & _Rev.Value)
            End If

            Return String.Join(" ", args)
        End Function
    End Class
End Namespace
