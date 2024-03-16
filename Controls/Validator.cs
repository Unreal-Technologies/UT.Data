namespace UT.Data.Controls
{
    public class Validator : IValidatable
    {
        #region Members
        private readonly List<IValidatable> list = [];
        private bool isValid = false;
        private bool isRequired = false;
        #endregion //Members

        #region Properties
        public bool IsRequired { get { return this.isRequired; } set { this.isRequired = value; } }
        public bool IsValid { get { return this.isValid; } }
        public Control? Control { get { return null; } }
        #endregion //Properties

        #region Public Methods
        public void Add<T>(IValidatable<T>? control)
            where T: Control
        {
            if (control != null && control is IValidatable converted)
            {
                this.list.Add(converted);
            }
        }

        public void Validate()
        {
            this.isValid = true;
            foreach(IValidatable control in list)
            {
                control.Validate();
                if(!control.IsValid)
                {
                    this.isValid = false;
                }
            }
        }
        #endregion //Public Methods
    }
}
