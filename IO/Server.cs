using System.Net;
using System.Net.Sockets;
using UT.Data.Extensions;

namespace UT.Data.IO
{
    public class Server
    {
        #region Members
        private readonly State state;
        #endregion //Members

        #region Classes
        private sealed class State
        {
            public bool IsRunning { get; set; }
            public bool IsActive { get; set; }
            public bool IsStopped { get { return !IsRunning && IsActive; } }
            public bool IsListening { get { return IsRunning && IsActive; } }
        }

        private sealed class ThreadData
        {
            public IPAddress? Ip { get; set; }
            public int Port { get; set; }
        }
        #endregion //Classes

        #region Constructors
        public Server(string[] ip, int port) : this(Server.Parse(ip), port) { }

        public Server(string[] ip, int[] ports) : this(Server.Parse(ip), ports) { }

        public Server(IPAddress[] ip, int port) : this(ip, [port]) { }

        public Server(IPAddress[] ip, int[] ports)
        {
            state = new State
            {
                IsActive = true
            };

            DirectoryInfo dir = new("Logging");
            if(!dir.Exists)
            {
                dir.Create();
            }

            foreach (IPAddress v in ip)
            {
                foreach (int port in ports)
                {
                    Thread t = new(new ParameterizedThreadStart(InitializerThread));
                    t.Start(new ThreadData()
                    {
                        Ip = v,
                        Port = port
                    });
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
            state.IsRunning = true;
        }

        public void Stop()
        {
            state.IsRunning = false;
            state.IsActive = false;
        }
        #endregion //Public Methods

        #region Private Methods

        private static IPAddress[] Parse(string[] ip)
        {
            List<IPAddress> list = [];
            foreach (string v in ip)
            {
                if (IPAddress.TryParse(v, out IPAddress? parsed))
                {
                    list.Add(parsed);
                }
            }

            return [.. list];
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

            while (state.IsActive)
            {
                while (state.IsStopped) { Thread.Sleep(1); }
                while (state.IsListening)
                {
                    while(!listener.Pending() && state.IsActive)
                    {
                        Thread.Sleep(1);
                    }
                    if (state.IsActive)
                    {
                        Socket s = listener.AcceptSocket();
                        Thread t = new(new ParameterizedThreadStart(SocketThread));
                        t.Start(s);
                        Thread.Sleep(1);
                    }
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

            if(OnDataReceived == null)
            {
                s.Close();
                return;
            }

            byte[] bIn = s.Read();
            byte[] bOut = OnDataReceived.Invoke(bIn, s.RemoteEndPoint, this);
            if(Logging)
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
