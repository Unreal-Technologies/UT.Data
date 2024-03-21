using System.Diagnostics.CodeAnalysis;

namespace UT.Data.Controls
{
    public class Validated<T> : Panel, IValidatable<T>
        where T: Control
    {
        #region Constants
        private const int InterControlPadding = 3;
        #endregion //Constants

        #region Members
        [AllowNull]
        private readonly T control;
        private readonly Label symbol;
        private bool isRequired = false;
        private bool isValid = false;
        private readonly ValueHandler valueHandler;
        private readonly Color defaultBackColor;
        #endregion //Members

        #region Properties
        public T Control { get { return this.control; } }
        public bool IsRequired { get { return this.isRequired; } set { this.isRequired = value; this.symbol.Visible = value; } }
        public bool IsValid { get { return this.isValid; } }
        Control IValidatable.Control { get { return this.control; } }
        #endregion //Properties

        #region Delegates
        public delegate string? ValueHandler(T control);
        public delegate Tuple<bool, string> ExtraValidationHandler(T control);
        #endregion //Delegates

        #region Events
        private event ExtraValidationHandler? ExtraValidation;
        #endregion //Events

        #region Constructors
        public Validated(ValueHandler handler)
        {
            this.valueHandler = handler;
            T? temp = (T?)Activator.CreateInstance(typeof(T));
            if(temp != null)
            {
                temp.Size = temp.PreferredSize;
                temp.Dock = DockStyle.Left;
                this.control = temp;
            }
            this.defaultBackColor = this.control?.BackColor ?? this.BackColor;
            this.symbol = new()
            {
                Text = "*",
                Font = new Font(this.Font.FontFamily, 10, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                Visible = false,
                Dock = DockStyle.Right
            };

            this.Controls.Add(this.control);
            this.Controls.Add(this.symbol);

            this.symbol.Size = this.symbol.PreferredSize;
            if(this.control == null)
            {
                return;
            }
            this.SizeChanged += Validated_SizeChanged;
        }

        #endregion //Constructors

        #region Public Methods
        public void AddValidation(ExtraValidationHandler validation)
        {
            this.ExtraValidation += validation;
        }

        public void Validate()
        {
            this.isValid = true;
            T control = this.control;
            string? text = this.valueHandler(control);

            this.ClearError();
            if ((text == null || text == string.Empty) && this.isRequired)
            {
                this.SetErrorInternal("This field is required");
                this.isValid = false;
                return;
            }

            if (text != null && this.ExtraValidation != null && this.isValid)
            {
                List<string> buffer = [];
                foreach (Delegate delegateItem in this.ExtraValidation.GetInvocationList())
                {
                    ExtraValidationHandler? handler = delegateItem as ExtraValidationHandler;
                    Tuple<bool, string>? output = handler?.Invoke(control);
                    if(output != null)
                    {
                        if(!output.Item1)
                        {
                            buffer.Add(output.Item2);
                            this.isValid = false;
                        }
                    }
                }
                if (!this.isValid)
                {
                    this.SetErrorInternal(string.Join("\r\n", buffer));
                }
            }
        }
        #endregion //Public Methods

        #region Private Methods
        private void ClearError()
        {
            this.SetErrorInternal(null);
        }

        public void SetError(string text)
        {
            this.isValid = false;
            this.SetErrorInternal(text);
        }

        private void SetErrorInternal(string? text = null)
        {
            ToolTip tooltip = new()
            {
                ShowAlways = true,
                ToolTipTitle = "Input Error",
                ToolTipIcon = ToolTipIcon.Error,
                IsBalloon = true,
                InitialDelay = 1,
                AutomaticDelay = 1,
                ReshowDelay = 1,
                AutoPopDelay = 1,
            };

            bool isError = text != null;
            if (isError)
            {
                tooltip.SetToolTip(this.control, text);
                tooltip.Show(text, this.control, this.control.Width, 0, 3000);
                this.control.BackColor = Color.Red;
            }
            else
            {
                this.control.BackColor = this.defaultBackColor;
            }
        }

        private void Validated_SizeChanged(object? sender, EventArgs e)
        {
            int width = this.Width;
            this.control.Width = width - this.symbol.Width - Validated<T>.InterControlPadding;
        }
        #endregion //Private Methods
    }
}
