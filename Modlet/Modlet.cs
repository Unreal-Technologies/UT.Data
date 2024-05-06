using UT.Data.IO.Assemblies;

namespace UT.Data.Modlet
{
    public static class Modlet
    {
        public static IModlet[] Load<T>(bool byRequirements = true)
            where T : class, IModlet
        {
            IModlet[] list = Loader.GetInstances<IModlet>(byRequirements).Where(x => x is T).ToArray();
            return [.. list.Distinct()];
        }
    }
}
