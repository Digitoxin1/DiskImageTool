Imports System.ComponentModel

Public Class HexSearchForm
    Public Sub New(HexSearch As HexSearch)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        LocalizeForm()

        If HexSearch.SearchHex Then
            RadBtnHex.Checked = True
            ChkCaseSensitive.Enabled = False
            ChkCaseSensitive.Checked = False
        Else
            RadBtnText.Checked = True
            ChkCaseSensitive.Enabled = True
            ChkCaseSensitive.Checked = HexSearch.CaseSensitive
        End If
        TextSearch.Text = HexSearch.SearchString
        BtnOK.Enabled = TextSearch.Text.Length > 0
    End Sub

    Private Sub LocalizeForm()
        BtnOK.Text = My.Resources.Menu_Ok
        BtnCancel.Text = My.Resources.Menu_Cancel
        Me.Text = My.Resources.Label_Find
    End Sub

    Public Function Search() As HexSearch
        Return New HexSearch With {
            .SearchHex = RadBtnHex.Checked,
            .SearchString = TextSearch.Text.Trim,
            .CaseSensitive = ChkCaseSensitive.Checked
        }
    End Function

    Private Function CheckHex() As Boolean
        Dim Result As Boolean = True

        For Counter = 0 To TextSearch.Text.Length - 1
            Dim C = TextSearch.Text.Substring(Counter, 1)
            Result = (C >= "0" And C <= "9") Or (C >= "a" And C <= "f") Or (C >= "A" And C <= "F") Or C = Chr(13) Or C = Chr(10) Or C = " "
            If Not Result Then
                Exit For
            End If
        Next

        Return Result
    End Function

    Private Sub HexSearchForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Me.DialogResult = DialogResult.OK Then
            If RadBtnHex.Checked Then
                If Not CheckHex() Then
                    MsgBox(My.Resources.Dialog_InvalidSearchString, MsgBoxStyle.Exclamation)
                    e.Cancel = True
                End If
            End If
        End If
    End Sub

    Private Sub RadBtn_CheckedChanged(sender As Object, e As EventArgs) Handles RadBtnText.CheckedChanged, RadBtnHex.CheckedChanged
        ChkCaseSensitive.Enabled = RadBtnText.Checked
    End Sub

    Private Sub TextSearch_TextChanged(sender As Object, e As EventArgs) Handles TextSearch.TextChanged
        BtnOK.Enabled = TextSearch.Text.Length > 0
    End Sub
End Class

Public Class HexSearch
    Public Sub New()
        _CaseSensitive = False
        _SearchHex = False
        _SearchString = ""
    End Sub

    Public Property CaseSensitive As Boolean
    Public Property SearchHex As Boolean
    Public Property SearchString As String
End Class
