using System.Linq;

namespace UT.Data.Controls
{
    public class InfoBar : Panel
    {
        #region Members
        private readonly PictureBox btnClose;
        private readonly PictureBox icon;
        private readonly Label title;
        #endregion //Members

        #region Properties
        public PictureBox Close { get { return btnClose; } }
        #endregion //Properties

        public InfoBar()
            : base()
        {
            btnClose = new PictureBox
            {
                Image = Resources.Delete,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Size = new Size(75, 50),
                Dock = DockStyle.Right
            };

            icon = new PictureBox
            {
                Size = new Size(50, 50),
                Dock = DockStyle.Left,
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            title = new Label()
            {
                Dock = DockStyle.Fill
            };

            int h = new int[] { btnClose.Height, icon.Height, title.Height }.OrderByDescending(x => x).FirstOrDefault();
            int w = new int[] { btnClose.Width, icon.Width, title.Width }.Sum();

            this.Controls.Add(btnClose);
            this.Controls.Add(icon);
            this.Controls.Add(title);

            MinimumSize = new Size(w, h);
            MaximumSize = new Size(int.MaxValue, h);

            ParentChanged += InfoBar_ParentChanged;
        }

        private void InfoBar_ParentChanged(object? sender, EventArgs e)
        {
            Control? parent = Parent;
            if(parent != null)
            {
                while(parent != null && parent is not Form)
                {
                    parent = parent.Parent;
                }
                if(parent is Form form)
                {
                    icon.Image = form.Icon?.ToBitmap();
                }
            }
        }
    }
}
