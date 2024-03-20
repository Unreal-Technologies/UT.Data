using System.Net;
using System.Net.Sockets;
using UT.Data.Extensions;

namespace UT.Data.IO
{
    public class Client(IPAddress ip, int port)
    {
        #region Members
        private readonly IPAddress ip = ip;
        private readonly int port = port;
        #endregion //Members

        #region Constructors
        public Client(int port) : this("127.0.0.1", port) { }

        public Client(string ip, int port) : this(IPAddress.Parse(ip), port) { }
        #endregion //Constructors

        #region Public Methods
        internal T? Send<T>(T obj)
            where T : class
        {
            return Serializer<T>.Deserialize(this.Send(Serializer<T>.Serialize(obj)));
        }

        internal byte[] Send(byte[] data)
        {
            TcpClient client = new();
            client.Connect(this.ip, this.port);
            Stream stream = client.GetStream();

            stream.Write(data, 0, data.Length);

            byte[] response = stream.Read();

            client.Close();

            return response;
        }
        #endregion //Public Methods
    }
}
