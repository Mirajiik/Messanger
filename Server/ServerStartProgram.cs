using System.IO;
using System.Net;
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
            Console.WriteLine("End program");
            Console.ReadLine();
        }
    }

    static public class Server
    {
        static private TcpListener tcpListener;
        static private FileStream fileStream = new FileStream("HistoryMessage.txt", FileMode.Append);
        static private bool ServerIsRunning;
        static Server()
        {
            Console.WriteLine("Server Start");
            try
            {
                tcpListener = new TcpListener(IPAddress.Loopback, 8080);
                tcpListener.Start();
                ServerIsRunning = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                fileStream?.Close();
                throw;
            }
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CloseServer);
            Task taskAcceptingConnections = new Task(AcceptingConnections);
            taskAcceptingConnections.RunSynchronously();
        }

        static public void Start() { }

        static void CloseServer(object? sender, EventArgs e)
        {
            Console.WriteLine("END PROGRAM");
            Console.ReadLine();
            ServerIsRunning = false;
            try
            {
                tcpListener.Stop();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async void AcceptingConnections()
        {
            while (true)
            {
                Console.WriteLine("Ожидание пользователя");
                TcpClient userSocket = await tcpListener.AcceptTcpClientAsync();
                Task taskUserConnect = UserConnect(userSocket);
                await taskUserConnect;
                /*Task taskUserConnect = Task.Run(() => UserConnect(userSocket));
                Console.WriteLine(Task.CurrentId);
                //await taskUserConnect;
                //taskUserConnect.Start();
                Console.WriteLine(taskUserConnect.Status);*/
            }
        }

        static async Task UserConnect(TcpClient client)
        {
            Console.WriteLine("Start Task");
            using var stream = client.GetStream();
            {
                while (ServerIsRunning)
                {
                    Console.WriteLine("Wait message");
                    foreach (var item in stream.Read())
                    {
                        Console.WriteLine(item.GetText());
                        await fileStream.WriteAsync(item.BytesMessage);
                        Console.WriteLine("End Writing");
                    }
                }
            }
        }
    }
}