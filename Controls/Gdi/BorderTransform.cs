using UT.Data.Extensions;

namespace UT.Data.Controls.Gdi
{
    public class BorderTransform
    {
        #region Members
        private readonly RadialTransform radialTransform;
        private PointF[][] lines = [];
        private readonly BorderStyle borderStyle;
        private readonly Color color;
        private PointF[] tlArc = [];
        private PointF[] trArc = [];
        private PointF[] blArc = [];
        private PointF[] brArc = [];
        #endregion //Members

        internal BorderTransform(RadialTransform radialTransform, BorderStyle borderStyle, Color color)
        {
            if (borderStyle != BorderStyle.None)
            {
                radialTransform.Control.Resize += Control_Resize;
                radialTransform.Control.Paint += Control_Paint;
            }

            this.radialTransform = radialTransform;
            this.color = color;
            this.borderStyle = borderStyle;

            CalculateBorder();
        }

        #region Events
        private void Control_Paint(object? sender, PaintEventArgs e)
        {
            Pen pen = new(color);
            Pen pen3d = new(Color.FromArgb(0x7f, color));
            System.Drawing.Graphics graphics = e.Graphics;

            if (lines.Length > 0)
            {
                graphics.DrawLine(pen, lines[0][0], lines[0][1]);
                graphics.DrawLine(pen, lines[1][0], lines[1][1]);
                graphics.DrawLine(pen, lines[2][0], lines[2][1]);
                graphics.DrawLine(pen, lines[3][0], lines[3][1]);

                if (borderStyle == BorderStyle.Fixed3D)
                {
                    graphics.DrawLine(pen3d, lines[0][0].IncrementY(1), lines[0][1].IncrementY(1));
                    graphics.DrawLine(pen3d, lines[1][0].IncrementX(1), lines[1][1].IncrementX(1));
                    graphics.DrawLine(pen3d, lines[2][0].IncrementX(-1), lines[2][1].IncrementX(-1));
                    graphics.DrawLine(pen3d, lines[3][0].IncrementY(-1), lines[3][1].IncrementY(-1));

                    graphics.DrawLine(pen3d, lines[0][0].IncrementY(-1), lines[0][1].IncrementY(-1));
                    graphics.DrawLine(pen3d, lines[1][0].IncrementX(-1), lines[1][1].IncrementX(-1));
                    graphics.DrawLine(pen3d, lines[2][0].IncrementX(1), lines[2][1].IncrementX(1));
                    graphics.DrawLine(pen3d, lines[3][0].IncrementY(1), lines[3][1].IncrementY(1));
                }
            }

            for (int i = 0; i < tlArc.Length - 1; i++)
            {
                PointF a = tlArc[i];
                PointF b = tlArc[i + 1];

                graphics.DrawLine(pen, a, b);
                if (borderStyle == BorderStyle.Fixed3D)
                {
                    graphics.DrawLine(pen3d, a.Increment(1), b.Increment(1));
                    graphics.DrawLine(pen3d, a.Increment(-1), b.Increment(-1));
                }
            }

            for (int i = 0; i < trArc.Length - 1; i++)
            {
                PointF a = trArc[i];
                PointF b = trArc[i + 1];

                graphics.DrawLine(pen, a, b);
                if (borderStyle == BorderStyle.Fixed3D)
                {
                    graphics.DrawLine(pen3d, a.IncrementX(-1).IncrementY(1), b.IncrementX(-1).IncrementY(1));
                    graphics.DrawLine(pen3d, a.IncrementX(1).IncrementY(-1), b.IncrementX(1).IncrementY(-1));
                }
            }

            for (int i = 0; i < blArc.Length - 1; i++)
            {
                PointF a = blArc[i];
                PointF b = blArc[i + 1];

                graphics.DrawLine(pen, a, b);
                if (borderStyle == BorderStyle.Fixed3D)
                {
                    graphics.DrawLine(pen3d, a.IncrementX(1).IncrementY(-1), b.IncrementX(1).IncrementY(-1));
                    graphics.DrawLine(pen3d, a.IncrementX(-1).IncrementY(1), b.IncrementX(-1).IncrementY(1));
                }
            }

            for (int i = 0; i < brArc.Length - 1; i++)
            {
                PointF a = brArc[i];
                PointF b = brArc[i + 1];

                graphics.DrawLine(pen, a, b);
                if (borderStyle == BorderStyle.Fixed3D)
                {
                    graphics.DrawLine(pen3d, a.Increment(-1), b.Increment(-1));
                    graphics.DrawLine(pen3d, a.Increment(1), b.Increment(1));
                }
            }
        }

        private void Control_Resize(object? sender, EventArgs e)
        {
            CalculateBorder();
        }
        #endregion //Events

        #region Private Methods
        private void CalculateBorder()
        {
            RectangleF bounds = radialTransform.Control.Bounds;
            PointF location = radialTransform.Control.Location;

            tlArc = radialTransform.TL?.Take(90).ToArray() ?? [];
            trArc = radialTransform.TR?.Take(90).ToArray() ?? [];
            blArc = radialTransform.BL?.Take(90).ToArray() ?? [];
            brArc = radialTransform.BR?.Take(90).ToArray() ?? [];

            PointF[] tlOffset = radialTransform.TL?.Skip(90).Take(3).ToArray() ?? [];
            PointF[] trOffset = radialTransform.TR?.Skip(90).Take(3).ToArray() ?? [];
            PointF[] blOffset = radialTransform.BL?.Skip(90).Take(3).ToArray() ?? [];
            PointF[] brOffset = radialTransform.BR?.Skip(90).Take(3).ToArray() ?? [];

            List<PointF[]> linesBuffer =
            [
                [
                    tlOffset.Length > 0 ? tlOffset[1].IncrementX(-2) : new PointF(bounds.Left - location.X, bounds.Top - location.Y), 
                    trOffset.Length > 0 ? trOffset[1].IncrementX(-2) : new PointF(bounds.Right - location.X, bounds.Top - location.Y)
                ],
                [
                    tlOffset.Length > 0 ? tlOffset[0].IncrementY(-2) : new PointF(bounds.Left - location.X, bounds.Top - location.Y), 
                    blOffset.Length > 0 ? blOffset[2].IncrementY(-2) : new PointF(bounds.Left - location.X, bounds.Bottom - location.Y)
                ],
                new PointF[] { 
                    trOffset.Length > 0 ? trOffset[2].IncrementY(-2) : new PointF(bounds.Right - location.X, bounds.Top - location.Y), 
                    brOffset.Length > 0 ? brOffset[1].IncrementY(2) : new PointF(bounds.Right - location.X, bounds.Bottom - location.Y)
                }.IncrementX(-1).ToArray(),
                new PointF[] { 
                    blOffset.Length > 0 ? blOffset[0].IncrementX(-2) : new PointF(bounds.Left - location.X, bounds.Bottom - location.Y), 
                    brOffset.Length > 0 ? brOffset[2].IncrementX(2) : new PointF(bounds.Right - location.X, bounds.Bottom - location.Y)
                }.IncrementY(-1).ToArray(),
            ];
            this.lines = [.. linesBuffer];

            if (radialTransform.Control.HasChildren)
            {
                UpdateChildren();
            }
        }

        private void UpdateChildren()
        {
            if(radialTransform.Children.Count == 0 && radialTransform.Control.Controls.Count != 0)
            {
                radialTransform.UpdateChildren();
            }

            foreach (Control child in radialTransform.Control.Controls)
            {
                if(radialTransform.Children.TryGetValue(child, out RadialTransform? value))
                {
                    value.BorderTransform(borderStyle, color);
                }
            }
        }
        #endregion //Private Methods
    }
}
