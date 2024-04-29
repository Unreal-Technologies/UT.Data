namespace UT.Data.Controls
{
    public partial class ExtendedForm : Form
    {
        #region Properties
        public InfoBar InfoBar { 
            get { return this.infoBar1; } 
        }
        #endregion //Properties

        #region Constructors
        public ExtendedForm() : base()
        {
            Icon = Resources.Favicon;
            Font = new Font(FontFamily.GenericMonospace, 9);

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
