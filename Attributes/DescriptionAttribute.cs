namespace UT.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class DescriptionAttribute(string text) : Attribute
    {
        #region Members
        private readonly string _text = text;
        #endregion //Members

        #region Properties
        public string Text { get { return _text; } }

        #endregion //Properties
    }
}
