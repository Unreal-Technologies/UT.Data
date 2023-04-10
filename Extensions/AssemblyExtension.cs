using System.Reflection;

namespace UT.Data.Extensions
{
    public static class AssemblyExtension
    {
        #region Public Methods
        public static Assembly[] LoadAllReferenced(this Assembly self)
        {
            DirectoryInfo? dir = new FileInfo(self.Location).Directory;
            if (dir == null)
            {
                return Array.Empty<Assembly>();
            }

            Assembly? entry = Assembly.GetEntryAssembly();
            List<Assembly> assemblies = new();
            assemblies.Add(Assembly.GetExecutingAssembly());
            if (entry != null)
            {
                assemblies.Add(entry);
            }
            assemblies.Add(Assembly.GetCallingAssembly());
            List<string> loaded = new();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!loaded.Contains(assembly.Location) && !assemblies.Contains(assembly))
                {
                    loaded.Add(assembly.Location);
                    assemblies.Add(assembly);
                }

            }
            string[] mask = new[] { "*.dll" };
            foreach (FileInfo fi in mask.SelectMany(dir.EnumerateFiles))
            {
                if (!loaded.Contains(fi.FullName))
                {
                    Assembly assem = Assembly.LoadFile(fi.FullName);
                    loaded.Add(fi.FullName);
                    assemblies.Add(assem);
                }
            }

            return assemblies.DistinctBy(x => x.Location).OrderBy(x => x.Location).ToArray();
        }

        public static Type[] GetTypes<T>(this Assembly assembly)
        {
            Type self = typeof(T);
            List<Type> types = new();
            foreach (Type type in assembly.ExportedTypes)
            {
                if (
                    (
                        type.IsSubclassOf(self) ||
                        type is T ||
                        type.IsAssignableFrom(self) ||
                        type.GetInterface(self.Name) != null
                    ) &&
                    type != typeof(object) &&
                    type != self
                )
                {
                    types.Add(type);
                }
            }

            return types.ToArray();
        }
        #endregion //Public Methods
    }
}
