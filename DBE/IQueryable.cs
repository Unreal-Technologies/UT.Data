namespace UT.Data.DBE
{
    public interface IQueryable
    {
        public string Compose(Query query);
        public object[]? Execute(Query query);
    }
}
