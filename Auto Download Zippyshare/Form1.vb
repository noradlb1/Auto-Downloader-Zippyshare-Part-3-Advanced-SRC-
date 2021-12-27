Imports System.Net

Public Class Form1
    Dim LagiDunlud As Boolean = True
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If Button1.Text = "Download" Then
            CUT = 1
            LagiDunlud = True
            WebBrowser1.Navigate(TextBox1.Text)
            Button1.Text = "Cancel"
            TextBox1.Enabled = False
        Else
            LagiDunlud = False
            Button1.Text = "Download"
            TextBox1.Enabled = True
        End If
    End Sub
    Dim WithEvents Dwn As Net.WebClient
    Dim CUT As Integer = 1, sz As String
    Private Sub WebBrowser1_DocumentCompleted(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        If CUT = 1 Then
            CUT += 1
            Try
                For Each Ghtm As HtmlElement In WebBrowser1.Document.GetElementsByTagName("a")
                    If Ghtm.GetAttribute("href").StartsWith("/d/") Then
                        Dim Spl As String() = Split(TextBox1.Text, "/v/")
                        Dim LinkDownload As String = Spl(0) & Ghtm.GetAttribute("href")
                        Dim Info As String() = Split(GetNameAndSize(LinkDownload), "|")

                        FileNama.Text = Info(0)
                        UkuranFile.Text = Info(1)
                        sz = Info(1)
                        WebBrowser1.Navigate("about:blank")
                        WebBrowser1 = New WebBrowser
                        WebBrowser1.ScriptErrorsSuppressed = True

                        Try : IO.Directory.CreateDirectory(TextBox2.Text) : Catch : End Try
                        TextBox2.Text = TextBox2.Text & "\" & Info(0)

                        Dwn = New Net.WebClient ' jangan lupa kasih ini biar gak gagal downloadnya!
                        Dwn.DownloadFileAsync(New Uri(LinkDownload), TextBox2.Text)
                    End If
                Next
            Catch : End Try
        End If
    End Sub
    Function GetNameAndSize(ByVal Link As String) As String
        Dim HttpWebReq As HttpWebRequest, HttpWebRes As HttpWebResponse = Nothing, TempStr As String = String.Empty
        'Try
        HttpWebReq = WebRequest.Create(Link)
        HttpWebReq.Timeout = 8000
        HttpWebReq.Proxy = Nothing
        HttpWebReq.KeepAlive = False
        HttpWebReq.AllowAutoRedirect = True
        HttpWebRes = HttpWebReq.GetResponse
        'Catch : End Try
        Dim Sr As IO.StreamReader = New IO.StreamReader(WebBrowser1.DocumentStream)
        Dim Count As Integer = 0
        For Each Gtt As String In Split(Sr.ReadToEnd, ";"">")
            Dim SPL2 As String() = Split(Gtt, "</font>")
            If SPL2(0) = "Name:" Then
                Count += 1
            ElseIf Count = 1 Then
                Count = 0
                If SPL2(0).StartsWith("<img") Then
                    Dim SPLTep As String() = Split(HttpWebRes.Headers.Item(0), "filename")
                    TempStr = SPLTep(1).Replace("*=UTF-8''", String.Empty).Replace("=", String.Empty) & "|"
                Else
                    TempStr = SPL2(0) & "|"
                End If
            End If
        Next
        Dim Tmpp As String = GetUkuran(HttpWebRes.ContentLength)
        HttpWebReq.Abort()
        HttpWebReq = Nothing
        HttpWebRes.Close()
        HttpWebRes = Nothing
        Return TempStr & Tmpp
    End Function
    Function GetUkuran(ByVal Sz As Object) As String
        Dim fsize As Long = Convert.ToInt64(Sz), Fzn As String = String.Empty
        If fsize > 1073741824 Then
            Dim size As Double = fsize / 1073741824
            Fzn = Math.Round(size, 2).ToString & "GB"
        ElseIf fsize > 1048576 Then
            Dim size As Double = fsize / 1048576
            Fzn = Math.Round(size, 2).ToString & "MB"
        ElseIf fsize > 1024 Then
            Dim size As Double = fsize / 1024
            Fzn = Math.Round(size, 2).ToString & "KB"
        Else
            Fzn = fsize.ToString & "B"
        End If
        Return Fzn
    End Function
    Dim Tm As Integer = 0
    Dim TmpByts As Long = 0
    Private Sub Dwn_DownloadProgressChanged(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs) Handles Dwn.DownloadProgressChanged
        If Not LagiDunlud Then
            Dwn.CancelAsync()
            Exit Sub
        End If
        ProgressBar1.Value = e.ProgressPercentage
        ProgressBar3.Maximum = Convert.ToInt32(e.TotalBytesToReceive)
        Dim Spedcek As Integer
        If sz.EndsWith("KB") Then
            Spedcek = 1
        ElseIf sz.EndsWith("B") Then
            Spedcek = 1
        Else
            Spedcek = 500
        End If
        If Tm = 500 Then ' 500ms,1000 = 1sec
            Tm = 0
            Dim fsize As Long
            If Not TmpByts = 0 Then
                fsize = e.BytesReceived - TmpByts
            Else
                fsize = e.BytesReceived
            End If
            TmpByts = e.BytesReceived
            kecepatandunlud.Text = GetUkuran(fsize)
            Try : ProgressBar3.Value = Convert.ToInt32(fsize) : Catch : End Try
        Else
            Tm += 1
        End If
        Label1.Text = ProgressBar1.Value & "%"
        UkuranFile.Text = GetUkuran(e.BytesReceived) & "/" & sz
        If ProgressBar1.Value = 100 Then Button1.Text = "Download"
    End Sub


    Private Sub WebBrowser1_ProgressChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.WebBrowserProgressChangedEventArgs) Handles WebBrowser1.ProgressChanged
        ProgressBar2.Maximum = e.MaximumProgress
        Try
            ProgressBar2.Value = e.CurrentProgress
        Catch : End Try
    End Sub


    Private Sub FileNama_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles FileNama.KeyPress
        e.Handled = True
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim Ofd As New FolderBrowserDialog
        With Ofd
            .Description = "Choose Directory"
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                TextBox2.Text = .SelectedPath
            End If
        End With
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        TextBox2.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("\Desktop", "\Downloads")
    End Sub

    Private Sub TextBox2_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        If Button1.Text = "Cancel" Then
            e.Handled = True
        End If
    End Sub
End Class
