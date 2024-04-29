﻿namespace UT.Data.Controls
{
    public partial class ExtendedForm : Form
    {
        #region Constructors
        public ExtendedForm() : base()
        {
            Icon = Resources.Favicon;

            InitializeComponent();

            this.Resize += ExtendedForm_Resize;
        }

        private void ExtendedForm_Resize(object? sender, EventArgs e)
        {
            infoBar1.Width = Width;
        }


        #endregion //Constructors
    }
}
