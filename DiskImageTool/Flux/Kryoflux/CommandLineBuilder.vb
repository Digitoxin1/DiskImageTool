Namespace Flux
    Namespace Kryoflux
        Class CommandLineBuilder

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

            Public Property DeviceMode As DeviceModeEnum = DeviceModeEnum.KryoFluxDevice
            Public Property InFile As String
            Public Property InImageType As ImageTypeEnum = ImageTypeEnum.KryoFlux_Stream
            Public Property LogLevel As LogMask = LogMask.Format
            Public Property OutFile As String
            Public Property OutImageType As ImageTypeEnum = ImageTypeEnum.MFM_SectorImage
            Public Property SingleSidedMode As SingleSidedModeEnum = SingleSidedModeEnum.off
            Public Property TrackDistance As TrackDistanceEnum = TrackDistanceEnum.Tracks80
            Public Property TrackEnd As Short = -1
            Public Property TrackStart As Short = -1

            Public Function Arguments() As String
                Dim args = New List(Of String) From {
                    "-m" & CByte(DeviceMode).ToString,
                    "-f" & Quoted(_InFile),
                    "-i" & GetImageTypeString(InImageType),
                    "-k" & CByte(TrackDistance).ToString
                }

                If TrackStart > -1 Then
                    args.Add("-s" & TrackStart)
                End If
                If TrackEnd > -1 Then
                    args.Add("-e" & TrackEnd)
                End If
                If SingleSidedMode <> SingleSidedModeEnum.off Then
                    args.Add("-g" & CInt(SingleSidedMode).ToString)
                End If
                args.Add("-f" & Quoted(_OutFile))
                args.Add("-i" & GetImageTypeString(OutImageType))
                args.Add("-l" & LogLevel)

                Return String.Join(" ", args)
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
End Namespace
