using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using nsMessage;
using Server.NetworkStreamExtension;

namespace Server
{
    internal class ServerStartProgram
    {
        static void Main(string[] args)
        {
            Server.Start();
            Console.WriteLine("Enter for disconnect");
            Console.ReadLine();
        }
    }

    static public class Server
    {
        static private TcpListener tcpListener;
        static private List<TcpClient> clients = new List<TcpClient>();
        static private FileStream fileStream = new FileStream("HistoryMessage.txt", FileMode.Append);
        static Server()
        {
            Console.WriteLine("Server Start");
            try
            {
                tcpListener = new TcpListener(IPAddress.Loopback, 8080);
                tcpListener.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                fileStream?.Close();
                throw;
            }
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CloseServer);
            AcceptingConnections();
        }

        static public void Start() { }

        static private void CloseServer(object? sender, EventArgs e)
        {
           Console.WriteLine("END PROGRAM");
            try
            {
                foreach (var client in clients)
                {
                    client.Close();
                }
                tcpListener.Stop();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static private async void AcceptingConnections()
        {
            int i = 0;
            while (true)
            {
                TcpClient userSocket = await tcpListener.AcceptTcpClientAsync();
                clients.Add(userSocket);
                Task.Run(async () => await UserConnectAsync(userSocket));
                Console.WriteLine($"{i++} запущен");
            }
        }

        static private async Task UserConnectAsync(TcpClient client)
        {
            Console.WriteLine("Start Task");
            using var stream = client.GetStream();
            {
                while (client.Connected)
                {
                    Console.WriteLine("Wait message");
                    foreach (var item in stream.Read())
                    {
                        Console.WriteLine(item.GetText());
                        fileStream.Write(item.BytesMessage);
                        Task.Run(async() => await BroadcastMessageAsync(item, client));
                        Console.WriteLine("Messages broadcasing");
                    }
                }
            }
            Console.WriteLine("Close connect");
            RemoveConnection(client);
        }

        static private async Task BroadcastMessageAsync(Message message, TcpClient sourceClient)
        {
            foreach (var client in clients)
            {
                if (client != sourceClient)
                {
                    await client.GetStream().WriteAsync(message);
                    await client.GetStream().FlushAsync();
                }
            }
        }

        static private void RemoveConnection(TcpClient client)
        {
            clients.Remove(client);
            client?.Close();
        }
    }
}