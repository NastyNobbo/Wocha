using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Program
    {
        public string ipAddress;
        public int port;
        private static List<TcpClient> _clients = new List<TcpClient>();

        static async Task Main(string[] args)
        {
            using (var pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.In))
            {
                Console.WriteLine("Waiting for connection...");
                await pipeServer.WaitForConnectionAsync();
                Console.WriteLine("Client connected.");

                byte[] serverIP = new byte[256];
                byte[] serverPort = new byte[256];
                int bytesRead = await pipeServer.ReadAsync(serverIP,0, serverIP.Length);
                ipAddress = Encoding.UTF8.GetString(serverIP, 0, bytesRead);
                int bytesRead2 = await pipeServer.ReadAsync(serverIP, 0, serverIP.Length);
                port = Encoding.UTF8.GetString(serverPort, 0, bytesRead);
                
            }
            TcpListener server = new TcpListener(IPAddress.Parse(ipAddress), port);
            server.Start();
            Console.WriteLine("Сервер запущен...");

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                _clients.Add(client);
                Console.WriteLine("Клиент подключен.");
                _ = Task.Run(() => HandleClient(client));
            }
        }

        private static async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Получено сообщение: {message}");
                        await BroadcastMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                _clients.Remove(client);
                client.Close();
            }
        }

        private static async Task BroadcastMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (var client in _clients)
            {
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }
        }
    }
}