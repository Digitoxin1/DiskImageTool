Namespace Flux.Kryoflux
    Class CommandLineBuilder
        Private Const DEFAULT_DEVICE_MODE As DeviceModeEnum = DeviceModeEnum.KryoFluxDevice
        Private Const DEFAULT_LOG_LEVEL As LogMask = 62
        Private Const DEFAULT_MISSING_SECTORS_AS_BAD As Boolean = True
        Private Const DEFAULT_RPM As UShort = 300
        Private Const DEFAULT_SECTOR_COUNT As UShort = 0
        Private Const DEFAULT_SECTOR_SIZE As SectorSizeEnum = SectorSizeEnum.Size512
        Private Const DEFAULT_SINGLE_SIDED_MODE As SingleSidedModeEnum = SingleSidedModeEnum.off
        Private Const DEFAULT_TRACK_DISTANCE As TrackDistanceEnum = TrackDistanceEnum.Tracks80
        Public Enum DeviceModeEnum As Byte
            ImageFile = 1
            KryoFluxDevice = 2
        End Enum

        Public Enum ImageTypeEnum
            KryoFlux_Stream = 0
            CT_RawImage = 2
            FM_SectorImage = 3
            MFM_SectorImage = 4
            Amigc_DOS_SectorImage = 5
            CBM_DOS_SectorImage = 6
            Apple_DOS32_SectorImage = 7
            Apple_DOS33_SectorImage = 8
            Apple_DOS400K_SectorImage = 9
            Emu_SectorImage = 10
            EmuII_SectorImage = 11
            Amiga_DiskSpare_SectorImage = 12
            DEC_RX01_SectorImage = 13
            DEC_RX02_SectorImage = 14
            CBM_MicroProse_SectorImage = 15
            CBM_RapidLok_SectorImage = 16
            CBM_Datasoft_SectorImage = 17
            CBM_Vorpal_SectorImage = 18
            CBM_V_MAX_SectorImage = 19
            CBM_Teque_SectorImage = 20
            CBM_TDP_SectorImage = 21
            CBM_GCR_Image = 22
            CBM_BigFive_SectorImage = 23
            CBM_DOSExtended_SectorImage = 24
            CBM_OziSoft_SectorImage = 25

            KryoFlux_Stream_Guided = 100
            FM_XFD = 103
            MF_XFD = 104
            CBM_DOS_SectorImage_ErrorMap = 106
            DSK_DOS33_SectorImage = 108
            CBM_GCR_Image_MasteringInfo = 122
        End Enum

        Public Enum LogMask As Byte
            Device = 1
            Read = 2
            Cell = 4
            Format = 8
            Write = 16
            Verify = 32
            TI = 64
        End Enum

        Public Enum SectorSizeEnum As Byte
            Size128 = 0
            Size256 = 1
            Size512 = 2
            Size1024 = 3
        End Enum
        Public Enum SingleSidedModeEnum
            off = -1
            side0 = 0
            side1 = 1
            both = 2
        End Enum

        Public Enum TrackDistanceEnum As Byte
            Tracks80 = 1
            Tracks40 = 2
        End Enum

        Public Property DeviceMode As DeviceModeEnum = DEFAULT_DEVICE_MODE
        Public Property InFile As String
        Public Property InImageType As ImageTypeEnum = ImageTypeEnum.KryoFlux_Stream
        Public Property LogLevel As LogMask = DEFAULT_LOG_LEVEL
        Public Property MissingSectorsAsBad As Boolean = DEFAULT_MISSING_SECTORS_AS_BAD
        Public Property OutFile As String
        Public Property OutImageType As ImageTypeEnum = ImageTypeEnum.MFM_SectorImage
        Public Property OutputTrackEnd As Short = -1
        Public Property OutputTrackStart As Short = -1
        Public Property RPM As UShort = DEFAULT_RPM
        Public Property SectorCount As Short = DEFAULT_SECTOR_COUNT
        Public Property SectorSize As SectorSizeEnum = DEFAULT_SECTOR_SIZE
        Public Property SingleSidedMode As SingleSidedModeEnum = DEFAULT_SINGLE_SIDED_MODE
        Public Property TrackDistance As TrackDistanceEnum = DEFAULT_TRACK_DISTANCE
        Public Property TrackEnd As Short = -1
        Public Property TrackStart As Short = -1
        Public Function Arguments() As String
            Dim args As New List(Of String)

            If DeviceMode <> DEFAULT_DEVICE_MODE Then
                args.Add("-m" & CByte(DeviceMode).ToString)
            End If

            args.Add("-f" & Quoted(_InFile))

            args.Add("-i" & GetImageTypeString(InImageType))

            If MissingSectorsAsBad <> DEFAULT_MISSING_SECTORS_AS_BAD Then
                args.Add("-tm" & If(MissingSectorsAsBad, "1", "0"))
            End If

            If TrackStart > -1 Then
                args.Add("-s" & TrackStart)
            End If

            If TrackEnd > -1 Then
                args.Add("-e" & TrackEnd)
            End If

            If SingleSidedMode <> DEFAULT_SINGLE_SIDED_MODE Then
                args.Add("-g" & CInt(SingleSidedMode).ToString)
            End If

            If SectorSize <> DEFAULT_SECTOR_SIZE Then
                args.Add("-z" & CByte(SectorSize).ToString)
            End If

            If SectorCount <> DEFAULT_SECTOR_COUNT Then
                args.Add("-n" & CShort(SectorCount).ToString)
            End If

            If TrackDistance <> DEFAULT_TRACK_DISTANCE Then
                args.Add("-k" & CByte(TrackDistance).ToString)
            End If

            If RPM <> DEFAULT_RPM Then
                args.Add("-v" & RPM.ToString)
            End If

            If OutputTrackStart > -1 Then
                args.Add("-os" & OutputTrackStart)
            End If
            If OutputTrackEnd > -1 Then
                args.Add("-oe" & OutputTrackEnd)
            End If

            args.Add("-f" & Quoted(_OutFile))
            args.Add("-i" & GetImageTypeString(OutImageType))

            If LogLevel <> DEFAULT_LOG_LEVEL Then
                args.Add("-l" & LogLevel)
            End If

            Return String.Join(" ", args)
        End Function

        Public Function GetSectorType(Value As UShort) As SectorSizeEnum
            Select Case Value
                Case 128
                    Return SectorSizeEnum.Size128
                Case 256
                    Return SectorSizeEnum.Size256
                Case 1024
                    Return SectorSizeEnum.Size1024
                Case Else
                    Return SectorSizeEnum.Size512
            End Select
        End Function

        Private Function GetImageTypeString(ImageType As ImageTypeEnum) As String
            Dim value As Integer = CInt(ImageType)

            If ImageType > 99 Then
                Return (value - 100).ToString() & "a"
            Else
                Return value.ToString
            End If
        End Function
    End Class
End Namespace