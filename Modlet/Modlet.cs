using UT.Data.IO.Assemblies;

namespace UT.Data.Modlet
{
    public class Modlet
    {
        public static IModlet[] Load<T>(SequentialExecution? se)
            where T : class, IModlet
        {
            IModlet[] list = Loader.GetInstances<IModlet>().Where(x => (x as T) != null).ToArray();
            if (se != null)
            {
                foreach (IModlet modlet in list)
                {
                    modlet.OnSequentialExecutionConfiguration(se);
                }
            }
            return [.. list];
        }
    }
}
