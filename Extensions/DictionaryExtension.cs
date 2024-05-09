namespace UT.Data.Extensions
{
    public static class DictionaryExtension
    {
        #region Public Methods
        #region AddAfter
        public static void AddAfter<TKey, TValue>(this Dictionary<TKey, TValue> obj, TValue after, TKey key, TValue value)
            where TKey : notnull
        {
            TValue[] data = [.. obj.Values];
            int? index = Array.IndexOf(data, after);
            if (index == -1)
            {
                return;
            }

            obj.AddAt(key, value, index + 1);
        }

        public static void AddAfter<TKey, TValue>(this Dictionary<TKey, TValue> obj, TKey after, TKey key, TValue value)
            where TKey : notnull
        {
            TKey[] data = [.. obj.Keys];
            int? index = Array.IndexOf(data, after);
            if (index == -1)
            {
                return;
            }

            obj.AddAt(key, value, index + 1);
        }
        #endregion //AddAfter

        #region AddBefore
        public static void AddBefore<TKey, TValue>(this Dictionary<TKey, TValue> obj, TValue before, TKey key, TValue value)
            where TKey : notnull
        {
            TValue[] data = [.. obj.Values];
            int? index = Array.IndexOf(data, before);
            if (index == -1)
            {
                return;
            }

            obj.AddAt(key, value, index);
        }

        public static void AddBefore<TKey, TValue>(this Dictionary<TKey, TValue> obj, TKey before, TKey key, TValue value)
            where TKey : notnull
        {
            TKey[] data = [.. obj.Keys];
            int? index = Array.IndexOf(data, before);
            if (index == -1)
            {
                return;
            }

            obj.AddAt(key, value, index);
        }
        #endregion //AddBefore

        public static void AddAt<TKey, TValue>(this Dictionary<TKey, TValue> obj, TKey key, TValue value, int? position)
            where TKey : notnull
        {
            if (position == null)
            {
                obj.Add(key, value);
                return;
            }

            KeyValuePair<TKey, TValue>[] left = obj.Take(position.Value).ToArray();
            KeyValuePair<TKey, TValue>[] right = obj.Skip(position.Value).ToArray();

            obj.Clear();
            obj.AddRange(left);
            obj.Add(key, value);
            obj.AddRange(right);
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> obj, KeyValuePair<TKey, TValue>[] list)
            where TKey : notnull
        {
            foreach (KeyValuePair<TKey, TValue> kvp in list)
            {
                obj.Add(kvp.Key, kvp.Value);
            }
        }
        #endregion //Public Methods
    }
}
