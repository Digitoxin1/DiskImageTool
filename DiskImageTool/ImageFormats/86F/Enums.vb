Namespace ImageFormats
    Namespace D86F
        Public Enum BitRate As Byte
            BitRate500 = 0
            BitRate300 = 1
            BitRate250 = 2
            BitRate1000 = 3
            BitRate2000 = 5
        End Enum

        Public Enum DiskFlags As UShort
            HasSurfaceData = 1
            Sides = 8
            WriteProtect = 16
            BitcellMode = 128
            DiskType = 256
            ReverseEndian = 2048
            AlternateBitcellCalculation = 4096
        End Enum

        Public Enum DiskHole As Byte
            DD = 0
            HD = 1
            ED = 2
            ED2000 = 3
        End Enum

        Public Enum DiskType As Byte
            FixedRPM = 0
            Zoned = 1
        End Enum

        Public Enum Encoding As Byte
            FM = 0
            MFM = 1
            M2MF = 2
            GCR = 3
        End Enum

        Public Enum RPM As Byte
            RPM300 = 0
            RPM360 = 1
        End Enum

        Public Enum ZoneType As Byte
            PreApple1 = 0
            PreApple2 = 1
            Apple = 2
            Commodore64 = 3
        End Enum

    End Namespace
End Namespace
