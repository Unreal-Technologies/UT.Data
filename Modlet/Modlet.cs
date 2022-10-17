using UT.Data.IO.Assemblies;

namespace UT.Data.Modlet
{
    public class Modlet
    {
        public static IModlet[] Load(SequentialExecution? se)
        {
            IModlet[] list = Loader.GetInstances<IModlet>();
            if (se != null)
            {
                foreach (IModlet modlet in list)
                {
                    modlet.OnSequentialExecutionConfiguration(se);
                }
            }
            return list.ToArray();
        }
    }
}
