using System.Net.Sockets;
using nsMessage;

namespace Server.NetworkStreamExtension
{
    public static class NetworkStreamExtension
    {
        public static void Write(this NetworkStream stream, Message message)
        {
            stream.Write(message.GetBytes());
        }

        public static async Task WriteAsync(this NetworkStream stream, Message message)
        {
            await stream.WriteAsync(message.GetBytes());
        }

        public static IEnumerable<Message> Read(this NetworkStream stream)
        {
            byte[] bytesRead;
            do
            {
                bytesRead = new byte[Message.sizeFullMessageBytes];
                stream.Read(bytesRead, 0, bytesRead.Length);
                yield return new Message(bytesRead.Skip(Message.sizeServiceBytes).ToArray(), (EnumMessageType)bytesRead[0]);
            } while (((EnumMessageType)bytesRead[0]) != EnumMessageType.EndMessage);
        }
    }
}