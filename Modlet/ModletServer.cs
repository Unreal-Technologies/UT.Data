using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using UT.Data.Efc;
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
        public IModlet[] Modules { get { return [.. modules]; } }
        #endregion //Properties

        #region Constructors
        public ModletServer(string[] ip, int port) : base(ip, port)
        {
            ConstructExtension();
        }

        public ModletServer(string[] ip, int[] ports) : base(ip, ports)
        {
            ConstructExtension();
        }

        public ModletServer(IPAddress[] ip, int port) : base(ip, port)
        {
            ConstructExtension();
        }

        public ModletServer(IPAddress[] ip, int[] ports) : base(ip, ports)
        {
            ConstructExtension();
        }
        #endregion //Constructors

        #region Public Methods
        public bool Register(IModlet module, ServerContext? context)
        {
            if(modules == null)
            {
                return false;
            }
            module.OnServerConfiguration(context);
            modules.Add(module);
            return true;
        }
        #endregion //Public Methods

        #region Private Methods
        private void ConstructExtension()
        {
            Random rnd = new((int)DateTime.Now.Ticks);

            keys = [];
            semiRand = rnd.Next(0xffff);
            modules = [];
            OnDataReceived += ModletServer_OnDataReceived;
        }

        private byte[] ModletServer_OnDataReceived(byte[] data, EndPoint? ep, Server server)
        {
            Dataset? dsIn = Serializer<Dataset>.Deserialize(data);
            if (dsIn == null || keys == null || ep == null || modules == null)
            {
                return [];
            }
            string lockKey = ep.ToString()?.Split(':')[0] ?? string.Empty;

            Dataset? dsOut = null;
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
                        string unique = Hashing.Guid(ep.ToString() + "/" + computer + "/" + semiRand).ToString();
                        keys.Add(lockKey, unique);
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

                    string aeskey = keys[lockKey];
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
                        foreach(IModlet m in modules)
                        {
                            m.OnGlobalServerAction(stream, ip);
                        }
                    }
                    else
                    {
                        IModlet? m = modules.Find(x => x.GetType().AssemblyQualifiedName == type.AssemblyQualifiedName);
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

            return Serializer<Dataset>.Serialize(dsOut);
        }
        #endregion //Prviate Methods
    }
}
