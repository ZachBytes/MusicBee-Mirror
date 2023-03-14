using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MusicBeePlugin.Plugin;
using static MusicBeePlugin.Utils;

namespace MusicBeePlugin
{
    public partial class MusicBeeMirror : Form
    {
        private Plugin.MusicBeeApiInterface mApi;
        private Plugin _musicBeeMirrorPlugin;
        public IntPtr hWnd;
        public uint pID;
        public bool isMusicBeeProcessFound;

        // Constants used for hook installation
        private const int WH_CALLWNDPROC = 4;
        private const int HC_ACTION = 0;

        // Delegate used for hook procedure
        private delegate IntPtr HookProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        // Import the WinAPI functions
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProcDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // WinAPI structures used in the hook procedure
        [StructLayout(LayoutKind.Sequential)]
        private struct CWPSTRUCT
        {
            public IntPtr lParam;
            public IntPtr wParam;
            public int message;
            public IntPtr hwnd;
        }

        private const int WM_CREATE = 0x0001;
        private const int WM_INITDIALOG = 0x0110;

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Hook procedure that intercepts window messages
        private IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == HC_ACTION)
            {
                var msg = Marshal.PtrToStructure<CWPSTRUCT>(lParam);

                Debug.WriteLine("Message ID: " + msg.message);

                if (msg.message == WM_CREATE || msg.message == WM_INITDIALOG)
                {
                    // Prevent the creation of the window
                    return IntPtr.Zero;
                }
            }

            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        public MusicBeeMirror(Plugin.MusicBeeApiInterface pApi, Plugin musicBeeMirrorPlugin)
        {
            _musicBeeMirrorPlugin = musicBeeMirrorPlugin;
            mApi = pApi;
            Shown += Form1_Shown;
            StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();

            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                if (process.ProcessName == "MusicBee")
                {
                    isMusicBeeProcessFound = true;
                    hWnd = process.Handle;
                    pID = (uint)process.Id;
                    break;
                }
            }
            if (!isMusicBeeProcessFound)
            {
                MessageBox.Show("Cannot find a running instance of MusicBee!");
                return;
            }

            //FormHelper.PreventChildWindowsFromShowing(hWnd);

            // Get the HWND of the process you want to hook
            //var hWnd = FindWindow(null, "MusicBee");

            //IntPtr hWnd = pApi.MB_GetWindowHandle();

            // Get the thread ID of the window with the given HWND
            uint threadId;
            GetWindowThreadProcessId(hWnd, out threadId);

            // Install the hook procedure
            var hookProc = new HookProcDelegate(HookProc);
            var hook = SetWindowsHookEx(WH_CALLWNDPROC, hookProc, IntPtr.Zero, threadId);

            if (hook == IntPtr.Zero)
            {
                var errorCode = Marshal.GetLastWin32Error();
                MessageBox.Show("Failed to install hook: " + errorCode);
            }

            // Don't forget to unhook the hook when you're done with it
            // UnhookWindowsHookEx(hook);
        }



        private void Form1_Shown(object sender, EventArgs e)
        {

            if (TcpServer.SocketThread.ThreadState == System.Threading.ThreadState.Running)
            {
                ToggleControls("server");
                serverButton.Checked = true;
            }
            else
            {
                serverButton.Checked = false;
                ToggleControls(null);
            }
        }

        private string getPlayerState()
        {
            return mApi.Player_GetPlayState().ToString();
        }

        private void serverButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                clientButton.Checked = false;
            }

            ToggleControls("server");
            PortTextbox.Text = "1234";
        }

        private void clientButton_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                serverButton.Checked = false;
            }

            ToggleControls("client");
            PortTextbox.Text = "1234";
        }

        void ToggleControls(string type)
        {
            ListenButton.Visible = type == "server";
            ServerIPLabel.Visible = type == "client";
            ServerIPTextbox.Visible = type == "client";
            ConnectButton.Visible = type == "client";
            PortLabel.Visible = type == "client" || type == "server";
            PortTextbox.Visible = type == "client" || type == "server";
            ListenLabel.Visible = type == "server" && TcpServer.SocketThread.ThreadState == System.Threading.ThreadState.Running;
            CommandTextbox.Visible = type == "server" && TcpServer.SocketThread.ThreadState == System.Threading.ThreadState.Running;
        }

        private void ListenButton_Click(object sender, EventArgs e)
        {
            int serverPort = Convert.ToInt32(PortTextbox.Text);

            if (TcpServer.SocketThread.ThreadState == System.Threading.ThreadState.Unstarted)
            {
                TcpServer tcpServer = new TcpServer(serverPort, _musicBeeMirrorPlugin);
                TcpServer.SocketThread.Start();
            }
            else
            {
                MessageBox.Show("Server is already listening!");
            }

            ToggleControls("server");
            //Visible = false;
        }
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            //Visible = false;
        }
    }
}
