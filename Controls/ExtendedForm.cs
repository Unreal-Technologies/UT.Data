namespace UT.Data.Controls
{
    public partial class ExtendedForm : Form
    {
        #region Members
        private string text;
        #endregion //Members

        #region Properties
        public InfoBar InfoBar 
        { 
            get { return this.infoBar1; } 
        }

        public string Title { get; set; }
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
            Title = string.Empty;
            text = string.Empty;

            InitializeComponent();

            this.Resize += ExtendedForm_Resize;
        }
        #endregion //Constructors

        #region Private Methods
        private void ExtendedForm_Resize(object? sender, EventArgs e)
        {
            infoBar1.Width = Width;
        }
        #endregion //Private Methods
    }
}
