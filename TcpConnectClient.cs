using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MusicBeePlugin
{
    internal class TcpConnectClient
    {
        public static void SendData(byte[] data, string ipAddress, int port)
        {
            using (TcpClient client = new TcpClient(ipAddress, port))
            using (NetworkStream stream = client.GetStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }
    }


}
