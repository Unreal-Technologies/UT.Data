namespace Ut.Data.Helpers
{
    public class AlignmentHelper
    {
        #region Classes
        public struct Settings(Settings.Horizontal? hAlign = null, Settings.Vertical? vAlign = null, float? xOffset = null, float? yOffset = null)
        {
            #region Enums
            public enum Horizontal
            {
                Left, Right, Center
            }

            public enum Vertical
            {
                Top, Center, Bottom
            }
            #endregion //Enums

            #region Properties
            public Horizontal? HAlign { get; set; } = hAlign;
            public Vertical? VAlign { get; set; } = vAlign;
            public float? XOffset { get; set; } = xOffset;
            public float? YOffset { get; set; } = yOffset;
            #endregion //Properties
        }
        #endregion //Classes

        #region Public Methods
        public static PointF CalculatePosition(RectangleF source, RectangleF destination, Settings settings)
        {
            float x = settings.HAlign switch
            {
                Settings.Horizontal.Right => destination.Width - source.Width,
                Settings.Horizontal.Center => (destination.Width - source.Width) / 2,
                _ => 0,
            };

            float y = settings.VAlign switch
            {
                Settings.Vertical.Center => (destination.Height - source.Height) / 2,
                Settings.Vertical.Bottom => destination.Height - source.Height,
                _ => 0,
            };

            if (settings.XOffset.HasValue)
            {
                x += settings.XOffset.Value;
            }
            if (settings.YOffset.HasValue)
            {
                y += settings.YOffset.Value;
            }

            return new(x, y);
        }

        public static PointF[] CalculateArcCorner(float radius, RectangleF bounds, AlignmentHelper.Settings settings)
        {
            return CalculateArcCorner(new SizeF(radius, radius), bounds, settings);
        }

        public static PointF[] CalculateArcCorner(SizeF radius, RectangleF bounds, AlignmentHelper.Settings settings)
        {
            float x, y;
            int xDirection = 0;
            int yDirection = 0;

            switch (settings.HAlign)
            {
                case AlignmentHelper.Settings.Horizontal.Left:
                    x = 0;
                    xDirection = 1;
                    break;
                case AlignmentHelper.Settings.Horizontal.Right:
                    x = bounds.Width;
                    xDirection = -1;
                    break;
                default:
                    x = 0;
                    break;
            }
            switch (settings.VAlign)
            {
                case AlignmentHelper.Settings.Vertical.Top:
                    y = 0;
                    yDirection = 1;
                    break;
                case AlignmentHelper.Settings.Vertical.Bottom:
                    y = bounds.Height;
                    yDirection = -1;
                    break;
                default:
                    y = 0;
                    break;
            }

            int angleOffset = 0;

            if (settings.VAlign == AlignmentHelper.Settings.Vertical.Top && settings.HAlign == AlignmentHelper.Settings.Horizontal.Left)
            {
                angleOffset = 180;
            }
            else if (settings.VAlign == AlignmentHelper.Settings.Vertical.Top && settings.HAlign == AlignmentHelper.Settings.Horizontal.Right)
            {
                angleOffset = 270;
            }
            else if (settings.VAlign == AlignmentHelper.Settings.Vertical.Bottom && settings.HAlign == AlignmentHelper.Settings.Horizontal.Right)
            {
                angleOffset = 0;
            }
            else if (settings.VAlign == AlignmentHelper.Settings.Vertical.Bottom && settings.HAlign == AlignmentHelper.Settings.Horizontal.Left)
            {
                angleOffset = 90;
            }

            if (settings.XOffset != null)
            {
                x += settings.XOffset.Value;
            }

            if (settings.YOffset != null)
            {
                y += settings.YOffset.Value;
            }

            List<PointF> buffer = [];

            PointF elipseCenter = new(
                x + (xDirection > 0 ? (radius.Width / 2) : -(radius.Width / 2)),
                y + (yDirection > 0 ? (radius.Height / 2) : -(radius.Height / 2))
            );

            float aXOffset = (xDirection > 0 ? radius.Width : -radius.Width) / 2;
            float aYOffset = (yDirection > 0 ? radius.Height : -radius.Height) / 2;

            for (int angle = 0; angle < 90; angle++)
            {
                float a = (angle + angleOffset) * (float)(Math.PI / 180);
                float aX = elipseCenter.X + (radius.Width * (float)Math.Cos(a)) + aXOffset;
                float aY = elipseCenter.Y + (radius.Height * (float)Math.Sin(a)) + aYOffset;
                buffer.Add(new PointF(aX, aY));
            }

            if (settings.VAlign == AlignmentHelper.Settings.Vertical.Top && settings.HAlign == AlignmentHelper.Settings.Horizontal.Left)
            {
                buffer.Add(new PointF(x, y + (yDirection > 0 ? radius.Height : -radius.Height)));
                buffer.Add(new PointF(x + (xDirection > 0 ? radius.Width : -radius.Width), y));
                buffer.Add(new PointF(x, y));
            }
            else if (settings.VAlign == AlignmentHelper.Settings.Vertical.Top && settings.HAlign == AlignmentHelper.Settings.Horizontal.Right)
            {
                buffer.Add(new PointF(x, y));
                buffer.Add(new PointF(x + (xDirection > 0 ? radius.Width : -radius.Width), y));
                buffer.Add(new PointF(x, y + (yDirection > 0 ? radius.Height : -radius.Height)));
            }
            else if (settings.VAlign == AlignmentHelper.Settings.Vertical.Bottom && settings.HAlign == AlignmentHelper.Settings.Horizontal.Left)
            {
                buffer.Add(new PointF(x + (xDirection > 0 ? radius.Width : -radius.Width), y));
                buffer.Add(new PointF(x, y));
                buffer.Add(new PointF(x, y + (yDirection > 0 ? radius.Height : -radius.Height)));
            }
            else if (settings.VAlign == AlignmentHelper.Settings.Vertical.Bottom && settings.HAlign == AlignmentHelper.Settings.Horizontal.Right)
            {
                buffer.Add(new PointF(x, y));
                buffer.Add(new PointF(x, y + (yDirection > 0 ? radius.Height : -radius.Height)));
                buffer.Add(new PointF(x + (xDirection > 0 ? radius.Width : -radius.Width), y));
            }

            return [.. buffer];
        }
        #endregion //Public Methods
    }
}
