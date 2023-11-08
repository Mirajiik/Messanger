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
        private Message(IEnumerable<byte> bytes)
        {
            _bytesMessage = bytes.Take(sizeMessageBytes).ToArray();
            if (bytes.Count() > sizeMessageBytes)
                _messageType = EnumMessageType.PartMessage;
            else
                _messageType = EnumMessageType.EndMessage;
        }
        public Message(byte[] bytes, EnumMessageType messageType)
        {
            
            _bytesMessage = bytes.ToArray();
            _messageType = messageType;
        }
        static public IEnumerable<Message> CreateMessage(string message)
        {
            byte[] bytesMessage = Encoding.UTF8.GetBytes(message);
            for (int i = 0; i < Math.Ceiling(Encoding.UTF8.GetByteCount(message) / (double)sizeMessageBytes); i++)
            {
                yield return new Message(bytesMessage.Skip(sizeMessageBytes * i));
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