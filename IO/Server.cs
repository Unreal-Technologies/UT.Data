using System.Net;
using System.Net.Sockets;
using UT.Data.Extensions;

namespace UT.Data.IO
{
    public class Server
    {
        #region Members
        private bool _running = false;
        private bool _isActive = false;
        #endregion //Members

        private class ThreadData
        {
            public IPAddress? Ip { get; set; }
            public int Port { get; set; }
        }

        #region Constructors
        public Server(string[] ip, int port) : this(Server.Parse(ip), port) { }

        public Server(string[] ip, int[] ports) : this(Server.Parse(ip), ports) { }

        public Server(IPAddress[] ip, int port) : this(ip, new[] { port }) { }

        public Server(IPAddress[] ip, int[] ports)
        {
            this._isActive = true;
            DirectoryInfo dir = new("Logging");
            if(!dir.Exists)
            {
                dir.Create();
            }

            foreach (IPAddress v in ip)
            {
                foreach (int port in ports)
                {
                    Thread t = new(new ParameterizedThreadStart(this.InitializerThread));
                    t.Start(new ThreadData()
                    {
                        Ip = v,
                        Port = port
                    }); ;
                }
            }
        }
        #endregion //Constructors

        #region Properties
        public bool Logging { get; set; }
        #endregion //Properties

        #region Events
        public event DataProcessingHandle? OnDataReceived;
        #endregion //Events

        #region Delegates
        public delegate byte[] DataProcessingHandle(byte[] data, EndPoint? ep, Server server);
        #endregion //Delegate

        #region Public Methods
        public void Start()
        {
            this._running = true;
        }

        public void Stop()
        {
            this._running = false;
            this._isActive = false;
        }
        #endregion //Public Methods

        #region Private Methods

        private static IPAddress[] Parse(string[] ip)
        {
            List<IPAddress> list = new();
            foreach (string v in ip)
            {
                if (IPAddress.TryParse(v, out IPAddress? parsed))
                {
                    list.Add(parsed);
                }
            }

            return list.ToArray();
        }

        private void InitializerThread(object? data)
        {
            if(data == null)
            {
                return;
            }
            ThreadData td = (ThreadData)data;
            if(td.Ip == null)
            {
                return;
            }

            TcpListener listener = new(td.Ip, td.Port);
            listener.Start();

            while (this._isActive)
            {
                while (!this._running) { Thread.Sleep(1); }
                while (this._running)
                {
                    Socket s = listener.AcceptSocket();
                    Thread t = new(new ParameterizedThreadStart(this.SocketThread));
                    t.Start(s);
                    Thread.Sleep(25);
                }
            }
            listener.Stop();
        }

        private void SocketThread(object? data)
        {
            if(data == null)
            {
                return;
            }
            Socket s = (Socket)data;

            if(this.OnDataReceived == null)
            {
                s.Close();
                return;
            }

            byte[] bIn = s.Read();
            byte[] bOut = this.OnDataReceived.Invoke(bIn, s.RemoteEndPoint, this);
            if(this.Logging)
            {
                FileStream fs = new("Logging/" + DateTime.Now.ToString("yyyyMMdd-HHmmss.fffffff") + ".bin", FileMode.Create, FileAccess.Write);
                fs.Write(bIn);
                fs.Write("\r\n\r\n".AsBytes());
                fs.Write(bOut);

                fs.Close();
            }

            s.Send(bOut);
            s.Close();
        }
        #endregion //Private Methods
    }
}
