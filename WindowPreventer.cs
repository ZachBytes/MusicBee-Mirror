using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MusicBeePlugin
{
    internal class WindowPreventer
    {
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string className, string windowTitle);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public static void RunWindowPreventer(IntPtr hwnd, uint processId)
        {
            if (hwnd != IntPtr.Zero)
            {
                // Get the process ID of the target window
                //GetWindowThreadProcessId(hwnd, out uint processId);

                // Attach to the target process and get the handle to the main window
                Process process = Process.GetProcessById((int)processId);
                IntPtr handle = process.MainWindowHandle;

                // Use the handle to prevent the creation of new windows and message boxes
                NativeMethods.PreventWindowFromShowing(handle);
                NativeMethods.PreventMessageBoxFromShowing(handle);

                // Do other stuff with the target window
                // ...
            }
            else
            {
                Console.WriteLine("Could not find window");
            }
        }
    }

    public static class NativeMethods
{
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    private const int GWL_STYLE = -16;
    private const int WS_VISIBLE = 0x10000000;
    private const int WS_EX_DLGMODALFRAME = 0x0001;
    private const int WS_EX_CLIENTEDGE = 0x200;

    private const uint WM_CLOSE = 0x0010;

    public static void PreventWindowFromShowing(IntPtr handle)
    {
        // Remove the WS_VISIBLE style to prevent the window from showing
        int style = GetWindowLong(handle, GWL_STYLE);
        SetWindowLong(handle, GWL_STYLE, style & ~WS_VISIBLE);
    }

    public static void PreventMessageBoxFromShowing(IntPtr handle)
    {
        // Find the message box dialog window and close it
        IntPtr messageBoxHandle = FindWindowEx(handle, IntPtr.Zero, "#32770", IntPtr.Zero);
        if (messageBoxHandle != IntPtr.Zero)
        {
            SendMessage(messageBoxHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
}
