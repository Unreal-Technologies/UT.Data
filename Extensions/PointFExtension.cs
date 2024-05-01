namespace UT.Data.Extensions
{
    public static class PointFExtension
    {
        #region Public Methods
        public static PointF IncrementX(this PointF point, int value)
        {
            point.X += value;
            return point;
        }

        public static PointF IncrementY(this PointF point, int value)
        {
            point.Y += value;
            return point;
        }

        public static PointF Increment(this PointF point, int value)
        {
            return point.IncrementX(value).IncrementY(value);
        }
        #endregion //Public Methods
    }
}
