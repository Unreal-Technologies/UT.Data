namespace UT.Data.Controls
{
    public interface IValidatable
    {
        #region Properties
        public bool IsRequired { get; set; }
        public bool IsValid { get; }
        public Control? Control { get; }
        #endregion //Properties

        #region Public Methods
        public void Validate();
        public void SetError(string text);
        #endregion //Public Methods
    }

    public interface IValidatable<T> : IValidatable
        where T : Control
    {
        public new T Control { get; }
    }
}
