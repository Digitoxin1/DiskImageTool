Namespace DiskImage
    Public Class ExpandedDate
        Public Sub New(FATDate As UShort, FATTime As UShort, Milliseconds As Byte)
            _Day = FATDate Mod 32
            FATDate >>= 5
            _Month = FATDate Mod 16
            FATDate >>= 4
            _Year = 1980 + FATDate Mod 128

            _Second = (FATTime Mod 32) * 2
            FATTime >>= 5
            _Minute = FATTime Mod 64
            FATTime >>= 6
            _Hour = FATTime Mod 32

            If Milliseconds < 200 Then
                _Second += (Milliseconds \ 100)
                _Milliseconds = (Milliseconds Mod 100) * 10
            Else
                _Milliseconds = 0
            End If

            _IsPM = (_Hour > 11)

            If _Hour > 12 Then
                _Hour12 = _Hour - 12
            ElseIf _Hour = 0 Then
                _Hour12 = 12
            Else
                _Hour12 = _Hour
            End If

            SetDateObject()
        End Sub

        Public ReadOnly Property DateObject As Date
        Public ReadOnly Property Day As Byte
        Public ReadOnly Property Hour As Byte
        Public ReadOnly Property Hour12 As Byte
        Public ReadOnly Property IsPM As Boolean
        Public ReadOnly Property IsValidDate As Boolean
        Public ReadOnly Property Milliseconds As UShort
        Public ReadOnly Property Minute As Byte
        Public ReadOnly Property Month As Byte
        Public ReadOnly Property Second As Byte
        Public ReadOnly Property Year As UShort

        Public Shared Function ExpandDate(FATDate As UShort) As ExpandedDate
            Return New ExpandedDate(FATDate, 0, 0)
        End Function

        Public Shared Function ExpandDate(FATDate As UShort, FATTime As UShort) As ExpandedDate
            Return New ExpandedDate(FATDate, FATTime, 0)
        End Function

        Public Shared Function ExpandDate(FATDate As UShort, FATTime As UShort, Milliseconds As Byte) As ExpandedDate
            Return New ExpandedDate(FATDate, FATTime, Milliseconds)
        End Function

        Public Shared Function ExpandTime(FATTime As UShort) As ExpandedDate
            Return New ExpandedDate(33, FATTime, 0)
        End Function

        Public Shared Function ExpandTimeDate(FATTimeDate As UInteger) As ExpandedDate
            Dim FATDate As UShort
            Dim FATTime As UShort

            FATTime = FATTimeDate Mod 65536
            FATTimeDate >>= 16
            FATDate = FATTimeDate

            Return New ExpandedDate(FATDate, FATTime, 0)
        End Function

        Public Overloads Function ToString() As String
            Return ToString(False, False, False, False)
        End Function

        Public Overloads Function ToString(IncludeTime As Boolean, IncludeSeconds As Boolean, IncludeMilliseconds As Boolean, Use24Hour As Boolean) As String
            Dim Response As String = Format(_Year, "0000") & "-" & Format(_Month, "00") & "-" & Format(_Day, "00")
            If IncludeTime Then
                Response &= "  " & Format(If(Use24Hour, _Hour, _Hour12), "00") _
                    & ":" & Format(_Minute, "00")

                If IncludeSeconds Then
                    Response &= ":" & Format(_Second, "00")
                End If

                If IncludeMilliseconds Then
                    Response &= Format(_Milliseconds / 1000, ".000")
                End If

                If Not Use24Hour Then
                    Response &= If(_IsPM, " PM", " AM")
                End If
            End If

            Return Response
        End Function

        Private Sub SetDateObject()
            Dim Year As UShort
            Dim Month As Byte
            Dim Day As Byte
            Dim Hour As Byte
            Dim Minute As Byte
            Dim Second As Byte
            Dim Milliseconds As UShort

            _IsValidDate = True

            If _Year < 1 Then
                Year = 1
                _IsValidDate = False
            ElseIf _Year > 9999 Then
                Year = 9999
                _IsValidDate = False
            Else
                Year = _Year
            End If

            If _Month < 1 Then
                Month = 1
                _IsValidDate = False
            ElseIf _Month > 12 Then
                Month = 12
                _IsValidDate = False
            Else
                Month = _Month
            End If

            Dim DaysInMonth = DateTime.DaysInMonth(Year, Month)

            If _Day < 1 Then
                Day = 1
                _IsValidDate = False
            ElseIf _Day > DaysInMonth Then
                Day = DaysInMonth
                _IsValidDate = False
            Else
                Day = _Day
            End If

            If _Hour > 23 Then
                Hour = 23
                _IsValidDate = False
            Else
                Hour = _Hour
            End If

            If _Minute > 59 Then
                Minute = 59
                _IsValidDate = False
            Else
                Minute = _Minute
            End If

            If _Second > 59 Then
                Second = 59
                _IsValidDate = False
            Else
                Second = _Second
            End If

            If _Milliseconds > 999 Then
                Milliseconds = 999
                _IsValidDate = False
            Else
                Milliseconds = _Milliseconds
            End If

            _DateObject = New Date(Year, Month, Day, Hour, Minute, Second, Milliseconds)
        End Sub
    End Class
End Namespace
