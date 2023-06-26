Imports System.Runtime.InteropServices
Imports System.Text

Public Class IniFile
    Private ReadOnly filePath As String

    Public Sub New(path As String)
        filePath = path
    End Sub

    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function GetPrivateProfileString(
        section As String, key As String, defaultValue As String,
        <MarshalAs(UnmanagedType.LPWStr)> returnValue As StringBuilder, size As Integer,
        filePath As String) As Integer
    End Function

    <DllImport("kernel32.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function WritePrivateProfileString(
        section As String, key As String, value As String,
        filePath As String) As Integer
    End Function

    Public Function ReadValue(section As String, key As String, defaultValue As String) As String
        Dim sb As New StringBuilder(255)
        GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, filePath)
        Return sb.ToString()
    End Function

    Public Sub WriteValue(section As String, key As String, value As String)
        WritePrivateProfileString(section, key, value, filePath)
    End Sub
End Class

