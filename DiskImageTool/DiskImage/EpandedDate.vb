Namespace DiskImage
    Public Structure ExpandedDate
        Dim DateObject As Date
        Dim Day As Byte
        Dim Hour As Byte
        Dim Hour12 As Byte
        Dim IsPM As Boolean
        Dim IsValidDate As Boolean
        Dim Milliseconds As UShort
        Dim Minute As Byte
        Dim Month As Byte
        Dim Second As Byte
        Dim Year As UShort
    End Structure
    Module EpandedDate
        Public Function ExpandDate(FATDate As UShort) As ExpandedDate
            Return ExpandDate(FATDate, 0, 0)
        End Function

        Public Function ExpandDate(FATDate As UShort, FATTime As UShort) As ExpandedDate
            Return ExpandDate(FATDate, FATTime, 0)
        End Function

        Public Function ExpandDate(FATDate As UShort, FATTime As UShort, Milliseconds As Byte) As ExpandedDate
            Dim DT As ExpandedDate

            DT.Day = FATDate Mod 32
            FATDate >>= 5
            DT.Month = FATDate Mod 16
            FATDate >>= 4
            DT.Year = 1980 + FATDate Mod 128

            DT.Second = (FATTime Mod 32) * 2
            FATTime >>= 5
            DT.Minute = FATTime Mod 64
            FATTime >>= 6
            DT.Hour = FATTime Mod 32

            If Milliseconds < 200 Then
                DT.Second += (Milliseconds \ 100)
                DT.Milliseconds = (Milliseconds Mod 100) * 10
            Else
                DT.Milliseconds = 0
            End If

            Dim DateString As String = DT.Year & "-" & Format(DT.Month, "00") & "-" & Format(DT.Day, "00") & " " & DT.Hour & ":" & Format(DT.Minute, "00") & ":" & Format(DT.Second, "00") & "." & DT.Milliseconds.ToString.PadLeft(3, "0")

            DT.IsValidDate = IsDate(DateString)

            DT.IsPM = (DT.Hour > 11)

            If DT.Hour > 12 Then
                DT.Hour12 = DT.Hour - 12
            ElseIf DT.Hour = 0 Then
                DT.Hour12 = 12
            Else
                DT.Hour12 = DT.Hour
            End If

            If DT.IsValidDate Then
                DT.DateObject = New Date(DT.Year, DT.Month, DT.Day, DT.Hour, DT.Minute, DT.Second, DT.Milliseconds)
            End If

            Return DT
        End Function

        Public Function ExpandTime(FATTime As UShort) As ExpandedDate
            Return ExpandDate(33, FATTime, 0)
        End Function

        Public Function ExpandTimeDate(FATTimeDate As UInteger) As ExpandedDate
            Dim FATDate As UShort
            Dim FATTime As UShort

            FATTime = FATTimeDate Mod 65536
            FATTimeDate >>= 16
            FATDate = FATTimeDate

            Return ExpandDate(FATDate, FATTime)
        End Function

        Public Function ExpandedDateToString(D As DiskImage.ExpandedDate) As String
            Return ExpandedDateToString(D, False, False, False, False)
        End Function

        Public Function ExpandedDateToString(D As DiskImage.ExpandedDate, IncludeTime As Boolean, IncludeSeconds As Boolean, IncludeMilliseconds As Boolean, Use24Hour As Boolean) As String
            Dim Response As String = Format(D.Year, "0000") & "-" & Format(D.Month, "00") & "-" & Format(D.Day, "00")
            If IncludeTime Then
                Response &= "  " & Format(IIf(Use24Hour, D.Hour, D.Hour12), "00") _
                    & ":" & Format(D.Minute, "00")

                If IncludeSeconds Then
                    Response &= ":" & Format(D.Second, "00")
                End If

                If IncludeMilliseconds Then
                    Response &= Format(D.Milliseconds / 1000, ".000")
                End If

                If Not Use24Hour Then
                    Response &= IIf(D.IsPM, " PM", " AM")
                End If
            End If

            Return Response
        End Function
    End Module
End Namespace
