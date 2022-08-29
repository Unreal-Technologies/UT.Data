namespace UT.Data.Forms
{
    public class Invoker<T>
        where T: Control
    {
        #region Delegates
        public delegate void Action(T control, object[]? data);
        #endregion //Delegates

        #region Public Methods
        public static void Invoke(T control, Action action, object[]? data)
        {
            if(control.InvokeRequired)
            {
                control.BeginInvoke(new MethodInvoker(delegate () { Invoker<T>.Invoke(control, action, data); }));
            }
            else
            {
                action(control, data);
            }
        }

        public static void Invoke(T control, Action action)
        {
            Invoker<T>.Invoke(control, action, null);
        }
        #endregion //Public Methods
    }
}
