namespace UT.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        #region Members
        private readonly string _text;
        #endregion //Members

        #region Properties
        public string Text { get { return this._text; } }
        #endregion //Properties

        #region Constructors
        public DescriptionAttribute(string text)
        {
            this._text = text;
        }
        #endregion //Constructors
    }
}
