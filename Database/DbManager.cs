using UT.Data.Database.Handlers;

namespace UT.Data.Database
{
    public abstract class DbManager
    {
        #region Properties
        private bool exists;
        private readonly List<Tuple<Filetypes, FileInfo?, object>> connections;
        #endregion //Properties

        #region Properties
        public bool Exists
        {
            get { return this.exists; }
            internal set { this.exists = value; }
        }

        internal List<Tuple<Filetypes, FileInfo?, object>> Connections
        {
            get { return this.connections; }
        }
        #endregion //Properties

        #region Enums
        internal enum Filetypes
        {
            Main, Temp
        }
        #endregion //Enums

        #region Constructors
        public DbManager()
        {
            this.exists = false;
            this.connections = new List<Tuple<Filetypes, FileInfo?, object>>();
        }
        #endregion //Constructors

        #region Abstracts
        #region Internal
        internal abstract void Connection(string connection, FileInfo? file, Filetypes filetype);
        internal abstract bool Open(Filetypes filetype);
        internal abstract bool Close(Filetypes filetype);
        internal abstract Tuple<int, Dictionary<string, object?>[]?>? Execute(string command, Filetypes filetype);
        #endregion //Internal

        #region Public
        public abstract bool Open();
        public abstract bool Close();
        public abstract Tuple<int, Dictionary<string, object?>[]?>? Execute(string command);
        #endregion //Public
        #endregion //Abstracts

        #region Public Methods
        public static DbManager MySql(string server, string database, string username = "root", string password = "", int port = 3306)
        {
            string cs = "server=" + server + ";user=" + username + ";database=[DB];port=" + port  + (password == String.Empty ? "" : ";password=" + password);
            FileInfo fi = new FileInfo(Path.GetTempFileName());

            DbManager manager = new UT.Data.Database.Handlers.MySql()
            {

            };
            manager.Connection(cs.Replace("[DB]", database), null, Filetypes.Main);
            manager.Connection(cs.Replace("[DB]", fi.Name), null, Filetypes.Temp);

            return manager;
        }

        //public static Manager MsAccess(FileInfo file)
        //{
        //    string temp = Manager.GetTempName(file);

        //    Manager manager = new MsAccess
        //    {
        //        Exists = File.Exists(file.FullName)
        //    };
        //    manager.Connection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file.FullName, file, Filetypes.Main);
        //    manager.Connection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Manager.GetTempName(file), new FileInfo(temp), Filetypes.Temp);

        //    return manager;
        //}
        #endregion //Public Methods

        #region Private Methods
        //private static string GetTempName(FileInfo file)
        //{
        //    return Path.GetTempFileName() + "." + file.Extension;
        //}
        #endregion //Private Methods
    }
}
