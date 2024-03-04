using UT.Data.Attributes;
using UT.Data.DBE.Attributes;
using DefaultAttribute = UT.Data.DBE.Attributes.DefaultAttribute;

namespace UT.Data.DBE
{
    [Description("UT.Data-TableInfo")]
    internal class TableInfo : Table<TableInfo, int>
    {
        #region Properties
        public int? Field_Id { get { return this.id; } }
        public string? Field_Table { get { return this.table; } set { this.table = value; this.Changed.Add("table"); } }
        public string? Field_Hash { get { return this.hash; } set { this.hash = value; this.Changed.Add("hash"); } }
        public DateTime? Field_TransStartDate { get { return this.transStartDate; } }
        
        #endregion //Properties

        #region Members
        [PrimaryKey]
        private int? id;
        [Length(32)]
        private string? table;
        [Length(32)]
        private string? hash;
        [Default("now()")]
        private DateTime? transStartDate;
        #endregion //Members
    }
}
