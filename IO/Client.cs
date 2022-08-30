using System.Net;
using System.Net.Sockets;
using UT.Data.Extensions;

namespace UT.Data.IO
{
    public class Client
    {
        #region Members
        private readonly IPAddress ip;
        private readonly int port;
        #endregion //Members

        #region Constructors
        public Client(int port) : this("127.0.0.1", port) { }

        public Client(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        public Client(string ip, int port) : this(IPAddress.Parse(ip), port) { }
        #endregion //Constructors

        #region Public Methods
        public byte[] Send(byte[] data)
        {
            TcpClient client = new TcpClient();
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
