using Microsoft.EntityFrameworkCore;
using System.Net;
using UT.Data.Attributes;

namespace UT.Data
{
    [Default("Password", ""), Default("Username", "root"), Default("Port", "3306"), Default("Server", "127.0.0.1")]
    public abstract class ExtendedDbContext : DbContext
    {
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
        private Dictionary<Parameters, object> config;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //base.OnConfiguring(optionsBuilder);
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
    }
}
