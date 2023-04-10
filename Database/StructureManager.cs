using System.Reflection;
using UT.Data.Extensions;

namespace UT.Data.Database
{
    public class StructureManager
    {
        #region Constructors
        public StructureManager(DbManager dbManager)
        {

        }
        #endregion //Constructors

        #region Public Methods
        public void Update()
        {
            Type[]? list =  StructureManager.LoadAssemblyTypes<ITable>();

        }
        #endregion //Public Methods

        #region Private Methods
        private static Type[]? LoadAssemblyTypes<T>()
            where T : ITable
        {
            Assembly? current = Assembly.GetAssembly(typeof(T));
            if (current == null)
            {
                return null;
            }

            List<Type> types = new();
            foreach (Assembly assembly in current.LoadAllReferenced())
            {
                types.AddRange(assembly.GetTypes<T>());
            }

            return types.ToArray();
        }
        #endregion //Private Methods
    }
}
