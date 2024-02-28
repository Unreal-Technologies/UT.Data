namespace UT.Data.DBE
{
    public interface IDatabaseConnection
    {
        public object[]? Execute(Query query);
        public string Compose(Query query);
    }
}
