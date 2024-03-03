namespace UT.Data.DBE.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DefaultAttribute(string value) : Attribute
    {
        #region Properties
        public string Value { get { return this.value; } }
        #endregion //Properties

        #region Members
        private string value = value;
        #endregion //Members
    }
}
