Namespace ImageFormats
    Namespace TC
        Public Enum TransCopyDiskType As Byte
            Unknown = &HFF
            MFMHighDensity = &H2
            MFMDoubleDensity360RPM = &H3
            AppleIIGCR = &H4
            FMSingleDensity = &H5
            CommodoreGCR = &H6
            MFMDoubleDensity = &H7
            CommodoreAmiga = &H8
            AtariFM = &HC
        End Enum

        Public Enum TransCopyTrackFlags As Byte
            KeepTrackLength = 1
            CopyAcrossIndex = 2
            CopyWeakBits = 4
            VerifyWrite = 8
            Bit4 = 16
            LengthTolerance = 32
            Bit6 = 64
            NoAddressMarks = 128
        End Enum
    End Namespace
End Namespace

