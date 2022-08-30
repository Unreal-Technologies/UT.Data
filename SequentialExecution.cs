namespace UT.Data
{
    public class SequentialExecution
    {
        #region Members
        private readonly List<Tuple<Task, string>> tasks;
        private int position;
        private bool isValid;
        #endregion //Members

        #region Delegates
        public delegate bool Task(SequentialExecution self);
        public delegate void OnOutput(string text, bool isValid);
        #endregion //Delegates

        #region Properties
        public bool IsValid
        {
            get { return this.isValid; }
            set { this.isValid = value; }
        }
        #endregion //Properties

        #region Events
        public event OnOutput? Output;
        #endregion //Events

        #region Constructors
        public SequentialExecution()
        {
            this.tasks = new List<Tuple<Task, string>>();
            this.position = 0;
            this.isValid = true;
        }
        #endregion //Constructors

        #region Public Methods
        public void Add(Task task, string title)
        {
            this.tasks.Add(new Tuple<Task, string>(task, title));
        }

        public void Start()
        {
            this.isValid = true;
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
            this.Output(composed, this.isValid);
            Thread.Sleep(25);
        }
        #endregion //Public Methods

        #region Private Methods
        private void SubThread()
        {
            bool state = true;
            while(this.position < this.tasks.Count)
            {
                Tuple<Task, string> data = this.tasks[this.position];
                Task task = data.Item1;
                this.SetOutput(null);
                bool result = task(this);
                if(!result)
                {
                    state = false;
                    break;
                }
                this.position++;
            }
            this.isValid = state;
            if (!state)
            {
                this.SetOutput("System aborted!");
            }
        }
        #endregion //Private Methods
    }
}
