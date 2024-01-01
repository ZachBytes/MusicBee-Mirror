using System;
using System.Windows.Forms;

namespace MusicBeePlugin
{
    public partial class MusicBeeMirror : Form
    {
        private Plugin.MusicBeeApiInterface mApi;
        private Plugin _musicBeeMirrorPlugin;
        public bool isMusicBeeProcessFound;
        public MusicBeeMirror(Plugin.MusicBeeApiInterface pApi, Plugin musicBeeMirrorPlugin)
        {
            _musicBeeMirrorPlugin = musicBeeMirrorPlugin;
            mApi = pApi;
            Shown += Form1_Shown;
            StartPosition = FormStartPosition.CenterScreen;

            InitializeComponent();
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
            //CommandTextbox.Visible = type == "server" && TcpServer.SocketThread.ThreadState == System.Threading.ThreadState.Running;
        }

        private void ListenButton_Click(object sender, EventArgs e)
        {
            int serverPort = Convert.ToInt32(PortTextbox.Text);

            if (TcpServer.SocketThread.ThreadState == System.Threading.ThreadState.Unstarted)
            {
                TcpServer tcpServer = new TcpServer(serverPort, _musicBeeMirrorPlugin);
                TcpServer.SocketThread.Start();
                CommandTextbox.Show();
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
            if (string.IsNullOrEmpty(ServerIPTextbox.Text))
            {
                MessageBox.Show("No IP Address detected!");
            }
            else
            {
                // Suppresses all Error window popups
                PopupBlocker.Prevent();
            }
        }
    }
}
