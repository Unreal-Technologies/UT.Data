namespace UT.Data.Structure
{
    public class Tree<T> : Branch<T>
            where T : struct
    {
        #region Public Methods
        public static Tree<T>[] Create(Dictionary<T, T[]> data)
        {
            List<Tree<T>> buffer = new();
            foreach (KeyValuePair<T, T[]> kvp in data)
            {
                if (kvp.Value.Length == 0)
                {
                    Tree<T> tree = new(kvp.Key);
                    buffer.Add(tree);
                }
            }
            foreach (KeyValuePair<T, T[]> kvp in data)
            {
                if (kvp.Value.Length != 0)
                {
                    foreach (Tree<T> tree in buffer)
                    {
                        tree.AddBranches(data);
                    }
                }
            }

            return buffer.ToArray();
        }
        #endregion //Public Methods

        #region Constructors
        public Tree(T ordinal) : base(ordinal) { }
        #endregion //Constructors
    }
}
