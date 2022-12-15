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
            get { return this.ordinal; }
        }

        public Branch<T>[] Branches
        {
            get { return this.branches.ToArray(); }
        }
        #endregion //Properties

        #region Constructors
        protected Branch(T i)
        {
            this.ordinal = i;
            this.branches = new List<Branch<T>>();
        }
        #endregion //Constructors

        #region Public Methods
        public void AddBranches(Dictionary<T, T[]> data)
        {
            T[] branches = data.Where(x => Array.IndexOf(x.Value, this.ordinal) != -1).Select(x => x.Key).Distinct().ToArray();
            foreach (T branch in branches)
            {
                Branch<T> b = new(branch);
                if (!this.branches.Contains(b) && this.branches.Where(x => x.ordinal.Equals(this.ordinal)).FirstOrDefault() == null)
                {
                    b.AddBranches(data);
                    this.branches.Add(b);
                }
            }
        }
        #endregion //Public Methods
    }
}
