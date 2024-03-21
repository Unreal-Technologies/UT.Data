using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using UT.Data.Encryption;
using UT.Data.IO;

namespace UT.Data.Modlet
{
    public class ModletServer : Server
    {
        #region Members
        private Dictionary<string, string>? keys;
        private List<IModlet>? modules;
        private int semiRand;
        #endregion //Members

        #region Properties
        public IModlet[] Modules { get { return [.. this.modules]; } }
        #endregion //Properties

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
        public bool Register(IModlet module, DbContext? context, ref Dictionary<string, object?> configuration)
        {
            if(this.modules == null)
            {
                return false;
            }
            module.OnServerConfiguration(context, ref configuration);
            this.modules.Add(module);
            return true;
        }
        #endregion //Public Methods

        #region Private Methods
        private void ConstructExtension()
        {
            Random rnd = new((int)DateTime.Now.Ticks);

            this.keys = [];
            this.semiRand = rnd.Next(0xffff);
            this.modules = [];
            this.OnDataReceived += ModletServer_OnDataReceived;
        }

        private byte[] ModletServer_OnDataReceived(byte[] data, EndPoint? ep, Server server)
        {
            Dataset? dsIn = Serializer<Dataset>.Deserialize(data);
            if (dsIn == null || this.keys == null || ep == null || this.modules == null)
            {
                return [];
            }
            string lockKey = ep.ToString()?.Split(':')[0] ?? string.Empty;

            Dataset dsOut;
            switch (dsIn.Command)
            {
                case ModletCommands.Commands.Connect:
                    dsOut = new Dataset(ModletCommands.Commands.Response, [], null);
                    break;
                case ModletCommands.Commands.Serverkey:
                    if (dsIn.Data == null)
                    {
                        return [];
                    }

                    string computer = ASCIIEncoding.UTF8.GetString(dsIn.Data);
                    string key;

                    if (keys.TryGetValue(lockKey, out string? value))
                    {
                        key = value;
                    }
                    else
                    {
                        string unique = Hashing.Guid(ep.ToString() + "/" + computer + "/" + this.semiRand).ToString();
                        this.keys.Add(lockKey, unique);
                        key = unique;
                        ExtendedConsole.WriteLine(string.Format("Registered key <yellow>{0}</yellow> for <cyan>{1}</cyan>", unique, lockKey));
                    }
                    dsOut = new Dataset(ModletCommands.Commands.Response, ASCIIEncoding.UTF8.GetBytes(key), null);
                    break;
                case ModletCommands.Commands.Action:
                    byte[]? module = dsIn.Module;
                    byte[]? stream = dsIn.Data;
                    IPAddress? ip = (ep as IPEndPoint)?.Address;
                    if(ip == null)
                    {
                        return [];
                    }

                    string aeskey = this.keys[lockKey];
                    if(stream != null)
                    {
                        stream = Aes.Decrypt(stream, aeskey);
                    }
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
                            m.OnGlobalServerAction(stream, ip);
                        }
                    }
                    else
                    {
                        IModlet? m = this.modules.Where(x => x.GetType().AssemblyQualifiedName == type.AssemblyQualifiedName).FirstOrDefault();
                        if(m != null)
                        {
                            output = m.OnLocalServerAction(stream, ip);
                        }
                    }

                    if(output != null)
                    {
                        output = Aes.Encrypt(output, aeskey);
                    }
                    dsOut = new Dataset(ModletCommands.Commands.Response, output, null);
                    break;
                default:
                    throw new NotImplementedException();
            }

            if(dsOut == null)
            {
                return [];
            }
            return Serializer<Dataset>.Serialize(dsOut);
        }
        #endregion //Prviate Methods
    }
}
