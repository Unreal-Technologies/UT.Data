using UT.Data.Controls.Gdi;

namespace UT.Data.Extensions
{
    public static class ControlExtension
    {
        #region Public Methods
        public static RadialTransform RadialTransform(
            this Control control,
            SizeF topLeftRadial,
            SizeF topRightRadial,
            SizeF bottomLeftRadial,
            SizeF bottomRightRadial,
            Func<Control, bool>? query = null,
            Color? color = null
        )
        {
            return new RadialTransform(control, topLeftRadial, topRightRadial, bottomLeftRadial, bottomRightRadial, query, color);
        }

        public static RadialTransform RadialTransform(
            this Control control,
            float topLeftRadial,
            float topRightRadial,
            float bottomLeftRadial,
            float bottomRightRadial,
            Func<Control, bool>? query = null,
            Color? color = null
        )
        {
            return new RadialTransform(control, topLeftRadial, topRightRadial, bottomLeftRadial, bottomRightRadial, query, color);
        }

        public static RadialTransform RadialTransform(
            this Control control,
            float radial,
            Func<Control, bool>? query = null,
            Color? color = null
        )
        {
            return new RadialTransform(control, radial, query, color);
        }

        public static RadialTransform RadialTransform(
            this Control control,
            SizeF radial,
            Func<Control, bool>? query = null,
            Color? color = null
        )
        {
            return new RadialTransform(control, radial, query, color);
        }

        public static BorderTransform BorderTransform(this Control control, BorderStyle borderStyle, Color color)
        {
            return control.RadialTransform(0).BorderTransform(borderStyle, color);
        }
        #endregion //Public Methods
    }
}
