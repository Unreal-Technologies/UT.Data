using System.Diagnostics.CodeAnalysis;

namespace UT.Data.Controls
{
    public class Validated<T> : Panel, IValidatable<T>
        where T: Control
    {
        #region Members
        [AllowNull]
        private readonly T control;
        private readonly Label symbol;
        private readonly ToolTip tooltip;
        private bool isRequired = false;
        private bool isValid = false;
        private readonly ValueHandler valueHandler;
        private readonly Color defaultBackColor;
        #endregion //Members

        #region Properties
        public T Control { get { return this.control; } }
        public bool IsRequired { get { return this.isRequired; } set { this.isRequired = value; this.SetSymbol(); } }
        public bool IsValid { get { return this.isValid; } }
        Control IValidatable.Control { get { return this.control; } }
        #endregion //Properties

        #region Delegates
        public delegate string? ValueHandler(T control);
        #endregion //Delegates

        #region Constructors
        public Validated(ValueHandler handler)
        {
            this.tooltip = new()
            {
                ShowAlways = true,
                ToolTipTitle = Strings.String_InputError,
                ToolTipIcon = ToolTipIcon.Error,
                IsBalloon = true,
                InitialDelay = 0,
                Active = true,
                AutomaticDelay = 0,
                ReshowDelay = 0,
                AutoPopDelay = 0,
            };

            this.valueHandler = handler;
            T? temp = (T?)Activator.CreateInstance(typeof(T));
            if(temp != null)
            {
                temp.SizeChanged += Temp_SizeChanged;
                this.control = temp;
            }
            this.defaultBackColor = this.control?.BackColor ?? this.BackColor;
            this.symbol = new()
            {
                Text = "*",
                Font = new Font(this.Font.FontFamily, 11, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                Location = new Point(this.Location.X + this.Width + 1, this.Location.Y)
            };

            this.Controls.Add(this.control);
        }
        #endregion //Constructors

        #region Public Methods
        public void Validate()
        {
            this.isValid = true;
            T control = this.control;
            string? text = this.valueHandler(control);

            this.ClearError();
            if ((text == null || text == string.Empty) && this.isRequired)
            {
                this.SetError(Strings.String_RequiredField);
                this.isValid = false;
                return;
            }

            if (text != null)
            {

            }
        }
        #endregion //Public Methods

        #region Private Methods
        private void ClearError()
        {
            this.SetError(null);
        }

        private void SetError(string? text = null)
        {
            bool isError = text != null;
            if (isError)
            {
                this.tooltip.SetToolTip(this.control, text);
                this.control.BackColor = Color.Red;
            }
            else
            {
                this.tooltip.RemoveAll();
                this.control.BackColor = this.defaultBackColor;
            }
        }

        private void Temp_SizeChanged(object? sender, EventArgs e)
        {
            this.symbol.Location = new Point(this.control.Location.X + this.control.Width + 1, this.control.Location.Y);
            this.Size = this.PreferredSize;
        }

        private void SetSymbol()
        {
            if (this.isRequired && this.symbol != null)
            {
                this.Controls.Add(this.symbol);
                this.symbol.Size = this.symbol.PreferredSize;
            }
            else if (this.symbol != null)
            {
                this.symbol.Location = new Point(this.Location.X + this.Width + 1, this.Location.Y);
                this.Controls.Remove(this.symbol);
            }
            this.Size = this.PreferredSize;
        }
        #endregion //Private Methods
    }
}
