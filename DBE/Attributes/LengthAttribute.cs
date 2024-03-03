namespace UT.Data.DBE.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class LengthAttribute(int value) : Attribute
    {
        #region Members
        private int value = value;
        #endregion //Members

        #region Properties
        public int Value { get { return this.value; } }
        #endregion //Properties
    }
}
