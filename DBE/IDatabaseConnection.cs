using System.Net;

namespace UT.Data.DBE
{
    public interface IDatabaseConnection
    {
        #region Public Methods
        public object[]? Execute(Query query);
        public string Compose(Query query);
        public bool Open(IPAddress ip, int port, string database, string username, string password);
        public bool Close();
        public void CreateOrUpdateTable(ITable table);
        public bool Save<T>(ref Table<T, int> table, bool refresh = false) where T : class;
        public bool StartTransaction();
        public bool CommitTransaction();
        public bool RevertTransaction();
        #endregion //Public Methods

        #region Delegates
        public delegate void OnExceptionHandler(Exception mex);
        #endregion //Delegates

        #region Events
        public event OnExceptionHandler? OnException;
        #endregion //Events
    }
}
