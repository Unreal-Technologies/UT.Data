namespace UT.Data.Controls
{
    public class Validator : IValidatable
    {
        #region Members
        private readonly List<IValidatable> list = [];
        private bool isValid = false;
        #endregion //Members

        #region Properties
        public bool IsRequired { get; set; } = false;
        public bool IsValid { get { return isValid; } }
        public Control? Control { get { return null; } }
        #endregion //Properties

        #region Public Methods
        public void SetError<T>(IValidatable<T>? control, string text)
            where T: Control
        {
            if(control == null || !list.Contains(control))
            {
                return;
            }
            control.SetError(text);
        }

        public void Add<T>(IValidatable<T>? control)
            where T: Control
        {
            if (control is IValidatable converted)
            {
                list.Add(converted);
            }
        }

        public void Validate()
        {
            isValid = true;
            foreach(IValidatable control in list)
            {
                control.Validate();
                if(!control.IsValid)
                {
                    isValid = false;
                }
            }
        }

        public void SetError(string text)
        {
            throw new NotImplementedException();
        }
        #endregion //Public Methods
    }
}
