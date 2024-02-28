namespace UT.Data.DBE
{
    public interface IDbSelect<T>
            where T : class
    {
        public abstract static T Single(int? id);
    }
}
