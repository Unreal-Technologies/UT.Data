namespace UT.Data.Extensions
{
    public static class ObjectExtension
    {
        #region Public Methods
        public static T? GetAttribute<T>(this object value)
        {
            return (T?)Array.Find(value.GetType().GetCustomAttributes(true), x => x.GetType() == typeof(T));
        }
        #endregion //Public Methods
    }
}
