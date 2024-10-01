Namespace ImageFormats
    Namespace _86F
        Module Functions
            Public Function GetBitRate(BitRate As BitRate, IsMFM As Boolean) As UShort
                Dim Result As UShort

                Select Case BitRate
                    Case BitRate.BitRate250
                        Result = 250
                    Case BitRate.BitRate300
                        Result = 300
                    Case BitRate.BitRate500
                        Result = 500
                    Case BitRate.BitRate1000
                        Result = 1000
                    Case BitRate.BitRate2000
                        Result = 2000
                    Case Else
                        Result = 250
                End Select

                If Not IsMFM Then
                    Result /= 2
                End If

                Return Result
            End Function

            Public Function GetBitRate(BitRate As UShort, IsMFM As Boolean) As BitRate
                Dim Result As BitRate

                BitRate = Bitstream.RoundBitRate(BitRate)

                If Not IsMFM Then
                    BitRate *= 2
                End If

                Select Case BitRate
                    Case 250
                        Result = _86F.BitRate.BitRate250
                    Case 300
                        Result = _86F.BitRate.BitRate300
                    Case 500
                        Result = _86F.BitRate.BitRate500
                    Case 1000
                        Result = _86F.BitRate.BitRate1000
                    Case 2000
                        Result = _86F.BitRate.BitRate2000
                    Case Else
                        Result = _86F.BitRate.BitRate250
                End Select

                Return Result
            End Function

            Public Function GetRPM(RPM As RPM) As UShort
                Select Case RPM
                    Case RPM.RPM300
                        Return 300
                    Case RPM.RPM360
                        Return 360
                    Case Else
                        Return 300
                End Select
            End Function

            Public Function GetRPM(RPM As UShort) As RPM
                RPM = Bitstream.RoundRPM(RPM)

                Select Case RPM
                    Case 300
                        Return _86F.RPM.RPM300
                    Case 350
                        Return _86F.RPM.RPM360
                    Case Else
                        Return _86F.RPM.RPM300
                End Select
            End Function
        End Module
    End Namespace
End Namespace
