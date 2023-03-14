using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MusicBeePlugin
{
    internal class Utils
    {
        //Attach to, intercept, and prevent Form and MessageBox creation given a HWND
        public static class FormHelper
        {
            private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern int EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern int EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

            private const int WM_CLOSE = 0x0010;

            private static readonly string[] ignoredWindowClasses = { "#32770", "Tooltips_window32", "SysShadow", "IME" };

            private static readonly string[] ignoredWindowTitles = { "Microsoft Visual Studio", "TeamViewer", "Radeon Settings", "Chrome" };

            public static void PreventChildWindowsFromShowing(IntPtr hWnd)
            {
                EnumChildWindows(hWnd, (childHWnd, lParam) =>
                {
                    if (!ShouldIgnoreWindow(childHWnd))
                    {
                        SendMessage(childHWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }

                    return true;
                }, IntPtr.Zero);
            }

            private static bool ShouldIgnoreWindow(IntPtr hWnd)
            {
                // Check if the window is in the ignored classes list
                System.Text.StringBuilder sb = new System.Text.StringBuilder(256);
                GetClassName(hWnd, sb, sb.Capacity);
                string className = sb.ToString();
                if (Array.IndexOf(ignoredWindowClasses, className) != -1)
                {
                    return true;
                }

                // Check if the window title is in the ignored titles list
                sb.Clear();
                GetWindowText(hWnd, sb, sb.Capacity);
                string title = sb.ToString();
                if (Array.IndexOf(ignoredWindowTitles, title) != -1)
                {
                    return true;
                }

                // Check if the window belongs to a different process
                uint processId;
                GetWindowThreadProcessId(hWnd, out processId);
                if (processId != GetCurrentProcessId())
                {
                    return true;
                }

                return false;
            }

            private static uint GetCurrentProcessId()
            {
                return (uint)System.Diagnostics.Process.GetCurrentProcess().Id;
            }
        }
    }
}
