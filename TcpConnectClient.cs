using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MusicBeePlugin
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    internal class TcpConnectClient
    {
        public static async Task SendData(byte[] data, string ipAddress, int port)
        {
            //using (TcpClient client = new TcpClient(ipAddress, port))
            //using (NetworkStream stream = client.GetStream())
            //{
            //    stream.Write(data, 0, data.Length);
            //}

            using (TcpClient client = new TcpClient())
            {
                try
                {
                    Task connectTask = client.ConnectAsync(ipAddress, port);
                    Task delayTask = Task.Delay(5000); // Set a timeout of 5000 milliseconds

                    // Wait for either the connection or the timeout
                    await Task.WhenAny(connectTask, delayTask);

                    if (connectTask.IsCompleted)
                    {
                        // Connection succeeded
                        using (NetworkStream stream = client.GetStream())
                        {
                            await stream.WriteAsync(data, 0, data.Length);
                        }
                    }
                    else
                    {
                        // Handle timeout
                        Console.WriteLine("Connection timeout");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception caught: {ex.Message}");
                }
            }
        }
    }
}
