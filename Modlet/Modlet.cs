using UT.Data.IO.Assemblies;

namespace UT.Data.Modlet
{
    public static class Modlet
    {
        public static IModlet[] Load<T>()
            where T : class, IModlet
        {
            IModlet[] list = Loader.GetInstances<IModlet>().Where(x => x is T).ToArray();
            return [.. list.Distinct()];
        }
    }
}
