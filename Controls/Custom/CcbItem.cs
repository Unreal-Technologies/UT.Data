namespace UT.Data.Controls.Custom
{
    public class CcbItem
    {
        #region Properties
        public int Value { get; set; }

        public string Name { get; set; }
        #endregion //Properties

        #region Constructors
        public CcbItem()
        {
            Value = int.MinValue;
            Name = string.Empty;
        }

        public CcbItem(string name, int val)
        {
            Name = name;
            Value = val;
        }
        #endregion //Constructors

        #region Public Methods
        public override string ToString()
        {
            return string.Format("name: '{0}', value: {1}", Name, Value);
        }
        #endregion //Public Methods
    }
}
