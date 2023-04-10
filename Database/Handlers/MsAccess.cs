using System.Data.OleDb;

namespace UT.Data.Database.Handlers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    internal class MsAccess : Manager
    {
        #region Members
        private readonly Dictionary<Filetypes, OleDbCommand> commands;
        #endregion //Members

        #region Constructors
        
        public MsAccess() : base()
        {
            this.commands = new Dictionary<Filetypes, OleDbCommand>();
        }
        #endregion //Constructors

        #region Internal Methods
        internal override void Connection(string connection, FileInfo? file, Filetypes filetype)
        {
            this.Connections.Add(new Tuple<Filetypes, FileInfo?, object>(filetype, file, new OleDbConnection(connection)));
        }

        internal override bool Close(Filetypes filetype)
        {
            OleDbConnection? con = (OleDbConnection?)this.Connections.Where(x => x.Item1 == filetype).FirstOrDefault()?.Item3;
            if (con == null)
            {
                return false;
            }
            con.Close();
            return true;
        }

        internal override bool Open(Filetypes filetype)
        {
            OleDbConnection? con = (OleDbConnection?)this.Connections.Where(x => x.Item1 == filetype).FirstOrDefault()?.Item3;
            if (con == null)
            {
                return false;
            }
            con.Open();
            return true;
        }
        #endregion //Internal Methods

        #region Public Methods
        public override bool Close()
        {
            return this.Close(Filetypes.Main);
        }

        public override bool Open()
        {
            return this.Open(Filetypes.Main);
        }
        #endregion //Public Methods
    }
}
