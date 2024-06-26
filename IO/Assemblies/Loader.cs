﻿using System.Reflection;
using UT.Data.Attributes;
using UT.Data.Extensions;
using UT.Data.Structure;

namespace UT.Data.IO.Assemblies
{
    public static class Loader
    {
        #region Public Methods
        public static T[] GetInstances<T>(bool byRequirements = true, params object?[]? args)
            where T : class
        {
            List<T> buffer = [];
            Assembly[] assemblies = Loader.GetAssemblies<T>();
            bool isInterface = typeof(T).IsInterface;

            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetExportedTypes();
                foreach (Type type in types)
                {
                    if (!type.IsInterface && !type.IsAbstract && isInterface)
                    {
                        Type? modlet = Array.Find(type.GetInterfaces(), x => x == typeof(T));
                        if (modlet != null)
                        {
                            T? instance = (T?)Activator.CreateInstance(type);
                            if (instance != null)
                            {
                                buffer.Add(instance);
                            }
                        }
                    }
                    else if(!type.IsInterface && !type.IsAbstract && type == typeof(T))
                    {
                        T? instance = (T?)Activator.CreateInstance(type);
                        if (instance != null)
                        {
                            buffer.Add(instance);
                        }
                    }
                    else if(!type.IsInterface && !type.IsAbstract && !isInterface && type.BaseType == typeof(T))
                    {
                        T? instance = (T?)Activator.CreateInstance(type, args);
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
                return [.. ordinal.Distinct()];
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

            Assembly? selfAssem = Assembly.GetAssembly(typeof(T)) ?? throw new NotImplementedException();
            FileInfo file = new(selfAssem.Location);
            DirectoryInfo? root = file.Directory ?? throw new NotImplementedException();
            foreach (string fullName in root.EnumerateFiles("*.dll").Select(x => x.FullName))
            {
                if (fullName == file.FullName)
                {
                    continue;
                }
                Assembly? match = assemblies.Find(x => x.Location == fullName);
                if (match != null)
                {
                    continue;
                }

                Assembly assem = Assembly.LoadFrom(fullName);
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
                        T? parent = Array.Find(list, x => x.GetType() == t);
                        if(parent != null)
                        {
                            int pIdx = Array.IndexOf(list, parent);
                            pBuffer.Add(pIdx);
                        }
                    }
                    parents.Add(Array.IndexOf(list, item), [.. pBuffer]);
                }
            }

            List<T> buffer = [];
            Tree<int>[] trees = Tree<int>.Create(parents);

            Loader.FillBuffer(buffer, trees, list);

            return [.. buffer.Distinct()];
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
                return [];
            }
            List<Type> buffer = [];
            foreach(Type t in list)
            {
                if(Array.Find(t.GetInterfaces(), x => x == typeof(T)) != null)
                {
                    buffer.Add(t);
                }
            }

            return [.. buffer];
        }
        #endregion //Private Methods
    }
}
