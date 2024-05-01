using System.Net;
using UT.Data.IO;

namespace UT.Data.Modlet
{
    public class ModletClient : Client
    {
        #region Properties
        public string? Aes { get; set; }
        #endregion //Properties

        #region Constructors
        public ModletClient(string ip, int port): base(ip, port)
        {

        }

        public ModletClient(IPAddress ip, int port) : base(ip, port)
        {
        }

        public ModletClient(int port) : base(port)
        {

        }
        #endregion //Constructors

        #region Public Methods
        public byte[]? Send(byte[]? data, ModletCommands.Commands command, IModlet? module)
        {
            if(data != null && Aes != null)
            {
                data = Encryption.Aes.Encrypt(data, Aes);
            }
            Dataset dsIn = new(command, data, module);
            Dataset? dsOut = base.Send<Dataset>(dsIn);
            if (dsOut == null)
            {
                return null;
            }
            byte[]? output = dsOut.Data;
            if(output != null && Aes != null)
            {
                output = Encryption.Aes.Decrypt(output, Aes);
            }
            return output;
        }

        public byte[]? Send(ModletCommands.Commands command, IModlet? module)
        {
            return Send(null, command, module);
        }
        #endregion //Public Methods
    }
}
