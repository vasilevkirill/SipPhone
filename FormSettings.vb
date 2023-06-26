Imports System.Net
Imports System.Text.RegularExpressions
Imports Microsoft.Win32

Public Class FormSettings

    Public Structure Settings
        Public Lisense As String
        Public Server As String
        Public Name As String
        Public Number As String
        Public Password As String

        Public Sub Validate()
            If Not (Regex.IsMatch(Lisense, GlobalConstants.LicenseKeyPattern)) Then
                Throw New Exception("Лицензия не верная")
            End If
            If String.IsNullOrEmpty(Server) Then
                Throw New Exception("Сервер не должен быть пустым")
            End If
            If String.IsNullOrEmpty(Name) Then
                Throw New Exception("Имя не должно быть пустым")
            End If

            If String.IsNullOrEmpty(Password) Then
                Throw New Exception("Пароль не должен быть пустым")
            End If

            If String.IsNullOrEmpty(Number) Then
                Throw New Exception("Номер не должен быть пустым")
            End If
            Dim hostIP As IPAddress()

            Try
                hostIP = Dns.GetHostEntry(Server).AddressList()

            Catch ex As Exception
                Throw New Exception(String.Format("Не верное доменное имя или IP: {0} ", Server))
            End Try

            If hostIP.Count() = 0 Then
                Throw New Exception(String.Format("Не верное доменное имя или IP: {0} ", Server))
            End If

        End Sub


    End Structure
    Public GlobalSettings As Settings
    Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Button1.Text = "Проверка..."

        Me.Refresh()

        Dim s As Settings
        s.Lisense = Me.TextBoxLicense.Text
        s.Server = Me.TextBoxServer.Text
        s.Name = Me.TextBoxName.Text
        s.Number = Me.TextBoxNumber.Text
        s.Password = Me.TextBoxPassword.Text
        Try
            s.Validate()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Me.Button1.Text = "OK"
            Me.Refresh()
            Exit Sub

        End Try

        Try
            SaveSettings(s)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Me.Button1.Text = "OK"
            Me.Refresh()
        End Try
        GlobalSettings = s
        Me.Button1.Text = "OK"
        Me.Refresh()
        Me.Close()

    End Sub




    Public Sub LoadSettings()
        Dim keyBoll As Boolean = Registry.CurrentUser.OpenSubKey(GlobalConstants.RegisterKey) IsNot Nothing
        If keyBoll = False Then
            Dim createdKey As RegistryKey = Registry.CurrentUser.CreateSubKey(GlobalConstants.RegisterKey)
            Exit Sub
        End If

        Dim KeyOpen As RegistryKey = Registry.CurrentUser.OpenSubKey(GlobalConstants.RegisterKey, False)
        Dim settings As Settings = New Settings()

        Dim kLisense As Boolean = KeyOpen.GetValue("Lisense") IsNot Nothing
        If kLisense = True Then
            settings.Lisense = KeyOpen.GetValue("Lisense").ToString()
        End If
        Dim kServer As Boolean = KeyOpen.GetValue("Server") IsNot Nothing
        If kServer = True Then
            settings.Server = KeyOpen.GetValue("Server").ToString()
        End If
        Dim kName As Boolean = KeyOpen.GetValue("Name") IsNot Nothing
        If kName = True Then
            settings.Name = KeyOpen.GetValue("Name").ToString()
        End If

        Dim kNumber As Boolean = KeyOpen.GetValue("Number") IsNot Nothing
        If kNumber = True Then
            settings.Number = KeyOpen.GetValue("Number").ToString()
        End If
        Dim kPassword As Boolean = KeyOpen.GetValue("Password") IsNot Nothing
        If kPassword = True Then
            settings.Password = KeyOpen.GetValue("Password").ToString()
        End If

        If Not (settings.Equals(New Settings())) Then
            GlobalSettings = settings
        End If
        KeyOpen.Close()
    End Sub

    Private Sub SaveSettings(settings As Settings)

        Try
            ' Открываем ключ реестра
            Using registryKey As RegistryKey = Registry.CurrentUser.OpenSubKey(GlobalConstants.RegisterKey, True)
                ' Сохраняем  в реестре
                registryKey.SetValue("Lisense", settings.Lisense)
                registryKey.SetValue("Server", settings.Server)
                registryKey.SetValue("Name", settings.Name)
                registryKey.SetValue("Number", settings.Number)
                registryKey.SetValue("Password", settings.Password)

            End Using
        Catch ex As Exception
            ' Обработка ошибки
            Knock.AddLog("Ошибка при сохранении данных в реестре: " & ex.Message)
            Throw New Exception("Что то пошло нет так")

        End Try
    End Sub

    Private Sub FormSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim icon As Icon = My.Resources.icon
        Me.Icon = icon
        Me.TextBoxLicense.Text = GlobalSettings.Lisense
        Me.TextBoxServer.Text = GlobalSettings.Server
        Me.TextBoxName.Text = GlobalSettings.Name
        Me.TextBoxNumber.Text = GlobalSettings.Number
        Me.TextBoxPassword.Text = GlobalSettings.Password
    End Sub

    Public Sub CreateIniSipConfig()
        Form1.Form1_Label1_SetText("Подготовка конфига")

        Dim ini As New IniFile(GlobalConstants.MicroSipIniPath)
        ini.WriteValue("Account1", "label", GlobalSettings.Name)
        ini.WriteValue("Account1", "displayName", GlobalSettings.Name)
        ini.WriteValue("Account1", "server", GlobalSettings.Server)
        ini.WriteValue("Account1", "domain", GlobalSettings.Server)
        ini.WriteValue("Account1", "username", GlobalSettings.Number)
        ini.WriteValue("Account1", "authID", GlobalSettings.Number)
        ini.WriteValue("Account1", "password", GlobalSettings.Password)
    End Sub

    Private Sub FormSettings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Вызов функции LoadSettings для загрузки настроек перед закрытием формы
        LoadSettings()
        Try
            ' Проверка валидности глобальных настроек
            GlobalSettings.Validate()
            Exit Sub ' Если настройки валидны, выходим из события
        Catch ex As Exception
            ' Если возникло исключение при проверке настроек
            ' Выводим сообщение об ошибке и закрываем приложение
            MsgBox("Данные введены некорректно, приложение будет закрыто")
            Environment.Exit(-100)
        End Try

    End Sub
End Class