Namespace ImageFormats
    Namespace TC
        Module Functions
            Public Function DiskTypeToString(DiskType As TransCopyDiskType) As String
                Select Case DiskType
                    Case TransCopyDiskType.Unknown
                        Return "Unknown"
                    Case TransCopyDiskType.MFMHighDensity
                        Return "MFM High Density"
                    Case TransCopyDiskType.MFMDoubleDensity360RPM
                        Return "MFM Double Density 360 RPM"
                    Case TransCopyDiskType.AppleIIGCR
                        Return "Apple II GCR"
                    Case TransCopyDiskType.FMSingleDensity
                        Return "FM Single Density"
                    Case TransCopyDiskType.CommodoreGCR
                        Return "Commodore GCR"
                    Case TransCopyDiskType.MFMDoubleDensity
                        Return "MFM Double Density"
                    Case TransCopyDiskType.CommodoreAmiga
                        Return "Commodore Amiga"
                    Case TransCopyDiskType.AtariFM
                        Return "Atari FM"
                    Case Else
                        Return "Unknown"
                End Select
            End Function
        End Module
    End Namespace
End Namespace

