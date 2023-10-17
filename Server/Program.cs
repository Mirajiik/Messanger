using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Server.Status);
            Console.WriteLine("End program");
            Console.ReadLine();
        }
    }

    static public class Server
    {
        //static private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static private TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 8080);
        static private List<string> historyMessage = new();
        static Server()
        {
            tcpListener.Start();
            //serverSocket.Bind(new IPEndPoint(IPAddress.Loopback, 8080));
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CloseServer);
            Task taskAcceptingConnections = new Task(AcceptingConnections);
            taskAcceptingConnections.RunSynchronously();
        }

        static public string Status = "Create";

        static void CloseServer(object? sender, EventArgs e)
        {
            try
            {
                tcpListener.Stop();
                //serverSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            { 
                //serverSocket.Close();
            }
        }

        static void AcceptingConnections()
        {
            while (true)
            {
                TcpClient userSocket = tcpListener.AcceptTcpClient();
                Task taskUserConnect = new Task(() => UserConnect(userSocket));
                taskUserConnect.Start();
                Console.WriteLine(Task.CurrentId);
            }
        }

        static async void UserConnect(TcpClient client)
        {
            historyMessage.Add(client.)
        }
    }
}