using System.Reflection;
using UT.Data.DBE.Attributes;

namespace UT.Data.DBE
{
    public abstract class Table<Ttable, Tprimary> : ITable<Tprimary>, IDbSelect<Ttable>
        where Ttable : class
        where Tprimary : struct
    {
        public List<string> Changed { get; set; }
        #region Constructors
        public Table()
        {
            this.Changed = [];
        }
        #endregion //Constructors

        #region Implementations
        public static Ttable Single(int? id)
        {
            throw new NotImplementedException();
        }

        public static void CreateOrUpdate(IDatabaseConnection dbc)
        {
            if (Activator.CreateInstance(typeof(Ttable)) is not ITable table)
            {
                return;
            }
            dbc.CreateOrUpdateTable(table);
        }

        public Tprimary? GetPrimary()
        {
            Type t = typeof(Ttable);
            foreach (MemberInfo mi in t.GetRuntimeFields())
            {
                if(mi.GetCustomAttribute<PrimaryKeyAttribute>() != null)
                {
                    switch (mi.MemberType)
                    {
                        case MemberTypes.Field:
                            return ((FieldInfo)mi).GetValue(this) as Tprimary?;
                        case MemberTypes.Property:
                            return ((PropertyInfo)mi).GetValue(this) as Tprimary?;
                        case MemberTypes.Constructor:
                            break;
                        case MemberTypes.Event:
                            break;
                        case MemberTypes.Method:
                            break;
                        case MemberTypes.TypeInfo:
                            break;
                        case MemberTypes.Custom:
                            break;
                        case MemberTypes.NestedType:
                            break;
                        case MemberTypes.All:
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            return default(Tprimary);
        }

        Tprimary? ITable<Tprimary>.GetPrimary()
        {
            return this.GetPrimary();
        }

        static void ITable.CreateOrUpdate(IDatabaseConnection dbc)
        {
            throw new NotImplementedException();
        }
        #endregion //Implementations
    }
}
