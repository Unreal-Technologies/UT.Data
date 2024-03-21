using Microsoft.EntityFrameworkCore;
using System.Net;

namespace UT.Data
{
    public abstract class ExtendedDbContext(ExtendedDbContext.Configuration configuration) : DbContext()
    {
        #region Classes
        public class Configuration(string connectionString, ExtendedDbContext.Types type)
        {
            #region Members
            private readonly string connectionString = connectionString;
            private readonly Types type = type;
            #endregion //Members

            #region Properties
            public string ConnectionString { get { return this.connectionString; } }
            public Types Type { get { return this.type; } }
            #endregion //Properties
        }
        #endregion //Classes

        #region Enums
        public enum Types
        {
            Mysql
        }
        #endregion //Enums

        #region Members
        private readonly Configuration configuration = configuration;

        #endregion //Members

        #region Overrides
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (this.configuration.Type)
            {
                case Types.Mysql:
                    optionsBuilder.UseMySQL(this.configuration.ConnectionString);
                    break;
            }
        }
        #endregion //Overrides

        #region Public Methods
        public static Configuration? CreateConnection(Types type, IPAddress ip, int port, string username, string password, string db)
        {
            return type switch
            {
                Types.Mysql => ExtendedDbContext.CreateMysqlConnection(ip, port, username, password, db),
                _ => null,
            };
        }

        public static Configuration CreateMysqlConnection(IPAddress ip, int port, string username, string password, string db)
        {
            string connection = "server={0};port={1};database={2};user={3};password={4};";

            return new Configuration(string.Format(connection, ip, port, db, username, password), Types.Mysql);
        }
        #endregion //Public Methods
    }
}
