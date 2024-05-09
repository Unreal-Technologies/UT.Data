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
        public T Control { get { return control; } }
        public bool IsRequired { get { return isRequired; } set { isRequired = value; symbol.Visible = value; } }
        public bool IsValid { get { return isValid; } }
        Control IValidatable.Control { get { return control; } }
        #endregion //Properties

        #region Delegates
        public delegate string? ValueHandler(T control);
        public delegate Tuple<bool, string> ExtraValidationHandler(T control);
        #endregion //Delegates

        #region Events
#pragma warning disable S3264 // Events should be invoked, is invoked see Validate method
        private event ExtraValidationHandler? ExtraValidation;
#pragma warning restore S3264 // Events should be invoked, is invoked see Validate method
        #endregion //Events

        #region Constructors
        public Validated(ValueHandler handler)
        {
            valueHandler = handler;
            T? temp = (T?)Activator.CreateInstance(typeof(T));
            if(temp != null)
            {
                temp.Size = temp.PreferredSize;
                temp.Dock = DockStyle.Left;
                control = temp;
            }
            defaultBackColor = control?.BackColor ?? BackColor;
            symbol = new()
            {
                Text = "*",
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                Visible = false,
                Dock = DockStyle.Right
            };

            Controls.Add(control);
            Controls.Add(symbol);

            symbol.Size = symbol.PreferredSize;
            if(control == null)
            {
                return;
            }
            SizeChanged += Validated_SizeChanged;
        }

        #endregion //Constructors

        #region Public Methods
        public void AddValidation(ExtraValidationHandler validation)
        {
            ExtraValidation += validation;
        }

        public void Validate()
        {
            isValid = true;
            string? text = valueHandler(control);

            ClearError();
            if ((text == null || text == string.Empty) && isRequired)
            {
                SetErrorInternal("This field is required");
                isValid = false;
                return;
            }

            if (text != null && ExtraValidation != null && isValid)
            {
                List<string> buffer = [];
                foreach (Delegate delegateItem in ExtraValidation.GetInvocationList())
                {
                    ExtraValidationHandler? handler = delegateItem as ExtraValidationHandler;
                    Tuple<bool, string>? output = handler?.Invoke(control);
                    if (output != null && !output.Item1)
                    {
                        buffer.Add(output.Item2);
                        isValid = false;
                    }
                }
                if (!isValid)
                {
                    SetErrorInternal(string.Join("\r\n", buffer));
                }
            }
        }
        #endregion //Public Methods

        #region Private Methods
        private void ClearError()
        {
            SetErrorInternal(null);
        }

        public void SetError(string text)
        {
            isValid = false;
            SetErrorInternal(text);
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
                tooltip.SetToolTip(control, text);
                tooltip.Show(text, control, control.Width, 0, 3000);
                control.BackColor = Color.Red;
            }
            else
            {
                control.BackColor = defaultBackColor;
            }
        }

        private void Validated_SizeChanged(object? sender, EventArgs e)
        {
            control.MinimumSize = new Size(Width - symbol.Width - InterControlPadding, Height);
        }
        #endregion //Private Methods
    }
}
