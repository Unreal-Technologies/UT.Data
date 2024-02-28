using System.Net;

namespace UT.Data.DBE
{
    public interface IDatabaseConnection
    {
        public object[]? Execute(Query query);
        public string Compose(Query query);
        public bool Connect(IPAddress ip, int port, string database, string username, string password);
        public bool Close();
    }
}
