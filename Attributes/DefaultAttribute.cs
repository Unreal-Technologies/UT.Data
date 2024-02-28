namespace UT.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DefaultAttribute(string name, string value) : Attribute
    {
        #region Members
        private readonly string name = name;
        private readonly string value = value;
        #endregion //Members

        #region Properties
        public string Name
        {
            get { return this.name; }
        }

        public string Value
        {
            get { return this.value; }
        }

        #endregion //Properties
    }
}
