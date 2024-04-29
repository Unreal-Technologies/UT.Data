using Ut.Data.Helpers;
using UT.Data.Extensions;

namespace UT.Data.Controls.Gdi
{
    public class RadialTransform
    {
        #region Members
        private readonly SizeF topLeftRadial;
        private readonly SizeF topRightRadial;
        private readonly SizeF bottomLeftRadial;
        private readonly SizeF bottomRightRadial;
        private readonly Control control;
        private PointF[]? topLeft;
        private PointF[]? topRight;
        private PointF[]? bottomLeft;
        private PointF[]? bottomRight;
        private Dictionary<Control, RadialTransform> children;
        private static readonly Color transparencyKey = Color.FromArgb(0xFF, 1, 1, 1);
        private readonly Func<Control, bool>? query;
        private readonly Color? color;
        #endregion //Members

        #region Properties
        public static Color TransparencyKey { get { return transparencyKey; } }
        public PointF[]? TL { get { return topLeft; } }
        public PointF[]? TR { get { return topRight; } }
        public PointF[]? BL { get { return bottomLeft; } }
        public PointF[]? BR { get { return bottomRight; } }
        public Control Control { get { return this.control; } }
        public Dictionary<Control, RadialTransform> Children { get { return children; } }
        #endregion //Properties

        #region Constructors
        public RadialTransform(
            Control control,
            float topleftRadial,
            float topRightRadial,
            float bottomLeftRadial,
            float bottomRightRadial,
            Func<Control, bool>? query = null,
            Color? color = null
        )
        : this(
                control,
                new SizeF(topleftRadial, topleftRadial),
                new SizeF(topRightRadial, topRightRadial),
                new SizeF(bottomLeftRadial, bottomLeftRadial),
                new SizeF(bottomRightRadial, bottomRightRadial),
                query,
                color
        )
        { }

        public RadialTransform(
            Control control, 
            float radial,
            Func<Control, bool>? query = null,
            Color? color = null
        )
        : this(
              control, 
              new SizeF(radial, radial),
              query,
              color
        ) { }

        public RadialTransform(
            Control control, 
            SizeF radial,
            Func<Control, bool>? query = null,
            Color? color = null
        )
        : this(
              control, 
              radial, 
              radial, 
              radial, 
              radial,
              query,
              color
        ) { }

        public RadialTransform(
            Control control, 
            SizeF topLeftRadial, 
            SizeF topRightRadial, 
            SizeF bottomLeftRadial, 
            SizeF bottomRightRadial,
            Func<Control, bool>? query = null,
            Color? color = null
        )
        {
            this.topLeftRadial = topLeftRadial;
            this.topRightRadial = topRightRadial;
            this.bottomLeftRadial = bottomLeftRadial;
            this.bottomRightRadial = bottomRightRadial;
            this.control = control;
            this.query = query;
            this.color = color;

            children = [];

            control.Paint += Control_Paint;
            control.Resize += Control_Resize;

            CalculatePoints();
        }
        #endregion //Constructors

        #region Private Methods
        public void UpdateChildren()
        {
            RectangleF parentBounds = control.Bounds;

            RectangleF tlR = new(new PointF(0, 0), topLeftRadial);
            RectangleF trR = new(new PointF(parentBounds.Width - topRightRadial.Width, 0), topRightRadial);
            RectangleF blR = new(new PointF(0, parentBounds.Height - bottomLeftRadial.Height), bottomLeftRadial);
            RectangleF brR = new(new PointF(parentBounds.Width - bottomRightRadial.Width, parentBounds.Height - bottomRightRadial.Height), bottomRightRadial);

            Dictionary<Control, RadialTransform> buffer = [];

            IEnumerable<Control> list = query == null ? control.Controls.Cast<Control>() : control.Controls.Cast<Control>().Where(query);
            foreach (Control child in list)
            {
                RectangleF childBounds = child.Bounds;
                if(parentBounds.IntersectsWith(childBounds))
                {
                    List<SizeF> radials =
                    [
                        tlR.IntersectsWith(childBounds) ? topLeftRadial : SizeF.Empty,
                        trR.IntersectsWith(childBounds) ? topRightRadial : SizeF.Empty,
                        blR.IntersectsWith(childBounds) ? bottomLeftRadial : SizeF.Empty,
                        brR.IntersectsWith(childBounds) ? bottomRightRadial : SizeF.Empty,
                    ];

                    buffer.Add(child, child.RadialTransform(radials[0], radials[1], radials[2], radials[3], query, color));
                }
            }
            children = buffer;
        }

        private void CalculatePoints()
        {
            bool tlSet = !topLeftRadial.IsEmpty;
            bool trSet = !topRightRadial.IsEmpty;
            bool blSet = !bottomLeftRadial.IsEmpty;
            bool brSet = !bottomRightRadial.IsEmpty;

            RectangleF bounds = control.DisplayRectangle;
            topLeft = tlSet ? AlignmentHelper.CalculateArcCorner(
                topLeftRadial,
                bounds,
                new AlignmentHelper.Settings(
                    AlignmentHelper.Settings.Horizontal.Left,
                    AlignmentHelper.Settings.Vertical.Top
                )
            ) : [];
            topRight = trSet ? AlignmentHelper.CalculateArcCorner(
                topRightRadial,
                bounds,
                new AlignmentHelper.Settings(
                    AlignmentHelper.Settings.Horizontal.Right,
                    AlignmentHelper.Settings.Vertical.Top
                )
            ) : [];
            bottomLeft = blSet ? AlignmentHelper.CalculateArcCorner(
                bottomLeftRadial,
                bounds,
                new AlignmentHelper.Settings(
                    AlignmentHelper.Settings.Horizontal.Left,
                    AlignmentHelper.Settings.Vertical.Bottom
                )
            ) : [];
            bottomRight = brSet ? AlignmentHelper.CalculateArcCorner(
                bottomRightRadial,
                bounds,
                new AlignmentHelper.Settings(
                    AlignmentHelper.Settings.Horizontal.Right,
                    AlignmentHelper.Settings.Vertical.Bottom
                )
            ) : [];

            if(control.HasChildren)
            {
                UpdateChildren();
            }
        }
        #endregion //Private Methods

        #region Events
        private void Control_Resize(object? sender, EventArgs e)
        {
            CalculatePoints();
        }

        private void Control_Paint(object? sender, PaintEventArgs e)
        {
            System.Drawing.Graphics graphics = e.Graphics;

            if (topLeft != null && topRight != null && bottomLeft != null && bottomRight != null)
            {
                Brush brush = new SolidBrush(color == null ? TransparencyKey : color.Value);

                if (topLeft.Length != 0)
                {
                    graphics.FillPolygon(brush, topLeft);
                }
                if (topRight.Length != 0)
                {
                    graphics.FillPolygon(brush, topRight);
                }
                if (bottomLeft.Length != 0)
                {
                    graphics.FillPolygon(brush, bottomLeft);
                }
                if (bottomRight.Length != 0)
                {
                    graphics.FillPolygon(brush, bottomRight);
                }
            }
        }
        #endregion //Events
    }
}
