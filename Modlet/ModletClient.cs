using System.Net;
using UT.Data.IO;

namespace UT.Data.Modlet
{
    public class ModletClient : Client
    {
        #region Properties
        public byte[]? Aes { get; set; }
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
            if(this.Aes != null)
            {
                //throw new NotImplementedException("Encode data to AES with KEY");
            }
            Dataset dsIn = new(command, data, module);
            Dataset? dsOut = base.Send<Dataset>(dsIn);
            if (dsOut == null)
            {
                return null;
            }
            return dsOut.Data;
        }

        public byte[]? Send(ModletCommands.Commands command, IModlet? module)
        {
            return this.Send(null, command, module);
        }
        #endregion //Public Methods
    }
}
