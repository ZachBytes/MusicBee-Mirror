using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static MusicBeePlugin.Plugin;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Drawing;
using Newtonsoft.Json;

namespace MusicBeePlugin
{
    public class TcpServer
    {
        static int _port;
        static Plugin _MusicBeeMirrorPlugin;
        public TcpServer(int port, Plugin MusicBeeMirrorPlugin)
        {
            _MusicBeeMirrorPlugin = MusicBeeMirrorPlugin;
            _port = port;
        }
        public static Thread SocketThread = new Thread(() => StartTcpServer());

        //Need this for deserialization to work
        private static Assembly MyTypeResolveEventHandler(object sender, ResolveEventArgs args)
        {
            return typeof(NotificationType).Assembly;
        }

        public static void StartTcpServer()
        {
            int portInt = Convert.ToInt32(_port);

            TcpListener server = new TcpListener(IPAddress.Any, portInt);
            server.Start();

            myForm.Invoke(new Action(() =>
            {
                myForm.CommandTextbox.Text += $"TcpServer listening on port {portInt} {Environment.NewLine}";
            }));

            while (true)
            {

                using (TcpClient client = server.AcceptTcpClient())
                using (NetworkStream networkStream = client.GetStream())
                {
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int bytesRead = networkStream.Read(buffer, 0, client.ReceiveBufferSize);

                    //Need this for deserialization to work
                    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(MyTypeResolveEventHandler);

                    // Deserialize fileUrl and type
                    //string sourceFileUrl;
                    //NotificationType type = new NotificationType();
                    
                    //using (MemoryStream memoryStream = new MemoryStream(buffer))
                    //{
                    //    BinaryFormatter formatter = new BinaryFormatter();
                    //    type = (NotificationType)formatter.Deserialize(memoryStream);
                    //    sourceFileUrl = (string)formatter.Deserialize(memoryStream);
                    //}

                    // Deserialize JSON data
                    string jsonData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    NotificationData notificationData = JsonConvert.DeserializeObject<NotificationData>(jsonData);

                    if (notificationData != null)
                    {
                        // Use the deserialized data
                        _MusicBeeMirrorPlugin.ReceiveNotification(notificationData.SourceFileUrl, notificationData.Type);
                    }
                    else
                    {
                        // Handle the case where deserialization failed or data is not in the expected format
                        Console.WriteLine("Received invalid or null notification data.");
                    }
                }

            }
        }
    }
}
