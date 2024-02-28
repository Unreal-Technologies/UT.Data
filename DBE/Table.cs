using System.Reflection;
using UT.Data.DBE.Attributes;

namespace UT.Data.DBE
{
    public abstract class Table<Ttable, Tprimary> : ITable<Tprimary>, IDbSelect<Ttable>
        where Ttable : class
        where Tprimary : struct
    {
        #region Implementations
        public static Ttable Single(int? id)
        {
            throw new NotImplementedException();
        }

        public Tprimary? GetPrimary()
        {
            Type t = typeof(Ttable);
            foreach (MemberInfo mi in t.GetRuntimeFields())
            {
                if(mi.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                {
                    switch(mi.MemberType)
                    {
                        case MemberTypes.Field:
                            return ((FieldInfo)mi).GetValue(this) as Tprimary?;
                        case MemberTypes.Property:
                            return ((PropertyInfo)mi).GetValue(this) as Tprimary?;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            return default(Tprimary);
        }
        #endregion //Implementations
    }
}
