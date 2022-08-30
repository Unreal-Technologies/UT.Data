using System.Reflection;
using UT.Data.Attributes;

namespace UT.Data.Extensions
{
    public static class EnumExtension
    {
        #region Public Methods
        public static string Description(this Enum value)
        {
            string v = value.ToString();
            Type t = value.GetType();
            DescriptionAttribute? da = t.GetCustomAttribute<DescriptionAttribute>();
            if(da == null)
            {
                MemberInfo mi = t.GetMembers().First(x => x.Name == value.ToString());
                da = mi.GetCustomAttribute<DescriptionAttribute>();
            }

            if (da != null)
            {
                v = da.Text;
            }

            return v;
        }

        public static T? FromDescription<T>()
            where T : Enum
        {

            return default;
        }
        #endregion //Public Methods
    }
}
