Namespace Flux
    Namespace Kryoflux
        Public Class BaseForm
            Inherits Flux.BaseForm

            Private ReadOnly _Parser As ConsoleParser
            Private ReadOnly _TrackStatus As Dictionary(Of String, TrackStatusInfo)
            Private _CurrentStatusInfo As TrackStatusInfo = Nothing

            Public Sub New(Optional UseGrid As Boolean = True)
                MyBase.New(KryofluxSettings.LogFileName, UseGrid)

                _Parser = New ConsoleParser
                _TrackStatus = New Dictionary(Of String, TrackStatusInfo)
            End Sub

            Public Sub ClearTrackStatus()
                _TrackStatus.Clear()
                _CurrentStatusInfo = Nothing
            End Sub

            Public Class TrackStatusInfo
                Public Sub New()
                End Sub

                Public Property Action As ActionTypeEnum
                Public Property BadSectors As UShort = 0
                Public Property Failed As Boolean
                Public Property Retries As UShort = 0
                Public Property Side As Integer
                Public Property Track As Integer
            End Class
        End Class
    End Namespace
End Namespace
