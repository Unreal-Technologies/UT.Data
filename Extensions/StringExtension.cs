using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UT.Data.Attributes;

namespace UT.Data.Extensions
{
    public static class StringExtension
    {
        #region Public Methods
        public static string Repeat(this string value, int times)
        {
            string sOut = "";
            for(int i=0;i<times; i++)
            {
                sOut += value;
            }

            return sOut;
        }

        public static bool AsBoolean(this string value)
        {
            return value.ToLower() == "true";
        }

        public static byte[] AsBytes(this string value)
        {
            return (new ASCIIEncoding()).GetBytes(value);
        }

        public static string Md5(this string value, bool lowercase = true)
        {
            MD5 md5 = MD5.Create();
            byte[] input = value.AsBytes();
            byte[] hash = md5.ComputeHash(input);
            StringBuilder sb = new();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            if (lowercase)
            {
                return sb.ToString().ToLower();
            }
            return sb.ToString();
        }

        public static T? AsEnum<T>(this string value)
            where T : struct, Enum
        {
            foreach(MemberInfo mi in typeof(T).GetMembers())
            {
                bool found = false;
                if(mi.Name.ToLower() == value.ToLower())
                {
                    found = true;
                }

                if (!found)
                {
                    DescriptionAttribute? da = mi.GetCustomAttribute<DescriptionAttribute>();
                    if (da != null && da.Text.ToLower() == value.ToLower())
                    {
                        found = true;
                    }
                }

                if(found)
                {
                    if(Enum.GetValues(typeof(T)) is not T[] values)
                    {
                        continue;
                    }
                    return (T)values.First(x => x.ToString().ToLower() == mi.Name.ToLower());
                }
            }

            return default;
        }
        #endregion //Public Methods
    }
}
