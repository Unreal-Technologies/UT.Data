using System.Reflection;
using UT.Data.Extensions;

namespace UT.Data.Modlet
{
    public class Modlet
    {
        public static IModlet[] Load(SequentialExecution? se)
        {
            List<IModlet> buffer = new();
            Assembly[] assemblies = Modlet.GetAssemblies();
            
            foreach(Assembly assembly in assemblies)
            {
                foreach(Type type in assembly.GetExportedTypes())
                {
                    if(!type.IsInterface)
                    {
                        Type? modlet = type.GetInterfaces().Where(x => x == typeof(IModlet)).FirstOrDefault();
                        if(modlet != null)
                        {
                            IModlet? instance = (IModlet?)Activator.CreateInstance(type);
                            if (instance != null)
                            {
                                if (se != null)
                                {
                                    instance.OnSequentialExecutionConfiguration(se);
                                }
                                buffer.Add(instance);
                            }
                        }
                    }
                }
            }

            return buffer.OrderBy(x => x.GetAttribute<PositionAttribute>()?.Position ?? int.MaxValue).ThenBy(x => x.GetType().AssemblyQualifiedName).ToArray();
        }

        private static Assembly[] GetAssemblies()
        {
            List<Assembly> assemblies = new();
            assemblies.Add(Assembly.GetExecutingAssembly());

            Assembly? entryAssem = Assembly.GetEntryAssembly();
            if (entryAssem != null)
            {
                assemblies.Add(entryAssem);
            }

            Assembly? modletAssem = Assembly.GetAssembly(typeof(IModlet));
            if (modletAssem == null)
            {
                throw new NotImplementedException();
            }

            FileInfo file = new(modletAssem.Location);
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

                Assembly assem = Assembly.LoadFile(fi.FullName);
                assemblies.Add(assem);
            }

            return assemblies.ToArray();
        }
    }
}
