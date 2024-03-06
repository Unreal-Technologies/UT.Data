using System.Reflection;
using UT.Data.Attributes;
using UT.Data.Extensions;
using UT.Data.Structure;

namespace UT.Data.IO.Assemblies
{
    public class Loader
    {
        #region Public Methods
        public static T[] GetInstances<T>(bool byRequirements = true)
            where T : class
        {
            List<T> buffer = [];
            Assembly[] assemblies = Loader.GetAssemblies<T>();
            bool isInterface = typeof(T).IsInterface;

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (!type.IsInterface && isInterface)
                    {
                        Type? modlet = type.GetInterfaces().Where(x => x == typeof(T)).FirstOrDefault();
                        if (modlet != null)
                        {
                            T? instance = (T?)Activator.CreateInstance(type);
                            if (instance != null)
                            {
                                buffer.Add(instance);
                            }
                        }
                    }
                    else if(!type.IsInterface && type == typeof(T))
                    {
                        T? instance = (T?)Activator.CreateInstance(type);
                        if (instance != null)
                        {
                            buffer.Add(instance);
                        }
                    }
                }
            }

            T[] ordinal = [.. buffer.OrderBy(x => x.GetAttribute<PositionAttribute>()?.Position ?? int.MaxValue).ThenBy(x => x.GetType().AssemblyQualifiedName)];
            if(!byRequirements)
            {
                return ordinal;
            }
            return Loader.GetInstancesOrdinalByRequirements<T>(ordinal);
        }

        public static Assembly[] GetAssemblies<T>()
            where T : class
        {
            List<Assembly> assemblies = [Assembly.GetExecutingAssembly()];

            Assembly? entryAssem = Assembly.GetEntryAssembly();
            if (entryAssem != null)
            {
                assemblies.Add(entryAssem);
            }

            Assembly? selfAssem = Assembly.GetAssembly(typeof(T));
            if (selfAssem == null)
            {
                throw new NotImplementedException();
            }

            FileInfo file = new(selfAssem.Location);
            DirectoryInfo? root = file.Directory;
            if (root == null)
            {
                throw new NotImplementedException();
            }

            foreach (FileInfo fi in root.EnumerateFiles("*.dll"))
            {
                if (fi.FullName == file.FullName)
                {
                    continue;
                }
                Assembly? match = assemblies.Where(x => x.Location == fi.FullName).FirstOrDefault();
                if (match != null)
                {
                    continue;
                }

                Assembly assem = Assembly.LoadFrom(fi.FullName);
                assemblies.Add(assem);
            }

            return [.. assemblies];
        }
        #endregion //Public Methods

        #region Private Methods
        private static T[] GetInstancesOrdinalByRequirements<T>(T[] list)
            where T : class
        {
            Dictionary<int, int[]> parents = [];
            foreach(T item in list)
            {
                PositionAttribute? position = item.GetAttribute<PositionAttribute>();
                if (position != null)
                {
                    List<int> pBuffer = [];
                    foreach(Type t in Loader.GetValidTypes<T>(position.Requires))
                    {
                        T? parent = list.Where(x => x.GetType() == t).FirstOrDefault();
                        if(parent != null)
                        {
                            int pIdx = Array.IndexOf(list, parent);
                            pBuffer.Add(pIdx);
                        }
                    }
                    parents.Add(Array.IndexOf(list, item), [.. pBuffer]);
                }
                else
                {
                    
                    throw new NotImplementedException();
                }
            }

            List<T> buffer = [];
            Tree<int>[] trees = Tree<int>.Create(parents);

            Loader.FillBuffer(buffer, trees, list);

            return [.. buffer];
        }

        private static void FillBuffer<T>(List<T> buffer, Branch<int>[] branches, T[] list)
        {
            foreach(Branch<int> branch in branches)
            {
                Loader.FillBuffer(buffer, branch, list);
            }
        }

        private static void FillBuffer<T>(List<T> buffer, Branch<int> branch, T[] list)
        {
            T self = list[branch.Ordinal];
            buffer.Add(self);

            foreach(Branch<int> b in branch.Branches)
            {
                Loader.FillBuffer(buffer, b, list);
            }
        }

        private static Type[] GetValidTypes<T>(Type[]? list)
            where T : class
        {
            if(list == null)
            {
                return Array.Empty<Type>();
            }
            List<Type> buffer = new();
            foreach(Type t in list)
            {
                if(t.GetInterfaces().Where(x => x == typeof(T)).FirstOrDefault() != null)
                {
                    buffer.Add(t);
                }
            }

            return buffer.ToArray();
        }
        #endregion //Private Methods
    }
}
