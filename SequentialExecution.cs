namespace UT.Data
{
    public class SequentialExecution
    {
        #region Members
        private readonly List<Tuple<Task, string>> tasks;
        private int position;
        #endregion //Members

        #region Delegates
        public delegate void Task(SequentialExecution self);
        public delegate void OnOutput(string text);
        #endregion //Delegates

        #region Events
        public event OnOutput? Output;
        #endregion //Events

        #region Constructors
        public SequentialExecution()
        {
            this.tasks = new List<Tuple<Task, string>>();
            this.position = 0;
        }
        #endregion //Constructors

        #region Public Methods
        public void Add(Task task, string title)
        {
            this.tasks.Add(new Tuple<Task, string>(task, title));
        }

        public void Start()
        {
            Thread thread = new(new ThreadStart(this.SubThread));
            thread.Start();
        }

        public void SetOutput(string? text)
        {
            if (this.Output == null)
            {
                return;
            }

            string progression = (this.position + 1) + " / " + this.tasks.Count;

            Tuple<Task, string> data = this.tasks[this.position];
            string title = data.Item2;

            string composed = "Task " + progression + ", " + title + (text == null ? "" : ": " + text);
            this.Output(composed);
        }
        #endregion //Public Methods

        #region Private Methods
        private void SubThread()
        {
            while(this.position < this.tasks.Count)
            {
                Tuple<Task, string> data = this.tasks[this.position];
                Task task = data.Item1;
                this.SetOutput(null);
                task(this);

                this.position++;
            }
        }
        #endregion //Private Methods
    }
}
