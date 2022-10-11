using System.Net;
using UT.Data.IO;
using System.Text;

namespace UT.Data.Modlet
{
    public class ModletServer : Server
    {
        #region Members
        private Dictionary<string, string>? keys;
        private List<IModlet>? modules;
        private int semiRand;
        #endregion //Members

        #region Constructors
        public ModletServer(string[] ip, int port) : base(ip, port)
        {
            this.ConstructExtension();
        }

        public ModletServer(string[] ip, int[] ports) : base(ip, ports)
        {
            this.ConstructExtension();
        }

        public ModletServer(IPAddress[] ip, int port) : base(ip, port)
        {
            this.ConstructExtension();
        }

        public ModletServer(IPAddress[] ip, int[] ports) : base(ip, ports)
        {
            this.ConstructExtension();
        }
        #endregion //Constructors

        #region Public Methods
        public bool Register(IModlet module)
        {
            if(this.modules == null)
            {
                return false;
            }
            this.modules.Add(module);
            return true;
        }
        #endregion //Public Methods

        #region Private Methods
        private void ConstructExtension()
        {
            Random rnd = new((int)DateTime.Now.Ticks);

            this.keys = new Dictionary<string, string>();
            this.semiRand = rnd.Next(0xffff);
            this.modules = new List<IModlet>();
            this.OnDataReceived += ModletServer_OnDataReceived;
        }

        private byte[] ModletServer_OnDataReceived(byte[] data, EndPoint? ep, Server server)
        {
            Dataset? dsIn = Serializer<Dataset>.Deserialize(data);
            if (dsIn == null || this.keys == null || ep == null || this.modules == null)
            {
                return Array.Empty<byte>();
            }
            string lockKey = ep.ToString()?.Split(':')[0] ?? string.Empty;

            Dataset dsOut;
            switch (dsIn.Command)
            {
                case ModletCommands.Commands.Connect:
                    dsOut = new Dataset(ModletCommands.Commands.Response, Array.Empty<byte>(), null);
                    break;
                case ModletCommands.Commands.Serverkey:
                    if (dsIn.Data == null)
                    {
                        return Array.Empty<byte>();
                    }

                    string computer = ASCIIEncoding.UTF8.GetString(dsIn.Data);
                    string key;

                    if (this.keys.ContainsKey(lockKey))
                    {
                        key = this.keys[lockKey];
                    }
                    else
                    {
                        string unique = Hashing.Guid(ep.ToString() + "/" + computer + "/" + this.semiRand).ToString();
                        this.keys.Add(lockKey, unique);
                        key = unique;
                        ExtendedConsole.WriteLine("Registered key: <yellow>" + unique + "</yellow> for <cyan>" + lockKey + "</cyan>");
                    }
                    dsOut = new Dataset(ModletCommands.Commands.Response, ASCIIEncoding.UTF8.GetBytes(key), null);
                    break;
                case ModletCommands.Commands.Action:
                    byte[]? module = dsIn.Module;
                    byte[]? stream = dsIn.Data;
                    byte[]? output = null;
                    Type? type = null;
                    if(module != null)
                    {
                        type = Type.GetType(ASCIIEncoding.UTF8.GetString(module));
                    }

                    if(type == null)
                    {
                        foreach(IModlet m in this.modules)
                        {
                            m.OnGlobalServerAction(stream);
                        }
                    }
                    else
                    {
                        IModlet? m = this.modules.Where(x => x.GetType().AssemblyQualifiedName == type.AssemblyQualifiedName).FirstOrDefault();
                        if(m != null)
                        {
                            output = m.OnLocalServerAction(stream);
                        }
                    }

                    dsOut = new Dataset(ModletCommands.Commands.Response, output, null);
                    break;
                default:
                    throw new NotImplementedException();
            }

            if(dsOut == null)
            {
                return Array.Empty<byte>();
            }
            return Serializer<Dataset>.Serialize(dsOut);
        }
        #endregion //Prviate Methods
    }
}
