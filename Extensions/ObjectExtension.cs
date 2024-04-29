namespace UT.Data.Extensions
{
    public static class ObjectExtension
    {
        #region Public Methods
        public static T? GetAttribute<T>(this object value)
        {
            return (T?)value.GetType().GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(T));
        }
        #endregion //Public Methods
    }
}
