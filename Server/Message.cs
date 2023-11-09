using System.Text;

namespace nsMessage
{
    public enum EnumMessageType
    {
        EndMessage,
        PartMessage,
        EndFile,
        PartFile
    }
    public partial class Message
    {
        public const int sizeServiceBytes = 64;   //Размер сервисной информации 64 Б
        public const int sizeMessageBytes = 1024;  //Пока макс. размер сообщения 1 КБ
        public const int sizeFullMessageBytes = sizeServiceBytes + sizeMessageBytes;
        private readonly EnumMessageType _messageType;
        public EnumMessageType MessageType { get => _messageType; }
        private readonly byte[] _bytesMessage = new byte[sizeMessageBytes];
        public byte[] BytesMessage { get => _bytesMessage; }
        public Message(byte[] bytes, EnumMessageType messageType)
        {
            bytes.CopyTo(_bytesMessage, sizeServiceBytes);
            _messageType = messageType;
        }

        public Message(byte[] bytes)
        {
            _messageType = (EnumMessageType)bytes[0];
            bytes.Skip(sizeServiceBytes).ToArray().CopyTo(_bytesMessage, 0);
        }
        static public IEnumerable<Message> CreateMessage(string message)
        {
            byte[] bytesMessage = Encoding.UTF8.GetBytes(message);
            int partCount = (int)Math.Ceiling(Encoding.UTF8.GetByteCount(message) / (double)sizeMessageBytes);
            for (int i = 0; i < partCount; i++)
            {
                yield return new Message(bytesMessage.Skip(sizeMessageBytes * i).ToArray(), (i == partCount - 1) ? EnumMessageType.EndMessage : EnumMessageType.PartMessage);
            }
        }
        static public IEnumerable<Message> CreateMessageAsync(FileInfo file)
        {
            int readedBytes = 1;
            using (FileStream fileStream = file.OpenRead())
            {
                while (readedBytes > 0)
                {
                    byte[] filesByte = new byte[sizeMessageBytes];
                    readedBytes = fileStream.ReadAsync(filesByte, 0, sizeMessageBytes).Result;
                    yield return new Message(filesByte.Take(readedBytes).ToArray(), (fileStream.Length == fileStream.Position) ? EnumMessageType.EndFile : EnumMessageType.PartFile);
                }
            }
        }
        public string GetText()
        {
            return Encoding.UTF8.GetString(_bytesMessage);
        }
        public byte[] GetBytes()
        {
            byte[] serviceBytes = new byte[sizeServiceBytes];
            serviceBytes[0] = (byte)MessageType;
            return serviceBytes.Concat(BytesMessage).ToArray();
        }
    }
}