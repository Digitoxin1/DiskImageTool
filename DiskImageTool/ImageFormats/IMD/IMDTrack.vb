Namespace ImageFormats
    Namespace IMD
        Public Class IMDTrack
            Public Sub New()
                _Sectors = New List(Of IMDSector)
                _FirstSector = -1
                _LastSector = -1
            End Sub

            Public Property FirstSector As Integer
            Public Property LastSector As Integer
            Public Property Mode As TrackMode
            Public Property Sectors As List(Of IMDSector)
            Public Property SectorSize As SectorSize
            Public Property Side As Byte
            Public Property Track As Byte

            Public Sub AddSector(Sector As IMDSector)
                _Sectors.Add(Sector)

                If _FirstSector = -1 Or Sector.SectorId < _FirstSector Then
                    _FirstSector = Sector.SectorId
                End If

                If Sector.SectorId > _LastSector Then
                    _LastSector = Sector.SectorId
                End If
            End Sub

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
