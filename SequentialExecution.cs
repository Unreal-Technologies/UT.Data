namespace UT.Data
{
    public class SequentialExecution(Form? parent)
    {
        #region Members
        private readonly List<Tuple<Task, string>> tasks = [];
        private int position = 0;
        private bool isValid = true;
        private readonly Form? parent = parent;
        private bool active = true;
        #endregion //Members

        #region Delegates
        public delegate bool Task(SequentialExecution self);
        public delegate void OnOutput(string text, bool isValid);
        #endregion //Delegates

        #region Properties
        public Form? Parent
        {
            get { return this.parent; }
        }

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
        #endregion //Constructors

        #region Public Methods
        public void Exit()
        {
            this.active = false;
        }

        public void Add(Task task, string title, int? position = null)
        {
            if (position == null)
            {
                this.tasks.Add(new Tuple<Task, string>(task, title));
            }
            else
            {
                int pos = (int)position < 0 ? this.tasks.Count + (int)position : (int)position;
                Tuple<Task, string>[] left = this.tasks.Take(pos).ToArray();
                Tuple<Task, string>[] right = this.tasks.Skip(pos).ToArray();

                this.tasks.Clear();
                this.tasks.AddRange(left);
                this.tasks.Add(new Tuple<Task, string>(task, title));
                this.tasks.AddRange(right);
            }
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

            string composed = Strings.Word_Task + " " + progression + ", " + title + (text == null ? "" : ": " + text);
            this.Output(composed, this.isValid);
            Thread.Sleep(25);
        }
        #endregion //Public Methods

        #region Private Methods
        private void SubThread()
        {
            bool state = true;
            while(this.position < this.tasks.Count && this.active)
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
