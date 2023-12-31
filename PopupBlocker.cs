using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicBeePlugin
{
    public class PopupBlocker
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventProcDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, CBTProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        private delegate IntPtr CBTProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static CBTProc cbtProcDelegate;

        private static IntPtr hookHandleCBT;

        private delegate void WinEventProcDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private static WinEventProcDelegate winEventProcDelegate; // Declare as a class field

        //private static GCHandle delegateHandle; // Declare as a class field

        private const uint WM_CLOSE = 0x0010;
        private const uint EVENT_OBJECT_CREATE = 0x8000;
        private const int WH_CBT = 5;
        private const int HCBT_CREATEWND = 3;

        private static Dictionary<uint, string> eventNames = new Dictionary<uint, string>
        {
            { 0x8000, "EVENT_OBJECT_CREATE" },
            { 0x0000, "EVENT_MIN" },
            { 0x7FFFFFFF, "EVENT_MAX" },
            { 0x00000008, "EVENT_SYSTEM_MENUSTART" },
            { 0x00000020, "EVENT_SYSTEM_MENUPOPUPSTART" },
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct CBT_CREATEWND
        {
            public IntPtr lpcs;    // Pointer to a CREATESTRUCT structure that contains information about the window being created.
            public IntPtr hwndInsertAfter;   // Handle to the window to precede the one being created.
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CREATESTRUCT
        {
            public IntPtr lpCreateParams; // Specifies additional data for use in creating the window.
            public IntPtr hInstance;       // Handle to the module that owns the new window.
            public IntPtr hMenu;           // Handle to the menu to be used by the new window.
            public IntPtr hwndParent;       // Handle to the parent window, if the window is a child window.
            public int cy;                 // Specifies the height of the new window, in pixels.
            public int cx;                 // Specifies the width of the new window, in pixels.
            public int y;                  // Specifies the y-coordinate of the upper left corner of the new window.
            public int x;                  // Specifies the x-coordinate of the upper left corner of the new window.
            public int style;              // Specifies the style of the new window.
            public IntPtr lpszName;        // Pointer to a null-terminated string that specifies the name of the new window.
            public IntPtr lpszClass;       // Pointer to a null-terminated string that specifies the class name of the new window.
            public int dwExStyle;          // Specifies the extended window style of the new window.
        }

        private static string GetWindowText(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }
        private static string GetWindowTitle(IntPtr hWnd)
        {
            const int nChars = 256;
            System.Text.StringBuilder sb = new System.Text.StringBuilder(nChars);
            GetWindowText(hWnd, sb, nChars);
            return sb.ToString();
        }
        private static void SuppressDialog(IntPtr hwnd)
        {
            // Send a WM_CLOSE message to the window to close it
            SendMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
        private static T ReadProcessMemory<T>(IntPtr processHandle, IntPtr baseAddress)
        {
            int size = Marshal.SizeOf<T>();
            byte[] buffer = new byte[size];

            ReadProcessMemory(processHandle, baseAddress, buffer, size, out _);

            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            T result = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());
            handle.Free();

            return result;
        }
        private static string ReadStringFromMemory(IntPtr processHandle, IntPtr address, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            List<byte> buffer = new List<byte>();
            byte[] temp = new byte[sizeof(char)];

            while (true)
            {
                ReadProcessMemory(processHandle, address, temp, temp.Length, out _);

                char ch = BitConverter.ToChar(temp, 0);
                if (ch == '\0') // Check for null terminator
                    break;

                buffer.AddRange(temp);
                address += sizeof(char);
            }

            // Remove null terminator from the end of the string
            buffer.RemoveAll(b => b == 0);

            return encoding.GetString(buffer.ToArray());
        }
        public static void InstallHooks(uint targetProcessId)
        {
            Process targetProcess = Process.GetProcessesByName("MusicBee")[0];
            uint targetThreadId = (uint)targetProcess.Threads[0].Id;

            //winEventProcDelegate = new WinEventProcDelegate(WinEventProc);

            //hookHandle = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_CREATE, IntPtr.Zero, winEventProcDelegate, targetProcessId, 0, 0);
            //if (hookHandle == IntPtr.Zero)
            //{
            //    Console.WriteLine($"SetWinEventHook failed with error code {Marshal.GetLastWin32Error()}");
            //}

            cbtProcDelegate = new CBTProc(CBTProcCallback);

            hookHandleCBT = SetWindowsHookEx(WH_CBT, cbtProcDelegate, IntPtr.Zero, targetThreadId);
            if (hookHandleCBT == IntPtr.Zero)
            {
                Console.WriteLine($"SetWindowsHookEx failed with error code {Marshal.GetLastWin32Error()}");
            }
        }
        private static IntPtr CBTProcCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == HCBT_CREATEWND)
            {
                
                // This is called when a window is about to be created
                // You can inspect the window attributes here and decide whether to proceed

                // Cast lParam to CBT_CREATEWND structure
                CBT_CREATEWND cbtCreateWnd = (CBT_CREATEWND)Marshal.PtrToStructure(lParam, typeof(CBT_CREATEWND));

                // Access window creation information
                string windowTitleFromStruct = "";

                CREATESTRUCT createStruct = ReadProcessMemory<CREATESTRUCT>(Process.GetCurrentProcess().Handle, cbtCreateWnd.lpcs);

                // Access window creation information
                windowTitleFromStruct = ReadStringFromMemory(Process.GetCurrentProcess().Handle, createStruct.lpszName);

                if (windowTitleFromStruct == "TimerNativeWindow")
                    return CallNextHookEx(hookHandleCBT, nCode, wParam, lParam);

                // Example: Prevent windows with "Error" in the title from being created
                string windowTitle = GetWindowTitle(wParam);

                Console.WriteLine($"Event received - Type: CBT Event: {nCode}, Title: {windowTitle}, TitleFromStruct: {windowTitleFromStruct}");

                if (windowTitle.ToLower().Contains("error") || windowTitleFromStruct.ToLower().Contains("error"))
                {
                    Console.WriteLine("Preventing window creation.");
                    return (IntPtr)1; // Prevent the window from being created
                }
            }

            return CallNextHookEx(hookHandleCBT, nCode, wParam, lParam);
        }
        //private static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        //{
        //    string eventName = GetEventName(eventType);

        //    if (eventType == EVENT_OBJECT_CREATE)
        //    {
        //        // Get the window title
        //        string windowTitle = GetWindowTitle(hwnd);
        //        string windowText = GetWindowText(hwnd);
        //        if ((windowTitle != "" || windowText != "") && (windowTitle != "TimerNativeWindow" && windowText != "TimerNativeWindow"))
        //        {
        //            Console.WriteLine($"Event received - Type: {eventName}, hwnd: {hwnd}, idObject: {idObject}, idChild: {idChild}");
        //            // Check if the window title contains "Error" (modify as needed)
        //            if (windowTitle.ToLower().Contains("error"))
        //            {
        //                // Optionally, prevent the window from being created
        //                Console.WriteLine("Preventing window creation.");
        //                // Uncomment the next line to prevent the window from being created
        //                SuppressDialog(hwnd);
        //                //return IntPtr.Zero;
        //            }
        //        }
        //    }
        //}
        //public static void UninstallHooks()
        //{
        //    if (hookHandle != IntPtr.Zero)
        //    {
        //        UnhookWinEvent(hookHandle);
        //        hookHandle = IntPtr.Zero;
        //    }
        //    if (hookHandleCBT != IntPtr.Zero)
        //    {
        //        UnhookWindowsHookEx(hookHandleCBT);
        //        hookHandleCBT = IntPtr.Zero;
        //    }
        //}
        public static void Prevent()
        {
            Process[] processes = Process.GetProcessesByName("MusicBee");

            if (processes.Length > 0)
            {
                uint targetProcessId = (uint)processes[0].Id;

                InstallHooks(targetProcessId);

                //// Keep the application running
                //Console.WriteLine("Press Enter to exit...");
                //Console.ReadLine();

                //// Uninstall the hook before exiting
                //UninstallHook();
            }
            else
            {
                Console.WriteLine("Process 'MusicBee' not found.");
            }
        }
    }
}
