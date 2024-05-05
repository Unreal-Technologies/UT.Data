using Microsoft.EntityFrameworkCore;

namespace UT.Data.Efc
{
    public abstract class ExtendedDbContext(ExtendedDbContext.Configuration configuration) : DbContext()
    {
        #region Classes
        public class Configuration(string connectionString, Types type)
        {
            #region Members
            private readonly string connectionString = connectionString;
            private readonly Types type = type;
            #endregion //Members

            #region Properties
            public string ConnectionString { get { return connectionString; } }
            public Types Type { get { return type; } }
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
            switch (configuration.Type)
            {
                case Types.Mysql:
                    optionsBuilder.UseMySQL(configuration.ConnectionString);
                    break;
            }
        }
        #endregion //Overrides
    }
}
