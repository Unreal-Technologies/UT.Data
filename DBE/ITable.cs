namespace UT.Data.DBE
{
    public interface ITable
    {

    }

    public interface ITable<T> : ITable
        where T : struct
    {
        public abstract T? GetPrimary();
    }
}
