namespace UT.Data.Modlet
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PositionAttribute : Attribute
    {
        #region Members
        private readonly int position;
        #endregion //Members

        #region Properties
        public int Position
        {
            get { return this.position; }
        }
        #endregion //Properties

        #region Constructors
        public PositionAttribute(int position)
        {
            this.position = position;
        }
        #endregion //Constructors
    }
}
