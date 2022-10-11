using System.Text;

namespace UT.Data.Modlet
{
    internal class Dataset
    {
        #region Properties
        public ModletCommands.Commands Command { get; set; }
        public byte[]? Data { get; set; }
        public byte[]? Module { get; set; }
        #endregion //Properties

        #region Constructors
        public Dataset() {  }

        public Dataset(ModletCommands.Commands command, IModlet? module) : this(command, null, module) { }

        public Dataset(ModletCommands.Commands command, byte[]? data, IModlet? module)
        {
            this.Command = command;
            this.Data = data;
            if (module != null)
            {
                string? type = module.GetType().AssemblyQualifiedName;

                this.Module = type == null ? null : ASCIIEncoding.UTF8.GetBytes(type);
            }
        }
        #endregion //Constructors
    }
}
