using UT.Data.Controls.Gdi;
using UT.Data.Extensions;

namespace UT.Data.Controls
{
    public partial class ExtendedForm : Form
    {
        #region Members
        private string text;
        private string title;
        #endregion //Members

        #region Events
        public event EventHandler? TitleChanged;
        #endregion //Events

        #region Properties
        public InfoBar? InfoBar 
        { 
            get { return infoBar1; } 
        }

        public string Title 
        {
            get { return title; }
            set
            {
                bool isChanged = false;
                if(title != value)
                {
                    isChanged = true;
                }
                title = value;
                if(TitleChanged != null && isChanged)
                {
                    TitleChanged.Invoke(this, new EventArgs());
                }
            }
        }

        public new string Text 
        {
            get { return Title + (Title != string.Empty && text != string.Empty ? " - " : string.Empty) + text; }
            set { text = value; }
        }
        #endregion //Properties

        #region Constructors
        public ExtendedForm() : base()
        {
            Icon = Resources.Favicon;
            Font = new Font(FontFamily.GenericMonospace, 9);
            title = string.Empty;
            text = string.Empty;

            InitializeComponent();

            Resize += ExtendedForm_Resize;
            TitleChanged += ExtendedForm_TextOrTitleChanged;
            TextChanged += ExtendedForm_TextOrTitleChanged;

            if (InfoBar != null)
            {
                InfoBar.Close.Click += InfoBar_Close_Click;
            }

            TransparencyKey = RadialTransform.TransparencyKey;
        }

        private void ExtendedForm_TextOrTitleChanged(object? sender, EventArgs e)
        {
            if (infoBar1 != null) 
            { 
                infoBar1.Text = Text; 
            }
        }
        #endregion //Constructors

        #region Private Methods
        private void ExtendedForm_Resize(object? sender, EventArgs e)
        {
            infoBar1.Width = Width;
        }

        private void InfoBar_Close_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
        #endregion //Private Methods
    }
}
