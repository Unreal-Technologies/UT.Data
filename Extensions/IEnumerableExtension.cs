namespace UT.Data.Extensions
{
    public static class IEnumerableExtension
    {
        #region Public Methods
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

        public static IEnumerable<Tvalue> OrderBy<Tvalue, Tkey>(this IEnumerable<Tvalue> self, Func<Tvalue, Tkey> keySelector, int direction)
        {
            if (direction == 1)
            {
                return self.OrderBy(keySelector);
            }
            return self.OrderByDescending(keySelector);
        }
        #endregion //Public Methods
    }
}
