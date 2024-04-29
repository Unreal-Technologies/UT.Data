using UT.Data.Controls.Gdi;

namespace UT.Data.Extensions
{
    public static class RadialTransformExtension
    {
        #region Public Methods
        public static BorderTransform BorderTransform(this RadialTransform radialTransform, BorderStyle borderStyle, Color color)
        {
            return new BorderTransform(radialTransform, borderStyle, color);
        }
        #endregion //Public Methods
    }
}
