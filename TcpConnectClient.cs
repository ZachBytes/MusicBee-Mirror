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
    using System.Threading;
    using System.Threading.Tasks;

    internal class TcpConnectClient
    {
        private const int MaxReconnectionAttempts = 3;
        public static async Task SendDataAsync(byte[] data, string ipAddress, int port)
        {
            int attempt = 0;
            //using (TcpClient client = new TcpClient(ipAddress, port))
            //using (NetworkStream stream = client.GetStream())
            //{
            //    stream.Write(data, 0, data.Length);
            //}
            while (attempt < MaxReconnectionAttempts)
            {
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
                            if (client.Connected)
                            {
                                // Connection succeeded
                                using (NetworkStream stream = client.GetStream())
                                {
                                    await stream.WriteAsync(data, 0, data.Length);
                                }
                            }
                            else
                            {
                                Console.WriteLine("TcpClient not connected");
                                System.Windows.Forms.MessageBox.Show("TcpClient not connected");
                            }
                        }
                        else
                        {
                            // Handle timeout
                            Console.WriteLine("TcpClient connection timeout");
                            System.Windows.Forms.MessageBox.Show("TcpClient connection timeout");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception caught: {ex.Message}");
                    }
                }

                // Wait for a short duration before attempting reconnection
                Thread.Sleep(1000); // Adjust the sleep duration as needed
                attempt++;
            }
        }
    }
}
