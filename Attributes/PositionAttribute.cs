namespace UT.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PositionAttribute : Attribute
    {
        #region Members
        private readonly int position;
        private readonly Type[]? requires;
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

        public PositionAttribute(int position, Type[]? requires = null)
        {
            this.position = position;
            this.requires = requires;
        }
        #endregion //Constructors
    }
}
