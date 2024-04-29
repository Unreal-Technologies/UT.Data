using UT.Data.Extensions;

namespace UT.Data.Controls
{
    public class GdiText : Panel
    {
        #region Properties;
        public new string Text { get; set; }
        public StringAlignment HorizontalAlignment { get; set; }
        public StringAlignment VerticalAlignment { get; set; }
        public bool DrawShadow { get; set; }
        public Shadows Shadow { get; set; }
        public bool DrawBackground { get; set; }
        public int Opacity { get; set; }
        public Color BackgroundColor { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }
        #endregion //Properties

        #region Enums
        public enum Shadows
        {
            TopLeft, TopRight, BottomLeft, BottomRight
        }
        #endregion //Enums

        #region Constructor
        public GdiText()
        {
            Text = "--text--";
            BackColor = Color.Transparent;
            BorderStyle = BorderStyle.None;
            HorizontalAlignment = StringAlignment.Near;
            VerticalAlignment = StringAlignment.Near;
            DrawShadow = false;
            Shadow = Shadows.BottomRight;
            Opacity = 0x7f;
            BackgroundColor = Color.Gray;
        }
        #endregion //Constructors

        #region Protected Methods
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Method intentionally left empty.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            if(DrawBackground)
            {
                Brush backgroundBrush = new SolidBrush(Color.FromArgb(Opacity, BackgroundColor));
                graphics.FillRectangle(backgroundBrush, DisplayRectangle);
            }

            if(DrawShadow)
            {
                Point shadow = GetShadow(Shadow);
                RectangleF shadowBounds = new(
                    ((PointF)DisplayRectangle.Location).IncrementX(shadow.X).IncrementY(shadow.Y), 
                    DisplayRectangle.Size
                );
                Drawstring(shadowBounds, Brushes.Black, graphics);
            }

            RectangleF textbounds = DisplayRectangle;
            Brush textBrush = new SolidBrush(ForeColor);
            Drawstring(textbounds, textBrush, graphics);

            base.OnPaint(e);
        }
        #endregion //Protected Methods

        #region Private Methods
        private static Point GetShadow(Shadows shadow)
        {
            return shadow switch
            {
                Shadows.TopLeft => new Point(-1, -1),
                Shadows.TopRight => new Point(1, -1),
                Shadows.BottomLeft => new Point(-1, 1),
                Shadows.BottomRight => new Point(1, 1),
                _ => Point.Empty,
            };
        }

        private void Drawstring(RectangleF bounds, Brush brush, Graphics graphics)
        {
            graphics.DrawString(Text, Font, brush, bounds, new StringFormat()
            {
                Alignment = HorizontalAlignment,
                FormatFlags = StringFormatFlags.NoWrap,
                LineAlignment = VerticalAlignment
            });
        }
        #endregion //Private Methods
    }
}
