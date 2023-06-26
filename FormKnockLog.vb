Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading



Public Class Knock


    Public Sub AddLog(msg As String)
        Me.DataGridView1.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
        Dim newRow As DataGridViewRow = New DataGridViewRow()
        Dim cell1 As DataGridViewCell = New DataGridViewTextBoxCell()
        Dim currentDate As DateTime = DateTime.Now
        cell1.Value = currentDate.ToString
        Dim cell2 As DataGridViewCell = New DataGridViewTextBoxCell()
        cell2.Value = msg
        newRow.Cells.Add(cell1)
        newRow.Cells.Add(cell2)
        Me.DataGridView1.Rows.Add(newRow)
        If DataGridView1.Rows.Count = 1000 Then
            DataGridView1.Rows.RemoveAt(0)
        End If
        Me.DataGridView1.Rows.Remove(newRow)
        Me.DataGridView1.Rows.Insert(0, newRow)
    End Sub

    Public Function RunKcnock(Optional daemon As Boolean = True)
        Me.AddLog("Инициализация")
        Dim KnockJob As KnockTask
        Dim KnockPinInJob As New List(Of KnockPin)
        KnockJob.Host = FormSettings.GlobalSettings.Server
        KnockJob.HostIP = Dns.GetHostEntry(FormSettings.GlobalSettings.Server).AddressList()
        Dim unused = Integer.TryParse(60, KnockJob.TimeOutLoop)

        Dim LicenseSubStrings As String() = FormSettings.GlobalSettings.Lisense.Split("-")
        For Each knockPin As String In LicenseSubStrings
            Dim K As KnockPin
            K.Protocol = knockPin(0)
            Dim PortString As String = knockPin.Substring(1)
            Integer.TryParse(PortString, K.Port)
            KnockPinInJob.Add(K)
        Next
        KnockJob.Pins = KnockPinInJob
        If daemon = False Then
            RunKcnockTask(KnockJob, Me, daemon)
        Else
            Dim task As Task = Task.Run(Sub() RunKcnockTask(KnockJob, Me, daemon))
        End If

        Return False
    End Function

    Private Sub RunKcnockTask(task As KnockTask, f As Knock, daemon As Boolean)
        f.Invoke(Sub()
                     f.AddLog("Запускаем кракена")
                 End Sub)
        Dim index As Integer = 0
        Dim str As String = "я так устал!!!"
        Dim data_strutf8Bytes As Byte() = Encoding.UTF8.GetBytes(str)
        While True
            Dim cur As KnockPin = task.Pins(index)
            Dim protocol As String = ""
            Dim port As String = cur.Port
            Dim ip As IPAddress = task.HostIP(0)
            Dim ipStr As String = ip.ToString()
            Dim socketTcp As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            Dim socketUdp As New Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            Dim udpClient As New UdpClient()
            Dim tcpClient As New TcpClient()
            If cur.Protocol = "T" Then
                protocol = "tcp"
                Try
                    tcpClient.Client.SendTo(New Byte() {}, New IPEndPoint(ip, cur.Port))
                Catch ex As Exception
                End Try

            End If
            If cur.Protocol = "U" Then
                protocol = "udp"
                Try
                    udpClient.Send(New Byte() {}, 0, New IPEndPoint(ip, cur.Port))
                Catch ex As Exception

                End Try

            End If
            Dim strSend As String = String.Format("send ->({0}) {1}:{2}", protocol, ipStr, port)
            f.Invoke(Sub()
                         f.AddLog(strSend)
                     End Sub)
            index += 1

            If index >= task.Pins.Count Then
                If daemon = False Then
                    Exit Sub
                End If
                index = 0
                Dim slp As Integer = (task.TimeOutLoop - 5)
                f.Invoke(Sub()
                             f.AddLog(String.Format("Кракен жуёт {0} секунд", slp))
                         End Sub)
                Thread.Sleep((task.TimeOutLoop - 5) * 1000)
            Else
                Thread.Sleep(0.5 * 1000)
            End If
        End While

    End Sub


    Private Sub Knock_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Отменить закрытие формы
        e.Cancel = True
        ' Скрыть форму вместо закрытия
        Me.Hide()
    End Sub

    Private Sub Knock_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim icon As Icon = My.Resources.icon
        Me.Icon = icon
    End Sub

    Structure KnockTask
        Public Host As String
        Public HostIP As IPAddress()
        Public TimeOutLoop As Integer
        Public Pins As List(Of KnockPin)
    End Structure
    Structure KnockPin
        Public Protocol As String
        Public Port As Integer
    End Structure
End Class