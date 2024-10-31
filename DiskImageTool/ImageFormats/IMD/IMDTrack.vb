Namespace ImageFormats
    Namespace IMD
        Public Class IMDTrack
            Public Sub New()
                _Sectors = New List(Of IMDSector)
            End Sub

            Public Property Mode As TrackMode
            Public Property Cylinder As Byte
            Public Property Head As Byte
            Public Property SectorSize As SectorSize
            Public Property Sectors As List(Of IMDSector)

            Public Function GetSizeBytes() As UShort
                Select Case _SectorSize
                    Case SectorSize.SectorSize128
                        Return 128
                    Case SectorSize.SectorSize256
                        Return 256
                    Case SectorSize.Sectorsize512
                        Return 512
                    Case SectorSize.SectorSize1024
                        Return 1024
                    Case SectorSize.SectorSize2048
                        Return 2048
                    Case SectorSize.SectorSize4096
                        Return 4096
                    Case SectorSize.SectorSize8192
                        Return 8192
                    Case Else
                        Return 0
                End Select
            End Function

            Public Function IsMFM() As Boolean
                Return _Mode = TrackMode.MFM250kbps Or _Mode = TrackMode.MFM300kbps Or _Mode = TrackMode.MFM500kbps
            End Function
        End Class
    End Namespace
End Namespace
