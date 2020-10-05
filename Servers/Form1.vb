Imports System.ComponentModel
Imports System.Net
Imports Microsoft.VisualBasic.Devices
Public Class Form1
    Public gameslist As New List(Of String)
    Dim att As Integer = 0
    Dim str As New List(Of String)
    Public wc As New Net.WebClient
    Function getdescription(game As String)
        Try
            Dim str As String = wc.DownloadString("https://downdetector.com/status/" + game + "/")
            Dim des As String = System.Text.RegularExpressions.Regex.Match(str, """description"": (.*?),").Groups.Item(1).Value
            des = des.Replace("\u002D", "-")
            des = des.Replace("""", Nothing)
            Return des
        Catch ex As Exception
            If Not My.Computer.Network.IsAvailable Then
                MsgBox("Lost connection!", MsgBoxStyle.Critical, "Error")
            Else
                MsgBox("Game not found!", MsgBoxStyle.Information, "Not found")
            End If
        End Try
    End Function
    Function Status(game As String) As String
        Try
            Dim wc As New WebClient
            Dim s As String = wc.DownloadString("https://downdetector.com/status/" + game + "/")
            If game = "gta5" Then
                game = "GTA 5"
            End If
            game = game.Replace("-", " ")
            If s.Contains("No problems at " + game) Then
                Return "Online"
            ElseIf s.Contains("Possible problems at " + game) Then
                Return "possible problems"
            Else
                Return "Offline"
            End If
        Catch ex As Exception
            If Not My.Computer.Network.IsAvailable Then
                MsgBox("Lost connection!", MsgBoxStyle.Critical, "Error")
            End If
        End Try
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Games/apps list *case sensitive* according to "downdetector.com" standards '
        gameslist.Add("Valorant")
        gameslist.Add("Call-of-Duty")
        gameslist.Add("Overwatch")
        gameslist.Add("Fortnite")
        gameslist.Add("Fifa")
        gameslist.Add("gta5")
        gameslist.Add("Instagram")
        gameslist.Add("Snapchat")
        gameslist.Add("Gmail")
        gameslist.Add("Twitter")
        gameslist.Add("Playstation-Network")
        gameslist.Add("Xbox-Live")
        gameslist.Add("Fall-Guys")
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' Server-status project by NWA ' 
        ' Instagram @_n8z ' 
        Control.CheckForIllegalCrossThreadCalls = False
        Timer1.Start()
        Dim nwa = New Threading.Thread(AddressOf lloop) : nwa.Start()
    End Sub
    Sub lloop()
        While True
            Try
                Parallel.ForEach(gameslist, Sub(game)
                                                If Status(game).Contains("Online") Then
                                                    'if the "OFFLINE" game or the game that has "PROBLEMS" is now online 
                                                    'it will get removed from the offline/has problems list
                                                    If str.Contains(game) Then
                                                        str.Remove(game)
                                                    End If
                                                    If Not CheckBox1.Checked Then
                                                        TextBox1.AppendText("*" + game + " : " + "ONLINE" + "*" + vbNewLine)
                                                    End If
                                                    att += 1
                                                    Me.Text = "Online: " + att.ToString
                                                ElseIf Status(game).Contains("possible problems") Then
                                                    TextBox1.AppendText("*" + game + " : " + "HAS PROBLEMS" + "*" + vbNewLine)
                                                    If Not str.Contains(game) Then
                                                        str.Add(game)
                                                        '   send status to discord server // discord(game, "Server Status: possible problems")
                                                    End If
                                                ElseIf Status(game).Contains("Offline") Then
                                                    TextBox1.AppendText("*" + game + " : " + "OFFLINE" + "*" + vbNewLine)
                                                    If Not str.Contains(game) Then
                                                        str.Add(game)
                                                        '   send status to discord server // discord(game, "Server Status: Offline")
                                                    End If
                                                End If
                                            End Sub)
                Threading.Thread.Sleep(TimeSpan.FromSeconds(2))
                TextBox1.Clear()
            Catch ex As Exception

            End Try
        End While
    End Sub
    Sub discord(game As String, status As String)
        Try : Net.ServicePointManager.CheckCertificateRevocationList = False : Net.ServicePointManager.DefaultConnectionLimit = 300 : Net.ServicePointManager.UseNagleAlgorithm = False : Net.ServicePointManager.Expect100Continue = False : Net.ServicePointManager.SecurityProtocol = 3072
            Dim Encoding As New Text.UTF8Encoding
            Dim str As String = "{""embeds"":[{""title"":""" + game + """,""description"":""" + status + """,""color"":" + rndcolor() + "}]}"
            Dim Bytes As Byte() = Encoding.GetBytes(str)
            Dim AJ As Net.HttpWebRequest = DirectCast(Net.WebRequest.Create("*webhook*"), Net.HttpWebRequest)
            With AJ
                .Method = "POST"
                .Proxy = Nothing
                .ContentType = "application/json"
                .ContentLength = Bytes.Length
            End With
            Dim Stream As IO.Stream = AJ.GetRequestStream() : Stream.Write(Bytes, 0, Bytes.Length) : Stream.Dispose() : Stream.Close()
            Dim Reader As New IO.StreamReader(DirectCast(AJ.GetResponse(), Net.HttpWebResponse).GetResponseStream()) : Dim Text As String = Reader.ReadToEnd : Reader.Dispose() : Reader.Close()
        Catch ex As WebException
        End Try
    End Sub
    Function rndcolor() As String
        Dim Random As New Random()
        Dim color = String.Format("{0:X6}", Random.Next(0, 1000000))
        Dim re = CInt("&H" & color)
        Return re
    End Function
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' clears the list every 10m to avoid spamming the messages
        str.Clear()
    End Sub
    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        End
    End Sub
End Class
