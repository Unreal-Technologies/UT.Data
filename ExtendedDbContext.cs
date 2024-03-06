using Microsoft.EntityFrameworkCore;
using System.Net;
using UT.Data.Attributes;

namespace UT.Data
{
    [Default("Password", Password), Default("Username", Username), Default("Port", Port), Default("Server", Server)]
    public abstract class ExtendedDbContext : DbContext
    {
        #region Constants
        public const string Username = "root";
        public const string Password = "";
        public const int Port = 3306;
        public const string Server = "127.0.0.1";
        #endregion //Constants

        #region Enums
        public enum Types
        {
            Mysql
        }

        private enum Parameters
        {
            IP, Port, Username, Password, Database, Type
        }
        #endregion //Enums

        #region Members
        private readonly Dictionary<Parameters, object> config;
        #endregion //Members

        #region Constructors
        public ExtendedDbContext(IPAddress ip, int port, string username, string password, string db, Types type) : base()
        {
            this.config = [];
            this.config.Add(Parameters.IP, ip);
            this.config.Add(Parameters.Port, port);
            this.config.Add(Parameters.Username, username);
            this.config.Add(Parameters.Password, password);
            this.config.Add(Parameters.Database, db);
            this.config.Add(Parameters.Type, type);
        }
        #endregion //Constructors

        #region Overrides
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Types type = (Types)this.config[Parameters.Type];
            int port = (int)this.config[Parameters.Port];
            string username = (string)this.config[Parameters.Username];
            string password = (string)this.config[Parameters.Password];
            string database = (string)this.config[Parameters.Database];
            IPAddress ip = (IPAddress)this.config[Parameters.IP];

            switch (type)
            {
                case Types.Mysql:
                    optionsBuilder.UseMySQL("server=" + ip + ";port=" + port + ";database=" + database + ";user=" + username + ";password=" + password + ";");
                    break;
            }
        }
        #endregion //Overrides
    }
}
