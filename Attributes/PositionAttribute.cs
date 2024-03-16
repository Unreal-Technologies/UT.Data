namespace UT.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PositionAttribute(int position, Type[]? requires = null) : Attribute
    {
        #region Members
        private readonly int position = position;
        private readonly Type[]? requires = requires;
        #endregion //Members

        #region Properties
        public int Position
        {
            get { return this.position; }
        }

        public Type[]? Requires
        {
            get { return this.requires; }
        }
        #endregion //Properties

        #region Constructors
        public PositionAttribute(int position) : this(position, null) { }
        #endregion //Constructors
    }
}
