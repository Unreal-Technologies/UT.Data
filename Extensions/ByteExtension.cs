using System.Text;
using System;

namespace UT.Data.Extensions
{
    public static class ByteExtension
    {
        public static string AsString(this byte[] value)
        {
            return value.AsString(Encoding.UTF8);
        }

        public static string AsString(this byte[] value, Encoding encoding)
        {
            return encoding.GetString(value);
        }

        public static byte[] ToBase64(this byte[] value)
        {
            return Convert.ToBase64String(value).AsBytes();
        }

        public static byte[] FromBase64(this byte[] value)
        {
            return Convert.FromBase64String(value.AsString());
        }

        public static MemoryStream ToStream(this byte[] value)
        {
            MemoryStream ms = new();
            ms.Write(value, 0, value.Length);
            ms.Position = 0;
            return ms;
        }
    }
}
