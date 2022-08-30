namespace UT.Data.Extensions
{
    public static class IntExtension
    {
        public static T? AsEnum<T>(this int value)
            where T : struct, Enum
        {
            try
            {
                return (T)Enum.ToObject(typeof(T), value);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
