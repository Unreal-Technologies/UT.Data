namespace UT.Data.Structure
{
    public class Branch<T>
        where T : struct
    {
        #region Members
        private readonly T ordinal;
        private readonly List<Branch<T>> branches;
        #endregion //Members

        #region Properties
        public T Ordinal
        {
            get { return ordinal; }
        }

        public Branch<T>[] Branches
        {
            get { return [.. branches]; }
        }
        #endregion //Properties

        #region Constructors
        protected Branch(T i)
        {
            ordinal = i;
            branches = [];
        }
        #endregion //Constructors

        #region Public Methods
        public void AddBranches(Dictionary<T, T[]> data)
        {
            T[] branchList = data.Where(x => Array.IndexOf(x.Value, ordinal) != -1).Select(x => x.Key).Distinct().ToArray();
            foreach (T branch in branchList)
            {
                Branch<T> b = new(branch);
                if (!branches.Contains(b) && branches.Find(x => x.ordinal.Equals(ordinal)) == null)
                {
                    b.AddBranches(data);
                    branches.Add(b);
                }
            }
        }
        #endregion //Public Methods
    }
}
