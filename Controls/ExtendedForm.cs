namespace UT.Data.Controls
{
    public partial class ExtendedForm : Form
    {
        #region Members
        private string text;
        private string title;
        #endregion //Members

        #region Properties
        public InfoBar? InfoBar 
        { 
            get { return this.infoBar1; } 
        }

        public string Title 
        {
            get { return this.title; }
            set { this.title = value; if (infoBar1 != null) { infoBar1.Text = Text; } }
        }
        public new string Text 
        {
            get { return Title + (Title != string.Empty && text != string.Empty ? " - " : string.Empty) + text; }
            set { text = value; if (infoBar1 != null) { infoBar1.Text = Text; } }
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
            InfoBar.Close.Click += InfoBar_Close_Click;
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
