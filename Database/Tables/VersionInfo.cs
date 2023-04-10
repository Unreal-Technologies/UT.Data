using UT.Data.Database.Attributes;

namespace UT.Data.Database.Tables
{
    public class VersionInfo : ITable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Class { get; set; }
        public int Version { get; set; }
        public string Hash { get; set; }

        public VersionInfo()
        {
            this.Class = string.Empty;
            this.Hash = string.Empty;
            this.Id = -1;
        }
    }
}
