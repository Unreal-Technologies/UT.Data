namespace UT.Data.Controls
{
    public class InfoBar : Panel
    {
        #region Members
        private readonly PictureBox pbClose;
        private readonly PictureBox pbIcon;
        private readonly Label lblTitle;
        #endregion //Members

        #region Properties
        public PictureBox Close { get { return pbClose; } }
        public new string Text 
        { 
            get { return lblTitle.Text; }
            set 
            { 
                lblTitle.Text = value; 
                lblTitle.Size = lblTitle.PreferredSize; 
                lblTitle.BringToFront(); 
                PositionTitle(); 
            }
        }
        #endregion //Properties

        public InfoBar()
            : base()
        {
            pbClose = new PictureBox
            {
                Image = Resources.Delete,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Size = new Size(75, 50),
                Dock = DockStyle.Right
            };
            pbClose.MouseEnter += PictureBox_MouseEnter;
            pbClose.MouseLeave += PictureBox_MouseLeave;

            pbIcon = new PictureBox
            {
                Size = new Size(50, 50),
                Dock = DockStyle.Left,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            lblTitle = new Label()
            {
                Location = new Point(50, 0),
                Font = new Font(Font.FontFamily, 16, FontStyle.Bold),
                ForeColor = SystemColors.Highlight
            };

            int h = new int[] { pbClose.Height, pbIcon.Height, lblTitle.Height }.OrderByDescending(x => x).FirstOrDefault();
            int w = new int[] { pbClose.Width, pbIcon.Width, lblTitle.Width }.Sum();

            this.Controls.Add(pbClose);
            this.Controls.Add(pbIcon);
            this.Controls.Add(lblTitle);

            MinimumSize = new Size(w, h);
            MaximumSize = new Size(int.MaxValue, h);

            ParentChanged += InfoBar_ParentChanged;
            BackColor = Color.LightGoldenrodYellow;
        }

        #region Private Methods
        private void PictureBox_MouseLeave(object? sender, EventArgs e)
        {
            if (sender is PictureBox pb)
            {
                pb.BackColor = BackColor;
            }
        }

        private void PictureBox_MouseEnter(object? sender, EventArgs e)
        {
            if (sender is PictureBox pb)
            {
                pb.BackColor = Color.YellowGreen;
            }
        }

        private void PositionTitle()
        {
            int y = (pbIcon.Height - lblTitle.Height) / 2;
            int x = pbIcon.Width + 5;

            lblTitle.Location = new Point(x, y);
        }

        private void InfoBar_ParentChanged(object? sender, EventArgs e)
        {
            Control? parent = Parent;
            if (parent != null)
            {
                while (parent != null && parent is not Form)
                {
                    parent = parent.Parent;
                }
                if (parent is Form form)
                {
                    pbIcon.Image = form.Icon?.ToBitmap();
                }
            }
        }
        #endregion //Private Methods
    }
}
