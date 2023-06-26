Imports System.IO

Module GlobalConstants
    Public ProjectName As String = "MicroSIP-Wrapper"
    Public Const RegisterKey As String = "Software\MicroSIP-Wrapper"
    Public Const LicenseKeyPattern As String = "[T|U]\d{5}-[T|U]\d{5}-[T|U]\d{5}-[T|U]\d{5}-[T|U]\d{5}"
    Public ProjectAppPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProjectName)
    Public ProjectAppMicroSipPath As String = Path.Combine(ProjectAppPath, "microsip")
    Public MicroSipIniPath As String = Path.Combine(ProjectAppMicroSipPath, "microsip.ini")
    Public LicenseKey As String
End Module
