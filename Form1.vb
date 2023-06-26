Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Security
Imports System.Reflection
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Threading
Imports System.Timers

Public Class Form1
    Private externalProcessSip As Process
    Private ContactsByte As Byte()



    Public Sub Form1_Label1_SetText(text As String)

        Thread.Sleep(200)
        Me.Label1.Text = text + "..."
        Me.Label1.Hide()

        Dim NewL As Point
        NewL.X = (Me.Panel1.Width / 2) - (Me.Label1.Width / 2)
        NewL.Y = (Me.Panel1.Height / 2) - (Me.Label1.Height / 2)
        Me.Label1.Location = NewL
        Me.Label1.Show()
        Me.Refresh()
    End Sub

    Private Sub RunSipClient()
        Me.Form1_Label1_SetText("Проверяем запущенные процессы")
        Dim processes As Process() = process.GetProcesses()

        ' Проход по списку процессов и поиск процесса с именем microsip.exe и путем ProjectAppMicroSipPath
        For Each process As Process In processes
            Try
                If process.ProcessName = "microsip" AndAlso process.MainModule.FileName.StartsWith(ProjectAppMicroSipPath) Then
                    ' Процесс найден - завершаем его
                    process.Kill()
                End If
            Catch ex As Exception
                ' Обработка возможных ошибок при попытке завершить процесс
                Console.WriteLine("Ошибка при завершении процесса: " & ex.Message)
            End Try
        Next

        Me.Form1_Label1_SetText("Запуск звонилки")
        Dim PathToExe As String = Path.Combine(GlobalConstants.ProjectAppMicroSipPath, "microsip.exe")
        '
        Dim microsipProcess As Process = New Process

        microsipProcess.StartInfo.FileName = PathToExe

        microsipProcess.Start()
        microsipProcess.Refresh()

        microsipProcess.WaitForInputIdle()
        microsipProcess.Refresh()

        Thread.Sleep(0.5 * 1000)
        Dim window As New window()
        Dim form As New Form
        window.SetForegroundWindow(microsipProcess.MainWindowHandle)

        Dim hwhndl As IntPtr = microsipProcess.MainWindowHandle

        For Each ctl As Control In Me.Controls
            If TypeOf ctl Is MdiClient Then
                Me.AutoScroll = False
                window.ChangeParenthWnd(hwhndl, ctl.Handle)

                window.ResizeMoveWindowByHandle(hwhndl, 0, 0, 200, 300)
                Dim size As Size = window.GetWindowSize(hwhndl)
                window.ChangeWindowBorderStyle(hwhndl, FormBorderStyle.None)
                Me.ClientSize = New Size(300, 500)
                Me.Panel1.Visible = False
                window.MaximizedByhWnd(hwhndl)
                Me.MinimizeBox = True
                Exit For
            End If
        Next
        externalProcessSip = microsipProcess
    End Sub


    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        ImKill()

    End Sub

    Private Sub ImKill()
        ' Проверяем, если процесс внешнего приложения запущен
        Try
            If externalProcessSip IsNot Nothing AndAlso Not externalProcessSip.HasExited Then
                ' Закрываем процесс внешнего приложения
                externalProcessSip.CloseMainWindow()
                externalProcessSip.Close()
            End If
        Catch ex As Exception

        End Try

        Try
            File.Delete("./microsip/Contacts.xml")
        Catch
            ' Игнорирование всех исключений и ошибок
        End Try
    End Sub
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        ' Проверяем комбинацию клавиш CTRL+SHIFT+F9
        If e.Control AndAlso e.Shift AndAlso e.KeyCode = Keys.F9 Then
            UpdateContacts()
        End If

        ' Проверяем комбинацию клавиш CTRL+SHIFT+F10
        If e.Control AndAlso e.Shift AndAlso e.KeyCode = Keys.F10 Then
            ' Открываем форму
            AboutBox1.Show()
        End If

        ' Проверяем комбинацию клавиш CTRL+SHIFT+F11
        If e.Control AndAlso e.Shift AndAlso e.KeyCode = Keys.F11 Then
            ' Открываем форму
            Knock.Show()
        End If

        ' Проверяем комбинацию клавиш CTRL+SHIFT+F12
        If e.Control AndAlso e.Shift AndAlso e.KeyCode = Keys.F12 Then
            ' Открываем форму
            FormSettings.Show()
        End If




    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        Me.Form1_Label1_SetText("Загружаем файлы")
        CreateAppDataFolderAndCopyResources()
        Me.Form1_Label1_SetText("Инициализация")
        Me.Form1_Label1_SetText("Запускаем таймер")
        StartTimer()
        Me.Form1_Label1_SetText("Загружаем настройки")
        FormSettings.LoadSettings()


        Knock.Show()
        Knock.Hide()

        If FormSettings.GlobalSettings.Equals(New FormSettings.Settings()) Then
            FormSettings.ShowDialog()
            Me.Form1_Label1_SetText("Загружаем настройки")
            FormSettings.LoadSettings()
        End If

        Me.Form1_Label1_SetText("Проверяем настройки")
        If FormSettings.GlobalSettings.Equals(New FormSettings.Settings()) Then
            Environment.Exit(-100)
        End If

        Me.Form1_Label1_SetText("Проверяем настройки")
        Try
            FormSettings.GlobalSettings.Validate()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            FormSettings.ShowDialog()
            FormSettings.LoadSettings()
        End Try



        Me.Form1_Label1_SetText("Кнокаем разок")
        Knock.RunKcnock(False)
        Me.Form1_Label1_SetText("Загружаем Контакты")
        Dim cancellationTokenSource As New CancellationTokenSource()
        Dim cancellationToken As CancellationToken = cancellationTokenSource.Token
        Dim task As Task = Task.Run(Sub()
                                        UpdateContacts()
                                    End Sub, cancellationToken)
        If Not task.Wait(2000) Then
            ' Превышено время выполнения
            cancellationTokenSource.Cancel()
            Me.Form1_Label1_SetText("Время выполнения UpdateContacts() истекло")
        End If

        Me.Form1_Label1_SetText("Загружаем веб сервер")
        StartWebServer()
        Me.Form1_Label1_SetText("Активируем кноку коку")
        Knock.RunKcnock(True)
        Me.Form1_Label1_SetText("Запускаем клиента")
        FormSettings.CreateIniSipConfig()
        RunSipClient()
    End Sub

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        ImKill()

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim icon As Icon = My.Resources.icon
        Me.Icon = icon
    End Sub


    Private Sub StartWebServer()
        ' Запуск веб-сервера с использованием Task
        Dim serverTask As Task = Task.Run(AddressOf RunWebServer)
    End Sub

    Private Sub RunWebServer()
        ' Код для запуска веб-сервера
        ' ...

        ' Пример: Запуск простого веб-сервера на порту 9999
        Dim server As New HttpListener()
        server.Prefixes.Add("http://127.0.0.1:9999/")
        server.Start()

        ' Обработка входящих запросов
        While True
            Dim context As HttpListenerContext = server.GetContext()
            ' Обработка запроса
            ' ...
            Dim requestedUrl As String = context.Request.Url.AbsolutePath
            If requestedUrl = "/contacts.xml" Then
                Dim updatedContactsByte As Byte() = GetUpdatedContactsByte()
                If updatedContactsByte IsNot Nothing AndAlso updatedContactsByte.Length > 0 Then
                    context.Response.ContentType = "application/xml"
                    context.Response.ContentLength64 = updatedContactsByte.Length
                    context.Response.ContentLength64 = updatedContactsByte.Length
                    context.Response.OutputStream.Write(updatedContactsByte, 0, updatedContactsByte.Length)
                Else
                    context.Response.StatusCode = 404
                    context.Response.ContentType = "text/plain"
                    Dim errorMessage As String = "File not found."
                    Dim errorBytes As Byte() = Encoding.UTF8.GetBytes(errorMessage)
                    context.Response.ContentLength64 = errorBytes.Length
                    context.Response.OutputStream.Write(errorBytes, 0, errorBytes.Length)
                End If

            End If
        End While

        ' Остановка сервера
        server.Stop()
    End Sub
    Private Function GetUpdatedContactsByte() As Byte()
        ' Ваш код для обновления значения ContactsByte
        ' ...
        Return ContactsByte
    End Function

    Private Sub StartTimer()
        ' Создание и настройка таймера
        Dim timer As New Timers.Timer()
        timer.Interval = TimeSpan.FromMinutes(10).TotalMilliseconds ' Установка интервала в 10 минут
        timer.AutoReset = True ' Установка автоматического повторения
        AddHandler timer.Elapsed, AddressOf UpdateContacts ' Подписка на событие Elapsed

        ' Запуск таймера
        timer.Start()
    End Sub
    Private Sub UpdateContacts(Optional sender As Object = Nothing, Optional e As ElapsedEventArgs = Nothing)
        ContactsByte = New Byte() {}
        Dim Url As String = String.Format("https://{0}:9443/contacts.xml", FormSettings.GlobalSettings.Server)
        Dim handler As New HttpClientHandler()
        Try
            handler.ServerCertificateCustomValidationCallback = AddressOf IgnoreCertificateErrorsCallback
            ' Создание HttpClient с HttpClientHandler
            Dim httpClient As New HttpClient(handler)
            ' Загрузка файла
            Dim response As HttpResponseMessage = httpClient.GetAsync(Url).GetAwaiter().GetResult()
            response.EnsureSuccessStatusCode()
            Dim content As HttpContent = response.Content
            Dim fileContent As Byte() = content.ReadAsByteArrayAsync().GetAwaiter().GetResult()
            ContactsByte = fileContent
        Catch ex As Exception
            'MsgBox("Что-то пошло нет так, попробуйте через 5 минут")

        End Try

    End Sub


    Private Function IgnoreCertificateErrorsCallback(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) As Boolean
        ' Всегда возвращаем True для игнорирования ошибок сертификата
        Return True
    End Function

    Private Sub CreateAppDataFolderAndCopyResources()

        ' Проверяем, существует ли папка, и если нет, то создаем ее
        If Not Directory.Exists(GlobalConstants.ProjectAppPath) Then
            Directory.CreateDirectory(GlobalConstants.ProjectAppPath)
        End If
        If Not Directory.Exists(ProjectAppMicroSipPath) Then
            Directory.CreateDirectory(ProjectAppMicroSipPath)
        End If


        ' Получение списка всех ресурсов
        Dim resourceSet As Resources.ResourceSet = My.Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, True, True)

        If resourceSet IsNot Nothing Then
            ' Перебор всех ресурсов
            For Each resource As DictionaryEntry In resourceSet
                Dim resourceName As String = resource.Key.ToString()
                Dim resourceValue As Object = resource.Value
                Try
                    ' Проверка, является ли ресурс файлом
                    If TypeOf resourceValue Is Byte() Then
                        ' Сохранение файла на диск
                        Dim filePath As String = Path.Combine(ProjectAppMicroSipPath, resourceName)
                        File.WriteAllBytes(filePath, DirectCast(resourceValue, Byte()))
                    End If

                    If TypeOf resourceValue Is UnmanagedMemoryStream Then
                        ' Сохранение данных потока в файл на диске
                        Dim filePath As String = Path.Combine(ProjectAppMicroSipPath, resourceName)
                        Using outputStream As FileStream = File.Create(filePath)
                            DirectCast(resourceValue, UnmanagedMemoryStream).CopyTo(outputStream)
                        End Using
                    End If
                    If TypeOf resourceValue Is String Then
                        ' Сохранение текстового файла на диске
                        Dim filePath As String = Path.Combine(ProjectAppMicroSipPath, resourceName)
                        Dim content As String = DirectCast(resourceValue, String)
                        File.WriteAllText(filePath, content)
                    End If
                Catch ex As Exception

                End Try

            Next
        End If




        ' Вы можете повторить аналогичные действия для копирования ресурсов из подпапок
        ' используя рекурсивный обход директорий

        ' Теперь все ресурсы скопированы в папку AppData для вашего приложения
    End Sub
End Class
