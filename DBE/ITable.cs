namespace UT.Data.DBE
{
    public interface ITable
    {
        public static abstract void CreateOrUpdate(IDatabaseConnection dbc);
        public List<string> Changed { get; protected set; }
    }

    public interface ITable<T> : ITable
        where T : struct
    {
        public abstract T? GetPrimary();
    }
}
