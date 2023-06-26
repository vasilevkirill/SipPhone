Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms

Public Class window
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function EnumWindows(ByVal lpEnumFunc As EnumWindowsProc, ByVal lParam As IntPtr) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function GetClassName(ByVal hWnd As IntPtr, ByVal lpClassName As StringBuilder, ByVal nMaxCount As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function GetWindowText(ByVal hWnd As IntPtr, ByVal lpWindowText As StringBuilder, ByVal nMaxCount As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowThreadProcessId(ByVal hWnd As IntPtr, ByRef lpdwProcessId As Integer) As Integer
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetWindowRect(ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function GetClientRect(ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    Private Delegate Function EnumWindowsProc(ByVal hWnd As IntPtr, ByVal lParam As IntPtr) As Boolean

    Public Function FindWindowByProcessId(ByVal processId As Integer) As IntPtr
        Dim targetWindow As IntPtr = IntPtr.Zero
        EnumWindows(
            Function(hWnd, lParam)
                Dim windowProcessId As Integer
                GetWindowThreadProcessId(hWnd, windowProcessId)
                If windowProcessId = processId Then
                    targetWindow = hWnd
                    Return False
                End If
                Return True
            End Function,
            IntPtr.Zero
        )

        If targetWindow <> IntPtr.Zero Then
            Return targetWindow
        End If

        Return Nothing
    End Function



    <StructLayout(LayoutKind.Sequential)>
    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    Public Function GetWindowSize(ByVal handle As IntPtr) As Size
        Dim windowRect As RECT
        GetWindowRect(handle, windowRect)

        Dim clientRect As RECT
        GetClientRect(handle, clientRect)

        Dim borderWidth As Integer = (windowRect.Right - windowRect.Left) - (clientRect.Right - clientRect.Left)
        Dim borderHeight As Integer = (windowRect.Bottom - windowRect.Top) - (clientRect.Bottom - clientRect.Top)

        Dim windowWidth As Integer = clientRect.Right - clientRect.Left + borderWidth
        Dim windowHeight As Integer = clientRect.Bottom - clientRect.Top + borderHeight

        Return New Size(windowWidth, windowHeight)
    End Function


    Private Const GWL_STYLE As Integer = -16
    Private Const WS_BORDER As Integer = &H800000

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetWindowLong(ByVal hWnd As IntPtr, ByVal nIndex As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetWindowLong(ByVal hWnd As IntPtr, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As Integer
    End Function

    Public Sub ChangeWindowBorderStyle(ByVal handle As IntPtr, ByVal borderStyle As FormBorderStyle)
        Dim currentStyle As Integer = GetWindowLong(handle, GWL_STYLE)

        ' Удаление старого стиля границы окна
        currentStyle = currentStyle And (Not WS_BORDER)

        ' Добавление нового стиля границы окна в соответствии с FormBorderStyle
        Select Case borderStyle
            Case FormBorderStyle.FixedSingle
                currentStyle = currentStyle Or WS_BORDER
            Case FormBorderStyle.Fixed3D, FormBorderStyle.FixedDialog, FormBorderStyle.FixedToolWindow
                ' Для этих стилей граница окна добавляется автоматически, не требуется дополнительных действий
            Case Else
                ' Для остальных стилей граница окна не нужна, не требуется дополнительных действий
        End Select

        ' Установка нового стиля границы окна
        SetWindowLong(handle, GWL_STYLE, currentStyle)

        ' Перерисовка окна, чтобы изменения вступили в силу
        RedrawWindow(handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Frame Or RedrawWindowFlags.UpdateNow)
    End Sub

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function RedrawWindow(ByVal hWnd As IntPtr, ByVal lprcUpdate As IntPtr, ByVal hrgnUpdate As IntPtr, ByVal flags As RedrawWindowFlags) As Boolean
    End Function

    <Flags>
    Private Enum RedrawWindowFlags As UInteger
        Invalidate = &H1
        InternalPaint = &H2

        Validate = &H8
        NoInternalPaint = &H10
        NoErase = &H20
        NoChildren = &H40
        AllChildren = &H80
        UpdateNow = &H100
        EraseNow = &H200
        Frame = &H400
        NoFrame = &H800
    End Enum
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As Integer) As Boolean
    End Function

    Public Shared Sub ResizeMoveWindowByHandle(ByVal handle As IntPtr, ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        Dim SWP_SHOWWINDOW As Integer = &H40
        Dim SWP_NOZORDER As Integer = &H4
        Dim SWP_NOACTIVATE As Integer = &H10

        SetWindowPos(handle, IntPtr.Zero, x, y, width, height, SWP_SHOWWINDOW Or SWP_NOZORDER Or SWP_NOACTIVATE)
    End Sub

    Private Declare Function SetParent Lib "user32" Alias "SetParent" (ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As Integer

    Public Shared Sub ChangeParenthWnd(ByVal hWndChild As IntPtr, ByVal hWndParent As IntPtr)
        SetParent(hWndChild, hWndParent)
    End Sub
    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Public Shared Function SetForegroundWindow(hWnd As IntPtr) As Boolean
    End Function


    Private Const SW_MAXIMIZE As Integer = 3

    Public Shared Sub MaximizedByhWnd(ByVal hWnd As IntPtr)
        ShowWindow(hWnd, SW_MAXIMIZE)
    End Sub
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function ShowWindow(hWnd As IntPtr, nCmdShow As Integer) As Boolean
    End Function

End Class
