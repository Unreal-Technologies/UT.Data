namespace UT.Data.Controls.Custom
{
    public class CcbItem
    {
        #region Members
        private int val;
        private string name;
        #endregion //Members

        #region Properties
        public int Value
        {
            get { return val; }
            set { val = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion //Properties

        #region Constructors
        public CcbItem()
        {
            val = int.MinValue;
            name = string.Empty;
        }

        public CcbItem(string name, int val)
        {
            this.name = name;
            this.val = val;
        }
        #endregion //Constructors

        #region Public Methods
        public override string ToString()
        {
            return string.Format("name: '{0}', value: {1}", name, val);
        }
        #endregion //Public Methods
    }
}
