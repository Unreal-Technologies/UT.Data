namespace UT.Data.Extensions
{
    public static class ObjectExtension
    {
        #region Public Methods
        public static T? GetAttribute<T>(this object value)
        {
            return (T?)value.GetType().GetCustomAttributes(true).Where(x => x.GetType() == typeof(T)).FirstOrDefault();
        }
        #endregion //Public Methods
    }
}
