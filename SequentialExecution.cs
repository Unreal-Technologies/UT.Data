namespace UT.Data
{
    public class SequentialExecution(Form? parent)
    {
        #region Members
        private readonly List<Tuple<Task, string>> tasks = [];
        private int position = 0;
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
            get { return parent; }
        }

        public bool IsValid { get; set; } = true;
        #endregion //Properties

        #region Events
        public event OnOutput? Output;

        #endregion //Events

        #region Constructors
        #endregion //Constructors

        #region Public Methods
        public void Exit()
        {
            active = false;
        }

        public void Add(Task task, string title, int? position = null)
        {
            if (position == null)
            {
                tasks.Add(new Tuple<Task, string>(task, title));
            }
            else
            {
                int pos = (int)position < 0 ? tasks.Count + (int)position : (int)position;
                Tuple<Task, string>[] left = tasks.Take(pos).ToArray();
                Tuple<Task, string>[] right = tasks.Skip(pos).ToArray();

                tasks.Clear();
                tasks.AddRange(left);
                tasks.Add(new Tuple<Task, string>(task, title));
                tasks.AddRange(right);
            }
        }

        public void Start()
        {
            IsValid = true;
            Thread thread = new(new ThreadStart(SubThread));
            thread.Start();
        }

        public void SetOutput(string? text)
        {
            if (Output == null)
            {
                return;
            }

            string progression = (position + 1) + " / " + tasks.Count;

            Tuple<Task, string> data = tasks[position];
            string title = data.Item2;

            string composed = "Task " + progression + ", " + title + (text == null ? "" : ": " + text);
            Output(composed, IsValid);
            Thread.Sleep(25);
        }
        #endregion //Public Methods

        #region Private Methods
        private void SubThread()
        {
            bool state = true;
            while(position < tasks.Count && active)
            {
                Tuple<Task, string> data = tasks[position];
                Task task = data.Item1;
                SetOutput(null);
                bool result = task(this);
                if(!result)
                {
                    state = false;
                    break;
                }
                position++;
            }
            IsValid = state;
            if (!state)
            {
                SetOutput("System aborted!");
            }
        }
        #endregion //Private Methods
    }
}
