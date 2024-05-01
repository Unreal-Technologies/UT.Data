namespace UT.Data.Extensions
{
    public static class IEnumerablePointFExtension
    {
        public static IEnumerable<PointF> IncrementX(this IEnumerable<PointF> list, int value)
        {
            return list.Select(x => new PointF(x.X + value, x.Y));
        }

        public static IEnumerable<PointF> IncrementY(this IEnumerable<PointF> list, int value)
        {
            return list.Select(x => new PointF(x.X, x.Y + value));
        }

        public static IEnumerable<PointF> Increment(this IEnumerable<PointF> list, int value)
        {
            return list.IncrementX(value).IncrementY(value);
        }
    }
}
